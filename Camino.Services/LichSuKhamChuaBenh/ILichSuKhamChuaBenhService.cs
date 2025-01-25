using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LichSuKhamChuaBenhs;

namespace Camino.Services.LichSuKhamChuaBenh
{
    public interface ILichSuKhamChuaBenhService : IMasterFileService<YeuCauTiepNhan>
    {
        #region grid
        Task<GridDataSource> GetDataForGridTimKiemNguoiBenhDaTiepNhanAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridTimKiemNguoiBenhDaTiepNhanAsync(QueryInfo queryInfo);
        #endregion

        #region get data
        Task<LichSuKhamChuaBenhTheoNguoiBenhVo> GetLichSuKhamChuaBenhTheoNguoiBenhAsnc(long nguoiBenhId);
        Task<List<YeuCauTiepNhan>> GetLichSuKhamYeuCauTiepNhanAsnc(long? nguoiBenhId = null, long? yeuCauTiepNhanId = null);
        #endregion
    }
}
