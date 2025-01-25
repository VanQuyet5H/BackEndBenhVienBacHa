using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Org.BouncyCastle.Bcpg;

namespace Camino.Api.Models.XuatKhos
{
    public class XuatKhoDuocPhamViewModel : BaseViewModel
    {
        public XuatKhoDuocPhamViewModel()
        {
            XuatKhoDuocPhamChiTiets = new List<DuocPhamXuatChiTiet>();
        }
        public long? KhoDuocPhamXuatId { get; set; }
        public string KhoDuocPhamXuatDisplay { get; set; }
        public long? KhoDuocPhamNhapId { get; set; }
        public string KhoDuocPhamNhapDisplay { get; set; }
        public string SoPhieu { get; set; }
        public Enums.XuatKhoDuocPham? LoaiXuatKho { get; set; }
        public string LoaiXuatKhoDisplay { get; set; }
        public long? NguoiNhanId { get; set; }
        public string NguoiNhanDisplay { get; set; }
        public long? NguoiXuatId { get; set; }
        public string NguoiXuatDisplay { get; set; }
        public Enums.LoaiNguoiGiaoNhan? LoaiNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; }
        public string LyDoXuatKho { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string HostingName { get; set; }

        
        public bool IsXuatKhac { get; set; }

        public bool? TraNCCDisplayTheoKho { get; set; }
        public bool? TraNCC { get; set; }

        //public List<XuatKhoDuocPhamChiTietViewModel> XuatKhoDuocPhamChiTiets { get; set; }
        public List<DuocPhamXuatChiTiet> XuatKhoDuocPhamChiTiets { get; set; }
    }

    public class DuocPhamXuatChiTiet : BaseViewModel
    {
        public DuocPhamXuatChiTiet()
        {
            XuatKhoDuocPhamChiTietViTris = new List<XuatKhoDuocPhamChiTietViTriViewModel>();
        }

        public string Id { get; set; }
        public int STT { get; set; }
        public string TenDuocPham { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Loai { get { return LaDuocPhamBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay
        {
            get { return SoLuongTon.ApplyFormatMoneyToDouble(); }
        }
        public double? SoLuongXuat { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }

        public List<XuatKhoDuocPhamChiTietViTriViewModel> XuatKhoDuocPhamChiTietViTris { get; set; }
    }

    public class XuatKhoDuocPhamChiTietViewModel : BaseViewModel
    {
        public XuatKhoDuocPhamChiTietViewModel()
        {
            XuatKhoDuocPhamChiTietViTris = new List<XuatKhoDuocPhamChiTietViTriViewModel>();
        }
        public bool? DatChatLuong { get; set; }
        public long? XuatKhoDuocPhamId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public DateTime? NgayXuat { get; set; }
        public  List<XuatKhoDuocPhamChiTietViTriViewModel> XuatKhoDuocPhamChiTietViTris { get; set; }
        #region Other

        public double? TongSoLuongXuat { get; set; }

        public string TenDuocPham { get; set; }
        public string ChatLuong { get; set; }
        public long? DuocPhamId { get; set; }

        #endregion Other



        #region update 12/3/2020

        public decimal? DonGiaBan { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }

        #endregion update 12/3/2020
    }

    public class XuatKhoDuocPhamChiTietViTriViewModel : BaseViewModel
    {
        public long? XuatKhoDuocPhamChiTietId { get; set; }
        public long? NhapKhoDuocPhamChiTietId { get; set; }
        public double? SoLuongXuat { get; set; }
        public DateTime? NgayXuat { get; set; }

        #region Display

        public string ViTri { get; set; }
        public string SoLo { get; set; }
        public string HanSuDungDisplay { get; set; }

        #endregion Display
    }
}