using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Services.LichSuKhamChuaBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LichSuKhamChuaBenhs;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.DuyetKetQuaXetNghiems;
using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Api.Models.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Helpers;
using Camino.Services.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Domain.ValueObject.PhieuKhaiThacTienSuDiUng;
using Camino.Core.Domain.ValueObject.PhieuSoKet15NgayDieuTri;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Data;

namespace Camino.Api.Controllers
{
    public class LichSuKhamChuaBenhController : CaminoBaseController
    {
        private readonly ILichSuKhamChuaBenhService _lichSuKhamChuaBenhService;
        private readonly IPdfService _pdfService;
        private readonly ILocalizationService _localizationService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IDuyetKetQuaXetNghiemService _duyetKetQuaXetNghiemService;
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;
        private readonly IPhauThuatThuThuatService _phauThuatThuThuatService;
        private readonly INoiTruHoSoKhacService _noiTruHoSoKhacService;
        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTiet;
        public LichSuKhamChuaBenhController(
            ILichSuKhamChuaBenhService lichSuKhamChuaBenhService
            , IPdfService pdfService
            , ILocalizationService localizationService
            , IYeuCauKhamBenhService yeuCauKhamBenhService
            , IYeuCauTiepNhanService yeuCauTiepNhanService
            , IDuyetKetQuaXetNghiemService duyetKetQuaXetNghiemService
            , IDieuTriNoiTruService dieuTriNoiTruService
            , IDanhSachChoKhamService danhSachChoKhamService
            , IPhauThuatThuThuatService phauThuatThuThuatService
            , INoiTruHoSoKhacService noiTruHoSoKhacService
            , IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTiet
            )
        {
            _lichSuKhamChuaBenhService = lichSuKhamChuaBenhService;
            _pdfService = pdfService;
            _localizationService = localizationService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _duyetKetQuaXetNghiemService = duyetKetQuaXetNghiemService;
            _dieuTriNoiTruService = dieuTriNoiTruService;
            _danhSachChoKhamService = danhSachChoKhamService;
            _phauThuatThuThuatService = phauThuatThuThuatService;
            _noiTruHoSoKhacService = noiTruHoSoKhacService;
            _phienXetNghiemChiTiet = phienXetNghiemChiTiet;
        }

        #region grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridTimKiemNguoiBenhDaTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuKhamChuaBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridTimKiemNguoiBenhDaTiepNhanAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _lichSuKhamChuaBenhService.GetDataForGridTimKiemNguoiBenhDaTiepNhanAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridTimKiemNguoiBenhDaTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuKhamChuaBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridTimKiemNguoiBenhDaTiepNhanAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _lichSuKhamChuaBenhService.GetTotalPageForGridTimKiemNguoiBenhDaTiepNhanAsync(queryInfo);
            }

