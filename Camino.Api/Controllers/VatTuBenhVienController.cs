using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.VatTuBenhViens;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.VatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.VatTuBenhViens;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.VatTu;
using Camino.Services.VatTuBenhViens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class VatTuBenhVienController : CaminoBaseController
    {
        private readonly IVatTuBenhVienService _vatTuBenhVienYTeService;
        private readonly IVatTuService _vatTuService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        public VatTuBenhVienController(IVatTuBenhVienService vatTuBenhVienYTeService, ILocalizationService localizationService, IVatTuService vatTuService, IExcelService excelService)
        {
            _vatTuService = vatTuService;
            _vatTuBenhVienYTeService = vatTuBenhVienYTeService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _vatTuBenhVienYTeService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _vatTuBenhVienYTeService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var vattuBV = await _vatTuBenhVienYTeService.GetByIdAsync(id, w => w.Include(e => e.VatTus));
            if (vattuBV == null)
            {
                return NotFound();
            }
            vattuBV.VatTus.WillDelete = true;
            await _vatTuBenhVienYTeService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("GetVatTuYTeBenhVien")]
        public async Task<ActionResult> GetVatTuYTeBenhVien(DropDownListRequestModel model)
        {

            var lookup = await _vatTuBenhVienYTeService.GetVatTuYTeBenhVienKhamBenh(model);
            return Ok(lookup);
        }
        [HttpPost("GetVatTuYTeBenhVienKhamBenhUpdate")]
        public async Task<ActionResult> GetVatTuYTeBenhVienKhamBenhUpdate(DropDownListRequestModel model, long vattuId)
        {
            var lookup = await _vatTuBenhVienYTeService.GetVatTuYTeBenhVienKhamBenhUpdate(model, vattuId);
            return Ok(lookup);

        }
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<VatTuBenhVienViewModel>> Get(long id)
        {
            var result = await _vatTuBenhVienYTeService.GetByIdAsync(id, w => w.Include(e => e.VatTus).ThenInclude(z => z.NhomVatTu));
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<VatTuBenhVienViewModel>();
            return Ok(resultData);
        }
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        public async Task<ActionResult> Put([FromBody] VatTuBenhVienViewModel vattuViewModel)
        {
            var entity = await _vatTuBenhVienYTeService.GetByIdAsync(vattuViewModel.Id, w => w.Include(e => e.VatTus).ThenInclude(z => z.NhomVatTu));
            if (entity == null) return NotFound();
            vattuViewModel.ToEntity(entity);
            entity.VatTus.Ma = vattuViewModel.MaVatTuBenhVien;
            entity.VatTus.Ten = vattuViewModel.Ten;
            entity.VatTus.NhomVatTuId = vattuViewModel.NhomVatTuId.GetValueOrDefault();
            entity.VatTus.DonViTinh = vattuViewModel.DonViTinh;
            entity.VatTus.TyLeBaoHiemThanhToan = vattuViewModel.TyLeBaoHiemThanhToan.GetValueOrDefault();
            entity.VatTus.QuyCach = vattuViewModel.QuyCach;
            entity.VatTus.NhaSanXuat = vattuViewModel.NhaSanXuat;
            entity.VatTus.NuocSanXuat = vattuViewModel.NuocSanXuat;
            entity.VatTus.MoTa = vattuViewModel.MoTa;
            entity.VatTus.IsDisabled = vattuViewModel.IsDisabled;
            entity.VatTus.HeSoDinhMucDonViTinh = vattuViewModel.HeSoDinhMucDonViTinh;
            await _vatTuBenhVienYTeService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        public async Task<ActionResult> Post([FromBody] VatTuBenhVienViewModel vattuViewModel)
        {
            var entity = vattuViewModel.ToEntity<VatTuBenhVien>();
            var vatTu = new VatTu
            {
                Ma = vattuViewModel.MaVatTuBenhVien,
                Ten = vattuViewModel.Ten,
                NhomVatTuId = vattuViewModel.NhomVatTuId.GetValueOrDefault(),
                DonViTinh = vattuViewModel.DonViTinh,
                TyLeBaoHiemThanhToan = vattuViewModel.TyLeBaoHiemThanhToan.GetValueOrDefault(),
                QuyCach = vattuViewModel.QuyCach,
                NhaSanXuat = vattuViewModel.NhaSanXuat,
                NuocSanXuat = vattuViewModel.NuocSanXuat,
                MoTa = vattuViewModel.MoTa,
                IsDisabled = vattuViewModel.IsDisabled,
                HeSoDinhMucDonViTinh = vattuViewModel.HeSoDinhMucDonViTinh,
            };
            entity.VatTus = vatTu;
            await _vatTuBenhVienYTeService.AddAsync(entity);
            return Ok();
        }

        [HttpPost("ExportVatTuYTeBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        public async Task<ActionResult> ExportVatTuYTeBenhVien(QueryInfo queryInfo)
        {
            var gridData = await _vatTuBenhVienYTeService.GetDataForGridAsync(queryInfo, true);
            var vatTuData = gridData.Data.Select(p => (VatTuBenhVienGridVo)p).ToList();
            var excelData = vatTuData.Map<List<VatTuBenhVienExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.TenNhomVatTu), "Nhóm"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.TenDonViTinh), "Đơn vị tính"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.QuyCach), "Quy cách"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.NhaSanXuat), "Nhà sản xuất"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.NuocSanXuat), "Nước sản xuất"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.BaoHiemChiTra), "Bảo hiểm chi trả"));
            lstValueObject.Add((nameof(VatTuBenhVienExportExcel.HieuLuc), "Hiệu lực"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Vật tư y tế bệnh viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VatTuYTeBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("KichHoatVatTuBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien)]
        public async Task<ActionResult> KichHoatVatTuBenhVien(long id)
        {
            var entity = await _vatTuBenhVienYTeService.GetByIdAsync(id, w => w.Include(e => e.VatTus));
            entity.VatTus.IsDisabled = entity.VatTus.IsDisabled == null ? true : !entity.VatTus.IsDisabled;
            await _vatTuBenhVienYTeService.UpdateAsync(entity);
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetLoaiSuDung")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetLoaiSuDung([FromBody]LookupQueryInfo model)
        {
            var listIcd = _vatTuBenhVienYTeService.GetLoaiSuDung(model);
            return Ok(listIcd);
        }

        #region //BVHD-3472
        [HttpPost("GetMaTaoMoiVatTu")]
        public async Task<ActionResult<string>> GetMaTaoMoiVatTuAsync(MaVatTuTaoMoiInfoVo model)
        {
            var maDuocPham = await _vatTuBenhVienYTeService.GetMaTaoMoiVatTuAsync(model);
            return Ok(maDuocPham);
        }

        [HttpPost("KiemTraTrungVatTuBenhVien")]
        public async Task<ActionResult<bool>> KiemTraTrungVatTuBenhVienAsync([FromBody] VatTuBenhVienViewModel vattuViewModel)
        {
            var vatTu = new VatTu
            {
                Ma = vattuViewModel.MaVatTuBenhVien,
                Ten = vattuViewModel.Ten,
                NhomVatTuId = vattuViewModel.NhomVatTuId.GetValueOrDefault(),
                DonViTinh = vattuViewModel.DonViTinh,
                TyLeBaoHiemThanhToan = vattuViewModel.TyLeBaoHiemThanhToan.GetValueOrDefault(),
                QuyCach = vattuViewModel.QuyCach,
                NhaSanXuat = vattuViewModel.NhaSanXuat,
                NuocSanXuat = vattuViewModel.NuocSanXuat,
                MoTa = vattuViewModel.MoTa,
                IsDisabled = vattuViewModel.IsDisabled,
                HeSoDinhMucDonViTinh = vattuViewModel.HeSoDinhMucDonViTinh,
            };
            var kiemtra = await _vatTuBenhVienYTeService.KiemTraTrungVatTuBenhVienAsync(vatTu);
            return Ok(kiemtra);
        }
        #endregion
    }
}