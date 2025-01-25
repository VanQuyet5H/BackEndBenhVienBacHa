using Camino.Api.Auth;
using Camino.Api.Models.DuyetHoanTraKSNK;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using  Camino.Core.Domain.ValueObject.KhoKSNKs;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraKSNKController
    {
        #region Thông tin chung get hoàn trả kiểm soát nhiêm khuẩn

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoanTraKSNK/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult<ThongTinHoanTraKSNK>> GetThongTinDuyetHoanTraKSNK(long id)
        {
            var thongTinDuyetKhoVatTu = await _ycHoanTraKSNKService.GetThongTinHoanTraKSNK(id);
            return Ok(thongTinDuyetKhoVatTu);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuyetHoanTraDuocPhamKSNK/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult<ThongTinHoanTraKSNK>> GetThongTinDuyetHoanTraDuocPhamKSNK(long id)
        {
            var thongTinDuyetHoanTraDuocPham = await _ycHoanTraKSNKService.GetThongTinDuyetHoanTraDuocPham(id);
            return Ok(thongTinDuyetHoanTraDuocPham);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiHoanTraKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult> TuChoiHoanTraVatTuNhapKho(TuChoiDuyetHoanTraKSNKViewModel tuChoiDuyetHoanTraVatTuViewModel)
        {
            if (tuChoiDuyetHoanTraVatTuViewModel.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham)
            {
                await _ycHoanTraKSNKService.TuChoiDuyetHoanTraDuocPhamKSNK(tuChoiDuyetHoanTraVatTuViewModel.Id, tuChoiDuyetHoanTraVatTuViewModel.LyDoHuy);
            }
            else
            {
                await _ycHoanTraKSNKService.TuChoiDuyetHoanTraKSNK(tuChoiDuyetHoanTraVatTuViewModel.Id, tuChoiDuyetHoanTraVatTuViewModel.LyDoHuy);
            }
            return Ok();          
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HoanTraKSNKKho")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult<ThongTinHoanTraKSNK>> HoanTraKSNKKho(DuyetHoanTraKSNKViewModel duyetHoanTraVatTuViewModel)
        {
            if (duyetHoanTraVatTuViewModel.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham)
            {
                await _ycHoanTraKSNKService.DuyetHoanTraDuocPham(duyetHoanTraVatTuViewModel.Id, duyetHoanTraVatTuViewModel.NguoiNhanId, duyetHoanTraVatTuViewModel.NguoiTraId);

                var thongTinDuyetKhoDuocPham = await _ycHoanTraKSNKService.GetThongTinDuyetHoanTraDuocPham(duyetHoanTraVatTuViewModel.Id);
                return Ok(thongTinDuyetKhoDuocPham);
            }
            else
            {
                await _ycHoanTraKSNKService.DuyetHoanTraNhapKho(duyetHoanTraVatTuViewModel.Id, duyetHoanTraVatTuViewModel.NguoiNhanId, duyetHoanTraVatTuViewModel.NguoiTraId);

                var thongTinDuyetKhoVatTu = await _ycHoanTraKSNKService.GetThongTinHoanTraKSNK(duyetHoanTraVatTuViewModel.Id);
                return Ok(thongTinDuyetKhoVatTu);
            }
        }

        #endregion

        #region Thông tin hoàn trả kiểm soát nhiêm khuẩn

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachHoanTraKSNKForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachHoanTraKSNKForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDanhSachHoanTraKSNKForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachHoanTraKSNKForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachHoanTraKSNKForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetTotalDanhSachHoanTraKSNKForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachHoanTraKSNKChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachHoanTraKSNKChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDanhSachHoanTraKSNKChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachHoanTraKSNKChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachHoanTraKSNKChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetTotalDanhSachHoanTraKSNKChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportHoanTraKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetYeuCauHoanTraKSNK)]
        public async Task<ActionResult> ExportHoanTraKSNK(QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDanhSachHoanTraKSNKForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DanhSachHoanTraKSNKVo)p).ToList();
            var dataExcel = duocPhamData.Map<List<DuyetHoanTraKSNKExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id.ToString();
                var gridChildData = await _ycHoanTraKSNKService.GetDanhSachHoanTraKSNKChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachHoanTraKSNKChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetHoanTraKSNKExportExcelChild>>();
                item.DuyetHoanTraKSNKExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetHoanTraKSNKExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(DuyetHoanTraKSNKExportExcel.TenNhanVienYeuCau), "Người yêu cầu"),
                (nameof(DuyetHoanTraKSNKExportExcel.TenKhoCanHoanTra), "Hoàn Trả Từ Kho"),
                (nameof(DuyetHoanTraKSNKExportExcel.TenKhoNhanHoanTra), "Hoàn Trả Về Kho"),
                (nameof(DuyetHoanTraKSNKExportExcel.NgayYeuCauDisplay), "Ngày Yêu Cầu"),
                (nameof(DuyetHoanTraKSNKExportExcel.TinhTrangDuyet), "Tình Trạng"),
                (nameof(DuyetHoanTraKSNKExportExcel.TenNhanVienDuyet), "Người Duyệt"),
                (nameof(DuyetHoanTraKSNKExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetHoanTraKSNKExportExcel.DuyetHoanTraKSNKExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Hoàn Trả Kiểm Soát Nhiễm Khuẩn", 2, "Duyệt Hoàn Trả Kiểm Soát Nhiễm Khuẩn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetHoanTraKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        [HttpGet("GetHtmlPhieuInHoanTraKSNK")]
        public ActionResult GetHtmlPhieuInHoanTraKSNK(long yeuCauHoanTraVatTuId, string hostingName)
        {
            var htmlPhieuInHoanTra = _ycHoanTraKSNKService.GetHtmlPhieuInHoanTraKSNK(yeuCauHoanTraVatTuId, hostingName);
            return Ok(htmlPhieuInHoanTra);
        }       

    }
}
