using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.PhanQuyenNguoiDungs;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.PhanQuyenNguoiDungs;

namespace Camino.Api.Models.MappingProfile
{
    public class PhanQuyenNguoiDungMappingProfile : Profile
    {
        public PhanQuyenNguoiDungMappingProfile()
        {
            CreateMap<Role, PhanQuyenNguoiDungViewModel>().IgnoreAllNonExisting();
            CreateMap<PhanQuyenNguoiDungViewModel, Role>()
                .ForMember(x => x.NhanVienRoles, o => o.Ignore())
                .ForMember(x => x.RoleFunctions, o => o.Ignore());
            CreateMap<PhanQuyenNguoiDungGridVo, PhanQuyenNguoiDungExportExcel>()
                .AfterMap((source, dest) =>
            {
                dest.Quyen = source.Quyen ? "Phải" : "Không";
            });
        }
    }
}
