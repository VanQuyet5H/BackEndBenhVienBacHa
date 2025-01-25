using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.DichVuKhamBenh;
using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Extensions;
using Camino.Core.Domain.Entities.DichVuKhamBenhs;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using System;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.DichVuKhamBenh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DichVuKhamBenhController : CaminoBaseController
    {
        private readonly IDichVuKhamBenhService _dichVuKhamBenhService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DichVuKhamBenhController(IDichVuKhamBenhService dichVuKhamBenhService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _dichVuKhamBenhService = dichVuKhamBenhService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoas")]
        public async Task<List<LookupItemVo>> GetKhoas(DropDownListRequestModel model)
        {
            return await _dichVuKhamBenhService.GetKhoas(model);
        }


        #region Get/Add/Delete/Update
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult> Post([FromBody] DichVuKhamBenhViewModel dichVuKhamBenhViewModel)
        {
            var dvKhamBenh = dichVuKhamBenhViewModel.ToEntity<DichVuKhamBenh>();
            await _dichVuKhamBenhService.AddAsync(dvKhamBenh);
            return CreatedAtAction(nameof(Get), new { id = dvKhamBenh.Id }, dvKhamBenh.ToModel<DichVuKhamBenhViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DichVuKhamBenhViewModel>> Get(long id)
        {
            var dvKhamBenh = await _dichVuKhamBenhService.GetByIdAsync(id, s => s.Include(x => x.DichVuKhamBenhThongTinGias));
            if (dvKhamBenh == null)
            {
                return NotFound();
            }
            return Ok(dvKhamBenh.ToModel<DichVuKhamBenhViewModel>());
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult> Put([FromBody] DichVuKhamBenhViewModel dichVuKhamBenhViewModel)
        {
            var dvKhamBenh = await _dichVuKhamBenhService.GetByIdAsync(dichVuKhamBenhViewModel.Id, s => s.Include(x => x.DichVuKhamBenhThongTinGias));
            if (dvKhamBenh == null)
            {
                return NotFound();
            }
            dichVuKhamBenhViewModel.ToEntity(dvKhamBenh);
            await _dichVuKhamBenhService.UpdateAsync(dvKhamBenh);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult> Delete(long id)
        {
            var dvKhamBenh = await _dichVuKhamBenhService.GetByIdAsync(id, s => s.Include(x => x.DichVuKhamBenhThongTinGias));
            if (dvKhamBenh == null)
            {
                return NotFound();
            }
            await _dichVuKhamBenhService.DeleteByIdAsync(id);
            return NoContent();
        }
        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var dvKhamBenhs = await _dichVuKhamBenhService.GetByIdsAsync(model.Ids);
            if (dvKhamBenhs == null)
            {
                return NotFound();
            }
            if (dvKhamBenhs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _dichVuKhamBenhService.DeleteAsync(dvKhamBenhs);
            return NoContent();
        }
        #endregion
        [HttpPost("KichHoatDichVuKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDichVuKhamBenh)]
        public async Task<ActionResult> KichHoatDichVuKhamBenh(long id)
        {
            var entity = await _dichVuKhamBenhService.GetByIdAsync(id);
            //entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _dichVuKhamBenhService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportDichVuKhamBenh")]
        public async Task<ActionResult> ExportDichVuKhamBenh(QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetDataForGridAsync(queryInfo, true);
            var dichVuKhamBenhData = gridData.Data.Select(p => (DichVuKhamBenhGridVo)p).ToList();
            var excelData = dichVuKhamBenhData.Map<List<DichVuKhamBenhExportExcel>>();

            foreach (var item in excelData)
            {
                var gridChildData = await _dichVuKhamBenhService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var childData = gridChildData.Data.Select(p => (DichVuKhamBenhThongTinGiaGridVo)p).ToList();
                var childExcelData = childData.Map<List<DichVuKhamBenhExportExcelChild>>();
                item.DichVuKhamBenhExportExcelChild.AddRange(childExcelData);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DichVuKhamBenhExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DichVuKhamBenhExportExcel.MaTT37), "Mã TT37"));
            lstValueObject.Add((nameof(DichVuKhamBenhExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(DichVuKhamBenhExportExcel.TenKhoa), "Khoa"));
            lstValueObject.Add((nameof(DichVuKhamBenhExportExcel.TenHangBenhVien), "Hạng bệnh viện"));
            lstValueObject.Add((nameof(DichVuKhamBenhExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(DichVuKhamBenhExportExcel.DichVuKhamBenhExportExcelChild), ""));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dịch vụ khám bệnh");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuKhamBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}