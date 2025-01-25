using BarcodeLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Camino.Core.Helpers
{
    public static class BarcodeHelper
    {
        private const string EndCharacter = "@";
        public static string GenerateBarCode(string value, int imageWidth = 150, int imageHeight = 30,bool? endCharacter=true)
        {
            if (value == null)
            {
                return null;
            }
            Barcode barcodeAPI = new Barcode();

            // Todo: config

            Color foreColor = Color.Black;
            Color backColor = Color.Transparent;

            Image barcodeImage = barcodeAPI.Encode(TYPE.CODE128, value + (endCharacter==true?EndCharacter:""), foreColor, backColor, imageWidth, imageHeight);
            string base64ImageRepresentation = Convert.ToBase64String(ImageToByte(barcodeImage));
            return base64ImageRepresentation;
        }

        private static byte[] ImageToByte(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }
      
        public static string EncodeStringsToContentBarcode(bool hasEndCharacter, params string[] values)
        {
            if (values == null)
            {
                return null;
            }
            var contentBarcode = string.Empty;
            foreach (var value in values)
            {
                contentBarcode += (contentBarcode != string.Empty ? "|" : string.Empty) + value.ToHexString();
            }

            if (hasEndCharacter)
                contentBarcode += "|" + EndCharacter;

            return contentBarcode;
        }

        public static string[] DecodeContentBarcodeToStrings(string contentBarcode)
        {
            if (contentBarcode == null)
                return null;
            contentBarcode = contentBarcode.Replace(EndCharacter, "");
            var contents = contentBarcode.Split("|");
            return contents.Select(o => o.DecodeHexString()).ToArray();
        }

        private static Bitmap ToBufferedImage(BitMatrix matrix, Color backgroundColor, Color foregroundColor)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            //            Color foreColor = Color.Black;
            //            Color backColor = Color.White;
            Bitmap image = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image.SetPixel(x, y, (matrix[x, y] ? foregroundColor : backgroundColor));
                }
            }
            return image;
        }
        public static string GenerateQrCode(string contentBarcode, Color backgroundColor, Color foregroundColor)
        {
            if (string.IsNullOrEmpty(contentBarcode))
            {
                return null;
            }

            var options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 500,
                Height = 500
            };
            var writer = new BarcodeWriterGeneric
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };
            var bitMap = ToBufferedImage(writer.Encode(contentBarcode), backgroundColor, foregroundColor);
            return Convert.ToBase64String(ImageToByte(bitMap));
        }

    }
}
