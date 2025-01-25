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
    public class HopDongKhamSucKhoeMappingProfile : Profile
    {
        public HopDongKhamSucKhoeMappingProfile()
        {
            CreateMap<HopDongKhamSucKhoe, TimKiemHopDongKhamTheoCongTyViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.CongTyId = s.CongTyKhamSucKhoeId;
                    d.HopDongId = s.Id;
                    d.TrangThai = s.DaKetThuc ? Enums.TrangThaiHopDongKham.DaKetThucHD : Enums.TrangThaiHopDongKham.DangThucHienHD;
                    d.SoBenhNhan = s.SoNguoiKham;
                });
            CreateMap<TimKiemHopDongKhamTheoCongTyViewModel, HopDongKhamSucKhoe>().IgnoreAllNonExisting();
        }
    }
}
