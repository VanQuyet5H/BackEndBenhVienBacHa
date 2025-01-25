using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.HamGuiHoSoWatchings;
using Camino.Core.Helpers;
using Camino.Core.Infrastructure;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BHYT
{
    [ScopedDependency(ServiceType = typeof(IBaoHiemYTeService))]
    public class BaoHiemYTeService : IBaoHiemYTeService
    {
        private ILoggerManager _logger;
        private const string FormatNgayBHYT = "yyyyMMdd";
        private const string FormatNgayGioBHYT = "yyyyMMddHHmm";

        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;
        public BaoHiemYTeService(ILoggerManager logger, IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository)
        {
            _logger = logger;
            _benhVienRepository = benhVienRepository;
        }

        #region Add thông tin vào xml

        public async Task<HamGuiHoSoWatchingVO> GoiHoSoGiamDinh(ThongTinBenhNhan thongtin, bool isDownLoad)
        {
            var ThangQuyetToan = thongtin.ThangQuyetToan < 10 ? "0" + thongtin.ThangQuyetToan : thongtin.ThangQuyetToan.ToString();
            var tenNam = thongtin.NamQuyetToan + ThangQuyetToan;
            var result = new HamGuiHoSoWatchingVO();
            result.XMLJson = "{\"";
            string fileXML1 = null;
            string fileXML2 = null;
            string fileXML3 = null;
            string fileXML4 = null;
            string fileXML5 = null;
            int count = 0;
            string filedownload = null;
            if (isDownLoad == true)

            {
                filedownload = "Resource\\Download_01824_" + tenNam + "_" + thongtin.MaBenhNhan + ".xml";
                XDocument myXml = new XDocument(new XElement("DOWNLOAD", ""));

                myXml.Save(@filedownload);
            }
            //DeleteValue(@"Resource\\TongHop.xml", "HOSO");
            string tenFileXMLTongHop = "Resource\\TongHop_01824_" + tenNam + "_" + thongtin.MaBenhNhan + ".xml";
            XDocument myXml1 = new XDocument(new XElement("GIAMDINHHS", new XAttribute("xmlnsxsd", "http://www.w3.org/2001/XMLSchema"), new XAttribute("xmlnsxsi", "http://www.w3.org/2001/XMLSchema-instance"),
                                             new XElement("THONGTINDONVI", new XElement("MACSKCB", "01824")), new XElement("THONGTINHOSO", new XElement("NGAYLAP", "20191220"), new XElement("SOLUONGHOSO", "1"), new XElement("DANHSACHHOSO", new XElement("HOSO", ""))),
                                             new XElement("CHUKYDONVI", null)
                                             ));
            result.NameFileDown = tenFileXMLTongHop;
            myXml1.Save(@tenFileXMLTongHop);
            //AddHoSoToDanhSachHoSoTongHopXML();
            var sothutu = TokenBHYT.GetSoThuTu();
            if (thongtin != null)
            {
                result.XMLJson = result.XMLJson + "XML1\":";

                try
                {
                    TokenBHYT.ModifyNgayLapFileTongHop(@tenFileXMLTongHop);
                    thongtin.STT = sothutu.STTXMl1;
                    string tenFileXML1 = "Resource\\XML1_01824_" + tenNam + "_" + thongtin.MaBenhNhan + ".xml";
                    XDocument myXml = new XDocument(new XElement("TONG_HOP", ""));
                    myXml.Save(@tenFileXML1);
                    //ReplaceCharacterAdd(@"Resource\\XML1.xml");
                    AddThongTinBenhNhanVoXML(thongtin, @tenFileXML1);
                    fileXML1 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@tenFileXML1));
                    AddFileHoSoToHoSoTongHopXML(fileXML1, "XML1", @tenFileXMLTongHop);
                    result.TenFileVOs.Add(new TenFileVO { TenFile = "XML1_01824_" + tenNam + "_" + thongtin.MaBenhNhan, DuLieu = fileXML1 });
                    if (isDownLoad == true)
                    {
                        AddFileDown("XML1", fileXML1, filedownload);
                        count++;
                    }
                    result.XMLJson = result.XMLJson + fileXML1 + ",\"";
                    TokenBHYT.ModifySoThuTu("XML1", sothutu.STTXMl1 + 1);
                    //await DeleteFile(tenFileXML1);
                }
                catch (Exception ex)
                {
                    result.XMLError = "XML1: " + ex.Message;
                    result.XMLJson = result.XMLJson + "}";
                    result.ErrorCheck = true;
                    return result;
                }
            }
            if (thongtin.HoSoChiTietThuoc.Count() != 0)
            {

                result.XMLJson = result.XMLJson + "XML2\":";
                try
                {
                    string tenFileXML2 = "Resource\\XML2_01824_" + tenNam + "_" + thongtin.MaBenhNhan + ".xml";
                    XDocument myXml = new XDocument(new XElement("DSACH_CHI_TIET_THUOC", ""));
                    myXml.Save(@tenFileXML2);
                    //ReplaceCharacterAdd(@"Resource\\XML2.xml");
                    for (int i = 0; i < thongtin.HoSoChiTietThuoc.Count; i++)
                    {
                        thongtin.HoSoChiTietThuoc[i].STT = sothutu.STTXMl2;
                        thongtin.HoSoChiTietThuoc[i].NgayYLenh = thongtin.HoSoChiTietThuoc[i].NgayYLenh;
                        thongtin.HoSoChiTietThuoc[i].MaLienKet = thongtin.MaLienKet;
                        thongtin.HoSoChiTietThuoc[i].SoLuong = Math.Round(thongtin.HoSoChiTietThuoc[i].SoLuong, 3);
                        thongtin.HoSoChiTietThuoc[i].DonGia = Math.Round(thongtin.HoSoChiTietThuoc[i].DonGia, 3);
                        AddHoSoChiTietThuocVoXML(thongtin.HoSoChiTietThuoc[i], @tenFileXML2);
                        TokenBHYT.ModifySoThuTu("XML2", sothutu.STTXMl2 + 1);
                        sothutu = TokenBHYT.GetSoThuTu();
                    }
                    ReplaceCharacter(@tenFileXML2);
                    fileXML2 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@tenFileXML2));
                    result.XMLJson = result.XMLJson + fileXML2 + ",\"";
                    AddFileHoSoToHoSoTongHopXML(fileXML2, "XML2", @tenFileXMLTongHop);
                    result.TenFileVOs.Add(new TenFileVO { TenFile = "XML2_01824_" + tenNam + "_" + thongtin.MaBenhNhan, DuLieu = fileXML2 });
                    if (isDownLoad == true)
                    {
                        AddFileDown("XML2", fileXML2, filedownload);
                        count++;
                    }
                    //DeleteValue(@"Resource\\XML2.xml", "CHI_TIET_THUOC");
                    await DeleteFile(tenFileXML2);
                }
                catch (Exception ex)
                {
                    result.XMLError = "XML2: " + ex.Message;
                    result.XMLJson = result.XMLJson + "}";
                    result.ErrorCheck = true;
                    return result;
                }

            }
            if (thongtin.HoSoChiTietDVKT.Count() != 0)
            {
                result.XMLJson = result.XMLJson + "XML3\":";
                try
                {
                    string tenFileXML3 = "Resource\\XML3_01824_" + tenNam + "_" + thongtin.MaBenhNhan + ".xml";
                    XDocument myXml = new XDocument(new XElement("DSACH_CHI_TIET_DVKT", ""));
                    myXml.Save(@tenFileXML3);
                    for (int i = 0; i < thongtin.HoSoChiTietDVKT.Count; i++)
                    {

                        thongtin.HoSoChiTietDVKT[i].STT = sothutu.STTXMl3;
                        thongtin.HoSoChiTietDVKT[i].MaLienKet = thongtin.MaLienKet;
                        thongtin.HoSoChiTietDVKT[i].NgayYLenh = thongtin.HoSoChiTietDVKT[i].NgayYLenh;
                        thongtin.HoSoChiTietDVKT[i].NgayKetQua = thongtin.HoSoChiTietDVKT[i].NgayKetQua;
                        thongtin.HoSoChiTietDVKT[i].SoLuong = Math.Round(thongtin.HoSoChiTietDVKT[i].SoLuong, 3);
                        thongtin.HoSoChiTietDVKT[i].DonGia = Math.Round(thongtin.HoSoChiTietDVKT[i].DonGia, 3);
                        AddHoSoChiTietDVKTVoXML(thongtin.HoSoChiTietDVKT[i], @tenFileXML3);
                        TokenBHYT.ModifySoThuTu("XML3", sothutu.STTXMl3 + 1);
                        sothutu = TokenBHYT.GetSoThuTu();
                    }
                    ReplaceCharacter(@tenFileXML3);
                    fileXML3 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@tenFileXML3));
                    result.XMLJson = result.XMLJson + fileXML3 + ",\"";
                    AddFileHoSoToHoSoTongHopXML(fileXML3, "XML3", @tenFileXMLTongHop);
                    result.TenFileVOs.Add(new TenFileVO { TenFile = "XML3_01824_" + tenNam + "_" + thongtin.MaBenhNhan, DuLieu = fileXML3 });
                    if (isDownLoad == true)
                    {
                        AddFileDown("XML3", fileXML3, filedownload);
                        count++;
                    }
                    // DeleteValue(@"Resource\\XML3.xml", "CHI_TIET_DVKT");
                    await DeleteFile(tenFileXML3);
                }
                catch (Exception ex)
                {
                    result.XMLError = "XML3: " + ex.Message;
                    result.XMLJson = result.XMLJson + "}";
                    result.ErrorCheck = true;
                    return result;
                }


            }
            if (thongtin.HoSoCanLamSang.Count() != 0)
            {
                result.XMLJson = result.XMLJson + "XML4\":";
                try
                {
                    string tenFileXML4 = "Resource\\XML4_01824_" + tenNam + "_" + thongtin.MaBenhNhan + ".xml";
                    XDocument myXml = new XDocument(new XElement("DSACH_CHI_TIET_CLS", ""));
                    myXml.Save(@tenFileXML4);
                    //ReplaceCharacterAdd(@"Resource\\XML4.xml");
                    for (int i = 0; i < thongtin.HoSoCanLamSang.Count; i++)
                    {
                        thongtin.HoSoCanLamSang[i].STT = sothutu.STTXMl4;
                        thongtin.HoSoCanLamSang[i].MaLienKet = thongtin.MaLienKet;
                        thongtin.HoSoCanLamSang[i].NgayKQ = thongtin.HoSoCanLamSang[i].NgayKQ;
                        AddHoSoCanLamSangVoXML(thongtin.HoSoCanLamSang[i], @tenFileXML4);
                        TokenBHYT.ModifySoThuTu("XML4", sothutu.STTXMl4 + 1);
                        sothutu = TokenBHYT.GetSoThuTu();
                    }
                    ReplaceCharacter(@tenFileXML4);
                    fileXML4 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@tenFileXML4));
                    result.XMLJson = result.XMLJson + fileXML4 + ",\"";
                    AddFileHoSoToHoSoTongHopXML(fileXML4, "XML4", @tenFileXMLTongHop);
                    result.TenFileVOs.Add(new TenFileVO { TenFile = "XML4_01824_" + tenNam + "_" + thongtin.MaBenhNhan, DuLieu = fileXML4 });
                    if (isDownLoad == true)
                    {
                        AddFileDown("XML4", fileXML4, filedownload);
                        count++;
                    }
                    await DeleteFile(tenFileXML4);
                    // DeleteValue(@"Resource\\XML4.xml", "CHI_TIET_CLS");
                }
                catch (Exception ex)
                {
                    result.XMLError = "XML4: " + ex.Message;
                    result.XMLJson = result.XMLJson + "}";
                    result.ErrorCheck = true;
                    return result;
                }

            }
            if (thongtin.HoSoChiTietDienBienBenh.Count() != 0)
            {

                result.XMLJson = result.XMLJson + "XML5\":";
                try
                {
                    string tenFileXML5 = "Resource\\XML5_01824_" + tenNam + "_" + thongtin.MaBenhNhan + ".xml";
                    XDocument myXml = new XDocument(new XElement("DSACH_CHI_TIET_DIEN_BIEN_BENH", ""));
                    myXml.Save(@tenFileXML5);
                    //ReplaceCharacterAdd(@"Resource\\XML5.xml");
                    for (int i = 0; i < thongtin.HoSoChiTietDienBienBenh.Count; i++)
                    {
                        thongtin.HoSoChiTietDienBienBenh[i].STT = sothutu.STTXMl5;
                        thongtin.HoSoChiTietDienBienBenh[i].MaLienKet = thongtin.MaLienKet;
                        thongtin.HoSoChiTietDienBienBenh[i].NgayYLenh = thongtin.HoSoChiTietDienBienBenh[i].NgayYLenh;
                        AddHoSoChiTietDienBienBenhVoXML(thongtin.HoSoChiTietDienBienBenh[i], @tenFileXML5);
                        TokenBHYT.ModifySoThuTu("XML5", sothutu.STTXMl5 + 1);
                        sothutu = TokenBHYT.GetSoThuTu();
                    }
                    ReplaceCharacter(@tenFileXML5);
                    fileXML5 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@tenFileXML5));
                    result.XMLJson = result.XMLJson + fileXML5;
                    if (isDownLoad == true)
                    {
                        AddFileDown("XML5", fileXML5, filedownload);
                        count++;
                    }
                    AddFileHoSoToHoSoTongHopXML(fileXML5, "XML5", @tenFileXMLTongHop);
                    result.TenFileVOs.Add(new TenFileVO { TenFile = "XML5_01824_" + tenNam + "_" + thongtin.MaBenhNhan, DuLieu = fileXML5 });
                    await DeleteFile(tenFileXML5);
                    //DeleteValue(@"Resource\\XML5.xml", "CHI_TIET_DIEN_BIEN_BENH");
                }
                catch (Exception ex)
                {
                    result.XMLError = "XML5: " + ex.Message;
                    result.XMLJson = result.XMLJson + "}";
                    result.ErrorCheck = true;
                    return result;
                }

            }
            try
            {
                result.XMLJson = result.XMLJson + "}";
                var Bytearray = ConvertXmlToBase64Helper.XMLtoBytes(@tenFileXMLTongHop);
                result.ByteData = Bytearray;
                result.ErrorCheck = false;
                TokenBHYT.ResetSoThuTu();
                //await DeleteFile(tenFileXMLTongHop);
                result.countFile = count;
            }
            catch (Exception ex)
            {
                result.XMLError = "Finally: " + ex.Message;
                result.XMLJson = result.XMLJson + "}";
                result.ErrorCheck = true;
                return result;
            }

            return result;
        }
        public HamGuiHoSoWatchingVO addValueToXml(List<ThongTinBenhNhan> thongTinBenhNhans)
        {
            var result = new HamGuiHoSoWatchingVO();
            string tenFileXMLTongHop = string.Empty;
            XDocument myXml1 = new XDocument();
            if (thongTinBenhNhans.Count > 1)
            {
                tenFileXMLTongHop = "Resource\\TongHop_01824.xml";
                myXml1 = new XDocument(new XElement("GIAMDINHHS", new XAttribute("xmlnsxsd", "http://www.w3.org/2001/XMLSchema"), new XAttribute("xmlnsxsi", "http://www.w3.org/2001/XMLSchema-instance"),
                                                new XElement("THONGTINDONVI", new XElement("MACSKCB", "01824")), new XElement("THONGTINHOSO", new XElement("NGAYLAP", "20191220"), new XElement("SOLUONGHOSO", "1"), new XElement("DANHSACHHOSO", new XElement("HOSO", ""))),
                                                new XElement("CHUKYDONVI", null)
                                                ));
            }
            else
            {
                var ThangQuyetToan = thongTinBenhNhans.FirstOrDefault().ThangQuyetToan < 10 ? "0" + thongTinBenhNhans.FirstOrDefault().ThangQuyetToan : thongTinBenhNhans.FirstOrDefault().ThangQuyetToan.ToString();
                var tenNam = thongTinBenhNhans.FirstOrDefault().NamQuyetToan + ThangQuyetToan;
                tenFileXMLTongHop = "Resource\\TongHop_01824_" + tenNam + "_" + thongTinBenhNhans.FirstOrDefault().MaBenhNhan + ".xml";
                myXml1 = new XDocument(new XElement("GIAMDINHHS", new XAttribute("xmlnsxsd", "http://www.w3.org/2001/XMLSchema"), new XAttribute("xmlnsxsi", "http://www.w3.org/2001/XMLSchema-instance"),
                                                new XElement("THONGTINDONVI", new XElement("MACSKCB", "01824")), new XElement("THONGTINHOSO", new XElement("NGAYLAP", "20191220"), new XElement("SOLUONGHOSO", "1"), new XElement("DANHSACHHOSO", new XElement("HOSO", ""))),
                                                new XElement("CHUKYDONVI", null)
                                                ));
            }

            result.NameFileDown = tenFileXMLTongHop;
            myXml1.Save(@tenFileXMLTongHop);
            var sothutu = TokenBHYT.GetSoThuTu();
            foreach (var thongtin in thongTinBenhNhans)
            {
                int count = 0;
                if (thongtin != null)
                {
                    try
                    {
                        TokenBHYT.ModifyNgayLapFileTongHop(@tenFileXMLTongHop);
                        thongtin.STT = sothutu.STTXMl1;
                        AddThongTinBenhNhanVoXMLTongHop(thongtin, "XML1", @tenFileXMLTongHop);
                        TokenBHYT.ModifySoThuTu("XML1", sothutu.STTXMl1 + 1);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML1: " + ex.Message;
                        return result;
                    }
                }
                if (thongtin.HoSoChiTietThuoc.Count() != 0)
                {
                    try
                    {
                        AddHoSoChiTietThuocVoXMLTongHop(thongtin.HoSoChiTietThuoc, "XML2", @tenFileXMLTongHop);
                        TokenBHYT.ModifySoThuTu("XML2", sothutu.STTXMl2 + 1);
                        sothutu = TokenBHYT.GetSoThuTu();
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML2: " + ex.Message;
                        return result;
                    }

                }
                if (thongtin.HoSoChiTietDVKT.Count() != 0)
                {
                    try
                    {
                        AddHoSoChiTietDVKTVoXMLTongHop(thongtin.HoSoChiTietDVKT, "XML3", @tenFileXMLTongHop);
                        TokenBHYT.ModifySoThuTu("XML3", sothutu.STTXMl3 + 1);
                        sothutu = TokenBHYT.GetSoThuTu();

                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML3: " + ex.Message;
                        return result;
                    }


                }
                if (thongtin.HoSoCanLamSang.Count() != 0)
                {
                    try
                    {
                        for (int i = 0; i < thongtin.HoSoCanLamSang.Count; i++)
                        {
                            thongtin.HoSoCanLamSang[i].STT = sothutu.STTXMl4;
                            thongtin.HoSoCanLamSang[i].MaLienKet = thongtin.MaLienKet;
                            thongtin.HoSoCanLamSang[i].NgayKQ = thongtin.HoSoCanLamSang[i].NgayKQ;
                            AddHoSoCanLamSangVoXMLTongHop(thongtin.HoSoCanLamSang[i], "XML4", @tenFileXMLTongHop);
                            TokenBHYT.ModifySoThuTu("XML4", sothutu.STTXMl4 + 1);
                            sothutu = TokenBHYT.GetSoThuTu();
                        }
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML4: " + ex.Message;
                        return result;
                    }

                }
                if (thongtin.HoSoChiTietDienBienBenh.Count() != 0)
                {
                    try
                    {
                        for (int i = 0; i < thongtin.HoSoChiTietDienBienBenh.Count; i++)
                        {
                            thongtin.HoSoChiTietDienBienBenh[i].STT = sothutu.STTXMl5;
                            thongtin.HoSoChiTietDienBienBenh[i].MaLienKet = thongtin.MaLienKet;
                            thongtin.HoSoChiTietDienBienBenh[i].NgayYLenh = thongtin.HoSoChiTietDienBienBenh[i].NgayYLenh;
                            AddHoSoChiTietDienBienBenhVoXMLTongHop(thongtin.HoSoChiTietDienBienBenh[i], "XML5", @tenFileXMLTongHop);
                            TokenBHYT.ModifySoThuTu("XML5", sothutu.STTXMl5 + 1);
                            sothutu = TokenBHYT.GetSoThuTu();
                        }
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML5: " + ex.Message;
                        return result;
                    }

                }
                try
                {
                    var Bytearray = ConvertXmlToBase64Helper.XMLtoBytes(@tenFileXMLTongHop);
                    result.ByteData = Bytearray;
                    result.ErrorCheck = false;
                    TokenBHYT.ResetSoThuTu();
                    result.countFile = count;
                }
                catch (Exception ex)
                {
                    return result;
                }
            }


            return result;
        }

        public byte[] pathFileTongHop(string tenFile)
        {
            var byteArray = ConvertXmlToBase64Helper.XMLtoBytes(tenFile);
            return byteArray;
        }
        private void AddThongTinBenhNhanVoXMLTongHop(ThongTinBenhNhan thongtin, string nameFile, string @pathTongHop)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", "")));
            data.Descendants("NOIDUNGFILE").Last().Add(new XElement("TONG_HOP", new XElement("MA_LK", thongtin.MaLienKet), new XElement("STT", thongtin.STT), new XElement("MA_BN", thongtin.MaBenhNhan),
                                                          new XElement("HO_TEN", "![CDATA[" + thongtin.HoTen + "]]"), new XElement("NGAY_SINH", thongtin.NgaySinh),
                                                          new XElement("GIOI_TINH", ((int)thongtin.GioiTinh).ToString()), new XElement("DIA_CHI", "![CDATA[" + thongtin.DiaChi + "]]"),
                                                          new XElement("MA_THE", thongtin.MaThe), new XElement("MA_DKBD", thongtin.MaCoSoKCBBanDau),
                                                          new XElement("GT_THE_TU", thongtin.GiaTriTheTu), new XElement("GT_THE_DEN", thongtin.GiaTriTheDen?.ToString(FormatNgayBHYT) ?? ""),
                                                          new XElement("MIEN_CUNG_CT", thongtin.MienCungChiTra?.ToString(FormatNgayBHYT) ?? ""), new XElement("TEN_BENH", "![CDATA[" + thongtin.TenBenh + "]]"),
                                                          new XElement("MA_BENH", thongtin.MaBenh), new XElement("MA_BENHKHAC", thongtin.MaBenhKhac == null ? "" : thongtin.MaBenhKhac),
                                                          new XElement("MA_LYDO_VVIEN", ((int)thongtin.LyDoVaoVien).ToString()), new XElement("MA_NOI_CHUYEN", thongtin.MaNoiChuyen == null ? "" : thongtin.MaNoiChuyen),
                                                          new XElement("MA_TAI_NAN", thongtin.MaTaiNan == null ? "" : ((int)thongtin.MaTaiNan).ToString()), new XElement("NGAY_VAO", thongtin.NgayVao),
                                                          new XElement("NGAY_RA", thongtin.NgayRa), new XElement("SO_NGAY_DTRI", thongtin.SoNgayDieuTri.ToString()),
                                                          new XElement("KET_QUA_DTRI", thongtin.KetQuaDieuTri),
                                                          new XElement("TINH_TRANG_RV", thongtin.TinhTrangRaVien),
                                                          new XElement("NGAY_TTOAN", thongtin.NgayThanhToan?.ToString(FormatNgayGioBHYT) ?? ""),
                                                          new XElement("T_THUOC", thongtin.TienThuoc.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienThuoc)), new XElement("T_VTYT", thongtin.TienVatTuYTe.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienVatTuYTe)),
                                                          new XElement("T_TONGCHI", String.Format("{0:0.00}", thongtin.TienTongChi)), new XElement("T_BNTT", thongtin.TienBenhNhanTuTra.ToString() == null ? String.Format("{0:0.00}", 0) : String.Format("{0:0.00}", thongtin.TienBenhNhanTuTra)), new XElement("T_BNCCT", thongtin.TienBenhNhanCungChiTra.ToString() == null ? String.Format("{0:0.00}", 0) : String.Format("{0:0.00}", thongtin.TienBenhNhanCungChiTra)),
                                                          new XElement("T_BHTT", String.Format("{0:0.00}", thongtin.TienBaoHiemThanhToan)), new XElement("T_NGUONKHAC", thongtin.TienNguonKhac.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienNguonKhac)),
                                                          new XElement("T_NGOAIDS", thongtin.TienNgoaiDinhSuat.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienNgoaiDinhSuat)), new XElement("NAM_QT", thongtin.NamQuyetToan.ToString()),
                                                          new XElement("THANG_QT", thongtin.ThangQuyetToan.ToString()), new XElement("MA_LOAI_KCB", ((int)thongtin.MaLoaiKCB).ToString()),
                                                          new XElement("MA_KHOA", thongtin.MaKhoa), new XElement("MA_CSKCB", thongtin.MaCSKCB),
                                                          new XElement("MA_KHUVUC", thongtin.MaKhuVuc == null ? "" : thongtin.MaKhuVuc), new XElement("MA_PTTT_QT", thongtin.MaPhauThuatQuocTe == null ? "" : thongtin.MaPhauThuatQuocTe),
                                                          new XElement("CAN_NANG", thongtin.CanNang.ToString() == null ? "" : thongtin.CanNang.ToString())));
            data.Save(@pathTongHop);

        }
        private void AddHoSoChiTietThuocVoXMLTongHop(List<HoSoChiTietThuoc> hoSoChiTietThuocs, string nameFile, string @pathTongHop)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", ""), new XElement("DSACH_CHI_TIET_THUOC", "")));
            for (int i = 0; i < hoSoChiTietThuocs.Count; i++)
            {
                hoSoChiTietThuocs[i].STT = i + 1;
                hoSoChiTietThuocs[i].NgayYLenh = hoSoChiTietThuocs[i].NgayYLenh;
                hoSoChiTietThuocs[i].MaLienKet = hoSoChiTietThuocs[i].MaLienKet;
                hoSoChiTietThuocs[i].SoLuong = Math.Round(hoSoChiTietThuocs[i].SoLuong, 3);
                hoSoChiTietThuocs[i].DonGia = Math.Round(hoSoChiTietThuocs[i].DonGia, 3);
                

                data.Descendants("DSACH_CHI_TIET_THUOC").Last().Add(
                        new XElement("CHI_TIET_THUOC",
                        new XElement("MA_LK", hoSoChiTietThuocs[i].MaLienKet),
                        new XElement("STT", hoSoChiTietThuocs[i].STT),
                        new XElement("MA_THUOC", hoSoChiTietThuocs[i].MaThuoc),
                        new XElement("MA_NHOM", ((int)hoSoChiTietThuocs[i].MaNhom).ToString()),
                        new XElement("TEN_THUOC", "![CDATA[" + hoSoChiTietThuocs[i].TenThuoc + "]]"),
                        new XElement("DON_VI_TINH", hoSoChiTietThuocs[i].DonViTinh),
                        new XElement("HAM_LUONG", hoSoChiTietThuocs[i].HamLuong == null ? "![CDATA[]]" : "![CDATA[" + hoSoChiTietThuocs[i].HamLuong + "]]"),
                        new XElement("DUONG_DUNG", hoSoChiTietThuocs[i].DuongDung == null ? "" : hoSoChiTietThuocs[i].DuongDung),
                        new XElement("LIEU_DUNG", hoSoChiTietThuocs[i].LieuDung == null ? "![CDATA[]]" : "![CDATA[" + hoSoChiTietThuocs[i].LieuDung + "]]"),
                        new XElement("SO_DANG_KY", hoSoChiTietThuocs[i].SoDangKy == null ? "" : hoSoChiTietThuocs[i].SoDangKy),
                        new XElement("TT_THAU", hoSoChiTietThuocs[i].ThongTinThau == null ? "" : hoSoChiTietThuocs[i].ThongTinThau),
                        new XElement("PHAM_VI", hoSoChiTietThuocs[i].PhamVi),
                        new XElement("SO_LUONG", String.Format("{0:0.000}", hoSoChiTietThuocs[i].SoLuong)),
                        new XElement("DON_GIA", String.Format("{0:0.000}", hoSoChiTietThuocs[i].DonGia)),
                        new XElement("TYLE_TT", hoSoChiTietThuocs[i].TyLeThanhToan.ToString()),
                        new XElement("THANH_TIEN", String.Format("{0:0.00}", hoSoChiTietThuocs[i].ThanhTien)),
                        new XElement("MUC_HUONG", hoSoChiTietThuocs[i].MucHuong.ToString()),
                        new XElement("T_NGUONKHAC", hoSoChiTietThuocs[i].TienNguonKhac == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietThuocs[i].TienNguonKhac), 2)))),
                        new XElement("T_BNTT", hoSoChiTietThuocs[i].TienBenhNhanTuTra == null ? "0.00" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietThuocs[i].TienBenhNhanTuTra), 2)))),
                        new XElement("T_BHTT", String.Format("{0:0.00}", hoSoChiTietThuocs[i].TienBaoHiemThanhToan)),
                        new XElement("T_BNCCT", hoSoChiTietThuocs[i].TienBenhNhanCungChiTra == null ? "0.00" : String.Format("{0:0.00}", String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietThuocs[i].TienBenhNhanCungChiTra), 2))))),
                        new XElement("T_NGOAIDS", hoSoChiTietThuocs[i].TienNgoaiDinhSuat == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietThuocs[i].TienNgoaiDinhSuat), 2)))),
                        new XElement("MA_KHOA", hoSoChiTietThuocs[i].MaKhoa),
                        new XElement("MA_BAC_SI", hoSoChiTietThuocs[i].MaBacSi.ToString()),
                        new XElement("MA_BENH", hoSoChiTietThuocs[i].MaBenh),
                        new XElement("NGAY_YL", hoSoChiTietThuocs[i].NgayYLenh.ToString()),
                        new XElement("MA_PTTT", ((int)hoSoChiTietThuocs[i].MaPhuongThucThanhToan).ToString())
                        ));
            }


            data.Save(@pathTongHop);
        }
        private void AddHoSoChiTietDVKTVoXMLTongHop(List<HoSoChiTietDVKT> hoSoChiTietDVKTs, string nameFile, string @pathTongHop)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", ""), new XElement("DSACH_CHI_TIET_DVKT", "")));

            for (int i = 0; i < hoSoChiTietDVKTs.Count; i++)
            {
                hoSoChiTietDVKTs[i].STT = i + 1;
                hoSoChiTietDVKTs[i].MaLienKet = hoSoChiTietDVKTs[i].MaLienKet;
                hoSoChiTietDVKTs[i].NgayYLenh = hoSoChiTietDVKTs[i].NgayYLenh;
                hoSoChiTietDVKTs[i].NgayKetQua = hoSoChiTietDVKTs[i].NgayKetQua;

                hoSoChiTietDVKTs[i].SoLuong = Math.Round(hoSoChiTietDVKTs[i].SoLuong, 3);
                hoSoChiTietDVKTs[i].DonGia = Math.Round(hoSoChiTietDVKTs[i].DonGia, 3);
                  data.Descendants("DSACH_CHI_TIET_DVKT").Last().Add( new XElement("CHI_TIET_DVKT",
                  new XElement("MA_LK", hoSoChiTietDVKTs[i].MaLienKet),
                  new XElement("STT", hoSoChiTietDVKTs[i].STT),
                  new XElement("MA_DICH_VU", hoSoChiTietDVKTs[i].MaDichVu == null ? "" : hoSoChiTietDVKTs[i].MaDichVu),
                  new XElement("MA_VAT_TU", hoSoChiTietDVKTs[i].MaVatTu == null ? "" : hoSoChiTietDVKTs[i].MaVatTu),
                  new XElement("MA_NHOM", ((int)hoSoChiTietDVKTs[i].MaNhom).ToString()),
                  new XElement("GOI_VTYT", hoSoChiTietDVKTs[i].GoiVatTuYTe == null ? "" : hoSoChiTietDVKTs[i].GoiVatTuYTe),
                  new XElement("TEN_VAT_TU", hoSoChiTietDVKTs[i].TenVatTu == null ? "" : hoSoChiTietDVKTs[i].TenVatTu),
                  new XElement("TEN_DICH_VU", hoSoChiTietDVKTs[i].TenDichVu == null ? "![CDATA[]]" : "![CDATA[" + hoSoChiTietDVKTs[i].TenDichVu + "]]"),
                  new XElement("DON_VI_TINH", hoSoChiTietDVKTs[i].DonViTinh == null ? "" : hoSoChiTietDVKTs[i].DonViTinh),
                  new XElement("PHAM_VI", hoSoChiTietDVKTs[i].PhamVi),
                  new XElement("SO_LUONG", String.Format("{0:0.000}", hoSoChiTietDVKTs[i].SoLuong)),
                  new XElement("DON_GIA", String.Format("{0:0.000}", hoSoChiTietDVKTs[i].DonGia)),
                  new XElement("TT_THAU", hoSoChiTietDVKTs[i].ThongTinThau == null ? "" : hoSoChiTietDVKTs[i].ThongTinThau),
                  new XElement("TYLE_TT", hoSoChiTietDVKTs[i].TyLeThanhToan.ToString()),
                  new XElement("THANH_TIEN", String.Format("{0:0.00}", hoSoChiTietDVKTs[i].ThanhTien)),
                  new XElement("T_TRANTT", hoSoChiTietDVKTs[i].TienThanhToanToiDa == null ? "" : String.Format("{0:0.00}", hoSoChiTietDVKTs[i].TienThanhToanToiDa)),
                  new XElement("MUC_HUONG", hoSoChiTietDVKTs[i].MucHuong.ToString()),
                  new XElement("T_NGUONKHAC", hoSoChiTietDVKTs[i].TienNguonKhac == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].TienNguonKhac), 2)))),
                  new XElement("T_BNTT", hoSoChiTietDVKTs[i].TienBenhNhanTuTra == null ? "0.00" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].TienBenhNhanTuTra), 2)))),
                  new XElement("T_BHTT", String.Format("{0:0.00}", hoSoChiTietDVKTs[i].TienBaoHiemThanhToan)),
                  new XElement("T_BNCCT", hoSoChiTietDVKTs[i].TienBenhNhanCungChiTra == null ? "0.00" : String.Format("{0:0.00}", String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].TienBenhNhanCungChiTra), 2))))),
                  new XElement("T_NGOAIDS", hoSoChiTietDVKTs[i].TienNgoaiDinhSuat == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].TienNgoaiDinhSuat), 2)))),
                  new XElement("MA_KHOA", hoSoChiTietDVKTs[i].MaKhoa),
                  new XElement("MA_GIUONG", hoSoChiTietDVKTs[i].MaGiuong == null ? "" : hoSoChiTietDVKTs[i].MaGiuong),
                  new XElement("MA_BAC_SI", hoSoChiTietDVKTs[i].MaBacSi.ToString()),
                  new XElement("MA_BENH", hoSoChiTietDVKTs[i].MaBenh),
                  new XElement("NGAY_YL", hoSoChiTietDVKTs[i].NgayYLenh.ToString()),
                  new XElement("NGAY_KQ", hoSoChiTietDVKTs[i].NgayKetQua?.ToString(FormatNgayGioBHYT) ?? ""),
                  new XElement("MA_PTTT", ((int)hoSoChiTietDVKTs[i].MaPhuongThucThanhToan).ToString())));

            }


            data.Save(@pathTongHop);
        }
        private void AddHoSoCanLamSangVoXMLTongHop(HoSoCanLamSang thongtin, string nameFile, string @pathTongHop)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", "")));

            data.Descendants("NOIDUNGFILE").Last().Add(new XElement("DSACH_CHI_TIET_CLS", new XElement("CHI_TIET_CLS", new XElement("MA_LK", thongtin.MaLienKet), new XElement("STT", thongtin.STT), new XElement("MA_DICH_VU", thongtin.MaDichVu),
                                                         new XElement("MA_CHI_SO", thongtin.MaChiSo == null ? "" : thongtin.MaChiSo), new XElement("TEN_CHI_SO", thongtin.TenChiSo == null ? "![CDATA[]]" : "![CDATA[" + thongtin.TenChiSo + "]]")
                                                         , new XElement("GIA_TRI", thongtin.GiaTri == null ? "![CDATA[]]" : "![CDATA[" + thongtin.GiaTri + "]]"), new XElement("MA_MAY", thongtin.MaMay == null ? "" : thongtin.MaMay),
                                                         new XElement("MO_TA", thongtin.MoTa == null ? "![CDATA[]]" : "![CDATA[" + thongtin.MoTa + "]]"), new XElement("KET_LUAN", thongtin.KetLuan == null ? "![CDATA[]]" : "![CDATA[" + thongtin.KetLuan + "]]"),
                                                         new XElement("NGAY_KQ", thongtin.NgayKQ))));
            data.Save(@pathTongHop);
        }
        private void AddHoSoChiTietDienBienBenhVoXMLTongHop(HoSoChiTietDienBienBenh thongtin, string nameFile, string @pathTongHop)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", "")));

            data.Descendants("NOIDUNGFILE").Last().Add(new XElement("DSACH_CHI_TIET_DIEN_BIEN_BENH", new XElement("CHI_TIET_DIEN_BIEN_BENH", new XElement("MA_LK", thongtin.MaLienKet), new XElement("STT", thongtin.STT), new XElement("DIEN_BIEN", "![CDATA[" + thongtin.DienBien + "]]"),
                                                         new XElement("HOI_CHAN", thongtin.HoiChuan == null ? "![CDATA[]]" : "![CDATA[" + thongtin.HoiChuan + "]]"), new XElement("PHAU_THUAT", thongtin.PhauThuat == null ? "![CDATA[]]" : "![CDATA[" + thongtin.PhauThuat + "]]")
                                                         , new XElement("NGAY_YL", thongtin.NgayYLenh))));
            data.Save(@pathTongHop);
        }

        #endregion
        private void ReMoveDataXML1(string path)
        {
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement luuThongTinbenhNhan = data.Descendants("TONG_HOP").LastOrDefault();
            luuThongTinbenhNhan.Element("MA_LK").Remove();
            luuThongTinbenhNhan.Element("STT").Remove();
            luuThongTinbenhNhan.Element("MA_BN").Remove();
            luuThongTinbenhNhan.Element("HO_TEN").Remove();
            luuThongTinbenhNhan.Element("NGAY_SINH").Remove();
            luuThongTinbenhNhan.Element("GIOI_TINH").Remove();
            luuThongTinbenhNhan.Element("DIA_CHI").Remove();
            luuThongTinbenhNhan.Element("MA_THE").Remove();
            luuThongTinbenhNhan.Element("MA_DKBD").Remove();
            luuThongTinbenhNhan.Element("GT_THE_TU").Remove();
            luuThongTinbenhNhan.Element("GT_THE_DEN").Remove();
            luuThongTinbenhNhan.Element("MIEN_CUNG_CT").Remove();
            luuThongTinbenhNhan.Element("TEN_BENH").Remove();
            luuThongTinbenhNhan.Element("MA_BENH").Remove();
            luuThongTinbenhNhan.Element("MA_BENHKHAC").Remove();
            luuThongTinbenhNhan.Element("MA_LYDO_VVIEN").Remove();
            luuThongTinbenhNhan.Element("MA_NOI_CHUYEN").Remove();
            luuThongTinbenhNhan.Element("MA_TAI_NAN").Remove();
            luuThongTinbenhNhan.Element("NGAY_VAO").Remove();
            luuThongTinbenhNhan.Element("NGAY_RA").Remove();
            luuThongTinbenhNhan.Element("SO_NGAY_DTRI").Remove();
            luuThongTinbenhNhan.Element("KET_QUA_DTRI").Remove();
            luuThongTinbenhNhan.Element("TINH_TRANG_RV").Remove();
            luuThongTinbenhNhan.Element("NGAY_TTOAN").Remove();
            luuThongTinbenhNhan.Element("T_THUOC").Remove();
            luuThongTinbenhNhan.Element("T_VTYT").Remove();
            luuThongTinbenhNhan.Element("T_TONGCHI").Remove();
            luuThongTinbenhNhan.Element("T_BNTT").Remove();
            luuThongTinbenhNhan.Element("T_BNCCT").Remove();
            luuThongTinbenhNhan.Element("T_BHTT").Remove();
            luuThongTinbenhNhan.Element("T_NGUONKHAC").Remove();
            luuThongTinbenhNhan.Element("T_NGOAIDS").Remove();
            luuThongTinbenhNhan.Element("NAM_QT").Remove();
            luuThongTinbenhNhan.Element("THANG_QT").Remove();
            luuThongTinbenhNhan.Element("MA_LOAI_KCB").Remove();
            luuThongTinbenhNhan.Element("MA_KHOA").Remove();
            luuThongTinbenhNhan.Element("MA_CSKCB").Remove();
            luuThongTinbenhNhan.Element("MA_KHUVUC").Remove();
            luuThongTinbenhNhan.Element("MA_PTTT_QT").Remove();
            luuThongTinbenhNhan.Element("CAN_NANG").Remove();
            data.Save(path);
        }
        private async Task DeleteFile(string path)
        {
            System.IO.File.Delete(@path);
        }
        private static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        private void AddThongTinBenhNhanVoXML(ThongTinBenhNhan thongtin, string pathXml1)
        {

            XDocument data = XDocument.Load(pathXml1);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement luuThongTinbenhNhan = data.Descendants("TONG_HOP").LastOrDefault();
            data.Descendants("TONG_HOP").Last().Add(new XElement("MA_LK", thongtin.MaLienKet), new XElement("STT", thongtin.STT), new XElement("MA_BN", thongtin.MaBenhNhan),
                                                           new XElement("HO_TEN", "![CDATA[" + thongtin.HoTen + "]]"), new XElement("NGAY_SINH", thongtin.NgaySinh),
                                                           new XElement("GIOI_TINH", ((int)thongtin.GioiTinh).ToString()), new XElement("DIA_CHI", "![CDATA[" + thongtin.DiaChi + "]]"),
                                                           new XElement("MA_THE", thongtin.MaThe), new XElement("MA_DKBD", thongtin.MaCoSoKCBBanDau),
                                                           new XElement("GT_THE_TU", thongtin.GiaTriTheTu), new XElement("GT_THE_DEN", thongtin.GiaTriTheDen?.ToString(FormatNgayBHYT) ?? ""),
                                                           new XElement("MIEN_CUNG_CT", thongtin.MienCungChiTra?.ToString(FormatNgayBHYT) ?? ""), new XElement("TEN_BENH", "![CDATA[" + thongtin.TenBenh + "]]"),
                                                           new XElement("MA_BENH", thongtin.MaBenh), new XElement("MA_BENHKHAC", thongtin.MaBenhKhac == null ? "" : thongtin.MaBenhKhac),
                                                           new XElement("MA_LYDO_VVIEN", ((int)thongtin.LyDoVaoVien).ToString()), new XElement("MA_NOI_CHUYEN", thongtin.MaNoiChuyen == null ? "" : thongtin.MaNoiChuyen),
                                                           new XElement("MA_TAI_NAN", thongtin.MaTaiNan == null ? "" : ((int)thongtin.MaTaiNan).ToString()), new XElement("NGAY_VAO", thongtin.NgayVao),
                                                           new XElement("NGAY_RA", thongtin.NgayRa), new XElement("SO_NGAY_DTRI", thongtin.SoNgayDieuTri.ToString()),
                                                           new XElement("KET_QUA_DTRI", ((int)thongtin.KetQuaDieuTri).ToString()), new XElement("TINH_TRANG_RV", ((int)thongtin.TinhTrangRaVien).ToString()),
                                                           new XElement("NGAY_TTOAN", thongtin.NgayThanhToan?.ToString(FormatNgayGioBHYT) ?? ""),
                                                           new XElement("T_THUOC", thongtin.TienThuoc.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienThuoc)), new XElement("T_VTYT", thongtin.TienVatTuYTe.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienVatTuYTe)),
                                                           new XElement("T_TONGCHI", String.Format("{0:0.00}", thongtin.TienTongChi)), new XElement("T_BNTT", thongtin.TienBenhNhanTuTra.ToString() == null ? String.Format("{0:0.00}", 0) : String.Format("{0:0.00}", thongtin.TienBenhNhanTuTra)), new XElement("T_BNCCT", thongtin.TienBenhNhanCungChiTra.ToString() == null ? String.Format("{0:0.00}", 0) : String.Format("{0:0.00}", thongtin.TienBenhNhanCungChiTra)),
                                                           new XElement("T_BHTT", String.Format("{0:0.00}", thongtin.TienBaoHiemThanhToan)), new XElement("T_NGUONKHAC", thongtin.TienNguonKhac.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienNguonKhac)),
                                                           new XElement("T_NGOAIDS", thongtin.TienNgoaiDinhSuat.ToString() == null ? "" : String.Format("{0:0.00}", thongtin.TienNgoaiDinhSuat)), new XElement("NAM_QT", thongtin.NamQuyetToan.ToString()),
                                                           new XElement("THANG_QT", thongtin.ThangQuyetToan.ToString()), new XElement("MA_LOAI_KCB", ((int)thongtin.MaLoaiKCB).ToString()),
                                                           new XElement("MA_KHOA", thongtin.MaKhoa), new XElement("MA_CSKCB", thongtin.MaCSKCB),
                                                           new XElement("MA_KHUVUC", thongtin.MaKhuVuc == null ? "" : thongtin.MaKhuVuc), new XElement("MA_PTTT_QT", thongtin.MaPhauThuatQuocTe == null ? "" : thongtin.MaPhauThuatQuocTe),
                                                           new XElement("CAN_NANG", thongtin.CanNang.ToString() == null ? "" : thongtin.CanNang.ToString()));
            data.Save(pathXml1);
        }
        private void AddFileDown(string name, string duLieu, string path)
        {

            XDocument data = XDocument.Load(@path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement luuThongTinbenhNhan = data.Descendants("DOWNLOAD").LastOrDefault();
            data.Descendants("DOWNLOAD").Last().Add(new XElement(name, duLieu));
            data.Save(@path);
        }
        private void AddHoSoChiTietThuocVoXML(HoSoChiTietThuoc thongtin, string pathXml1)
        {
            XDocument data = XDocument.Load(pathXml1);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("DSACH_CHI_TIET_THUOC").Last().Add(new XElement("CHI_TIET_THUOC",
                new XElement("MA_LK", thongtin.MaLienKet),
                new XElement("STT", thongtin.STT),
                new XElement("MA_THUOC", thongtin.MaThuoc),
                new XElement("MA_NHOM", ((int)thongtin.MaNhom).ToString()),
                new XElement("TEN_THUOC", "![CDATA[" + thongtin.TenThuoc + "]]"),
                new XElement("DON_VI_TINH", thongtin.DonViTinh),
                new XElement("HAM_LUONG", thongtin.HamLuong == null ? "![CDATA[]]" : "![CDATA[" + thongtin.HamLuong + "]]"),
                new XElement("DUONG_DUNG", thongtin.DuongDung == null ? "" : thongtin.DuongDung),
                new XElement("LIEU_DUNG", thongtin.LieuDung == null ? "![CDATA[]]" : "![CDATA[" + thongtin.LieuDung + "]]"),
                new XElement("SO_DANG_KY", thongtin.SoDangKy == null ? "" : thongtin.SoDangKy),
                new XElement("TT_THAU", thongtin.ThongTinThau == null ? "" : thongtin.ThongTinThau),
                new XElement("PHAM_VI", thongtin.PhamVi),
                new XElement("SO_LUONG", String.Format("{0:0.000}", thongtin.SoLuong)),
                new XElement("DON_GIA", String.Format("{0:0.000}", thongtin.DonGia)),
                new XElement("TYLE_TT", thongtin.TyLeThanhToan.ToString()),
                new XElement("THANH_TIEN", String.Format("{0:0.00}", thongtin.ThanhTien)),
                new XElement("MUC_HUONG", thongtin.MucHuong.ToString()),
                new XElement("T_NGUONKHAC", thongtin.TienNguonKhac == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienNguonKhac), 2)))),
                new XElement("T_BNTT", thongtin.TienBenhNhanTuTra == null ? "0.00" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienBenhNhanTuTra), 2)))),
                new XElement("T_BHTT", String.Format("{0:0.00}", thongtin.TienBaoHiemThanhToan)),
                new XElement("T_BNCCT", thongtin.TienBenhNhanCungChiTra == null ? "0.00" : String.Format("{0:0.00}", String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienBenhNhanCungChiTra), 2))))),
                new XElement("T_NGOAIDS", thongtin.TienNgoaiDinhSuat == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienNgoaiDinhSuat), 2)))),
                new XElement("MA_KHOA", thongtin.MaKhoa),
                new XElement("MA_BAC_SI", thongtin.MaBacSi.ToString()),
                new XElement("MA_BENH", thongtin.MaBenh),
                new XElement("NGAY_YL", thongtin.NgayYLenh.ToString()),
                new XElement("MA_PTTT", ((int)thongtin.MaPhuongThucThanhToan).ToString())
                   ));
            data.Save(pathXml1);
        }
        private void AddHoSoChiTietDVKTVoXML(HoSoChiTietDVKT thongtin, string pathXml1)
        {
            XDocument data = XDocument.Load(pathXml1);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("DSACH_CHI_TIET_DVKT").Last().Add(new XElement("CHI_TIET_DVKT",
                new XElement("MA_LK", thongtin.MaLienKet),
                new XElement("STT", thongtin.STT),
                new XElement("MA_DICH_VU", thongtin.MaDichVu == null ? "" : thongtin.MaDichVu),
                new XElement("MA_VAT_TU", thongtin.MaVatTu == null ? "" : thongtin.MaVatTu),
                new XElement("MA_NHOM", ((int)thongtin.MaNhom).ToString()),
                new XElement("GOI_VTYT", thongtin.GoiVatTuYTe == null ? "" : thongtin.GoiVatTuYTe),
                new XElement("TEN_VAT_TU", thongtin.TenVatTu == null ? "" : thongtin.TenVatTu),
                new XElement("TEN_DICH_VU", thongtin.TenDichVu == null ? "![CDATA[]]" : "![CDATA[" + thongtin.TenDichVu + "]]"),
                new XElement("DON_VI_TINH", thongtin.DonViTinh == null ? "" : thongtin.DonViTinh),
                new XElement("PHAM_VI", thongtin.PhamVi),
                new XElement("SO_LUONG", String.Format("{0:0.000}", thongtin.SoLuong)),
                new XElement("DON_GIA", String.Format("{0:0.000}", thongtin.DonGia)),
                new XElement("TT_THAU", thongtin.ThongTinThau == null ? "" : thongtin.ThongTinThau),
                new XElement("TYLE_TT", thongtin.TyLeThanhToan.ToString()),
                new XElement("THANH_TIEN", String.Format("{0:0.00}", thongtin.ThanhTien)),
                new XElement("T_TRANTT", thongtin.TienThanhToanToiDa == null ? "" : String.Format("{0:0.00}", thongtin.TienThanhToanToiDa)),
                new XElement("MUC_HUONG", thongtin.MucHuong.ToString()),
                new XElement("T_NGUONKHAC", thongtin.TienNguonKhac == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienNguonKhac), 2)))),
                new XElement("T_BNTT", thongtin.TienBenhNhanTuTra == null ? "0.00" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienBenhNhanTuTra), 2)))),
                new XElement("T_BHTT", String.Format("{0:0.00}", thongtin.TienBaoHiemThanhToan)),
                new XElement("T_BNCCT", thongtin.TienBenhNhanCungChiTra == null ? "0.00" : String.Format("{0:0.00}", String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienBenhNhanCungChiTra), 2))))),
                new XElement("T_NGOAIDS", thongtin.TienNgoaiDinhSuat == null ? "" : String.Format("{0:0.00}", (double)(Math.Round(Convert.ToDecimal(thongtin.TienNgoaiDinhSuat), 2)))),
                new XElement("MA_KHOA", thongtin.MaKhoa),
                new XElement("MA_GIUONG", thongtin.MaGiuong == null ? "" : thongtin.MaGiuong),
                new XElement("MA_BAC_SI", thongtin.MaBacSi.ToString()),
                new XElement("MA_BENH", thongtin.MaBenh),
                new XElement("NGAY_YL", thongtin.NgayYLenh.ToString()),
                new XElement("NGAY_KQ", thongtin.NgayKetQua?.ToString(FormatNgayGioBHYT) ?? ""),
                new XElement("MA_PTTT", ((int)thongtin.MaPhuongThucThanhToan).ToString())));
            data.Save(pathXml1);
        }
        private void AddHoSoCanLamSangVoXML(HoSoCanLamSang thongtin, string pathXml1)
        {
            XDocument data = XDocument.Load(pathXml1);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("DSACH_CHI_TIET_CLS").Last().Add(new XElement("CHI_TIET_CLS", new XElement("MA_LK", thongtin.MaLienKet), new XElement("STT", thongtin.STT), new XElement("MA_DICH_VU", thongtin.MaDichVu),
                                                         new XElement("MA_CHI_SO", thongtin.MaChiSo == null ? "" : thongtin.MaChiSo), new XElement("TEN_CHI_SO", thongtin.TenChiSo == null ? "![CDATA[]]" : "![CDATA[" + thongtin.TenChiSo + "]]")
                                                         , new XElement("GIA_TRI", thongtin.GiaTri == null ? "![CDATA[]]" : "![CDATA[" + thongtin.GiaTri + "]]"), new XElement("MA_MAY", thongtin.MaMay == null ? "" : thongtin.MaMay),
                                                         new XElement("MO_TA", thongtin.MoTa == null ? "![CDATA[]]" : "![CDATA[" + thongtin.MoTa + "]]"), new XElement("KET_LUAN", thongtin.KetLuan == null ? "![CDATA[]]" : "![CDATA[" + thongtin.KetLuan + "]]"),
                                                         new XElement("NGAY_KQ", thongtin.NgayKQ)));
            data.Save(pathXml1);
        }
        private void AddHoSoChiTietDienBienBenhVoXML(HoSoChiTietDienBienBenh thongtin, string pathXml1)
        {
            XDocument data = XDocument.Load(pathXml1);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("DSACH_CHI_TIET_DIEN_BIEN_BENH").Last().Add(new XElement("CHI_TIET_DIEN_BIEN_BENH", new XElement("MA_LK", thongtin.MaLienKet), new XElement("STT", thongtin.STT), new XElement("DIEN_BIEN", "![CDATA[" + thongtin.DienBien + "]]"),
                                                         new XElement("HOI_CHAN", thongtin.HoiChuan == null ? "![CDATA[]]" : "![CDATA[" + thongtin.HoiChuan + "]]"), new XElement("PHAU_THUAT", thongtin.PhauThuat == null ? "![CDATA[]]" : "![CDATA[" + thongtin.PhauThuat + "]]")
                                                         , new XElement("NGAY_YL", thongtin.NgayYLenh)));
            data.Save(pathXml1);
        }
        private void AddFileHoSoToHoSoTongHopXML(string duLieu, string nameFile, string path)
        {
            //var pathChung = @"Resource\\TongHop.xml";
            XDocument data = XDocument.Load(@path);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", duLieu)));
            data.Save(@path);
        }
        private void AddHoSoToDanhSachHoSoTongHopXML()
        {
            var pathChung = @"Resource\\TongHop.xml";
            XDocument data = XDocument.Load(pathChung);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("DANHSACHHOSO").Last().Add(new XElement("HOSO", ""));
            data.Save(pathChung);

        }
        private void ReplaceCharacter(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            text = text.Replace("!", "<!");
            text = text.Replace("]]", "]]>");
            System.IO.File.WriteAllText(path, text);
        }
        private void ReplaceCharacterAdd(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            text = text.Replace("<!", "!");
            text = text.Replace("]]>", "]]");
            System.IO.File.WriteAllText(path, text);
        }
        private void DeleteValue(string path, string name)
        {
            XDocument doc = XDocument.Load(path);
            var q = from node in doc.Descendants(name)
                    select node;
            q.ToList().ForEach(x => x.Remove());
            doc.Save(path);

        }

        #region Valid cho các field
        public async Task<bool> CheckValidGiaTriTheDen(string giaTriTheDen, string giaTriTheTu)
        {
            if (!string.IsNullOrEmpty(giaTriTheTu) && !string.IsNullOrEmpty(giaTriTheDen))
            {
                int countgiatriTu = giaTriTheTu.Split(';').Length - 1;
                int countgiatriDen = giaTriTheDen.Split(';').Length - 1;
                char[] spearator = { ';', ' ' };
                if (countgiatriTu < countgiatriDen)
                {
                    return true;
                }
                else if (countgiatriTu > 0 && countgiatriDen > 0)
                {
                    String[] strlistTu = giaTriTheTu.Split(spearator,
                           countgiatriTu + 1, StringSplitOptions.None);
                    String[] strlistDen = giaTriTheDen.Split(spearator,
                           countgiatriDen + 1, StringSplitOptions.None);
                    for (int i = 0; i < strlistTu.Count(); i++)
                    {
                        if (i <= strlistDen.Count() - 1)
                        {
                            var timeTu = new DateTime(Convert.ToInt32(strlistTu[i].Substring(0, 4)), Convert.ToInt32(strlistTu[i].Substring(4, 2)), Convert.ToInt32(strlistTu[i].Substring(6, 2)));
                            var timeDen = new DateTime(Convert.ToInt32(strlistDen[i].Substring(0, 4)), Convert.ToInt32(strlistDen[i].Substring(4, 2)), Convert.ToInt32(strlistDen[i].Substring(6, 2)));
                            if (timeDen < timeTu)
                            {
                                return true;
                            }
                        }
                    }
                }
                else if (countgiatriTu >= 0 && countgiatriDen == 0)
                {
                    var timeTu = DateTime.Now;
                    var timeDen = new DateTime(Convert.ToInt32(giaTriTheDen.Substring(0, 4)), Convert.ToInt32(giaTriTheDen.Substring(4, 2)), Convert.ToInt32(giaTriTheDen.Substring(6, 2)));
                    if (countgiatriTu > 0)
                    {
                        String[] strlistTu = giaTriTheTu.Split(spearator,
                          countgiatriTu + 1, StringSplitOptions.None);
                        timeTu = new DateTime(Convert.ToInt32(strlistTu[0].Substring(0, 4)), Convert.ToInt32(strlistTu[0].Substring(4, 2)), Convert.ToInt32(strlistTu[0].Substring(6, 2)));
                    }
                    else
                    {
                        timeTu = new DateTime(Convert.ToInt32(giaTriTheTu.Substring(0, 4)), Convert.ToInt32(giaTriTheTu.Substring(4, 2)), Convert.ToInt32(giaTriTheTu.Substring(6, 2)));
                    }

                    if (timeDen < timeTu)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> CheckSpace(string giaTri)
        {
            int count = 0;
            if (giaTri != null)
            {
                count = giaTri.Split(' ').Length - 1;
            }
            return count > 0 ? true : false;
        }
        public async Task<bool> CheckMaDichVuHoSoDVKT(string giaTri, Enums.EnumDanhMucNhomTheoChiPhi? maNhom)
        {
            if (!string.IsNullOrEmpty(giaTri) && maNhom != null)
            {
                int count = giaTri.Split("VC.").Length - 1;
                if (maNhom == Enums.EnumDanhMucNhomTheoChiPhi.VanChuyen && count == 0)
                {
                    return true;
                }

            }
            return false;
        }
        public async Task<bool> CheckMaDichVuHoSoDVKTRequired(string giaTri, Enums.EnumDanhMucNhomTheoChiPhi? maNhom)
        {
            if (maNhom != null)
            {
                if (string.IsNullOrEmpty(giaTri) && (maNhom == Enums.EnumDanhMucNhomTheoChiPhi.XetNghiem || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh
                                                                 || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.ThamDoChucNang || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat
                                                                 || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.VanChuyen
                                                                  || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.KhamBenh || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNgoaiTru || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru))

                {
                    return true;

                }
            }

            return false;
        }
        public async Task<bool> CheckMaVatTuHoSoDVKTRequired(string giaTri, Enums.EnumDanhMucNhomTheoChiPhi? maNhom)
        {
            if (maNhom != null)
            {
                if (string.IsNullOrEmpty(giaTri) && (maNhom == Enums.EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.VTYTThanhToanTheoTyLe))

                {
                    return true;

                }
            }

            return false;
        }
        public async Task<bool> CheckThongTinThauValid(string giaTri, string maVattu, Enums.EnumDanhMucNhomTheoChiPhi? maNhom)
        {
            if (maNhom != null)
            {
                if (!string.IsNullOrEmpty(maVattu) && (maNhom == Enums.EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT || maNhom == Enums.EnumDanhMucNhomTheoChiPhi.VTYTThanhToanTheoTyLe))

                {
                    if (!string.IsNullOrEmpty(giaTri))
                    {
                        char[] spearator = { '.', ' ' };
                        int countgiatriTu = giaTri.Split('.').Length - 1;
                        if (countgiatriTu > 1 && countgiatriTu < 3)
                        {
                            String[] strlistTu = giaTri.Split(spearator,
                          countgiatriTu + 1, StringSplitOptions.None);
                            var isNumeric = int.TryParse(strlistTu.FirstOrDefault(), out int n);
                            if (isNumeric == false)
                            {
                                return true;
                            }

                        }
                        else
                        {
                            return true;
                        }
                    }

                }
            }

            return false;
        }
        public async Task<bool> CheckValidGoiVatTuXML3(string giaTri)
        {
            if (giaTri != null)
            {
                int count = giaTri.Split(';').Length - 1;
                if (count > 0)
                {
                    char[] spearator = { ';', ' ' };
                    String[] strlist = giaTri.Split(spearator,
                           count + 1, StringSplitOptions.None);
                    foreach (String item in strlist)
                    {
                        if (item.Length == 2)
                        {
                            if (!(item.Substring(0, 1)).Equals("G"))
                            {
                                return true;
                            }
                        }
                        else { return true; }
                    }
                }
                else
                {
                    var a = giaTri.Substring(0, 1);
                    if (giaTri.Length == 2)
                    {
                        if (!(giaTri.Substring(0, 1)).Equals("G"))
                        {
                            return true;
                        }
                    }
                    else { return true; }

                }
            }
            return false;
        }
        public async Task<bool> CheckValueTyLeThanhToan(int? giatri, int? phamvi)
        {

            if (giatri != null || phamvi != null)
            {
                if (phamvi == 2 && giatri != 0)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> CheckValueTienNguonKhac(double? giatri, double? thanhtien)
        {
            if (giatri != null || thanhtien != null)
            {
                if ((double)(Math.Round(Convert.ToDecimal(giatri), 2)) > (double)(Math.Round(Convert.ToDecimal(thanhtien), 2)))
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> CheckValidGiaTriTheTu(string giaTri)
        {
            if (!string.IsNullOrEmpty(giaTri))
            {
                int count = giaTri.Split(';').Length - 1;
                if (count > 0)
                {
                    char[] spearator = { ';', ' ' };

                    // Using the Method 
                    String[] strlist = giaTri.Split(spearator,
                           count + 1, StringSplitOptions.None);

                    foreach (String s in strlist)
                    {
                        if (s.Length == 8)
                        {
                            if (0 < Convert.ToInt32(s.Substring(4, 2)) && Convert.ToInt32(s.Substring(4, 2)) < 13 && 0 < Convert.ToInt32(s.Substring(6, 2)) && Convert.ToInt32(s.Substring(6, 2)) < 32)
                            {
                                if (Convert.ToInt32(s.Substring(4, 2)) == 2 && (Convert.ToInt32(s.Substring(6, 2)) == 30 || Convert.ToInt32(s.Substring(6, 2)) == 31))
                                {
                                    return true;
                                }
                                else
                                {
                                    var time = new DateTime(Convert.ToInt32(s.Substring(0, 4)), Convert.ToInt32(s.Substring(4, 2)), Convert.ToInt32(s.Substring(6, 2)));
                                    if (time > DateTime.Now.Date)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                return true;
                            }

                        }
                        else
                        {
                            return true;
                        }

                    }
                }
                else
                {
                    if (giaTri.Length == 8)
                    {
                        if (0 < Convert.ToInt32(giaTri.Substring(4, 2)) && Convert.ToInt32(giaTri.Substring(4, 2)) < 13 && 0 < Convert.ToInt32(giaTri.Substring(6, 2)) && Convert.ToInt32(giaTri.Substring(6, 2)) < 32)
                        {
                            if (Convert.ToInt32(giaTri.Substring(4, 2)) == 2 && (Convert.ToInt32(giaTri.Substring(6, 2)) == 30 || Convert.ToInt32(giaTri.Substring(6, 2)) == 31))
                            {
                                return true;
                            }
                            else
                            {
                                var time = new DateTime(Convert.ToInt32(giaTri.Substring(0, 4)), Convert.ToInt32(giaTri.Substring(4, 2)), Convert.ToInt32(giaTri.Substring(6, 2)));
                                if (time > DateTime.Now.Date)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return true;
                        }

                    }
                    else
                    {
                        return true;
                    }
                }


            }


            return false;
        }
        public async Task<bool> CheckValidSoNgayDieuTri(string ngayVao, string ngayRa, int? soNgayDieuTri)
        {
            if (!string.IsNullOrEmpty(ngayVao) && !string.IsNullOrEmpty(ngayRa) && soNgayDieuTri != 0 && soNgayDieuTri != null)
            {
                if (ngayVao.Length < 13 && ngayRa.Length < 13)
                {
                    if (soNgayDieuTri <= (Convert.ToInt32(ngayRa.Substring(6, 2)) - Convert.ToInt32(ngayVao.Substring(6, 2))))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }


            return false;
        }
        public async Task<bool> CheckValidNgayVaoRa(DateTime? ngay)
        {
            if (ngay != null)
            {
                if (ngay?.Day > DateTime.Now.Day)
                {
                    return true;
                }
            }



            return false;
        }
        public async Task<bool> CheckValidMaDKBD(string maDKBD)
        {
            if (!string.IsNullOrEmpty(maDKBD))
            {
                int count = maDKBD.Split(';').Length - 1;
                if (count > 0)
                {
                    char[] spearator = { ';', ' ' };

                    // Using the Method 
                    String[] strlist = maDKBD.Split(spearator,
                           count + 1, StringSplitOptions.None);
                    foreach (String s in strlist)
                    {
                        if (s.Length != 5)
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }
        public async Task<bool> CheckValidMaBenhKhac(string maBenhKhac)
        {
            if (!string.IsNullOrEmpty(maBenhKhac))
            {
                int count = maBenhKhac.Split(';').Length - 1;
                if (count > 0)
                {
                    char[] spearator = { ';', ' ' };

                    // Using the Method 
                    String[] strlist = maBenhKhac.Split(spearator,
                           count + 1, StringSplitOptions.None);
                    foreach (String s in strlist)
                    {
                        if (s.Length < 3 || s.Length > 8)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (maBenhKhac.Length < 3 || maBenhKhac.Length > 8)
                    {
                        return true;
                    }
                }
            }


            return false;
        }
        #endregion

        #region Lấy thông tin người bệnh từ bảo hiểm
        public ThongTinBHYTVO GetThongTin(ThongTinBenhNhanXemVO thongTinBenhNhan)
        {
            var month = thongTinBenhNhan.NgaySinh?.Month < 10 ? "0" + thongTinBenhNhan.NgaySinh?.Month : thongTinBenhNhan.NgaySinh?.Month + "";
            var day = thongTinBenhNhan.NgaySinh?.Day < 10 ? "0" + thongTinBenhNhan.NgaySinh?.Day : thongTinBenhNhan.NgaySinh?.Day + "";
            var ngaySinh = day + "/" + month + "/" + thongTinBenhNhan.NgaySinh?.Year;
            var token = TokenBHYT.GetTokenAPI().token;
            var id_token = TokenBHYT.GetTokenAPI().id_token;
            var client = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/egw/KQNhanLichSuKCB2019?token=" + token + "&id_token=" + id_token + "&username=" + Constants.UserNameBHYT + "&password=" + Constants.PassBHYT);

            var request = new RestRequest(Method.POST);

            var ngaySinhSendRequest = ngaySinh;
            if(thongTinBenhNhan.NgaySinh == null)
            {
                ngaySinhSendRequest = thongTinBenhNhan.NamSinh + "";
            }    

            request.AddJsonBody(
                new { maThe = thongTinBenhNhan.MaThe, hoTen = thongTinBenhNhan.TenBenhNhan, ngaySinh = ngaySinhSendRequest }); // uses JsonSerializer
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<ThongTinBHYTVO>(response.Content);
                if (content.dsLichSuKCB2018 != null && content.dsLichSuKCB2018.Any())
                {
                    foreach (var item in content.dsLichSuKCB2018)
                    {
                        var noiDangKyKCB =
                            _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(item.maCSKCB));
                        if (!string.IsNullOrEmpty(item.ngayVao))
                        {
                            var ngayVaoLst = item.ngayVao;
                            var ngayVaoFormat = new DateTime(int.Parse(ngayVaoLst.Substring(0, 4)), int.Parse(ngayVaoLst.Substring(4, 2))
                                , int.Parse(ngayVaoLst.Substring(6, 2)), int.Parse(ngayVaoLst.Substring(8, 2)), int.Parse(ngayVaoLst.Substring(10, 2)), 0);

                            item.ngayVaoDateTime = ngayVaoFormat;
                            item.ngayVaoDisplay = ngayVaoFormat.ApplyFormatDateTime();
                        }

                        if (!string.IsNullOrEmpty(item.ngayRa))
                        {
                            var ngayRaLst = item.ngayRa;
                            var ngayRaFormat = new DateTime(int.Parse(ngayRaLst.Substring(0, 4)), int.Parse(ngayRaLst.Substring(4, 2))
                                , int.Parse(ngayRaLst.Substring(6, 2)), int.Parse(ngayRaLst.Substring(8, 2)), int.Parse(ngayRaLst.Substring(10, 2)), 0);

                            item.ngayRaDateTime = ngayRaFormat;
                            item.ngayRaDisplay = ngayRaFormat.ApplyFormatDateTime();

                        }

                        item.coSoKCB = noiDangKyKCB?.Ten;

                        switch (item.tinhTrang)
                        {
                            case "1":
                                item.tinhTrangDisplay = "Ra viện";
                                break;
                            case "2":
                                item.tinhTrangDisplay = "Chuyển viện";
                                break;
                            case "3":
                                item.tinhTrangDisplay = "Trốn viện";
                                break;
                            case "4":
                                item.tinhTrangDisplay = "Xin ra viện";
                                break;
                        }

                        switch (item.kqDieuTri)
                        {
                            case "1":
                                item.kqDieuTriDisplay = "Khỏi";
                                break;
                            case "2":
                                item.kqDieuTriDisplay = "Đỡ";
                                break;
                            case "3":
                                item.kqDieuTriDisplay = "Không thay đổi";
                                break;
                            case "4":
                                item.kqDieuTriDisplay = "Nặng hơn";
                                break;
                            case "5":
                                item.kqDieuTriDisplay = "Tử vong";
                                break;
                        }

                        switch (item.lyDoVV)
                        {
                            case "1":
                                item.lyDoVVDisplay = "Đúng tuyến";
                                break;
                            case "2":
                                item.lyDoVVDisplay = "Cấp cứu";
                                break;
                            case "3":
                                item.lyDoVVDisplay = "Trái tuyến";
                                break;
                        }
                    }
                }

                if (content.dsLichSuKT2018 != null && content.dsLichSuKT2018.Any())
                {
                    foreach (var item in content.dsLichSuKT2018)
                    {
                        if (!string.IsNullOrEmpty(item.userKT))
                        {
                            var maBenhVien = item.userKT.Split("_")[0];
                            var noiDangKyKCB =
                                _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(maBenhVien));
                            item.tenCSKCB = noiDangKyKCB?.Ten;
                        }
                        if (!string.IsNullOrEmpty(item.thoiGianKT))
                        {
                            var thoiGianKTLst = item.thoiGianKT;
                            var ngayRaFormat = new DateTime(int.Parse(thoiGianKTLst.Substring(0, 4)), int.Parse(thoiGianKTLst.Substring(4, 2))
                                , int.Parse(thoiGianKTLst.Substring(6, 2)), int.Parse(thoiGianKTLst.Substring(8, 2)), int.Parse(thoiGianKTLst.Substring(10, 2)), 0);

                            item.thoiGianKTDateTime = ngayRaFormat;
                            item.thoiGianKTDisplay = ngayRaFormat.ApplyFormatDateTime();
                        }
                    }

                    content.dsLichSuKT2018 = content.dsLichSuKT2018.OrderByDescending(p => p.thoiGianKTDateTime).ToList();
                }

                return content;

            }
            else
            {
                _logger.LogError($"Something went wrong GetThongTin: {IRestResponseStringValue(response)}");

            }
            return null;
        }
        public ThongTinBHYTVO GetTokenAndAutoResendThongTin(ThongTinBenhNhanXemVO thongTinBenhNhan)
        {
            var takeAPI = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/token/take");
            var requesttoken = new RestRequest(Method.POST);
            requesttoken.AddJsonBody(
                                new { username = Constants.UserNameBHYT, password = Constants.PassBHYT }); // uses JsonSerializerTokenBHYTModel
            IRestResponse responsetoken = takeAPI.Execute(requesttoken);
            if (responsetoken.IsSuccessful)
            {
                var contentToken = JsonConvert.DeserializeObject<ThongTinTokenMoiVO>(responsetoken.Content);
                TokenBHYT.ModifyTokenBHYT(contentToken.APIKey.access_token, contentToken.APIKey.id_token);
                var result = GetThongTin(thongTinBenhNhan);


                return result;
            }
            else
            {

                _logger.LogError($"GetTokenAndAutoResendThongTin: {IRestResponseStringValue(responsetoken)}");
            }
            return null;
        }
        private string IRestResponseStringValue(IRestResponse thongTinBenhNhan)
        {
            string result = null;
            if (!string.IsNullOrEmpty(thongTinBenhNhan.Content))
            {
                result = " Content: " + thongTinBenhNhan.Content;
            }
            if (thongTinBenhNhan.ErrorException != null)
            {
                result = result + " ErrorException: " + thongTinBenhNhan.ErrorException;
            }
            if (thongTinBenhNhan.ErrorMessage != null)
            {
                result = result + " ErrorMessage: " + thongTinBenhNhan.ErrorMessage;
            }
            if (thongTinBenhNhan.StatusCode != null)
            {
                result = result + " StatusCode: " + thongTinBenhNhan.StatusCode;
            }
            return result;
        }
        #endregion
    }
}
