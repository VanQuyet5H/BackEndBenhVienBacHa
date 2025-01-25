using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class NoiTruBenhAnViewModel : BaseViewModel
    {
        public long? BenhNhanId { get; set; }
        public string SoBenhAn { get; set; }
        public string SoLuuTru { get; set; }
        public bool? LaCapCuu { get; set; }
        public Enums.LoaiBenhAn? LoaiBenhAn { get; set; }
        public string TenLoaiBenhAn => LoaiBenhAn.GetDescription();
        public DateTime? ThoiDiemTaoBenhAn { get; set; }
        public long? NhanVienTaoBenhAnId { get; set; }
        public long? KhoaPhongNhapVienId { get; set; }
        public DateTime? ThoiDiemNhapVien { get; set; }
        public NoiTruBenhAnThongTinHanhChinhViewModel ThongTinHanhChinh { get; set; }
        public DateTime? ThoiDiemTiepNhanNgoaiTru { get; set; }
    }

    public class NoiTruBenhAnThongTinHanhChinhViewModel : BaseViewModel
    {
        public long BenhNhanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public int? Tuoi
        {
            get { return NamSinh == null ? 0 : DateTime.Now.Year - NamSinh.Value; }
        }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenGioiTinh => GioiTinh.GetDescription();
        public string DanToc { get; set; }
        public string DiaChiDayDu { get; private set; }
        public string NgheNghiep { get; set; }
        public string DoiTuong => BHYTMucHuong != null ? "BHYT" : "Viện phí";
        public int? BHYTMucHuong { get; set; }

        public decimal SoDuTaiKhoan { get; set; }
        public decimal SoDuTaiKhoanConLai { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien => BHYTMucHuong == null ? (Enums.EnumLyDoVaoVien?) null : Enums.EnumLyDoVaoVien.DungTuyen;
        public string Tuyen => LyDoVaoVien.GetDescription();

        public NoiTruBenhAnViewModel NoiTruBenhAn { get; set; }
        public NoiTruBenhAnYeuCauNhapVienViewModel ThongTinNhapVien { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class NoiTruBenhAnYeuCauNhapVienViewModel : BaseViewModel
    {
        public long? KhoaNhapVienId { get; set; }
        public string KhoaNhapVien { get; set; }
        public string ChanDoanNhapVien { get; set; }
        public Enums.TrangThaiDieuTri? TrangThaiDieuTri { get; set; }
        public string TenTrangThaiDieuTri => TrangThaiDieuTri.GetDescription();
        public string NoiChiDinh { get; set; }
        public string ChanDoanKemTheo { get; set; }
        public string NguoiTiepNhan { get; set; }
        public string BacSiChiDinh { get; set; }
        public string LyDoNhapVien { get; set; }
    }
}
