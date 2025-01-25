using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject.ExcelChungTu;

namespace Camino.Api.Controllers
{
    public partial class BHYTController : CaminoBaseController
    {
        [HttpPost("GetMauBaoBaoBHYT")]
        public Task<List<LookupItemVo>> GetThongTinMauBaoCao()
        {
            var listEnum = EnumHelper.GetListEnum<MauBaoBaoBHYT>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }

        [HttpPost("GetHinhThucXuatFile")]
        public Task<List<LookupItemVo>> GetHinhThucXuatFile()
        {
            var listEnum = EnumHelper.GetListEnum<Camino.Core.Domain.ValueObject.ExcelChungTu.HinhThucXuatFile>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }

        [HttpPost("GetDanhSachDayFile7980ForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DayLenCongGiamDinh7980a)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDayFile7980ForGrid(ExcelFile7980AQueryInfo queryInfo)
        {
            var grid = await _goiBaoHiemYTeService.GetDataDanhSach79AForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportDanhSachDayFile7980")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DayLenCongGiamDinh7980a)]
        public ActionResult ExportDanhSachDayFile7980([FromBody] ExcelFile7980AQueryInfo queryInfo)
        {
            byte[] bytes = _goiBaoHiemYTeService.ExportDanhSachDayFile7980(queryInfo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachBHYTFileExcel7980" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}
