using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Newtonsoft.Json;
using System.Linq;
using Camino.Core.Domain;
using System;

namespace Camino.Api.Models.MappingProfile
{
    public class TiemChungMappingProfile : Profile
    {
        public TiemChungMappingProfile()
        {
            #region YeuCauTiepNhan
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, TiemChungYeuCauTiepNhanViewModel>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    //BVHD-3941
                    s.CoBaoHiemTuNhan = d.CoBHTN;
                });
            CreateMap<TiemChungYeuCauTiepNhanViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting()
                .ForMember(x => x.KetQuaSinhHieus, o => o.Ignore())
                .ForMember(x => x.PhuongXa, o => o.Ignore())
                .ForMember(x => x.QuanHuyen, o => o.Ignore())
                .ForMember(x => x.TinhThanh, o => o.Ignore())
                .ForMember(x => x.DanToc, o => o.Ignore())
                .ForMember(x => x.NgheNghiep, o => o.Ignore())
                .ForMember(x => x.LyDoTiepNhan, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    s.HopDongKhamSucKhoeNhanVienId = d.HopDongKhamSucKhoeNhanVienId;
                    s.HopDongKhamSucKhoeId = d.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoeId;
                    //AddOrUpdateKetQuaSinhHieu(s, d);
                });

            CreateMap<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu, TiemChungKetQuaSinhHieuViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.BMI, o => o.MapFrom(p2 => p2.Bmi));
            CreateMap<TiemChungKetQuaSinhHieuViewModel, Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu>().IgnoreAllNonExisting()
                .ForMember(p => p.Bmi, o => o.MapFrom(p2 => p2.BMI));
            #endregion

            #region YeuCauDichVuKyThuatKhamSangLocTiemChung
            CreateMap<YeuCauDichVuKyThuatKhamSangLocTiemChung, TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienHoanThanhKhamSangLocDisplay, o => o.MapFrom(p2 => p2.NhanVienHoanThanhKhamSangLoc.User.HoTen))
                .ForMember(p => p.TiemChungTheoDoiSauTiem, o => o.MapFrom(p2 => p2))
                .ForMember(p => p.YeuCauDichVuKyThuats, o => o.MapFrom(p2 => p2.YeuCauDichVuKyThuats))
                .ForMember(p => p.NgayHenTiemMuiTiepTheo, o => o.MapFrom(p2 => p2.SoNgayHenTiemMuiTiepTheo != null ? DateTime.Now.AddDays(p2.SoNgayHenTiemMuiTiepTheo.Value) : (DateTime?)null));
                
            CreateMap<TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel, YeuCauDichVuKyThuatKhamSangLocTiemChung>().IgnoreAllNonExisting()
                .ForMember(x => x.KetQuaSinhHieus, o => o.Ignore())
                .ForMember(x => x.YeuCauDichVuKyThuats, o => o.Ignore())
                //.ForMember(p => p.SoNgayHenTiemMuiTiepTheo, o => o.MapFrom(p2 => p2.NgayHenTiemMuiTiepTheo != null ? (int)(p2.NgayHenTiemMuiTiepTheo.Value - DateTime.Now).TotalDays : (int?)null))
                .AfterMap((s, d) =>
                {
                    AddOrUpdateKetQuaSinhHieu(s, d);
                    //AddOrUpdateDichVuKyThuat(s, d);
                });

            CreateMap<YeuCauDichVuKyThuatKhamSangLocTiemChung, TiemChungTheoDoiSauTiemViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienTheoDoiSauTiemDisplay, o => o.MapFrom(p2 => p2.NhanVienTheoDoiSauTiem.User.HoTen))
                .ForMember(p => p.TiemChungThongTinPhanUngSauTiem, o => o.MapFrom(p2 => !string.IsNullOrEmpty(p2.ThongTinPhanUngSauTiem) ? JsonConvert.DeserializeObject<TiemChungThongTinPhanUngSauTiem>(p2.ThongTinPhanUngSauTiem) : null));
            CreateMap<TiemChungTheoDoiSauTiemViewModel, YeuCauDichVuKyThuatKhamSangLocTiemChung>().IgnoreAllNonExisting()
                .ForMember(p => p.ThongTinPhanUngSauTiem, o => o.MapFrom(p2 => p2.TiemChungThongTinPhanUngSauTiem != null ? JsonConvert.SerializeObject(p2.TiemChungThongTinPhanUngSauTiem) : null));
            #endregion

            #region YeuCauDichVuKyThuatTiemChung
            CreateMap<YeuCauDichVuKyThuatTiemChung, TiemChungYeuCauDichVuKyThuatTiemChungViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienTiemDisplay, o => o.MapFrom(p2 => p2.NhanVienTiem != null ? p2.NhanVienTiem.User.HoTen : string.Empty))
                .AfterMap((m, v) =>
                {
                    v.SoLoVacXinDisplay = m.XuatKhoDuocPhamChiTiet == null 
                        ? null 
                        : string.Join(", ",m.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.Solo?.Trim()).Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList());
                });
            CreateMap<TiemChungYeuCauDichVuKyThuatTiemChungViewModel, YeuCauDichVuKyThuatTiemChung>().IgnoreAllNonExisting();
            //CreateMap<YeuCauDichVuKyThuat, TiemChungYeuCauDichVuKyThuatTiemChungViewModel>().IgnoreAllNonExisting()
            //    .ForMember(p => p.DichVuKyThuatBenhVienDisplay, o => o.MapFrom(p2 => p2.DichVuKyThuatBenhVien.Ten))
            //    .ForMember(p => p.DuocPhamBenhVienId, o => o.MapFrom(p2 => p2.TiemChung.DuocPhamBenhVienId))
            //    .ForMember(p => p.MuiSo, o => o.MapFrom(p2 => p2.TiemChung.MuiSo))
            //    .ForMember(p => p.ViTriTiem, o => o.MapFrom(p2 => p2.TiemChung.ViTriTiem))
            //    .ForMember(p => p.SoLuong, o => o.MapFrom(p2 => p2.TiemChung.SoLuong))
            //    .ForMember(p => p.DonGia, o => o.MapFrom(p2 => p2.Gia))
            //    .ForMember(p => p.TrangThaiTiemChung, o => o.MapFrom(p2 => p2.TiemChung.TrangThaiTiemChung))
            //    .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten))
            //    .ForMember(p => p.NhanVienChiDinhDisplay, o => o.MapFrom(p2 => p2.NhanVienChiDinh.User.HoTen))
            //    .ForMember(p => p.NhanVienTiemId, o => o.MapFrom(p2 => p2.TiemChung.NhanVienTiemId))
            //    .ForMember(p => p.NhanVienTiemDisplay, o => o.MapFrom(p2 => p2.TiemChung.NhanVienTiem.User.HoTen))
            //    .ForMember(p => p.ThoiDiemTiem, o => o.MapFrom(p2 => p2.TiemChung.ThoiDiemTiem));

            CreateMap<YeuCauKhamTiemChungViewModel, YeuCauKhamTiemChungVo>().IgnoreAllNonExisting();
            CreateMap<TiemChungYeuCauDichVuKyThuatTiemChungViewModel, YeuCauDichVuKyThuatTiemChungVo>().IgnoreAllNonExisting();

            //BVHD-3825
            CreateMap<TiemChungMienGiamChiPhiViewModel, TiemChungMienGiamChiPhiVo>().IgnoreAllNonExisting();
            #endregion

            #region BenhNhan
            CreateMap<BenhNhan, TiemChungBenhNhanViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.BenhNhanTiemVacxinDiUngThuocs, o => o.MapFrom(y => y.BenhNhanDiUngThuocs))
                .ForMember(p => p.BenhNhanTiemVacxinTienSuBenhs, o => o.MapFrom(y => y.BenhNhanTienSuBenhs))
                .ForMember(p => p.YeuCauTiepNhans, o => o.MapFrom(y => y.YeuCauTiepNhans));

            CreateMap<TiemChungBenhNhanViewModel, BenhNhan>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhanDiUngThuocs, o => o.Ignore())
                .ForMember(x => x.BenhNhanTienSuBenhs, o => o.Ignore())
                .ForMember(x => x.YeuCauTiepNhans, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateTienSuBenh(s, d);
                    AddOrUpdateDiUngThuoc(s, d);
                });

            CreateMap<BenhNhanDiUngThuoc, TiemChungBenhNhanDiUngThuocViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.MapFrom(y => y.BenhNhan));
            CreateMap<TiemChungBenhNhanDiUngThuocViewModel, BenhNhanDiUngThuoc>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.Ignore());

            CreateMap<BenhNhanTienSuBenh, TiemChungBenhNhanTienSuBenhViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.MapFrom(y => y.BenhNhan));
            CreateMap<TiemChungBenhNhanTienSuBenhViewModel, BenhNhanTienSuBenh>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.Ignore());
            #endregion

            #region Thực hiện tiêm
            CreateMap<YeuCauDichVuKyThuatKhamSangLocTiemChung, ThucHienTiemKhamSangLocViewModel>().IgnoreAllNonExisting();
            CreateMap<ThucHienTiemKhamSangLocViewModel, YeuCauDichVuKyThuatKhamSangLocTiemChung>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauDichVuKyThuats, a => a.Ignore())
                .AfterMap((s, d) => { AddOrUpdateThucHienTiemVacxin(s, d); });

            CreateMap<YeuCauDichVuKyThuatTiemChung, ThucHienTiemTiemChungViewModel>().IgnoreAllNonExisting();
            CreateMap<ThucHienTiemTiemChungViewModel, YeuCauDichVuKyThuatTiemChung>().IgnoreAllNonExisting();
            #endregion
        }

        private void AddOrUpdateTienSuBenh(TiemChungBenhNhanViewModel viewModel, BenhNhan model)
        {
            foreach (var item in viewModel.BenhNhanTiemVacxinTienSuBenhs)
            {
                if (item.Id == 0)
                {
                    var tienSuBenhEntity = new BenhNhanTienSuBenh();
                    model.BenhNhanTienSuBenhs.Add(item.ToEntity(tienSuBenhEntity));
                }
                else
                {
                    //var result = model.BenhNhanTienSuBenhs.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);

                    var result = model.BenhNhanTienSuBenhs.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.BenhNhanTienSuBenhs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.BenhNhanTiemVacxinTienSuBenhs.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }
            }
        }

        private void AddOrUpdateDiUngThuoc(TiemChungBenhNhanViewModel viewModel, BenhNhan model)
        {
            foreach (var item in viewModel.BenhNhanTiemVacxinDiUngThuocs)
            {
                if (item.Id == 0)
                {
                    var diUngEntity = new BenhNhanDiUngThuoc();
                    model.BenhNhanDiUngThuocs.Add(item.ToEntity(diUngEntity));
                }
                else
                {
                    //var result = model.BenhNhanDiUngThuocs.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);

                    var result = model.BenhNhanDiUngThuocs.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.BenhNhanDiUngThuocs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.BenhNhanTiemVacxinDiUngThuocs.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }
            }
        }

        private void AddOrUpdateKetQuaSinhHieu(TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel viewModel, YeuCauDichVuKyThuatKhamSangLocTiemChung model)
        {
            foreach (var item in viewModel.KetQuaSinhHieus)
            {
                if (item.Id == 0)
                {
                    var ketQuaSinhHieuEntity = new Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu();
                    model.KetQuaSinhHieus.Add(item.ToEntity(ketQuaSinhHieuEntity));
                }
                else
                {
                    var result = model.KetQuaSinhHieus.FirstOrDefault(p => p.Id == item.Id);

                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.KetQuaSinhHieus)
            {
                if (item.Id != 0)
                {
                    var isExisted = viewModel.KetQuaSinhHieus.Any(x => x.Id == item.Id);

                    if (!isExisted)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        //private void AddOrUpdateDichVuKyThuat(TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel viewModel, YeuCauDichVuKyThuatKhamSangLocTiemChung model)
        //{
        //    foreach (var item in viewModel.YeuCauDichVuKyThuats)
        //    {
        //        if (item.Id == 0)
        //        {
        //            var yeuCauDichVuKyThuatEntity = new YeuCauDichVuKyThuat();
        //            item.ToEntity(yeuCauDichVuKyThuatEntity);
        //            //model.YeuCauDichVuKyThuat.YeuCauTiepNhan.YeuCauDichVuKyThuats.Add(yeuCauDichVuKyThuatEntity);
        //            //model.YeuCauDichVuKyThuats.Add(yeuCauDichVuKyThuatEntity);
        //        }
        //    }
        //    foreach (var item in model.YeuCauDichVuKyThuats)
        //    {
        //        if (item.Id != 0)
        //        {
        //            var isExisted = viewModel.YeuCauDichVuKyThuats.Any(x => x.Id == item.Id);

        //            if (!isExisted)
        //            {
        //                item.WillDelete = true;
        //                item.TiemChung.XuatKhoDuocPhamChiTiet.WillDelete = true;

        //                foreach(var xuatKhoViTri in item.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
        //                {
        //                    xuatKhoViTri.WillDelete = true;
        //                }
        //            }
        //        }
        //    }
        //}

        private void AddOrUpdateThucHienTiemVacxin(ThucHienTiemKhamSangLocViewModel viewModel, YeuCauDichVuKyThuatKhamSangLocTiemChung model)
        {
            // chỉ dùng để thực hiện lưu data thực hiện tiêm xuống
            // và chỉ áp dụng map cho các vacxin theo phòng đang tiêm
            foreach (var item in viewModel.YeuCauDichVuKyThuats)
            {
                if (item.Id == 0)
                {
                    var icdPhuKhacEntity = new YeuCauDichVuKyThuat();
                    model.YeuCauDichVuKyThuats.Add(item.ToEntity(icdPhuKhacEntity));
                }
                else
                {
                    var result = model.YeuCauDichVuKyThuats.Single(p => p.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            //foreach (var item in model.YeuCauDichVuKyThuats.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //{
            //    if (item.Id != 0)
            //    {
            //        var countModel = viewModel.YeuCauDichVuKyThuats.Any(x => x.Id == item.Id);

            //        if (!countModel)
            //        {
            //            item.WillDelete = true;
            //        }
            //    }

            //}
        }
    }
}
