using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController
    {
        //TODO : Xem lại DocumentType (Ngọc Anh)
        #region Get data 
        [HttpPost("GetListNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhanVienAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetListNhanVienAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetDuocPhamYeuCauLinhBuDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDuocPhamYeuCauLinhBuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDuocPhamYeuCauLinhBuDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDuocPhamYeuCauLinhBuTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDuocPhamYeuCauLinhBuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDuocPhamYeuCauLinhBuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoDuocPhamCanLinhBuDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoDuocPhamCanLinhBuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetBenhNhanTheoDuocPhamCanLinhBuDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoDuocPhamCanLinhBuTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoDuocPhamCanLinhBuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetBenhNhanTheoDuocPhamCanLinhBuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //todo: update lại documentype
        [HttpPost("GetDuocPhamYeuCauLinhTrucTiepDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDuocPhamYeuCauLinhTrucTiepDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDuocPhamYeuCauLinhTrucTiepDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDuocPhamYeuCauLinhTrucTiepTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDuocPhamYeuCauLinhTrucTiepTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDuocPhamYeuCauLinhTrucTiepTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoDuocPhamCanLinhTrucTiepDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoDuocPhamCanLinhTrucTiepDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetBenhNhanTheoDuocPhamCanLinhTrucTiepDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoDuocPhamCanLinhTrucTiepTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoDuocPhamCanLinhTrucTiepTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetBenhNhanTheoDuocPhamCanLinhTrucTiepTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Xử lý thêm/xóa/sửa Lĩnh thường

        [HttpGet("GetYeuCauLinhDuocPhamThuong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhDuocPhamViewModel>> GetYeuCauLinhDuocPhamThuongAsync(long id)
        {
            var yeuCauLinh = await _yeuCauLinhDuocPhamService.GetByIdAsync(id,
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
                    var tonTheoDuocPham = thongTinNhapKho.Where(x => x.DuocPhamBenhVienId == linhChiTiet.DuocPhamBenhVienId && x.LaDuocPhamBHYT == linhChiTiet.LaDuocPhamBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
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

            //if (yeuCauLinh.DuocDuyet == true)
            //{
            //    viewModel.NguoiXuatKhoId = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuatId).FirstOrDefault();
            //    viewModel.TenNguoiXuatKho = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuat.User.HoTen).FirstOrDefault();
            //    viewModel.NguoiNhapKhoId = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhanId).FirstOrDefault();
            //    viewModel.TenNguoiNhapKho = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault();
            //}
            return Ok(viewModel);
        }


        [HttpPost("XuLyKhongDuyetYeuCauLinhDuocPhamThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhDuocPhamAsync(KhongDuyetYeuCauLinhViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(yeuCauLinhViewModel.Id, 
                x => x.Include(y => y.YeuCauLinhDuocPhamChiTiets));
            if (yeuCauLinhDuocPham.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhDuocPham);
            
            yeuCauLinhDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
            yeuCauLinhDuocPham.NgayDuyet = DateTime.Now;
            yeuCauLinhDuocPham.DuocDuyet = false;

            await _yeuCauLinhDuocPhamService.UpdateAsync(yeuCauLinhDuocPham);
            return Ok();
        }

        [HttpPost("XuLyDuyetYeuCauLinhDuocPhamThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhDuocPhamThuongAsync(DuyetYeuCauLinhDuocPhamViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauLinhDuocPhamChiTiets));
            if (yeuCauLinhDuocPham.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhDuocPham);
            yeuCauLinhDuocPham.DuocDuyet = true;
            var duocPhamCanXuatVos = yeuCauLinhViewModel.DuyetYeuCauLinhDuocPhamChiTiets
                .Select(o => new DuocPhamCanXuatVo
                {
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                    SoLuongXuat = o.SoLuongCoTheXuat.GetValueOrDefault()
                }).ToList();
            await _yeuCauLinhDuocPhamService.XuLyDuyetYeuCauLinhDuocPhamThuongAsync(yeuCauLinhDuocPham, duocPhamCanXuatVos, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

            var inPhieuVo = new InPhieuDuyetLinhDuocPham()
            {
                YeuCauLinhDuocPhamId = yeuCauLinhDuocPham.Id,
                HasHeader = true
            };
            var phieuXuat = await _yeuCauLinhDuocPhamService.InPhieuDuyetLinhThuongDuocPhamAsync(inPhieuVo);
            return Ok(phieuXuat);
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù

        [HttpGet("GetYeuCauLinhDuocPhamBu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhDuocPhamViewModel>> GetYeuCauLinhDuocPhamBuAsync(long id)
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
                var currentUser = await _userService.GetCurrentUser();
                viewModel.NguoiXuatKhoId = currentUser?.Id;
                viewModel.TenNguoiXuatKho = currentUser?.HoTen;
            }

            //if (yeuCauLinh.DuocDuyet == true)
            //{
            //    viewModel.NguoiXuatKhoId = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuatId).FirstOrDefault();
            //    viewModel.TenNguoiXuatKho = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuat.User.HoTen).FirstOrDefault();
            //    viewModel.NguoiNhapKhoId = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhanId).FirstOrDefault();
            //    viewModel.TenNguoiNhapKho = yeuCauLinh.XuatKhoDuocPhams.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault();
            //}
            return Ok(viewModel);
        }

        [HttpPost("XuLyKhongDuyetYeuCauLinhDuocPhamBu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhDuocPhamBuAsync(KhongDuyetYeuCauLinhViewModel yeuCauLinhViewModel)
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

            //foreach (var yeuCauDuocPhamBenhVien in yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens)
            //{
            //    yeuCauDuocPhamBenhVien.YeuCauLinhDuocPhamId = null;
            //}

            await _yeuCauLinhDuocPhamService.UpdateAsync(yeuCauLinhDuocPham);
            return Ok();
        }

        [HttpPost("XuLyDuyetYeuCauLinhDuocPhamBu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhDuocPhamBuAsync(DuyetYeuCauLinhDuocPhamViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhDuocPham = _yeuCauLinhDuocPhamService
                .GetById(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauLinhDuocPhamChiTiets).ThenInclude(b => b.YeuCauDuocPhamBenhVien));
            if (yeuCauLinhDuocPham.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamBu.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhDuocPham);
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
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh trực tiếp
        [HttpGet("GetYeuCauLinhDuocPhamTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhDuocPhamViewModel>> GetYeuCauLinhDuocPhamTrucTiepAsync(long id)
        {
            var yeuCauLinh = await _yeuCauLinhDuocPhamService.GetByIdAsync(id,
                x =>
                    x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                        .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                        .Include(a => a.KhoNhap)
                        .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoDuocPhams).ThenInclude(c => c.NhapKhoDuocPhamChiTiets)
                        .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                        .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User)
                        .Include(a => a.NoiYeuCau).ThenInclude(b => b.KhoaPhong));

            if (yeuCauLinh.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
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

            // thông tin phòng nhập về
            //cập nhật 29/03/2021: update nhập về phòng -> khoa
            //if (yeuCauLinh.DuocDuyet == false)
            //{
            //    viewModel.TenKhoNhap = yeuCauLinh.NoiYeuCau?.Ma + " - " + yeuCauLinh.NoiYeuCau?.Ten;
            //}
            //else
            //{
            //    var temp =
            //        await _yeuCauLinhDuocPhamService.GetByIdAsync(id,
            //            x => x.Include(y => y.YeuCauDuocPhamBenhViens).ThenInclude(z => z.NoiChiDinh));
            //    viewModel.TenKhoNhap = temp.YeuCauDuocPhamBenhViens
            //                               .Select(x => x.NoiChiDinh.Ma + " - " + x.NoiChiDinh.Ten)
            //                               .FirstOrDefault() ?? viewModel.TenKhoNhap;
            //}

            return Ok(viewModel);
        }
        

        [HttpPost("XuLyKhongDuyetYeuCauLinhDuocPhamTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhDuocPhamTrucTiepAsync(KhongDuyetYeuCauLinhViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauDuocPhamBenhViens));

            if (yeuCauLinhDuocPham.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }
            if (yeuCauLinhDuocPham.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamTrucTiep.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhDuocPham);

            yeuCauLinhDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
            yeuCauLinhDuocPham.NgayDuyet = DateTime.Now;
            yeuCauLinhDuocPham.DuocDuyet = false;
            foreach (var yeuCauDuocPhamBenhVien in yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens)
            {
                yeuCauDuocPhamBenhVien.YeuCauLinhDuocPhamId = null;
            }

            await _yeuCauLinhDuocPhamService.UpdateAsync(yeuCauLinhDuocPham);
            return Ok();
        }
        

        [HttpPost("XuLyDuyetYeuCauLinhDuocPhamTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhDuocPhamTrucTiepAsync(DuyetYeuCauLinhDuocPhamViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauDuocPhamBenhViens));
            if (yeuCauLinhDuocPham.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamTrucTiep.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhDuocPham);
            yeuCauLinhDuocPham.DuocDuyet = true;

            await _yeuCauLinhDuocPhamService.XuLyDuyetYeuCauLinhDuocPhamTrucTiepAsync(yeuCauLinhDuocPham, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

            var inPhieuVo = new InPhieuDuyetLinhDuocPham()
            {
                YeuCauLinhDuocPhamId = yeuCauLinhDuocPham.Id,
                HasHeader = true
            };
            var phieuXuat = await _yeuCauLinhDuocPhamService.InPhieuDuyetLinhTrucTiepDuocPhamAsync(inPhieuVo);
            return Ok(phieuXuat);
        }
        [HttpPost("InYeuCauLinhDuocPhamTrucTiep")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<string> InYeuCauLinhDuocPhamTrucTiep(long  yeuCauLinhId,long loaiPhieuLinh)
        {
            var inPhieuVo = new InPhieuDuyetLinhDuocPham()
            {
                YeuCauLinhDuocPhamId = yeuCauLinhId,
                HasHeader = true
            };
            if (loaiPhieuLinh == 1) // thuong
            {
                var phieuXuatLinhThuong = _yeuCauLinhDuocPhamService.InPhieuDuyetLinhThuongDuocPhamAsync(inPhieuVo);
                return phieuXuatLinhThuong;
            }
            else if (loaiPhieuLinh == 2) // bu
            {
                var phieuXuatLinhBu = _yeuCauLinhDuocPhamService.InPhieuDuyetLinhBuDuocPhamAsync(inPhieuVo);
                return phieuXuatLinhBu;
            }
            else
            {
                var phieuXuat = _yeuCauLinhDuocPhamService.InPhieuDuyetLinhTrucTiepDuocPhamAsync(inPhieuVo);
                return phieuXuat;
            }
            
            return null;
        }
        #endregion
    }
}
