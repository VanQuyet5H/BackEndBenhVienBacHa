using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhamDoans;

namespace Camino.Api.Models.MappingProfile
{
    public class HopDongKhamSucKhoeNhanVienMappingProfile : Profile
    {
        public HopDongKhamSucKhoeNhanVienMappingProfile()
        {
            CreateMap<HopDongKhamSucKhoeNhanVien, ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TinhTrangHonNhan = s.DaLapGiaDinh ? Enums.TinhTrangHonNhan.CoGiaDinh : Enums.TinhTrangHonNhan.ChuaCoGiaDinh;
                    d.TenGoiKhamSucKhoe = s.GoiKhamSucKhoe != null ? s.GoiKhamSucKhoe.Ten : null;
                    //MapDichVuTrongGoiKhiChuaBatDauKham(s, d);
                });
            CreateMap<ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel, HopDongKhamSucKhoeNhanVien>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    if (d.NgayThangNamSinh != null)
                    {
                        s.NamSinh = d.NgayThangNamSinh.Value.Year;
                        s.ThangSinh = d.NgayThangNamSinh.Value.Month;
                        s.NgaySinh = d.NgayThangNamSinh.Value.Day;
                    }

                    s.DaLapGiaDinh = d.TinhTrangHonNhan == Enums.TinhTrangHonNhan.CoGiaDinh;
                });
        }

        private void MapDichVuTrongGoiKhiChuaBatDauKham(HopDongKhamSucKhoeNhanVien model, ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel viewModel)
        {
            if (model.GoiKhamSucKhoe != null)
            {
                var lstDichVuTrongGoi = new List<TiepNhanDichVuChiDinhViewModel>();
                lstDichVuTrongGoi.AddRange(
                    model.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs
                        .Where(x => ((x.GioiTinhNam && model.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && model.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                    && ((x.CoMangThai && model.CoMangThai) || (x.KhongMangThai && !model.CoMangThai))
                                    && ((x.ChuaLapGiaDinh && !model.DaLapGiaDinh) || (x.DaLapGiaDinh && model.DaLapGiaDinh))
                                    && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (viewModel.Tuoi != null && ((x.SoTuoiTu == null || viewModel.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || viewModel.Tuoi <= x.SoTuoiDen)))))
                        .Select(item => new TiepNhanDichVuChiDinhViewModel()
                        {
                            LoaiDichVu = Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh,
                            DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                            Ten = item.DichVuKhamBenhBenhVien.Ten,
                            Ma = item.DichVuKhamBenhBenhVien.Ma,
                            DonGiaBenhVien = item.DonGiaBenhVien,
                            DonGiaMoi = item.DonGiaBenhVien,
                            DonGiaUuDai = item.DonGiaUuDai,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                            GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                            LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                            TenLoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                            NoiThucHienId = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVienId).FirstOrDefault(),
                            TenNoiThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                            SoLan = 1 // dịch vụ khám mặc định là 1 lần
                        })
                    );

                lstDichVuTrongGoi.AddRange(
                    model.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats
                        .Where(x => ((x.GioiTinhNam && model.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && model.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                    && ((x.CoMangThai && model.CoMangThai) || (x.KhongMangThai && !model.CoMangThai))
                                    && ((x.ChuaLapGiaDinh && !model.DaLapGiaDinh) || (x.DaLapGiaDinh && model.DaLapGiaDinh))
                                    && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (viewModel.Tuoi != null && ((x.SoTuoiTu == null || viewModel.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || viewModel.Tuoi <= x.SoTuoiDen)))))
                        .Select(item => new TiepNhanDichVuChiDinhViewModel()
                        {
                            LoaiDichVu = item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ma == "XN" ? Enums.NhomDichVuChiDinhKhamSucKhoe.XetNghiem : (item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ma == "CĐHA" ? Enums.NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh : Enums.NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang),
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            Ten = item.DichVuKyThuatBenhVien.Ten,
                            Ma = item.DichVuKyThuatBenhVien.Ma,
                            DonGiaBenhVien = item.DonGiaBenhVien,
                            DonGiaMoi = item.DonGiaBenhVien,
                            DonGiaUuDai = item.DonGiaUuDai,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                            GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                            LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVienId,
                            TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            NoiThucHienId = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVienId).FirstOrDefault(),
                            TenNoiThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                            SoLan = 1 //todo: cần cập nhật lại
                        })
                    );
                viewModel.DichVuChiDinhTrongGois.AddRange(lstDichVuTrongGoi.OrderBy(x => x.TenNhomDichVu).ToList());
            }
        }
    }
}
