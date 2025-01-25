using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.QuanLyTaiKhoan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuanLyTaiKhoan;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.QuanLyTaiKhoan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class QuanLyTaiKhoanController : CaminoBaseController
    {
        private readonly IQuanLyTaiKhoanService _quanLyTaiKhoanService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public QuanLyTaiKhoanController(IQuanLyTaiKhoanService quanLyTaiKhoanService, ILocalizationService localizationService, IExcelService excelService)
        {
            _quanLyTaiKhoanService = quanLyTaiKhoanService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quanLyTaiKhoanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quanLyTaiKhoanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridTimNhanVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridTimNhanVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quanLyTaiKhoanService.GetDataForGridTimNhanVienAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridTimNhanVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridTimNhanVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quanLyTaiKhoanService.GetTotalPageForGridTimNhanVienAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TaoTaiKhoan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.User)]
        public async Task<ActionResult> TaoTaiKhoan(QuanLyTaiKhoanNhanVienViewModel model)
        {
            if (model.NhanVienId == null || model.NhanVienId == 0)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.MustHaveNhanVien"));
            }

            var entity = await _quanLyTaiKhoanService.CreateEmployeeAccount(model.NhanVienId ?? 0, model.Password);
            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.DontExists"));
            }

            return Ok(entity);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult> GetNhanVien(long nhanVienId)
        {
            var nhanVien = await _quanLyTaiKhoanService.GetByIdAsync(nhanVienId, s => s.Include(p => p.User).Include(p => p.NhanVienRoles).ThenInclude(p => p.Role));
            var result = nhanVien.ToModel<QuanLyTaiKhoanNhanVienViewModel>();

            if (nhanVien.NhanVienRoles.Any())
            {
                var lstRole = nhanVien.NhanVienRoles.ToList();
                foreach (var item in lstRole)
                {
                    result.roleCurrent.Add(item.RoleId);
                    result.roleNew.Add(item.RoleId);
                }
            }

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ChangePassword")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.User)]
        public async Task<ActionResult> ChangePassword(QuanLyTaiKhoanNhanVienViewModel model)
        {
            if (model.NhanVienId == null || model.NhanVienId == 0)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.MustHaveNhanVien"));
            }

            var entity = await _quanLyTaiKhoanService.CreateEmployeeAccount(model.NhanVienId ?? 0, model.Password);
            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.DontExists"));
            }
            return Ok(entity);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetLstRole")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public async Task<ActionResult<List<LookupItemVo>>> GetLstRole([FromBody]DropDownListRequestModel model ,long nhanVienId)
        {
            var result = await _quanLyTaiKhoanService.GetListRole(model);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ChangeRole")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.User)]
        public async Task<ActionResult> ChangeRole(QuanLyTaiKhoanNhanVienViewModel model)
        {
            if (model.NhanVienId == null || model.NhanVienId == 0)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.MustHaveNhanVien"));
            }

            var entity = await _quanLyTaiKhoanService.ChangeRoleEmployeeAccount(model.NhanVienId ?? 0, model.roleNew);
            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.DontExists"));
            }
            return Ok(entity);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ChangeActive")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.User)]
        public async Task<ActionResult> ChangeActive(long nhanVienId)
        {
            var entity = await _quanLyTaiKhoanService.ChangeActiveEmployeeAccount(nhanVienId);
            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.DontExists"));
            }

            return Ok(entity);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("RemoveAccount")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.User)]
        public async Task<ActionResult> RemoveAccount(long nhanVienId)
        {
            var entity = await _quanLyTaiKhoanService.RemoveEmployeeAccount(nhanVienId);
            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("QuanLyTaiKhoan.NhanVienId.DontExists"));
            }

            return Ok();
        }

        [HttpPost("ExportTaiKhoan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.User)]
        public async Task<ActionResult> ExportTaiKhoan(QueryInfo queryInfo)
        {
            var gridData = await _quanLyTaiKhoanService.GetDataForGridAsync(queryInfo, true);
            var taiKhoanData = gridData.Data.Select(p => (QuanLyTaiKhoanGridVo)p).ToList();
            var excelData = taiKhoanData.Map<List<QuanLyTaiKhoanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(QuanLyTaiKhoanExportExcel.HoTen), "Họ tên"));
            lstValueObject.Add((nameof(QuanLyTaiKhoanExportExcel.SoDienThoai), "Số điện thoại"));
            lstValueObject.Add((nameof(QuanLyTaiKhoanExportExcel.Email), "Email"));
            lstValueObject.Add((nameof(QuanLyTaiKhoanExportExcel.DiaChi), "Địa chỉ"));
            lstValueObject.Add((nameof(QuanLyTaiKhoanExportExcel.IsActiveDisplay), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Tài khoản người dùng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TaiKhoanNguoiDung" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}