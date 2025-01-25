using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class PhieuInKetQuaInfoVo
    {
        public long Id { get; set; }
        public bool HasHeader { get; set; }
        public string HostingName { get; set; }
    }

    public class PhieuInKetQuaCDHATDCNVo
    {
        public string Header { get; set; }
        public string LogoUrl { get; set; }
        public string MaBarcode { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string DataKetQuaCanLamSang { get; set; }

        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string BacSiChiDinh { get; set; }
        public string NgayChiDinh { get; set; }
        public string NoiChiDinh { get; set; }
        public string NoiThucHien { get; set; }
        public string SoBenhAn { get; set; }
        public string ChanDoan { get; set; }
        public string TenChiDinhDichVu { get; set; }
        public string MaBenhNhan { get; set; }
        public string STTNhanVien { get; set; }

        public ChiTietKetQuaCDHATDCNVo KetQuaChiTiet { get; set; }
        public string TenKetQuaLabel => KetQuaChiTiet.TenKetQuaLabel;
        public string TenKetQua => KetQuaChiTiet.TenKetQua;
        public string KyThuat => KetQuaChiTiet.KyThuat;

        //Update 26/05/2021 khách hàng muốn coi kỹ thuật luôn hiện ra 
        //public string HienThiKyThuat => !string.IsNullOrEmpty(KyThuat) ? "" : "display:none";
        public string HienThiKyThuat => KyThuat;

        //xử lý trường hợp kết quả, kết luận có chứa table -> format cho table
        public string KetQua => KetQuaChiTiet.KetQua?.Replace("<table class=\"k-table\">", "<table class=\"k-table tbl-content-ket-qua\">");
        public string KetLuan => KetQuaChiTiet.KetLuan?.Replace("<table class=\"k-table\">", "<table class=\"k-table tbl-content-ket-qua\">");
        public bool CoInKetQuaKemHinhAnh => KetQuaChiTiet.InKemAnh;
        public List<HinhAnhDinhKemKetQuaVo> HinhAnhDinhKems => KetQuaChiTiet.HinhAnhDinhKems;
        public string HinhAnh { get; set; }

        public string BacSiChuyenKhoa { get; set; }
        public string HocViBacSi { get; set; }        
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string DienGiai { get; set; }

        //Khách hàng muốn bỏ bớt html trên UI nên sinh ra các field này 
        public string InLogo { get; set; }
        public string InBarcode { get; set; }
        public string InTieuDe { get; set; }
        public string InHoVaTen { get; set; }
        public string InNamSinh { get; set; }
        public string InGioiTinh { get; set; }
        public string InDiaChi { get; set; }
        public string InBSChiDinh { get; set; }

        public string InNgayChiDinh { get; set; }
        public string InNoiChiDinh { get; set; }
        public string InNoiThucHien { get; set; }
        public string InSoBenhAn { get; set; }
        public string InChuanDoan { get; set; }
        public string InDienGiai { get; set; }
        public string InChiDinh { get; set; }
        public string InThanhNgang { get; set; }
        public string InKyThuat { get; set; }
        public bool NhomDichVuBenhVienDaChon { get; set; }
    }

    public class CauHinhHinhVo
    {
        public long Id { get; set; }
        public bool HasHeader { get; set; }
        public string HostingName { get; set; }

        public bool InLogo { get; set; }
        public bool InBarcode { get; set; }
        public bool InTieuDe { get; set; }
        public bool InHoVaTen { get; set; }
        public bool InNamSinh { get; set; }
        public bool InGioiTinh { get; set; }
        public bool InDiaChi { get; set; }
        public bool InBSChiDinh { get; set; }

        public bool InNgayChiDinh { get; set; }
        public bool InNoiChiDinh { get; set; }
        public bool InNoiThucHien { get; set; }
        public bool InSoBenhAn { get; set; }
        public bool InChuanDoan { get; set; }
        public bool InDienGiai { get; set; }
        public bool InChiDinh { get; set; }
        public bool InThanhNgang { get; set; }
        public bool InKyThuat { get; set; }
        public int? Zoom { get; set; }
    }


    public class KyThuatBenhVienChonSan
    {
        public long NhomDichVuBenhVienId { get; set; }
        public string Ten { get; set; }
    }
}
