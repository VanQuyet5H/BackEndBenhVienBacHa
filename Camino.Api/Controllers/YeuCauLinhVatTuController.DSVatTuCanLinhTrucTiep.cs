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
using Newtonsoft.Json;


namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepVatTu)] 
        public ActionResult<GridDataSource> GetDanhSachVatTuCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauLinhVatTuService.GetDanhSachVatTuCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietVatTuCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepVatTu)] 
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietVatTuCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData =await _yeuCauLinhVatTuService.GetDanhSachChiTietVatTuCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietVatTuCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietVatTuCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageDanhSachChiTietVatTuCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep")]
        public ActionResult<List<LookupItemVo>> GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhVatTuService.GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(model);
            return Ok(lookup);
        }
        [HttpPost("GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep")]
        public ActionResult<List<LookupItemVo>> GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhVatTuService.GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(model);
            return Ok(lookup);
        }
    }
}
