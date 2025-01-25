using System;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.KhamDoanChiSoSinhTons
{
    public class KhamDoanHopDongKhamSucKhoeNhanVienViewModel : BaseViewModel
    {
        public string MaNhanVien { get; set; }

        public string HoTen { get; set; }

        public string HoTenKhac { get; set; }

        public int? NgaySinh { get; set; }

        public int? ThangSinh { get; set; }

        public int? NamSinh { get; set; }

       // public DateTime NgayThangNamSinh => ThangSinh != 0 && ThangSinh != null && NamSinh != null ? new DateTime(NamSinh ?? 1945, ThangSinh ?? 0, NgaySinh ?? 0) : new DateTime(NamSinh ?? 1945, 0, 0);
        public int Tuoi => DateTime.Now.Year - (NamSinh ?? DateTime.Now.Year);

        public string SoDienThoaiDisplay { get; set; }

        public string SoChungMinhThu { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public long? NgheNghiepId { get; set; }

        public string NgheNghiep { get; set; }

        public long? QuocTichId { get; set; }

        public string QuocTich { get; set; }

        public long? DanTocId { get; set; }

        public string DanToc { get; set; }

        public long? TinhThanhId { get; set; }

        public string TinhThanh { get; set; }

        public long? QuanHuyenId { get; set; }

        public string QuanHuyen { get; set; }

        public long? PhuongXaId { get; set; }

        public string PhuongXa { get; set; }

        public string DiaChi { get; set; }

        public string Email { get; set; }

        public string TenDonViHoacBoPhan { get; set; }

        public Enums.EnumNhomMau? NhomMau { get; set; }

        public string NhomMauDisplay => NhomMau != null ? NhomMau.GetDescription() : string.Empty;

        public Enums.EnumYeuToRh? YeuToRh { get; set; }

        public string YeuToRhDisplay => YeuToRh != null ? YeuToRh.GetDescription() : string.Empty;

        public bool DaLapGiaDinh { get; set; }

        public string DaLapGiaDinhDisplay => DaLapGiaDinh ? "Có gia đình" : "Chưa kết hôn";

        public bool CoMangThai { get; set; }

        public string NhomDoiTuongKhamSucKhoe { get; set; }

        public string GhiChuTienSuBenh { get; set; }

        public string GhiChuDiUngThuoc { get; set; }
    }
}
