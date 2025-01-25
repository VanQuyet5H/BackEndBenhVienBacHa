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
        [HttpPost("GetTatCakhoaNhapXuatTonLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCakhoaNhapXuatTonLookupAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetTatCakhoaNhapXuatTonLookupAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoKTNhapXuatTonForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoNhapXuatTon)]
        public async Task<ActionResult> GetDataBaoCaoKTNhapXuatTonForGridAsync(BaoCaoKTNhapXuatTonQueryInfo queryInfo)
        {
            return Ok();
        }

        [HttpPost("ExportBaoCaoKTNhapXuatTon")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoNhapXuatTon)]
        public async Task<ActionResult> ExportBaoCaoKTNhapXuatTon(BaoCaoKTNhapXuatTonQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKTNhapXuatTonForGridAsync(queryInfo);
            byte[] bytes = null;

            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoKTNhapXuatTon(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=tBaoCaoKTNhapXuatTon" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetDataBaoCaoKeToanNhapXuatTonForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoNhapXuatTon)]
        public async Task<ActionResult> GetDataBaoCaoKeToanNhapXuatTonForGridAsync(BaoCaoKTNhapXuatTonQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKTNhapXuatTonForGridAsync(queryInfo);

            var baoCaoKTNhapXuatTonVo = new BaoCaoKTNhapXuatTonVo();
            var baoCaoKTNhapXuatTonGridVos = new List<BaoCaoKTNhapXuatTonGridVo>();


            if (gridData.Data != null)
            {
                var datas = gridData.Data.Cast<BaoCaoKTNhapXuatTonGridVo>().ToList();
                baoCaoKTNhapXuatTonVo.DataSumPageTotal = datas;

                var listNhom = datas.GroupBy(s => new { s.Nhom }).Select(s => new NhomGroupBaoCaoKTNhapXuatTonVo
                {
                    Nhom = s.First().Nhom
                }).OrderBy(p => p.Nhom).ToList();


                foreach (var item in listNhom)
                {
                    baoCaoKTNhapXuatTonGridVos.AddRange(datas.Where(d=>d.Nhom == item.Nhom).ToList());
                }

                baoCaoKTNhapXuatTonVo.TotalRowCount = baoCaoKTNhapXuatTonGridVos.Count();
                baoCaoKTNhapXuatTonVo.Data = baoCaoKTNhapXuatTonGridVos.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
                baoCaoKTNhapXuatTonVo.ListGroupTheoFileExecls = baoCaoKTNhapXuatTonGridVos.Skip(queryInfo.Skip).Take(queryInfo.Take).GroupBy(s => new { s.Nhom }).Select(s => new NhomGroupBaoCaoKTNhapXuatTonVo
                {
                    Nhom = s.First().Nhom
                }).OrderBy(p => p.Nhom).ToList();
            }

            
            return Ok(baoCaoKTNhapXuatTonVo);
        }

        [HttpPost("GetTotalSumGetDataBaoCaoKeToanNhapXuatTonForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoNhapXuatTon)]
        public async Task<ActionResult> GetTotalSumGetDataBaoCaoKeToanNhapXuatTonForGridAsync(BaoCaoKTNhapXuatTonQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKTNhapXuatTonForGridAsync(queryInfo);
            return Ok(gridData);
        }
    }
}
