using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Services.Messages
{
    public interface ICloudMessagingHandler
    {
        Task<bool> SubscribeToTopicAsync(string topic, params string[] registrationTokens);
        Task<bool> SendToTopicAsync(string topic, Enums.MessagingType messagingType, string title, string body, string data);
        Task<bool> SendToMultiTopicsAsync(string[] topics, Enums.MessagingType messagingType, string title, string body, string data);
    }
}
