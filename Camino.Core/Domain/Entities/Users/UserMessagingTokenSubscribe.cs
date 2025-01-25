namespace Camino.Core.Domain.Entities.Users
{
    public class UserMessagingTokenSubscribe : BaseEntity
    {
        public long UserMessagingTokenId { get; set; }
        public string Topic { get; set; }
        public bool IsSubscribed { get; set; }

        public virtual UserMessagingToken UserMessagingToken { get; set; }
    }
}
