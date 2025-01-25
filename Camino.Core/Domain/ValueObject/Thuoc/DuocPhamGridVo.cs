using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class DuocPhamGridVo : GridItem
    {
        public string Ten { get; set; }
        public string SoDangKy { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public long DuongDungId { get; set; }
        public string TenDuongDung { get; set; }
        public string QuyCach { get; set; }
        public long DonViTinhId { get; set; }
        public string TenDonViTinh { get; set; }
        public bool? IsDisabled { get; set; }
        public string HamLuong { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChatId { get; set; }
        public string TenLoaiThuocHoacHoatChat { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
    }
}
