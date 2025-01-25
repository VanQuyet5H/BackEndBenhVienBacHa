using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [HttpGet("GetHdKhamVaDiaDiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult> GetHdKhamVaDiaDiemAsync(long id)
        {
            var gridData = await _khamDoanService.GetHdKhamVaDiaDiemAsync(id);
            return Ok(gridData);
        }

        [HttpGet("GetNhanVienRelatedInfo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult> GetNhanVienRelatedInfoAsync(long id)
        {
            var gridData = await _khamDoanService.GetNhanVienRelatedInfoAsync(id);
            return Ok(gridData);
        }

        [HttpPost("GetThongTinNhanSuConLai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult> GetThongTinNhanSuConLai([FromBody]YeuCauNhanSuKhamSucKhoeChiTietViewModel yeuCauNhanSuKhamSucKhoeViewModel)
        {
            var nhanVien = yeuCauNhanSuKhamSucKhoeViewModel.NhanVienId != null ?
                await _khamDoanService.GetNhanVienAsync(yeuCauNhanSuKhamSucKhoeViewModel.NhanVienId.GetValueOrDefault()) : string.Empty;
            var getLoaiNv = yeuCauNhanSuKhamSucKhoeViewModel.NhanVienId != null ?
                await _khamDoanService.GetLoaiNhanVienAsync(yeuCauNhanSuKhamSucKhoeViewModel.NhanVienId.GetValueOrDefault()) : 0;
            var nguoiGioiThieu = yeuCauNhanSuKhamSucKhoeViewModel.NguoiGioiThieuId != null ?
                await _khamDoanService.GetNhanVienAsync(yeuCauNhanSuKhamSucKhoeViewModel.NguoiGioiThieuId.GetValueOrDefault()) : string.Empty;

            yeuCauNhanSuKhamSucKhoeViewModel.HoTen = nhanVien;
            yeuCauNhanSuKhamSucKhoeViewModel.NguoiGioiThieu = nguoiGioiThieu;

            yeuCauNhanSuKhamSucKhoeViewModel.LoaiNhanVien = getLoaiNv == 1 ? LoaiNhanVien.BacSi :
                getLoaiNv == 5 ? LoaiNhanVien.DieuDuong : LoaiNhanVien.NhanVienKhac;

            return Ok(yeuCauNhanSuKhamSucKhoeViewModel);
        }

        [HttpPost("ThemYeuCauNhanSu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult> ThemYeuCauNhanSu([FromBody]YeuCauNhanSuKhamSucKhoeViewModel yeuCauNhanSuKhamSucKhoeViewModel)
        {
            var yeuCauNhanSuKhamSucKhoeEntity = yeuCauNhanSuKhamSucKhoeViewModel.ToEntity<YeuCauNhanSuKhamSucKhoe>();

            foreach (var ycNhanSuChiTietItem in yeuCauNhanSuKhamSucKhoeViewModel.YeuCauNhanSuKhamSucKhoeChiTiets)
            {
                var yeuCauNhanSuKhamSucKhoeChiTietEntity = ycNhanSuChiTietItem.ToEntity<YeuCauNhanSuKhamSucKhoeChiTiet>();
                yeuCauNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.Add(yeuCauNhanSuKhamSucKhoeChiTietEntity);
                if (ycNhanSuChiTietItem.NhanSuKhamSucKhoeTaiLieuDinhKem.Any())
                {
                    foreach (var item in ycNhanSuChiTietItem.NhanSuKhamSucKhoeTaiLieuDinhKem)
                    {
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                    }
                }
            }

            await _khamDoanService.ThemYeuCauNhanSuAsync(yeuCauNhanSuKhamSucKhoeEntity);

            return Ok();
        }

        [HttpGet("GetThongTinYeuCauNhanSuKhamSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult> GetThongTinYeuCauNhanSuKhamSucKhoeAsync(long id)
        {
            var ycNhanSuKhamSucKhoeEntity = await _khamDoanService.GetByYeuCauNhanSuKhamSucKhoeIdAsync(id,
                q => q.Include(w => w.HopDongKhamSucKhoe).ThenInclude(w => w.CongTyKhamSucKhoe)
                    .Include(w => w.HopDongKhamSucKhoe).ThenInclude(w => w.HopDongKhamSucKhoeDiaDiems)
                    .Include(w => w.YeuCauNhanSuKhamSucKhoeChiTiets).ThenInclude(w => w.NguoiGioiThieu).ThenInclude(w => w.User)
                    .Include(w => w.YeuCauNhanSuKhamSucKhoeChiTiets).ThenInclude(w => w.NhanSuKhamSucKhoeTaiLieuDinhKem)
                    .Include(w => w.YeuCauNhanSuKhamSucKhoeChiTiets).ThenInclude(w => w.NhanVien).ThenInclude(w => w.ChucDanh).ThenInclude(w => w.NhomChucDanh)
                    .Include(w => w.YeuCauNhanSuKhamSucKhoeChiTiets).ThenInclude(w => w.NhanVien).ThenInclude(w => w.User)
                    .Include(w => w.NhanVienGuiYeuCau).ThenInclude(w => w.User)
                    .Include(w => w.NhanVienKHTHDuyet).ThenInclude(w => w.User)
                    .Include(w => w.NhanVienNhanSuDuyet).ThenInclude(w => w.User)
                    .Include(w => w.GiamDoc).ThenInclude(w => w.User)
            );

            var ycNhanSuResultViewModel = ycNhanSuKhamSucKhoeEntity.ToModel<YeuCauNhanSuKhamSucKhoeViewModel>();

            var trangThaiAndSoLuong = _khamDoanService.GetTrangThaiAndSoLuong(ycNhanSuKhamSucKhoeEntity);

            ycNhanSuResultViewModel.TrangThai = trangThaiAndSoLuong.TrangThai;
            ycNhanSuResultViewModel.TongSoBs = trangThaiAndSoLuong.TongSoBs;
            ycNhanSuResultViewModel.TongSoDd = trangThaiAndSoLuong.TongSoDd;
            ycNhanSuResultViewModel.TongNvKhac = trangThaiAndSoLuong.TongNvKhac;
            ycNhanSuResultViewModel.IsDuyet = ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet;

            return Ok(ycNhanSuResultViewModel);
        }

        [HttpPost("UpdateYeuCauNhanSu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult> UpdateYeuCauNhanSuAsync([FromBody]YeuCauNhanSuKhamSucKhoeViewModel yeuCauNhanSuKhamSucKhoeViewModel)
        {
            var ycNhanSuKhamSucKhoeEntity = await _khamDoanService.GetByYeuCauNhanSuKhamSucKhoeIdAsync(yeuCauNhanSuKhamSucKhoeViewModel.Id,
                w => w.Include(q => q.YeuCauNhanSuKhamSucKhoeChiTiets));
            yeuCauNhanSuKhamSucKhoeViewModel.ToEntity(ycNhanSuKhamSucKhoeEntity);

            if (yeuCauNhanSuKhamSucKhoeViewModel.NhanSuBiLoaiBo.Any())
            {
                foreach (var nhanSuBiLoaiBoId in yeuCauNhanSuKhamSucKhoeViewModel.NhanSuBiLoaiBo)
                {
                    foreach (var ycNhanSuChiTietEntity in ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets)
                    {
                        if (ycNhanSuChiTietEntity.Id == nhanSuBiLoaiBoId)
                        {
                            ycNhanSuChiTietEntity.WillDelete = true;
                        }
                    }
                }
            }

            foreach (var ycNhanSuChiTietItem in yeuCauNhanSuKhamSucKhoeViewModel.YeuCauNhanSuKhamSucKhoeChiTiets.Where(q => q.IsCreate == true))
            {
                var yeuCauNhanSuKhamSucKhoeChiTietEntity = ycNhanSuChiTietItem.ToEntity<YeuCauNhanSuKhamSucKhoeChiTiet>();
                ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.Add(yeuCauNhanSuKhamSucKhoeChiTietEntity);
                if (ycNhanSuChiTietItem.NhanSuKhamSucKhoeTaiLieuDinhKem.Any())
                {
                    foreach (var item in ycNhanSuChiTietItem.NhanSuKhamSucKhoeTaiLieuDinhKem)
                    {
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                    }
                }
            }

            foreach (var ycNhanSuChiTietItem in yeuCauNhanSuKhamSucKhoeViewModel.YeuCauNhanSuKhamSucKhoeChiTiets.Where(q => q.IsUpdate == true))
            {
                var yeuCauNhanSuKhamSucKhoeChiTietEntity = ycNhanSuChiTietItem.ToEntity<YeuCauNhanSuKhamSucKhoeChiTiet>();

                if (ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                    .Any(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id))
                {
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).DonVi =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.DonVi;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).GhiChu =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.GhiChu;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).NhanVienId =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.NhanVienId;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).HoTen =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.HoTen;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).ViTriLamViec =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.ViTriLamViec;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).SoDienThoai =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.SoDienThoai;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).DoiTuongLamViec =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.DoiTuongLamViec;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).NguoiGioiThieuId =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.NguoiGioiThieuId;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).NhanSuKhamSucKhoeTaiLieuDinhKemId =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.NhanSuKhamSucKhoeTaiLieuDinhKemId;
                    if (ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id)
                            .NhanSuKhamSucKhoeTaiLieuDinhKem !=
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.NhanSuKhamSucKhoeTaiLieuDinhKem && yeuCauNhanSuKhamSucKhoeChiTietEntity.NhanSuKhamSucKhoeTaiLieuDinhKem!=null)
                    {
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(yeuCauNhanSuKhamSucKhoeChiTietEntity.NhanSuKhamSucKhoeTaiLieuDinhKem.DuongDan, yeuCauNhanSuKhamSucKhoeChiTietEntity.NhanSuKhamSucKhoeTaiLieuDinhKem.TenGuid);
                    }
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets
                            .First(w => w.Id == yeuCauNhanSuKhamSucKhoeChiTietEntity.Id).NhanSuKhamSucKhoeTaiLieuDinhKem =
                        yeuCauNhanSuKhamSucKhoeChiTietEntity.NhanSuKhamSucKhoeTaiLieuDinhKem;
                }
            }

            await _khamDoanService.UpdateYeuCauNhanSuAsync(ycNhanSuKhamSucKhoeEntity);
            return Ok();
        }
    }
}
