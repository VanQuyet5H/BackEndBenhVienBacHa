using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamDoan
{
    public class KetLuanKhamSucKhoeDoanViewModel : BaseViewModel
    {
        public KetLuanKhamSucKhoeDoanViewModel()
        {
            KetLuanKhamSucKhoeDoanDichVuKhamTemplates = new List<KetLuanKhamSucKhoeDoanDichVuKhamTemplateGroupVo>();
            DichVuKhamChuaKhams = new List<string>();
            DichVuKyThuatChuaThucHiens = new List<string>();
        }
        public long HopDongKhamSucKhoeId { get; set; }
        public long CongTyKhamSucKhoeId { get; set; }
        public string KSKKetQuaCanLamSang { get; set; }
        public string KSKDanhGiaCanLamSang { get; set; }
        public PhanLoaiSucKhoe? PhanLoaiSucKhoeId { get; set; }
        public EnumTrangThaiYeuCauTiepNhan? TrangThaiYeuCauTiepNhan { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public string KSKKetLuanGhiChu { get; set; }
        public string KSKKetLuanCacBenhTat { get; set; }
        public bool IsEnableKetLuan { get; set; }
        public bool CoHienThiPhanLoai { get; set; }
        //public bool IsHiddenKetLuan { get; set; }
        public bool IsOnlySave { get; set; }
        public bool? LaGetKetQuaMau { get; set; }
        public long? KSKNhanVienKetLuanId { get; set; }
        public DateTime? KSKThoiDiemKetLuan { get; set; }
        public bool CoDichVuChuaKham { get; set; }
        public List<string> DichVuKhamChuaKhams { get; set; }
        public List<string> DichVuKyThuatChuaThucHiens { get; set; }

        public List<KetLuanKhamSucKhoeDoanDichVuKhamTemplateGroupVo> KetLuanKhamSucKhoeDoanDichVuKhamTemplates { get; set; }
    }
    public class KetQuaKhamSucKhoe
    {
        public long YeuCauTiepNhanId { get; set; }
        public long HopDongKhamSucKhoeId { get; set; }
        public string JsonKetQuaKSK { get; set; }
        public string JsonKetLuan { get; set; }
    }
    public class InFoCapNhatTrangThaiDV
    {
        public EnumTypeLoaiChuyenKhoaEdit Type { get; set; }
        public long Id { get; set; }
    }
    public class LuuVaCapNhatTrangThaiViewModel : BaseViewModel
    {
        public LuuVaCapNhatTrangThaiViewModel()
        {
            ListDichVuKSKCanCapNhatTrangThaiDVKs = new List<InFoCapNhatTrangThaiDV>();
            ListDichVuKSKCanCapNhatTrangThaiDVKTs = new List<InFoCapNhatTrangThaiDV>();
        }
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long HopDongKhamSucKhoeId { get; set; }
        public string JsonKetQuaKSK { get; set; }
        public string JsonKetLuan { get; set; }

        public List<InFoCapNhatTrangThaiDV> ListDichVuKSKCanCapNhatTrangThaiDVKs { get; set; }
        public List<InFoCapNhatTrangThaiDV> ListDichVuKSKCanCapNhatTrangThaiDVKTs { get; set; }

    }
}
