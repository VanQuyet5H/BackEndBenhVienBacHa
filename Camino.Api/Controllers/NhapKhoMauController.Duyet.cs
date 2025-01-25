using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.NhapKhoMau;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class NhapKhoMauController
    {
        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDuyetNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDuyetNhapKhoMauAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetDataForGridNhapKhoMauAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDuyetNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDuyetNhapKhoMauAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetTotalPageForGridNhapKhoMauAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDuyetNhapKhoMauChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDuyetNhapKhoMauChiTietAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetDataForGridNhapKhoMauChiTietAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDuyetNhapKhoMauChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDuyetNhapKhoMauChiTietAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetTotalPageForGridNhapKhoMauChiTietAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion

        #region Duyệt
        [HttpGet("GetDuyetPhieuNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoMau)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PhieuNhapKhoMauViewModel>> GetDuyetPhieuNhapKhoMauAsync(long id)
        {
            var result = await _nhapKhoMauService.GetPhieuNhapKhoMauAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result.ToModel<DuyetPhieuNhapKhoMauViewModel>());
        }

        [HttpPut("XuLyNhapGiaMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoMau)]
        public async Task<ActionResult> XuLyNhapGiaMauAsync(DuyetPhieuNhapKhoMauViewModel phieuNhapMau)
        {
            var phieuNhapKhoMau =
                await _nhapKhoMauService.GetByIdAsync(phieuNhapMau.Id, x => x.Include(a => a.NhapKhoMauChiTiets).ThenInclude(b => b.YeuCauTruyenMau));
            var isChuaDuyet = phieuNhapKhoMau.DuocKeToanDuyet == null;
            phieuNhapMau.ToEntity(phieuNhapKhoMau);

            foreach (var phieuNhapChiTiet in phieuNhapKhoMau.NhapKhoMauChiTiets)
            {
                phieuNhapChiTiet.YeuCauTruyenMau.DonGiaNhap = phieuNhapChiTiet.DonGiaNhap;
                phieuNhapChiTiet.YeuCauTruyenMau.DonGiaBan = phieuNhapChiTiet.DonGiaBan;
                phieuNhapChiTiet.YeuCauTruyenMau.DonGiaBaoHiem = phieuNhapChiTiet.DonGiaBaoHiem;
            }

            if (isChuaDuyet)
            {
                _nhapKhoMauService.XuLyXuatKhoMau(phieuNhapKhoMau);
            }

            await _nhapKhoMauService.UpdateAsync(phieuNhapKhoMau);
            return Ok();
        }

        #endregion
    }
}
