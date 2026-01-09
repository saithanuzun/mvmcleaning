namespace mvmclean.backend.Infrastructure.MailingService;

/// <summary>
/// Base class for email templates
/// </summary>
public abstract class EmailTemplate
{
    public string Subject { get; protected set; }
    public string HtmlBody { get; protected set; }
    public string PlainTextBody { get; protected set; }

    public abstract void GenerateTemplate();
}

/// <summary>
/// Booking Created email template - sent when booking is first created (before payment)
/// Shows: Postcode and Telephone Number
/// </summary>
public class BookingCreatedEmailTemplate : EmailTemplate
{
    private string _bookingReference;
    private string _postcode;
    private string _telephoneNumber;

    public BookingCreatedEmailTemplate(
        string bookingId,
        string postcode,
        string telephoneNumber)
    {
        _bookingReference = bookingId.Substring(0, 8).ToUpper();
        _postcode = postcode;
        _telephoneNumber = telephoneNumber;
        
        GenerateTemplate();
    }

    public override void GenerateTemplate()
    {
        Subject = $"Booking Created - Reference #{_bookingReference}";

        // HTML Template
        HtmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            line-height: 1.6; 
            color: #333333; 
            margin: 0;
            padding: 0;
            background-color: #f5f7fa;
        }}
        .container {{ 
            max-width: 620px; 
            margin: 20px auto; 
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
        }}
        .header {{ 
            background: #194376; 
            color: white; 
            padding: 35px 30px; 
            text-align: center; 
        }}
        .header h1 {{ 
            margin: 0 0 10px 0; 
            font-size: 28px; 
            font-weight: 600;
        }}
        .header-subtitle {{
            font-size: 16px;
            opacity: 0.9;
            margin-bottom: 15px;
        }}
        .reference-badge {{
            display: inline-block;
            background: #46C6CE;
            color: white;
            padding: 8px 20px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 18px;
            letter-spacing: 1px;
        }}
        .content {{ 
            padding: 40px 30px; 
        }}
        .section {{ 
            margin-bottom: 32px; 
        }}
        .section-title {{ 
            color: #194376; 
            font-size: 18px; 
            font-weight: 600;
            padding-bottom: 12px;
            margin-bottom: 20px;
            border-bottom: 2px solid #46C6CE;
            display: flex;
            align-items: center;
            gap: 8px;
        }}
        .section-title svg {{
            fill: #194376;
        }}
        .info-card {{
            background: #ffffff;
            padding: 20px;
            border-left: 4px solid #46C6CE;
            margin: 12px 0;
            border-radius: 6px;
            border: 1px solid #eaeaea;
            display: flex;
            align-items: flex-start;
            gap: 15px;
        }}
        .info-icon {{
            background: #f0f9ff;
            width: 40px;
            height: 40px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
        }}
        .info-content {{
            flex: 1;
        }}
        .info-label {{
            color: #194376;
            font-weight: 600;
            font-size: 15px;
            margin-bottom: 4px;
        }}
        .info-value {{
            color: #333333;
            font-size: 16px;
            font-weight: 500;
        }}
        .step-card {{
            background: #f8fafc;
            border-radius: 8px;
            padding: 20px;
            margin: 15px 0;
            border-left: 3px solid #194376;
        }}
        .step-number {{
            display: inline-block;
            background: #194376;
            color: white;
            width: 28px;
            height: 28px;
            border-radius: 50%;
            text-align: center;
            line-height: 28px;
            font-weight: 600;
            margin-right: 12px;
        }}
        .step-title {{
            color: #194376;
            font-weight: 600;
            font-size: 15px;
            margin-bottom: 5px;
        }}
        .step-description {{
            color: #555555;
            font-size: 14px;
            line-height: 1.5;
            padding-left: 40px;
        }}
        .contact-box {{
            background: #f0f9ff;
            border: 1px solid #c6e6ff;
            border-radius: 8px;
            padding: 20px;
            text-align: center;
            margin-top: 30px;
        }}
        .footer {{ 
            background: #194376; 
            color: white; 
            padding: 25px 30px; 
            text-align: center; 
            font-size: 13px;
            line-height: 1.5;
        }}
        .footer p {{
            margin: 5px 0;
            opacity: 0.85;
        }}
        .divider {{
            height: 1px;
            background: #eaeaea;
            margin: 25px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✓ Booking Created Successfully!</h1>
            <div class='header-subtitle'>Your booking reference:</div>
            <div class='reference-badge'>#{_bookingReference}</div>
        </div>

        <div class='content'>
            <div class='section'>
                <div class='section-title'>
                    📋 Your Booking Details
                </div>
                
                <div class='info-card'>
                    <div class='info-icon'>📍</div>
                    <div class='info-content'>
                        <div class='info-label'>Postcode</div>
                        <div class='info-value'>{_postcode}</div>
                    </div>
                </div>
                
                <div class='info-card'>
                    <div class='info-icon'>📞</div>
                    <div class='info-content'>
                        <div class='info-label'>Telephone Number</div>
                        <div class='info-value'>{_telephoneNumber}</div>
                    </div>
                </div>
            </div>

            <div class='divider'></div>

            <div class='section'>
                <div class='section-title'>
                    📝 What Happens Next?
                </div>
                
                <div class='step-card'>
                    <div>
                        <span class='step-number'>1</span>
                        <span class='step-title'>Add Services</span>
                    </div>
                    <div class='step-description'>
                        Continue to add services you need cleaned. You can modify your booking until you complete payment.
                    </div>
                </div>
                
                <div class='step-card'>
                    <div>
                        <span class='step-number'>2</span>
                        <span class='step-title'>Review & Payment</span>
                    </div>
                    <div class='step-description'>
                        Review your booking details and complete payment to confirm your appointment.
                    </div>
                </div>
                
                <div class='step-card'>
                    <div>
                        <span class='step-number'>3</span>
                        <span class='step-title'>Confirmation</span>
                    </div>
                    <div class='step-description'>
                        Once payment is processed, you'll receive a booking confirmation email with all the details.
                    </div>
                </div>
            </div>

            <div class='contact-box'>
                <p style='margin: 0 0 10px 0; color: #194376; font-weight: 600;'>Need Assistance?</p>
                <p style='margin: 0; color: #555555; font-size: 14px;'>
                    Contact our support team at <strong>support@mvmclean.com</strong><br>
                    We're here to help with any questions.
                </p>
            </div>
        </div>

        <div class='footer'>
            <p>&copy; 2026 MVM Clean. All rights reserved.</p>
            <p>support@mvmclean.com | 020 XXXX XXXX</p>
            <p>This is an automated message, please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";

        // Plain text template
        PlainTextBody = $@"BOOKING CREATED SUCCESSFULLY!

Your booking reference: #{_bookingReference}

YOUR BOOKING DETAILS:
────────────────────
Postcode: {_postcode}
Telephone: {_telephoneNumber}

WHAT HAPPENS NEXT?
──────────────────
1. Add Services
   Continue to add services you need cleaned. You can modify your booking until you complete payment.

2. Review & Payment
   Review your booking details and complete payment to confirm your appointment.

3. Confirmation
   Once payment is processed, you'll receive a booking confirmation email with all the details.

Need Assistance?
Contact our support team at support@mvmclean.com
We're here to help with any questions.

────────────────────
© 2026 MVM Clean. All rights reserved.
support@mvmclean.com | 020 XXXX XXXX
This is an automated message, please do not reply to this email.";
    }
}

