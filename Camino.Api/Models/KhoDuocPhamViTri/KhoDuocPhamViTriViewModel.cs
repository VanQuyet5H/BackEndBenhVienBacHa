using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhoDuocPhamViTri
{
    public class KhoDuocPhamViTriViewModel : BaseViewModel
    {
        public long KhoDuocPhamId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public string TenKhoDuocPham { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
