using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Api.Models.ThongTinMienGiamThuNgan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.BenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class ThuNganController : CaminoBaseController
    {
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IBenhVienService _benhVienService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IExcelService _excelService;
        private readonly IPdfService _pdfService;

        public ThuNganController(IYeuCauTiepNhanService yeuCauTiepNhanService,
                                ITaiLieuDinhKemService taiLieuDinhKemService,
                                ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
                                IBenhVienService benhVienService,
                                IKhoaPhongService khoaPhongService,
                                IPdfService pdfService,
                                IExcelService excelService)
        {
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _khoaPhongService = khoaPhongService;
            _benhVienService = benhVienService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _excelService = excelService;
            _pdfService = pdfService;
        }

        [HttpPost("ThayDoiLoaiGiaChuaThu")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThayDoiLoaiGiaChuaThu(DoiLoaiGiaDanhSachChiPhiKhamChuaBenh doiLoaiGiaDanhSachChiPhiKhamChuaBenh)
        {
            await _yeuCauTiepNhanService.ThayDoiLoaiGiaChuaThu(doiLoaiGiaDanhSachChiPhiKhamChuaBenh);
            return Ok();
        }

        [HttpPost("ApDungMiemGiamTuBacSi")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ApDungMiemGiamTuBacSi(long yeuCauTiepNhanId)
        {
            await _yeuCauTiepNhanService.ApDungMiemGiamTuNoiGioiThieu(yeuCauTiepNhanId);
            return Ok();
        }

        [HttpPost("HuyApDungMiemGiamTuBacSi")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> HuyApDungMiemGiamTuBacSi(long yeuCauTiepNhanId)
        {
            await _yeuCauTiepNhanService.HuyApDungMiemGiamTuNoiGioiThieu(yeuCauTiepNhanId);
            return Ok();
        }


        [HttpPost("HoanUngKhongThucHienDichVu/{taiKhoanBenhNhanThuId}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult HoanUngKhongThucHienDichVu(long taiKhoanBenhNhanThuId)
        {
            return Ok(_yeuCauTiepNhanService.HoanUng(taiKhoanBenhNhanThuId));
        }

        [HttpPost("LuuTamChiPhiNgoaiTruTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> LuuTamChiPhiNgoaiTruTrongGoi(QuyetToanDichVuTrongGoiVo model)
        {
            await _yeuCauTiepNhanService.LuuTamChiPhiNgoaiTruTrongGoi(model);
            return Ok();
        }

        [HttpPost("InBangKeNgoaiTruChoThu")]
        public ActionResult<List<HtmlToPdfVo>> InBangKeNgoaiTruChoThu(ThuPhiKhamChuaBenhVo model)
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

            var htmlContent = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruChoThu(model);
            htmlToPdfVos.Add(new HtmlToPdfVo()
            {
                Html = htmlContent,
                FooterHtml = footerHtml,
                Bottom = 15
            });


            var bytes = _pdfService.ExportMultiFilePdfFromHtml(htmlToPdfVos);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeNgoaiTruChuaThu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("InBangKeNgoaiTruTrongGoiChoThu")]
        public ActionResult<List<HtmlToPdfVo>> InBangKeNgoaiTruTrongGoiChoThu(QuyetToanDichVuTrongGoiVo model)
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

            var htmlContent = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruTrongGoiChoThu(model);
            htmlToPdfVos.Add(new HtmlToPdfVo()
            {
                Html = htmlContent,
                FooterHtml = footerHtml,
                Bottom = 15
            });


            var bytes = _pdfService.ExportMultiFilePdfFromHtml(htmlToPdfVos);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeNgoaiTruChuaThu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("GetDataThuPhiNgoaiTruForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataThuPhiNgoaiTruForGridAsync([FromBody] ThuNganQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChuaThu = queryString.ChuaThu;
            var gridData = await _yeuCauTiepNhanService.GetDanhSachThuPhiNgoaiTruAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalThuPhiNgoaiTruPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetTotalThuPhiNgoaiTruPageForGridAsync([FromBody] ThuNganQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChuaThu = queryString.ChuaThu;
            var gridData = await _yeuCauTiepNhanService.GetTotalPageDanhSachThuPhiNgoaiTruAsync(queryInfo);
            return Ok(gridData);
        }

        #region Update 27/04/2021

        [HttpGet("KiemTraDichVuTrongGoiCoBHYT")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<bool> KiemTraDichVuTrongGoiCoBHYT(long yeuCauTiepNhanId)
        {
            return Ok(_yeuCauTiepNhanService.KiemTraDichVuTrongGoiCoBHYT(yeuCauTiepNhanId) > 0);
        }

        [HttpGet("SoTienQuyBHYTTTTrongGoi/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<decimal>> SoTienQuyBHYTTTTrongGoi(long id)
        {
            //var soTienQBHYTTrongGoi = _yeuCauTiepNhanService.GetDanhSachDichVuTrongGoiCoBHYTChuaQuyetToan(id).Where(o => o.KiemTraBHYTXacNhan).Select(o => o.BHYTThanhToan).DefaultIfEmpty().Sum();
            //return Ok(Math.Round(soTienQBHYTTrongGoi, MidpointRounding.AwayFromZero));
            return Ok(_yeuCauTiepNhanService.GetSoTienHoanUngTrongGoi(id));
        }

        [HttpPost("GetDataDichVuTrongGoiCoBHYTForGrid/{yeuCauTiepNhanId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public ActionResult<GridDataSource> GetDataDichVuTrongGoiCoBHYTForGrid(long yeuCauTiepNhanId)
        {
            var gridDataSource = new GridDataSource { Data = (_yeuCauTiepNhanService.GetDanhSachDichVuTrongGoiCoBHYTChuaQuyetToan(yeuCauTiepNhanId)).ToArray() };
            return Ok(gridDataSource);
        }

        [HttpPut("QuyetToanDichVuTrongGoiCoBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<KetQuaQuyetToanDichVuTrongGoiCoBHYT>> QuyetToanDichVuTrongGoiCoBHYT(QuyetToanDichVuTrongGoiVo chiTienQuyetToan)
        {
            return Ok(await _yeuCauTiepNhanService.QuyetToanDichVuTrongGoiCoBHYT(chiTienQuyetToan));
        }

        [HttpGet("KiemTraYeuCauTiepNhanCoKhuyenMai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public ActionResult<bool> KiemTraYeuCauTiepNhanCoKhuyenMai(long yeuCauTiepNhanId)
        {
            return Ok(_yeuCauTiepNhanService.KiemTraYeuCauTiepNhanCoKhuyenMai(yeuCauTiepNhanId));
        }

        [HttpGet("GetDataDichVuKhuyenMaiBenhNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataDichVuKhuyenMaiBenhNhanForGrid(long yeuCauTiepNhanId)
        {
            var gridDataSource = new GridDataSource { Data = (await _yeuCauTiepNhanService.GetDanhSachDichVuKhuyenMaiForGrid(yeuCauTiepNhanId)).ToArray() };
            return Ok(gridDataSource);
        }

        [HttpPut("ApDungDichVuKhuyenMai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public ActionResult ApDungDichVuKhuyenMai(ApDungKhuyenMaiBenhNhan ApDungKhuyenMaiBenhNhan)
        {
            _yeuCauTiepNhanService.ApDungDichVuKhuyenMai(ApDungKhuyenMaiBenhNhan);
            return Ok();
        }

        #endregion

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] ThuNganQueryInfo queryInfo)
        {
            //tam thoi khong su dung
            return null;

            //var queryString = JsonConvert.DeserializeObject<ThuNganQueryInfo>(queryInfo.AdditionalSearchString);
            //queryInfo.ChuaThu = queryString.ChuaThu;
            //var gridData = await _yeuCauTiepNhanService.GetDanhSachChuaThuNgoaiTruAsync(queryInfo, false);
            //return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] ThuNganQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChuaThu = queryString.ChuaThu;
            var gridData = await _yeuCauTiepNhanService.GetTotalPageDanhSachChuaThuNgoaiTruAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataThuTienDaHoanThanhForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataThuTienDaHoanThanhForGridAsync([FromBody] ThuNganQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChuaThu = queryString.DaThu;
            var gridData = await _yeuCauTiepNhanService.GetDanhSachDaThuNgoaiTruAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalThuTienDaHoanThanhPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetTotalThuTienDaHoanThanhPageForGridAsync([FromBody] ThuNganQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChuaThu = queryString.DaThu;

            var gridData = await _yeuCauTiepNhanService.GetTotalPageDanhSachDaThuNgoaiTruAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ThongTinThuNganBenhNhanViewModel> Get(long id)
        {
            var thongTinBenhNhan = _yeuCauTiepNhanService.GetById(id, s => s.Include(cc => cc.CongTyUuDai)
                                                                                       .Include(o => o.QuanHuyen)
                                                                                       .Include(o => o.TinhThanh)
                                                                                       .Include(o => o.PhuongXa)
                                                                                       .Include(o => o.BenhNhan)
                                                                                       .Include(o => o.DoiTuongUuDai)
                                                                                       .Include(o => o.NoiGioiThieu)
                                                                                       .Include(o => o.YeuCauTiepNhanTheBHYTs).ThenInclude(cc => cc.GiayMienCungChiTra)
                                                                                       .Include(cc => cc.GiayChuyenVien)
                                                                                       .Include(cc => cc.GiayMienGiamThem)
                                                                                       .Include(cc => cc.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                                                                                       .ThenInclude(cc => cc.CongTyBaoHiemTuNhan)
                                                                                       .Include(cc => cc.BHYTGiayMienCungChiTra));
            var benhVien = _benhVienService.GetBenhVienWithMaBenhVien(thongTinBenhNhan.BHYTMaDKBD).Result;            

            var model = thongTinBenhNhan.ToModel<ThongTinThuNganBenhNhanViewModel>();

            model.MaBN = thongTinBenhNhan.BenhNhan.MaBN;
            model.SoDienThoai = model.SoDienThoai.ApplyFormatPhone();
            model.BHYTDiaChi = benhVien != null ? benhVien.Ten : string.Empty;

            if (thongTinBenhNhan.GiayChuyenVien != null)
            {
                model.TaiLieuDinhKemGiayChuyenVien = new TaiLieuDinhKemGiayChuyenVien()
                {
                    Ten = thongTinBenhNhan.GiayChuyenVien.Ten,
                    DuongDan = thongTinBenhNhan.GiayChuyenVien.DuongDan,
                    TenGuid = thongTinBenhNhan.GiayChuyenVien.TenGuid,
                    KichThuoc = thongTinBenhNhan.GiayChuyenVien.KichThuoc,
                    LoaiTapTin = (int)thongTinBenhNhan.GiayChuyenVien.LoaiTapTin,
                };
            }

            if (thongTinBenhNhan.BHYTGiayMienCungChiTra != null)
            {
                model.TaiLieuDinhKemGiayMiemCungChiTra = new TaiLieuDinhKemGiayMiemCungChiTra()
                {
                    Ten = thongTinBenhNhan.BHYTGiayMienCungChiTra.Ten,
                    DuongDan = thongTinBenhNhan.BHYTGiayMienCungChiTra.DuongDan,
                    TenGuid = thongTinBenhNhan.BHYTGiayMienCungChiTra.TenGuid,
                    KichThuoc = thongTinBenhNhan.BHYTGiayMienCungChiTra.KichThuoc,
                    LoaiTapTin = (int)thongTinBenhNhan.BHYTGiayMienCungChiTra.LoaiTapTin,
                };
            }

            if (thongTinBenhNhan.GiayMienGiamThem != null)
            {
                model.TaiLieuDinhKemGiayMiemGiam = new TaiLieuDinhKemGiayMiemGiam
                {
                    Ten = thongTinBenhNhan.GiayMienGiamThem.Ten,
                    Id = thongTinBenhNhan.GiayMienGiamThem.Id,
                    Ma = thongTinBenhNhan.GiayMienGiamThem.Ma,
                    DuongDan = thongTinBenhNhan.GiayMienGiamThem.DuongDan,
                    TenGuid = thongTinBenhNhan.GiayMienGiamThem.TenGuid,
                    KichThuoc = thongTinBenhNhan.GiayMienGiamThem.KichThuoc,
                    LoaiTapTin = (int)thongTinBenhNhan.GiayMienGiamThem.LoaiTapTin
                };
            }

            model.DiaChi = thongTinBenhNhan.DiaChiDayDu;

            if (thongTinBenhNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                model.ThongTinBHYTNoiTrus = thongTinBenhNhan.YeuCauTiepNhanTheBHYTs
                                                            .Select(x => new ThongTinBHYTNoiTru
                                                            {
                                                                CoBHYT = x.IsCheckedBHYT,
                                                                BHYTMaSoThe = x.MaSoThe,
                                                                BHYTNgayHieuLuc = x.NgayHieuLuc,
                                                                BHYTNgayHetHan = x.NgayHetHan,
                                                                BHYTDiaChi = x.DiaChi,
                                                                BHYTMucHuong = x.MucHuong.ToString(),
                                                                TaiLieuDinhKemGiayMiemCungChiTra = x.GiayMienCungChiTra != null ? new TaiLieuDinhKemGiayMiemCungChiTra()
                                                                {
                                                                    Ten = x.GiayMienCungChiTra.Ten,
                                                                    DuongDan = x.GiayMienCungChiTra.DuongDan,
                                                                    TenGuid = x.GiayMienCungChiTra.TenGuid,
                                                                    KichThuoc = x.GiayMienCungChiTra.KichThuoc,
                                                                    LoaiTapTin = (int)x.GiayMienCungChiTra.LoaiTapTin,
                                                                } : new TaiLieuDinhKemGiayMiemCungChiTra()
                                                            }).ToList();
            }
            model.DangDieuTriNoiTru = _yeuCauTiepNhanService.KiemTraNgoaiTruCoDieuTriNoiTru(id);
            model.CoNoiGioiThieu = thongTinBenhNhan.NoiGioiThieuId != null;
            model.TenNoiGioiThieu = thongTinBenhNhan.NoiGioiThieu?.Ten;
            return Ok(model);
        }

        [HttpGet("GetThongTinMienGiam")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ThongTinMienGiamVo> GetThongTinMienGiam(long yeuCauTiepNhanId)
        {
            var thongTinMienGiamVo = _yeuCauTiepNhanService.GetThongTinMienGiam(yeuCauTiepNhanId);
            return Ok(thongTinMienGiamVo);
        }

        [HttpPost("ThemThongTinMiemGiamThemVaVoucher")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan, Enums.DocumentType.QuayThuoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<bool> ThemThongMiemGiamThemVaVoucher([FromBody]ThongTinChinhSachMiemGiamThem model)
        {
            bool res = false;
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(model.IdYeuCauTiepNhan);
            var coMienGiamThem = yeuCauTiepNhan.CoMienGiamThem ?? false;
            if (yeuCauTiepNhan != null)
            {
                yeuCauTiepNhan.CoMienGiamThem = true;
                yeuCauTiepNhan.LyDoMienGiamThem = model.LyDoMienGiam;
                yeuCauTiepNhan.GiayMienGiamThem = new Core.Domain.Entities.YeuCauTiepNhans.GiayMienGiamThem
                {
                    Ma = Guid.NewGuid().ToString(),
                    Ten = model.TaiLieuDinhKem.Ten,
                    TenGuid = model.TaiLieuDinhKem.TenGuid,
                    DuongDan = model.TaiLieuDinhKem.DuongDan,
                    KichThuoc = (long)model.TaiLieuDinhKem.KichThuoc,
                    LoaiTapTin = (Enums.LoaiTapTin)model.TaiLieuDinhKem.LoaiTapTin
                };

                yeuCauTiepNhan.NhanVienDuyetMienGiamThemId = model.NhanVienDuyetMienGiamThemId;
                _taiLieuDinhKemService.LuuTaiLieuAsync(yeuCauTiepNhan.GiayMienGiamThem.DuongDan, yeuCauTiepNhan.GiayMienGiamThem.TenGuid);
                _yeuCauTiepNhanService.Update(yeuCauTiepNhan);

                return true;
            }
            return Ok(res);
        }

        [HttpPost("XoaThongTinMienGiam")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan, Enums.DocumentType.QuayThuoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<bool> XoaThongTinMienGiam([FromBody]ThongTinChinhSachMiemGiamThem model)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(model.IdYeuCauTiepNhan);
            if (yeuCauTiepNhan != null)
            {
                yeuCauTiepNhan.GiayMienGiamThemId = null;
                _yeuCauTiepNhanService.Update(yeuCauTiepNhan);
                return true;
            }
            return Ok(false);
        }

        [HttpGet("DanhSachThuPhiDichVu/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<List<ChiPhiKhamChuaBenhVo>> GetDanhSachThuPhiDichVu(long id)
        {
            var model = _yeuCauTiepNhanService.GetDanhSachChiPhiKhamChuaBenhChuaThu(id);
            return Ok(model);
        }

        [HttpGet("GetDanhSachThuPhiDaThuPhiDichVu/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<ChiPhiKhamChuaBenhVo>>> GetDanhSachThuPhiDaThuPhiDichVu(long id)
        {
            var model = await _yeuCauTiepNhanService.GetDanhSachChiPhiKhamBenhDaThu(id);
            return Ok(model);
        }

        [HttpGet("KiemTraCoPhieuThuCongNo/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<bool>> KiemTraCoPhieuThuCongNo(long id)
        {
            var result = await _yeuCauTiepNhanService.KiemTraConPhieuThuCongNo(id);
            return Ok(result);
        }

        [HttpGet("SoTienTamUng/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<SoTienTamUngVo>> SoTienTamUng(long id)
        {
            // toDo : anh tuân model nay nha anh SoTienTamUngVo
            var soTienTamUng = await _yeuCauTiepNhanService.SoTienTamUng(id);
            return Ok(soTienTamUng);
        }

        [HttpGet("SoDuTaiKhoanBenhNhan/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan, Enums.DocumentType.QuayThuoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<int>> SoDuTaiKhoanBenhNhan(long id)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(id, c => c.Include(p => p.BenhNhan).ThenInclude(pc => pc.TaiKhoanBenhNhan));
            if (yeuCauTiepNhan.BenhNhan == null || yeuCauTiepNhan.BenhNhan.TaiKhoanBenhNhan == null)
            {
                return Ok(0);
            }
            var soDuTaiKhoanBenhNhan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
            return Ok(soDuTaiKhoanBenhNhan);
        }

        [HttpGet("SoTienUocLuongConLai/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan, Enums.DocumentType.QuayThuoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<int>> SoTienUocLuongConLai(long id)
        {
            var soTienUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(id);
            return Ok(soTienUocLuongConLai);
        }

        [HttpPost("GetThuNganByMaBNVaMaTT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<KetQuaCLS> GetThuNganByMaBNVaMaTT(TimKiemThongTinBenhNhan timKiemThongTinBenhNhan)
        {
            var ketQuaCLS = _yeuCauTiepNhanService.GetThuNganByMaBNVaMaTT(timKiemThongTinBenhNhan);
            return Ok(ketQuaCLS);
        }

        [HttpPost("ThuTienPhiBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThuTienPhiBenhNhan(ThuPhiKhamChuaBenhVo model)
        {
            //update BVHD-3584 luôn thu kèm trong gói
            var result = await _yeuCauTiepNhanService.ThuPhiKhamChuaBenhVaQuyetToanDichVuTrongGoi(model);
            //var result = await _yeuCauTiepNhanService.ThuPhiKhamChuaBenh(model);
            if (string.IsNullOrEmpty(result.Error)) return Ok(result);
            throw new ApiException(result.Error);
        }

        [HttpPost("ThuPhiKhamChuaBenhVaQuyetToanDichVuTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThuPhiKhamChuaBenhVaQuyetToanDichVuTrongGoi(ThuPhiKhamChuaBenhVo model)
        {
            var result = await _yeuCauTiepNhanService.ThuPhiKhamChuaBenhVaQuyetToanDichVuTrongGoi(model);
            if (string.IsNullOrEmpty(result.Error)) return Ok(result);
            throw new ApiException(result.Error);
        }

        [HttpPost("ThuTienTamUng")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ThuTienTamUng(ThuPhiTamUngVo model)
        {
            var result = await _yeuCauTiepNhanService.ThuTienTamUng(model);
            if (string.IsNullOrEmpty(result.Item2)) return Ok(result.Item1);
            throw new ApiException(result.Item2);
        }

        [HttpPost("TraTienBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> TraTienBenhNhan(ChiTienLaiBenhNhanVo model)
        {
            var result = await _yeuCauTiepNhanService.TraTienBenhNhan(model);
            if (string.IsNullOrEmpty(result.Item2)) return Ok(result.Item1);
            throw new ApiException(result.Item2);
        }

        [HttpPost("BenhNhanTraTien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> BenhNhanTraTien(BenhNhanTraLaiTien benhNhanTraLaiTienModel)
        {
            var result = await _yeuCauTiepNhanService.ThuNo(benhNhanTraLaiTienModel);
            if (string.IsNullOrEmpty(result.Item2)) return Ok(result.Item1);
            throw new ApiException(result.Item2);
        }

        [HttpGet("GetHtmlPhieuThu")]
        public ActionResult GetHtmlPhieuThu(long id, string hostingName)
        {
            var htmlPhieuThu = _yeuCauTiepNhanService.GetHtmlPhieuThuChiNgoaiTru(id, hostingName);
            return Ok(htmlPhieuThu);
        }

        [HttpGet("GetHtmlPhieuBangKeThuoc")]
        public ActionResult GetHtmlPhieuBangKeThuoc(long id, string hostingName)
        {
            var htmlBangBaoCaoToaThuoc = _yeuCauTiepNhanService.GetHtmlBaoCaoToaThuoc(id, hostingName);
            return Ok(htmlBangBaoCaoToaThuoc);
        }

        [HttpGet("GetHtmlPhieuThuTamUng")]
        public ActionResult GetHtmlPhieuThuTamUng(long id, string hostingName)
        {
            var htmlPhieuThuTamUng = _yeuCauTiepNhanService.GetHtmlPhieuThuTamUng(id, hostingName);
            return Ok(htmlPhieuThuTamUng);
        }

        [HttpGet("GetHtmlPhieuThuBenhNhanTraTien")]
        public ActionResult GetHtmlPhieuhuTienBenhNhan(long id, string hostingName)
        {
            var htmlPhieuThuTienBenhNhan = _yeuCauTiepNhanService.GetHtmlPhieuThuBenhNhanTraTien(id, hostingName);
            return Ok(htmlPhieuThuTienBenhNhan);
        }

        [HttpGet("GetHtmlPhieuChi")]
        public ActionResult GetHtmlPhieuChi(long id, string hostingName)
        {
            var htmlPhieuChi = _yeuCauTiepNhanService.GetHtmlPhieuChi(id, hostingName);
            return Ok(htmlPhieuChi);
        }

        [HttpPost("InPhieuThu")]
        public ActionResult<List<HtmlToPdfVo>> InPhieuThu(long taiKhoanThuId, long taiKhoanChiId, long yeuCauTiepNhanId, string hostingName, string loaiTypes, string phieuHoanUngIds)
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
                    if (loais[i] == InPhieuHoanUngStr && taiKhoanChiId != 0)
                    {
                        var phieuHoanUng = _yeuCauTiepNhanService.GetHtmlPhieuHoanUngNgoaiTru(taiKhoanChiId, hostingName);
                        var phieu = new HtmlToPdfVo()
                        {
                            Html = phieuHoanUng,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieu);
                    }

                    if (loais[i] == InPhieuHoanThuStr && taiKhoanChiId != 0)
                    {
                        var phieuHoanThuNgoaiTru = _yeuCauTiepNhanService.GetHtmlPhieuHoanThuNgoaiTru(taiKhoanChiId, hostingName);
                        var phieu = new HtmlToPdfVo()
                        {
                            Html = phieuHoanThuNgoaiTru,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieu);
                    }

                    if (loais[i] == BangKeThuocStr && taiKhoanThuId != 0)
                    {
                        var phieuThuoc = _yeuCauTiepNhanService.GetHtmlBaoCaoToaThuoc(taiKhoanThuId, hostingName);
                        var phieu = new HtmlToPdfVo()
                        {
                            Html = phieuThuoc,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieu);
                    }

                    if (loais[i] == InCPKhamNgoaiTruStr && yeuCauTiepNhanId != 0)
                    {
                        var htmlCoBHYT = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruCoBHYT(yeuCauTiepNhanId, hostingName);
                        if (!string.IsNullOrEmpty(htmlCoBHYT))
                        {
                            var bangKeCoBHYT = htmlCoBHYT;
                            var phieuBHYT = new HtmlToPdfVo()
                            {
                                Html = bangKeCoBHYT,
                                FooterHtml = footerHtml,
                                Bottom = 15
                            };
                            phieuThuNgans.Add(phieuBHYT);
                        }

                        var bangKeKhamBenh = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTru(yeuCauTiepNhanId, hostingName);
                        if (!string.IsNullOrEmpty(bangKeKhamBenh))
                        {
                            var phieu = new HtmlToPdfVo()
                            {
                                Html = bangKeKhamBenh,
                                FooterHtml = footerHtml,
                                Bottom = 15
                            };

                            phieuThuNgans.Add(phieu);
                        }
                    }

                    if (loais[i] == InBangKeChiPhiGoiKhamChuaBenhStr && yeuCauTiepNhanId != 0)
                    {
                        var bangKeChiPhiGoiKhamChuaBenh = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruTrongGoiDv(yeuCauTiepNhanId, hostingName);
                        if (!string.IsNullOrEmpty(bangKeChiPhiGoiKhamChuaBenh))
                        {
                            var phieu = new HtmlToPdfVo()
                            {
                                Html = bangKeChiPhiGoiKhamChuaBenh,
                                FooterHtml = footerHtml,
                                Bottom = 15
                            };

                            phieuThuNgans.Add(phieu);
                        }
                    }


                    if (loais[i] == InPhieuThuTamUngStr && taiKhoanThuId != 0)
                    {
                        var phieuThuTamUng = _yeuCauTiepNhanService.GetHtmlPhieuThuTamUng(taiKhoanThuId, hostingName);
                        var phieu = new HtmlToPdfVo()
                        {
                            Html = phieuThuTamUng,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieu);
                    }

                    if (loais[i] == InPhieuThuChiStr && taiKhoanThuId != 0)
                    {
                        var htmlContent = _yeuCauTiepNhanService.GetHtmlPhieuThuChiNgoaiTru(taiKhoanThuId, hostingName);
                        var phieu = new HtmlToPdfVo()
                        {
                            Html = htmlContent,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieu);
                    }

                    if (loais[i] == InPhieuChitr && taiKhoanChiId != 0)
                    {
                        var htmlContent = _yeuCauTiepNhanService.GetHtmlPhieuChiDichVuTrongGoiCoBHYT(taiKhoanChiId, hostingName);
                        var phieu = new HtmlToPdfVo()
                        {
                            Html = htmlContent,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieu);
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
                        var phieu = new HtmlToPdfVo()
                        {

                            Html = phieuHoanUng,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                            PageOrientation = PageOrientationLandscape,
                            PageSize = PageSizeA5
                        };
                        phieuThuNgans.Add(phieu);
                    }
                }
            }


            var bytes = _pdfService.ExportMultiFilePdfFromHtml(phieuThuNgans);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopPhieu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("XemNhieuPhieuThu")]
        public ActionResult<List<HtmlToPdfVo>> XemNhieuPhieuThu(string hostingName, string taiKhoanThuIds, string phieuHoanUngIds, long yeuCauTiepNhanId, string loaiTypes)
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

                                if (loaiPhieuMuonIns[i] == InCPKhamNgoaiTruStr)
                                {

                                    var bangKeChiPhiGoiKhamChuaBenh = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruTrongGoiDv(long.Parse(tkThuId), hostingName);
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

                                    var htmlCoBHYT = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruCoBHYT(long.Parse(tkThuId), hostingName);
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

                                    var bangKeKhamBenh = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTru(yeuCauTiepNhanId, hostingName);
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

        [HttpPost("XemTruocBangKeChiPhi")]
        public ActionResult<List<PhieuThuNgan>> XemTruocBangKeChiPhi(long yeuCauTiepNhanId, string hostingName)
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

            var htmlCoBHYT = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruCoBHYT(yeuCauTiepNhanId, hostingName, true);
            if (!string.IsNullOrEmpty(htmlCoBHYT))
            {
                var bangKeCoBHYT = htmlCoBHYT + "<div class=\"pagebreak\"></div>";
                var phieuBHYT = new HtmlToPdfVo()
                {
                    Html = bangKeCoBHYT,
                    FooterHtml = footerHtml,
                    Bottom = 15
                };
                phieuThuNgans.Add(phieuBHYT);
            }

            var bangKeKhamBenh = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTru(yeuCauTiepNhanId, hostingName, true);
            if (!string.IsNullOrEmpty(bangKeKhamBenh))
            {
                var phieuThuNgan = new HtmlToPdfVo()
                {
                    Html = bangKeKhamBenh,
                    FooterHtml = footerHtml,
                    Bottom = 15
                };

                phieuThuNgans.Add(phieuThuNgan);
            }

            var bangKeChiPhiGoiKhamChuaBenh = _yeuCauTiepNhanService.GetHtmlBangKeNgoaiTruTrongGoiDv(yeuCauTiepNhanId, hostingName);
            if (!string.IsNullOrEmpty(bangKeChiPhiGoiKhamChuaBenh))
            {
                var phieuThuNgan = new HtmlToPdfVo()
                {
                    Html = bangKeChiPhiGoiKhamChuaBenh,
                    FooterHtml = footerHtml,
                    Bottom = 15
                };
                phieuThuNgans.Add(phieuThuNgan);
            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(phieuThuNgans);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeChiPhi" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("GetHinhThucThanhToanPhiDescription/{loai}/{id}")]
        public List<LookupItemVo> GetHinhThucThanhToanPhiDescription(string loai, int id)
        {
            var model = new List<LookupItemVo>();
            var list = Enum.GetValues(typeof(Enums.HinhThucThanhToanPhi)).Cast<Enum>();

            if (!String.IsNullOrEmpty(loai) && loai == "tratienbn")
            {
                model = list.Where(ccs => ccs.ToString() != Enums.HinhThucThanhToanPhi.Pos.ToString() &&
                                          ccs.ToString() != Enums.HinhThucThanhToanPhi.CongNo.ToString())
                    .Select(item => new LookupItemVo
                    {
                        DisplayName = item.GetDescription(),
                        KeyId = Convert.ToInt32(item),
                    })
                    .ToList();
            }
            else
            {
                model = list.Where(ccs => ccs.ToString() != Enums.HinhThucThanhToanPhi.CongNo.ToString()
                ).Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                }).ToList();
            }

            if (!String.IsNullOrEmpty(loai) && loai == "thutien")
            {
                model = list.Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                }).ToList();
            }

            return model;
        }

        [HttpPost("GetDataLichSuThuNganForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuThuNgan)]
        public ActionResult<GridDataSource> GetDataLichSuThuNganForGridAsync([FromBody] QueryInfo queryInfo, long yeuCauTiepNhanId)
        {
            var gridData = _yeuCauTiepNhanService.GetDataForGridDanhSachLichSuThuNganAsync(queryInfo, false, yeuCauTiepNhanId);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridDanhSachLichSuThuNganNganAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuThuNgan)]
        public ActionResult<GridDataSource> GetTotalPageForGridDanhSachLichSuThuNganNganAsync([FromBody] QueryInfo queryInfo, long yeuCauTiepNhanId)
        {
            var gridData = _yeuCauTiepNhanService.GetTotalPageForGridDanhSachLichSuThuNganNganAsync(queryInfo, yeuCauTiepNhanId);
            return Ok(gridData);
        }

        [HttpGet("GetDanhSachThuNgan/{taiKhoanThuId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<ChiPhiKhamChuaBenhVo>>> GetDanhSachThuNgan(long taiKhoanThuId)
        {
            var model = await _yeuCauTiepNhanService.GetDanhSachLichSuDaThu(taiKhoanThuId);
            return Ok(model);
        }

        [HttpGet("GetDanhSachHuyThuNgan/{taiKhoanHuyId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<ChiPhiKhamChuaBenhVo>>> GetDanhSachHuyThuNgan(long taiKhoanHuyId)
        {
            var model = await _yeuCauTiepNhanService.GetDanhSachHuyThuNgan(taiKhoanHuyId);
            return Ok(model);
        }

        [HttpGet("GetThongTinThanhToan/{taiKhoanThuId}/{loaiPhieu}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ThongTinThanhToanThuChiVo>> GetThongTinThanhToan(long taiKhoanThuId, int loaiPhieu)
        {
            var model = await _yeuCauTiepNhanService.GetThongTinThanhToanThuChiVo(taiKhoanThuId, loaiPhieu);
            return Ok(model);
        }

        [HttpGet("GetDichVuThuTheoChiPhi/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<List<ChiPhiKhamChuaBenhVo>> GetDichVuThuTheoChiPhi(long id)
        {
            var model = _yeuCauTiepNhanService.GetDanhSachChiPhiKhamChuaBenhChuaThu(id);
            return Ok(model);
        }

        [HttpPost("ExportThuNgan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult> ExportThuNgan([FromBody]ThuNganQueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<ThuNganQueryInfo>(queryInfo.AdditionalSearchString);
            queryInfo.ChuaThu = queryString.ChuaThu;
            queryInfo.CongNo = queryString.CongNo;
            queryInfo.HoanUng = queryString.HoanUng;
            queryInfo.DaThu = queryString.DaThu;

            var gridData = await _yeuCauTiepNhanService.GetDataForGridThuNganAsync(queryInfo, true);
            var danhSachThuNgans = gridData.Data.Select(p => (DanhSachBenhNhanChoThuNganGridVo)p).ToList();
            var dataExcel = danhSachThuNgans.Map<List<ThuTienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(ThuTienExportExcel.MaTN), "Mã TN"),
                (nameof(ThuTienExportExcel.MaBN), "Mã BN"),
                (nameof(ThuTienExportExcel.HoTen), "Họ Tên"),
                (nameof(ThuTienExportExcel.NamSinh), "Năm Sinh"),
                (nameof(ThuTienExportExcel.GioiTinhStr), "Giới Tính"),
                (nameof(ThuTienExportExcel.DoiTuong), "Đối Tượng"),
                (nameof(ThuTienExportExcel.SoTienTamUng), "Số Tiền Tạm Ứng"),
                (nameof(ThuTienExportExcel.SoTienDuTaiKhoan), "Số Dư Tài Khoản"),
                (nameof(ThuTienExportExcel.SoTienBNDaTT), "Số Tiền Đã Thu"),
                (nameof(ThuTienExportExcel.SoTienBNPhaiTT), "Số Tiền Phải Thu"),
                (nameof(ThuTienExportExcel.Status), "Trạng Thái"),
                (nameof(ThuTienExportExcel.ThoiDiemTiepNhanDisplay), "Tiếp Nhận Lúc")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Thu Ngân", 2, "Thu Ngân");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThuNgan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportLichSuThuNgan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuThuNgan)]
        public ActionResult ExportLichSuThuNgan([FromBody]QueryInfo queryInfo)
        {
            GridDataSource gridData = _yeuCauTiepNhanService.GetDataForGridDanhSachLichSuThuNganAsync(queryInfo, true);
            var danhSachThuNgans = gridData.Data.Select(p => (DanhSachLichSuThuNganGridVo)p).ToList();
            var dataExcel = danhSachThuNgans.Map<List<LichSuThuTienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(LichSuThuTienExportExcel.SoBLHD), "Số BLHD"),
                (nameof(LichSuThuTienExportExcel.MaBN), "Mã BN"),
                (nameof(LichSuThuTienExportExcel.HoTen), "Họ Tên"),
                (nameof(LichSuThuTienExportExcel.NgayThuStr), "Ngày Thực Hiện"),
                (nameof(LichSuThuTienExportExcel.NguoiThu), "Người Thực Hiện"),
                (nameof(LichSuThuTienExportExcel.LyDoHuy), "Lý Do Hủy"),
                (nameof(LichSuThuTienExportExcel.ThuChiTienBenhNhanStr), "Loại Thanh Toán"),
                (nameof(LichSuThuTienExportExcel.SoTienThu), "Số Tiền"),
                (nameof(LichSuThuTienExportExcel.TienMat), "Tiền Mặt"),
                (nameof(LichSuThuTienExportExcel.ChuyenKhoan), "Chuyển Khoản"),
                (nameof(LichSuThuTienExportExcel.Pos), "Pos")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lịch Sử Thu Ngân", 2, "Lịch Sử Thu Ngân");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuThuNgan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportDVThuNganDaHoanThanh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult> ExportDVThuNganDaHoanThanh([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetDataForGridDanhSachThuNganDaHoanThanhAsync(queryInfo, true);
            var danhSachThuNgans = gridData.Data.Select(p => (DanhSachBenhNhanChoThuNganGridVo)p).ToList();
            var dataExcel = danhSachThuNgans.Map<List<ThuTienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(ThuTienExportExcel.MaTN), "Mã TN"),
                (nameof(ThuTienExportExcel.MaBN), "Mã BN"),
                (nameof(ThuTienExportExcel.HoTen), "Họ Tên"),
                (nameof(ThuTienExportExcel.NamSinh), "Năm Sinh"),
                (nameof(ThuTienExportExcel.GioiTinhStr), "Giới Tính"),
                (nameof(ThuTienExportExcel.DoiTuong), "Đối Tượng"),
                (nameof(ThuTienExportExcel.SoTienBNDaTT), "Số Tiền Đã Thu")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Dịch Vụ Thu Ngân Đã Hoàn Thành", 2, "Dịch Vụ Thu Ngân Đã Hoàn Thành");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DVThuNganDaHoanThanh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetDataThuChiTienForThuNganAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataThuChiTienForThuNganAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetDataThuChiTienForThuNganAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataChiTietChiThuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataChiTietChiThuForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetDataDanhSachDaThuTheoSoHDAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("KiemTraThongTinMaVoucher")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult KiemTraThongTinMaVoucher(string maVoucher, long yeuCauTiepNhanId, long benhNhanId)
        {
            var result = _yeuCauTiepNhanService.KiemTraThongTinVoucher(maVoucher, yeuCauTiepNhanId, benhNhanId);
            return Ok(result);
        }

        [HttpGet("GetMaVoucher")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<List<ThongTinVoucherVo>> GetMaVoucher(long yeucauTiepNhanId)
        {
            var getThongTinVouchers = _yeuCauTiepNhanService.GetThongTinVouchers(yeucauTiepNhanId);
            return Ok(getThongTinVouchers);
        }

        [HttpPost("KiemTraTheVoucherSuDung")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult KiemTraTheVoucherSuDung([FromBody]ThongTinVoucherTheoYeuCauTiepNhan model)
        {
            var getThongTinVouchers = _yeuCauTiepNhanService.DeleteVouchers(model);
            return Ok(getThongTinVouchers);
        }

        [HttpGet("KiemTraYeuCauNhapVien/{yeuCauTiepNhanId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> KiemTraYeuCauNhapVien(long yeuCauTiepNhanId)
        {
            var result = await _yeuCauTiepNhanService.KiemTraYeuCauNhapVien(yeuCauTiepNhanId);
            return Ok(result);
        }

        [HttpGet("ChuyenVaoNoiTru/{yeuCauTiepNhanId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ChuyenVaoNoiTru(long yeuCauTiepNhanId)
        {
            var result = await _yeuCauTiepNhanService.ChuyenVaoNoiTru(yeuCauTiepNhanId);
            return Ok(result);
        }

        [HttpGet("KiemTraSuDungGoi/{yeuCauTiepNhanId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> KiemTraSuDungGoi(long yeuCauTiepNhanId)
        {
            var result = _yeuCauTiepNhanService.KiemTraSuDungGoi(yeuCauTiepNhanId);
            return Ok(result);
        }


        [HttpGet("KiemTraPhieuThuCoBHYT/{yeuCauTiepNhanId}")]
        public ActionResult<(long, string)> KiemTraPhieuThuCoBHYT(long yeuCauTiepNhanId)
        {
            var lookup = _yeuCauTiepNhanService.KiemTraPhieuThuCoBHYT(yeuCauTiepNhanId);
            return Ok(lookup);
        }

        [HttpGet("KiemTraPhieuThuGoiCoBHYT/{yeuCauTiepNhanId}")]
        public ActionResult<(long, string)> KiemTraPhieuThuGoiCoBHYT(long yeuCauTiepNhanId)
        {
            var lookup = _yeuCauTiepNhanService.KiemTraPhieuThuGoiCoBHYT(yeuCauTiepNhanId);
            return Ok(lookup);
        }

        [HttpPost("GetSoPhieuNgoaiTru/{yeuCauTiepNhanId}")]
        public async Task<ActionResult<ICollection<ThongTinPhieuThuVo>>> GetSoPhieuNgoaiTru([FromBody]DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var lookup = await _yeuCauTiepNhanService.GetSoPhieu(queryInfo, yeuCauTiepNhanId);
            return Ok(lookup);
        }

        [HttpGet("GetThongTinPhieuThu/{soPhieu}/{loaiPhieu}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ThongTinPhieuThu> GetThongTinPhieuThu(long soPhieu, LoaiPhieuThuChiThuNgan loaiPhieu)
        {
            var phieuThu = _yeuCauTiepNhanService.GetThongTinPhieuThu(soPhieu, loaiPhieu);
            return Ok(phieuThu);
        }

        [HttpPost("HuyPhieuThu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult HuyPhieuThu(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            thongTinHuyPhieuVo.LoaiPhieuThuChiThuNgan = huyPhieuViewModel.LoaiPhieuThuChiThuNgan;
            _yeuCauTiepNhanService.HuyPhieu(thongTinHuyPhieuVo);
            return Ok();
        }

        [HttpPost("HoanPhieuThu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult HoanPhieuThu(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            thongTinHuyPhieuVo.LoaiPhieuThuChiThuNgan = huyPhieuViewModel.LoaiPhieuThuChiThuNgan;
            _yeuCauTiepNhanService.HuyPhieu(thongTinHuyPhieuVo);
            return Ok();
        }

        [HttpPost("CapnhatNguoiThuHoiPhieuThu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult CapnhatNguoiThuHoiPhieuThu(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            _yeuCauTiepNhanService.CapnhatNguoiThuHoiPhieuThu(thongTinHuyPhieuVo);
            return Ok();
        }

        [HttpPost("LuuTamChiPhiNgoaiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> LuuTamChiPhiNgoaiTru(ThuPhiKhamChuaBenhVo model)
        {
            await _yeuCauTiepNhanService.LuuTamChiPhiNgoaiTru(model);
            return Ok();
        }

        [HttpPost("HoanThuTheoDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> HoanThuTheoDichVu(ChiPhiKhamChuaBenhVo chiPhiKhamChuaBenhNoiTruVo)
        {
            var result = await _yeuCauTiepNhanService.HoanThuDichVu(chiPhiKhamChuaBenhNoiTruVo);
            if (string.IsNullOrEmpty(result.Item2)) return Ok(result.Item1);
            throw new ApiException(result.Item2);
        }

        [HttpPost("ChuyenTamUng/{taiKhoanBenhNhanThuId}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult ChuyenTamUng(long taiKhoanBenhNhanThuId)
        {
            _yeuCauTiepNhanService.ChuyenPhieuThuQuaTamUng(taiKhoanBenhNhanThuId);
            return Ok();
        }

        #region Static Value

        private readonly string InPhieuThuChiStr = "InPhieuThu";
        private readonly string InPhieuHoanUngStr = "InPhieuHoanUng";
        private readonly string InPhieuHoanThuStr = "InPhieuHoanThu";
        private readonly string BangKeThuocStr = "BangKeThuoc";
        private readonly string InCPKhamNgoaiTruStr = "InCPKhamNgoaiTru";
        private readonly string InBangKeChiPhiGoiKhamChuaBenhStr = "InBangKeChiPhiGoiKhamChuaBenhStr";
        private readonly string InPhieuThuTamUngStr = "InPhieuThuTamUng";
        private readonly string InPhieuChitr = "InPhieuChi";

        private readonly string PageSizeA5 = "A5";
        private readonly string PageOrientationLandscape = "Landscape";

        #endregion
    }
}