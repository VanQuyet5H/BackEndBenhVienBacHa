using System;
using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KetQuaCDHATDCN;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Newtonsoft.Json;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauDichVuKyThuatMappingProfile : Profile
    {
        public YeuCauDichVuKyThuatMappingProfile()
        {
            CreateMap<KhamBenhYeuCauDichVuKyThuatViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting();
            CreateMap<YeuCauDichVuKyThuat, KhamBenhYeuCauDichVuKyThuatViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.KetQuaChuanDoanHinhAnhs, o => o.MapFrom(y => y.KetQuaChuanDoanHinhAnhs))
                .ForMember(x => x.KetQuaXetNghiems, o => o.MapFrom(y => y.KetQuaXetNghiems));

            CreateMap<KhamBenhChiDinhDichVuKyThuatMultiselectViewModel, ChiDinhDichVuKyThuatMultiselectVo>().IgnoreAllNonExisting();
            CreateMap<PhauThuatThuThuatChiDinhDichVuKyThuatMultiSelectViewModel, ChiDinhDichVuKyThuatMultiselectVo>().IgnoreAllNonExisting();

            #region Ket qua cdha-tdcn

            CreateMap<YeuCauDichVuKyThuat, KetQuaCDHATDCNViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.FileKetQuaCanLamSangs, o => o.MapFrom(y => y.FileKetQuaCanLamSangs))
                .AfterMap((s, d) =>
                {
                    d.ThongTinHanhChinhMaTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan;
                    d.ThongTinHanhChinhMaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN;
                    d.ThongTinHanhChinhHoTen = s.YeuCauTiepNhan.HoTen;
                    d.ThongTinHanhChinhNgaySinh = s.YeuCauTiepNhan.NgaySinh;
                    d.ThongTinHanhChinhThangSinh = s.YeuCauTiepNhan.ThangSinh;
                    d.ThongTinHanhChinhNamSinh = s.YeuCauTiepNhan.NamSinh;
                    d.ThongTinHanhChinhTenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription();
                    d.ThongTinHanhChinhDiaChi = s.YeuCauTiepNhan.DiaChiDayDu;
                    d.ThongTinHanhChinhDoiTuong = s.YeuCauTiepNhan.CoBHYT == true ? "BHYT" : "Viện phí";
                    d.ThongTinHanhChinhBacSiChiDinhId = s.NhanVienChiDinhId;
                    d.ThongTinHanhChinhBacSiChiDinh = s.NhanVienChiDinh.User.HoTen;
                    d.ThongTinHanhChinhNgayChiDinh = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH();
                    d.ThongTinHanhChinhNoiChiDinh = s.NoiChiDinh.Ten + (!string.IsNullOrEmpty(s.NoiChiDinh.Tang) ? "(" + s.NoiChiDinh.Tang + ")" : "");
                    d.ThongTinHanhChinhSoBenhAn = s.YeuCauTiepNhan.NoiTruBenhAn != null ? s.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : "";
                    d.ThongTinHanhChinhChanDoan = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.ChanDoanChinhICD?.TenTiengViet : s.YeuCauKhamBenh?.Icdchinh?.TenTiengViet;
                    d.ThongTinHanhChinhChiDinh = s.TenDichVu;
                    d.ChiTietKetQuaObj = string.IsNullOrEmpty(s.DataKetQuaCanLamSang) ? new ChiTietKetQuaCDHATDCNViewModel() : JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNViewModel>(s.DataKetQuaCanLamSang);
                    d.ChiTietKetQuaObj.InKemAnh = string.IsNullOrEmpty(s.DataKetQuaCanLamSang) || d.ChiTietKetQuaObj.InKemAnh;
                    d.DichVuCoInKetQuaKemHinhAnh = s.DichVuKyThuatBenhVien.CoInKetQuaKemHinhAnh == true;
                });

            CreateMap<KetQuaCDHATDCNViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting()
                .ForMember(x => x.FileKetQuaCanLamSangs, o => o.Ignore())
                .AfterMap((d, s) => { AddOrUpdateFileChuKy(d, s); });

            CreateMap<FileChuKyKetQuaCDHATDCNViewModel, FileKetQuaCanLamSang>().IgnoreAllNonExisting();
            CreateMap<FileKetQuaCanLamSang, FileChuKyKetQuaCDHATDCNViewModel>().IgnoreAllNonExisting();


            CreateMap<Camino.Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, KetQuaCDHATDCNViewModel>().IgnoreAllNonExisting()
               .AfterMap((s, d) =>
               {
                   d.ThongTinHanhChinhMaTiepNhan = s.MaYeuCauTiepNhan;
                   d.ThongTinHanhChinhMaBenhNhan = s.BenhNhan.MaBN;
                   d.ThongTinHanhChinhHoTen = s.HoTen;
                   d.ThongTinHanhChinhNgaySinh = s.NgaySinh;
                   d.ThongTinHanhChinhThangSinh = s.ThangSinh;
                   d.ThongTinHanhChinhNamSinh = s.NamSinh;
                   d.ThongTinHanhChinhTenGioiTinh = s.GioiTinh.GetDescription();
                   d.ThongTinHanhChinhDiaChi = s.DiaChiDayDu;
                   d.ThongTinHanhChinhDoiTuong = s.CoBHYT == true ? "BHYT" : "Viện phí";

                   // d.ThongTinHanhChinhBacSiChiDinhId = s.NhanVienChiDinhId;

                   d.ThongTinHanhChinhSoBenhAn = s.NoiTruBenhAn != null ? s.NoiTruBenhAn.SoBenhAn : "";
               });

            #endregion

            #region Cập nhật thông tin thực hiện dịch vụ kỹ thuật
            CreateMap<TrangThaiThucHienYeuCauDichVuKyThuatViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting();
            CreateMap<YeuCauDichVuKyThuat, TrangThaiThucHienYeuCauDichVuKyThuatViewModel>().IgnoreAllNonExisting();

            #endregion

            #region Tiêm chủng
            CreateMap<YeuCauDichVuKyThuat, YeuCauKhamTiemChungViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.DichVuKyThuatBenhVienDisplay, o => o.MapFrom(p2 => p2.DichVuKyThuatBenhVien.Ten))
                .ForMember(p => p.NhanVienChiDinhDisplay, o => o.MapFrom(p2 => p2.NhanVienChiDinh.User.HoTen))
                .ForMember(p => p.NoiChiDinhDisplay, o => o.MapFrom(p2 => p2.NoiChiDinh != null ? p2.NoiChiDinh.Ten : string.Empty))
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien != null ? p2.NhanVienThucHien.User.HoTen : string.Empty))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien != null ? p2.NoiThucHien.Ten : string.Empty))
                .ForMember(p => p.NhanVienTiemId, o => o.MapFrom(p2 => p2.TiemChung != null ? p2.TiemChung.NhanVienTiemId : null))
                .ForMember(p => p.NhanVienTiemDisplay, o => o.MapFrom(p2 => p2.TiemChung != null ? p2.TiemChung.NhanVienTiem.User.HoTen : null))
                .ForMember(p => p.IsDichVuHuyThanhToan, o => o.MapFrom(p2 => p2.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && p2.TaiKhoanBenhNhanChis.Any()))
                .ForMember(p => p.IsKhongTiemChung, o => o.MapFrom(p2 => p2.KhamSangLocTiemChung == null || !p2.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(o2 => o2.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)))
                .ForMember(p => p.MienGiamChiPhis, o => o.MapFrom(p2 => p2.MienGiamChiPhis))
                .AfterMap((s, d) =>
                    {
                        d.YeuCauGoiDichVuKhuyenMaiId =
                            s.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                ? s.MienGiamChiPhis.First(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null).YeuCauGoiDichVuId
                                : null;
                        d.TenGoiDichVu = (s.YeuCauGoiDichVu != null && s.YeuCauGoiDichVu.ChuongTrinhGoiDichVu != null)
                            ? ("Dịch vụ chọn từ gói: " + s.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + s.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu)
                            : (d.YeuCauGoiDichVuKhuyenMaiId != null
                                ? (s.MienGiamChiPhis
                                    .Where(a => a.DaHuy != true
                                                && a.YeuCauGoiDichVuId == d.YeuCauGoiDichVuKhuyenMaiId
                                                && a.YeuCauGoiDichVu != null
                                                && a.YeuCauGoiDichVu.ChuongTrinhGoiDichVu != null)
                                    .Select(a => "Dịch vụ chọn từ gói: " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).FirstOrDefault())
                                : null);
                    });
            CreateMap<YeuCauKhamTiemChungViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting()
                .ForMember(x => x.MienGiamChiPhis, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateMienGiamChiPhi(d, s);
                })
                ;

            CreateMap<ThucHienTiemVacxinViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting();
            CreateMap<YeuCauDichVuKyThuat, ThucHienTiemVacxinViewModel>().IgnoreAllNonExisting();

            CreateMap<TiemChungMienGiamChiPhiViewModel, MienGiamChiPhi>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauGoiDichVu, o => o.Ignore());
            CreateMap<MienGiamChiPhi, TiemChungMienGiamChiPhiViewModel>().IgnoreAllNonExisting();
            #endregion
        }

        private void AddOrUpdateFileChuKy(KetQuaCDHATDCNViewModel viewModel, YeuCauDichVuKyThuat model)
        {
            foreach (var item in viewModel.FileKetQuaCanLamSangs)
            {
                if (item == null)
                    continue;
                if (item.Id == 0)
                {
                    var chuKyEntity = new FileKetQuaCanLamSang();
                    item.ToEntity(chuKyEntity);
                    chuKyEntity.Ma = Guid.NewGuid().ToString();
                    model.FileKetQuaCanLamSangs.Add(chuKyEntity);
                }
                else
                {
                    var result = model.FileKetQuaCanLamSangs.Single(p => p.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in model.FileKetQuaCanLamSangs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.FileKetQuaCanLamSangs.Any(x => x != null && x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }

            }
        }

        #region BVHD-3825
        private void AddOrUpdateMienGiamChiPhi(YeuCauKhamTiemChungViewModel viewModel, YeuCauDichVuKyThuat model)
        {
            foreach (var item in viewModel.MienGiamChiPhis)
            {
                if (item.Id == 0)
                {
                    var mienGiamChiPhiEntity = new MienGiamChiPhi();
                    model.MienGiamChiPhis.Add(item.ToEntity(mienGiamChiPhiEntity));
                }
                else
                {
                    var result = model.MienGiamChiPhis.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.MienGiamChiPhis)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.MienGiamChiPhis.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }


        #endregion
    }
}
