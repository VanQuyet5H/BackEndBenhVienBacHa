using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.YeuCauTiepNhanLichSuChuyenDoiTuongs
{
    public class YeuCauTiepNhanLichSuChuyenDoiTuong : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public Enums.DoiTuongTiepNhan DoiTuongTiepNhan { get; set; }
        public string MaSoThe { get; set; }
        public int MucHuong { get; set; }
        public string MaDKBD { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string DiaChi { get; set; }
        public string CoQuanBHXH { get; set; }
        public DateTime? NgayDu5Nam { get; set; }
        public DateTime? NgayDuocMienCungChiTra { get; set; }
        public string MaKhuVuc { get; set; }
        public bool? DuocMienCungChiTra { get; set; }
        public long? GiayMienCungChiTraId { get; set; }
        public Enums.EnumTinhTrangThe? TinhTrangThe { get; set; }
        public bool? IsCheckedBHYT { get; set; }
        public bool? DuocGiaHanThe { get; set; }
        public bool? DaHuy { get; set; }


        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual GiayMienCungChiTra GiayMienCungChiTra { get; set; }
    }
}
