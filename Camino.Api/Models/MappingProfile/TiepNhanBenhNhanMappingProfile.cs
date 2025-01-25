using System;
using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class TiepNhanBenhNhanMappingProfile : Profile
    {
        public TiepNhanBenhNhanMappingProfile()
        {
            #region yeu cau kham benh -> update yeu cau tiep nhan

            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, TiepNhanBenhNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.BenhNhan, o => o.MapFrom(s => s.BenhNhan))
                .ForMember(d => d.GiayChuyenVien, o => o.MapFrom(s => s.GiayChuyenVien))
                .ForMember(d => d.YeuCauDichVuKyThuats, o => o.MapFrom(s => s.YeuCauDichVuKyThuats))
                .ForMember(d => d.BaoHiemTuNhans, o => o.MapFrom(s => s.YeuCauTiepNhanCongTyBaoHiemTuNhans))

                .ForMember(d => d.ThoiGianTiepNhan, o => o.MapFrom(s => s.ThoiDiemTiepNhan))

                //.ForMember(d => d.QuanHuyenId, o => o.MapFrom(s => s.QuanHuyenId))
                //.ForMember(d => d.PhuongXaId, o => o.MapFrom(s => s.PhuongXaId))
                //.ForMember(d => d.TinhThanhId, o => o.MapFrom(s => s.TinhThanhId))
                //.ForMember(d => d.LyDoKhamModelText, o => o.MapFrom(s => s.LyDoKhamBenh != null ? s.LyDoKhamBenh.Ten : "" ))
                //.ForMember(d => d.PhongKhamModelText, o => o.MapFrom(s => s.PhongKham != null && s.BacSiChiDinh != null && s.BacSiChiDinh.User != null
                //    ? s.PhongKham.Ma + " - " + s.BacSiChiDinh.User.HoTen : ""))
                //.ForMember(d => d.DoiTuongUuTienModelText, o => o.MapFrom(s => s.DoiTuongUuTienKhamChuaBenh != null ? s.DoiTuongUuTienKhamChuaBenh.Ten : ""))
                //.ForMember(d => d.PhongKhamVaBacSiId, o => o.MapFrom(s => s.PhongKhamId + "," + s.BacSiChiDinhId))

                .AfterMap((s, d) => AfterMapEntityToModelYeuCauTiepNhan(s, d));
            CreateMap<TiepNhanBenhNhanViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting()
                .ForMember(d => d.NgaySinh, o => o.MapFrom(s => s.NgayThangNamSinh != null ? s.NgayThangNamSinh.Value.Day : 0))
                .ForMember(d => d.ThangSinh, o => o.MapFrom(s => s.NgayThangNamSinh != null ? s.NgayThangNamSinh.Value.Month : 0))
                .ForMember(d => d.NamSinh, o => o.MapFrom(s => s.NgayThangNamSinh != null ? s.NgayThangNamSinh.Value.Year : s.NamSinh))
                .ForMember(d => d.HoSoYeuCauTiepNhans, o => o.Ignore())
                .ForMember(d => d.DoiTuongUuTienKhamChuaBenh, o => o.Ignore())
                .ForMember(d => d.ThoiDiemTiepNhan, o => o.MapFrom(s => s.ThoiGianTiepNhan))

                //DiaChi va HoTen format
                .ForMember(d => d.HoTen, o => o.MapFrom(s => !string.IsNullOrEmpty(s.HoTen) ? s.HoTen.ToUpper() : s.HoTen))
                .ForMember(d => d.DiaChi, o => o.MapFrom(s => !string.IsNullOrEmpty(s.DiaChi) ? s.DiaChi.ToUpperCaseTheFirstCharacter() : s.DiaChi))

                //.ForMember(d => d.BenhNhan, o => o.MapFrom(s => s.BenhNhan))
                //.ForMember(d => d.GiayChuyenVien, o => o.MapFrom(s => s.GiayChuyenVien))
                .AfterMap((s, d) => AddOrUpdate(s, d));

            CreateMap<Core.Domain.Entities.CongTyBaoHiemTuNhans.CongTyBaoHiemTuNhanCongNo, CongTyBaoHiemTuNhanCongNoViewModel>().IgnoreAllNonExisting();
            #endregion yeu cau kham benh -> update yeu cau tiep nhan

            #region BHTN tiep nhan benh nhan
            CreateMap<BaoHiemTuNhanViewModel, YeuCauTiepNhanCongTyBaoHiemTuNhan>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauTiepNhanId, o => o.MapFrom(s => s.YeuCauTiepNhanId))
                .ForMember(d => d.CongTyBaoHiemTuNhanId, o => o.MapFrom(s => s.BHTNCongTyBaoHiemId))
                .ForMember(d => d.MaSoThe, o => o.MapFrom(s => s.BHTNMaSoThe))
                .ForMember(d => d.NgayHieuLuc, o => o.MapFrom(s => s.BHTNNgayHieuLuc))
                .ForMember(d => d.NgayHetHan, o => o.MapFrom(s => s.BHTNNgayHetHan))
                .ForMember(d => d.SoDienThoai, o => o.MapFrom(s => s.BHTNSoDienThoai))
                .ForMember(d => d.DiaChi, o => o.MapFrom(s => s.BHTNDiaChi));
            CreateMap<YeuCauTiepNhanCongTyBaoHiemTuNhan, BaoHiemTuNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauTiepNhanId, o => o.MapFrom(s => s.YeuCauTiepNhanId))

                .ForMember(d => d.BHTNCongTyBaoHiemId, o => o.MapFrom(s => s.CongTyBaoHiemTuNhanId))
                .ForMember(d => d.CongTyDisplay, o => o.MapFrom(s => s.CongTyBaoHiemTuNhan.Ten))

                .ForMember(d => d.BHTNMaSoThe, o => o.MapFrom(s => s.MaSoThe))
                .ForMember(d => d.BHTNNgayHieuLuc, o => o.MapFrom(s => s.NgayHieuLuc))
                .ForMember(d => d.BHTNNgayHetHan, o => o.MapFrom(s => s.NgayHetHan))
                .ForMember(d => d.BHTNSoDienThoai, o => o.MapFrom(s => s.SoDienThoai))
                .ForMember(d => d.BHTNDiaChi, o => o.MapFrom(s => s.DiaChi));
            #endregion BHTN tiep nhan benh nhan

            #region cac yeu cau lien quan

            CreateMap<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh, YeuCauKhamBenhTiepNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NhomGiaDichVuKhamBenhBenhVien, o => o.MapFrom(s => s.NhomGiaDichVuKhamBenhBenhVien))
                .AfterMap((s, d) =>
                {
                    d.IsDichVuHuyThanhToan = s.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && s.TaiKhoanBenhNhanChis.Any();
                    d.TenNhanVienChiDinh = s.NhanVienChiDinh?.User?.HoTen;
                });

            CreateMap<YeuCauKhamBenhTiepNhanViewModel, Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>()
                .IgnoreAllNonExisting();

            CreateMap<YeuCauDichVuKyThuat, YeuCauDichVuKyThuatTiepNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NhomGiaDichVuKyThuatBenhVien, o => o.MapFrom(s => s.NhomGiaDichVuKyThuatBenhVien))
                .ForMember(d => d.TenNhanVienChiDinh, o => o.MapFrom(s => s.NhanVienChiDinh.User.HoTen))
                .AfterMap((s, d) =>
                {
                    d.IsDichVuHuyThanhToan = s.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && s.TaiKhoanBenhNhanChis.Any();
                    d.LaDichVuVacxin = s.YeuCauDichVuKyThuatKhamSangLocTiemChung != null;
                });
            CreateMap<YeuCauDichVuKyThuatTiepNhanViewModel, YeuCauDichVuKyThuat>()
                .IgnoreAllNonExisting();

            CreateMap<YeuCauVatTuBenhVien, YeuCauVatTuBenhVienTiepNhanViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<YeuCauVatTuBenhVienTiepNhanViewModel, YeuCauVatTuBenhVien>()
                .IgnoreAllNonExisting();

            CreateMap<YeuCauDuocPhamBenhVien, YeuCauDuocPhamBenhVienTiepNhanViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<YeuCauDuocPhamBenhVienTiepNhanViewModel, YeuCauDuocPhamBenhVien>()
                .IgnoreAllNonExisting();

            CreateMap<NhomGiaDichVuKhamBenhBenhVien, NhomGiaDichVuKhamBenhBenhVienViewModel>()
               .IgnoreAllNonExisting();
            CreateMap<NhomGiaDichVuKhamBenhBenhVienViewModel, NhomGiaDichVuKhamBenhBenhVien>()
               .IgnoreAllNonExisting();

            CreateMap<NhomGiaDichVuKyThuatBenhVien, NhomGiaDichVuKyThuatBenhVienViewModel>()
               .IgnoreAllNonExisting();
            CreateMap<NhomGiaDichVuKyThuatBenhVienViewModel, NhomGiaDichVuKyThuatBenhVien>()
               .IgnoreAllNonExisting();

            #endregion cac yeu cau lien quan

            #region ho so yeu cau tiep nhan

            CreateMap<HoSoYeuCauTiepNhanViewModel, HoSoYeuCauTiepNhan>()
                .IgnoreAllNonExisting();
            CreateMap<HoSoYeuCauTiepNhan, HoSoYeuCauTiepNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.LoaiDisplay, o => o.MapFrom(s => s.LoaiHoSoYeuCauTiepNhan.Ten));

            #endregion ho so yeu cau tiep nhan

            #region benh nhan

            CreateMap<BenhNhanTiepNhanBenhNhanViewModel, BenhNhan>()
                .ForMember(d => d.NgaySinh, o => o.MapFrom(s => s.NgayThangNamSinh != null ? (s.NgayThangNamSinh ?? new DateTime()).Day : 0))
                .ForMember(d => d.ThangSinh, o => o.MapFrom(s => s.NgayThangNamSinh != null ? (s.NgayThangNamSinh ?? new DateTime()).Month : 0))
                .ForMember(d => d.NamSinh, o => o.MapFrom(s => s.NgayThangNamSinh != null ? (s.NgayThangNamSinh ?? new DateTime()).Year : 0))
                .ForMember(d => d.BenhNhanCongTyBaoHiemTuNhans, o => o.Ignore())
                .IgnoreAllNonExisting();
            CreateMap<BenhNhan, BenhNhanTiepNhanBenhNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.DanTocModelText, o => o.MapFrom(s => s.DanToc != null ? s.DanToc.Ten : ""))
                .ForMember(d => d.QuocTichModelText, o => o.MapFrom(s => s.QuocTich != null ? s.QuocTich.QuocTich : ""))
                //.ForMember(d => d.QuocTichModelText, o => o.MapFrom(s => s.QuocTich != null ? s.QuocTich.QuocTich : ""))
                .ForMember(d => d.TinhThanhModelText, o => o.MapFrom(s => s.TinhThanh != null ? s.TinhThanh.Ma + " - " + s.TinhThanh.Ten : ""))
                .ForMember(d => d.QuanHuyenModelText, o => o.MapFrom(s => s.QuanHuyen != null ? s.QuanHuyen.Ma + " - " + s.QuanHuyen.Ten : ""))
                .ForMember(d => d.PhuongXaModelText, o => o.MapFrom(s => s.PhuongXa != null ? s.PhuongXa.Ma + " - " + s.PhuongXa.Ten : ""))
                .ForMember(d => d.NgheNghiepModelText, o => o.MapFrom(s => s.NgheNghiep != null ? s.NgheNghiep.Ten : ""))
                .ForMember(d => d.NguoiLienHeThanNhanModelText, o => o.MapFrom(s => s.NguoiLienHeQuanHeNhanThan != null ? s.NguoiLienHeQuanHeNhanThan.Ten : ""))
                .ForMember(d => d.BaoHiemTuNhans, o => o.MapFrom(s => s.BenhNhanCongTyBaoHiemTuNhans))

                .ForMember(d => d.BHYTNgayDu5NamDisplay, o => o.MapFrom(s => (s.BHYTNgayDu5Nam ?? DateTime.Now).ApplyFormatDate()))

                .AfterMap((s, d) => BenhNhanFunction(s, d))
                ;

            CreateMap<BaoHiemTuNhanViewModel, BenhNhanCongTyBaoHiemTuNhan>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.BenhNhanId, o => o.MapFrom(s => s.BenhNhanId))
                .ForMember(d => d.CongTyBaoHiemTuNhanId, o => o.MapFrom(s => s.BHTNCongTyBaoHiemId))
                .ForMember(d => d.MaSoThe, o => o.MapFrom(s => s.BHTNMaSoThe))
                .ForMember(d => d.NgayHieuLuc, o => o.MapFrom(s => s.BHTNNgayHieuLuc))
                .ForMember(d => d.NgayHetHan, o => o.MapFrom(s => s.BHTNNgayHetHan))
                .ForMember(d => d.SoDienThoai, o => o.MapFrom(s => s.BHTNSoDienThoai))
                .ForMember(d => d.DiaChi, o => o.MapFrom(s => s.BHTNDiaChi));

            CreateMap<BenhNhanCongTyBaoHiemTuNhan, BaoHiemTuNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.BenhNhanId, o => o.MapFrom(s => s.BenhNhanId))
                .ForMember(d => d.BHTNCongTyBaoHiemId, o => o.MapFrom(s => s.CongTyBaoHiemTuNhanId))
                .ForMember(d => d.BHTNMaSoThe, o => o.MapFrom(s => s.MaSoThe))
                .ForMember(d => d.BHTNNgayHieuLuc, o => o.MapFrom(s => s.NgayHieuLuc))
                .ForMember(d => d.BHTNNgayHetHan, o => o.MapFrom(s => s.NgayHetHan))
                .ForMember(d => d.BHTNSoDienThoai, o => o.MapFrom(s => s.SoDienThoai))
                .ForMember(d => d.BHTNDiaChi, o => o.MapFrom(s => s.DiaChi));

            #endregion benh nhan

            #region giay chuyen vien

            CreateMap<GiayChuyenVienBenhNhanViewModel, GiayChuyenVien>().IgnoreAllNonExisting()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<GiayChuyenVien, GiayChuyenVienBenhNhanViewModel>().IgnoreAllNonExisting();


            #endregion giay chuyen vien

            #region phong kham

            CreateMap<PhongNgoaiTruBenhNhanViewModel, Core.Domain.Entities.PhongBenhViens.PhongBenhVien>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.PhongBenhViens.PhongBenhVien, PhongNgoaiTruBenhNhanViewModel>().IgnoreAllNonExisting();

            #endregion phong kham

            #region ly do kham benh

            //CreateMap<LyDoKhamBenhBenhNhanViewModel, Core.Domain.Entities.LyDoKhamBenhs.LyDoKhamBenh>().IgnoreAllNonExisting();
            //CreateMap<Core.Domain.Entities.LyDoKhamBenhs.LyDoKhamBenh, LyDoKhamBenhBenhNhanViewModel>().IgnoreAllNonExisting();

            #endregion ly do kham benh

            #region doi tuong uu tien kham chua benh

            CreateMap<DoiTuongUuTienKhamChuaBenhBenhNhanViewModel, DoiTuongUuTienKhamChuaBenh>().IgnoreAllNonExisting();
            CreateMap<DoiTuongUuTienKhamChuaBenh, DoiTuongUuTienKhamChuaBenhBenhNhanViewModel>().IgnoreAllNonExisting();

            #endregion doi tuong uu tien kham chua benh

            #region don vi hanh chinh

            CreateMap<DonViHanhChinhBenhNhanViewModel, Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh, DonViHanhChinhBenhNhanViewModel>().IgnoreAllNonExisting();

            #endregion don vi hanh chinh

            #region Giay mien cung chi tra

            CreateMap<GiayMienCungChiTraViewModel, GiayMienCungChiTra>().IgnoreAllNonExisting()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<GiayMienCungChiTra, GiayMienCungChiTraViewModel>().IgnoreAllNonExisting();

            #endregion Giay mien cung chi tra

            #region tim kiem benh nhan

            CreateMap<BenhNhan, TimKiemBenhNhanGridVo>().IgnoreAllNonExisting()
                .ForMember(d => d.SoChungMinhThu, o => o.MapFrom(s => s.SoChungMinhThu))
                .ForMember(d => d.SoDienThoai, o => o.MapFrom(s => s.SoDienThoai))
                .ForMember(d => d.DiaChi, o => o.MapFrom(s => s.DiaChi))
                .ForMember(d => d.HoTen, o => o.MapFrom(s => s.HoTen))
                .ForMember(d => d.MaBHYT, o => o.MapFrom(s => s.BHYTMaSoThe))
                .ForMember(d => d.MaBN, o => o.MapFrom(s => s.MaBN))
                .ForMember(d => d.NgaySinh, o => o.Ignore())
                //.ForMember(d => d.NgaySinhDisplay, o => o.MapFrom(s => (new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1)).ApplyFormatDate()))
                .ForMember(d => d.GioiTinh, o => o.MapFrom(s => s.GioiTinh))
                .ForMember(d => d.GioiTinhDisplay, o => o.MapFrom(s => s.GioiTinh != null ? s.GioiTinh.GetDescription() : ""))
                .AfterMap((s, d) => TimKiemBenhNhanFunction(s, d));

            #endregion tim kiem benh nhan

            //CreateMap<BienVienViewModel, Core.Domain.Entities.BenhVien.BenhVien>()
            //    .ForMember(d => d.CapQuanLyBenhVien, o => o.Ignore())
            //    .ForMember(d => d.DonViHanhChinh, o => o.Ignore())
            //    .ForMember(d => d.LoaiBenhVien, o => o.Ignore());

        }


        #region private class
        private void AfterMapEntityToModelYeuCauTiepNhan(Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan s, TiepNhanBenhNhanViewModel d)
        {
            if (s.NgaySinh != null && s.ThangSinh != null && s.NamSinh != null && s.NgaySinh != 0 && s.ThangSinh != 0 && s.NamSinh != 0)
            {
                d.NgayThangNamSinh = new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1);
            }
            else
            {
                d.NgaySinh = null;
            }
        }

        private void TimKiemBenhNhanFunction(BenhNhan s, TimKiemBenhNhanGridVo d)
        {
            if (s.NgaySinh != null && s.ThangSinh != null && s.NamSinh != null && s.NgaySinh != 0 && s.ThangSinh != 0 && s.NamSinh != 0)
            {
                d.NgaySinh = new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1);
                d.NgaySinhDisplay = new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1).ApplyFormatDate();
            }
            else
            {
                d.NgaySinh = null;
                d.NgaySinhDisplay = "";
            }


            //if (s.PhuongXa != null && s.QuanHuyen != null && s.TinhThanh != null)
            //{
            //    d.DiaChi = s.DiaChi + ", " + s.PhuongXa.Ten + ", " + s.QuanHuyen.Ten + " ," + s.TinhThanh.Ten;
            //}
            //else if (s.PhuongXa != null && s.QuanHuyen != null)
            //{
            //    d.DiaChi = s.DiaChi + ", " + s.PhuongXa.Ten + ", " + s.QuanHuyen.Ten;
            //}
            //else if (s.QuanHuyen != null && s.TinhThanh != null)
            //{
            //    d.DiaChi = s.DiaChi + ", " + s.QuanHuyen.Ten + " ," + s.TinhThanh.Ten;
            //}
            //else if (s.PhuongXa != null && s.TinhThanh != null)
            //{
            //    d.DiaChi = s.DiaChi + ", " + s.PhuongXa.Ten + " ," + s.TinhThanh.Ten;
            //}
            //else if (s.PhuongXa != null)
            //{
            //    d.DiaChi = s.DiaChi + ", " + s.PhuongXa.Ten;
            //}
            //else if (s.QuanHuyen != null)
            //{
            //    d.DiaChi = s.DiaChi + ", " + s.QuanHuyen.Ten;
            //}
            //else if (s.TinhThanh != null)
            //{
            //    d.DiaChi = s.DiaChi + ", " + s.TinhThanh.Ten;
            //}
        }

        private void BenhNhanFunction(BenhNhan s, BenhNhanTiepNhanBenhNhanViewModel d)
        {
            if (s.NgaySinh != null && s.ThangSinh != null && s.NamSinh != null && s.NgaySinh != 0 && s.ThangSinh != 0 && s.NamSinh != 0)
            {
                d.NgayThangNamSinh = new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1);
                d.NgayThangNamSinhDisplay = new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1).ApplyFormatDate();
            }
            else
            {
                d.NgayThangNamSinh = null;
                d.NgayThangNamSinhDisplay = "";
            }
        }

        private void AddOrUpdateBenhNhan(BenhNhanTiepNhanBenhNhanViewModel s, BenhNhan d)
        {
            foreach (var model in d.BenhNhanCongTyBaoHiemTuNhans)
            {
                if (!s.BaoHiemTuNhans.Any(c => c.Id == model.Id))
                {
                    model.WillDelete = true;
                }
            }
            foreach (var model in s.BaoHiemTuNhans)
            {
                if (model.Id == 0)
                {
                    var newEntity = new BenhNhanCongTyBaoHiemTuNhan();
                    d.BenhNhanCongTyBaoHiemTuNhans.Add(model.ToEntity(newEntity));
                }
                else
                {
                    if (d.BenhNhanCongTyBaoHiemTuNhans.Any())
                    {
                        var result = d.BenhNhanCongTyBaoHiemTuNhans.Single(c => c.Id == model.Id);
                        model.BenhNhanId = d.Id;
                        result = model.ToEntity(result);

                    }
                }
            }
        }

        private void AddOrUpdate(TiepNhanBenhNhanViewModel s, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan d)
        {
            foreach (var model in d.HoSoYeuCauTiepNhans)
            {
                if (!s.HoSoYeuCauTiepNhans.Any(c => c.Id == model.Id))
                {
                    model.WillDelete = true;
                }
            }
            foreach (var model in s.HoSoYeuCauTiepNhans)
            {
                if (model.Id == 0)
                {
                    var newEntity = new HoSoYeuCauTiepNhan();
                    d.HoSoYeuCauTiepNhans.Add(model.ToEntity(newEntity));
                }
                else
                {
                    if (d.HoSoYeuCauTiepNhans.Any())
                    {
                        var result = d.HoSoYeuCauTiepNhans.Single(c => c.Id == model.Id);
                        model.YeuCauTiepNhanId = d.Id;
                        result = model.ToEntity(result);

                    }
                }
            }
        }

        #endregion private class

    }
}