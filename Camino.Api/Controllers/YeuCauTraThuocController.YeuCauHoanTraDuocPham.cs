using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauHoanTraDuocPham;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraThuocController

    {
        [HttpPost("GetDataForGridAsyncDuocPhamTuTrucDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDuocPhamTuTrucDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetDataForGridAsyncDuocPhamTuTrucDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDuocPhamTuTrucDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDuocPhamTuTrucDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetTotalPageForGridAsyncDuocPhamTuTrucDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridChildAsyncDaDuyet")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsyncDaDuyet([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetDataForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridChildAsyncDaDuyet")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsyncDaDuyet([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetTotalPageForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetTrangThaiYeuCauHoanTraDuocPham")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiYeuCauHoanTraDuocPham(long yeuCauTraDuocPhamId)
        {
            var result = await _ycHoanTraDuocPhamService.GetTrangThaiYeuCauHoanTraDuocPham(yeuCauTraDuocPhamId);
            return Ok(result);
        }

        [HttpPost("XuatDuocPhamHoanTraTheoNhom")]
        public async Task<ActionResult> XuatDuocPhamHoanTraTheoNhom(YeuCauHoanTraDuocPhamChiTietTheoKhoXuatVos model)
        {
            return Ok(model);
        }

        #region CRUD

        [HttpPost("ThemYeuCauHoanTraDuocPhamTuTruc")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult> ThemYeuCauHoanTraDuocPhamTuTruc(YeuCauHoanTraDuocPhamTuTrucViewModel model)
        {
            if (!model.YeuCauHoanTraDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));
            }
            if (model.YeuCauHoanTraDuocPhamChiTiets.All(z => z.SoLuongTra == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var yeuCauhoanTra = model.ToEntity<YeuCauTraDuocPham>();
            await _ycHoanTraDuocPhamService.XuLyThemHoacCapNhatHoanTraThuocAsync(yeuCauhoanTra, model.YeuCauHoanTraDuocPhamChiTiets);
            return Ok(yeuCauhoanTra.Id);
        }

        [HttpGet("GetYeuCauHoanTraDuocPhamTuTruc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<YeuCauHoanTraDuocPhamTuTrucViewModel>> GetYeuCauHoanTraDuocPhamTuTruc(long id)
        {
            var yeuCauhoanTra = await _ycHoanTraDuocPhamService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham)
                             .Include(r => r.KhoNhap)
                             .Include(r => r.KhoXuat)
                             .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(r => r.User)
                             )
                             ;
            if (yeuCauhoanTra == null)
            {
                return NotFound();
            }
            var model = yeuCauhoanTra.ToModel<YeuCauHoanTraDuocPhamTuTrucViewModel>();
            model.YeuCauHoanTraDuocPhamChiTietHienThis = await _ycHoanTraDuocPhamService.YeuCauTraDuocPhamTuTrucChiTiets(id);
            return Ok(model);
        }


        [HttpPost("CapNhatYeuCauHoanTraDuocPhamTuTruc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult> CapNhatYeuCauHoanTraDuocPhamTuTruc(YeuCauHoanTraDuocPhamTuTrucViewModel yeuCauhoanTraVM)
        {
            if (!yeuCauhoanTraVM.YeuCauHoanTraDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DieuChuyenNoiBoDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Required"));
            }
            if (yeuCauhoanTraVM.YeuCauHoanTraDuocPhamChiTiets.All(z => z.SoLuongTra == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var yeuCauhoanTra = await _ycHoanTraDuocPhamService
                .GetByIdAsync(yeuCauhoanTraVM.Id, s =>
                            s.Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham)
                             .Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.NhapKhoDuocPhamChiTiet)
                             .Include(r => r.KhoNhap)
                             .Include(r => r.KhoXuat)
                             .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(r => r.User)
                             )
                             ;

            if (yeuCauhoanTra == null)
            {
                throw new ApiException("Yêu cầu hoàn trả này không tồn tại.");
            }
            if (yeuCauhoanTra.DuocDuyet != null)
            {
                throw new ApiException("Yêu cầu hoàn trả này đã được duyệt.");
            }
            yeuCauhoanTraVM.ToEntity(yeuCauhoanTra);
            await _ycHoanTraDuocPhamService.XuLyThemHoacCapNhatHoanTraThuocAsync(yeuCauhoanTra, yeuCauhoanTraVM.YeuCauHoanTraDuocPhamChiTiets, false);
            var result = new
            {
                yeuCauhoanTra.Id,
                yeuCauhoanTra.LastModified
            };
            return Ok(result);
        }
        #endregion
    }
}
