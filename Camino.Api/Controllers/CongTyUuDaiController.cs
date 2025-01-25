using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CongTyUuDais;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CongTyUuDais;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.CongTyUuDais;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class CongTyUuDaiController : CaminoBaseController
    {
        private readonly ICongTyUuDaiService _congTyUuDai;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public CongTyUuDaiController(ICongTyUuDaiService congTyUuDai, ILocalizationService localizationService, IExcelService excelService)
        {
            _congTyUuDai = congTyUuDai;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCongTyUuDai)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _congTyUuDai.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCongTyUuDai)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _congTyUuDai.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucCongTyUuDai)]
        public async Task<ActionResult<CongTyUuDaiViewModel>> Post
            ([FromBody]CongTyUuDaiViewModel congTyUuDaiViewModel)
        {
            var congTyUuDai = congTyUuDaiViewModel.ToEntity<CongTyUuDai>();
            await _congTyUuDai.AddAsync(congTyUuDai);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCongTyUuDai)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CongTyUuDaiViewModel>> Get(long id)
        {
            var congTyUuDai = await _congTyUuDai.GetByIdAsync(id);
            if (congTyUuDai == null)
            {
                return NotFound();
            }

            return Ok(congTyUuDai.ToModel<CongTyUuDaiViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCongTyUuDai)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Put([FromBody]CongTyUuDaiViewModel congTyUuDaiViewModel)
        {
            var congTyUuDai = await _congTyUuDai.GetByIdAsync(congTyUuDaiViewModel.Id);
            congTyUuDaiViewModel.ToEntity(congTyUuDai);
            await _congTyUuDai.UpdateAsync(congTyUuDai);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCongTyUuDai)]
        public async Task<ActionResult> Delete(long id)
        {
            var congTyUuDai = await _congTyUuDai.GetByIdAsync(id);
            if (congTyUuDai == null)
            {
                return NotFound();
            }

            await _congTyUuDai.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCongTyUuDai)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var congTyUuDais = await _congTyUuDai.GetByIdsAsync(model.Ids);
            if (congTyUuDais == null)
            {
                return NotFound();
            }

            var congTyUuDaisEnumerableList = congTyUuDais.ToList();

            if (congTyUuDaisEnumerableList.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _congTyUuDai.DeleteAsync(congTyUuDaisEnumerableList);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportCongTyUuDai")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCongTyUuDai)]
        public async Task<ActionResult> ExportCongTyUuDai(QueryInfo queryInfo)
        {
            var gridData = await _congTyUuDai.GetDataForGridAsync(queryInfo, true);
            var congTyUuDaiData = gridData.Data.Select(p => (CongTyUuDaiGridVo)p).ToList();
            var excelData = congTyUuDaiData.Map<List<CongTyUuDaiExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CongTyUuDaiExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(CongTyUuDaiExportExcel.MoTa), "Mô Tả"));
            lstValueObject.Add((nameof(CongTyUuDaiExportExcel.SuDung), "Trạng Thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Công Ty Ưu Đãi");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ExportCongTyUuDai" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("KichHoatCongTyUuDai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCongTyUuDai)]
        public async Task<ActionResult> KichHoatCongTyUuDai(long id)
        {
            var entity = await _congTyUuDai.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _congTyUuDai.UpdateAsync(entity);
            return NoContent();
        }
    }
}
