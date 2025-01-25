using System;
using System.Collections.Generic;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Data;
using Camino.Services.Messages;
using System.Linq;
using Camino.Core.Domain.Entities.Messages;
using Camino.Core.Domain.ValueObject;
using Newtonsoft.Json;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ISendEmailJob))]
    public class SendEmailJob : ISendEmailJob
    {
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private readonly IRepository<LichSuEmail> _lichSuEmailRepository;
        private readonly IEmailSender _emailSender;
        private readonly EmailConfig _emailConfig;

        public SendEmailJob(IRepository<QueuedEmail> queuedEmailRepository, IRepository<LichSuEmail> lichSuEmailRepository, IEmailSender emailSender, EmailConfig emailConfig)
        {
            _queuedEmailRepository = queuedEmailRepository;
            _lichSuEmailRepository = lichSuEmailRepository;
            _emailSender = emailSender;
            _emailConfig = emailConfig;
        }
        
        public void Run()
        {
            _queuedEmailRepository.AutoCommitEnabled = false;
            _lichSuEmailRepository.AutoCommitEnabled = false;
            var queuedEmails = _queuedEmailRepository.Table.Where(o =>
                    !o.SentOn.HasValue && o.SentTries < _emailConfig.MaxRetries &&
                    (o.DontSendBeforeDate == null || (o.DontSendBeforeDate != null && o.DontSendBeforeDate < DateTime.Now)))
                .Take(10).ToList();

            foreach (var queuedEmail in queuedEmails)
            {
                var sendEmailSuccess = false;
                try
                {
                    var attachmentFiles = new List<AttachmentFileVo>();
                    if (!string.IsNullOrEmpty(queuedEmail.AttachmentFile))
                    {
                        attachmentFiles = JsonConvert.DeserializeObject<List<AttachmentFileVo>>(queuedEmail.AttachmentFile);
                    }
                    _emailSender.SendEmail(queuedEmail.Subject, queuedEmail.Body, queuedEmail.To, attachmentFiles:attachmentFiles);
                    sendEmailSuccess = true;
                    queuedEmail.SentOn = DateTime.Now;

                }
                finally
                {
                    queuedEmail.SentTries = queuedEmail.SentTries + 1;
                    _queuedEmailRepository.Update(queuedEmail);
                    if (sendEmailSuccess || queuedEmail.SentTries == _emailConfig.MaxRetries)
                    {
                        _lichSuEmailRepository.Add(new LichSuEmail
                        {
                            GoiDen = queuedEmail.To,
                            NgayGui = DateTime.Now,
                            TieuDe = queuedEmail.Subject,
                            NoiDung = queuedEmail.Body,
                            TapTinDinhKem = null,
                            TrangThai = sendEmailSuccess ? Enums.TrangThaiLishSu.ThanhCong : Enums.TrangThaiLishSu.ThatBai
                        });
                    }
                    _queuedEmailRepository.Context.SaveChanges();
                }
            }
            
        }
    }
}
