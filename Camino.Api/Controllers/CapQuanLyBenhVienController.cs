using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhVien.CapQuanLyBenhVien;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhVien.CapQuanLyBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.BenhVien.CapQuanLyBenhVien;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BenhVien.CapQuanLyBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class CapQuanLyBenhVienController : CaminoBaseController
    {
        readonly ICapQuanLyBenhVienService _capQuanLyBenhVienService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public CapQuanLyBenhVienController(
                                  IExcelService excelService,
                                  ICapQuanLyBenhVienService capQuanLyBenhVienService,
                                  ILocalizationService localizationService
            )
        {
            _excelService = excelService;
            _capQuanLyBenhVienService = capQuanLyBenhVienService;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucCapQuanLyBenhVien
            )]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _capQuanLyBenhVienService
                .GetDataForGridAsync(queryInfo);

            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucCapQuanLyBenhVien
            )]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _capQuanLyBenhVienService
                .GetTotalPageForGridAsync(queryInfo);

            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(
            Enums.SecurityOperation.Add,
            Enums.DocumentType.DanhMucCapQuanLyBenhVien
            )]
        public async Task<ActionResult<CapQuanLyBenhVienViewModel>> Post
            ([FromBody]CapQuanLyBenhVienViewModel capQuanLyBenhVienViewModel)
        {
            var capQuanLyBenhVien = capQuanLyBenhVienViewModel
                .ToEntity<CapQuanLyBenhVien>();

            await _capQuanLyBenhVienService.AddAsync(capQuanLyBenhVien);

            var capQuanLyBenhVienId = await _capQuanLyBenhVienService
                .GetByIdAsync(capQuanLyBenhVien.Id);

            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = capQuanLyBenhVien.Id },
                capQuanLyBenhVienId.ToModel<CapQuanLyBenhVienViewModel>()
            );
        }

        [HttpGet("{id}")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucCapQuanLyBenhVien
            )]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CapQuanLyBenhVienViewModel>> Get(long id)
        {
            var capQuanLyBenhVien = await _capQuanLyBenhVienService
                .GetByIdAsync(id);

            if (capQuanLyBenhVien == null)
            {
                return NotFound();
            }

            return Ok(capQuanLyBenhVien.ToModel<CapQuanLyBenhVienViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(
            Enums.SecurityOperation.Update,
            Enums.DocumentType.DanhMucCapQuanLyBenhVien
            )]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatCapQuanLyBenhVien(
            [FromBody]CapQuanLyBenhVienViewModel capQuanLyBenhVienViewModel
            )
        {
            var capQuanLyBenhVien = await _capQuanLyBenhVienService
                .GetByIdAsync(capQuanLyBenhVienViewModel.Id);

            capQuanLyBenhVienViewModel.ToEntity(capQuanLyBenhVien);
            await _capQuanLyBenhVienService.UpdateAsync(capQuanLyBenhVien);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(
            Enums.SecurityOperation.Delete,
            Enums.DocumentType.DanhMucCapQuanLyBenhVien
        )]
        public async Task<ActionResult> Delete(long id)
        {
            var capQuanLyBenhVien = await _capQuanLyBenhVienService
                .GetByIdAsync(id);

            if (capQuanLyBenhVien == null)
            {
                return NotFound();
            }

            await _capQuanLyBenhVienService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(
            Enums.SecurityOperation.Delete,
            Enums.DocumentType.DanhMucCapQuanLyBenhVien
        )]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var capQuanLyBenhViens = await _capQuanLyBenhVienService.GetByIdsAsync(model.Ids);
            if (capQuanLyBenhViens == null)
            {
                return NotFound();
            }

            var quanLyBenhViens = capQuanLyBenhViens.ToList();
            if (quanLyBenhViens.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _capQuanLyBenhVienService.DeleteAsync(quanLyBenhViens);
            return NoContent();
        }

        [HttpPost("ExportCapQuanLyBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCapQuanLyBenhVien)]
        public async Task<ActionResult> ExportCapQuanLyBenhVien(QueryInfo queryInfo)
        {
            var gridData = await _capQuanLyBenhVienService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (CapQuanLyBenhVienGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<CapQuanLyBenhVienExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CapQuanLyBenhVienExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(CapQuanLyBenhVienExportExcel.MoTa), "Mô Tả"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Cấp Quản Lý Bệnh Viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CapQuanLyBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
