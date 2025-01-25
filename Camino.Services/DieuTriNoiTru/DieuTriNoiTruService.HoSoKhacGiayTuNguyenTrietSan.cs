using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public NoiTruHoSoKhac GetThongTinHoSoKhacGiayTuNguyenTrietSan(long yeuCauTiepNhanId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayTuNguyenTrietSan)
                                                            .Include(p => p.NoiTruHoSoKhacFileDinhKems)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .FirstOrDefault();
        }

        public async Task<string> InGiayTuNguyenTrietSan(long yeuCauTiepNhanId, string hostingName, bool isInFilePDF = true)
        {
            var today = DateTime.Now;

            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals(isInFilePDF ? "GiayTuNguyenTrietSanPDF" : "GiayTuNguyenTrietSan"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.NguoiLienHeQuanHeNhanThan)
                                                                     .Include(p => p.NoiTruBenhAn)
                                                                     .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                     .FirstOrDefaultAsync();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayTuNguyenTrietSan)
                                                               .FirstOrDefault();

            if (noiTruHoSoKhac == null || noiTruHoSoKhac.ThongTinHoSo == null)
            {
                var defaultData = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    HoTen = yeuCauTiepNhan.HoTen,
                    Tuoi = yeuCauTiepNhan.NamSinh != null ? (today.Year - yeuCauTiepNhan.NamSinh.Value).ToString() : "",
                    GioiTinh = yeuCauTiepNhan.GioiTinh?.GetDescription(),
                    DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                    NguoiTrietSan = yeuCauTiepNhan.HoTen,
                    NguoiThanTrietSan = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten,
                    //NhanVienTrietSan = thongTin.BacSiThucHienDisplay,
                    Ngay = today.Day,
                    Thang = today.Month,
                    Nam = today.Year
                };

                return TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, defaultData);
            }

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacGiayTuNguyenTrietSanVo>(noiTruHoSoKhac.ThongTinHoSo);

           
            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == phongBenhVienId)
                                                                        .Include(p => p.KhoaPhong)
                                                                        .FirstOrDefault();

            var tuoi = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh);
            var gt = yeuCauTiepNhan.GioiTinh?.GetDescription();

            var data = new HoSoKhacGiayInTuNguyenTrietSan();
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "";
            data.MaNB = yeuCauTiepNhan.MaYeuCauTiepNhan;
           
            //string[] listStringKhoas = new string[] { "Khoa", "KHOA" };
            //var khoa = string.Empty;
            //if (!string.IsNullOrEmpty(phongBenhVien.KhoaPhong.Ten))
            //{
            //    foreach (var item in listStringKhoas)
            //    {
            //        khoa = phongBenhVien.KhoaPhong.Ten.Replace(item, "").ToString();
            //    }
            //}
            data.Khoa = "&nbsp;"+phongBenhVien.KhoaPhong.Ten;
            data.HoTen = !string.IsNullOrEmpty(yeuCauTiepNhan.HoTen) ? "<div style='grid - column: 1 / span 1; display: flex'><p style = 'width: 70px'> Tôi tên: &nbsp; </p><p class='show'><b>" + yeuCauTiepNhan.HoTen + "</b></p></div>"
                : "<div style='grid - column: 1 / span 1; display: flex'><p style = 'width: 70px'> Tôi tên: &nbsp; </p><p class='free-space-dotted'>" + yeuCauTiepNhan.HoTen + "</p></div>";

            data.Tuoi = !string.IsNullOrEmpty(tuoi) ? "<div style='grid - column: 2 / span 1; display: flex'><p style='width: 70px'> Năm sinh: &nbsp;</p><p class='show'><b>" + tuoi + "</b></p></div>"
                : "<div style='grid - column: 2 / span 1; display: flex'><p style='width: 70px'> Năm sinh:&nbsp; </p><p class='free-space-dotted'>" + tuoi + "</p></div>";

            data.GioiTinh = !string.IsNullOrEmpty(gt) ? "<div style='grid - column: 3 / span 1; display: flex'><p id = 'gioi-tinh' style='width: 70px'> Giới tính:&nbsp; </p><p class='show'><b>" + gt + "</b></p></div>"
                : "<div style='grid - column: 3 / span 1; display: flex'><p id = 'gioi-tinh' style='width: 70px'> Giới tính:&nbsp; </p><p class='free-space-dotted'>" + gt + "</p></div>";

            data.DiaChi = !string.IsNullOrEmpty(yeuCauTiepNhan.DiaChiDayDu) ? "<div style='grid - column: 1 / span 3; display: flex'><p style ='width: 60px' > Địa chỉ: &nbsp </p><p class='show'><b>" + yeuCauTiepNhan.DiaChiDayDu + "</b></p></div>"
                : "<div style='grid - column: 1 / span 3; display: flex'>< p style = 'width: 60px' > Địa chỉ: &nbsp </p><p class='free-space-dotted'>" + yeuCauTiepNhan.DiaChiDayDu + "</p></div>";

            data.NguoiTrietSan = yeuCauTiepNhan.HoTen;
            //data.NguoiThanTrietSan = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten;
            data.NhanVienTrietSan = thongTin.BacSiThucHienDisplay;

            
            var ngayThucHien = new DateTime(thongTin.NgayThucHien.Year, thongTin.NgayThucHien.Month, (thongTin.NgayThucHien.Day +1));
            
            data.Ngay = "<p>Ngày</p><p class='' style='width: auto;'>" + (ngayThucHien.Day < 9 ? "&nbsp;0" + ngayThucHien.Day + "&nbsp;" : "&nbsp;" + ngayThucHien.Day.ToString() + "&nbsp;") + "</p>";
            data.Thang = "<p>tháng</p><p class='' style='width: auto;'>" + (ngayThucHien.Month < 9 ? "&nbsp;0" + ngayThucHien.Month + "&nbsp;" : "&nbsp;" + ngayThucHien.Month.ToString() + "&nbsp;") + "</p>" ;
            data.Nam = "<p>năm</p><p class='' style='width: auto;'>" + ("&nbsp;" + ngayThucHien.Year.ToString()) + "</p>" ;

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
    }
}