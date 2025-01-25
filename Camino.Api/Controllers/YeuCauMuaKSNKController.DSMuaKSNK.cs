using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaKSNK;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.VatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauMuaKSNK;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaKSNKController
    {
        #region DS Kiểm Soát Nhiễm Khuẩn

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetYeuCauMuaKSNKDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauMuaKSNKDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetYeuCauMuaKSNKTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauMuaKSNKTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        [HttpPost("GetKyDuTruKSNK")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKyDuTruKSNK(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetKyDuTruKSNK(model);
            return Ok(lookup);
        }

        [HttpPost("GetKSNKMuaDuTru")]
        public async Task<ActionResult<ICollection<KSNKTemplateLookupItem>>> GetKSNKMuaDuTru(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetKSNKMuaDuTrus(model);
            return Ok(lookup);
        }

        [HttpPost("GetNhomKSNKTreeView")]
        public async Task<ActionResult<List<LookupTreeItemVo>>> GetNhomKSNKTreeView(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetNhomKSNKTreeView(model);
            return Ok(lookup);
        }

        [HttpPost("ThongTinKSNK")]
        public ActionResult ThongTinKSNK(ThongTinChiTietKSNKTonKho thongTinThuocVM)
        {
            var entity = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.ThongTinDuTruMuaKSNK(thongTinThuocVM);
            return Ok(entity);
        }

        [HttpGet("GetTrangThaiPhieuMuaKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult<TrangThaiDuyetDuTruMuaVo>> GetTrangThaiPhieuMuaKSNK(long phieuMuaKSNKId)
        {
            var result = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTrangThaiPhieuMuaKSNK(phieuMuaKSNKId);
            return Ok(result);
        }

        #region Xử lý Kiểm Soát Nhiễm Khuẩn

        [HttpPost("ThemKSNKChiTietGridVo")]
        public async Task<ActionResult> ThemKSNKChiTietGridVo(KSNKDuTruGridViewModel model)
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

        #region Add Kiểm Soát Nhiễm Khuẩn

        [HttpPost("GuiPhieuMuaKSNKDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult> GuiPhieuMuaKSNKDuTru(DuTruMuaKSNKViewModel duTruMuaVatTuVM)
        {
            if (!duTruMuaVatTuVM.DuTruMuaVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DuTruMuaVatTu.DuTruMuaVatTuChiTiets.Required"));
            }
            var getKyDuTruMuaDuocPhamVatTuVo = await _yeuCauMuaDuTruDuocPhamService.GetKyDuTruMuaDuocPhamVatTu(duTruMuaVatTuVM.KyDuTruMuaDuocPhamVatTuId.Value);
            duTruMuaVatTuVM.TuNgay = getKyDuTruMuaDuocPhamVatTuVo.TuNgay;
            duTruMuaVatTuVM.DenNgay = getKyDuTruMuaDuocPhamVatTuVo.DenNgay;
            var soPhieu = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetSoPhieuDuTruKSNK();
            duTruMuaVatTuVM.SoPhieu = soPhieu;
            var duTruMuaVatTu = duTruMuaVatTuVM.ToEntity<DuTruMuaVatTu>();
            await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.AddAsync(duTruMuaVatTu);
            return Ok(duTruMuaVatTu.Id);
        }
        #endregion

        #region Get

        [HttpGet("GetPhieuMuaKSNKDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult<DuTruMuaKSNKViewModel>> Get(long id)
        {
            var phieuMuaDuTru = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService
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
            var model = phieuMuaDuTru.ToModel<DuTruMuaKSNKViewModel>();
            foreach (var item in model.DuTruMuaVatTuChiTiets)
            {
                item.TenLoaiVatTu = item.LaVatTuBHYT ? "BHYT" : "Không BHYT";
            }
            return Ok(model);
        }
        #endregion

        #region Update

        [HttpPost("GuiLaiPhieuMuaKSNKDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult> GuiLaiPhieuMuaKSNKDuTru(DuTruMuaKSNKViewModel duTruMuaVatTuVM)
        {
            if (!duTruMuaVatTuVM.DuTruMuaVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DuTruMuaVatTu.DuTruMuaVatTuChiTiets.Required"));
            }
            var phieuMuaDuTru = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService
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
            await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.UpdateAsync(duTruMuaDuocPham);
            var result = new
            {
                phieuMuaDuTru.Id,
                phieuMuaDuTru.LastModified
            };
            return Ok(result);
        }

        #endregion

        #region Delete

        [HttpPost("XoaYeuCauMuaDuTruKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetByIdAsync(id, s =>
                            s.Include(r => r.DuTruMuaVatTuChiTiets).ThenInclude(ct => ct.VatTu).ThenInclude(dp => dp.NhomVatTu));
            await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion

        #endregion

        [HttpPost("InPhieuMuaDuTruKSNK")]
        public string InPhieuMuaDuTruVatTu(PhieuMuaDuTruKSNK phieuMuaDuTruVatTu)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuMuaDuTruKSNK(phieuMuaDuTruVatTu);
            return result;
        }

        [HttpPost("InPhieuMuaDuTruDuocPhamKSNK")]
        public string InPhieuMuaDuTruDuocPhamKSNK(PhieuMuaDuTruKSNK phieuMuaDuTruVatTu)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuMuaDuTruDuocPham(phieuMuaDuTruVatTu);
            return result;
        }


        [HttpPost("InPhieuMuaDuTruVatTuVaDuocPhamKSNK")]
        public string InPhieuMuaDuTruVatTuVaDuocPhamKSNK(PhieuMuaDuTruKSNK phieuMuaDuTruVatTu)
        {
            var htmlAll = string.Empty;
            if (phieuMuaDuTruVatTu != null && phieuMuaDuTruVatTu.DuTruMuaVatTuId != null)
            {
                htmlAll = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuMuaDuTruKSNK(phieuMuaDuTruVatTu) + "<div class=\"pagebreak\"> </div>"; 
            }

            if (phieuMuaDuTruVatTu != null && phieuMuaDuTruVatTu.DuTruMuaDuocPhamId != null)
            {
                htmlAll += _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuMuaDuTruDuocPham(phieuMuaDuTruVatTu)  ;
            }

            return htmlAll;
        }

        [HttpPost("ExportYeuCauMuaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK)]
        public async Task<ActionResult> ExportYeuCauMuaVatTu(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataForGridAsync(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (YeuCauMuaKSNKGridVo)p).ToList();
            var excelData = chucVuData.Map<List<YeuCauDuTruKSNKExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(YeuCauDuTruKSNKExcel.SoPhieu), "Số Phiếu"),
                (nameof(YeuCauDuTruKSNKExcel.TenKho), "Kho"),
                (nameof(YeuCauDuTruKSNKExcel.KyDuTru), "Kỳ dự trù"),
                (nameof(YeuCauDuTruKSNKExcel.NgayYeuCauDisplay), "Ngày yêu cầu"),
                (nameof(YeuCauDuTruKSNKExcel.NhanVienYeuCau), "Người yêu cầu"),
                (nameof(YeuCauDuTruKSNKExcel.TinhTrang), "Tình trạng"),
                (nameof(YeuCauDuTruKSNKExcel.NgayTaiKhoaDisplay), "Ngày T.Khoa Duyệt"),
                (nameof(YeuCauDuTruKSNKExcel.NgayTaiKhoDuocDisplay), "Ngày K.Dược Duyệt"),
                (nameof(YeuCauDuTruKSNKExcel.NgayTaiGiamDocDisplay), "Ngày G.Đốc Duyệt")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dự trù mua kiểm soát nhiễm khuẩn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=YeuCauDuTruMuaKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
