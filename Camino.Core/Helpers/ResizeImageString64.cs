using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Camino.Core.Configuration;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Core.Helpers
{
    public static class ResizeImageString64
    {
        public static string ResizeImage(string encodedImage, Size size)
        {

            Bitmap imgToResize = Base64StringToBitmap(encodedImage);
          
            float ratio = 1;
            float minSize = Math.Min(size.Height, size.Width);

            if (imgToResize.Width > imgToResize.Height)
            {
                ratio = minSize / (float)imgToResize.Width;
            }
            else
            {
                ratio = minSize / (float)imgToResize.Height;
            }
            try
            {
                SizeF newSize = new SizeF(imgToResize.Width * ratio, imgToResize.Height * ratio);
                Bitmap target = new Bitmap((int)newSize.Width, (int)newSize.Height);
                using (Graphics g = Graphics.FromImage((Image)target))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, newSize.Width, newSize.Height);
                }

                using (var ms = new MemoryStream())
                {
                    using (var bitmap = new Bitmap(target))
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        var stringBase64 = Convert.ToBase64String(ms.GetBuffer());
                        return "data:image/png;base64," + stringBase64;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return string.Empty;
            }
        }

        private static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            string[] pd = base64String.Split(',');
            byte[] byteBuffer = Convert.FromBase64String(pd[1]);

            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
        }
    }
}
