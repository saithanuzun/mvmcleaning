using MediatR;
using mvmclean.backend.Application.Services;
using mvmclean.backend.Domain.Aggregates.Booking.Events;
using mvmclean.backend.Domain.Aggregates.Invoice.Events;

namespace mvmclean.backend.Application.Features.Invoice.Events;

public class InvoiceCreatedEventHandler: INotificationHandler<InvoiceCreatedEvent>
{
    private readonly IMailingService _mailingService;

    public InvoiceCreatedEventHandler(IMailingService mailingService)
    {
        _mailingService = mailingService ?? throw new ArgumentNullException(nameof(mailingService));
    }

    public async Task Handle(InvoiceCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling BookingCreatedEvent: {ex.Message}");
            throw;
        }
    }
}