using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.DuyetKetQuaXetNghiems;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhAnDienTus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiDuongThai;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LichSuKhamChuaBenhs;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Domain.ValueObject.PhieuKhaiThacTienSuDiUng;
using Camino.Core.Domain.ValueObject.PhieuSoKet15NgayDieuTri;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Core.Infrastructure;
using Camino.Data;
using Camino.Services.BenhAnDienTus;
using Camino.Services.BenhNhans;
using Camino.Services.BenhVien;
using Camino.Services.CauHinh;
using Camino.Services.DichVuKhamBenhBenhViens;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.DuyetKetQuaXetNghiems;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.ICDs;
using Camino.Services.InputStringStored;
using Camino.Services.KetQuaSinhHieus;
using Camino.Services.KhamBenhs;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using Camino.Services.NhanVien;
using Camino.Services.NoiDungMauLoiDanBacSi;
using Camino.Services.NoiDuTruHoSoKhac;
using Camino.Services.NoiDuTruHoSoKhacFileDinhKem;
using Camino.Services.NoiTruBenhAn;
using Camino.Services.PhauThuatThuThuat;
using Camino.Services.PhongBenhVien;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.TinhTrangRaVienHoSoKhac;
using Camino.Services.ToaThuocMau;
using Camino.Services.Users;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class BenhAnDienTuController : DieuTriNoiTruController
    {
        private readonly IBenhAnDienTuService _benhAnDienTuService;
        private readonly ILocalizationService _localizationService;
        private readonly IDuyetKetQuaXetNghiemService _duyetKetQuaXetNghiemService;
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly INoiTruHoSoKhacService _noiTruHoSoKhacService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;
        private readonly IPhauThuatThuThuatService _phauThuatThuThuatService;
        private readonly IExcelService _excelService;
        private readonly IRoleService _roleService;
        private readonly ITinhTrangRaVienHoSoKhacService _tinhTrangRaVienHoSoKhacService;
        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTiet;

        //public BenhAnDienTuController(
        //    IBenhAnDienTuService benhAnDienTuService
        //    , ILocalizationService localizationService
        //    , IDuyetKetQuaXetNghiemService duyetKetQuaXetNghiemService
        //    , IDieuTriNoiTruService dieuTriNoiTruService
        //    , IYeuCauKhamBenhService yeuCauKhamBenhService
        //    , INoiTruHoSoKhacService noiTruHoSoKhacService
        //    , IYeuCauTiepNhanService yeuCauTiepNhanService
        //    , IDanhSachChoKhamService danhSachChoKhamService
        //    , IPhauThuatThuThuatService phauThuatThuThuatService
        //    )
        //{
        //    _benhAnDienTuService = benhAnDienTuService;
        //    _localizationService = localizationService;
        //    _duyetKetQuaXetNghiemService = duyetKetQuaXetNghiemService;
        //    _dieuTriNoiTruService = dieuTriNoiTruService;
        //    _yeuCauKhamBenhService = yeuCauKhamBenhService;
        //    _noiTruHoSoKhacService = noiTruHoSoKhacService;
        //    _yeuCauTiepNhanService = yeuCauTiepNhanService;
        //    _danhSachChoKhamService = danhSachChoKhamService;
        //    _phauThuatThuThuatService = phauThuatThuThuatService;
        //}

        public BenhAnDienTuController(
            IUserAgentHelper userAgentHelper, 
            IDieuTriNoiTruService dieuTriNoiTruService, 
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService, 
            INoiTruHoSoKhacService noiTruHoSoKhacService, 
            IYeuCauTiepNhanService yeuCauTiepNhanService, 
            IExcelService excelService, 
            IYeuCauKhamBenhService yeuCauKhamBenhService, 
            ITiepNhanBenhNhanService tiepNhanBenhNhanService, 
            ILocalizationService localizationService, 
            IKhamBenhService khamBenhService, 
            IPhauThuatThuThuatService phauThuatThuThuatService, 
            INoiDuTruHoSoKhacService noiDuTruHoSoKhacService, 
            ITaiLieuDinhKemService taiLieuDinhKemService, 
            INoiDuTruHoSoKhacFileDinhKemService noiDuTruHoSoKhacFileDinhKemService, 
            IKetQuaSinhHieuService ketQuaSinhHieuService, 
            INoiTruBenhAnService noiTruBenhAnService, 
            IUserService userService, 
            IICDService icdService, 
            IDanhSachChoKhamService danhSachChoKhamService, 
            INhanVienService nhanVienService, 
            IMapper mapper, 
            IPdfService pdfService, 
            IPhongBenhVienService phongBenhVienService, 
            IBenhVienService benhVienService, 
            IThuNganNoiTruService thuNganNoiTruService, 
            IYeuCauGoiDichVuService yeuCauGoiDichVuService, 
            ICauHinhService cauHinhService, 
            IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService, 
            INoiDungMauLoiDanBacSiService noiDungMauLoiDanBacSiService, 
            IToaThuocMauService toaThuocMauService, 
            IInputStringStoredService inputStringStoredService, 
            IKhoaPhongService khoaPhongService,
            ILoggerManager logger,
            IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTiet,
                IBenhAnDienTuService benhAnDienTuService
                , IDuyetKetQuaXetNghiemService duyetKetQuaXetNghiemService
            , IRoleService roleService,
            ITinhTrangRaVienHoSoKhacService tinhTrangRaVienHoSoKhacService
            , IDichVuKhamBenhBenhVienService dichVuKhamBenhBenhVienService
            )
            : base(
                userAgentHelper, 
                dieuTriNoiTruService, 
                taiKhoanBenhNhanService, 
                noiTruHoSoKhacService, 
                yeuCauTiepNhanService, 
                excelService, 
                yeuCauKhamBenhService, 
                tiepNhanBenhNhanService, 
                localizationService, 
                khamBenhService, 
                phauThuatThuThuatService, 
                noiDuTruHoSoKhacService, 
                taiLieuDinhKemService, 
                noiDuTruHoSoKhacFileDinhKemService, 
                ketQuaSinhHieuService, 
                noiTruBenhAnService, 
                userService, 
                icdService, 
                danhSachChoKhamService, 
                nhanVienService, 
                mapper, 
                pdfService, 
                phongBenhVienService, 
                benhVienService, 
                thuNganNoiTruService, 
                yeuCauGoiDichVuService, 
                cauHinhService, 
                yeuCauDichVuKyThuatService, 
                noiDungMauLoiDanBacSiService, 
                toaThuocMauService, 
                inputStringStoredService, 
                khoaPhongService,
                logger,
                roleService,
                tinhTrangRaVienHoSoKhacService,
                dichVuKhamBenhBenhVienService
                )
        {
            _benhAnDienTuService = benhAnDienTuService;
            _localizationService = localizationService;
            _duyetKetQuaXetNghiemService = duyetKetQuaXetNghiemService;
            _dieuTriNoiTruService = dieuTriNoiTruService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _noiTruHoSoKhacService = noiTruHoSoKhacService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _danhSachChoKhamService = danhSachChoKhamService;
            _phauThuatThuThuatService = phauThuatThuThuatService;
            _excelService = excelService;
            _phienXetNghiemChiTiet = phienXetNghiemChiTiet;
        }

        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridTimKiemNoiTruBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhAnDienTu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridTimKiemNoiTruBenhAnAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _benhAnDienTuService.GetDataForGridTimKiemNoiTruBenhAnAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridTimKiemNoiTruBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhAnDienTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridTimKiemNoiTruBenhAnAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _benhAnDienTuService.GetTotalPageForGridTimKiemNoiTruBenhAnAsync(queryInfo);
            }

            return Ok(gridData);
        }

        [HttpPost("TimKiemNoiTruBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhAnDienTu)]
        public async Task<ActionResult<GridDataSource>> TimKiemNoiTruBenhAnAsync(LichSuKhamChuaBenhTimKiemVo timKiemObj)
        {
            var gridData = new GridDataSource();
            var queryInfo = new QueryInfo()
            {
                AdditionalSearchString = JsonConvert.SerializeObject(timKiemObj),
                SearchString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("<AdvancedQueryParameters><SearchTerms></SearchTerms></AdvancedQueryParameters>")),
                Take = 50
            };
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _benhAnDienTuService.GetDataForGridTimKiemNoiTruBenhAnAsync(queryInfo);
            }

            return Ok(gridData);
        }

        #endregion

        #region get data
        [HttpGet("GetGayBenhAnDienTuTheoBenhAn")]
        public async Task<ActionResult<BenhAnDienTuDetailVo>> GetGayBenhAnDienTuTheoBenhAnAsnc(long noiTruBenhAnId)
        {
            var result = await _benhAnDienTuService.GetGayBenhAnDienTuTheoBenhAnAsnc(noiTruBenhAnId);
            return result;
        }

        [HttpPost("GetFilePDFBenhAnDienTuFromHtml")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhAnDienTu)]
        public async Task<ActionResult<List<string>>> GetFilePDFBenhAnDienTuFromHtmlAsync(BenhAnDienTuDetailVo benhAnDienTuDetailVo)
        {
            var listHtml = new List<string>();
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

            var yeuCauTiepNhanNoiTru = await _benhAnDienTuService.GetLanTiepNhanTheoBenhAnAsnc(benhAnDienTuDetailVo.NoiTruBenhAnId);
            var yeuCauTiepNhanNgoaiTru = yeuCauTiepNhanNoiTru.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan;

            foreach (var gayBenhAn in benhAnDienTuDetailVo.GayBenhAns)
            {
                foreach (var hoSo in gayBenhAn.ChiTietHoSos)
                {
                    var html = string.Empty;
                    var htmls = new List<string>();
                    var breakTag = "<div class=\"pagebreak\"></div>";
                    switch (hoSo.LoaiHoSo)
                    {
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.BiaBenhAn:
                            var biaBenhAnInfoVo = new BiaBenhAnDienTuInVo()
                            {
                                NoiTruBenhAnId = benhAnDienTuDetailVo.NoiTruBenhAnId,
                                Hosting = benhAnDienTuDetailVo.Hosting
                            };
                            html = await  _benhAnDienTuService.InBiaBenhAnDienTuAsync(biaBenhAnInfoVo);
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.HanhChinhBenhAn:
                            var result = await InPhieuRaVien(benhAnDienTuDetailVo.NoiTruBenhAnId, benhAnDienTuDetailVo.Hosting);
                            html = ((ObjectResult)result).Value as string;
                            breakTag = "<div style=\"break-after:page\"></div>";

                            //#region Xử lý tách gộp style cho các break page html nếu có

                            //var indexOfCloseTagStyle = html.IndexOf("</style>", StringComparison.Ordinal);
                            //if (indexOfCloseTagStyle != -1)
                            //{
                            //    var indexEndCLoseTag = indexOfCloseTagStyle + 8; // index dấu >
                            //    var htmlTagStyle = html.Substring(0, indexEndCLoseTag);
                            //    var arrHtml = html.Split(breakTag);
                            //    var len = arrHtml.Length;
                            //    for (int i = 1; i < len; i++)
                            //    {
                            //        arrHtml[i] = htmlTagStyle + arrHtml[i];
                            //    }

                            //    htmls = arrHtml.ToList();
                            //    html = string.Empty;
                            //}


                            //#endregion
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.TaiLieuDieuTriLanTruoc:
                            if (yeuCauTiepNhanNgoaiTru != null && yeuCauTiepNhanNgoaiTru.HoSoYeuCauTiepNhans.Any())
                            {
                                html = await _benhAnDienTuService.InTaiLieuDieuTriTruocBenhAnDienTuAsync(yeuCauTiepNhanNgoaiTru);
                            }
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.PhieuKhamBenhVaoVien:
                            if (yeuCauTiepNhanNoiTru.YeuCauNhapVien?.YeuCauKhamBenhId != null)
                            {
                                var phieuKhamBenhVo = new PhieuKhamBenhVo()
                                {
                                    YeuCauKhamBenhId = yeuCauTiepNhanNoiTru.YeuCauNhapVien.YeuCauKhamBenhId.Value,
                                    CoKhamBenhVaoVien = true
                                };
                                htmls = _yeuCauKhamBenhService.InPhieuKhamBenh(phieuKhamBenhVo);
                            }
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.PhieuChiDinh:
                            html = _benhAnDienTuService.InDichVuChiDinh(benhAnDienTuDetailVo.Hosting, benhAnDienTuDetailVo.NoiTruBenhAnId,false);
                            breakTag = "<div class=\"pagebreak\"> </div>";
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS:
                            if (hoSo.Value != null)
                            {
                                var lstDichVuKyThuat = 
                                    yeuCauTiepNhanNoiTru.YeuCauDichVuKyThuats.Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien 
                                                                                         && x.NhomDichVuBenhVienId == hoSo.Value.Value).ToList();
                                if (yeuCauTiepNhanNgoaiTru != null && yeuCauTiepNhanNgoaiTru.YeuCauDichVuKyThuats.Any())
                                {
                                    var lstYeucauDichVuKyThuatNgoaiTru =
                                        yeuCauTiepNhanNgoaiTru.YeuCauDichVuKyThuats.Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                                               && x.NhomDichVuBenhVienId == hoSo.Value.Value).ToList();
                                    if (lstYeucauDichVuKyThuatNgoaiTru.Any())
                                    {
                                        lstDichVuKyThuat.AddRange(lstYeucauDichVuKyThuatNgoaiTru);
                                    }
                                }

                                #region // CDHA - TDCN
                                var dichVuCLSs = lstDichVuKyThuat.Where(a => a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                                         || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang).ToList();
                                foreach (var dichVuCLS in dichVuCLSs)
                                {
                                    var phieuInKetQuaInfoVo = new PhieuInKetQuaInfoVo()
                                    {
                                        HasHeader = false,
                                        HostingName = benhAnDienTuDetailVo.Hosting,
                                        Id = dichVuCLS.Id
                                    };
                                    var phieuInCLSHtml = _yeuCauTiepNhanService.XuLyInPhieuKetQuaAsync(phieuInKetQuaInfoVo);
                                    htmls.Add(phieuInCLSHtml);
                                }
                                #endregion

                                #region // dịch vụ xét nghiệm
                                var dichVuXetNghiems = lstDichVuKyThuat.Where(d => d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).ToList();
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
                                            HostingName = benhAnDienTuDetailVo.Hosting,
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

                                    htmls.AddRange(lstHtml);
                                }
                                #endregion
                            }
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.PhieuKhamTheoChuyenKhoa:
                            if (yeuCauTiepNhanNgoaiTru != null)
                            {
                                var lstKhamBenh = yeuCauTiepNhanNgoaiTru.YeuCauKhamBenhs.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham 
                                                                                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                                    .ToList();
                                foreach (var yeuCauKhamBenh in lstKhamBenh)
                                {
                                    var phieuKhamBenhVo = new PhieuKhamBenhVo()
                                    {
                                        YeuCauKhamBenhId = yeuCauKhamBenh.Id,
                                        CoKhamBenh = true
                                    };
                                    var phieuKham = _yeuCauKhamBenhService.InPhieuKhamBenh(phieuKhamBenhVo);
                                    htmls.AddRange(phieuKham);
                                }
                            }
                            
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.HoSoKhac:
                            if (hoSo.Value != null)
                            {
                                var enumHoSoKhac = (Enums.LoaiHoSoDieuTriNoiTru)hoSo.Value.Value;

                                #region phiếu chăm sóc
                                if (enumHoSoKhac == Enums.LoaiHoSoDieuTriNoiTru.PhieuChamSoc)
                                {
                                    var phieuChamSocs = yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                                        .Where(d => d.YeuCauDichVuKyThuatId == null
                                                    && d.YeuCauTruyenMauId == null
                                                    && d.YeuCauVatTuBenhVienId == null
                                                    && d.NoiTruChiDinhDuocPhamId == null)
                                        .ToList();
                                    if (phieuChamSocs.Any())
                                    {
                                        var detail = new InPhieuChamSocVo()
                                        {
                                            BenhNhanId = benhAnDienTuDetailVo.BenhNhanId,
                                            NoiTruBenhAnId = benhAnDienTuDetailVo.NoiTruBenhAnId,
                                            NgayDieuTriStr = null,
                                            YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                            InTatCa = true,
                                            KhongHienHeader = true
                                        };
                                        html = await _dieuTriNoiTruService.InPhieuChamSocAsyncVer2(detail);
                                    }
                                }

                                #endregion

                                #region Hồ sơ khác
                                else
                                {
                                    var lstHoSoKhac = yeuCauTiepNhanNoiTru.NoiTruHoSoKhacs.Where(x => x.LoaiHoSoDieuTriNoiTru == enumHoSoKhac).ToList();
                                    foreach (var hoSoKhac in lstHoSoKhac)
                                    {
                                        var resultHTML = string.Empty;
                                        var typeLandscapeLayout = false;
                                        switch (enumHoSoKhac)
                                        {
                                            case Enums.LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong:
                                                var thongTin = JsonConvert.DeserializeObject<HoSoKhacPhieuSangLocDinhDuongVo>(hoSoKhac.ThongTinHoSo);
                                                if (thongTin.DungChoPhuNuMangThai)
                                                {
                                                    resultHTML = await _dieuTriNoiTruService.InPhieuSangLocDinhDuongPhuSan(yeuCauTiepNhanNoiTru.Id, benhAnDienTuDetailVo.Hosting);
                                                }
                                                else
                                                {
                                                    resultHTML = await _dieuTriNoiTruService.InPhieuSangLocDinhDuong(yeuCauTiepNhanNoiTru.Id, benhAnDienTuDetailVo.Hosting, (long)thongTin.NoiTruHoSoKhacId);
                                                }
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.PhieuKhaiThacTienSuDiUng:
                                                var xacNhanIn = new XacNhanInTienSu()
                                                {
                                                    NoiTruHoSoKhacId = hoSoKhac.Id,
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                                    Hosting = benhAnDienTuDetailVo.Hosting
                                                };
                                                resultHTML = await _dieuTriNoiTruService.PhieuKhaiThacTienSuBenh(xacNhanIn);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.TrichBienBanHoiChan:
                                                var xacNhanInBienBanHoiChan = new XacNhanInTrichBienBanHoiChan()
                                                {
                                                    NoiTruHoSoKhacId = hoSoKhac.Id,
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                                    Hosting = benhAnDienTuDetailVo.Hosting
                                                };
                                                resultHTML = await _dieuTriNoiTruService.BienBanHoiChan(xacNhanInBienBanHoiChan);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiChucNangSong:
                                                resultHTML = await _dieuTriNoiTruService.InPhieuTheoDoiChucNangSong(yeuCauTiepNhanNoiTru.Id);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri:
                                                var detail = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriVo>(hoSoKhac.ThongTinHoSo);
                                                var xacNhanInPhieuSoKet15NgayDieuTri = new XacNhanInTrichBienBanHoiChan()
                                                {
                                                    NoiTruHoSoKhacId = hoSoKhac.Id,
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
                                                    TuNgay = detail.TuNgay?.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"),
                                                    DenNgay = detail.DenNgay?.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"),
                                                };
                                                resultHTML = await _dieuTriNoiTruService.PhieuSoKet15NgayDieuTri(xacNhanInPhieuSoKet15NgayDieuTri);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.PhieuTomTatThongTinDieuTri:
                                                var xacNhanInPhieuTomTatThongTinDieuTri = new PhieuDieuTriVaServicesHttpParams()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
                                                    Header = false
                                                };
                                                resultHTML = await _noiTruHoSoKhacService.PhieuInThongTinDieuTriVaCacDichVu(xacNhanInPhieuTomTatThongTinDieuTri);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat:
                                                var xacNhanInBienBanHoiChanPhauThuat = new BangTheoDoiHoiTinhHttpParamsRequest()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    IdNoiTruHoSo = hoSoKhac.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
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
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
                                                    Header = false
                                                };
                                                resultHTML = await _noiTruHoSoKhacService.PhieuInBienBanCamKetPhauThuat(xacNhanInBienBanCamKetPhauThuat);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanPhauThuat:
                                                var xacNhanInBangKiemAnToanPhauThuat = new PhieuDieuTriVaServicesHttpParams()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
                                                };
                                                resultHTML = await _dieuTriNoiTruService.BangKiemAnToanNBPT(xacNhanInBangKiemAnToanNguoiBenhPhauThuat);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.PhieuKhamGayMeTruocMo:
                                                var xacNhanInPhieuKhamGayMeTruocMo = new PhieuDieuTriVaServicesHttpParams()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
                                                    Header = false
                                                };
                                                resultHTML = await _noiTruHoSoKhacService.PhieuInPhieuKhamGayMeTruocMo(xacNhanInPhieuKhamGayMeTruocMo);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.GiayTuNguyenTrietSan:
                                                resultHTML = await _dieuTriNoiTruService.InGiayTuNguyenTrietSan(yeuCauTiepNhanNoiTru.Id, benhAnDienTuDetailVo.Hosting);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetKyThuatMoi:
                                                var xacNhanInGiayCamKetKyThuatMoi = new PhieuDieuTriVaServicesHttpParams()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
                                                    Header = false
                                                };
                                                resultHTML = await _noiTruHoSoKhacService.PhieuInGiayCamKetKyThuatMoi(xacNhanInGiayCamKetKyThuatMoi);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.GiayKhamChuaBenhTheoYc:
                                                var xacNhanInGiayKhamChuaBenhTheoYc = new PhieuDieuTriVaServicesHttpParams()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
                                                    Header = false
                                                };
                                                resultHTML = await _noiTruHoSoKhacService.PhieuInGiayKhamChuaBenhTheoYc(xacNhanInGiayKhamChuaBenhTheoYc);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh:
                                                var xacNhanInBangTheoDoiHoiTinh = new BangTheoDoiHoiTinhHttpParamsRequest()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    IdNoiTruHoSo = hoSoKhac.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
                                                    LoaiPhieuIn = 1
                                                };

                                                //todo: cần update lại
                                                var resultHTMLPhieuDeNghi = await _dieuTriNoiTruService.InPhieuDeNghiTestTruocKhiDungThuoc(xacNhanInInfoPhieuDeNghiTestTruocKhiDungThuoc);
                                                htmls.Add(resultHTMLPhieuDeNghi);

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
                                                    Hosting = benhAnDienTuDetailVo.Hosting
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting
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
                                                    HostingName = benhAnDienTuDetailVo.Hosting,
                                                    Header = false
                                                };
                                                resultHTML = await _noiTruHoSoKhacService.PhieuInTomTatHoSoBenhAn(xacNhanInInfoTomTatHoSoBenhAn);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh:
                                                breakTag = "<div class=\"pagebreak\"> </div>";
                                                resultHTML = _dieuTriNoiTruService.InGiayChungSinh(hoSoKhac.Id, benhAnDienTuDetailVo.Hosting);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBenhVien:
                                                var xacNhanInInfoGiayCamKet = new XacNhanInPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    NoiTruHoSoKhacId = hoSoKhac.Id,
                                                    LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                                    Hosting = benhAnDienTuDetailVo.Hosting
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
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
                                                    Hosting = benhAnDienTuDetailVo.Hosting,
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
                                            case Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuatSuDungThuocCoDau:
                                                var phieuInBienBanHoiChan = new BangTheoDoiHoiTinhHttpParamsRequest()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    IdNoiTruHoSo = hoSoKhac.Id,
                                                    HostingName = benhAnDienTuDetailVo.Hosting
                                                };
                                                resultHTML = await _dieuTriNoiTruService.PhieuInBienBanHoiChanPhauThuatCoDau(phieuInBienBanHoiChan);
                                                break;
                                            case Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai:
                                                var giayNghiDuongThaiVo = new XacNhanInPhieuGiayChungNhanNghiDuongThai()
                                                {
                                                    YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.Id,
                                                    NoiTruHoSoKhacId = hoSoKhac.Id,
                                                    LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                                    Hosting = benhAnDienTuDetailVo.Hosting
                                                };
                                                resultHTML = await _dieuTriNoiTruService.InGiayChungNhanNghiDuongThai(giayNghiDuongThaiVo);
                                                break;

                                            default: break;
                                        }

                                        htmls.Add(resultHTML);
                                    }
                                }
                                #endregion
                            }
                            break;
                        case Enums.LoaiPhieuHoSoBenhAnDienTu.ToDieuTri:

                            if (yeuCauTiepNhanNoiTru != null)
                            {
                                var contents = await _dieuTriNoiTruService.GetContentInPhieuThamKham(yeuCauTiepNhanNoiTru.Id);
                                html += string.Join("<div class=\"pagebreak\"> </div>", contents);

                                //var lstPhieuDieuTri = yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderBy(o => o.NgayDieuTri).ToList();

                                //var countPage = 1;

                                //foreach (var phieuDieuTri in lstPhieuDieuTri)
                                //{
                                //    if (countPage != 1)
                                //    {
                                //        html += "<div style=\"break-after:page\"></div>";
                                //    }

                                //    html += await getContent(yeuCauTiepNhanNoiTru, phieuDieuTri, benhAnDienTuDetailVo.NoiTruBenhAnId);
                                //    countPage++;
                                //}
                            }
                            break;

                        default: break;
                    }

                    var nganTrangHtml = "<br><hr style =\"border-top: 1px dotted gray;\"><br>";

                    if (!string.IsNullOrEmpty(html))
                    {
                        if (!benhAnDienTuDetailVo.LaInPhieu)
                        {
                            html = html.Replace(breakTag, nganTrangHtml);
                            listHtml.Add(html);
                            //htmls.AddRange(html.Split(breakTag).Where(x => !string.IsNullOrEmpty(x)).ToList());
                        }
                        else
                        {
                            listHtml.Add(html);
                        }
                    }

                    if (htmls.Any())
                    {
                        if (!benhAnDienTuDetailVo.LaInPhieu)
                        {
                            htmls = htmls.Select(x => x.Replace(breakTag, nganTrangHtml)).ToList();
                            //htmls = htmls.SelectMany(x => x.Split(breakTag).Where(y => !string.IsNullOrEmpty(y)).ToList()).ToList();
                        }

                        listHtml.AddRange(htmls);
                    }
                }
            }


            if (!listHtml.Any())
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            //var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuKhamSucKhoe" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            //Response.ContentType = "application/pdf";
            //return new FileContentResult(bytes, "application/pdf");
            return listHtml;
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
        //        ChanDoan = "<b>"+ phieuDieuTri.ChanDoanChinhGhiChu + (cdkt.Length > 0 ? "; " + string.Join("; ", cdkt) : "") + "</b>"
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

        //            var bacSiThoiGianDienBienNull = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //                 ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
        //                     .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //                     .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";


        //            var bacSiThoiGianDienBienKhacNull = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s =>
        //                                                                      (s.TuNgay <= dienBien.ThoiGian &&
        //                                                                      (s.DenNgay == null || dienBien.ThoiGian <= s.DenNgay.Value)))
        //               ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
        //                   .Where(s =>
        //                               (s.TuNgay <= dienBien.ThoiGian && (s.DenNgay == null || dienBien.ThoiGian <= s.DenNgay.Value)))
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
        //                        dienBienBenh += "<div style='padding:1px'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </p>";
        //                    }
        //                }
        //                //Y lệnh
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
        //                                     !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
        //                                     !string.IsNullOrEmpty(item.GhiChu) ?
        //                                     "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
        //                                     item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

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
        //                             !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
        //                             !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

        //                             "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
        //                             (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
        //                             " " + (item.CachGioTruyenDich != null
        //                                 ? "cách " + item.CachGioTruyenDich + " giờ,"
        //                                 : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
        //                                 ? "giờ sử dụng: " + item.GioSuDung
        //                                 : "") + "</div>"

        //                             : "";
        //                        }
        //                        else
        //                        {
        //                            yLenh += "<div><b>" + (item.LaThuocHuongThanGayNghien == true ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + item.Ten + (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") + (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                                     (item.LaThuocHuongThanGayNghien == true
        //                                         ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                         : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";
        //                            yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : "";
        //                            yLenh += "<div style='margin-left:15px;margin-bottom:8px'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
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
        //                yLenh += "<div><b>>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div><br/>";
        //                //CHẾ ĐỘ CHĂM SÓC
        //                yLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
        //                //BÁC SĨ
        //                yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
        //                yLenh += "<p style='text-align: center;height:30px'></p>";
        //                if (dienBien.ThoiGian != null)
        //                {
        //                    yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienKhacNull + "</p>";
        //                }
        //                else
        //                {
        //                    yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienNull + "</p>";
        //                }

        //                if (i == 0)
        //                {
        //                    data.ToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\"> ";
        //                }

        //                data.ToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">>";
        //                data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //                data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //                data.ToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
        //            }
        //            ngayGio = string.Empty;
        //            dienBienBenh = string.Empty;
        //            yLenh = string.Empty;
        //            ngayGio += dienBien.ThoiGian.Hour + " giờ " + dienBien.ThoiGian.Minute + " phút, " + dienBien.ThoiGian.ApplyFormatDate();
        //            //DIỄN BIẾN
        //            dienBienBenh += "<div" + (!string.IsNullOrEmpty(dienBien?.DienBien) ? dienBien?.DienBien.Replace("\n", "<br>") : "") + " </div>";
        //            yLenh += !string.IsNullOrEmpty(dienBien?.DienBien) ? "<br>" : "";

        //            ////DVKT
        //            //dienBienBenh += "<p> <b>DVKT</b>: </p>";
        //            if (lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
        //            {
        //                foreach (var item in lstDVKT.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
        //                {
        //                    dienBienBenh += "<p style='margin-bottom:8px'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </p>";
        //                }
        //            }
        //            //Y lệnh
        //            yLenh += "<div>" + (!string.IsNullOrEmpty(dienBien?.YLenh) ? dienBien?.YLenh.Replace("\n", "<br>") : "") + " </div>";
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
        //                              !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
        //                              !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

        //                              "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
        //                              (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
        //                              " " + (item.CachGioTruyenDich != null
        //                                  ? "cách " + item.CachGioTruyenDich + " giờ,"
        //                                  : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
        //                                  ? "giờ sử dụng: " + item.GioSuDung
        //                                  : "") + "</div>"

        //                              : "";
        //                    }
        //                    else
        //                    {
        //                        yLenh += "<div><b>" + (item.LaThuocHuongThanGayNghien == true ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + item.Ten + (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") + (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                                 (item.LaThuocHuongThanGayNghien == true
        //                                     ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                     : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";
        //                        yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : "";
        //                        yLenh += "<div style='margin-left:15px;padding:1px;'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
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
        //            yLenh += "<div><b>>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div><br/>";
        //            //CHẾ ĐỘ CHĂM SÓC
        //            yLenh += "<div><b>Chế độ chắm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
        //            //BÁC SĨ
        //            yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
        //            yLenh += "<p style='text-align: center;height:30px'></p>";

        //            if (dienBien.ThoiGian != null)
        //            {
        //                yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienKhacNull + "</p>";
        //            }
        //            else
        //            {
        //                yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienNull + "</p>";
        //            }

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


        //            data.ToDieuTri += "<tr style='border-top:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">>";
        //            data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
        //            data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
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
        //        //DIỄN BIẾN
        //        //dienBienBenh += "<p><b>DIỄN BIẾN</b>: </p><br/>";
        //        ////DVKT
        //        //dienBienBenh += "<p> <b>DVKT</b>: </p>";
        //        foreach (var item in lstDVKT)
        //        {
        //            dienBienBenh += "<div style='padding:1px'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </p>";
        //        }
        //        ////Y lệnh
        //        //yLenh += "<p><b>Y LỆNH</b>: </p> <br>";
        //        //THUỐC
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
        //                                    !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
        //                                    !string.IsNullOrEmpty(item.GhiChu) ?
        //                                    "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
        //                                    item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";


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
        //                                !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
        //                                !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

        //                                "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
        //                                (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
        //                                " " + (item.CachGioTruyenDich != null
        //                                    ? "cách " + item.CachGioTruyenDich + " giờ,"
        //                                    : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
        //                                    ? "giờ sử dụng: " + item.GioSuDung
        //                                    : "") + "</div>"

        //                                : "";
        //                }
        //                else
        //                {
        //                    yLenh += "<div><b>" + (item.LaThuocHuongThanGayNghien == true ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + item.Ten + (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") + (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
        //                             (item.LaThuocHuongThanGayNghien == true
        //                                 ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
        //                                 : item.SoLuong.ToString()) + " " + item.DVT + "</b></div>";
        //                    yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : "";
        //                    yLenh += "<div style='margin-left:15px;padding:1px;'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
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
        //        yLenh += "<div><b>Chế độ ăn: </b> " + phieuDieuTri?.CheDoAn?.Ten + "</div><br/>";
        //        //CHẾ ĐỘ CHĂM SÓC
        //        yLenh += "<div><b>Chế độ chăm sóc:</b> " + phieuDieuTri?.CheDoChamSoc?.GetDescription() + "</div>";
        //        //BÁC SĨ
        //        var bacSi = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //            ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
        //                .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
        //                .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";
        //        //BÁC SĨ
        //        yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
        //        yLenh += "<p style='text-align: center;height:30px'></p>";
        //        if (string.IsNullOrEmpty(ngayGio))
        //        {
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
        #region in dịch vụ chỉ định theo mã tiếp nhận bệnh nhân

        [HttpPost("InDichVuChiDinh")]
        public async Task<ActionResult<string>> InDichVuChiDinh(InDichVuKyThuatVo inDichVuKyThuatVo)
        {
            var result = _benhAnDienTuService.InDichVuChiDinh(inDichVuKyThuatVo.Hosting, inDichVuKyThuatVo.YeuCauTiepNhanId,false);

            return Ok(result);
        }

        #endregion
    }
}
