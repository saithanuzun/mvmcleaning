

using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Invoice;
using mvmclean.backend.Domain.Aggregates.Invoice.ValueObjects;

namespace mvmclean.backend.Application.Features.Invoice.Commands;

public class CreateInvoiceRequest : IRequest<CreateInvoiceResponse>
{
    public Guid BookingId { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
}

public class CreateInvoiceResponse
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceRequest, CreateInvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBookingRepository _bookingRepository;

    public CreateInvoiceHandler(
        IInvoiceRepository invoiceRepository,
        IBookingRepository bookingRepository)
    {
        _invoiceRepository = invoiceRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<CreateInvoiceResponse> Handle(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        // Get booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            return new CreateInvoiceResponse
            {
                Success = false,
                Message = "Booking not found"
            };
        }

        // Check if booking has customer and total price
        if (booking.CustomerId == Guid.Empty)
        {
            return new CreateInvoiceResponse
            {
                Success = false,
                Message = "Booking must have a customer assigned"
            };
        }

        // Create payment terms
        var paymentTerms = new PaymentTerms(request.PaymentTermsDays);

        // Create invoice from booking
        var invoice = Domain.Aggregates.Invoice.Invoice.CreateForBooking(booking, paymentTerms);

        // Save invoice
        await _invoiceRepository.AddAsync(invoice);

        return new CreateInvoiceResponse
        {
            InvoiceId = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            Message = "Invoice created successfully",
            Success = true
        };
    }
}

