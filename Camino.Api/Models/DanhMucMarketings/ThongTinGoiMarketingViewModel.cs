using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DanhMucMarketings
{
    public class ThongTinGoiMarketingViewModel : BaseViewModel
    {
        public ThongTinGoiMarketingViewModel()
        {
            //DichVus = new List<DanhSachDichVuTrongGoi>();
            //QuaTangs = new List<DanhSachQuaTangTrongGoi>();
            LstDaChon = new List<long>();
            LstDaHoanThanh = new List<long>();
        }
        //public string TenGoi { get; set; }
        //public List<DanhSachDichVuTrongGoi> DichVus { get; set; }
        //public List<DanhSachQuaTangTrongGoi> QuaTangs { get; set; }



        //public double TiLeChietKhau { get; set; }
        //public string TiLeChietKhauDisplay { get; set; }

        //public decimal GiaTruocChietKhau { get; set; }
        //public decimal GiaSauChietKhau { get; set; }
        //public string GiaTruocChietKhauDisplay { get; set; }
        //public string GiaSauChietKhauDisplay { get; set; }

        public long? BenhNhanId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? QuocTichId { get; set; }
        public long? TinhThanhId { get; set; }
        public string SoChungMinhThu { get; set; }
        public string SoDienThoai { get; set; }

        public List<long> LstDaChon { get; set; }
        public List<long> LstDaHoanThanh { get; set; }
    }

    public class DanhSachDichVuTrongGoi : BaseViewModel
    {
        public int STT { get; set; }
        public string TenNhomDichVu { get; set; }

        public string TenDichVu { get; set; }
        public string Ma { get; set; }
        public string LoaiGiaDisplay { get; set; }
        public string SoLuongDisplay { get; set; }
        public int SoLuong { get; set; }
        public string DonGiaDisplay { get; set; }
        public string ThanhTienDisplay { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
    public class DanhSachQuaTangTrongGoi : BaseViewModel
    {
        public int STT { get; set; }
        public string Ten { get; set; }
        public string SoLuongDisplay { get; set; }
        public int SoLuong { get; set; }
        public string GhiChu { get; set; }

        public long QuaTangId { get; set; }
    }
}
