using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        #region khám đoàn: lịch sử tiếp nhận khám sức khỏe 
        [HttpPost("GetDataLichSuTiepNhanKhamSucKhoeForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetDataLichSuTiepNhanKhamSucKhoeForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataLichSuTiepNhanKhamSucKhoeForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageLichSuTiepNhanKhamSucKhoeForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageLichSuTiepNhanKhamSucKhoeForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageLichSuTiepNhanKhamSucKhoeForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region chi tiết hợp đồng khám sức khỏe Id
        [HttpPost("GetHopDongKhamSucKhoeTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetHopDongKhamSucKhoeTiepNhanAsync([FromBody]QueryInfo queryInfo)
        {
            //var gridData = await .GetDataForGridAsync(queryInfo);
            List<DSNhanVien> gridDataList = new List<DSNhanVien>();
            var gridData = new DSNhanVien()
            {
                Id = 1,
                MaNhanVien = "001",
                TenNhanVien = "Nguyễn Văn A",
                DonViBoPhan ="Kế toán",
                GioiTinh =Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai ="0926562365",
                Email="",
                ChungMinhThu ="256365236",
                SHC ="",
                DanToc ="Kinh",
                TinhThanh = "Hà Nội",
                NhomKham ="KSK Định kỳ",
                GhiChu ="",
                TinhTrang =1
            };
            var gridData1 = new DSNhanVien()
            {
                Id = 2,
                MaNhanVien = "002",
                TenNhanVien = "Nguyễn Văn B",
                DonViBoPhan = "Kế toán",
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai = "0926562365",
                Email = "",
                ChungMinhThu = "256365236",
                SHC = "",
                DanToc = "Kinh",
                TinhThanh = "Hà Nội",
                NhomKham = "KSK Định kỳ",
                GhiChu = "",
                TinhTrang = 2
            };
            var gridData2 = new DSNhanVien()
            {
                Id = 2,
                MaNhanVien = "003",
                TenNhanVien = "Nguyễn Văn C",
                DonViBoPhan = "Kế toán",
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai = "0926562365",
                Email = "",
                ChungMinhThu = "256365236",
                SHC = "",
                DanToc = "Kinh",
                TinhThanh = "Hà Nội",
                NhomKham = "KSK Định kỳ",
                GhiChu = "",
                TinhTrang = 2
            };
            var gridData3 = new DSNhanVien()
            {
                Id = 2,
                MaNhanVien = "004",
                TenNhanVien = "Nguyễn Văn D",
                DonViBoPhan = "Kế toán",
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai = "0926562365",
                Email = "",
                ChungMinhThu = "256365236",
                SHC = "",
                DanToc = "Kinh",
                TinhThanh = "Hà Nội",
                NhomKham = "KSK Định kỳ",
                GhiChu = "",
                TinhTrang = 2
            };
            gridDataList.Add(gridData);
            gridDataList.Add(gridData1);
            gridDataList.Add(gridData2);
            gridDataList.Add(gridData3);

            var dataOrderBy = gridDataList.AsQueryable();
            var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();

            var data = new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
            return Ok(data);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageHopDongKhamSucKhoeTiepNhanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageHopDongKhamSucKhoeTiepNhanForGridAsync([FromBody]QueryInfo queryInfo)
        {
            //var gridData = await .GetDataForGridAsync(queryInfo);
            List<DSNhanVien> gridDataList = new List<DSNhanVien>();
            var gridData = new DSNhanVien()
            {
                Id = 1,
                MaNhanVien = "001",
                TenNhanVien = "Nguyễn Văn A",
                DonViBoPhan = "Kế toán",
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai = "0926562365",
                Email = "",
                ChungMinhThu = "256365236",
                SHC = "",
                DanToc = "Kinh",
                TinhThanh = "Hà Nội",
                NhomKham = "KSK Định kỳ",
                GhiChu = "",
                TinhTrang = 1
            };
            var gridData1 = new DSNhanVien()
            {
                Id = 2,
                MaNhanVien = "002",
                TenNhanVien = "Nguyễn Văn B",
                DonViBoPhan = "Kế toán",
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai = "0926562365",
                Email = "",
                ChungMinhThu = "256365236",
                SHC = "",
                DanToc = "Kinh",
                TinhThanh = "Hà Nội",
                NhomKham = "KSK Định kỳ",
                GhiChu = "",
                TinhTrang = 2
            };
            var gridData2 = new DSNhanVien()
            {
                Id = 2,
                MaNhanVien = "003",
                TenNhanVien = "Nguyễn Văn C",
                DonViBoPhan = "Kế toán",
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai = "0926562365",
                Email = "",
                ChungMinhThu = "256365236",
                SHC = "",
                DanToc = "Kinh",
                TinhThanh = "Hà Nội",
                NhomKham = "KSK Định kỳ",
                GhiChu = "",
                TinhTrang = 2
            };
            var gridData3 = new DSNhanVien()
            {
                Id = 2,
                MaNhanVien = "004",
                TenNhanVien = "Nguyễn Văn D",
                DonViBoPhan = "Kế toán",
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                NamSinh = 1985,
                SoDienThoai = "0926562365",
                Email = "",
                ChungMinhThu = "256365236",
                SHC = "",
                DanToc = "Kinh",
                TinhThanh = "Hà Nội",
                NhomKham = "KSK Định kỳ",
                GhiChu = "",
                TinhTrang = 2
            };
            gridDataList.Add(gridData);
            gridDataList.Add(gridData1);
            gridDataList.Add(gridData2);
            gridDataList.Add(gridData3);

            var dataOrderBy = gridDataList.AsQueryable();
            var countTask = dataOrderBy.Count();

            var data = new GridDataSource { TotalRowCount = countTask };
            return Ok(data);
        }
        #endregion
        #endregion
        #region Export exel
        [HttpPost("ExportLichSuTiepNhanKhamSucKhoeDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe)]
        public async Task<ActionResult> ExportLichSuTiepNhanKhamSucKhoeDoan(QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataLichSuTiepNhanKhamSucKhoeForGridAsync(queryInfo, true);
            var chucDanhData = gridData.Data.Select(p => (LichSuTiepNhanKhamSucKhoeDoanGridVo)p).ToList();
            var excelData = chucDanhData.Map<List<LichSuTiepNhanKhamSucKhoeDoanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuTiepNhanKhamSucKhoeDoanExportExcel.TenHopDong), "Hợp Đồng"));
            lstValueObject.Add((nameof(LichSuTiepNhanKhamSucKhoeDoanExportExcel.TenCongTy), "Tên Công Ty"));
            lstValueObject.Add((nameof(LichSuTiepNhanKhamSucKhoeDoanExportExcel.SoLuongBenhNhan), "SL BN"));
            lstValueObject.Add((nameof(LichSuTiepNhanKhamSucKhoeDoanExportExcel.SoBenhNhanDaDen), "SL BN Đã Đến"));
            lstValueObject.Add((nameof(LichSuTiepNhanKhamSucKhoeDoanExportExcel.NgayBatDauKhamDisplay), "Ngày BĐ Khám"));
            lstValueObject.Add((nameof(LichSuTiepNhanKhamSucKhoeDoanExportExcel.NgayKetThucKhamDisplay), "Ngày KT Khám"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch Sử Tiếp Nhận Khám Sức Khỏe Đoàn",2, "Lịch Sử Tiếp Nhận Khám Sức Khỏe Đoàn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuTiepNhanKhamSucKhoeDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #region In Sổ khám sức khỏe định kỳ
        [HttpPost("XuLyInKhamSucKhoeAsync")]
        
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<string>> XuLyInKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            var phieuIn = await _khamDoanService.XuLyInKhamSucKhoeAsync(phieuInNhanVienKhamSucKhoeInfoVo);
            var list = new List<HtmlToPdfVo>();
            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            // BVHD-3946
            var footerHtml = _khamDoanService.GetTemplatePhieuDangKyKham(0);
            // BVHD-3946 
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = phieuIn,
                FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                Bottom = 15
            };
            list.Add(htmlToPdfVo);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }
        [HttpPost("XuLyInPhieuDangKyKhamSucKhoeAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<string>> XuLyInPhieuDangKyKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            var phieuIn = await _khamDoanService.XuLyInPhieuDangKyKhamSucKhoeAsync(phieuInNhanVienKhamSucKhoeInfoVo);
            var list = new List<HtmlToPdfVo>();
            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            // BVHD-3946
            var footerHtml = _khamDoanService.GetTemplatePhieuDangKyKham(1);
            // BVHD-3946 
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = phieuIn,
                FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                Bottom = 15
            };
            list.Add(htmlToPdfVo);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }
        [HttpPost("XuLyInKetQuaKhamSucKhoeAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<string>> XuLyInKetQuaKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            var phieuIn = await _khamDoanService.XuLyInKetQuaKhamSucKhoeAsync(phieuInNhanVienKhamSucKhoeInfoVo);
            var list = new List<HtmlToPdfVo>();
            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            // BVHD-3946
            var footerHtml = _khamDoanService.GetTemplatePhieuDangKyKham(2);
            // BVHD-3946 
            htmlContent.TenFile = "KetQuaKSK";
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = phieuIn,
                FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                Bottom = 15
            };
            list.Add(htmlToPdfVo);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");

        }
        #region pdf k dùng nữa
        [HttpPost("GetFilePDFFromHtml")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public ActionResult GetFilePDFFromHtml(PhieuInNhanVienKhamSucKhoeViewModel htmlContent)
        {
            //var footerHtml = @"<!DOCTYPE html>
            //<html>
            //<head>
            //    <meta charset='utf-8'>
            //    <script charset='utf-8'>

            //    function replaceParams() {
            //      var url = window.location.href
            //        .replace(/#$/, '');
            //      var params = (url.split('?')[1] || '').split('&');
            //      for (var i = 0; i < params.length; i++) {
            //          var param = params[i].split('=');
            //          var key = param[0];
            //          var value = param[1] || '';
            //          var regex = new RegExp('{' + key + '}', 'g');
            //          document.body.innerText = document.body.innerText.replace(regex, value);
            //        }
            //    }
            //    </script>
            //</head>";
            //if (htmlContent.TenFile == "KetQuaKhamSucKhoe")
            //{
            //    footerHtml += @"<body onload='replaceParams()'><div style='font-weight:bold;padding-right:40px;{page!=topage?'display:none':''}'><b>*Ghi chú: Phân loại khám sức khỏe: Loại 1: Rất khỏe; Loại 2: Khỏe; Loại 3: Trung bình; Loại 4: Yếu; Loại 5: Rất yếu.</b></div>Trang {page}/{topage}</body></html>";
            //}
            //else {
            //    footerHtml += @"<body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}</body></html>";
            //}

            var footerHtml = @"<!DOCTYPE html>
              <html><head><script>
                function subst() {
                   var vars={};
                    var x=window.location.search.substring(1).split('&');
                    for (var i in x) {var z=x[i].split('=',2);vars[z[0]] = unescape(z[1]);}
                    var x=['frompage','topage','page','webpage','section','subsection','subsubsection'];
                    for (var i in x) {
                        var y = document.getElementsByClassName(x[i]);
                        for (var j=0; j<y.length; ++j) y[j].textContent = vars[x[i]];
                        ";
                        //if (htmlContent.TenFile == "KetQuaKhamSucKhoe")
                        //{
                        //    footerHtml += @"if(vars['page'] != vars['topage']) { 
                        //       document.getElementById('footer').style.display = 'none';
                        //    }";
                        //}
            footerHtml += @"}}
              </script></head><body style='border:0; margin: 0;' onload='subst()'>
                <table style='width: 100%'>   
                   <tr>";
            if (htmlContent.TenFile == "KetQuaKhamSucKhoe")
            {
                footerHtml += @"<td><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Ghi ch&#250;: Ph&#226;n lo&#7841;i kh&#225;m s&#7913;c kh&#7887;e: Lo&#7841;i 1: R&#7845;t kh&#7887;e; Lo&#7841;i 2: Kh&#7887;e; Lo&#7841;i 3: Trung b&#236;nh; Lo&#7841;i 4: Y&#7871;u; Lo&#7841;i 5: R&#7845;t y&#7871;u.</b></td>";
            }
            else {
                footerHtml += @"<td class='section'></td>";
            }                    
                  footerHtml +=@"<td style='text-align:right'>
                    Trang <span class='page'></span>/<span class='topage'></span>
                  </td>
                </tr>
              </table>
              </body></html>";
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = htmlContent.Html,
                FooterHtml =htmlContent.NoFooter!=true?footerHtml:"",
                Bottom=15
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }


        #endregion
        [HttpPost("GetPhanLoaiSucKhoePopupketLuan")]
        public ActionResult<ICollection<LookupItemVo>> GetPhanLoaiSucKhoePopupketLuan(DropDownListRequestModel model, long? phanLoaiId)
        {
            var lookup = _khamDoanService.GetPhanLoaiSucKhoeKetLuan(model, phanLoaiId);
            return Ok(lookup);
        }
        #endregion

        #region //BVHD-3929

        [HttpPost("XuLyInNhieuPhieuDangKyKhamSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<string>> XuLyInNhieuPhieuDangKyKhamSucKhoeAsync(PhieuInDangKyKSKVo phieuInDangKyKSKVo)
        {
            var phieuIns = await _khamDoanService.XuLyInNhieuPhieuDangKyKhamSucKhoeAsync(phieuInDangKyKSKVo);
            var list = new List<HtmlToPdfVo>();
            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            // BVHD-3946
            var footerHtml = _khamDoanService.GetTemplatePhieuDangKyKham(1);
            // BVHD-3946 

            foreach (var html in phieuIns)
            {
                var htmlToPdfVo = new HtmlToPdfVo
                {
                    Html = html,
                    FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                    Bottom = 15
                };
                list.Add(htmlToPdfVo);
            }
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        #endregion
    }
}
