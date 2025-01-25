using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.ComponentModel;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.ExcelChungTu
{
    public class ExcelFile7980AQueryInfo :QueryInfo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public MauBaoBaoBHYT MauBaoBaoBHYT { get; set; }
        public HinhThucXuatFile HinhThucXuatFile { get; set; }
    }

    public enum MauBaoBaoBHYT
    {
        [Description("Ngoại Trú")]
        MauBaoCao79a = 1,
        [Description("Nội Trú")]
        MauBaoCao80a = 2
    }

    public enum HinhThucXuatFile
    {
        [Description("Báo cáo theo ngày")]
        BaoCaoTheoNgay = 1,
        [Description("Báo cáo theo tháng")]
        BaoCaoTheoThang = 2,
        [Description("Báo cáo theo quí")]
        BaoCaoTheoQuy = 3,
    }


    #region Excel file 79a 80a

    public class ExcelFile7980A : GridItem
    {       
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string NgaySinhDisplay { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string MaThe { get; set; }
        public string MaDKBD { get; set; }
        public string GiaTriTheTu { get; set; }
        public string GiaTriTheDen { get; set; }
        public string MaBenh { get; set; }
        public string MaBenhKhac { get; set; }
        public int MaLyDoVaoVien { get; set; }
        public string MaNoiChuyen { get; set; }
        public string NgayVaoDisplay { get; set; }
        public string NgayRaDisplay { get; set; }
        public int SoNgayDieuTri { get; set; }

        public int KetQuaDieuTri { get; set; }
        public int TinhTrangRaVien { get; set; }


        public decimal TienTongChi { get; set; }
        public decimal TienXetNghiem { get; set; }
        public decimal TienCDHA { get; set; }
        public decimal TienThuoc { get; set; }
        public decimal TienMau { get; set; }
        public decimal TienPTTT { get; set; }
        public decimal TienVTYT { get; set; }
    
        public decimal TienDVKTTyLe { get; set; }
        public decimal TienThuocTyLe { get; set; }
        public decimal TienVTYTTyLe { get; set; }

        public decimal TienKham { get; set; }
        public decimal TienGiuong { get; set; }
        public decimal TienVanChuyen { get; set; }
        public decimal TienBNTuTra { get; set; }
        public decimal TienBHTuTra { get; set; }
        public decimal TienNgoaiDanhSach { get; set; }

        public string MaKhoa { get; set; }
        public int? NamQuyetToan { get; set; }
        public int? ThangQuyetToan { get; set; }

        public string MaKhuVuc { get; set; }
        public int MaLoaiKCB { get; set; }
        public string MaCSKCB { get; set; }
        public decimal TienNguonKhac { get; set; }
    }
    #endregion Excel file 79a 80a
}
