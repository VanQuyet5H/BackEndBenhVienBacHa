using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetDataBaoCaoKeHoachTongHopForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoNguoiBenhDenKham)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoKeHoachTongHopForGridAsync(BaoCaoKeHoachTongHopQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoKeHoachTongHopForGridAsync(queryInfo);
            return Ok(grid);
        }
    }
}
