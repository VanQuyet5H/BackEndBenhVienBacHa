using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KetLuanKhamSucKhoeDoanVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? CongTyKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
         public bool? ChuaKetLuan { get; set; }
        public bool? DaKetLuan { get; set; }
        public string SearchString { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string TenNgheNghiep { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string SoChungMinhThu { get; set; }
        public string TenDanToc { get; set; }
        public string TenTinhThanh { get; set; }
        public string NhomDoiTuongKhamSucKhoe { get; set; }
        public int? DichVuDaThucHien { get; set; }
        public int? TongDichVu { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public EnumTrangThaiYeuCauTiepNhan? TrangThaiYeuCauTiepNhan { get; set; }
        public string KSKKetLuanDisplay => TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat ? "Rồi" : "Chưa";
        public int? TinhTrang => TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat ? 1 : 0;
        public bool? LaHopDongDaKetLuan { get; set; }
        // 
        public string KSKKetLuanCLSDisplay => LaHopDongDaKetLuan == true ? "Rồi" :"Chưa";
        public int? TinhTrangCLS => LaHopDongDaKetLuan == true ? 1 : 0;
        // 
        public long GoiDichVuId { get; set; }
        // BVHD-3722
        public string KetQuaKhamSucKhoeData { get; set; }
        public string HighLightClass => string.IsNullOrEmpty(KetQuaKhamSucKhoeData) ? "bg-row-lightRed" : "";
    }

    public class KetLuanKhamSucKhoeDoanDichVuKhamVo
    {
        public KetLuanKhamSucKhoeDoanDichVuKhamVo()
        {
            TemplateObj = new List<KetLuanKhamSucKhoeDoanDichVuKhamTemplateObjVo>();
        }
        public string Title { get; set; }
        public List<KetLuanKhamSucKhoeDoanDichVuKhamTemplateObjVo> TemplateObj { get; set; }
    }

    public class KetLuanKhamSucKhoeDoanDichVuKhamTemplateObjVo
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string fxFlex { get; set; }
    }
    public class KetLuanKhamSucKhoeDoanDichVuKhamTemplateGroupVo
    {
        public ChuyenKhoaKhamSucKhoe? Type { get; set; }
        public string Title { get; set; }
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
        //public int TypeTemplate => Type != ChuyenKhoaKhamSucKhoe.Mat && Type != ChuyenKhoaKhamSucKhoe.TaiMuiHong ? 3 : 4;
    }

    public class KetLuanKhamSucKhoeDoanChiTietVo : GridItem
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
    }

    public class InSoKSKVaKetQua
    {
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public string HostingName { get; set; }
        public bool IsInSoKSKDinhKy { get; set; }
    }

    public class KetQuaMauVo
    {
        public bool IsCheck { get; set; }
        public string Value { get; set; }

    }
    public class ThongTinCheckHopDong
    {
        public long HopDongId { get; set; }
        public long CongTyId { get; set; }
    }

    public class KetQuaKhamSucKhoeValueOBJ
    {
        public KetQuaKhamSucKhoeValueOBJ()
        {
            ListDichVuKSKCanCapNhatTrangThaiDVKs = new List<InFoCapNhatTrangThaiDVValueOBJ>();
            ListDichVuKSKCanCapNhatTrangThaiDVKTs = new List<InFoCapNhatTrangThaiDVValueOBJ>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public string JsonKetLuan { get; set; }
        public string Json { get; set; }
        public bool? CapNhatTrangThaiDicVu { get; set; }
        public List<InFoCapNhatTrangThaiDVValueOBJ> ListDichVuKSKCanCapNhatTrangThaiDVKs { get; set; }
        public List<InFoCapNhatTrangThaiDVValueOBJ> ListDichVuKSKCanCapNhatTrangThaiDVKTs { get; set; }
    }
    public class InFoCapNhatTrangThaiDVValueOBJ
    {
        public EnumTypeLoaiChuyenKhoaEdit Type { get; set; }
        public long Id { get; set; }
    }
}
