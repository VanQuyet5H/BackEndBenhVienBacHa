using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.GoiDichVu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuQuaTangs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.LoaiGoiDichVus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.ExportImport;
using Camino.Services.GoiDichVuMarketings;
using Camino.Services.GoiDichVus;
using Camino.Services.Helpers;
using Camino.Services.LoaiGoiDichVus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class GoiDichVuChuongTrinhMarketingController : CaminoBaseController
    {
        private readonly IChuongTrinhMarketingGoiDvService _goiDvChuongTrinhMarketingService;
        private readonly IGoiDvService _goiDvService;
        private readonly IGoiDvMarketingService _goiDvMarketingService;
        private readonly IExcelService _excelService;
        private readonly ILoaiGoiDichVuService _loaiGoiDichVuService;

        public GoiDichVuChuongTrinhMarketingController(IChuongTrinhMarketingGoiDvService goiDvChuongTrinhMarketingService, IExcelService excelService,
            ILoaiGoiDichVuService loaiGoiDichVuService,
            IGoiDvMarketingService goiDvMarketingService, IGoiDvService goiDvService)
        {
            _goiDvChuongTrinhMarketingService = goiDvChuongTrinhMarketingService;
            _excelService = excelService;
            _goiDvMarketingService = goiDvMarketingService;
            _goiDvService = goiDvService;
            _loaiGoiDichVuService = loaiGoiDichVuService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiDvChuongTrinhMarketingService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiDvChuongTrinhMarketingService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridYeuCauGoiDichVuAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridYeuCauGoiDichVuAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiDvChuongTrinhMarketingService.GetDataForGridYeuCauGoiDichVuAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridYeuCauGoiDichVuAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridYeuCauGoiDichVuAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiDvChuongTrinhMarketingService.GetTotalPageForGridYeuCauGoiDichVuAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLoaiGoiMarketingAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLoaiGoiMarketingAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiDvChuongTrinhMarketingService.GetDataForGridLoaiGoiMarketingAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLoaiGoiMarketingAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLoaiGoiMarketingAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiDvChuongTrinhMarketingService.GetTotalPageForGridLoaiGoiMarketingAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetGoiDv")]
        public async Task<ActionResult> GetGoiDv(DropDownListRequestModel model)
        {
            var result = await _goiDvChuongTrinhMarketingService.GetGoiDv(model);
            return Ok(result);
        }

        [HttpPost("GetListQuaTang")]
        public async Task<ActionResult> GetListQuaTang(DropDownListRequestModel model)
        {
            var result = await _goiDvChuongTrinhMarketingService.GetListQuaTang(model);
            return Ok(result);
        }

        [HttpGet("GetChiTietChuongTrinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult<ChuongTrinhGoiDvMarketingViewModel>> GetChiTietChuongTrinh(long id)
        {
            var chuongTrinhGoiDvMarketingEntity =
                await _goiDvChuongTrinhMarketingService.GetByIdAsync(id, w => w.Include(e => e.YeuCauGoiDichVus).ThenInclude(e => e.BenhNhan));
            if (chuongTrinhGoiDvMarketingEntity == null)
            {
                return NotFound();
            }

            var goiDvMarketingViewModel = chuongTrinhGoiDvMarketingEntity.ToModel<ChuongTrinhGoiDvMarketingViewModel>();

            if (chuongTrinhGoiDvMarketingEntity.YeuCauGoiDichVus.Any())
            {
                goiDvMarketingViewModel.YeuCauSuDungChuongTrinhs = new List<YeuCauSuDungChuongTrinhViewModel>();
                foreach (var yeuCauGoiDv in chuongTrinhGoiDvMarketingEntity.YeuCauGoiDichVus)
                {
                    var yeuCauGoiDvViewModel = new YeuCauSuDungChuongTrinhViewModel
                    {
                        MaBn = yeuCauGoiDv.BenhNhan.MaBN,
                        DiaChi = yeuCauGoiDv.BenhNhan.DiaChiDayDu,
                        NgayDangKy = yeuCauGoiDv.ThoiDiemChiDinh,
                        TenBn = yeuCauGoiDv.BenhNhan.HoTen,
                        Id = yeuCauGoiDv.Id
                    };
                    goiDvMarketingViewModel.YeuCauSuDungChuongTrinhs.Add(yeuCauGoiDvViewModel);
                }
            }

            return Ok(goiDvMarketingViewModel);
        }

        [HttpGet("GetDanhSachNhomDichVu")]
        public async Task<ActionResult> GetDanhSachNhomDichVu(long goiDvId)
        {
            var goiDvMarketing = await _goiDvMarketingService.GetByIdAsync(goiDvId, w => w.Include(e => e.GoiDichVuChiTietDichVuKhamBenhs)
                .ThenInclude(e => e.DichVuKhamBenhBenhVien).ThenInclude(e => e.DichVuKhamBenhBenhVienGiaBenhViens)
                .Include(e => e.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(e => e.NhomGiaDichVuKhamBenhBenhVien)
                .Include(e => e.GoiDichVuChiTietDichVuKyThuats).ThenInclude(e => e.DichVuKyThuatBenhVien).ThenInclude(e => e.DichVuKyThuatVuBenhVienGiaBenhViens)
                .Include(e => e.GoiDichVuChiTietDichVuKyThuats).ThenInclude(e => e.NhomGiaDichVuKyThuatBenhVien)
                .Include(e => e.GoiDichVuChiTietDichVuGiuongs).ThenInclude(e => e.DichVuGiuongBenhVien).ThenInclude(e => e.DichVuGiuongBenhVienGiaBenhViens)
                .Include(e => e.GoiDichVuChiTietDichVuGiuongs).ThenInclude(e => e.NhomGiaDichVuGiuongBenhVien));

            var nhomDichVus = goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs.Select(o =>
                new NhomDichVuViewModel
                {
                    Id = o.Id,
                    DvId = o.DichVuKhamBenhBenhVienId,
                    MaDv = o.DichVuKhamBenhBenhVien?.Ma,
                    TenDv = o.DichVuKhamBenhBenhVien?.Ten,
                    Nhom = Enums.EnumDichVuTongHop.KhamBenh,
                    LoaiGia = o.NhomGiaDichVuKhamBenhBenhVienId,
                    LoaiGiaDisplay = o.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                    SoLuong = o.SoLan,
                    DonGiaBenhVien = o.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens.Where(p =>
                                         p.NhomGiaDichVuKhamBenhBenhVienId == o.NhomGiaDichVuKhamBenhBenhVienId &&
                                         p.DichVuKhamBenhBenhVienId == o.DichVuKhamBenhBenhVienId &&
                                         p.TuNgay.Date <= DateTime.Now.Date &&
                                         (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date)).Select(p => p.Gia).LastOrDefault() ?? 0,
                    DonGiaTruocChietKhau = o.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens.Where(p =>
                                               p.NhomGiaDichVuKhamBenhBenhVienId == o.NhomGiaDichVuKhamBenhBenhVienId &&
                                               p.DichVuKhamBenhBenhVienId == o.DichVuKhamBenhBenhVienId &&
                                               p.TuNgay.Date <= DateTime.Now.Date &&
                                               (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date)).Select(p => p.Gia).LastOrDefault() ?? 0,
                    DonGiaSauChietKhau = o.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens.Where(p =>
                                             p.NhomGiaDichVuKhamBenhBenhVienId == o.NhomGiaDichVuKhamBenhBenhVienId &&
                                             p.DichVuKhamBenhBenhVienId == o.DichVuKhamBenhBenhVienId &&
                                             p.TuNgay.Date <= DateTime.Now.Date &&
                                             (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date)).Select(p => p.Gia).LastOrDefault() ?? 0
                }).Union(goiDvMarketing.GoiDichVuChiTietDichVuKyThuats.Select(o =>
                new NhomDichVuViewModel
                {
                    Id = o.Id,
                    DvId = o.DichVuKyThuatBenhVienId,
                    MaDv = o.DichVuKyThuatBenhVien?.Ma,
                    TenDv = o.DichVuKyThuatBenhVien?.Ten,
                    Nhom = Enums.EnumDichVuTongHop.KyThuat,
                    LoaiGia = o.NhomGiaDichVuKyThuatBenhVienId,
                    LoaiGiaDisplay = o.NhomGiaDichVuKyThuatBenhVien?.Ten,
                    SoLuong = o.SoLan,
                    DonGiaBenhVien = o.DichVuKyThuatBenhVien?.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p =>
                                             p.NhomGiaDichVuKyThuatBenhVienId == o.NhomGiaDichVuKyThuatBenhVienId &&
                                             p.DichVuKyThuatBenhVienId == o.DichVuKyThuatBenhVienId &&
                                             p.TuNgay.Date <= DateTime.Now.Date &&
                                             (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date))
                                         .Select(p => p.Gia).LastOrDefault() ?? 0,
                    DonGiaTruocChietKhau = o.DichVuKyThuatBenhVien?.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p =>
                                                   p.NhomGiaDichVuKyThuatBenhVienId ==
                                                   o.NhomGiaDichVuKyThuatBenhVienId &&
                                                   p.DichVuKyThuatBenhVienId == o.DichVuKyThuatBenhVienId &&
                                                   p.TuNgay.Date <= DateTime.Now.Date &&
                                                   (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date))
                                               .Select(p => p.Gia).LastOrDefault() ?? 0,
                    DonGiaSauChietKhau = o.DichVuKyThuatBenhVien?.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p =>
                                                 p.NhomGiaDichVuKyThuatBenhVienId == o.NhomGiaDichVuKyThuatBenhVienId &&
                                                 p.DichVuKyThuatBenhVienId == o.DichVuKyThuatBenhVienId &&
                                                 p.TuNgay.Date <= DateTime.Now.Date &&
                                                 (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date))
                                             .Select(p => p.Gia).LastOrDefault() ?? 0
                })).Union(goiDvMarketing.GoiDichVuChiTietDichVuGiuongs.Select(o =>
                new NhomDichVuViewModel
                {
                    Id = o.Id,
                    DvId = o.DichVuGiuongBenhVienId,
                    MaDv = o.DichVuGiuongBenhVien?.Ma,
                    TenDv = o.DichVuGiuongBenhVien?.Ten,
                    Nhom = Enums.EnumDichVuTongHop.GiuongBenh,
                    LoaiGia = o.NhomGiaDichVuGiuongBenhVienId,
                    LoaiGiaDisplay = o.NhomGiaDichVuGiuongBenhVien?.Ten,
                    SoLuong = o.SoLan,
                    DonGiaBenhVien = o.DichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens.Where(p =>
                                             p.NhomGiaDichVuGiuongBenhVienId == o.NhomGiaDichVuGiuongBenhVienId &&
                                             p.DichVuGiuongBenhVienId == o.DichVuGiuongBenhVienId &&
                                             p.TuNgay.Date <= DateTime.Now.Date &&
                                             (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date))
                                         .Select(p => p.Gia).LastOrDefault() ?? 0,
                    DonGiaTruocChietKhau = o.DichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens.Where(p =>
                                                   p.NhomGiaDichVuGiuongBenhVienId == o.NhomGiaDichVuGiuongBenhVienId &&
                                                   p.DichVuGiuongBenhVienId == o.DichVuGiuongBenhVienId &&
                                                   p.TuNgay.Date <= DateTime.Now.Date &&
                                                   (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date))
                                               .Select(p => p.Gia).LastOrDefault() ?? 0,
                    DonGiaSauChietKhau = o.DichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens.Where(p =>
                                                 p.NhomGiaDichVuGiuongBenhVienId == o.NhomGiaDichVuGiuongBenhVienId &&
                                                 p.DichVuGiuongBenhVienId == o.DichVuGiuongBenhVienId &&
                                                 p.TuNgay.Date <= DateTime.Now.Date &&
                                                 (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date))
                                             .Select(p => p.Gia).LastOrDefault() ?? 0
                })).ToList();
            return Ok(nhomDichVus);
        }

        [HttpPost("KichHoatGoiDichVuMarketing")]
        public async Task<ActionResult> KichHoatGoiDichVuMarketing(long id)
        {
            var entity = await _goiDvChuongTrinhMarketingService.GetByIdAsync(id);
            entity.TamNgung = entity.TamNgung == null ? true : !entity.TamNgung;
            await _goiDvChuongTrinhMarketingService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("LoaiGoiDichVus")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGoiDichVus([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _goiDvChuongTrinhMarketingService.GetLoaiGoiDichVus(queryInfo);
            return Ok(lookup);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult> Post
            ([FromBody]ChuongTrinhGoiDvMarketingViewModel chuongTrinhGoiDichVuMarketingViewModel)
        {
            //if (chuongTrinhGoiDichVuMarketingViewModel.LoaiGoiDichVuId != null)
            if (chuongTrinhGoiDichVuMarketingViewModel.LoaiGoiDichVuId == (long)EnumLoaiGoiDichVuMarketing.GoiSoSinh)
            //{
            {
                chuongTrinhGoiDichVuMarketingViewModel.GoiSoSinh = true;
            }
            else
            {
                chuongTrinhGoiDichVuMarketingViewModel.GoiSoSinh = null;
            }
            var goiDichVuMarketingEntity = chuongTrinhGoiDichVuMarketingViewModel.ToEntity<ChuongTrinhGoiDichVu>();

            if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o => o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
            {
                foreach (var goiDvKhamBenh in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
                {
                    var chuongTrinhDvKhamBenh = new ChuongTrinhGoiDichVuDichVuKhamBenh
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = goiDichVuMarketingEntity.Id,
                        DichVuKhamBenhBenhVienId = goiDvKhamBenh.DvId,
                        NhomGiaDichVuKhamBenhBenhVienId = goiDvKhamBenh.LoaiGia,
                        DonGia = goiDvKhamBenh.DonGiaBenhVien,
                        DonGiaSauChietKhau = goiDvKhamBenh.DonGiaSauChietKhau,
                        DonGiaTruocChietKhau = goiDvKhamBenh.DonGiaTruocChietKhau,
                        SoLan = goiDvKhamBenh.SoLuong
                    };
                    goiDichVuMarketingEntity.ChuongTrinhGoiDichVuDichKhamBenhs.Add(chuongTrinhDvKhamBenh);
                }
            }
            if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o => o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
            {
                foreach (var goiDvKyThuat in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
                {
                    var chuongTrinhDvKyThuat = new ChuongTrinhGoiDichVuDichVuKyThuat
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = goiDichVuMarketingEntity.Id,
                        DichVuKyThuatBenhVienId = goiDvKyThuat.DvId,
                        NhomGiaDichVuKyThuatBenhVienId = goiDvKyThuat.LoaiGia,
                        DonGia = goiDvKyThuat.DonGiaBenhVien,
                        DonGiaSauChietKhau = goiDvKyThuat.DonGiaSauChietKhau,
                        DonGiaTruocChietKhau = goiDvKyThuat.DonGiaTruocChietKhau,
                        SoLan = goiDvKyThuat.SoLuong
                    };
                    goiDichVuMarketingEntity.ChuongTrinhGoiDichVuDichVuKyThuats.Add(chuongTrinhDvKyThuat);
                }
            }
            if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o => o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
            {
                foreach (var goiDvGiuongBenh in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o => o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
                {
                    var chuongTrinhDvGiuongBenh = new ChuongTrinhGoiDichVuDichVuGiuong
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = goiDichVuMarketingEntity.Id,
                        DichVuGiuongBenhVienId = goiDvGiuongBenh.DvId,
                        NhomGiaDichVuGiuongBenhVienId = goiDvGiuongBenh.LoaiGia,
                        DonGia = goiDvGiuongBenh.DonGiaBenhVien,
                        DonGiaSauChietKhau = goiDvGiuongBenh.DonGiaSauChietKhau,
                        DonGiaTruocChietKhau = goiDvGiuongBenh.DonGiaTruocChietKhau,
                        SoLan = goiDvGiuongBenh.SoLuong
                    };
                    goiDichVuMarketingEntity.ChuongTrinhGoiDichVuDichVuGiuongs.Add(chuongTrinhDvGiuongBenh);
                }
            }
            if (chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Any(o => o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
            {
                foreach (var dv in chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
                {
                    var chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh = new ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = goiDichVuMarketingEntity.Id,
                        DichVuKhamBenhBenhVienId = dv.DvId,
                        NhomGiaDichVuKhamBenhBenhVienId = dv.LoaiGia,
                        DonGia = dv.DonGia,
                        DonGiaKhuyenMai = dv.DonGiaKhuyenMai,
                        SoLan = dv.SoLuong,
                        SoNgaySuDung = dv.SoNgaySuDung,
                        GhiChu = dv.GhiChu

                    };
                    goiDichVuMarketingEntity.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Add(chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh);
                }
            }
            if (chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Any(o => o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
            {
                foreach (var dv in chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
                {
                    var chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat = new ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = goiDichVuMarketingEntity.Id,
                        DichVuKyThuatBenhVienId = dv.DvId,
                        NhomGiaDichVuKyThuatBenhVienId = dv.LoaiGia,
                        DonGia = dv.DonGia,
                        DonGiaKhuyenMai = dv.DonGiaKhuyenMai,
                        SoLan = dv.SoLuong,
                        SoNgaySuDung = dv.SoNgaySuDung,
                        GhiChu = dv.GhiChu

                    };
                    goiDichVuMarketingEntity.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Add(chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat);
                }
            }
            if (chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Any(o => o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
            {
                foreach (var dv in chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
                {
                    var chuongTrinhGoiDichVuKhuyenMaiDichVuGiuong = new ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = goiDichVuMarketingEntity.Id,
                        DichVuGiuongBenhVienId = dv.DvId,
                        NhomGiaDichVuGiuongBenhVienId = dv.LoaiGia,
                        DonGia = dv.DonGia,
                        DonGiaKhuyenMai = dv.DonGiaKhuyenMai,
                        SoLan = dv.SoLuong,
                        SoNgaySuDung = dv.SoNgaySuDung,
                        GhiChu = dv.GhiChu

                    };
                    goiDichVuMarketingEntity.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Add(chuongTrinhGoiDichVuKhuyenMaiDichVuGiuong);
                }
            }
            await _goiDvChuongTrinhMarketingService.AddAsync(goiDichVuMarketingEntity);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ChuongTrinhGoiDvMarketingViewModel>> Get(long id)
        {
            var goiDvMarketing =
                await _goiDvChuongTrinhMarketingService.GetByIdAsync(id,
                    w => w.Include(e => e.ChuongTrinhGoiDichVuQuaTangs)
                        .ThenInclude(e => e.QuaTang).Include(e => e.GoiDichVu)
                        .Include(e => e.YeuCauGoiDichVus)
            .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(e => e.DichVuKhamBenhBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(e => e.NhomGiaDichVuKhamBenhBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs).ThenInclude(e => e.DichVuGiuongBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs).ThenInclude(e => e.NhomGiaDichVuGiuongBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(e => e.DichVuKyThuatBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(e => e.NhomGiaDichVuKyThuatBenhVien)
                        .Include(e => e.LoaiGoiDichVu)
                        .Include(e => e.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(e => e.DichVuKhamBenhBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(e => e.NhomGiaDichVuKhamBenhBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(e => e.DichVuGiuongBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(e => e.NhomGiaDichVuGiuongBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(e => e.DichVuKyThuatBenhVien)
                        .Include(e => e.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(e => e.NhomGiaDichVuKyThuatBenhVien));
            if (goiDvMarketing == null)
            {
                return NotFound();
            }

            var goiDvMarketingViewModel = goiDvMarketing.ToModel<ChuongTrinhGoiDvMarketingViewModel>();

            goiDvMarketingViewModel.CoYeuCauSuDung = goiDvMarketing.YeuCauGoiDichVus.Any();
            goiDvMarketingViewModel.GoiDvIdCu = goiDvMarketingViewModel.GoiDvId;
            goiDvMarketingViewModel.GoiDv = goiDvMarketing.GoiDichVu.Ten;
            goiDvMarketingViewModel.KhuyenMaiKems = (goiDvMarketingViewModel.KhuyenMaiKemsGiuong ?? new List<KhuyenMaiKemViewModel>())
                .Union((goiDvMarketingViewModel.KhuyenMaiKemsKhamBenh ?? new List<KhuyenMaiKemViewModel>()))
                .Union((goiDvMarketingViewModel.KhuyenMaiKemsKyThuat ?? new List<KhuyenMaiKemViewModel>())).ToList();
            goiDvMarketingViewModel.NhomDichVus = goiDvMarketing.ChuongTrinhGoiDichVuDichKhamBenhs.Select(o =>
                new NhomDichVuViewModel
                {
                    Id = o.Id,
                    DvId = o.DichVuKhamBenhBenhVienId,
                    MaDv = o.DichVuKhamBenhBenhVien?.Ma,
                    TenDv = o.DichVuKhamBenhBenhVien?.Ten,
                    Nhom = Enums.EnumDichVuTongHop.KhamBenh,
                    LoaiGia = o.NhomGiaDichVuKhamBenhBenhVienId,
                    LoaiGiaDisplay = o.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                    SoLuong = o.SoLan,
                    DonGiaBenhVien = o.DonGia,
                    DonGiaTruocChietKhau = o.DonGiaTruocChietKhau,
                    DonGiaSauChietKhau = o.DonGiaSauChietKhau
                }).Union(goiDvMarketing.ChuongTrinhGoiDichVuDichVuKyThuats.Select(o =>
                new NhomDichVuViewModel
                {
                    Id = o.Id,
                    DvId = o.DichVuKyThuatBenhVienId,
                    MaDv = o.DichVuKyThuatBenhVien?.Ma,
                    TenDv = o.DichVuKyThuatBenhVien?.Ten,
                    Nhom = Enums.EnumDichVuTongHop.KyThuat,
                    LoaiGia = o.NhomGiaDichVuKyThuatBenhVienId,
                    LoaiGiaDisplay = o.NhomGiaDichVuKyThuatBenhVien?.Ten,
                    SoLuong = o.SoLan,
                    DonGiaBenhVien = o.DonGia,
                    DonGiaTruocChietKhau = o.DonGiaTruocChietKhau,
                    DonGiaSauChietKhau = o.DonGiaSauChietKhau
                })).Union(goiDvMarketing.ChuongTrinhGoiDichVuDichVuGiuongs.Select(o =>
                new NhomDichVuViewModel
                {
                    Id = o.Id,
                    DvId = o.DichVuGiuongBenhVienId,
                    MaDv = o.DichVuGiuongBenhVien?.Ma,
                    TenDv = o.DichVuGiuongBenhVien?.Ten,
                    Nhom = Enums.EnumDichVuTongHop.GiuongBenh,
                    LoaiGia = o.NhomGiaDichVuGiuongBenhVienId,
                    LoaiGiaDisplay = o.NhomGiaDichVuGiuongBenhVien?.Ten,
                    SoLuong = o.SoLan,
                    DonGiaBenhVien = o.DonGia,
                    DonGiaTruocChietKhau = o.DonGiaTruocChietKhau,
                    DonGiaSauChietKhau = o.DonGiaSauChietKhau
                })).ToList();

            return Ok(goiDvMarketingViewModel);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Put([FromBody]ChuongTrinhGoiDvMarketingViewModel chuongTrinhGoiDichVuMarketingViewModel)
        {
            var chuongTrinhGoiDvMarketing = await _goiDvChuongTrinhMarketingService.GetByIdAsync(
                chuongTrinhGoiDichVuMarketingViewModel.Id,
                w => w.Include(e => e.ChuongTrinhGoiDichVuQuaTangs)
                .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                    .Include(e => e.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(e => e.ChuongTrinhGoiDichVuDichKhamBenhs)
                    .Include(e => e.ChuongTrinhGoiDichVuDichVuKyThuats)
                    .Include(e => e.ChuongTrinhGoiDichVuDichVuGiuongs));

            if (chuongTrinhGoiDichVuMarketingViewModel.LoaiGoiDichVuId == (long)EnumLoaiGoiDichVuMarketing.GoiSoSinh)
            {
                chuongTrinhGoiDichVuMarketingViewModel.GoiSoSinh = true;
            }
            else
            {
                chuongTrinhGoiDichVuMarketingViewModel.GoiSoSinh = null;
            }
            chuongTrinhGoiDvMarketing.Ten = chuongTrinhGoiDichVuMarketingViewModel.Ten;
            chuongTrinhGoiDvMarketing.LoaiGoiDichVuId = chuongTrinhGoiDichVuMarketingViewModel.LoaiGoiDichVuId;
            chuongTrinhGoiDvMarketing.Ma = chuongTrinhGoiDichVuMarketingViewModel.Ma;
            chuongTrinhGoiDvMarketing.GoiDichVuId = chuongTrinhGoiDichVuMarketingViewModel.GoiDvId;
            chuongTrinhGoiDvMarketing.TenGoiDichVu = chuongTrinhGoiDichVuMarketingViewModel.GoiDichVu;
            chuongTrinhGoiDvMarketing.MoTaGoiDichVu = chuongTrinhGoiDichVuMarketingViewModel.MoTaGoiDichVu;
            chuongTrinhGoiDvMarketing.GiaTruocChietKhau = chuongTrinhGoiDichVuMarketingViewModel.GiaTruocChietKhau;
            chuongTrinhGoiDvMarketing.GiaSauChietKhau = chuongTrinhGoiDichVuMarketingViewModel.GiaSauChietKhau.GetValueOrDefault();
            chuongTrinhGoiDvMarketing.TamNgung = chuongTrinhGoiDichVuMarketingViewModel.TamNgung;
            chuongTrinhGoiDvMarketing.TuNgay = chuongTrinhGoiDichVuMarketingViewModel.TuNgay;
            chuongTrinhGoiDvMarketing.DenNgay = chuongTrinhGoiDichVuMarketingViewModel.DenNgay;
            chuongTrinhGoiDvMarketing.GoiSoSinh = chuongTrinhGoiDichVuMarketingViewModel.GoiSoSinh;
            chuongTrinhGoiDvMarketing.CongTyBaoHiemTuNhanId = chuongTrinhGoiDichVuMarketingViewModel.CongTyBaoHiemTuNhanId;

            if (chuongTrinhGoiDichVuMarketingViewModel.GoiDvIdCu != chuongTrinhGoiDichVuMarketingViewModel.GoiDvId)
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichKhamBenhs.Any())
                {
                    foreach (var chuongTrinhKhamBenh in chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichKhamBenhs)
                    {
                        chuongTrinhKhamBenh.WillDelete = true;
                    }
                }

                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuKyThuats.Any())
                {
                    foreach (var chuongTrinhKyThuat in chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuKyThuats)
                    {
                        chuongTrinhKyThuat.WillDelete = true;
                    }
                }

                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
                {
                    foreach (var chuongTrinhGiuong in chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuGiuongs)
                    {
                        chuongTrinhGiuong.WillDelete = true;
                    }
                }

                if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o =>
                    o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
                {
                    foreach (var goiDvKhamBenh in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o =>
                        o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
                    {
                        var chuongTrinhDvKhamBenh = new ChuongTrinhGoiDichVuDichVuKhamBenh
                        {
                            Id = 0,
                            ChuongTrinhGoiDichVuId = chuongTrinhGoiDvMarketing.Id,
                            DichVuKhamBenhBenhVienId = goiDvKhamBenh.DvId,
                            NhomGiaDichVuKhamBenhBenhVienId = goiDvKhamBenh.LoaiGia,
                            DonGia = goiDvKhamBenh.DonGiaBenhVien,
                            DonGiaSauChietKhau = goiDvKhamBenh.DonGiaSauChietKhau,
                            DonGiaTruocChietKhau = goiDvKhamBenh.DonGiaTruocChietKhau,
                            SoLan = goiDvKhamBenh.SoLuong
                        };
                        chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichKhamBenhs.Add(chuongTrinhDvKhamBenh);
                    }
                }
                if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o =>
                    o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
                {
                    foreach (var goiDvKyThuat in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o =>
                        o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
                    {
                        var chuongTrinhDvKyThuat = new ChuongTrinhGoiDichVuDichVuKyThuat
                        {
                            Id = 0,
                            ChuongTrinhGoiDichVuId = chuongTrinhGoiDvMarketing.Id,
                            DichVuKyThuatBenhVienId = goiDvKyThuat.DvId,
                            NhomGiaDichVuKyThuatBenhVienId = goiDvKyThuat.LoaiGia,
                            DonGia = goiDvKyThuat.DonGiaBenhVien,
                            DonGiaSauChietKhau = goiDvKyThuat.DonGiaSauChietKhau,
                            DonGiaTruocChietKhau = goiDvKyThuat.DonGiaTruocChietKhau,
                            SoLan = goiDvKyThuat.SoLuong
                        };
                        chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuKyThuats.Add(chuongTrinhDvKyThuat);
                    }
                }
                if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o =>
                    o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
                {
                    foreach (var goiDvGiuongBenh in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o =>
                        o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
                    {
                        var chuongTrinhDvGiuongBenh = new ChuongTrinhGoiDichVuDichVuGiuong
                        {
                            Id = 0,
                            ChuongTrinhGoiDichVuId = chuongTrinhGoiDvMarketing.Id,
                            DichVuGiuongBenhVienId = goiDvGiuongBenh.DvId,
                            NhomGiaDichVuGiuongBenhVienId = goiDvGiuongBenh.LoaiGia,
                            DonGia = goiDvGiuongBenh.DonGiaBenhVien,
                            DonGiaSauChietKhau = goiDvGiuongBenh.DonGiaSauChietKhau,
                            DonGiaTruocChietKhau = goiDvGiuongBenh.DonGiaTruocChietKhau,
                            SoLan = goiDvGiuongBenh.SoLuong
                        };
                        chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuGiuongs.Add(chuongTrinhDvGiuongBenh);
                    }
                }

            }
            else
            {
                if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o =>
                   o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
                {
                    foreach (var goiDvKhamBenh in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o =>
                        o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
                    {
                        var item = chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(o =>
                            o.Id == goiDvKhamBenh.Id);
                        if (item != null)
                        {
                            item.DonGiaSauChietKhau = goiDvKhamBenh.DonGiaSauChietKhau;
                            item.DonGiaTruocChietKhau = goiDvKhamBenh.DonGiaTruocChietKhau;
                            item.SoLan = goiDvKhamBenh.SoLuong;

                        }
                    }
                }
                if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o =>
                    o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
                {
                    foreach (var goiDvKyThuat in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o =>
                        o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
                    {
                        var item = chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(o =>
                            o.Id == goiDvKyThuat.Id);
                        if (item != null)
                        {
                            item.DonGiaSauChietKhau = goiDvKyThuat.DonGiaSauChietKhau;
                            item.DonGiaTruocChietKhau = goiDvKyThuat.DonGiaTruocChietKhau;
                            item.SoLan = goiDvKyThuat.SoLuong;

                        }
                    }
                }
                if (chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Any(o =>
                    o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
                {
                    foreach (var goiDvGiuongBenh in chuongTrinhGoiDichVuMarketingViewModel.NhomDichVus.Where(o =>
                        o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
                    {
                        var item = chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(o =>
                            o.Id == goiDvGiuongBenh.Id);
                        if (item != null)
                        {
                            item.DonGiaSauChietKhau = goiDvGiuongBenh.DonGiaSauChietKhau;
                            item.DonGiaTruocChietKhau = goiDvGiuongBenh.DonGiaTruocChietKhau;
                            item.SoLan = goiDvGiuongBenh.SoLuong;

                        }
                    }
                }
            }

            foreach (var quaTangViewModel in chuongTrinhGoiDichVuMarketingViewModel.QuaTangKems)
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuQuaTangs.Any(e => e.Id == quaTangViewModel.IdSys && quaTangViewModel.IdSys != 0))
                {
                    foreach (var quaTangEntity in chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuQuaTangs.Where(e => e.Id == quaTangViewModel.IdSys && quaTangViewModel.IdSys != 0))
                    {
                        quaTangEntity.ChuongTrinhGoiDichVuId = quaTangViewModel.GoiDvChuongTrinhMarketingId;
                        quaTangEntity.QuaTangId = quaTangViewModel.QuaTangId;
                        quaTangEntity.SoLuong = Convert.ToInt32(quaTangViewModel.SoLuong);
                        quaTangEntity.GhiChu = quaTangViewModel.GhiChu;
                    }
                }
                else
                {
                    var quaTangEntity = new ChuongTrinhGoiDichVuQuaTang
                    {
                        QuaTangId = quaTangViewModel.QuaTangId,
                        SoLuong = Convert.ToInt32(quaTangViewModel.SoLuong),
                        GhiChu = quaTangViewModel.GhiChu,
                        Id = 0,
                        ChuongTrinhGoiDichVuId = quaTangViewModel.GoiDvChuongTrinhMarketingId
                    };
                    chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuQuaTangs.Add(quaTangEntity);
                }
            }

            var lstEntityQuaTangCanXoa = chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuQuaTangs
                .Select(e => e.Id)
                .Where(q => !chuongTrinhGoiDichVuMarketingViewModel.QuaTangKems
                                .Select(e => e.IdSys).Contains(q) && q != 0);

            foreach (var quaTangId in lstEntityQuaTangCanXoa)
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuQuaTangs.Any(e => e.Id == quaTangId))
                {
                    var entityQuaTangNeedToRemove =
                        chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuQuaTangs.First(c => c.Id == quaTangId);
                    entityQuaTangNeedToRemove.WillDelete = true;
                }
            }
            //DV khuyến mãi kèm theo
            //Khám bệnh
            foreach (var khuyenMaiViewModel in chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(e => e.Id == khuyenMaiViewModel.IdDatabase && khuyenMaiViewModel.IdDatabase != 0))
                {
                    foreach (var khuyenMaiEntity in chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Where(e => e.Id == khuyenMaiViewModel.IdDatabase && khuyenMaiViewModel.IdDatabase != 0))
                    {
                        khuyenMaiEntity.SoNgaySuDung = khuyenMaiViewModel.SoNgaySuDung;
                        khuyenMaiEntity.DonGiaKhuyenMai = khuyenMaiViewModel.DonGiaKhuyenMai;
                        khuyenMaiEntity.SoLan = Convert.ToInt32(khuyenMaiViewModel.SoLuong);
                        khuyenMaiEntity.GhiChu = khuyenMaiViewModel.GhiChu;
                    }
                }
                else
                {
                    var chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh = new ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = khuyenMaiViewModel.GoiDichVuId,
                        DichVuKhamBenhBenhVienId = khuyenMaiViewModel.DvId,
                        NhomGiaDichVuKhamBenhBenhVienId = khuyenMaiViewModel.LoaiGia,
                        DonGia = khuyenMaiViewModel.DonGia,
                        DonGiaKhuyenMai = khuyenMaiViewModel.DonGiaKhuyenMai,
                        SoLan = khuyenMaiViewModel.SoLuong,
                        SoNgaySuDung = khuyenMaiViewModel.SoNgaySuDung,
                        GhiChu = khuyenMaiViewModel.GhiChu

                    };
                    chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Add(chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh);
                }
            }

            var lstEntityKhuyenMaiKhamBenhCanXoa = chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                .Select(e => e.Id)
                .Where(q => !chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KhamBenh)
                                .Select(e => e.IdDatabase).Contains(q) && q != 0);

            foreach (var khuyenMaiId in lstEntityKhuyenMaiKhamBenhCanXoa)
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(e => e.Id == khuyenMaiId))
                {
                    var entityKhuyenMaiNeedToRemove =
                        chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.First(c => c.Id == khuyenMaiId);
                    entityKhuyenMaiNeedToRemove.WillDelete = true;
                }
            }

            //Giường
            foreach (var khuyenMaiViewModel in chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any(e => e.Id == khuyenMaiViewModel.IdDatabase && khuyenMaiViewModel.IdDatabase != 0))
                {
                    foreach (var khuyenMaiEntity in chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Where(e => e.Id == khuyenMaiViewModel.IdDatabase && khuyenMaiViewModel.IdDatabase != 0))
                    {
                        khuyenMaiEntity.SoNgaySuDung = khuyenMaiViewModel.SoNgaySuDung;
                        khuyenMaiEntity.DonGiaKhuyenMai = khuyenMaiViewModel.DonGiaKhuyenMai;
                        khuyenMaiEntity.SoLan = Convert.ToInt32(khuyenMaiViewModel.SoLuong);
                        khuyenMaiEntity.GhiChu = khuyenMaiViewModel.GhiChu;
                    }
                }
                else
                {
                    var chuongTrinhGoiDichVuKhuyenMaiDichVuGiuong = new ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = khuyenMaiViewModel.GoiDichVuId,
                        DichVuGiuongBenhVienId = khuyenMaiViewModel.DvId,
                        NhomGiaDichVuGiuongBenhVienId = khuyenMaiViewModel.LoaiGia,
                        DonGia = khuyenMaiViewModel.DonGia,
                        DonGiaKhuyenMai = khuyenMaiViewModel.DonGiaKhuyenMai,
                        SoLan = khuyenMaiViewModel.SoLuong,
                        SoNgaySuDung = khuyenMaiViewModel.SoNgaySuDung,
                        GhiChu = khuyenMaiViewModel.GhiChu

                    };
                    chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Add(chuongTrinhGoiDichVuKhuyenMaiDichVuGiuong);
                }
            }

            var lstEntityKhuyenMaiGiuongBenhCanXoa = chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs
                .Select(e => e.Id)
                .Where(q => !chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh)
                                .Select(e => e.IdDatabase).Contains(q) && q != 0);

            foreach (var khuyenMaiId in lstEntityKhuyenMaiGiuongBenhCanXoa)
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any(e => e.Id == khuyenMaiId))
                {
                    var entityKhuyenMaiNeedToRemove =
                        chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.First(c => c.Id == khuyenMaiId);
                    entityKhuyenMaiNeedToRemove.WillDelete = true;
                }
            }

            //KỸ thuật
            foreach (var khuyenMaiViewModel in chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(e => e.Id == khuyenMaiViewModel.IdDatabase && khuyenMaiViewModel.IdDatabase != 0))
                {
                    foreach (var khuyenMaiEntity in chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Where(e => e.Id == khuyenMaiViewModel.IdDatabase && khuyenMaiViewModel.IdDatabase != 0))
                    {
                        khuyenMaiEntity.SoNgaySuDung = khuyenMaiViewModel.SoNgaySuDung;
                        khuyenMaiEntity.DonGiaKhuyenMai = khuyenMaiViewModel.DonGiaKhuyenMai;
                        khuyenMaiEntity.SoLan = Convert.ToInt32(khuyenMaiViewModel.SoLuong);
                        khuyenMaiEntity.GhiChu = khuyenMaiViewModel.GhiChu;
                    }
                }
                else
                {
                    var chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat = new ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat
                    {
                        Id = 0,
                        ChuongTrinhGoiDichVuId = khuyenMaiViewModel.GoiDichVuId,
                        DichVuKyThuatBenhVienId = khuyenMaiViewModel.DvId,
                        NhomGiaDichVuKyThuatBenhVienId = khuyenMaiViewModel.LoaiGia,
                        DonGia = khuyenMaiViewModel.DonGia,
                        DonGiaKhuyenMai = khuyenMaiViewModel.DonGiaKhuyenMai,
                        SoLan = khuyenMaiViewModel.SoLuong,
                        SoNgaySuDung = khuyenMaiViewModel.SoNgaySuDung,
                        GhiChu = khuyenMaiViewModel.GhiChu

                    };
                    chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Add(chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat);
                }
            }

            var lstEntityKhuyenMaiKyThuatCanXoa = chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                .Select(e => e.Id)
                .Where(q => !chuongTrinhGoiDichVuMarketingViewModel.KhuyenMaiKems.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KyThuat)
                                .Select(e => e.IdDatabase).Contains(q) && q != 0);

            foreach (var khuyenMaiId in lstEntityKhuyenMaiKyThuatCanXoa)
            {
                if (chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(e => e.Id == khuyenMaiId))
                {
                    var entityKhuyenMaiNeedToRemove =
                        chuongTrinhGoiDvMarketing.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.First(c => c.Id == khuyenMaiId);
                    entityKhuyenMaiNeedToRemove.WillDelete = true;
                }
            }
            await _goiDvChuongTrinhMarketingService.UpdateAsync(chuongTrinhGoiDvMarketing);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult> Delete(long id)
        {
            var chuongTrinhGoiDvMarketing = await _goiDvChuongTrinhMarketingService.GetByIdAsync(id, e => e.Include(w => w.ChuongTrinhGoiDichVuQuaTangs)
                .Include(w => w.ChuongTrinhGoiDichVuDichKhamBenhs).Include(w => w.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(w => w.ChuongTrinhGoiDichVuDichVuGiuongs));
            if (chuongTrinhGoiDvMarketing == null)
            {
                return NotFound();
            }

            await _goiDvChuongTrinhMarketingService.DeleteByIdAsync(id);
            return NoContent();
        }


        #region Get/Add/Delete/Update ==>> LoaiGoi
        //Add
        [HttpPost("ThemLoaiGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult> ThemLoaiGoi(LoaiGoiDichVuModel loaiGoiDichVuModel)
        {
            var model = loaiGoiDichVuModel.ToEntity<LoaiGoiDichVu>();
            await _loaiGoiDichVuService.AddAsync(model);
            return Ok();
        }
        //Get
        [HttpGet("GetLoaiGoi")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LoaiGoiDichVuModel>> GetLoaiGoi(long id)
        {
            var model = await _loaiGoiDichVuService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel<LoaiGoiDichVuModel>());
        }
        //Update
        [HttpPost("CapNhatLoaiGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult> CapNhatLoaiGoi(LoaiGoiDichVuModel loaiGoiDichVuModel)
        {
            var model = await _loaiGoiDichVuService.GetByIdAsync(loaiGoiDichVuModel.Id);
            if (model == null)
            {
                return NotFound();
            }
            loaiGoiDichVuModel.ToEntity(model);
            await _loaiGoiDichVuService.UpdateAsync(model);
            return Ok();
        }
        //Delete
        [HttpPost("XoaLoaiGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult> XoaLoaiGoi(long id)
        {
            var model = await _loaiGoiDichVuService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await _loaiGoiDichVuService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportChuongTrinhGoiDichVuMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.GoiDvChuongTrinhMarketing)]
        public async Task<ActionResult> ExportChuongTrinhGoiDichVuMarketing(QueryInfo queryInfo)
        {
            var gridData = await _goiDvChuongTrinhMarketingService.GetDataForGridAsync(queryInfo, true);
            var goiDvMarketingData = gridData.Data.Select(p => (ChuongTrinhGoiDvMarketingGridVo)p).ToList();
            var excelData = goiDvMarketingData.Map<List<ChuongTrinhGoiDvMarketingExportExcel>>();
            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.Ma), "Mã CT"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.Ten), "Tên CT"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.TenDv), "Tên gói DV"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.GiaTruocChietKhau), "Giá trước CK"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.TiLeChietKhau), "Chiết khấu"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.GiaSauChietKhau), "Giá sau CK"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.TuNgay), "Thời gian bắt đầu"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.DenNgay), "Thời gian kết thúc"));
            lstValueObject.Add((nameof(ChuongTrinhGoiDvMarketingExportExcel.TamNgung), "Tình trạng"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Chương trình gói dịch vụ marketing", labelName: "Chương trình gói dịch vụ marketing");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ChuongTrinhGoiDichVuMarketing" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
