using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhBuDuocPham;
using Camino.Api.Models.LinhBuKSNK;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Api.Models.LinhThuongKSNK;
using Camino.Api.Models.LinhVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhKSNKController
    {
        [HttpPost("GetYeuCauKSNKBenhVienDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauKSNKBenhVienDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetYeuCauKSNKBenhVienDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetYeuCauKSNKBenhVienTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauKSNKBenhVienTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetYeuCauKSNKBenhVienTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoKSNKDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoKSNKDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetBenhNhanTheoKSNKDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoKSNKTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoKSNKTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetBenhNhanTheoKSNKTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoCurrentUserLinhBuKSNK")]
        public async Task<ActionResult> GetKhoCurrentUserLinhBuKSNK([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhKSNKService.GetKhoCurrentUserLinhBu(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetYeuCauLinhKSNKBuTao")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhKSNKViewModel>> GetYeuCauLinhKSNKBuTaoAsync(GridVoYeuCauLinh vo)
        {
            if(vo.LoaiDuocPhamHayVatTu == true)
            {
                var yeuCauLinh = await _yeuCauLinhDuocPhamService.GetByIdAsync(vo.YeuCauLinhId,
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
                var duyetYeuCauLinhDuocPhamChiTiets = viewModel.DuyetYeuCauLinhDuocPhamChiTiets
                     .Select(d => new DuyetYeuCauLinhKSNKChiTietViewModel() {
                         Id = d.Id,
                         VatTuBenhVienId = d.DuocPhamBenhVienId,
                         LaVatTuBHYT = d.LaDuocPhamBHYT,
                         TenVatTu = d.TenDuocPham,
                         SoLuong = d.SoLuong,
                         SLTon = d.SLTon,
                         SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                         Nhom  = d.Nhom,
                         DVT = d.DVT,
                         HangSanXuat = d.HangSanXuat,
                         NuocSanXuat = d.NuocSanXuat,
                         isTuChoi = d.isTuChoi,
                         Index = d.Index
                     }).ToList();
                var newModel = new DuyetYeuCauLinhKSNKViewModel()
                {
                    Id = viewModel.Id,
                    KhoXuatId = viewModel.KhoXuatId,
                    TenKhoXuat = viewModel.TenKhoXuat,
                    KhoNhapId = viewModel.KhoNhapId,
                    TenKhoNhap = viewModel.TenKhoNhap,
                    NhanVienDuyetId = viewModel.NhanVienDuyetId,
                    TenNhanVienDuyet = viewModel.TenNhanVienDuyet,
                    NhanVienYeuCauId = viewModel.NhanVienYeuCauId,

                    TenNhanVienYeuCau = viewModel.TenNhanVienYeuCau,
                    NgayYeuCau = viewModel.NgayYeuCau,
                    NgayDuyet = viewModel.NgayDuyet,
                    GhiChu = viewModel.GhiChu,
                    NguoiXuatKhoId = viewModel.NguoiXuatKhoId,
                    TenNguoiXuatKho = viewModel.TenNguoiXuatKho,
                    NguoiNhapKhoId = viewModel.NguoiNhapKhoId,


                    TenNguoiNhapKho = viewModel.TenNguoiNhapKho,
                    LoaiPhieuLinh = viewModel.LoaiPhieuLinh,
                    DuocDuyet = viewModel.DuocDuyet,
                    LyDoKhongDuyet = viewModel.LyDoKhongDuyet,
                    LastModified = viewModel.LastModified,
                    LaNguoiTaoPhieu = viewModel.LaNguoiTaoPhieu,

                    DuyetYeuCauLinhVatTuChiTiets = duyetYeuCauLinhDuocPhamChiTiets
                };
                return Ok(newModel);
            }
            else
            {
                var yeuCauLinh = await _yeuCauLinhKSNKService.GetByIdAsync(vo.YeuCauLinhId,
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
           
        }


        [HttpPost("GuiPhieuLinhBuKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult> GuiPhieuLinhBu(LinhBuKSNKViewModel linhBuKSNKVM)
        {
            var yeuCauLinhDuocPhamVatTuIds = new List<GridVoYeuCauLinhDuocTao>();
            if (linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d=>d.LoaiDuocPhamHayVatTu == true && d.CheckBox == true).Count() != 0)
            {
                var yeuCauLinhDuocPhamChiTiets = linhBuKSNKVM.YeuCauLinhVatTuChiTiets.Where(d=>d.LoaiDuocPhamHayVatTu == true)
                    .Select(d => new LinhBuDuocPhamChiTietViewModel()
                    {
                        Id = d.Id,
                        Nhom = d.Nhom,
                        YeuCauLinhDuocPhamId = d.YeuCauLinhVatTuId,
                        DuocPhamBenhVienId = d.VatTuBenhVienId,
                        LaDuocPhamBHYT = d.LaVatTuBHYT,
                        SoLuong = d.SoLuong,
                        Ten = d.Ten,
                        DuocDuyet = d.DuocDuyet,
                        SLYeuCau = d.SoLuong , // to do
                        SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                        DVT = d.DVT,
                        NhaSX = d.NhaSX,
                        NuocSX = d.NuocSX,
                        SLCanBu = d.SoLuong , //to do
                        //SLTon = d.so
                        //SLYeuCauLinhThucTe = d.so
                    }).ToList();

                var yeuCauDuocPhamBenhViens = linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == true && d.CheckBox == true)
                     .Select(d => new YeuCauDuocPhamBenhVienViewModel()
                     {
                         Id = d.Id,
                         DuocPhamBenhVienId = d.VatTuBenhVienId,
                         LaDuocPhamBHYT = d.LaVatTuBHYT,
                         YeuCauDuocPhamBenhVienIds = d.YeuCauVatTuBenhVienIds,
                         KhoLinhTuId = d.KhoLinhTuId,
                         KhoLinhVeId = d.KhoLinhVeId,
                         SoLuongCanBu = d.SoLuongCanBu,
                         SoLuongTon = d.SoLuongTon,
                         SoLuongYeuCau = d.SoLuongYeuCau, // to do
                         SoLuongDaBu = d.SoLuongDaBu,
                         SLYeuCauLinhThucTe = d.SLYeuCauLinhThucTe,
                         SLYeuCauLinhThucTeMax = d.SLYeuCauLinhThucTeMax,
                         CheckBox = d.CheckBox,
                     }).ToList();

                var linhBuDuocPhamViewModel = new LinhBuDuocPhamViewModel()
                {
                    KhoXuatId = linhBuKSNKVM.KhoXuatId,
                    KhoNhapId = linhBuKSNKVM.KhoNhapId,
                    LoaiPhieuLinh = linhBuKSNKVM.LoaiPhieuLinh,
                    NhanVienYeuCauId = linhBuKSNKVM.NhanVienYeuCauId,
                    NgayYeuCau = linhBuKSNKVM.NgayYeuCau,
                    GhiChu = linhBuKSNKVM.GhiChu,
                    TenKhoXuat = linhBuKSNKVM.TenKhoXuat,
                    TenKhoNhap = linhBuKSNKVM.TenKhoNhap,
                    DuocDuyet = linhBuKSNKVM.DuocDuyet,
                    HoTenNguoiYeuCau = linhBuKSNKVM.HoTenNguoiYeuCau,
                    DaGui = linhBuKSNKVM.DaGui,
                    ThoiDiemChiDinhTu = linhBuKSNKVM.ThoiDiemChiDinhTu,
                    ThoiDiemChiDinhDen = linhBuKSNKVM.ThoiDiemChiDinhDen,
                    YeuCauLinhDuocPhamChiTiets = yeuCauLinhDuocPhamChiTiets,
                    YeuCauDuocPhamBenhViens = yeuCauDuocPhamBenhViens,
                    Id = linhBuKSNKVM.Id
                };


                if (linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.Count == 1 && linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
                {
                    throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
                }
                var thongTinDuocPhamChiTietItems = new List<ThongTinDuocPhamChiTietItem>();
                var tuNgay = new DateTime(1970, 1, 1);
                var denNgay = DateTime.Now;
                if (!string.IsNullOrEmpty(linhBuDuocPhamViewModel.ThoiDiemChiDinhTu))
                {
                    DateTime.TryParseExact(linhBuDuocPhamViewModel.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
                }
                if (!string.IsNullOrEmpty(linhBuDuocPhamViewModel.ThoiDiemChiDinhDen))
                {
                    DateTime.TryParseExact(linhBuDuocPhamViewModel.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                }
                foreach (var item in linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox))
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

                linhBuDuocPhamViewModel.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhBu;
                linhBuDuocPhamViewModel.NgayYeuCau = DateTime.Now;

                var linhBuDuocPham = linhBuDuocPhamViewModel.ToEntity<YeuCauLinhDuocPham>();
                linhBuDuocPham.ThoiDiemLinhTongHopTuNgay = !string.IsNullOrEmpty(linhBuDuocPhamViewModel.ThoiDiemChiDinhTu) ?
                                                            DateTime.ParseExact(linhBuDuocPhamViewModel.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : (DateTime?)null;
                linhBuDuocPham.ThoiDiemLinhTongHopDenNgay = !string.IsNullOrEmpty(linhBuDuocPhamViewModel.ThoiDiemChiDinhDen) ?
                                                        DateTime.ParseExact(linhBuDuocPhamViewModel.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : DateTime.Now;
                linhBuDuocPham.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();

                var yeuCauDuocPhamBenhVienDateTimes = new List<DateTime>();
                var yeuCauDuocPhamBenhVienIds = new List<long>();
                if (linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.Any())
                {
                    var itemIds = _yeuCauLinhDuocPhamService.GetIdsYeuCauKhamBenh((long)linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.First().KhoLinhTuId, (long)linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.First().KhoLinhVeId);
                    if (itemIds.Any())
                    {
                        yeuCauDuocPhamBenhVienIds.AddRange(itemIds);
                    }
                }
                if (yeuCauDuocPhamBenhVienIds.Any())
                {
                    foreach (var item in yeuCauDuocPhamBenhVienIds)
                    {
                        var itemDateTiem = _yeuCauLinhDuocPhamService.GetDateTime(item);
                        yeuCauDuocPhamBenhVienDateTimes.Add(itemDateTiem);
                    }
                }
                if (yeuCauDuocPhamBenhVienDateTimes.Any())
                {
                    var sortDateTimeTungay = yeuCauDuocPhamBenhVienDateTimes.OrderBy(s => s).ToList();
                    if (string.IsNullOrEmpty(linhBuDuocPhamViewModel.ThoiDiemChiDinhTu))
                    {
                        linhBuDuocPham.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
                    }
                }




                await _yeuCauLinhDuocPhamService.XuLyThemYeuCauLinhDuocPhamBuAsync(linhBuDuocPham, thongTinDuocPhamChiTietItems);
                await _yeuCauLinhDuocPhamService.AddAsync(linhBuDuocPham);
                var newPhieu = new GridVoYeuCauLinhDuocTao
                {
                    LoaiDuocPhamHayVatTu = true,
                    YeuCauLinhVatTuId = linhBuDuocPham.Id
                };
                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                
            }






            if (linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == false && d.CheckBox == true).Count() != 0)
            {
                if (linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == false && d.CheckBox == true).Count() == 1 && linhBuKSNKVM.YeuCauVatTuBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
                {
                    throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
                }
                var thongTinKSNKChiTietItems = new List<ThongTinKSNKTietItem>();
                var tuNgay = new DateTime(1970, 1, 1);
                var denNgay = DateTime.Now;
                if (!string.IsNullOrEmpty(linhBuKSNKVM.ThoiDiemChiDinhTu))
                {
                    DateTime.TryParseExact(linhBuKSNKVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
                }
                if (!string.IsNullOrEmpty(linhBuKSNKVM.ThoiDiemChiDinhDen))
                {
                    DateTime.TryParseExact(linhBuKSNKVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                }
                foreach (var item in linhBuKSNKVM.YeuCauVatTuBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox && z.LoaiDuocPhamHayVatTu == false))
                {
                    var thongTinKSNKChiTietItem = new ThongTinKSNKTietItem
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
                    thongTinKSNKChiTietItems.Add(thongTinKSNKChiTietItem);
                }

                linhBuKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhBu;
                linhBuKSNKVM.NgayYeuCau = DateTime.Now;
                var linhBuKSNK = linhBuKSNKVM.ToEntity<YeuCauLinhVatTu>();

                linhBuKSNK.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                linhBuKSNK.ThoiDiemLinhTongHopTuNgay = !string.IsNullOrEmpty(linhBuKSNKVM.ThoiDiemChiDinhTu) ?
                                                         DateTime.ParseExact(linhBuKSNKVM.ThoiDiemChiDinhTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : (DateTime?)null;
                linhBuKSNK.ThoiDiemLinhTongHopDenNgay = !string.IsNullOrEmpty(linhBuKSNKVM.ThoiDiemChiDinhDen) ?
                                                        DateTime.ParseExact(linhBuKSNKVM.ThoiDiemChiDinhDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : DateTime.Now;
                var yeuCauKSNKBenhVienDateTimes = new List<DateTime>();
                var yeuCauDuocPhamBenhVienIds = new List<long>();
                if (linhBuKSNKVM.YeuCauVatTuBenhViens.Any())
                {
                    var itemIds = _yeuCauLinhKSNKService.GetIdsYeuCauKSNK((long)linhBuKSNKVM.YeuCauVatTuBenhViens.First().KhoLinhTuId, (long)linhBuKSNKVM.YeuCauVatTuBenhViens.First().KhoLinhVeId);
                    if (itemIds.Any())
                    {
                        yeuCauDuocPhamBenhVienIds.AddRange(itemIds);
                    }
                }
                if (yeuCauDuocPhamBenhVienIds.Any())
                {
                    foreach (var item in yeuCauDuocPhamBenhVienIds)
                    {
                        var itemDateTiem = _yeuCauLinhKSNKService.GetDateTime(item);
                        yeuCauKSNKBenhVienDateTimes.Add(itemDateTiem);
                    }
                }
                if (yeuCauKSNKBenhVienDateTimes.Any())
                {
                    var sortDateTimeTungay = yeuCauKSNKBenhVienDateTimes.OrderBy(s => s).ToList();
                    if (string.IsNullOrEmpty(linhBuKSNKVM.ThoiDiemChiDinhTu))
                    {
                        linhBuKSNK.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
                    }
                }

                await _yeuCauLinhKSNKService.XuLyThemYeuCauLinhKSNKBuAsync(linhBuKSNK, thongTinKSNKChiTietItems);
                await _yeuCauLinhKSNKService.AddAsync(linhBuKSNK);
                var newPhieu = new GridVoYeuCauLinhDuocTao
                {
                    LoaiDuocPhamHayVatTu = false,
                    YeuCauLinhVatTuId = linhBuKSNK.Id
                };
                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
            }

            return Ok(yeuCauLinhDuocPhamVatTuIds.ToList());
        }

        [HttpPost("GuiLaiPhieuLinhBu")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.TaoYeuCauLinhBuKSNK)]
        public async Task<ActionResult<LinhBuKSNKViewModel>> GuiLaiPhieuLinhBu(LinhBuKSNKViewModel linhBuKSNKVM)
        {
            var yeuCauLinhDuocPhamVatTuIds = new List<GridVoYeuCauLinhDuocTao>();
            if (linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == true && d.CheckBox == true).Count() != 0)
            {

                var yeuCauLinhDuocPhamChiTiets = linhBuKSNKVM.YeuCauLinhVatTuChiTiets.Where(d => d.LoaiDuocPhamHayVatTu == true)
                    .Select(d => new LinhBuDuocPhamChiTietViewModel()
                    {
                        Id = d.Id,
                        Nhom = d.Nhom,
                        YeuCauLinhDuocPhamId = d.YeuCauLinhVatTuId,
                        DuocPhamBenhVienId = d.VatTuBenhVienId,
                        LaDuocPhamBHYT = d.LaVatTuBHYT,
                        SoLuong = d.SoLuong,
                        Ten = d.Ten,
                        DuocDuyet = d.DuocDuyet,
                        SLYeuCau = d.SoLuong, // to do
                        SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                        DVT = d.DVT,
                        NhaSX = d.NhaSX,
                        NuocSX = d.NuocSX,
                        SLCanBu = d.SoLuong, //to do
                        //SLTon = d.so
                        //SLYeuCauLinhThucTe = d.so
                    }).ToList();

                var yeuCauDuocPhamBenhViens = linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == true && d.CheckBox == true)
                     .Select(d => new YeuCauDuocPhamBenhVienViewModel()
                     {
                         Id = d.Id,
                         DuocPhamBenhVienId = d.VatTuBenhVienId,
                         LaDuocPhamBHYT = d.LaVatTuBHYT,
                         YeuCauDuocPhamBenhVienIds = d.YeuCauVatTuBenhVienIds,
                         KhoLinhTuId = d.KhoLinhTuId,
                         KhoLinhVeId = d.KhoLinhVeId,
                         SoLuongCanBu = d.SoLuongCanBu,
                         SoLuongTon = d.SoLuongTon,
                         SoLuongYeuCau = d.SoLuongYeuCau, // to do
                         SoLuongDaBu = d.SoLuongDaBu,
                         SLYeuCauLinhThucTe = d.SLYeuCauLinhThucTe,
                         SLYeuCauLinhThucTeMax = d.SLYeuCauLinhThucTeMax,
                         CheckBox = d.CheckBox,
                     }).ToList();

                var linhBuDuocPhamViewModel = new LinhBuDuocPhamViewModel()
                {
                    KhoXuatId = linhBuKSNKVM.KhoXuatId,
                    KhoNhapId = linhBuKSNKVM.KhoNhapId,
                    LoaiPhieuLinh = linhBuKSNKVM.LoaiPhieuLinh,
                    NhanVienYeuCauId = linhBuKSNKVM.NhanVienYeuCauId,
                    NgayYeuCau = linhBuKSNKVM.NgayYeuCau,
                    GhiChu = linhBuKSNKVM.GhiChu,
                    TenKhoXuat = linhBuKSNKVM.TenKhoXuat,
                    TenKhoNhap = linhBuKSNKVM.TenKhoNhap,
                    DuocDuyet = linhBuKSNKVM.DuocDuyet,
                    HoTenNguoiYeuCau = linhBuKSNKVM.HoTenNguoiYeuCau,
                    DaGui = linhBuKSNKVM.DaGui,
                    ThoiDiemChiDinhTu = linhBuKSNKVM.ThoiDiemChiDinhTu,
                    ThoiDiemChiDinhDen = linhBuKSNKVM.ThoiDiemChiDinhDen,
                    YeuCauLinhDuocPhamChiTiets = yeuCauLinhDuocPhamChiTiets,
                    YeuCauDuocPhamBenhViens = yeuCauDuocPhamBenhViens,
                    Id = linhBuKSNKVM.Id
                };

                if (linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.Count() == 1 && linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
                {
                    throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
                }
                var yeuCauLinhBu = await _yeuCauLinhDuocPhamService.GetByIdAsync(linhBuDuocPhamViewModel.Id,
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
                
                foreach (var item in linhBuDuocPhamViewModel.YeuCauDuocPhamBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox))
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
                yeuCauLinhBu.GhiChu = linhBuDuocPhamViewModel.GhiChu;
                yeuCauLinhBu.DaGui = linhBuDuocPhamViewModel.DaGui;
                if (linhBuDuocPhamViewModel.DaGui == true)
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
                var newPhieu = new GridVoYeuCauLinhDuocTao
                {
                    LoaiDuocPhamHayVatTu = false,
                    YeuCauLinhVatTuId = result.Id
                };
                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
            }
            if (linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == false && d.CheckBox == true).Count() != 0)
            {
                if (linhBuKSNKVM.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == false && d.CheckBox == true).Count() == 1 && linhBuKSNKVM.YeuCauVatTuBenhViens.All(p => p.SLYeuCauLinhThucTe == 0))
                {
                    throw new ApiException(_localizationService.GetResource("LinhBu.SoLuongThucTe.Valid"));
                }
                var yeuCauLinhBu = await _yeuCauLinhKSNKService.GetByIdAsync(linhBuKSNKVM.Id,
                   x =>
                       x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                           .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                           .Include(a => a.KhoNhap)
                           .Include(a => a.YeuCauVatTuBenhViens)
                           .Include(a => a.YeuCauLinhVatTuChiTiets)
                           .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoVatTus).ThenInclude(c => c.NhapKhoVatTuChiTiets)
                           .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                           .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));
                var thongTinKSNKChiTietItems = new List<ThongTinKSNKTietItem>();
                foreach (var item in linhBuKSNKVM.YeuCauVatTuBenhViens.Where(z => z.SLYeuCauLinhThucTe > 0 && z.CheckBox && z.LoaiDuocPhamHayVatTu == false))
                {
                    var thongTinKSNKChiTietItem = new ThongTinKSNKTietItem
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
                    thongTinKSNKChiTietItems.Add(thongTinKSNKChiTietItem);
                }
                yeuCauLinhBu.GhiChu = linhBuKSNKVM.GhiChu;
                yeuCauLinhBu.DaGui = linhBuKSNKVM.DaGui;
                if (linhBuKSNKVM.DaGui == true)
                {
                    yeuCauLinhBu.NgayYeuCau = DateTime.Now;
                }
                await _yeuCauLinhKSNKService.XuLyCapNhatYeuCauLinhKSNKBuAsync(yeuCauLinhBu, thongTinKSNKChiTietItems);
                await _yeuCauLinhKSNKService.UpdateAsync(yeuCauLinhBu);
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
                var newPhieu = new GridVoYeuCauLinhDuocTao
                {
                    LoaiDuocPhamHayVatTu = false,
                    YeuCauLinhVatTuId = result.Id
                };
                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
            }
           
            return Ok(yeuCauLinhDuocPhamVatTuIds);
        }

        [HttpPost("InPhieuLinhBuKSNK")]
        public string InPhieuLinhBuKSNK(PhieuLinhThuongDPVTModel phieuLinhThuongKSNK)
        {
            var result = _yeuCauLinhKSNKService.InPhieuLinhBuKSNK(phieuLinhThuongKSNK);
            return result;
        }
        [HttpPost("InPhieuLinhBuKSNKXemTruoc")]
        public string InPhieuLinhBuKSNKXemTruoc(PhieuLinhThuongVatTuXemTruoc phieuLinhThuongVatTuXemTruoc)
        {
            var htmls = new List<string>();
            if (phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == false).Count() != 0)
            {
                var yeuCauVatTuBenhVienDateTimes = new List<DateTime>();
                var yeuCauVatTuVienIds = new List<long>();
                if (phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens != null)
                {
                    foreach (var item in phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList())
                    {
                        var itemIds = _yeuCauLinhVatTuService.GetIdsYeuCauVT(item.KhoLinhTuId, item.KhoLinhVeId, item.VatTuBenhVienId);
                        if (itemIds.Any())
                        {
                            yeuCauVatTuVienIds.AddRange(itemIds);
                        }
                    }

                }
                if (yeuCauVatTuVienIds.Any())
                {
                    foreach (var item in yeuCauVatTuVienIds)
                    {
                        var itemDateTiem = _yeuCauLinhVatTuService.GetDateTime(item);
                        yeuCauVatTuBenhVienDateTimes.Add(itemDateTiem);
                    }
                }
                if (yeuCauVatTuBenhVienDateTimes.Any())
                {
                    var sortDateTimeTungay = yeuCauVatTuBenhVienDateTimes.OrderBy(s => s).ToList();
                    //r sortDateTimeDenNgay = yeuCauVatTuBenhVienDateTimes.OrderByDescending(s => s).ToList();
                    phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
                    phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopDenNgay = DateTime.Now;
                }
                phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhVienIds = yeuCauVatTuVienIds;
                var result = _yeuCauLinhVatTuService.InPhieuLinhBuVatTuXemTruoc(phieuLinhThuongVatTuXemTruoc);
                htmls.Add(result);
            }
            if (phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == true).Count() != 0)
            {
                var yeuCauDuocPhamBenhVienDateTimes = new List<DateTime>();
                var yeuCauDuocPhamBenhVienIds = new List<long>();
                if (phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens != null)
                {
                    foreach (var item in phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens.Where(d => d.LoaiDuocPhamHayVatTu == true).ToList())
                    {
                        var itemIds = _yeuCauLinhDuocPhamService.GetIdsYeuCauDuocPhamBenhVien(item.KhoLinhTuId, item.KhoLinhVeId, item.VatTuBenhVienId);
                        if (itemIds.Any())
                        {
                            yeuCauDuocPhamBenhVienIds.AddRange(itemIds);
                        }
                    }

                }
                if (yeuCauDuocPhamBenhVienIds.Any())
                {
                    foreach (var item in yeuCauDuocPhamBenhVienIds)
                    {
                        var itemDateTiem = _yeuCauLinhDuocPhamService.GetDateTime(item);
                        yeuCauDuocPhamBenhVienDateTimes.Add(itemDateTiem);
                    }
                }
                if (yeuCauDuocPhamBenhVienDateTimes.Any())
                {
                    var sortDateTimeTungay = yeuCauDuocPhamBenhVienDateTimes.OrderBy(s => s).ToList();
                    //r sortDateTimeDenNgay = yeuCauDuocPhamBenhVienDateTimes.OrderByDescending(s => s).ToList();
                    phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
                    phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopDenNgay = DateTime.Now;
                }
                phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhVienIds = yeuCauDuocPhamBenhVienIds;

                var YeuCauDPBenhViens = phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens
                    .Select(d => new YeuCauDuocPhamBenhViens()
                    {
                        KhoLinhTuId = d.KhoLinhTuId,
                        KhoLinhVeId = d.KhoLinhVeId,
                        DuocPhamBenhVienId = d.VatTuBenhVienId,
                        LoaiDuocPhamHayVatTu = d.LoaiDuocPhamHayVatTu
                    }).ToList();

                var newPhieuLinhThuongDPXemTruoc = new XemPhieuLinhBuDuocPham()
                {
                    HostingName = phieuLinhThuongVatTuXemTruoc.HostingName,
                    KhoLinhId = phieuLinhThuongVatTuXemTruoc.KhoLinhId,
                    KhoLinhBuId = phieuLinhThuongVatTuXemTruoc.KhoLinhBuId,
                    ThoiDiemLinhTongHopDenNgay = phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopDenNgay,
                    ThoiDiemLinhTongHopTuNgay = phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopTuNgay,
                    YeuCauDuocPhamBenhVienIds = phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhVienIds,
                    YeuCauDuocPhamBenhViens = YeuCauDPBenhViens
                };

                var result = _yeuCauLinhDuocPhamService.InPhieuLinhBuDuocPhamXemTruoc(newPhieuLinhThuongDPXemTruoc);
                htmls.Add(result);
            }
            return htmls.Join("<div class=\"pagebreak\"> </div>");
        }

    }
}
