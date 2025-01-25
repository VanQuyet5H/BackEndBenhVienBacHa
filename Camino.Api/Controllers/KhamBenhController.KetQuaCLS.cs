using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.XetNghiem;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using Camino.Core.Domain.ValueObject.PhieuInXetNghiem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {

        #region  Kết quả cận lâm sàng 25/05/2021

        [HttpPost("GetDataForGridKetQuaCLS")]      
        public GridDataSource GetDataForGridKetQuaCLS([FromBody]QueryInfo queryInfo , long congtyId , long hopDongId)
        {
            var gridData = _yeuCauDichVuKyThuatService.GetDataKetQuaCDHATDCN(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForGridKetQuaCLS")]       
        public GridDataSource GetTotalPageForGridKetQuaCLS([FromBody]QueryInfo queryInfo, long congtyId, long hopDongId)
        {
            var gridData = _yeuCauDichVuKyThuatService.GetTotalKetQuaCDHATDCN(queryInfo);
            return gridData;
        }

        [HttpGet("GetDataForGridXetNghiemKetQuaCLS")]       
        public GridDataSource GetDataForGridXetNghiemKetQuaCLS(long yeuCauTiepNhanId)
        {
            var gridData = _yeuCauDichVuKyThuatService.GetDataKetQuaXetNghiem(yeuCauTiepNhanId);
            return gridData;
        }

        #endregion


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncKetQuaCLS")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncKetQuaCLS([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauDichVuKyThuatService.GetDataForGridAsyncKetQuaCLS(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncKetQuaCLS")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncKetQuaCLS([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauDichVuKyThuatService.GetTotalPageForGridAsyncKetQuaCLS(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncKetQuaCLSDetail")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncKetQuaCLSDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauDichVuKyThuatService.GetDataForGridAsyncKetQuaCLSDetail(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncKetQuaCLSDetail")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncKetQuaCLSDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauDichVuKyThuatService.GetTotalPageForGridAsyncKetQuaCLSDeTail(queryInfo);
            return Ok(gridData);
        }

        #region Gởi và hủy yêu cầu xét nghiệm lại

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GoiYeuCauChayLaiXetNghiem")]
        public ActionResult<bool> GoiYeuCauChayLaiXetNghiem(KetQuaGoiLaiXetNghiem modelGoiYCChayLaiXetNghhiem)
        {
            var kiemTraYCChayLaiXetNghiem = _yeuCauDichVuKyThuatService.GoiYeuCauChayLaiXetNghiem(modelGoiYCChayLaiXetNghhiem);
            return Ok(kiemTraYCChayLaiXetNghiem);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyYeuCauChayLaiXetNghiem/{PhienXetNghiemId}")]
        public ActionResult<bool> HuyYeuCauChayLaiXetNghiem(long phienXetNghiemId)
        {
            var huyPhienXN = _yeuCauDichVuKyThuatService.HuyYeuCauChayLaiXetNghiem(phienXetNghiemId);
            return Ok(huyPhienXN);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInNhomXetNghiem")]
        public async Task<ActionResult<List<Core.Domain.ValueObject.DuyetKetQuaXetNghiems.PhieuInXetNghiemModel>>> PhieuInNhomXetNghiem(PhieuInXetNghiemVo ketQuaXetNghiemPhieuIn)
        {
            DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
            {
                Id = ketQuaXetNghiemPhieuIn.PhienXetNghiemId,
                HostingName = ketQuaXetNghiemPhieuIn.HostingName,
                Header = ketQuaXetNghiemPhieuIn.Header,
            };

            ketQuaVo.NhomDichVuBenhVienIds.Add(ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId);
            var phieuIns = await _duyetKetQuaXetNghiemService.InKetQuaXetNghiem(ketQuaVo);
            return Ok(phieuIns);
        }

        [HttpPost("ExportPdfKetQuaXetNghiem")]      
        public async Task<ActionResult<List<string>>> ExportPdfKetQuaXetNghiemAsync(PhieuInXetNghiemVo ketQuaXetNghiemPhieuIn)
        {
            DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
            {
                Id = ketQuaXetNghiemPhieuIn.PhienXetNghiemId,
                HostingName = ketQuaXetNghiemPhieuIn.HostingName,
                Header = ketQuaXetNghiemPhieuIn.Header,
            };

            ketQuaVo.NhomDichVuBenhVienIds.Add(ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId);
            var lstPhieuIn = await _duyetKetQuaXetNghiemService.InKetQuaXetNghiemNew(ketQuaVo);

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
            //htmlContent += "</body></html>";
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
            //      }
            //    }
            //    </script>
            //</head>
            //< body onload = 'replaceParams()' style = 'text-align: right;' > Trang { page}/{ topage}
            //</body>
            //</html>";
            //var htmlToPdfVo = new HtmlToPdfVo
            //{
            //    Html = htmlContent,
            //    FooterHtml = footerHtml
            //};
            //var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);

            //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KetQuaXetNghiem" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            //Response.ContentType = "application/pdf";
            //return new FileContentResult(bytes, "application/pdf");

        }


        [HttpPost("GetFilePDFFromHtml")]
        public ActionResult GetFilePDFFromHtml(LayMauXetNghiemFileKetQuaViewModel htmlContent)
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
                FooterHtml = footerHtml
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("LichSuYeuCauChayLai/{yeuCauTiepNhanId}")]
        public ActionResult<List<LichSuYeuCauChayLai>> LichSuYeuCauChayLai(long yeuCauTiepNhanId)
        {
            var lichSuYeuCauChayLais = _yeuCauDichVuKyThuatService.LichSuYeuCauChayLai(yeuCauTiepNhanId);
            return Ok(lichSuYeuCauChayLais);
        }

        #endregion
    }
}
