using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BenhVien
{
    public class BenhVienGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string DiaChi { get; set; }
        public bool? HieuLuc { get; set; }
        public long LoaiBenhVienId { get; set; }
        //public long CapQuanLyBenhVienId { get; set; }
        public long DonViHanhChinhId { get; set; }
        public Enums.HangBenhVien? HangBenhVien { get; set; }
        public string HangBenhVienDisplay { get; set; }
        public Enums.TuyenChuyenMonKyThuat? TuyenChuyenMonKyThuat { get; set; }
        public string TuyenChuyenMonKyThuatDisplay { get; set; }
        public string SoDienThoaiLanhDao { get; set; }

        public string SoDienThoaiDisplay { get; set; }

        public string TenLoaiBenhVien { get; set; }
        public string TenCapQuanLyBenhVien { get; set; }
        public string TenDonViHanhChinh { get; set; }
    }
}
