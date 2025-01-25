using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.Auth
{
    public class AccessUser
    {
        public AccessToken AccessToken { get; set; }
        public string UserName { get; set; }
        public long Id { get; set; }
        public MenuInfo MenuInfo { get; set; }
        public long PhongBenhVienId { get; set; }
        public HinhThucKhamBenh HinhThucKhamBenh { get; set; }
        public ICollection<CaminoPermission> Permissions { get; set; }
    }
}
