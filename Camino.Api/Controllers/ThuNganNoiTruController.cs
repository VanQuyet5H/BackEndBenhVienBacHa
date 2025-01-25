using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.BenhNhans;
using Camino.Services.BenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class ThuNganNoiTruController : CaminoBaseController
    {
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IThuNganNoiTruService _thuNganNoiTruService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IBenhVienService _benhVienService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IExcelService _excelService;
        private readonly IPdfService _pdfService;

        public ThuNganNoiTruController(IYeuCauTiepNhanService yeuCauTiepNhanService,
                                IThuNganNoiTruService thuNganNoiTruService,
                                ITaiLieuDinhKemService taiLieuDinhKemService,
                                ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
                                IBenhVienService benhVienService,
                                IKhoaPhongService khoaPhongService,
                                IPdfService pdfService,
                                IExcelService excelService)
        {
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _thuNganNoiTruService = thuNganNoiTruService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _khoaPhongService = khoaPhongService;
            _benhVienService = benhVienService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _excelService = excelService;
            _pdfService = pdfService;
        }

        [HttpPost("ThayDoiLoaiGiaChuaThu")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThayDoiLoaiGiaChuaThu(DoiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru)
        {
            await _thuNganNoiTruService.ThayDoiLoaiGiaChuaThu(doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru);
            return Ok();
        }

        [HttpPost("ApDungMiemGiamTuBacSi")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ApDungMiemGiamTuBacSi(long yeuCauTiepNhanId)
        {
            await _thuNganNoiTruService.ApDungMiemGiamTuNoiGioiThieu(yeuCauTiepNhanId);
            return Ok();
        }

        [HttpPost("HuyApDungMiemGiamTuBacSi")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> HuyApDungMiemGiamTuBacSi(long yeuCauTiepNhanId)
        {
            await _thuNganNoiTruService.HuyApDungMiemGiamTuNoiGioiThieu(yeuCauTiepNhanId);
            return Ok();
        }

        #region Update 27/04/2021

        [HttpPost("HoanUngKhongThucHienDichVu/{taiKhoanBenhNhanThuId}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult HoanUngKhongThucHienDichVu(long taiKhoanBenhNhanThuId)
        {
            return Ok(_thuNganNoiTruService.HoanUng(taiKhoanBenhNhanThuId));
        }

        [HttpPost("LuuTamChiPhiNoiTruTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> LuuTamChiPhiNoiTruTrongGoi(QuyetToanDichVuTrongGoiVo model)
        {
            await _thuNganNoiTruService.LuuTamChiPhiNoiTruTrongGoi(model);
            return Ok();
        }

        [HttpGet("SoTienQuyBHYTTTTrongGoi/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<decimal>> SoTienQuyBHYTTTTrongGoi(long id)
        {
            //var soTienQBHYTTrongGoi = _thuNganNoiTruService.GetDanhSachDichVuTrongGoiCoBHYTChuaQuyetToanNoiTru(id).Where(o => o.KiemTraBHYTXacNhan).Select(o=>o.BHYTThanhToan).DefaultIfEmpty().Sum();
            //return Ok(Math.Round(soTienQBHYTTrongGoi, MidpointRounding.AwayFromZero));
            return Ok(_thuNganNoiTruService.GetSoTienHoanUngTrongGoi(id));
        }

        [HttpPost("CapNhatDonGiaMoi")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<bool>> CapNhatDonGiaMoi(CapNhatDonGiaMoi capNhatDonGiaMoi)
        {
            return await _thuNganNoiTruService.CapNhatDonGiaMoi(capNhatDonGiaMoi);
        }

        [HttpGet("KiemTraDichVuTrongGoiCoBHYT")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<bool>> KiemTraDichVuTrongGoiCoBHYT(long yeuCauTiepNhanId)
        {
            return Ok(await _thuNganNoiTruService.KiemTraDichVuTrongGoiCoBHYTNoiTru(yeuCauTiepNhanId) > 0);
        }

        [HttpPost("GetDataDichVuTrongGoiCoBHYTForGrid/{yeuCauTiepNhanId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public ActionResult<GridDataSource> GetDataDichVuTrongGoiCoBHYTForGrid(long yeuCauTiepNhanId)
        {
            var gridDataSource = new GridDataSource { Data = (_thuNganNoiTruService.GetDanhSachDichVuTrongGoiCoBHYTChuaQuyetToanNoiTru(yeuCauTiepNhanId)).ToArray() };
            return Ok(gridDataSource);
        }

        [HttpPut("QuyetToanDichVuTrongGoiCoBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult> QuyetToanDichVuTrongGoiCoBHYT(QuyetToanDichVuTrongGoiVo chiTienQuyetToan)
        {
            return Ok(await _thuNganNoiTruService.QuyetToanDichVuTrongGoiCoBHYTNoiTru(chiTienQuyetToan));
        }


        [HttpGet("KiemTraYeuCauTiepNhanCoKhuyenMai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public ActionResult<bool> KiemTraYeuCauTiepNhanCoKhuyenMai(long yeuCauTiepNhanId)
        {
            return Ok(_thuNganNoiTruService.KiemTraYeuCauTiepNhanCoKhuyenMai(yeuCauTiepNhanId));
        }

        [HttpGet("GetDataDichVuKhuyenMaiBenhNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataDichVuKhuyenMaiBenhNhanForGrid(long yeuCauTiepNhanId)
        {
            var gridDataSource = new GridDataSource { Data = (await _thuNganNoiTruService.GetDanhSachDichVuKhuyenMaiForGrid(yeuCauTiepNhanId)).ToArray() };
            return Ok(gridDataSource);
        }

        [HttpPut("ApDungDichVuKhuyenMai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public ActionResult ApDungDichVuKhuyenMai(ApDungKhuyenMaiBenhNhanNoiTru ApDungKhuyenMaiBenhNhan)
        {
            _thuNganNoiTruService.ApDungDichVuKhuyenMai(ApDungKhuyenMaiBenhNhan);
            return Ok();
        }


        #endregion

        [HttpPost("GetDataNoiTruForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataNoiTruForGridAsync([FromBody] ThuNganNoiTruQueryInfo queryInfo)
        {

            var queryString = JsonConvert.DeserializeObject<ThuNganNoiTruQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChoQuyetToan = queryString.ChoQuyetToan;
            queryInfo.ChoTamUngThem = queryString.ChoTamUngThem;
            queryInfo.DaTamUng = queryString.DaTamUng;
            queryInfo.ChuaTaoBenhAn = queryString.ChuaTaoBenhAn;

            var gridData = await _thuNganNoiTruService.GetDanhSachChuaQuyetToanNoiTruAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalNoiTruPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetTotalNoiTruPageForGridAsync([FromBody] ThuNganNoiTruQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganNoiTruQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChoQuyetToan = queryString.ChoQuyetToan;
            queryInfo.ChoTamUngThem = queryString.ChoTamUngThem;
            queryInfo.DaTamUng = queryString.DaTamUng;
            queryInfo.ChuaTaoBenhAn = queryString.ChuaTaoBenhAn;

            var gridData = await _thuNganNoiTruService.GetTotalPageDanhSachChuaQuyetToanNoiTruAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataNoiTruDaQuyetToanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataNoiTruDaQuyetToanForGridAsync([FromBody] ThuNganNoiTruDaQuyetToanQueryInfo queryInfo)
        {

            var queryString = JsonConvert.DeserializeObject<ThuNganNoiTruDaQuyetToanQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.CongNo = queryString.CongNo;
            queryInfo.HoanTien = queryString.HoanTien;
            queryInfo.DaQuyetToan = queryString.DaQuyetToan;

            var gridData = await _thuNganNoiTruService.GetDanhSachDaQuyetToanNoiTruAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalNoiTruDaQuyetToanPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetTotalNoiTruDaQuyetToanPageForGridAsync([FromBody] ThuNganNoiTruDaQuyetToanQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganNoiTruDaQuyetToanQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.CongNo = queryString.CongNo;
            queryInfo.HoanTien = queryString.HoanTien;
            queryInfo.DaQuyetToan = queryString.DaQuyetToan;

            var gridData = await _thuNganNoiTruService.GetTotalPageDanhSachDaQuyetToanNoiTruAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("DanhSachThuPhiDichVuNoiTru/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<ChiPhiKhamChuaBenhNoiTruVo>>> DanhSachThuPhiDichVuNoiTru(long id)
        {
            var model = await _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(id);
            return Ok(model);
        }

        [HttpPost("GetSoPhieu/{yeuCauTiepNhanId}")]
        public async Task<ActionResult<ICollection<ThongTinPhieuThuVo>>> GetSoPhieu([FromBody]DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var lookup = await _thuNganNoiTruService.GetSoPhieu(queryInfo, yeuCauTiepNhanId);
            return Ok(lookup);
        }

        [HttpGet("GetThongTinPhieuThu/{soPhieu}/{loaiPhieu}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ThongTinPhieuThu> GetThongTinPhieuThu(long soPhieu, LoaiPhieuThuChiThuNgan loaiPhieu)
        {
            var model = _thuNganNoiTruService.GetThongTinPhieuThu(soPhieu, loaiPhieu);
            return Ok(model);
        }

        [HttpGet("DanhSachDaThuTienNoiTru/{id}/{soPhieuThu}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<ChiPhiKhamChuaBenhNoiTruVo>>> DanhSachDaThuTienNoiTru(long id)
        {
            var model = await _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(id);
            return Ok(model);
        }

        [HttpGet("KiemTraCoPhieuThuCongNo/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<bool>> KiemTraCoPhieuThuCongNo(long id)
        {
            var result = await _thuNganNoiTruService.KiemTraConPhieuThuCongNo(id);
            return Ok(result);
        }

        [HttpGet("SoDuTaiKhoanTamUngBenhNhan/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<decimal>> SoDuTaiKhoanTamUngBenhNhan(long id)
        {
            var soTienDaTamUng = await _thuNganNoiTruService.GetSoTienDaTamUngAsync(id);
            return Ok(soTienDaTamUng);
        }

        [HttpPost("ThuTienTamUng")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThuTienTamUng(ThuPhiTamUngNoiTruVo model)
        {
            var result = await _thuNganNoiTruService.ThuTienTamUngNoiTru(model);
            if (string.IsNullOrEmpty(result.Item2)) return Ok(result.Item1);
            throw new ApiException(result.Item2);
        }

        [HttpPost("LuuTamChiPhiNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> LuuTamChiPhiNoiTru(ThuPhiKhamChuaBenhNoiTruVo model)
        {
            await _thuNganNoiTruService.LuuTamChiPhiNoiTru(model);
            return Ok();
        }

        [HttpGet("KiemTraSuDungGoi/{yeuCauTiepNhanId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> KiemTraSuDungGoi(long yeuCauTiepNhanId)
        {
            var result = _thuNganNoiTruService.KiemTraSuDungGoi(yeuCauTiepNhanId);
            return Ok(result);
        }

        [HttpPost("ThuTienPhiBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThuTienPhiBenhNhan(ThuPhiKhamChuaBenhNoiTruVo model)
        {
            //update BVHD-3584 luôn thu kèm trong gói
            var result = await _thuNganNoiTruService.ThuPhiNoiTruVaQuyetToanDichVuTrongGoi(model);
            //var result = await _thuNganNoiTruService.ThuPhiNoiTru(model);
            if (string.IsNullOrEmpty(result.Error)) return Ok(result);
            throw new ApiException(result.Error);
        }

        [HttpPost("HuyThanhToan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult HuyThanhToan(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            thongTinHuyPhieuVo.LoaiPhieuThuChiThuNgan = huyPhieuViewModel.LoaiPhieuThuChiThuNgan;
            _thuNganNoiTruService.HuyPhieu(thongTinHuyPhieuVo);

            return Ok();
        }

        [HttpPost("HoanPhieuThu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult HoanPhieuThu(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            thongTinHuyPhieuVo.LoaiPhieuThuChiThuNgan = huyPhieuViewModel.LoaiPhieuThuChiThuNgan;
            _thuNganNoiTruService.HuyPhieu(thongTinHuyPhieuVo);
            return Ok();
        }

        [HttpPost("InPhieuThu")]
        public ActionResult<List<HtmlToPdfVo>> InPhieuThu(long taiKhoanThuId, long taiKhoanChiId, string hostingName, string loaiTypes, string phieuHoanUngIds)
        {
            var phieuThuNgans = new List<HtmlToPdfVo>();
            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            if (!string.IsNullOrEmpty(loaiTypes))
            {
                var loais = loaiTypes.Split(",");
                for (var i = 0; i < loais.Length; i++)
                {
                    if (loais[i] == InPhieuThuChiStr && taiKhoanThuId != 0)
                    {
                        var htmlContent = _thuNganNoiTruService.GetHtmlPhieuThuChiNoiTru(taiKhoanThuId, hostingName) + "</body></html><div class=\"pagebreak\"></div>";
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = htmlContent,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }
                    if (loais[i] == InPhieuHoanUngStr && taiKhoanChiId != 0)
                    {
                        var phieuHoanUng = _thuNganNoiTruService.GetHtmlPhieuHoanUngNoiTru(taiKhoanChiId, hostingName) + "<div class=\"pagebreak\"></div>";
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = phieuHoanUng,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }
                    if (loais[i] == InPhieuHoanThuStr && taiKhoanChiId != 0)
                    {
                        var phieuHoanThuNoiTru = _thuNganNoiTruService.GetHtmlPhieuHoanThuNoiTru(taiKhoanChiId, hostingName) + "<div class=\"pagebreak\"></div>";
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = phieuHoanThuNoiTru,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }
                    if (loais[i] == BangKeThuocStr && taiKhoanThuId != 0)
                    {
                        var phieuThuoc = _yeuCauTiepNhanService.GetHtmlBaoCaoToaThuoc(taiKhoanThuId, hostingName) + "<div class=\"pagebreak\"></div>";
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = phieuThuoc,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }
                    TaiKhoanBenhNhanThu taiKhoanBenhNhanThu = null;
                    List<ChiPhiKhamChuaBenhNoiTruVo> tatCaDichVuKhamChuaBenh = null;
                    if (loais[i] == InCPKhamNoiTruStr && taiKhoanThuId != 0)
                    {
                        var htmlCoBHYT = _thuNganNoiTruService.GetHtmlBangKeCoBHYT(taiKhoanThuId, hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                        if (!string.IsNullOrEmpty(htmlCoBHYT))
                        {
                            var bangKeCoBHYT = htmlCoBHYT + "<div class=\"pagebreak\"></div>";
                            var phieuBHYT = new HtmlToPdfVo()
                            {
                                Html = bangKeCoBHYT,
                                FooterHtml = footerHtml,
                                Bottom = 15,
                            };
                            phieuThuNgans.Add(phieuBHYT);
                        }

                        var bangKeKhamBenh = _thuNganNoiTruService.GetHtmlBangKe(taiKhoanThuId, hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                        if (!string.IsNullOrEmpty(bangKeKhamBenh))
                        {
                            var phieuThuNgan = new HtmlToPdfVo()
                            {
                                Html = bangKeKhamBenh,
                                FooterHtml = footerHtml,
                                Bottom = 15,
                            };
                            phieuThuNgans.Add(phieuThuNgan);
                        }
                    }

                    if (loais[i] == InBangKeChiPhiGoiKhamChuaBenhStr && taiKhoanThuId != 0)
                    {
                        var bangKeChiPhiGoiKhamChuaBenh = _thuNganNoiTruService.GetHtmlBangKeTrongGoiDv(taiKhoanThuId, hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                        if (!string.IsNullOrEmpty(bangKeChiPhiGoiKhamChuaBenh))
                        {
                            var phieuThuNgan = new HtmlToPdfVo()
                            {
                                Html = bangKeChiPhiGoiKhamChuaBenh,
                                FooterHtml = footerHtml,
                                Bottom = 15,
                            };

                            phieuThuNgans.Add(phieuThuNgan);
                        }
                    }

                    if (loais[i] == InPhieuThuTamUngStr && taiKhoanThuId != 0)
                    {
                        var phieuThuTamUng = _yeuCauTiepNhanService.GetHtmlPhieuThuTamUng(taiKhoanThuId, hostingName);
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = phieuThuTamUng,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }

                }
            }

            if (!string.IsNullOrEmpty(phieuHoanUngIds))
            {
                var hoanUngIds = phieuHoanUngIds.Split(",");
                foreach (var hoanUngId in hoanUngIds)
                {
                    if (!string.IsNullOrEmpty(hoanUngId))
                    {
                        var phieuHoanUng = _yeuCauTiepNhanService.GetHtmlPhieuHoanUngNgoaiTru(long.Parse(hoanUngId), hostingName) + "<div class=\"pagebreak\"></div>";
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = phieuHoanUng,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }
                }
            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(phieuThuNgans);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopPhieu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("GetFilePDFFromHtml")]
        public ActionResult GetFilePDFFromHtml(PhieuThuNgan htmlContent)
        {
            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = htmlContent.Html,
                FooterHtml = footerHtml,
                PageSize = htmlContent.PageSize,
                PageOrientation = htmlContent.PageOrientation,
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");

        }

        [HttpPost("CapnhatNguoiThuHoiPhieuThu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult CapnhatNguoiThuHoiPhieuThu(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            _thuNganNoiTruService.CapnhatNguoiThuHoiPhieuThu(thongTinHuyPhieuVo);
            return Ok();
        }

        [HttpPost("HoanThuTheoDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> HoanThuTheoDichVu(ChiPhiKhamChuaBenhNoiTruVo chiPhiKhamChuaBenhNoiTruVo)
        {
            var result = await _thuNganNoiTruService.HoanThuDichVu(chiPhiKhamChuaBenhNoiTruVo);
            if (string.IsNullOrEmpty(result.Item2)) return Ok(result.Item1);
            throw new ApiException(result.Item2);
        }
        
        [HttpPost("ThuPhiNoiTruVaQuyetToanDichVuTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThuPhiNoiTruVaQuyetToanDichVuTrongGoi(ThuPhiKhamChuaBenhNoiTruVo model)
        {
            var result = await _thuNganNoiTruService.ThuPhiNoiTruVaQuyetToanDichVuTrongGoi(model);
            if (string.IsNullOrEmpty(result.Error)) return Ok(result);
            throw new ApiException(result.Error);
        }

        [HttpPost("XemNhieuPhieuThu")]
        public ActionResult<List<HtmlToPdfVo>> XemNhieuPhieuThu(string hostingName, string taiKhoanThuIds, string phieuHoanUngIds, string loaiTypes)
        {

            var phieuThuNgans = new List<HtmlToPdfVo>();
            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            if (!string.IsNullOrEmpty(taiKhoanThuIds))
            {
                var tkThuIds = taiKhoanThuIds.Split(",");
                foreach (var tkThuId in tkThuIds)
                {
                    if (!string.IsNullOrEmpty(tkThuId))
                    {
                        if (!string.IsNullOrEmpty(loaiTypes))
                        {
                            var loaiPhieuMuonIns = loaiTypes.Split(",");
                            if (loaiPhieuMuonIns.Length > 0)
                            {
                                for (var i = 0; i < loaiPhieuMuonIns.Length; i++)
                                {
                                    if (loaiPhieuMuonIns[i] == InPhieuThuChiStr)
                                    {
                                        var htmlContent = _yeuCauTiepNhanService.GetHtmlPhieuThuChiNgoaiTru(long.Parse(tkThuId), hostingName);
                                        var phieuThuNgan = new HtmlToPdfVo()
                                        {
                                            Html = htmlContent,
                                            FooterHtml = footerHtml,
                                            Bottom = 15,
                                            PageOrientation = PageOrientationLandscape,
                                            PageSize = PageSizeA5
                                        };
                                        phieuThuNgans.Add(phieuThuNgan);
                                    }

                                    if (loaiPhieuMuonIns[i] == InCPKhamNoiTruStr)
                                    {
                                        TaiKhoanBenhNhanThu taiKhoanBenhNhanThu = null;
                                        List<ChiPhiKhamChuaBenhNoiTruVo> tatCaDichVuKhamChuaBenh = null;
                                        var bangKeChiPhiGoiKhamChuaBenh = _thuNganNoiTruService.GetHtmlBangKeTrongGoiDv(long.Parse(tkThuId), hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                                        if (!string.IsNullOrEmpty(bangKeChiPhiGoiKhamChuaBenh))
                                        {
                                            var phieuBangKeChiPhiGoiKhamChuaBenh = new HtmlToPdfVo()
                                            {
                                                Html = bangKeChiPhiGoiKhamChuaBenh,
                                                FooterHtml = footerHtml,
                                                Bottom = 15,
                                            };

                                            phieuThuNgans.Add(phieuBangKeChiPhiGoiKhamChuaBenh);
                                        }

                                        var htmlCoBHYT = _thuNganNoiTruService.GetHtmlBangKeCoBHYT(long.Parse(tkThuId), hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                                        if (!string.IsNullOrEmpty(htmlCoBHYT))
                                        {
                                            var phieuBHYT = new HtmlToPdfVo()
                                            {
                                                Html = htmlCoBHYT,
                                                FooterHtml = footerHtml,
                                                Bottom = 15
                                            };
                                            phieuThuNgans.Add(phieuBHYT);
                                        }

                                        var bangKeKhamBenh = _thuNganNoiTruService.GetHtmlBangKe(long.Parse(tkThuId), hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                                        if (!string.IsNullOrEmpty(bangKeKhamBenh))
                                        {
                                            var phieuBangKeKhongCoBHYT = new HtmlToPdfVo()
                                            {
                                                Html = bangKeKhamBenh,
                                                FooterHtml = footerHtml,
                                                Bottom = 15
                                            };
                                            phieuThuNgans.Add(phieuBangKeKhongCoBHYT);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(phieuHoanUngIds))
            {
                var hoanUngIds = phieuHoanUngIds.Split(",");
                foreach (var hoanUngId in hoanUngIds)
                {
                    if (!string.IsNullOrEmpty(hoanUngId))
                    {
                        var phieuHoanUng = _yeuCauTiepNhanService.GetHtmlPhieuHoanUngNgoaiTru(long.Parse(hoanUngId), hostingName);
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = phieuHoanUng,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }
                }
            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(phieuThuNgans);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopPhieu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("InBangKeNoiTruChoThu")]
        public ActionResult<List<HtmlToPdfVo>> InBangKeNoiTruChoThu(ThuPhiKhamChuaBenhNoiTruVo model)
        {
            var htmlToPdfVos = new List<HtmlToPdfVo>();
            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            var hostingName = string.Empty;
            var htmlContent = _thuNganNoiTruService.GetHtmlBangKeNoiTruChoThu(model, hostingName, out var danhSachTatCaChiPhi);
            if (!string.IsNullOrEmpty(htmlContent))
            {
                htmlToPdfVos.Add(new HtmlToPdfVo()
                {
                    Html = htmlContent,
                    FooterHtml = footerHtml,
                    Bottom = 15,
                });
            }
            if(danhSachTatCaChiPhi != null && danhSachTatCaChiPhi.Any(o=>o.YeuCauGoiDichVuId != null))
            {
                var bangKeNgoaiGoiChuaQT = _thuNganNoiTruService.GetHtmlBangKeNgoaiGoiChuaQuyetToan(model.Id, hostingName, danhSachTatCaChiPhi);
                if (!string.IsNullOrEmpty(bangKeNgoaiGoiChuaQT))
                {
                    htmlToPdfVos.Add(new HtmlToPdfVo()
                    {
                        Html = bangKeNgoaiGoiChuaQT,
                        FooterHtml = footerHtml,
                        Bottom = 15,
                    });
                }
            }
            

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(htmlToPdfVos);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeNoiTruChoThu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("InBangKeNoiTruTrongGoiChoThu")]
        public ActionResult<List<HtmlToPdfVo>> InBangKeNoiTruTrongGoiChoThu(QuyetToanDichVuTrongGoiVo model)
        {
            var htmlToPdfVos = new List<HtmlToPdfVo>();
            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            var hostingName = string.Empty;
            var htmlContent = _thuNganNoiTruService.GetHtmlBangKeNoiTruChoThuTrongGoi(model, hostingName);

            htmlToPdfVos.Add(new HtmlToPdfVo()
            {
                Html = htmlContent,
                FooterHtml = footerHtml,
                Bottom = 15,
            });

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(htmlToPdfVos);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeNoiTruChoThu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }


        #region Static Value

        private readonly string InPhieuThuChiStr = "InPhieuThu";
        private readonly string InPhieuHoanUngStr = "InPhieuHoanUng";
        private readonly string InPhieuHoanThuStr = "InPhieuHoanThu";
        private readonly string BangKeThuocStr = "BangKeThuoc";
        private readonly string InBangKeChiPhiGoiKhamChuaBenhStr = "InBangKeChiPhiGoiKhamChuaBenhStr";
        private readonly string InCPKhamNoiTruStr = "InCPKhamNoiTru";
        private readonly string InPhieuThuTamUngStr = "InPhieuThuTamUng";
        private readonly string PageSizeA5 = "A5";
        private readonly string PageOrientationLandscape = "Landscape";

        #endregion
    }
}
