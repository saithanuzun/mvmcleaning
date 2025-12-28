using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public Postcode Postcode { get; private set; }
    public string? AdditionalInfo { get; private set; }
    
    // Geographic coordinates
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    private Address() { }

    public static Address Create(string street, string city, Postcode postcode, string? additionalInfo = null, double? latitude = null, double? longitude = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty");
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty");

        return new Address
        {
            Street = street,
            City = city,
            Postcode = postcode,
            AdditionalInfo = additionalInfo,
            Latitude = latitude,
            Longitude = longitude
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Postcode;
    }
}