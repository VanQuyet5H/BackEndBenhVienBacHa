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
        [HttpPost("GetNguoiBenhTheoNoiGioiThieu")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNguoiBenhTheoNoiGioiThieuAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetNguoiBenhTheoNoiGioiThieuAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoBangKeChiTietTheoNguoiBenhForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBangKeChiTietTheoNguoiBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetTotalPageBaoCaoBangKeChiTietTheoNguoiBenhForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBangKeChiTietTheoNguoiBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalPageBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBangKeChiTietTheoNguoiBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBangKeChiTietTheoNguoiBenh)]
        public async Task<ActionResult> ExportBaoCaoBangKeChiTietTheoNguoiBenh([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBangKeChiTietTheoNguoiBenh(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBangKeChiTietTheoNguoiBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetTongCongBangKeChiTietTheoNguoiBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBangKeChiTietTheoNguoiBenh)]
        public async Task<ActionResult<BaoCaoBangKeChiTietTheoNguoiBenhGridVo>> GetTongCongBangKeChiTietTheoNguoiBenhAsync(QueryInfo queryInfo)
        {
            var tongCong = new BaoCaoBangKeChiTietTheoNguoiBenhGridVo();

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(queryInfo);
            var datas = (ICollection<BaoCaoBangKeChiTietTheoNguoiBenhGridVo>)gridData.Data;

            tongCong.TongCongChiPhi = datas.Sum(x => x.ThanhTien);
            tongCong.TongChiPhiGiaTonKho = datas.Sum(x => x.TongCongGiaTonKho ?? 0);
            return Ok(tongCong);
        }

        [HttpPost("GetMaYeuCauTiepNhanTheoHinhThucDen")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetMaYeuCauTiepNhanTheoHinhThucDenAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetMaYeuCauTiepNhanTheoHinhThucDenAsync(queryInfo);
            return Ok(result);
        }
    }
}
