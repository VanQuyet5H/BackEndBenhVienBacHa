using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenefitInsurance;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridXacNhanNoiTruVaNgoaiTruBHYTAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DSXNNgoaiTruVaNoiTruBHYT)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridXacNhanNoiTruVaNgoaiTruBHYTAsync([FromBody]QueryInfo queryInfo)
        {
            var filterDanhSachBHYTNoiTruVaNgoaiTruGridVo = new FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                filterDanhSachBHYTNoiTruVaNgoaiTruGridVo = JsonConvert.DeserializeObject<FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.FromDate) || !string.IsNullOrEmpty(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ToDate))
                {
                    DateTime denNgay;

                    filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.FromDate.TryParseExactCustom(out var tuNgay);

                    if (string.IsNullOrEmpty(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ToDate.TryParseExactCustom(out denNgay);

                    }

                    filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ThoiDiemTiepNhanTu = tuNgay;
                    filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ThoiDiemTiepNhanDen = denNgay;
                }
            }

            if (filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.NgoaiTru == null && filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.NoiTru == null)
            {
                filterDanhSachBHYTNoiTruVaNgoaiTruGridVo = new FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo
                {
                    NgoaiTru = true,
                    NoiTru = true
                };
            }

            var gridDataAll = await _xacNhanNoiTruVaNgoaiTruBHYTService.GetDataXacNhanNoiTruVaNgoaiTruHoanThanh(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo, false);

            return Ok(gridDataAll);
        }
      
        [HttpPost("ExportNoiTruVaNgoaiTruBHYTHoanThanh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DSXNNgoaiTruVaNoiTruBHYT)]
        public async Task<ActionResult> ExportNoiTruVaNgoaiTruBHYTHoanThanh([FromBody]QueryInfo queryInfo)
        {
            var filterDanhSachBHYTNoiTruVaNgoaiTruGridVo = new FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                filterDanhSachBHYTNoiTruVaNgoaiTruGridVo = JsonConvert.DeserializeObject<FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.FromDate) || !string.IsNullOrEmpty(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ToDate))
                {
                    DateTime denNgay;

                    filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.FromDate.TryParseExactCustom(out var tuNgay);

                    if (string.IsNullOrEmpty(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ToDate.TryParseExactCustom(out denNgay);

                    }

                    filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ThoiDiemTiepNhanTu = tuNgay;
                    filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.ThoiDiemTiepNhanDen = denNgay;
                }
            }

            if (filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.NgoaiTru == null && filterDanhSachBHYTNoiTruVaNgoaiTruGridVo.NoiTru == null)
            {
                filterDanhSachBHYTNoiTruVaNgoaiTruGridVo = new FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo
                {
                    NgoaiTru = true,
                    NoiTru = true
                };
            }

            byte[] bytes = null;
            var gridDataAll = await _xacNhanNoiTruVaNgoaiTruBHYTService.GetDataXacNhanNoiTruVaNgoaiTruHoanThanh(filterDanhSachBHYTNoiTruVaNgoaiTruGridVo, true);
            if (gridDataAll != null)
            {
                bytes = _xacNhanNoiTruVaNgoaiTruBHYTService.ExportXacNhanNoiTruVaNgoaiTruHoanThanh(gridDataAll, filterDanhSachBHYTNoiTruVaNgoaiTruGridVo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NoiTruVaNgoaiTruBHYTHoanThanh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}
