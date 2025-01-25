using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DuyetKetQuaXetNghiems;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Core.Helpers;
using Camino.Services.DuyetKetQuaXetNghiems;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public class DuyetKetQuaXetNghiemController : CaminoBaseController
    {
        private readonly IDuyetKetQuaXetNghiemService _duyetKqXetNghiemService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IExcelService _excelService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IPdfService _pdfService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;

        public DuyetKetQuaXetNghiemController(
            IDuyetKetQuaXetNghiemService duyetKqXetNghiemService,
            IExcelService excelService,
            IUserAgentHelper userAgentHelper,
            IPdfService pdfService,
            IYeuCauTiepNhanService yeuCauTiepNhanService
            , ITiepNhanBenhNhanService tiepNhanBenhNhanService
        )
        {
            _duyetKqXetNghiemService = duyetKqXetNghiemService;
            _excelService = excelService;
            _userAgentHelper = userAgentHelper;
            _pdfService = pdfService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _duyetKqXetNghiemService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _duyetKqXetNghiemService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChildrenAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataChildrenAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _duyetKqXetNghiemService.GetDataChildrenAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageChildrenAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageChildrenAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _duyetKqXetNghiemService.GetTotalPageChildrenAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("InPhieuDuyetKetQua")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult<List<string>>> InPhieuDuyetKetQua(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn)
        {
            var lstPhieuIn = new List<PhieuInXetNghiemModel>();
            var result = await _duyetKqXetNghiemService.InDuyetKetQuaXetNghiem(ketQuaXetNghiemPhieuIn);
            lstPhieuIn.AddRange(result);
            var lstHtml = new List<string>();

            var typeSize = "A4";
            var typeLayout = "portrait";

            var i = 0;
            foreach (var phieuIn in lstPhieuIn)
            {
                var htmlContent = "";
                htmlContent +=
                    "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                htmlContent += "</head><body>";
                htmlContent += phieuIn.Html;
                i++;
                if (i < lstPhieuIn.Count)
                {
                    htmlContent += "<div class='pagebreak'></div>";
                }
                htmlContent += "</body></html>";
                lstHtml.Add(htmlContent);
            }

            return lstHtml;
        }

        [HttpPost("DownloadDuyetKetQuaXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> DownloadDuyetKetQuaXetNghiem(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn)
        {
            var lstPhieuIn = await _duyetKqXetNghiemService.InDuyetKetQuaXetNghiem(ketQuaXetNghiemPhieuIn);
            var htmlContent = "";
            var typeSize = "A4";
            var typeLayout = "portrait";
            htmlContent +=
                "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
            htmlContent += "</head><body>";
            foreach (var phieuIn in lstPhieuIn)
            {
                htmlContent += phieuIn.Html + "<div class='pagebreak'></div>";
            }
            htmlContent += "</body></html>";

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
                Html = htmlContent,
                FooterHtml = footerHtml
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetKetQuaXetNghiem" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("ExportDuyetKetQuaXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> ExportDuyetKetQuaXetNghiem(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var gridData = await _duyetKqXetNghiemService.GetDataForGridAsync(queryInfo);
            var data = gridData.Data.Select(p => (DuyetKetQuaXetNghiemGridVo)p).ToList();
            var dataExcel = data.Map<List<DuyetKetQuaXetNghiemExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort
            {
                Field = "MaDv",
                Dir = "asc"
            });

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _duyetKqXetNghiemService.GetDataChildrenAsync(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (DuyetKetQuaXetNghiemDetailGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetKetQuaXetNghiemExportExcelChild>>();
                item.DuyetKetQuaXetNghiemExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetKetQuaXetNghiemExportExcel.Barcode), "Barcode"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.MaTn), "Mã TN"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.MaBn), "Mã BN"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.HoTen), "Họ Tên"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.GioiTinhDisplay), "Giới Tính"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.NamSinh), "Năm Sinh"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.DiaChi), "Địa Chỉ"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.TrangThaiDisplay), "Trạng Thái"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.NguoiThucHien), "Người Thực Hiện"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.NgayThucHienDisplay), "Ngày Thực Hiện"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.NguoiDuyetKq), "Người Duyệt KQ"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.NgayDuyetKqDisplay), "Ngày Duyệt KQ"),
                (nameof(DuyetKetQuaXetNghiemExportExcel.DuyetKetQuaXetNghiemExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Danh Sách Duyệt Kết Quả Xét Nghiệm", labelName: "Danh Sách Duyệt Kết Quả Xét Nghiệm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetKetQuaXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";


            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        //get dataIn
        [HttpPost("GetDataXetNghiemKetQuaIn")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetKetQuaXetNghiemViewModel>> GetDataXetNghiemKetQuaIn(long yeuCauTiepNhanId, long? phienXetNghiemId, bool? inDanhSachLayMauDaCoKetQua)
        {
            List<DuyetKetQuaXetNghiemViewModel> list = new List<DuyetKetQuaXetNghiemViewModel>();
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, x => x.Include(y => y.PhienXetNghiems).ThenInclude(z => z.PhienXetNghiemChiTiets).ThenInclude(s => s.NhanVienKetLuan));
            if (yeuCauTiepNhan.PhienXetNghiems.Any())
            {
                if (phienXetNghiemId != null)
                {
                    foreach (var item in yeuCauTiepNhan.PhienXetNghiems.Where(s => s.Id == phienXetNghiemId).ToList())
                    {
                        var resultDatas = await AddPhien(item.Id);
                        list.Add(resultDatas.Value);
                    }
                }
                if (phienXetNghiemId == null)
                {
                    if (inDanhSachLayMauDaCoKetQua != null) // get list phiên đã có kết quả lấy mẫu
                    {
                        foreach (var item in yeuCauTiepNhan.PhienXetNghiems.Where(s => s.PhienXetNghiemChiTiets.Any(p => p.NhanVienKetLuanId != null)).ToList())
                        {
                            var resultDatas = await AddPhien(item.Id);
                            list.Add(resultDatas.Value);

                        }
                    }
                    if (inDanhSachLayMauDaCoKetQua == null)
                    {
                        foreach (var item in yeuCauTiepNhan.PhienXetNghiems.ToList())
                        {

                            var resultDatas = await AddPhien(item.Id);
                            list.Add(resultDatas.Value);
                        }
                    }
                }
            }

            return Ok(list);
        }
        private async Task<ActionResult<DuyetKetQuaXetNghiemViewModel>> AddPhien(long phienXetNghiemId)
        {
            var result = await _duyetKqXetNghiemService.GetByIdAsync(phienXetNghiemId
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
                  );
            if (result == null)
                return NotFound();

            var yeuCauKhamBenhs = result.YeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && cc.IcdchinhId != null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh).ToList();

            var chanDoans = yeuCauKhamBenhs.Select(cc => cc.Icdchinh).Select(p => p.TenTiengViet);

            var resultData = new DuyetKetQuaXetNghiemViewModel
            {
                Id = phienXetNghiemId,
                YeuCauTiepNhanId = result.YeuCauTiepNhanId,
                MaSoBHYT = result.YeuCauTiepNhan.BHYTMaSoThe,
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
            };

            if (result.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiChiDinhId != null))
            {
                var yckb = result.YeuCauTiepNhan.YeuCauKhamBenhs
                    .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiChiDinhId != null).ToList();
                resultData.Phong = yckb.Select(p => p.NoiChiDinh).Select(p => p.Ten).Distinct().Join(" , ");
                resultData.KhoaChiDinh = yckb.Select(p => p.NoiChiDinh).Select(p => p.KhoaPhong).Select(p => p.Ten).Distinct().Join(" , ");
            }

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
                var phienXetNghiemChiTietLast = result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).OrderBy(s => s.Id).LastOrDefault();
                if (phienXetNghiemChiTietLast != null && phienXetNghiemChiTietLast.NhanVienKetLuanId != null)
                {
                    var res = phienXetNghiemChiTietLast.KetQuaXetNghiemChiTiets.ToList();
                    listChiTiet.AddRange(res);
                }
                //if (!result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId && p.NhanVienKetLuanId != null).OrderBy(s=>s.Id).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                //var res = result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId && p.NhanVienKetLuanId != null).OrderBy(s => s.Id).Last().KetQuaXetNghiemChiTiets.ToList();
                //listChiTiet.AddRange(res);
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
                    var mauXetNghiem = _duyetKqXetNghiemService.GetById(phienXetNghiemId, p => p.Include(o => o.MauXetNghiems))
                        .MauXetNghiems
                        .Where(p => p.LoaiMauXetNghiem.GetDescription() == loaiMau && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).LastOrDefault();
                    if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
                    {
                        lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
                    }
                }

                detail.DanhSachLoaiMau = lstTongCong;
                detail.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
            }
            return resultData;
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetKetQuaXetNghiemViewModel>> Get(long id)
        {
            var phienXetNghiemData = _duyetKqXetNghiemService.GetPhienXetNghiemData(id);
            if (phienXetNghiemData == null)
                return NotFound();

            var resultData = new DuyetKetQuaXetNghiemViewModel
            {
                Id = id,
                YeuCauTiepNhanId = phienXetNghiemData.YeuCauTiepNhanId,
                MaSoBHYT = phienXetNghiemData.MaSoBHYT,
                CoBHYT = phienXetNghiemData.CoBHYT,
                BarCodeID = phienXetNghiemData.BarCodeID,
                MaYeuCauTiepNhan = phienXetNghiemData.MaYeuCauTiepNhan,
                MaBN = phienXetNghiemData.MaBN,
                HoTen = phienXetNghiemData.HoTen,
                NgaySinh = phienXetNghiemData.NgaySinh,
                ThangSinh = phienXetNghiemData.ThangSinh,
                NamSinh = phienXetNghiemData.NamSinh,
                GioiTinh = phienXetNghiemData.GioiTinh,
                BHYTMucHuong = phienXetNghiemData.BHYTMucHuong,
                LoaiYeuCauTiepNhan = phienXetNghiemData.LoaiYeuCauTiepNhan,
                SoDienThoai = phienXetNghiemData.SoDienThoai,
                DiaChi = phienXetNghiemData.DiaChi,
                ChanDoan = phienXetNghiemData.ChanDoan,
                NguoiThucHienId = phienXetNghiemData.NguoiThucHienId,
                NguoiThucHien = phienXetNghiemData.NguoiThucHien,
                GhiChu = phienXetNghiemData.GhiChu,
                TenCongTy = phienXetNghiemData.TenCongTy,
                LaCapCuu = phienXetNghiemData.LaCapCuu,
                Phong = phienXetNghiemData.Phong,
                KhoaChiDinh = phienXetNghiemData.KhoaChiDinh,
                TrangThai = phienXetNghiemData.TrangThai
            };


            var lstYeuCauDichVuKyThuatId = phienXetNghiemData.PhienXetNghiemChiTietDataVos.Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();

            foreach (var ycId in lstYeuCauDichVuKyThuatId)
            {
                if (!phienXetNghiemData.PhienXetNghiemChiTietDataVos.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                var res = phienXetNghiemData.PhienXetNghiemChiTietDataVos.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                listChiTiet.AddRange(res);
            }

            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            resultData.ChiTietKetQuaXetNghiems = AddDetailDataChildV2(phienXetNghiemData, _duyetKqXetNghiemService.GetTenMayXetNghiems(), _duyetKqXetNghiemService.GetTenNhanViens(), _duyetKqXetNghiemService.GetTenDichVuXetNghiems()
                , listChiTiet, listChiTiet, new List<DuyetKqXetNghiemChiTietViewModel>(), true);


            // BVHD-3919
            if (resultData.ChiTietKetQuaXetNghiems.Count() != 0)
            {
                foreach (var item in resultData.ChiTietKetQuaXetNghiems)
                {
                    item.NguoiThucHien = phienXetNghiemData.NguoiThucHien;
                }
            }
            // 


            var dichVuSarV2Id2 = await _duyetKqXetNghiemService.CauHinhDichVuTestSarsCovids();
            var dropDownModel = new DropDownListRequestModel();
            var loaiKitThus = await _duyetKqXetNghiemService.DichVuTestSarsCovids(dropDownModel);
            foreach (var detail in resultData.ChiTietKetQuaXetNghiems)
            {
                detail.DanhSachLoaiMauDaCoKetQua = resultData.ChiTietKetQuaXetNghiems
                    .Where(p => p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).Select(p => p.LoaiMau)
                    .Distinct().ToList();

                if (dichVuSarV2Id2.FirstOrDefault(c => c.DichVuKyThuatBenhVienId == detail.DichVuKyThuatBenhVienId) != null)
                {
                    detail.LaDichVuSarCovid2 = true;
                }
                if (loaiKitThus.FirstOrDefault(c => c.DisplayName == detail.LoaiKitThu) != null)
                {
                    detail.LoaiKitThuId = loaiKitThus.FirstOrDefault(c => c.DisplayName.Contains(detail.LoaiKitThu))?.KeyId;
                }
                var lstTongCong = phienXetNghiemData.PhienXetNghiemChiTietDataVos
                    .Where(p => p.LoaiMauXetNghiem != null && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
                    .Select(p => p.LoaiMauXetNghiem.GetDescription())
                    .Distinct().ToList();


                var lstLoaiMauKhongDat = new List<string>();

                //foreach (var loaiMau in lstTongCong)
                //{
                //    var mauXetNghiem = result.MauXetNghiems
                //        .Where(p => p.LoaiMauXetNghiem.GetDescription() == loaiMau && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).LastOrDefault();
                //    if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
                //    {
                //        lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
                //    }
                //}

                detail.DanhSachLoaiMau = lstTongCong;
                detail.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
                detail.IsParent = detail.DaGoiDuyet == true && detail.IdChilds.Count == 0;
            }

            #region BVHD-3941
            if (phienXetNghiemData.CoBHTN == true)
            {
                resultData.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(phienXetNghiemData.YeuCauTiepNhanId).Result;
            }
            #endregion

            return Ok(resultData);
        }

        //[HttpGet("{id}")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<ActionResult<DuyetKetQuaXetNghiemViewModel>> Get(long id)
        //{
        //    //var result = _duyetKqXetNghiemService.GetById(id
        //    //    ,
        //    //    u => u
        //    //    .Include(x => x.MauXetNghiems)

        //    //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
        //    //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiThucHien)
        //    //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
        //    //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.CongTyKhamSucKhoe)
        //    //    .Include(x => x.BenhNhan)
        //    //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
        //    //    //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
        //    //    .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)                
        //    //    .Include(o => o.MauXetNghiems)

        //    //    //BVHD-3800
        //    //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
        //    //    );


        //    var result = _duyetKqXetNghiemService.GetChiTietById(id);
        //    if (result == null)
        //        return NotFound();



        //    var chanDoans = result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
        //        (result.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
        //            ? result.YeuCauTiepNhan?.YeuCauKhamBenhs?
        //                .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
        //                .Select(p => p.ChanDoanSoBoGhiChu).ToList().Distinct().Join(" ; ")
        //            : "") :
        //        (result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
        //            result.PhienXetNghiemChiTiets.Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().Join(" ; ") :
        //            (result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "Khám sức khỏe" : ""));

        //    var resultData = new DuyetKetQuaXetNghiemViewModel
        //    {
        //        Id = id,
        //        YeuCauTiepNhanId = result.YeuCauTiepNhanId,
        //        MaSoBHYT = result.YeuCauTiepNhan.BHYTMaSoThe,
        //        CoBHYT = result.YeuCauTiepNhan.CoBHYT,
        //        BarCodeID = result.BarCodeId,
        //        MaYeuCauTiepNhan = result.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //        MaBN = result.BenhNhan.MaBN,
        //        HoTen = result.YeuCauTiepNhan.HoTen,
        //        NgaySinh = result.YeuCauTiepNhan.NgaySinh,
        //        ThangSinh = result.YeuCauTiepNhan.ThangSinh,
        //        NamSinh = result.YeuCauTiepNhan.NamSinh,
        //        GioiTinh = result.YeuCauTiepNhan.GioiTinh,
        //        BHYTMucHuong = result.YeuCauTiepNhan.BHYTMucHuong,
        //        LoaiYeuCauTiepNhan = result.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
        //        SoDienThoai = result.YeuCauTiepNhan.SoDienThoaiDisplay ?? result.YeuCauTiepNhan.NguoiLienHeSoDienThoai?.ApplyFormatPhone(),
        //        DiaChi = result.YeuCauTiepNhan.DiaChiDayDu,
        //        ChanDoan = chanDoans,
        //        NguoiThucHienId = result.NhanVienThucHienId,
        //        NguoiThucHien = result.NhanVienThucHien?.User.HoTen,
        //        GhiChu = result.GhiChu,
        //        //BVHD-3364
        //        TenCongTy = result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? result.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : null,

        //        //BVHD-3800
        //        LaCapCuu = result.YeuCauTiepNhan.LaCapCuu ?? result.YeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu
        //    };

        //    resultData.Phong = result.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.Ten).Distinct().Join(" , ");
        //    resultData.KhoaChiDinh = result.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.KhoaPhong).Select(p => p.Ten).Distinct().Join(" , ");

        //    if (result.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan != null))
        //    {
        //        resultData.TrangThai = null;
        //    }
        //    else if (result.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))))
        //    {
        //        resultData.TrangThai = true;
        //    }
        //    else
        //    {
        //        resultData.TrangThai = false;
        //    }

        //    var lstYeuCauDichVuKyThuatId = result.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

        //    var listChiTiet = new List<KetQuaXetNghiemChiTiet>();

        //    foreach (var ycId in lstYeuCauDichVuKyThuatId)
        //    {
        //        if (!result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
        //        var res = result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
        //        listChiTiet.AddRange(res);
        //    }

        //    listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
        //    resultData.ChiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<DuyetKqXetNghiemChiTietViewModel>(), true);


        //    // BVHD-3919
        //    if (resultData.ChiTietKetQuaXetNghiems.Count() != 0)
        //    {
        //        foreach (var item in resultData.ChiTietKetQuaXetNghiems)
        //        {
        //            item.NguoiThucHien = result?.NhanVienThucHien?.User?.HoTen;
        //        }
        //    }
        //    // 


        //    var dichVuSarV2Id2 = await _duyetKqXetNghiemService.CauHinhDichVuTestSarsCovids();
        //    var dropDownModel = new DropDownListRequestModel();
        //    var loaiKitThus = await _duyetKqXetNghiemService.DichVuTestSarsCovids(dropDownModel);
        //    foreach (var detail in resultData.ChiTietKetQuaXetNghiems)
        //    {
        //        detail.DanhSachLoaiMauDaCoKetQua = resultData.ChiTietKetQuaXetNghiems
        //            .Where(p => p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).Select(p => p.LoaiMau)
        //            .Distinct().ToList();

        //        if (dichVuSarV2Id2.FirstOrDefault(c => c.DichVuKyThuatBenhVienId == detail.DichVuKyThuatBenhVienId) != null)
        //        {
        //            detail.LaDichVuSarCovid2 = true;
        //        }
        //        if (loaiKitThus.FirstOrDefault(c => c.DisplayName == detail.LoaiKitThu) != null)
        //        {
        //            detail.LoaiKitThuId = loaiKitThus.FirstOrDefault(c => c.DisplayName.Contains(detail.LoaiKitThu))?.KeyId;
        //        }
        //        var lstTongCong = result.YeuCauTiepNhan.YeuCauDichVuKyThuats
        //            .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
        //            .Select(p => p.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription())
        //            .Distinct().Where(p => p != null).ToList();


        //        var lstLoaiMauKhongDat = new List<string>();

        //        foreach (var loaiMau in lstTongCong)
        //        {
        //            var mauXetNghiem = result.MauXetNghiems
        //                .Where(p => p.LoaiMauXetNghiem.GetDescription() == loaiMau && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).LastOrDefault();
        //            if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
        //            {
        //                lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
        //            }
        //        }

        //        detail.DanhSachLoaiMau = lstTongCong;
        //        detail.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
        //        detail.IsParent = detail.DaGoiDuyet == true && detail.IdChilds.Count == 0;
        //    }

        //    #region BVHD-3941
        //    if (result.YeuCauTiepNhan?.CoBHTN == true)
        //    {
        //        resultData.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.YeuCauTiepNhanId).Result;
        //    }
        //    #endregion

        //    return Ok(resultData);
        //}

        private List<DuyetKqXetNghiemChiTietViewModel> AddDetailDataChildV2(PhienXetNghiemDataVo phienXetNghiemDataVo, List<LookupItemVo> tenMayXetNghiems, List<LookupItemVo> tenNhanViens, List<LookupItemVo> tenDichVuXetNghiems, List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai
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
                    var phienXetNghiemChiTietVo = phienXetNghiemDataVo.PhienXetNghiemChiTietDataVos.First(o => o.Id == parent.PhienXetNghiemChiTietId);
                    var ketQua = new DuyetKqXetNghiemChiTietViewModel
                    {
                        Id = parent.Id,
                        Ten = phienXetNghiemChiTietVo.TenDichVu,
                        YeuCauTiepNhanId = phienXetNghiemDataVo.YeuCauTiepNhanId,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        DichVuKyThuatBenhVienId = parent.DichVuKyThuatBenhVienId,
                        LoaiKitThu = phienXetNghiemChiTietVo.LoaiKitThu,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        Csbt = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        GiaTriMin = parent.GiaTriMin,
                        GiaTriMax = parent.GiaTriMax,
                        DonVi = parent.DonVi,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = tenMayXetNghiems.FirstOrDefault(o=>o.KeyId == parent.MayXetNghiemId)?.DisplayName,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = tenNhanViens.FirstOrDefault(o => o.KeyId == parent.NhanVienDuyetId)?.DisplayName,
                        LoaiMau = phienXetNghiemChiTietVo.LoaiMauXetNghiem?.GetDescription(),
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        DaGoiDuyet = phienXetNghiemChiTietVo.DaGoiDuyet,
                        //structure tree
                        Level = level,
                        //YeuCauChayLai = parent.PhienXetNghiemChiTiet?.ChayLaiKetQua,
                        DaDuyet = phienXetNghiemChiTietVo.DaGoiDuyet == true && phienXetNghiemChiTietVo.ThoiDiemKetLuan != null,
                        //NguoiYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        //LyDoYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        //NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        //NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        //NgayDuyetDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet != null
                        //    ? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                        //    : "",
                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                        LoaiKetQuaNhapTay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriNhapTay),
                        Nhom = phienXetNghiemChiTietVo.TenNhomDichVuBenhVien,
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
                    var phienXetNghiemChiTietVo = phienXetNghiemDataVo.PhienXetNghiemChiTietDataVos.First(o => o.Id == parent.PhienXetNghiemChiTietId);
                    var ketQua = new DuyetKqXetNghiemChiTietViewModel
                    {
                        Id = parent.Id,
                        Ten = tenDichVuXetNghiems.FirstOrDefault(o => o.KeyId == parent.DichVuXetNghiemId)?.DisplayName,
                        YeuCauTiepNhanId = phienXetNghiemDataVo.YeuCauTiepNhanId,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        DichVuKyThuatBenhVienId = parent.DichVuKyThuatBenhVienId,
                        LoaiKitThu = phienXetNghiemChiTietVo.LoaiKitThu,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        Csbt = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        GiaTriMin = parent.GiaTriMin,
                        GiaTriMax = parent.GiaTriMax,
                        DonVi = parent.DonVi,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = tenMayXetNghiems.FirstOrDefault(o => o.KeyId == parent.MayXetNghiemId)?.DisplayName,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = tenNhanViens.FirstOrDefault(o => o.KeyId == parent.NhanVienDuyetId)?.DisplayName,
                        DaGoiDuyet = phienXetNghiemChiTietVo.DaGoiDuyet,
                        //???
                        //LoaiMau = phienXetNghiemChiTietVo.TenNhomDichVuBenhVien,
                        LoaiMau = phienXetNghiemChiTietVo.LoaiMauXetNghiem?.GetDescription(),
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        //structure tree
                        Level = level,
                        //YeuCauChayLai = parent.PhienXetNghiemChiTiet?.ChayLaiKetQua,
                        DaDuyet = phienXetNghiemChiTietVo.DaGoiDuyet == true && phienXetNghiemChiTietVo.ThoiDiemKetLuan != null,
                        //NguoiYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //    ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        //LyDoYeuCau = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //    ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        //NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //    ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        //NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //    ? parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        //NgayDuyetDisplay = parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem != null
                        //    ? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet != null
                        //        ? (parent.PhienXetNghiemChiTiet?.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                        //    : "",
                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                            , parent.GiaTriTuMay),
                        LoaiKetQuaNhapTay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriNhapTay),
                        NhomId = parent.NhomDichVuBenhVienId,
                        Nhom = phienXetNghiemChiTietVo.TenNhomDichVuBenhVien,
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
            return AddDetailDataChildV2(phienXetNghiemDataVo,tenMayXetNghiems,tenNhanViens, tenDichVuXetNghiems, lstChiTietNhomConLai, lstChiTietChild, result, false, level);
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
                        YeuCauTiepNhanId = parent.PhienXetNghiemChiTiet.PhienXetNghiem.YeuCauTiepNhanId,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        DichVuKyThuatBenhVienId = parent.DichVuKyThuatBenhVienId,
                        LoaiKitThu = parent.YeuCauDichVuKyThuat.LoaiKitThu,
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
                        //// BVHD-3919
                        //NguoiThucHien = parent?.PhienXetNghiemChiTiet?.PhienXetNghiem?.NhanVienThucHien?.User?.HoTen
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
                        YeuCauTiepNhanId = parent.PhienXetNghiemChiTiet.PhienXetNghiem.YeuCauTiepNhanId,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        DichVuKyThuatBenhVienId = parent.DichVuKyThuatBenhVienId,
                        LoaiKitThu = parent.YeuCauDichVuKyThuat.LoaiKitThu,
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
                        //// BVHD-3919
                        //NguoiThucHien = parent?.PhienXetNghiemChiTiet?.PhienXetNghiem?.NhanVienThucHien?.User?.HoTen

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

        [HttpPost("DuyetPhienXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> DuyetPhienXetNghiem([FromBody] DuyetKetQuaXetNghiemViewModel model)
        {
            var userId = _userAgentHelper.GetCurrentUserId();
            var now = DateTime.Now;

            var phienXetNghiem = await _duyetKqXetNghiemService.GetByIdAsync(model.Id,
                                    u => u.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                        .Include(q => q.BenhNhan).ThenInclude(q => q.YeuCauTiepNhans)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat));
            phienXetNghiem.KetLuan = model.ChanDoan;
            phienXetNghiem.GhiChu = model.GhiChu;
            var cauHinhNguoiDuyets = _duyetKqXetNghiemService.GetCauHinhNguoiDuyetTheoNhomDichVu();
            foreach (var chiTietKetQuaXetNghiem in model.ChiTietKetQuaXetNghiems)
            {
                if (phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Any(w => w.Id == chiTietKetQuaXetNghiem.Id))
                {
                    var ketQuaXetNghiemChiTietEntityNeedToUpdate = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).First(w => w.Id == chiTietKetQuaXetNghiem.Id);
                    ketQuaXetNghiemChiTietEntityNeedToUpdate.ToDamGiaTri = chiTietKetQuaXetNghiem.ToDamGiaTri;
                    ketQuaXetNghiemChiTietEntityNeedToUpdate.GiaTriDuyet = chiTietKetQuaXetNghiem.GiaTriDuyet;
                    ketQuaXetNghiemChiTietEntityNeedToUpdate.DaDuyet = true;
                    if (ketQuaXetNghiemChiTietEntityNeedToUpdate.ThoiDiemDuyetKetQua == null)
                    {
                        ketQuaXetNghiemChiTietEntityNeedToUpdate.ThoiDiemDuyetKetQua = DateTime.Now;
                    }
                    if (ketQuaXetNghiemChiTietEntityNeedToUpdate.NhanVienDuyetId == null)
                    {
                        //update: BVHD-3919
                        var nhanVienDuyetId = cauHinhNguoiDuyets.FirstOrDefault(o => o.NhomDichVuBenhVienId == ketQuaXetNghiemChiTietEntityNeedToUpdate.NhomDichVuBenhVienId)?.NhanVienId ?? userId;
                        ketQuaXetNghiemChiTietEntityNeedToUpdate.NhanVienDuyetId = nhanVienDuyetId;
                    }
                }
            }
            Enums.EnumNhomMau? nhomMauABO = null;
            Enums.EnumYeuToRh? nhomMauRh = null;
            var phienXetNghiemChiTietsDaGoiDuyetUpdate = phienXetNghiem.PhienXetNghiemChiTiets.Where(p => p.DaGoiDuyet == true).ToList();
            var dichVuSarV2Id2 = await _duyetKqXetNghiemService.CauHinhDichVuTestSarsCovids();
            foreach (var item in phienXetNghiemChiTietsDaGoiDuyetUpdate)
            {
                if (item.KetQuaXetNghiemChiTiets.Any(o => model.ChiTietKetQuaXetNghiems.Select(m => m.Id).Contains(o.Id)))
                {
                    if (item.ThoiDiemKetLuan == null)
                    {
                        item.NhanVienKetLuanId = userId;
                        item.ThoiDiemKetLuan = now;
                        item.KetLuan = model.ChanDoan;
                        item.GhiChu = model.GhiChu;
                    }

                    if (item.YeuCauDichVuKyThuat != null)
                    {
                        if (dichVuSarV2Id2.FirstOrDefault(c => c.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId) != null)
                        {
                            item.YeuCauDichVuKyThuat.LoaiKitThu = dichVuSarV2Id2.FirstOrDefault(c => c.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId)?.TenDichVu;
                        }
                        else
                        {
                            item.YeuCauDichVuKyThuat.LoaiKitThu = null;
                        }
                        if (item.YeuCauDichVuKyThuat.ThoiDiemHoanThanh == null)
                        {
                            item.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = now;
                        }
                        item.YeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                        item.YeuCauDichVuKyThuat.KetLuan = model.ChanDoan;
                        item.YeuCauDichVuKyThuat.NhanVienKetLuanId = userId;
                    }
                    var nhomMau = LISHelper.GetKetQuaNhomMau(item.KetQuaXetNghiemChiTiets.OrderBy(o => o.Id).ToList());
                    if (nhomMau.Item1 != null)
                    {
                        nhomMauABO = nhomMau.Item1;
                    }
                    if (nhomMau.Item2 != null)
                    {
                        nhomMauRh = nhomMau.Item2;
                    }
                }
            }
            if (nhomMauABO != null)
            {
                phienXetNghiem.BenhNhan.NhomMau = nhomMauABO;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.NhomMau = nhomMauABO;
                }
            }
            if (nhomMauRh != null)
            {
                phienXetNghiem.BenhNhan.YeuToRh = nhomMauRh;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.YeuToRh = nhomMauRh;
                }
            }
            if (phienXetNghiem.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan != null))
            {
                phienXetNghiem.NhanVienKetLuanId = userId;
                phienXetNghiem.ThoiDiemKetLuan = now;
            }

            await _duyetKqXetNghiemService.UpdateAsync(phienXetNghiem);

            return NoContent();
        }

        [HttpPost("DuyetKetQuaXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> DuyetKetQuaXetNghiem([FromBody] DuyetKetQuaXetNghiemViewModel model)
        {
            var userId = _userAgentHelper.GetCurrentUserId();
            var now = DateTime.Now;

            var phienXetNghiem = _duyetKqXetNghiemService.GetById(model.Id,
                                    u => u.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                        .Include(q => q.BenhNhan).ThenInclude(q => q.YeuCauTiepNhans)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat));
            phienXetNghiem.KetLuan = model.ChanDoan;
            phienXetNghiem.GhiChu = model.GhiChu;
            if (model.NguoiThucHienId.GetValueOrDefault() != 0)
            {
                phienXetNghiem.NhanVienThucHienId = model.NguoiThucHienId;
            }
            else if (phienXetNghiem.NhanVienThucHienId == null)
            {
                phienXetNghiem.NhanVienThucHienId = userId;
            }

            if (phienXetNghiem.PhongThucHienId == null)
            {
                phienXetNghiem.PhongThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            }

            Enums.EnumNhomMau? nhomMauABO = null;
            Enums.EnumYeuToRh? nhomMauRh = null;
            var dropDownModel = new DropDownListRequestModel();
            var loaiKitThus = await _duyetKqXetNghiemService.DichVuTestSarsCovids(dropDownModel);
            var checkedKqXetNghiemChiTiets = model.ChiTietKetQuaXetNghiems.Where(o => o.CheckBox == true);
            var cauHinhNguoiDuyets = _duyetKqXetNghiemService.GetCauHinhNguoiDuyetTheoNhomDichVu();
            foreach (var item in checkedKqXetNghiemChiTiets)
            {
                item.LoaiKitThu = loaiKitThus.FirstOrDefault(c => c.KeyId == item.LoaiKitThuId)?.DisplayName;
            }
            var phienXetNghiemChiTietsDaGoiDuyetUpdate = phienXetNghiem.PhienXetNghiemChiTiets.ToList();
            foreach (var phienXetNghiemChiTiet in phienXetNghiemChiTietsDaGoiDuyetUpdate)
            {
                if (phienXetNghiemChiTiet.ThoiDiemKetLuan == null && phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.Any(o => checkedKqXetNghiemChiTiets.Select(m => m.Id).Contains(o.Id)))
                {
                    foreach (var ketQuaXetNghiemChiTiet in phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets)
                    {
                        var chiTietKetQuaXetNghiem = model.ChiTietKetQuaXetNghiems.FirstOrDefault(o => o.Id == ketQuaXetNghiemChiTiet.Id);
                        if (chiTietKetQuaXetNghiem != null)
                        {
                            ketQuaXetNghiemChiTiet.ToDamGiaTri = chiTietKetQuaXetNghiem.ToDamGiaTri;
                            ketQuaXetNghiemChiTiet.GiaTriDuyet = chiTietKetQuaXetNghiem.GiaTriDuyet;
                        }
                        ketQuaXetNghiemChiTiet.DaDuyet = true;
                        ketQuaXetNghiemChiTiet.ThoiDiemDuyetKetQua = DateTime.Now;
                        //update: BVHD-3919
                        var nhanVienDuyetId = cauHinhNguoiDuyets.FirstOrDefault(o => o.NhomDichVuBenhVienId == ketQuaXetNghiemChiTiet.NhomDichVuBenhVienId)?.NhanVienId ?? userId;
                        ketQuaXetNghiemChiTiet.NhanVienDuyetId = nhanVienDuyetId;
                    }

                    phienXetNghiemChiTiet.DaGoiDuyet = true;
                    phienXetNghiemChiTiet.NhanVienKetLuanId = userId;
                    phienXetNghiemChiTiet.ThoiDiemKetLuan = now;
                    phienXetNghiemChiTiet.KetLuan = model.ChanDoan;
                    phienXetNghiemChiTiet.GhiChu = model.GhiChu;

                    if (phienXetNghiemChiTiet.YeuCauDichVuKyThuat != null)
                    {
                        if (checkedKqXetNghiemChiTiets.FirstOrDefault(c => c.YeuCauDichVuKyThuatId == phienXetNghiemChiTiet.YeuCauDichVuKyThuatId) != null)
                        {
                            phienXetNghiemChiTiet.YeuCauDichVuKyThuat.LoaiKitThu = checkedKqXetNghiemChiTiets.FirstOrDefault(c => c.YeuCauDichVuKyThuatId == phienXetNghiemChiTiet.YeuCauDichVuKyThuatId)?.LoaiKitThu;
                        }
                        if (phienXetNghiemChiTiet.YeuCauDichVuKyThuat.ThoiDiemHoanThanh == null)
                        {
                            phienXetNghiemChiTiet.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = now;
                        }
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.KetLuan = model.ChanDoan;
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhanVienKetLuanId = userId;
                    }
                    var nhomMau = LISHelper.GetKetQuaNhomMau(phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.OrderBy(o => o.Id).ToList());
                    if (nhomMau.Item1 != null)
                    {
                        nhomMauABO = nhomMau.Item1;
                    }
                    if (nhomMau.Item2 != null)
                    {
                        nhomMauRh = nhomMau.Item2;
                    }
                }
            }
            if (nhomMauABO != null)
            {
                phienXetNghiem.BenhNhan.NhomMau = nhomMauABO;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.NhomMau = nhomMauABO;
                }
            }
            if (nhomMauRh != null)
            {
                phienXetNghiem.BenhNhan.YeuToRh = nhomMauRh;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.YeuToRh = nhomMauRh;
                }
            }
            if (phienXetNghiem.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan != null))
            {
                phienXetNghiem.NhanVienKetLuanId = userId;
                phienXetNghiem.ThoiDiemKetLuan = now;
            }

            await _duyetKqXetNghiemService.UpdateAsync(phienXetNghiem);

            return NoContent();
        }


        [HttpPost("CapNhatDichVuXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> CapNhatDichVuXetNghiem([FromBody] DuyetKetQuaXetNghiemViewModel model)
        {
            var userId = _userAgentHelper.GetCurrentUserId();
            var now = DateTime.Now;

            var phienXetNghiem = await _duyetKqXetNghiemService.GetByIdAsync(model.Id,
                                    u => u.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                        .Include(q => q.BenhNhan).ThenInclude(q => q.YeuCauTiepNhans)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat));

            Enums.EnumNhomMau? nhomMauABO = null;
            Enums.EnumYeuToRh? nhomMauRh = null;
            var phienXetNghiemChiTietsDaGoiDuyetUpdate = phienXetNghiem.PhienXetNghiemChiTiets.Where(p => p.DaGoiDuyet == true).ToList();
            var dropDownModel = new DropDownListRequestModel();
            var loaiKitThus = await _duyetKqXetNghiemService.DichVuTestSarsCovids(dropDownModel);
            var cauHinhNguoiDuyets = _duyetKqXetNghiemService.GetCauHinhNguoiDuyetTheoNhomDichVu();
            foreach (var item in model.ChiTietKetQuaXetNghiems)
            {
                item.LoaiKitThu = loaiKitThus.FirstOrDefault(c => c.KeyId == item.LoaiKitThuId)?.DisplayName;
            }
            foreach (var phienXetNghiemChiTiet in phienXetNghiemChiTietsDaGoiDuyetUpdate)
            {
                var hasUpdate = false;
                foreach (var ketQuaXetNghiemChiTiet in phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets)
                {
                    var chiTietKetQuaXetNghiem = model.ChiTietKetQuaXetNghiems.FirstOrDefault(o => o.Id == ketQuaXetNghiemChiTiet.Id);

                    if (chiTietKetQuaXetNghiem != null)
                    {
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.LoaiKitThu = chiTietKetQuaXetNghiem.LoaiKitThu;//Update 13/01/2021
                        if (ketQuaXetNghiemChiTiet.GiaTriDuyet != chiTietKetQuaXetNghiem.GiaTriDuyet || ketQuaXetNghiemChiTiet.ToDamGiaTri != chiTietKetQuaXetNghiem.ToDamGiaTri)
                        {
                            ketQuaXetNghiemChiTiet.ToDamGiaTri = chiTietKetQuaXetNghiem.ToDamGiaTri;
                            ketQuaXetNghiemChiTiet.GiaTriDuyet = chiTietKetQuaXetNghiem.GiaTriDuyet;
                            ketQuaXetNghiemChiTiet.ThoiDiemDuyetKetQua = DateTime.Now;
                            //update: BVHD-3919
                            var nhanVienDuyetId = cauHinhNguoiDuyets.FirstOrDefault(o => o.NhomDichVuBenhVienId == ketQuaXetNghiemChiTiet.NhomDichVuBenhVienId)?.NhanVienId ?? userId;
                            ketQuaXetNghiemChiTiet.NhanVienDuyetId = nhanVienDuyetId;
                            hasUpdate = true;
                        }
                    }
                }

                if (hasUpdate)
                {
                    phienXetNghiemChiTiet.NhanVienKetLuanId = userId;
                    phienXetNghiemChiTiet.ThoiDiemKetLuan = now;
                    var nhomMau = LISHelper.GetKetQuaNhomMau(phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.OrderBy(o => o.Id).ToList());
                    if (nhomMau.Item1 != null)
                    {
                        nhomMauABO = nhomMau.Item1;
                    }
                    if (nhomMau.Item2 != null)
                    {
                        nhomMauRh = nhomMau.Item2;
                    }
                }
            }

            if (nhomMauABO != null)
            {
                phienXetNghiem.BenhNhan.NhomMau = nhomMauABO;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.NhomMau = nhomMauABO;
                }
            }
            if (nhomMauRh != null)
            {
                phienXetNghiem.BenhNhan.YeuToRh = nhomMauRh;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.YeuToRh = nhomMauRh;
                }
            }

            await _duyetKqXetNghiemService.UpdateAsync(phienXetNghiem);

            return NoContent();
        }

        private void UpdateDichVuTheoKetQua(PhienXetNghiem phienXetNghiem)
        {
            Enums.EnumNhomMau? nhomMauABO = null;
            Enums.EnumYeuToRh? nhomMauRh = null;
            foreach (var phienXetNghiemChiTiet in phienXetNghiem.PhienXetNghiemChiTiets)
            {
                if (phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.Any())
                {
                    if (phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.All(o => o.NhanVienDuyetId != null))
                    {
                        phienXetNghiemChiTiet.KetLuan = phienXetNghiem.KetLuan;
                        phienXetNghiemChiTiet.NhanVienKetLuanId = phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.OrderBy(o => o.Id).Last().NhanVienDuyetId;
                        phienXetNghiemChiTiet.ThoiDiemKetLuan = phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.OrderBy(o => o.Id).Last().ThoiDiemDuyetKetQua;

                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.KetLuan = phienXetNghiem.KetLuan;
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhanVienKetLuanId = phienXetNghiemChiTiet.NhanVienKetLuanId;
                        if (phienXetNghiemChiTiet.YeuCauDichVuKyThuat.ThoiDiemHoanThanh == null)
                        {
                            phienXetNghiemChiTiet.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = phienXetNghiemChiTiet.ThoiDiemKetLuan;
                        }
                        var nhomMau = LISHelper.GetKetQuaNhomMau(phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.OrderBy(o => o.Id).ToList());
                        if (nhomMau.Item1 != null)
                        {
                            nhomMauABO = nhomMau.Item1;
                        }
                        if (nhomMau.Item2 != null)
                        {
                            nhomMauRh = nhomMau.Item2;
                        }
                    }
                    else
                    {
                        phienXetNghiemChiTiet.NhanVienKetLuanId = null;
                        phienXetNghiemChiTiet.ThoiDiemKetLuan = null;

                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien;
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhanVienKetLuanId = null;
                        phienXetNghiemChiTiet.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = null;
                    }
                }
            }

            if (nhomMauABO != null)
            {
                phienXetNghiem.BenhNhan.NhomMau = nhomMauABO;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.NhomMau = nhomMauABO;
                }
            }
            if (nhomMauRh != null)
            {
                phienXetNghiem.BenhNhan.YeuToRh = nhomMauRh;
                foreach (var yeuCauTiepNhan in phienXetNghiem.BenhNhan.YeuCauTiepNhans.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien))
                {
                    yeuCauTiepNhan.YeuToRh = nhomMauRh;
                }
            }

            if (phienXetNghiem.PhienXetNghiemChiTiets.Any() && phienXetNghiem.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan != null))
            {
                phienXetNghiem.NhanVienKetLuanId = phienXetNghiem.PhienXetNghiemChiTiets.First().NhanVienKetLuanId;
                phienXetNghiem.ThoiDiemKetLuan = DateTime.Now;
            }
            else
            {
                phienXetNghiem.NhanVienKetLuanId = null;
                phienXetNghiem.ThoiDiemKetLuan = null;
            }
        }


        [HttpPost("DuyetOnGrid")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> DuyetOnGrid([FromBody] DuyetKetQuaXetNghiemForServiceProcessingVo requestUpdateModelOnGrid, long currentId, bool currentCheck)
        {
            var phienXetNghiem = await _duyetKqXetNghiemService.GetByIdAsync(requestUpdateModelOnGrid.Id, w => w.Include(q => q.PhienXetNghiemChiTiets).ThenInclude(q => q.YeuCauDichVuKyThuat)
                .Include(q => q.BenhNhan).ThenInclude(q => q.YeuCauTiepNhans)
                .Include(q => q.PhienXetNghiemChiTiets).ThenInclude(q => q.KetQuaXetNghiemChiTiets));
            var res = await _duyetKqXetNghiemService.DuyetOnGrid(requestUpdateModelOnGrid, phienXetNghiem, currentCheck, currentId);
            phienXetNghiem.KetLuan = requestUpdateModelOnGrid.ChanDoan;
            phienXetNghiem.GhiChu = requestUpdateModelOnGrid.GhiChu;
            if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any(w => w.Id == currentId && w.Level != 1))
            {
                var entityResult = _duyetKqXetNghiemService.DuyetOnLevelOne(requestUpdateModelOnGrid, res, currentId, currentCheck);
                UpdateDichVuTheoKetQua(phienXetNghiem);
                await _duyetKqXetNghiemService.UpdateAsync(entityResult);
            }
            else
            {
                UpdateDichVuTheoKetQua(phienXetNghiem);
                await _duyetKqXetNghiemService.UpdateAsync(res);
            }


            return Ok(requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems);
        }

        [HttpPost("ApproveOnGroup")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> ApproveOnGroup([FromBody] DuyetKetQuaXetNghiemForServiceProcessingVo duyetKetQuaTheoNhom, long currentNhomId, bool currentCheck)
        {
            var phienXetNghiem = await _duyetKqXetNghiemService.GetByIdAsync(duyetKetQuaTheoNhom.Id, w => w.Include(q => q.PhienXetNghiemChiTiets)
                .ThenInclude(q => q.KetQuaXetNghiemChiTiets));

            var res = await _duyetKqXetNghiemService.ApproveOnGroup(duyetKetQuaTheoNhom, phienXetNghiem, currentCheck, currentNhomId);
            UpdateDichVuTheoKetQua(phienXetNghiem);
            await _duyetKqXetNghiemService.UpdateAsync(res);

            return Ok(duyetKetQuaTheoNhom.ChiTietKetQuaXetNghiems);
        }

        [HttpGet("GetNhomDichVuDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> GetNhomDichVuDuyets(long phienXetNghiemId)
        {
            var nhomDichVuXetNghiemDuyetVos = await _duyetKqXetNghiemService.GetNhomDichVuDuyets(phienXetNghiemId);
            return Ok(nhomDichVuXetNghiemDuyetVos);
        }

        [HttpPost("TimKiemPhienXetNghiemGanNhat")]
        public async Task<ActionResult> TimKiemPhienXetNghiemGanNhat(PhienXNGanNhatVo phienXNGanNhatVo)
        {
            var phienXetNghiemId = await _duyetKqXetNghiemService.TimKiemPhienXetNghiemGanNhat(phienXNGanNhatVo);
            if (phienXetNghiemId == null)
            {
                throw new ApiException("Barcode này không tồn tại.");
            }
            return Ok(phienXetNghiemId);
        }

        [HttpPost("DichVuTestSarsCovids")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> DichVuTestSarsCovids(DropDownListRequestModel model)
        {
            var lookup = await _duyetKqXetNghiemService.DichVuTestSarsCovids(model);
            return Ok(lookup);
        }
        
        [HttpPost("GetNgayDuyetKetQuaCu")]
        public async Task<ActionResult> GetNgayDuyetKetQuaCu(KetQuaXetNghiemVo ketQuaXetNghiemVo)
        {
            var result = await _duyetKqXetNghiemService.GetNgayDuyetKetQuaCu(ketQuaXetNghiemVo);
            return Ok(result);
        }
    }
}
