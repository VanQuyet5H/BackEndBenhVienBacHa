using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.DuocPhamBenhVien;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.DuocPhamBenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.Thuocs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class DuocPhamBenhVienController : CaminoBaseController
    {
        private readonly IDuocPhamBenhVienService _duocPhamBenhVienService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IDuocPhamService _duocPhamService;

        public DuocPhamBenhVienController(IDuocPhamBenhVienService duocPhamBenhVienService, ILocalizationService localizationService, IExcelService excelService, IDuocPhamService duocPhamService)
        {
            _duocPhamBenhVienService = duocPhamBenhVienService;
            _localizationService = localizationService;
            _excelService = excelService;
            _duocPhamService = duocPhamService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPhamBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duocPhamBenhVienService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPhamBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duocPhamBenhVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetListDuocPham")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListDuocPham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _duocPhamBenhVienService.GetListDuocPham(queryInfo);
            return Ok(lookup);
        }
        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDuocPhamBenhVien)]
        public async Task<ActionResult<DuocPhamBenhVienViewModel>> Post([FromBody] DuocPhamBenhVienViewModel duocPhamBenhVienViewModel)
        {

            if (!await _duocPhamService.CheckDuongDungAsync(duocPhamBenhVienViewModel.DuongDungId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DuongDung.NotExists"));
            if (!await _duocPhamService.CheckDVTAsync(duocPhamBenhVienViewModel.DonViTinhId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DVT.NotExists"));
            var entity = duocPhamBenhVienViewModel.ToEntity<DuocPhamBenhVien>();
            var duocPham = new DuocPham
            {
                Ten = duocPhamBenhVienViewModel.Ten,
                TenTiengAnh = duocPhamBenhVienViewModel.TenTiengAnh,
                SoDangKy = duocPhamBenhVienViewModel.SoDangKy,
                STTHoatChat = duocPhamBenhVienViewModel.STTHoatChat,
                MaHoatChat = duocPhamBenhVienViewModel.MaHoatChat,
                HoatChat = duocPhamBenhVienViewModel.HoatChat,
                LoaiThuocHoacHoatChat = LoaiThuocHoacHoatChat.ThuocTanDuoc,
                NhaSanXuat = duocPhamBenhVienViewModel.NhaSanXuat,
                NuocSanXuat = duocPhamBenhVienViewModel.NuocSanXuat,
                DuongDungId = duocPhamBenhVienViewModel.DuongDungId.GetValueOrDefault(),
                DonViTinhId = duocPhamBenhVienViewModel.DonViTinhId.GetValueOrDefault(),
                HamLuong = duocPhamBenhVienViewModel.HamLuong,
                QuyCach = duocPhamBenhVienViewModel.QuyCach,
                HeSoDinhMucDonViTinh = duocPhamBenhVienViewModel.HeSoDinhMucDonViTinh,
                TieuChuan = duocPhamBenhVienViewModel.TieuChuan,
                DangBaoChe = duocPhamBenhVienViewModel.DangBaoChe,
                MoTa = duocPhamBenhVienViewModel.MoTa,
                TheTich = duocPhamBenhVienViewModel.TheTich,
                HuongDan = duocPhamBenhVienViewModel.HuongDan,
                ChiDinh = duocPhamBenhVienViewModel.ChiDinh,
                ChongChiDinh = duocPhamBenhVienViewModel.ChongChiDinh,
                LieuLuongCachDung = duocPhamBenhVienViewModel.LieuLuongCachDung,
                TacDungPhu = duocPhamBenhVienViewModel.TacDungPhu,
                ChuYDePhong = duocPhamBenhVienViewModel.ChuYDePhong,
                LaThucPhamChucNang = duocPhamBenhVienViewModel.LaThucPhamChucNang,
                LaThuocHuongThanGayNghien = duocPhamBenhVienViewModel.LaThuocHuongThanGayNghien,
                MaATC = duocPhamBenhVienViewModel.MaATC,
                DuocPhamCoDau = duocPhamBenhVienViewModel.DuocPhamCoDau
            };

            entity.DuocPham = duocPham;

            if (duocPhamBenhVienViewModel.MayXetNghiemIds != null &&  duocPhamBenhVienViewModel.MayXetNghiemIds.Any())
            {
                foreach (var mayXetNghiemId in duocPhamBenhVienViewModel.MayXetNghiemIds)
                {
                    var duocPhamBenhVienMayXetNghiem = new DuocPhamBenhVienMayXetNghiem();
                    duocPhamBenhVienMayXetNghiem.DuocPhamBenhVienId = duocPhamBenhVienViewModel.Id;
                    duocPhamBenhVienMayXetNghiem.MayXetNghiemId = mayXetNghiemId;
                    entity.DuocPhamBenhVienMayXetNghiems.Add(duocPhamBenhVienMayXetNghiem);
                }
            }
            entity.LoaiDieuKienBaoQuanDuocPham = duocPhamBenhVienViewModel.LoaiDieuKienBaoQuanDuocPham;
            entity.ThongTinDieuKienBaoQuanDuocPham = duocPhamBenhVienViewModel.LoaiDieuKienBaoQuanDuocPham == LoaiDieuKienBaoQuanDuocPham.Khac ? duocPhamBenhVienViewModel.ThongTinDieuKienBaoQuanDuocPham : null;
            await _duocPhamBenhVienService.AddAsync(entity);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPhamBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuocPhamBenhVienViewModel>> Get(long id)
        {

            var result = await _duocPhamBenhVienService.GetByIdAsync(id, s => s.Include(k => k.DuocPham).ThenInclude(p => p.DonViTinh)
                                                                               .Include(k => k.DuocPham).ThenInclude(p => p.DuongDung)
                                                                               .Include(p => p.DuocPhamBenhVienPhanNhom)
                                                                               .Include(p => p.DuocPhamBenhVienMayXetNghiems));
            if (result == null)
            {
                return NotFound();
            }
            var duocPhamBenhVienPhanNhoms = await _duocPhamBenhVienService.DuocPhamBenhVienPhanNhoms();
            var resultData = result.ToModel<DuocPhamBenhVienViewModel>();

            if (result.DuocPhamBenhVienPhanNhomId != null)
            {
                var ladpbvNhomCon = await _duocPhamBenhVienService.LaDuocPhamBenhVienPhanNhomCon(result.DuocPhamBenhVienPhanNhomId.Value);
                if (ladpbvNhomCon)
                {
                    resultData.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(result.DuocPhamBenhVienPhanNhomId.Value, duocPhamBenhVienPhanNhoms);
                    resultData.TenDuocPhamBenhVienPhanNhomCha = await _duocPhamBenhVienService.GetTenDuocPhamBenhVienPhanNhom(resultData.DuocPhamBenhVienPhanNhomChaId.Value);
                    resultData.DuocPhamBenhVienPhanNhomConId = result.DuocPhamBenhVienPhanNhomId.Value;
                    resultData.TenDuocPhamBenhVienPhanNhomCon = await _duocPhamBenhVienService.GetTenDuocPhamBenhVienPhanNhom(resultData.DuocPhamBenhVienPhanNhomConId.Value);
                }
                else
                {
                    resultData.DuocPhamBenhVienPhanNhomChaId = result.DuocPhamBenhVienPhanNhomId.Value;
                    resultData.TenDuocPhamBenhVienPhanNhomCha = result.DuocPhamBenhVienPhanNhom?.Ten;
                    resultData.DuocPhamBenhVienPhanNhomConId = null;
                    resultData.TenDuocPhamBenhVienPhanNhomCon = null;
                }
            }

            resultData.MayXetNghiemIds = result.DuocPhamBenhVienMayXetNghiems.Any() ? result.DuocPhamBenhVienMayXetNghiems.Select(c => c.MayXetNghiemId).ToList() : null;
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDuocPhamBenhVien)]
        public async Task<ActionResult> Put([FromBody] DuocPhamBenhVienViewModel duocPhamBenhVienViewModel)
        {
            if (!await _duocPhamService.CheckDuongDungAsync(duocPhamBenhVienViewModel.DuongDungId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DuongDung.NotExists"));
            if (!await _duocPhamService.CheckDVTAsync(duocPhamBenhVienViewModel.DonViTinhId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DVT.NotExists"));
            var entity = await _duocPhamBenhVienService.GetByIdAsync(duocPhamBenhVienViewModel.Id, p => p.Include(s => s.DuocPham).Include(c=>c.DuocPhamBenhVienMayXetNghiems));
            if (entity == null)
            {
                return NotFound();
            }
            duocPhamBenhVienViewModel.ToEntity(entity);
            entity.DuocPham.Ten = duocPhamBenhVienViewModel.Ten;
            entity.DuocPham.TenTiengAnh = duocPhamBenhVienViewModel.TenTiengAnh;
            entity.DuocPham.SoDangKy = duocPhamBenhVienViewModel.SoDangKy;
            entity.DuocPham.STTHoatChat = duocPhamBenhVienViewModel.STTHoatChat;
            entity.DuocPham.MaHoatChat = duocPhamBenhVienViewModel.MaHoatChat;
            entity.DuocPham.HoatChat = duocPhamBenhVienViewModel.HoatChat;
            entity.DuocPham.NhaSanXuat = duocPhamBenhVienViewModel.NhaSanXuat;
            entity.DuocPham.NuocSanXuat = duocPhamBenhVienViewModel.NuocSanXuat;
            entity.DuocPham.DuongDungId = duocPhamBenhVienViewModel.DuongDungId.GetValueOrDefault();
            entity.DuocPham.DonViTinhId = duocPhamBenhVienViewModel.DonViTinhId.GetValueOrDefault();
            entity.DuocPham.HamLuong = duocPhamBenhVienViewModel.HamLuong;
            entity.DuocPham.QuyCach = duocPhamBenhVienViewModel.QuyCach;
            entity.DuocPham.HeSoDinhMucDonViTinh = duocPhamBenhVienViewModel.HeSoDinhMucDonViTinh;
            entity.DuocPham.TieuChuan = duocPhamBenhVienViewModel.TieuChuan;
            entity.DuocPham.DangBaoChe = duocPhamBenhVienViewModel.DangBaoChe;
            entity.DuocPham.MoTa = duocPhamBenhVienViewModel.MoTa;
            entity.DuocPham.TheTich = duocPhamBenhVienViewModel.TheTich;
            entity.DuocPham.HuongDan = duocPhamBenhVienViewModel.HuongDan;
            entity.DuocPham.ChiDinh = duocPhamBenhVienViewModel.ChiDinh;
            entity.DuocPham.ChongChiDinh = duocPhamBenhVienViewModel.ChongChiDinh;
            entity.DuocPham.LieuLuongCachDung = duocPhamBenhVienViewModel.LieuLuongCachDung;
            entity.DuocPham.TacDungPhu = duocPhamBenhVienViewModel.TacDungPhu;
            entity.DuocPham.ChuYDePhong = duocPhamBenhVienViewModel.ChuYDePhong;
            entity.DuocPham.LaThucPhamChucNang = duocPhamBenhVienViewModel.LaThucPhamChucNang;
            entity.DuocPham.LaThuocHuongThanGayNghien = duocPhamBenhVienViewModel.LaThuocHuongThanGayNghien;
            entity.DuocPham.MaATC = duocPhamBenhVienViewModel.MaATC;
            entity.DuocPham.DuocPhamCoDau = duocPhamBenhVienViewModel.DuocPhamCoDau;

            if (entity.DuocPhamBenhVienMayXetNghiems.Any())
            {
                var getAllDuocPhamCoMayXNs = entity.DuocPhamBenhVienMayXetNghiems.Where(c => c.DuocPhamBenhVienId == duocPhamBenhVienViewModel.Id);
                if (getAllDuocPhamCoMayXNs.Any())
                {
                    foreach (var item in getAllDuocPhamCoMayXNs)
                    {
                        item.WillDelete = true;
                    }
                }
            }

            if (duocPhamBenhVienViewModel.MayXetNghiemIds != null && duocPhamBenhVienViewModel.MayXetNghiemIds.Any())
            {
                foreach (var mayXetNghiemId in duocPhamBenhVienViewModel.MayXetNghiemIds)
                {
                    var duocPhamBenhVienMayXetNghiem = new DuocPhamBenhVienMayXetNghiem();
                    duocPhamBenhVienMayXetNghiem.DuocPhamBenhVienId = duocPhamBenhVienViewModel.Id;
                    duocPhamBenhVienMayXetNghiem.MayXetNghiemId = mayXetNghiemId;
                    entity.DuocPhamBenhVienMayXetNghiems.Add(duocPhamBenhVienMayXetNghiem);
                }
            }

            entity.LoaiDieuKienBaoQuanDuocPham = duocPhamBenhVienViewModel.LoaiDieuKienBaoQuanDuocPham;
            entity.ThongTinDieuKienBaoQuanDuocPham = duocPhamBenhVienViewModel.LoaiDieuKienBaoQuanDuocPham == LoaiDieuKienBaoQuanDuocPham.Khac ? duocPhamBenhVienViewModel.ThongTinDieuKienBaoQuanDuocPham : null;

            await _duocPhamBenhVienService.UpdateAsync(entity);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDuocPhamBenhVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var resultData = await _duocPhamBenhVienService.GetByIdAsync(id, s => s.Include(k => k.DuocPham));
            if (resultData == null)
            {
                return NotFound();
            }
            resultData.DuocPham.WillDelete = true;
            await _duocPhamBenhVienService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion

        #region   // export excel

        [HttpPost("ExportDuocPhamBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDuocPhamBenhVien)]
        public async Task<ActionResult> ExportExportDuocPhamBenhVien(QueryInfo queryInfo)
        {
            var gridData = await _duocPhamBenhVienService.GetDataForGridAsync(queryInfo, true);
            var lsBanThuoc = gridData.Data.Select(p => (DuocPhamBenhVienGridVo)p).ToList();
            var dataExcel = lsBanThuoc.Map<List<ExportDuocPhamBenhVienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(ExportDuocPhamBenhVienExportExcel.Ma), "Mã"),
                (nameof(ExportDuocPhamBenhVienExportExcel.Ten), "Tên"),
                (nameof(ExportDuocPhamBenhVienExportExcel.HamLuong), "Hàm Lượng"),
                (nameof(ExportDuocPhamBenhVienExportExcel.HoatChat), "Hoạt Chất"),
                (nameof(ExportDuocPhamBenhVienExportExcel.TenDonViTinh), "Đơn Vị Tính"),
                (nameof(ExportDuocPhamBenhVienExportExcel.TenDuongDung), "Đường Dùng"),
                (nameof(ExportDuocPhamBenhVienExportExcel.QuyCach), "Quy Cách"),
                (nameof(ExportDuocPhamBenhVienExportExcel.SoDangKy), "Số Đăng Ký"),
                (nameof(ExportDuocPhamBenhVienExportExcel.MaHoatChat), "Mã Hoạt Chất")
                //(nameof(ExportDuocPhamBenhVienExportExcel.TenLoaiThuocHoacHoatChat), "Tên Loại Thuốc Hoặc Hoạt Chất")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Dược Phẩm Bệnh Viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuocPhamBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #region update 5/2/2021
        [HttpPost("GetListDichVuBenhVienPhanNhomAsync")]
        public async Task<ActionResult<ICollection<NhomDichVuBenhVienPhanNhomTreeViewVo>>> GetListDichVuBenhVienPhanNhomAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _duocPhamBenhVienService.GetListDichVuBenhVienPhanNhomAsync(model);
            return Ok(lookup);
        }
        #endregion

        [HttpPost("DuocPhamBenhVienChaPhanNhoms")]
        public async Task<ActionResult> DuocPhamBenhVienChaPhanNhoms([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _duocPhamBenhVienService.DuocPhamBenhVienChaPhanNhoms(queryInfo);
            return Ok(result);
        }

        [HttpPost("DichVuBenhVienPhanNhomsLv2VaLv3")]
        public async Task<ActionResult<ICollection<NhomDichVuBenhVienPhanNhomTreeViewVo>>> DichVuBenhVienPhanNhomsLv2VaLv3([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _duocPhamBenhVienService.DichVuBenhVienPhanNhomsLv2VaLv3(model);
            return Ok(lookup);
        }

        [HttpPost("PhanLoaiThuocTheoQuanLys")]
        public async Task<ActionResult> PhanLoaiThuocTheoQuanLys([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _duocPhamBenhVienService.PhanLoaiThuocTheoQuanLys(model);
            return Ok(lookup);
        }

        [HttpPost("GetAllLoaiDieuKienBaoQuanDuocPham")]
        public async Task<ActionResult> GetAllLoaiDieuKienBaoQuanDuocPham([FromBody]DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiDieuKienBaoQuanDuocPham>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Ok(result);
        }


        [HttpPost("GetAllMayXetNghiem")]
        public ActionResult GetAllMayXetNghiem([FromBody]DropDownListRequestModel model)
        {
            return Ok(_duocPhamBenhVienService.GetAllMayXetNghiem(model));
        }

        #region //BVHD-3454
        [HttpPost("GetMaTaoMoiDuocPham")]
        public async Task<ActionResult<string>> GetMaTaoMoiDuocPhamAsync(MaDuocPhamTaoMoiInfoVo model)
        {
            var maDuocPham = await _duocPhamBenhVienService.GetMaTaoMoiDuocPhamAsync(model);
            return Ok(maDuocPham);
        }

        [HttpPost("KiemTraTrungDuocPhamBenhVien")]
        public async Task<ActionResult<bool>> KiemTraTrungDuocPhamBenhVienAsync([FromBody] DuocPhamBenhVienViewModel duocPhamBenhVienViewModel)
        {
            var duocPham = new DuocPham
            {
                Ten = duocPhamBenhVienViewModel.Ten,
                SoDangKy = duocPhamBenhVienViewModel.SoDangKy,
                MaHoatChat = duocPhamBenhVienViewModel.MaHoatChat,
                HoatChat = duocPhamBenhVienViewModel.HoatChat,
                NhaSanXuat = duocPhamBenhVienViewModel.NhaSanXuat,
                NuocSanXuat = duocPhamBenhVienViewModel.NuocSanXuat,
                DuongDungId = duocPhamBenhVienViewModel.DuongDungId.GetValueOrDefault(),
                DonViTinhId = duocPhamBenhVienViewModel.DonViTinhId.GetValueOrDefault(),
                HamLuong = duocPhamBenhVienViewModel.HamLuong,
                TheTich = duocPhamBenhVienViewModel.TheTich
            };
            var kiemtra = await _duocPhamBenhVienService.KiemTraTrungDuocPhamBenhVienAsync(duocPham);
            return Ok(kiemtra);
        }
        #endregion
    }
}