using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamDoan;
using Camino.Api.Models.KhamDoanCongTies;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataListCongTyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanCongTy)]
        public async Task<ActionResult<GridDataSource>> GetDataListCongTyForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataListCongTyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageListCongTyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanCongTy)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageListCongTyForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageListCongTyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanCongTy)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhamDoanCongTyViewModel>> Get(long id)
        {
            var khamDoanCongTy = await _khamDoanService.GetByCongTyIdAsync(id);

            if (khamDoanCongTy == null)
            {
                return NotFound();
            }

            return Ok(khamDoanCongTy.ToModel<KhamDoanCongTyViewModel>());
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanCongTy)]
        public async Task<ActionResult<KhamDoanCongTyViewModel>> Post
            ([FromBody]KhamDoanCongTyViewModel khamDoanCongTyViewModel)
        {
            var khamDoanCongTy = khamDoanCongTyViewModel.ToEntity<CongTyKhamSucKhoe>();
            await _khamDoanService.AddCongTyAsync(khamDoanCongTy);
            var khamDoanCongTyId = await _khamDoanService.GetByCongTyIdAsync(khamDoanCongTy.Id);
            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = khamDoanCongTy.Id },
                khamDoanCongTyId.ToModel<KhamDoanCongTyViewModel>()
            );
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanCongTy)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Update([FromBody]KhamDoanCongTyViewModel khamDoanCongTyViewModel)
        {
            var khamDoanCongTy = await _khamDoanService.GetByCongTyIdAsync(khamDoanCongTyViewModel.Id);
            khamDoanCongTyViewModel.ToEntity(khamDoanCongTy);
            await _khamDoanService.UpdateCongTyAsync(khamDoanCongTy);
            return NoContent();
        }

        [HttpDelete("XuLyXoaCongTy")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanCongTy)]
        public async Task<ActionResult> DeleteCongTy(long id)
        {
            var khamDoanCongTy = await _khamDoanService.GetByCongTyIdAsync(id, w => w.Include(q => q.HopDongKhamSucKhoes));
            if (khamDoanCongTy == null)
            {
                return NotFound();
            }

            if (khamDoanCongTy.HopDongKhamSucKhoes.Any())
            {
                throw new ApiException(_localizationService.GetResource("CongTy.HasHopDongKhamSucKhoe"), (int)HttpStatusCode.BadRequest);
            }

            await _khamDoanService.DeleteByCongTyIdAsync(id);
            return NoContent();
        }


        [HttpPost("ExportDanhSachCongTy")]  
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult> ExportDanhSachCongTy(QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataListCongTyForGridAsync(queryInfo , true);

            var congTyDatas = gridData.Data.Select(p => (KhamDoanCongTyGridVo)p).ToList();
            var excelData = congTyDatas.Map<List<CongTyKhamExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(CongTyKhamExportExcel.MaCongTy), "Mã công ty"),
                (nameof(CongTyKhamExportExcel.TenCongTy), "Tên công ty"),
                (nameof(CongTyKhamExportExcel.LoaiCongTy), "Loại công ty"),
                (nameof(CongTyKhamExportExcel.DiaChi), "Địa chỉ"),
                (nameof(CongTyKhamExportExcel.DienThoai), "Điện thoại"),
                (nameof(CongTyKhamExportExcel.MaSoThue), "Mã số thuế"),
                (nameof(CongTyKhamExportExcel.TaiKhoanNganHang), "TK NH"),
                (nameof(CongTyKhamExportExcel.DaiDien), "Đại diện"),
                (nameof(CongTyKhamExportExcel.NguoiLienHe), "Người liên hệ"),
                (nameof(CongTyKhamExportExcel.TrangThai), "Trạng thái"),
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Công Ty", 2, "DS Công Ty");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSHopDongKham" + DateTime.Now.Year + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}
