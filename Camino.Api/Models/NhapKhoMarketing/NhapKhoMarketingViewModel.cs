using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoMarketing
{
    public class NhapKhoMarketingViewModel : BaseViewModel
    {
        public NhapKhoMarketingViewModel()
        {
            DanhSachQuaTangDuocThemList = new List<DanhSachQuaTangDuocThem>();
        }

        public string SoChungTu { get; set; }
        public Enums.LoaiNguoiGiaoNhan? LoaiNguoiGiao { get; set; }
        public string TenNguoiGiao{ get; set; }
        //public long? NhanVienGiaoId { get; set; }
        public long? NguoiGiaoId { get; set; }
        public long? NguoiNhapId { get; set; }
        public DateTime? NgayNhap { get; set; }
        public bool DaHet { get; set; }
        public string NhaCungCap { get; set; }
        public string QuaTang { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuong { get; set; }
        public double? GiaNhap { get; set; }
        public double? ThanhTien { get; set; }
        public List<DanhSachQuaTangDuocThem> DanhSachQuaTangDuocThemList { get; set; }
    }
    public class DanhSachQuaTangDuocThem : BaseViewModel
    {
        public long NhapKhoQuaTangId { get; set; }
        public long QuaTangId { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public int SoLuongDaXuat { get; set; }
        public string NhaCungCap { get; set; }
        public string DonViTinh { get; set; }
        public int? SoLuongNhap { get; set; }
    }
  
        
    
}
