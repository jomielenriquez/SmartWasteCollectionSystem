using System.Net.Mail;
using System.Net;
using SmartWasteCollectionSystem.Models;

namespace SmartWasteCollectionSystem.Service
{
    /*
    Email Service
    ✅ Steps to set up Gmail for SMTP in .NET
    1. Enable 2-Step Verification (2FA)
        - Log in to your Gmail account.
        - Go to Google Account → Security → 2-Step Verification.
        - Turn it on. This is required to generate app passwords.
    2. Generate an App Password (instead of your Gmail password)
        - Still in Google Account → Security, go to App passwords.
        - Choose Mail as the app and Windows Computer (or Custom) as the device.
        - Google will give you a 16-character password (like abcd efgh ijkl mnop).
    */
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Results<string> SendEmail(string to, string subject, string body)
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUser = _configuration["Email:Email"] ?? "";
                string smtpPass = _configuration["Email:Password"] ?? "";

                using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = true;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(smtpUser);
                    mail.To.Add(to);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true; // ✅ Enable HTML format

                    smtp.Send(mail);
                    Console.WriteLine("HTML Email sent successfully!");
                }

                return new Results<string>
                {
                    IsSuccess = true,
                    Data = "Email sent successfully!"
                };
            }
            catch (Exception ex)
            {
                return new Results<string>
                {
                    IsSuccess = false,
                    Data = ex.Message
                };
            }
        }

        public Results<string> SendEmail(string to, string subject, string body, byte[] attachmentBytes = null, string attachmentFileName = "attachment.pdf")
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUser = _configuration["Email:Email"] ?? "";
                string smtpPass = _configuration["Email:Password"] ?? "";

                using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = true;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(smtpUser);
                    mail.To.Add(to);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true; // ✅ For HTML content

                    // ✅ Add attachment if provided
                    if (attachmentBytes != null && attachmentBytes.Length > 0)
                    {
                        using (var stream = new MemoryStream(attachmentBytes))
                        {
                            Attachment attachment = new Attachment(stream, attachmentFileName, "application/pdf");
                            mail.Attachments.Add(attachment);

                            smtp.Send(mail);
                        }
                    }
                    else
                    {
                        smtp.Send(mail);
                    }
                }

                return new Results<string>
                {
                    IsSuccess = true,
                    Data = "Email sent successfully!"
                };
            }
            catch (Exception ex)
            {
                return new Results<string>
                {
                    IsSuccess = false,
                    Data = ex.Message
                };
            }
        }
    }
}
