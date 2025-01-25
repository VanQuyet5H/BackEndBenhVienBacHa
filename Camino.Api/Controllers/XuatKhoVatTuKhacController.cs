using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.XuatKhos;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Services.NhapKhoVatTus;
using Camino.Services.VatTu;
using Camino.Services.XuatKhoKhacs;
using Camino.Services.XuatKhos;
using Camino.Services.YeuCauXuatKhacVatTus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class XuatKhoVatTuKhacController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoVatTuService _xuatKhoVatTuService;
        private readonly IVatTuService _vatTuService;
        private readonly INhapKhoVatTuService _nhapKhoVatTuService;
        private readonly IKhoDuocPhamService _khoDuocPhamService;
        private readonly IXuatKhoService _xuatKhoService;
        private readonly IXuatKhoVatTuKhacService _xuatKhoVatTuKhacService;
        private readonly IYeuCauXuatKhoVatTuService _yeuCauXuatKhoVatTuService;

        public XuatKhoVatTuKhacController(ILocalizationService localizationService
            , IVatTuService vatTuService, IKhoDuocPhamService khoDuocPhamService
            , INhapKhoVatTuService nhapKhoVatTuService, IXuatKhoService xuatKhoService
            , IExcelService excelService, IXuatKhoVatTuService xuatKhoVatTuService
            , IYeuCauXuatKhoVatTuService yeuCauXuatKhoVatTuService
            , IXuatKhoVatTuKhacService xuatKhoVatTuKhacService)
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _xuatKhoVatTuService = xuatKhoVatTuService;
            _vatTuService = vatTuService;
            _khoDuocPhamService = khoDuocPhamService;
            _nhapKhoVatTuService = nhapKhoVatTuService;
            _xuatKhoService = xuatKhoService;
            _yeuCauXuatKhoVatTuService = yeuCauXuatKhoVatTuService;
            _xuatKhoVatTuKhacService = xuatKhoVatTuKhacService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncVatTuDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncVatTuDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridAsyncVatTuDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncVatTuDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncVatTuDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridAsyncVatTuDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoVatTu")]
        public async Task<ActionResult> GetKhoVatTu(DropDownListRequestModel model)
        {
            var result = await _xuatKhoVatTuKhacService.GetKhoVatTu(model);
            return Ok(result);
        }

        [HttpPost("GetKhoTheoLoaiVatTu")]
        public async Task<ActionResult> GetKhoTheoLoaiVatTu(DropDownListRequestModel model)
        {
            var result = await _xuatKhoVatTuKhacService.GetKhoTheoLoaiVatTu(model);
            return Ok(result);
        }


        [HttpPost("GetSoLuongTonThucTe")]
        public async Task<ActionResult> GetSoLuongTonThucTe(YeuCauXuatKhoVatTuGridVo yeuCauXuatKhoVatTuGridVo)
        {
            var result = await _xuatKhoVatTuKhacService.GetSoLuongTonThucTe(yeuCauXuatKhoVatTuGridVo);
            return Ok(result);
        }

        [HttpPost("XuatVatTuTheoNhom")]
        public async Task<ActionResult> XuatVatTuTheoNhom(YeuCauXuatVatTuChiTietTheoKhoXuatVos model)
        {
            return Ok(model);
        }

        [HttpPost("GetSoHoaDonTheoKhoVatTus")]
        public async Task<ActionResult> GetSoHoaDonTheoKhoVatTus(DropDownListRequestModel model)
        {
            var result = await _xuatKhoVatTuKhacService.GetSoHoaDonTheoKhoVatTus(model);
            return Ok(result);
        }

        #region CRUD

        [HttpPost("ThemYeuCauXuatVatTuKhac")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult> ThemYeuCauXuatVatTuKhac(XuatKhoVatTuKhacViewModel model)
        {
            if (!model.YeuCauXuatKhoVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.Required"));
            }
            if (model.YeuCauXuatKhoVatTuChiTiets.All(z => z.SoLuongXuat == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var yeuCauXuatKhac = model.ToEntity<YeuCauXuatKhoVatTu>();
            //await _xuatKhoVatTuKhacService.XuLyThemHoacCapNhatVaDuyetYeuCauVatTuAsync(yeuCauXuatKhac, model.YeuCauXuatKhoVatTuChiTiets, model.LaLuuDuyet, true);
            await _xuatKhoVatTuKhacService.XuLyThemYeuCauXuatKhoVatTuAsync(yeuCauXuatKhac, model.YeuCauXuatKhoVatTuChiTiets, model.LaLuuDuyet);
            return Ok(yeuCauXuatKhac.Id);
        }

        [HttpPost("CapNhatYeuCauXuatVatTuKhac")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult> CapNhatYeuCauXuatVatTuKhac(XuatKhoVatTuKhacViewModel model)
        {
            if (!model.YeuCauXuatKhoVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.Required"));
            }
            if (model.YeuCauXuatKhoVatTuChiTiets.All(z => z.SoLuongXuat == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            await _xuatKhoVatTuKhacService.CheckPhieuYeuCauXuatVTKhacDaDuyetHoacDaHuy(model.Id);
            var yeuCauXuatKhac = await _yeuCauXuatKhoVatTuService
                .GetByIdAsync(model.Id, s =>
                            s.Include(r => r.YeuCauXuatKhoVatTuChiTiets).ThenInclude(ct => ct.VatTuBenhVien).ThenInclude(dpct => dpct.VatTus)
                             .Include(r => r.YeuCauXuatKhoVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(dpct => dpct.XuatKhoVatTuChiTiet).ThenInclude(dp => dp.XuatKhoVatTu)
                             .Include(r => r.YeuCauXuatKhoVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(ct => ct.NhapKhoVatTuChiTiet)
                             .Include(r => r.KhoXuat)
                             .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                             .Include(r => r.NguoiNhan).ThenInclude(r => r.User)
                             .Include(r => r.NguoiXuat).ThenInclude(r => r.User)
                             )
                             ;

            if (yeuCauXuatKhac == null)
            {
                return NotFound();
            }
            model.ToEntity(yeuCauXuatKhac);
            var result = await _xuatKhoVatTuKhacService.XuLyCapNhatYeuCauXuatKhoVatTuAsync(yeuCauXuatKhac, model.YeuCauXuatKhoVatTuChiTiets, model.LaLuuDuyet);
            return Ok(result);
            //await _xuatKhoVatTuKhacService.XuLyThemHoacCapNhatVaDuyetYeuCauVatTuAsync(yeuCauXuatKhac, model.YeuCauXuatKhoVatTuChiTiets, model.LaLuuDuyet, false);
            //if (!model.LaLuuDuyet)
            //{
            //    var result = new
            //    {
            //        yeuCauXuatKhac.Id,
            //        yeuCauXuatKhac.LastModified
            //    };
            //    return Ok(result);
            //}
            //else
            //{
            //    var yeuCauXuatKhacDelete = await _yeuCauXuatKhoVatTuService
            //   .GetByIdAsync(model.Id, s => s.Include(r => r.YeuCauXuatKhoVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(dpct => dpct.XuatKhoVatTuChiTiet).ThenInclude(dp => dp.XuatKhoVatTu));
            //    var resDelete = await _xuatKhoVatTuKhacService.XuLyXoaYeuCauVatTuAsync(yeuCauXuatKhacDelete);
            //    var result = new
            //    {
            //        resDelete.Id,
            //        resDelete.LastModified
            //    };
            //    return Ok(result);
            //}

        }

        [HttpPost("XoaYeuCauXuatVatTu")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult> Delete(long id)
        {
            var dieuChuyenThuoc = await _yeuCauXuatKhoVatTuService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauXuatKhoVatTuChiTiets));

            if (dieuChuyenThuoc == null)
            {
                throw new ApiException(_localizationService.GetResource("YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists"));
            }
            await _yeuCauXuatKhoVatTuService.DeleteByIdAsync(id);
            return NoContent();
        }


        [HttpGet("GetXuatKhoVatTuKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult> GetXuatKhoVatTuKhac(long id)
        {

            var xuatKhoDuoc = await _xuatKhoVatTuKhacService.GetByIdAsync(id,
                                    p => p.Include(x => x.XuatKhoVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTris)
                                                                                  .ThenInclude(x => x.NhapKhoVatTuChiTiet).ThenInclude(x => x.KhoViTri)
                                        .Include(x => x.XuatKhoVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTu).ThenInclude(x => x.NhapKhoVatTus).ThenInclude(x => x.NhapKhoVatTuChiTiets)
                                        .Include(x => x.KhoVatTuXuat)
                                        .Include(x => x.KhoVatTuNhap)
                                        .Include(x => x.NhaThau)
                                        .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                        .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                        .Include(x => x.XuatKhoVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
                                        );

            var result = xuatKhoDuoc.ToModel<XuatKhoVatTuKhacViewModel>();
            result.LoaiKho = xuatKhoDuoc.KhoVatTuXuat.LoaiKho;
            result.NhapKhoVatTuId = await _xuatKhoVatTuKhacService.GetNhapKhoVatTuIdBy(result.SoChungTu);
            return Ok(result);
        }


        [HttpGet("GetYeuCauXuatKhoVatTuKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult> GetYeuCauXuatKhoVatTuKhac(long id)
        {
            var yeuCauXuatKhoDuoc = await _yeuCauXuatKhoVatTuService.GetByIdAsync(id,
                            p => p.Include(x => x.YeuCauXuatKhoVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
                                .Include(x => x.KhoXuat)
                                .Include(x => x.NhaThau)
                                .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                );
            var result = yeuCauXuatKhoDuoc.ToModel<XuatKhoVatTuKhacViewModel>();
            result.YeuCauXuatKhoVatTuChiTietHienThis = await _xuatKhoVatTuKhacService.YeuCauXuatVatTuChiTiets(id);
            result.LoaiKho = yeuCauXuatKhoDuoc.KhoXuat.LoaiKho;
            result.NhapKhoVatTuId = await _xuatKhoVatTuKhacService.GetNhapKhoVatTuIdBy(result.SoChungTu);
            return Ok(result);
        }

        [HttpGet("GetTrangThaiYeuCauXuatKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoVatTuKhac)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiYeuCauXuatKhac(long yeuCauXuatKhoVatTuId)
        {
            var result = await _xuatKhoVatTuKhacService.GetTrangThaiPhieuLinh(yeuCauXuatKhoVatTuId);
            return Ok(result);
        }
        #endregion

        #region export excel
        [HttpPost("ExportData")]
        public async Task<ActionResult> ExportData(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (XuatKhoVatTuKhacGridVo)p).ToList();
            var dataExcel = data.Map<List<XuatKhoVatTuKhacExportExcel>>();

            foreach (var item in dataExcel)
            {
                var queryThuoc = new QueryInfo()
                {
                    Skip = queryInfo.Skip,
                    Take = queryInfo.Take,
                    AdditionalSearchString = item.Id + ";" + item.TinhTrang,
                };
                var gridChildData = await _xuatKhoVatTuKhacService.GetDataForGridChildAsync(queryThuoc, true);
                var dataChild = gridChildData.Data.Select(p => (YeuCauXuatKhoVatTuGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<XuatKhoVatTuKhacExportExcelChild>>();
                item.XuatKhoVatTuKhacExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(XuatKhoVatTuKhacExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(XuatKhoVatTuKhacExportExcel.KhoVatTuXuat), "Nơi Xuất"),
                (nameof(XuatKhoVatTuKhacExportExcel.LyDoXuatKho), "Lý Do Xuất"),
                (nameof(XuatKhoVatTuKhacExportExcel.TenNguoiNhan), "Người Nhận"),
                (nameof(XuatKhoVatTuKhacExportExcel.TenNguoiXuat), "Người Xuất"),
                (nameof(XuatKhoVatTuKhacExportExcel.TinhTrangDisplay), "Tình Trạng"),
                (nameof(XuatKhoVatTuKhacExportExcel.NgayXuatDisplay), "Ngày Xuất"),
                (nameof(XuatKhoVatTuKhacExportExcel.XuatKhoVatTuKhacExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Xuất kho vật tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatKhoVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion export excel
    }
}