/// <summary>
/// Booking Confirmed email template - sent after successful payment
/// Shows booking completion with all details
/// </summary>
public class BookingConfirmedEmailTemplate : EmailTemplate
{
    private string _bookingReference;
    private string _customerName;
    private string _address;
    private List<string> _services;
    private decimal _totalAmount;
    private DateTime _bookingDate;
    private string? _invoiceHtml;

    public BookingConfirmedEmailTemplate(
        string bookingId,
        string customerName,
        string address,
        List<string> services,
        decimal totalAmount,
        DateTime bookingDate,
        string? invoiceHtml = null)
    {
        _bookingReference = bookingId.Substring(0, 8).ToUpper();
        _customerName = customerName;
        _address = address;
        _services = services ?? new List<string>();
        _totalAmount = totalAmount;
        _bookingDate = bookingDate;
        _invoiceHtml = invoiceHtml;
        
        GenerateTemplate();
    }

    public override void GenerateTemplate()
    {
        Subject = $"Booking Confirmed! Reference #{_bookingReference}";

        // Build invoice section if invoiceHtml is provided
        var invoiceSection = string.IsNullOrEmpty(_invoiceHtml) ? "" : $@"
            <div class='section'>
                <div class='section-title'>
                    📋 Invoice Details
                </div>
                <div class='invoice-container'>
                    {_invoiceHtml}
                </div>
            </div>";

        // HTML Template
        HtmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            line-height: 1.6; 
            color: #333333; 
            margin: 0;
            padding: 0;
            background-color: #f5f7fa;
        }}
        .container {{ 
            max-width: 620px; 
            margin: 20px auto; 
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
        }}
        .header {{ 
            background: #194376; 
            color: white; 
            padding: 35px 30px; 
            text-align: center; 
        }}
        .header h1 {{ 
            margin: 0 0 10px 0; 
            font-size: 28px; 
            font-weight: 600;
        }}
        .header-subtitle {{
            font-size: 16px;
            opacity: 0.9;
            margin-bottom: 15px;
        }}
        .reference-badge {{
            display: inline-block;
            background: #46C6CE;
            color: white;
            padding: 8px 20px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 18px;
            letter-spacing: 1px;
        }}
        .status-badge {{
            display: inline-block;
            background: #2ecc71;
            color: white;
            padding: 6px 16px;
            border-radius: 20px;
            font-weight: 600;
            font-size: 14px;
            margin-top: 10px;
        }}
        .content {{ 
            padding: 40px 30px; 
        }}
        .greeting {{
            font-size: 18px;
            color: #194376;
            font-weight: 500;
            margin-bottom: 25px;
            padding-bottom: 15px;
            border-bottom: 1px solid #eaeaea;
        }}
        .section {{ 
            margin-bottom: 32px; 
        }}
        .section-title {{ 
            color: #194376; 
            font-size: 18px; 
            font-weight: 600;
            padding-bottom: 12px;
            margin-bottom: 20px;
            border-bottom: 2px solid #46C6CE;
            display: flex;
            align-items: center;
            gap: 8px;
        }}
        .info-card {{
            background: #ffffff;
            padding: 20px;
            border-left: 4px solid #46C6CE;
            margin: 12px 0;
            border-radius: 6px;
            border: 1px solid #eaeaea;
            display: flex;
            align-items: flex-start;
            gap: 15px;
        }}
        .info-icon {{
            background: #f0f9ff;
            width: 40px;
            height: 40px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
        }}
        .info-content {{
            flex: 1;
        }}
        .info-label {{
            color: #194376;
            font-weight: 600;
            font-size: 15px;
            margin-bottom: 4px;
        }}
        .info-value {{
            color: #333333;
            font-size: 16px;
            font-weight: 500;
        }}
        .services-container {{
            background: #f8fafc;
            border-radius: 8px;
            padding: 25px;
            margin: 15px 0;
        }}
        .service-item {{
            display: flex;
            align-items: center;
            padding: 12px 0;
            border-bottom: 1px solid #eaeaea;
        }}
        .service-item:last-child {{
            border-bottom: none;
        }}
        .service-check {{
            color: #2ecc71;
            font-weight: bold;
            margin-right: 12px;
            font-size: 18px;
        }}
        .service-name {{
            color: #333333;
            font-size: 15px;
            font-weight: 500;
        }}
        .total-box {{
            background: #194376;
            color: white;
            padding: 25px;
            border-radius: 8px;
            margin-top: 20px;
            text-align: center;
        }}
        .total-label {{
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 8px;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}
        .total-amount {{
            font-size: 32px;
            font-weight: 700;
            color: #46C6CE;
        }}
        .next-steps {{
            background: #f0f9ff;
            border-radius: 8px;
            padding: 25px;
            margin: 25px 0;
        }}
        .step {{
            display: flex;
            align-items: flex-start;
            margin-bottom: 20px;
        }}
        .step:last-child {{
            margin-bottom: 0;
        }}
        .step-number {{
            background: #194376;
            color: white;
            width: 30px;
            height: 30px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 600;
            flex-shrink: 0;
            margin-right: 15px;
        }}
        .step-content {{
            flex: 1;
        }}
        .step-title {{
            color: #194376;
            font-weight: 600;
            font-size: 15px;
            margin-bottom: 5px;
        }}
        .step-description {{
            color: #555555;
            font-size: 14px;
            line-height: 1.5;
        }}
        .invoice-container {{
            background: white;
            padding: 20px;
            border-radius: 6px;
            border: 1px solid #eaeaea;
        }}
        .contact-box {{
            background: #f0f9ff;
            border: 1px solid #c6e6ff;
            border-radius: 8px;
            padding: 20px;
            text-align: center;
            margin-top: 30px;
        }}
        .footer {{ 
            background: #194376; 
            color: white; 
            padding: 25px 30px; 
            text-align: center; 
            font-size: 13px;
            line-height: 1.5;
        }}
        .footer p {{
            margin: 5px 0;
            opacity: 0.85;
        }}
        .divider {{
            height: 1px;
            background: #eaeaea;
            margin: 25px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Booking Confirmed!</h1>
            <div class='header-subtitle'>Your booking reference:</div>
            <div class='reference-badge'>#{_bookingReference}</div>
            <div class='status-badge'>✓ Confirmed</div>
        </div>

        <div class='content'>
            <div class='greeting'>
                Hello {_customerName},<br>
                Your booking has been confirmed! We're excited to serve you.
            </div>

            <div class='section'>
                <div class='section-title'>
                    📅 Booking Details
                </div>
                
                <div class='info-card'>
                    <div class='info-icon'>📅</div>
                    <div class='info-content'>
                        <div class='info-label'>Scheduled Date</div>
                        <div class='info-value'>{_bookingDate:dddd, MMMM d, yyyy}</div>
                    </div>
                </div>
                
                <div class='info-card'>
                    <div class='info-icon'>📍</div>
                    <div class='info-content'>
                        <div class='info-label'>Service Address</div>
                        <div class='info-value'>{_address}</div>
                    </div>
                </div>
            </div>

            <div class='divider'></div>

            <div class='section'>
                <div class='section-title'>
                    🧹 Services Confirmed
                </div>
                
                <div class='services-container'>
                    {string.Join("", _services.Select(s => $@"
                    <div class='service-item'>
                        <div class='service-check'>✓</div>
                        <div class='service-name'>{s}</div>
                    </div>"))}
                    
                    <div class='total-box'>
                        <div class='total-label'>Total Amount</div>
                        <div class='total-amount'>£{_totalAmount:F2}</div>
                    </div>
                </div>
            </div>

            {invoiceSection}

            <div class='section'>
                <div class='section-title'>
                    📋 What Happens Next?
                </div>
                
                <div class='next-steps'>
                    <div class='step'>
                        <div class='step-number'>1</div>
                        <div class='step-content'>
                            <div class='step-title'>Contact from Contractor</div>
                            <div class='step-description'>
                                Our contractor will contact you within 24 hours to confirm the final details and answer any questions.
                            </div>
                        </div>
                    </div>
                    
                    <div class='step'>
                        <div class='step-number'>2</div>
                        <div class='step-content'>
                            <div class='step-title'>Preparation</div>
                            <div class='step-description'>
                                We'll prepare and ensure everything is ready for your scheduled appointment.
                            </div>
                        </div>
                    </div>
                    
                    <div class='step'>
                        <div class='step-number'>3</div>
                        <div class='step-content'>
                            <div class='step-title'>Service Delivery</div>
                            <div class='step-description'>
                                Our professional team will arrive on the scheduled date to provide your cleaning services.
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class='contact-box'>
                <p style='margin: 0 0 10px 0; color: #194376; font-weight: 600;'>Have Questions?</p>
                <p style='margin: 0; color: #555555; font-size: 14px;'>
                    Our support team is available at <strong>support@mvmclean.com</strong><br>
                    We're here to ensure your experience is perfect.
                </p>
            </div>
        </div>

        <div class='footer'>
            <p>&copy; 2026 MVM Clean. All rights reserved.</p>
            <p>support@mvmclean.com | 020 XXXX XXXX</p>
            <p>This is an automated message, please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";

        // Plain text template
        PlainTextBody = $@"BOOKING CONFIRMED!

Reference: #{_bookingReference}

Hello {_customerName},

Your booking has been confirmed! We're excited to serve you.

BOOKING DETAILS:
────────────────
Scheduled Date: {_bookingDate:dddd, MMMM d, yyyy}
Service Address: {_address}

SERVICES CONFIRMED:
───────────────────
{string.Join("\n", _services.Select(s => $"✓ {s}"))}

Total Amount: £{_totalAmount:F2}

WHAT HAPPENS NEXT?
──────────────────
1. Contact from Contractor
   Our contractor will contact you within 24 hours to confirm the final details and answer any questions.

2. Preparation
   We'll prepare and ensure everything is ready for your scheduled appointment.

3. Service Delivery
   Our professional team will arrive on the scheduled date to provide your cleaning services.

Have Questions?
Our support team is available at support@mvmclean.com
We're here to ensure your experience is perfect.

────────────────────
© 2026 MVM Clean. All rights reserved.
support@mvmclean.com | 020 XXXX XXXX
This is an automated message, please do not reply to this email.";
    }
}