using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class NoiTruYeuCauTiepNhanTheBHYTViewModel : BaseViewModel
    {
        public long BenhNhanId { get; set; }
        public string MaSoThe { get; set; }
        public int? MucHuong { get; set; }
        public string MaDKBD { get; set; }
        public string NoiDangKyBHYT { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgayDu5Nam { get; set; }
        public DateTime? NgayDuocMienCungChiTra { get; set; }
        public string CoQuanBHXH { get; set; }
        public string MaKhuVuc { get; set; }
        public bool? DuocMienCungChiTra { get; set; }
        public long? GiayMienCungChiTraId { get; set; }
        public Enums.EnumTinhTrangThe? TinhTrangThe { get; set; }
        public bool? IsCheckedBHYT { get; set; }
        public bool? DuocGiaHanThe { get; set; }

        public bool DisableGiaHanThe =>
            DuocGiaHanThe == true || (NgayHetHan != null && NgayHetHan.Value.Date > DateTime.Now.Date);

        public virtual NoiTruGiayMienCungChiTraViewModel GiayMienCungChiTra { get; set; }
    }
}
