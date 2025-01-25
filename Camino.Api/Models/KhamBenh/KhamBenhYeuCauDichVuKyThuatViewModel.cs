using Camino.Api.Models.ChiSoXetNghiems;
using Camino.Api.Models.ChuanDoanHinhAnhViewModelCategory;
using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauDichVuKyThuatViewModel : BaseViewModel
    {
        public KhamBenhYeuCauDichVuKyThuatViewModel()
        {
            KetQuaChuanDoanHinhAnhs = new List<KetQuaChuanDoanHinhAnhViewModel>();
            KetQuaXetNghiems = new List<KetQuaXetNghiemViewModel>();
        }

        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string MaGiaDichVu { get; set; }
        public string Ma4350DichVu { get; set; }
        public string TenDichVu { get; set; }
        public string TenTiengAnhDichVu { get; set; }
        public string TenGiaDichVu { get; set; }
        //public int? NhomChiPhi { get; set; }
        public EnumDanhMucNhomTheoChiPhi? NhomChiPhi { get; set; }

        public LoaiPhauThuatThuThuat? LoaiPhauThuatThuThuat { get; set; }
        public decimal? Gia { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public long? GoiDichVuId { get; set; }
        public int? TiLeUuDai { get; set; }
        public int? TiLeChietKhau { get; set; }
        public int? SoLan { get; set; }
        public bool? DuocHuongBaoHiem { get;set;}
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string NoiBanHanh { get; set; }
        public string MoTa { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public string TenTrangThai { get; set; }

        public bool IsDone
        {
            get { return TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien; }
        }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public bool? DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public long? NoiThucHienId { get; set; }
        public string MaGiuong { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string KetLuan { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public string DeNghi { get; set; }
        public TrangThaiThanhToan? TrangThaiThanhToan { get; set; }
        public long? NhomDichVuId { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public string TenNhomDichVu { get; set; }
        public bool? DieuTriNgoaiTru { get; set; }
        public DateTime? ThoiDiemBatDauDieuTri { get; set; }
        public byte[] LastModifiedYeuCauKhamBenh { get; set; }
        public List<KetQuaChuanDoanHinhAnhViewModel> KetQuaChuanDoanHinhAnhs { get; set; }
        public List<KetQuaXetNghiemViewModel> KetQuaXetNghiems { get; set; }
    }

    public class NoiChiDinhUpdate
    {
        public long YeuCauId { get; set; }
        public long? KhoaPhongId { get; set; }
        public long? BacSiId { get; set; }
    }
}
