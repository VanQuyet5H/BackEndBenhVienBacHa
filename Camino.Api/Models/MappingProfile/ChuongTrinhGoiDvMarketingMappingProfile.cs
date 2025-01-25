using System.Globalization;
using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GoiDichVu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuQuaTangs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.LoaiGoiDichVus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class ChuongTrinhGoiDvMarketingMappingProfile : Profile
    {
        public ChuongTrinhGoiDvMarketingMappingProfile()
        {
            CreateMap<ChuongTrinhGoiDvMarketingViewModel, ChuongTrinhGoiDichVu>()
                .ForMember(x => x.GoiDichVu, o => o.Ignore())
                .ForMember(x => x.GoiDichVuId, o => o.MapFrom(w => w.GoiDvId))
                .ForMember(x => x.ChuongTrinhGoiDichVuQuaTangs,
                    o => o.MapFrom(w => w.QuaTangKems))
                .AfterMap((s, d) =>
                {
                    d.TenGoiDichVu = s.GoiDichVu;

                });

            CreateMap<ChuongTrinhGoiDichVu, ChuongTrinhGoiDvMarketingViewModel>()
                .ForMember(x => x.QuaTangKems, o => o.MapFrom(w => w.ChuongTrinhGoiDichVuQuaTangs))
                .ForMember(x => x.KhuyenMaiKemsKhamBenh, o => o.MapFrom(w => w.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs))
                .ForMember(x => x.KhuyenMaiKemsGiuong, o => o.MapFrom(w => w.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs))
                .ForMember(x => x.KhuyenMaiKemsKyThuat, o => o.MapFrom(w => w.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats))
                .ForMember(x => x.GoiDichVu, o => o.MapFrom(w => w.TenGoiDichVu))
                .ForMember(x => x.GoiDvId, o => o.MapFrom(w => w.GoiDichVuId))
                 .AfterMap((s, d) =>
                 {
                     d.TenLoaiGoiDichVu = s.LoaiGoiDichVu?.Ten;
                 });

            CreateMap<QuaTangKemViewModel, ChuongTrinhGoiDichVuQuaTang>()
                .ForMember(x => x.Id, o => o.MapFrom(w => w.IdSys))
                .ForMember(x => x.QuaTang, o => o.Ignore())
                .ForMember(x => x.ChuongTrinhGoiDichVuId,
                    o => o.MapFrom(w => w.GoiDvChuongTrinhMarketingId));

            CreateMap<ChuongTrinhGoiDichVuQuaTang, QuaTangKemViewModel>()
                .ForMember(x => x.IdSys, o => o.MapFrom(w => w.Id))
                .ForMember(x => x.GoiDvChuongTrinhMarketingId,
                    o => o.MapFrom(w => w.ChuongTrinhGoiDichVuId))
                .ForMember(x => x.QuaTang,
                    o => o.MapFrom(w => w.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuQuaTangs.FirstOrDefault(e => e.QuaTangId == w.QuaTangId)
                        .QuaTang.Ten));

            CreateMap<KhuyenMaiKemViewModel, ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong>()
                .ForMember(x => x.Id, o => o.MapFrom(w => w.IdDatabase));

            CreateMap<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong, KhuyenMaiKemViewModel>()
                .AfterMap((s, d) =>
                {
                    d.DvId = s.DichVuGiuongBenhVienId;
                    d.IdDatabase = s.Id;
                    d.GoiDichVuId = s.ChuongTrinhGoiDichVuId;
                    d.MaDv = s.DichVuGiuongBenhVien.Ma;
                    d.TenDv = s.DichVuGiuongBenhVien.Ten;
                    d.LoaiGia = s.NhomGiaDichVuGiuongBenhVienId;
                    d.LoaiGiaDisplay = s.NhomGiaDichVuGiuongBenhVien.Ten;
                    d.SoLuong = s.SoLan;
                    d.Nhom = Enums.EnumDichVuTongHop.GiuongBenh;
                });

            CreateMap<KhuyenMaiKemViewModel, ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh>()
                .ForMember(x => x.Id, o => o.MapFrom(w => w.IdDatabase));

            CreateMap<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh, KhuyenMaiKemViewModel>()
                .AfterMap((s, d) =>
                {
                    d.DvId = s.DichVuKhamBenhBenhVienId;
                    d.IdDatabase = s.Id;
                    d.GoiDichVuId = s.ChuongTrinhGoiDichVuId;
                    d.MaDv = s.DichVuKhamBenhBenhVien.Ma;
                    d.TenDv = s.DichVuKhamBenhBenhVien.Ten;
                    d.LoaiGia = s.NhomGiaDichVuKhamBenhBenhVienId;
                    d.LoaiGiaDisplay = s.NhomGiaDichVuKhamBenhBenhVien.Ten;
                    d.SoLuong = s.SoLan;
                    d.Nhom = Enums.EnumDichVuTongHop.KhamBenh;
                });

            CreateMap<KhuyenMaiKemViewModel, ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat>()
                .ForMember(x => x.Id, o => o.MapFrom(w => w.IdDatabase));

            CreateMap<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat, KhuyenMaiKemViewModel>()
                .AfterMap((s, d) =>
                {
                    d.DvId = s.DichVuKyThuatBenhVienId;
                    d.IdDatabase = s.Id;
                    d.GoiDichVuId = s.ChuongTrinhGoiDichVuId;
                    d.MaDv = s.DichVuKyThuatBenhVien.Ma;
                    d.TenDv = s.DichVuKyThuatBenhVien.Ten;
                    d.LoaiGia = s.NhomGiaDichVuKyThuatBenhVienId;
                    d.LoaiGiaDisplay = s.NhomGiaDichVuKyThuatBenhVien.Ten;
                    d.SoLuong = s.SoLan;
                    d.Nhom = Enums.EnumDichVuTongHop.KyThuat;
                });

            CreateMap<ChuongTrinhGoiDvMarketingGridVo, ChuongTrinhGoiDvMarketingExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.GiaTruocChietKhau = s.GiaTruocChietKhau.ApplyFormatMoneyVND();
                    d.TiLeChietKhau = s.TiLeChietKhau.ToString(CultureInfo.InvariantCulture) + "%";
                    d.GiaSauChietKhau = s.GiaSauChietKhau.ApplyFormatMoneyVND();
                    d.TuNgay = s.TuNgayDisplay;
                    d.DenNgay = s.DenNgayDisplay;
                    d.TamNgung = s.TamNgung == true ? "Ngừng sử dụng" : "Đang sử dụng";
                });


            CreateMap<NhomDichVuViewModel, ChuongTrinhGoiDichVuDichVuKhamBenh>().AfterMap((s, d) =>
            {
                d.DichVuKhamBenhBenhVienId = s.DvId;
                d.NhomGiaDichVuKhamBenhBenhVienId = s.LoaiGia;
                d.DonGia = s.DonGiaBenhVien;
                d.DonGiaSauChietKhau = s.DonGiaSauChietKhau;
                d.DonGiaTruocChietKhau = s.DonGiaTruocChietKhau;
                d.SoLan = s.SoLuong;
            });
            CreateMap<NhomDichVuViewModel, ChuongTrinhGoiDichVuDichVuGiuong>().AfterMap((s, d) =>
            {
                d.DichVuGiuongBenhVienId = s.DvId;
                d.NhomGiaDichVuGiuongBenhVienId = s.LoaiGia;
                d.DonGia = s.DonGiaBenhVien;
                d.DonGiaSauChietKhau = s.DonGiaSauChietKhau;
                d.DonGiaTruocChietKhau = s.DonGiaTruocChietKhau;
                d.SoLan = s.SoLuong;
            });
            CreateMap<NhomDichVuViewModel, ChuongTrinhGoiDichVuDichVuKyThuat>().AfterMap((s, d) =>
            {
                d.DichVuKyThuatBenhVienId = s.DvId;
                d.NhomGiaDichVuKyThuatBenhVienId = s.LoaiGia;
                d.DonGia = s.DonGiaBenhVien;
                d.DonGiaSauChietKhau = s.DonGiaSauChietKhau;
                d.DonGiaTruocChietKhau = s.DonGiaTruocChietKhau;
                d.SoLan = s.SoLuong;
            });

            CreateMap<LoaiGoiDichVu, LoaiGoiDichVuModel>().IgnoreAllNonExisting();
            CreateMap<LoaiGoiDichVuModel, LoaiGoiDichVu>().IgnoreAllNonExisting();
        }
    }
}
