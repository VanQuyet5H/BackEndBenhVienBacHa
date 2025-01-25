using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Services.DichVuKyThuatBenhVien
{
    public interface IDichVuKyThuatBenhVienService : IMasterFileService<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien>
    {
        Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien CheckExistDichVuKyThuatBenhVien(long id);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuatBenhVien(DropDownListRequestModel model);
        Task<List<DuocPhamVacxinTemplateVo>> GetListDuocPhamBenhVien(DropDownListRequestModel model);
        Task<List<KhoaKhamTemplateVo>> GetKhoaKham(DropDownListRequestModel model);
        Task<List<NoiThucHienDichVuBenhVienVo>> GetPhongKhamTheoDichVuKyThuatBenhVien(DropDownListRequestModel model);

        Task<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> GetAfterEntity(long id);
        Task UpdateLastEntity(long id, DateTime tuNgay, long? dichVuKyThuatId);
        Task<bool> IsTuNgayValid(DateTime? tuNgay, long? id, long? dichVuKyThuatId);
        Task<GridDataSource> GetDataForGridChildBenhVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildBenhVienAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetNhomDichVu();
        Task<List<NhomGiaDichVuKyThuatBenhVien>> GetNhomGiaDichVuKyThuatBenhVien();
        Task AddAndUpdateLastEntity(DateTime tuNgayBaoHiem, ICollection<DichVuKyThuatBenhVienGiaBenhVien> giaBenhVienEntity);
        Task UpdateDayGiaBenhVienEntity(ICollection<DichVuKyThuatBenhVienGiaBenhVien> giaBenhVienEntity);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuatById(DropDownListRequestModel model, long id);
        Task<bool> IsExistsMaDichVuKyThuatBenhVien(long dichVuKyThuatBenhVienId, string ma);
        Task XuLyCapNhatNoiThucHienUuTienKhiCapNhatDichVuKyThuatAsync(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien dichVuKyThuatBenhVien);

        Task<List<NoiThucHienUuTienDichVuBenhVienVo>> GetListNoiThucHienUuTienTheoNoiThucHienDangChonAsync(DropDownListRequestModel model);
        Task<List<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien>> NhomDichVuBenhViens();

        Task XuLyChuyenNhomXetNghiem(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien dichVuKyThuatBenhVien, List<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhViens, long nhomDichVuBenhVienId, string ma, string ten);

        List<LookupItemVo> GetDanhSachPhanLoaiPTTT(DropDownListRequestModel queryInfo);
        Task<bool> KiemTraNgay(DateTime? tuNgay, DateTime? denNgay);
        Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucAsync(DropDownListRequestModel queryInfo);
        Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucDVKTAsync(DropDownListRequestModel queryInfo, long? DuocPhamVTYTId);
        Task<List<ThongTinDuocPhamQuayThuocVo>> GetThongTinDuocPham(long duocPhamId, long loaiDuocPhamHoacVatTu);
        Task<bool> KiemTraSLTonDuocPhamVatTu(double slTon, long? duocPhamId, long? vatTuId);
        Task<bool> KiemTraIsNull(int ? LaDuocPham, long? duocPhamId, long? vatTuId);
        Task<List<LookupItemVo>> GetListChuyenKhoaChuyenNganh(DropDownListRequestModel model);

        byte[] ExportDichVuKyThuatBenhVien(GridDataSource gridDataSource);
        Task<List<LookupItemVo>> GetNhomDichVuTheoDichVuKyThuat(DropDownListRequestModel queryInfo);

        #region BVHD-3937
        Task<GiaDichVuBenhVieDataImportVo> XuLyKiemTraDataGiaDichVuBenhVienImportAsync(GiaDichVuBenhVienFileImportVo info);
        Task KiemTraDataGiaDichVuBenhVienImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas, GiaDichVuBenhVieDataImportVo result);
        Task XuLyLuuGiaDichVuImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas);
        #endregion

        #region BVHD-3961
        Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongAll(DropDownListRequestModel queryInfo);
        #endregion
    }
}