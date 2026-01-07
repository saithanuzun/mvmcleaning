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
            // Step 1: Validate postcode with external API
            var client = _httpClientFactory.CreateClient();
            var validateResponse = await client.GetAsync($"https://api.postcodes.io/postcodes/{request.Postcode}/validate");

            if (!validateResponse.IsSuccessStatusCode)
                return Error("Unable to validate postcode");

            var content = await validateResponse.Content.ReadAsStringAsync();
            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                var root = doc.RootElement;
                var isValid = root.GetProperty("result").GetBoolean();

                if (!isValid)
                    return Error("Invalid UK postcode");
            }

            // Step 2: Create booking with postcode and phone
            var createBookingRequest = new CreateBookingRequest
            {
                Postcode = request.Postcode.ToUpper(),
                PhoneNumber = request.Phone
            };

            var bookingResponse = await _mediator.Send(createBookingRequest);

            if (bookingResponse == null)
                return Error("Failed to create booking", 500);

            // Step 3: Get contractors that cover this postcode
            var getContractorsRequest = new GetContractorsByPostcodeRequest
            {
                Postcode = request.Postcode.ToUpper(),
                BookingId = bookingResponse.BookingId.ToString()
            };

            var contractorsResponse = await _mediator.Send(getContractorsRequest);

            if (contractorsResponse.ContractorIds == null || contractorsResponse.ContractorIds.Count == 0)
                return Success(new
                {
                    bookingId = bookingResponse.BookingId,
                    postcode = request.Postcode.ToUpper(),
                    phone = request.Phone,
                    contractors = new List<string>(),
                    isCovered = false,
                    message = "No contractors available for this area"
                }, "Postcode validated but no contractors available");

            return Success(new
            {
                bookingId = bookingResponse.BookingId,
                postcode = request.Postcode.ToUpper(),
                phone = request.Phone,
                contractors = contractorsResponse.ContractorIds,
                isCovered = true
            }, $"Booking created. Found {contractorsResponse.ContractorIds.Count} contractor(s)");
        }
        catch (Exception ex)
        {
            return Error($"Error: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Validate a UK postcode using postcodes.io API (validation only)
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidatePostcode([FromBody] ValidatePostcodeRequest request)
    {
        if (string.IsNullOrEmpty(request.Postcode))
            return Error("Postcode is required");

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://api.postcodes.io/postcodes/{request.Postcode}/validate");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    var root = doc.RootElement;
                    var isValid = root.GetProperty("result").GetBoolean();

                    return Success(new
                    {
                        isValid = isValid,
                        isCovered = true,
                        postcode = request.Postcode.ToUpper()
                    }, isValid ? "Valid UK postcode" : "Invalid UK postcode");
                }
            }

            return Error("Unable to validate postcode");
        }
        catch (Exception ex)
        {
            return Error($"Error validating postcode: {ex.Message}", 500);
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
