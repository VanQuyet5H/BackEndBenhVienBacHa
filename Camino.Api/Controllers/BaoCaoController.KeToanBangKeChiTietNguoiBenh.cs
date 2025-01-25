using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoKeToanBangKeChiTietNguoiBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetTotalPageBaoCaoKeToanBangKeChiTietNguoiBenhForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoKeToanBangKeChiTietNguoiBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync(queryInfo, true);
            return Ok(gridData);
        }

        [HttpPost("GetTongCongBangKeChiTietNguoiBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKeToanBangKeChiTietNguoiBenh)]
        public async Task<ActionResult<BaoCaoKeToanBangKeChiTietNguoiBenhTongTienVo>> GetTongCongBangKeChiTietNguoiBenhAsync(QueryInfo queryInfo)
        {
            var tongCong = new BaoCaoKeToanBangKeChiTietNguoiBenhTongTienVo();

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync(queryInfo);
            var datas = (ICollection<BaoCaoKeToanBangKeChiTietNguoiBenhGridVo>)gridData.Data;

            tongCong.TongTienDonGiaNhapKho = datas.Sum(x => x.DonGiaNhapKho ?? 0);
            tongCong.TongTienDonGiaBan = datas.Sum(x => x.DonGiaBanThucTe ?? 0);
            tongCong.TongTienMienGiam = datas.Sum(x => x.MienGiam ?? 0);
            tongCong.TongTienThanhTienBan = datas.Sum(x => x.ThanhTienBan ?? 0);
            return Ok(tongCong);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoKeToanBangKeChiTietNguoiBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKeToanBangKeChiTietNguoiBenh)]
        public async Task<ActionResult> ExportBaoCaoKeToanBangKeChiTietNguoiBenh([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoKeToanBangKeChiTietNguoiBenh(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBangKeChiTietNguoiBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
