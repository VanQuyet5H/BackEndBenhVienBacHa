using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet;
using Camino.Api.Models.YeuCauKhamBenhKhamBoPhanKhac;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhYeuCauKhamBenhMappingProfile : Profile
    {
        public KhamBenhYeuCauKhamBenhMappingProfile()
        {
            CreateMap<KhamBenhYeuCauKhamBenhViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauDichVuKyThuats, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhDonThuocs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhTrieuChungs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhChuanDoans, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhKhamBoPhanKhacs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhChanDoanPhanBiets, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhBoPhanTonThuongs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhICDKhacs, o => o.Ignore());
            //.AfterMap((d, s) =>
            //{
            //    AddOrUpdateKhamBoPhanKhac(d, s);
            //    AddOrUpdateChanDoanPhanBiet(d, s);
            //});

            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh, KhamBenhYeuCauKhamBenhViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauDichVuKyThuats, o => o.MapFrom(y => y.YeuCauDichVuKyThuats))
                .ForMember(x => x.YeuCauKhamBenhTrieuChungs, o => o.MapFrom(y => y.YeuCauKhamBenhTrieuChungs))
                .ForMember(x => x.YeuCauKhamBenhChuanDoans, o => o.MapFrom(y => y.YeuCauKhamBenhChuanDoans))
                .ForMember(x => x.YeuCauKhamBenhICDKhacs, o => o.MapFrom(y => y.YeuCauKhamBenhICDKhacs))
                .ForMember(x => x.YeuCauKhamBenhKhamBoPhanKhacs, o => o.MapFrom(y => y.YeuCauKhamBenhKhamBoPhanKhacs))
                .ForMember(x => x.YeuCauKhamBenhChanDoanPhanBiets, o => o.MapFrom(y => y.YeuCauKhamBenhChanDoanPhanBiets))
                .ForMember(x => x.YeuCauKhamBenhBoPhanTonThuongs, o => o.MapFrom(y => y.YeuCauKhamBenhBoPhanTonThuongs))
                .AfterMap((s, d) =>
                {
                    d.TenICDChinh = s.Icdchinh != null ? s.Icdchinh.Ma + " - " + s.Icdchinh.TenTiengViet : "";
                    d.TenDichVuDisplay = s.DichVuKhamBenhBenhVienId == 0 ? null : (s.MaDichVu ?? "") + " - " + (s.TenDichVu ?? "");
                    d.TenLoaiGia = s.NhomGiaDichVuKhamBenhBenhVien?.Ten;
                    //d.TenNoiDangKyThucHien = ((s.NoiDangKy != null && s.BacSiDangKy == null) ? s.NoiDangKy.Ma : (s.NoiDangKy != null && s.BacSiDangKy != null && s.BacSiDangKy.User != null) ? s.NoiDangKy.Ma + " - " + s.BacSiDangKy.User.HoTen : null);
                    d.TenNoiDangKyThucHien = s.NoiDangKy?.Ma + " - " + s.NoiDangKy?.Ten;
                    d.BacSiNoiDangKyId = s.NoiDangKyId?.ToString() + "," + (s.BacSiDangKyId == null ? 0.ToString() : s.BacSiDangKyId?.ToString());
                    d.TenChanDoanSoBoICD = s.ChanDoanSoBoICD != null ? s.ChanDoanSoBoICD.Ma + " - " + s.ChanDoanSoBoICD.TenTiengViet : "";
                    d.TenKhoaPhongNhapVien = s.KhoaPhongNhapVien?.Ten;
                    d.TenBenhVienChuyenVien = s.BenhVienChuyenVien?.Ten;
                    d.HoTenNhanVienHoTong = s.NhanVienHoTongChuyenVien?.User.HoTen
                    + (s.NhanVienHoTongChuyenVien?.ChucDanh != null ? " - " + s.NhanVienHoTongChuyenVien?.ChucDanh?.NhomChucDanh.Ten : null)
                                                    + (s.NhanVienHoTongChuyenVien?.VanBangChuyenMon != null ? " - " + s.NhanVienHoTongChuyenVien?.VanBangChuyenMon?.Ten : null);
                });

            #region Đơn thuốc
            CreateMap<KhamBenhYeuCauDonThuocViewModel, YeuCauKhamBenhDonThuoc>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenhDonThuoc, KhamBenhYeuCauDonThuocViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauKhamBenh, o => o.MapFrom(y => y.YeuCauKhamBenh))
                .ForMember(x => x.KhamBenhYeuCauDonThuocChiTiets,
                    o => o.MapFrom(y => y.YeuCauKhamBenhDonThuocChiTiets));


            CreateMap<KhamBenhYeuCauDonThuocChiTietViewModel, YeuCauKhamBenhDonThuocChiTiet>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenhDonThuocChiTiet, KhamBenhYeuCauDonThuocChiTietViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauKhamBenhDonThuoc, o => o.MapFrom(y => y.YeuCauKhamBenhDonThuoc))
                .ForMember(x => x.DuongDung, o => o.MapFrom(y => y.DuongDung))
                .ForMember(x => x.DuocPham, o => o.MapFrom(y => y.DuocPham))
                .ForMember(x => x.DonViTinh, o => o.MapFrom(y => y.DonViTinh))
                .ForMember(x => x.DuongDung, o => o.MapFrom(y => y.DuongDung));
            #endregion

            #region Vật tư
            CreateMap<KhamBenhYeuCauDonVTYTViewModel, YeuCauKhamBenhDonVTYT>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenhDonVTYT, KhamBenhYeuCauDonVTYTViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauKhamBenh, o => o.MapFrom(y => y.YeuCauKhamBenh))
                .ForMember(x => x.KhamBenhYeuCauDonVTYTChiTiets,
                    o => o.MapFrom(y => y.YeuCauKhamBenhDonVTYTChiTiets));

            CreateMap<KhamBenhYeuCauDonVTYTChiTietViewModel, YeuCauKhamBenhDonVTYTChiTiet>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenhDonVTYTChiTiet, KhamBenhYeuCauDonVTYTChiTietViewModel>().IgnoreAllNonExisting();
            #endregion


            #region Kết Luận
            CreateMap<KetLuanKhamBenhViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>().IgnoreAllNonExisting()
               .ForMember(d => d.YeuCauDichVuKyThuats, o => o.Ignore())
               .ForMember(d => d.YeuCauKhamBenhDonThuocs, o => o.Ignore())
               .ForMember(d => d.YeuCauKhamBenhTrieuChungs, o => o.Ignore())
               .ForMember(d => d.YeuCauKhamBenhChuanDoans, o => o.Ignore())
               .ForMember(d => d.YeuCauDichVuGiuongBenhViens, o => o.Ignore())
               .ForMember(d => d.InverseYeuCauKhamBenhTruocs, o => o.Ignore())
               .ForMember(d => d.YeuCauDuocPhamBenhViens, o => o.Ignore())
               .ForMember(d => d.YeuCauKhamBenhICDKhacs, o => o.Ignore())
               .ForMember(d => d.YeuCauKhamBenhLichSuTrangThais, o => o.Ignore())
               .ForMember(d => d.YeuCauVatTuBenhViens, o => o.Ignore())
               .ForMember(d => d.PhongBenhVienHangDois, o => o.Ignore())

               // bổ sung map fix trường hợp lưu đè dữ liệu được hưởng BH
               .ForMember(d => d.DuocHuongBaoHiem, o => o.Ignore())
               .AfterMap((d, s) =>
               {
                   AddOrUpdateICDsKhac(d, s);
               })
               ;

            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh, KetLuanKhamBenhViewModel>().IgnoreAllNonExisting()
               .ForMember(d => d.YeuCauKhamBenhICDKhacs, o => o.Ignore());

            CreateMap<YeuCauDichVuKyThuatViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting()
               .ForMember(d => d.KetQuaChuanDoanHinhAnhs, o => o.Ignore())
               .ForMember(d => d.KetQuaXetNghiems, o => o.Ignore())
               .ForMember(d => d.YeuCauDuocPhamBenhViens, o => o.Ignore())
               .ForMember(d => d.YeuCauVatTuBenhViens, o => o.Ignore())
               .ForMember(d => d.PhongBenhVienHangDois, o => o.Ignore())
               .ForMember(d => d.YeuCauDichVuGiuongBenhViens, o => o.Ignore())
               .ForMember(d => d.DuyetBaoHiemChiTiets, o => o.Ignore())
               .ForMember(d => d.TaiKhoanBenhNhanThus, o => o.Ignore())
               .ForMember(d => d.TaiKhoanBenhNhanChis, o => o.Ignore())
               .ForMember(d => d.CongTyBaoHiemTuNhanCongNos, o => o.Ignore())
               .ForMember(d => d.MienGiamChiPhis, o => o.Ignore())
               .ForMember(d => d.HoatDongGiuongBenhs, o => o.Ignore());

            CreateMap<YeuCauDichVuKyThuat, YeuCauDichVuKyThuatViewModel>().IgnoreAllNonExisting()
               .AfterMap((entity, viewModel) =>
               {
                   viewModel.TenDichVuHienThi = entity.DichVuKyThuatBenhVien?.Ma + " - " + entity.DichVuKyThuatBenhVien?.Ten;
               });
            #endregion


            #region Khám bộ phận khác
            CreateMap<YeuCauKhamBenhKhamBoPhanKhacViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac, YeuCauKhamBenhKhamBoPhanKhacViewModel>().IgnoreAllNonExisting();

            #endregion

            #region dùng cho hàm lưu 1 lần tại màn hình khám bệnh
            CreateMap<PhongBenhVienHangDoiKhamBenhViewModel, PhongBenhVienHangDoi>().IgnoreAllNonExisting();
            CreateMap<PhongBenhVienHangDoi, PhongBenhVienHangDoiKhamBenhViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauTiepNhan, o => o.MapFrom(y => y.YeuCauTiepNhan))
                .ForMember(x => x.YeuCauKhamBenh, o => o.MapFrom(y => y.YeuCauKhamBenh));

            CreateMap<YeuCauKhamBenhKhamBenhViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>().IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauDichVuKyThuats, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhDonThuocs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhTrieuChungs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhChuanDoans, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhKhamBoPhanKhacs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhChanDoanPhanBiets, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhICDKhacs, o => o.Ignore())
                .ForMember(d => d.YeuCauKhamBenhBoPhanTonThuongs, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateKhamBoPhanKhac(d, s);
                    AddOrUpdateChanDoanPhanBiet(d, s);
                    AddOrUpdateBoPhanTonThuong(d, s);
                    AddChanDoanICDKhacTheoChanDoanICDPhanBietFirst(d, s);
                });
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh, YeuCauKhamBenhKhamBenhViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauKhamBenhKhamBoPhanKhacs, o => o.MapFrom(y => y.YeuCauKhamBenhKhamBoPhanKhacs))
                .ForMember(x => x.YeuCauKhamBenhChanDoanPhanBiets, o => o.MapFrom(y => y.YeuCauKhamBenhChanDoanPhanBiets))
                .ForMember(x => x.YeuCauKhamBenhBoPhanTonThuongs, o => o.MapFrom(y => y.YeuCauKhamBenhBoPhanTonThuongs))
                .ForMember(x => x.YeuCauKhamBenhICDKhacs, o => o.MapFrom(y => y.YeuCauKhamBenhICDKhacs))
                .AfterMap((s, d) =>
                {
                    d.TenChanDoanSoBoICD = s.ChanDoanSoBoICD != null ? s.ChanDoanSoBoICD.Ma + " - " + s.ChanDoanSoBoICD.TenTiengViet : null;
                });

            CreateMap<YeuCauTiepNhanKhamBenhViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.KetQuaSinhHieus, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateKetQuaSinhHieu(s, d);
                    d.KSKKetLuanPhanLoaiSucKhoe = s.PhanLoaiSucKhoeId.GetDescription();
                });
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, YeuCauTiepNhanKhamBenhViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.KetQuaSinhHieus, o => o.MapFrom(y => y.KetQuaSinhHieus))
                .AfterMap((d, s) =>
                {
                    s.PhanLoaiSucKhoeId = string.IsNullOrEmpty(d.KSKKetLuanPhanLoaiSucKhoe) ? (Enums.PhanLoaiSucKhoe?)null : EnumHelper.GetValueFromDescription<Enums.PhanLoaiSucKhoe>(d.KSKKetLuanPhanLoaiSucKhoe);
                });
            #endregion


            #region dùng cho thêm chỉ định

            CreateMap<ChiDinhDichVuKhamBenhViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>()
                .IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.ThoiDiemChiDinh = DateTime.Now;
                    d.ThoiDiemDangKy = DateTime.Now;
                });

            #endregion

            #region Export excel

            CreateMap<KhamBenhDangKhamGridVo, KhamBenhDangKhamExportExcel>().IgnoreAllNonExisting();
            CreateMap<KhamBenhDangKhamTheoPhongKhamGridVo, KhamBenhDangKhamTheoPhongKhamExportExcel>().IgnoreAllNonExisting();

            #endregion
        }

        private void AddOrUpdateICDsKhac(KetLuanKhamBenhViewModel viewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh model)
        {
            foreach (var item in viewModel.YeuCauKhamBenhICDKhacs)
            {
                if (item.Id == 0)
                {
                    var icdPhuKhacEntity = new YeuCauKhamBenhICDKhac();
                    item.YeuCauKhamBenhId = viewModel.Id;
                    model.YeuCauKhamBenhICDKhacs.Add(item.ToEntity(icdPhuKhacEntity));
                }
                else
                {
                    var result = model.YeuCauKhamBenhICDKhacs.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.YeuCauKhamBenhICDKhacs)
            {
                if (item.Id != 0)
                {
                    //var countModel = viewModel.YeuCauKhamBenhICDKhacs.Where(x => x.Id == item.Id).ToList();
                    var countModel = viewModel.YeuCauKhamBenhICDKhacs.Any(x => x.Id == item.Id);

                    //if (countModel.Count == 0)
                    //{
                    //    item.WillDelete = true;
                    //}
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }

            }
        }

        private void AddOrUpdateKhamBoPhanKhac(YeuCauKhamBenhKhamBenhViewModel viewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh model)
        {
            foreach (var item in viewModel.YeuCauKhamBenhKhamBoPhanKhacs)
            {
                if (item.Id == 0)
                {
                    var khamBoPhanKhacEntity = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac();
                    model.YeuCauKhamBenhKhamBoPhanKhacs.Add(item.ToEntity(khamBoPhanKhacEntity));
                }
                else
                {
                    //var result = model.YeuCauKhamBenhKhamBoPhanKhacs.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);

                    var result = model.YeuCauKhamBenhKhamBoPhanKhacs.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.YeuCauKhamBenhKhamBoPhanKhacs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauKhamBenhKhamBoPhanKhacs.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddOrUpdateChanDoanPhanBiet(YeuCauKhamBenhKhamBenhViewModel viewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh model)
        {
            foreach (var item in viewModel.YeuCauKhamBenhChanDoanPhanBiets)
            {

                if (item.Id == 0)
                {
                    var chanDoanPhanBietEntity = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet();
                    model.YeuCauKhamBenhChanDoanPhanBiets.Add(item.ToEntity(chanDoanPhanBietEntity));
                }
                else
                {
                    //var result = model.YeuCauKhamBenhChanDoanPhanBiets.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);
                    var result = model.YeuCauKhamBenhChanDoanPhanBiets.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }

            }
            foreach (var item in model.YeuCauKhamBenhChanDoanPhanBiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauKhamBenhChanDoanPhanBiets.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddOrUpdateKetQuaSinhHieu(YeuCauTiepNhanKhamBenhViewModel viewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan model)
        {
            foreach (var item in viewModel.KetQuaSinhHieus)
            {
                if (item.Id == 0)
                {
                    var ketQuaSinhHieuEntity = new Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu();
                    ketQuaSinhHieuEntity.KSKPhanLoaiTheLuc = model.KSKPhanLoaiTheLuc != null ? (int?)model.KSKPhanLoaiTheLuc : null;
                    model.KetQuaSinhHieus.Add(item.ToEntity(ketQuaSinhHieuEntity));
                }
                else
                {
                    //var result = model.KetQuaSinhHieus.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);
                    var result = model.KetQuaSinhHieus.FirstOrDefault(p => p.Id == item.Id);                   
                    //result.KSKPhanLoaiTheLuc = model.KSKPhanLoaiTheLuc != null ? (int?)model.KSKPhanLoaiTheLuc : null;

                    if (result != null)
                    {
                        result.KSKPhanLoaiTheLuc = model.KSKPhanLoaiTheLuc != null ? (int?)model.KSKPhanLoaiTheLuc : null;
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.KetQuaSinhHieus)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.KetQuaSinhHieus.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddOrUpdateBoPhanTonThuong(YeuCauKhamBenhKhamBenhViewModel viewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh model)
        {
            foreach (var item in viewModel.YeuCauKhamBenhBoPhanTonThuongs)
            {
                if (item.Id == 0)
                {
                    var boPhanTonThuongEntity = new YeuCauKhamBenhBoPhanTonThuong();
                    model.YeuCauKhamBenhBoPhanTonThuongs.Add(item.ToEntity(boPhanTonThuongEntity));
                }
                else
                {
                    //var result = model.YeuCauKhamBenhBoPhanTonThuongs.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);
                    var result = model.YeuCauKhamBenhBoPhanTonThuongs.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.YeuCauKhamBenhBoPhanTonThuongs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauKhamBenhBoPhanTonThuongs.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddChanDoanICDKhacTheoChanDoanICDPhanBietFirst(YeuCauKhamBenhKhamBenhViewModel viewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh model)
        {
            if (!model.YeuCauKhamBenhICDKhacs.Any() && viewModel.YeuCauKhamBenhChanDoanPhanBiets.Any())
            {
                foreach (var item in viewModel.YeuCauKhamBenhChanDoanPhanBiets)
                {
                    if (item.Id == 0)
                    {
                        var iCDVM = new YeuCauKhamBenhICDKhacViewModel
                        {
                            YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                            ICDId = item.ICDId.Value,
                            GhiChu = item.GhiChu,
                            TenICD = item.TenICD
                        };
                        model.YeuCauKhamBenhICDKhacs.Add(iCDVM.ToEntity<YeuCauKhamBenhICDKhac>());
                    }
                }

            }
        }
    }
}
