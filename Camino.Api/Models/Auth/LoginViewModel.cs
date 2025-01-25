using Camino.Core.Domain;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.Auth
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string UserNameRemoveFormat => !string.IsNullOrEmpty(UserName) ? UserName.RemoveFormatPhone() : "";
        public string Password { get; set; }
        public string PassCode { get; set; }
        public string FcmToken { get; set; }

        public HinhThucKhamBenh HinhThucKhamBenh { get; set; }
    }
    public class LoginPassCodeViewModel
    {
        public string Phone { get; set; }
        public string PassCode { get; set; }
        public string FcmToken { get; set; }
    }
}
