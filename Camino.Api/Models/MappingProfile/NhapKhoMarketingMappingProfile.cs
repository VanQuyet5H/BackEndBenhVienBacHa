using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhapKhoMarketing;
using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Marketing;
using Camino.Core.Domain.ValueObject.NhapKhoMarketting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class NhapKhoMarketingMappingProfile : Profile
    {
        public NhapKhoMarketingMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhapKhoQuaTangs.NhapKhoQuaTang, NhapKhoMarketingViewModel>()
                .AfterMap((s, d) =>
                {
                });
            CreateMap<NhapKhoMarketingViewModel,Core.Domain.Entities.NhapKhoQuaTangs.NhapKhoQuaTang>()
                .ForMember(x => x.NhapKhoQuaTangChiTiets, o => o.Ignore()).AfterMap((d, s) =>
                {
                    AddOrUpdateNhapKhoQuaTangChiTiet(d, s);
                });
            CreateMap<DanhSachQuaTangDuocThem, NhapKhoQuaTangChiTiet>();

            CreateMap<NhapKhoQuaTangChiTiet, DanhSachQuaTangDuocThem>();

            CreateMap<NhapKhoQuaTangMarketingGridVo, NhapKhoQuaTangMarketingExcel>().IgnoreAllNonExisting();


            #region XuatKhoMarketingMappingExcel
            CreateMap<XuatKhoQuaTangMarketingGridVo, XuatKhoMarketingExporExcel>().IgnoreAllNonExisting();
            #endregion
        }


        private void AddOrUpdateNhapKhoQuaTangChiTiet(NhapKhoMarketingViewModel viewModel, Core.Domain.Entities.NhapKhoQuaTangs.NhapKhoQuaTang model)
        {
            
                foreach (var item in viewModel.DanhSachQuaTangDuocThemList)
                {
                    if (item.Id == 0)
                    {
                        var ChiTietNhapKhoQuaTangEntity = new NhapKhoQuaTangChiTiet();
                        model.NhapKhoQuaTangChiTiets.Add(item.ToEntity(ChiTietNhapKhoQuaTangEntity));
                    }
                    else
                    {
                        var result = model.NhapKhoQuaTangChiTiets.FirstOrDefault(p => p.Id == item.Id);
                        if (result != null)
                        {
                            result = item.ToEntity(result);
                        }
                    }
                }
           
        }
    }
}
