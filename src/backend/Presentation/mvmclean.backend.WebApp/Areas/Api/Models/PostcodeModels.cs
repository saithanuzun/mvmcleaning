namespace mvmclean.backend.WebApp.Areas.Api.Models;

public class ValidatePostcodeRequest
{
    public string Postcode { get; set; }
}

public class ValidatePostcodeResponse
{
    public bool IsValid { get; set; }
    public bool IsCovered { get; set; }
    public string Postcode { get; set; }
}
