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
        [HttpPost("GetTatKhoaChoTiaPlasmaHoTroDieuTriVetThuong")]
        public ActionResult<ICollection<LookupItemVo>> GetTatKhoaChoTiaPlasmaHoTroDieuTriVetThuong(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatKhoaTiaPlasmaHoTroVetThuong(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDichVuChoTiaPlasMaHoTroDTVetThuong")]
        public async Task<ActionResult<ICollection<LookupItemTongHopDichVuVo>>> GetDichVuChoTiaPlasMaHoTroDTVetThuong(DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetTongHopDichVuBaoCaoAsync(queryInfo);
            return Ok(result);
        }


        [HttpPost("GetDataBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetTotalPageBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetTotalPageBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong)]
        public async Task<ActionResult> ExportBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDichVuDaThucHien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetTongCongBaoCaoDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong)]
        public async Task<ActionResult<DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong>> GetTongCongBaoCaoDichVuAsync(QueryInfo queryInfo)
        {
            var tongCong = new DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong();

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(queryInfo);
            var datas = (ICollection<DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong>)gridData.Data;

            tongCong.TongCongSoLuong = datas.Sum(x => x.SoLuong);
            tongCong.TongCongThanhToan = datas.Sum(x => x.ThanhToan);
            return Ok(tongCong);
        }
    }
}
