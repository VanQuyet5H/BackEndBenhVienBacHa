using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhBuVatTu;
using Camino.Api.Models.LinhVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController
    {
        [HttpPost("GetYeuCauVatTuBenhVienDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauVatTuBenhVienDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetYeuCauVatTuBenhVienDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetYeuCauVatTuBenhVienTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauVatTuBenhVienTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetYeuCauVatTuBenhVienTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoVatTuDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetBenhNhanTheoVatTuDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoVatTuTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetBenhNhanTheoVatTuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoCurrentUserLinhBuVatTu")]
        public async Task<ActionResult> GetKhoCurrentUserLinhBuVatTu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhVatTuService.GetKhoCurrentUserLinhBu(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetYeuCauLinhVatTuBuTao")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhVatTuViewModel>> GetYeuCauLinhVatTuBuTaoAsync(long id)
        {
            var yeuCauLinh = await _yeuCauLinhVatTuService.GetByIdAsync(id,
                x =>
                    x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                        .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                        .Include(a => a.KhoNhap)
                        .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoVatTus).ThenInclude(c => c.NhapKhoVatTuChiTiets)
                        .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                        .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));

            if (yeuCauLinh.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhBu)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var viewModel = yeuCauLinh.ToModel<DuyetYeuCauLinhVatTuViewModel>();
            // thông tin nhân viên duyệt gán mặc định
            if (yeuCauLinh.NhanVienDuyetId == null)
            {
                var nhanVien = await _userService.GetCurrentUser();
                viewModel.NhanVienDuyetId = nhanVien.Id;
                viewModel.TenNhanVienDuyet = nhanVien.HoTen;
            }
            // thông tin nhân viên xuất nếu chưa có
            if (viewModel.NguoiXuatKhoId == null)
            {
                viewModel.NguoiXuatKhoId = _userAgentHelper.GetCurrentUserId();
            }
            if (yeuCauLinh.NhanVienYeuCauId == _userAgentHelper.GetCurrentUserId())
            {
                viewModel.LaNguoiTaoPhieu = true;
            }
            else
            {
                viewModel.LaNguoiTaoPhieu = false;
            }
            return Ok(viewModel);
        }


        [HttpPost("GuiPhieuLinhBuVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult> GuiPhieuLinhBu(LinhBuVatTuViewModel linhBuVatTuVM)
        {
            if (linhBuVatTuVM.YeuCauVatTuBenhViens.Count == 1 && linhBuVatTuVM.YeuCauVatTuBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
            {
                throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
            }
            var thongTinVatTuChiTietItems = new List<ThongTinVatTuTietItem>();
            var tuNgay = new DateTime(1970, 1, 1);
            var denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(linhBuVatTuVM.ThoiDiemChiDinhTu))
            {
                DateTime.TryParseExact(linhBuVatTuVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
            }
            if (!string.IsNullOrEmpty(linhBuVatTuVM.ThoiDiemChiDinhDen))
            {
                DateTime.TryParseExact(linhBuVatTuVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            }
            foreach (var item in linhBuVatTuVM.YeuCauVatTuBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox))
            {
                var thongTinVatTuChiTietItem = new ThongTinVatTuTietItem
                {
                    YeuCauVatTuBenhVienId = item.Id,
                    VatTuBenhVienId = item.VatTuBenhVienId,
                    LaVatTuBHYT = item.LaVatTuBHYT,
                    KhoLinhTuId = item.KhoLinhTuId,
                    KhoLinhVeId = item.KhoLinhVeId,
                    SoLuongCanBu = item.SoLuongCanBu,
                    SoLuongTon = item.SoLuongTon,
                    SoLuongYeuCau = item.SoLuongYeuCau,
                    SoLuongDaBu = item.SoLuongDaBu,
                    SLYeuCauLinhThucTe = item.SLYeuCauLinhThucTe,
                    ThoiDiemChiDinhTu = tuNgay,
                    ThoiDiemChiDinhDen = denNgay,
                };
                thongTinVatTuChiTietItems.Add(thongTinVatTuChiTietItem);
            }

            linhBuVatTuVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhBu;
            linhBuVatTuVM.NgayYeuCau = DateTime.Now;
            var linhBuVatTu = linhBuVatTuVM.ToEntity<YeuCauLinhVatTu>();
            linhBuVatTu.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
            linhBuVatTu.ThoiDiemLinhTongHopTuNgay = !string.IsNullOrEmpty(linhBuVatTuVM.ThoiDiemChiDinhTu) ?
                                                     DateTime.ParseExact(linhBuVatTuVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : (DateTime?)null;
            linhBuVatTu.ThoiDiemLinhTongHopDenNgay = !string.IsNullOrEmpty(linhBuVatTuVM.ThoiDiemChiDinhDen) ?
                                                    DateTime.ParseExact(linhBuVatTuVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : DateTime.Now;
            var yeuCauVatTuBenhVienDateTimes = new List<DateTime>();
            var yeuCauDuocPhamBenhVienIds = new List<long>();
            if (linhBuVatTuVM.YeuCauVatTuBenhViens.Any())
            {
                var itemIds = _yeuCauLinhVatTuService.GetIdsYeuCauVatTu((long)linhBuVatTuVM.YeuCauVatTuBenhViens.First().KhoLinhTuId, (long)linhBuVatTuVM.YeuCauVatTuBenhViens.First().KhoLinhVeId);
                if (itemIds.Any())
                {
                    yeuCauDuocPhamBenhVienIds.AddRange(itemIds);
                }
            }
            if (yeuCauDuocPhamBenhVienIds.Any())
            {
                foreach (var item in yeuCauDuocPhamBenhVienIds)
                {
                    var itemDateTiem = _yeuCauLinhVatTuService.GetDateTime(item);
                    yeuCauVatTuBenhVienDateTimes.Add(itemDateTiem);
                }
            }
            if (yeuCauVatTuBenhVienDateTimes.Any())
            {
                var sortDateTimeTungay = yeuCauVatTuBenhVienDateTimes.OrderBy(s => s).ToList();
                if (string.IsNullOrEmpty(linhBuVatTuVM.ThoiDiemChiDinhTu))
                {
                    linhBuVatTu.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
                }
            }

            await _yeuCauLinhVatTuService.XuLyThemYeuCauLinhVatTuBuAsync(linhBuVatTu, thongTinVatTuChiTietItems);
            await _yeuCauLinhVatTuService.AddAsync(linhBuVatTu);
            return Ok(linhBuVatTu.Id);
        }

        [HttpPost("GuiLaiPhieuLinhBu")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.TaoYeuCauLinhBuVatTu)]
        public async Task<ActionResult<LinhBuVatTuViewModel>> GuiLaiPhieuLinhBu(LinhBuVatTuViewModel linhBuVatTuVM)
        {

            if (linhBuVatTuVM.YeuCauVatTuBenhViens.Count == 1 && linhBuVatTuVM.YeuCauVatTuBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
            {
                throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
            }
            var yeuCauLinhBu = await _yeuCauLinhVatTuService.GetByIdAsync(linhBuVatTuVM.Id,
               x =>
                   x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                       .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                       .Include(a => a.KhoNhap)
                       .Include(a => a.YeuCauVatTuBenhViens)
                       .Include(a => a.YeuCauLinhVatTuChiTiets)
                       .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoVatTus).ThenInclude(c => c.NhapKhoVatTuChiTiets)
                       .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                       .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));
            var thongTinVatTuChiTietItems = new List<ThongTinVatTuTietItem>();
            foreach (var item in linhBuVatTuVM.YeuCauVatTuBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox))
            {
                var thongTinVatTuChiTietItem = new ThongTinVatTuTietItem
                {
                    YeuCauVatTuBenhVienId = item.Id,
                    VatTuBenhVienId = item.VatTuBenhVienId,
                    LaVatTuBHYT = item.LaVatTuBHYT,
                    KhoLinhTuId = item.KhoLinhTuId,
                    KhoLinhVeId = item.KhoLinhVeId,
                    SoLuongCanBu = item.SoLuongCanBu,
                    SoLuongTon = item.SoLuongTon,
                    SoLuongYeuCau = item.SoLuongYeuCau,
                    SoLuongDaBu = item.SoLuongDaBu,
                    SLYeuCauLinhThucTe = item.SLYeuCauLinhThucTe,
                };
                thongTinVatTuChiTietItems.Add(thongTinVatTuChiTietItem);
            }
            yeuCauLinhBu.GhiChu = linhBuVatTuVM.GhiChu;
            yeuCauLinhBu.DaGui = linhBuVatTuVM.DaGui;
            if (linhBuVatTuVM.DaGui == true)
            {
                yeuCauLinhBu.NgayYeuCau = DateTime.Now;
            }
            await _yeuCauLinhVatTuService.XuLyCapNhatYeuCauLinhVatTuBuAsync(yeuCauLinhBu, thongTinVatTuChiTietItems);
            await _yeuCauLinhVatTuService.UpdateAsync(yeuCauLinhBu);
            EnumTrangThaiPhieuLinh enumTrangThaiPhieuLinh;
            if (yeuCauLinhBu.DaGui != true)
            {
                enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
            }
            else
            {
                if (yeuCauLinhBu.DuocDuyet == true)
                {
                    enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                }
                else if (yeuCauLinhBu.DuocDuyet == false)
                {
                    enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                }
                else
                {
                    enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                }
            }
            var ten = enumTrangThaiPhieuLinh.GetDescription();
            var result = new
            {
                yeuCauLinhBu.Id,
                yeuCauLinhBu.LastModified,
                enumTrangThaiPhieuLinh,
                ten
            };
            return Ok(result);
        }
    }
}
