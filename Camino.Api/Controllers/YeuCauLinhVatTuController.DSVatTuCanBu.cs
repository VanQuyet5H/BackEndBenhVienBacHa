using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DanhSachVatTuCanBu;
using Newtonsoft.Json;


namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)] 
        public ActionResult<GridDataSource> GetDanhSachVatTuCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauLinhVatTuService.GetDanhSachVatTuCanBuForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietVatTuCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)] 
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietVatTuCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData =await _yeuCauLinhVatTuService.GetDanhSachChiTietVatTuCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietVatTuCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietVatTuCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageDanhSachChiTietVatTuCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietYeuCauTheoVatTuCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietYeuCauTheoVatTuCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDanhSachChiTietYeuCauTheoVatTuCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanBuForGrid(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTatCakhoLinhTuCuaNhanVienLoginLinhBu")]
        public ActionResult<List<LookupItemVo>> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhVatTuService.GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(model);
            return Ok(lookup);
        }
        [HttpPost("GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu")]
        public ActionResult<List<LookupItemVo>> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhVatTuService.GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(model);
            return Ok(lookup);
        }

        [HttpPost("KhongBuVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult> KhongBuVatTu(string yeuCauLinhVatTuIdstring)
        {
            await _yeuCauLinhVatTuService.UpdateYeuCauVatTuBenhVien(yeuCauLinhVatTuIdstring);
            return NoContent();
        }
    }
}
