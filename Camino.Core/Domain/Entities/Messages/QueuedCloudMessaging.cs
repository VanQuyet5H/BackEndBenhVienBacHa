using System;

namespace Camino.Core.Domain.Entities.Messages
{
    public class QueuedCloudMessaging : BaseEntity
    {
        public string ToUserIds { get; set; }
        public string ToMessagingUserGroupIds { get; set; }
        public string SentUserIds { get; set; }
        public string SentMessagingUserGroupIds { get; set; }
        public Enums.MessagingType MessagingType { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Data { get; set; }
        public string Link { get; set; }
        public DateTime? DontSendBeforeDate { get; set; }
        public int SentTries { get; set; }
        public DateTime? SentDoneOn { get; set; }
    }
}
