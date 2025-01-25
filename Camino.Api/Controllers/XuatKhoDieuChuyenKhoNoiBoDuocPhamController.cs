using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Localization;
using Camino.Services.ExportImport;
using Camino.Services.XuatKhoDieuChuyenKhoNoiBoDuocPhams;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams;
using System.Collections.Generic;
using Camino.Api.Models.YeuCauDieuChuyenKhoThuoc;
using System.Linq;
using Camino.Api.Models.Error;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Camino.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Camino.Services.Helpers;
using System;

namespace Camino.Api.Controllers
{
    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamController : CaminoBaseController
    {
        private readonly IXuatKhoDieuChuyenKhoNoiBoDuocPhamService _xuatKhoDieuChuyenKhoNoiBoDuocPhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        public XuatKhoDieuChuyenKhoNoiBoDuocPhamController(IXuatKhoDieuChuyenKhoNoiBoDuocPhamService xuatKhoDieuChuyenKhoNoiBoDuocPhamService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _xuatKhoDieuChuyenKhoNoiBoDuocPhamService = xuatKhoDieuChuyenKhoNoiBoDuocPhamService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        #region DANH SÁCH ĐIỀU CHUYỂN THUỐC

        #region DS yêu cầu / duyệt điều chuyển
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham, DocumentType.DanhSachDuyetDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham, DocumentType.DanhSachDuyetDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS yêu cầu/ duyệt điều chuyển chi tiết
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham, DocumentType.DanhSachDuyetDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham, DocumentType.DanhSachDuyetDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        [HttpPost("GetDataForGridAsyncDuocPhamDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDuocPhamDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridAsyncDuocPhamDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDuocPhamDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDuocPhamDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetTotalPageForGridAsyncDuocPhamDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncDaTaoYeuCau")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDaTaoYeuCau([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridAsyncDaTaoYeuCau(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDaTaoYeuCau")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDaTaoYeuCau([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetTotalPageForGridAsyncDaTaoYeuCau(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoTongCap2VaNhaThuocs")]
        public async Task<ActionResult> GetKhoTongCap2VaNhaThuocs([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetKhoTongCap2VaNhaThuocs(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetNguoiNhap")]
        public async Task<ActionResult> GetNguoiNhap([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetNguoiNhap(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetTrangThaiYeuCauDieuChuyen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiYeuCauDieuChuyen(long yeuCauDieuChuyenDuocPhamId)
        {
            var result = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetTrangThaiPhieuLinh(yeuCauDieuChuyenDuocPhamId);
            return Ok(result);
        }

        [HttpPost("DieuChuyenDuocPhamTheoNhom")]
        public async Task<ActionResult> DieuChuyenDuocPhamTheoNhom(YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVos model)
        {
            return Ok(model);
        }

        #region CRUD

        //GET
        [HttpGet("GetYeuCauDieuChuyenThuoc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult<XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel>> Get(long id)
        {
            var yeuCauDieuChuyenThuoc = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham)
                             .Include(r => r.KhoNhap)
                             .Include(r => r.KhoXuat)
                             .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                             .Include(r => r.NguoiNhap).ThenInclude(r => r.User)
                             .Include(r => r.NguoiXuat).ThenInclude(r => r.User)
                             )
                             ;

            if (yeuCauDieuChuyenThuoc == null)
            {
                return NotFound();
            }
            var model = yeuCauDieuChuyenThuoc.ToModel<XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel>();
            return Ok(model);
        }


        [HttpPost("ThemYeuCauDieuChuyenThuoc")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult> ThemYeuCauDieuChuyenThuoc(XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel dieuChuyenThuocVM)
        {
            if (!dieuChuyenThuocVM.YeuCauDieuChuyenDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("DieuChuyenNoiBoDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Required"));
            }
            if (dieuChuyenThuocVM.YeuCauDieuChuyenDuocPhamChiTiets.All(z => z.SoLuongDieuChuyen == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var dieuChuyenThuoc = dieuChuyenThuocVM.ToEntity<YeuCauDieuChuyenDuocPham>();
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.XuLyThemDieuChuyenThuocAsync(dieuChuyenThuoc, dieuChuyenThuocVM.YeuCauDieuChuyenDuocPhamChiTiets);
            return Ok(dieuChuyenThuoc.Id);
        }



        [HttpPost("CapNhatYeuCauDieuChuyenThuoc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult> CapNhatYeuCauDieuChuyenThuoc(XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel dieuChuyenThuocVM)
        {
            if (!dieuChuyenThuocVM.YeuCauDieuChuyenDuocPhamChiTiets.Any() || dieuChuyenThuocVM.YeuCauDieuChuyenDuocPhamChiTiets.All(z => z.WillDelete == true))
            {
                throw new ApiException(_localizationService.GetResource("DieuChuyenNoiBoDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Required"));
            }
            if (dieuChuyenThuocVM.YeuCauDieuChuyenDuocPhamChiTiets.All(z => z.SoLuongDieuChuyen == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.CheckPhieuYeuCauDieuChuyenDaDuyetHoacDaHuy(dieuChuyenThuocVM.Id);
            var dieuChuyenThuoc = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService
                .GetByIdAsync(dieuChuyenThuocVM.Id, s =>
                            s.Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham)
                             .Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.NhapKhoDuocPhamChiTiet)
                             .Include(r => r.KhoNhap)
                             .Include(r => r.KhoXuat)
                             .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                             .Include(r => r.NguoiNhap).ThenInclude(r => r.User)
                             .Include(r => r.NguoiXuat).ThenInclude(r => r.User)
                             )
                             ;

            if (dieuChuyenThuoc == null)
            {
                return NotFound();
            }
            dieuChuyenThuocVM.ToEntity(dieuChuyenThuoc);
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.XuLyCapNhatDieuChuyenThuocAsync(dieuChuyenThuoc, dieuChuyenThuocVM.YeuCauDieuChuyenDuocPhamChiTiets);
            var result = new
            {
                dieuChuyenThuoc.Id,
                dieuChuyenThuoc.LastModified
            };
            return Ok(result);
        }

        [HttpPost("XoaYeuCauDieuChuyenThuoc")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult> Delete(long id)
        {
            var dieuChuyenThuoc = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet)
                             .Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.NhapKhoDuocPhamChiTiet));

            if (dieuChuyenThuoc == null)
            {
                throw new ApiException(_localizationService.GetResource("YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists"));
            }
            if (dieuChuyenThuoc.DuocKeToanDuyet != null)
            {
                throw new ApiException(_localizationService.GetResource("YeuCauDieuChuyenDuocPham.PhieuYeuCau.DaDuyet"));
            }
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.XuLyXoaDieuChuyenThuocAsync(dieuChuyenThuoc, false);
            return NoContent();
        }
        #endregion

        #region Excel

        [HttpPost("ExportYeuCauDieuChuyenThuoc")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult> ExportYeuCauDieuChuyenThuoc(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (YeuCauDieuChuyenDuocPhamVo)p).ToList();
            var dataExcel = data.Map<List<YeuCauDieuChuyenThuocExportExcel>>();

            foreach (var item in dataExcel)
            {
                var queryThuoc = new QueryInfo()
                {
                    Skip = queryInfo.Skip,
                    Take = queryInfo.Take,
                    AdditionalSearchString = item.Id.ToString()
                };
                var gridChildData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridChildAsync(queryThuoc, true);
                var dataChild = gridChildData.Data.Select(p => (YeuCauDieuChuyenDuocPhamChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<YeuCauDieuChuyenThuocExportExcelChild>>();
                item.YeuCauDieuChuyenThuocExportExcelChild.AddRange(dataChildExcel);
            }
            var lstValueObject = new List<(string, string)>
            {
                (nameof(YeuCauDieuChuyenThuocExportExcel.SoPhieu), "Số phiếu"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenKhoXuat), "Kho xuất"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenKhoNhap), "Kho nhập"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenNhanVienYeuCau), "Người yêu cầu"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.NgayYeuCauDisplay), "Ngày yêu cầu"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TinhTrangDisplay), "Tình trạng"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenNhanVienDuyet), "Người duyệt"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.NgayDuyetDisplay), "Ngày duyệt"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.YeuCauDieuChuyenThuocExportExcelChild), "")
            };
            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "DS Điều Chuyển Kho Nội Bộ", 2, "DS Điều Chuyển Kho Nội Bộ", true);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DieuChuyenKhoNoiBo" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #endregion

        #region DUYỆT 

        [HttpPost("XuLyDuyetDieuChuyenThuocAsync")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult> XuLyDuyetDieuChuyenThuocAsync(long yeuCauDieuChuyenDuocPhamId)
        {
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.CheckPhieuYeuCauDieuChuyenDaDuyetHoacDaHuy(yeuCauDieuChuyenDuocPhamId);
            var dieuChuyenThuoc = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService
                .GetByIdAsync(yeuCauDieuChuyenDuocPhamId, s =>
                            s.Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet)
                            .Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.NhapKhoDuocPhamChiTiet).ThenInclude(dpct => dpct.NhapKhoDuocPhams)
                             .Include(r => r.NguoiXuat).ThenInclude(r => r.User)
                             .Include(r => r.NguoiNhap).ThenInclude(r => r.User)
                            )
                             ;
            if (dieuChuyenThuoc == null)
            {
                throw new ApiException(_localizationService.GetResource("YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists"));
            }
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.XuLyDuyetDieuChuyenThuocAsync(dieuChuyenThuoc);
            return Ok();
        }

        [HttpPost("XuLyKhongDuyetDieuChuyenThuocAsync")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult> XuLyKhongDuyetDieuChuyenThuocAsync(long yeuCauDieuChuyenDuocPhamId, string lyDoKhongDuyet)
        {
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.CheckPhieuYeuCauDieuChuyenDaDuyetHoacDaHuy(yeuCauDieuChuyenDuocPhamId);
            var dieuChuyenThuoc = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService
                .GetByIdAsync(yeuCauDieuChuyenDuocPhamId, s => s.Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet)
                                                                .Include(r => r.YeuCauDieuChuyenDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.NhapKhoDuocPhamChiTiet));
            if (dieuChuyenThuoc == null)
            {
                throw new ApiException(_localizationService.GetResource("YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists"));
            }
            await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.XuLyKhongDuyetDieuChuyenThuocAsync(dieuChuyenThuoc, lyDoKhongDuyet);
            return Ok();
        }

        [HttpPost("ExportDuyetYeuCauDieuChuyenThuoc")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuChuyenNoiBoDuocPham)]
        public async Task<ActionResult> ExportDuyetYeuCauDieuChuyenThuoc(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (YeuCauDieuChuyenDuocPhamVo)p).ToList();
            var dataExcel = data.Map<List<YeuCauDieuChuyenThuocExportExcel>>();

            foreach (var item in dataExcel)
            {
                var queryThuoc = new QueryInfo()
                {
                    Skip = queryInfo.Skip,
                    Take = queryInfo.Take,
                    AdditionalSearchString = item.Id.ToString()
                };
                var gridChildData = await _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.GetDataForGridChildAsync(queryThuoc, true);
                var dataChild = gridChildData.Data.Select(p => (YeuCauDieuChuyenDuocPhamChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<YeuCauDieuChuyenThuocExportExcelChild>>();
                item.YeuCauDieuChuyenThuocExportExcelChild.AddRange(dataChildExcel);
            }
            var lstValueObject = new List<(string, string)>
            {
                (nameof(YeuCauDieuChuyenThuocExportExcel.SoPhieu), "Số phiếu"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenKhoXuat), "Kho xuất"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenKhoNhap), "Kho nhập"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenNhanVienYeuCau), "Người yêu cầu"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.NgayYeuCauDisplay), "Ngày yêu cầu"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TinhTrangDisplay), "Tình trạng"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.TenNhanVienDuyet), "Người duyệt"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.NgayDuyetDisplay), "Ngày duyệt"),
                (nameof(YeuCauDieuChuyenThuocExportExcel.YeuCauDieuChuyenThuocExportExcelChild), "")
            };
            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "DS Duyệt Điều Chuyển Kho Nội Bộ", 2, "DS Duyệt Điều Chuyển Kho Nội Bộ", true);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetDieuChuyenKhoNoiBo" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        [HttpPost("InPhieuXuatKhoDuocPhamDieuChuyen")]
        public string InPhieuXuatKhoDuocPhamDieuChuyen(YeuCauDieuChuyenDuocPhamDataVo yeuCauDieuChuyenDuocPhamDataVo)
        {
            var result = _xuatKhoDieuChuyenKhoNoiBoDuocPhamService.InPhieuXuatKhoDuocPhamDieuChuyen(yeuCauDieuChuyenDuocPhamDataVo);
            return result;
        }
    }
}