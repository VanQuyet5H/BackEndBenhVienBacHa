using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamDoan
{
    public class YeuCauTiepNhanKhamSucKhoeViewModel : BaseViewModel
    {
        public ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel HopDongKhamSucKhoeNhanVien { get; set; }
        public Enums.EnumTrangThaiYeuCauTiepNhan? TrangThaiYeuCauTiepNhan { get; set; }
        public bool IsBatDauKhamTuDanhSach { get; set; }
        public long? HopDongKhamSucKhoeNhanVienDanhSachId { get; set; }
    }

    public class HopDongKhamNhanVienBatDauKhamIdViewModel
    {
        public HopDongKhamNhanVienBatDauKhamIdViewModel()
        {
            HopDongKhamSucKhoeNhanVienIds = new List<long>();
        }
        public List<long> HopDongKhamSucKhoeNhanVienIds { get; set; }
    }

    public class HopDongKhamNhanVienBatDauKhamListViewModel
    {
        public HopDongKhamNhanVienBatDauKhamListViewModel()
        {
            HopDongKhamSucKhoeNhanVienDetails = new List<HopDongKhamNhanVienBatDauKhamDetailViewModel>();
        }
        public List<HopDongKhamNhanVienBatDauKhamDetailViewModel> HopDongKhamSucKhoeNhanVienDetails { get; set; }
    }

    public class HopDongKhamNhanVienBatDauKhamDetailViewModel
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public YeuCauTiepNhanKhamSucKhoeViewModel HopDongKhamNhanVienModel { get; set; }
        public Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan HopDongKhamNhanVienEntity { get; set; }
    }
}
