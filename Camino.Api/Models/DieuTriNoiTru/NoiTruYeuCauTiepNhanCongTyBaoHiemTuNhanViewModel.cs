using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class NoiTruYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel : BaseViewModel
    {
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
        public string MaSoThe { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }
}
