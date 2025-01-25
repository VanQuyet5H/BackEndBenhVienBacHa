using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Marketing;
using Camino.Api.Models.QuaTang;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Camino.Core.Domain.ValueObject.Marketing;

namespace Camino.Api.Models.MappingProfile
{
    public class XuatKhoQuaTangMarketingMappingProfile : Profile
    {
        public XuatKhoQuaTangMarketingMappingProfile()
        {
            CreateMap<XuatKhoQuaTang, XuatKhoQuaTangMarketingViewModel>()
                 .AfterMap((s, d) =>
                 {
                     d.NguoiNhan = s.BenhNhan.HoTen;
                     d.NhanVienXuat = s.NguoiXuat.User.HoTen;
                 });

            CreateMap<XuatKhoQuaTangMarketingViewModel, XuatKhoQuaTang>()
                .ForMember(d => d.XuatKhoQuaTangChiTiet, o => o.Ignore());

            CreateMap<XuatKhoQuaTangChiTiet, XuatKhoQuaTangMarketingChiTietViewModel>()
                .AfterMap((s, d) =>
                {
                    //d.NguoiNhan = s.XuatKhoQuaTang.BenhNhan.HoTen;
                    //d.NhanVienXuat = s.XuatKhoQuaTang.NguoiXuat.User.HoTen;
                });

            CreateMap<XuatKhoQuaTangMarketingChiTietViewModel, XuatKhoQuaTangChiTiet>();
        }
    }
}
