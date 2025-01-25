using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Camino.Services.Helpers
{
    public static class ResourceHelper
    {
        public static string CreateMaYeuCauTiepNhan()
        {
            var now = DateTime.Now;

            var path = @"Resource\\YeuCauTiepNhan.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement yeuCauTiepNhanXML = data.Descendants(root + "YeuCauTiepNhan").FirstOrDefault();
            var maYeuCauTiepNhan = (string)yeuCauTiepNhanXML.Element(root + "MaYeuCauTiepNhan");
            var preMaYeuCauTiepNhan = (string)yeuCauTiepNhanXML.Element(root + "PreMaYeuCauTiepNhan");
            //Lấy ra ngày trong năm và format luôn luôn 3 chữ số
            //var dayOfYearFormat = now.DayOfYear.ToString();
            //switch (now.DayOfYear.ToString().Length)
            //{
            //    case 1:
            //        dayOfYearFormat = "00" + now.DayOfYear;
            //        break;
            //    case 2:
            //        dayOfYearFormat = "0" + now.DayOfYear;
            //        break;
            //}
            //Prefix mới của Mã YCTN
            //var newPreMaYeuCauTiepNhan = now.Year + dayOfYearFormat;

            var newPreMaYeuCauTiepNhan = DateTime.Now.ToString("yyMMdd"); // cập nhật theo feedback #70

            //Tăng suffiex cũa Mã YCTN
            var newMaYeuCauTiepNhan = !string.IsNullOrEmpty(maYeuCauTiepNhan) ? Convert.ToInt32(maYeuCauTiepNhan) + 1 : 1;
            if (newPreMaYeuCauTiepNhan != preMaYeuCauTiepNhan)
            {
                newMaYeuCauTiepNhan = 1;
            }
            //Format suffiex cũa Mã YCTN luôn luôn 4 chữ số
            var maYeuCauTiepNhanFormat = newMaYeuCauTiepNhan.ToString();
            switch (newMaYeuCauTiepNhan.ToString().Length)
            {
                case 1:
                    maYeuCauTiepNhanFormat = "000" + newMaYeuCauTiepNhan;
                    break;
                case 2:
                    maYeuCauTiepNhanFormat = "00" + newMaYeuCauTiepNhan;
                    break;
                case 3:
                    maYeuCauTiepNhanFormat = "0" + newMaYeuCauTiepNhan;
                    break;
            }
            //Cập nhập vào file
            yeuCauTiepNhanXML.Element("MaYeuCauTiepNhan").Value = newMaYeuCauTiepNhan.ToString();
            yeuCauTiepNhanXML.Element("PreMaYeuCauTiepNhan").Value = newPreMaYeuCauTiepNhan;
            data.Save(path);
            return newPreMaYeuCauTiepNhan + maYeuCauTiepNhanFormat;
        }

        public static string CreateSoQuyetDinh(bool isVatTu = false)
        {
            var path = @"Resource\\NhapKho.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement nhapKhoXml = isVatTu ? data.Descendants(root + "NhapKhoVatTu").FirstOrDefault() : data.Descendants(root + "NhapKhoDuocPham").FirstOrDefault();
            var maSoQuyetDinh = (string)nhapKhoXml.Element(root + "SoQuyetDinh");
            var preSoQuyetDinh = (string)nhapKhoXml.Element(root + "PreSoQuyetDinh");

            var newPreSoQuyetDinh = DateTime.Now.ToString("yyMMdd");

            var newMaQuyetDinh = !string.IsNullOrEmpty(maSoQuyetDinh) ? Convert.ToInt32(maSoQuyetDinh) + 1 : 1;
            if (newPreSoQuyetDinh != preSoQuyetDinh)
            {
                newMaQuyetDinh = 1;
            }
            var soQuyetDinhFormat = newMaQuyetDinh.ToString();
            switch (newMaQuyetDinh.ToString().Length)
            {
                case 1:
                    soQuyetDinhFormat = "00" + newMaQuyetDinh;
                    break;
                case 2:
                    soQuyetDinhFormat = "0" + newMaQuyetDinh;
                    break;
            }
            //Cập nhập vào file
            nhapKhoXml.Element("SoQuyetDinh").Value = newMaQuyetDinh.ToString();
            nhapKhoXml.Element("PreSoQuyetDinh").Value = newPreSoQuyetDinh;
            data.Save(path);
            return "SQDHT" + newPreSoQuyetDinh + soQuyetDinhFormat;
        }

        public static string CreateSoHopDong(bool isVatTu = false)
        {
            var path = @"Resource\\NhapKho.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement nhapKhoXml = isVatTu ? data.Descendants(root + "NhapKhoVatTu").FirstOrDefault() : data.Descendants(root + "NhapKhoDuocPham").FirstOrDefault();
            var maSoHopDong = (string)nhapKhoXml.Element(root + "SoHopDong");
            var preSoHopDong = (string)nhapKhoXml.Element(root + "PreSoHopDong");

            var newPreSoHopDong = DateTime.Now.ToString("yyMMdd");

            var newMaHopDong = !string.IsNullOrEmpty(maSoHopDong) ? Convert.ToInt32(maSoHopDong) + 1 : 1;
            if (newPreSoHopDong != preSoHopDong)
            {
                newMaHopDong = 1;
            }
            var soHopDongFormat = newMaHopDong.ToString();
            switch (newMaHopDong.ToString().Length)
            {
                case 1:
                    soHopDongFormat = "00" + newMaHopDong;
                    break;
                case 2:
                    soHopDongFormat = "0" + newMaHopDong;
                    break;
            }
            //Cập nhập vào file
            nhapKhoXml.Element("SoHopDong").Value = newMaHopDong.ToString();
            nhapKhoXml.Element("PreSoHopDong").Value = newPreSoHopDong;
            data.Save(path);
            return "SHDHT" + newPreSoHopDong + soHopDongFormat;
        }

        public static List<Core.Domain.ValueObject.YeuCauTiepNhans.BoPhanCoThe> GetBoPhanCoTheFileJSON()
        {
            var jsonFilePath = @"Resource\\BoPhanCoThe.json";
            string json = File.ReadAllText(jsonFilePath);
            var boPhanCoThes = JsonConvert.DeserializeObject<List<Core.Domain.ValueObject.YeuCauTiepNhans.BoPhanCoThe>>(json);

            return boPhanCoThes;
        }

        public static string CreateSoChungTuGachNo()
        {
            var path = @"Resource\\GachNo.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement soChungTuGachNoXML = data.Descendants(root + "GachNo").FirstOrDefault();
            var soChungTuGachNo = (string)soChungTuGachNoXML.Element(root + "SoChungTuGachNo");
            var preSoChungTuGachNo = (string)soChungTuGachNoXML.Element(root + "PreSoChungTuGachNo");

            var newPreSoChungTuGachNo = DateTime.Now.ToString("yyyyMM");

            var newSoChungTuGachNo = !string.IsNullOrEmpty(soChungTuGachNo) ? Convert.ToInt32(soChungTuGachNo) + 1 : 1;
            if (newPreSoChungTuGachNo != preSoChungTuGachNo)
            {
                newSoChungTuGachNo = 1;
            }

            var soChungTuGachNoFormat = newSoChungTuGachNo.ToString();
            switch (newSoChungTuGachNo.ToString().Length)
            {
                case 1:
                    soChungTuGachNoFormat = "000" + newSoChungTuGachNo;
                    break;
                case 2:
                    soChungTuGachNoFormat = "00" + newSoChungTuGachNo;
                    break;
                case 3:
                    soChungTuGachNoFormat = "0" + newSoChungTuGachNo;
                    break;
            }
            //Cập nhập vào file
            soChungTuGachNoXML.Element("SoChungTuGachNo").Value = newSoChungTuGachNo.ToString();
            soChungTuGachNoXML.Element("PreSoChungTuGachNo").Value = newPreSoChungTuGachNo;
            data.Save(path);
            return newPreSoChungTuGachNo + soChungTuGachNoFormat;
        }

        public static string CreateSoBenhAn()
        {
            var path = @"Resource\\TiepNhanNoiTru.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement tiepNhanNoiTruXML = data.Descendants(root + "BenhAn").FirstOrDefault();
            var soBenhAn = (string)tiepNhanNoiTruXML.Element(root + "SoBenhAn");
            var preSoBenhAn = (string)tiepNhanNoiTruXML.Element(root + "PreSoBenhAn");

            //BVHD-3878
            //var newPreSoBenhAn = DateTime.Now.ToString("yyyyMM");
            var newPreSoBenhAn = DateTime.Now.ToString("yy");

            var newSoBenhAn = !string.IsNullOrEmpty(soBenhAn) ? Convert.ToInt32(soBenhAn) + 1 : 1;
            if (newPreSoBenhAn != preSoBenhAn)
            {
                newSoBenhAn = 1;
            }

            var soBenhAnFormat = newSoBenhAn.ToString();

            //BVHD-3878
            //switch (newSoBenhAn.ToString().Length)
            //{
            //    case 1:
            //        soBenhAnFormat = "000" + newSoBenhAn;
            //        break;
            //    case 2:
            //        soBenhAnFormat = "00" + newSoBenhAn;
            //        break;
            //    case 3:
            //        soBenhAnFormat = "0" + newSoBenhAn;
            //        break;
            //}
            soBenhAnFormat = newSoBenhAn.ToString("000000");

            //Cập nhập vào file
            tiepNhanNoiTruXML.Element("SoBenhAn").Value = newSoBenhAn.ToString();
            tiepNhanNoiTruXML.Element("PreSoBenhAn").Value = newPreSoBenhAn;
            data.Save(path);
            return newPreSoBenhAn + soBenhAnFormat;
        }

        public static string CreateSoLuuTru()
        {
            var path = @"Resource\\TiepNhanNoiTru.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement tiepNhanNoiTruXML = data.Descendants(root + "LuuTru").FirstOrDefault();
            var soLuuTru = (string)tiepNhanNoiTruXML.Element(root + "SoLuuTru");
            var preSoLuuTru = (string)tiepNhanNoiTruXML.Element(root + "PreSoLuuTru");

            //BVHD-3878
            //var newPreSoLuuTru = DateTime.Now.ToString("yyyyMM");
            var newPreSoLuuTru = DateTime.Now.ToString("yy");

            var newLuuTru = !string.IsNullOrEmpty(soLuuTru) ? Convert.ToInt32(soLuuTru) + 1 : 1;
            if (newPreSoLuuTru != preSoLuuTru)
            {
                newLuuTru = 1;
            }

            var soLuuTruFormat = newLuuTru.ToString();

            //BVHD-3878
            //switch (newLuuTru.ToString().Length)
            //{
            //    case 1:
            //        soLuuTruFormat = "000" + newLuuTru;
            //        break;
            //    case 2:
            //        soLuuTruFormat = "00" + newLuuTru;
            //        break;
            //    case 3:
            //        soLuuTruFormat = "0" + newLuuTru;
            //        break;
            //}
            soLuuTruFormat = newLuuTru.ToString("000000");

            //Cập nhập vào file
            tiepNhanNoiTruXML.Element("SoLuuTru").Value = newLuuTru.ToString();
            tiepNhanNoiTruXML.Element("PreSoLuuTru").Value = newPreSoLuuTru;
            data.Save(path);
            return newPreSoLuuTru + soLuuTruFormat;
        }

        public static string CreateSoTamUng()
        {
            var path = @"Resource\\ThuNgan.xml";
            XDocument data = XDocument.Load(path);
            XElement eTamUng = data.Root.Element("TamUng");

            var eNam = eTamUng.Element("Nam");
            var eThang = eTamUng.Element("Thang");
            var eSoTamUng = eTamUng.Element("SoTamUng");
            var now = DateTime.Now;

            if (eNam.Value != now.Year.ToString() || eThang.Value != now.Month.ToString())
            {
                eNam.Value = now.Year.ToString();
                eThang.Value = now.Month.ToString();
                eSoTamUng.Value = "1";
            }
            else
            {
                eSoTamUng.Value = (int.Parse(eSoTamUng.Value) + 1).ToString();
            }
            data.Save(path);
            return $"TU{now:yy}{now:MM}{int.Parse(eSoTamUng.Value).ToString("D6")}";
        }
        public static string CreateSoHoanUng()
        {
            var path = @"Resource\\ThuNgan.xml";
            XDocument data = XDocument.Load(path);
            XElement eHoanUng = data.Root.Element("HoanUng");

            var eNam = eHoanUng.Element("Nam");
            var eThang = eHoanUng.Element("Thang");
            var eSoHoanUng = eHoanUng.Element("SoHoanUng");
            var now = DateTime.Now;

            if (eNam.Value != now.Year.ToString() || eThang.Value != now.Month.ToString())
            {
                eNam.Value = now.Year.ToString();
                eThang.Value = now.Month.ToString();
                eSoHoanUng.Value = "1";
            }
            else
            {
                eSoHoanUng.Value = (int.Parse(eSoHoanUng.Value) + 1).ToString();
            }
            data.Save(path);
            return $"HU{now:yy}{now:MM}{int.Parse(eSoHoanUng.Value).ToString("D6")}";
        }

        public static string CreateSoPhieuChi()
        {
            var path = @"Resource\\ThuNgan.xml";
            XDocument data = XDocument.Load(path);
            XElement ePhieuChi = data.Root.Element("PhieuChi");

            var eNam = ePhieuChi.Element("Nam");
            var eThang = ePhieuChi.Element("Thang");
            var eSoPhieuChi = ePhieuChi.Element("SoPhieuChi");
            var now = DateTime.Now;

            if (eNam.Value != now.Year.ToString() || eThang.Value != now.Month.ToString())
            {
                eNam.Value = now.Year.ToString();
                eThang.Value = now.Month.ToString();
                eSoPhieuChi.Value = "1";
            }
            else
            {
                eSoPhieuChi.Value = (int.Parse(eSoPhieuChi.Value) + 1).ToString();
            }
            data.Save(path);
            return $"PC{now:yy}{now:MM}{int.Parse(eSoPhieuChi.Value).ToString("D6")}";
        }

        public static string CreateSoPhieuThu(long userId, string maTaiKhoan)
        {
            var path = @"Resource\\ThuNgan.xml";
            XDocument data = XDocument.Load(path);
            XElement ePhieuThus = data.Root.Element("PhieuThus");

            var eUser = ePhieuThus.Elements().FirstOrDefault(o => o.Element("Id")?.Value == userId.ToString());
            XElement eSoQuyen = null;
            XElement eSoPhieu = null;
            if (eUser == null)
            {
                eUser = new XElement("User");
                eUser.Add(new XElement("Id", userId));
                eSoQuyen = new XElement("SoQuyen", 1);
                eSoPhieu = new XElement("SoPhieu", 1);
                eUser.Add(eSoQuyen);
                eUser.Add(eSoPhieu);
                ePhieuThus.Add(eUser);
            }
            else
            {
                eSoQuyen = eUser.Element("SoQuyen");
                eSoPhieu = eUser.Element("SoPhieu");
                if (int.Parse(eSoPhieu.Value) >= 1000)
                {
                    eSoQuyen.Value = (int.Parse(eSoQuyen.Value) + 1).ToString();
                    eSoPhieu.Value = "1";
                }
                else
                {
                    eSoPhieu.Value = (int.Parse(eSoPhieu.Value) + 1).ToString();
                }
            }
            data.Save(path);
            return $"{maTaiKhoan}/{eSoQuyen.Value}.{int.Parse(eSoPhieu.Value).ToString("D4")}";
        }

        //public static string CreateSoPhieuXuat(string maKho)
        //{
        //    var path = @"Resource\\XuatKho.xml";
        //    XDocument data = XDocument.Load(path);
        //    XElement ePhieuThus = data.Root.Element("PhieuXuatKhos");

        //    var eUser = ePhieuThus.Elements().FirstOrDefault(o => o.Element("Id")?.Value == maKho);
        //    //XElement eSoQuyen = null;
        //    XElement eNam = null;
        //    XElement eThang = null;
        //    XElement eSoPhieu = null;
        //    if (eUser == null)
        //    {
        //        eUser = new XElement("Kho");
        //        eUser.Add(new XElement("Id", maKho));

        //        eNam = new XElement("Nam",DateTime.Now.Year);
        //        eThang = new XElement("Thang", DateTime.Now.Month);
        //        //eSoQuyen = new XElement("SoQuyen", 1);
        //        eSoPhieu = new XElement("SoTT", 1);
        //        eUser.Add(eSoQuyen);
        //        eUser.Add(eNam);
        //        eUser.Add(eThang);
        //        eUser.Add(eSoPhieu);
        //        ePhieuThus.Add(eUser);
        //    }
        //    else
        //    {
        //        eSoQuyen = eUser.Element("SoQuyen");
        //        eSoPhieu = eUser.Element("SoTT");
        //        if (int.Parse(eSoPhieu.Value) >= 1000)
        //        {
        //            eSoQuyen.Value = (int.Parse(eSoQuyen.Value) + 1).ToString();
        //            eSoPhieu.Value = "1";
        //        }
        //        else
        //        {
        //            eSoPhieu.Value = (int.Parse(eSoPhieu.Value) + 1).ToString();
        //        }
        //    }
        //    data.Save(path);
        //    return $"{maKho}/{eSoQuyen.Value}.{int.Parse(eSoPhieu.Value).ToString("D5")}";
        //}

        public static List<Core.Domain.ValueObject.LookupItemVo> GetChucDanh(DropDownListRequestModel model)
        {
            var path = @"Resource\\ChucDanh.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            var chucDanhXMLs = data.Descendants(root + "ChucDanh").Select(c => new Core.Domain.ValueObject.LookupItemVo
            {
                KeyId = (long)c.Element(root + "STT"),
                DisplayName = (string)c.Element(root + "PreTenChucDanh")
            });

            if (!string.IsNullOrEmpty(model.Query))
            {
                var sreachcChucDanhXMLs = chucDanhXMLs.Where(c => c.DisplayName.ToUpper().RemoveVietnameseDiacritics().Contains(model.Query.ToUpper().RemoveVietnameseDiacritics()));
                return sreachcChucDanhXMLs.ToList();
            }

            return chucDanhXMLs.ToList();
        }
        public static void CreateChucDanh(DropDownListRequestModel model)
        {
            var path = @"Resource\\ChucDanh.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();

            XElement chucDanhXml = data.Root.Element("ChucDanh");

            var chucDanhXMLs = data.Descendants(root + "ChucDanh").Select(c => new Core.Domain.ValueObject.LookupItemVo
            {
                KeyId = (long)c.Element(root + "STT"),
                DisplayName = (string)c.Element(root + "PreTenChucDanh")
            });

            if (!string.IsNullOrEmpty(model.Query))
            {
                var sreachcChucDanhXMLs = chucDanhXMLs.Where(c => c.DisplayName.ToUpper().RemoveVietnameseDiacritics().Contains(model.Query.ToUpper().RemoveVietnameseDiacritics()));
                if (sreachcChucDanhXMLs != null)
                {
                    var xmlSTT = chucDanhXml.Element("STT");
                    var xmlPreTenChucDanh = chucDanhXml.Element("PreTenChucDanh");

                    xmlSTT.Value = (chucDanhXMLs.Count() + 1).ToString();
                    xmlPreTenChucDanh.Value = model.Query;

                    data.Save(path);
                }
            }
        }


        public static List<LookupItemVo> GetViTriLamViecs(DropDownListRequestModel model)
        {
            var path = @"Resource\\ViTriLamViec.xml";
            XDocument data = XDocument.Load(path);
            if (data.Root != null)
            {
                XNamespace root = data.Root.GetDefaultNamespace();
                var viTriLamViecXmls = data.Descendants(root + "ViTriLamViec").Select(c => new LookupItemVo
                {
                    KeyId = (long)c.Element(root + "STT"),
                    DisplayName = (string)c.Element(root + "Ten")
                });
                return viTriLamViecXmls.ToList();
            }

            return null;
        }

        public static List<Core.Domain.ValueObject.LookupItemVo> GetBenhNgheNghiep(DropDownListRequestModel model)
        {
            var path = @"Resource\\BenhNgheNghiep.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            var benhNgheNghiepXMLs = data.Descendants(root + "BenhNgheNghiep").Select(c => new Core.Domain.ValueObject.LookupItemVo
            {
                DisplayName = (string)c.Element(root + "PreTenBenhNgheNghiep")
            });

            if (!string.IsNullOrEmpty(model.Query))
            {
                var sreachcChucDanhXMLs = benhNgheNghiepXMLs.Where(c => c.DisplayName.RemoveVietnameseDiacritics().Contains(model.Query.RemoveVietnameseDiacritics()));
            }

            return benhNgheNghiepXMLs.ToList();
        }

        public static string CreateSTTChuyenTuyen()
        {
            var path = @"Resource\\STTChuyenTuyen.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement sTTXml = data.Descendants(root + "SoThuTuChuyenTuyen").FirstOrDefault();
            var soThuTu = (string)sTTXml.Element(root + "SoThuTu");
            var namHienTai = (string)sTTXml.Element(root + "NamHienTai");
            var soThuTuNumber = Convert.ToInt32(soThuTu);
            var namHienTaiCompare = DateTime.Now.Year;

            if (Convert.ToInt32(namHienTai) == namHienTaiCompare)
            {
                soThuTuNumber = Convert.ToInt32(soThuTu) + 1;
                sTTXml.Element("SoThuTu").Value = soThuTuNumber < 9 ? "0" + soThuTuNumber : soThuTuNumber.ToString();
            }
            else
            {
                soThuTuNumber = 1;
                sTTXml.Element("SoThuTu").Value = soThuTuNumber.ToString();
                sTTXml.Element("NamHienTai").Value = namHienTaiCompare.ToString();
            }
            //Cập nhập vào file
            data.Save(path);
            return soThuTuNumber < 9 ? "0" + soThuTuNumber : soThuTuNumber.ToString();
        }

        public static string CreateSTTChuyenTuyenTheoNguoiBenh()
        {
            var path = @"Resource\\STTChuyenTuyen.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            //List<XElement> sTTXmls = data.Descendants(root + "SoThuTuChuyenTuyen").ToList();
            //XElement sTTXml = null;
            XElement sTTXml = data.Descendants(root + "SoThuTuChuyenTuyen").First();

            var soThuTuNumber = (long)0;
            string namHienTai = DateTime.Now.Year.ToString();
            var namHienTaiCompare = DateTime.Now.Year;
            //if (sTTXmls.Any())
            //{
            //    foreach (var item in sTTXmls)
            //    {
            //        if (nguoiBenhId == (long?)item.Element(root + "NguoiBenhId"))
            //        {
            //            sTTXml = item;
            //            break;
            //        }
            //    }
            //}

            //if (sTTXml == null)
            //{
            //    sTTXml = new XElement("SoThuTuChuyenTuyen",
            //        new XElement("SoThuTu", soThuTuNumber.ToString()),
            //        new XElement("NamHienTai", namHienTai),
            //        new XElement("NguoiBenhId", nguoiBenhId.ToString())
            //    );
            //    data.Root.Add(sTTXml);
            //}

            if (sTTXml == null)
            {
                sTTXml = new XElement("SoThuTuChuyenTuyen",
                    new XElement("SoThuTu", soThuTuNumber.ToString()),
                    new XElement("NamHienTai", namHienTai)
                );
                data.Root.Add(sTTXml);
            }

            soThuTuNumber = (long)sTTXml.Element(root + "SoThuTu");
            namHienTai = (string)sTTXml.Element(root + "NamHienTai");

            if (Convert.ToInt32(namHienTai) == namHienTaiCompare)
            {
                soThuTuNumber += 1;
                sTTXml.Element("SoThuTu").Value = soThuTuNumber.ToString();
            }
            else
            {
                soThuTuNumber = 1;
                sTTXml.Element("SoThuTu").Value = soThuTuNumber.ToString();
                sTTXml.Element("NamHienTai").Value = namHienTaiCompare.ToString();
            }
            //Cập nhật vào file
            data.Save(path);
            return soThuTuNumber.ToString("00"); // soThuTuNumber < 9 ? "0" + soThuTuNumber : soThuTuNumber.ToString();
        }
    }
}
