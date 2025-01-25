using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.Users
{
    public class UserMessagingToken : BaseEntity
    {
        public long UserId { get; set; }
        public string MessagingToken { get; set; }
        public Enums.DeviceType DeviceType { get; set; }
        public DateTime LastAccess { get; set; }

        public virtual User User { get; set; }

        private ICollection<UserMessagingTokenSubscribe> _userMessagingTokenSubscribes;
        public virtual ICollection<UserMessagingTokenSubscribe> UserMessagingTokenSubscribes
        {
            get => _userMessagingTokenSubscribes ?? (_userMessagingTokenSubscribes = new List<UserMessagingTokenSubscribe>());
            protected set => _userMessagingTokenSubscribes = value;
        }
    }
}
