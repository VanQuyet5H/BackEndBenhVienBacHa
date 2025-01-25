using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService : IYeuCauTiepNhanBaseService
    {
        #region Get lookup

        Task<List<LookupItemTemplateVo>> GetCongTy(DropDownListRequestModel queryInfo);
        //Task<List<LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo>> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens
        //(DropDownListRequestModel queryInfo, bool chonDichVuKham = true, bool chonDichVuKyThuat = true, bool fullNhomDichVu = false, List<long> dichVuKhamIds = null, List<long> dichVuKyThuatIds = null);
        Task<List<LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo>> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(DropDownListRequestModel queryInfo, DichVuKhamBenhBVHoacDVKTBenhVienParams dvKBHoacDVKTBvParam = null);
        Task<List<LookupItemDVKhamBenhKyThuatBvVo>> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienTaoGoiKSKs(DropDownListRequestModel queryInfo, DichVuKhamBenhBVHoacDVKTBenhVienParams dvKBHoacDVKTBvParam = null);
        Task<List<LookupItemTemplateVo>> GetHopDong(DropDownListRequestModel queryInfo);
        Task<List<LookupItemTemplateVo>> GetListBacSiAsync(DropDownListRequestModel queryInfo);
        Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> GetHopDongKhamSucKhoe(DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false);
        Task<List<LookupItemTemplateVo>> GetKhoaPhongGoiKham(DropDownListRequestModel queryInfo, string selectedItems = null);
        Task<List<string>> GetListNhomDoiTuongKhamSucKhoeAsync(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetDiaDiemKhamCongTyTonTai(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetLoaiGiaDichVuKyThuat(DropDownListRequestModel model);
        List<LookupItemVo> GetThongTinGoiKhamTheoHopDong(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetPhongBenhVienTheoTen(DropDownListRequestModel model);
        List<LookupItemVo> GetGoiKhamTheoTen(DropDownListRequestModel model);
        List<LookupItemVo> GetPhanLoaiSucKhoe(DropDownListRequestModel model);
        List<LookupItemVo> GetListDanhSachNhanSuMultiSelect(DropDownListRequestModel model, long hopDongKhamSucKhoeId);
        Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> TimKiemHopDongConHieuLucNhanVien(DropDownListRequestModel queryInfo, string phoneNumberOrEmail);
        Task<List<LookupItemNhomDichVuChiDinhKhamSucKhoeVo>> GetLoaiDichVuTrenHeThongVaGoiChungs(DropDownListRequestModel queryInfo);
        (List<long>, List<long>) GetListDichVuIdTTrongGoiKhamSucKhoe(string lookupInfo);
        List<LookupItemTemplateVo> GetGoiKhamChungs(DropDownListRequestModel queryInfo);
        #endregion

        #region Get data
        Task<KhamDoanThongTinHanhChinhVo> GetThongTinHanhChinhAsync(long yeuCauTiepNhanId);
        Task<HopDongKhamSucKhoe> TimKiemThongTinHopDongKhamTheoCongTyAsync(long hopDongId);
        Task<YeuCauTiepNhan> GetThongTinHanhChinhNhanVienAsync(long hopDongKhamNhanVienId);
        List<HopDongKhamSucKhoeNhanVien> GetHopDongKhamSucKhoeNhanViens(List<long> hopDongKhamNhanVienIds);
        bool CheckHopDongKhamNhanVienDaBatDauKham(List<long> hopDongKhamNhanVienIds);
        Task<long> GetDVKBVaDVKT(DropDownListRequestModel queryInfo);
        bool KiemTraGoiKhamDungDungGoiChung(long goiKhamId);

        Task<decimal> GetDonGiaBenhVien(DichVuKhamBenhGiaBenhVienVo dichVuKhamBenhGiaBenhVienVo);
        Task<string> GetTenNhomGiaDichVuKhamBenhBenhVien(long nhomGiaDichVuKhamBenhBenhVienId, bool laDichVuKham);
        Task<GoiKhamSucKhoe> GetGoiKhamTheoTenVaHopDong(string tenGoiKham, long hopDongKhamSucKhoeId);
        Task<ThongTinGiaDichVuTrongGoiKhamSucKhoeVo> GetThongTinGiaDichVuTrongGoi(DichVuKhamBenhGiaBenhVienVo dichVuKhamBenhGiaBenhVienVo);
        bool KiemTraMaGoiKham(string maGoiKham, long hopDongKhamSucKhoeId);
        #endregion
    }
}
