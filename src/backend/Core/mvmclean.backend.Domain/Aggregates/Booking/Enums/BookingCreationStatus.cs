namespace mvmclean.backend.Domain.Aggregates.Booking.Enums;

public enum BookingCreationStatus
{
    BasicInfo = 1,
    ContractorSelected = 2,
    ServicesAdded = 3,
    TimeSlotSelected = 4,
    CustomerAdded = 5,
    PaymentInitiated = 6,
    Completed = 7
}