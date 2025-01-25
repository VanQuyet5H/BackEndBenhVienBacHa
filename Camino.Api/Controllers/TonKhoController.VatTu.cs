using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.VatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TonKhos;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class TonKhoController
    {
        #region Vật tư tồn kho (cảnh báo)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuTonKhoCanhBaoForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuTonKhoCanhBaoForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoCanhBaoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalVatTuTonKhoCanhBaoPagesForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalVatTuTonKhoCanhBaoPagesForGridAsync([FromBody] QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoCanhBaoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetVatTuTonKhoCanhBaoHTML")]
        public ActionResult GetVatTuTonKhoCanhBaoHTML(string search)
        {
            var result = _tonKhoService.GetVatTuTonKhoCanhBaoHTML(search);
            return Ok(result);
        }

        [HttpPost("ExportVatTuTonKhoCanhBao")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.VatTuTonKho)]
        public async Task<ActionResult> ExportVatTuTonKho(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoCanhBaoForGridAsync(queryInfo, true);

            var vatTuTonKhoCanhBaoData = gridData.Data.Select(p => (VatTuTonKhoCanhBaoGridVo)p).ToList();
            var excelData = vatTuTonKhoCanhBaoData.Map<List<VatTuTonKhoCanhBaoExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VatTuTonKhoCanhBaoExportExcel.TenVatTu), "Vật tư"));
            lstValueObject.Add((nameof(VatTuTonKhoCanhBaoExportExcel.DonViTinh), "Đơn vị tính"));
            lstValueObject.Add((nameof(VatTuTonKhoCanhBaoExportExcel.SoLuongTon), "Số lượng tồn"));
            lstValueObject.Add((nameof(VatTuTonKhoCanhBaoExportExcel.CanhBao), "Cảnh báo"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Cảnh báo vật tư tồn kho");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CanhBaoVatTuTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Vật tư tồn kho (tổng hợp)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuTonKhoTongHopForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuTonKhoTongHopForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoTongHopForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalVatTuTonKhoTongHopPagesForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalVatTuTonKhoTongHopPagesForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetTotalVatTuTonKhoTongHopPagesForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetVatTuTonKhoTongHopHTML")]
        public ActionResult GetVatTuTonKhoTongHopHTML(string search)
        {
            var result = _tonKhoService.GetVatTuTonKhoTongHopHTML(search);
            return Ok(result);
        }

        [HttpPost("ExportVatTuTonKhoTongHop")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.VatTuTonKho)]
        public async Task<ActionResult> ExportVatTuTonKhoTongHop(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoTongHopForGridAsync(queryInfo, true);

            var vatTuTonKhoTongHopData = gridData.Data.Select(p => (VatTuTonKhoTongHopGridVo)p).ToList();
            var excelData = vatTuTonKhoTongHopData.Map<List<VatTuTonKhoTongHopExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VatTuTonKhoTongHopExportExcel.TenVatTu), "Vật tư"));
            lstValueObject.Add((nameof(VatTuTonKhoTongHopExportExcel.DonViTinh), "Đơn vị tính"));
            lstValueObject.Add((nameof(VatTuTonKhoTongHopExportExcel.SoLuongTon), "Số lượng tồn"));
            lstValueObject.Add((nameof(VatTuTonKhoTongHopExportExcel.GiaTriSoLuongTonFormat), "Giá trị tồn"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Tổng hợp vật tư tồn kho");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VatTuTonKhoTongHop" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Vật tư tồn kho (nhập xuất)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuTonKhoNhapXuatForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuTonKhoNhapXuatForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoNhapXuatForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalVatTuTonKhoNhapXuatPagesForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalVatTuTonKhoNhapXuatPagesForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetTotalVatTuTonKhoNhapXuatPagesForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetVatTuTonKhoNhapXuatHTML")]
        public ActionResult GetVatTuTonKhoNhapXuatHTML(string search)
        {
            var result = _tonKhoService.GetVatTuTonKhoNhapXuatHTML(search);
            return Ok(result);
        }

        [HttpPost("ExportVatTuTonKhoNhapXuat")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.VatTuTonKho)]
        public async Task<ActionResult> ExportVatTuTonKhoNhapXuat(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoNhapXuatForGridAsync(queryInfo, true);

            var vatTuTonKhoNhapXuatData = gridData.Data.Select(p => (VatTuTonKhoNhapXuatGridVo)p).ToList();
            var excelData = vatTuTonKhoNhapXuatData.Map<List<VatTuTonKhoNhapXuatExportExcel>>().OrderBy(z => z.TenNhomVatTu).ToList(); 

            var lstValueObject = new List<(string, string)>
            {
                (nameof(VatTuTonKhoNhapXuatExportExcel.TenNhomVatTu), "Nhóm"),
                (nameof(VatTuTonKhoNhapXuatExportExcel.Ma), "Mã vật tư"),
                (nameof(VatTuTonKhoNhapXuatExportExcel.TenVatTu), "Vật tư"),
                (nameof(VatTuTonKhoNhapXuatExportExcel.DonViTinh), "Đơn vị tính"),
                (nameof(VatTuTonKhoNhapXuatExportExcel.TonDauKy), "Tồn đầu kỳ"),
                (nameof(VatTuTonKhoNhapXuatExportExcel.NhapTrongKy), "Nhập trong kỳ"),
                (nameof(VatTuTonKhoNhapXuatExportExcel.XuatTrongKy), "Xuất trong kỳ"),
                (nameof(VatTuTonKhoNhapXuatExportExcel.TonCuoiKy), "Tồn cuối kỳ")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nhập xuất vật tư tồn kho", 1, "Nhập xuất vật tư tồn kho", true);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VatTuTonKhoNhapXuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuTonKhoNhapXuatChiTietForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuTonKhoNhapXuatChiTietForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoNhapXuatChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuTonKhoNhapXuatChiTietPagesForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuTonKhoNhapXuatChiTietPagesForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoNhapXuatChiTietPagesForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportVatTuTonKhoNhapXuatChiTiet")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.VatTuTonKho)]
        public async Task<ActionResult> ExportVatTuTonKhoNhapXuatChiTiet(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuTonKhoNhapXuatChiTietForGridAsync(queryInfo, true);

            var vatTuTonKhoNhapXuatChiTietData = gridData.Data.Select(p => (VatTuTonKhoNhapXuatDetailGridVo)p).ToList();
            var excelData = vatTuTonKhoNhapXuatChiTietData.Map<List<VatTuTonKhoNhapXuatChiTietExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VatTuTonKhoNhapXuatChiTietExportExcel.STT), "STT"));
            lstValueObject.Add((nameof(VatTuTonKhoNhapXuatChiTietExportExcel.NgayDisplay), "Ngày"));
            lstValueObject.Add((nameof(VatTuTonKhoNhapXuatChiTietExportExcel.MaChungTu), "Mã chứng từ"));
            lstValueObject.Add((nameof(VatTuTonKhoNhapXuatChiTietExportExcel.Nhap), "Nhập"));
            lstValueObject.Add((nameof(VatTuTonKhoNhapXuatChiTietExportExcel.Xuat), "Xuất"));
            lstValueObject.Add((nameof(VatTuTonKhoNhapXuatChiTietExportExcel.Ton), "Tồn"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nhập xuất vật tư tồn kho chi tiết");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VatTuTonKhoNhapXuatChiTiet" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Kho
        [HttpPost("GetKhoVatTus")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoVatTus([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetKhoVatTusWithoutTatCa(queryInfo);
            //result.Add(new LookupItemVo { DisplayName = "Tất cả", KeyId = 0 });
            //result.Reverse();
            return Ok(result);
        }

        [HttpPost("GetFirstKhoVatTu")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetFirstKhoVatTu()
        {
            var result = await _tonKhoService.GetFirstKhoVatTu();
            return Ok(result);
        }

        [HttpPost("GetVatTuAndKhoName")]
        public async Task<ActionResult> GetVatTuAndKhoName([FromBody] ChiTietVatTuTonKhoNhapXuat model)
        {
            var result = await _tonKhoService.GetVatTuAndKhoName(model);
            return Ok(result);
        }
        #endregion

        #region Cap nhat ton kho
        [HttpPost("GetChiTietTonKhoCuaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CapNhatVatTuTonKho)]
        public async Task<ActionResult> GetChiTietTonKhoCuaVatTu([FromBody] QueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetChiTietTonKhoCuaVatTu(queryInfo);
            return Ok(result);
        }
        [HttpPost("GetTongTonKhoCuaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CapNhatDuocPhamTonKho)]
        public async Task<ActionResult> GetTongTonKhoCuaVatTu([FromBody] QueryInfo queryInfo)
        {
            var result = _tonKhoService.GetTongTonKhoCuaVatTu(queryInfo);
            return Ok(result);
        }
        [HttpPost("UpdateChiTietTonKhoCuaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CapNhatVatTuTonKho)]
        public ActionResult UpdateChiTietTonKhoCuaVatTu([FromBody] CapNhatTonKhoVatTuItem capNhatTonKhoItem)
        {
            var errors = new List<dynamic>();
            _tonKhoService.UpdateChiTietTonKhoCuaVatTu(capNhatTonKhoItem, out errors);
            return Ok(errors);
        }

        [HttpPost("CapNhatChiTietVatTuTonKho")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CapNhatVatTuTonKho)]
        public async Task<ActionResult> CapNhatChiTietVatTuTonKho(NhapKhoVatTuTonKhoViewModel nhapKhoVatTuTonKhoViewModel)
        {
            var capNhatTonKhoVatTuVo = nhapKhoVatTuTonKhoViewModel.Map<CapNhatTonKhoVatTuVo>();
            await _tonKhoService.CapNhatChiTietVatTu(capNhatTonKhoVatTuVo);
            return NoContent();
        }

        [HttpPost("GetVatTuBenhVien")]
        public async Task<ActionResult> GetVatTuBenhVien([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _tonKhoService.GetVatTuBenhVien(queryInfo);
            return Ok(result);
        }
        #endregion Cap nhat ton kho
    }
}
