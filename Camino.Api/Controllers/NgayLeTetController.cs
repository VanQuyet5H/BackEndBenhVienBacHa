using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.NgayLeTet;
using Camino.Api.Models.QuocGia;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NgayLeTet;
using Camino.Core.Domain.ValueObject.QuocGia;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NgayLeTet;
using Camino.Services.QuocGias;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NgayLeTetController : CaminoBaseController
    {      
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly INgayLeTetService _ngayLeTetService;

        public NgayLeTetController(
            ILocalizationService localizationService,
            INgayLeTetService ngayLeTetService,
            IExcelService excelService)
        {          
            _localizationService = localizationService;
            _excelService = excelService;
            _ngayLeTetService = ngayLeTetService;
        }

        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNgayLe)]
        [HttpPost("GetDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ngayLeTetService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNgayLe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ngayLeTetService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNgayLe)]
        public async Task<ActionResult<QuocGiaViewModel>> Get(long id)
        {
            var ngayNghiTet = await _ngayLeTetService.GetByIdAsync(id);

            if (ngayNghiTet == null)
                return NotFound();

            NgayLeTetViewModel ngayLeTetViewModel = ngayNghiTet.ToModel<NgayLeTetViewModel>();
            return Ok(ngayLeTetViewModel);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuanLyNgayLe)]
        public async Task<ActionResult<QuocGiaViewModel>> Post([FromBody] NgayLeTetViewModel ngayLeTetViewModel)
        {
            var ngayLeTet = ngayLeTetViewModel.ToEntity<NgayLeTet>();
            await _ngayLeTetService.AddAsync(ngayLeTet);

            var ngayLeTetEntity = await _ngayLeTetService.GetByIdAsync(ngayLeTet.Id);
            var actionName = nameof(Get);

            return CreatedAtAction(actionName, new { id = ngayLeTetEntity.Id }, ngayLeTetEntity.ToModel<NgayLeTetViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuanLyNgayLe)]
        public async Task<ActionResult> Put([FromBody] NgayLeTetViewModel ngayLeTetViewModel)
        {
            var ngayLeTetEntity = await _ngayLeTetService.GetByIdAsync(ngayLeTetViewModel.Id);

            if (ngayLeTetEntity == null)
                return NotFound();

            ngayLeTetViewModel.ToEntity(ngayLeTetEntity);
            await _ngayLeTetService.UpdateAsync(ngayLeTetEntity);

            return NoContent();
        }
        
        [HttpPost("Delete")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.QuanLyNgayLe)]
        public async Task<ActionResult> Delete(long id)
        {
            var ngayLeTetEntity = await _ngayLeTetService.GetByIdAsync(id);

            if (ngayLeTetEntity == null)
                return NotFound();

            _ngayLeTetService.DeleteById(ngayLeTetEntity.Id);
            return NoContent();
        }

        [HttpPost("GetNam")]
        public ActionResult<ICollection<LookupItemVo>> GetNam([FromBody] DropDownListRequestModel model)
        {
            var nam = JsonConvert.DeserializeObject<NamSearch>(model.ParameterDependencies.Replace("undefined", "null"));
            var lookup = _ngayLeTetService.GetNamSreachs(model, nam);
            return Ok(lookup);
        }

        [HttpPost("ExportNgayLeTet")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuanLyNgayLe)]
        public async Task<ActionResult> ExportNgayLeTet(QueryInfo queryInfo)
        {
            var gridData = await _ngayLeTetService.GetDataForGridAsync(queryInfo, true);
            var danTocData = gridData.Data.Select(p => (NgayLeTetGridVo)p).ToList();
            var excelData = danTocData.Map<List<NgayLeTetExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NgayLeTetExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(NgayLeTetExportExcel.Ngay), "Ngày"));
            lstValueObject.Add((nameof(NgayLeTetExportExcel.Thang), "Tháng"));
            lstValueObject.Add((nameof(NgayLeTetExportExcel.Nam), "Năm"));
            lstValueObject.Add((nameof(NgayLeTetExportExcel.LeHangNam), "Lễ hàng năm"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lễ hàng năm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NgayLeTet" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion        
    }
}
