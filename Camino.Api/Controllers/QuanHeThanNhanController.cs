using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.QuanHeThanNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.QuanHeThanNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Localization;
using Camino.Services.QuanHeThanNhan;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.QuanHeThanNhan;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class QuanHeThanNhanController : CaminoBaseController
    {
        private readonly IExcelService _excelService;
        readonly IQuanHeThanNhanService _quanHeThanNhanService;
        private readonly ILocalizationService _localizationService;
        public QuanHeThanNhanController(
            IQuanHeThanNhanService quanHeThanNhanService,
            IExcelService excelService,
            ILocalizationService localizationService
        )
        {
            _quanHeThanNhanService = quanHeThanNhanService;
            _excelService = excelService;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quanHeThanNhanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quanHeThanNhanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        public async Task<ActionResult<QuanHeThanNhanViewModel>> Post
            ([FromBody]QuanHeThanNhanViewModel quanHeThanNhanViewModel)
        {
            var quanHeThanNhan = quanHeThanNhanViewModel.ToEntity<QuanHeThanNhan>();
            await _quanHeThanNhanService.AddAsync(quanHeThanNhan);
            var quanHeThanNhanId = await _quanHeThanNhanService.GetByIdAsync(quanHeThanNhan.Id);
            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = quanHeThanNhan.Id },
                quanHeThanNhanId.ToModel<QuanHeThanNhanViewModel>()
            );
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<QuanHeThanNhanViewModel>> Get(long id)
        {
            var quanHeThanNhan = await _quanHeThanNhanService.GetByIdAsync(id);
            if (quanHeThanNhan == null)
            {
                return NotFound();
            }

            return Ok(quanHeThanNhan.ToModel<QuanHeThanNhanViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatQuanHeThanNhan([FromBody]QuanHeThanNhanViewModel quanHeThanNhanViewModel)
        {
            var quanHeThanNhan = await _quanHeThanNhanService.GetByIdAsync(quanHeThanNhanViewModel.Id);
            quanHeThanNhanViewModel.ToEntity(quanHeThanNhan);
            await _quanHeThanNhanService.UpdateAsync(quanHeThanNhan);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        public async Task<ActionResult> Delete(long id)
        {
            var quanHeThanNhan = await _quanHeThanNhanService.GetByIdAsync(id);
            if (quanHeThanNhan == null)
            {
                return NotFound();
            }

            await _quanHeThanNhanService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var quanHeThanNhans = await _quanHeThanNhanService.GetByIdsAsync(model.Ids);
            if (quanHeThanNhans == null)
            {
                return NotFound();
            }

            var quanHeThanNhanList = quanHeThanNhans.ToList();
            if (quanHeThanNhanList.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _quanHeThanNhanService.DeleteAsync(quanHeThanNhanList);
            return NoContent();
        }

        [HttpPost("KichHoatQuanHeThanNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        public async Task<ActionResult> KichHoatQuanHeThanNhan(long id)
        {
            var entity = await _quanHeThanNhanService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _quanHeThanNhanService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportQuanHeThanNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucQuanHeThanNhan)]
        public async Task<ActionResult> ExportQuanHeThanNhan(QueryInfo queryInfo)
        {
            var gridData = await _quanHeThanNhanService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (QuanHeThanNhanGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<QuanHeThanNhanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(QuanHeThanNhanExportExcel.TenVietTat), "Tên Viết Tắt"));
            lstValueObject.Add((nameof(QuanHeThanNhanExportExcel.Ten), "Tên Đầy Đủ"));
            lstValueObject.Add((nameof(QuanHeThanNhanExportExcel.MoTa), "Mô Tả"));
            lstValueObject.Add((nameof(QuanHeThanNhanExportExcel.HieuLuc), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Quan Hệ Thân Nhân");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=QuanHeThanNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
