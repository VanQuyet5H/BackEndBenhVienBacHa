using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.PhuongPhapVoCams;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhuongPhapVoCams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhuongPhapVoCams;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.PhuongPhapVoCam;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class PhuongPhapVoCamController : CaminoBaseController
    {
        private readonly IPhuongPhapVoCamService _phuongPhapVoCamService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public PhuongPhapVoCamController(IPhuongPhapVoCamService phuongPhapVoCamService, ILocalizationService localizationService, IExcelService excelService)
        {
            _phuongPhapVoCamService = phuongPhapVoCamService;
            _excelService = excelService;
            _localizationService = localizationService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phuongPhapVoCamService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phuongPhapVoCamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        public async Task<ActionResult<PhuongPhapVoCamViewModel>> Post
            ([FromBody]PhuongPhapVoCamViewModel phuongPhapVoCamViewModel)
        {
            var phuongPhapVoCam = phuongPhapVoCamViewModel.ToEntity<PhuongPhapVoCam>();
            await _phuongPhapVoCamService.AddAsync(phuongPhapVoCam);
            var phuongPhapVoCamItem = await _phuongPhapVoCamService.GetByIdAsync(phuongPhapVoCam.Id);
            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = phuongPhapVoCam.Id },
                phuongPhapVoCamItem.ToModel<PhuongPhapVoCamViewModel>()
            );
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PhuongPhapVoCamViewModel>> Get(long id)
        {
            var phuongPhapVoCam = await _phuongPhapVoCamService.GetByIdAsync(id);
            if (phuongPhapVoCam == null)
            {
                return NotFound();
            }

            return Ok(phuongPhapVoCam.ToModel<PhuongPhapVoCamViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatPhuongPhapVoCam([FromBody]PhuongPhapVoCamViewModel phuongPhapVoCamViewModel)
        {
            var phuongPhapVoCam = await _phuongPhapVoCamService.GetByIdAsync(phuongPhapVoCamViewModel.Id);
            phuongPhapVoCamViewModel.ToEntity(phuongPhapVoCam);
            await _phuongPhapVoCamService.UpdateAsync(phuongPhapVoCam);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        public async Task<ActionResult> Delete(long id)
        {
            var phuongPhapVoCam = await _phuongPhapVoCamService.GetByIdAsync(id);
            if (phuongPhapVoCam == null)
            {
                return NotFound();
            }

            await _phuongPhapVoCamService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var isDeletePhuongPhapVoCamMulti = await _phuongPhapVoCamService.DeletePhuongPhapVoCamMultiAsync(model.Ids);

            switch (isDeletePhuongPhapVoCamMulti.ErrorType)
            {
                case 1:
                    return NotFound();
                case 2:
                    throw new ArgumentException(_localizationService
                        .GetResource("Common.WrongLengthMultiDelete"));
                default:
                    return Ok();
            }
        }

        [HttpPost("ExportPhuongPhapVoCam")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucPhuongPhapVoCam)]
        public async Task<ActionResult> ExportPhuongPhapVoCam(QueryInfo queryInfo)
        {
            var gridData = await _phuongPhapVoCamService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (PhuongPhapVoCamGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<PhuongPhapVoCamExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(PhuongPhapVoCamExportExcel.Ma), "Mã"),
                (nameof(PhuongPhapVoCamExportExcel.Ten), "Tên"), 
                (nameof(PhuongPhapVoCamExportExcel.MoTa), "Mô Tả")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Loại Bệnh Viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LoaiBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
