using Camino.Core.Domain;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.XuatKhos
{
    public class XuatKhoVatTuViewModel : BaseViewModel
    {
        public XuatKhoVatTuViewModel()
        {
            XuatKhoVatTuChiTiets = new List<VatTuXuatChiTiet>();
        }
        public long? KhoXuatId { get; set; }
        public string KhoXuatDisplay { get; set; }
        public long? KhoNhapId { get; set; }
        public string KhoNhapDisplay { get; set; }
        public string SoPhieu { get; set; }
        public Enums.EnumLoaiXuatKho? LoaiXuatKho { get; set; }
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

        public List<VatTuXuatChiTiet> XuatKhoVatTuChiTiets { get; set; }
    }

    public class VatTuXuatChiTiet : BaseViewModel
    {
        public VatTuXuatChiTiet()
        {
            XuatKhoVatTuChiTietViTris = new List<XuatKhoVatTuChiTietViTriViewModel>();
        }

        public string Id { get; set; }
        public int STT { get; set; }
        public string TenVatTu { get; set; }
        public string DVT { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Loai { get { return LaVatTuBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay
        {
            get { return SoLuongTon.ApplyFormatMoneyToDouble(); }
        }
        public double? SoLuongXuat { get; set; }

        //public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }

        public List<XuatKhoVatTuChiTietViTriViewModel> XuatKhoVatTuChiTietViTris { get; set; }
    }

    public class XuatKhoVatTuChiTietViTriViewModel : BaseViewModel
    {
        public long? XuatKhoVatTuChiTietId { get; set; }
        public long? NhapKhoVatTuChiTietId { get; set; }
        public double? SoLuongXuat { get; set; }
        public DateTime? NgayXuat { get; set; }

        #region Display

        public string ViTri { get; set; }
        public string SoLo { get; set; }
        public string HanSuDungDisplay { get; set; }

        #endregion Display
    }



    public class XuatKhoVatTuChiTietViewModel : BaseViewModel
    {
        public XuatKhoVatTuChiTietViewModel()
        {
            XuatKhoVatTuChiTietViTris = new List<XuatKhoVatTuChiTietViTriViewModel>();
        }
        public bool? DatChatLuong { get; set; }
        public long? XuatKhoVatTuId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public DateTime? NgayXuat { get; set; }
        public List<XuatKhoVatTuChiTietViTriViewModel> XuatKhoVatTuChiTietViTris { get; set; }
        #region Other

        public double? TongSoLuongXuat { get; set; }

        public string TenVatTu { get; set; }
        public string ChatLuong { get; set; }
        public long? VatTuId { get; set; }

        #endregion Other



        #region update 12/3/2020

        public decimal? DonGiaBan { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }

        #endregion update 12/3/2020
    }
}
