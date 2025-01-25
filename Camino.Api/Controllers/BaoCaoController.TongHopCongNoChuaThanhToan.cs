using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetDataBaoCaoTongHopCongNoChuaThanhToanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTongHopCongNoChuaThanhToan)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoTongHopCongNoChuaThanhToanForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoTongHopCongNoChuaThanhToanForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetTotalPageBaoCaoTongHopCongNoChuaThanhToanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTongHopCongNoChuaThanhToan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoTongHopCongNoChuaThanhToanForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalPageBaoCaoTongHopCongNoChuaThanhToanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoTongHopCongNoChuaThanhToan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopCongNoChuaThanhToan)]
        public async Task<ActionResult> ExportBaoCaoTongHopCongNoChuaThanhToan([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopCongNoChuaThanhToanForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTongHopCongNoChuaThanhToan(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopCongNoChuaThanhToan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetNoiGioiThieuDaCoNguoiBenhCoTatCa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNoiGioiThieuDaCoNguoiBenhCoTatCaAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetNoiGioiThieuDaCoNguoiBenhAsync(queryInfo, true);
            if (result.Count > 0)
            {
                result.Insert(0, new LookupItemTemplateVo()
                {
                    KeyId = 0,
                    DisplayName = "Tất cả",
                    Ten = "Tất cả"
                });
            }
            
            return Ok(result);
        }

        [HttpPost("GetTongCongCongNoChuaThanhToan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopCongNoChuaThanhToan)]
        public async Task<ActionResult<BaoCaoTongHopCongNoChuaThanhToanGridVo>> GetTongCongCongNoChuaThanhToanAsync(QueryInfo queryInfo)
        {
            var tongCong = new BaoCaoTongHopCongNoChuaThanhToanGridVo();

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopCongNoChuaThanhToanForGridAsync(queryInfo);
            var datas = (ICollection<BaoCaoTongHopCongNoChuaThanhToanGridVo>)gridData.Data;

            tongCong.ChiPhiCanLamSan = datas.Sum(x => x.ChiPhiCanLamSan ?? 0);
            tongCong.ChiPhiCanLamSanNgoaiTru = datas.Sum(x => x.ChiPhiCanLamSanNgoaiTru ?? 0);
            tongCong.ChiPhiCanLamSanNoiTru = datas.Sum(x => x.ChiPhiCanLamSanNoiTru ?? 0);
            tongCong.ChiPhiChuaThucHien = datas.Sum(x => x.ChiPhiChuaThucHien ?? 0);
            tongCong.ChiPhiGiuong = datas.Sum(x => x.ChiPhiGiuong ?? 0);
            tongCong.ChiPhiThuocVTYT = datas.Sum(x => x.ChiPhiThuocVTYT ?? 0);
            tongCong.ChiPhiThuoc = datas.Sum(x => x.ChiPhiThuoc ?? 0);
            tongCong.ChiPhiVTYT = datas.Sum(x => x.ChiPhiVTYT ?? 0);
            tongCong.TestCovid = datas.Sum(x => x.TestCovid ?? 0);
            tongCong.SuatAn = datas.Sum(x => x.SuatAn ?? 0);
            tongCong.NguoiBenhDaThanhToan = datas.Sum(x => x.NguoiBenhDaThanhToan ?? 0);
            tongCong.ChiPhiCaPhauThuat = datas.Sum(x => x.ChiPhiCaPhauThuat ?? 0);

            tongCong.ChiPhiThuePhongMo = datas.Sum(x => x.ChiPhiThuePhongMo ?? 0);
            tongCong.GiamDau = datas.Sum(x => x.GiamDau ?? 0);
            tongCong.CongNoChuaThanhToan = datas.Sum(x => x.CongNoChuaThanhToan);
            return Ok(tongCong);
        }

        [HttpPost("GetHinhThucDenCoTatCa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetHinhThucDenCoTatCaAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetHinhThucDenCoTatCaAsync(queryInfo);

            if (result.Count > 0)
            {
                result.Insert(0, new LookupItemTemplateVo()
                {
                    KeyId = 0,
                    DisplayName = "Tất cả",
                    Ten = "Tất cả"
                });
            }
            
            return Ok(result);
        }

        [HttpPost("GetMaYeuCauTiepNhanTheoHinhThucDenCoTatCa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetMaYeuCauTiepNhanTheoHinhThucDenCoTatCaAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetMaYeuCauTiepNhanTheoHinhThucDenAsync(queryInfo);
            if (result.Count > 0)
            {
                result.Insert(0, new MaTiepNhanTheoHinhThucDenLookupItemVo()
                {
                    KeyId = "0",
                    DisplayName = "Tất cả",
                    MaNguoiBenh = "Tất cả",
                    MaYeuCauTiepNhan = "Tất cả",
                    TenNguoiBenh = "Tất cả"
                });
            }
            return Ok(result);
        }
    }
}
