using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuocPhamCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)] 
        public ActionResult<GridDataSource> GetDanhSachDuocPhamCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauLinhDuocPhamService.GetDanhSachDuocPhamCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)] 
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData =await _yeuCauLinhDuocPhamService.GetDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep")]
        public ActionResult<List<LookupItemVo>> GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(model);
            return Ok(lookup);
        }

        [HttpPost("GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep")]
        public ActionResult<List<LookupItemVo>> GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(model);
            return Ok(lookup);
        }
    }
}
