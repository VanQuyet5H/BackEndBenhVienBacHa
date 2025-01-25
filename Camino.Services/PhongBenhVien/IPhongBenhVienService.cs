using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.PhongBenhVien
{
    public interface IPhongBenhVienService
        : IMasterFileService<Core.Domain.Entities.PhongBenhViens.PhongBenhVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenExists(string ten = null, long id = 0);

        Task<bool> IsMaExists(string ma = null, long id = 0);

        Task<List<LookupItemVo>> GetNamePhongBenhVienCreate(long id);

        Task<List<LookupItemVo>> GetNamePhongBenhVienDetail(long Id, List<long> phongNgoaiTruIds);

        Task<List<long>> GetListPhongBenhVien(long id);

        Task<List<Core.Domain.Entities.PhongBenhViens.PhongBenhVien>> GetListPhongBenhVienByKhoaPhongId(long id, DropDownListRequestModel model = null);
        Task<List<LookupItemVo>> GetListPhongNgoaiVienByHopDongKhamSreach(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetListPhongBenhVienByCurrentUser();
        Task<List<LookupItemTemplateVo>> GetListPhongBenhVienByKhoa(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListPhongBenhVienByKhoaSreach(DropDownListRequestModel model, long khoaPhongId);
        Task<List<LookupItemVo>> GetPhongBenhViensByKhoaPhongId(DropDownListRequestModel model, long khoaPhongId);
        Task<LookupItemVo> GetKhoaByPhong(long phongId);
        Task<LookupItemVo> GetTenKhoaByPhong(long phongId);
        Task<List<long>> GetPhongByListKhoa(List<long> khoaIds);
        LookupItemVo GetKhoaPhongNgoaiVien();

        Task<List<LookupItemTemplateVo>> GetListPhongTatCa(DropDownListRequestModel model);
    }
}
