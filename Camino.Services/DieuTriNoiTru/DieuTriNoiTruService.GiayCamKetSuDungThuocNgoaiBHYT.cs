using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

        public string GetChanDoanNhapVienGiayCamKetSuDungNgoaiBHYT(long yctnId)
        {
            var cd = string.Empty;


            var yeuCauNhapVienId = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == yctnId).Select(d => d.YeuCauNhapVienId).FirstOrDefault();
            if(yeuCauNhapVienId != null)
            {
                var yeuCauKhamBenhId = _yeuCauNhapVienRepository.TableNoTracking.Where(x => x.Id == yeuCauNhapVienId)
               .Select(x => x.YeuCauKhamBenhId).FirstOrDefault();
                if (yeuCauKhamBenhId != null)
                {
                    var info = _yeuCauKhamBenhRepository.TableNoTracking.Where(d => d.Id == yeuCauKhamBenhId)
                         .Select(d => new {
                             ChanDoanChinh = d.IcdchinhId != null ? d.Icdchinh.TenTiengViet : "",
                             YeuCauKhamBenhICDKhacs = d.YeuCauKhamBenhICDKhacs.Select(f => (f.ICDId != null ? f.ICD.TenTiengViet : "")).ToList()

                         }).FirstOrDefault();
                    cd += info.ChanDoanChinh;
                    if (info.YeuCauKhamBenhICDKhacs.Count() != 0)
                    {
                        cd += "(Chẩn đoản kèm theo: " + info.YeuCauKhamBenhICDKhacs.ToList().Join(", ") +")";
                    }


                }
            }
            return cd;
        }
        public async Task<string> PhieuInGiayCamKetSuDungNgoaiBHYT(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("GiayCamKetSuDungThuocNgoaiBHYT"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            var result = await _yeuCauTiepNhanRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
           
            var khoa = infoBn.Khoa;
            GiayCamKetSuDungThuocNgoaiBHYTVo giayCamKet;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetSuDungThuocNgoaiBHYT)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();

            giayCamKet = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<GiayCamKetSuDungThuocNgoaiBHYTVo>(thongTinHoSo)
                : new GiayCamKetSuDungThuocNgoaiBHYTVo();
            var hoTenNguoiThan = giayCamKet.TaoLaAi == 1 ? giayCamKet.HoTen : result.HoTen;
            var diaChinguoiThan = giayCamKet.TaoLaAi == 1 ? giayCamKet.DiaChi : result.DiaChiDayDu;
            var namSinh = giayCamKet.TaoLaAi == 1 ? giayCamKet.NamSinh : result.NamSinh;
            var gioiTinh = giayCamKet.TaoLaAi == 1 ? giayCamKet.GioiTinh != null ? giayCamKet.GioiTinh == 1 ? "Nam" : "Nữ" : string.Empty : result.GioiTinh != null ? result.GioiTinh.GetDescription() : "";
            
            var hoTenBenhNhan = giayCamKet.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var nguoiVietCamKet = giayCamKet.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var chanDoan = "";
            var tmp = "\n";
            var replace = "<br>";
            if (giayCamKet.ChanDoan != null)
            {
                chanDoan = giayCamKet.ChanDoan.Replace(tmp, replace);
            }

            var ngay = string.Empty;
            var thang = string.Empty;
            var nam = string.Empty;
            if (!string.IsNullOrEmpty(giayCamKet.NgayThucHienString))
            {
                DateTime ngayTH = DateTime.Now;
                DateTime.TryParseExact(giayCamKet.NgayThucHienString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayTH);

                ngay = ngayTH.Day > 9 ? ngayTH.Day + "" : "0" + ngayTH.Day;
                thang = ngayTH.Month > 9 ? ngayTH.Month + "" : "0" + ngayTH.Month;
                nam = ngayTH.Year +"";
            }
            else
            {
                ngay = "......";
                thang = "......";
                nam = "......";
            }
         
           

            var data = new InGiayCamKetSuDungThuocNgoaiBHYT();
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(result.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(result.MaYeuCauTiepNhan.ToString()) : "";
            data.MaTN = result.MaYeuCauTiepNhan;
          

            data.TenToiLa = !string.IsNullOrEmpty(hoTenBenhNhan) ? "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='values' style='text-transform: uppercase;'><b>" + hoTenBenhNhan + "</b></div></div>"
                : "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='value'><b>" + "&nbsp;" + "</b></div></div>";

            var ns = DateHelper.DOBFormat(result.NgaySinh, result.ThangSinh, result.NamSinh);

            data.NamSinh = !string.IsNullOrEmpty(ns) ? "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='values'><b>" + ns + "</b></div></div>"
                : "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='value'><b>" + ns + "</b></div></div>";

            data.GioiTinh = !string.IsNullOrEmpty(result.GioiTinh?.GetDescription()) ? "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='values'><b>" + result.GioiTinh?.GetDescription() + "</b></div></div>"
                : "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";



            data.ChanDoan = !string.IsNullOrEmpty(giayCamKet.ChanDoan) ? "<div class='container'><div class='label'></div><div class='values'>Chẩn đoán:&nbsp;" + giayCamKet.ChanDoan?.Replace("\n","<br>") + "</div></div>"
                : "<div class='container'><div class='label'>Chẩn đoán:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";


            data.DiaChi = !string.IsNullOrEmpty(result.DiaChiDayDu) ? "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='values'>" + result.DiaChiDayDu + "</div></div>"
                : "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";

            data.NguoiThan = !string.IsNullOrEmpty(hoTenNguoiThan) ? "<div class='container'><div class='label'>là người bệnh/đại diện gia đình người bệnh/họ tên là:&nbsp;</div><div class='values'><b>" + hoTenNguoiThan + "</b></div></div>"
                : "<div class='container'><div class='label'>là người bệnh/đại diện gia đình người bệnh/họ tên là:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";

            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();

            data.KhoaCreate = tenKhoa;

            data.Khoa = !string.IsNullOrEmpty(tenKhoa) ? "<div class='container'><div class='label'>hiện đang điều trị tại khoa:&nbsp;</div><div class='values'>" + tenKhoa + " Bệnh viện đa khoa Quốc tế Bắc Hà." + "</div></div>"
                : "<div class='container'><div class='label'>hiện đang điều trị tại khoa:&nbsp;</div><div class='value'>" + "&nbsp;" + " Bệnh viện đa khoa Quốc tế Bắc Hà." + "</div></div>"; ;

            data.NgayThangNam = $"ngày {ngay} tháng {thang} năm {nam}";

            data.NguoiDaiDien = hoTenNguoiThan;

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
    }
}
