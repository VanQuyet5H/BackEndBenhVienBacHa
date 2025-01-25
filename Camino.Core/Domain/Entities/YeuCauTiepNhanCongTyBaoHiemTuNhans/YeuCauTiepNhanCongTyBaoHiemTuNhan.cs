using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
namespace Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans
{
    public class YeuCauTiepNhanCongTyBaoHiemTuNhan : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long CongTyBaoHiemTuNhanId { get; set; }
        public string MaSoThe { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual CongTyBaoHiemTuNhan CongTyBaoHiemTuNhan { get; set; }
    }
}
