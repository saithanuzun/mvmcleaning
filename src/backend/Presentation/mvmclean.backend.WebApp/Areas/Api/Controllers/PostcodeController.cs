using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using mvmclean.backend.Application.Features.Booking.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class PostcodeController : BaseApiController
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PostcodeController(IMediator mediator, IHttpClientFactory httpClientFactory) : base(mediator)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Validate a UK postcode using postcodes.io API, create booking, and get available contractors
    /// </summary>
    [HttpPost("validate-and-book")]
    public async Task<IActionResult> ValidateAndBook([FromBody] ValidateAndBookRequest request)
    {
        if (string.IsNullOrEmpty(request.Postcode) || string.IsNullOrEmpty(request.Phone))
            return Error("Postcode and phone number are required");

        try
        {
            // Clean up postcode - keep it simple
            var cleanPostcode = request.Postcode.ToUpper().Replace(" ", "").Trim();
            
            // Basic validation: just check it's not empty
            if (string.IsNullOrEmpty(cleanPostcode))
                return Error("Postcode is required");

            // Step 1: Create booking with postcode and phone
            var createBookingRequest = new CreateBookingRequest
            {
                Postcode = cleanPostcode,
                PhoneNumber = request.Phone
            };

            var bookingResponse = await _mediator.Send(createBookingRequest);

            if (bookingResponse == null)
                return Error("Failed to create booking", 500);

            // Step 2: Get contractors that cover this postcode
            var getContractorsRequest = new GetContractorsByPostcodeRequest
            {
                Postcode = cleanPostcode,
                BookingId = bookingResponse.BookingId.ToString()
            };

            var contractorsResponse = await _mediator.Send(getContractorsRequest);

            if (contractorsResponse == null || contractorsResponse.ContractorIds == null || contractorsResponse.ContractorIds.Count == 0)
                return Success(new
                {
                    bookingId = bookingResponse.BookingId,
                    postcode = cleanPostcode,
                    phone = request.Phone,
                    contractors = new List<string>(),
                    isCovered = false,
                    message = "No contractors available for this area"
                }, "Postcode validated but no contractors available");

            return Success(new
            {
                bookingId = bookingResponse.BookingId,
                postcode = cleanPostcode,
                phone = request.Phone,
                contractors = contractorsResponse.ContractorIds,
                isCovered = true
            }, $"Booking created. Found {contractorsResponse.ContractorIds.Count} contractor(s)");
        }
        catch (HttpRequestException ex)
        {
            return Error($"Network error: Unable to validate postcode. Please try again.", 500);
        }
        catch (TaskCanceledException)
        {
            return Error("Request timed out. Please try again.", 500);
        }
        catch (JsonException)
        {
            return Error("Invalid response from validation service. Please try again.", 500);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PostcodeController error: {ex.Message}\n{ex.StackTrace}");
            return Error($"An error occurred while validating postcode. Please try again.", 500);
        }
    }
    
}

public class ValidatePostcodeRequest
{
    public string Postcode { get; set; }
}

public class ValidateAndBookRequest
{
    public string Postcode { get; set; }
    public string Phone { get; set; }
}
