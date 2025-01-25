using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class TimKiemController : CaminoBaseController
    {
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        public TimKiemController(IYeuCauTiepNhanService yeuCauTiepNhanService)
        {
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
        }
        [HttpPost("TimKiemTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult<GridDataSource> TimKiemTiepNhan([FromBody] TimKiemQueryInfo queryInfo)
        {        
            var gridData = _yeuCauTiepNhanService.TimKiemTiepNhan(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("TotalTimKiemTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult<GridDataSource> TotalTimKiemTiepNhan([FromBody] TimKiemQueryInfo queryInfo)
        {
            var gridData = _yeuCauTiepNhanService.TimKiemTiepNhan(queryInfo);
            return Ok(gridData);
        }
    }
}
