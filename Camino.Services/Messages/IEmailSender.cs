using System.Collections.Generic;
using System.Net.Mail;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.Messages
{
    public interface IEmailSender
    {
        void SendEmail(SmtpClient smtpClient, MailAddress fromEmail, string[] toEmail, string emailSubject, string emailContent, bool isHtmlFormat, Dictionary<string, string> listAttachmentFilename = null, string[] ccEmail = null);

        void SendEmail(string subject, string body,
            string toAddress, string toName = null, string fromAddress = null, string fromName = null,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            List<AttachmentFileVo> attachmentFiles = null,
            IDictionary<string, string> headers = null);
    }
}
