using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using System;

namespace Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans
{
    public class BenhNhanCongTyBaoHiemTuNhan : BaseEntity
    {
        public long BenhNhanId { get; set; }
        public long CongTyBaoHiemTuNhanId { get; set; }
        public string MaSoThe { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual CongTyBaoHiemTuNhan CongTyBaoHiemTuNhan { get; set; }
    }
}
