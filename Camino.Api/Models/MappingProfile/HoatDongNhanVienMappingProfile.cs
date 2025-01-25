using AutoMapper;
using Camino.Api.Models.KhoaPhong;
using Camino.Core.Domain.Entities.NhanViens;
using System;

namespace Camino.Api.Models.MappingProfile
{
    public class HoatDongNhanVienMappingProfile : Profile
    {
        public HoatDongNhanVienMappingProfile()
        {
            CreateMap<PhongNhanVienViewModel, HoatDongNhanVien>()
                  .AfterMap((source, dest) =>
                  {
                      dest.ThoiDiemBatDau = DateTime.Now;
                  });

            CreateMap<PhongNhanVienViewModel, LichSuHoatDongNhanVien>()
                  .AfterMap((source, dest) =>
                  {
                      dest.ThoiDiemBatDau = DateTime.Now;
                  });
        }
    }
}
