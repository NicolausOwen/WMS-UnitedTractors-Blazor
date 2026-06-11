namespace WMS_UnitedTracors_Blazor.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendEmailToMultipleAsync(IEnumerable<string> toEmails, string subject, string body);
}
