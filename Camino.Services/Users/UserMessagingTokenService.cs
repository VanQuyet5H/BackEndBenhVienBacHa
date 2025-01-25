using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Helpers;
using Camino.Services.Messages;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.Users
{
    [ScopedDependency(ServiceType = typeof(IUserMessagingTokenService))]
    public class UserMessagingTokenService : MasterFileService<UserMessagingToken>, IUserMessagingTokenService
    {
        private readonly ICloudMessagingHandler _cloudMessagingHandler;

        public UserMessagingTokenService(IRepository<UserMessagingToken> repository, ICloudMessagingHandler cloudMessagingHandler) :
            base(repository)
        {
            _cloudMessagingHandler = cloudMessagingHandler;
        }

        public async Task SetupUserMessagingTokenAsync(long userId, string messagingToken, Enums.DeviceType deviceType)
        {
            var userMessagingToken = BaseRepository.Table.Include(o=>o.UserMessagingTokenSubscribes).FirstOrDefault(o => o.UserId == userId && o.MessagingToken == messagingToken);
            if (userMessagingToken != null)
            {
                //var messagingUserGroups = _messagingUserGroupRepository.TableNoTracking.Where(g => g.UserMessagingUserGroups.Any(ug => ug.UserId == userId)).ToList();
                //foreach (var messagingUserGroup in messagingUserGroups)
                //{
                //    if(userMessagingToken.UserMessagingTokenSubscribes.Any(o=>o.Topic == messagingUserGroup.Topic))
                //        continue;

                //    var userMessagingTokenSubscribe = new UserMessagingTokenSubscribe {Topic = messagingUserGroup.Topic};
                //    userMessagingTokenSubscribe.IsSubscribed = await _cloudMessagingHandler.SubscribeToTopicAsync(userMessagingTokenSubscribe.Topic, userMessagingToken.MessagingToken);
                //    userMessagingToken.UserMessagingTokenSubscribes.Add(userMessagingTokenSubscribe);
                //}

                userMessagingToken.LastAccess = DateTime.Now;
                BaseRepository.Update(userMessagingToken);
            }
            else
            {
                var newUserMessagingToken = new UserMessagingToken
                {
                    UserId = userId,
                    MessagingToken = messagingToken,
                    DeviceType = deviceType,
                    LastAccess = DateTime.Now
                };
                newUserMessagingToken.UserMessagingTokenSubscribes.Add(new UserMessagingTokenSubscribe{Topic = Constants.UserTopicPrefix + userId});

                //var messagingUserGroups = _messagingUserGroupRepository.TableNoTracking.Where(g => g.UserMessagingUserGroups.Any(ug => ug.UserId == userId)).ToList();
                //foreach (var messagingUserGroup in messagingUserGroups)
                //{
                //    newUserMessagingToken.UserMessagingTokenSubscribes.Add(new UserMessagingTokenSubscribe { Topic = messagingUserGroup.Topic });
                //}

                foreach (var userMessagingTokenSubscribe in newUserMessagingToken.UserMessagingTokenSubscribes)
                {
                    //Subscribe topic
                    var isSubscribed = await _cloudMessagingHandler.SubscribeToTopicAsync(userMessagingTokenSubscribe.Topic, messagingToken);
                    userMessagingTokenSubscribe.IsSubscribed = isSubscribed;
                }
                BaseRepository.Add(newUserMessagingToken);
            }
        }
    }
}
