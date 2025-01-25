using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using FirebaseAdmin.Messaging;
using Newtonsoft.Json;

namespace Camino.Services.Messages
{
    [ScopedDependency(ServiceType = typeof(ICloudMessagingHandler))]
    public class CloudMessagingHandler : ICloudMessagingHandler
    {
        public async Task<string> SendAsync(string token)
        {
            // See documentation on defining a message payload.
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { "score", "850" },
                    { "time", "2:45" },
                },
                Token = token,
            };

            // Send a message to the device corresponding to the provided
            // registration token.
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            // Response is a message ID string.
            Console.WriteLine("Successfully sent message: " + response);
            return response;
        }

        public async Task<bool> SubscribeToTopicAsync(string topic, params string[] registrationTokens)
        {
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(registrationTokens, topic);
                return response.SuccessCount == registrationTokens.Length;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> SendToTopicAsync(string topic, Enums.MessagingType messagingType, string title, string body, string data)
        {
            try
            {
                var message = new Message
                {
                    Notification = new Notification
                    {
                        Body = body,
                        Title = title
                    },
                    Topic = topic,
                };
                message.Data = CreateMessageData(messagingType, data);
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return !string.IsNullOrEmpty(response);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Send to multi topics
        /// </summary>
        /// <param name="topics">Can include up to 5 topics</param>
        /// <param name="messagingType"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> SendToMultiTopicsAsync(string[] topics, Enums.MessagingType messagingType, string title, string body, string data)
        {
            if (topics.Length == 0)
                return true;
            try
            {
                var condition = $"'{topics[0]}' in topics";
                for (int i = 1; i < topics.Length; i++)
                {
                    condition += $" || '{topics[i]}' in topics";
                }

                var message = new Message
                {
                    Notification = new Notification
                    {
                        Body = body,
                        Title = title
                    },
                    Condition = condition
                };
                message.Data = CreateMessageData(messagingType, data);
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return !string.IsNullOrEmpty(response);
            }
            catch
            {
                return false;
            }
        }

        private Dictionary<string, string> CreateMessageData(Enums.MessagingType messagingType, string data)
        {
            var messageData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(data))
            {
                messageData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            }
            messageData.Add("MessagingType", ((int)messagingType).ToString());
            return messageData;
        }
    }
}
