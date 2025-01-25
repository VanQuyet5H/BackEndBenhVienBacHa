using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.NhaThau;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhaThau;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class NhaThauController : CaminoBaseController
    {
        private readonly INhaThauService _nhaThauService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public NhaThauController(
            INhaThauService nhaThauService
            , IExcelService excelService
            , ILocalizationService localizationService)
        {
            _excelService = excelService;
            _nhaThauService = nhaThauService;
            _localizationService = localizationService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhaThau)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhaThauService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhaThau)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhaThauService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNhaThau)]
        public async Task<ActionResult<NhaThauViewModel>> Post
            ([FromBody]NhaThauViewModel nhaThauViewModel)
        {
            var nhaThau = nhaThauViewModel.ToEntity<NhaThau>();
            await _nhaThauService.AddAsync(nhaThau);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhaThau)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhaThauViewModel>> Get(long id)
        {
            var nhaThau = await _nhaThauService.GetByIdAsync(id);

            if (nhaThau == null)
            {
                return NotFound();
            }

            return Ok(nhaThau.ToModel<NhaThauViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNhaThau)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatNhaThau([FromBody]NhaThauViewModel nhaThauViewModel)
        {
            var nhaThau = await _nhaThauService.GetByIdAsync(nhaThauViewModel.Id);
            nhaThauViewModel.ToEntity(nhaThau);
            await _nhaThauService.UpdateAsync(nhaThau);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhaThau)]
        public async Task<ActionResult> Delete(long id)
        {
            var nhaThau = await _nhaThauService.GetByIdAsync(id);
            if (nhaThau == null)
            {
                return NotFound();
            }

            await _nhaThauService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhaThau)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var nhaThaus = await _nhaThauService.GetByIdsAsync(model.Ids);
            if (nhaThaus == null)
            {
                return NotFound();
            }

            var listNhaThaus = nhaThaus.ToList();
            if (listNhaThaus.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _nhaThauService.DeleteAsync(listNhaThaus);
            return NoContent();
        }

        [HttpPost("ExportNhaThau")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNhaThau)]
        public async Task<ActionResult> ExportNhaThau(QueryInfo queryInfo)
        {
            var gridData = await _nhaThauService.GetDataForGridAsync(queryInfo, true);
            var nhaThauData = gridData.Data.Select(p => (NhaThauGridVo)p).ToList();
            var dataExcel = nhaThauData.Map<List<NhaThauExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(NhaThauExportExcel.Ten), "Tên"),
                (nameof(NhaThauExportExcel.DiaChi), "Địa Chỉ"),
                (nameof(NhaThauExportExcel.MaSoThue), "Mã Số Thuế"),
                (nameof(NhaThauExportExcel.TaiKhoanNganHang), "Tài Khoản Ngân Hàng"),
                (nameof(NhaThauExportExcel.NguoiDaiDien), "Người Đại Diện"),
                (nameof(NhaThauExportExcel.NguoiLienHe), "Người Liên Hệ"),
                (nameof(NhaThauExportExcel.SoDienThoaiLienHe), "Số Điện Thoại Liên Hệ"),
                (nameof(NhaThauExportExcel.EmailLienHe), "Email Liên Hệ")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Nhà Thầu");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhaThau" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}