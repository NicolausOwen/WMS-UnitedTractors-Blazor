using Azure;
using Azure.Communication.Email;
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

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(_smtpSettings.ConnectionString))
        {
            _logger.LogWarning("Email not sent to {ToEmail}. ConnectionString not configured.", toEmail);
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
                    var emailClient = new EmailClient(_smtpSettings.ConnectionString);
                    var emailContent = new EmailContent(subject)
                    {
                        Html = body
                    };

                    var emailMessage = new EmailMessage(
                        senderAddress: _smtpSettings.FromEmail,
                        content: emailContent,
                        recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(toEmail) })
                    );

                    EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                        WaitUntil.Started,
                        emailMessage);

                    await Task.Delay(2000); // Ensures a 2 second gap between emails globally to respect Azure free tier rate limits
                    _logger.LogInformation("Email sent successfully to {ToEmail} with subject: {Subject}. OperationId: {OperationId}", toEmail, subject, emailSendOperation.Id);
                    return; // Success, exit loop
                }
                catch (RequestFailedException ex) when (ex.Status == 429)
                {
                    _logger.LogWarning("Rate limit (429) hit when sending email to {ToEmail}. Retrying in {Delay}ms... (Attempt {Attempt} of {MaxRetries})", toEmail, delayMs, i + 1, maxRetries);
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
                    break; // Don't retry on non-429 errors
                }
            }
        }
        finally
        {
            _emailSemaphore.Release();
        }
    }

    public async Task SendEmailToMultipleAsync(IEnumerable<string> toEmails, string subject, string body)
    {
        var validEmails = toEmails.Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();
        if (!validEmails.Any() || string.IsNullOrWhiteSpace(_smtpSettings.ConnectionString))
        {
            _logger.LogWarning("Multiple email not sent. Targets empty or ConnectionString not configured.");
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
                    var emailClient = new EmailClient(_smtpSettings.ConnectionString);
                    var emailContent = new EmailContent(subject)
                    {
                        Html = body
                    };

                    var emailAddresses = validEmails.Select(e => new EmailAddress(e)).ToList();

                    var emailMessage = new EmailMessage(
                        senderAddress: _smtpSettings.FromEmail,
                        content: emailContent,
                        recipients: new EmailRecipients(emailAddresses)
                    );

                    EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                        WaitUntil.Started,
                        emailMessage);

                    await Task.Delay(2000); // Ensures a 2 second gap between emails globally to respect Azure free tier rate limits
                    _logger.LogInformation("Email sent successfully to {Count} recipients with subject: {Subject}. OperationId: {OperationId}", validEmails.Count, subject, emailSendOperation.Id);
                    return; // Success, exit loop
                }
                catch (RequestFailedException ex) when (ex.Status == 429)
                {
                    _logger.LogWarning("Rate limit (429) hit when sending multiple emails. Retrying in {Delay}ms... (Attempt {Attempt} of {MaxRetries})", delayMs, i + 1, maxRetries);
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
    }
}
