using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan
{
    public class BaoCaoTongHopKetQuaKhamDoanGridVo :GridItem
    {
        public BaoCaoTongHopKetQuaKhamDoanGridVo()
        {
            ListDichVuIds = new List<long>();
            GoiKhamSucKhoeDichVuDichVuKyThuats = new  List<GoiKhamSucKhoeDichVuDichVuKyThuat>(); 
        }
        public string TenDichVu { get; set; }
        public long  IdDichVuTrongGoi { get; set; }
        public string  KetQua { get; set; }
        public Enums.EnumNhomGoiDichVu NhomId { get; set; }
        public List<long> IdDichVuTrongGois { get; set; }
        public string MaDichVu { get; set; }
        public List<long> ListDichVuIds { get; set; }
        public List<GoiKhamSucKhoeDichVuDichVuKyThuat> GoiKhamSucKhoeDichVuDichVuKyThuats { get; set; }
    }
    public class BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo : GridItem
    {
        public int? STT { get; set; }
        public string HoTen { get; set; }
        public DateTime? ThoiGianChuyenKhoaDauTien { get; set; }
        public string ThoiGianChuyenKhoaDauTienFormat => ThoiGianChuyenKhoaDauTien != null ? ((DateTime)ThoiGianChuyenKhoaDauTien).ApplyFormatDateTime() : "";
        public int? NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string ChieuCao { get; set; }
        public string CanNang { get; set; }
        public string HuyetAp { get; set; }
        public List<BaoCaoTongHopKetQuaKhamDoanGridVo> BaoCaoTongHopKetQuaKhamDoanGridVos { get; set; }
        public string PhanLoai { get; set; }
        public string KetLuan { get; set; }
        public string DeNghi { get; set; }
        public string KetQuaKhamSucKhoeData { get; set; }
        public string KSKKetLuanData { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public string KSKKetLuanCacBenhTat { get; set; }
        public string KSKKetLuanGhiChu { get; set; }

        public List<ChiSoNguoibenh> ChiSos { get; set; }
        public List<ThongTinThoiGianKhamBenh> ThongTinThoiGianKhamBenhs { get; set; }
        // BVHD-3676
        public string MaNB { get; set; }
        public string MaTN { get; set; }

    }
    public class ThongTinThoiGianKhamBenh
    {
        public long Id { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }

    }
    public class ChiSoNguoibenh : GridItem
    {
        public double? ChieuCao { get; set; }
        public double? CanNang { get; set; }
        public int? HuyetApTamTruong { get; set; }
        public int? HuyetApTamThu { get; set; }

    }
    public class BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo : QueryInfo
    {
        public long? HopDongId { get; set; }
        public long? CongTyId { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }
    }
    public class ModelVoNhanVien {
        public long HopDongId { get; set; }
        public long CongTyId { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }
    }
    public class TongHopKetQuaKhamDoanGridVo : GridItem
    {
        public TongHopKetQuaKhamDoanGridVo()
        {
            GoiKhamSucKhoeDichVuDichVuKyThuats = new List<GoiKhamSucKhoeDichVuDichVuKyThuat>();
            YeuCauDichVuKyThuaIds = new List<InFoYeuCauDichVuKyThuat>();
            YeuCauDichKhamIds = new List<InFoYeuCauKham>();
            GoiKhamSucKhoeDichVuKhamBenhs = new List<GoiKhamSucKhoeDichVuKhamBenh>();
        }
       
        public List<InFoYeuCauDichVuKyThuat> YeuCauDichVuKyThuaIds { get; set; }
        public List<InFoYeuCauKham> YeuCauDichKhamIds { get; set; } 
        public List<GoiKhamSucKhoeDichVuDichVuKyThuat> GoiKhamSucKhoeDichVuDichVuKyThuats { get; set; }
        public List<GoiKhamSucKhoeDichVuKhamBenh> GoiKhamSucKhoeDichVuKhamBenhs { get; set; }
    }
    public class InFoYeuCauDichVuKyThuat
    {
         public long Id  { get; set; }
         public long DichVuKyThuatBenhVienId { get; set; }
    }
    public class InFoYeuCauKham
    {
        public long Id { get; set; }
        public long DichVuKhamBenhVienId { get; set; }
    }

    public class InFoDichVuKhamBenh
    {
        public long Id { get; set; }
        public string Ten { get; set; }
        public string MaDichVu { get; set; }
    }
    public class InFoDichVuKyThuat
    {
        public long Id { get; set; }
        public string Ten { get; set; }
        public string MaDichVu { get; set; }
    }
}
