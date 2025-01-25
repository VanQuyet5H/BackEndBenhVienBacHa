using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhBuKSNK;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhKSNKController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachKSNKCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public ActionResult<GridDataSource> GetDanhSachKSNKCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauLinhKSNKService.GetDanhSachKSNKCanBuForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietKSNKCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietKSNKCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDanhSachChiTietKSNKCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietKSNKCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietKSNKCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetTotalPageDanhSachChiTietKSNKCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChiTietYeuCauTheoKSNKCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChiTietYeuCauTheoKSNKCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDanhSachChiTietYeuCauTheoKSNKCanBuForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChiTietYeuCauTheoKSNKCanBuForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChiTietYeuCauTheoKSNKCanBuForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetTotalPageDanhSachChiTietYeuCauTheoKSNKCanBuForGrid(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTatCakhoLinhTuCuaNhanVienLoginLinhBu")]
        public ActionResult<List<LookupItemVo>> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhKSNKService.GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(model);
            return Ok(lookup);
        }
        [HttpPost("GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu")]
        public ActionResult<List<LookupItemVo>> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var lookup = _yeuCauLinhKSNKService.GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(model);
            return Ok(lookup);
        }

        [HttpPost("KhongBuKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult> KhongBuKSNK(string yeuCauLinhVatTuIdstring)
        {
            var vO = JsonConvert.DeserializeObject<List<KhongYeuCauLinhBuKSNKVo>>(yeuCauLinhVatTuIdstring);
            await _yeuCauLinhKSNKService.UpdateYeuCauKSNKBenhVien(vO);
            return NoContent();
        }
    }
}
