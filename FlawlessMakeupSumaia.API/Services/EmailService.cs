using System.Net;
using System.Net.Mail;
using System.Text;
using FlawlessMakeupSumaia.API.Models;
using Microsoft.Extensions.Configuration;

namespace FlawlessMakeupSumaia.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendOrderNotificationToAdminAsync(Order order)
        {
            try
            {
                var adminEmail = _configuration["Email:AdminEmail"];
                if (string.IsNullOrEmpty(adminEmail))
                {
                    _logger.LogWarning("Admin email not configured. Skipping email notification.");
                    return;
                }

                var smtpHost = _configuration["Email:SmtpServer"] ?? _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"] ?? "Flawless Makeup Sumaia";

                // Check if SMTP is properly configured
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("SMTP not properly configured. Skipping email notification.");
                    return;
                }

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    Timeout = 10000 // 10 seconds timeout
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"New Order Received - #{order.OrderNumber}",
                    Body = BuildOrderEmailHtml(order),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(adminEmail);

                // Attach payment proof image if available (skip if too large to avoid slow emails)
                if (!string.IsNullOrEmpty(order.PaymentProofImageUrl) && order.PaymentProofImageUrl.StartsWith("data:image"))
                {
                    try
                    {
                        // Extract base64 data from data URI
                        var base64Data = order.PaymentProofImageUrl.Substring(order.PaymentProofImageUrl.IndexOf(",") + 1);
                        
                        // Skip if Base64 string is larger than 500KB (will be ~650KB as image)
                        if (base64Data.Length > 500000)
                        {
                            _logger.LogWarning($"Payment proof image too large ({base64Data.Length} bytes), skipping attachment for order {order.OrderNumber}");
                        }
                        else
                        {
                            var imageBytes = Convert.FromBase64String(base64Data);
                            
                            // Determine file extension from data URI
                            var extension = "png";
                            if (order.PaymentProofImageUrl.Contains("image/jpeg") || order.PaymentProofImageUrl.Contains("image/jpg"))
                                extension = "jpg";
                            
                            var stream = new MemoryStream(imageBytes);
                            var attachment = new Attachment(stream, $"payment-proof-{order.OrderNumber}.{extension}", "image/" + extension);
                            mailMessage.Attachments.Add(attachment);
                            
                            _logger.LogInformation($"Payment proof image attached to email for order {order.OrderNumber}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error attaching payment proof image: {ex.Message}");
                    }
                }

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Order notification email sent successfully for order {order.OrderNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send order notification email for order {order.OrderNumber}");
                // Don't throw - we don't want email failures to break order creation
            }
        }

        private string BuildOrderEmailHtml(Order order)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            sb.AppendLine(".header { background-color: #ff69b4; color: white; padding: 20px; text-align: center; }");
            sb.AppendLine(".content { padding: 20px; }");
            sb.AppendLine(".order-info { background-color: #f9f9f9; padding: 15px; margin: 10px 0; border-radius: 5px; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            sb.AppendLine("th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("th { background-color: #ff69b4; color: white; }");
            sb.AppendLine(".total { font-size: 18px; font-weight: bold; text-align: right; margin: 20px 0; }");
            sb.AppendLine("</style></head><body>");
            
            sb.AppendLine("<div class='header'>");
            sb.AppendLine("<h1>New Order Received!</h1>");
            sb.AppendLine($"<p>Order #{order.OrderNumber}</p>");
            sb.AppendLine("</div>");
            
            sb.AppendLine("<div class='content'>");
            
            // Customer Information
            sb.AppendLine("<div class='order-info'>");
            sb.AppendLine("<h2>Customer Information</h2>");
            
            if (!string.IsNullOrEmpty(order.UserId))
            {
                sb.AppendLine($"<p><strong>Customer Type:</strong> Registered User</p>");
                sb.AppendLine($"<p><strong>Name:</strong> {order.ShippingFirstName} {order.ShippingLastName}</p>");
            }
            else
            {
                sb.AppendLine($"<p><strong>Customer Type:</strong> Guest</p>");
                sb.AppendLine($"<p><strong>Name:</strong> {order.GuestName ?? $"{order.ShippingFirstName} {order.ShippingLastName}"}</p>");
                if (!string.IsNullOrEmpty(order.GuestEmail))
                {
                    sb.AppendLine($"<p><strong>Email:</strong> {order.GuestEmail}</p>");
                }
            }
            
            sb.AppendLine($"<p><strong>Phone:</strong> {order.ShippingPhone}</p>");
            sb.AppendLine($"<p><strong>Order Date:</strong> {order.OrderDate:f}</p>");
            sb.AppendLine("</div>");
            
            // Shipping Address
            sb.AppendLine("<div class='order-info'>");
            sb.AppendLine("<h2>Shipping Address</h2>");
            sb.AppendLine($"<p>{order.ShippingAddress}</p>");
            if (!string.IsNullOrEmpty(order.ShippingAddress2))
            {
                sb.AppendLine($"<p>{order.ShippingAddress2}</p>");
            }
            sb.AppendLine($"<p>{order.ShippingCity}, {order.ShippingState} {order.ShippingZipCode}</p>");
            sb.AppendLine($"<p>{order.ShippingCountry}</p>");
            sb.AppendLine("</div>");
            
            // Order Items
            sb.AppendLine("<h2>Order Items</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Product</th><th>Quantity</th><th>Unit Price</th><th>Total</th></tr>");
            
            foreach (var item in order.OrderItems)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{item.ProductName}</td>");
                sb.AppendLine($"<td>{item.Quantity}</td>");
                sb.AppendLine($"<td>{item.UnitPrice:C} JOD</td>");
                sb.AppendLine($"<td>{item.TotalPrice:C} JOD</td>");
                sb.AppendLine("</tr>");
            }
            
            sb.AppendLine("</table>");
            
            // Order Summary
            sb.AppendLine("<div class='order-info'>");
            sb.AppendLine("<h2>Order Summary</h2>");
            sb.AppendLine($"<p><strong>Subtotal:</strong> {order.SubTotal:C} JOD</p>");
            sb.AppendLine($"<p><strong>Shipping:</strong> {order.ShippingCost:C} JOD</p>");
            if (order.Tax > 0)
            {
                sb.AppendLine($"<p><strong>Tax:</strong> {order.Tax:C} JOD</p>");
            }
            sb.AppendLine($"<p class='total'><strong>Total:</strong> {order.TotalAmount:C} JOD</p>");
            sb.AppendLine("</div>");
            
            // Payment Information
            sb.AppendLine("<div class='order-info'>");
            sb.AppendLine("<h2>Payment Information</h2>");
            sb.AppendLine($"<p><strong>Payment Method:</strong> {order.PaymentMethod}</p>");
            sb.AppendLine("<p><strong>CliQ:</strong> SUMAIA1991</p>");
            
            // Indicate if payment proof is attached
            if (!string.IsNullOrEmpty(order.PaymentProofImageUrl))
            {
                sb.AppendLine("<div style='margin-top: 15px;'>");
                sb.AppendLine("<p><strong>Payment Proof:</strong> See attached image</p>");
                sb.AppendLine("</div>");
            }
            
            sb.AppendLine("</div>");
            
            // Notes
            if (!string.IsNullOrEmpty(order.Notes))
            {
                sb.AppendLine("<div class='order-info'>");
                sb.AppendLine("<h2>Delivery Notes</h2>");
                sb.AppendLine($"<p>{order.Notes}</p>");
                sb.AppendLine("</div>");
            }
            
            sb.AppendLine("</div>");
            sb.AppendLine("</body></html>");
            
            return sb.ToString();
        }
    }
}