            return Ok(gridData);
        }

        [HttpPost("TimKiemNguoiBenhDaTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuKhamChuaBenh)]
        public async Task<ActionResult<GridDataSource>> TimKiemNguoiBenhDaTiepNhanAsync(LichSuKhamChuaBenhTimKiemVo timKiemObj)
        {
            var gridData = new GridDataSource();
            var queryInfo = new QueryInfo()
            {
                AdditionalSearchString = JsonConvert.SerializeObject(timKiemObj),
                Take = 50
            };
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _lichSuKhamChuaBenhService.GetDataForGridTimKiemNguoiBenhDaTiepNhanAsync(queryInfo);
            }

            return Ok(gridData);
        }
        #endregion

        #region get data
        [HttpGet("GetLichSuKhamChuaBenhTheoNguoiBenh")]
        public async Task<ActionResult<LichSuKhamChuaBenhTheoNguoiBenhVo>> GetLichSuKhamChuaBenhTheoNguoiBenhAsnc(long nguoiBenhId)
        {
            var result = await _lichSuKhamChuaBenhService.GetLichSuKhamChuaBenhTheoNguoiBenhAsnc(nguoiBenhId);
            return result;
        }

        [HttpPost("GetFilePDFLichSuKhamChuaBenhFromHtml")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuKhamChuaBenh)]
        public async Task<ActionResult> GetFilePDFLichSuKhamChuaBenhFromHtmlAsync(PhieuInLichSuKhamQueryInfo queryInfo)
        {
            var list = new List<HtmlToPdfVo>();

            #region footer default
            var footerHtmlDefault = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script charset='utf-8'>
                    function hostfunction() {
                        replaceParams();
                        timeNow();
                    }
                    function replaceParams() {
                      var url = window.location.href
                        .replace(/#$/, '');
                      var params = (url.split('?')[1] || '').split('&');
                      for (var i = 0; i < params.length; i++) {
                          var param = params[i].split('=');
                          var key = param[0];
                          var value = param[1] || '';
                          var regex = new RegExp('{' + key + '}', 'g');
                          document.getElementById('total').innerHTML = document.body.innerText.replace(regex, value);
                      }
                    }
                        function timeNow() {
                                                  var today = new Date();
                                                  var day = today.getDate();
                                                  var month = today.getMonth() + 1;
                                                  var hour = today.getHours();
                                                  var minutes = today.getMinutes();
                                                   if(day < 10){
                                                        day = '0' + day;
                                                    };
                                                  if(month < 10){
                                                        month = '0' + month;
                                                    };
                                                  if(hour < 10){
                                                        hour = '0' + hour;
                                                    };
                                                  if(minutes < 10){
                                                        minutes = '0' + minutes;
                                                    };
                                                  var date = day+'/'+(month)+'/'+today.getFullYear();
                                                  var time = hour + ': ' + minutes;
                                                  document.getElementById('hvn').innerHTML = date + ' ' + time;
                                            }
                    
                    </script>
                </head>
                <body  onload='hostfunction()' >
                        <div id='hvn' style='float: left;  display: inline; width: 50%; '>
                        </div>
                        <div  id='total' style='float: left;display: inline; width: 50%; text-align: right'>
                        Trang {page}/{topage}
                        </div>  
                        <div style='clear: both; '></div>
                 </body>
                </html>";
            #endregion

            var typeSizeDefault = "A4";
            var typeLayoutDefault = "portrait";
            var typeLayoutLandscapeDefault = "landscape";
            var formatPageOpenTagDefault =
                "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSizeDefault + " " + typeLayoutDefault + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>"
                + "</head><body>";
            var formatScriptTagDefault = "<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js'></script>";
            var formatPageCloseTagDefault = "</body></html>";

            var yeuCauTiepNhan = new YeuCauTiepNhan();
            var lichSuKhams =
                await _lichSuKhamChuaBenhService.GetLichSuKhamYeuCauTiepNhanAsnc(null, queryInfo.YeuCauTiepNhanId);
            if (!lichSuKhams.Any())
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }
            else
            {
                yeuCauTiepNhan = lichSuKhams.First();
            }

            #region dịch vụ khám
            var lstYeuCauKham = yeuCauTiepNhan.YeuCauKhamBenhs.Where(a => (queryInfo.LoaiLichSuKhamChuaBenh == null
                                                                           && a.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                                                                          || (queryInfo.LoaiLichSuKhamChuaBenh == Enums.LoaiLichSuKhamChuaBenh.KhamBenh
                                                                              && queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.DichVuKhamBenh
                                                                              && a.Id == queryInfo.YeuCauDichVuId)).ToList();
            if (lstYeuCauKham.Any())
            {
                foreach (var yeuCauKhamBenh in lstYeuCauKham)
                {
                    var phieuKhamBenhVo = new PhieuKhamBenhVo()
                    {
                        YeuCauKhamBenhId = yeuCauKhamBenh.Id,
                        CoKhamBenh = true,
                        CoHeader = false,
                        CoKhamBenhVaoVien = yeuCauKhamBenh.CoNhapVien ?? false
                    };
                    var htmls = _yeuCauKhamBenhService.InPhieuKhamBenh(phieuKhamBenhVo);

                    foreach (var html in htmls)
                    {
                        var htmlToPdfVo = new HtmlToPdfVo
                        {
                            Html = html,
                            FooterHtml = footerHtmlDefault,
                            //Bottom = 15
                        };
                        list.Add(htmlToPdfVo);
                    }
                }
            }
            #endregion

            #region Đơn thuốc/đơn vật tư khám ngoại trú
            var lstYeuCauKhamCoKeToa = yeuCauTiepNhan.YeuCauKhamBenhs.Where(a => ((queryInfo.LoaiLichSuKhamChuaBenh == null
                                                                                     && a.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                                                                                  || (queryInfo.LoaiLichSuKhamChuaBenh == Enums.LoaiLichSuKhamChuaBenh.KhamBenh
                                                                                      && (queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.DonThuoc
                                                                                          || queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.DonVatTu)
                                                                                      && a.Id == queryInfo.YeuCauDichVuId))
                                                                               && a.CoKeToa == true).ToList();
            foreach (var yeuCauKhamBenh in lstYeuCauKhamCoKeToa)
            {
                if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any() && (queryInfo.YeuCauDichVuId == null || queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.DonThuoc))
                {
                    var inToaThuoc = new InToaThuocReOrder()
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        Header = false,
                        HostingName = queryInfo.Hosting,
                        YeuCauKhamBenhId = yeuCauKhamBenh.Id
                    };
                    var resultHtmlDonThuoc = _yeuCauKhamBenhService.InDonThuocKhamBenh(inToaThuoc);
                    var lstHTML = resultHtmlDonThuoc.Split("<div class=\"pagebreak\"> </div>");
                    foreach (var html in lstHTML)
                    {
                        if (!string.IsNullOrEmpty(html))
                        {
                            var htmlContent = formatPageOpenTagDefault;
                            htmlContent += formatScriptTagDefault;
                            htmlContent += html;
                            htmlContent += formatPageCloseTagDefault;
                            var htmlToPdfVo = new HtmlToPdfVo
                            {
                                Html = htmlContent,
                                FooterHtml = string.Empty,
                                Bottom = 15
                            };
                            list.Add(htmlToPdfVo);
                        }
                    }
                }

                if (yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any() && (queryInfo.YeuCauDichVuId == null || queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.DonVatTu))
                {
                    var inToaVatTu = new InVatTuReOrder()
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        Header = false,
                        HostingName = queryInfo.Hosting,
                        YeuCauKhamBenhId = yeuCauKhamBenh.Id
                    };
                    var resultHtmlVatTu = _yeuCauKhamBenhService.InVatTuKhamBenh(inToaVatTu);
                    var lstHTML = resultHtmlVatTu.Split("<div class=\"pagebreak\"> </div>");
                    foreach (var html in lstHTML)
                    {
                        if (!string.IsNullOrEmpty(html))
                        {
                            var htmlContent = formatPageOpenTagDefault;
                            htmlContent += formatScriptTagDefault;
                            htmlContent += html;
                            htmlContent += formatPageCloseTagDefault;
                            var htmlToPdfVo = new HtmlToPdfVo
                            {
                                Html = htmlContent,
                                FooterHtml = string.Empty,
                                Bottom = 15
                            };
                            list.Add(htmlToPdfVo);
                        }
                    }
                }
            }
            #endregion

            #region dịch vụ cls
            var lstYeuCauCLS = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(a => ((queryInfo.LoaiLichSuKhamChuaBenh == null && a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                                               || (queryInfo.LoaiLichSuKhamChuaBenh != null && a.Id == queryInfo.YeuCauDichVuId))
                                                                              && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                                                  || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                                                  || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)).ToList();

            var lstYeuCauKhamNoiTru = yeuCauTiepNhan.YeuCauKhamBenhs.Where(a => a.YeuCauNhapViens.Any(b => b.YeuCauTiepNhans.Any(c => c.YeuCauDichVuKyThuats.Any(d => (d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                                                                                                                                       || d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                                                                                                                                       || d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))))).ToList();
            foreach (var yeuCauKhamBenh in lstYeuCauKhamNoiTru)
            {
                if (yeuCauKhamBenh.YeuCauNhapViens.Any())
                {
                    lstYeuCauCLS.AddRange(yeuCauKhamBenh.YeuCauNhapViens
                        .SelectMany(x => x.YeuCauTiepNhans)
                        .SelectMany(x => x.YeuCauDichVuKyThuats)
                        .Where(a => ((queryInfo.LoaiLichSuKhamChuaBenh == null && a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                     || (queryInfo.LoaiLichSuKhamChuaBenh != null && a.Id == queryInfo.YeuCauDichVuId))
                                    && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                        || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                        || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                        .ToList());
                }
            }

            // CDHA - TDCN
            var dichVuCLSs = lstYeuCauCLS.Where(a => a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                     || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang).ToList();
            foreach (var dichVuCLS in dichVuCLSs)
            {
                var phieuInKetQuaInfoVo = new PhieuInKetQuaInfoVo()
                {
                    HasHeader = false,
                    HostingName = queryInfo.Hosting,
                    Id = dichVuCLS.Id
                };
                var phieuInCLSHtml = _yeuCauTiepNhanService.XuLyInPhieuKetQuaAsync(phieuInKetQuaInfoVo);
                var htmlToPdfVo = new HtmlToPdfVo
                {
                    Html = phieuInCLSHtml,
                    FooterHtml = footerHtmlDefault,
                    Bottom = 15
                };
                list.Add(htmlToPdfVo);
            }

            // dịch vụ xét nghiệm
            var dichVuXetNghiems = lstYeuCauCLS.Where(d => d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).ToList();
            if (dichVuXetNghiems.Any())
            {
                var lstPhieuIn = new List<PhieuInXetNghiemModel>();
                var listPhienId = dichVuXetNghiems.SelectMany(x => x.PhienXetNghiemChiTiets).Select(x => x.PhienXetNghiemId).Distinct().ToList();
                
                foreach (var phienXetNghiemId in listPhienId)
                {
                    var listPhienChiTietIds = new List<DuyetKqXetNghiemChiTietModel>();

                    //var getPhien = await GetPhien(phienXetNghiemId);
                    //foreach (var chiTietKetQuaXetNghiem in getPhien.ChiTietKetQuaXetNghiems.ToList())
                    //{
                    //    if (dichVuXetNghiems.Any(d => d.Id == chiTietKetQuaXetNghiem.YeuCauDichVuKyThuatId))
                    //    {
                    //        var obj = new DuyetKqXetNghiemChiTietModel()
                    //        {
                    //            Id = chiTietKetQuaXetNghiem.Id,
                    //            YeuCauDichVuKyThuatId = chiTietKetQuaXetNghiem.YeuCauDichVuKyThuatId
                    //        };
                    //        listPhienChiTietIds.Add(obj);
                    //    }
                    //}

                    var yeuCauDichVuKyThuatIds = dichVuXetNghiems.Select(o => o.Id).ToList();

                    var phienXetNghiemChiTiets = _phienXetNghiemChiTiet.TableNoTracking
                        .Where(o => o.PhienXetNghiemId == phienXetNghiemId && yeuCauDichVuKyThuatIds.Contains(o.YeuCauDichVuKyThuatId))
                        .Select(o => new
                        {
                            o.YeuCauDichVuKyThuatId,
                            KetQuaXetNghiemChiTietIds = o.KetQuaXetNghiemChiTiets.Select(kq => kq.Id).ToList()
                        }).ToList();

                    foreach (var phienXetNghiemChiTiet in phienXetNghiemChiTiets)
                    {
                        listPhienChiTietIds.AddRange(phienXetNghiemChiTiet.KetQuaXetNghiemChiTietIds.Select(kq => new DuyetKqXetNghiemChiTietModel { Id = kq, YeuCauDichVuKyThuatId = phienXetNghiemChiTiet.YeuCauDichVuKyThuatId }));
                    }

                    DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
                    {
                        Id = phienXetNghiemId,
                        HostingName = queryInfo.Hosting,
                        Header = false,
                        ListIn = listPhienChiTietIds.Distinct().ToList(),
                        LoaiIn = 3 // mặc định
                    };

                    var phieuInItems = await _duyetKetQuaXetNghiemService.InDuyetKetQuaXetNghiemManHinhDuyet(ketQuaVo);
                    lstPhieuIn.AddRange(phieuInItems);
                }

                var lstHtml = new List<string>();
                var typeSize = "A4";
                var typeLayout = "portrait";

                var i = 0;
                foreach (var phieuInXN in lstPhieuIn)
                {
                    var htmlContent = "";
                    htmlContent +=
                        "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                    htmlContent += "</head><body>";
                    htmlContent += phieuInXN.Html;
                    i++;
                    htmlContent += "</body></html>";
                    lstHtml.Add(htmlContent);
                }

                var footerHtml = string.Empty;
                string ngayGioHienTai = DateTime.Now.ApplyFormatDateTimeSACH();
                string classing = "luv";
                var lstHtmlXetNghiem = new List<HtmlToPdfVo>();

                foreach (var itemHTML in lstHtml)
                {
                    footerHtml = @"<!DOCTYPE html>
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
                        <body onload='replaceParams()'> " + classing +
                            ngayGioHienTai + classing
                                  + "Trang {page}/{topage}</body></html>";

                    var htmlToPdfVo = new HtmlToPdfVo
                    {
                        Html = itemHTML,
                        FooterHtml = footerHtml,
                        Bottom = 7
                    };
                    lstHtmlXetNghiem.Add(htmlToPdfVo);
                }

                foreach (var item in lstHtmlXetNghiem)
                {
                    if (!string.IsNullOrEmpty(item.FooterHtml))
                    {
                        var kyTu = item.FooterHtml.Split(classing);
                        if (kyTu.Length > 2)
                        {
                            item.FooterHtml = kyTu[0] + footerPhieuIn(kyTu[1], kyTu[2]);
                        }
                    }
                }
                list.AddRange(lstHtmlXetNghiem);
            }

            #endregion

            #region nội trú
            if (queryInfo.HienLichSuNoiTru)
            {
                var yeuCauTiepNhanNoiTru = yeuCauTiepNhan.YeuCauKhamBenhs
                    .Where(x => x.YeuCauNhapViens.Any(a => a.YeuCauTiepNhans.Any(c => c.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                                                                      && c.NoiTruBenhAn != null
                                                                                      && (c.NoiTruBenhAn.NoiTruPhieuDieuTris.Any()
                                                                                          || c.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(d => d.YeuCauDichVuKyThuatId == null
                                                                                                                                                     && d.YeuCauTruyenMauId == null
                                                                                                                                                     && d.YeuCauVatTuBenhVienId == null
                                                                                                                                                     && d.NoiTruChiDinhDuocPhamId == null)
                                                                                          || c.NoiTruHoSoKhacs.Any()))))
                    .SelectMany(x => x.YeuCauNhapViens)
                    .SelectMany(x => x.YeuCauTiepNhans)
                    .FirstOrDefault(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);


                if (yeuCauTiepNhanNoiTru != null)
                {
                    #region Phiếu điều trị
                    if (yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.Any() 
                        && (queryInfo.LoaiLichSuKhamChuaBenhChiTiet == null || queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.PhieuDieuTri))
                    {
                        //var entity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanNoiTru.Id
                        //    , s => s.Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris)
                        //        .ThenInclude(p => p.ChanDoanChinhICD)
                        //        .ThenInclude(p => p.NoiTruPhieuDieuTris)
                        //        .ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
                        //        .Include(u => u.BenhNhan).ThenInclude(p => p.NgheNghiep)

                        //        .Include(u => u.NgheNghiep)
                        //        .Include(u => u.DanToc)
                        //        .Include(u => u.QuocTich)
                        //        .Include(u => u.PhuongXa)
                        //        .Include(u => u.QuanHuyen)
                        //        .Include(u => u.TinhThanh)
                        //        .Include(u => u.HinhThucDen)
                        //        .Include(u => u.YeuCauNhapVien)
                        //        .Include(u => u.NguoiLienHePhuongXa)
                        //        .Include(u => u.NguoiLienHeQuanHuyen)
                        //        .Include(u => u.NguoiLienHeTinhThanh)
                        //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris)
                        //        .ThenInclude(p => p.YeuCauDichVuKyThuats)
                        //        .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh)
                        //        .ThenInclude(p => p.PhongBenhVien)
                        //        .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                        //        .ThenInclude(p => p.DichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhViens)
                        //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.ChuyenDenBenhVien)
                        //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris)
                        //        .ThenInclude(p => p.KhoaPhongChuyenDen)
                        //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NhanVienTaoBenhAn)
                        //        .ThenInclude(p => p.User)
                        //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris)
                        //        .ThenInclude(p => p.CheDoAn)
                        //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris)
                        //        .ThenInclude(p => p.BacSi).ThenInclude(p => p.User)
                        //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris)
                        //        .ThenInclude(p => p.BacSi).ThenInclude(p => p.HocHamHocVi)
                        //);

                        var lstHtml = new List<string>();
                        var content = "";
                        var typeSize = "A4";
                        var typeLayout = "portrait";

                        var contents = await _dieuTriNoiTruService.GetContentInPhieuThamKham(yeuCauTiepNhanNoiTru.Id);
                        foreach (var html in contents)
                        {
                            var htmlContent = "";
                            htmlContent +=
                                "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" +
                                typeSize + " " + typeLayout +
                                ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                            htmlContent += "</head><body>";
                            htmlContent += html;
                            htmlContent += "</body></html>";
                            lstHtml.Add(htmlContent);

                            var htmlToPdfVo = new HtmlToPdfVo
                            {
                                Html = htmlContent,
                                FooterHtml = string.Empty,
                                Bottom = 7
                            };
                            list.Add(htmlToPdfVo);
                        }

                        //if (entity != null)
                        //{
                        //    var lstPhieuDieuTri = entity.NoiTruBenhAn.NoiTruPhieuDieuTris
                        //        .OrderBy(o => o.NgayDieuTri).ToList();

                        //    foreach (var phieuDieuTri in lstPhieuDieuTri)
                        //    {
                        //        var html = await getContent(entity, phieuDieuTri, yeuCauTiepNhanNoiTru.Id);
                        //        var htmlContent = "";
                        //        htmlContent +=
                        //            "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" +
                        //            typeSize + " " + typeLayout +
                        //            ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                        //        htmlContent += "</head><body>";
                        //        htmlContent += html;
                        //        htmlContent += "</body></html>";
                        //        lstHtml.Add(htmlContent);

                        //        var htmlToPdfVo = new HtmlToPdfVo
                        //        {
                        //            Html = htmlContent,
                        //            FooterHtml = string.Empty,
                        //            Bottom = 7
                        //        };
                        //        list.Add(htmlToPdfVo);
                        //    }
                        //}
                    }
                    #endregion

                    #region Phiếu chăm sóc
                    var phieuChamSocs = yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                        .Where(d => d.YeuCauDichVuKyThuatId == null
                                    && d.YeuCauTruyenMauId == null
                                    && d.YeuCauVatTuBenhVienId == null
                                    && d.NoiTruChiDinhDuocPhamId == null)
                        .ToList();
                    if (phieuChamSocs.Any()
                        && (queryInfo.LoaiLichSuKhamChuaBenhChiTiet == null || queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.PhieuChamSoc))
                    {
                        var detail = new InPhieuChamSocVo()
                        {
                            BenhNhanId = yeuCauTiepNhan.BenhNhanId,
                            NoiTruBenhAnId = yeuCauTiepNhanNoiTru.NoiTruBenhAn.Id,
                            NgayDieuTriStr = null,
                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                            InTatCa = true,
                            KhongHienHeader = true
                        };
                        var phieuChamSoc = await _dieuTriNoiTruService.InPhieuChamSocAsyncVer2(detail);
                        var lstHTML = phieuChamSoc.Split("<div class='pagebreak'></div>");
                        foreach (var html in lstHTML)
                        {
                            if (!string.IsNullOrEmpty(html))
                            {
                                var htmlContent = formatPageOpenTagDefault;
                                htmlContent += formatScriptTagDefault;
                                htmlContent += html;
                                htmlContent += formatPageCloseTagDefault;
                                var htmlToPdfVo = new HtmlToPdfVo
                                {
                                    Html = htmlContent,
                                    FooterHtml = string.Empty,
                                    Bottom = 15
                                };
                                list.Add(htmlToPdfVo);
                            }
                        }
                    }
                    #endregion

                    #region Hồ sơ khác
                    var hoSoKhacs = yeuCauTiepNhanNoiTru.NoiTruHoSoKhacs
                        .Where(x => queryInfo.LoaiHoSoDieuTriNoiTru == null || x.LoaiHoSoDieuTriNoiTru == queryInfo.LoaiHoSoDieuTriNoiTru)
                        .ToList();
                    if (hoSoKhacs.Any()
                        && (queryInfo.LoaiLichSuKhamChuaBenhChiTiet == null || queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.HoSoKhac))
                    {
                        foreach (var hoSoKhac in hoSoKhacs)
                        {
                            var resultHTML = string.Empty;
                            var breakTag = "<div class=\"pagebreak\"></div>";
                            var typeLandscapeLayout = false;
                            if (queryInfo.NoiTruHoSoKhacId == null || queryInfo.NoiTruHoSoKhacId == hoSoKhac.Id)
                            {
                                switch (hoSoKhac.LoaiHoSoDieuTriNoiTru)
                                {

                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong: // cần cập nhật lại theo code mới
                                        var thongTin = JsonConvert.DeserializeObject<HoSoKhacPhieuSangLocDinhDuongVo>(hoSoKhac.ThongTinHoSo);
                                        if (thongTin.DungChoPhuNuMangThai)
                                        {
                                            resultHTML = await _dieuTriNoiTruService.InPhieuSangLocDinhDuongPhuSan(yeuCauTiepNhanNoiTru.Id, queryInfo.Hosting);
                                        }
                                        else
                                        {
                                            resultHTML = await _dieuTriNoiTruService.InPhieuSangLocDinhDuong(yeuCauTiepNhanNoiTru.Id, queryInfo.Hosting,thongTin.NoiTruHoSoKhacId.GetValueOrDefault());
                                        }
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuKhaiThacTienSuDiUng:
                                        var xacNhanIn = new XacNhanInTienSu()
                                        {
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting
                                        };
                                        resultHTML = await _dieuTriNoiTruService.PhieuKhaiThacTienSuBenh(xacNhanIn);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.TrichBienBanHoiChan:
                                        var xacNhanInBienBanHoiChan = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting
                                        };
                                        resultHTML = await _dieuTriNoiTruService.BienBanHoiChan(xacNhanInBienBanHoiChan);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiChucNangSong:
                                        resultHTML = await _dieuTriNoiTruService.InPhieuTheoDoiChucNangSong(yeuCauTiepNhanNoiTru.Id);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri: // có thể có nhiều phiếu
                                        var detail = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriVo>(hoSoKhac.ThongTinHoSo);
                                        var xacNhanInPhieuSoKet15NgayDieuTri = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                            TuNgay = detail.TuNgay?.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"),
                                            DenNgay = detail.DenNgay?.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"),
                                        };
                                        resultHTML = await _dieuTriNoiTruService.PhieuSoKet15NgayDieuTri(xacNhanInPhieuSoKet15NgayDieuTri);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuTomTatThongTinDieuTri:
                                        var xacNhanInPhieuTomTatThongTinDieuTri = new PhieuDieuTriVaServicesHttpParams()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInThongTinDieuTriVaCacDichVu(xacNhanInPhieuTomTatThongTinDieuTri);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat:
                                        var xacNhanInBienBanHoiChanPhauThuat = new BangTheoDoiHoiTinhHttpParamsRequest()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            IdNoiTruHoSo = hoSoKhac.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInBienBanHoiChanPhauThuat(xacNhanInBienBanHoiChanPhauThuat);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetGayMeGayTe:
                                        resultHTML = _dieuTriNoiTruService.InBienBanGayMeGayTe(hoSoKhac.Id);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetPhauThuat:
                                        var xacNhanInBienBanCamKetPhauThuat = new PhieuDieuTriVaServicesHttpParams()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInBienBanCamKetPhauThuat(xacNhanInBienBanCamKetPhauThuat);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanPhauThuat:
                                        var xacNhanInBangKiemAnToanPhauThuat = new PhieuDieuTriVaServicesHttpParams()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInBangKiemAnToanPhauThuat(xacNhanInBangKiemAnToanPhauThuat);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat:
                                        var xacNhanInBangKiemAnToanNguoiBenhPhauThuat = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                        };
                                        resultHTML = await _dieuTriNoiTruService.BangKiemAnToanNBPT(xacNhanInBangKiemAnToanNguoiBenhPhauThuat);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuKhamGayMeTruocMo:
                                        var xacNhanInPhieuKhamGayMeTruocMo = new PhieuDieuTriVaServicesHttpParams()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInPhieuKhamGayMeTruocMo(xacNhanInPhieuKhamGayMeTruocMo);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayTuNguyenTrietSan:
                                        resultHTML = await _dieuTriNoiTruService.InGiayTuNguyenTrietSan(yeuCauTiepNhanNoiTru.Id, queryInfo.Hosting);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetKyThuatMoi:
                                        var xacNhanInGiayCamKetKyThuatMoi = new PhieuDieuTriVaServicesHttpParams()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInGiayCamKetKyThuatMoi(xacNhanInGiayCamKetKyThuatMoi);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayKhamChuaBenhTheoYc:
                                        var xacNhanInGiayKhamChuaBenhTheoYc = new PhieuDieuTriVaServicesHttpParams()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInGiayKhamChuaBenhTheoYc(xacNhanInGiayKhamChuaBenhTheoYc);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh:
                                        var xacNhanInBangTheoDoiHoiTinh = new BangTheoDoiHoiTinhHttpParamsRequest()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            IdNoiTruHoSo = hoSoKhac.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInBangTheoDoiHoiTinh(xacNhanInBangTheoDoiHoiTinh);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetGayTeGiamDauTrongDeSauMo:
                                        resultHTML = await _dieuTriNoiTruService.InBienBanCamKetGayTeGiamDauTrongDeSauMo(yeuCauTiepNhanNoiTru.Id);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri:
                                        var xacNhanInInfo = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                            TuNgay = string.Empty,
                                            DenNgay = string.Empty
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InBangKiemAnToanNguoiBenhPTVeKhoaDieuTri(xacNhanInInfo);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiGayMeHoiSuc:
                                        resultHTML = await _dieuTriNoiTruService.InBangTheoDoiGayMeHoiSuc(yeuCauTiepNhanNoiTru.Id, hoSoKhac.Id);
                                        typeLandscapeLayout = true;
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiThuoc:
                                        breakTag = "<div style='break-after:page'></div><br/>";
                                        var xacNhanInPhieuCongKhaiThuoc = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                            TuNgay = string.Empty,
                                            DenNgay = string.Empty
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InPhieuCongKhaiThuoc(xacNhanInPhieuCongKhaiThuoc);

                                        var tagHeadDonThuoc = "<div class=\"header\"></div>";
                                        var headDonThuoc = resultHTML.Split(tagHeadDonThuoc);
                                        resultHTML = resultHTML.Remove(resultHTML.LastIndexOf(breakTag, StringComparison.Ordinal)).Replace(breakTag, breakTag + headDonThuoc[0]);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiVatTu:
                                        breakTag = "<div style='break-after:page'></div><br/>";
                                        var xacNhanInPhieuCongKhaiVatTu = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                            TuNgay = string.Empty,
                                            DenNgay = string.Empty
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InPhieuCongKhaiVatTu(xacNhanInPhieuCongKhaiVatTu);

                                        var tagHeadVatTu = "<div class=\"header\"></div>";
                                        var headVatTu = resultHTML.Split(tagHeadVatTu);
                                        resultHTML = resultHTML.Remove(resultHTML.LastIndexOf(breakTag, StringComparison.Ordinal)).Replace(breakTag, breakTag + headVatTu[0]);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuDeNghiTestTruocKhiDungThuoc:
                                        var xacNhanInInfoPhieuDeNghiTestTruocKhiDungThuoc = new InPhieuDeNghiTestTruocKhiDungThuoc()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                            LoaiPhieuIn = 1
                                        };
                                        var resultHTMLPhieuDeNghi = await _dieuTriNoiTruService.InPhieuDeNghiTestTruocKhiDungThuoc(xacNhanInInfoPhieuDeNghiTestTruocKhiDungThuoc);
                                        AddHtmlToListForPrintPdf(new HtmlXuatPdfInfoVo()
                                        {
                                            Html = resultHTMLPhieuDeNghi,
                                            BreakTag = breakTag,
                                            TypeSizeDefault = typeSizeDefault,
                                            TypeLayoutDefault = typeLayoutDefault,
                                            TypeLayoutLandscapeDefault = typeLayoutLandscapeDefault,
                                            FormatPageOpenTagDefault = formatPageOpenTagDefault,
                                            FormatScriptTagDefault = formatScriptTagDefault,
                                            FormatPageCloseTagDefault = formatPageCloseTagDefault,
                                            TypeLandscapeLayout = typeLandscapeLayout,
                                            ListHtml = list
                                        });

                                        // phần này sẽ xử lý ở phía dưới
                                        xacNhanInInfoPhieuDeNghiTestTruocKhiDungThuoc.LoaiPhieuIn = 2;
                                        resultHTML = await _dieuTriNoiTruService.InPhieuDeNghiTestTruocKhiDungThuoc(xacNhanInInfoPhieuDeNghiTestTruocKhiDungThuoc);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenDich:
                                        var xacNhanInInfoPhieuTheoDoiTruyenDich = new XacNhanInPhieuTheoDoiTruyenDich()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InPhieuTheoDoiTruyenDich(xacNhanInInfoPhieuTheoDoiTruyenDich);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayChuyenTuyen:
                                        resultHTML = await _dieuTriNoiTruService.InGiayChuyenTuyen(yeuCauTiepNhanNoiTru.Id);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BanKiemTruocTiemChungTreEm:
                                        resultHTML = _dieuTriNoiTruService.InBienKiemTruocTiemChungTE(hoSoKhac.Id);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau:
                                        var xacNhanInInfoPhieuTheoDoiTruyenMau = new XacNhanInPhieuTheoDoiTruyenMau()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InPhieuTheoDoiTruyenMau(xacNhanInInfoPhieuTheoDoiTruyenMau);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien:
                                        resultHTML = _dieuTriNoiTruService.InGiayRaVien(hoSoKhac.Id);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn:
                                        var xacNhanInInfoTomTatHoSoBenhAn = new PhieuDieuTriVaServicesHttpParams()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            HostingName = queryInfo.Hosting,
                                            Header = false
                                        };
                                        resultHTML = await _noiTruHoSoKhacService.PhieuInTomTatHoSoBenhAn(xacNhanInInfoTomTatHoSoBenhAn);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh:
                                        breakTag = "<div class=\"pagebreak\"> </div>";
                                        resultHTML = _dieuTriNoiTruService.InGiayChungSinh(hoSoKhac.Id, queryInfo.Hosting);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBenhVien:
                                        var xacNhanInInfoGiayCamKet = new XacNhanInPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InPhieuGiayCamKetTuNguyenSuDungThuoc(xacNhanInInfoGiayCamKet);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH:
                                        typeLandscapeLayout = true;
                                        var xacNhanInInfoGiayChungNhanNghiViecHuongBHXH = new XacNhanInPhieuGiayChungNhanNghiViecHuongBHXH()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InGiayChungNhanNghiViecHuongBHXH(xacNhanInInfoGiayChungNhanNghiViecHuongBHXH);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinhMangThaiHo:
                                        typeLandscapeLayout = true;
                                        breakTag = "<div style='break-after:big-page'></div>";
                                        var breakTagTemp = "<div style='break-after:page'></div>"
                                                   + "<table style=\"width:100%\">";
                                        var xacNhanInInfoGiayChungSinhMangThaiHo = new XacNhanInPhieuGiaySinhMangThaiHo()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InGiayChungSinhMangThaiHo(xacNhanInInfoGiayChungSinhMangThaiHo);
                                        resultHTML = resultHTML.Replace(breakTagTemp, breakTag + "<table style=\"width:100%\">");
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.BieuDoChuyenDa:
                                        resultHTML = await _dieuTriNoiTruService.InBieuDoChuyenDa(yeuCauTiepNhanNoiTru.Id);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiDichVuKyThuat:
                                        var xacNhanInInfoPhieuCongKhaiDichVuKyThuat = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                            TuNgay = string.Empty,
                                            DenNgay = string.Empty
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InPhieuCongKhaiDichVuKyThuat(xacNhanInInfoPhieuCongKhaiDichVuKyThuat);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiThuocVatTu:
                                        var xacNhanInInfoPhieuCongKhaiThuocVatTu = new XacNhanInTrichBienBanHoiChan()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                            Hosting = queryInfo.Hosting,
                                            TuNgay = string.Empty,
                                            DenNgay = string.Empty
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InPhieuCongKhaiThuocVatTu(xacNhanInInfoPhieuCongKhaiThuocVatTu);
                                        break;
                                    case Enums.LoaiHoSoDieuTriNoiTru.HoSoChamSocDieuDuongHoSinh:
                                        var xacNhanInHoSoChamSocDieuDuongHoSinh = new XacNhanInHoSoChamSocDieuDuongHoSinh()
                                        {
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            NoiTruHoSoKhacId = hoSoKhac.Id,
                                            LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru
                                        };
                                        resultHTML = await _dieuTriNoiTruService.InHoSoDieuDtriChamSocHoSinh(xacNhanInHoSoChamSocDieuDuongHoSinh);
                                        break;
                                }
                            }
                            
                            AddHtmlToListForPrintPdf(new HtmlXuatPdfInfoVo()
                            {
                                Html = resultHTML,
                                BreakTag = breakTag,
                                TypeSizeDefault = typeSizeDefault,
                                TypeLayoutDefault = typeLayoutDefault,
                                TypeLayoutLandscapeDefault = typeLayoutLandscapeDefault,
                                FormatPageOpenTagDefault = formatPageOpenTagDefault,
                                FormatScriptTagDefault = formatScriptTagDefault,
                                FormatPageCloseTagDefault = formatPageCloseTagDefault,
                                TypeLandscapeLayout = typeLandscapeLayout,
                                ListHtml = list
                            });
                        }
                    }


                    #endregion

                    #region Thuốc ra viện
                    var donThuocRaViens = yeuCauTiepNhanNoiTru.NoiTruDonThuocs.ToList();
                    if (donThuocRaViens.Any()
                        && (queryInfo.LoaiLichSuKhamChuaBenhChiTiet == null 
                            || (queryInfo.LoaiLichSuKhamChuaBenh == Enums.LoaiLichSuKhamChuaBenh.YLenh 
                                && queryInfo.LoaiLichSuKhamChuaBenhChiTiet == Enums.LoaiLichSuKhamChuaBenhChiTiet.DonThuoc)))
                    {
                        var inToaThuoc = new InToaThuocRaVien()
                        {
                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                            HostingName = queryInfo.Hosting,
                            GhiChu = donThuocRaViens.Select(x => x.GhiChu).FirstOrDefault(),
                            Header = false
                        };
                        var resultHTML = _dieuTriNoiTruService.InDonThuocRaVien(inToaThuoc);
                        var lstHTML = resultHTML.Split("<div class=\"pagebreak\"> </div>");
                        foreach (var html in lstHTML)
                        {
                            if (!string.IsNullOrEmpty(html))
                            {
                                var htmlContent = formatPageOpenTagDefault;
                                htmlContent += formatScriptTagDefault;
                                htmlContent += html;
                                htmlContent += formatPageCloseTagDefault;
                                var htmlToPdfVo = new HtmlToPdfVo
                                {
                                    Html = htmlContent,
                                    FooterHtml = string.Empty,
                                    Bottom = 15
                                };
                                list.Add(htmlToPdfVo);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            if (!list.Any())
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuKhamSucKhoe" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        private void AddHtmlToListForPrintPdf(HtmlXuatPdfInfoVo info)
        {
            if (!string.IsNullOrEmpty(info.Html))
            {
                var lstHTML = info.Html.Split(info.BreakTag);
                foreach (var html in lstHTML)
                {
                    if (!string.IsNullOrEmpty(html))
                    {
                        var htmlContent = info.TypeLandscapeLayout
                            ? info.FormatPageOpenTagDefault.Replace($"{info.TypeSizeDefault} {info.TypeLayoutDefault}", $"{info.TypeSizeDefault} {info.TypeLayoutLandscapeDefault}")
                            : info.FormatPageOpenTagDefault;
                        htmlContent += info.FormatScriptTagDefault;
                        htmlContent += html;
                        htmlContent += info.FormatPageCloseTagDefault;
                        var htmlToPdfVo = new HtmlToPdfVo
                        {
                            Html = htmlContent,
                            FooterHtml = string.Empty,
                            Bottom = 15
                        };
                        info.ListHtml.Add(htmlToPdfVo);
                    }
                }
            }
        }

        //BVHD-3727: tất cả các function phí dưới đều có clone sang controller BenhAnDienTu
        //Nếu có chỉnh sửa, thì hãy sửa luôn ở bên controller đó

        #region get List kết quả phiên chi tiết theo phiên 
        private async Task<DuyetKetQuaXetNghiemViewModel> GetPhien(long id)
        {
            var result = await _duyetKetQuaXetNghiemService.GetByIdAsync(id
                ,
                u => u.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                .Include(x => x.MauXetNghiems)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DanToc)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.Icdchinh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiThucHien)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.CongTyKhamSucKhoe)
                .Include(x => x.BenhNhan)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                    .ThenInclude(x => x.NhomDichVuBenhVien)

                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                    .ThenInclude(x => x.YeuCauDichVuKyThuat)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                    .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                .ThenInclude(x => x.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                .ThenInclude(x => x.MayXetNghiem)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)

                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem)
                    .ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)

                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem)
                    .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
                );
            //if (result == null)
            //    return NotFound();

            //            var yeuCauKhamBenhs = result.YeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && cc.IcdchinhId != null)
            //                .OrderBy(cc => cc.ThoiDiemHoanThanh).ToList();

            var chanDoans = result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                (result.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                    ? result.YeuCauTiepNhan?.YeuCauKhamBenhs?
                        .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                        .Select(p => p.ChanDoanSoBoGhiChu).ToList().Distinct().Join(" ; ")
                    : "") :
                (result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                    result.PhienXetNghiemChiTiets.Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().Join(" ; ") :
                    (result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "Khám sức khỏe" : ""));

            var resultData = new DuyetKetQuaXetNghiemViewModel
            {
                Id = id,
                YeuCauTiepNhanId = result.YeuCauTiepNhanId,
                MaSoBHYT = result.YeuCauTiepNhan.BHYTMaSoThe,
                CoBHYT = result.YeuCauTiepNhan.CoBHYT,
                BarCodeID = result.BarCodeId,
                MaYeuCauTiepNhan = result.YeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = result.BenhNhan.MaBN,
                HoTen = result.YeuCauTiepNhan.HoTen,
                NgaySinh = result.YeuCauTiepNhan.NgaySinh,
                ThangSinh = result.YeuCauTiepNhan.ThangSinh,
                NamSinh = result.YeuCauTiepNhan.NamSinh,
                GioiTinh = result.YeuCauTiepNhan.GioiTinh,
                BHYTMucHuong = result.YeuCauTiepNhan.BHYTMucHuong,
                LoaiYeuCauTiepNhan = result.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                SoDienThoai = result.YeuCauTiepNhan.SoDienThoaiDisplay ?? result.YeuCauTiepNhan.NguoiLienHeSoDienThoai?.ApplyFormatPhone(),
                DiaChi = result.YeuCauTiepNhan.DiaChiDayDu,
                ChanDoan = chanDoans,
                NguoiThucHienId = result.NhanVienThucHienId,
                NguoiThucHien = result.NhanVienThucHien?.User.HoTen,
                GhiChu = result.GhiChu,
                //BVHD-3364
                TenCongTy = result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? result.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : null
            };

            //if (result.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiChiDinhId != null))
            //{
            //    var yckb = result.YeuCauTiepNhan.YeuCauKhamBenhs
            //        .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiChiDinhId != null).ToList();

            //    resultData.Phong = yckb.Select(p => p.NoiChiDinh).Select(p => p.Ten).Distinct().Join(" , ");
            //    resultData.Khoa = yckb.Select(p => p.NoiChiDinh).Select(p => p.KhoaPhong).Select(p => p.Ten).Distinct().Join(" , ");
            //}


            resultData.Phong = result.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.Ten).Distinct().Join(" , ");
            resultData.KhoaChiDinh = result.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.KhoaPhong).Select(p => p.Ten).Distinct().Join(" , ");

            if (result.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan != null))
            {
                resultData.TrangThai = null;
            }
            else if (result.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))))
            {
                resultData.TrangThai = true;
            }
            else
            {
                resultData.TrangThai = false;
            }

            //if (result.PhienXetNghiemChiTiets.Any(o => o.KetQuaXetNghiemChiTiets.Any())
            //    && result.PhienXetNghiemChiTiets.Any(o => o.ThoiDiemKetLuan == null && o.DaGoiDuyet == true))
            //{
            //    resultData.TrangThai = result.PhienXetNghiemChiTiets.Any(o => o.ChayLaiKetQua == true);
            //}
            //else
            //{
            //    resultData.TrangThai = null;
            //}

            var lstYeuCauDichVuKyThuatId = result.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();

            foreach (var ycId in lstYeuCauDichVuKyThuatId)
            {
                if (!result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                var res = result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                listChiTiet.AddRange(res);
            }

            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            resultData.ChiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<DuyetKqXetNghiemChiTietViewModel>(), true);

            foreach (var detail in resultData.ChiTietKetQuaXetNghiems)
            {
                detail.DanhSachLoaiMauDaCoKetQua = resultData.ChiTietKetQuaXetNghiems
                    .Where(p => p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).Select(p => p.LoaiMau)
                    .Distinct().ToList();


                var lstTongCong = result.YeuCauTiepNhan.YeuCauDichVuKyThuats
                    .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
                    .Select(p => p.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription())
                    .Distinct().Where(p => p != null).ToList();


                var lstLoaiMauKhongDat = new List<string>();

                foreach (var loaiMau in lstTongCong)
                {
                    var mauXetNghiem = _duyetKetQuaXetNghiemService.GetById(id, p => p.Include(o => o.MauXetNghiems))
                        .MauXetNghiems
                        .Where(p => p.LoaiMauXetNghiem.GetDescription() == loaiMau && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).LastOrDefault();
                    if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
                    {
                        lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
                    }
                }

                detail.DanhSachLoaiMau = lstTongCong;
                detail.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
                detail.IsParent = detail.DaGoiDuyet == true && detail.IdChilds.Count == 0;
            }

            return resultData;
        }

        private List<DuyetKqXetNghiemChiTietViewModel> AddDetailDataChild(List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai
            , List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild, List<DuyetKqXetNghiemChiTietViewModel> result
            , bool theFirst = false, int level = 1)
        {
            if (!lstChiTietNhomChild.Any() && theFirst != true) return result;

            List<long> lstIdSearch = new List<long>();
            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                foreach (var parent in lstParent)
                {
                    var ketQua = new DuyetKqXetNghiemChiTietViewModel
                    {
                        Id = parent.Id,
                        Ten = parent.YeuCauDichVuKyThuat.TenDichVu,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        Csbt = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        GiaTriMin = parent.GiaTriMin,
                        GiaTriMax = parent.GiaTriMax,
                        DonVi = parent.DonVi,
                        //Duyet = parent.NhanVienDuyetId != null,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        LoaiMau = parent.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        DaGoiDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet,
                        //structure tree
                        Level = level,
                        YeuCauChayLai = parent.PhienXetNghiemChiTiet?.ChayLaiKetQua,
                        DaDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet == true && parent.PhienXetNghiemChiTiet?.ThoiDiemKetLuan != null,
                        NguoiYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        LyDoYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        NgayDuyetDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        ? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet != null
                            ? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                            : "",
                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                        LoaiKetQuaNhapTay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriNhapTay),
                        Nhom = parent.NhomDichVuBenhVien.Ten,
                        NhomId = parent.NhomDichVuBenhVienId,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                            && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,
                    };
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    result.Add(ketQua);
                }
            }
            else
            {
                var lstReOrderBySTT = lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
                foreach (var parent in lstReOrderBySTT)
                {
                    var ketQua = new DuyetKqXetNghiemChiTietViewModel
                    {
                        Id = parent.Id,
                        Ten = parent.DichVuXetNghiem?.Ten,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        //Csbt = parent.GiaTriMin != null || parent.GiaTriMax != null ? parent.GiaTriMin + " - " + parent.GiaTriMax : "",
                        Csbt = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        GiaTriMin = parent.GiaTriMin,
                        GiaTriMax = parent.GiaTriMax,
                        DonVi = parent.DonVi,
                        //Duyet = parent.NhanVienDuyetId != null,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        DaGoiDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet,
                        LoaiMau = parent.NhomDichVuBenhVien.Ten,
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        //structure tree
                        Level = level,
                        YeuCauChayLai = parent.PhienXetNghiemChiTiet?.ChayLaiKetQua,
                        DaDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet == true && parent.PhienXetNghiemChiTiet?.ThoiDiemKetLuan != null,
                        NguoiYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                            ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        LyDoYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                            ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                            ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                            ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        NgayDuyetDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                            ? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet != null
                                ? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                            : "",
                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                            , parent.GiaTriTuMay),
                        LoaiKetQuaNhapTay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriNhapTay),
                        NhomId = parent.NhomDichVuBenhVienId,
                        Nhom = parent.NhomDichVuBenhVien.Ten,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                                                                   && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,
                        DichVuXetNghiemChaId = parent.DichVuXetNghiemChaId,

                    };
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    var index = result.FindIndex(x => x.DichVuXetNghiemId == parent.DichVuXetNghiemChaId);
                    if (index >= 0)
                    {
                        var listChilds = result.Count(x => x.DichVuXetNghiemChaId == parent.DichVuXetNghiemChaId);
                        result.Insert(index + 1 + listChilds, ketQua);
                    }
                }
            }

            lstIdSearch = lstIdSearch.Distinct().ToList();
            var lstChiTietChild = lstChiTietNhomConLai.Where(p => lstIdSearch.Any(o => o == p.DichVuXetNghiemChaId)).ToList();
            level++;
            return AddDetailDataChild(lstChiTietNhomConLai, lstChiTietChild, result, false, level);
        }

        private string footerPhieuIn(string ngayGioHienTai, string page)
        {
            string htmlFooter = string.Empty;
            htmlFooter += "<table width = '100%'><tr><td style ='text-align: left;width:50%;'>" + ngayGioHienTai + "</td>";
            for (var item = 0; item < 9; item++)
            {
                htmlFooter += "<td>" + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;" + "</td>";
            }
            htmlFooter += "<td style ='text-align: right;width: 50 %;'>" + page + "</td></tr></table>";
            return htmlFooter;

        }
        #endregion


        #region phiếu điều trị
        //private async Task<string> getContent(YeuCauTiepNhan entity, NoiTruPhieuDieuTri phieuDieuTri, long yeuCauTiepNhanId)
        //{

        //    var content = "";
        //    var html = _danhSachChoKhamService.GetBodyByName("ToDieuTri");
        //    var soThuTu = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderBy(o => o.NgayDieuTri).IndexOf(phieuDieuTri) + 1;
        //    var cdkt = phieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Select(o => o.GhiChu).ToArray();
        //    var data = new PhieuDieuTriViewModel
        //    {
        //        So = soThuTu + "",
        //        SoVaoVien = entity.NoiTruBenhAn?.SoBenhAn ?? "",
        //        HoTen = entity.HoTen,
        //        Tuoi = DateTime.Now.Year - entity.NamSinh + "",
        //        GioiTinh = entity.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? "Nam" : "Nữ",

        //        Khoa = entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
        //            ? entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten.Replace("Khoa", string.Empty) : "",

        //        Buong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
        //            ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh?.PhongBenhVien?.Ten
        //                : "",
        //        Giuong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
        //            ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten
        //                : "",
        //        ChanDoan = phieuDieuTri.ChanDoanChinhGhiChu + (cdkt.Length > 0 ? "; " + string.Join("; ", cdkt) : "")
        //    };

        //    var lstDVKT = await _phauThuatThuThuatService.GetDichVuKyThuatsByYeuCauTiepNhan(yeuCauTiepNhanId, phieuDieuTri.Id);
        //    var lstThuoc = await _dieuTriNoiTruService.GetDanhSachThuoc(yeuCauTiepNhanId, phieuDieuTri.Id);
        //    //var lstTruyenDich = await _dieuTriNoiTruService.GetDanhSachTruyenDich(yeuCauTiepNhanId, phieuDieuTri.Id);

        //    if (!string.IsNullOrEmpty(phieuDieuTri.DienBien))
        //    {
        //        var dienBiens = JsonConvert.DeserializeObject<List<NoiTruPhieuDieuTriDienBien>>(phieuDieuTri.DienBien);
        //        var i = 0;
        //        dienBiens.OrderBy(o => o.ThoiGian).ToList().ForEach(dienBien =>
        //        {

        //            var bacSi = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //                ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
        //                    .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //                   .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";
        //            var ngayGio = string.Empty;
        //            var dienBienBenh = string.Empty;
        //            var yLenh = string.Empty;
        //            if (i == 0 && (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian) ||
        //                //lstTruyenDich.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian) ||
        //                lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian)))
        //            {
        //                ngayGio += "Trước " + dienBien.ThoiGian.Hour + " giờ " + dienBien.ThoiGian.Minute + " phút, " + dienBien.ThoiGian.ApplyFormatDate();
        //                ////DIỄN BIẾN
        //                //dienBienBenh += "<p><b>DIỄN BIẾN</b>: </p><br/>";
        //                ////DVKT
        //                //dienBienBenh += "<p> <b>DVKT</b>: </p>";
        //                if (lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
        //                {
        //                    foreach (var item in lstDVKT.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
        //                    {
        //                        dienBienBenh += "<div style='padding:1px;'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
        //                    }
        //                }
        //                ////Y lệnh
        //                //yLenh += "<p><b>Y LỆNH</b>: </p> <br>";
        //                ////THUỐC
        //                //yLenh += "<p><b>THUỐC</b>:" + " </p>";
        //                if (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
        //                {
        //                    foreach (var item in lstThuoc.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
        //                    {
        //                        //DỊCH TRUYỀN
        //                        if (item.LaDichTruyen == true)
        //                        {
        //                            yLenh += "<div><b>" +
        //                                     (item.LaThuocHuongThanGayNghien == true
        //                                         ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(
        //                                               yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) +
        //                                           ") "
        //                                         : "") + item.Ten +
        //                                     (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") +
        //                                     (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                                     (item.LaThuocHuongThanGayNghien == true
        //                                         ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                         : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";

        //                            yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
        //                                      !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
        //                                      !string.IsNullOrEmpty(item.GhiChu) ?
        //                                      "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
        //                                      item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

        //                            var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
        //                            if (thoiGianBatDauTruyen != null)
        //                            {
        //                                if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
        //                                {
        //                                    for (int j = 0; j < item.SoLanDungTrongNgay; j++)
        //                                    {
        //                                        var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
        //                                        thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
        //                                        item.GioSuDung += time + "; ";
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
        //                                }
        //                            }
        //                            yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
        //                            !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
        //                            !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

        //                            "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
        //                            (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
        //                            " " + (item.CachGioTruyenDich != null
        //                                ? "cách " + item.CachGioTruyenDich + " giờ,"
        //                                : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
        //                                ? "giờ sử dụng: " + item.GioSuDung
        //                                : "") + "</div>"

        //                            : "";
        //                        }
        //                        else
        //                        {
        //                            yLenh += "<div><b>" + (item.LaThuocHuongThanGayNghien == true ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + item.Ten + (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") + (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                                     (item.LaThuocHuongThanGayNghien == true
        //                                         ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                         : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";
        //                            yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : "";
        //                            yLenh += "<div style='margin-left:15px;margin-bottom:0.1px'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
        //                                     (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
        //                                         ? (" " + item.ThoiGianDungSangDisplay)
        //                                         : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
        //                                                                                          !string.IsNullOrEmpty(item.DungChieu) ||
        //                                                                                          !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                                     (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
        //                                     (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
        //                                         ? (" " + item.ThoiGianDungTruaDisplay)
        //                                         : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
        //                                                                                          !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                                     (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
        //                                     (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
        //                                         ? (" " + item.ThoiGianDungChieuDisplay)
        //                                         : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                                     (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
        //                                     (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
        //                                         ? (" " + item.ThoiGianDungToiDisplay)
        //                                         : "") + "</div>";
        //                        }
        //                    }
        //                }

        //                //CHẾ ĐỘ ĂN
        //                yLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div><br/>";
        //                //CHẾ ĐỘ CHĂM SÓC
        //                yLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
        //                //BÁC SĨ
        //                yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
        //                yLenh += "<p style='text-align: center;height:30px'></p>";
        //                yLenh += "<p style='text-align: center;'> " + bacSi + "</p>";

        //                if (i == 0)
        //                {
        //                    data.ToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\"> ";
        //                }

        //                data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
        //                data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //                data.ToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
        //            }
        //            ngayGio = string.Empty;
        //            dienBienBenh = string.Empty;
        //            yLenh = string.Empty;
        //            ngayGio += dienBien.ThoiGian.Hour + " giờ " + dienBien.ThoiGian.Minute + " phút, " + dienBien.ThoiGian.ApplyFormatDate();
        //            //DIỄN BIẾN
        //            dienBienBenh += "<div> " + dienBien?.DienBien + " </div>";
        //            yLenh += !string.IsNullOrEmpty(dienBien?.DienBien) ? "<br>" : "";
        //            //DVKT
        //            dienBienBenh += "<p> <b>DVKT</b>: </p>";
        //            if (lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
        //            {
        //                foreach (var item in lstDVKT.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
        //                {
        //                    dienBienBenh += "<p style='margin-bottom:8px'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </p>";
        //                }
        //            }
        //            //Y lệnh
        //            yLenh += "<div>" + dienBien?.YLenh + " </div>";
        //            yLenh += !string.IsNullOrEmpty(dienBien?.YLenh) ? "<br>" : "";
        //            ////THUỐC
        //            //yLenh += "<p><b>THUỐC</b>:" + " </p>";
        //            if (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
        //            {
        //                foreach (var item in lstThuoc.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
        //                {
        //                    //DỊCH TRUYỀN
        //                    if (item.LaDichTruyen == true)
        //                    {
        //                        yLenh += "<div><b>" +
        //                                 (item.LaThuocHuongThanGayNghien == true
        //                                     ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId,
        //                                           phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") "
        //                                     : "") + item.Ten +
        //                                 (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") +
        //                                 (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                                 (item.LaThuocHuongThanGayNghien == true
        //                                     ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                     : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";
        //                        yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
        //                                     !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
        //                                     !string.IsNullOrEmpty(item.GhiChu) ?
        //                                     "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
        //                                     item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

        //                        var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
        //                        if (thoiGianBatDauTruyen != null)
        //                        {
        //                            if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
        //                            {
        //                                for (int j = 0; j < item.SoLanDungTrongNgay; j++)
        //                                {
        //                                    var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
        //                                    thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
        //                                    item.GioSuDung += time + "; ";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
        //                            }
        //                        }
        //                        yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
        //                            !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
        //                            !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

        //                            "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
        //                            (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
        //                            " " + (item.CachGioTruyenDich != null
        //                                ? "cách " + item.CachGioTruyenDich + " giờ,"
        //                                : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
        //                                ? "giờ sử dụng: " + item.GioSuDung
        //                                : "") + "</div>"

        //                            : "";
        //                    }
        //                    else
        //                    {
        //                        yLenh += "<div><b>" + (item.LaThuocHuongThanGayNghien == true ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + item.Ten + (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") + (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                                 (item.LaThuocHuongThanGayNghien == true
        //                                     ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                     : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";
        //                        yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : "";
        //                        yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
        //                                 (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
        //                                     ? (" " + item.ThoiGianDungSangDisplay)
        //                                     : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
        //                                                                                      !string.IsNullOrEmpty(item.DungChieu) ||
        //                                                                                      !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                                 (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
        //                                 (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
        //                                     ? (" " + item.ThoiGianDungTruaDisplay)
        //                                     : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
        //                                                                                      !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                                 (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
        //                                 (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
        //                                     ? (" " + item.ThoiGianDungChieuDisplay)
        //                                     : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                                 (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
        //                                 (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
        //                                     ? (" " + item.ThoiGianDungToiDisplay)
        //                                     : "") + "</div>";
        //                    }
        //                }
        //            }
        //            //CHẾ ĐỘ ĂN
        //            yLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div><br/>";
        //            //CHẾ ĐỘ CHĂM SÓC
        //            yLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
        //            //BÁC SĨ
        //            yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
        //            yLenh += "<p style='text-align: center;height:30px'></p>";
        //            yLenh += "<p style='text-align: center;'> " + bacSi + "</p>";

        //            if (i != 0 && i != dienBiens.OrderBy(o => o.ThoiGian).ToList().Count())
        //            {
        //                data.ToDieuTri += "<tr style='border-top:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //            }
        //            else if (i == (dienBiens.OrderBy(o => o.ThoiGian).ToList().Count() - 1))
        //            {
        //                data.ToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //            }
        //            else
        //            {
        //                data.ToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //            }

        //            data.ToDieuTri += "<tr style='border-top:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //            data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
        //            data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //            data.ToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
        //            i++;
        //        });
        //    }
        //    else
        //    {

        //        var ngayGio = string.Empty;
        //        var dienBienBenh = string.Empty;
        //        var yLenh = string.Empty;

        //        if (phieuDieuTri.ThoiDiemThamKham != null)
        //        {
        //            ngayGio += phieuDieuTri.ThoiDiemThamKham.Value.Hour + " giờ " + phieuDieuTri.ThoiDiemThamKham.Value.Minute + " phút, " + phieuDieuTri.ThoiDiemThamKham.Value.ApplyFormatDate();

        //        }
        //        ////DIỄN BIẾN
        //        //dienBienBenh += "<p><b>DIỄN BIẾN</b>: </p><br/>";
        //        ////DVKT
        //        //dienBienBenh += "<p> <b>DVKT</b>: </p>";
        //        foreach (var item in lstDVKT)
        //        {
        //            dienBienBenh += "<div style='margin-bottom:8px'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
        //        }
        //        ////Y lệnh
        //        //yLenh += "<p><b>Y LỆNH</b>: </p> <br>";
        //        ////THUỐC
        //        //yLenh += "<p><b>THUỐC</b>:" + " </p>";
        //        if (lstThuoc.Any())
        //        {
        //            foreach (var item in lstThuoc)
        //            {
        //                //DỊCH TRUYỀN
        //                if (item.LaDichTruyen == true)
        //                {
        //                    yLenh += "<div><b>" +
        //                             (item.LaThuocHuongThanGayNghien == true
        //                                 ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId,
        //                                       phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") "
        //                                 : "") + item.Ten +
        //                             (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") +
        //                             (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                             (item.LaThuocHuongThanGayNghien == true
        //                                 ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                 : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";

        //                    yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
        //                                           !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
        //                                           !string.IsNullOrEmpty(item.GhiChu) ?
        //                                           "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
        //                                           item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

        //                    var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
        //                    if (thoiGianBatDauTruyen != null)
        //                    {
        //                        if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
        //                        {
        //                            for (int j = 0; j < item.SoLanDungTrongNgay; j++)
        //                            {
        //                                var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
        //                                thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
        //                                item.GioSuDung += time + "; ";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
        //                        }
        //                    }
        //                    yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
        //                                               !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
        //                                               !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

        //                                               "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
        //                                               (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
        //                                               " " + (item.CachGioTruyenDich != null
        //                                                   ? "cách " + item.CachGioTruyenDich + " giờ,"
        //                                                   : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
        //                                                   ? "giờ sử dụng: " + item.GioSuDung
        //                                                   : "") + "</div>"

        //                                               : "";
        //                }
        //                else
        //                {
        //                    yLenh += "<div><b>" + (item.LaThuocHuongThanGayNghien == true ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + item.Ten + (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") + (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                             (item.LaThuocHuongThanGayNghien == true
        //                                 ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                 : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";
        //                    yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : "";
        //                    yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
        //                             (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
        //                                 ? (" " + item.ThoiGianDungSangDisplay)
        //                                 : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
        //                                                                                  !string.IsNullOrEmpty(item.DungChieu) ||
        //                                                                                  !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                             (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
        //                             (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
        //                                 ? (" " + item.ThoiGianDungTruaDisplay)
        //                                 : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
        //                                                                                  !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                             (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
        //                             (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
        //                                 ? (" " + item.ThoiGianDungChieuDisplay)
        //                                 : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
        //                             (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
        //                             (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
        //                                 ? (" " + item.ThoiGianDungToiDisplay)
        //                                 : "") + "</div>";
        //                }
        //            }
        //        }
        //        //CHẾ ĐỘ ĂN
        //        yLenh += "<div><b>Chế độ ăn: </b> " + phieuDieuTri?.CheDoAn?.Ten + "</div>";
        //        //CHẾ ĐỘ CHĂM SÓC
        //        yLenh += "<div><b>Chế độ chăm sóc:</b> " + phieuDieuTri?.CheDoChamSoc?.GetDescription() + "</div>";
        //        //BÁC SĨ
        //        var bacSi = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //            ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
        //                .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //                .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";
        //        yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
        //        yLenh += "<p style='text-align: center;height:30px'></p>";

        //        if (string.IsNullOrEmpty(ngayGio)){
        //            yLenh += "<p style='text-align: center;'> " + bacSi + "</p>";
        //        }
        //        else
        //        {

        //            var bacSiTheoNgayGioThamKham = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.ThoiDiemThamKham && (s.DenNgay == null || phieuDieuTri.ThoiDiemThamKham <= s.DenNgay.Value.Date))
        //           ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
        //               .Where(s => s.TuNgay.Date <= phieuDieuTri.ThoiDiemThamKham && (s.DenNgay == null || phieuDieuTri.ThoiDiemThamKham <= s.DenNgay.Value.Date))
        //               .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";

        //            yLenh += "<p style='text-align: center;'> " + bacSiTheoNgayGioThamKham + "</p>";
        //        }

               

        //        data.ToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //        data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
        //        data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //        data.ToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
        //    }




        //    content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
        //    return content;
        //}


        #endregion
        #endregion
    }
}
