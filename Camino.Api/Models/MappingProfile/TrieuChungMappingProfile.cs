using AutoMapper;
using Camino.Api.Models.TrieuChung;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.TrieuChungs;
using Camino.Core.Domain.Entities.Users;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class TrieuChungMappingProfile : Profile
    {
        public TrieuChungMappingProfile()
        {
            CreateMap<Core.Domain.Entities.TrieuChungs.TrieuChung, TrieuChungViewModel>()
               .ForMember(x => x.TrieuChungDanhMucChuanDoans, o => o.MapFrom(s => s.TrieuChungDanhMucChuanDoans))
                .AfterMap((c, d) =>
                {
                    if (c.TrieuChungCha != null)
                    {
                        d.TenCha = c.TrieuChungCha.Ten;
                    }
                    else
                    {
                        d.TenCha = c.Ten;
                    }
                    d.DanhMucChuanIds = c.TrieuChungDanhMucChuanDoans.Select(r => r.DanhMucChuanDoanId).ToList();

                }); ;
            CreateMap<TrieuChungViewModel, Core.Domain.Entities.TrieuChungs.TrieuChung>()
                 .ForMember(x => x.TrieuChungDanhMucChuanDoans, o => o.Ignore())
                .AfterMap((c, d) =>
                {
                    foreach (var model in d.TrieuChungDanhMucChuanDoans)
                    {
                        if (c.TrieuChungDanhMucChuanDoans.All(x => x.DanhMucChuanDoanId != 0))
                        {
                            model.WillDelete = true;
                        }
                    }
                    foreach (var item in c.DanhMucChuanIds)
                    {
                        if (item != 0)
                        {
                            var entity = new Core.Domain.Entities.TrieuChungDanhMucChuanDoans.TrieuChungDanhMucChuanDoan()
                            {
                                DanhMucChuanDoanId = item,
                            };
                            d.TrieuChungDanhMucChuanDoans.Add(entity);
                        }

                    }

                }); ;
        }
    }
}
