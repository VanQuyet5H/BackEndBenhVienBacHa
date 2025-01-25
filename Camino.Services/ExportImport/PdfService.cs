using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using System.Web;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Infrastructure;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.Layout.Font;
using iTextSharp.text.pdf;
using Wkhtmltopdf.NetCore;

namespace Camino.Services.ExportImport
{
    [ScopedDependency(ServiceType = typeof(IPdfService))]
    public class PdfService : IPdfService
    {
        private const string TimesNewRomanFontPath = "~/App_Data/Pdf/times-new-roman.ttf";
        private readonly Lazy<ConverterProperties> _converterPropertiesVariable;
        readonly IGeneratePdf _generatePdf;
        private readonly ExportPdfApiConfig _exportPdfApiConfig;

        public PdfService(ICaminoFileProvider fileProvider, IGeneratePdf generatePdf, ExportPdfApiConfig exportPdfApiConfig)
        {
            _converterPropertiesVariable = new Lazy<ConverterProperties>(() =>
            {
                ConverterProperties properties = new ConverterProperties();
                FontProvider fontProvider = new DefaultFontProvider(true, true, true);
                FontProgram fontProgram = FontProgramFactory.CreateFont(FontConstants.TIMES_ROMAN);
                fontProvider.AddFont(fontProgram);
                properties.SetFontProvider(fontProvider);
                return properties;
            });
            _generatePdf = generatePdf;
            _exportPdfApiConfig = exportPdfApiConfig;
        }

        public void CreatePdf(string html, string destpath)
        {
            using (var stream = File.Create(destpath))
            {
                HtmlConverter.ConvertToPdf(html, stream, PdfConverterProperties);
            }
        }
        public void CreatePdf(string html, Stream destStream)
        {
            HtmlConverter.ConvertToPdf(html, destStream, PdfConverterProperties);
        }

        public byte[] ExportFilePdfFromHtml(HtmlToPdfVo htmlToPdfVo)
        {
            byte[] bytes=null;
            //using (var workStream = new MemoryStream())
            //{
            //    using (var pdfWriter = new PdfWriter(workStream))
            //    {
            //        using (var pdfDoc = new PdfDocument(pdfWriter))
            //        {
            //            pdfDoc.SetDefaultPageSize(PageSize.A4.Rotate());

            //            ConverterProperties props = _converterPropertiesVariable.Value;
            //            HtmlConverter.ConvertToPdf(HttpUtility.HtmlDecode(html), pdfDoc, props);
            //        }
            //        bytes = workStream.ToArray();
            //    }
            //}
            if (htmlToPdfVo.UrlApi == null)
            {
                htmlToPdfVo.UrlApi = _exportPdfApiConfig.Url;
            }
            if (htmlToPdfVo.MethodApi == null)
            {
                htmlToPdfVo.MethodApi = _exportPdfApiConfig.Method;
            }
            if (htmlToPdfVo.Zoom != null)
            {
                htmlToPdfVo.Html += "<style>html{zoom:" + htmlToPdfVo.Zoom + "%;}</style>";
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(htmlToPdfVo.UrlApi);

                //HTTP POST
                var postTask = client.PostAsJsonAsync(htmlToPdfVo.MethodApi, htmlToPdfVo);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    bytes = result.Content.ReadAsAsync<byte[]>().Result;
                }
            }
            return bytes ?? _generatePdf.GetPDF(htmlToPdfVo.Html);
        }

        public byte[] ExportMultiFilePdfFromHtml(List<HtmlToPdfVo> htmlToPdfVos)
        {
            if (htmlToPdfVos != null && htmlToPdfVos.Count > 1)
            {
                var arr = new List<byte[]>();
                foreach (var htmlToPdfVo in htmlToPdfVos)
                {
                    arr.Add(ExportFilePdfFromHtml(htmlToPdfVo));
                }
                return MergeMultiPdf(arr);
            }
            else
            {
                if (htmlToPdfVos != null && htmlToPdfVos.Count == 1)
                {
                    return ExportFilePdfFromHtml(htmlToPdfVos[0]);
                }
                else
                {
                    return null;
                }

            }
        }
        public byte[] MergeMultiPdf(List<byte[]> pdfByteContent)
        {

            using (var ms = new MemoryStream())
            {
                using (var doc = new iTextSharp.text.Document())
                {
                    using (var copy = new PdfSmartCopy(doc, ms))
                    {
                        doc.Open();

                        //Loop through each byte array
                        foreach (var p in pdfByteContent)
                        {

                            //Create a PdfReader bound to that byte array
                            using (var reader = new PdfReader(p))
                            {

                                //Add the entire document instead of page-by-page
                                copy.AddDocument(reader);
                            }
                        }

                        doc.Close();
                    }
                }

                //Return just before disposing
                return ms.ToArray();
            }

        }

        private ConverterProperties PdfConverterProperties => _converterPropertiesVariable.Value;
    }
   
}