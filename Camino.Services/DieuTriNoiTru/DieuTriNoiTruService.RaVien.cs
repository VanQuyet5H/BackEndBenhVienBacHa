using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Camino.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Services.Helpers;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {

        #region Lấy thông tin ra viện nội khoa và khoa nhi

        public RaVien GetRaVienNoiTruKhoaNoiKhoaNhi(long yeuCauTiepNhanId)
        {
            var raVienNoiTruKhoaNoiKhoaNhi = new RaVien();
            var noiTruBenhAn = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn).Include(cc => cc.NoiTruThoiGianDieuTriBenhAnSoSinhs)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).FirstOrDefault();
            if (noiTruBenhAn != null && !string.IsNullOrEmpty(noiTruBenhAn.ThongTinRaVien))
            {
                raVienNoiTruKhoaNoiKhoaNhi = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);
            }
            else
            {
                PrepareThongTinBenhAn(raVienNoiTruKhoaNoiKhoaNhi, noiTruBenhAn);
            }
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiSuaCuoiCung = _useRepository.GetById(currentUserId).HoTen;

            raVienNoiTruKhoaNoiKhoaNhi.TongSoNgayDieuTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(noiTruBenhAn);//((noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now) - noiTruBenhAn.ThoiDiemNhapVien).Days + 1;
            raVienNoiTruKhoaNoiKhoaNhi.NguoiSuaCuoiCung = raVienNoiTruKhoaNoiKhoaNhi == null || string.IsNullOrEmpty(raVienNoiTruKhoaNoiKhoaNhi.NguoiSuaCuoiCung) ?
                nguoiSuaCuoiCung : raVienNoiTruKhoaNoiKhoaNhi.NguoiSuaCuoiCung;

            raVienNoiTruKhoaNoiKhoaNhi.NgaySuaCuoiCungDislay = raVienNoiTruKhoaNoiKhoaNhi == null || string.IsNullOrEmpty(raVienNoiTruKhoaNoiKhoaNhi.NgaySuaCuoiCungDislay)
                ? (DateTime.Now).ApplyFormatDateTimeSACH() : raVienNoiTruKhoaNoiKhoaNhi.NgaySuaCuoiCungDislay;


            if (noiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh)
            {
                var thoiGianDieuTriSoSinhs = noiTruBenhAn.NoiTruThoiGianDieuTriBenhAnSoSinhs.ToList();
                raVienNoiTruKhoaNoiKhoaNhi.SoNgayDieuTriBenhAnSoSinh = thoiGianDieuTriSoSinhs.Any() ? NoiTruBenhAnHelper.SoNgayDieuTriBenhAnSoSinh(thoiGianDieuTriSoSinhs) : 0;
            }

            return raVienNoiTruKhoaNoiKhoaNhi;
        }

        public void LuuHoacCapNhatRaVienNoiTruKhoaNoiKhoaNhi(RaVien model)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == model.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                XuLyThongTinRaVien(model, noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin ra viện ngoại khoa thẩm mỹ

        public RaVien GetRaVienNoiTruNgoaiKhoaThamMy(long yeuCauTiepNhanId)
        {
            var raVienNoiTruNgoaiKhoaThamMy = new RaVien();
            var noiTruBenhAn = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).FirstOrDefault();
            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinRaVien != null)
            {
                raVienNoiTruNgoaiKhoaThamMy = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);
            }
            else
            {
                PrepareThongTinBenhAn(raVienNoiTruNgoaiKhoaThamMy, noiTruBenhAn);
            }


            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiSuaCuoiCung = _useRepository.GetById(currentUserId).HoTen;


            raVienNoiTruNgoaiKhoaThamMy.TongSoNgayDieuTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(noiTruBenhAn);//((noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now) - noiTruBenhAn.ThoiDiemNhapVien).Days + 1;
            raVienNoiTruNgoaiKhoaThamMy.NguoiSuaCuoiCung = raVienNoiTruNgoaiKhoaThamMy == null || string.IsNullOrEmpty(raVienNoiTruNgoaiKhoaThamMy.NguoiSuaCuoiCung) ?
                nguoiSuaCuoiCung : raVienNoiTruNgoaiKhoaThamMy.NguoiSuaCuoiCung;

            raVienNoiTruNgoaiKhoaThamMy.NgaySuaCuoiCungDislay = raVienNoiTruNgoaiKhoaThamMy == null || string.IsNullOrEmpty(raVienNoiTruNgoaiKhoaThamMy.NgaySuaCuoiCungDislay)
                ? (DateTime.Now).ApplyFormatDateTimeSACH() : raVienNoiTruNgoaiKhoaThamMy.NgaySuaCuoiCungDislay;

            return raVienNoiTruNgoaiKhoaThamMy;
        }

        public void LuuHoacCapNhatRaVienNoiKhoaThamMy(RaVien model)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == model.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                XuLyThongTinRaVien(model, noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin ra viện phụ khoa

        public RaVien GetRaVienNoiTruPhuKhoa(long yeuCauTiepNhanId)
        {
            var raVienNoiTruPhuKhoa = new RaVien();
            var noiTruBenhAn = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).FirstOrDefault();
            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinRaVien != null)
            {
                raVienNoiTruPhuKhoa = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);
            }
            else
            {
                PrepareThongTinBenhAn(raVienNoiTruPhuKhoa, noiTruBenhAn);
            }



            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiSuaCuoiCung = _useRepository.GetById(currentUserId).HoTen;

            raVienNoiTruPhuKhoa.TongSoNgayDieuTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(noiTruBenhAn);//((noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now) - noiTruBenhAn.ThoiDiemNhapVien).Days + 1;
            raVienNoiTruPhuKhoa.NguoiSuaCuoiCung = raVienNoiTruPhuKhoa == null || string.IsNullOrEmpty(raVienNoiTruPhuKhoa.NguoiSuaCuoiCung) ?
                nguoiSuaCuoiCung : raVienNoiTruPhuKhoa.NguoiSuaCuoiCung;

            raVienNoiTruPhuKhoa.NgaySuaCuoiCungDislay = raVienNoiTruPhuKhoa == null || string.IsNullOrEmpty(raVienNoiTruPhuKhoa.NgaySuaCuoiCungDislay)
                ? (DateTime.Now).ApplyFormatDateTimeSACH() : raVienNoiTruPhuKhoa.NgaySuaCuoiCungDislay;

            return raVienNoiTruPhuKhoa;
        }

        public void LuuHoacCapNhatRaVienNoiTruPhuKhoa(RaVien model)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == model.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                XuLyThongTinRaVien(model, noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin ra viện sản khoa mổ

        public RaVien GetRaVienNoiTruSanKhoaMo(long yeuCauTiepNhanId)
        {
            var raVienNoiTruSanKhoaMo = new RaVien();
            var noiTruBenhAn = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).FirstOrDefault();
            if (noiTruBenhAn != null && !string.IsNullOrEmpty(noiTruBenhAn.ThongTinRaVien))
            {
                raVienNoiTruSanKhoaMo = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);
                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
            else
            {
                PrepareThongTinBenhAn(raVienNoiTruSanKhoaMo, noiTruBenhAn);
            }

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiSuaCuoiCung = _useRepository.GetById(currentUserId).HoTen;

            raVienNoiTruSanKhoaMo.TongSoNgayDieuTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(noiTruBenhAn);//((noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now) - noiTruBenhAn.ThoiDiemNhapVien).Days + 1;
            raVienNoiTruSanKhoaMo.NguoiSuaCuoiCung = raVienNoiTruSanKhoaMo == null || string.IsNullOrEmpty(raVienNoiTruSanKhoaMo.NguoiSuaCuoiCung) ?
                nguoiSuaCuoiCung : raVienNoiTruSanKhoaMo.NguoiSuaCuoiCung;
            raVienNoiTruSanKhoaMo.NgaySuaCuoiCungDislay = raVienNoiTruSanKhoaMo == null || string.IsNullOrEmpty(raVienNoiTruSanKhoaMo.NgaySuaCuoiCungDislay)
                ? (DateTime.Now).ApplyFormatDateTimeSACH() : raVienNoiTruSanKhoaMo.NgaySuaCuoiCungDislay;

            return raVienNoiTruSanKhoaMo;
        }

        public void LuuHoacCapNhatRaVienNoiTruSanKhoaMo(RaVien model)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == model.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                XuLyThongTinRaVien(model, noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin ra viện sản khoa thường

        public RaVien GetRaVienNoiTruSanKhoaThuong(long yeuCauTiepNhanId)
        {
            var raVienNoiTruSanKhoaThuong = new RaVien();
            var noiTruBenhAn = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).FirstOrDefault();
            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinRaVien != null)
            {
                raVienNoiTruSanKhoaThuong = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);
            }
            else
            {
                PrepareThongTinBenhAn(raVienNoiTruSanKhoaThuong, noiTruBenhAn);
            }

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiSuaCuoiCung = _useRepository.GetById(currentUserId).HoTen;
            raVienNoiTruSanKhoaThuong.TongSoNgayDieuTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(noiTruBenhAn);//((noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now) - noiTruBenhAn.ThoiDiemNhapVien).Days + 1;

            raVienNoiTruSanKhoaThuong.NguoiSuaCuoiCung = raVienNoiTruSanKhoaThuong == null || string.IsNullOrEmpty(raVienNoiTruSanKhoaThuong.NguoiSuaCuoiCung) ?
                nguoiSuaCuoiCung : raVienNoiTruSanKhoaThuong.NguoiSuaCuoiCung;

            raVienNoiTruSanKhoaThuong.NgaySuaCuoiCungDislay = raVienNoiTruSanKhoaThuong == null || string.IsNullOrEmpty(raVienNoiTruSanKhoaThuong.NgaySuaCuoiCungDislay)
                ? (DateTime.Now).ApplyFormatDateTimeSACH() : raVienNoiTruSanKhoaThuong.NgaySuaCuoiCungDislay;

            return raVienNoiTruSanKhoaThuong;
        }

        public void LuuHoacCapNhatRaVienNoiTruSanKhoaThuong(RaVien model)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == model.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                XuLyThongTinRaVien(model, noiTruBenhAn);
            }
        }
        #endregion

        #region Lấy thông tin ra viện nội khoa và khoa nhi

        public RaVien GetRaVien(long yeuCauTiepNhanId)
        {
            var raVienNoiTruKhoaNoiKhoaNhi = new RaVien();
            var noiTruBenhAn = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn).FirstOrDefault();
            if (noiTruBenhAn != null && !string.IsNullOrEmpty(noiTruBenhAn.ThongTinRaVien))
            {
                raVienNoiTruKhoaNoiKhoaNhi = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);
            }
            else
            {
                PrepareThongTinBenhAn(raVienNoiTruKhoaNoiKhoaNhi, noiTruBenhAn);
            }

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiSuaCuoiCung = _useRepository.GetById(currentUserId).HoTen;
            raVienNoiTruKhoaNoiKhoaNhi.TongSoNgayDieuTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(noiTruBenhAn);//((noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now) - noiTruBenhAn.ThoiDiemNhapVien).Days + 1;

            raVienNoiTruKhoaNoiKhoaNhi.NguoiSuaCuoiCung = raVienNoiTruKhoaNoiKhoaNhi == null || string.IsNullOrEmpty(raVienNoiTruKhoaNoiKhoaNhi.NguoiSuaCuoiCung) ?
                nguoiSuaCuoiCung : raVienNoiTruKhoaNoiKhoaNhi.NguoiSuaCuoiCung;

            raVienNoiTruKhoaNoiKhoaNhi.NgaySuaCuoiCungDislay = raVienNoiTruKhoaNoiKhoaNhi == null || string.IsNullOrEmpty(raVienNoiTruKhoaNoiKhoaNhi.NgaySuaCuoiCungDislay)
                ? (DateTime.Now).ApplyFormatDateTimeSACH() : raVienNoiTruKhoaNoiKhoaNhi.NgaySuaCuoiCungDislay;

            return raVienNoiTruKhoaNoiKhoaNhi;
        }

        public void LuuHoacCapNhatRaVien(RaVien model)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == model.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                XuLyThongTinRaVien(model, noiTruBenhAn);
            }
        }

        #endregion

        #region Xử lý thông tin ra viện

        public void XuLyThongTinRaVien(RaVien model, Camino.Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn noiTruBenhAn)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiSuaCuoiCung = _useRepository.GetById(currentUserId).HoTen;
            model.NguoiSuaCuoiCung = nguoiSuaCuoiCung;
            model.NgaySuaCuoiCung = DateTime.Now;
            model.NgaySuaCuoiCungDislay = (DateTime.Now).ApplyFormatDateTimeSACH();
            model.NgayHienTaiKham = model.HenTaiKham != null ? model.NgayHienTaiKham : null;
            var modelBenhAnRaVien = JsonConvert.SerializeObject(model);
            noiTruBenhAn.ThongTinRaVien = modelBenhAnRaVien;
            if (noiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh)
            {
                noiTruBenhAn.SoNgayDieuTriBenhAnSoSinh = model.SoNgayDieuTriBenhAnSoSinh ?? 0M;

            }
            //cập nhật thêm chuẩn đoán phần ra viện
            noiTruBenhAn.ChanDoanChinhRaVienICDId = model.ChuanDoanRaVienId;
            noiTruBenhAn.ChanDoanChinhRaVienGhiChu = model.GhiChuChuanDoanRaVien;

            if (model.ChuanDoanKemTheos != null)
            {
                noiTruBenhAn.DanhSachChanDoanKemTheoRaVienICDId =
                 model.ChuanDoanKemTheos != null && model.ChuanDoanKemTheos.Count > 1
                     ? string.Join(Constants.ICDSeparator, model.ChuanDoanKemTheos.Select(o => o.ICD.ToString()))
                     : model.ChuanDoanKemTheos.FirstOrDefault()?.ICD.ToString();

                noiTruBenhAn.DanhSachChanDoanKemTheoRaVienGhiChu =
                    model.ChuanDoanKemTheos != null && model.ChuanDoanKemTheos.Count > 1
                        ? string.Join(Constants.ICDSeparator, model.ChuanDoanKemTheos.Select(o => o.ChuanDoan))
                        : model.ChuanDoanKemTheos.FirstOrDefault()?.ChuanDoan.ToString();
            }

            _noiTruBenhAnRepository.Update(noiTruBenhAn);
        }

        public void PrepareThongTinBenhAn(RaVien model, Camino.Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn noiTruBenhAn)
        {
            if (noiTruBenhAn.YeuCauTiepNhan != null)
            {
                var yeuCauKhamBenh = noiTruBenhAn.YeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh;
                if (yeuCauKhamBenh != null)
                {
                    model.ChuanDoanKKBCapCuuId = yeuCauKhamBenh.Icdchinh?.Id;
                    model.TenChuanDoanKKBCapCuu = yeuCauKhamBenh.Icdchinh?.Ma + " - " + yeuCauKhamBenh.Icdchinh?.TenTiengViet;
                    model.GhiChuChuanDoanKKBCapCuu = yeuCauKhamBenh.GhiChuICDChinh;

                    var thongTinBenhAn = noiTruBenhAn.ThongTinBenhAn != null ? JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn) : new ThongTinBenhAn();

                    model.NoiChuanDoanKhiVaoKhoaDieuTriId = thongTinBenhAn.ICDChinh == null ? yeuCauKhamBenh.Icdchinh?.Id : thongTinBenhAn.ICDChinh;
                    model.TenNoiChuanDoanKhiVaoKhoaDieuTri = thongTinBenhAn.ICDChinh == null ? yeuCauKhamBenh.Icdchinh?.TenTiengViet : thongTinBenhAn.TenICDChinh;
                    model.GhiChuNoiChuanDoanKhiVaoKhoaDieuTri = thongTinBenhAn.ICDChinh == null ? yeuCauKhamBenh.GhiChuICDChinh : thongTinBenhAn.ChuanDoan;

                    if (yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Any())
                    {
                        model.ChuanDoanKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => new ThongTinChuanDoanKemTheo
                        {
                            ICD = c.ICD.Id,
                            TenICD = c.ICD.Ma + " - " + c.ICD.TenTiengViet,
                            ChuanDoan = c.GhiChu,
                        }).ToList();
                    }
                }
            }
        }

        public void KetThucBenhAn(KetThucBenhAnVo ketThucBenhAnVo)
        {
            //đóng lý do là do khách hàng yêu cầu 31/03/2021
            //if (ketThucBenhAnVo.ThoiDiemRaVien > DateTime.Now)
            //    throw new Exception(_localizationService.GetResource("KetThucBenhAn.ThoiDiemRaVien.Wrong"));

            var yeuCauTiepNhan = BaseRepository.Table
                .Include(x => x.NoiTruBenhAn)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.NoiChiDinh).ThenInclude(gb => gb.KhoaPhong)
                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong)
                .First(o => o.Id == ketThucBenhAnVo.YeuCauTiepNhanId);

            if (yeuCauTiepNhan.NoiTruBenhAn.LoaiChuyenTuyen != null &&
                ketThucBenhAnVo.HinhThucRaVien != Enums.EnumHinhThucRaVien.ChuyenVien)
            {
                throw new Exception(_localizationService.GetResource("KetThucBenhAn.HinhThucRaVien.DaChuyenVien"));
            }
            yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien = ketThucBenhAnVo.ThoiDiemRaVien;
            //kiem tra cac phieu dieu tri
            //cap nhat thong tin cac dv giuong
            var chiPhiDichVuGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
            foreach (var yeuCauDichVuGiuongBenhVienChiPhiBenhVien in chiPhiDichVuGiuong.Item1)
            {
                yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(yeuCauDichVuGiuongBenhVienChiPhiBenhVien);
            }
            foreach (var yeuCauDichVuGiuongBenhVienChiPhiBHYT in chiPhiDichVuGiuong.Item2)
            {
                yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(yeuCauDichVuGiuongBenhVienChiPhiBHYT);
            }

            //cap nhat thong tin khoa phong dieu tri

            // cập nhật thêm chuyển viện update lại bênh viên muốn chuyển tới
            var thongTinRaVien = JsonConvert.DeserializeObject<RaVien>(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien);
            yeuCauTiepNhan.NoiTruBenhAn.ChuyenDenBenhVienId = thongTinRaVien.BenhVienId;

            yeuCauTiepNhan.NoiTruBenhAn.KetQuaDieuTri = ketThucBenhAnVo.KetQuaDieuTri;
            yeuCauTiepNhan.NoiTruBenhAn.HinhThucRaVien = ketThucBenhAnVo.HinhThucRaVien;
            yeuCauTiepNhan.NoiTruBenhAn.TinhTrangRaVien =
                GetTinhTrangRaVienTheoHinhThucRaVien(ketThucBenhAnVo.HinhThucRaVien);
            yeuCauTiepNhan.NoiTruBenhAn.NgayTaiKham = ketThucBenhAnVo.NgayHenTaiKham;
            yeuCauTiepNhan.NoiTruBenhAn.GhiChuTaiKham = ketThucBenhAnVo.GhiChuTaiKham;

            BaseRepository.Update(yeuCauTiepNhan);
        }

        private Enums.EnumTinhTrangRaVien GetTinhTrangRaVienTheoHinhThucRaVien(Enums.EnumHinhThucRaVien hinhThucRaVien)
        {
            switch (hinhThucRaVien)
            {
                case Enums.EnumHinhThucRaVien.ChuyenVien:
                    return Enums.EnumTinhTrangRaVien.ChuyenVien;
                case Enums.EnumHinhThucRaVien.BoVe:
                    return Enums.EnumTinhTrangRaVien.TronVien;
                case Enums.EnumHinhThucRaVien.XinVe:
                    return Enums.EnumTinhTrangRaVien.XinRaVien;
                case Enums.EnumHinhThucRaVien.NangXinVe:
                    return Enums.EnumTinhTrangRaVien.XinRaVien;
                default:
                    return Enums.EnumTinhTrangRaVien.RaVien;
            }
        }

        public void MoLaiBenhAn(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.Table
                .Include(x => x.NoiTruBenhAn)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(c => c.MienGiamChiPhis)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(c => c.CongTyBaoHiemTuNhanCongNos)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(c => c.DuyetBaoHiemChiTiets)
                .First(o => o.Id == yeuCauTiepNhanId);

            if (yeuCauTiepNhan.NoiTruBenhAn.DaQuyetToan == true)
            {
                throw new Exception(_localizationService.GetResource("MoLaiBenhAn.DaQuyetToan"));
            }

            yeuCauTiepNhan.NoiTruBenhAn.KetQuaDieuTri = null;
            yeuCauTiepNhan.NoiTruBenhAn.HinhThucRaVien = null;
            yeuCauTiepNhan.NoiTruBenhAn.TinhTrangRaVien = null;
            yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien = null;

            foreach (var yeuCauDichVuGiuongBenhVienChiPhiBenhVien in yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
            {
                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.WillDelete = true;
            }
            foreach (var yeuCauDichVuGiuongBenhVienChiPhiBHYT in yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
            {
                yeuCauDichVuGiuongBenhVienChiPhiBHYT.WillDelete = true;
            }

            BaseRepository.Update(yeuCauTiepNhan);
        }

        public async Task<bool> KiemTraNgayRaVien(long yeuCauTiepNhanId, DateTime? ngayRaVien)
        {
            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).Include(c => c.NoiTruBenhAn).FirstOrDefaultAsync();
            if (ngayRaVien != null && yeuCauTiepNhan?.NoiTruBenhAn?.ThoiDiemNhapVien > ngayRaVien)
            {
                return false;
            }
            return true;
        }

        public string KiemTraKhoaKhongChoRaVien(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).Include(c => c.NoiTruBenhAn)
                                                     .ThenInclude(c => c.NoiTruKhoaPhongDieuTris).FirstOrDefault();

            var cauHinh = _cauHinhService.GetSetting("CauHinhNoiTru.DanhSachKhoaKhongRaVien");
            var danhSachKhoaKhongRaViens = JsonConvert.DeserializeObject<List<DanhSachKhoaKhongRaVien>>(cauHinh.Value);

            if (yeuCauTiepNhan != null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any())
            {
                var benhNhanDangKhoaHienTai = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(c => c.ThoiDiemRaKhoa == null).FirstOrDefault();
                var khoaKoChoRaVien = danhSachKhoaKhongRaViens.Where(o => o.KhoaId == benhNhanDangKhoaHienTai.KhoaPhongChuyenDenId).FirstOrDefault();

                if (khoaKoChoRaVien != null)
                {
                    return $"Người bệnh không được ra viện tại {khoaKoChoRaVien.Ten}";
                }
            }
            // cập nhật 30/11/2022
            // 2 tap
            if (yeuCauTiepNhan != null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                return $"Bệnh nhân đã kết thúc bệnh án vui lòng tải lại trang.";
            }
           return string.Empty;
        }


        public List<KiemTraThongTinKetThucBenhAnError> KiemTraThongTinKetThucBenhAn(long yeuCauTiepNhanId)
        {
            var kiemTraThongTinKetThucBenhAnErrors = new List<KiemTraThongTinKetThucBenhAnError>();
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                .Include(c => c.NoiTruBenhAn).ThenInclude(c => c.NoiTruPhieuDieuTris)
                .Include(c => c.YeuCauDichVuKyThuats).ThenInclude(c => c.NoiTruPhieuDieuTri)
                .Include(c => c.YeuCauDichVuKyThuats).ThenInclude(c => c.DichVuKyThuatBenhVien)
                .Include(c => c.YeuCauDuocPhamBenhViens).ThenInclude(c => c.NoiTruPhieuDieuTri)
                .Include(c => c.YeuCauVatTuBenhViens).ThenInclude(c => c.NoiTruPhieuDieuTri)
                .Include(c => c.YeuCauTruyenMaus).ThenInclude(c => c.NoiTruPhieuDieuTri)
                .Include(c => c.YeuCauVatTuBenhViens).ThenInclude(c => c.YeuCauTraVatTuTuBenhNhanChiTiets)
                .Include(c => c.YeuCauDuocPhamBenhViens).ThenInclude(c => c.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .Include(c => c.YeuCauDichVuKyThuats).ThenInclude(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .Include(c => c.YeuCauDichVuKyThuats).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Include(c => c.YeuCauDuocPhamBenhViens).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Include(c => c.YeuCauVatTuBenhViens).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Include(c => c.YeuCauTruyenMaus).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Include(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(c => c.YeuCauDichVuKyThuats).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Include(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(c => c.YeuCauKhamBenhs).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Include(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(c => c.YeuCauDichVuKyThuats).ThenInclude(c => c.DichVuKyThuatBenhVien)
                .Include(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(c => c.YeuCauDichVuKyThuats).ThenInclude(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .Include(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(c => c.YeuCauDuocPhamBenhViens).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Include(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(c => c.YeuCauVatTuBenhViens).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .FirstOrDefault();




            if (yeuCauTiepNhan != null)
            {
                DateTime? thoiGianRaVien = null;
                if (!string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien))
                {
                    var thongTinRaVien = JsonConvert.DeserializeObject<RaVien>(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien);
                    if (thongTinRaVien != null)
                    {
                        thoiGianRaVien = thongTinRaVien.ThoiGianRaVien;
                    }
                    else
                    {
                        thoiGianRaVien = DateTime.Now;
                    }
                }
                else
                {
                    thoiGianRaVien = DateTime.Now;
                }

                var phieuDieuTriTruocNgayRaVien = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(o =>
                    o.NgayDieuTri <= thoiGianRaVien);
                var phieuDieuTriSauNgayRaVien = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(o =>
                    o.NgayDieuTri > thoiGianRaVien);
                //1. Đối với ngày điều trị <= Ngày ra viện
                //DVKT đã thực hiện đối với DV không có KQ lâu, DVKT đang thực hiện đối với DV có kết quả lâu
                //Thuốc, vật tư đã xuất(Đã có số)
                //Máu, xuất ăn đã thực hiện
                if (phieuDieuTriTruocNgayRaVien.Any())
                {
                    var dvktChuaThucHienXong = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(o => o.NoiTruPhieuDieuTri != null && phieuDieuTriTruocNgayRaVien.Any(p => p.NgayDieuTri == o.NoiTruPhieuDieuTri.NgayDieuTri) &&
                        !(o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien ||
                        o.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien == true ||
                        (o.DichVuKyThuatBenhVien?.DichVuCoKetQuaLau == true && o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)));
                    if (dvktChuaThucHienXong.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(dvktChuaThucHienXong.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Dịch vụ kỹ thuật",
                            TenDichVu = o.TenDichVu,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = o.DichVuKyThuatBenhVien?.DichVuCoKetQuaLau == true ||
                                      (o.DichVuKyThuatBenhVien?.DichVuCoKetQuaLau != true && o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien) ? "Chưa thực hiện" : "Chưa thực hiện xong"
                        }));
                    }
                    var thuocChuaXuatHoacChuaTaoPhieuLinh = yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(o => o.NoiTruPhieuDieuTri != null && phieuDieuTriTruocNgayRaVien.Any(p => p.NgayDieuTri == o.NoiTruPhieuDieuTri.NgayDieuTri) &&
                    o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy &&
                        o.XuatKhoDuocPhamChiTietId == null);
                    if (thuocChuaXuatHoacChuaTaoPhieuLinh.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(thuocChuaXuatHoacChuaTaoPhieuLinh.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Thuốc",
                            TenDichVu = o.Ten,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Chưa xuất thuốc"
                        }));
                    }
                    var vatTuChuaXuatHoacChuaTaoPhieuLinh = yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(o => o.NoiTruPhieuDieuTri != null && phieuDieuTriTruocNgayRaVien.Any(p => p.NgayDieuTri == o.NoiTruPhieuDieuTri.NgayDieuTri) &&
                    o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy &&
                        o.XuatKhoVatTuChiTietId == null);
                    if (vatTuChuaXuatHoacChuaTaoPhieuLinh.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(vatTuChuaXuatHoacChuaTaoPhieuLinh.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Vật tư",
                            TenDichVu = o.Ten,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Chưa xuất vật tư"
                        }));
                    }
                    var mauChuaXuat = yeuCauTiepNhan.YeuCauTruyenMaus.Where(o => o.NoiTruPhieuDieuTri != null && phieuDieuTriTruocNgayRaVien.Any(p => p.NgayDieuTri == o.NoiTruPhieuDieuTri.NgayDieuTri) &&
                        o.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy && o.XuatKhoMauChiTietId == null);
                    if (mauChuaXuat.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(mauChuaXuat.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Máu",
                            TenDichVu = o.TenDichVu,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Chưa xuất máu"
                        }));
                    }
                }
                //2. Đối với ngày điều trị > Ngày ra viện
                //DVKT: không có
                //Thuốc, vật tư: Không có hoặc Đã tạo phiếu trả hết số lượng
                //Máu, xuất ăn: không có
                if (phieuDieuTriSauNgayRaVien.Any())
                {
                    //Kiểm tra ko phát sinh dịch vụ đã hoàn thành trong tờ điều trị tương lai
                    var kiemTraDVKTTuongLai = phieuDieuTriSauNgayRaVien.SelectMany(c => c.YeuCauDichVuKyThuats.Where(o => o.NoiTruPhieuDieuTri != null && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy));
                    //Kiểm tra ko phát sinh truyền máu đã hoàn thành trong tờ điều trị tương lai
                    var kiemTraMauTuongLai = phieuDieuTriSauNgayRaVien.SelectMany(c => c.YeuCauTruyenMaus.Where(o => o.NoiTruPhieuDieuTri != null && o.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy));
                    //Kiểm tra ko phát sinh thuốc đã hoàn thành trong tờ điều trị tương lai
                    var kiemTraThuocTuongLai = phieuDieuTriSauNgayRaVien.SelectMany(c => c.YeuCauDuocPhamBenhViens
                        .Where(x => x.NoiTruPhieuDieuTri != null && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && x.SoLuong - x.YeuCauTraDuocPhamTuBenhNhanChiTiets.Sum(o => o.SoLuongTra) > 0));
                    //Kiểm tra ko phát sinh vật tư đã hoàn thành trong tờ điều trị tương lai
                    var kiemTraVatTuTuongLai = phieuDieuTriSauNgayRaVien.SelectMany(c => c.YeuCauVatTuBenhViens
                                            .Where(x => x.NoiTruPhieuDieuTri != null && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && x.SoLuong - x.YeuCauTraVatTuTuBenhNhanChiTiets.Sum(o => o.SoLuongTra) > 0));
                    if (kiemTraDVKTTuongLai.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(kiemTraDVKTTuongLai.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Dịch vụ kỹ thuật",
                            TenDichVu = o.TenDichVu,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "DVKT sau ngày ra viện"
                        }));
                    }
                    if (kiemTraThuocTuongLai.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(kiemTraThuocTuongLai.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Thuốc",
                            TenDichVu = o.Ten,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Thuốc sau ngày ra viện"
                        }));
                    }
                    if (kiemTraVatTuTuongLai.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(kiemTraVatTuTuongLai.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Vật tư",
                            TenDichVu = o.Ten,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Vật tư sau ngày ra viện"
                        }));
                    }
                    if (kiemTraMauTuongLai.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(kiemTraMauTuongLai.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = o.NoiTruPhieuDieuTri?.NgayDieuTri.ApplyFormatDate(),
                            Loai = "Máu",
                            TenDichVu = o.TenDichVu,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Máu sau ngày ra viện"
                        }));
                    }
                    //                    if (kiemTraDVKTTuongLai.Any() || kiemTraMauTuongLai.Any() || !kiemTraThuocTuongLai || !kiemTraVatTuTuongLai)
                    //                    {
                    //                        kiemTraThongTinKetThucBenhAnErrors.AddRange(phieuDieuTriSauNgayRaVien.Select(o => new KiemTraThongTinKetThucBenhAnError
                    //                        {
                    //                            NgayDieuTri = o.NgayDieuTri.ApplyFormatDate(),
                    //                            Loai = "Phiếu điều trị tương lai",
                    //                            TenDichVu = "Ngày điều trị " + o.NgayDieuTri.ApplyFormatDate(),
                    //                            NoiDung = "Có phát sinh chi phí"
                    //                        }));
                    //                    }
                    //else
                    //{
                    //    kiemTraThongTinKetThucBenhAnErrors.AddRange(phieuDieuTriTuongLai.Select(o => new KiemTraThongTinKetThucBenhAnError
                    //    {
                    //        NgayDieuTri = o.NgayDieuTri.ApplyFormatDate(),
                    //        Loai = "Phiếu điều trị",
                    //        TenDichVu = "Ngày điều trị " + o.NgayDieuTri.ApplyFormatDate(),
                    //        NoiDung = "Xóa phiếu điều trị tương lai"
                    //    }));
                    //}                  
                }

                var yeuCauTiepNhanNgoaiTru = yeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan;
                if (yeuCauTiepNhanNgoaiTru != null)
                {
                    var dvkhamChuaThucHienXong = yeuCauTiepNhanNgoaiTru.YeuCauKhamBenhs.Where(o =>
                         !(o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham || o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham));
                    if (dvkhamChuaThucHienXong.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(dvkhamChuaThucHienXong.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = "DV Chỉ Định Ngoại Trú",
                            Loai = "Dịch vụ khám",
                            TenDichVu = o.TenDichVu,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Chưa hoàn thành khám"
                        }));
                    }

                    var dvktChuaThucHienXong = yeuCauTiepNhanNgoaiTru.YeuCauDichVuKyThuats.Where(o =>
                           !(o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien ||
                           o.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien == true ||
                           (o.DichVuKyThuatBenhVien?.DichVuCoKetQuaLau == true && o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)));
                    if (dvktChuaThucHienXong.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(dvktChuaThucHienXong.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = "DV Chỉ Định Ngoại Trú",
                            Loai = "Dịch vụ kỹ thuật",
                            TenDichVu = o.TenDichVu,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = o.DichVuKyThuatBenhVien?.DichVuCoKetQuaLau == true ||
                                      (o.DichVuKyThuatBenhVien?.DichVuCoKetQuaLau != true && o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien) ? "Chưa thực hiện" : "Chưa thực hiện xong"
                        }));
                    }
                    var thuocChuaXuatHoacChuaTaoPhieuLinh = yeuCauTiepNhanNgoaiTru.YeuCauDuocPhamBenhViens.Where(o =>
                    o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy &&
                        o.XuatKhoDuocPhamChiTietId == null);
                    if (thuocChuaXuatHoacChuaTaoPhieuLinh.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(thuocChuaXuatHoacChuaTaoPhieuLinh.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = "DV Chỉ Định Ngoại Trú",
                            Loai = "Thuốc",
                            TenDichVu = o.Ten,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Chưa xuất thuốc"
                        }));
                    }
                    var vatTuChuaXuatHoacChuaTaoPhieuLinh = yeuCauTiepNhanNgoaiTru.YeuCauVatTuBenhViens.Where(o =>
                    o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy &&
                        o.XuatKhoVatTuChiTietId == null);
                    if (vatTuChuaXuatHoacChuaTaoPhieuLinh.Any())
                    {
                        kiemTraThongTinKetThucBenhAnErrors.AddRange(vatTuChuaXuatHoacChuaTaoPhieuLinh.Select(o => new KiemTraThongTinKetThucBenhAnError
                        {
                            NgayDieuTri = "DV Chỉ Định Ngoại Trú",
                            Loai = "Vật tư",
                            TenDichVu = o.Ten,
                            KhoaChiDinh = o.NoiChiDinh?.KhoaPhong?.Ten,
                            NoiDung = "Chưa xuất vật tư"
                        }));
                    }
                }

            }
            return kiemTraThongTinKetThucBenhAnErrors;
        }
        #endregion
    }
}
