using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.XetNghiem;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class XetNghiemController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachNhanMauXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachNhanMauXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachNhanMauXetNghiemForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachNhanMauXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachNhanMauXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalPagesDanhSachNhanMauXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachNhanMauNhomXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachNhanMauNhomXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachNhanMauNhomXetNghiemForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachNhanMauNhomXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachNhanMauNhomXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalPagesDanhSachNhanMauNhomXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachNhanMauDichVuXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachNhanMauDichVuXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachNhanMauDichVuXetNghiemForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachNhanMauDichVuXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachNhanMauDichVuXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalPagesDanhSachNhanMauDichVuXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportNhanMauXetNghiem")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult> ExportNhanMauXetNghiem(QueryInfo queryInfo)
        {
            var gridNhanMauXetNghiemData = await _xetNghiemService.GetDanhSachNhanMauXetNghiemForGrid(queryInfo, true);
            if (gridNhanMauXetNghiemData == null || gridNhanMauXetNghiemData.Data.Count == 0)
            {
                return NoContent();
            }

            var NhanMauXetNghiemData = gridNhanMauXetNghiemData.Data.Cast<NhanMauDanhSachXetNghiemGridVo>().ToList();
            foreach (var nhanMauXetNghiem in NhanMauXetNghiemData)
            {
                var queryNhomXetNghiem = new QueryInfo();
                queryNhomXetNghiem.AdditionalSearchString = queryInfo.AdditionalSearchString.Replace("}", "," + "\"PhieuGoiMauXetNghiemId\":\"" + nhanMauXetNghiem.Id.ToString() + "\" }");

                var dataNhoms = await _xetNghiemService.GetDanhSachNhanMauNhomXetNghiemForGrid(queryNhomXetNghiem, true);
                nhanMauXetNghiem.NhanMauDanhSachNhomXetNghiems = dataNhoms.Data.Cast<NhanMauDanhSachNhomXetNghiemGridVo>().ToList();

                foreach (var nhomXetNghiem in nhanMauXetNghiem.NhanMauDanhSachNhomXetNghiems)
                {
                    var queryDichVuXetNghiem = new QueryInfo();
                    queryDichVuXetNghiem.AdditionalSearchString = "{ PhienXetNghiemId: " + nhomXetNghiem.PhienXetNghiemId + ", NhomDichVuBenhVienId: " + nhomXetNghiem.NhomDichVuBenhVienId + " }";

                    var dataDichVus = await _xetNghiemService.GetDanhSachNhanMauDichVuXetNghiemForGrid(queryDichVuXetNghiem, true);
                    nhomXetNghiem.NhanMauDanhSachDichVuXetNghiems = dataDichVus.Data.Cast<NhanMauDanhSachDichVuXetNghiemGridVo>().ToList();
                }
            }

            var bytes = _excelService.ExportDanhSachNhanMauXetNghiem(NhanMauXetNghiemData, queryInfo.AdditionalSearchString);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhanMauXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetDanhSachKhongTiepNhanMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachKhongTiepNhanMau([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachKhongTiepNhanMau(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("SoLuongMauCoTheTuChoi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<int>> SoLuongMauCoTheTuChoi(long phieuGoiMauXetNghiemId, long nhomDichVuBenhVienId, long phienXetNghiemId)
        {
            var gridData = await _xetNghiemService.SoLuongMauCoTheTuChoi(phieuGoiMauXetNghiemId, nhomDichVuBenhVienId, phienXetNghiemId);
            return Ok(gridData);
        }

        [HttpPost("TiepNhanMau")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> TiepNhanMau(long phieuGoiMauXetNghiemId)
        {
            var nhanVienNhanMauId = _userAgentHelper.GetCurrentUserId();
            await _xetNghiemService.DuyetPhieuGuiMauXetNghiem(phieuGoiMauXetNghiemId, nhanVienNhanMauId);
            return Ok();
        }

        [HttpPost("KhongTiepNhanMau")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult> KhongTiepNhanMau([FromBody] List<KhongTiepNhanMauViewModel> khongTiepNhanMauViewModels)
        {
            var nhanVienXetKhongDatId = _userAgentHelper.GetCurrentUserId();

            foreach (var item in khongTiepNhanMauViewModels)
            {
                await _xetNghiemService.TuChoiMau(item.Id, nhanVienXetKhongDatId, item.LyDoKhongDat);
            }

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("TinhSoLuongMauCoTheTuChoi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.NhanMauXetNghiem, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> TinhSoLuongMauCoTheTuChoi(long phieuGoiMauId)
        {
            var soLuongMauCoTheTuChoi = await _xetNghiemService.TinhSoLuongMauCoTheTuChoi(phieuGoiMauId);
            return Ok(soLuongMauCoTheTuChoi);
        }
    }
}
