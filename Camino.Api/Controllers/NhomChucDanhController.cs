using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using Camino.Services.NhomChucDanh;
using Camino.Api.Models.NhomChucDanh;
using Camino.Core.Domain.Entities.NhomChucDanhs;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.NhomChucDanh;
using System.Collections.Generic;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class NhomChucDanhController : CaminoBaseController    {
        private readonly INhomChucDanhService _nhomChucDanhService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public NhomChucDanhController(INhomChucDanhService nhomChucDanhService, ILocalizationService localizationService, IExcelService excelService)
        {
            _nhomChucDanhService = nhomChucDanhService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomChucDanh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhomChucDanhService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomChucDanh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhomChucDanhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNhomChucDanh)]
        public async Task<ActionResult<NhomChucDanhViewModel>> Post([FromBody] NhomChucDanhViewModel nhomChucDanhViewModel)
        {
            var user = nhomChucDanhViewModel.ToEntity<NhomChucDanh>();
            _nhomChucDanhService.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<NhomChucDanhViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomChucDanh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhomChucDanhViewModel>> Get(long id)
        {
            var result = await _nhomChucDanhService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<NhomChucDanhViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNhomChucDanh)]
        public async Task<ActionResult> Put([FromBody] NhomChucDanhViewModel nhomChucDanhViewModel)
        {
            var nhomchucdanh = await _nhomChucDanhService.GetByIdAsync(nhomChucDanhViewModel.Id);
            if (nhomchucdanh == null)
            {
                return NotFound();
            }
            nhomChucDanhViewModel.ToEntity(nhomchucdanh);
            await _nhomChucDanhService.UpdateAsync(nhomchucdanh);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhomChucDanh)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucdanh = await _nhomChucDanhService.GetByIdAsync(id);
            if (chucdanh == null)
            {
                return NotFound();
            }
            await _nhomChucDanhService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhomChucDanh)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var nhomchucDanhs = await _nhomChucDanhService.GetByIdsAsync(model.Ids);
            if (nhomchucDanhs == null)
            {
                return NotFound();
            }
            if (nhomchucDanhs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _nhomChucDanhService.DeleteAsync(nhomchucDanhs);
            return NoContent();
        }


        #endregion

        [HttpPost("ExportNhomchucDanh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNhomChucDanh)]
        public async Task<ActionResult> ExportNhomchucDanh(QueryInfo queryInfo)
        {
            var gridData = await _nhomChucDanhService.GetDataForGridAsync(queryInfo, true);
            var nhomChucDanhData = gridData.Data.Select(p => (NhomChucDanhGridVo)p).ToList();
            var excelData = nhomChucDanhData.Map<List<NhomChucDanhExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NhomChucDanhExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(NhomChucDanhExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(NhomChucDanhExportExcel.MoTa), "Mô tả"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nhóm chức danh");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhomChucDanh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}