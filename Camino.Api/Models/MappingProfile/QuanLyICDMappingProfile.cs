using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.ICDs;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject.ICDs;

namespace Camino.Api.Models.MappingProfile
{
    public class QuanLyICDMappingProfile : Profile
    {
        public QuanLyICDMappingProfile()
        {
            CreateMap<ICD, QuanLyICDViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenLoaiICD = s.LoaiICD?.TenTiengViet;
                    d.TenKhoa = s.Khoa?.Ten;
                });

            CreateMap<QuanLyICDViewModel, ICD>().IgnoreAllNonExisting()
                .ForMember(x => x.ICDDoiTuongBenhNhanChiTiets, o => o.Ignore())
                .ForMember(x => x.ChuanDoanLienKetICDs, o => o.Ignore())
                .ForMember(x => x.YeuCauKhamBenhs, o => o.Ignore())
                .ForMember(x => x.YeuCauKhamBenhICDKhacs, o => o.Ignore())
                .ForMember(x => x.ToaThuocMaus, o => o.Ignore())
                .ForMember(x => x.YeuCauKhamBenhChanDoanPhanBiets, o => o.Ignore())
                .ForMember(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTSauPhauThuats, o => o.Ignore())
                .ForMember(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTTruocPhauThuats, o => o.Ignore())
                ;

            CreateMap<QuanLyICDGridVo, QuanLyICDExportExcel>().IgnoreAllNonExisting()
                  .ForMember(m => m.HieuLuc, o => o.MapFrom(p => p.HieuLuc != true ? "Ngừng sử dụng" : "Đang sử dụng"));
        }
    }
}
