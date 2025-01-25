using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {

        public string GetTenNhanVienGiaiThich(long nhanVienId)
        {
            var res = _nhanVienRepository.TableNoTracking.Where(p => p.Id == nhanVienId).Select(p => p.User.HoTen).FirstOrDefault();
            return res;
        }

        public string GetTenQuanHeThanNhan(long? quanHeThanNhanId)
        {
            if (quanHeThanNhanId == 0 || quanHeThanNhanId == null)
            {
                return null;
            }
            var res = _quanHeThanNhanRepository.TableNoTracking.Where(p => p.Id == quanHeThanNhanId).Select(p => p.Ten).FirstOrDefault();
            return res;
        }

        public string InBienBanGayMeGayTe(long noiTruHoSoKhacId)
        {
            var noiTruHoSoKhac = _noiTruHoSoKhacRepository.TableNoTracking
                .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.NoiTruBenhAn).ThenInclude(p => p.KhoaPhongNhapVien)
                .Where(p => p.Id == noiTruHoSoKhacId && p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BienBanCamKetGayMeGayTe).First();

            var bienBanGayMeTemplate = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BienBanGayMeGayTe")).First();
            var content = string.Empty;
            var quanHeThanNhanKhacs = new List<ThongTinQuanHeThanNhanVo>();
            var qhThanNhans = string.Empty;
            var thongTinHoSo = JsonConvert.DeserializeObject<ThongTinHoSoJSON>(noiTruHoSoKhac.ThongTinHoSo);
            var nhanVienGiaiThich = GetTenNhanVienGiaiThich(thongTinHoSo.NhanVienGiaiThichId);

            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var benhVien = _benhVienRepository.TableNoTracking
                          .Where(p => p.Ma == settings.BenhVienTiepNhan).FirstOrDefault();

            var khoa = string.Empty;
            string[] listKhoas = new string[] { "Khoa", "Khoa", "KHOA" };
            if (!string.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVien.Ten))
            {
                var tenKhoa = noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVien.Ten;

                khoa = tenKhoa.Replace(listKhoas[0], "");
                khoa = tenKhoa.Replace(listKhoas[0], "");
                khoa = tenKhoa.Replace(listKhoas[0], "");
            }
         
            var nb = !string.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.HoTen) ?
                  ("<div class='container'>" +
                         "<div class='label'>Người bệnh:&nbsp;</div>" +
                          $"<div class='values'><b>{noiTruHoSoKhac.YeuCauTiepNhan.HoTen}</b></div>" +
                  "</div>")
                  :
                  ("<div class='container'>" +
                         "<div class='label'>Sinh năm:&nbsp;</div>" +
                         $"<div class='value'><b>{noiTruHoSoKhac.YeuCauTiepNhan.HoTen}</b></div>" +
                  "</div>");


            var ns = DateHelper.DOBFormat(noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh, noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh, noiTruHoSoKhac.YeuCauTiepNhan.NamSinh);
            var sn = !string.IsNullOrEmpty(ns) ?
                  ("<div class='container'>" +
                         "<div class='label'>Sinh năm:&nbsp;</div>" +
                          $"<div class='values'><b>{ns}</b></div>" +
                  "</div>")
                  :
                  ("<div class='container'>" +
                         "<div class='label'>Sinh năm:&nbsp;</div>" +
                         $"<div class='value'>{ns}</div>" +
                  "</div>");

            var cmnd = !string.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.SoChungMinhThu) ?
                   ("<div class='container'>" +
                          "<div class='label'>CMND/CCCD/HC:&nbsp;</div>" +
                           $"<div class='values'>{noiTruHoSoKhac.YeuCauTiepNhan.SoChungMinhThu}</div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>CMND/CCCD/HC:&nbsp;</div>" +
                          $"<div class='value'>{noiTruHoSoKhac.YeuCauTiepNhan.SoChungMinhThu}</div>" +
                   "</div>");

            var msbv = !string.IsNullOrEmpty(benhVien?.Ma) ?
                   ("<div class='container'>" +
                          "<div class='label'>Mã số bệnh viện:&nbsp;</div>" +
                           $"<div class='values'>{benhVien?.Ma}</div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>Mã số bệnh viện:&nbsp;</div>" +
                          $"<div class='value'>{benhVien?.Ma}</div>" +
                   "</div>");

            var dcbv = !string.IsNullOrEmpty(benhVien?.DiaChi) ?
                   ("<div class='container'>" +
                          "<div class='label'>Địa chỉ:&nbsp;</div>" +
                           $"<div class='values'>{benhVien?.DiaChi}</div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>Địa chỉ:&nbsp;</div>" +
                          $"<div class='value'>{benhVien?.DiaChi}</div>" +
                   "</div>");
            var nvgt = !string.IsNullOrEmpty(nhanVienGiaiThich) ?
                   ("<div class='container'>" +
                           $"<div class='values'>Chúng tôi đã được bác sĩ:&nbsp;{nhanVienGiaiThich} giải thích và hiểu rõ về các vấn đề liên quan đến gây mê/ gây tê, cụ thể như sau:</div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>Chúng tôi đã được bác sĩ:&nbsp;</div>" +
                          $"<div class='value'>{nhanVienGiaiThich}giải thích và hiểu rõ về các vấn đề liên quan đến gây mê/ gây tê, cụ thể như sau:</div>" +
                   "</div>");

            foreach (var item in thongTinHoSo.ThongTinQuanHeThanNhans)
            {
                var quanHeThanNhanKhac = new ThongTinQuanHeThanNhanVo
                {
                    HoTen = item.HoTen,
                    NamSinh = item.NamSinh,
                    CMND = item.CMND,
                    TenQuanHeThanNhan = GetTenQuanHeThanNhan(item.QuanHeThanNhanId),
                    DiaChi = item.DiaChi
                };
                quanHeThanNhanKhacs.Add(quanHeThanNhanKhac);
            }
            var count = 0;
            if (quanHeThanNhanKhacs.Any())
            {
                foreach (var item in quanHeThanNhanKhacs)
                {
                    count++;
                    var nstn = item.NamSinh != null ? item.NamSinh.ToString() :"";
                    //&nbsp
                    if (count == 1)
                    {
                        qhThanNhans += "<tr>"
                                     + "<td> <div class='container'>" +
                                              "<div class='label'>Và người cần báo tin của người bệnh " + (!string.IsNullOrEmpty(item.TenQuanHeThanNhan) ? "(" + item.TenQuanHeThanNhan + ")" : " ") + (!string.IsNullOrEmpty(item.HoTen) ?  ":</div><div class='values'>" + item.HoTen + "</div></div>" 
                                                                                                                                                                                                                                         : ":</div><div class='value'>" + item.HoTen + "</div></div>")
                                       + "</td>"
                                      + "<td> <div class='container'>" +
                                        (!string.IsNullOrEmpty(nstn) ? "<div class='label'>Sinh năm:</div><div class='values'>" + item.NamSinh + "</div></div>"
                                                                             : "<div class='label'>Sinh năm:</div><div class='value'>" + item.NamSinh + "</div></div>")
                                           
                                      + "</td>"
                                      + "</tr>"
                                      + "<tr>"
                                       + "<td> <div class='container'>" +
                                       (!string.IsNullOrEmpty(item.DiaChi) ?
                                            "<div class='label'>Địa chỉ:</div><div class='values'>" + item.DiaChi + "</div></div>"
                                            : "<div class='label'>Địa chỉ:</div><div class='value'>" + item.DiaChi + "</div></div>")
                                       + "</td>"
                                       + "<td> <div class='container'>" +
                                         (!string.IsNullOrEmpty(item.CMND) ?
                                            "<div class='label'>CMND:</div><div class='values'>" + item.CMND + "</div></div>"
                                            : "<div class='label'>CMND:</div><div class='value'>" + item.CMND + "</div></div>")
                                       + "</td>"
                                       + "</tr>"
                                      ;
                    }
                    if (count > 1)
                    {
                        qhThanNhans += "<tr>"
                                       + "<td> <div class='container'>" +
                                              "<div class='label'>người cần báo tin của người bệnh " + (!string.IsNullOrEmpty(item.TenQuanHeThanNhan) ? "(" + item.TenQuanHeThanNhan + ")" : " ") + (!string.IsNullOrEmpty(item.HoTen) ? ":</div><div class='values'>" + item.HoTen + "</div></div>"
                                                                                                                                                                                                                                         : ":</div><div class='value'>" + item.HoTen + "</div></div>")
                                       + "</td>"
                                       + "<td> <div class='container'>" +
                                             (!string.IsNullOrEmpty(nstn) ? "<div class='label'>Sinh năm:</div><div class='values'>" + item.NamSinh + "</div></div>"
                                                                             : "<div class='label'>Sinh năm:</div><div class='value'>" + item.NamSinh + "</div></div>")
                                       + "</td>"
                                       + "</tr>"
                                       + "<tr>"
                                       + "<td> <div class='container'>" +
                                             (!string.IsNullOrEmpty(item.DiaChi) ?
                                            "<div class='label'>Địa chỉ:</div><div class='values'>" + item.DiaChi + "</div></div>"
                                            : "<div class='label'>Địa chỉ:</div><div class='value'>" + item.DiaChi + "</div></div>")
                                       + "</td>"
                                       + "<td> <div class='container'>" +
                                            (!string.IsNullOrEmpty(item.CMND) ?
                                            "<div class='label'>CMND:</div><div class='values'>" + item.CMND + "</div></div>"
                                            : "<div class='label'>CMND:</div><div class='value'>" + item.CMND + "</div></div>")
                                       + "</td>"
                                       + "</tr>"
                                       ;
                    }
                }
                if (count == 1)
                {
                    qhThanNhans += "<tr>"
                                      + "<td> <div class='container'>" +
                                              "<div class='label'>người cần báo tin của người bệnh(....):</div><div class='value'>&nbsp</div></div>"
                                      + "</td>"
                                      + "<td> <div class='container'>" +
                                           "<div class='label'>Sinh năm:</div><div class='value'>&nbsp</div></div>"
                                      + "</td>"
                                      + "</tr>"
                                       + "<tr>"
                                      + "<td> <div class='container'>" +
                                              "<div class='label'>Địa chỉ: </div><div class='value'>&nbsp</div></div>"
                                      + "</td>"
                                      + "<td> <div class='container'>" +
                                           "<div class='label'>CMND :</div><div class='value'>&nbsp</div></div>"
                                      + "</td>"
                                      + "</tr>";
                }

            }
            else
            {
                qhThanNhans += "<tr>"
                               + "<td> <div class='container'>" +
                                    "<div class='label'>Và người cần báo tin của người bệnh (chồng):</div><div class='value'>&nbsp;</div></div>"
                               + "</td>"
                               + "<td> <div class='container'>" +
                                    "<div class='label'>Sinh năm:</div><div class='value'></div></div>"
                               + "</td>"
                               + "</tr>"

                               + "<tr>"
                               + "<td> <div class='container'>" +
                                    "<div class='label'>người cần báo tin của người bệnh (............):</div><div class='value'></div></div>"
                               + "</td>"
                               + "<td> <div class='container'>" +
                                    "<div class='label'>Sinh năm:</div><div class='value'></div></div>"
                               + "</td>"
                               + "</tr>"

                               + "<tr>"
                               + "<td> <div class='container'>" +
                                    "<div class='label'>Địa chỉ:</div><div class='value'></div></div>"
                               + "</td>"
                               + "<td> <div class='container'>" +
                                    "<div class='label'>CMND:</div><div class='value'></div></div>"
                               + "</td>"
                               + "</tr>"
                               ;
            }
         
            var data = new BienBanGayTeGayMe
            {
                Khoa = khoa,
                NguoiBenh = nb ,
                SinhNam = sn,
                CMND = cmnd,
                MSBenhVien = msbv,
                DiaChiBV = dcbv,
                QuanHeThanNhanKhacs = qhThanNhans,
                BSGiaiThich = nvgt ,
                NhanVienThucHien = noiTruHoSoKhac.NhanVienThucHien.User.HoTen,
                Ngay = (noiTruHoSoKhac.ThoiDiemThucHien.Day > 9 ? noiTruHoSoKhac.ThoiDiemThucHien.Day +"": "0" + noiTruHoSoKhac.ThoiDiemThucHien.Day),
                Thang = (noiTruHoSoKhac.ThoiDiemThucHien.Month > 9 ? noiTruHoSoKhac.ThoiDiemThucHien.Month + "" : "0" + noiTruHoSoKhac.ThoiDiemThucHien.Month),
                Nam = noiTruHoSoKhac.ThoiDiemThucHien.Year.ConvertDateToString(),
                MaTN = noiTruHoSoKhac.YeuCauTiepNhan.MaYeuCauTiepNhan,
                BarCodeImgBase64 = !string.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(noiTruHoSoKhac.YeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                TenNB = noiTruHoSoKhac.YeuCauTiepNhan.HoTen
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(bienBanGayMeTemplate.Body, data);
            return content;
        }

        public async Task<ThongTinNhanVienDangNhap> ThongTinNhanVienDangNhap()
        {
            var nhanVienId = _userAgentHelper.GetCurrentUserId();
            var tenNhanVien = await _nhanVienRepository.TableNoTracking.Where(p => p.Id == nhanVienId).Select(p => p.User.HoTen).FirstAsync();
            return new ThongTinNhanVienDangNhap
            {
                NhanVienDangNhapId = nhanVienId,
                NhanVienDangNhap = tenNhanVien,
                NgayThucHien = DateTime.Now
            };
        }
    }
}
