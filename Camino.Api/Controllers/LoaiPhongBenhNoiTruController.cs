using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.LoaiPhongBenh.LoaiPhongBenhNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.LoaiPhongBenh.LoaiPhongBenhNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.LoaiPhongBenh.LoaiPhongBenhNoiTru;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.LoaiPhongBenh.LoaiPhongBenhNoiTru;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class LoaiPhongBenhNoiTruController : CaminoBaseController
    {
        private readonly ILoaiPhongBenhNoiTruService _loaiPhongBenhNoiTruService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public LoaiPhongBenhNoiTruController(
            ILoaiPhongBenhNoiTruService loaiPhongBenhNoiTruService,
            IExcelService excelService,
            ILocalizationService localizationService
                )
        {
            _loaiPhongBenhNoiTruService = loaiPhongBenhNoiTruService;
            _excelService = excelService;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucLoaiPhongBenh
            )]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiPhongBenhNoiTruService
                .GetDataForGridAsync(queryInfo);

            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucLoaiPhongBenh
            )]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiPhongBenhNoiTruService
                .GetTotalPageForGridAsync(queryInfo);

            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(
            Enums.SecurityOperation.Add,
            Enums.DocumentType.DanhMucLoaiPhongBenh
            )]
        public async Task<ActionResult<LoaiPhongBenhNoiTruViewModel>> Post
            ([FromBody]LoaiPhongBenhNoiTruViewModel loaiPhongBenhNoiTruViewModel)
        {
            var loaiPhongBenhNoiTru = loaiPhongBenhNoiTruViewModel
                .ToEntity<LoaiPhongBenhNoiTru>();

            await _loaiPhongBenhNoiTruService.AddAsync(loaiPhongBenhNoiTru);

            var loaiPhongBenhNoiTruId = await _loaiPhongBenhNoiTruService
                .GetByIdAsync(loaiPhongBenhNoiTru.Id);

            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = loaiPhongBenhNoiTru.Id },
                loaiPhongBenhNoiTruId.ToModel<LoaiPhongBenhNoiTruViewModel>()
            );
        }

        [HttpGet("{id}")]
        [ClaimRequirement(
            Enums.SecurityOperation.View,
            Enums.DocumentType.DanhMucLoaiPhongBenh
            )]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LoaiPhongBenhNoiTruViewModel>> Get(long id)
        {
            var loaiPhongBenhNoiTru = await _loaiPhongBenhNoiTruService
                .GetByIdAsync(id);

            if (loaiPhongBenhNoiTru == null)
            {
                return NotFound();
            }

            return Ok(loaiPhongBenhNoiTru.ToModel<LoaiPhongBenhNoiTruViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(
            Enums.SecurityOperation.Update,
            Enums.DocumentType.DanhMucLoaiPhongBenh
            )]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatLoaiPhongBenhNoiTru(
            [FromBody]LoaiPhongBenhNoiTruViewModel loaiPhongBenhNoiTruViewModel
            )
        {
            var loaiPhongBenhNoiTru = await _loaiPhongBenhNoiTruService
                .GetByIdAsync(loaiPhongBenhNoiTruViewModel.Id);

            loaiPhongBenhNoiTruViewModel.ToEntity(loaiPhongBenhNoiTru);
            await _loaiPhongBenhNoiTruService.UpdateAsync(loaiPhongBenhNoiTru);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(
            Enums.SecurityOperation.Delete,
            Enums.DocumentType.DanhMucLoaiPhongBenh
            )]
        public async Task<ActionResult> Delete(long id)
        {
            var loaiPhongBenhNoiTru = await _loaiPhongBenhNoiTruService
                .GetByIdAsync(id);

            if (loaiPhongBenhNoiTru == null)
            {
                return NotFound();
            }

            await _loaiPhongBenhNoiTruService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete,
            Enums.DocumentType.DanhMucLoaiPhongBenh)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var loaiPhongBenhNoiTrus = await _loaiPhongBenhNoiTruService
                .GetByIdsAsync(model.Ids);
            if (loaiPhongBenhNoiTrus == null)
            {
                return NotFound();
            }

            var phongBenhNoiTrus = loaiPhongBenhNoiTrus.ToList();
            if (phongBenhNoiTrus.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _loaiPhongBenhNoiTruService.DeleteAsync(phongBenhNoiTrus);
            return NoContent();
        }

        [HttpPost("ExportLoaiPhongBenhNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucLoaiPhongBenh)]
        public async Task<ActionResult> ExportLoaiPhongBenhNoiTru(QueryInfo queryInfo)
        {
            var gridData = await _loaiPhongBenhNoiTruService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (LoaiPhongBenhNoiTruGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<LoaiPhongBenhNoiTruExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LoaiPhongBenhNoiTruExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(LoaiPhongBenhNoiTruExportExcel.MoTa), "Mô Tả"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Loại Phòng Bệnh Nội Trú");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LoaiPhongBenhNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}