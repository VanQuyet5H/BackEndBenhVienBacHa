using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Configuration;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Core.Helpers
{
    public static class FileHelper
    {
        public static int GetLoaiTapTin(string extension, FileTypeConfig fileTypeConfig)
        {
            var arrImageExt = fileTypeConfig.Image.Split(',');
            if (arrImageExt.IndexOf(extension.ToUpper()) >= 0|| arrImageExt.IndexOf(extension.ToLower()) >= 0)
            {
                return (int)Enums.LoaiTapTin.Image;
            }
            else
            {
                var arrPdfExt = fileTypeConfig.Pdf.Split(',');
                if (arrPdfExt.IndexOf(extension.ToUpper()) >= 0 || arrPdfExt.IndexOf(extension.ToLower()) >= 0)
                {
                    return (int)Enums.LoaiTapTin.Pdf;
                }
                else
                {
                    return (int)Enums.LoaiTapTin.Khac;
                }
            }
        }
    }
}
