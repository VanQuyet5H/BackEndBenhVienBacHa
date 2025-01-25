using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhVien.LoaiBenhVien;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhVien.LoaiBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.BenhVien.LoaiBenhVien;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BenhVien.LoaiBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class LoaiBenhVienController : CaminoBaseController
    {
        private readonly ILoaiBenhVienService _loaiBenhVienService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public LoaiBenhVienController(
            ILoaiBenhVienService loaiBenhVienService,
            ILocalizationService localizationService,
            IExcelService excelService
            )
        {
            _loaiBenhVienService = loaiBenhVienService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucLoaiBenhVien
            )]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiBenhVienService
                .GetDataForGridAsync(queryInfo);

            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucLoaiBenhVien
            )]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiBenhVienService
                .GetTotalPageForGridAsync(queryInfo);

            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(
            Enums.SecurityOperation.Add,
            Enums.DocumentType.DanhMucLoaiBenhVien
            )]
        public async Task<ActionResult<LoaiBenhVienViewModel>> Post
            ([FromBody]LoaiBenhVienViewModel loaiBenhVienViewModel)
        {
            var loaiBenhVien = loaiBenhVienViewModel
                .ToEntity<LoaiBenhVien>();

            await _loaiBenhVienService.AddAsync(loaiBenhVien);

            var loaiBenhVienId = await _loaiBenhVienService
                .GetByIdAsync(loaiBenhVien.Id);

            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = loaiBenhVien.Id },
                loaiBenhVienId.ToModel<LoaiBenhVienViewModel>()
            );
        }

        [HttpGet("{id}")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucLoaiBenhVien
            )]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LoaiBenhVienViewModel>> Get(long id)
        {
            var loaiBenhVien = await _loaiBenhVienService
                .GetByIdAsync(id);

            if (loaiBenhVien == null)
            {
                return NotFound();
            }

            return Ok(loaiBenhVien.ToModel<LoaiBenhVienViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(
            Enums.SecurityOperation.Update,
            Enums.DocumentType.DanhMucLoaiBenhVien
            )]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatLoaiBenhVien(
            [FromBody]LoaiBenhVienViewModel loaiBenhVienViewModel
            )
        {
            var loaiBenhVien = await _loaiBenhVienService
                .GetByIdAsync(loaiBenhVienViewModel.Id);

            loaiBenhVienViewModel.ToEntity(loaiBenhVien);
            await _loaiBenhVienService.UpdateAsync(loaiBenhVien);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(
            Enums.SecurityOperation.Delete,
            Enums.DocumentType.DanhMucLoaiBenhVien
            )]
        public async Task<ActionResult> Delete(long id)
        {
            var loaiBenhVien = await _loaiBenhVienService
                .GetByIdAsync(id);

            if (loaiBenhVien == null)
            {
                return NotFound();
            }

            await _loaiBenhVienService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(
            Enums.SecurityOperation.Delete,
            Enums.DocumentType.DanhMucLoaiBenhVien
            )]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var loaiBenhViens = await _loaiBenhVienService
                .GetByIdsAsync(model.Ids);
            if (loaiBenhViens == null)
            {
                return NotFound();
            }

            var benhViens = loaiBenhViens.ToList();
            if (benhViens.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _loaiBenhVienService.DeleteAsync(benhViens);
            return NoContent();
        }

        [HttpPost("ExportLoaiBenhVien")]
        [ClaimRequirement(
            Enums.SecurityOperation.Process,
            Enums.DocumentType.DanhMucLoaiBenhVien
        )]
        public async Task<ActionResult> ExportLoaiBenhVien(QueryInfo queryInfo)
        {
            var gridData = await _loaiBenhVienService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (LoaiBenhVienGirdVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<LoaiBenhVienExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LoaiBenhVienExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(LoaiBenhVienExportExcel.MoTa), "Mô Tả"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Loại Bệnh Viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LoaiBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
