using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;

namespace WMS_UnitedTracors_Blazor.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

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
                WaitUntil.Completed,
                emailMessage);

            _logger.LogInformation("Email sent successfully to {ToEmail} with subject: {Subject}. OperationId: {OperationId}", toEmail, subject, emailSendOperation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
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
                WaitUntil.Completed,
                emailMessage);

            _logger.LogInformation("Email sent successfully to {Count} recipients with subject: {Subject}. OperationId: {OperationId}", validEmails.Count, subject, emailSendOperation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to multiple recipients.");
        }
    }
}
