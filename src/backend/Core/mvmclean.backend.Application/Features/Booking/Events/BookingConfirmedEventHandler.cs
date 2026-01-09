using MediatR;
using mvmclean.backend.Application.Services;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Events;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Invoice;
using mvmclean.backend.Domain.Aggregates.Invoice.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Events;

public class BookingConfirmedEventHandler : INotificationHandler<BookingConfirmedEvent>
{
    private readonly IMailingService _mailingService;
    private readonly IInvoiceService _invoiceService;
    private readonly IContractorRepository  _contractorRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBookingRepository  _bookingRepository;
    
    

    public BookingConfirmedEventHandler(
        IMailingService mailingService, 
        IInvoiceService invoiceService,
        IContractorRepository contractorRepository, 
        IInvoiceRepository invoiceRepository, 
        IBookingRepository bookingRepository)
    {
        _mailingService = mailingService ?? throw new ArgumentNullException(nameof(mailingService));
        _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        _contractorRepository = contractorRepository;
        _invoiceRepository = invoiceRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Create and save invoice
            var booking = await _bookingRepository.GetByIdAsync(notification.BookingId);
            var invoice = Domain.Aggregates.Invoice.Invoice.CreateForBooking(booking, new PaymentTerms(0));
            await _invoiceRepository.AddAsync(invoice);

            // Prepare service list from booking items
            var services = notification.Items?.Select(item => item.ServiceName).ToList() ?? new List<string>();

            // Generate HTML invoice for email with all details
            var invoiceAmounts = new InvoiceAmounts
            {
                Subtotal = notification.Total.Amount,
                Discount = 0, // You can update this if discount logic exists
                Total = notification.Total.Amount
            };

            var invoiceHtml = await _invoiceService.GenerateHtmlInvoiceAsync(
                invoiceId: invoice.Id,
                invoiceNumber: invoice.Id.ToString().Substring(0, 8).ToUpper(),
                customerName: notification.CustomerName,
                customerAddress: string.Concat(notification.Address.City, " ", notification.Address.Street, " ", notification.Address.Postcode) ?? "N/A",
                services: services,
                amounts: invoiceAmounts,
                issueDate: DateTime.Now,
                dueDate: DateTime.Now.AddDays(14) // 14 days payment term
            );

            // Send booking confirmation email to admin with invoice
            await _mailingService.SendBookingConfirmationAsync(
                recipientEmail: "saithan.uzun@gmail.com", // admin email address //todo get admin email from config or database
                bookingId: notification.BookingId,
                customerName: notification.CustomerName,
                address: string.Concat(notification.Address.City," ", notification.Address.Street," ",notification.Address.Postcode) ?? "N/A",
                services: services,
                totalAmount: notification.Total.Amount,
                bookingDate: notification.TimeSlot.StartTime,
                invoiceHtml: invoiceHtml
            );

            // Send booking confirmation email to contractor with invoice
            /*var contractor = await _contractorRepository.GetByIdAsync(notification.ContractorId ?? Guid.Empty);
            await _mailingService.SendBookingConfirmationAsync(
                recipientEmail: contractor.Email ?? "saithan.uzun@gmail.com", 
                bookingId: notification.BookingId,
                customerName: notification.CustomerName,
                address: string.Concat(notification.Address.City," ", notification.Address.Street," ",notification.Address.Postcode) ?? "N/A",
                services: services,
                totalAmount: notification.Total.Amount,
                bookingDate: notification.TimeSlot.StartTime,
                invoiceHtml: invoiceHtml
            );*/
            
            // mailing for customer
            await _mailingService.SendBookingConfirmationAsync(
                recipientEmail: booking.Customer.Email, 
                bookingId: notification.BookingId,
                customerName: notification.CustomerName,
                address: string.Concat(notification.Address.City," ", notification.Address.Street," ",notification.Address.Postcode) ?? "N/A",
                services: services,
                totalAmount: notification.Total.Amount,
                bookingDate: notification.TimeSlot.StartTime,
                invoiceHtml: invoiceHtml
            );

            Console.WriteLine(
                $"Booking confirmed event handled: BookingId: {notification.BookingId}, " +
                $"ContractorId: {notification.ContractorId}, " +
                $"TimeSlot: {notification.TimeSlot.StartTime}, " +
                $"Total: {notification.Total.Amount}, " +
                $"Invoice: {invoice.Id}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling BookingConfirmedEvent: {ex.Message}");
            throw;
        }
    }
}