using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.PhamViHanhNghe;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhamViHanhNghes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Localization;
using Camino.Services.PhamViHanhNghe;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.PhamViHanhNghe;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class PhamViHanhNgheController : CaminoBaseController
    {
        private readonly IPhamViHanhNgheService _phamViHanhNgheService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public PhamViHanhNgheController(IPhamViHanhNgheService phamViHanhNgheService, IExcelService excelService, ILocalizationService localizationService)
        {
            _phamViHanhNgheService = phamViHanhNgheService;
            _excelService = excelService;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phamViHanhNgheService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phamViHanhNgheService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        public async Task<ActionResult<PhamViHanhNgheViewModel>> Post
            ([FromBody]PhamViHanhNgheViewModel phamViHanhNgheViewModel)
        {
            var phamViHanhNghe = phamViHanhNgheViewModel.ToEntity<PhamViHanhNghe>();
            await _phamViHanhNgheService.AddAsync(phamViHanhNghe);
            var phamViHanhNgheId = await _phamViHanhNgheService.GetByIdAsync(phamViHanhNghe.Id);
            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = phamViHanhNghe.Id },
                phamViHanhNgheId.ToModel<PhamViHanhNgheViewModel>()
            );
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PhamViHanhNgheViewModel>> Get(long id)
        {
            var phamViHanhNghe = await _phamViHanhNgheService.GetByIdAsync(id);
            if (phamViHanhNghe == null)
            {
                return NotFound();
            }

            return Ok(phamViHanhNghe.ToModel<PhamViHanhNgheViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatPhamViHanhNghe([FromBody]PhamViHanhNgheViewModel phamViHanhNgheViewModel)
        {
            var phamViHanhNghe = await _phamViHanhNgheService.GetByIdAsync(phamViHanhNgheViewModel.Id);
            phamViHanhNgheViewModel.ToEntity(phamViHanhNghe);
            await _phamViHanhNgheService.UpdateAsync(phamViHanhNghe);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        public async Task<ActionResult> Delete(long id)
        {
            var phamViHanhNghe = await _phamViHanhNgheService.GetByIdAsync(id);
            if (phamViHanhNghe == null)
            {
                return NotFound();
            }

            await _phamViHanhNgheService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var phamViHanhNghes = await _phamViHanhNgheService.GetByIdsAsync(model.Ids);
            if (phamViHanhNghes == null)
            {
                return NotFound();
            }

            var viHanhNghes = phamViHanhNghes.ToList();
            if (viHanhNghes.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _phamViHanhNgheService.DeleteAsync(viHanhNghes);
            return NoContent();
        }

        #region GetListLookupItemVo
        [HttpPost("GetListPhamViHanhNghe")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListPhamViHanhNghe([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _phamViHanhNgheService.GetListPhamViHanhNghe(model);
            return Ok(lookup);
        }

        [HttpPost("ExportPhamViHanhNghe")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucPhamViHanhNghe)]
        public async Task<ActionResult> ExportPhamViHanhNghe(QueryInfo queryInfo)
        {
            var gridData = await _phamViHanhNgheService.GetDataForGridAsync(queryInfo, true);
            var phamViHanhNgheData = gridData.Data.Select(p => (PhamViHanhNgheGridVo)p).ToList();
            var dataExcel = phamViHanhNgheData.Map<List<PhamViHanhNgheExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(PhamViHanhNgheExportExcel.Ten), "Tên Đầy Đủ"),
                (nameof(PhamViHanhNgheExportExcel.Ma), "Mã"),
                (nameof(PhamViHanhNgheExportExcel.MoTa), "Mô Tả")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Phạm Vi Hành Nghề");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=PhamViHanhNghe" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
