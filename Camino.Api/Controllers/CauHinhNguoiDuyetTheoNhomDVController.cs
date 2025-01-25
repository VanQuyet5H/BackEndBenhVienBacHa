using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Cauhinh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.CauHinh;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class CauHinhNguoiDuyetTheoNhomDVController : CaminoBaseController
    {
        private readonly ICauHinhNguoiDuyetTheoNhomDVService _cauHinhNguoiDuyetTheoNhomDVService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public CauHinhNguoiDuyetTheoNhomDVController(ICauHinhNguoiDuyetTheoNhomDVService cauHinhNguoiDuyetTheoNhomDVService, ILocalizationService localizationService, IExcelService excelService)
        {
            _cauHinhNguoiDuyetTheoNhomDVService = cauHinhNguoiDuyetTheoNhomDVService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _cauHinhNguoiDuyetTheoNhomDVService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _cauHinhNguoiDuyetTheoNhomDVService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }



        [HttpPost("GetListNhanVienBenhViens")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetListNhanVienBenhViens([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhNguoiDuyetTheoNhomDVService.GetListNhanViens(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetLisNhomDVXetNghiems")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetLisNhomDVXetNghiems([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhNguoiDuyetTheoNhomDVService.GetLisNhomDVXetNghiems(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetLisNhomDVXetNghiemUpdates")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetLisNhomDVXetNghiemUpdates([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhNguoiDuyetTheoNhomDVService.GetLisNhomDVXetNghiemUpdates(queryInfo);
            return Ok(lookup);
        }
        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu)]
        public async Task<ActionResult<CauHinhNguoiDuyetTheoNhomDichVuViewModel>> Post([FromBody] CauHinhNguoiDuyetTheoNhomDichVuViewModel cauHinh)
        {
            var user = cauHinh.ToEntity<CauHinhNguoiDuyetTheoNhomDichVu>();
            _cauHinhNguoiDuyetTheoNhomDVService.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<CauHinhNguoiDuyetTheoNhomDichVuViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CauHinhNguoiDuyetTheoNhomDichVuViewModel>> Get(long id)
        {
            var result = await _cauHinhNguoiDuyetTheoNhomDVService.GetByIdAsync(id,s=>s.Include(d=>d.NhomDichVuBenhVien).Include(g=>g.NhanVien).ThenInclude(d=>d.User));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<CauHinhNguoiDuyetTheoNhomDichVuViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu)]
        public async Task<ActionResult> Put([FromBody] CauHinhNguoiDuyetTheoNhomDichVuViewModel chViewModel)
        {
            var cd = await _cauHinhNguoiDuyetTheoNhomDVService.GetByIdAsync(chViewModel.Id);
            if (cd == null)
            {
                return NotFound();
            }
            var kiemTraNhomDichVuBenhVienDaSetNhanVienDuyetChua = _cauHinhNguoiDuyetTheoNhomDVService.KiemTraNhomDichVuDaSetNhanVienDuyet(chViewModel.NhomDichVuBenhVienId, chViewModel.Id);
            if (kiemTraNhomDichVuBenhVienDaSetNhanVienDuyetChua == true)
            {
                throw new Models.Error.ApiException(_localizationService.GetResource("NhomDichVuBenhVien.DichVuExit.Required"));
            }
            chViewModel.ToEntity(cd);
            await _cauHinhNguoiDuyetTheoNhomDVService.UpdateAsync(cd);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucdanh = await _cauHinhNguoiDuyetTheoNhomDVService.GetByIdAsync(id);
            if (chucdanh == null)
            {
                return NotFound();
            }
            await _cauHinhNguoiDuyetTheoNhomDVService.DeleteByIdAsync(id);
            return NoContent();
        }

     
        #endregion

        [HttpPost("ExportCauHinhNguoiDuyetTheoNhomDV")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu)]
        public async Task<ActionResult> ExportChucDanh(QueryInfo queryInfo)
        {
            var gridData = await _cauHinhNguoiDuyetTheoNhomDVService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (CauHinhNguoiDuyetTheoNhomDVGridVo)p).ToList();
            var excelData = data.Map<List<CauHinhNguoiDuyetTheoNhomDichVuExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CauHinhNguoiDuyetTheoNhomDichVuExportExcel.TenNhanVienDuyet), "Tên nhân viên duyệt"));
            lstValueObject.Add((nameof(CauHinhNguoiDuyetTheoNhomDichVuExportExcel.TenNhomDichVuBenhVien), "Tên nhóm dịch vụ bệnh viện"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Cấu hình người duyệt theo nhóm dịch vụ");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CauHinhNguoiDuyetTheoNhomDV" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}