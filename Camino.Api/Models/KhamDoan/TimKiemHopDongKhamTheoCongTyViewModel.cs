using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamDoan
{
    public class TimKiemHopDongKhamTheoCongTyViewModel : BaseViewModel
    {
        public long? CongTyId { get; set; }
        public long? HopDongId { get; set; }
        public DateTime? NgayHopDong { get; set; }
        public Enums.LoaiHopDong? LoaiHopDong { get; set; }
        public Enums.TrangThaiHopDongKham? TrangThai { get; set; }
        public int? SoBenhNhan { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }
}
