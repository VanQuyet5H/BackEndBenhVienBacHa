using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.DonViTinh;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DonViTinh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DonViTinh;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.MauVaChePham;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{

    public class DonViTinhController : CaminoBaseController
    {
        private readonly IDonViTinhService _donViTinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DonViTinhController(IDonViTinhService donViTinhService, IExcelService excelService)
        {
            _donViTinhService = donViTinhService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _donViTinhService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _donViTinhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region CRUD
        /// <summary>
        ///     Add can ho
        /// </summary>
        /// <param name="DonViTinhViewModel"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult<DonViTinhViewModel>> Post([FromBody] DonViTinhViewModel donViTinhViewModel)
        {
            var donvitinh = donViTinhViewModel.ToEntity<DonViTinh>();
            _donViTinhService.Add(donvitinh);
            return CreatedAtAction(nameof(Get), new { id = donvitinh.Id }, donvitinh.ToModel<DonViTinhViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDonViTinh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DonViTinhViewModel>> Get(long id)
        {
            var result = await _donViTinhService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<DonViTinhViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult> Put([FromBody] DonViTinhViewModel donViTinhViewModel)
        {
            var donvitinh = await _donViTinhService.GetByIdAsync(donViTinhViewModel.Id);
            if (donvitinh == null)
            {
                return NotFound();
            }
            donViTinhViewModel.ToEntity(donvitinh);
            await _donViTinhService.UpdateAsync(donvitinh);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult> Delete(long id)
        {
            var donvitinh = await _donViTinhService.GetByIdAsync(id);
            if (donvitinh == null)
            {
                return NotFound();
            }

            await _donViTinhService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var donvitinhs = await _donViTinhService.GetByIdsAsync(model.Ids);
            if (donvitinhs == null)
            {
                return NotFound();
            }
            if (donvitinhs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _donViTinhService.DeleteAsync(donvitinhs);
            return NoContent();
        }
        #endregion

        [HttpPost("GetDanhSachDonViTinh")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachDonViTinhAsync([FromBody]DropDownListRequestModel model)
        {
            var lstDonViTinh = await _donViTinhService.GetDanhSachDonViTinhAsync(model);
            return Ok(lstDonViTinh);
        }
        #region   // export excel

        [HttpPost("ExportDonViTinh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult> ExportExportExportDonViTinh(QueryInfo queryInfo)
        {
            var gridData = await _donViTinhService.GetDataForGridAsync(queryInfo, true);
            var lsDonViTinh = gridData.Data.Select(p => (DonViTinhGridVo)p).ToList();
            var dataExcel = lsDonViTinh.Map<List<ExportDonViTinhExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ExportDonViTinhExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ExportDonViTinhExportExcel.Ten), "Tên"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Đơn Vị Tính");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DonViTinh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}