using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using System.Collections.Generic;
using System;
using Camino.Api.Models.KhamDoan;
using System.Linq;
using Camino.Api.Models.Error;
using Camino.Api.Extensions;
using Camino.Core.Domain.Entities.KhamDoans;
using static Camino.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDanhSachKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]//DanhSachKhamSucKhoeDoan
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachKhamSucKhoeDoan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachGoiKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachKhamSucKhoeDoan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForGridAsyncDanhSachGoiKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }

        #region CRUD

        [HttpPost("ThemDichVuKhamDoan")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> ThemDichVuKhamDoan(GoiKhamDichVuKhamSucKhoeDoanViewModel viewModel) // DichVuKhamSucKhoeDoanViewModelValidator
        {
            viewModel.LoaiGia = await _khamDoanService.GetTenNhomGiaDichVuKhamBenhBenhVien(viewModel.NhomGiaDichVuKyThuatBenhVienId, viewModel.LaDichVuKham);
            foreach (var item in viewModel.GoiKhamSucKhoeNoiThucHiens)
            {
                viewModel.NoiThucHienString += await _khamDoanService.GetNoiThucHienString(item.PhongBenhVienId) + "; ";
            }
            if (viewModel.LaDichVuKham)
            {
                viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
            }
            else
            {
                if (viewModel.MaNhomDichVuBenhVien == "XN" || viewModel.MaNhomDichVuBenhVienCha == "XN")
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.XetNghiem;
                }
                else if (viewModel.MaNhomDichVuBenhVien == "CĐHA" || viewModel.MaNhomDichVuBenhVienCha == "CĐHA")
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh;
                }
                else if (viewModel.MaNhomDichVuBenhVien == "TDCN" || viewModel.MaNhomDichVuBenhVienCha == "TDCN")
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang;
                }
                else
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.KH;
                }
            }
            //if (viewModel.DonGiaChuaUuDai == null || viewModel.DonGiaChuaUuDai == 0)
            //{
            //    viewModel.DonGiaChuaUuDai = 0;
            //}
            return Ok(viewModel);
        }

        [HttpPost("CapNhatDichVuKhamDoan")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> CapNhatDichVuKhamDoan(GoiKhamDichVuKhamSucKhoeDoanViewModel viewModel)//KhamSucKhoeDoanViewModelValidator
        {
            viewModel.LoaiGia = await _khamDoanService.GetTenNhomGiaDichVuKhamBenhBenhVien(viewModel.NhomGiaDichVuKyThuatBenhVienId, viewModel.LaDichVuKham);
            foreach (var item in viewModel.GoiKhamSucKhoeNoiThucHiens)
            {
                viewModel.NoiThucHienString += await _khamDoanService.GetNoiThucHienString(item.PhongBenhVienId) + "; ";
            }
            if (viewModel.LaDichVuKham)
            {
                viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
            }
            else
            {
                if (viewModel.MaNhomDichVuBenhVien == "XN" || viewModel.MaNhomDichVuBenhVienCha == "XN")
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.XetNghiem;
                }
                else if (viewModel.MaNhomDichVuBenhVien == "CĐHA" || viewModel.MaNhomDichVuBenhVienCha == "CĐHA")
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh;
                }
                else if (viewModel.MaNhomDichVuBenhVien == "TDCN" || viewModel.MaNhomDichVuBenhVienCha == "TDCN")
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang;
                }
                else
                {
                    viewModel.Nhom = NhomDichVuChiDinhKhamSucKhoe.KH;
                }
            }
            return Ok(viewModel);
        }

        [HttpPost("ThemGoiKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> ThemGoiKhamSucKhoeDoan(GoiKhamSucKhoeViewModel viewModel)
        {
            //if (
            //      !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat))
            //{
            //    throw new ApiException(_localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.Required"));
            //}

            if (!viewModel.DichVuKhamSucKhoeDoans.Any())
            {
                throw new ApiException("Vui lòng chọn ít nhất 1 dịch vụ trong gói khám");
            }

            var goiKhamSucKhoe = viewModel.ToEntity<GoiKhamSucKhoe>();
            foreach (var item in viewModel.DichVuKhamSucKhoeDoans.Where(p => p.LaDichVuKham))
            {
                var goiKhamKhamBenh = new GoiKhamSucKhoeDichVuKhamBenh
                {
                    DichVuKhamBenhBenhVienId = item.DichVuKyThuatBenhVienId.Value,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                    NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    DonGiaBenhVien = item.DonGiaBenhVien.Value,
                    DonGiaUuDai = item.DonGiaUuDai.Value,
                    DonGiaThucTe = item.DonGiaThucTe.Value,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai.Value,
                    GioiTinhNam = item.GioiTinhNam.GetValueOrDefault(),
                    GioiTinhNu = item.GioiTinhNu.GetValueOrDefault(),
                    CoMangThai = item.CoMangThai.GetValueOrDefault(),
                    KhongMangThai = item.KhongMangThai.GetValueOrDefault(),
                    DaLapGiaDinh = item.DaLapGiaDinh.GetValueOrDefault(),
                    ChuaLapGiaDinh = item.ChuaLapGiaDinh.GetValueOrDefault(),
                    SoTuoiTu = item.SoTuoiTu,
                    SoTuoiDen = item.SoTuoiDen,
                };
                foreach (var noiThucHien in item.GoiKhamSucKhoeNoiThucHiens)
                {
                    var goiKhamNoiThucHien = new GoiKhamSucKhoeNoiThucHien
                    {
                        PhongBenhVienId = noiThucHien.PhongBenhVienId
                    };
                    goiKhamKhamBenh.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamNoiThucHien);
                }
                goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Add(goiKhamKhamBenh);
            }

            foreach (var item in viewModel.DichVuKhamSucKhoeDoans.Where(p => !p.LaDichVuKham))
            {
                var goiKhamKyThuat = new GoiKhamSucKhoeDichVuDichVuKyThuat
                {
                    DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.Value,
                    NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    SoLan = item.SoLan.Value,
                    DonGiaBenhVien = item.DonGiaBenhVien.Value,
                    DonGiaUuDai = item.DonGiaUuDai.Value,
                    DonGiaThucTe = item.DonGiaThucTe.Value,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai.Value,
                    GioiTinhNam = item.GioiTinhNam.GetValueOrDefault(),
                    GioiTinhNu = item.GioiTinhNu.GetValueOrDefault(),
                    CoMangThai = item.CoMangThai.GetValueOrDefault(),
                    KhongMangThai = item.KhongMangThai.GetValueOrDefault(),
                    DaLapGiaDinh = item.DaLapGiaDinh.GetValueOrDefault(),
                    ChuaLapGiaDinh = item.ChuaLapGiaDinh.GetValueOrDefault(),
                    SoTuoiTu = item.SoTuoiTu,
                    SoTuoiDen = item.SoTuoiDen
                };
                foreach (var noiThucHien in item.GoiKhamSucKhoeNoiThucHiens)
                {
                    var goiKhamNoiThucHien = new GoiKhamSucKhoeNoiThucHien
                    {
                        PhongBenhVienId = noiThucHien.PhongBenhVienId
                    };
                    goiKhamKyThuat.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamNoiThucHien);
                }
                goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.Add(goiKhamKyThuat);
            }
            await _goiKhamSucKhoeService.AddAsync(goiKhamSucKhoe);
            return NoContent();
        }

        [HttpGet("GetGoiKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GoiKhamSucKhoeViewModel>> GetGoiKhamSucKhoeDoan(long id)
        {
            var entity = await _goiKhamSucKhoeService
                .GetByIdAsync(id, x => x.Include(a => a.HopDongKhamSucKhoe).ThenInclude(p => p.CongTyKhamSucKhoe)
                                        .Include(a => a.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens).ThenInclude(p => p.PhongBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens).ThenInclude(p => p.PhongBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.NhomDichVuBenhVien).ThenInclude(p => p.NhomDichVuBenhVienCha)
                                        .Include(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                                        .Include(a => a.YeuCauKhamBenhs)
                                        .Include(a => a.YeuCauDichVuKyThuats)
                                        );
            var viewModel = entity.ToModel<GoiKhamSucKhoeViewModel>();
            if (entity.YeuCauKhamBenhs.Any(z => z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham) || entity.YeuCauDichVuKyThuats.Any(z => z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            {
                viewModel.CoSuDungGoiChung = true;
            }
            viewModel.IsKetThucHopDong = entity.HopDongKhamSucKhoe.DaKetThuc;
            viewModel.DichVuKhamSucKhoeDoans.AddRange(viewModel.GoiKhamSucKhoeDichVuKhamBenhs);
            viewModel.DichVuKhamSucKhoeDoans.AddRange(viewModel.GoiKhamSucKhoeDichVuDichVuKyThuats);
            return Ok(viewModel);
        }

        [HttpPost("CapNhatGoiKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> CapNhatGoiKhamSucKhoeDoan(GoiKhamSucKhoeViewModel viewModel)
        {
            await _khamDoanService.CheckGoiKhamDaKetThucHoacDaXoa(viewModel.Id);
            var entity = await _goiKhamSucKhoeService
              .GetByIdAsync(viewModel.Id, x =>
                                        x.Include(a => a.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens)
                                         .Include(a => a.YeuCauKhamBenhs)
                                         .Include(a => a.YeuCauDichVuKyThuats)
                                         .Include(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens));
            //if (
            //      !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong)
            //   || !viewModel.DichVuKhamSucKhoeDoans.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat))
            //{
            //    throw new ApiException(_localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.Required"));
            //}

            if (!viewModel.DichVuKhamSucKhoeDoans.Any())
            {
                throw new ApiException("Vui lòng chọn ít nhất 1 dịch vụ trong gói khám");
            }

            if (viewModel.IsCopy != true && viewModel.GoiChung != entity.GoiChung && (entity.YeuCauKhamBenhs.Any(z => z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham) || entity.YeuCauDichVuKyThuats.Any(z => z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)))
            {
                throw new ApiException(_localizationService.GetResource("KhamDoan.GoiChung.DaSuDung"));
            }
            viewModel.GoiKhamSucKhoeDichVuKhamBenhs.AddRange(viewModel.DichVuKhamSucKhoeDoans.Where(p => p.LaDichVuKham));
            viewModel.GoiKhamSucKhoeDichVuDichVuKyThuats.AddRange(viewModel.DichVuKhamSucKhoeDoans.Where(p => !p.LaDichVuKham));
            viewModel.ToEntity(entity);
            await _goiKhamSucKhoeService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("KiemTraNhanVienSuDungGoiKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> KiemTraNhanVienSuDungGoiKham(long id)
        {
            var entity = await _goiKhamSucKhoeService.GetByIdAsync(id,
                x => x.Include(a => a.GoiKhamSucKhoeDichVuKhamBenhs)
                     .Include(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats)
                     .Include(c => c.HopDongKhamSucKhoeNhanViens).ThenInclude(p => p.YeuCauTiepNhans)
                     .Include(a => a.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens).ThenInclude(c => c.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.YeuCauTiepNhans)
                     .Include(a => a.GoiKhamSucKhoeChungDichVuKyThuatNhanViens).ThenInclude(c => c.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.YeuCauTiepNhans));

            var kiemTraGoiKhamNhanVien = entity.HopDongKhamSucKhoeNhanViens
                             .Any(c => c.YeuCauTiepNhans.Any(yc => yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien 
                                        || yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat));

            var kiemTraNhanVienDaKhamTrongGoiDVChung = 
                   entity.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens.Any(x=>x.HopDongKhamSucKhoeNhanVien.YeuCauTiepNhans
                   .Any(yc => yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien || yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat)) 
                || entity.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Any(x => x.HopDongKhamSucKhoeNhanVien.YeuCauTiepNhans
                   .Any(yc => yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien || yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat))
                || entity.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Any();

            if (kiemTraGoiKhamNhanVien || kiemTraNhanVienDaKhamTrongGoiDVChung)
            {
                return Ok((true, "Đã có nhân viên đang sử dụng gói khám này nên không xóa được."));
            }

            return Ok((false, string.Empty));
        }

        [HttpPost("XoaGoiKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _goiKhamSucKhoeService.GetByIdAsync(id,
                x => x.Include(a => a.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens)
                     .Include(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens)
                     .Include(a => a.HopDongKhamSucKhoe).ThenInclude(c => c.HopDongKhamSucKhoeNhanViens).ThenInclude(p => p.YeuCauTiepNhans)
                     .Include(a => a.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens).ThenInclude(c => c.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.YeuCauTiepNhans)
                     .Include(a => a.GoiKhamSucKhoeChungDichVuKyThuatNhanViens).ThenInclude(c => c.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.YeuCauTiepNhans));
            if (entity == null)
            {
                return NotFound();
            }

            var kiemTraGoiKhamNhanVien = entity.HopDongKhamSucKhoeNhanViens
                          .Any(c => c.YeuCauTiepNhans.Any(yc => yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                     || yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat));

            var kiemTraNhanVienDaKhamTrongGoiDVChung =
                   entity.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens.Any(x => x.HopDongKhamSucKhoeNhanVien.YeuCauTiepNhans
                 .Any(yc => yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien || yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat))
                || entity.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Any(x => x.HopDongKhamSucKhoeNhanVien.YeuCauTiepNhans
                   .Any(yc => yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien || yc.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat))
                || entity.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Any();

            if (kiemTraGoiKhamNhanVien || kiemTraNhanVienDaKhamTrongGoiDVChung)
            {
                throw new ApiException("Đã có nhân viên đang sử dụng gói khám này nên không xóa được.");
            }

            await _goiKhamSucKhoeService.DeleteAsync(entity);
            return NoContent();
        }

        #endregion

        #region Excel
        [HttpPost("ExportDanhSachKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> ExportDanhSachKhamSucKhoeDoan(QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachGoiKhamSucKhoe(queryInfo, true);
            var DanhSachKhamSucKhoeDoanData = gridData.Data.Select(p => (GoiKhamSucKhoeDoanVo)p).ToList();
            var excelData = DanhSachKhamSucKhoeDoanData.Map<List<GoiKhamSucKhoeDoanExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(GoiKhamSucKhoeDoanExportExcel.MaGoiKham), "Mã gói khám"),
                (nameof(GoiKhamSucKhoeDoanExportExcel.TenGoiKham), "Tên gói khám"),
                (nameof(GoiKhamSucKhoeDoanExportExcel.TenCongTy), "Tên công ty"),
                (nameof(GoiKhamSucKhoeDoanExportExcel.SoHopDong), "SHĐ"),
                (nameof(GoiKhamSucKhoeDoanExportExcel.NgayHieuLucDisplay), "Ngày hiệu lực"),
                (nameof(GoiKhamSucKhoeDoanExportExcel.NgayKetThucDisplay), "Ngày kết thúc")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Gói Khám Sức Khỏe", 2, "DS Gói Khám Sức Khỏe");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSKhamSucKhoeDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
