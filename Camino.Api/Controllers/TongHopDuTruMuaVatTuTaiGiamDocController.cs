using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.TongHopDuTruMuaVatTuTaiGiamDocs;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaVatTuTaiGiamDocs;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.TongHopDuTruMuaVatTuTaiGiamDocs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class TongHopDuTruMuaVatTuTaiGiamDocController : CaminoBaseController
    {
        private readonly ITongHopDuTruMuaVatTuTaiGiamDocService _duTruGiamDocService;
        private readonly IExcelService _excelService;
        public TongHopDuTruMuaVatTuTaiGiamDocController(ITongHopDuTruMuaVatTuTaiGiamDocService duTruGiamDocService, IExcelService excelService)
        {
            _duTruGiamDocService = duTruGiamDocService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duTruGiamDocService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duTruGiamDocService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildChuaDuyetAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildChuaDuyetAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duTruGiamDocService.GetDataForGridChildChuaDuyetAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildChuaDuyetTaiGiamDocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildChuaDuyetTaiGiamDocAsync
           ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duTruGiamDocService.GetDataForGridChildChuaDuyetTaiGiamDocAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildDuyetAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildDuyetAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duTruGiamDocService.GetDataForGridChildDuyetAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duTruGiamDocService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuTruVatTuGiamDocViewModel>> Get(long id)
        {
            var duTruGiamDocEntity = await _duTruGiamDocService.GetByIdAsync(id, e => e.Include(q => q.KyDuTruMuaDuocPhamVatTu)
                .Include(q => q.GiamDoc).ThenInclude(q => q.User)
                .Include(q => q.NhanVienYeuCau).ThenInclude(q => q.User));

            if (duTruGiamDocEntity == null)
            {
                return NotFound();
            }

            var duTruGiamDocResult = duTruGiamDocEntity.ToModel<DuTruVatTuGiamDocViewModel>();
            duTruGiamDocResult.NguoiYeuCau = duTruGiamDocEntity.NhanVienYeuCau.User.HoTen;
            duTruGiamDocResult.NguoiDuyet = duTruGiamDocEntity.GiamDoc?.User.HoTen;
            duTruGiamDocResult.NgayDuyet = duTruGiamDocEntity.NgayGiamDocDuyet;
            return Ok(duTruGiamDocResult);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDuyetDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDuyetDetail
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duTruGiamDocService.GetDataForGridDuyetDetail(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Approve")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> Approve
            (long id)
        {
            await _duTruGiamDocService.Approve(id);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ApproveForm")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> ApproveForm
            ([FromBody]DuTruGiamDocVatTuApproveGridVo duTruGiamDocApprove)
        {
            await _duTruGiamDocService.ApproveForm(duTruGiamDocApprove);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Decline")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult<GridDataSource>> Decline
            ([FromBody]DuTruGiamDocVatTuApproveGridVo duTruGiamDocApprove)
        {
            await _duTruGiamDocService.Decline(duTruGiamDocApprove);
            return Ok();
        }

        [HttpPost("ExportTongHopDuTruMuaVatTuTaiGiamDoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc)]
        public async Task<ActionResult> ExportTongHopDuTruMuaVatTuTaiGiamDoc(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = int.MaxValue;

            var gridData = await _duTruGiamDocService.GetDataForGridAsync(queryInfo);
            var data = gridData.Data.Select(p => (DuTruGiamDocVatTuGridVo)p).ToList();
            var dataExcel = data.Map<List<DuTruGiamDocVatTuExportExcel>>();

            queryInfo.Sort = new List<Sort> { new Sort { Field = "VatTu", Dir = "asc" } };

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _duTruGiamDocService.GetDataForGridChildDuyetAsync(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (DuTruGiamDocVatTuDetailGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuTruGiamDocVatTuExportExcelChild>>();
                item.DuTruGiamDocVatTuExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuTruGiamDocVatTuExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(DuTruGiamDocVatTuExportExcel.KyDuTruDisplay), "Kỳ Dự Trù"),
                (nameof(DuTruGiamDocVatTuExportExcel.NguoiYeuCau), "Người Yêu Cầu"),
                (nameof(DuTruGiamDocVatTuExportExcel.NgayYeuCauDisplay), "Ngày Yêu Cầu"),
                (nameof(DuTruGiamDocVatTuExportExcel.TrangThai), "Trạng Thái"),
                (nameof(DuTruGiamDocVatTuExportExcel.NgayDuyetDisplay), "Ngày G.Đốc Duyệt"),
                (nameof(DuTruGiamDocVatTuExportExcel.DuTruGiamDocVatTuExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Tổng Hợp Dự Trù Mua Vật Tư Tại Giám Đốc");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopDuTruMuaVatTuTaiGiamDoc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";


            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
