using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DuTruMuaKSNKTaiKhoaDuoc
{
   
        public class DuTruMuaKSNKTaiKhoaDuocDaXuLyGridVo : GridItem
        {
            public string SoPhieu { get; set; }
            public string DuTruTheo { get; set; }
            public string NguoiYeuCau { get; set; }
            public DateTime NgayYeuCau { get; set; }
            public string TinhTrang { get; set; }
            public string NgayGiamDocDuyet { get; set; }
            public string GhiChu { get; set; }
        }
        public class DuTruMuaKSNKTaiKhoaDuocDaXuLyChildGridVo : GridItem
        {
            public string Loai { get; set; }
            public string VatTu { get; set; }
            public string HoatChat { get; set; }
            public string NongDoVaHamLuong { get; set; }
            public string SDK { get; set; }
            public string DVT { get; set; }
            public string DD { get; set; }
            public string NhaSX { get; set; }
            public string NuocSX { get; set; }
            public string NhomDieuTriDuPhong { get; set; }
            public int SLDuTru { get; set; }
            public int SLDuKienSuDungTrongKy { get; set; }
            public int SLDuTruTKhoaDuyet { get; set; }
            public int SLDuTruKDuocDuyet { get; set; }
        }
        public class DuTruMuaKSNKTaiKhoaDuocDaXuLyChildChildGridVo : GridItem
        {
            public string Nhom { get; set; }
            public string Kho { get; set; }
            public string KyDuTru { get; set; }
            public int SLDuTru { get; set; }
            public int SLDuKienSuDungTrongKy { get; set; }

        }
}
