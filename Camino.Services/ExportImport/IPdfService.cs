using System;
using System.Collections.Generic;
using System.IO;
using Camino.Core.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.ExportImport
{
    public interface IPdfService
    {
        void CreatePdf(string html, string destpath);
        void CreatePdf(string html, Stream destStream);
        byte[] ExportFilePdfFromHtml(HtmlToPdfVo htmlToPdfVo);
        byte[] ExportMultiFilePdfFromHtml(List<HtmlToPdfVo> htmlToPdfVos);
        byte[] MergeMultiPdf(List<byte[]> pdfByteContent);
    }
}