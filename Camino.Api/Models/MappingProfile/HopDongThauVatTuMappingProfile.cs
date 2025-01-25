using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.HopDongThauVatTu;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.HopDongThauVatTu;

namespace Camino.Api.Models.MappingProfile
{
    public class HopDongThauVatTuMappingProfile : Profile
    {
        public HopDongThauVatTuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.HopDongThauVatTus.HopDongThauVatTu, HopDongThauVatTuViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(e => e.HopDongThauVatTuChiTiets, o => o.Ignore())
                .ForMember(e => e.NhaThau,
                    o => o.MapFrom(s => s.NhaThau != null ? s.NhaThau.Ten : string.Empty))
                .AfterMap((s,d) =>
                {
                    d.CoNhapKho = s.NhapKhoVatTuChiTiets.Any();
                    MapVtEntityToVm(s, d);
                });

            CreateMap<HopDongThauVatTuViewModel, Core.Domain.Entities.HopDongThauVatTus.HopDongThauVatTu>()
                .IgnoreAllNonExisting()
                .ForMember(e => e.NhaThau, o => o.Ignore())
                .ForMember(e => e.HopDongThauVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) => MapVtChiTietVmToEntity(s, d));

            CreateMap<HopDongThauVatTuChiTiet, HopDongThauVatTuChiTietViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(e => e.VatTu,
                    o => o.MapFrom(s => s.VatTu != null ? s.VatTu.Ten : string.Empty));

            CreateMap<HopDongThauVatTuChiTietViewModel, HopDongThauVatTuChiTiet>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.HopDongThauVatTu, o => o.Ignore())
                .ForMember(d => d.VatTu, o => o.Ignore());
            
            CreateMap<HdtVatTuGridVo, HopDongThauVatTuExportExcel>()
                .IgnoreAllNonExisting();

            CreateMap<HdtVatTuChiTietGridVo, HopDongThauVatTuExportExcelChild>()
                .IgnoreAllNonExisting();
        }

        private void MapVtChiTietVmToEntity(HopDongThauVatTuViewModel s, Core.Domain.Entities.HopDongThauVatTus.HopDongThauVatTu d)
        {
            foreach (var model in d.HopDongThauVatTuChiTiets)
            {
                if (s.HopDongThauVatTuChiTiets.All(x => x.Id != model.Id))
                {
                    model.WillDelete = true;
                }
            }

            foreach (var model in s.HopDongThauVatTuChiTiets.Where(e => e.Id == 0))
            {
                var hdtVtChiTietEntity = model.ToEntity<HopDongThauVatTuChiTiet>();
                d.HopDongThauVatTuChiTiets.Add(hdtVtChiTietEntity);
            }

            foreach (var model in s.HopDongThauVatTuChiTiets.Where(e => e.Id != 0))
            {
                foreach (var hdtVtChiTietHaveInEntity in d.HopDongThauVatTuChiTiets.Where(e => e.Id == model.Id))
                {
                    model.ToEntity(hdtVtChiTietHaveInEntity);
                }
            }
        }

        private void MapVtEntityToVm(Core.Domain.Entities.HopDongThauVatTus.HopDongThauVatTu s,
            HopDongThauVatTuViewModel d)
        {
            foreach (var vatTuChiTietEntity in s.HopDongThauVatTuChiTiets)
            {
                var vatTuChiTietViewModel = vatTuChiTietEntity.ToModel<HopDongThauVatTuChiTietViewModel>();
                d.HopDongThauVatTuChiTiets.Add(vatTuChiTietViewModel);
            }
        }
    }
}
