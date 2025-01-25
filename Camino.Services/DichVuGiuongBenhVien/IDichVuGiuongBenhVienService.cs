using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Services.DichVuGiuongBenhVien
{
    public interface IDichVuGiuongBenhVienService 
        : IMasterFileService<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien>
    {
        Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien CheckExistDichVuGiuongBenhVien(long id);
        Task<long> GetIdKhoaPhongFromRequestDropDownList(DropDownListRequestModel model);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuGiuongId = null, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<List<DichVuGiuongTemplateVo>> GetListDichVuGiuong(DropDownListRequestModel model);

        Task<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> GetAfterEntity(long id);

        Task UpdateLastEntity(long id, DateTime tuNgay, long? dichVuGiuongId);

        Task<bool> IsTuNgayValid(DateTime? tuNgay, long? id, long? dichVuGiuongId);
        Task<GridDataSource> GetTotalPageForGridChildBenhVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildBenhVienAsync(QueryInfo queryInfo, long? dichVuGiuongId = null, bool forExportExcel = false);
        Task<List<NhomGiaDichVuGiuongBenhVien>> GetNhomGiaDichVuKyThuatBenhVien();
        Task<List<LookupItemVo>> GetNhomDichVu();
        Task<List<DichVuGiuongTemplateVo>> GetDichVuGiuongById(DropDownListRequestModel model, long id);
        Task<List<DichVuGiuongTemplateVo>> GetDichVuGiuong(DropDownListRequestModel model);
        Task<List<NhomGiaDichVuGiuongBenhVien>> NhomGiaDichVuGiuongBenhVien();
        void UpdateDayGiaBenhVienEntity(ICollection<DichVuGiuongBenhVienGiaBenhVien> giaBenhVienEntity);
        Task<List<KhoaKhamTemplateVo>> GetKhoaKhamTheoDichVuGiuongBenhVien(DropDownListRequestModel model);
        Task<List<NoiThucHienDichVuBenhVienVo>> GetPhongKhamTheoDichVuGiuongBenhVien(DropDownListRequestModel model);
        Task<bool> IsExistsMaDichVuGiuongBenhVien(long dichVuGiuongBenhVienId, string ma);
        List<LookupItemVo> GetListLoaiGiuongAsync(DropDownListRequestModel model);

        byte[] ExportDichVuGiuongBenhVien(GridDataSource gridDataSource);

        #region BVHD-3937
        Task<GiaDichVuBenhVieDataImportVo> XuLyKiemTraDataGiaDichVuBenhVienImportAsync(GiaDichVuBenhVienFileImportVo info);
        Task KiemTraDataGiaDichVuBenhVienImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas, GiaDichVuBenhVieDataImportVo result);
        Task XuLyLuuGiaDichVuImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas);
        Task<List<LookupItemVo>> GetNhomDichVuTheoDichVuKyThuat(DropDownListRequestModel queryInfo);
        #endregion
    }
}