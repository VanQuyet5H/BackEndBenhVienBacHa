using System;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.KhamBenhs
{
    public interface IBenhNhanTienSuBenhService : IMasterFileService<BenhNhanTienSuBenh>
    {
        Task<ActionResult<GridDataSource>> GetDataGridTienSuBenh(QueryInfo queryInfo);

        Task<bool> CheckValidDate(DateTime? ngayPhatHien);
    }
}