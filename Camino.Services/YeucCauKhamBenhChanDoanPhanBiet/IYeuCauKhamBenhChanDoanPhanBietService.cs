using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauChanDoanPhanBiet;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeucCauKhamBenhChanDoanPhanBiet
{
    public interface IYeuCauKhamBenhChanDoanPhanBietService : IMasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet>
    {
        Task<List<ChucDanhItemVo>> GetListICD(DropDownListRequestModel model);
        Task<ActionResult<GridDataSource>> GetDataGridChanDoanPhanBiet(long idYCKB);
        Task<List<YeuCauKhamBenhChanDoanPhanBiet>> GetChanDoanPhanBiet(long id);
        Task<bool> IsTenExists(long? idICD, long id, long yeuCauKhamBenhId);
    }
}
