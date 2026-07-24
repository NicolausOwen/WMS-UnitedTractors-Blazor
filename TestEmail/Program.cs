using System;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;

namespace TestEmail
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "endpoint=https://wmsemailcs.asiapacific.communication.azure.com/;accesskey=2iFm7TUaINjYSBGHCoaPuCGDX37MByT4puKXPHdhNYOZRsFyZGdtJQQJ99CFACULyCpkKCLvAAAAAZCSvyEC";
            string fromEmail = "DoNotReply@e0ca7c57-01b7-46cb-bbf4-51b18d6836f5.azurecomm.net";
            string toEmail = "test@example.com"; 
            
            try
            {
                var emailClient = new EmailClient(connectionString);
                var emailContent = new EmailContent("Test Email") { Html = "<p>Test from WMS Debugger</p>" };
                var emailMessage = new EmailMessage(
                    senderAddress: fromEmail,
                    content: emailContent,
                    recipients: new EmailRecipients(new System.Collections.Generic.List<EmailAddress> { new EmailAddress(toEmail) })
                );

                Console.WriteLine("Sending email...");
                EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
                Console.WriteLine("Success! OperationId: " + emailSendOperation.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }
    }
}
