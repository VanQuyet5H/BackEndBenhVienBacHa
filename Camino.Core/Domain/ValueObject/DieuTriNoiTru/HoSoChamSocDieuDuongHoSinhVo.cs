using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class HoSoChamSocDieuDuongHoSinhVo :GridItem
    {
        public HoSoChamSocDieuDuongHoSinhVo()
        {
            NoiTruPhieuDieuTris = new List<NoiTruPhieuDieuTri>();
            ChanDoanKemTheos = new List<string>();
            KhaiThacBenhSus = new List<string>();
            TienSuBenhNhans = new List<string>();
            BenhNhanDiUngThuocs = new List<string>();
            ChanDoanVaoViens = new List<string>();
            LyDoVaoViens = new List<string>();
        }
        public string HoVaTen { get; set; }
        public int? Tuoi { get; set; }
        public bool? GioiTinh { get; set; }
        public string GioiTinhText => GioiTinh == null ? "" : GioiTinh == true ? "Nam" : "Nữ";
        public string DiaChi { get; set; }
        public string NgheNghiep    { get; set; }
        public string DienThoai   { get; set; }
        public string HoTenNguoiNha   { get; set; }
        public string DienThoaiLienLacNguoiNha    { get; set; }
        public DateTime? NgayVaoVien   { get; set; }
        public string NgayVaoVienText { get; set; }
        public string ChanDoankhiVaoVien   { get; set; }
        public string LyDoVaoVien   { get; set; }
        public string KhaiThacBenhSu   { get; set; }
        public string TienSu   { get; set; }
        public bool TienSuDiUngCo   { get; set; }
        //public bool TienSuDiUngKhong   { get; set; }
        public string TienSuDiUngneuCo   { get; set; }
        public bool CoHutThuoc   { get; set; }
        public bool CoNghienRuouBia   { get; set; }
        public string NguoiBenhCoKhuyetTat   { get; set; }
        public string TinhTrangHienTaiCuaNB   { get; set; }
        public bool TinhTrangTinhTao   { get; set; }
        public bool TinhTrangMe   { get; set; }
        public bool TinhTrangLoMo   { get; set; }
        public string KeHoachChamSocVaTheoDoi   { get; set; }
        public bool NBKhoi   { get; set; }
        public bool BSChoVe   { get; set; }
        public bool ChuyenVien   { get; set; }
        public bool NangXinVe   { get; set; }
        public string DanhGiaTinhTrangNBKhiRaVien   { get; set; }
        public string HuongDanNBNhungDieuCanThiet   { get; set; }
        public bool GiayRaVien   { get; set; }
        public bool DonThuoc   { get; set; }
        public bool BienLaiThanhToanVienPhi   { get; set; }
        public bool GiayChungNhanPhauThuat   { get; set; }
        public bool GiayChungSinh   { get; set; }
        public string HenDenKham   { get; set; }
        public string HoSinhTruong   { get; set; }
        public string HoSinhChamSocNguoiBenh { get; set; }

        public List<NoiTruPhieuDieuTri> NoiTruPhieuDieuTris { get; set; }
        public List<string> ChanDoanKemTheos { get; set; }
        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
        public string ThongTinBenhAn { get; set; }
        public string LyDoVaoVienTheoYeuCauTiepNhan { get; set; }
        public List<string> KhaiThacBenhSus { get; set; }
        public List<string> TienSuBenhNhans{ get; set; }
        public List<string> BenhNhanDiUngThuocs { get; set; }

        public List<string> ChanDoanVaoViens { get; set; }
        public List<string> LyDoVaoViens { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }

    }
    public class XacNhanInHoSoChamSocDieuDuongHoSinh
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
    }
    public class InHoSoChamSocDieuDuongHoSinhVo : GridItem
    {
        public string KhoaPhongDangIn { get; set; }
        public string MaTN { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string HoTen { get; set; }
        public string NgayThangNam { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string DienThoai { get; set; }
        public string HoVaTenNguoiNhaKhiCanBaoTin { get; set; }
        public string DienThoaiLienLac { get; set; }
        public string NgayVaoVien { get; set; }
        public string ChanDoanKhiVaoVien { get; set; }
        public string LyDoVaoVien { get; set; }
        public string KhaiThacBenhSu { get; set; }
        public string TienSu { get; set; }
        public string Co { get; set; }
        public string Khong { get; set; }
        public string NeuCoKeTen { get; set; }
        public string CoHutThuoc { get; set; }

        public string CoNghienRuouBia { get; set; }
        public string NguoiBenhCoKhuyetTat { get; set; }
        public string TinhTaoTiepXuc { get; set; }
        public string Me { get; set; }
        public string LoMo { get; set; }
        public string TinhTrangHienTaiCuaNguoiBenh { get; set; }
        public string KeHoachChamSocVaTheoDoi { get; set; }
        public string BSChoVeNha { get; set; }
        public string ChuyenVien { get; set; }
        public string NangXinVe { get; set; }
        public string DanhGiaTinhTrangNBKhiRaVien { get; set; }
        public string HuongDanNBNhungDieuCanThiet { get; set; }
        public string GiayRaVien { get; set; }
        public string DonThuoc { get; set; }
        public string BienLaiThanhToanVienPhi { get; set; }
        public string GiayChungNhanPhauThuat { get; set; }

        public string GiayChungSinh { get; set; }
        public string HenDenKhamLai { get; set; }
        public string NgayHienTai { get; set; }
        public string ThangHienTai { get; set; }
        public string NamHienTai { get; set; }

        public string DieuDuongTruongKhoa { get; set; }
        public string DieuDuongChamSocNguoiBenh { get; set; }
    }
 }
