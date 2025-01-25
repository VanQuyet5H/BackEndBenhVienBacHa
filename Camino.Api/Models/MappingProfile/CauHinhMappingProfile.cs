using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Cauhinh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Newtonsoft.Json;

namespace Camino.Api.Models.MappingProfile
{
    public class CauHinhMappingProfile : Profile
    {
        public CauHinhMappingProfile()
        {
            CreateMap<CauHinh, CauhinhViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    if (s.DataType == Enums.DataType.List)
                    {
                        d.CauHinhDanhSachChiTiets.AddRange(JsonConvert.DeserializeObject<List<LookupItemCauHinhVo>>(s.Value).Select(item => new CauHinhDanhSachChiTietViewModel()
                        {
                            KeyId = item.KeyId,
                            DisplayName = item.DisplayName,
                            Value = item.Value,
                            GhiChu = item.GhiChu,
                            DataType = item.DataType != 0 ? (Enums.DataType)item.DataType : Enums.DataType.String,
                            IsDisabled = true
                        }));
                    }
                });
            CreateMap<CauhinhViewModel, CauHinh>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    if (s.DataType == Enums.DataType.List)
                    {
                        int i = 1;
                        foreach (var item in d.CauHinhDanhSachChiTiets)
                        {
                            item.DataType = item.DataType ?? Enums.DataType.String;
                            if (d.DataTypeLoaiCauHinh == Enums.LoaiCauHinh.CauHinhGachNo)
                            {
                                item.DisplayName = item.DisplayName.Trim();
                                item.KeyId = item.DisplayName;
                            }
                            else
                            {
                                item.KeyId = item.KeyId ?? DateTime.Now.ToString("ddMMyyyHHmmss") + i++;
                            }
                        }
                        s.Value = JsonConvert.SerializeObject(d.CauHinhDanhSachChiTiets);
                    }
                });
        }
    }
}
