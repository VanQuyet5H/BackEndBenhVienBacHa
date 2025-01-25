using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public string InBienKiemTruocTiemChungTE(long noiTruHoSoKhacId)
        {
            var noiTruHoSoKhac = _noiTruHoSoKhacRepository.TableNoTracking
               .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.NguoiLienHeQuanHeNhanThan)
               .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.NoiTruBenhAn)
               .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)

               .Where(p => p.Id == noiTruHoSoKhacId && p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BanKiemTruocTiemChungTreEm).First();

            var banKiemTruocTiemChungTreEmTemplate = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BanKiemTruocTiemChungTreEm")).First();
            var thongTinHoSo = JsonConvert.DeserializeObject<ThongTinHoSoBienKiemTruocTiemChungTE>(noiTruHoSoKhac.ThongTinHoSo);
            var tenDuocPhams = new List<string>();
            foreach (var item in thongTinHoSo.DuocPhamIds)
            {
                var tenDuocPham = _duocPhamRepository.TableNoTracking.Where(p => p.Id == item).Select(p => p.Ten).FirstOrDefault();
                tenDuocPhams.Add(tenDuocPham);
            }

            var thuocVacxin = string.Empty;
            if (tenDuocPhams.Any())
            {
                foreach (var item in tenDuocPhams)
                {
                    thuocVacxin += "<br>+ " + item;
                }
            }
            string gio = "";
            string phut = "";
            if (noiTruHoSoKhac.YeuCauTiepNhan.GioSinh != null)
            {
                TimeSpan t = TimeSpan.FromSeconds((int)noiTruHoSoKhac.YeuCauTiepNhan.GioSinh);
                string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                  t.Hours,
                  t.Minutes,
                  t.Seconds,
                  t.Milliseconds);
                var gioString = answer.Split('h');
                if(gioString != null)
                {
                    gio = gioString[0];
                }
                if(gioString[1] != null)
                {
                    var phutString = gioString[1].ToString().Split('m');
                    if (phutString != null)
                    {
                        phut = phutString[0];
                    }
                }
                
            }
            var data = new BienKiemTruocTiemChungTE
            {
                SoBA = noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                BarCodeImgBase64 = !string.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn) ? BarcodeHelper.GenerateBarCode(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn) : "",
                HoTen = noiTruHoSoKhac.YeuCauTiepNhan.HoTen,
                GTNam = noiTruHoSoKhac.YeuCauTiepNhan.GioiTinh != null && noiTruHoSoKhac.YeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNam ? "&#10004;" : "&nbsp;",
                GTNu = noiTruHoSoKhac.YeuCauTiepNhan.GioiTinh != null && noiTruHoSoKhac.YeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNu ? "&#10004;" : "&nbsp;",
                Gio = gio,
                Phut = phut,
                NgaySinh = noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh != null && noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh != 0 ? noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh.Value.ConvertDateToString() : "",
                ThangSinh = noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh != null && noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh != 0 ? noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh.Value.ConvertMonthToString() : "",
                NamSinh = noiTruHoSoKhac.YeuCauTiepNhan.NamSinh != null && noiTruHoSoKhac.YeuCauTiepNhan.NamSinh != 0 ? noiTruHoSoKhac.YeuCauTiepNhan.NamSinh.Value.ConvertYearToString() : "",
                DiaChi = noiTruHoSoKhac.YeuCauTiepNhan.DiaChiDayDu,
                HoTenBoMe = noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThan != null && (noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThan.TenVietTat == "ChaDe" || noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThan.TenVietTat == "MeDe") ? noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeHoTen : "",
                ThuocVacXin = thuocVacxin,
                Khong1 = thongTinHoSo.SotHaThanNhiet == true ? "&nbsp;" : "&#10004;",
                Co1 = thongTinHoSo.SotHaThanNhiet != true ? "&nbsp;" : "&#10004;",
                Khong2 = thongTinHoSo.NgheTimBatThuong == true ? "&nbsp;" : "&#10004;",
                Co2 = thongTinHoSo.NgheTimBatThuong != true ? "&nbsp;" : "&#10004;",
                Khong3 = thongTinHoSo.NghePhoiBatThuong == true ? "&nbsp;" : "&#10004;",
                Co3 = thongTinHoSo.NghePhoiBatThuong != true ? "&nbsp;" : "&#10004;",
                Khong4 = thongTinHoSo.TriGiacBatThuong == true ? "&nbsp;" : "&#10004;",
                Co4 = thongTinHoSo.TriGiacBatThuong != true ? "&nbsp;" : "&#10004;",
                Khong5 = thongTinHoSo.CanNangDuoi2000g == true ? "&nbsp;" : "&#10004;",
                Co5 = thongTinHoSo.CanNangDuoi2000g != true ? "&nbsp;" : "&#10004;",
                Khong6 = thongTinHoSo.CoCacChongChiDinhKhac == true ? "&nbsp;" : "&#10004;",
                Co6 = thongTinHoSo.CoCacChongChiDinhKhac != true ? "&nbsp;" : "&#10004;",
                DuDieuKien = thongTinHoSo.DuDieuKienTiemChung == true ? "&#10004;" : "&nbsp;",
                TamHoan = thongTinHoSo.TamHoanTiemChung == true ? "&#10004;" : "&nbsp;",
                ThoiDiemIn = "Hồi " + DateTime.Now.Hour.ConvertHourToString() + " giờ " + DateTime.Now.Minute.ConvertMinuteToString() + " phút",
                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString(),
                BSThucHien = noiTruHoSoKhac.NhanVienThucHien.User.HoTen,
            };
            var content = string.Empty;
            content = TemplateHelpper.FormatTemplateWithContentTemplate(banKiemTruocTiemChungTreEmTemplate.Body, data);
            return content;
        }
    }
}
