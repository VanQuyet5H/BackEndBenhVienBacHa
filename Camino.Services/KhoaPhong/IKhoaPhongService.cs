using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Services.KhoaPhong
{
    public interface IKhoaPhongService
        : IMasterFileService<Core.Domain.Entities.KhoaPhongs.KhoaPhong>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenExists(string ten = null, long id = 0);

        Task<bool> IsMaExists(string ma = null, long id = 0);

        Task<List<KhoaKhamTemplateVo>> GetListKhoaPhong(DropDownListRequestModel queryInfo);

        Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongAll(DropDownListRequestModel queryInfo);
        Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongThuocNoiTruAll(DropDownListRequestModel queryInfo);

        List<LookupItemVo> GetListLoaiKhoaPhong(LookupQueryInfo queryInfo);

        List<LookupItemVo> GetListKieuKham(LookupQueryInfo queryInfo);

        Task<List<KhoaKhamTemplateVo>> GetListKhoa(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetListKhoaPhongThuNgan(DropDownListRequestModel model);
    }
}
