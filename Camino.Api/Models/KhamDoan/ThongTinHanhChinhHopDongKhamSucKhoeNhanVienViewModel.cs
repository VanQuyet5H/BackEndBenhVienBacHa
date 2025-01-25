using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamDoan
{
    public class ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel : BaseViewModel
    {
        public ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel()
        {
            DichVuChiDinhThems = new List<TiepNhanDichVuChiDinhViewModel>();
            DichVuChiDinhTrongGois = new List<TiepNhanDichVuChiDinhViewModel>();
        }
        public long HopDongKhamSucKhoeId { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string HoTenKhac { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public DateTime? NgayThangNamSinh => (NgaySinh == null || ThangSinh == null || NamSinh == null || NgaySinh == 0 || ThangSinh == 0 || NamSinh == 0)
                                                    ? (DateTime?)null : new DateTime(NamSinh.Value, ThangSinh.Value, NgaySinh.Value);
        public int? Tuoi => NamSinh != null ? (DateTime.Now.Year - NamSinh.Value) : (int?) null;
        public string SoDienThoai { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? NgheNghiepId { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public int? NhomMau { get; set; }
        public int? YeuToRh { get; set; }
        public string Email { get; set; }
        public string TenDonViHoacBoPhan { get; set; }
        public string NhomDoiTuongKhamSucKhoe { get; set; }
        public long GoiKhamSucKhoeId { get; set; }
        public string TenGoiKhamSucKhoe { get; set; }
        public Enums.TinhTrangHonNhan TinhTrangHonNhan { get; set; }
        public bool CoMangThai { get; set; }
        public string GhiChuTienSuBenh { get; set; }
        public string GhiChuDiUngThuoc { get; set; }
        public string DiaChiDayDu { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public bool? HuyKham { get; set; }


        public List<TiepNhanDichVuChiDinhViewModel> DichVuChiDinhThems { get; set; }
        public List<TiepNhanDichVuChiDinhViewModel> DichVuChiDinhTrongGois { get; set; }
    }
}
