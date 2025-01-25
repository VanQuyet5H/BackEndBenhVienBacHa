using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiDuongThai;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public GiayChungNhanNghiDuongThaiGrid GetThongTinGiayChungNhanNghiDuongThai(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai)
                                                                  .Select(s => new GiayChungNhanNghiDuongThaiGrid()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                  }).FirstOrDefault();
            return query;
        }
        public ThongTinChungNhanNghiDuongThai GetDataChungNhanNghiDuongThai(long yeuCauTiepNhanId)
        {
            ThongTinChungNhanNghiDuongThai thongTinChungNhanNghiDuongThai = new ThongTinChungNhanNghiDuongThai();
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            thongTinChungNhanNghiDuongThai.TenNhanVien = nguoiLogin;
            var query = BaseRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => new
            {
                chanDoan = s.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,

            }).FirstOrDefault();
            var entity = _noiTruBenhAnRepository.TableNoTracking.Include(p => p.ThongTinTongKetBenhAn).Where(s => s.Id == yeuCauTiepNhanId).Select(o => o.ThongTinTongKetBenhAn);
            var phuongPhapDieuTri = "";

            if (!string.IsNullOrEmpty(entity.LastOrDefault()))
            {
                var result = JsonConvert.DeserializeObject<PhuongPhapDieuTriModel>(entity.LastOrDefault());
                phuongPhapDieuTri = result.PhuongPhapDieuTri;
            }
            thongTinChungNhanNghiDuongThai.ChanDoanVaPhuongPhapDieuTri = query.chanDoan + phuongPhapDieuTri;
            thongTinChungNhanNghiDuongThai.NgayThucHienDisplay = DateTime.Now.ApplyFormatDateTime();
            return thongTinChungNhanNghiDuongThai;
        }

        public async Task<string> InGiayChungNhanNghiDuongThai(XacNhanInPhieuGiayChungNhanNghiDuongThai xacNhanIn)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InPhieuGiayChungNhanNghiDuongThai>(thongtinIn);
            var content = "";
            var result = _templateRepository.TableNoTracking
                       .FirstOrDefault(x => x.Name.Equals("PhieuNghiDuongThai"));

            var TenBenhVien = "BỆNH VIỆN ĐKQT BẮC HÀ";
            var chanDoan = "";
            var tmp = "\n";
            var replace = "<br>";
            if (queryString.ChanDoan != null)
            {
                chanDoan = queryString.ChanDoan.Replace(tmp, replace);
            }
            var soSeri = "";
            if (!string.IsNullOrEmpty(queryString.SoSeri) && queryString.SoSeri != " ")
            {
                soSeri = queryString.SoSeri;
            }
            else
            {
                soSeri = ".............................";
            }
            var mauSo = "";
            if (!string.IsNullOrEmpty(queryString.MauSo) && queryString.MauSo != " ")
            {
                mauSo = queryString.SoSeri;
            }
            else
            {
                mauSo = ".............................";
            }

            var tuNgayDisplay = string.Empty;
            var denNgayDisplay = string.Empty;
            var soNgayNghiDisplay = string.Empty;

            if (!string.IsNullOrEmpty(queryString.NghiTuNgayDisplay) &&!string.IsNullOrEmpty(queryString.NghiDenNgayDisplay))
            {
              
                DateTime.TryParseExact(queryString.NghiTuNgayDisplay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgay);
                //tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
                tuNgayDisplay = tuNgay.ApplyFormatDate();

                DateTime.TryParseExact(queryString.NghiDenNgayDisplay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgay);
                //denNgay = new DateTime(denNgayTemp.Year, denNgayTemp.Month, denNgayTemp.Day, 23, 59, 59);
                denNgayDisplay = denNgay.ApplyFormatDate();
                List<string> returnHtml = new List<string>();

                TimeSpan difference = denNgay - tuNgay;
                soNgayNghiDisplay = difference.Days + "";
            }
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                .Select(s => new {
                    So = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoVaTen = s.YeuCauTiepNhan.HoTen,
                    NgaySinh = DateHelper.DOBFormat(s.YeuCauTiepNhan.NgaySinh, s.YeuCauTiepNhan.ThangSinh, s.YeuCauTiepNhan.NamSinh),
                    MaSoBHXH = s.YeuCauTiepNhan.BHYTMaSoThe,
                    GioiTinh = s.YeuCauTiepNhan.GioiTinh != null ? s.YeuCauTiepNhan.GioiTinh.GetDescription():"",
                    DonViLamViec = s.YeuCauTiepNhan.NoiLamViec,
                    DayNow = DateTime.Now.Day,
                    MonthNow = DateTime.Now.Month,
                    YearNow = DateTime.Now.Year,
                    ChanDoan = chanDoan,
                    SoNgay = soNgayNghiDisplay,
                    TuNgay = tuNgayDisplay,
                    DenNgay = denNgayDisplay,
                    NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                    SoSeRi = soSeri,
                    MauSo = mauSo
                }).FirstOrDefault();

            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, query);
            return content;
        }
    }
}
