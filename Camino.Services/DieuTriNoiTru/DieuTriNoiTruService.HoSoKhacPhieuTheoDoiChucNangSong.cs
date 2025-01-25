using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public NoiTruHoSoKhac GetThongTinHoSoKhacPhieuTheoDoiChucNangSong(long yeuCauTiepNhanId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.PhieuTheoDoiChucNangSong)
                                                            .Include(p => p.NoiTruHoSoKhacFileDinhKems)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .FirstOrDefault();
        }

        public async Task<string> InPhieuTheoDoiChucNangSong(long yeuCauTiepNhanId, bool isInFilePDF = true)
        {
            var today = DateTime.Now;

            var currentPhongBenhVien = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentKhoaPhong = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == currentPhongBenhVien)
                                                                           .Select(p => p.KhoaPhong)
                                                                           .FirstOrDefault();

            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals(isInFilePDF ? "PhieuTheoDoiChucNangSongPDF" : "PhieuTheoDoiChucNangSong"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.NoiTruBenhAn)
                                                                     .Include(p => p.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                                                                     .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                     .FirstOrDefaultAsync();

            var yeuCauDichVuGiuongBenhVien = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(p => p.ThoiDiemBatDauSuDung <= today &&
                                                                                                   (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))
                                                                                       .FirstOrDefault();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.PhieuTheoDoiChucNangSong)
                                                               .FirstOrDefault();

            if (noiTruHoSoKhac == null || noiTruHoSoKhac.ThongTinHoSo == null)
            {
                var defaultData = new
                {
                    Khoa = currentKhoaPhong.Ten,
                    SoVaoVien = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = yeuCauTiepNhan.HoTen,
                    Tuoi = yeuCauTiepNhan.NamSinh != null ? (today.Year - yeuCauTiepNhan.NamSinh.Value).ToString() : "",
                    GioiTinh = yeuCauTiepNhan.GioiTinh?.GetDescription(),
                    Buong = $"{yeuCauDichVuGiuongBenhVien?.GiuongBenh?.PhongBenhVien?.Ten}",
                    SoGiuong = $"{yeuCauDichVuGiuongBenhVien?.GiuongBenh?.Ten}",
                    //ChanDoan = thongTin.ChanDoan,
                    Ngay = today.Day,
                    Thang = today.Month,
                    Nam = today.Year
                };

                return TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, defaultData);
            }

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacPhieuTheoDoiChucNangSongVo>(noiTruHoSoKhac.ThongTinHoSo);

            var data = new HoSoKhacPhieuInTheoDoiChucNangSong
            {
                Khoa = currentKhoaPhong.Ten,
                SoVaoVien = yeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = yeuCauTiepNhan.HoTen,
                Tuoi = yeuCauTiepNhan.NamSinh != null ? (today.Year - yeuCauTiepNhan.NamSinh.Value).ToString() : "",
                GioiTinh = yeuCauTiepNhan.GioiTinh?.GetDescription(),
                Buong = $"{yeuCauDichVuGiuongBenhVien?.GiuongBenh?.PhongBenhVien?.Ten}",
                SoGiuong = $"{yeuCauDichVuGiuongBenhVien?.GiuongBenh?.Ten}",
                ChanDoan = thongTin.ChanDoan,
                Ngay = today.Day,
                Thang = today.Month,
                Nam = today.Year
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
    }
}
