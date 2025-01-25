using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Api.Models.Voucher;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Voucher;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.Voucher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : CaminoBaseController
    {
        private readonly IVoucherService _voucherService;
        private readonly IVoucherChiTietMienGiamService _voucherChiTietMienGiamService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public VoucherController(IVoucherService voucherService, IVoucherChiTietMienGiamService voucherChiTietMienGiamService, IExcelService excelService, ILocalizationService localizationService)
        {
            _voucherService = voucherService;
            _voucherChiTietMienGiamService = voucherChiTietMienGiamService;
            _excelService = excelService;
            _localizationService = localizationService;
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        [HttpPost("GetDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _voucherService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _voucherService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<VoucherMarketingViewModel>> Get(long id)
        {
            var voucher = await _voucherService.GetByIdAsync(id, o => o.Include(p => p.VoucherChiTietMienGiams));

            if (voucher == null)
            {
                return NotFound();
            }

            VoucherMarketingViewModel voucherVM = voucher.ToModel<VoucherMarketingViewModel>();

            if(voucher.ChietKhauTatCaDichVu != null && voucher.ChietKhauTatCaDichVu.Value == true)
            {
                voucherVM.LoaiDichVuVoucherMarketing = EnumLoaiDichVuVoucherMarketing.TatCaDichVu;
            }
            else if(voucher.VoucherChiTietMienGiams.Any(p => p.DichVuKhamBenhBenhVienId != null || p.DichVuKyThuatBenhVienId != null))
            {
                voucherVM.LoaiDichVuVoucherMarketing = EnumLoaiDichVuVoucherMarketing.DichVu;
            }
            else
            {
                voucherVM.LoaiDichVuVoucherMarketing = EnumLoaiDichVuVoucherMarketing.NhomDichVu;
            }

            return Ok(voucherVM);
        }

        [HttpPost]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<VoucherMarketingViewModel>> Post([FromBody] VoucherMarketingViewModel voucherViewModel)
        {
            var voucher = voucherViewModel.ToEntity<Voucher>();

            foreach(var item in voucher.VoucherChiTietMienGiams)
            {
                if(item.NhomDichVuBenhVienId != null && item.NhomDichVuBenhVienId != 0)
                {
                    if(_voucherService.IsNhomDichVuKhamBenh(item.NhomDichVuBenhVienId.GetValueOrDefault()))
                    {
                        item.NhomDichVuBenhVienId = null;
                        item.NhomDichVuKhamBenh = true;
                    }
                    else
                    {
                        item.NhomDichVuKhamBenh = false;
                    }
                }
            }

            await _voucherService.AddAsync(voucher);

            var voucherResponse = await _voucherService.GetByIdAsync(voucher.Id);

            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = voucher.Id },
                voucherResponse.ToModel<VoucherMarketingViewModel>()
            );
        }

        [HttpPut]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.VoucherMarketing)]
        public async Task<ActionResult> Put([FromBody] VoucherMarketingViewModel voucherViewModel)
        {
            var id = voucherViewModel.Id;
            var voucher = await _voucherService.GetByIdAsync(id, o => o.Include(p => p.VoucherChiTietMienGiams));

            if (voucher == null)
            {
                return NotFound();
            }

            voucherViewModel.ToEntity(voucher);

            foreach (var item in voucher.VoucherChiTietMienGiams)
            {
                if (item.NhomDichVuBenhVienId != null && item.NhomDichVuBenhVienId != 0)
                {
                    if (_voucherService.IsNhomDichVuKhamBenh(item.NhomDichVuBenhVienId.GetValueOrDefault()))
                    {
                        item.NhomDichVuBenhVienId = null;
                        item.NhomDichVuKhamBenh = true;
                    }
                    else
                    {
                        item.NhomDichVuKhamBenh = false;
                    }
                }
            }

            await _voucherService.UpdateAsync(voucher);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.VoucherMarketing)]
        public async Task<ActionResult> Delete(long id)
        {
            var voucher = await _voucherService.GetByIdAsync(id, p => p.Include(o => o.VoucherChiTietMienGiams));

            if (voucher == null)
            {
                return NotFound();
            }

            await _voucherService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.VoucherMarketing)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var vouchers = await _voucherService.GetByIdsAsync(model.Ids, p => p.Include(o => o.VoucherChiTietMienGiams));

            if (vouchers == null)
            {
                return NotFound();
            }

            if (vouchers.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }

            IEnumerable<Voucher> list = await _voucherService.GetByIdsAsync(model.Ids);
            await _voucherService.DeleteAsync(list);

            return NoContent();
        }
        #endregion

        [HttpPost("GetListDichVuChoVoucher")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetListDichVuChoVoucher([FromBody] DropDownListRequestModel queryInfo)
        {
            return Ok(await _voucherService.GetListDichVuChoVoucher(queryInfo));
        }

        [HttpPost("GetListNhomDichVuChoVoucher")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetListNhomDichVuChoVoucher([FromBody] DropDownListRequestModel queryInfo)
        {
            return Ok(await _voucherService.GetListNhomDichVuChoVoucher(queryInfo));
        }

        [HttpPost("GetListLoaiGiaChoDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetListLoaiGiaChoDichVu(long dichVuId, EnumDichVuTongHop loaiDichVu)
        {
            return Ok(await _voucherService.GetListLoaiGiaChoDichVu(dichVuId, loaiDichVu));
        }

        [HttpPost("GetListTatCaLoaiGiaChoDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetListTatCaLoaiGiaChoDichVu(EnumDichVuTongHop loaiDichVu)
        {
            return Ok(await _voucherService.GetListTatCaLoaiGiaChoDichVu(loaiDichVu));
        }

        [HttpPost("GetDonGiaChoDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDonGiaChoDichVu(long dichVuId, long loaiGiaId, EnumDichVuTongHop loaiDichVu)
        {
            return Ok(await _voucherService.GetDonGiaChoDichVu(dichVuId, loaiGiaId, loaiDichVu));
        }

        [HttpPost("GetListDichVuForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetListDichVuForGridAsync(long voucherId)
        {
            return Ok(await _voucherService.GetListDichVuForGridAsync(voucherId));
        }

        [HttpPost("GetPagesListDichVuForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetPagesListDichVuForGridAsync(long voucherId)
        {
            return Ok(await _voucherService.GetPagesListDichVuForGridAsync(voucherId));
        }

        [HttpPost("GetListNhomDichVuForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetListNhomDichVuForGridAsync(long voucherId)
        {
            return Ok(await _voucherService.GetListNhomDichVuForGridAsync(voucherId));
        }

        [HttpPost("GetPagesListNhomDichVuForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetPagesListNhomDichVuForGridAsync(long voucherId)
        {
            return Ok(await _voucherService.GetPagesListNhomDichVuForGridAsync(voucherId));
        }

        [HttpPost("KiemTraDichVuDaTonTaiTrongNhomDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> KiemTraDichVuDaTonTaiTrongNhomDichVu(long voucherId, long dichVuId, EnumDichVuTongHop loaiDichVuBenhVien)
        {
            return Ok(await _voucherService.KiemTraDichVuDaTonTaiTrongNhomDichVu(voucherId, dichVuId, loaiDichVuBenhVien));
        }

        [HttpPost("KiemTraNhomDichVuDaBaoGomDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> KiemTraNhomDichVuDaBaoGomDichVu(long voucherId, long nhomDichVuId)
        {
            return Ok(await _voucherService.KiemTraNhomDichVuDaBaoGomDichVu(voucherId, nhomDichVuId));
        }

        [HttpPost("KiemTraDichVuDaTonTaiTrongNhomDichVuTheoDanhSach")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> KiemTraDichVuDaTonTaiTrongNhomDichVuTheoDanhSach([FromBody] VoucherMarketingViewModel voucherViewModel, long dichVuId, EnumDichVuTongHop loaiDichVuBenhVien)
        {
            var lstChiTietMienGiam = new List<VoucherChiTietMienGiam>();

            foreach(var item in voucherViewModel.lstVoucherChiTietMienGiamNhomDichVu)
            {
                var chiTietEntity = item.ToEntity<VoucherChiTietMienGiam>();
                lstChiTietMienGiam.Add(chiTietEntity);
            }

            return Ok(await _voucherService.KiemTraDichVuDaTonTaiTrongNhomDichVuTheoDanhSach(dichVuId, loaiDichVuBenhVien, lstChiTietMienGiam));
        }

        [HttpPost("KiemTraNhomDichVuDaBaoGomDichVuTheoDanhSach")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> KiemTraNhomDichVuDaBaoGomDichVuTheoDanhSach([FromBody] VoucherMarketingViewModel voucherViewModel, long nhomDichVuId)
        {
            var lstChiTietMienGiam = new List<VoucherChiTietMienGiam>();

            foreach (var item in voucherViewModel.lstVoucherChiTietMienGiam)
            {
                var chiTietEntity = item.ToEntity<VoucherChiTietMienGiam>();
                lstChiTietMienGiam.Add(chiTietEntity);
            }

            return Ok(await _voucherService.KiemTraNhomDichVuDaBaoGomDichVuTheoDanhSach(nhomDichVuId, lstChiTietMienGiam));
        }

        [HttpGet("GetBarcodeBasedOnMa")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public ActionResult<string> GetBarcodeBasedOnMa(string ma)
        {
            return Ok(_voucherService.GetBarcodeBasedOnMa(ma));
        }

        [HttpPost("GetHtmlVoucher")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public ActionResult<string> GetHtmlVoucher(VoucherThongTinHTMLViewModel voucherHTMLViewModel)
        {
            return Ok(_voucherService.GetHtmlVoucher(voucherHTMLViewModel.HostingName, voucherHTMLViewModel.Ten, voucherHTMLViewModel.Ma, voucherHTMLViewModel.SoLuong, voucherHTMLViewModel.MaSoTu, voucherHTMLViewModel.SoLuongPhatHanh));
        }

        [HttpPost("GetListChiTietBenhNhanDaSuDungForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetListChiTietBenhNhanDaSuDungForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _voucherService.GetListChiTietBenhNhanDaSuDungForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetPagesListChiTietBenhNhanDaSuDungForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetPagesListChiTietBenhNhanDaSuDungForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _voucherService.GetPagesListChiTietBenhNhanDaSuDungForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetTongSoBenhNhanSuDungDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTongSoBenhNhanSuDungDichVu(long voucherId)
        {
            return Ok(await _voucherService.GetTongSoBenhNhanSuDungDichVu(voucherId));
        }

        [HttpGet("GetSoLuongPhatHanhVoucher")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VoucherMarketing)]
        public async Task<ActionResult<GridDataSource>> GetSoLuongPhatHanhVoucher(long voucherId)
        {
            return Ok(await _voucherService.GetSoLuongPhatHanhVoucher(voucherId));
        }

        [HttpPost("ExportChuongTrinhMarketingTheoVoucher")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.VoucherMarketing)]
        public async Task<ActionResult> ExportChuongTrinhMarketingTheoVoucher(QueryInfo queryInfo)
        {
            var gridData = await _voucherService.GetDataForGridAsync(queryInfo, true);
            var voucherData = gridData.Data.Select(p => (VoucherMarketingGridVo)p).ToList();
            var excelData = voucherData.Map<List<VoucherExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VoucherExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(VoucherExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(VoucherExportExcel.SoLuongPhatHanhDisplay), "SL phát hành"));
            lstValueObject.Add((nameof(VoucherExportExcel.TuNgayDisplay), "Thời gian bắt đầu"));
            lstValueObject.Add((nameof(VoucherExportExcel.DenNgayDisplay), "Thời gian kết thúc"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Chương trình Marketing theo Voucher");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ChuongTrinhMarketingTheoVoucher" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
