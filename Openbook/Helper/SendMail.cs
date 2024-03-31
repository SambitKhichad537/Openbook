using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace Openbook.Helper
{

    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        [HttpPost("SendMail")]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            // Validate and use the email request data to send an email using your preferred email sending method (e.g., using SmtpClient)
            // Note: Implement proper validation, error handling, and security measures in a production scenario.

            try
            {
                // Code to send an email using SmtpClient or other email sending method
                // Replace the placeholders with your actual email settings
                string host = "smtp.gmail.com";
                int port = 587;
                string mailAddress = "dreamerbryan2043@gmail.com";
                string password = "prrphbppjninexxv";

                using (SmtpClient client = new SmtpClient(host, port))
                {
                    client.Credentials = new NetworkCredential(mailAddress, password);
                    client.EnableSsl = true;

                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress(mailAddress),
                        Subject = request.Subject,
                        Body = request.Message,
                        IsBodyHtml = true
                    };

                    message.To.Add(request.To);

                    client.Send(message);

                    return Ok(new { Message = "Email sent successfully!" });
                }
            }
            catch (Exception ex)
            {
                // Handle errors appropriately
                return BadRequest(new { Message = $"Error sending email: {ex.Message}" });
            }
        }
    }

    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

}

