using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System.Linq;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamDoanGoiKhamSucKhoeMappingProfile : Profile
    {
        public KhamDoanGoiKhamSucKhoeMappingProfile()
        {
            CreateMap<GoiKhamSucKhoe, GoiKhamSucKhoeViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Id;
                    d.TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten;
                    d.SoHopDong = s.HopDongKhamSucKhoe.SoHopDong;
                    d.NgayHieuLuc = s.HopDongKhamSucKhoe.NgayHieuLuc;
                    d.NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc;
                });
                     

            CreateMap<GoiKhamSucKhoeViewModel, GoiKhamSucKhoe>().IgnoreAllNonExisting()
                .ForMember(d => d.HopDongKhamSucKhoeNhanViens, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhs, o => o.Ignore())
                .ForMember(d => d.YeuCauDichVuKyThuats, o => o.Ignore())
                .ForMember(d => d.GoiKhamSucKhoeDichVuKhamBenhs, o => o.Ignore())
                .ForMember(d => d.GoiKhamSucKhoeDichVuDichVuKyThuats, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateGoiKhamSucKhoeDichVuKhamBenh(d, s);
                    AddOrUpdateGoiKhamSucKhoeDichVuDVKT(d, s);
                });

            CreateMap<GoiKhamSucKhoeDichVuKhamBenh, GoiKhamDichVuKhamSucKhoeDoanViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeNoiThucHiens, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.DichVuKyThuatBenhVienId = s.DichVuKhamBenhBenhVien?.Id;
                    d.TenDichVuKyThuatBenhVien = s.DichVuKhamBenhBenhVien?.Ten;
                    d.NoiThucHienString += string.Join("; ", s.GoiKhamSucKhoeNoiThucHiens.Select(p => p.PhongBenhVien.Ten));
                    d.LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien?.Ten;
                    d.NhomGiaDichVuKyThuatBenhVienId = s.NhomGiaDichVuKhamBenhBenhVien.Id;
                    d.Nhom = NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
                    d.LaDichVuKham = true;
                    d.HinhThucKhamBenh = s.GoiKhamSucKhoeNoiThucHiens.Any(p => p.PhongBenhVien.HopDongKhamSucKhoeId == null) ? HinhThucKhamBenh.NoiVien : HinhThucKhamBenh.KhamDoanNgoaiVien;
                    d.GoiKhamSucKhoeNoiThucHienIds.AddRange(s.GoiKhamSucKhoeNoiThucHiens.Select(p => p.PhongBenhVienId));
                    foreach (var item in d.GoiKhamSucKhoeNoiThucHienIds)
                    {
                        var goiKhamSucKhoeNoiThucHien = new GoiKhamSucKhoeNoiThucHienViewModel
                        {
                            PhongBenhVienId = item,
                            GoiKhamSucKhoeDichVuKhamBenhId = s.Id
                        };
                        d.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamSucKhoeNoiThucHien);
                    }
                });

            CreateMap<GoiKhamDichVuKhamSucKhoeDoanViewModel, GoiKhamSucKhoeDichVuKhamBenh>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeNoiThucHiens, o => o.Ignore())
                    .AfterMap((d, s) =>
                    {
                        AddOrUpdateGoiKhamSucKhoeDichVuKhamBenhNoiThucHien(d, s);
                    });


            CreateMap<GoiKhamSucKhoeDichVuDichVuKyThuat, GoiKhamDichVuKhamSucKhoeDoanViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeNoiThucHiens, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVien?.Id;
                    d.TenDichVuKyThuatBenhVien = s.DichVuKyThuatBenhVien?.Ten;
                    d.NoiThucHienString += string.Join("; ", s.GoiKhamSucKhoeNoiThucHiens.Select(p => p.PhongBenhVien.Ten));
                    d.LoaiGia = s.NhomGiaDichVuKyThuatBenhVien?.Ten;
                    d.NhomGiaDichVuKyThuatBenhVienId = s.NhomGiaDichVuKyThuatBenhVien.Id;
                    d.MaNhomDichVuBenhVien = s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma;
                    d.MaNhomDichVuBenhVienCha = s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma;
                    d.Nhom = (s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "XN" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "XN") ? NhomDichVuChiDinhKhamSucKhoe.XetNghiem :
                            ((s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "CĐHA" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "CĐHA") ? NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh :
                            (s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "TDCN" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "TDCN") ? NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang : NhomDichVuChiDinhKhamSucKhoe.KH);
                    d.HinhThucKhamBenh = s.GoiKhamSucKhoeNoiThucHiens.Any(p => p.PhongBenhVien.HopDongKhamSucKhoeId == null) ? HinhThucKhamBenh.NoiVien : HinhThucKhamBenh.KhamDoanNgoaiVien;
                    d.GoiKhamSucKhoeNoiThucHienIds.AddRange(s.GoiKhamSucKhoeNoiThucHiens.Select(p => p.PhongBenhVienId));
                    foreach (var item in d.GoiKhamSucKhoeNoiThucHienIds)
                    {
                        var goiKhamSucKhoeNoiThucHien = new GoiKhamSucKhoeNoiThucHienViewModel
                        {
                            PhongBenhVienId = item,
                            GoiKhamSucKhoeDichVuKhamBenhId = s.Id
                        };
                        d.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamSucKhoeNoiThucHien);
                    }
                });

            CreateMap<GoiKhamDichVuKhamSucKhoeDoanViewModel, GoiKhamSucKhoeDichVuDichVuKyThuat>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeNoiThucHiens, o => o.Ignore())
                 .AfterMap((d, s) =>
                 {
                     AddOrUpdateGoiKhamSucKhoeDichVuDVKTNoiThucHien(d, s);
                 });

            CreateMap<GoiKhamSucKhoeNoiThucHien, GoiKhamSucKhoeNoiThucHienViewModel>().IgnoreAllNonExisting();
            CreateMap<GoiKhamSucKhoeNoiThucHienViewModel, GoiKhamSucKhoeNoiThucHien>().IgnoreAllNonExisting();

            CreateMap<GoiKhamSucKhoeDoanVo, GoiKhamSucKhoeDoanExportExcel>().IgnoreAllNonExisting();


            //map gói chung qa gói dùng 
            CreateMap<GoiKhamSucKhoeChung, GoiKhamSucKhoeViewModel>().IgnoreAllNonExisting();
            CreateMap<GoiKhamSucKhoeChungDichVuKhamBenh, GoiKhamDichVuKhamSucKhoeDoanViewModel>().IgnoreAllNonExisting()
               .ForMember(d => d.GoiKhamSucKhoeNoiThucHiens, o => o.Ignore())
               .AfterMap((s, d) =>
               {
                   d.DichVuKyThuatBenhVienId = s.DichVuKhamBenhBenhVien?.Id;
                   d.TenDichVuKyThuatBenhVien = s.DichVuKhamBenhBenhVien?.Ten;
                   d.NoiThucHienString += string.Join("; ", s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVien.Ten));
                   d.LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien?.Ten;
                   d.NhomGiaDichVuKyThuatBenhVienId = s.NhomGiaDichVuKhamBenhBenhVien.Id;
                   d.Nhom = NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
                   d.LaDichVuKham = true;
                   d.HinhThucKhamBenh = s.GoiKhamSucKhoeChungNoiThucHiens.Any(p => p.PhongBenhVien.HopDongKhamSucKhoeId == null) ? HinhThucKhamBenh.NoiVien : HinhThucKhamBenh.KhamDoanNgoaiVien;
                   d.GoiKhamSucKhoeNoiThucHienIds.AddRange(s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVienId));
                   foreach (var item in d.GoiKhamSucKhoeNoiThucHienIds)
                   {
                       var goiKhamSucKhoeNoiThucHien = new GoiKhamSucKhoeNoiThucHienViewModel
                       {
                           PhongBenhVienId = item,
                           GoiKhamSucKhoeDichVuKhamBenhId = s.Id
                       };
                       d.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamSucKhoeNoiThucHien);
                   }
               });
            CreateMap<GoiKhamSucKhoeChungDichVuDichVuKyThuat, GoiKhamDichVuKhamSucKhoeDoanViewModel>().IgnoreAllNonExisting()
               .ForMember(d => d.GoiKhamSucKhoeNoiThucHiens, o => o.Ignore())
               .AfterMap((s, d) =>
               {
                   d.DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVien?.Id;
                   d.TenDichVuKyThuatBenhVien = s.DichVuKyThuatBenhVien?.Ten;
                   d.NoiThucHienString += string.Join("; ", s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVien.Ten));
                   d.LoaiGia = s.NhomGiaDichVuKyThuatBenhVien?.Ten;
                   d.NhomGiaDichVuKyThuatBenhVienId = s.NhomGiaDichVuKyThuatBenhVien.Id;
                   d.MaNhomDichVuBenhVien = s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma;
                   d.MaNhomDichVuBenhVienCha = s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma;
                   d.Nhom = (s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "XN" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "XN") ? NhomDichVuChiDinhKhamSucKhoe.XetNghiem :
                           ((s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "CĐHA" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "CĐHA") ? NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh :
                           (s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "TDCN" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "TDCN") ? NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang : NhomDichVuChiDinhKhamSucKhoe.KH);
                   d.HinhThucKhamBenh = s.GoiKhamSucKhoeChungNoiThucHiens.Any(p => p.PhongBenhVien.HopDongKhamSucKhoeId == null) ? HinhThucKhamBenh.NoiVien : HinhThucKhamBenh.KhamDoanNgoaiVien;
                   d.GoiKhamSucKhoeNoiThucHienIds.AddRange(s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVienId));
                   foreach (var item in d.GoiKhamSucKhoeNoiThucHienIds)
                   {
                       var goiKhamSucKhoeNoiThucHien = new GoiKhamSucKhoeNoiThucHienViewModel
                       {
                           PhongBenhVienId = item,
                           GoiKhamSucKhoeDichVuKhamBenhId = s.Id
                       };
                       d.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamSucKhoeNoiThucHien);
                   }
               });
            CreateMap<GoiKhamSucKhoeChungNoiThucHien, GoiKhamSucKhoeNoiThucHienViewModel>().IgnoreAllNonExisting();
            //Ket Luan Kham Doan
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, KetLuanKhamSucKhoeDoanViewModel>().IgnoreAllNonExisting();
            CreateMap<KetLuanKhamSucKhoeDoanViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting()
                 .ForMember(d => d.PhongBenhVienHangDois, o => o.Ignore())
                 .ForMember(d => d.YeuCauKhamBenhs, o => o.Ignore())
                 .ForMember(d => d.YeuCauDichVuKyThuats, o => o.Ignore())
                 .ForMember(d => d.YeuCauTiepNhanLichSuTrangThais, o => o.Ignore())
                 .ForMember(d => d.KetQuaSinhHieus, o => o.Ignore())
                 .ForMember(d => d.KetQuaNhomXetNghiems, o => o.Ignore())
                 .ForMember(d => d.YeuCauDuocPhamBenhViens, o => o.Ignore())
                 .ForMember(d => d.YeuCauVatTuBenhViens, o => o.Ignore())
                 .ForMember(d => d.YeuCauTiepNhanCongTyBaoHiemTuNhans, o => o.Ignore())
                 .ForMember(d => d.YeuCauDichVuGiuongBenhViens, o => o.Ignore())
                 .ForMember(d => d.DonThuocThanhToans, o => o.Ignore())
                 .ForMember(d => d.TaiKhoanBenhNhanThus, o => o.Ignore())
                 .ForMember(d => d.TaiKhoanBenhNhanChis, o => o.Ignore())
                 .ForMember(d => d.HoSoYeuCauTiepNhans, o => o.Ignore())
                 .ForMember(d => d.TheVoucherYeuCauTiepNhans, o => o.Ignore())
                 .ForMember(d => d.MienGiamChiPhis, o => o.Ignore())
                 .ForMember(d => d.DuyetBaoHiems, o => o.Ignore())
                 .ForMember(d => d.YeuCauTiepNhanLichSuKiemTraTheBHYTs, o => o.Ignore())
                 .ForMember(d => d.YeuCauTiepNhanLichSuKhamBHYT, o => o.Ignore())
                 .ForMember(d => d.TaiKhoanBenhNhanHuyDichVus, o => o.Ignore())
                 .ForMember(d => d.HoatDongGiuongBenhs, o => o.Ignore())
                 .ForMember(d => d.YeuCauTiepNhanDuLieuGuiCongBHYTs, o => o.Ignore())
                 .ForMember(d => d.TheoDoiSauPhauThuatThuThuats, o => o.Ignore())
                 .ForMember(d => d.DonVTYTThanhToans, o => o.Ignore())
                 .ForMember(d => d.PhienXetNghiems, o => o.Ignore())
                 .ForMember(d => d.NoiTruHoSoKhacs, o => o.Ignore())
                 .ForMember(d => d.YeuCauDichVuGiuongBenhVienChiPhis, o => o.Ignore())
                 .ForMember(d => d.YeuCauTruyenMaus, o => o.Ignore())
                 .ForMember(d => d.NoiTruChiDinhDuocPhams, o => o.Ignore())
                 .ForMember(d => d.YeuCauDichVuGiuongBenhVienChiPhiBHYTs, o => o.Ignore())
                 .ForMember(d => d.YeuCauDichVuGiuongBenhVienChiPhiBenhViens, o => o.Ignore())
                 .ForMember(d => d.YeuCauTiepNhanLichSuChuyenDoiTuongs, o => o.Ignore())
                 .ForMember(d => d.YeuCauTiepNhanTheBHYTs, o => o.Ignore())
                 .ForMember(d => d.TuVanThuocKhamSucKhoes, o => o.Ignore())
                 .AfterMap((d, s) =>
                 {
                   
                 });
        }

        #region Dịch vụ khám bệnh bệnh viện
        private void AddOrUpdateGoiKhamSucKhoeDichVuKhamBenh(GoiKhamSucKhoeViewModel viewModel, GoiKhamSucKhoe entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeDichVuKhamBenhs)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeDichVuKhamBenh();
                    var goiKhamSucKhoeDichVuKhamBenh = item.ToEntity(newEntity);
                    goiKhamSucKhoeDichVuKhamBenh.DichVuKhamBenhBenhVienId = item.DichVuKyThuatBenhVienId.Value;// đang dùng chung 1 biến  DichVuKyThuatBenhVienId
                    goiKhamSucKhoeDichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId;// đang dùng chung 1 biến  NhomGiaDichVuKyThuatBenhVienId
                    entity.GoiKhamSucKhoeDichVuKhamBenhs.Add(goiKhamSucKhoeDichVuKhamBenh);
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeDichVuKhamBenhs.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeDichVuKhamBenhs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeDichVuKhamBenhs.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
        private void AddOrUpdateGoiKhamSucKhoeDichVuKhamBenhNoiThucHien(GoiKhamDichVuKhamSucKhoeDoanViewModel viewModel, GoiKhamSucKhoeDichVuKhamBenh entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeNoiThucHiens)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeNoiThucHien();
                    entity.GoiKhamSucKhoeNoiThucHiens.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeNoiThucHiens.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeNoiThucHiens)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeNoiThucHiens.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
        #endregion

        #region Dịch vụ kỹ thuật bệnh viện
        private void AddOrUpdateGoiKhamSucKhoeDichVuDVKT(GoiKhamSucKhoeViewModel viewModel, GoiKhamSucKhoe entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeDichVuDichVuKyThuats)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeDichVuDichVuKyThuat();
                    entity.GoiKhamSucKhoeDichVuDichVuKyThuats.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeDichVuDichVuKyThuats.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeDichVuDichVuKyThuats)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeDichVuDichVuKyThuats.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }
                }
            }
        }

        private void AddOrUpdateGoiKhamSucKhoeDichVuDVKTNoiThucHien(GoiKhamDichVuKhamSucKhoeDoanViewModel viewModel, GoiKhamSucKhoeDichVuDichVuKyThuat entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeNoiThucHiens)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeNoiThucHien();
                    entity.GoiKhamSucKhoeNoiThucHiens.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeNoiThucHiens.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeNoiThucHiens)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeNoiThucHiens.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }
                }
            }
        }
        #endregion

    }
}
