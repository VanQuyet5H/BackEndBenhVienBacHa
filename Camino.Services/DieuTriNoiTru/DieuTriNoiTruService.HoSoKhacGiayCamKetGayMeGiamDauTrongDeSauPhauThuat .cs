using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
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
        public string GetTenBS(long bsId)
        {
            var result = _useRepository.TableNoTracking.Where(d => d.Id == bsId).Select(d => d.HoTen).FirstOrDefault();
            return result;
        }
        public async Task<string> PhieuInGiayCamKetGayMeGiamDauTrongDeSauPhauThuat(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("GiayCamKetGayTeGiamDauTrongDeSauPhauThuat"));
            var infoBn = await ThongTinBenhNhanGiayCamKetGayMeGiamDauTrongDeSauPhauThuat(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            var result = await _yeuCauTiepNhanRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );

            var khoa = infoBn.Khoa;
            GiayCamKetGayMeGiamDauTrongDeSauPhauThuatVo giayCamKet;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetGayMeGiamDauTrongDeSauPhauThuat)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();

            giayCamKet = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<GiayCamKetGayMeGiamDauTrongDeSauPhauThuatVo>(thongTinHoSo)
                : new GiayCamKetGayMeGiamDauTrongDeSauPhauThuatVo();
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
                nam = ngayTH.Year + "";
            }
            else
            {
                ngay = "......";
                thang = "......";
                nam = "......";
            }



            var data = new InGiayCamKetGayMeGiamDauTrongDeSauPhauThuat();
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(result.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(result.MaYeuCauTiepNhan.ToString()) : "";
            data.MaTN = result.MaYeuCauTiepNhan;

            data.MaNB = result?.BenhNhan?.MaBN;

            data.TenToiLa = !string.IsNullOrEmpty(hoTenBenhNhan) ? "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='values' style='text-transform: uppercase;'><b>" + hoTenBenhNhan + "</b></div></div>"
                : "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='value'><b>" + "&nbsp;" + "</b></div></div>";

            var ns = DateHelper.DOBFormat(result.NgaySinh, result.ThangSinh, result.NamSinh);

            data.NamSinh = !string.IsNullOrEmpty(ns) ? "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='values'><b>" + ns + "</b></div></div>"
                : "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='value'><b>" + ns + "</b></div></div>";

            data.GioiTinh = !string.IsNullOrEmpty(result.GioiTinh?.GetDescription()) ? "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='values'><b>" + result.GioiTinh?.GetDescription() + "</b></div></div>"
                : "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";


            data.CMND = !string.IsNullOrEmpty(infoBn.Cmnd) ? "<div class='container'><div class='label'>CMND/CCCD/Hộ chiếu:&nbsp;</div><div class='values'>" + infoBn.Cmnd + "</div></div>"
                : "<div class='container'><div class='label'>CMND/CCCD/Hộ chiếu:</div><div class='value'>" + "&nbsp;" + "</div></div>";

            data.CoQuanCap = !string.IsNullOrEmpty(giayCamKet.CoQuanCapCMND) ? "<div class='container'><div class='label'>Cơ quan cấp:&nbsp;</div><div class='values'>" + giayCamKet.CoQuanCapCMND + "</div></div>"
               : "<div class='container'><div class='label'>Cơ quan cấp:</div><div class='value'>" + "&nbsp;" + "</div></div>";

         

            data.DanToc = !string.IsNullOrEmpty(infoBn.DanToc) ? "<div class='container'><div class='label'>Dân tộc:&nbsp;</div><div class='values'>" + infoBn.DanToc + "</div></div>"
                : "<div class='container'><div class='label'>Dân tộc:</div><div class='value'>" + "&nbsp;" + "</div></div>";

            data.QuocTich = !string.IsNullOrEmpty(infoBn.QuocTich) ? "<div class='container'><div class='label'>Quốc tịch:&nbsp;</div><div class='values'>" + infoBn.QuocTich + "</div></div>"
                : "<div class='container'><div class='label'>Quốc tịch:</div><div class='value'>" + "&nbsp;" + "</div></div>";
            data.NgheNghiep = !string.IsNullOrEmpty(infoBn.NgheNghiep) ? "<div class='container'><div class='label'>Nghề nghiệp:&nbsp;</div><div class='values'>" + infoBn.NgheNghiep + "</div></div>"
              : "<div class='container'><div class='label'>Nghề nghiệp:</div><div class='value'>" + "&nbsp;" + "</div></div>";

            data.NoiLamViec = !string.IsNullOrEmpty(infoBn.NoiLamViec) ? "<div class='container'><div class='label'>Nơi làm việc:&nbsp;</div><div class='values'>" + infoBn.NoiLamViec + "</div></div>"
              : "<div class='container'><div class='label'>Nơi làm việc:</div><div class='value'>" + "&nbsp;" + "</div></div>";

            data.DiaChi = !string.IsNullOrEmpty(result.DiaChiDayDu) ? "<div class='container'><div class='label'></div><div class='values'>Địa chỉ:&nbsp;<b>" + result.DiaChiDayDu + "</b></div></div>"
                : "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";

            var nguoiThan = string.Empty;

            if(giayCamKet.TaoLaAi == 1)
            {
                nguoiThan = hoTenNguoiThan;
            }
            data.KhiCanBaoTin = !string.IsNullOrEmpty(nguoiThan) ? "<div class='container'><div class='label'>Khi cần báo tin:&nbsp;</div><div class='values'>" + nguoiThan + "</div></div>"
               : "<div class='container'><div class='label'>Khi cần báo tin:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";
            
            var nsNguoiThan = string.Empty;
            if(giayCamKet.NamSinh != null)
            {
                nsNguoiThan = giayCamKet.NamSinh.ToString();
            }
            data.NamSinhNguoiThan = !string.IsNullOrEmpty(nsNguoiThan) ? "<div class='container'><div class='label'>Năm sinh:&nbsp;</div><div class='values'>" + nsNguoiThan + "</div></div>"
               : "<div class='container'><div class='label'>Năm sinh:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";

            var dc = string.Empty;
            if (!string.IsNullOrEmpty(giayCamKet.DiaChi))
            {
                dc = giayCamKet.DiaChi.Replace("\n", "<br>");
            }

            data.DiaChiNguoiThan = !string.IsNullOrEmpty(giayCamKet.DiaChi) ? "<div class='container'><div class='label'></div><div class='values'> Địa chỉ:&nbsp;" + dc + "</div></div>"
              : "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";

            data.CMNDNguoiThan = !string.IsNullOrEmpty(giayCamKet.CMND) ? "<div class='container'><div class='label'>CMND/CCCD:&nbsp;</div><div class='values'>" + giayCamKet.CMND + "</div></div>"
                 : "<div class='container'><div class='label'>CMND/CCCD:</div><div class='value'>" + "&nbsp;" + "</div></div>";

            data.DTLienLac = !string.IsNullOrEmpty(giayCamKet.SDT) ? "<div class='container'><div class='label'>Điện thoại liên lạc:&nbsp;</div><div class='values'>" + giayCamKet.SDT + "</div></div>"
                : "<div class='container'><div class='label'>Điện thoại liên lạc::</div><div class='value'>" + "&nbsp;" + "</div></div>";



            data.NguoiThan = !string.IsNullOrEmpty(hoTenNguoiThan) ? "<div class='container'><div class='label'>Là đại diện gia đình người bệnh:&nbsp;</div><div class='values'><b>" + hoTenNguoiThan + "</b></div></div>"
                : "<div class='container'><div class='label'>Là đại diện gia đình người bệnh:&nbsp;</div><div class='value'>" + "&nbsp;" + "</div></div>";

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

            data.NguoiDaiDien = $"<b>{hoTenNguoiThan}</b>";

            var bsGayMe = string.Empty;
            if(giayCamKet.BSGMHSId != null)
            {
                bsGayMe = _useRepository.TableNoTracking.Where(d => d.Id == giayCamKet.BSGMHSId).Select(d => d.HoTen).FirstOrDefault();
            }
            data.BSGayMeHoiSuc = $"<b>{bsGayMe}</b>";

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
        private async Task<DataInPhieuDieuTriVaSerivcesGiayCamKetGayMeGiamDauTrongDeSauPhauThuatVo> ThongTinBenhNhanGiayCamKetGayMeGiamDauTrongDeSauPhauThuat(long yeuCauTiepNhanId)
        {
            var thongTinBenhNhanPhieuThuoc = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(s => s.Id == yeuCauTiepNhanId)
                .Select(s => new DataInPhieuDieuTriVaSerivcesGiayCamKetGayMeGiamDauTrongDeSauPhauThuatVo
                {
                    HoTenNgBenh = s.HoTen,
                    NamSinh = s.NamSinh,
                    NgaySinh = s.NgaySinh,
                    ThangSinh = s.ThangSinh,
                    GTNgBenh = s.GioiTinh.GetDescription(),
                    DiaChi = s.BenhNhan.DiaChiDayDu,
                    Cmnd = s.SoChungMinhThu,
                    MaSoTiepNhan = s.MaYeuCauTiepNhan,
                    QuocTich = s.QuocTichId != null ?s.QuocTich .Ten :"",
                    DanToc =s.DanTocId != null ? s.DanToc.Ten :"",
                    NgheNghiep =s.NgheNghiepId != null ? s.NgheNghiep.Ten:"",
                    NoiLamViec = s.NoiLamViec,
                });
            var thongTinBenhNhan = await thongTinBenhNhanPhieuThuoc.FirstAsync();
            return thongTinBenhNhan;
        }
    }
}
