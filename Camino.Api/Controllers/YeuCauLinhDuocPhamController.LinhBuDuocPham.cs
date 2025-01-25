using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhBuDuocPham;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController
    {
        [HttpPost("GetYeuCauDuocPhamBenhVienDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauDuocPhamBenhVienDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetYeuCauDuocPhamBenhVienDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetYeuCauDuocPhamBenhVienTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauDuocPhamBenhVienTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetYeuCauDuocPhamBenhVienTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoDuocPhamDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoDuocPhamDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetBenhNhanTheoDuocPhamDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoDuocPhamTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoDuocPhamTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetBenhNhanTheoDuocPhamTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoCurrentUserLinhBu")]
        public async Task<ActionResult> GetKhoCurrentUserLinhBu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetKhoCurrentUserLinhBu(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetYeuCauLinhDuocPhamBuTao")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhDuocPhamViewModel>> GetYeuCauLinhDuocPhamBuTaoAsync(long id)
        {
            var yeuCauLinh = await _yeuCauLinhDuocPhamService.GetByIdAsync(id,
                x =>
                    x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                        .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                        .Include(a => a.KhoNhap)
                        .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoDuocPhams).ThenInclude(c => c.NhapKhoDuocPhamChiTiets)
                        .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                        .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));

            if (yeuCauLinh.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhBu)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var viewModel = yeuCauLinh.ToModel<DuyetYeuCauLinhDuocPhamViewModel>();
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


        [HttpPost("GuiPhieuLinhBu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult> GuiPhieuLinhBu(LinhBuDuocPhamViewModel linhBuDuocPhamVM)
        {
            if (linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.Count == 1 && linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
            {
                throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
            }
            var thongTinDuocPhamChiTietItems = new List<ThongTinDuocPhamChiTietItem>();
            var tuNgay = new DateTime(1970, 1, 1);
            var denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhTu))
            {
                DateTime.TryParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
            }
            if (!string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhDen))
            {
                DateTime.TryParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            }
            foreach (var item in linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox))
            {
                var thongTinDuocPhamChiTietItem = new ThongTinDuocPhamChiTietItem
                {
                    YeuCauDuocPhamBenhVienId = item.Id,
                    DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                    LaDuocPhamBHYT = item.LaDuocPhamBHYT,
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
                thongTinDuocPhamChiTietItems.Add(thongTinDuocPhamChiTietItem);
            }

            linhBuDuocPhamVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhBu;
            linhBuDuocPhamVM.NgayYeuCau = DateTime.Now;

            var linhBuDuocPham = linhBuDuocPhamVM.ToEntity<YeuCauLinhDuocPham>();
            linhBuDuocPham.ThoiDiemLinhTongHopTuNgay = !string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhTu) ?
                                                        DateTime.ParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : (DateTime?)null;
            linhBuDuocPham.ThoiDiemLinhTongHopDenNgay = !string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhDen) ?
                                                    DateTime.ParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : DateTime.Now;
            linhBuDuocPham.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();

            //var yeuCauDuocPhamBenhVienDateTimes = new List<DateTime>();
            //var yeuCauDuocPhamBenhVienIds = new List<long>();
            //if (linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.Any())
            //{
            //    var itemIds = _yeuCauLinhDuocPhamService.GetIdsYeuCauKhamBenh((long)linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.First().KhoLinhTuId, (long)linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.First().KhoLinhVeId);
            //    if (itemIds.Any())
            //    {
            //        yeuCauDuocPhamBenhVienIds.AddRange(itemIds);
            //    }
            //}
            //if (yeuCauDuocPhamBenhVienIds.Any())
            //{
            //    foreach (var item in yeuCauDuocPhamBenhVienIds)
            //    {
            //        var itemDateTiem = _yeuCauLinhDuocPhamService.GetDateTime(item);
            //        yeuCauDuocPhamBenhVienDateTimes.Add(itemDateTiem);
            //    }
            //}
            //if (yeuCauDuocPhamBenhVienDateTimes.Any())
            //{
            //    var sortDateTimeTungay = yeuCauDuocPhamBenhVienDateTimes.OrderBy(s => s).ToList();
            //    if (string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhTu))
            //    {
            //        linhBuDuocPham.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
            //    }
            //}
            if(string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhTu) || string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhDen))
            {
                var yeuCauDuocPhamBenhVienVos = thongTinDuocPhamChiTietItems;
                var itemIds = _yeuCauLinhDuocPhamService.GetChiTietYeuCauDuocPhamBenhVienCanBu(yeuCauDuocPhamBenhVienVos.First().KhoLinhTuId.GetValueOrDefault(), yeuCauDuocPhamBenhVienVos.First().KhoLinhVeId.GetValueOrDefault(), yeuCauDuocPhamBenhVienVos.Select(o => o.DuocPhamBenhVienId.GetValueOrDefault()).ToList());

                linhBuDuocPham.ThoiDiemLinhTongHopTuNgay = !string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhTu) ?
                                                        DateTime.ParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : itemIds.Select(o => o.NgayChiDinh).OrderBy(o => o).First();
                linhBuDuocPham.ThoiDiemLinhTongHopDenNgay = !string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhDen) ?
                                                        DateTime.ParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : itemIds.Select(o => o.NgayChiDinh).OrderBy(o => o).Last();
            }

            await _yeuCauLinhDuocPhamService.XuLyThemYeuCauLinhDuocPhamBuAsync(linhBuDuocPham, thongTinDuocPhamChiTietItems);
            await _yeuCauLinhDuocPhamService.AddAsync(linhBuDuocPham);
            return Ok(linhBuDuocPham.Id);
        }

        [HttpPost("GuiLaiPhieuLinhBu")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.TaoYeuCauLinhBuDuocPham)]
        public async Task<ActionResult<LinhBuDuocPhamViewModel>> GuiLaiPhieuLinhBu(LinhBuDuocPhamViewModel linhBuDuocPhamVM)
        {
            if (linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.Count == 1 && linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
            {
                throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
            }
            var yeuCauLinhBu = await _yeuCauLinhDuocPhamService.GetByIdAsync(linhBuDuocPhamVM.Id,
               x =>
                   x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                       .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                       .Include(a => a.KhoNhap)
                       .Include(a => a.YeuCauDuocPhamBenhViens)
                       .Include(a => a.YeuCauLinhDuocPhamChiTiets).ThenInclude(a => a.YeuCauDuocPhamBenhVien).ThenInclude(a => a.NoiTruPhieuDieuTri)
                       .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoDuocPhams).ThenInclude(c => c.NhapKhoDuocPhamChiTiets)
                       .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                       .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));
            var thongTinDuocPhamChiTietItems = new List<ThongTinDuocPhamChiTietItem>();
            //var tuNgay = new DateTime(1970, 1, 1);
            //var denNgay = DateTime.Now;
            //if (!string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhTu))
            //{
            //    DateTime.TryParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
            //}
            //if (!string.IsNullOrEmpty(linhBuDuocPhamVM.ThoiDiemChiDinhDen))
            //{
            //    DateTime.TryParseExact(linhBuDuocPhamVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //}
            foreach (var item in linhBuDuocPhamVM.YeuCauDuocPhamBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox))
            {
                var thongTinDuocPhamChiTietItem = new ThongTinDuocPhamChiTietItem
                {
                    YeuCauDuocPhamBenhVienId = item.Id,
                    DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                    LaDuocPhamBHYT = item.LaDuocPhamBHYT,
                    KhoLinhTuId = item.KhoLinhTuId,
                    KhoLinhVeId = item.KhoLinhVeId,
                    SoLuongCanBu = item.SoLuongCanBu,
                    SoLuongTon = item.SoLuongTon,
                    SoLuongYeuCau = item.SoLuongYeuCau,
                    SoLuongDaBu = item.SoLuongDaBu,
                    SLYeuCauLinhThucTe = item.SLYeuCauLinhThucTe,
                    //ThoiDiemChiDinhTu = tuNgay,
                    //ThoiDiemChiDinhDen = denNgay,
                };
                thongTinDuocPhamChiTietItems.Add(thongTinDuocPhamChiTietItem);
            }
            yeuCauLinhBu.GhiChu = linhBuDuocPhamVM.GhiChu;
            yeuCauLinhBu.DaGui = linhBuDuocPhamVM.DaGui;
            if (linhBuDuocPhamVM.DaGui == true)
            {
                yeuCauLinhBu.NgayYeuCau = DateTime.Now;
            }
            await _yeuCauLinhDuocPhamService.XuLyCapNhatYeuCauLinhDuocPhamBuAsync(yeuCauLinhBu, thongTinDuocPhamChiTietItems);

            await _yeuCauLinhDuocPhamService.UpdateAsync(yeuCauLinhBu);
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

        [HttpPost("InPhieuLinhBuDuocPham")]
        public string InPhieuLinhBuDuocPham(PhieuLinhThuongDuocPham phieuLinhThuongDuoc)
        {
            var result = _yeuCauLinhDuocPhamService.InPhieuLinhThuongDuocPham(phieuLinhThuongDuoc);
            return result;
        }
        #region in lĩnh bù xem trước
        [HttpPost("InPhieuLinhBuDuocPhamXemTruoc")]
        public string InPhieuLinhBuDuocPhamXemTruoc(XemPhieuLinhBuDuocPham phieuLinhThuongDuoc)
        {
            //var yeuCauDuocPhamBenhVienDateTimes = new List<DateTime>();
            //var yeuCauDuocPhamBenhVienIds = new List<long>();
            //if (phieuLinhThuongDuoc.YeuCauDuocPhamBenhViens != null)
            //{
            //    foreach (var item in phieuLinhThuongDuoc.YeuCauDuocPhamBenhViens.ToList())
            //    {
            //        var itemIds = _yeuCauLinhDuocPhamService.GetIdsYeuCauDuocPhamBenhVien(item.KhoLinhTuId,item.KhoLinhVeId, item.DuocPhamBenhVienId);
            //        if (itemIds.Any())
            //        {
            //            yeuCauDuocPhamBenhVienIds.AddRange(itemIds);
            //        }
            //    }
               
            //}
            //if (yeuCauDuocPhamBenhVienIds.Any())
            //{
            //    foreach (var item in yeuCauDuocPhamBenhVienIds)
            //    {
            //        var itemDateTiem = _yeuCauLinhDuocPhamService.GetDateTime(item);
            //        yeuCauDuocPhamBenhVienDateTimes.Add(itemDateTiem);
            //    }
            //}
            //if (yeuCauDuocPhamBenhVienDateTimes.Any())
            //{
            //    var sortDateTimeTungay = yeuCauDuocPhamBenhVienDateTimes.OrderBy(s => s).ToList();
            //    //r sortDateTimeDenNgay = yeuCauDuocPhamBenhVienDateTimes.OrderByDescending(s => s).ToList();
            //    phieuLinhThuongDuoc.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
            //    phieuLinhThuongDuoc.ThoiDiemLinhTongHopDenNgay = DateTime.Now;
            //}
            var yeuCauDuocPhamBenhVienVos = phieuLinhThuongDuoc.YeuCauDuocPhamBenhViens.ToList();
            var itemIds = _yeuCauLinhDuocPhamService.GetChiTietYeuCauDuocPhamBenhVienCanBu(yeuCauDuocPhamBenhVienVos.First().KhoLinhTuId, yeuCauDuocPhamBenhVienVos.First().KhoLinhVeId, yeuCauDuocPhamBenhVienVos.Select(o=>o.DuocPhamBenhVienId).ToList());
            phieuLinhThuongDuoc.ThoiDiemLinhTongHopTuNgay = itemIds.Select(o => o.NgayChiDinh).OrderBy(o => o).First();
            phieuLinhThuongDuoc.ThoiDiemLinhTongHopDenNgay = itemIds.Select(o => o.NgayChiDinh).OrderBy(o => o).Last();
            phieuLinhThuongDuoc.YeuCauDuocPhamBenhVienIds = itemIds.Select(o=>o.Id).ToList();
            var result = _yeuCauLinhDuocPhamService.InPhieuLinhBuDuocPhamXemTruoc(phieuLinhThuongDuoc);
            return result;
        }
        #endregion
    }
}
