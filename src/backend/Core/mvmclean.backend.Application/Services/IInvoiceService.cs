namespace mvmclean.backend.Application.Services;

/// <summary>
/// Service for managing invoice operations
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// Creates an HTML invoice for display in emails and web
    /// </summary>
    /// <param name="invoiceId">Invoice ID</param>
    /// <param name="invoiceNumber">Invoice number</param>
    /// <param name="customerName">Customer name</param>
    /// <param name="customerAddress">Customer address</param>
    /// <param name="services">List of services</param>
    /// <param name="amounts">Amount details (subtotal, discount, total)</param>
    /// <param name="issueDate">Invoice issue date</param>
    /// <param name="dueDate">Payment due date</param>
    /// <returns>HTML formatted invoice</returns>
    Task<string> GenerateHtmlInvoiceAsync(
        Guid invoiceId,
        string invoiceNumber,
        string customerName,
        string customerAddress,
        List<string> services,
        InvoiceAmounts amounts,
        DateTime issueDate,
        DateTime dueDate);

    /// <summary>
    /// Gets invoice summary for email content
    /// </summary>
    Task<InvoiceSummary> GetInvoiceSummaryAsync(Guid invoiceId);
}

/// <summary>
/// Invoice amount details
/// </summary>
public class InvoiceAmounts
{
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}

/// <summary>
/// Invoice summary for emails
/// </summary>
public class InvoiceSummary
{
    public string InvoiceNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal Total { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
}
