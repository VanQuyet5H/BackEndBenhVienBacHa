namespace Camino.Core.Domain.Entities.Users
{
    public class RoleFunction : BaseEntity
    {
        public long RoleId { get; set; }
        public Enums.SecurityOperation SecurityOperation { get; set; }
        public Enums.DocumentType DocumentType { get; set; }
        public virtual Role Role { get; set; }
    }
}
