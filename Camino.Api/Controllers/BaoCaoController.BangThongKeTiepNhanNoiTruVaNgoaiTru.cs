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
        [HttpPost("GetTatKhoaChoTiepNhanNoiVaNgoaiTrus")]
        public ActionResult<ICollection<LookupItemVo>> GetTatKhoaChoTiepNhanNoiVaNgoaiTrus(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatKhoaChoTiepNhanNoiVaNgoaiTru(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetLoaiYeuCauTiepNhanNoiTruVaNgoaiTru")]
        public List<LookupItemVo> GetLoaiYeuCauTiepNhanNoiTruVaNgoaiTru()
        {
            var loaiYeuCauTiepNhans = EnumHelper.GetListEnum<LoaiYeuCauTiepNhan>();
            var res = loaiYeuCauTiepNhans.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return res;
        }

        [HttpPost("GetTrangThaiDieuTri")]
        public List<LookupItemVo> GetTrangThaiDieuTri(DropDownListRequestModel queryInfo)
        {
            var loaiYeuCauTiepNhan =  JsonConvert.DeserializeObject<LoaiYeuCauTiepNhanJson>(queryInfo.ParameterDependencies);
            var trangThaiDieuTris = EnumHelper.GetListEnum<TrangThaiDieuTri>();
            if (loaiYeuCauTiepNhan.LoaiYeuCauTiepNhan == LoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
            {
                var res = trangThaiDieuTris.Where(x => x == TrangThaiDieuTri.TatCa).Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                }).ToList();

                return res;
            }
            else
            {
                var res = trangThaiDieuTris.Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                }).ToList();

                return res;
            }


        }

        [HttpPost("GetDataBangThongKeTiepNhanNoiTruVaNgoaiTruForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BangThongKeTiepNhanNoiTruVaNgoaiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataBangThongKeTiepNhanNoiTruVaNgoaiTruForGrid(BangThongKeTiepNhanNoiTruVaNgoaiTruQueryInfoVo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBangThongKeTiepNhanNoiTruVaNgoaiTruForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBangThongKeTiepNhanNoiTruVaNgoaiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BangThongKeTiepNhanNoiTruVaNgoaiTru)]
        public async Task<ActionResult> ExportBangThongKeTiepNhanNoiTruVaNgoaiTru([FromBody] BangThongKeTiepNhanNoiTruVaNgoaiTruQueryInfoVo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBangThongKeTiepNhanNoiTruVaNgoaiTruForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBangThongKeTiepNhanNoiTruVaNgoaiTru(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangThongKeTiepNhanNoiTruVaNgoaiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
