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
        [HttpPost("GetDataForGridAsyncDanhSachKhamChungSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachKhamChungSucKhoeDoan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachGoiKhamChungSucKhoe(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachKhamChungSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachKhamChungSucKhoeDoan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForGridAsyncDanhSachGoiKhamChungSucKhoe(queryInfo);
            return Ok(gridData);
        }

        #region CRUD

        [HttpGet("GetGoiKhamSucKhoeDoanChung")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GoiKhamSucKhoeChungViewModel>> GetGoiKhamSucKhoeDoanChung(long id)
        {
            var entity = await _goiKhamSucKhoeChungService
                .GetByIdAsync(id, x => x.Include(a => a.GoiKhamSucKhoeChungDichVuKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoeChungNoiThucHiens).ThenInclude(p => p.PhongBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeChungDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeChungDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeChungDichVuDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoeChungNoiThucHiens).ThenInclude(p => p.PhongBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeChungDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                                        .Include(a => a.GoiKhamSucKhoeChungDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.NhomDichVuBenhVien).ThenInclude(p => p.NhomDichVuBenhVienCha)
                                        .Include(a => a.GoiKhamSucKhoeChungDichVuDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien));

            var viewModel = entity.ToModel<GoiKhamSucKhoeChungViewModel>();

            viewModel.DichVuKhamSucKhoeDoans.AddRange(viewModel.GoiKhamSucKhoeChungDichVuKhamBenhs);
            viewModel.DichVuKhamSucKhoeDoans.AddRange(viewModel.GoiKhamSucKhoeChungDichVuDichVuKyThuats);
            return Ok(viewModel);
        }

        [HttpPost("ThemDichVuKhamDoanChung")]
        public async Task<ActionResult> ThemDichVuKhamDoanChung(GoiKhamDichVuKhamSucKhoeDoanChungViewModel viewModel)
        {
            viewModel.LoaiGia = await _khamDoanService.GetTenNhomGiaDichVuKhamBenhBenhVien(viewModel.NhomGiaDichVuKyThuatBenhVienId, viewModel.LaDichVuKham);
            foreach (var item in viewModel.GoiKhamSucKhoeChungNoiThucHiens)
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

        [HttpPost("CapNhatDichVuKhamDoanChung")]
        public async Task<ActionResult> CapNhatDichVuKhamDoanChung(GoiKhamDichVuKhamSucKhoeDoanChungViewModel viewModel)
        {
            viewModel.LoaiGia = await _khamDoanService.GetTenNhomGiaDichVuKhamBenhBenhVien(viewModel.NhomGiaDichVuKyThuatBenhVienId, viewModel.LaDichVuKham);
            foreach (var item in viewModel.GoiKhamSucKhoeChungNoiThucHiens)
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

        [HttpPost("ThemGoiKhamSucKhoeDoanChung")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> ThemGoiKhamSucKhoeDoanChung(GoiKhamSucKhoeChungViewModel viewModel)
        {
            if (!viewModel.DichVuKhamSucKhoeDoans.Any())
            {
                throw new ApiException("Vui lòng chọn ít nhất 1 dịch vụ trong gói khám");
            }

            var goiKhamSucKhoe = viewModel.ToEntity<GoiKhamSucKhoeChung>();
            foreach (var item in viewModel.DichVuKhamSucKhoeDoans.Where(p => p.LaDichVuKham))
            {
                var goiKhamKhamBenh = new GoiKhamSucKhoeChungDichVuKhamBenh
                {
                    DichVuKhamBenhBenhVienId = item.DichVuKyThuatBenhVienId.Value,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe.Value,
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
                foreach (var noiThucHien in item.GoiKhamSucKhoeChungNoiThucHiens)
                {
                    var goiKhamNoiThucHien = new GoiKhamSucKhoeChungNoiThucHien
                    {
                        PhongBenhVienId = noiThucHien.PhongBenhVienId
                    };
                    goiKhamKhamBenh.GoiKhamSucKhoeChungNoiThucHiens.Add(goiKhamNoiThucHien);
                }
                goiKhamSucKhoe.GoiKhamSucKhoeChungDichVuKhamBenhs.Add(goiKhamKhamBenh);
            }

            foreach (var item in viewModel.DichVuKhamSucKhoeDoans.Where(p => !p.LaDichVuKham))
            {
                var goiKhamKyThuat = new GoiKhamSucKhoeChungDichVuDichVuKyThuat
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
                foreach (var noiThucHien in item.GoiKhamSucKhoeChungNoiThucHiens)
                {
                    var goiKhamNoiThucHien = new GoiKhamSucKhoeChungNoiThucHien
                    {
                        PhongBenhVienId = noiThucHien.PhongBenhVienId
                    };
                    goiKhamKyThuat.GoiKhamSucKhoeChungNoiThucHiens.Add(goiKhamNoiThucHien);
                }
                goiKhamSucKhoe.GoiKhamSucKhoeChungDichVuDichVuKyThuats.Add(goiKhamKyThuat);
            }
            await _goiKhamSucKhoeChungService.AddAsync(goiKhamSucKhoe);
            return NoContent();
        }

        [HttpPost("CapNhatGoiKhamSucKhoeDoanChung")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> CapNhatGoiKhamSucKhoeDoanChung(GoiKhamSucKhoeChungViewModel viewModel)
        {
            if (!viewModel.DichVuKhamSucKhoeDoans.Any())
            {
                throw new ApiException("Vui lòng chọn ít nhất 1 dịch vụ trong gói khám");
            }
            var entity = await _goiKhamSucKhoeChungService
              .GetByIdAsync(viewModel.Id, x =>
                                        x.Include(a => a.GoiKhamSucKhoeChungDichVuKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoeChungNoiThucHiens)
                                         .Include(a => a.GoiKhamSucKhoeChungDichVuDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoeChungNoiThucHiens));

            viewModel.GoiKhamSucKhoeChungDichVuKhamBenhs.AddRange(viewModel.DichVuKhamSucKhoeDoans.Where(p => p.LaDichVuKham));
            viewModel.GoiKhamSucKhoeChungDichVuDichVuKyThuats.AddRange(viewModel.DichVuKhamSucKhoeDoans.Where(p => !p.LaDichVuKham));

            viewModel.ToEntity(entity);
            await _goiKhamSucKhoeChungService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpPost("XoaGoiKhamSucKhoeDoanChung")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> XoaGoiKhamSucKhoeDoanChung(long id)
        {
            var entity = await _goiKhamSucKhoeChungService.GetByIdAsync(id,
                x => x.Include(a => a.GoiKhamSucKhoeChungDichVuKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoeChungNoiThucHiens)
                      .Include(a => a.GoiKhamSucKhoeChungDichVuDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoeChungNoiThucHiens));

            if (entity == null)
            {
                return NotFound();
            }
            await _goiKhamSucKhoeChungService.DeleteAsync(entity);
            return NoContent();
        }

        #endregion

        #region Excel
        [HttpPost("ExportDanhSachKhamSucKhoeDoanChung")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanGoiKhamSucKhoe)]
        public async Task<ActionResult> ExportDanhSachKhamSucKhoeDoanChung(QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachGoiKhamChungSucKhoe(queryInfo, true);
            var DanhSachKhamSucKhoeDoanData = gridData.Data.Select(p => (GoiKhamSucKhoeChungDoanVo)p).ToList();
            var excelData = DanhSachKhamSucKhoeDoanData.Map<List<GoiKhamSucKhoeDoanChungExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(GoiKhamSucKhoeDoanChungExportExcel.MaGoiKham), "Mã gói khám"),
                (nameof(GoiKhamSucKhoeDoanChungExportExcel.TenGoiKham), "Tên gói khám")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Gói Khám Sức Khỏe", 2, "DS Gói Khám Sức Khỏe");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSKhamSucKhoeDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion
    }
}
