using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhKSNK;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Api.Models.LinhThuongKSNK;
using Camino.Api.Models.LinhThuongVatTu;
using Camino.Api.Models.LinhVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhKSNKController
    {
        //TODO : Xem lại DocumentType (Ngọc Anh)
        #region Get data 
        [HttpPost("GetListNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhanVienAsync(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauLinhKSNKService.GetListNhanVienAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetKSNKYeuCauLinhBuDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetKSNKYeuCauLinhBuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetKSNKYeuCauLinhBuDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetKSNKYeuCauLinhBuTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetVatTuYeuCauLinhBuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetKSNKYeuCauLinhBuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoKSNKCanLinhBuDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoKSNKCanLinhBuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetBenhNhanTheoKSNKCanLinhBuDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoKSNKCanLinhBuTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuCanLinhBuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetBenhNhanTheoKSNKCanLinhBuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //todo: update lại documentype
        [HttpPost("GetKSNKYeuCauLinhTrucTiepDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetKSNKYeuCauLinhTrucTiepDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetKSNKYeuCauLinhTrucTiepDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetKSNKYeuCauLinhTrucTiepTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetKSNKYeuCauLinhTrucTiepTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetKSNKYeuCauLinhTrucTiepTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoKSNKCanLinhTrucTiepDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuCanLinhTrucTiepDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetBenhNhanTheoKSNKCanLinhTrucTiepDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoKSNKCanLinhTrucTiepTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoKSNKCanLinhTrucTiepTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetBenhNhanTheoKSNKCanLinhTrucTiepTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Xử lý thêm/xóa/sửa Lĩnh thường

        [HttpPost("GetYeuCauLinhKSNKThuong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhKSNKViewModel>> GetYeuCauLinhKSNKThuongAsync(VoGetInFoKSNk vo)
        {
            if(vo.LoaiDuocPhamHayVatTu == true)
            {
                var yeuCauLinh = await _yeuCauLinhDuocPhamService.GetByIdAsync(vo.YeuCauLinhId,
                x => x.Include(a => a.YeuCauLinhDuocPhamChiTiets).ThenInclude(b => b.DuocPhamBenhVien).ThenInclude(c => c.DuocPham).ThenInclude(e => e.DonViTinh)
                      .Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                      .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                      .Include(a => a.KhoNhap)
                      .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoDuocPhams).ThenInclude(c => c.NhapKhoDuocPhamChiTiets)
                      .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                      .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));

                if (yeuCauLinh.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhDuTru)
                {
                    throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
                }

                var viewModel = yeuCauLinh.ToModel<DuyetYeuCauLinhDuocPhamViewModel>();
               

                viewModel.DuyetYeuCauLinhDuocPhamChiTiets = viewModel.DuyetYeuCauLinhDuocPhamChiTiets.OrderBy(x => x.Nhom).ThenBy(x => x.TenDuocPham).ToList();
                var yeuCauLinhDuocPhamChiTiet = viewModel.DuyetYeuCauLinhDuocPhamChiTiets;

                var thongTinNhapKho = yeuCauLinh.KhoXuat.NhapKhoDuocPhams.SelectMany(x => x.NhapKhoDuocPhamChiTiets).Where(y => y.SoLuongDaXuat < y.SoLuongNhap).ToList();
                if (thongTinNhapKho.Any())
                {
                    var index = 0;
                    foreach (var linhChiTiet in yeuCauLinhDuocPhamChiTiet)
                    {
                        linhChiTiet.Index = index++;
                        var tonTheoDuocPham = thongTinNhapKho.Where(x => x.DuocPhamBenhVienId == linhChiTiet.DuocPhamBenhVienId && x.LaDuocPhamBHYT == linhChiTiet.LaDuocPhamBHYT && x.NhapKhoDuocPhams.KhoId == yeuCauLinh.KhoXuatId).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                        linhChiTiet.SLTon = tonTheoDuocPham;
                    }
                }

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
                    var currentUser = await _userService.GetCurrentUser();
                    viewModel.NguoiXuatKhoId = currentUser?.Id;
                    viewModel.TenNguoiXuatKho = currentUser?.HoTen;
                }

                var tenKhoLinh = yeuCauLinh?.KhoXuat?.Ten;

                var yeuCauChiTiet = viewModel.DuyetYeuCauLinhDuocPhamChiTiets
                                             .Select(d => new DuyetYeuCauLinhKSNKChiTietViewModel() {
                                                 VatTuBenhVienId = d.DuocPhamBenhVienId,
                                                 LaVatTuBHYT = d.LaDuocPhamBHYT,
                                                 TenVatTu =d.TenDuocPham,
                                                 SoLuong = d.SoLuong,
                                                 SLTon = d.SLTon,
                                                 SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                                                 Nhom = d.Nhom,
                                                 DVT = d.DVT,
                                                 HangSanXuat = d.HangSanXuat,
                                                 NuocSanXuat = d.NuocSanXuat,
                                                 isTuChoi = d.isTuChoi,
                                                 Index = d.Index,
                                                 TenKhoLinh = tenKhoLinh,
                                                 LoaiDuocPhamHayVatTu = true,
                                                 Id = d.Id
                                                 // lĩnh bù
                                                 // to do
                                                 // end lĩnh bù

                                             }).ToList();
                var viewModelBind = new DuyetYeuCauLinhKSNKViewModel()
                {
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
                    DaGui = viewModel.DaGui,
                    LyDoKhongDuyet = viewModel.LyDoKhongDuyet,
                    DuyetYeuCauLinhVatTuChiTiets = yeuCauChiTiet,
                    LastModified = viewModel.LastModified,
                    LaNguoiTaoPhieu = viewModel.LaNguoiTaoPhieu,
                    Id = vo.YeuCauLinhId
                };
                return Ok(viewModelBind);
            }
            else
            {
                var yeuCauLinh = await _yeuCauLinhKSNKService.GetByIdAsync(vo.YeuCauLinhId,
                x => x.Include(a => a.YeuCauLinhVatTuChiTiets).ThenInclude(b => b.VatTuBenhVien).ThenInclude(c => c.VatTus)
                      .Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                      .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                      .Include(a => a.KhoNhap)
                      .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoVatTus).ThenInclude(c => c.NhapKhoVatTuChiTiets)
                      .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                      .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));

                if (yeuCauLinh.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhDuTru)
                {
                    throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
                }

                var viewModel = yeuCauLinh.ToModel<DuyetYeuCauLinhKSNKViewModel>();

                viewModel.DuyetYeuCauLinhVatTuChiTiets = viewModel.DuyetYeuCauLinhVatTuChiTiets.OrderBy(x => x.Nhom).ThenBy(x => x.TenVatTu).ToList();
                var yeuCauLinhVatTuChiTiet = viewModel.DuyetYeuCauLinhVatTuChiTiets;
                var thongTinNhapKho = yeuCauLinh.KhoXuat.NhapKhoVatTus.SelectMany(x => x.NhapKhoVatTuChiTiets).Where(y => y.SoLuongDaXuat < y.SoLuongNhap).ToList();
                if (thongTinNhapKho.Any())
                {
                    var index = 0;
                    foreach (var linhChiTiet in yeuCauLinhVatTuChiTiet)
                    {
                        linhChiTiet.Index = index++;
                        var tonTheoVatTu = thongTinNhapKho.Where(x => x.VatTuBenhVienId == linhChiTiet.VatTuBenhVienId && x.LaVatTuBHYT == linhChiTiet.LaVatTuBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                        linhChiTiet.SLTon = tonTheoVatTu;
                    }
                }

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
                    var currentUser = await _userService.GetCurrentUser();
                    viewModel.NguoiXuatKhoId = currentUser?.Id;
                    viewModel.TenNguoiXuatKho = currentUser?.HoTen;
                }

                var tenKhoLinh = yeuCauLinh?.KhoXuat?.Ten;
                foreach(var item in viewModel.DuyetYeuCauLinhVatTuChiTiets.ToList())
                {
                    item.TenKhoLinh = tenKhoLinh;
                }
                return Ok(viewModel);
            }

           
        }


        [HttpPost("XuLyKhongDuyetYeuCauLinhKSNKThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhKSNKAsync(KhongDuyetYeuCauLinhKSNKViewModel yeuCauLinhViewModel)
        {
            if (yeuCauLinhViewModel.DuyetYeuCauLinhVatTuChiTiets.Where(d => d.LoaiDuocPhamHayVatTu == true).Count() != 0)
            {
                {
                    var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(yeuCauLinhViewModel.Id,
                    x => x.Include(y => y.YeuCauLinhDuocPhamChiTiets));
                    if (yeuCauLinhDuocPham.DuocDuyet == true)
                    {
                        throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.TrangThai.DaDuyet"));
                    }
                    var yeuCauChiTiets = yeuCauLinhViewModel.DuyetYeuCauLinhVatTuChiTiets.Where(d => d.LoaiDuocPhamHayVatTu == true)
                         .Select(d => new DuyetYeuCauLinhDuocPhamChiTietViewModel()
                         {
                             Index = d.Index,
                             DuocPhamBenhVienId = (long)d.VatTuBenhVienId,
                             LaDuocPhamBHYT = (bool)d.LaVatTuBHYT,
                             TenDuocPham = d.TenVatTu,
                             HangSanXuat = d.HangSanXuat,
                             DVT = d.DVT,
                             NuocSanXuat = d.NhaSx,
                             SLTon = (double)d.SLTon,
                             SoLuong = (double)d.SoLuong,
                             SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                             Nhom = d.Nhom,
                             isTuChoi = d.isTuChoi,

                         }).ToList();

                    var newBind = new KhongDuyetYeuCauLinhViewModel()
                    {
                        LastModified = yeuCauLinhViewModel.LastModified,
                        Id = yeuCauLinhViewModel.Id,
                        DuyetYeuCauLinhDuocPhamChiTiets = yeuCauChiTiets,
                        LyDoKhongDuyet = yeuCauLinhViewModel.LyDoKhongDuyet
                    };
                    newBind.ToEntity(yeuCauLinhDuocPham);

                    yeuCauLinhDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                    yeuCauLinhDuocPham.NgayDuyet = DateTime.Now;
                    yeuCauLinhDuocPham.DuocDuyet = false;

                    await _yeuCauLinhDuocPhamService.UpdateAsync(yeuCauLinhDuocPham);
                    return Ok();
                }
            }
                var yeuCauLinhVatTu = await _yeuCauLinhKSNKService.GetByIdAsync(yeuCauLinhViewModel.Id,
                x => x.Include(y => y.YeuCauLinhVatTuChiTiets));
            if (yeuCauLinhVatTu.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuThuong.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);

            yeuCauLinhVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
            yeuCauLinhVatTu.NgayDuyet = DateTime.Now;
            yeuCauLinhVatTu.DuocDuyet = false;

            await _yeuCauLinhKSNKService.UpdateAsync(yeuCauLinhVatTu);
            return Ok();
        }

        [HttpPost("XuLyDuyetYeuCauLinhKSNKThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhKSNKThuongAsync(DuyetYeuCauLinhKSNKViewModel yeuCauLinhViewModel)
        {
            if(yeuCauLinhViewModel.DuyetYeuCauLinhVatTuChiTiets.Where(d=>d.LoaiDuocPhamHayVatTu == true).Count() != 0)
            {
                var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauLinhDuocPhamChiTiets));
                if (yeuCauLinhDuocPham.DuocDuyet == true)
                {
                    throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.TrangThai.DaDuyet"));
                }
                var duyetYeuCauLinhDuocPhamChiTiets = yeuCauLinhViewModel.DuyetYeuCauLinhVatTuChiTiets.Where(d => d.LoaiDuocPhamHayVatTu == true)
                 .Select(d => new DuyetYeuCauLinhDuocPhamChiTietViewModel()
                 {
                     Index = d.Index,
                     DuocPhamBenhVienId = (long)d.VatTuBenhVienId,
                     LaDuocPhamBHYT = (bool)d.LaVatTuBHYT,
                     TenDuocPham = d.TenVatTu,
                     HangSanXuat = d.HangSanXuat,
                     DVT = d.DVT,
                     NuocSanXuat = d.NhaSx,
                     SLTon = (double)d.SLTon,
                     SoLuong = (double)d.SoLuong,
                     SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                     Nhom = d.Nhom,
                     isTuChoi = d.isTuChoi,
                     Id = d.Id
                 }).ToList();
           
                var viewModelBind = new DuyetYeuCauLinhKSNKViewModel()
                {
                    KhoXuatId = yeuCauLinhViewModel.KhoXuatId,
                    TenKhoXuat = yeuCauLinhViewModel.TenKhoXuat,
                    KhoNhapId = yeuCauLinhViewModel.KhoNhapId,
                    TenKhoNhap = yeuCauLinhViewModel.TenKhoNhap,
                    NhanVienDuyetId = yeuCauLinhViewModel.NhanVienDuyetId,
                    TenNhanVienDuyet = yeuCauLinhViewModel.TenNhanVienDuyet,
                    NhanVienYeuCauId = yeuCauLinhViewModel.NhanVienYeuCauId,
                    TenNhanVienYeuCau = yeuCauLinhViewModel.TenNhanVienYeuCau,
                    NgayYeuCau = yeuCauLinhViewModel.NgayYeuCau,
                    NgayDuyet = yeuCauLinhViewModel.NgayDuyet,
                    GhiChu = yeuCauLinhViewModel.GhiChu,
                    NguoiXuatKhoId = yeuCauLinhViewModel.NguoiXuatKhoId,
                    TenNguoiXuatKho = yeuCauLinhViewModel.TenNguoiXuatKho,
                    NguoiNhapKhoId = yeuCauLinhViewModel.NguoiNhapKhoId,
                    TenNguoiNhapKho = yeuCauLinhViewModel.TenNguoiNhapKho,
                    LoaiPhieuLinh = yeuCauLinhViewModel.LoaiPhieuLinh,
                    DuocDuyet = yeuCauLinhViewModel.DuocDuyet,
                    DaGui = yeuCauLinhViewModel.DaGui,
                    LyDoKhongDuyet = yeuCauLinhViewModel.LyDoKhongDuyet,
                    DuyetYeuCauLinhVatTuChiTiets = yeuCauLinhViewModel.DuyetYeuCauLinhVatTuChiTiets,
                    LastModified = yeuCauLinhViewModel.LastModified,
                    LaNguoiTaoPhieu = yeuCauLinhViewModel.LaNguoiTaoPhieu,
                    Id = yeuCauLinhViewModel.Id
                };
                var viewModelBindDP = new DuyetYeuCauLinhDuocPhamViewModel()
                {
                    KhoXuatId = viewModelBind.KhoXuatId,
                    TenKhoXuat = viewModelBind.TenKhoXuat,
                    KhoNhapId = viewModelBind.KhoNhapId,
                    TenKhoNhap = viewModelBind.TenKhoNhap,
                    NhanVienDuyetId = viewModelBind.NhanVienDuyetId,
                    TenNhanVienDuyet = viewModelBind.TenNhanVienDuyet,
                    NhanVienYeuCauId = viewModelBind.NhanVienYeuCauId,
                    TenNhanVienYeuCau = viewModelBind.TenNhanVienYeuCau,
                    NgayYeuCau = viewModelBind.NgayYeuCau,
                    NgayDuyet = viewModelBind.NgayDuyet,
                    GhiChu = viewModelBind.GhiChu,
                    NguoiXuatKhoId = viewModelBind.NguoiXuatKhoId,
                    TenNguoiXuatKho = viewModelBind.TenNguoiXuatKho,
                    NguoiNhapKhoId = viewModelBind.NguoiNhapKhoId,
                    TenNguoiNhapKho = viewModelBind.TenNguoiNhapKho,
                    LoaiPhieuLinh = viewModelBind.LoaiPhieuLinh,
                    DuocDuyet = viewModelBind.DuocDuyet,
                    DaGui = viewModelBind.DaGui,
                    LyDoKhongDuyet = viewModelBind.LyDoKhongDuyet,
                    DuyetYeuCauLinhDuocPhamChiTiets = duyetYeuCauLinhDuocPhamChiTiets,
                    LastModified = viewModelBind.LastModified,
                    LaNguoiTaoPhieu = viewModelBind.LaNguoiTaoPhieu,
                    Id = viewModelBind.Id
                };

                viewModelBindDP.ToEntity(yeuCauLinhDuocPham);
                yeuCauLinhDuocPham.DuocDuyet = true;

                var duocPhamCanXuatVos = duyetYeuCauLinhDuocPhamChiTiets
                    .Select(o => new DuocPhamCanXuatVo
                    {
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                        SoLuongXuat = o.SoLuongCoTheXuat.GetValueOrDefault()
                    }).ToList();
                await _yeuCauLinhDuocPhamService.XuLyDuyetYeuCauLinhDuocPhamThuongAsync(yeuCauLinhDuocPham, duocPhamCanXuatVos, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

                var inPhieuVos = new InPhieuDuyetLinhDuocPham()
                {
                    YeuCauLinhDuocPhamId = yeuCauLinhDuocPham.Id,
                    HasHeader = true
                };
                var phieuXuats = await _yeuCauLinhDuocPhamService.InPhieuDuyetLinhThuongDuocPhamAsync(inPhieuVos);
                return Ok(phieuXuats);
            }
            var yeuCauLinhVatTu = await _yeuCauLinhKSNKService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauLinhVatTuChiTiets));
            if (yeuCauLinhVatTu.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuThuong.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);
            yeuCauLinhVatTu.DuocDuyet = true;

            await _yeuCauLinhKSNKService.XuLyDuyetYeuCauLinhKSNKThuongAsync(yeuCauLinhVatTu, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

            var inPhieuVo = new InPhieuDuyetLinhKSNK()
            {
                YeuCauLinhVatTuId = yeuCauLinhVatTu.Id,
                HasHeader = true
            };
            var phieuXuat = await _yeuCauLinhKSNKService.InPhieuDuyetLinhThuongKSNKAsync(inPhieuVo);
            return Ok(phieuXuat);
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù

        [HttpPost("GetYeuCauLinhKSNKBu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhVatTuViewModel>> GetYeuCauLinhKSNKAsync(VoGetInFoKSNk vo)
        {
            if (vo.LoaiDuocPhamHayVatTu == true)
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
                    var currentUser = await _userService.GetCurrentUser();
                    viewModel.NguoiXuatKhoId = currentUser?.Id;
                    viewModel.TenNguoiXuatKho = currentUser?.HoTen;
                }


                var duyetYeuCauLinhVatTuChiTiets = viewModel.DuyetYeuCauLinhDuocPhamChiTiets
                    .Select(d => new DuyetYeuCauLinhVatTuChiTietViewModel() {
                        VatTuBenhVienId = d.DuocPhamBenhVienId,
                        LaVatTuBHYT = d.LaDuocPhamBHYT,

                        TenVatTu = d.TenDuocPham,
                        SoLuong = d.SoLuong,
                        SLTon = d.SLTon,
                        SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                        Nhom = d.Nhom,
                        DVT = d.DVT,
                        HangSanXuat = d.HangSanXuat,
                        NuocSanXuat = d.NuocSanXuat,
                        NhaSx = d.HangSanXuat,
                        isTuChoi = d.isTuChoi,
                        Index = d.Index,
                        Id = d.Id

                    })
                    .ToList();

                var viewModels = new DuyetYeuCauLinhVatTuViewModel()
                {
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

                    DaGui = viewModel.DaGui,
                    LyDoKhongDuyet = viewModel.LyDoKhongDuyet,
                    DuyetYeuCauLinhVatTuChiTiets = duyetYeuCauLinhVatTuChiTiets,
                    LastModified = viewModel.LastModified,
                    LaNguoiTaoPhieu = viewModel.LaNguoiTaoPhieu,
                    Id = viewModel.Id
                };

                return Ok(viewModels);
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
                    var currentUser = await _userService.GetCurrentUser();
                    viewModel.NguoiXuatKhoId = currentUser?.Id;
                    viewModel.TenNguoiXuatKho = currentUser?.HoTen;
                }

                
                return Ok(viewModel);
            }
           
        }

        [HttpPost("XuLyKhongDuyetYeuCauLinhKSNKBu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhKSNKBuAsync(KhongDuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            if(yeuCauLinhViewModel.LoaiDuocPhamHayVatTu == true)
            {
                var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(yeuCauLinhViewModel.Id); //, x => x.Include(a => a.YeuCauDuocPhamBenhViens)
                if (yeuCauLinhDuocPham.DuocDuyet == true)
                {
                    throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamBu.TrangThai.DaDuyet"));
                }

                yeuCauLinhDuocPham.LyDoKhongDuyet = yeuCauLinhViewModel.LyDoKhongDuyet;
                yeuCauLinhDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauLinhDuocPham.NgayDuyet = DateTime.Now;
                yeuCauLinhDuocPham.DuocDuyet = false;


                await _yeuCauLinhDuocPhamService.UpdateAsync(yeuCauLinhDuocPham);
                return Ok();
            }
            else
            {
                var yeuCauLinhVatTu = await _yeuCauLinhKSNKService.GetByIdAsync(yeuCauLinhViewModel.Id); // , x => x.Include(a => a.YeuCauVatTuBenhViens)
                if (yeuCauLinhVatTu.DuocDuyet == true)
                {
                    throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuBu.TrangThai.DaDuyet"));
                }

                yeuCauLinhVatTu.LyDoKhongDuyet = yeuCauLinhViewModel.LyDoKhongDuyet;
                yeuCauLinhVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauLinhVatTu.NgayDuyet = DateTime.Now;
                yeuCauLinhVatTu.DuocDuyet = false;

                //foreach (var yeuCauVatTuBenhVien in yeuCauLinhVatTu.YeuCauVatTuBenhViens)
                //{
                //    yeuCauVatTuBenhVien.YeuCauLinhVatTuId = null;
                //}
                await _yeuCauLinhKSNKService.UpdateAsync(yeuCauLinhVatTu);
                return Ok();
            }
            
        }

        [HttpPost("XuLyDuyetYeuCauLinhKSNKBu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhKSNKBuAsync(DuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            if(yeuCauLinhViewModel.LoaiDuocPhamHayVatTu == true)
            {
                var duyetYeuCauLinhDuocPhamChiTietViewModel = yeuCauLinhViewModel.DuyetYeuCauLinhVatTuChiTiets
                    .Select(d => new DuyetYeuCauLinhDuocPhamChiTietViewModel()
                    {
                        Index = d.Index,
                        DuocPhamBenhVienId =(long) d.VatTuBenhVienId,
                        LaDuocPhamBHYT = (bool)d.LaVatTuBHYT,
                        TenDuocPham = d.TenVatTu,
                        DVT = d.DVT,
                        HangSanXuat = d.HangSanXuat,
                        NuocSanXuat = d.NuocSanXuat,
                        SLTon = (double)d.SLTon,
                        SoLuong = (double)d.SoLuong,
                        SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                        Nhom = d.Nhom,
                        isTuChoi = d.isTuChoi,
                        Id = d.Id
                    }).ToList();
                var duyetYeuCauLinhDuocPhamViewModel = new DuyetYeuCauLinhDuocPhamViewModel()
                {
                    Id = yeuCauLinhViewModel.Id,
                    KhoXuatId = yeuCauLinhViewModel.KhoXuatId,
                    TenKhoXuat = yeuCauLinhViewModel.TenKhoXuat,
                    KhoNhapId = yeuCauLinhViewModel.KhoNhapId,
                    TenKhoNhap = yeuCauLinhViewModel.TenKhoNhap,
                    LoaiPhieuLinh = yeuCauLinhViewModel.LoaiPhieuLinh,

                    NhanVienYeuCauId = yeuCauLinhViewModel.NhanVienYeuCauId,
                    TenNhanVienYeuCau = yeuCauLinhViewModel.TenNhanVienYeuCau,
                    NhanVienDuyetId = yeuCauLinhViewModel.NhanVienDuyetId,
                    TenNhanVienDuyet = yeuCauLinhViewModel.TenNhanVienDuyet,
                    NgayYeuCau = yeuCauLinhViewModel.NgayYeuCau,
                    NgayDuyet = yeuCauLinhViewModel.NgayDuyet,
                    GhiChu = yeuCauLinhViewModel.GhiChu,
                    NguoiXuatKhoId = yeuCauLinhViewModel.NguoiXuatKhoId,
                    TenNguoiXuatKho = yeuCauLinhViewModel.TenNguoiXuatKho,
                    NguoiNhapKhoId = yeuCauLinhViewModel.NguoiNhapKhoId,
                    TenNguoiNhapKho = yeuCauLinhViewModel.TenNguoiNhapKho,
                    DuocDuyet = yeuCauLinhViewModel.DuocDuyet,
                    DaGui = yeuCauLinhViewModel.DaGui,
                    LyDoKhongDuyet = yeuCauLinhViewModel.LyDoKhongDuyet,
                    LastModified = yeuCauLinhViewModel.LastModified,
                    LaNguoiTaoPhieu = yeuCauLinhViewModel.LaNguoiTaoPhieu,
                    DuyetYeuCauLinhDuocPhamChiTiets = duyetYeuCauLinhDuocPhamChiTietViewModel,

                };

                var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService
                .GetByIdAsync(duyetYeuCauLinhDuocPhamViewModel.Id, x => x.Include(a => a.YeuCauLinhDuocPhamChiTiets).ThenInclude(b => b.YeuCauDuocPhamBenhVien));
                if (yeuCauLinhDuocPham.DuocDuyet == true)
                {
                    throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamBu.TrangThai.DaDuyet"));
                }

                duyetYeuCauLinhDuocPhamViewModel.ToEntity(yeuCauLinhDuocPham);
                yeuCauLinhDuocPham.DuocDuyet = true;

                await _yeuCauLinhDuocPhamService.XuLyDuyetYeuCauLinhDuocPhamBuAsync(yeuCauLinhDuocPham, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

                var inPhieuVo = new InPhieuDuyetLinhDuocPham()
                {
                    YeuCauLinhDuocPhamId = yeuCauLinhDuocPham.Id,
                    HasHeader = true
                };
                var phieuXuat = await _yeuCauLinhDuocPhamService.InPhieuDuyetLinhBuDuocPhamAsync(inPhieuVo);
                return Ok(phieuXuat);
            }
            else
            {
                var yeuCauLinhVatTu = await _yeuCauLinhKSNKService
                .GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauLinhVatTuChiTiets).ThenInclude(b => b.YeuCauVatTuBenhVien));
                if (yeuCauLinhVatTu.DuocDuyet == true)
                {
                    throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuBu.TrangThai.DaDuyet"));
                }

                yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);
                yeuCauLinhVatTu.DuocDuyet = true;

                await _yeuCauLinhKSNKService.XuLyDuyetYeuCauLinhKSNKBuAsync(yeuCauLinhVatTu, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

                var inPhieuVo = new InPhieuDuyetLinhKSNK()
                {
                    YeuCauLinhVatTuId = yeuCauLinhVatTu.Id,
                    HasHeader = true
                };
                var phieuXuat = await _yeuCauLinhKSNKService.InPhieuDuyetLinhBuKSNKAsync(inPhieuVo);
                return Ok(phieuXuat);
            }
            
           
        }
        #endregion
       
    }
}
