using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaVatTu;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.VatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauMuaVatTu;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaVatTuController
    {
        #region Ds yeu cau mua vat tu
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetYeuCauMuaVatTuDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauMuaVatTuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetYeuCauMuaVatTuTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauMuaVatTuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        [HttpPost("GetKyDuTruVatTu")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKyDuTru(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruVatTuService.GetKyDuTruVatTu(model);
            return Ok(lookup);
        }

        [HttpPost("GetVatTuMuaDuTru")]
        public async Task<ActionResult<ICollection<VatTuTemplateLookupItem>>> GetVatTuMuaDuTru(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruVatTuService.GetVatTuMuaDuTrus(model);
            return Ok(lookup);
        }

        [HttpPost("GetNhomVatTuTreeView")]
        public async Task<ActionResult<List<LookupTreeItemVo>>> GetNhomVatTuTreeView(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruVatTuService.GetNhomVatTuTreeView(model);
            return Ok(lookup);
        }

        [HttpPost("ThongTinVatTu")]
        public ActionResult ThongTinVatTu(ThongTinChiTietVatTuTonKho thongTinThuocVM)
        {
            var entity = _yeuCauMuaDuTruVatTuService.ThongTinDuTruMuaVatTu(thongTinThuocVM);
            return Ok(entity);
        }

        [HttpGet("GetTrangThaiPhieuMuaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult<TrangThaiDuyetDuTruMuaVo>> GetTrangThaiPhieuMuaVatTu(long phieuMuaVatTuId)
        {
            var result = await _yeuCauMuaDuTruVatTuService.GetTrangThaiPhieuMuaVatTu(phieuMuaVatTuId);
            return Ok(result);
        }
        #region XuLy
        [HttpPost("ThemVatTuChiTietGridVo")]
        public async Task<ActionResult> ThemVatTuChiTietGridVo(VatTuDuTruGridViewModel model)
        {
            if (model.IsThemVatTu)
            {
                model.Ma = "Chưa nhập";
                var vatTu = model.ToEntity<VatTu>();
                _vatTuService.Add(vatTu);
                if (model.LoaiVatTu == 1)
                {
                    model.TenLoaiVatTu = "Không BHYT";
                    model.VatTuId = vatTu.Id;
                    model.LaVatTuBHYT = false;
                    return Ok(model);
                }
                else
                {
                    model.TenLoaiVatTu = "BHYT";
                    model.VatTuId = vatTu.Id;
                    model.LaVatTuBHYT = true;
                    return Ok(model);
                }
            }
            else
            {
                model.TenLoaiVatTu = model.LoaiVatTu == 2 ? "BHYT" : "Không BHYT";
                return Ok(model);
            }
        }
        #region Add
        [HttpPost("GuiPhieuMuaVatTuDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu)]
        public async Task<ActionResult> GuiPhieuMuaVatTuDuTru(DuTruMuaVatTuViewModel duTruMuaVatTuVM)
        {
            if (!duTruMuaVatTuVM.DuTruMuaVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DuTruMuaVatTu.DuTruMuaVatTuChiTiets.Required"));
            }
            var getKyDuTruMuaDuocPhamVatTuVo = await _yeuCauMuaDuTruDuocPhamService.GetKyDuTruMuaDuocPhamVatTu(duTruMuaVatTuVM.KyDuTruMuaDuocPhamVatTuId.Value);
            duTruMuaVatTuVM.TuNgay = getKyDuTruMuaDuocPhamVatTuVo.TuNgay;
            duTruMuaVatTuVM.DenNgay = getKyDuTruMuaDuocPhamVatTuVo.DenNgay;
            var soPhieu = await _yeuCauMuaDuTruVatTuService.GetSoPhieuDuTruVatTu();
            duTruMuaVatTuVM.SoPhieu = soPhieu;
            var duTruMuaVatTu = duTruMuaVatTuVM.ToEntity<DuTruMuaVatTu>();
            await _yeuCauMuaDuTruVatTuService.AddAsync(duTruMuaVatTu);
            return Ok(duTruMuaVatTu.Id);
        }
        #endregion

        #region Get
        [HttpGet("GetPhieuMuaVatTuDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu)]
        public async Task<ActionResult<DuTruMuaVatTuViewModel>> Get(long id)
        {
            var phieuMuaDuTru = await _yeuCauMuaDuTruVatTuService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.DuTruMuaVatTuChiTiets).ThenInclude(ct => ct.VatTu).ThenInclude(dp => dp.NhomVatTu)
                             .Include(r => r.KyDuTruMuaDuocPhamVatTu).ThenInclude(ct => ct.NhanVien).ThenInclude(dpct => dpct.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                             .Include(r => r.TruongKhoa).ThenInclude(tk => tk.User)
                             .Include(r => r.Kho)
                             .Include(r => r.DuTruMuaVatTuTheoKhoa).ThenInclude(dptk => dptk.DuTruMuaVatTuKhoDuoc)
                             .Include(r => r.DuTruMuaVatTuTheoKhoa).ThenInclude(dptk => dptk.DuTruMuaVatTuTheoKhoaChiTiets)
                             .Include(r => r.DuTruMuaVatTuKhoDuoc).ThenInclude(dptk => dptk.DuTruMuaVatTuKhoDuocChiTiets)
                             .Include(r => r.DuTruMuaVatTuTheoKhoa).ThenInclude(dptk => dptk.NhanVienKhoDuoc).ThenInclude(dpct => dpct.User)
                             .Include(r => r.DuTruMuaVatTuKhoDuoc).ThenInclude(dptk => dptk.GiamDoc).ThenInclude(dpct => dpct.User)
                             );
            if (phieuMuaDuTru == null)
            {
                return NotFound();
            }
            if (phieuMuaDuTru.DuTruMuaVatTuTheoKhoa == null && phieuMuaDuTru.DuTruMuaVatTuKhoDuoc == null && phieuMuaDuTru.TruongKhoaDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.LyDoTruongKhoaTuChoi;
            }
            else if (phieuMuaDuTru.DuTruMuaVatTuTheoKhoa != null && phieuMuaDuTru.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.DuTruMuaVatTuTheoKhoa.LyDoKhoDuocTuChoi;
            }
            else if (phieuMuaDuTru.DuTruMuaVatTuTheoKhoa != null && phieuMuaDuTru.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null && phieuMuaDuTru.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.LyDoGiamDocTuChoi;
            }
            else if (phieuMuaDuTru.DuTruMuaVatTuKhoDuoc != null && phieuMuaDuTru.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.DuTruMuaVatTuKhoDuoc?.LyDoGiamDocTuChoi;
            }
            var model = phieuMuaDuTru.ToModel<DuTruMuaVatTuViewModel>();
            foreach (var item in model.DuTruMuaVatTuChiTiets)
            {
                item.TenLoaiVatTu = item.LaVatTuBHYT ? "BHYT" : "Không BHYT";
            }
            return Ok(model);
        }
        #endregion

        #region Update
        [HttpPost("GuiLaiPhieuMuaVatTuDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu)]
        public async Task<ActionResult> GuiLaiPhieuMuaVatTuDuTru(DuTruMuaVatTuViewModel duTruMuaVatTuVM)
        {
            if (!duTruMuaVatTuVM.DuTruMuaVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DuTruMuaVatTu.DuTruMuaVatTuChiTiets.Required"));
            }
            var phieuMuaDuTru = await _yeuCauMuaDuTruVatTuService
                .GetByIdAsync(duTruMuaVatTuVM.Id, s =>
                            s.Include(r => r.DuTruMuaVatTuChiTiets).ThenInclude(ct => ct.VatTu)
                             .Include(r => r.KyDuTruMuaDuocPhamVatTu).ThenInclude(ct => ct.NhanVien).ThenInclude(dpct => dpct.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                             .Include(r => r.TruongKhoa).ThenInclude(tk => tk.User)
                             .Include(r => r.Kho)
                             .Include(r => r.DuTruMuaVatTuTheoKhoa).ThenInclude(dptk => dptk.DuTruMuaVatTuTheoKhoaChiTiets)
                             .Include(r => r.DuTruMuaVatTuKhoDuoc).ThenInclude(dptk => dptk.DuTruMuaVatTuKhoDuocChiTiets)
                             .Include(r => r.DuTruMuaVatTuTheoKhoa).ThenInclude(dptk => dptk.NhanVienKhoDuoc).ThenInclude(dpct => dpct.User)
                             .Include(r => r.DuTruMuaVatTuKhoDuoc).ThenInclude(dptk => dptk.GiamDoc).ThenInclude(dpct => dpct.User)
                             );
            if (phieuMuaDuTru == null)
            {
                return NotFound();
            }
            var duTruMuaDuocPham = duTruMuaVatTuVM.ToEntity(phieuMuaDuTru);
            await _yeuCauMuaDuTruVatTuService.UpdateAsync(duTruMuaDuocPham);
            var result = new
            {
                phieuMuaDuTru.Id,
                phieuMuaDuTru.LastModified
            };
            return Ok(result);
        }
        #endregion

        #region Delete
        [HttpPost("XoaYeuCauMuaDuTruVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _yeuCauMuaDuTruVatTuService.GetByIdAsync(id, s =>
                            s.Include(r => r.DuTruMuaVatTuChiTiets).ThenInclude(ct => ct.VatTu).ThenInclude(dp => dp.NhomVatTu));
            await _yeuCauMuaDuTruVatTuService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion

        #endregion

        [HttpPost("InPhieuMuaDuTruVatTu")]
        public string InPhieuMuaDuTruVatTu(PhieuMuaDuTruVatTu phieuMuaDuTruVatTu)
        {
            var result = _yeuCauMuaDuTruVatTuService.InPhieuMuaDuTruVatTu(phieuMuaDuTruVatTu);
            return result;
        }

        [HttpPost("ExportYeuCauMuaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu)]
        public async Task<ActionResult> ExportYeuCauMuaVatTu(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataForGridAsync(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (YeuCauMuaVatTuGridVo)p).ToList();
            var excelData = chucVuData.Map<List<YeuCauDuTruVatTuExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(YeuCauDuTruVatTuExcel.SoPhieu), "Số Phiếu"),
                (nameof(YeuCauDuTruVatTuExcel.TenKho), "Kho"),
                (nameof(YeuCauDuTruVatTuExcel.KyDuTru), "Kỳ dự trù"),
                (nameof(YeuCauDuTruVatTuExcel.NgayYeuCauDisplay), "Ngày yêu cầu"),
                (nameof(YeuCauDuTruVatTuExcel.NhanVienYeuCau), "Người yêu cầu"),
                (nameof(YeuCauDuTruVatTuExcel.TinhTrang), "Tình trạng"),
                (nameof(YeuCauDuTruVatTuExcel.NgayTaiKhoaDisplay), "Ngày T.Khoa Duyệt"),
                (nameof(YeuCauDuTruVatTuExcel.NgayTaiKhoDuocDisplay), "Ngày K.Dược Duyệt"),
                (nameof(YeuCauDuTruVatTuExcel.NgayTaiGiamDocDisplay), "Ngày G.Đốc Duyệt")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dự trù mua vật tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=YeuCauDuTruMuaVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
