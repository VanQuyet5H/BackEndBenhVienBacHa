using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.TiemChung
{
    public class HangDoiTiemChungInputViewModel
    {
        public long? HangDoiDangKhamId { get; set; }
        public long HangDoiBatDauKhamId { get; set; }
        public long? HangDoiVacXinId { get; set; }
        public bool HoanThanhKham { get; set; }
        public long? PhongBenhVienId { get; set; }
        public Enums.LoaiHangDoiTiemVacxin LoaiHangDoi { get; set; }
    }
}
