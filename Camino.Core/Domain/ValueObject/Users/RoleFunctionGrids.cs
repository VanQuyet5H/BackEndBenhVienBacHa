namespace Camino.Core.Domain.ValueObject.Users
{
    public class RoleFunctionGrids
    {
        public bool IsView { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public long RoleId { get; set; }
        public Enums.DocumentType DocumentType { get; set; }
        public string DocumentName { get; set; }

    }
}