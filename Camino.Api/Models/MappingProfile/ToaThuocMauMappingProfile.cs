using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.ToaThuocMau;
using Camino.Core.Domain.Entities.Thuocs;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.ToaThuocMau;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class ToaThuocMauMappingProfile : Profile
    {
        public ToaThuocMauMappingProfile()
        {
            CreateMap<Core.Domain.Entities.Thuocs.ToaThuocMau, ToaThuocMauViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.ToaThuocMauChiTiets, o => o.MapFrom(s => s.ToaThuocMauChiTiets))
                .AfterMap((s, d) =>
                {
                    d.TenBacSiKeToa = s.BacSiKeToa?.User.HoTen;
                    d.TenTrieuChung = s.TrieuChung?.Ten;
                    //d.TenChuanDoan = s.ChuanDoan?.Ma + " - " + ((s.ChuanDoan?.TenTiengViet != "" && s.ChuanDoan?.TenTiengViet != null) ? s.ChuanDoan?.TenTiengViet : s.ChuanDoan?.TenTiengAnh);
                    d.TenChuanDoan = ((s.ChuanDoan?.TenTiengViet != "" && s.ChuanDoan?.TenTiengViet != null) ? s.ChuanDoan?.TenTiengViet : s.ChuanDoan?.TenTiengAnh);

                    d.TenICD = s.ICD?.Ma + " - " + s.ICD?.TenTiengViet;
                    //d.TenICD = s.ICD?.TenTiengViet;

                    foreach (var item in d.ToaThuocMauChiTiets)
                    {
                        item.DungSangDisplay = item.DungSang.FloatToStringFraction();
                        item.DungTruaDisplay = item.DungTrua.FloatToStringFraction();
                        item.DungChieuDisplay = item.DungChieu.FloatToStringFraction();
                        item.DungToiDisplay = item.DungToi.FloatToStringFraction();
                    }
                });

            CreateMap<ToaThuocMauViewModel, Core.Domain.Entities.Thuocs.ToaThuocMau>().IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauKhamBenhDonThuocs, o => o.Ignore())
                .ForMember(d => d.ToaThuocMauChiTiets, o => o.Ignore())
                 .AfterMap((d, s) =>
                 {
                     AddOrUpdateToaThuocMauChiTiet(d, s);
                 });

            CreateMap<ToaThuocMauGridVo, ToaThuocMauExportExcel>().IgnoreAllNonExisting();
            CreateMap<ToaThuocMauChiTietGridVo, ToaThuocMauExportExcelChild>().IgnoreAllNonExisting();
        }
       
        private void AddOrUpdateToaThuocMauChiTiet(ToaThuocMauViewModel viewModel, Core.Domain.Entities.Thuocs.ToaThuocMau entity)
        {
            foreach (var item in viewModel.ToaThuocMauChiTiets)
            {
                if (item.Id == 0)
                {
                    var newEntity = new ToaThuocMauChiTiet();
                    entity.ToaThuocMauChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.ToaThuocMauChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.ToaThuocMauChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.ToaThuocMauChiTiets.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }


    }
}
