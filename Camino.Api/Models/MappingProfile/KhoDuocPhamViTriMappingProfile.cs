using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhoDuocPhamViTri;
using Camino.Core.Domain.ValueObject.KhoDuocPhamViTris;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class KhoDuocPhamViTriMappingProfile : Profile
    {
        public KhoDuocPhamViTriMappingProfile()
        {
            CreateMap<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri, KhoDuocPhamViTriViewModel>()
                .AfterMap((s, d) =>
                {
                    d.Ten = s.Ten;
                    d.TenKhoDuocPham = s.KhoDuocPham?.Ten;
                });
            CreateMap<KhoDuocPhamViTriViewModel, Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri>();
                //.ForMember(d => d.NhomChucDanh, o => o.Ignore());
            CreateMap<KhoDuocPhamViTriGridVo, KhoDuocPhamViTriExportExcel>()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
    }
}
