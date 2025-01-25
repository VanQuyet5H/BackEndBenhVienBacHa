using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    public static class ConvertXmlToBase64Helper
    {
        public static string XMLtoBase64(string path)
        {
            Byte[] bytes = System.IO.File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
        public static void Base64ToXML(string path,string base64String)
        {
            Byte[] bytes = Convert.FromBase64String(base64String);
            System.IO.File.WriteAllBytes(path, bytes);
        }
        public static Byte[] XMLtoBytes(string path)
        {
            string xmlData = System.IO.File.ReadAllText(path);

            Byte[] dataBuffer = Encoding.UTF8.GetBytes(xmlData);
            return dataBuffer;
        }
        public static Byte[] XMLtoBytesString(string path)
        {

            Byte[] dataBuffer = Encoding.UTF8.GetBytes(path);
            return dataBuffer;
        }
    }
}
