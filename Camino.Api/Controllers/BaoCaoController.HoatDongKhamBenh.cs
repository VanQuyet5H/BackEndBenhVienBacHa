using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        #region Theo dịch vụ
        [HttpPost("GetDataBaoCaoHoatDongKhamBenhTheoDichVuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoDichVu)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetTotalPageBaoCaoHoatDongKhamBenhTheoDichVuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoDichVu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalPageBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoHoatDongKhamBenhTheoDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoDichVu)]
        public async Task<ActionResult> ExportBaoCaoHoatDongKhamBenhTheoDichVu([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHoatDongKhamBenhTheoDichVu(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongKhamBenhTheoDichVu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Theo khoa phòng
        [HttpPost("GetDataBaoCaoHoatDongKhamBenhTheoKhoaPhongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoKhoaPhong)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetTotalPageBaoCaoHoatDongKhamBenhTheoKhoaPhongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoKhoaPhong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalPageBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoHoatDongKhamBenhTheoKhoaPhong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoKhoaPhong)]
        public async Task<ActionResult> ExportBaoCaoHoatDongKhamBenhTheoKhoaPhong([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHoatDongKhamBenhTheoKhoaPhong(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongKhamBenhTheoKhoaPhong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        #endregion
    }
}
