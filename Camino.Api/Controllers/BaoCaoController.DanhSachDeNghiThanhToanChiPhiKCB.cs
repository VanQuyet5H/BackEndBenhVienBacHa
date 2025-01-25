using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachDeNghiThanhToanChiPhiKCB;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetMauBaoBaoBHYT")]
        public Task<List<LookupItemVo>> GetThongTinMauBaoCao()
        {
            var listEnum = EnumHelper.GetListEnum<MauBaoBaoBHYT>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }

        [HttpPost("GetHinhThucXuatBaoBao")]
        public Task<List<LookupItemVo>> GetThongTinHinhThucXuatBaoCao()
        {
            var listEnum = EnumHelper.GetListEnum<HinhThucXuatBaoBao>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }

        [HttpPost("GetDanhSachDeNghiThanhToanChiPhiKCBNgoaiTruForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GiamDinhBHYT7980aXuatChoKeToan)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDeNghiThanhToanChiPhiKCBNgoaiTruForGrid(DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo queryInfo)
        {
            if (queryInfo.MauBaoBaoBHYT == MauBaoBaoBHYT.BaoCao79)
            {
                var grid = await _baoCaoService.GetDanhSachDeNghiThanhToanChiPhiKCBNgoaiTruForGrid(queryInfo);
                return Ok(grid);
            }
            else
            {
                var grid = await _baoCaoService.GetDanhSachDeNghiThanhToanChiPhiKCBNoiTruForGrid(queryInfo);
                return Ok(grid);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportDanhSachDeNghiThanhToanChiPhiKCBNgoaiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.GiamDinhBHYT7980aXuatChoKeToan)]
        public async Task<ActionResult> ExportDanhSachDeNghiThanhToanChiPhiKCBNgoaiTru([FromBody] DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo queryInfo)
        {
            if (queryInfo.MauBaoBaoBHYT == MauBaoBaoBHYT.BaoCao79)
            {
                var gridData = await _baoCaoService.GetDanhSachDeNghiThanhToanChiPhiKCBNgoaiTruForGrid(queryInfo);

                byte[] bytes = null;
                if (gridData != null)
                {
                    bytes = _baoCaoService.ExportDanhSachDeNghiThanhToanChiPhiKCBNgoaiTru(gridData, queryInfo);
                }

                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                return new FileContentResult(bytes, "application/vnd.ms-excel");
            }
            else
            {
                var gridData = await _baoCaoService.GetDanhSachDeNghiThanhToanChiPhiKCBNoiTruForGrid(queryInfo);

                byte[] bytes = null;
                if (gridData != null)
                {
                    bytes = _baoCaoService.ExportDanhSachDeNghiThanhToanChiPhiKCBNoiTru(gridData, queryInfo);
                }

                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDeNghiThanhToanChiPhiKCBNoiTru" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                return new FileContentResult(bytes, "application/vnd.ms-excel");
            }
        }

    }
}
