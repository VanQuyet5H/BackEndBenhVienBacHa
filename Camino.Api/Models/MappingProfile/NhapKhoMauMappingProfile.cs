using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhapKhoMau;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.NhapKhoMaus;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Api.Models.MappingProfile
{
    public class NhapKhoMauMappingProfile : Profile
    {
        public NhapKhoMauMappingProfile()
        {
            #region Export Excel
            CreateMap<PhieuNhapKhoMauGridVo, NhapKhoMauExportExcel>().IgnoreAllNonExisting();
            CreateMap<PhieuNhapKhoMauGridChiTietVo, NhapKhoMauExportExcelChild>().IgnoreAllNonExisting();

            #endregion

            #region Thêm/xóa/sửa
            CreateMap<Core.Domain.Entities.MauVaChePhams.NhapKhoMau, PhieuNhapKhoMauViewModel>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.TrangThai = d.DuocKeToanDuyet == null ? Enums.TrangThaiNhapKhoMau.ChoNhapGia : Enums.TrangThaiNhapKhoMau.DaNhapGia;
                    s.TenNhaThau = d.NhaThau?.Ten;
                    s.TenNguoiGiao = d.NguoiGiao?.User.HoTen;
                });
            CreateMap<PhieuNhapKhoMauViewModel, Core.Domain.Entities.MauVaChePhams.NhapKhoMau>().IgnoreAllNonExisting()
                .ForMember(x => x.NhapKhoMauChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateNhapKhoMauChiTiet(s,d);
                });

            CreateMap<NhapKhoMauChiTiet, PhieuNhapKhoMauChiTietViewModel>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.TenMauVaChePham = d.MauVaChePham.Ten;
                    s.TenBenhNhanTruyenMau = d.YeuCauTruyenMau.YeuCauTiepNhan.HoTen;
                    s.NguoiLamXetNghiemHoaHopId = d.NguoiLamXetNghiemHoaHopId;
                    s.NguoiLamXetNghiemHoaHop = d.NguoiLamXetNghiemHoaHop;
                    s.ThongTinYeuCauTruyenMau = new LookupItemYeuCauTruyenMauVo()
                    {
                        KeyId = d.YeuCauTruyenMauId,
                        DisplayName = d.YeuCauTruyenMau.YeuCauTiepNhan.HoTen,
                        MaBenhAn = d.YeuCauTruyenMau.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                        MaYeuCauTiepNhan = d.YeuCauTruyenMau.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = d.YeuCauTruyenMau.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = d.YeuCauTruyenMau.YeuCauTiepNhan.HoTen,
                        MaChePhamMau = d.MaDichVu,
                        TenChePhamMau = d.TenDichVu,
                        NhomMau = d.YeuCauTruyenMau.NhomMau != null ? "\"" + d.YeuCauTruyenMau.NhomMau.GetDescription() + "\"" + (d.YeuCauTruyenMau.YeuToRh != null ? " Rh(" + d.YeuCauTruyenMau.YeuToRh.GetDescription() + ")" : "") : null,
                            //d.NhomMau.GetDescription() + d.YeuToRh.GetDescription(),
                        PhanLoaiMau = (int)d.PhanLoaiMau,
                        TheTich = d.TheTich,
                        TheTichDisplay = d.TheTich + "ml"
                    };

                    s.KetQuaXetNghiemKhacs = string.IsNullOrEmpty(d.KetQuaXetNghiemKhac) ? new List<KetQuaXetNghiemKhac>() : JsonConvert.DeserializeObject<List<KetQuaXetNghiemKhac>>(d.KetQuaXetNghiemKhac) ; 
                });
            CreateMap<PhieuNhapKhoMauChiTietViewModel, NhapKhoMauChiTiet>().IgnoreAllNonExisting()
                .ForMember(x => x.NhapKhoMau, o => o.Ignore())
                .AfterMap((s, d) =>
                {                    
                    d.KetQuaXetNghiemKhac = JsonConvert.SerializeObject(s.KetQuaXetNghiemKhacs);
                });
            #endregion

            #region Duyệt
            CreateMap<Core.Domain.Entities.MauVaChePhams.NhapKhoMau, DuyetPhieuNhapKhoMauViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.DuyetNhapKhoMauChiTiets, o => o.MapFrom(y => y.NhapKhoMauChiTiets))
                .AfterMap((d, s) =>
                {
                    s.TrangThai = d.DuocKeToanDuyet == null ? Enums.TrangThaiNhapKhoMau.ChoNhapGia : Enums.TrangThaiNhapKhoMau.DaNhapGia;
                    s.TenNhaThau = d.NhaThau?.Ten;
                    s.TenNguoiGiao = d.NguoiGiao?.User.HoTen;
                });
            CreateMap<DuyetPhieuNhapKhoMauViewModel, Core.Domain.Entities.MauVaChePhams.NhapKhoMau>().IgnoreAllNonExisting()
                .ForMember(x => x.NhapKhoMauChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateDuyetNhapKhoMauChiTiet(s, d);
                });

            CreateMap<NhapKhoMauChiTiet, DuyetPhieuNhapKhoMauChiTietViewModel>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.TenMauVaChePham = d.MauVaChePham.Ten;
                    s.IsThanhToan = d.YeuCauTruyenMau.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan;
                });
            CreateMap<DuyetPhieuNhapKhoMauChiTietViewModel, NhapKhoMauChiTiet>().IgnoreAllNonExisting()
                .ForMember(x => x.NhapKhoMau, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.DonGiaBan != d.DonGiaNhap)
                    {
                        d.DonGiaBan = d.DonGiaNhap;
                    }
                });
            #endregion
        }

        private void AddOrUpdateNhapKhoMauChiTiet(PhieuNhapKhoMauViewModel viewModel, Core.Domain.Entities.MauVaChePhams.NhapKhoMau model)
        {
            foreach (var item in viewModel.NhapKhoMauChiTiets)
            {
                if (item.Id == 0)
                {
                    var nhapKhoMauChiTietEntity = new NhapKhoMauChiTiet();
                    model.NhapKhoMauChiTiets.Add(item.ToEntity(nhapKhoMauChiTietEntity));
                }
                else
                {
                    var result = model.NhapKhoMauChiTiets.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.NhapKhoMauChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.NhapKhoMauChiTiets.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddOrUpdateDuyetNhapKhoMauChiTiet(DuyetPhieuNhapKhoMauViewModel viewModel, Core.Domain.Entities.MauVaChePhams.NhapKhoMau model)
        {
            foreach (var item in viewModel.DuyetNhapKhoMauChiTiets)
            {
                if (item.Id == 0)
                {
                    var nhapKhoMauChiTietEntity = new NhapKhoMauChiTiet();
                    model.NhapKhoMauChiTiets.Add(item.ToEntity(nhapKhoMauChiTietEntity));
                }
                else
                {
                    var result = model.NhapKhoMauChiTiets.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.NhapKhoMauChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DuyetNhapKhoMauChiTiets.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }
    }
}
