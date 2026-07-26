using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace WMS_UnitedTracors_Blazor.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;
    private static readonly SemaphoreSlim _emailSemaphore = new SemaphoreSlim(1, 1);

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public Task SendEmailAsync(string toEmail, string subject, string body)
    {
        _ = Task.Run(async () =>
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(_smtpSettings.Host))
            {
                _logger.LogWarning("Email not sent to {ToEmail}. SMTP Host not configured.", toEmail);
                return;
            }

            int maxRetries = 3;
            int delayMs = 2000;

            await _emailSemaphore.WaitAsync();
            try
            {
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                        {
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                            EnableSsl = _smtpSettings.EnableSsl,
                            DeliveryMethod = SmtpDeliveryMethod.Network
                        };

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        };
                        mailMessage.To.Add(toEmail);

                        await client.SendMailAsync(mailMessage);
                        _logger.LogInformation("Email sent successfully to {ToEmail} with subject: {Subject}", toEmail, subject);
                        return; // Success, exit loop
                    }
                    catch (SmtpException ex)
                    {
                        _logger.LogWarning("SMTP Exception hit when sending email to {ToEmail}. Retrying in {Delay}ms... (Attempt {Attempt} of {MaxRetries}). Error: {Error}", toEmail, delayMs, i + 1, maxRetries, ex.Message);
                        if (i == maxRetries - 1)
                        {
                            _logger.LogError(ex, "Max retries reached. Error sending email to {ToEmail}", toEmail);
                        }
                        else
                        {
                            await Task.Delay(delayMs);
                            delayMs *= 2; // Exponential backoff
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
                        break; 
                    }
                }
            }
            finally
            {
                _emailSemaphore.Release();
            }
        });

        return Task.CompletedTask;
    }

    public Task SendEmailToMultipleAsync(IEnumerable<string> toEmails, string subject, string body)
    {
        _ = Task.Run(async () =>
        {
            var validEmails = toEmails.Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();
            if (!validEmails.Any() || string.IsNullOrWhiteSpace(_smtpSettings.Host))
            {
                _logger.LogWarning("Multiple email not sent. Targets empty or SMTP Host not configured.");
                return;
            }
            
            _logger.LogInformation("DEBUG EMAIL: Will send email with subject '{Subject}' to {Count} recipients: {Emails}", subject, validEmails.Count, string.Join(", ", validEmails));

            int maxRetries = 3;
            int delayMs = 2000;

            await _emailSemaphore.WaitAsync();
            try
            {
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                        {
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                            EnableSsl = _smtpSettings.EnableSsl,
                            DeliveryMethod = SmtpDeliveryMethod.Network
                        };

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        };

                        // CC all valid emails so they all get the email
                        foreach (var email in validEmails)
                        {
                            mailMessage.Bcc.Add(email); 
                        }
                        
                        // We need at least one To recipient, usually the sender themselves for bulk bcc
                        mailMessage.To.Add(_smtpSettings.FromEmail);

                        await client.SendMailAsync(mailMessage);
                        _logger.LogInformation("Email sent successfully to {Count} recipients with subject: {Subject}", validEmails.Count, subject);
                        return; // Success, exit loop
                    }
                    catch (SmtpException ex)
                    {
                        _logger.LogWarning("SMTP Exception hit when sending multiple emails. Retrying in {Delay}ms... (Attempt {Attempt} of {MaxRetries}). Error: {Error}", delayMs, i + 1, maxRetries, ex.Message);
                        if (i == maxRetries - 1)
                        {
                            _logger.LogError(ex, "Max retries reached. Error sending email to multiple recipients.");
                        }
                        else
                        {
                            await Task.Delay(delayMs);
                            delayMs *= 2;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending email to multiple recipients.");
                        break;
                    }
                }
            }
            finally
            {
                _emailSemaphore.Release();
            }
        });

        return Task.CompletedTask;
    }
}
