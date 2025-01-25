using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.LoaiGiaDichVu;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LoaiGiaDichVus;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.LoaiGiaDichVus;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class LoaiGiaDichVuController : CaminoBaseController
    {
        private readonly ILoaiGiaDichVuService _loaiGiaDichVuService;
        private readonly IExcelService _excelService;
        public LoaiGiaDichVuController(
            ILoaiGiaDichVuService loaiGiaDichVuService
            , IExcelService excelService
            )
        {
            _loaiGiaDichVuService = loaiGiaDichVuService;
            _excelService = excelService;
        }

        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiGiaDichVuService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiGiaDichVuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion

        #region Lookup
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("NhomDichVus")]
        public ActionResult GetGioiTinh([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.NhomDichVuLoaiGia>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }


        #endregion

        #region CRUD
        [HttpPost("GetThongTinLoaiGia")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult> GetThongTinLoaiGia(LoaiGiaDichVuInfoVo info)
        {
            var result = await _loaiGiaDichVuService.GetThongTinLoaiGiaAsync(info);
            return Ok(result.Map<LoaiGiaDichVuViewModel>());
        }

        [HttpPost("ThemLoaiGia")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult> ThemLoaiGia(LoaiGiaDichVuViewModel viewModel)
        {
            var info = viewModel.Map<LoaiGiaDichVuGridVo>();
            await _loaiGiaDichVuService.ThemLoaiGia(info);
            return Ok();
        }

        [HttpPut("CapNhatLoaiGia")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult> CapNhatLoaiGia(LoaiGiaDichVuViewModel viewModel)
        {
            var info = viewModel.Map<LoaiGiaDichVuGridVo>();
            await _loaiGiaDichVuService.CapNhatLoaiGia(info);
            return Ok();
        }

        [HttpDelete("XoaLoaiGiaDichVuKham")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult> XoaLoaiGiaDichVuKham(long id)
        {
            await _loaiGiaDichVuService.XuLyXoaLoaiGiaAsync(id, Enums.NhomDichVuLoaiGia.DichVuKhamBenh);
            return NoContent();
        }

        [HttpDelete("XoaLoaiGiaDichVuKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult> XoaLoaiGiaDichVuKyThuat(long id)
        {
            await _loaiGiaDichVuService.XuLyXoaLoaiGiaAsync(id, Enums.NhomDichVuLoaiGia.DichVuKyThuat);
            return NoContent();
        }

        [HttpDelete("XoaLoaiGiaDichVuGiuong")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult> XoaLoaiGiaDichVuGiuong(long id)
        {
            await _loaiGiaDichVuService.XuLyXoaLoaiGiaAsync(id, Enums.NhomDichVuLoaiGia.DichVuGiuongBenh);
            return NoContent();
        }
        #endregion

        #region Excel
        [HttpPost("ExportDanhSachLoaiGiaDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucLoaiGiaDichVu)]
        public async Task<ActionResult> ExportChucDanh(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _loaiGiaDichVuService.GetDataForGridAsync(queryInfo);
            var chucDanhData = gridData.Data.Select(p => (LoaiGiaDichVuGridVo)p).ToList();
            var excelData = chucDanhData.Map<List<LoaiGiaDichVuExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LoaiGiaDichVuExportExcel.TenNhom), "Nhóm dịch vụ"));
            lstValueObject.Add((nameof(LoaiGiaDichVuExportExcel.Ten), "Tên"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Loại giá");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LoaiGiaDichVu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
