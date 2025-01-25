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
        [HttpPost("GetTatKhoaChoTiepNhanNoiTrus")]
        public ActionResult<ICollection<LookupItemVo>> GetTatKhoaChoTiepNhanNoiTrus(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatKhoaChoTiepNhanNoiTru(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoBenhNhanRaVienNoiTruForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBenhNhanRaVienNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoBenhNhanRaVienNoiTruForGrid(BaoCaoRaVienNoiTruQueryInfoVo queryInfo)
        {
            queryInfo.LoaiYeuCauTiepNhan = LoaiYeuCauTiepNhanNoiTru.KhamChuaBenhNoiTru;
            queryInfo.TrangThaiDieuTri = TrangThaiDieuTriNoiTru.DaRaVien;

            var grid = await _baoCaoService.GetDataBaoCaoBenhNhanRaVienNoiTruForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBenhNhanRaVienNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBenhNhanRaVienNoiTru)]
        public async Task<ActionResult> ExportBaoCaoBenhNhanRaVienNoiTru([FromBody] BaoCaoRaVienNoiTruQueryInfoVo queryInfo)
        {
            queryInfo.LoaiYeuCauTiepNhan = LoaiYeuCauTiepNhanNoiTru.KhamChuaBenhNoiTru;
            queryInfo.TrangThaiDieuTri = TrangThaiDieuTriNoiTru.DaRaVien;

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoBenhNhanRaVienNoiTruForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBenhNhanRaVienNoiTru(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBenhNhanRaVienNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
