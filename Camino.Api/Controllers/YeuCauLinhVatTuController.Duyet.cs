using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Api.Models.LinhThuongVatTu;
using Camino.Api.Models.LinhVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController
    {
        //TODO : Xem lại DocumentType (Ngọc Anh)
        #region Get data 
        [HttpPost("GetListNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhanVienAsync(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauLinhVatTuService.GetListNhanVienAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetVatTuYeuCauLinhBuDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetVatTuYeuCauLinhBuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetVatTuYeuCauLinhBuDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetVatTuYeuCauLinhBuTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetVatTuYeuCauLinhBuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetVatTuYeuCauLinhBuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoVatTuCanLinhBuDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuCanLinhBuDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetBenhNhanTheoVatTuCanLinhBuDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoVatTuCanLinhBuTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuCanLinhBuTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetBenhNhanTheoVatTuCanLinhBuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //todo: update lại documentype
        [HttpPost("GetVatTuYeuCauLinhTrucTiepDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetVatTuYeuCauLinhTrucTiepDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetVatTuYeuCauLinhTrucTiepDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetVatTuYeuCauLinhTrucTiepTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetVatTuYeuCauLinhTrucTiepTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetVatTuYeuCauLinhTrucTiepTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetBenhNhanTheoVatTuCanLinhTrucTiepDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuCanLinhTrucTiepDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetBenhNhanTheoVatTuCanLinhTrucTiepDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBenhNhanTheoVatTuCanLinhTrucTiepTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetBenhNhanTheoVatTuCanLinhTrucTiepTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetBenhNhanTheoVatTuCanLinhTrucTiepTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Xử lý thêm/xóa/sửa Lĩnh thường

        [HttpGet("GetYeuCauLinhVatTuThuong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhVatTuViewModel>> GetYeuCauLinhVatTuThuongAsync(long id)
        {
            var yeuCauLinh = await _yeuCauLinhVatTuService.GetByIdAsync(id,
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

            var viewModel = yeuCauLinh.ToModel<DuyetYeuCauLinhVatTuViewModel>();

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

            //if (yeuCauLinh.DuocDuyet == true)
            //{
            //    viewModel.NguoiXuatKhoId = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuatId).FirstOrDefault();
            //    viewModel.TenNguoiXuatKho = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuat.User.HoTen).FirstOrDefault();
            //    viewModel.NguoiNhapKhoId = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhanId).FirstOrDefault();
            //    viewModel.TenNguoiNhapKho = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault();
            //}
            return Ok(viewModel);
        }


        [HttpPost("XuLyKhongDuyetYeuCauLinhVatTuThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhVatTuAsync(KhongDuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhVatTu = await _yeuCauLinhVatTuService.GetByIdAsync(yeuCauLinhViewModel.Id, 
                x => x.Include(y => y.YeuCauLinhVatTuChiTiets));
            if (yeuCauLinhVatTu.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuThuong.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);
            
            yeuCauLinhVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
            yeuCauLinhVatTu.NgayDuyet = DateTime.Now;
            yeuCauLinhVatTu.DuocDuyet = false;

            await _yeuCauLinhVatTuService.UpdateAsync(yeuCauLinhVatTu);
            return Ok();
        }

        [HttpPost("XuLyDuyetYeuCauLinhVatTuThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhVatTuThuongAsync(DuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhVatTu = await _yeuCauLinhVatTuService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauLinhVatTuChiTiets));
            if (yeuCauLinhVatTu.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuThuong.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);
            yeuCauLinhVatTu.DuocDuyet = true;
            
            await _yeuCauLinhVatTuService.XuLyDuyetYeuCauLinhVatTuThuongAsync(yeuCauLinhVatTu, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

            var inPhieuVo = new InPhieuDuyetLinhVatTu()
            {
                YeuCauLinhVatTuId = yeuCauLinhVatTu.Id,
                HasHeader = true
            };
            var phieuXuat = await _yeuCauLinhVatTuService.InPhieuDuyetLinhThuongVatTuAsync(inPhieuVo);
            return Ok(phieuXuat);
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù

        [HttpGet("GetYeuCauLinhVatTuBu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhVatTuViewModel>> GetYeuCauLinhVatTuBuAsync(long id)
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
                var currentUser = await _userService.GetCurrentUser();
                viewModel.NguoiXuatKhoId = currentUser?.Id;
                viewModel.TenNguoiXuatKho = currentUser?.HoTen;
            }

            //if (yeuCauLinh.DuocDuyet == true)
            //{
            //    viewModel.NguoiXuatKhoId = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuatId).FirstOrDefault();
            //    viewModel.TenNguoiXuatKho = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiXuat != null).Select(x => x.NguoiXuat.User.HoTen).FirstOrDefault();
            //    viewModel.NguoiNhapKhoId = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhanId).FirstOrDefault();
            //    viewModel.TenNguoiNhapKho = yeuCauLinh.XuatKhoVatTus.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault();
            //}
            return Ok(viewModel);
        }

        [HttpPost("XuLyKhongDuyetYeuCauLinhVatTuBu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhVatTuBuAsync(KhongDuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhVatTu = await _yeuCauLinhVatTuService.GetByIdAsync(yeuCauLinhViewModel.Id); // , x => x.Include(a => a.YeuCauVatTuBenhViens)
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
            await _yeuCauLinhVatTuService.UpdateAsync(yeuCauLinhVatTu);
            return Ok();
        }

        [HttpPost("XuLyDuyetYeuCauLinhVatTuBu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhVatTuBuAsync(DuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhVatTu = await _yeuCauLinhVatTuService
                .GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauLinhVatTuChiTiets).ThenInclude(b => b.YeuCauVatTuBenhVien));
            if (yeuCauLinhVatTu.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuBu.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);
            yeuCauLinhVatTu.DuocDuyet = true;

            await _yeuCauLinhVatTuService.XuLyDuyetYeuCauLinhVatTuBuAsync(yeuCauLinhVatTu, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

            var inPhieuVo = new InPhieuDuyetLinhVatTu()
            {
                YeuCauLinhVatTuId = yeuCauLinhVatTu.Id,
                HasHeader = true
            };
            var phieuXuat = await _yeuCauLinhVatTuService.InPhieuDuyetLinhBuVatTuAsync(inPhieuVo);
            return Ok(phieuXuat);
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh trực tiếp
        [HttpGet("GetYeuCauLinhVatTuTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetYeuCauLinhVatTuViewModel>> GetYeuCauLinhVatTuTrucTiepAsync(long id)
        {
            var yeuCauLinh = await _yeuCauLinhVatTuService.GetByIdAsync(id,
                x =>
                    x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                        .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                        .Include(a => a.KhoNhap)
                        .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoVatTus).ThenInclude(c => c.NhapKhoVatTuChiTiets)
                        .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                        .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User)
                        .Include(a => a.NoiYeuCau).ThenInclude(b => b.KhoaPhong));

            if (yeuCauLinh.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
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

            // thông tin phòng nhập về
            //cập nhật 29/03/2021: update nhập về phòng -> khoa
            //if (yeuCauLinh.DuocDuyet == false)
            //{
            //    viewModel.TenKhoNhap = yeuCauLinh.NoiYeuCau?.Ma + " - " + yeuCauLinh.NoiYeuCau?.Ten;
            //}
            //else
            //{
            //    var phongLinh =
            //        await _yeuCauLinhVatTuService.GetByIdAsync(id, x => x.Include(y => y.YeuCauVatTuBenhViens).ThenInclude(z => z.NoiChiDinh));
            //    viewModel.TenKhoNhap = phongLinh.YeuCauVatTuBenhViens.Select(x => x.NoiChiDinh.Ma + " - " + x.NoiChiDinh.Ten)
            //                               .FirstOrDefault() ?? viewModel.TenKhoNhap;
            //}

            return Ok(viewModel);
        }
        

        [HttpPost("XuLyKhongDuyetYeuCauLinhVatTuTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XuLyKhongDuyetYeuCauLinhVatTuTrucTiepAsync(KhongDuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhVatTu = await _yeuCauLinhVatTuService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauVatTuBenhViens));

            if (yeuCauLinhVatTu.LoaiPhieuLinh != Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }
            if (yeuCauLinhVatTu.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuTrucTiep.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);

            yeuCauLinhVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
            yeuCauLinhVatTu.NgayDuyet = DateTime.Now;
            yeuCauLinhVatTu.DuocDuyet = false;

            foreach (var yeuCauVatTuBenhVien in yeuCauLinhVatTu.YeuCauVatTuBenhViens)
            {
                yeuCauVatTuBenhVien.YeuCauLinhVatTuId = null;
            }

            await _yeuCauLinhVatTuService.UpdateAsync(yeuCauLinhVatTu);
            return Ok();
        }
        

        [HttpPost("XuLyDuyetYeuCauLinhVatTuTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> XuLyDuyetYeuCauLinhVatTuTrucTiepAsync(DuyetYeuCauLinhVatTuViewModel yeuCauLinhViewModel)
        {
            var yeuCauLinhVatTu = await _yeuCauLinhVatTuService.GetByIdAsync(yeuCauLinhViewModel.Id, x => x.Include(a => a.YeuCauVatTuBenhViens));
            if (yeuCauLinhVatTu.DuocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("DuyetYeuCauLinhVatTuTrucTiep.TrangThai.DaDuyet"));
            }

            yeuCauLinhViewModel.ToEntity(yeuCauLinhVatTu);
            yeuCauLinhVatTu.DuocDuyet = true;

            await _yeuCauLinhVatTuService.XuLyDuyetYeuCauLinhVatTuTrucTiepAsync(yeuCauLinhVatTu, yeuCauLinhViewModel.NguoiXuatKhoId ?? 0, yeuCauLinhViewModel.NguoiNhapKhoId ?? 0);

            var inPhieuVo = new InPhieuDuyetLinhVatTu()
            {
                YeuCauLinhVatTuId = yeuCauLinhVatTu.Id,
                HasHeader = true
            };
            var phieuXuat = await _yeuCauLinhVatTuService.InPhieuDuyetLinhTrucTiepVatTuAsync(inPhieuVo);
            return Ok(phieuXuat);
        }
        [HttpPost("InYeuCauLinhVatTuTrucTiep")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<string> InYeuCauLinhVatTuTrucTiep(long yeuCauLinhId ,long loaiPhieuLinh)
        {
            var inPhieuVo = new InPhieuDuyetLinhVatTu()
            {
                YeuCauLinhVatTuId = yeuCauLinhId,
                HasHeader = true
            };
            var phieuXuat = "";
            if (loaiPhieuLinh == 1) // thuong
            {
                var phieuXuatLinhThuong = _yeuCauLinhVatTuService.InPhieuDuyetLinhThuongVatTuAsync(inPhieuVo);
                return phieuXuatLinhThuong;
            }
            else if(loaiPhieuLinh == 2) // bu
            {
                var phieuXuatLinhBu = _yeuCauLinhVatTuService.InPhieuDuyetLinhBuVatTuAsync(inPhieuVo);
                return phieuXuatLinhBu;
            }
            else
            {
               var  phieuXuatLinhTT = _yeuCauLinhVatTuService.InPhieuDuyetLinhTrucTiepVatTuAsync(inPhieuVo);
                return phieuXuatLinhTT;
            }
            return null;
        }
        #endregion
    }
}
