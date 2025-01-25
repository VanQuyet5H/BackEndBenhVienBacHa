using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;

namespace Camino.Services.DichVuKhamBenhBenhViens
{
    public interface IDichVuKhamBenhBenhVienService : IMasterFileService<DichVuKhamBenhBenhVien>
    {
        DichVuKhamBenhBenhVien CheckExistDichVuKhamBenhBenhVien(long id);
        Task<long> GetIdKhoaPhongFromRequestDropDownList(DropDownListRequestModel model);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model);
        Task UpdateLastEntity(long id, DateTime tuNgay, long dichVuKhamBenhId);
        Task<bool> IsTuNgayValid(DateTime? tuNgay, long? id, long? dichVuKhamBenhId);
        Task<DichVuKhamBenhBenhVien> GetAfterEntity(long id, long dichVuKhamBenhId);
        Task<GridDataSource> GetTotalPageForGridChildGiaBenhVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildGiaBenhVienAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetNhomDichVu();
        Task<bool> IsTuNgayBaoHiemValid(DateTime? tuNgay, long? id);
        Task<bool> IsTuNgayBenhVienValid(DateTime? tuNgay, long? id, long? nhom);
        Task AddAndUpdateLastEntity(DateTime tuNgayBaoHiem, ICollection<DichVuKhamBenhBenhVienGiaBenhVien> giaBenhVienEntity);
        Task UpdateDeleteEntity(long id, int loai, long nhom = 0);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKhamBenhById(DropDownListRequestModel model, long id, long? khoaPhongId);
        Task UpdateDayGiaBenhVienEntity(ICollection<DichVuKhamBenhBenhVienGiaBenhVien> giaBenhVienEntity);
        Task<List<NhomGiaDichVuKhamBenhBenhVien>> GetNhomGiaDichVuKhamBenhBenhVien();
        Task<List<DichVuKyThuatTemplateVo>> GetKhoaPhong(DropDownListRequestModel model);
        Task<List<NoiThucHienDichVuBenhVienVo>> GetPhongBenhVienDichVuKhamBenh(DropDownListRequestModel model);
        Task<bool> IsExistsMaDichVuKhamBenhBenhVien(long dichVuKhamBenhBenhVienId, string ma);
        Task<bool> KiemTraNgay(DateTime? tuNgay, DateTime? denNgay);

        Task<bool> kiemTraCoBaoNhieuLoaiGiaBenhVien(DateTime? tuNgay, DateTime? denNgay,bool? validate);

        Task<List<LookupItemVo>> GetNhomDichVuTheoDichVuKhamBenh(DropDownListRequestModel queryInfo);
        #region BVHD-3937
        Task<GiaDichVuBenhVieDataImportVo> XuLyKiemTraDataGiaDichVuBenhVienImportAsync(GiaDichVuBenhVienFileImportVo info);
        Task KiemTraDataGiaDichVuBenhVienImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas, GiaDichVuBenhVieDataImportVo result);
        Task XuLyLuuGiaDichVuImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas);
        #endregion
    }
}
