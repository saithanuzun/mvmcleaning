using Microsoft.Extensions.Logging;
using mvmclean.backend.Application.Services;
using mvmclean.backend.Domain.Aggregates.Invoice;

namespace mvmclean.backend.Infrastructure.Services;

/// <summary>
/// Service for managing invoice operations
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly ILogger<InvoiceService> _logger;
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(ILogger<InvoiceService> logger, IInvoiceRepository invoiceRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
    }

    /// <summary>
    /// Generates an HTML invoice for display in emails and web
    /// </summary>
    public async Task<string> GenerateHtmlInvoiceAsync(
        Guid invoiceId,
        string invoiceNumber,
        string customerName,
        string customerAddress,
        List<string> services,
        InvoiceAmounts amounts,
        DateTime issueDate,
        DateTime dueDate)
    {
        try
        {
            var servicesList = string.Join("", services.Select(s => 
                $"<tr><td style='padding: 10px; border-bottom: 1px solid #eee;'>{s}</td><td style='text-align: right;'></td></tr>"));

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 800px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #194376 0%, #46C6CE 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .section {{ margin-bottom: 25px; }}
        .section h2 {{ color: #194376; font-size: 16px; border-bottom: 2px solid #46C6CE; padding-bottom: 10px; margin-top: 0; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin-bottom: 25px; }}
        .info-box {{ background: white; padding: 15px; border-left: 4px solid #46C6CE; border-radius: 4px; }}
        .info-box strong {{ color: #194376; display: block; margin-bottom: 5px; }}
        .info-box p {{ margin: 0; font-size: 14px; }}
        table {{ width: 100%; border-collapse: collapse; background: white; border-radius: 4px; margin: 15px 0; }}
        th {{ background: #194376; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; }}
        .totals {{ background: white; padding: 15px; border-radius: 4px; text-align: right; }}
        .total-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .total-row.final {{ border: none; border-top: 2px solid #194376; padding-top: 15px; font-size: 18px; font-weight: bold; color: #194376; }}
        .footer {{ background: #194376; color: white; padding: 20px; text-align: center; border-radius: 0 0 8px 8px; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>INVOICE</h1>
            <p>Reference: <strong>{invoiceNumber}</strong></p>
        </div>

        <div class='content'>
            <div class='info-grid'>
                <div class='info-box'>
                    <strong>Bill To:</strong>
                    <p>{customerName}</p>
                    <p>{customerAddress}</p>
                </div>
                <div class='info-box'>
                    <strong>Invoice Details:</strong>
                    <p>Issue Date: {issueDate:dd/MM/yyyy}</p>
                    <p>Due Date: {dueDate:dd/MM/yyyy}</p>
                </div>
            </div>

            <div class='section'>
                <h2>Services</h2>
                <table>
                    <thead>
                        <tr>
                            <th>Description</th>
                            <th style='text-align: right;'>Amount</th>
                        </tr>
                    </thead>
                    <tbody>
                        {servicesList}
                    </tbody>
                </table>
            </div>

            <div class='section'>
                <div class='totals'>
                    <div class='total-row'>
                        <span>Subtotal:</span>
                        <span>£{amounts.Subtotal:F2}</span>
                    </div>
                    {(amounts.Discount > 0 ? $@"
                    <div class='total-row'>
                        <span>Discount:</span>
                        <span>-£{amounts.Discount:F2}</span>
                    </div>" : "")}
                    <div class='total-row final'>
                        <span>Total Due:</span>
                        <span>£{amounts.Total:F2}</span>
                    </div>
                </div>
            </div>

            <div class='section' style='text-align: center;'>
                <p style='color: #666; font-size: 12px;'>Thank you for your business!</p>
            </div>
        </div>

        <div class='footer'>
            <p>&copy; 2026 MVM Clean. All rights reserved.</p>
            <p>Contact: support@mvmclean.com | Phone: 020 XXXX XXXX</p>
        </div>
    </div>
</body>
</html>";

            _logger.LogInformation("Generated HTML invoice for {InvoiceNumber}", invoiceNumber);
            return await Task.FromResult(html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate HTML invoice for {InvoiceNumber}", invoiceNumber);
            throw;
        }
    }

    /// <summary>
    /// Gets invoice summary for email content
    /// </summary>
    public async Task<InvoiceSummary> GetInvoiceSummaryAsync(Guid invoiceId)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, true);
            
            if (invoice == null)
                throw new InvalidOperationException($"Invoice {invoiceId} not found");

            return new InvoiceSummary
            {
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.CustomerName,
                Total = invoice.TotalAmount.Amount,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get invoice summary for {InvoiceId}", invoiceId);
            throw;
        }
    }
}
