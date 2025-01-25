using AutoMapper;
using Camino.Api.Models.BenhVien;
using Camino.Core.Domain.ValueObject.BenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class BenhVienMappingProfile : Profile
    {
        public BenhVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.BenhVien.BenhVien, BienVienViewModel>()
                .AfterMap((s, d) =>
                {
                    //d.TenCapQuanLyBenhVien = s.CapQuanLyBenhVien?.Ten;
                    d.TenDonViHanhChinh = s.DonViHanhChinh?.Ten ?? string.Empty;
                    d.TenLoaiBenhVien = s.LoaiBenhVien?.Ten;
                    d.HangBenhVienDisplay = s.HangBenhVien.GetDescription();
                    d.TuyenChuyenMonKyThuatDisplay = s.TuyenChuyenMonKyThuat.GetDescription();
                });
            CreateMap<BienVienViewModel, Core.Domain.Entities.BenhVien.BenhVien>()
                      //.ForMember(d => d.CapQuanLyBenhVien, o => o.Ignore())
                      .ForMember(d => d.DonViHanhChinh, o => o.Ignore())
                      .ForMember(d => d.LoaiBenhVien, o => o.Ignore());

            CreateMap<BenhVienGridVo, BenhVienExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.HieuLucDisplay = s.HieuLuc != true ? "Ngừng sử dụng" : "Đang sử dụng";
                });
        }
    }
}
