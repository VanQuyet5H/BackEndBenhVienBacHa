using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.KhamBenhs
{
    public interface IBenhNhanDiUngThuocService : IMasterFileService<BenhNhanDiUngThuoc>
    {
        Task<ActionResult<GridDataSource>> GetDataGridBenhNhanDiUngThuoc(QueryInfo queryInfo);

        Task<bool> IsThuocExists(Enums.LoaiDiUng loaiDiUng, long thuocId, long benhNhanId, long id);
        List<LookupItemVo> GetListMucDoDiUng();
        Task<ActionResult<GridDataSource>> GetDataGridChanDoanPhanBiet(long idYCKB);
        Task<ActionResult<GridDataSource>> GetDataGridChanDoanKemTheo(long idYCKB);
    }
}