using System;
using System.Linq;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Messages;
using Camino.Data;
using Camino.Services.Messages;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ISendSmsJob))]
    public class SendSmsJob : ISendSmsJob
    {
        private readonly IRepository<QueuedSms> _queuedSmsRepository;
        private readonly IRepository<LichSuSMS> _lichSuSmsRepository;
        private readonly ISmsSender _smsSender;
        private readonly SmsConfig _smsConfig;

        public SendSmsJob(IRepository<QueuedSms> queuedSmsRepository, IRepository<LichSuSMS> lichSuSmsRepository, ISmsSender smsSender, SmsConfig smsConfig)
        {
            _queuedSmsRepository = queuedSmsRepository;
            _lichSuSmsRepository = lichSuSmsRepository;
            _smsSender = smsSender;
            _smsConfig = smsConfig;
        }

        public void Run()
        {
            _queuedSmsRepository.AutoCommitEnabled = false;
            _lichSuSmsRepository.AutoCommitEnabled = false;
            var queuedSmsess = _queuedSmsRepository.Table.Where(o =>
                    !o.SentOn.HasValue && o.SentTries < _smsConfig.MaxRetries &&
                    (o.DontSendBeforeDate == null || (o.DontSendBeforeDate != null && o.DontSendBeforeDate < DateTime.Now)))
                .Take(10).ToList();

            foreach (var queuedSms in queuedSmsess)
            {
                var sendSmsSuccess = false;
                try
                {
                    _smsSender.SendSms(queuedSms.To, queuedSms.Body);
                    sendSmsSuccess = true;
                    queuedSms.SentOn = DateTime.Now;
                }
                finally
                {
                    queuedSms.SentTries = queuedSms.SentTries + 1;
                    _queuedSmsRepository.Update(queuedSms);
                    if (sendSmsSuccess || queuedSms.SentTries == _smsConfig.MaxRetries)
                    {
                        _lichSuSmsRepository.Add(new LichSuSMS
                        {
                            GoiDen = queuedSms.To,
                            NgayGui = DateTime.Now,
                            NoiDung = queuedSms.Body,
                            TrangThai = sendSmsSuccess ? Enums.TrangThaiLishSu.ThanhCong : Enums.TrangThaiLishSu.ThatBai
                        });
                    }
                    _queuedSmsRepository.Context.SaveChanges();
                }
            }

        }
    }
}
