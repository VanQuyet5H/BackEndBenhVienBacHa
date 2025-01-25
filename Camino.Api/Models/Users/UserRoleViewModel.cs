namespace Camino.Api.Models.Users
{
    public class UserRoleViewModel : BaseViewModel
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public UserViewModel User {get; set; }
        public RoleViewModel Role { get; set; }
    }
}