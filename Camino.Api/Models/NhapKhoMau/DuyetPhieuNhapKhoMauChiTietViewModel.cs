using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Helpers;

namespace Camino.Api.Models.NhapKhoMau
{
    public class DuyetPhieuNhapKhoMauChiTietViewModel : BaseViewModel
    {
        public string MaTuiMau { get; set; }
        public string TenMauVaChePham { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public string NgaySanXuatDisplay => NgaySanXuat?.ApplyFormatDate();
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public decimal? DonGiaNhap { get; set; }
        public decimal? DonGiaBan { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public bool IsThanhToan { get; set; }

        public virtual DuyetPhieuNhapKhoMauViewModel NhapKhoMau { get; set; }
    }
}
