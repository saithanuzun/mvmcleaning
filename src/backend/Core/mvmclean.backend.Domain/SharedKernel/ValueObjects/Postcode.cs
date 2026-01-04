using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.SharedKernel.ValueObjects;

public class Postcode : ValueObject
{
    public string Value { get; private set; }
    public string Area { get; private set; } // e.g., "SW1"
    public string District { get; private set; } // e.g., "SW1A"
    public string Sector { get; private set; } // e.g., "SW1A 1"

    private Postcode()
    {
    }

    public static Postcode Create(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            throw new ArgumentException("Postcode cannot be empty");

        var normalized = postcode.ToUpper().Replace(" ", "");

        if (!IsValidFormat(normalized))
            throw new ArgumentException("Invalid postcode format");

        var area = ExtractArea(normalized);
        var district = ExtractDistrict(normalized);
        var sector = ExtractSector(normalized);

        return new Postcode
        {
            Value = FormatPostcode(normalized),
            Area = area,
            District = district,
            Sector = sector
        };
    }

    private static bool IsValidFormat(string postcode)
    {
        return postcode.Length >= 5 && postcode.Length <= 8;
    }

    private static string ExtractArea(string postcode)
    {
        return postcode.Substring(0, Math.Min(2, postcode.Length));
    }

    private static string ExtractDistrict(string postcode)
    {
        return postcode.Substring(0, Math.Min(4, postcode.Length));
    }

    private static string ExtractSector(string postcode)
    {
        if (postcode.Length < 5) return postcode;
        return postcode.Substring(0, postcode.Length - 2) + " " + postcode.Substring(postcode.Length - 2, 1);
    }

    private static string FormatPostcode(string postcode)
    {
        if (postcode.Length <= 5) return postcode;
        var outward = postcode.Substring(0, postcode.Length - 3);
        var inward = postcode.Substring(postcode.Length - 3);
        return $"{outward} {inward}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

}