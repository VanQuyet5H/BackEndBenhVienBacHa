using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.TonKho;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TonKhos;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class TonKhoController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            //bỏ lazy load
            var gridData = await _tonKhoService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridTatCaAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public ActionResult<GridDataSource> GetTotalPageForGridTatCaAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = _tonKhoService.GetTotalPageForGridTatCaAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridTatCaAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridTatCaAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForGridTatCaAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetValueTest")]
        public async Task<ActionResult> GetValueTest()
        {
            var lookup = await _tonKhoService.GetDataTonKho();
            return Ok(lookup);
        }
        [HttpPost("GetKhoDuocPham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoDuocPhamAsync([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetKhoDuocPhamAsync(queryInfo);
            return Ok(result);
        }
        [HttpPost("GetKhoTatCa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoTatCa([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetKho(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetKhoVatTuChoKT")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoVatTuChoKT([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetKhoVatTuChoKT(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetCanhBao")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetCanhBao()
        {
            var model = new List<LookupItemVo>
            {
                new LookupItemVo { DisplayName = "Tất cả", KeyId = 0 },
                new LookupItemVo { DisplayName = "Tồn kho quá nhiều", KeyId = 1 },
                new LookupItemVo { DisplayName = "Hết tồn kho", KeyId = 2 },
                new LookupItemVo { DisplayName = "Sắp hết tồn kho", KeyId = 3 }
            };
            return Ok(model);
        }
        [HttpPost("GetKhoFirst")]
        public async Task<ActionResult> GetKhoFirst([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetKhoDuocPhamAsync(queryInfo);
            return Ok(result.Any() ? result.First() : null);
        }

        [HttpPost("GetListKhoToView")]
        public ActionResult GetTonKhoCanhBao(string search)
        {
            var result = _tonKhoService.GetTonKhoCanhBao(search);
            return Ok(result);
        }
        [HttpPost("GetCanhBaoDuocPhamHTML")]
        public ActionResult GetCanhBaoDuocPhamHTML(string search)
        {
            var result = _tonKhoService.GetCanhBaoDuocPhamHTML(search);
            return Ok(result);
        }
        [HttpPost("GetListKhoTonKhoToView")]
        public ActionResult GetTongHopTonKho(string search)
        {
            var result = _tonKhoService.GetTongHopTonKho(search);
            return Ok(result);
        }
        [HttpPost("GetTonKhoDuocPhamHTML")]
        public ActionResult GetTonKhoDuocPhamHTML(string search)
        {
            var result = _tonKhoService.GetTonKhoDuocPhamHTML(search);
            return Ok(result);
        }
        [HttpGet("InBaoCaoDuocPhamTongHopTonKho")]
        public async Task<ActionResult> InBaoCaoDuocPhamTongHopTonKho(string search)
        {
            var result = _tonKhoService.GetTonKhoDuocPhamHTML(search);
            return Ok(result);
        }
        [HttpPost("ExportTongHopTonKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult> ExportTongHopTonKho(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForGridTatCaAsync(queryInfo, true);
            var tongHopTonKhoData = gridData.Data.Select(p => (TonKhoTatCaGridVo)p).ToList();
            var excelData = tongHopTonKhoData.Map<List<TonKhoTatCaExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.MaDuocPham), "Mã dược phẩm"));
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.DuocPham), "Dược phẩm"));
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.HoatChat), "Hoạt chất"));
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.HamLuong), "Hàm lượng"));
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.PhanLoai), "Phân nhóm"));
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.DonViTinhName), "Đơn vị tính"));
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.SoLuongTon), "Số lượng tồn"));
            lstValueObject.Add((nameof(TonKhoTatCaExportExcel.GiaTriSoLuongTonFormat), "Giá trị tồn"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Tổng hợp tồn kho");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("ExportCanhBaoTonKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult> ExportCanhBaoTonKho(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForGridAsync(queryInfo, true);
            var tongHopTonKhoData = gridData.Data.Select(p => (TonKhoGridVo)p).ToList();
            var excelData = tongHopTonKhoData.Map<List<TonKhoGridVo>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.MaDuocPham), "Mã dược phẩm"));
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.DuocPham), "Dược phẩm"));
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.HoatChat), "Hoạt chất"));
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.HamLuong), "Hàm lượng"));
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.PhanLoai), "Phân nhóm"));
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.DonViTinhName), "Đơn vị tính"));
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.SoLuongTon), "Số lượng tồn"));
            lstValueObject.Add((nameof(CanhBaoTonKhoExportExcel.CanhBao), "Cảnh báo"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Cảnh báo tồn kho");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CanhBaoTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #region Nhap xuat ton

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNhapXuatTonAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNhapXuatTonAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForGridNhapXuatTonAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNhapXuatTonAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNhapXuatTonAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetTotalPageForGridNhapXuatTonAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GetXuatNhapTonKhoHTML")]
        public ActionResult GetXuatNhapTonKhoHTML(string search)
        {
            var result = _tonKhoService.GetXuatNhapTonKhoHTML(search);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNhapXuatTonChiTietAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNhapXuatTonChiTietAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForGridNhapXuatTonChiTietAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNhapXuatTonChiTietAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNhapXuatTonChiTietAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetTotalPageForGridNhapXuatTonChiTietAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDuocPhamAndKhoName")]
        public async Task<ActionResult> GetDuocPhamAndKhoName([FromBody] ChiTietItem model)
        {
            var result = await _tonKhoService.GetChiTiet(model);
            return Ok(result);
        }

        [HttpPost("ExportNhapXuatTonKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuocPhamTonKho)]
        public async Task<ActionResult> ExportNhapXuatTonKho(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForGridNhapXuatTonAsync(queryInfo, true);
            var nhapXuatTonKhoData = gridData.Data.Select(p => (NhapXuatTonKhoGridVo)p).ToList();
            var excelData = nhapXuatTonKhoData.Map<List<NhapXuatTonKhoExportExcel>>().OrderBy(z => z.TenDuocPhamBenhVienPhanNhom).ToList();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(NhapXuatTonKhoExportExcel.TenDuocPhamBenhVienPhanNhom), "Nhóm"),
                (nameof(NhapXuatTonKhoExportExcel.Ma), "Mã dược phẩm"),
                (nameof(NhapXuatTonKhoExportExcel.DuocPham), "Dược phẩm"),
                (nameof(NhapXuatTonKhoExportExcel.HoatChat), "Hoạt chất"),
                (nameof(NhapXuatTonKhoExportExcel.HamLuong), "Hàm lượng"),
                (nameof(NhapXuatTonKhoExportExcel.DonViTinhDisplay), "Đơn vị tính"),
                (nameof(NhapXuatTonKhoExportExcel.TonDauKy), "Tồn đầu kỳ"),
                (nameof(NhapXuatTonKhoExportExcel.NhapTrongKy), "Nhập trong kỳ"),
                (nameof(NhapXuatTonKhoExportExcel.XuatTrongKy), "Xuất trong kỳ"),
                (nameof(NhapXuatTonKhoExportExcel.TonCuoiKy), "Tồn cuối kỳ")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nhập xuất tồn kho", 1, "Nhập xuất tồn kho", true);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhapXuatTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion Nhap xuat ton

        #region Cap nhat ton kho
        [HttpPost("GetChiTietTonKhoCuaDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CapNhatDuocPhamTonKho)]
        public async Task<ActionResult> GetChiTietTonKhoCuaDuocPham([FromBody] QueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetChiTietTonKhoCuaDuocPham(queryInfo);
            return Ok(result);
        }
        [HttpPost("GetTongTonKhoCuaDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CapNhatDuocPhamTonKho)]
        public async Task<ActionResult> GetTongTonKhoCuaDuocPham([FromBody] QueryInfo queryInfo)
        {
            var result = _tonKhoService.GetTongTonKhoCuaDuocPham(queryInfo);
            return Ok(result);
        }
        [HttpPost("UpdateChiTietTonKhoCuaDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CapNhatDuocPhamTonKho)]
        public ActionResult UpdateChiTietTonKhoCuaDuocPham([FromBody] CapNhatTonKhoDuocPhamViewModel capNhatTonKhoDuocPhamViewModel)
        {
            var capNhatTonKhoItem = capNhatTonKhoDuocPhamViewModel.Map<CapNhatTonKhoItem>();
            _tonKhoService.UpdateChiTietTonKhoCuaDuocPham(capNhatTonKhoItem);
            return NoContent();
        }

        [HttpPost("GetDuocPhamBenhVien")]
        public async Task<ActionResult> GetDuocPhamBenhVien([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _tonKhoService.GetDuocPhamBenhVien(queryInfo);
            return Ok(result);
        }
        #endregion Cap nhat ton kho
    }
}
