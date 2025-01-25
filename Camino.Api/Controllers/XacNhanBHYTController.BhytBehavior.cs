using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenefitInsurance;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

// connected to APIs that changes the behavior or changes data
namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuLinhThuocBenhNhanXacNhanBHYT")]     
        public async Task<ActionResult<GridDataSource>> PhieuLinhThuocBenhNhanXacNhanBHYT(long baoHiemYteId, string hostingName)
        {
            var phieuLinhThuocBenhNhanBHYT = await _xacNhanBhytService.PhieuLinhThuocBenhNhanXacNhanBHYT(baoHiemYteId, hostingName);
            return Ok(phieuLinhThuocBenhNhanBHYT);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuLinhThuocBenhNhanBHYTTheoYCTN")]
        public async Task<ActionResult<GridDataSource>> PhieuLinhThuocBenhNhanBHYTTheoYCTN(long yeuCauTiepNhanId, string hostingName)
        {
            var phieuLinhThuocBenhNhanBHYT = await _xacNhanBhytService.PhieuLinhThuocBenhNhanBHYTTheoYCTN(yeuCauTiepNhanId, hostingName);
            return Ok(phieuLinhThuocBenhNhanBHYT);
        }

        [HttpPost("GetFilePDFFromHtml")]
        public ActionResult GetFilePDFFromHtml(PhieuXacNhanBHYT htmlContent)
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("DuyetBaoHiemChoXnAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> DuyetBaoHiemChoXnAsync
            (long yeuCauTiepNhanId)
        {
            await _bhytConfirmByDayService.DuyetBaoHiemAsync(yeuCauTiepNhanId);
            return Ok();
        }

        //Hàm xác nhận duyệt bảo hiểm y tế
        [HttpPost("ConfirmBenefitInsuranceAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<BenefitInsuranceResultVo>> ConfirmBenefitInsuranceAsync([FromBody]BenefitInsuranceVo duyetBaoHiemVo)
        {
            if (duyetBaoHiemVo.BenefitInsurance.Any(x => x.MucHuong == null || x.MucHuong == 0))
            {
                var errors = new List<ValidationError>();

                for (int i = 0; i < duyetBaoHiemVo.BenefitInsurance.Count; i++)
                {
                    if (duyetBaoHiemVo.BenefitInsurance[i].MucHuong.GetValueOrDefault() == 0)
                    {
                        errors.Add(new ValidationError($"BenefitInsurance[{i}].TiLeBhyt", duyetBaoHiemVo.BenefitInsurance[i].MucHuong == null ? _localizationService.GetResource("BHYT.TiLeHuong.NotEmpty") : _localizationService.GetResource("BHYT.TiLeHuong.NotEqualZero")));
                    }
                }
                throw new ApiException(_localizationService.GetResource("BHYT.TiLeHuong.NotEqualZero"), 500, errors);
            }

            return Ok(await _bhytConfirmByDayService.ConfirmBenefitInsuranceAsync(duyetBaoHiemVo));
        }

        //Hàm hủy duyệt bảo hiểm y tế
        [HttpPost("HuyDuyetBaoHiemYte")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<BenefitInsuranceResultVo>> HuyDuyetBaoHiemYte([FromBody]BenefitInsuranceVo dsBaoHiemVo)
        {
            return Ok(_bhytConfirmByDayService.HuyDuyetBaoHiemYte(dsBaoHiemVo));
        }
    }
}
