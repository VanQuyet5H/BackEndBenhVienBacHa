using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaDuocPham;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaDuocPhamController
    {
        #region Ds yeu cau mua duoc pham
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetYeuCauMuaDuocPhamDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauMuaDuocPhamDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetYeuCauMuaDuocPhamTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauMuaDuocPhamTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        [HttpPost("GetNhomDuocPhamDuTru")]
        public ActionResult<ICollection<LookupItemVo>> GetNhomDuocPhamDuTru(DropDownListRequestModel model)
        {
            var lookup = _yeuCauMuaDuTruDuocPhamService.NhomDuocPhamDuTru(model);
            return Ok(lookup);
        }

        [HttpPost("GetNhomDuocPhamDieuTriDuPhong")]
        public ActionResult<ICollection<LookupItemVo>> GetNhomDuocPhamDieuTriDuPhong(DropDownListRequestModel model)
        {
            var lookup = _yeuCauMuaDuTruDuocPhamService.NhomDuocPhamDieuTriDuPhong(model);
            return Ok(lookup);
        }

        [HttpPost("GetKho")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKho(DropDownListRequestModel model, bool laDuocPham)
        {
            var lookup = await _yeuCauMuaDuTruDuocPhamService.GetKhoCurrentUser(model, laDuocPham);
            return Ok(lookup);
        }

        [HttpPost("GetKyDuTru")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKyDuTru(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruDuocPhamService.GetKyDuTru(model);
            return Ok(lookup);
        }

        [HttpPost("GetDuocPhamMuaDuTru")]
        public async Task<ActionResult<ICollection<DuocPhamTemplateLookupItem>>> GetDuocPhamMuaDuTru(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauMuaDuTruDuocPhamService.GetDuocPhamMuaDuTrus(model);
            return Ok(lookup);
        }

        [HttpPost("ThongTinDuocPham")]
        public ActionResult ThongTinDuocPham(ThongTinChiTietDuocPhamTonKho thongTinThuocVM)
        {
            var entity = _yeuCauMuaDuTruDuocPhamService.ThongTinDuTruMuaDuocPham(thongTinThuocVM);
            return Ok(entity);
        }

        [HttpGet("GetTrangThaiPhieuDuocPhamDuTru")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult<TrangThaiDuyetDuTruMuaVo>> GetTrangThaiPhieuDuocPhamDuTru(long phieuMuaDuocPhamId)
        {
            var result = await _yeuCauMuaDuTruDuocPhamService.GetTrangThaiPhieuMua(phieuMuaDuocPhamId);
            return Ok(result);
        }

        #region XuLy
        [HttpPost("ThemDuocPhamChiTietGridVo")]
        public async Task<ActionResult> ThemDuocPhamChiTietGridVo(DuocPhamDuTruGridViewModel model)
        {
            if (model.IsThemDuocPham)
            {
                if (!await _thuocBenhVienService.CheckDuongDungAsync(model.DuongDungId ?? 0))
                    throw new ApiException(_localizationService.GetResource("DuocPham.DuongDung.NotExists"), (int)HttpStatusCode.BadRequest);
                if (!await _thuocBenhVienService.CheckDVTAsync(model.DonViTinhId ?? 0))
                    throw new ApiException(_localizationService.GetResource("DuocPham.DVT.NotExists"), (int)HttpStatusCode.BadRequest);

                model.MaHoatChat = "Chưa nhập";
                model.HoatChat = "Chưa nhập";
                model.LoaiThuocHoacHoatChat = Enums.LoaiThuocHoacHoatChat.ThuocTanDuoc;
                model.SoDangKy = await _yeuCauKhamBenhService.GetSoDangKyDuocPhamNgoaiBv();
                var thuocBenhVien = model.ToEntity<DuocPham>();
                _thuocBenhVienService.Add(thuocBenhVien);
                if (model.LoaiDuocPham == 1)
                {
                    model.TenLoaiDuocPham = "Không BHYT";
                    model.NhomDieuTriDuTru = model.NhomDieuTriDuPhong != null ? (model.NhomDieuTriDuPhong == 1 ? "Điều trị" : "Dự phòng") : null;
                    model.DuocPhamId = thuocBenhVien.Id;
                    model.LaDuocPhamBHYT = false;
                    return Ok(model);
                }
                else
                {
                    model.TenLoaiDuocPham = "BHYT";
                    model.NhomDieuTriDuTru = model.NhomDieuTriDuPhong != null ? (model.NhomDieuTriDuPhong == 1 ? "Điều trị" : "Dự phòng") : null;
                    model.DuocPhamId = thuocBenhVien.Id;
                    model.LaDuocPhamBHYT = true;
                    return Ok(model);
                }

            }
            else
            {
                model.TenLoaiDuocPham = model.LoaiDuocPham == 2 ? "BHYT" : "Không BHYT";
                model.NhomDieuTriDuTru = model.NhomDieuTriDuPhong != null ? (model.NhomDieuTriDuPhong == 1 ? "Điều trị" : "Dự phòng") : null;
                return Ok(model);
            }

        }
        #region Add
        [HttpPost("GuiPhieuMuaDuocPhamDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult> GuiPhieuMuaDuocPhamDuTru(DuTruMuaDuocPhamViewModel duTruMuaDuocPhamVM)
        {
            if (!duTruMuaDuocPhamVM.DuTruMuaDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DuTruMuaDuocPham.DuTruMuaDuocPhamChiTiets.Required"));
            }
            var getKyDuTruMuaDuocPhamVatTuVo = await _yeuCauMuaDuTruDuocPhamService.GetKyDuTruMuaDuocPhamVatTu(duTruMuaDuocPhamVM.KyDuTruMuaDuocPhamVatTuId.Value);
            duTruMuaDuocPhamVM.TuNgay = getKyDuTruMuaDuocPhamVatTuVo.TuNgay;
            duTruMuaDuocPhamVM.DenNgay = getKyDuTruMuaDuocPhamVatTuVo.DenNgay;
            var soPhieu = await _yeuCauMuaDuTruDuocPhamService.GetSoPhieuDuTru();
            duTruMuaDuocPhamVM.SoPhieu = soPhieu;
            var duTruMuaDuocPham = duTruMuaDuocPhamVM.ToEntity<DuTruMuaDuocPham>();
            await _yeuCauMuaDuTruDuocPhamService.AddAsync(duTruMuaDuocPham);
            return Ok(duTruMuaDuocPham.Id);
        }
        #endregion

        #region Get
        [HttpGet("GetPhieuMuaDuocPhamDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult<DuTruMuaDuocPhamViewModel>> Get(long id)
        {
            var phieuMuaDuTru = await _yeuCauMuaDuTruDuocPhamService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DuongDung)
                             .Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.KyDuTruMuaDuocPhamVatTu).ThenInclude(ct => ct.NhanVien).ThenInclude(dpct => dpct.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                             .Include(r => r.TruongKhoa).ThenInclude(tk => tk.User)
                             .Include(r => r.Kho)
                             .Include(r => r.DuTruMuaDuocPhamTheoKhoa).ThenInclude(dptk => dptk.DuTruMuaDuocPhamKhoDuoc)
                             .Include(r => r.DuTruMuaDuocPhamTheoKhoa).ThenInclude(dptk => dptk.DuTruMuaDuocPhamTheoKhoaChiTiets)
                             .Include(r => r.DuTruMuaDuocPhamKhoDuoc).ThenInclude(dptk => dptk.DuTruMuaDuocPhamKhoDuocChiTiets)
                             .Include(r => r.DuTruMuaDuocPhamTheoKhoa).ThenInclude(dptk => dptk.NhanVienKhoDuoc).ThenInclude(dpct => dpct.User)
                             .Include(r => r.DuTruMuaDuocPhamKhoDuoc).ThenInclude(dptk => dptk.GiamDoc).ThenInclude(dpct => dpct.User)
                             );
            if (phieuMuaDuTru == null)
            {
                return NotFound();
            }
            if (phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa == null && phieuMuaDuTru.DuTruMuaDuocPhamKhoDuoc == null && phieuMuaDuTru.TruongKhoaDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.LyDoTruongKhoaTuChoi;
            }
            else if (phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa != null && phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa.LyDoKhoDuocTuChoi;
            }
            else if (phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa != null && phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null && phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.LyDoGiamDocTuChoi;
            }
            else if (phieuMuaDuTru.DuTruMuaDuocPhamKhoDuoc != null && phieuMuaDuTru.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false)
            {
                phieuMuaDuTru.LyDoTruongKhoaTuChoi = phieuMuaDuTru.DuTruMuaDuocPhamKhoDuoc?.LyDoGiamDocTuChoi;
            }
          
            var model = phieuMuaDuTru.ToModel<DuTruMuaDuocPhamViewModel>();
            foreach (var item in model.DuTruMuaDuocPhamChiTiets)
            {
                item.TenLoaiDuocPham = item.LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
                item.NhomDieuTriDuTru = item.NhomDieuTriDuPhong != null ? (item.NhomDieuTriDuPhong == EnumNhomDieuTriDuPhong.DieuTri ? "Điều trị" : "Dự phòng") : null;
            }
            return Ok(model);
        }
        #endregion

        #region Update
        [HttpPost("GuiLaiPhieuMuaDuocPhamDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult> GuiLaiPhieuMuaDuocPhamDuTru(DuTruMuaDuocPhamViewModel duTruMuaDuocPhamVM)
        {
            if (!duTruMuaDuocPhamVM.DuTruMuaDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DuTruMuaDuocPham.DuTruMuaDuocPhamChiTiets.Required"));
            }
            var phieuMuaDuTru = await _yeuCauMuaDuTruDuocPhamService
                .GetByIdAsync(duTruMuaDuocPhamVM.Id, s =>
                            s.Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DuongDung)
                             .Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.KyDuTruMuaDuocPhamVatTu).ThenInclude(ct => ct.NhanVien).ThenInclude(dpct => dpct.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                             .Include(r => r.TruongKhoa).ThenInclude(tk => tk.User)
                             .Include(r => r.Kho)
                             .Include(r => r.DuTruMuaDuocPhamTheoKhoa).ThenInclude(dptk => dptk.DuTruMuaDuocPhamTheoKhoaChiTiets)
                             .Include(r => r.DuTruMuaDuocPhamKhoDuoc).ThenInclude(dptk => dptk.DuTruMuaDuocPhamKhoDuocChiTiets)
                             .Include(r => r.DuTruMuaDuocPhamTheoKhoa).ThenInclude(dptk => dptk.NhanVienKhoDuoc).ThenInclude(dpct => dpct.User)
                             .Include(r => r.DuTruMuaDuocPhamKhoDuoc).ThenInclude(dptk => dptk.GiamDoc).ThenInclude(dpct => dpct.User)
                             );
            if (phieuMuaDuTru == null)
            {
                return NotFound();
            }
            var duTruMuaDuocPham = duTruMuaDuocPhamVM.ToEntity(phieuMuaDuTru);
            await _yeuCauMuaDuTruDuocPhamService.UpdateAsync(duTruMuaDuocPham);
            var result = new
            {
                phieuMuaDuTru.Id,
                phieuMuaDuTru.LastModified
            };
            return Ok(result);
        }
        #endregion

        #region Delete
        [HttpPost("XoaYeuCauMuaDuTru")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _yeuCauMuaDuTruDuocPhamService.GetByIdAsync(id, s =>
                            s.Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DuongDung)
                             .Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.KyDuTruMuaDuocPhamVatTu).ThenInclude(ct => ct.NhanVien).ThenInclude(dpct => dpct.User)
                             );
            if (entity == null)
            {
                return NotFound();
            }
            await _yeuCauMuaDuTruDuocPhamService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion

        #endregion

        [HttpPost("InPhieuDuTruMuaDuocPham")]
        public string InPhieuDuTruMuaDuocPham(PhieuMuaDuTruDuocPham phieuMuaDuTruDuocPham)
        {
            var result = _yeuCauMuaDuTruDuocPhamService.InPhieuMuaDuTruDuocPham(phieuMuaDuTruDuocPham);
            return result;
        }

        [HttpPost("ExportYeuCauMuaDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham)]
        public async Task<ActionResult> ExportYeuCauMuaDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataForGridAsync(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (YeuCauDuTruDuocPhamGridVo)p).ToList();
            var excelData = chucVuData.Map<List<YeuCauDuTruDuocPhamExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(YeuCauDuTruDuocPhamExcel.SoPhieu), "Số Phiếu"),
                (nameof(YeuCauDuTruDuocPhamExcel.TenNhomDuTru), "Nhóm"),
                (nameof(YeuCauDuTruDuocPhamExcel.TenKho), "Kho"),
                (nameof(YeuCauDuTruDuocPhamExcel.KyDuTru), "Kỳ dự trù"),
                (nameof(YeuCauDuTruDuocPhamExcel.NgayYeuCauDisplay), "Ngày yêu cầu"),
                (nameof(YeuCauDuTruDuocPhamExcel.NhanVienYeuCau), "Người yêu cầu"),
                (nameof(YeuCauDuTruDuocPhamExcel.TinhTrang), "Tình trạng"),
                (nameof(YeuCauDuTruDuocPhamExcel.NgayTaiKhoaDisplay), "Ngày T.Khoa Duyệt"),
                (nameof(YeuCauDuTruDuocPhamExcel.NgayTaiKhoDuocDisplay), "Ngày K.Dược Duyệt"),
                (nameof(YeuCauDuTruDuocPhamExcel.NgayTaiGiamDocDisplay), "Ngày G.Đốc Duyệt")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dự trù mua dược phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=YeuCauDuTruMuaDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
