using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.GoiDichVu;
using Camino.Api.Models.NhaThau;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Core.Domain.ValueObject.NhomDichVuBenhVien;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhaThau;
using Camino.Services.NhomDichVuThuongDung;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class NhomDichVuThuongDungController : CaminoBaseController
    {
        private readonly INhomDichVuThuongDungService _nhomDichVuThuongDungService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public NhomDichVuThuongDungController(INhomDichVuThuongDungService nhomDichVuThuongDungService, IExcelService excelService, ILocalizationService localizationService)
        {
            _excelService = excelService;
            _nhomDichVuThuongDungService = nhomDichVuThuongDungService;
            _localizationService = localizationService;
        }

        #region GetDataForGrid

        [HttpPost("GetBoPhan")]
        public Task<List<LookupItemVo>> GetBoPhan()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.BoPhan>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDichVuNhomThuongDung)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhomDichVuThuongDungService.GetDataForGridAsync(queryInfo, true);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDichVuNhomThuongDung)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhomDichVuThuongDungService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.GoiDichVuNhomThuongDung)]
        public async Task<ActionResult> Post([FromBody]GoiDvMarketingViewModel goiDichVuViewModel)
        {
            var goiDichVu = goiDichVuViewModel.ToEntity<GoiDichVu>();
            await _nhomDichVuThuongDungService.AddAsync(goiDichVu);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GoiDichVuNhomThuongDung)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GoiDvMarketingViewModel>> Get(long id)
        {
            var goiDvMarketing = await _nhomDichVuThuongDungService.GetByIdAsync(id, w => w.Include(e => e.GoiDichVuChiTietDichVuKhamBenhs)
                .ThenInclude(e => e.DichVuKhamBenhBenhVien).ThenInclude(e => e.DichVuKhamBenhBenhVienGiaBenhViens)
                .Include(e => e.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(e => e.NhomGiaDichVuKhamBenhBenhVien)
                .Include(e => e.GoiDichVuChiTietDichVuKyThuats).ThenInclude(e => e.DichVuKyThuatBenhVien).ThenInclude(e => e.DichVuKyThuatVuBenhVienGiaBenhViens)
                .Include(e => e.GoiDichVuChiTietDichVuKyThuats).ThenInclude(e => e.NhomGiaDichVuKyThuatBenhVien)
                .Include(e => e.GoiDichVuChiTietDichVuGiuongs).ThenInclude(e => e.DichVuGiuongBenhVien).ThenInclude(e => e.DichVuGiuongBenhVienGiaBenhViens)
                .Include(e => e.GoiDichVuChiTietDichVuGiuongs).ThenInclude(e => e.NhomGiaDichVuGiuongBenhVien));
            if (goiDvMarketing == null)
            {
                return NotFound();
            }

            var goiDvMarketingViewModel = goiDvMarketing.ToModel<GoiDvMarketingViewModel>();

            goiDvMarketingViewModel.DvTrongGois = new List<DvTrongGoiViewModel>();
            foreach (var khamBenhEntity in goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs)
            {
                var khamBenhViewModel = new DvTrongGoiViewModel
                {
                    IdDatabase = khamBenhEntity.Id,
                    SoLuong = khamBenhEntity.SoLan,
                    Nhom = Enums.EnumDichVuTongHop.KhamBenh,
                    LoaiGia = khamBenhEntity.NhomGiaDichVuKhamBenhBenhVienId,
                    DvId = khamBenhEntity.DichVuKhamBenhBenhVienId,
                    GhiChu = khamBenhEntity.GhiChu,
                    GoiDichVuId = goiDvMarketing.Id,
                    MaDv = khamBenhEntity.DichVuKhamBenhBenhVien.Ma,
                    TenDv = khamBenhEntity.DichVuKhamBenhBenhVien.Ten,
                    LoaiGiaDisplay = khamBenhEntity.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    DonGia = khamBenhEntity.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.Where(p => p.NhomGiaDichVuKhamBenhBenhVienId == khamBenhEntity.NhomGiaDichVuKhamBenhBenhVienId &&
                                                                                                                         p.DichVuKhamBenhBenhVienId == khamBenhEntity.DichVuKhamBenhBenhVienId &&
                                                                                                                         p.TuNgay.Date <= DateTime.Now.Date &&
                                                                                                                         (p.DenNgay == null))
                        .Select(p => p.Gia).LastOrDefault()
                };
                goiDvMarketingViewModel.DvTrongGois.Add(khamBenhViewModel);
            }

            foreach (var kyThuatEntity in goiDvMarketing.GoiDichVuChiTietDichVuKyThuats)
            {
                var kyThuatViewModel = new DvTrongGoiViewModel
                {
                    IdDatabase = kyThuatEntity.Id,
                    SoLuong = kyThuatEntity.SoLan,
                    Nhom = Enums.EnumDichVuTongHop.KyThuat,
                    LoaiGia = kyThuatEntity.NhomGiaDichVuKyThuatBenhVienId,
                    DvId = kyThuatEntity.DichVuKyThuatBenhVienId,
                    GhiChu = kyThuatEntity.GhiChu,
                    GoiDichVuId = goiDvMarketing.Id,
                    MaDv = kyThuatEntity.DichVuKyThuatBenhVien.Ma,
                    TenDv = kyThuatEntity.DichVuKyThuatBenhVien.Ten,
                    LoaiGiaDisplay = kyThuatEntity.NhomGiaDichVuKyThuatBenhVien.Ten,
                    DonGia = kyThuatEntity.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p => p.NhomGiaDichVuKyThuatBenhVienId == kyThuatEntity.NhomGiaDichVuKyThuatBenhVienId &&
                                                                                                                p.DichVuKyThuatBenhVienId == kyThuatEntity.DichVuKyThuatBenhVienId &&
                                                                                                                p.TuNgay.Date <= DateTime.Now.Date)
                        .Select(p => p.Gia).LastOrDefault()
                };
                goiDvMarketingViewModel.DvTrongGois.Add(kyThuatViewModel);
            }

            foreach (var giuongBenhEntity in goiDvMarketing.GoiDichVuChiTietDichVuGiuongs)
            {
                var giuongBenhViewModel = new DvTrongGoiViewModel
                {
                    IdDatabase = giuongBenhEntity.Id,
                    SoLuong = giuongBenhEntity.SoLan,
                    Nhom = Enums.EnumDichVuTongHop.GiuongBenh,
                    LoaiGia = giuongBenhEntity.NhomGiaDichVuGiuongBenhVienId,
                    DvId = giuongBenhEntity.DichVuGiuongBenhVienId,
                    GhiChu = giuongBenhEntity.GhiChu,
                    GoiDichVuId = goiDvMarketing.Id,
                    MaDv = giuongBenhEntity.DichVuGiuongBenhVien.Ma,
                    TenDv = giuongBenhEntity.DichVuGiuongBenhVien.Ten,
                    LoaiGiaDisplay = giuongBenhEntity.NhomGiaDichVuGiuongBenhVien.Ten,
                    DonGia = giuongBenhEntity.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVienId == giuongBenhEntity.NhomGiaDichVuGiuongBenhVienId &&
                                                                                                                p.DichVuGiuongBenhVienId == giuongBenhEntity.DichVuGiuongBenhVienId &&
                                                                                                                p.TuNgay.Date <= DateTime.Now.Date)
                        .Select(p => p.Gia).LastOrDefault()
                };
                goiDvMarketingViewModel.DvTrongGois.Add(giuongBenhViewModel);
            }

            goiDvMarketingViewModel.BoPhanId = goiDvMarketing.BoPhanId;
            goiDvMarketingViewModel.TenBoPhan = goiDvMarketing.BoPhanId?.GetDescription();

            return Ok(goiDvMarketingViewModel);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.GoiDichVuNhomThuongDung)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Put([FromBody]GoiDvMarketingViewModel goiDvMarketingViewModel)
        {
            var goiDvMarketing = await _nhomDichVuThuongDungService.GetByIdAsync(goiDvMarketingViewModel.Id,
                w => w.Include(e => e.GoiDichVuChiTietDichVuKhamBenhs)
                    .Include(e => e.GoiDichVuChiTietDichVuKyThuats)
                    .Include(e => e.GoiDichVuChiTietDichVuGiuongs));

            goiDvMarketing.Ten = goiDvMarketingViewModel.TenGoiDv;
            goiDvMarketing.MoTa = goiDvMarketingViewModel.MoTa;
            goiDvMarketing.LoaiGoiDichVu = goiDvMarketingViewModel.LoaiGoiDichVu;
            goiDvMarketing.IsDisabled = goiDvMarketingViewModel.IsDisabled;
            goiDvMarketing.BoPhanId = goiDvMarketingViewModel.BoPhanId;

            foreach (var dvTrongGoiViewModel in goiDvMarketingViewModel.DvTrongGois.Where(e => e.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
            {
                if (goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs.Any(e => e.Id == dvTrongGoiViewModel.IdDatabase && dvTrongGoiViewModel.IdDatabase != 0))
                {
                    foreach (var dvTrongGoiMarketingEntity in goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs.Where(e => e.Id == dvTrongGoiViewModel.IdDatabase && dvTrongGoiViewModel.IdDatabase != 0))
                    {
                        dvTrongGoiMarketingEntity.DichVuKhamBenhBenhVienId = dvTrongGoiViewModel.DvId;
                        dvTrongGoiMarketingEntity.NhomGiaDichVuKhamBenhBenhVienId = dvTrongGoiViewModel.LoaiGia;
                        dvTrongGoiMarketingEntity.SoLan = dvTrongGoiViewModel.SoLuong;
                        dvTrongGoiMarketingEntity.GhiChu = dvTrongGoiViewModel.GhiChu;
                    }
                }
                else
                {
                    var dvTrongGoiMarketingEntity = new GoiDichVuChiTietDichVuKhamBenh
                    {
                        DichVuKhamBenhBenhVienId = dvTrongGoiViewModel.DvId,
                        NhomGiaDichVuKhamBenhBenhVienId = dvTrongGoiViewModel.LoaiGia,
                        SoLan = dvTrongGoiViewModel.SoLuong,
                        GhiChu = dvTrongGoiViewModel.GhiChu,
                        Id = 0,
                        GoiDichVuId = goiDvMarketingViewModel.Id
                    };
                    goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs.Add(dvTrongGoiMarketingEntity);
                }
            }

            foreach (var dvTrongGoiViewModel in goiDvMarketingViewModel.DvTrongGois.Where(e => e.Nhom == Enums.EnumDichVuTongHop.KyThuat))
            {
                if (goiDvMarketing.GoiDichVuChiTietDichVuKyThuats.Any(e => e.Id == dvTrongGoiViewModel.IdDatabase && dvTrongGoiViewModel.IdDatabase != 0))
                {
                    foreach (var dvTrongGoiMarketingEntity in goiDvMarketing.GoiDichVuChiTietDichVuKyThuats.Where(e => e.Id == dvTrongGoiViewModel.IdDatabase && dvTrongGoiViewModel.IdDatabase != 0))
                    {
                        dvTrongGoiMarketingEntity.DichVuKyThuatBenhVienId = dvTrongGoiViewModel.DvId;
                        dvTrongGoiMarketingEntity.NhomGiaDichVuKyThuatBenhVienId = dvTrongGoiViewModel.LoaiGia;
                        dvTrongGoiMarketingEntity.SoLan = dvTrongGoiViewModel.SoLuong;
                        dvTrongGoiMarketingEntity.GhiChu = dvTrongGoiViewModel.GhiChu;
                    }
                }
                else
                {
                    var dvTrongGoiMarketingEntity = new GoiDichVuChiTietDichVuKyThuat
                    {
                        DichVuKyThuatBenhVienId = dvTrongGoiViewModel.DvId,
                        NhomGiaDichVuKyThuatBenhVienId = dvTrongGoiViewModel.LoaiGia,
                        SoLan = dvTrongGoiViewModel.SoLuong,
                        GhiChu = dvTrongGoiViewModel.GhiChu,
                        Id = 0,
                        GoiDichVuId = goiDvMarketingViewModel.Id
                    };
                    goiDvMarketing.GoiDichVuChiTietDichVuKyThuats.Add(dvTrongGoiMarketingEntity);
                }
            }

            foreach (var dvTrongGoiViewModel in goiDvMarketingViewModel.DvTrongGois.Where(e => e.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
            {
                if (goiDvMarketing.GoiDichVuChiTietDichVuGiuongs.Any(e => e.Id == dvTrongGoiViewModel.IdDatabase && dvTrongGoiViewModel.IdDatabase != 0))
                {
                    foreach (var dvTrongGoiMarketingEntity in goiDvMarketing.GoiDichVuChiTietDichVuGiuongs.Where(e => e.Id == dvTrongGoiViewModel.IdDatabase && dvTrongGoiViewModel.IdDatabase != 0))
                    {
                        dvTrongGoiMarketingEntity.DichVuGiuongBenhVienId = dvTrongGoiViewModel.DvId;
                        dvTrongGoiMarketingEntity.NhomGiaDichVuGiuongBenhVienId = dvTrongGoiViewModel.LoaiGia;
                        dvTrongGoiMarketingEntity.SoLan = dvTrongGoiViewModel.SoLuong;
                        dvTrongGoiMarketingEntity.GhiChu = dvTrongGoiViewModel.GhiChu;
                    }
                }
                else
                {
                    var dvTrongGoiMarketingEntity = new GoiDichVuChiTietDichVuGiuong
                    {
                        DichVuGiuongBenhVienId = dvTrongGoiViewModel.DvId,
                        NhomGiaDichVuGiuongBenhVienId = dvTrongGoiViewModel.LoaiGia,
                        SoLan = dvTrongGoiViewModel.SoLuong,
                        GhiChu = dvTrongGoiViewModel.GhiChu,
                        Id = 0,
                        GoiDichVuId = goiDvMarketingViewModel.Id
                    };
                    goiDvMarketing.GoiDichVuChiTietDichVuGiuongs.Add(dvTrongGoiMarketingEntity);
                }
            }

            var lstEntityCanXoaDvKhamBenh = goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs
                .Select(e => e.Id)
                .Where(q => !goiDvMarketingViewModel.DvTrongGois.Where(e => e.Nhom == Enums.EnumDichVuTongHop.KhamBenh)
                                .Select(e => e.IdDatabase).Contains(q) && q != 0);

            foreach (var entityXoaDvKhamBenhId in lstEntityCanXoaDvKhamBenh)
            {
                if (goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs.Any(e => e.Id == entityXoaDvKhamBenhId))
                {
                    var entityKhamBenhNeedToRemove =
                        goiDvMarketing.GoiDichVuChiTietDichVuKhamBenhs.First(c => c.Id == entityXoaDvKhamBenhId);
                    entityKhamBenhNeedToRemove.WillDelete = true;
                }
            }

            var lstEntityCanXoaDvKyThuat = goiDvMarketing.GoiDichVuChiTietDichVuKyThuats
                .Select(e => e.Id)
                .Where(q => !goiDvMarketingViewModel.DvTrongGois.Where(e => e.Nhom == Enums.EnumDichVuTongHop.KyThuat)
                                .Select(e => e.IdDatabase).Contains(q) && q != 0);

            foreach (var entityXoaDvKyThuatId in lstEntityCanXoaDvKyThuat)
            {
                if (goiDvMarketing.GoiDichVuChiTietDichVuKyThuats.Any(e => e.Id == entityXoaDvKyThuatId))
                {
                    var entityKyThuatNeedToRemove =
                        goiDvMarketing.GoiDichVuChiTietDichVuKyThuats.First(c => c.Id == entityXoaDvKyThuatId);
                    entityKyThuatNeedToRemove.WillDelete = true;
                }
            }

            var lstEntityCanXoaDvGiuongBenh = goiDvMarketing.GoiDichVuChiTietDichVuGiuongs
                .Select(e => e.Id)
                .Where(q => !goiDvMarketingViewModel.DvTrongGois.Where(e => e.Nhom == Enums.EnumDichVuTongHop.GiuongBenh)
                                .Select(e => e.IdDatabase).Contains(q) && q != 0);

            foreach (var entityXoaDvGiuongBenhId in lstEntityCanXoaDvGiuongBenh)
            {
                if (goiDvMarketing.GoiDichVuChiTietDichVuGiuongs.Any(e => e.Id == entityXoaDvGiuongBenhId))
                {
                    var entityGiuongBenhNeedToRemove =
                        goiDvMarketing.GoiDichVuChiTietDichVuGiuongs.First(c => c.Id == entityXoaDvGiuongBenhId);
                    entityGiuongBenhNeedToRemove.WillDelete = true;
                }
            }

            await _nhomDichVuThuongDungService.UpdateAsync(goiDvMarketing);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.GoiDichVuNhomThuongDung)]
        public async Task<ActionResult> Delete(long id)
        {
            var goiDvMarketing = await _nhomDichVuThuongDungService.GetByIdAsync(id, e => e.Include(w => w.GoiDichVuChiTietDichVuGiuongs)
                .Include(w => w.GoiDichVuChiTietDichVuKhamBenhs)
                .Include(w => w.GoiDichVuChiTietDichVuKyThuats));
            if (goiDvMarketing == null)
            {
                return NotFound();
            }

            await _nhomDichVuThuongDungService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        public async Task<ActionResult> Deletes([FromBody]  DeletesViewModel DeletesViewModel)
        {
            var nhomICDTheoBenhVien = await _nhomDichVuThuongDungService.GetByIdsAsync(DeletesViewModel.Ids,
                e => e.Include(w => w.GoiDichVuChiTietDichVuGiuongs)
                .Include(w => w.GoiDichVuChiTietDichVuKhamBenhs)
                .Include(w => w.GoiDichVuChiTietDichVuKyThuats));

            if (nhomICDTheoBenhVien == null)
            {
                return NotFound();
            }
            await _nhomDichVuThuongDungService.DeleteAsync(nhomICDTheoBenhVien);
            return NoContent();
        }


        [HttpPost("ExportGoiDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.GoiDichVuNhomThuongDung)]
        public async Task<ActionResult> ExportGoiDichVu(QueryInfo queryInfo)
        {
            var gridData = await _nhomDichVuThuongDungService.GetDataForGridAsync(queryInfo, true);
            var goiDvMarketingData = gridData.Data.Select(p => (NhomDichVuThuongDungGridVo)p).ToList();
            var excelData = goiDvMarketingData.Map<List<NhomDichVuExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NhomDichVuExportExcel.TenNhom), "Tên nhóm"));
            lstValueObject.Add((nameof(NhomDichVuExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(NhomDichVuExportExcel.SuDung), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nhóm dịch vụ thường dùng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhomDichVuThuongDung" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("KichHoatNhomDichVu")]
        public async Task<ActionResult> KichHoatNhomDichVu(long id)
        {
            var entity = await _nhomDichVuThuongDungService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _nhomDichVuThuongDungService.UpdateAsync(entity);
            return NoContent();
        }
        #endregion        
    }
}