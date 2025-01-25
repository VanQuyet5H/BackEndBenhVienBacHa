using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetDuocPhamMappingProfile : Profile
    {
        public DuyetDuocPhamMappingProfile()
        {
            CreateMap<DanhSachDuyetKhoDuocPhamVo, DuyetDuocPhamExportExcel>();
            CreateMap<DanhSachDuyetKhoDuocPhamChiTietVo, DuyetDuocPhamExportExcelChild>()                 
                .IgnoreAllNonExisting();
        }
    }
}
