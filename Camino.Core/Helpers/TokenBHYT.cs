using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Camino.Core.Helpers
{
    public static class TokenBHYT
    {
        public static TokenBHYTModel GetTokenAPI()
        {
            var token = new TokenBHYTModel();
            var path = @"Resource\\Token.xml";
            using (var reader = XmlReader.Create(path))
            {
                XDocument data = XDocument.Load(reader);
                XNamespace root = data.Root.GetDefaultNamespace();
                XElement tokenXML = data.Descendants(root + "BHYT").FirstOrDefault();
                token.token = (string)tokenXML.Element(root + "Token");
                token.id_token = (string)tokenXML.Element(root + "TokenId");

                return token;
            }
        }
        public static FileDownLoad GetValueFileDownload(string path , string nameFile)
        {
            var token = new FileDownLoad();
            XDocument data = XDocument.Load(@path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement tokenXML = data.Descendants(root + "DOWNLOAD").FirstOrDefault();
            token.StringValue = (string)tokenXML.Element(root + nameFile);
            token.size = data.Descendants(root + "DOWNLOAD").Count();
            return token;
        }
        public static SoThuTuModel GetSoThuTu()
        {
            var stt = new SoThuTuModel();
            var path = @"Resource\\SoThuTu.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement XML = data.Descendants(root + "XML1").FirstOrDefault();
            stt.STTXMl1 = Convert.ToInt16(XML.Element(root + "SoThuTu").Value);
            XML = data.Descendants(root + "XML2").FirstOrDefault();
            stt.STTXMl2 = Convert.ToInt16(XML.Element(root + "SoThuTu").Value);
            XML = data.Descendants(root + "XML3").FirstOrDefault();
            stt.STTXMl3 = Convert.ToInt16(XML.Element(root + "SoThuTu").Value);
            XML = data.Descendants(root + "XML4").FirstOrDefault();
            stt.STTXMl4 = Convert.ToInt16(XML.Element(root + "SoThuTu").Value);
            XML = data.Descendants(root + "XML5").FirstOrDefault();
            stt.STTXMl5 = Convert.ToInt16(XML.Element(root + "SoThuTu").Value);
            return stt;
        }
        public static void ModifyTokenBHYT(string token ,string id_token)
        {
            var path = @"Resource\\Token.xml";


            try
            {
                XDocument data = XDocument.Load(path);
                XNamespace root = data.Root.GetDefaultNamespace();
                XElement hdKho = data.Descendants(root + "BHYT").FirstOrDefault();
                XElement ctheTam = data.Descendants("BHYT").FirstOrDefault();
                ctheTam.Element("Token").Value = token;
                ctheTam.Element("TokenId").Value = id_token;
                data.Save(path);
            }
            catch (Exception e)
            {
            }
           


        }
        public static void ModifySoThuTu(string XML, int value)
        {
            
            var path = @"Resource\\SoThuTu.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement hdKho = data.Descendants(root + XML).FirstOrDefault();
            XElement ctheTam = data.Descendants(XML).FirstOrDefault();
            ctheTam.Element("SoThuTu").Value = value.ToString();
            data.Save(path);
        }
        public static void ModifyNgayLapFileTongHop(string path)
        {

           
            XDocument data = XDocument.Load(@path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement hdKho = data.Descendants(root + "THONGTINHOSO").FirstOrDefault();
            XElement ctheTam = data.Descendants("THONGTINHOSO").FirstOrDefault();
            ctheTam.Element("NGAYLAP").Value = DateTime.Now.Year+(DateTime.Now.Month>9? DateTime.Now.Month.ToString():"0"+ DateTime.Now.Month)+ (DateTime.Now.Day > 9 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day);
            data.Save(path);
        }
        public static void ResetSoThuTu()
        {
            var path = @"Resource\\SoThuTu.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement hdKho = data.Descendants(root + "XML1").FirstOrDefault();
            XElement ctheTam = data.Descendants("XML1").FirstOrDefault();
            ctheTam.Element("SoThuTu").Value = "1";
            hdKho = data.Descendants(root + "XML2").FirstOrDefault();
            ctheTam = data.Descendants("XML2").FirstOrDefault();
            ctheTam.Element("SoThuTu").Value = "1";
             hdKho = data.Descendants(root + "XML3").FirstOrDefault();
            ctheTam = data.Descendants("XML3").FirstOrDefault();
            ctheTam.Element("SoThuTu").Value = "1";
             hdKho = data.Descendants(root + "XML4").FirstOrDefault();
             ctheTam = data.Descendants("XML4").FirstOrDefault();
            ctheTam.Element("SoThuTu").Value = "1";
             hdKho = data.Descendants(root + "XML5").FirstOrDefault();
             ctheTam = data.Descendants("XML5").FirstOrDefault();
            ctheTam.Element("SoThuTu").Value = "1";
            data.Save(path);
        }
    }
}
