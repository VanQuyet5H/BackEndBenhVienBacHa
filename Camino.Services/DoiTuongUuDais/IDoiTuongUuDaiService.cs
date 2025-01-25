using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DoiTuongUuDais
{
    public interface IDoiTuongUuDaiService : IMasterFileService<DoiTuongUuDai>
    {
        Task<bool> CheckDichVuKyThuatActive(long id);
        Task<bool> CheckDichVuKhamBenhActive(long id);
        Task<bool> CheckDichVuKhamBenhExit(long doiTuongUuDaiId, long dichVuTaiBenhVienId);
        Task<bool> CheckDichVuKyThuatExit(long doiTuongUuDaiId, long dichVuKyThuatBenhVienId);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo, long parrentId = 0);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long parrentId = 0);
        Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetDoiTuong();
        Task AddDoiTuongEntity(DoiTuongUuDaiDichVuKyThuatBenhVien entity);
        Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKyThuatById(DropDownListRequestModel model, long id);
        Task DeleteDoiTuongDichVuKyThuat(long id);
        Task<List<DoiTuongUuDaiDichVuKyThuatBenhVien>> GetData(long id);
        Task<string> GetNameDichVuKyThuat(long id);
        Task AddDoiTuongEntity(long idCu, long idMoi, int TiLeUuDai, long doiTuong, long doiTuongOld);
        Task DeleteToAdd(long id);
        Task<GridDataSource> GetDataForGridBenhVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridBenhVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridBenhVienChildAsync(QueryInfo queryInfo, long parentId = 0);
        Task<GridDataSource> GetDataForGridBenhVienChildAsync(QueryInfo queryInfo, long parentId = 0);
        Task DeleteDoiTuongDichVuKhamBenh(long id);
        Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model);
        Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKhamBenhById(DropDownListRequestModel model, long id);
        Task AddDoiTuongUuDaiKhamBenhEntity(DoiTuongUuDaiDichVuKhamBenhBenhVien entity);
        Task<List<DoiTuongUuDaiDichVuKhamBenhBenhVien>> GetDataDoiTuongUuDaiKhamBenh(long id);
        Task DeleteToAddDoiTuongUuDaiKhamBenh(long id);
        Task<string> GetNameDichVuKhamBenh(long id);
        Task<bool> CheckDichVuKhamBenhExist(long id);
        Task<bool> CheckDichVuKyThuatExist(long id);
    }
}
