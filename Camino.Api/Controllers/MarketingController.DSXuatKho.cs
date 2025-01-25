using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.QuaTang;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Marketing;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class MarketingController
    {
        [HttpPost("GetDSXuatKhoMarketingDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDSXuatKhoMarketingDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoQuaTangService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDSXuatKhoMarketingTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDSXuatKhoMarketingTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoQuaTangService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDSQuaTangMarketingDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoQuaTangService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDSQuaTangMarketingTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoQuaTangService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetNhanVienXuatMarketing")]
        public async Task<ActionResult> GetNhanVienXuatMarketing([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _xuatKhoQuaTangService.GetNhanVienXuat(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetNguoiNhanMarketing")]
        public async Task<ActionResult> GetNguoiNhanMarketing([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _xuatKhoQuaTangService.GetNguoiNhan(queryInfo);
            return Ok(lookup);
        }


        [HttpGet("GetXuatKhoMarketingChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoMarketing)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<XuatKhoQuaTangMarketingViewModel>> Get(long id)
        {
            var quaTang = await _xuatKhoQuaTangService.GetByIdAsync(id, x => 
                                                                        x.Include(p => p.BenhNhan)
                                                                         .Include(p=>p.NguoiXuat).ThenInclude(nx=>nx.User)
                                                                         .Include(p=>p.XuatKhoQuaTangChiTiet)
                                                                        );
            if (quaTang == null)
            {
                return NotFound();
            }
            var viewModel = quaTang.ToModel<XuatKhoQuaTangMarketingViewModel>();
            viewModel.KhoXuatId = 1;
            viewModel.TenKhoXuat = "Kho Marketing";
            return Ok(viewModel);
        }

        [HttpPost("ExportXuatKhoMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.XuatKhoMarketing)]
        public async Task<ActionResult> ExportXuatKhoMarketing(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoQuaTangService.GetDataForGridAsync(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (XuatKhoQuaTangMarketingGridVo)p).ToList();
            var excelData = chucVuData.Map<List<XuatKhoMarketingExporExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(XuatKhoMarketingExporExcel.SoPhieu), "Số PX"),
                (nameof(XuatKhoMarketingExporExcel.NoiXuat), "Nơi xuất"),
                (nameof(XuatKhoMarketingExporExcel.NhanVienXuat), "Người xuất"),
                (nameof(XuatKhoMarketingExporExcel.NgayXuatDisplay), "Ngày xuất"),
                (nameof(XuatKhoMarketingExporExcel.NguoiNhan), "Người nhận"),
                (nameof(XuatKhoMarketingExporExcel.GhiChu), "Lý Do xuất")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Xuất kho marketing");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatKhoMarketing" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
