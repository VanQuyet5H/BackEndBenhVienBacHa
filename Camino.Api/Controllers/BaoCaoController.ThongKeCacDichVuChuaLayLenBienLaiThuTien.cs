using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeCacDichVuChuaLayLenBienLaiThuTien)]
        public async Task<ActionResult<GridDataSource>> GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetTotalPageThongKeCacDVChuaLayLenBienLaiThuTienForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeCacDichVuChuaLayLenBienLaiThuTien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageThongKeCacDVChuaLayLenBienLaiThuTienForGrid(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetTotalPageThongKeCacDVChuaLayLenBienLaiThuTienForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportThongKeCacDichVuChuaLayLenBienLaiThuTien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.ThongKeCacDichVuChuaLayLenBienLaiThuTien)]
        public async Task<ActionResult> ExportThongKeCacDichVuChuaLayLenBienLaiThuTien([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportThongKeCacDichVuChuaLayLenBienLaiThuTien(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThongKeCacDichVuChuaLayLenBienLaiThuTien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetTongCongBaoCaoThongKeCacDichVuChuaLayLenBienLaiThuTien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeCacDichVuChuaLayLenBienLaiThuTien)]
        public async Task<ActionResult<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>> GetTongCongBaoCaoThongKeCacDichVuChuaLayLenBienLaiThuTien(QueryInfo queryInfo)
        {
            var tongCong = new ThongTinTiepNhanChuaThuTienCoHinhThucDenVo();

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid(queryInfo);
            var datas = (ICollection<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>)gridData.Data;
            
            tongCong.TongTienChuaThanhToan = datas.Sum(x => x.TongTienChuaThanhToan);
            return Ok(tongCong);
        }
    }
}
