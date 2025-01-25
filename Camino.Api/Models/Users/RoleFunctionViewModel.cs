using Camino.Core.Domain;

namespace Camino.Api.Models.Users
{
    public class RoleFunctionViewModel : BaseViewModel
    {
        public long RoleId { get; set; }
        public Enums.SecurityOperation SecurityOperation { get; set; }
        public Enums.DocumentType DocumentType { get; set; }
        public RoleViewModel Role { get; set; }
    }
}