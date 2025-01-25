using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.CauHinh
{
   
    public interface ICauHinhNguoiDuyetTheoNhomDVService : IMasterFileService<Core.Domain.Entities.CauHinhs.CauHinhNguoiDuyetTheoNhomDichVu>
    {

        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetListNhanViens(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetLisNhomDVXetNghiems(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetLisNhomDVXetNghiemUpdates(DropDownListRequestModel model);
        bool KiemTraNhomDichVuDaSetNhanVienDuyet(long? nhomDichVuBenhVienId ,long id);
        
    }
}
