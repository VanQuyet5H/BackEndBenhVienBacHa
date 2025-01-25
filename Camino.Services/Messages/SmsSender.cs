using System;
using System.Net;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Amazon;
using Camino.Core.Helpers;

namespace Camino.Services.Messages
{
    [ScopedDependency(ServiceType = typeof(ISmsSender))]
    public class SmsSender : ISmsSender
    {
        private readonly SmsConfig _smsConfig;
        public SmsSender(SmsConfig smsConfig)
        {
            _smsConfig = smsConfig;
        }

        public bool SendSms(string to, string msg)
        {
            var areaCode = Enums.AreaCode.VietNam.GetDescription();
            bool sendSmsSuccess = false;
            try
            {
                
                var twilioConfig = _smsConfig.Twilio;
                var awsConfig = _smsConfig.Aws;
                if (twilioConfig != null && twilioConfig.Active)
                {
                    //TwilioClient.Init(twilioConfig.AccountSID, twilioConfig.AuthToken);
                    //var toNumber = new PhoneNumber(areaCode + to);
                    //var message = MessageResource.Create(toNumber,from: new PhoneNumber(twilioConfig.FromNumber), body: msg);

                    //if (string.IsNullOrEmpty(message.ErrorMessage))
                    //{
                    //    sendSmsSuccess = true;
                    //}
                }
                else if (awsConfig != null && awsConfig.Active)
                {
                    var snsClient = new AmazonSimpleNotificationServiceClient(new BasicAWSCredentials(awsConfig.AccessKey, awsConfig.SecretKey), RegionEndpoint.USEast1);
                    var toCheck = to;
                    if (to.StartsWith("0"))
                    {
                        toCheck = to.Remove(0, 1);
                    }
                    var response = snsClient.PublishAsync(new PublishRequest
                    {
                        Message = msg,
                        PhoneNumber = areaCode + toCheck
                    });
                    if (response.Result.HttpStatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Result.MessageId))
                    {
                        sendSmsSuccess = true;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return sendSmsSuccess;
        }
    }
}
