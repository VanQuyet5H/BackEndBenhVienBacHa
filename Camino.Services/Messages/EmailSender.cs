using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Services.TaiLieuDinhKem;
using DotLiquid.Tags;

namespace Camino.Services.Messages
{
    [ScopedDependency(ServiceType = typeof(IEmailSender))]
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _emailConfig;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;

        public EmailSender(EmailConfig emailConfig, ITaiLieuDinhKemService taiLieuDinhKemService)
        {
            _emailConfig = emailConfig;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }
        public void SendEmail(SmtpClient smtpClient, MailAddress fromEmail, string[] toEmail, string emailSubject, string emailContent, bool isHtmlFormat
                               , Dictionary<string, string> listAttachmentFilename = null, string[] ccEmail = null)
        {
            if (string.IsNullOrEmpty(smtpClient.Host) || smtpClient.Port == 0 || string.IsNullOrEmpty(emailSubject))
                return;
            var message = new MailMessage { From = fromEmail };
            if (toEmail.Length > 0)
            {
                foreach (var item in toEmail.Where(item => !string.IsNullOrEmpty(item)))
                {
                    message.To.Add(new MailAddress(item));
                }
            }
            if (ccEmail != null && ccEmail.Length > 0)
            {
                foreach (var item in toEmail.Where(item => !string.IsNullOrEmpty(item)))
                {
                    message.CC.Add(new MailAddress(item));
                }
            }
            message.Subject = emailSubject;
            message.Body = emailContent;
            message.IsBodyHtml = isHtmlFormat;
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            if (listAttachmentFilename != null && listAttachmentFilename.Count > 0)
            {
                foreach (var attachmentFilename in listAttachmentFilename)
                {
                    if (File.Exists(attachmentFilename.Value))
                    {
                        var attachment = new Attachment(attachmentFilename.Value, MediaTypeNames.Application.Octet);
                        var disposition = attachment.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(attachmentFilename.Value);
                        disposition.ModificationDate = File.GetLastWriteTime(attachmentFilename.Value);
                        disposition.ReadDate = File.GetLastAccessTime(attachmentFilename.Value);
                        disposition.FileName = attachmentFilename.Key;
                        disposition.Size = new FileInfo(attachmentFilename.Value).Length;
                        disposition.DispositionType = DispositionTypeNames.Attachment;
                        message.Attachments.Add(attachment);
                    }
                }
            }
            smtpClient.Send(message);
        }

        public virtual void SendEmail(string subject, string body,
            string toAddress, string toName = null,
            string fromAddress = null, string fromName = null,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            List<AttachmentFileVo> attachmentFiles = null, IDictionary<string, string> headers = null)
        {
            var message = new MailMessage
            {
                //from, to, reply to
                From = new MailAddress(string.IsNullOrEmpty(fromAddress) ? _emailConfig.From : fromAddress, string.IsNullOrEmpty(fromName) ? _emailConfig.DisplayName : fromName)
            };
            message.To.Add(string.IsNullOrEmpty(toName) ? new MailAddress(toAddress) : new MailAddress(toAddress, toName));
            if (!string.IsNullOrEmpty(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
            }

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            //CC
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            //content
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;

            //headers
            if (headers != null)
                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

            //create the file attachment for this e-mail message

            if (attachmentFiles != null)
            {
                foreach (var attachmentFileVo in attachmentFiles)
                {
                    using (var stream = _taiLieuDinhKemService.GetObjectStream(attachmentFileVo.DuongDan, attachmentFileVo.TenGuid))
                    {
                        message.Attachments.Add(new Attachment(stream, attachmentFileVo.Ten));
                    }
                }
            }

            //send email
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = _emailConfig.UseDefaultCredentials;
                smtpClient.Host = _emailConfig.Host;
                smtpClient.Port = _emailConfig.Port;
                smtpClient.EnableSsl = _emailConfig.EnableSsl;
                smtpClient.Credentials = _emailConfig.UseDefaultCredentials ?
                    CredentialCache.DefaultNetworkCredentials :
                    new NetworkCredential(_emailConfig.From, _emailConfig.Password);
                smtpClient.Send(message);
            }
        }
    }
}
