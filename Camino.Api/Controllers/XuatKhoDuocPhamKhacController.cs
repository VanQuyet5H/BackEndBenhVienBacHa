using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.XuatKhos;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using Camino.Services.XuatKhoKhacs;
using Camino.Services.XuatKhos;
using Camino.Services.YeuCauXuatKhacDuocPhams;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class XuatKhoDuocPhamKhacController : CaminoBaseController
    {
        private readonly IKhoDuocPhamService _khoDuocPhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoKhacService _xuatKhoKhacService;
        private readonly INhapKhoDuocPhamService _nhapKhoDuocPhamService;
        private readonly IXuatKhoService _xuatKhoService;
        private readonly IYeuCauXuatKhoDuocPhamService _yeuCauXuatKhoDuocPhamService;

        public XuatKhoDuocPhamKhacController(
            IKhoDuocPhamService khoDuocPhamService,
            ILocalizationService localizationService,
            IExcelService excelService,
            IXuatKhoKhacService xuatKhoKhacService,
            IYeuCauXuatKhoDuocPhamService yeuCauXuatKhoDuocPhamService,
            INhapKhoDuocPhamService nhapKhoDuocPhamService,
            IXuatKhoService xuatKhoService
            )
        {
            _khoDuocPhamService = khoDuocPhamService;
            _excelService = excelService;
            _localizationService = localizationService;
            _xuatKhoKhacService = xuatKhoKhacService;
            _nhapKhoDuocPhamService = nhapKhoDuocPhamService;
            _xuatKhoService = xuatKhoService;
            _yeuCauXuatKhoDuocPhamService = yeuCauXuatKhoDuocPhamService;
        }

        [HttpPost("KiemTraXuatHoaChatTheoMayXetNghiem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<List<string>>> KiemTraXuatHoaChatTheoMayXetNghiem(XuatKhoDuocPhamKhacViewModel model)
        {
            var yeuCauXuatKhoDuocPhamChiTiets = model.YeuCauXuatKhoDuocPhamChiTiets;           
            return Ok(_xuatKhoKhacService.KiemTraXuatHoaChatTheoMayXetNghiem(yeuCauXuatKhoDuocPhamChiTiets));
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetDataForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetTotalPageForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncDuocPhamDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDuocPhamDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetDataForGridAsyncDuocPhamDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDuocPhamDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDuocPhamDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetTotalPageForGridAsyncDuocPhamDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoDuocPham")]
        public async Task<ActionResult> GetKhoDuocPham(DropDownListRequestModel model)
        {
            var result = await _xuatKhoKhacService.GetKhoDuocPham(model);
            return Ok(result);
        }

        [HttpPost("GetKhoTheoLoaiDuocPham")]
        public async Task<ActionResult> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model)
        {
            var result = await _xuatKhoKhacService.GetKhoTheoLoaiDuocPham(model);
            return Ok(result);
        }

        [HttpPost("GetNhaCungCapTheoKhoDuocPhams")]
        public async Task<ActionResult> GetNhaCungCapTheoKhoDuocPhams(DropDownListRequestModel model)
        {
            var result = await _xuatKhoKhacService.GetNhaCungCapTheoKhoDuocPhams(model);
            return Ok(result);
        }

        [HttpPost("GetSoHoaDonTheoKhoDuocPhams")]
        public async Task<ActionResult> GetSoHoaDonTheoKhoDuocPhams(DropDownListRequestModel model)
        {
            var result = await _xuatKhoKhacService.GetSoHoaDonTheoKhoDuocPhams(model);
            return Ok(result);
        }

        [HttpPost("GetNguoiNhanXuatKhac")]
        public async Task<ActionResult> GetNguoiNhanXuatKhac(DropDownListRequestModel model)
        {
            var result = await _xuatKhoKhacService.GetNguoiNhanXuatKhac(model);
            return Ok(result);
        }

        [HttpPost("GetSoLuongTonThucTe")]
        public async Task<ActionResult> GetSoLuongTonThucTe(YeuCauXuatKhoDuocPhamGridVo yeuCauXuatKhoDuocPhamGridVo)
        {
            var result = await _xuatKhoKhacService.GetSoLuongTonThucTe(yeuCauXuatKhoDuocPhamGridVo);
            return Ok(result);
        }

        [HttpPost("XuatDuocPhamTheoNhom")]
        public async Task<ActionResult> XuatDuocPhamTheoNhom(YeuCauXuatDuocPhamChiTietTheoKhoXuatVos model)
        {
            return Ok(model);
        }


        [HttpPost("GetAllMayXetNghiemKhoKhac")]
        public ActionResult GetAllMayXetNghiemYeuCauLinh([FromBody]DropDownListRequestModel model)
        {
            var duocPhamMayXetNghiemJson = JsonConvert.DeserializeObject<DuocPhamBenhVienMayXetNghiemJson>(model.ParameterDependencies.Replace("undefined", "null"));
            return Ok(_xuatKhoKhacService.GetAllMayXetNghiemKhoKhac(model, duocPhamMayXetNghiemJson));
        }

        [HttpGet("GetLoaiKho")]
        public async Task<ActionResult> GetLoaiKho(long khoId)
        {
            var result = await _xuatKhoKhacService.GetLoaiKho(khoId);
            return Ok(result);
        }

        #region CRUD

        [HttpPost("ThemYeuCauXuatThuocKhac")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult> ThemYeuCauXuatThuocKhac(XuatKhoDuocPhamKhacViewModel model)
        {
            if (!model.YeuCauXuatKhoDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));
            }
            if (model.YeuCauXuatKhoDuocPhamChiTiets.All(z => z.SoLuongXuat == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var yeuCauXuatKhac = model.ToEntity<YeuCauXuatKhoDuocPham>();
            //await _xuatKhoKhacService.XuLyThemHoacCapNhatVaDuyetYeuCauThuocAsync(yeuCauXuatKhac, model.YeuCauXuatKhoDuocPhamChiTiets, model.LaLuuDuyet, true);
            await _xuatKhoKhacService.XuLyThemYeuCauXuatKhoKSNKAsync(yeuCauXuatKhac, model.YeuCauXuatKhoDuocPhamChiTiets, model.LaLuuDuyet);
            return Ok(yeuCauXuatKhac.Id);
        }

        [HttpPost("CapNhatYeuCauXuatThuocKhac")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult> CapNhatYeuCauXuatThuocKhac(XuatKhoDuocPhamKhacViewModel model)
        {
            if (!model.YeuCauXuatKhoDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));
            }
            if (model.YeuCauXuatKhoDuocPhamChiTiets.All(z => z.SoLuongXuat == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            await _xuatKhoKhacService.CheckPhieuYeuCauXuatThuocKhacDaDuyetHoacDaHuy(model.Id);
            var yeuCauXuatKhac = await _yeuCauXuatKhoDuocPhamService
                .GetByIdAsync(model.Id, s =>
                            s.Include(r => r.YeuCauXuatKhoDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.YeuCauXuatKhoDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham)
                             .Include(r => r.YeuCauXuatKhoDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.NhapKhoDuocPhamChiTiet)
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
            //await _xuatKhoKhacService.XuLyThemHoacCapNhatVaDuyetYeuCauThuocAsync(yeuCauXuatKhac, model.YeuCauXuatKhoDuocPhamChiTiets, model.LaLuuDuyet, false);
            var result = await _xuatKhoKhacService.XuLyCapNhatYeuCauXuatKhoKSNKAsync(yeuCauXuatKhac, model.YeuCauXuatKhoDuocPhamChiTiets, model.LaLuuDuyet);
            return Ok(result);
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
            //    var yeuCauXuatKhacDelete = await _yeuCauXuatKhoDuocPhamService
            //   .GetByIdAsync(model.Id, s => s.Include(r => r.YeuCauXuatKhoDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham));
            //    var resDelete = await _xuatKhoKhacService.XuLyXoaYeuCauThuocAsync(yeuCauXuatKhacDelete);
            //    var result = new
            //    {
            //        resDelete.Id,
            //        resDelete.LastModified
            //    };
            //    return Ok(result);
            //}
        }

        [HttpPost("XoaYeuCauXuatThuoc")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult> Delete(long id)
        {
            var dieuChuyenThuoc = await _yeuCauXuatKhoDuocPhamService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauXuatKhoDuocPhamChiTiets));

            if (dieuChuyenThuoc == null)
            {
                throw new ApiException(_localizationService.GetResource("YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists"));
            }
            await _yeuCauXuatKhoDuocPhamService.DeleteByIdAsync(id);
            return NoContent();
        }


        [HttpGet("GetXuatKhoDuocPhamKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult> GetXuatKhoDuocPhamKhac(long id)
        {

            var xuatKhoDuoc = await _xuatKhoKhacService.GetByIdAsync(id,
                                    p => p.Include(x => x.XuatKhoDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris)
                                                                                  .ThenInclude(x => x.NhapKhoDuocPhamChiTiet).ThenInclude(x => x.KhoDuocPhamViTri)
                                        .Include(x => x.XuatKhoDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPham).ThenInclude(x => x.NhapKhoDuocPhams).ThenInclude(x => x.NhapKhoDuocPhamChiTiets)
                                        .Include(x => x.KhoDuocPhamNhap)
                                        .Include(x => x.KhoDuocPhamXuat)
                                        .Include(x => x.NhaThau)
                                        .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                        .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                        .Include(x => x.XuatKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham)
                                        );

            var result = xuatKhoDuoc.ToModel<XuatKhoDuocPhamKhacViewModel>();
            result.LoaiKho = xuatKhoDuoc.KhoDuocPhamXuat.LoaiKho;
            result.NhapKhoDuocPhamId = await _xuatKhoKhacService.GetNhapKhoDuocPhamIdBy(result.SoChungTu);
            return Ok(result);
        }


        [HttpGet("GetYeuCauXuatKhoDuocPhamKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult> GetYeuCauXuatKhoDuocPhamKhac(long id)
        {
            var yeuCauXuatKhoDuoc = await _yeuCauXuatKhoDuocPhamService.GetByIdAsync(id,
                            p => p.Include(x => x.YeuCauXuatKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                                .Include(x => x.KhoXuat)
                                .Include(x => x.NhaThau)
                                .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                );
            var result = yeuCauXuatKhoDuoc.ToModel<XuatKhoDuocPhamKhacViewModel>();
            result.YeuCauXuatKhoDuocPhamChiTietHienThis = await _xuatKhoKhacService.YeuCauXuatDuocPhamChiTiets(id);
            result.LoaiKho = yeuCauXuatKhoDuoc.KhoXuat.LoaiKho;
            result.NhapKhoDuocPhamId = await _xuatKhoKhacService.GetNhapKhoDuocPhamIdBy(result.SoChungTu);
            return Ok(result);
        }

        [HttpGet("GetTrangThaiYeuCauXuatKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoDuocPhamKhac)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiYeuCauXuatKhac(long yeuCauXuatKhoDuocPhamId)
        {
            var result = await _xuatKhoKhacService.GetTrangThaiPhieuLinh(yeuCauXuatKhoDuocPhamId);
            return Ok(result);
        }
        #endregion

        [HttpPost("ExportData")]
        public async Task<ActionResult> ExportData(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKhacService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (XuatKhoDuocPhamKhacGridVo)p).ToList();
            var dataExcel = data.Map<List<XuatKhoDuocPhamKhacExportExcel>>();

            foreach (var item in dataExcel)
            {
                var queryThuoc = new QueryInfo()
                {
                    Skip = queryInfo.Skip,
                    Take = queryInfo.Take,
                    AdditionalSearchString = item.Id + ";" + item.TinhTrang,
                };
                var gridChildData = await _xuatKhoKhacService.GetDataForGridChildAsync(queryThuoc, true);
                var dataChild = gridChildData.Data.Select(p => (YeuCauXuatKhoDuocPhamGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<XuatKhoDuocPhamKhacExportExcelChild>>();
                item.XuatKhoDuocPhamKhacExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(XuatKhoDuocPhamKhacExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(XuatKhoDuocPhamKhacExportExcel.KhoDuocPhamXuat), "Nơi Xuất"),
                (nameof(XuatKhoDuocPhamKhacExportExcel.TenNguoiXuat), "Người Xuất"),
                (nameof(XuatKhoDuocPhamKhacExportExcel.NgayXuatDisplay), "Ngày Xuất"),
                (nameof(XuatKhoDuocPhamKhacExportExcel.TenNguoiNhan), "Người Nhận"),
                (nameof(XuatKhoDuocPhamKhacExportExcel.TinhTrangDisplay), "Tình Trạng"),
                (nameof(XuatKhoDuocPhamKhacExportExcel.LyDoXuatKho), "Lý Do Xuất"),
                (nameof(XuatKhoDuocPhamKhacExportExcel.XuatKhoDuocPhamKhacExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Xuất kho dược phẩm khác");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("InPhieuXuatKhoKhac")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string InPhieuXuatKhoKhac(PhieuXuatKhoKhacVo phieuXuatKhoKhac)
        {
            var html = _xuatKhoKhacService.InPhieuXuatKhoKhac(phieuXuatKhoKhac);
            return html;
        }
    }
}
