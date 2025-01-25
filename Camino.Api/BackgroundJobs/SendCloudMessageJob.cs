using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Messages;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Messages;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ISendCloudMessageJob))]
    public class SendCloudMessageJob : ISendCloudMessageJob
    {
        private readonly IRepository<QueuedCloudMessaging> _queuedCloudMessagingRepository;
        private readonly ICloudMessagingHandler _cloudMessagingHandler;
        //private readonly IRepository<MessagingUserGroup> _messagingUserGroupRepository;

        public SendCloudMessageJob(IRepository<QueuedCloudMessaging> queuedCloudMessagingRepository, ICloudMessagingHandler cloudMessagingHandler 
            //IRepository<MessagingUserGroup> messagingUserGroupRepository
            )
        {
            _queuedCloudMessagingRepository = queuedCloudMessagingRepository;
            _cloudMessagingHandler = cloudMessagingHandler;
            //_messagingUserGroupRepository = messagingUserGroupRepository;
        }

        public void Run()
        {
            var queuedCloudMessagings = _queuedCloudMessagingRepository.Table.Where(o =>
                    !o.SentDoneOn.HasValue && o.SentTries < 3 &&
                    (o.DontSendBeforeDate == null || (o.DontSendBeforeDate != null && o.DontSendBeforeDate < DateTime.Now)))
                .Take(10).ToList();

            foreach (var queuedCloudMessaging in queuedCloudMessagings)
            {
                var sendUsersFail = false;
                var sendGroupsFail = false;
                if (!string.IsNullOrEmpty(queuedCloudMessaging.ToUserIds))
                {
                    var sentUserIds = string.IsNullOrEmpty(queuedCloudMessaging.SentUserIds)
                        ? new List<string>()
                        : queuedCloudMessaging.SentUserIds.Split(Constants.IdSeparator).ToList();
                    var toUserIds = queuedCloudMessaging.ToUserIds.Split(Constants.IdSeparator);
                    List<string> toUserIdsPerRequest = new List<string>();
                    for (int i = 0; i < toUserIds.Length; i++)
                    {
                        if (sentUserIds.Contains(toUserIds[i]))
                            continue;

                        toUserIdsPerRequest.Add(toUserIds[i]);
                        if ((i + 1) % 5 == 0 || i == toUserIds.Length - 1)
                        {
                            //send success
                            if (_cloudMessagingHandler.SendToMultiTopicsAsync(
                                toUserIdsPerRequest.Select(o => Constants.UserTopicPrefix + o).ToArray(),
                                queuedCloudMessaging.MessagingType, queuedCloudMessaging.Title,
                                queuedCloudMessaging.Body, queuedCloudMessaging.Data).Result)
                            {
                                sentUserIds.AddRange(toUserIdsPerRequest);
                                toUserIdsPerRequest = new List<string>();
                            }
                            //send fail
                            else
                            {
                                sendUsersFail = true;
                                break;
                            }
                        }
                    }
                    if (sentUserIds.Count > 0)
                    {
                        queuedCloudMessaging.SentUserIds = string.Join(Constants.IdSeparator, sentUserIds);
                    }
                }
                //if (!sendUsersFail && !string.IsNullOrEmpty(queuedCloudMessaging.ToMessagingUserGroupIds))
                //{
                //    var sentMessagingUserGroupIds = string.IsNullOrEmpty(queuedCloudMessaging.SentMessagingUserGroupIds)
                //        ? new List<string>()
                //        : queuedCloudMessaging.SentMessagingUserGroupIds.Split(Constants.IdSeparator).ToList();
                //    var toMessagingUserGroupIds = queuedCloudMessaging.ToMessagingUserGroupIds.Split(Constants.IdSeparator);
                //    List<string> toGroupIdsPerRequest = new List<string>();
                //    List<string> toGroupTopicsPerRequest = new List<string>();
                //    for (int i = 0; i < toMessagingUserGroupIds.Length; i++)
                //    {
                //        if (sentMessagingUserGroupIds.Contains(toMessagingUserGroupIds[i]))
                //            continue;

                //        toGroupIdsPerRequest.Add(toMessagingUserGroupIds[i]);

                //        var messagingUserGroup = _messagingUserGroupRepository.TableNoTracking.FirstOrDefault(o =>o.Id == long.Parse(toMessagingUserGroupIds[i]));
                //        if (messagingUserGroup != null)
                //        {
                //            toGroupTopicsPerRequest.Add(messagingUserGroup.Topic);
                //        }

                //        if ((i + 1) % 5 == 0 || i == toMessagingUserGroupIds.Length - 1)
                //        {
                //            //send success
                //            if (_cloudMessagingHandler.SendToMultiTopicsAsync(
                //                toGroupTopicsPerRequest.ToArray(),
                //                queuedCloudMessaging.MessagingType, queuedCloudMessaging.Title,
                //                queuedCloudMessaging.Body, queuedCloudMessaging.Data).Result)
                //            {
                //                sentMessagingUserGroupIds.AddRange(toGroupIdsPerRequest);
                //                toGroupIdsPerRequest = new List<string>();
                //                toGroupTopicsPerRequest = new List<string>();
                //            }
                //            //send fail
                //            else
                //            {
                //                sendGroupsFail = true;
                //                break;
                //            }
                //        }
                //    }
                //    if (sentMessagingUserGroupIds.Count > 0)
                //    {
                //        queuedCloudMessaging.SentMessagingUserGroupIds = string.Join(Constants.IdSeparator, sentMessagingUserGroupIds);
                //    }
                //}

                queuedCloudMessaging.SentTries += 1;

                if (!sendUsersFail && !sendGroupsFail)
                {
                    queuedCloudMessaging.SentDoneOn = DateTime.Now;
                }
                _queuedCloudMessagingRepository.Update(queuedCloudMessaging);
            }
        }
    }
}
