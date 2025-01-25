using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Localization;
using Camino.Services.NoiDungGhiChuMiemGiam;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{

    public class NoiDungGhiChuMiemGiamController : CaminoBaseController
    {
        private readonly INoiDungGhiChuMiemGiamService _noiDungGhiChuMiemGiamService;
        private readonly ILocalizationService _localizationService;

        public NoiDungGhiChuMiemGiamController(INoiDungGhiChuMiemGiamService noiDungGhiChuMiemGiamService,
               ILocalizationService localizationService)
        {
            _noiDungGhiChuMiemGiamService = noiDungGhiChuMiemGiamService;
            _localizationService = localizationService;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNoiDungGhiChuMiemGiam")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNoiDungGhiChuMiemGiamAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiDungGhiChuMiemGiamService.GetDataForGridNoiDungGhiChuMiemGiamAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridNoiDungGhiChuMiemGiam")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNoiDungGhiChuMiemGiamAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiDungGhiChuMiemGiamService.GetTotalPageForGridNoiDungGhiChuMiemGiamAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetThongTinNoiDungGhiChuMiemGiam")]
        public async Task<ActionResult<NoiDungGhiChuMiemGiamViewModel>> GetThongTinNoiDungGhiChuMiemGiam(long id)
        {
            var result = await _noiDungGhiChuMiemGiamService.GetByIdAsync(id);
            return result.ToModel<NoiDungGhiChuMiemGiamViewModel>();
        }

        [HttpPost("GetListNoiDungMau")]
        public async Task<ActionResult<ICollection<NoiDungGhiChuMiemGiamLookupItemVo>>> GetListNoiDungMauAsync(DropDownListRequestModel model)
        {
            var lookup = await _noiDungGhiChuMiemGiamService.GetListNoiDungMauAsync(model);
            return Ok(lookup);
        }

        [HttpPost("LuuNoiDungGhiChuMiemGiam")]
        public async Task<ActionResult> LuuNoiDungGhiChuMiemGiam([FromBody] NoiDungGhiChuMiemGiamViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                var noiDungMauLoiDanBS = await _noiDungGhiChuMiemGiamService.GetByIdAsync(viewModel.Id,
                    x => x.Include(a => a.YeuCauKhamBenhs)
                      .Include(a => a.YeuCauDichVuKyThuats)
                      .Include(a => a.YeuCauDuocPhamBenhViens)
                      .Include(a => a.YeuCauVatTuBenhViens)
                      .Include(a => a.DonThuocThanhToanChiTiets)
                      .Include(a => a.YeuCauDichVuGiuongBenhViens)
                      .Include(a => a.YeuCauGoiDichVus)
                      .Include(a => a.YeuCauTruyenMaus)
                      .Include(a => a.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                      );

                viewModel.ToEntity(noiDungMauLoiDanBS);

                if (noiDungMauLoiDanBS.YeuCauKhamBenhs.Any() )
                {
                    foreach (var yeuCauKhamBenhs in noiDungMauLoiDanBS.YeuCauKhamBenhs)
                    {
                        yeuCauKhamBenhs.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.YeuCauDichVuKyThuats.Any())
                {
                    foreach (var yeuCauDichVuKyThuat in noiDungMauLoiDanBS.YeuCauDichVuKyThuats)
                    {
                        yeuCauDichVuKyThuat.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.YeuCauDuocPhamBenhViens.Any())
                {
                    foreach (var yeuCauDuocPhamBenhVien in noiDungMauLoiDanBS.YeuCauDuocPhamBenhViens)
                    {
                        yeuCauDuocPhamBenhVien.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.YeuCauVatTuBenhViens.Any())
                {
                    foreach (var yeuCauVatTuBenhVien in noiDungMauLoiDanBS.YeuCauVatTuBenhViens)
                    {
                        yeuCauVatTuBenhVien.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.DonThuocThanhToanChiTiets.Any())
                {
                    foreach (var donThuocThanhToanChiTiet in noiDungMauLoiDanBS.DonThuocThanhToanChiTiets)
                    {
                        donThuocThanhToanChiTiet.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.YeuCauDichVuGiuongBenhViens.Any())
                {
                    foreach (var yeuCauDichVuGiuongBenhVien in noiDungMauLoiDanBS.YeuCauDichVuGiuongBenhViens)
                    {
                        yeuCauDichVuGiuongBenhVien.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.YeuCauGoiDichVus.Any())
                {
                    foreach (var yeuCauGoiDichVu in noiDungMauLoiDanBS.YeuCauGoiDichVus)
                    {
                        yeuCauGoiDichVu.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.YeuCauTruyenMaus.Any())
                {
                    foreach (var yeuCauTruyenMaus in noiDungMauLoiDanBS.YeuCauTruyenMaus)
                    {
                        yeuCauTruyenMaus.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                if (noiDungMauLoiDanBS.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Any())
                {
                    foreach (var yeuCauDichVuGiuongBenhVienChiPhiBenhVien in noiDungMauLoiDanBS.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                    {
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.GhiChuMienGiamThem = viewModel.NoiDungMiemGiam;
                    }
                }

                await _noiDungGhiChuMiemGiamService.UpdateAsync(noiDungMauLoiDanBS);
            }
            else
            {
                var noiDungMauLoiDanBS = viewModel.ToEntity<NoiDungGhiChuMiemGiam>();
                await _noiDungGhiChuMiemGiamService.AddAsync(noiDungMauLoiDanBS);
            }

            return Ok();
        }

        [HttpDelete("XoaNoiDungGhiChuMiemGiam")]
        public async Task<ActionResult<NoiDungGhiChuMiemGiamViewModel>> XoaNoiDungGhiChuMiemGiam(long id)
        {
            var result = await _noiDungGhiChuMiemGiamService.GetByIdAsync(id,
                       x => x.Include(a => a.YeuCauKhamBenhs)
                      .Include(a => a.YeuCauDichVuKyThuats)
                      .Include(a => a.YeuCauDuocPhamBenhViens)
                      .Include(a => a.YeuCauVatTuBenhViens)
                      .Include(a => a.DonThuocThanhToanChiTiets)
                      .Include(a => a.YeuCauDichVuGiuongBenhViens)
                      .Include(a => a.YeuCauGoiDichVus)
                      .Include(a => a.YeuCauTruyenMaus)
                      .Include(a => a.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                      );

        
            if (result.YeuCauKhamBenhs.Any() || result.YeuCauDichVuKyThuats.Any() || result.YeuCauDuocPhamBenhViens.Any()
                 || result.YeuCauVatTuBenhViens.Any() || result.DonThuocThanhToanChiTiets.Any() || result.YeuCauDichVuGiuongBenhViens.Any()
                  || result.YeuCauGoiDichVus.Any() || result.YeuCauTruyenMaus.Any() || result.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Any())
            {
                throw new ArgumentException("Nội dung miễm giảm này đã được sử dụng");
            }

            if (result == null)
            {
                return NotFound();
            }
            await _noiDungGhiChuMiemGiamService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
