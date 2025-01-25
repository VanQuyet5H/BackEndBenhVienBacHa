using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanBu;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuocPhamCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)] 
        public ActionResult<GridDataSource> GetDanhSachDuocPhamCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauLinhDuocPhamService.GetDanhSachDuocPhamCanBuForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietDuocPhamCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)] 
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietDuocPhamCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData =await _yeuCauLinhDuocPhamService.GetDanhSachChiTietDuocPhamCanBuForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietDuocPhamCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietDuocPhamCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanBuQueryInfo>(queryInfo.AdditionalSearchString);
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageDanhSachChiTietDuocPhamCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanBuChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTatCakhoLinhTuCuaNhanVienLoginLinhBu")]
        public ActionResult<List<LookupItemVo>> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(model);
            return Ok(lookup);
        }
        [HttpPost("GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu")]
        public ActionResult<List<LookupItemVo>> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(model);
            return Ok(lookup);
        }

        [HttpPost("KhongBuDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult> KhongBuDuocPham(string yeuCauLinhDuocPhamIdstring)
        {
            await _yeuCauLinhDuocPhamService.UpdateYeuCauDuocPhamBenhVien(yeuCauLinhDuocPhamIdstring);
            return NoContent();
        }
    }
}
