using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public NoiTruHoSoKhac GetThongTinBienBanCamKetGayTeGiamDauTrongDeSauMo(long yeuCauTiepNhanId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BienBanCamKetGayTeGiamDauTrongDeSauMo)
                                                            .Include(p => p.NoiTruHoSoKhacFileDinhKems)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .FirstOrDefault();
        }

        public async Task<string> InBienBanCamKetGayTeGiamDauTrongDeSauMo(long yeuCauTiepNhanId, bool isInFilePDF = true)
        {
            var today = DateTime.Now;

            var cauHinhMaSoBenhVien = await _cauHinhRepository.TableNoTracking.FirstOrDefaultAsync(q => q.Name.Equals("BaoHiemYTe.BenhVienTiepNhan"));

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == phongBenhVienId)
                                                                        .Include(p => p.KhoaPhong)
                                                                        .FirstOrDefault();

            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals(isInFilePDF ? "BienBanCamKetGayTeGiamDauTrongDeSauMoPDF" : "BienBanCamKetGayTeGiamDauTrongDeSauMo"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.NguoiLienHeQuanHeNhanThan)
                                                                     .Include(p => p.NoiTruBenhAn)
                                                                     .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                     .FirstOrDefaultAsync();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BienBanCamKetGayTeGiamDauTrongDeSauMo)
                                                               .FirstOrDefault();
            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var benhVien = _benhVienRepository.TableNoTracking
                          .Where(p => p.Ma == settings.BenhVienTiepNhan).FirstOrDefault();

            var khoa = string.Empty;
            string[] listKhoas = new string[] { "Khoa", "Khoa", "KHOA" };
            if(!string.IsNullOrEmpty(phongBenhVien.KhoaPhong.Ten))
            {
                var tenKhoa = phongBenhVien.KhoaPhong.Ten;

                khoa = tenKhoa.Replace(listKhoas[0], "");
                khoa = tenKhoa.Replace(listKhoas[0], "");
                khoa = tenKhoa.Replace(listKhoas[0], "");
            }

            

            var nb = !string.IsNullOrEmpty(yeuCauTiepNhan.HoTen) ?
                   ("<div class='container'>" +
                          "<div class='label'>Người bệnh:&nbsp;</div>" +
                           $"<div class='values'><b>{yeuCauTiepNhan.HoTen}</b></div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>Người bệnh:&nbsp;</div>" +
                          $"<div class='value'><b>{yeuCauTiepNhan.HoTen}</b></div>" +
                   "</div>");

            var ns = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh);
            var sn = !string.IsNullOrEmpty(ns) ?
                  ("<div class='container'>" +
                         "<div class='label'>Sinh năm:&nbsp;</div>" +
                          $"<div class='values'><b>{ns}</b></div>" +
                  "</div>")
                  :
                  ("<div class='container'>" +
                         "<div class='label'>Sinh năm:&nbsp;</div>" +
                         $"<div class='value'><b>{ns}</b></div>" +
                  "</div>");

            var cmnd = !string.IsNullOrEmpty(yeuCauTiepNhan.SoChungMinhThu) ?
                   ("<div class='container'>" +
                          "<div class='label'>CMND/CCCD/HC:&nbsp;</div>" +
                           $"<div class='values'>{yeuCauTiepNhan.SoChungMinhThu}</div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>CMND/CCCD/HC:&nbsp;</div>" +
                          $"<div class='value'>{yeuCauTiepNhan.SoChungMinhThu}</div>" +
                   "</div>");

            var msbv = !string.IsNullOrEmpty(cauHinhMaSoBenhVien.Value) ?
                   ("<div class='container'>" +
                          "<div class='label'>Mã số bệnh viện:&nbsp;</div>" +
                           $"<div class='values'>{cauHinhMaSoBenhVien.Value}</div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>Mã số bệnh viện:&nbsp;</div>" +
                          $"<div class='value'>{cauHinhMaSoBenhVien.Value}</div>" +
                   "</div>");

            var dc = !string.IsNullOrEmpty(benhVien?.DiaChi) ?
                   ("<div class='container'>" +
                          "<div class='label'>Địa chỉ:&nbsp;</div>" +
                           $"<div class='values'>{benhVien?.DiaChi}</div>" +
                   "</div>")
                   :
                   ("<div class='container'>" +
                          "<div class='label'>Địa chỉ:&nbsp;</div>" +
                          $"<div class='value'>{benhVien?.DiaChi}</div>" +
                   "</div>");

            if (noiTruHoSoKhac == null || noiTruHoSoKhac.ThongTinHoSo == null)
            {
                var nvgtnull =
                ("<div class='container'>" +
                       "<div class='label'>Chúng tôi đã được bác sĩ:&nbsp;</div>" +
                       $"<div class='value'>............................................giải thích và hiểu rõ về các vấn đề liên quan đến gây mê/ gây tê, cụ thể như sau:</div>" +
                "</div>");

                var defaultData = new
                {
                    Khoa = !string.IsNullOrEmpty(khoa) ? khoa : "...........",
                    NguoiBenh = nb,
                    SinhNam = sn,
                    CMNDCCCDHC = cmnd,
                    NgayCap = "",
                    NoiCap = "",
                    MaSoBenhVien = msbv,
                    DiaChi = dc,
                    //QuanHeNhanThans = quanHeThanNhans,
                    //BacSiGiaiThich = thongTin.BacSiGiaiThich,
                    Ngay = today.Day,
                    Thang = today.Month,
                    Nam = today.Year,
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    TenNB = yeuCauTiepNhan.HoTen
                };

                return TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, defaultData);
            }

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoVo>(noiTruHoSoKhac.ThongTinHoSo);

            var quanHeNhanThanChong = _quanHeThanNhanRepository.TableNoTracking.Where(p => p.TenVietTat.Equals("Chong")).FirstOrDefault();
            var thongTinChong = new ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMo();
            if(quanHeNhanThanChong != null && thongTin.QuanHeThanNhans.Any())
            {
                thongTinChong = thongTin.QuanHeThanNhans.Where(p => p.QuanHeId == quanHeNhanThanChong.Id).FirstOrDefault();
            }

            var quanHeThanNhans = string.Empty;

            //Chồng

            if(isInFilePDF)
            {
                quanHeThanNhans += quanHeThanNhans +=
                   
                "<tr>" +
                    "<td colspan='6' style='width: 75%;'>" +
                    (!string.IsNullOrEmpty(thongTinChong?.HoTen) ?
                        "<div class='container'>" +
                            "<div class='label'>Và người cần báo tin của người bệnh(chồng):&nbsp;</div>" +
                            $"<div class='values'>{thongTinChong?.HoTen}</div>" +
                        "</div>" :
                         "<div class='container'>" +
                            "<div class='label'>Và người cần báo tin của người bệnh(chồng):&nbsp;</div>" +
                            $"<div class='value'>{thongTinChong?.HoTen}</div>" +
                        "</div>"
                     ) +
                    "</td>" +
                    "<td colspan='2' style='width: 25%;'>" +
                    (thongTinChong?.NamSinh != null ?
                        "<div class='container'>" +
                            "<div class='label'>Sinh năm: &nbsp;</div>" +
                            $"<div class='values'>{thongTinChong?.NamSinh}</div>" +
                        "</div>" :
                        "<div class='container'>" +
                            "<div class='label'>Sinh năm: &nbsp;</div>" +
                            $"<div class='value'>{thongTinChong?.NamSinh}</div>" +
                        "</div>"
                    ) +
                    "</td>" +
                "</tr>";
            }
            else
            {
                quanHeThanNhans += "<table style='width: 100%;'>" +
                        "<tr>" +
                            "<td width='75%' >" +
                              (!string.IsNullOrEmpty(thongTinChong?.HoTen) ?
                                "<div class='container'>" +
                                    "<div class='label'>Và người cần báo tin của người bệnh (chồng):<span> </span></div>" +
                                    $"<div class='values'>{thongTinChong?.HoTen}</div>" +
                                "</div>" :
                                "<div class='container'>" +
                                    "<div class='label'>Và người cần báo tin của người bệnh (chồng):<span> </span></div>" +
                                    $"<div class='value'>{thongTinChong?.HoTen}</div>" +
                                "</div>"
                               ) +
                            "</td>" +
                            "<td width='25%' style='font-size: 13px;'>" +
                            (thongTinChong?.NamSinh != null ?
                                "<div class='container'>" +
                                    "<div class='label'>Sinh năm:<span> </span></div>" +
                                    $"<div class='values'>{thongTinChong?.NamSinh}</div>" +
                                "</div>":
                                "<div class='container'>" +
                                    "<div class='label'>Sinh năm:<span> </span></div>" +
                                    $"<div class='value'>{thongTinChong?.NamSinh}</div>" +
                                "</div>"
                            ) +
                            "</td>" +
                        "</tr>" +
                    "</table>";
            }

            if (thongTin.QuanHeThanNhans.Any(p => p.Id != (thongTinChong?.Id ?? 0)))
            {
                foreach(var thanNhan in thongTin.QuanHeThanNhans)
                {
                    if(thanNhan.Id != (thongTinChong?.Id ?? 0))
                    {
                        if (isInFilePDF)
                        {
                            quanHeThanNhans +=
                                "<tr>" +
                                    "<td colspan='6' style='width: 75%;'>" +
                                        "<div class='container'>" +
                                            $"<div class='label'>người cần báo tin của người bệnh({thanNhan.QuanHeDisplay}):&nbsp;</div>" +
                                         (!string.IsNullOrEmpty(thanNhan.HoTen) ?
                                            $"<div class='values'>{thanNhan.HoTen}</div>" :
                                            $"<div class='value'>{thanNhan.HoTen}</div>" 
                                         )+
                                        "</div>" +
                                    "</td>" +
                                    "<td colspan='2' style='width: 25%;'>" +
                                        "<div class='container'>" +
                                            "<div class='label'>Sinh năm:&nbsp;</div>" +
                                         (thanNhan?.NamSinh != null ?
                                            $"<div class='values'>{thanNhan.NamSinh}</div>":
                                            $"<div class='value'>{thanNhan.NamSinh}</div>"
                                         ) +
                                        "</div>" +
                                    "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan='6' style='width: 75%;'>" +
                                        "<div class='container'>" +
                                            "<div class='label'>Địa chỉ: &nbsp;</div>" +
                                        (!string.IsNullOrEmpty(thanNhan.DiaChi) ?
                                            $"<div class='values'>{thanNhan.DiaChi}</div>":
                                            $"<div class='value'>{thanNhan.DiaChi}</div>"
                                        ) +
                                        "</div>" +
                                    "</td>" +
                                    "<td colspan='2' style='width: 25%;'>" +
                                        "<div class='container'>" +
                                            "<div class='label'>CMTND:&nbsp;</div>" +
                                        (!string.IsNullOrEmpty(thanNhan.CMND) ?
                                            $"<div class='values'>{thanNhan.CMND}</div>":
                                            $"<div class='value'>{thanNhan.CMND}</div>"
                                        ) +
                                        "</div>" +
                                    "</td>" +
                                "</tr>";
                        }
                        else
                        {
                            quanHeThanNhans += "<table style='width:100%'>" +
                                "<tr>" +
                                    "<td width='75%' >" +
                                        "<div class='container'>" +
                                            $"<div class='label'><span>người cần báo tin của người bệnh ({thanNhan.QuanHeDisplay}):</span></div>" +
                                            (!string.IsNullOrEmpty(thanNhan.HoTen) ?
                                            $"<div class='values'>{thanNhan.HoTen}</div>":
                                            $"<div class='value'>{thanNhan.HoTen}</div>"
                                            ) +
                                        "</div>" +
                                    "</td>" +
                                    "<td width='25%' >" +
                                        "<div class='container'>" +
                                            "<div class='label'>Sinh năm:<span> </span></div>" +
                                            (thanNhan?.NamSinh != null ?
                                            $"<div class='values'>{thanNhan.NamSinh}</div>" :
                                            $"<div class='value'>{thanNhan.NamSinh}</div>"
                                            ) +
                                        "</div>" +
                                    "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td width='75%' >" +
                                        "<div class='container'>" +
                                            "<div class='label'> Địa chỉ:</div>" +
                                             (!string.IsNullOrEmpty(thanNhan.DiaChi) ?
                                            $"<div class='values'>{thanNhan.DiaChi}</div>":
                                            $"<div class='value'>{thanNhan.DiaChi}</div>"
                                            ) +
                                        "</div>" +
                                    "</td>" +
                                    "<td width='25%' >" +
                                        "<div class='container'>" +
                                            "<div class='label'>CMTND:</div>" +
                                             (!string.IsNullOrEmpty(thanNhan.CMND) ?
                                            $"<div class='values'>{thanNhan.CMND}</div>":
                                            $"<div class='value'>{thanNhan.CMND}</div>"
                                            ) +
                                        "</div>" +
                                    "</td>" +
                                "</tr>" +
                           "</table>";
                        }
                    }
                }
            }
            else
            {
                if (isInFilePDF)
                {
                    quanHeThanNhans +=
                    "<tr>" +
                        "<td colspan='6' style='width: 75%;'>" +
                            "<div class='container'>" +
                                "<div class='label'>người cần báo tin của người bệnh(&emsp;):&nbsp;</div>" +
                                "<div class='value'>&emsp;</div>" +
                            "</div>" +
                        "</td>" +
                        "<td colspan='2' style='width: 25%;>" +
                            "<div class='container'>" +
                                "<div class='label'>Sinh năm:&nbsp;</div>" +
                                "<div class='value'>&emsp;</div>" +
                            "</div>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td colspan='6' style='width: 75%;'>" +
                            "<div class='container'>" +
                                "<div class='label'>Địa chỉ: &nbsp;</div>" +
                                "<div class='value'>&emsp;</div>" +
                            "</div>" +
                        "</td>" +
                        "<td colspan='2' style='width: 25%;'>" +
                            "<div class='container'>" +
                                "<div class='label'>CMTND:&nbsp;</div>" +
                                "<div class='value'>&emsp;</div>" +
                            "</div>" +
                        "</td>" +
                    "</tr>";
                }
                else
                {
                    quanHeThanNhans += "<table style='width:100%'>" +
                        "<tr>" +
                            "<td width='25%' >" +
                                "<div class='container'>" +
                                    "<div class='label'> người cần báo tin của người bệnh</div>" +
                                    "<div class='value'>(&nbsp;)</div>" +
                                "</div>" +
                            "</td>" +
                            "<td width='55%' >" +
                                "<div class='container'>" +
                                    "<div class='label'>:</div>" +
                                    "<div class='value'></div>" +
                                "</div>" +
                            "</td>" +
                            "<td >" +
                                "<div class='container'>" +
                                    "<div class='label'>Sinh năm:</div>" +
                                    "<div class='value'></div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>" +
                   "</table>" +
                   "<table style='width:100%'>" +
                        "<tr>" +
                            "<td width='65%' >" +
                                "<div class='container'>" +
                                    "<div class='label'> Địa chỉ:</div>" +
                                    "<div class='value'></div>" +
                                "</div>" +
                            "</td>" +
                            "<td width='35%'>" +
                                "<div class='container'>" +
                                    "<div class='label'>CMTND:</div>" +
                                    "<div class='value'></div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>" +
                "</table>";
                }
            }
            var nvgt = !string.IsNullOrEmpty(thongTin.BacSiGiaiThich) ?
                  ("<div class='container'>" +
                          $"<div class='values'>Chúng tôi đã được bác sĩ:&nbsp;{thongTin.BacSiGiaiThich} giải thích và hiểu rõ về các vấn đề liên quan đến gây mê/ gây tê, cụ thể như sau:</div>" +
                  "</div>")
                  :
                  ("<div class='container'>" +
                         "<div class='label'>Chúng tôi đã được bác sĩ:&nbsp;</div>" +
                         $"<div class='value'>{thongTin.BacSiGiaiThich}giải thích và hiểu rõ về các vấn đề liên quan đến gây mê/ gây tê, cụ thể như sau:</div>" +
                  "</div>");

            
            CultureInfo ci = new CultureInfo("en-NZ");
            string date = thongTin.NgayThucHien.GetValueOrDefault().ToString("R", ci);
            DateTime convertedDate = DateTime.Parse(date);


            var data = new HoSoKhacBienBanInCamKetGayTeGiamDauTrongDeSauMo
            {
                Khoa = !string.IsNullOrEmpty(khoa) ? khoa : "...........",
                NguoiBenh = nb,
                SinhNam =sn,
                CMNDCCCDHC = cmnd,
                NgayCap = "",
                NoiCap = "",
                MaSoBenhVien = msbv,
                DiaChi = dc,
                QuanHeNhanThans = quanHeThanNhans,
                //BacSiGiaiThich = thongTin.BacSiGiaiThich,
                BacSiGiaiThich = nvgt,
                BacSiGayMe = thongTin.BacSiGMHSDisplay,
                Ngay = convertedDate.Day,
                Thang = convertedDate.Month,
                Nam = convertedDate.Year,
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                TenNB = yeuCauTiepNhan.HoTen
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
    }
}
