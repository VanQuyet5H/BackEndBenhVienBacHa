using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using Camino.Services.BangKeThuocVaVatTuPhauThuat;
using Camino.Api.Auth;
using Camino.Core.Domain;

namespace Camino.Api.Controllers
{

    public class BaoCaoBangKeThuocVaVatTuPhauThuatController : CaminoBaseController
    {
        private readonly IBangKeThuocVaVatTuPhauThuatService _bangKeThuocVaVatTuPhauThuatService;
        public BaoCaoBangKeThuocVaVatTuPhauThuatController(IBangKeThuocVaVatTuPhauThuatService bangKeThuocVaVatTuPhauThuatService)
        {
            _bangKeThuocVaVatTuPhauThuatService = bangKeThuocVaVatTuPhauThuatService;
        }

        [HttpPost("GetPhongPhauThuats")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetPhongPhauThuats(DropDownListRequestModel model)
        {
            var phongPhauThuats = await _bangKeThuocVaVatTuPhauThuatService.GetPhongPhauThuats();
            return Ok(phongPhauThuats);
        }

        [HttpPost("GetBenhNhanPhongPhauThuats")]
        public async Task<ActionResult<ICollection<ThongTinBenhNhanLookupItemVo>>> GetBenhNhanPhongPhauThuats(ThongTinBenhNhanPhauThuatQueryInfo queryInfo)
        {
            var benhNhanPhongPhauThuats = await _bangKeThuocVaVatTuPhauThuatService.GetBenhNhanPhongPhauThuats(queryInfo);
            return Ok(benhNhanPhongPhauThuats);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BangKeThuocVatTuPhauThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]BaoCaoThuocVaVatTuPhauThuatQueryInfoVo queryInfo)
        {
            var gridData = await _bangKeThuocVaVatTuPhauThuatService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBangKeThuocVatTuPT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BangKeThuocVatTuPhauThuat)]
        public async Task<ActionResult> ExportBangKeThuocVatTuPT([FromBody] BaoCaoThuocVaVatTuPhauThuatQueryInfoVo queryInfo)
        {
            var gridData = await _bangKeThuocVaVatTuPhauThuatService.GetDataForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _bangKeThuocVaVatTuPhauThuatService.ExportBangKeThuocVatTuPT(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeThuocVatTuPT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}