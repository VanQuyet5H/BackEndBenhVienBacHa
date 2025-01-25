using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauHoanTraVatTu;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
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
    public partial class YeuCauTraVatTuController
    {
        [HttpPost("GetDataForGridAsyncVatTuTuTrucDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncVatTuTuTrucDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraVatTuService.GetDataForGridAsyncVatTuTuTrucDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncVatTuTuTrucDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncVatTuTuTrucDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraVatTuService.GetTotalPageForGridAsyncVatTuTuTrucDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridChildAsyncDaDuyetVatTu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsyncDaDuyetVatTu([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraVatTuService.GetDataForGridChildAsyncDaDuyetVatTu(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridChildAsyncDaDuyetVatTu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsyncDaDuyetVatTu([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraVatTuService.GetTotalPageForGridChildAsyncDaDuyetVatTu(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetTrangThaiYeuCauHoanTraVT")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiYeuCauHoanTraVT(long yeuCauTraVTId)
        {
            var result = await _ycHoanTraVatTuService.GetTrangThaiYeuCauHoanTraVT(yeuCauTraVTId);
            return Ok(result);
        }

        [HttpPost("XuatVTHoanTraTheoNhom")]
        public async Task<ActionResult> XuatVTHoanTraTheoNhom(YeuCauHoanTraVatTuChiTietTheoKhoXuatVos model)
        {
            return Ok(model);
        }

        #region CRUD

        [HttpPost("ThemYeuCauHoanTraVatTuTuTruc")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult> ThemYeuCauHoanTraVatTuTuTruc(YeuCauHoanTraVatTuTuTrucViewModel model)
        {
            if (!model.YeuCauHoanTraVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));
            }
            if (model.YeuCauHoanTraVatTuChiTiets.All(z => z.SoLuongTra == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var yeuCauhoanTra = model.ToEntity<YeuCauTraVatTu>();
            await _ycHoanTraVatTuService.XuLyThemHoacCapNhatHoanTraVTAsync(yeuCauhoanTra, model.YeuCauHoanTraVatTuChiTiets);
            return Ok(yeuCauhoanTra.Id);
        }


        [HttpGet("GetYeuCauHoanTraVatTuTuTruc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<YeuCauHoanTraVatTuTuTrucViewModel>> GetYeuCauHoanTraVatTuTuTruc(long id)
        {
            var yeuCauhoanTra = await _ycHoanTraVatTuService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.VatTuBenhVien).ThenInclude(dpct => dpct.VatTus)
                             .Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(dpct => dpct.XuatKhoVatTuChiTiet).ThenInclude(dp => dp.XuatKhoVatTu)
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
            var model = yeuCauhoanTra.ToModel<YeuCauHoanTraVatTuTuTrucViewModel>();
            model.YeuCauHoanTraVatTuChiTietHienThis = await _ycHoanTraVatTuService.YeuCauHoanTraVatTuChiTiets(id);
            return Ok(model);
        }

        [HttpPost("CapNhatYeuCauHoanTraVatTuTuTruc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult> CapNhatYeuCauHoanTraVatTuTuTruc(YeuCauHoanTraVatTuTuTrucViewModel yeuCauhoanTraVM)
        {
            if (!yeuCauhoanTraVM.YeuCauHoanTraVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DieuChuyenNoiBoDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Required"));
            }
            if (yeuCauhoanTraVM.YeuCauHoanTraVatTuChiTiets.All(z => z.SoLuongTra == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var yeuCauhoanTra = await _ycHoanTraVatTuService
                .GetByIdAsync(yeuCauhoanTraVM.Id, s =>
                            s.Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.VatTuBenhVien).ThenInclude(dpct => dpct.VatTus)
                             .Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(dpct => dpct.XuatKhoVatTuChiTiet).ThenInclude(dp => dp.XuatKhoVatTu)
                             .Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(ct => ct.NhapKhoVatTuChiTiet)
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
            await _ycHoanTraVatTuService.XuLyThemHoacCapNhatHoanTraVTAsync(yeuCauhoanTra, yeuCauhoanTraVM.YeuCauHoanTraVatTuChiTiets, false);
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
