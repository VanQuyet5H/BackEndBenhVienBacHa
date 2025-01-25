using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.XuatKhoKSNKs;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

//using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;


using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Services.NhapKhoVatTus;
using Camino.Services.VatTu;
using Camino.Services.XuatKhoKhacKSNKs;
using Camino.Services.XuatKhoKhacs;
using Camino.Services.XuatKhos;
using Camino.Services.YeuCauXuatKhacVatTus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class XuatKhoKhacKSNKController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoVatTuService _xuatKhoVatTuService;
        private readonly IVatTuService _vatTuService;
        private readonly INhapKhoVatTuService _nhapKhoVatTuService;
        private readonly IKhoDuocPhamService _khoDuocPhamService;
        private readonly IXuatKhoService _xuatKhoService;
        private readonly IXuatKhoKhacKSNKService _xuatKhoVatTuKhacService;
        private readonly IYeuCauXuatKhoVatTuService _yeuCauXuatKhoVatTuService;

        private readonly Camino.Services.XuatKhoKhacs.IXuatKhoKhacService _xuatKhoDuocPhamKhacService;
        private readonly Camino.Services.YeuCauXuatKhacDuocPhams.IYeuCauXuatKhoDuocPhamService _yeuCauXuatKhoDuocPhamService;

        public XuatKhoKhacKSNKController(ILocalizationService localizationService
            , IVatTuService vatTuService, IKhoDuocPhamService khoDuocPhamService
            , INhapKhoVatTuService nhapKhoVatTuService, IXuatKhoService xuatKhoService
            , IExcelService excelService, IXuatKhoVatTuService xuatKhoVatTuService
            , IYeuCauXuatKhoVatTuService yeuCauXuatKhoVatTuService
            , Camino.Services.XuatKhoKhacs.IXuatKhoKhacService xuatKhoDuocPhamKhacService
            , Camino.Services.YeuCauXuatKhacDuocPhams.IYeuCauXuatKhoDuocPhamService yeuCauXuatKhoDuocPhamService
            , IXuatKhoKhacKSNKService xuatKhoVatTuKhacService)
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
            _xuatKhoDuocPhamKhacService = xuatKhoDuocPhamKhacService;
            _yeuCauXuatKhoDuocPhamService = yeuCauXuatKhoDuocPhamService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridVatTuChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridVatTuChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridVatTuChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridVatTuChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridVatTuChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridVatTuChildAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDuocPhamChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDuocPhamChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridDuocPhamChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDuocPhamChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDuocPhamChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridDuocPhamChildAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncVatTuDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncVatTuDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridAsyncDpVtKsnkDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncVatTuDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncVatTuDaChon([FromBody]QueryInfo queryInfo)
        {
            //bỏ lazyLoadPage
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridAsyncDpVtKsnkDaChon(queryInfo);
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
            var result = await _xuatKhoVatTuKhacService.GetKhoKsnk(model);
            return Ok(result);
        }


        [HttpPost("GetSoLuongTonThucTe")]
        public async Task<ActionResult> GetSoLuongTonThucTe(YeuCauXuatKhoKSNKGridVo yeuCauXuatKhoVatTuGridVo)
        {
            var result = await _xuatKhoVatTuKhacService.GetSoLuongTonThucTe(yeuCauXuatKhoVatTuGridVo);
            return Ok(result);
        }

        [HttpPost("XuatVatTuTheoNhom")]
        public async Task<ActionResult> XuatVatTuTheoNhom(YeuCauXuatKSNKChiTietTheoKhoXuatVos model)
        {
            return Ok(model);
        }

        [HttpPost("GetSoHoaDonTheoKhoVatTus")]
        public async Task<ActionResult> GetSoHoaDonTheoKhoVatTus(DropDownListRequestModel model)
        {
            var result = await _xuatKhoVatTuKhacService.GetSoHoaDonTheoKhoKSNK(model);
            return Ok(result);
        }

        #region CRUD

        [HttpPost("ThemYeuCauXuatVatTuKhac")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult> ThemYeuCauXuatVatTuKhac(XuatKhoKSNKKhacViewModel model)
        {
            if (!model.YeuCauXuatKhoVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.Required"));
            }
            if (model.YeuCauXuatKhoVatTuChiTiets.All(z => z.SoLuongXuat == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }

            var xuatKhoKhacKSNKVo = model.Map<XuatKhoKhacKSNKVo>();
            var resultVo = await _xuatKhoVatTuKhacService.XuLyThemYeuCauXuatKhoKSNKAsync(xuatKhoKhacKSNKVo, model.YeuCauXuatKhoVatTuChiTiets, model.LaLuuDuyet);

            var lstPhieuXuatDPVaVT = new List<string>();

            if (resultVo != null && resultVo.XuatVatTuId != null)
            {
                var phieuXuatKhoKhacVatTuVo = new PhieuXuatKhoKhacVo
                {
                    Id = model.Id = resultVo.XuatVatTuId ?? 0,
                    CoNCC = model.TraNCC,
                    DuocDuyet = model.LaLuuDuyet,
                    HostingName = model.HostingName,
                    LaDuocPham = false
                };

                var phieuXuatVT = _xuatKhoVatTuKhacService.InPhieuXuatKhoKhacKSNK(phieuXuatKhoKhacVatTuVo) + "<div class=\"pagebreak\"> </div>";
                lstPhieuXuatDPVaVT.Add(phieuXuatVT);
            }
            if (resultVo != null && resultVo.XuatDuocPhamId != null)
            {
                var phieuXuatKhoKhacDuocPhamVo = new PhieuXuatKhoKhacVo
                {
                    Id = model.Id = resultVo.XuatDuocPhamId ?? 0,
                    CoNCC = model.TraNCC,
                    DuocDuyet = model.LaLuuDuyet,
                    HostingName = model.HostingName,
                    LaDuocPham = true
                };

                var phieuXuatVT = _xuatKhoVatTuKhacService.InPhieuXuatKhoKhacKSNK(phieuXuatKhoKhacDuocPhamVo);
                lstPhieuXuatDPVaVT.Add(phieuXuatVT);
            }
            resultVo.PhieuInDuocPhamVaVatTus.AddRange(lstPhieuXuatDPVaVT);

            return Ok(resultVo);

        }

        [HttpPost("CapNhatYeuCauXuatVatTuKhac")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult> CapNhatYeuCauXuatVatTuKhac(XuatKhoKSNKKhacViewModel model)
        {
            if (!model.YeuCauXuatKhoVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.Required"));
            }
            if (model.YeuCauXuatKhoVatTuChiTiets.All(z => z.SoLuongXuat == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }

            var xuatKhoKhacKSNKVo = model.Map<XuatKhoKhacKSNKVo>();
            var resultVo = await _xuatKhoVatTuKhacService.XuLyCapNhatYeuCauXuatKhoKSNKAsync(xuatKhoKhacKSNKVo, model.YeuCauXuatKhoVatTuChiTiets, model.LaLuuDuyet);
            return Ok(resultVo);
        }

        [HttpPost("XoaYeuCauXuatVatTu")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
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
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
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

            var result = xuatKhoDuoc.ToModel<XuatKhoKSNKKhacViewModel>();
            result.LoaiKho = xuatKhoDuoc.KhoVatTuXuat.LoaiKho;
            result.NhapKhoVatTuId = await _xuatKhoVatTuKhacService.GetNhapKhoVatTuIdBy(result.SoChungTu);
            return Ok(result);
        }


        [HttpGet("GetYeuCauXuatKhoVatTuKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult> GetYeuCauXuatKhoVatTuKhac(long id)
        {
            var yeuCauXuatKhoDuoc = await _yeuCauXuatKhoVatTuService.GetByIdAsync(id,
                            p => p.Include(x => x.YeuCauXuatKhoVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
                                .Include(x => x.KhoXuat)
                                .Include(x => x.NhaThau)
                                .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                );
            var result = yeuCauXuatKhoDuoc.ToModel<XuatKhoKSNKKhacViewModel>();
            result.YeuCauXuatKhoVatTuChiTietHienThis = await _xuatKhoVatTuKhacService.YeuCauXuatVatTuChiTiets(id);
            result.LoaiKho = yeuCauXuatKhoDuoc.KhoXuat.LoaiKho;
            result.NhapKhoVatTuId = await _xuatKhoVatTuKhacService.GetNhapKhoVatTuIdBy(result.SoChungTu);
            return Ok(result);
        }

        [HttpGet("GetTrangThaiYeuCauXuatKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
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
            var dataExcel = data.Map<List<Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoKhacKSNKExportExcel>>();

            foreach (var item in dataExcel)
            {
                if (item.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu)
                {
                    var queryThuoc = new QueryInfo()
                    {
                        Skip = queryInfo.Skip,
                        Take = queryInfo.Take,
                        AdditionalSearchString = item.Id + ";" + item.TinhTrang,
                    };

                    var gridChildData = await _xuatKhoVatTuKhacService.GetDataForGridVatTuChildAsync(queryThuoc, true);

                    var dataChild = gridChildData.Data.Select(p => (YeuCauXuatKhoKSNKGridVo)p).ToList();
                    var dataChildExcel = dataChild.Map<List<XuatKhoKhacKSNKExportExcelChild>>();

                    item.XuatKhoKhacKSNKExportExcelChild.AddRange(dataChildExcel);
                }

                if (item.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham)
                {
                    var queryThuoc = new QueryInfo()
                    {
                        Skip = queryInfo.Skip,
                        Take = queryInfo.Take,
                        AdditionalSearchString = item.Id + ";" + item.TinhTrang,
                    };

                    var gridChildData = await _xuatKhoVatTuKhacService.GetDataForGridDuocPhamChildAsync(queryThuoc, true);

                    var dataChild = gridChildData.Data.Select(p => (YeuCauXuatKhoKSNKGridVo)p).ToList();
                    var dataChildExcel = dataChild.Map<List<XuatKhoKhacKSNKExportExcelChild>>();

                    item.XuatKhoKhacKSNKExportExcelChild.AddRange(dataChildExcel);
                }
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(XuatKhoKhacKSNKExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(XuatKhoKhacKSNKExportExcel.KhoVatTuXuat), "Nơi Xuất"),
                (nameof(XuatKhoKhacKSNKExportExcel.LyDoXuatKho), "Lý Do Xuất"),
                (nameof(XuatKhoKhacKSNKExportExcel.TenNguoiNhan), "Người Nhận"),
                (nameof(XuatKhoKhacKSNKExportExcel.TenNguoiXuat), "Người Xuất"),
                (nameof(XuatKhoKhacKSNKExportExcel.TinhTrangDisplay), "Tình Trạng"),
                (nameof(XuatKhoKhacKSNKExportExcel.NgayXuatDisplay), "Ngày Xuất"),
                (nameof(XuatKhoKhacKSNKExportExcel.XuatKhoKhacKSNKExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Kiểm Soát Nhiễm Khuẩn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatKhoKhacKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion export excel     


        [HttpPost("InPhieuXuatKhoKhacKSNK")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string InPhieuXuatKhoKhacKSNK(PhieuXuatKhoKhacVo phieuXuatKhoKhac)
        {
            var html = _xuatKhoVatTuKhacService.InPhieuXuatKhoKhacKSNK(phieuXuatKhoKhac);
            return html;
        }


        [HttpGet("GetXuatKhoDuocPhamKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult> GetXuatKhoDuocPhamKhac(long id)
        {

            var xuatKhoDuoc = await _xuatKhoDuocPhamKhacService.GetByIdAsync(id,
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

            var result = xuatKhoDuoc.ToModel<XuatKhoKSNKKhacViewModel>();
            result.LoaiKho = xuatKhoDuoc.KhoDuocPhamXuat.LoaiKho;
            result.NhapKhoVatTuId = await _xuatKhoDuocPhamKhacService.GetNhapKhoDuocPhamIdBy(result.SoChungTu);

            return Ok(result);
        }

        [HttpGet("GetYeuCauXuatKhoDuocPhamKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult> GetYeuCauXuatKhoDuocPhamKhac(long id)
        {
            var yeuCauXuatKhoDuoc = await _yeuCauXuatKhoDuocPhamService.GetByIdAsync(id,
                            p => p.Include(x => x.YeuCauXuatKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                                .Include(x => x.KhoXuat)
                                .Include(x => x.NhaThau)
                                .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                );

            var result = yeuCauXuatKhoDuoc.ToModel<XuatKhoKSNKKhacViewModel>();
            result.YeuCauXuatKhoVatTuChiTietHienThis = await _xuatKhoVatTuKhacService.YeuCauXuatDuocPhamChiTiets(id);
            result.LoaiKho = yeuCauXuatKhoDuoc.KhoXuat.LoaiKho;
            result.NhapKhoVatTuId = await _xuatKhoDuocPhamKhacService.GetNhapKhoDuocPhamIdBy(result.SoChungTu);

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildDuocPhamAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildDuocPhamAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetDataForGridChildAsyncDuocPhamDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildDuocPhamAsyncDaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildDuocPhamAsyncDaDuyet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuKhacService.GetTotalPageForGridChildAsyncDuocPhamDaDuyet(queryInfo);
            return Ok(gridData);
        }


    }
}
