using AutoMapper;
using System.Linq;
using Camino.Api.Extensions;
using Camino.Api.Models.QuocGia;
using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Api.Models.MappingProfile
{
    public class NhomICDTheoBenhVienMappingProfile : Profile
    {
        public NhomICDTheoBenhVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.ICDs.NhomICDTheoBenhVien, NhomICDTheoBenhVienViewModel>()
                .AfterMap((s, d) =>
              {
                  //if (s.NhomICDLienKetICDTheoBenhViens.Any())
                  //{
                  //    var nhomICDLienKetICDTheoBenhViens = s.NhomICDLienKetICDTheoBenhViens.Select(c => new NhomICDLienKetICDTheoBenhVienViewModel
                  //    {
                  //        Id = c.Id,
                  //        ICDId = c.ICDId,
                  //        NhomICDTheoBenhVienId = c.NhomICDTheoBenhVienId

                  //    }).ToList();
                  //    d.NhomICDLienKetICDTheoBenhViens.AddRange(nhomICDLienKetICDTheoBenhViens);
                  //}

                  //if (s.NhomICDLienKetICDTheoBenhViens.Any())
                  //{
                  //    var nhomICDLienKetICDTheoBenhViens = s.NhomICDLienKetICDTheoBenhViens.Select(c => c.ICDId).ToList();
                  //    d.MaICDs.AddRange(nhomICDLienKetICDTheoBenhViens);
                  //}

                  d.TextChuongICD = s.ChuongICD.TenTiengViet + "-" + s.ChuongICD.TenTiengAnh;
                  if (!string.IsNullOrEmpty(s.MoTa))
                  {
                      d.MaICDs = s.MoTa.Split(";").ToList();
                  }
              });

            CreateMap<NhomICDLienKetICDTheoBenhVienViewModel, Core.Domain.Entities.ICDs.NhomICDLienKetICDTheoBenhVien>();
            CreateMap<NhomICDTheoBenhVienViewModel, Core.Domain.Entities.ICDs.NhomICDTheoBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.NhomICDLienKetICDTheoBenhViens, o => o.Ignore())
                .ForMember(d => d.ChuongICD, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.MaICDs.Any())
                    {
                        d.MoTa = string.Join(";", s.MaICDs.ToArray());
                    }
                    //foreach (var model in d.NhomICDLienKetICDTheoBenhViens)
                    //{
                    //    if (s.NhomICDLienKetICDTheoBenhViens.All(c => c.Id != model.Id))
                    //    {
                    //        model.WillDelete = true;
                    //    }
                    //}
                    //foreach (var model in s.NhomICDLienKetICDTheoBenhViens)
                    //{
                    //    if (model.Id == 0)
                    //    {
                    //        var newEntity = new Core.Domain.Entities.ICDs.NhomICDLienKetICDTheoBenhVien();
                    //        d.NhomICDLienKetICDTheoBenhViens.Add(model.ToEntity(newEntity));
                    //    }
                    //    else
                    //    {
                    //        if (d.NhomICDLienKetICDTheoBenhViens.Any())
                    //        {
                    //            var result = d.NhomICDLienKetICDTheoBenhViens.Single(c => c.Id == model.Id);
                    //            result = model.ToEntity(result);
                    //        }

                    //    }
                    //}
                });
        }
    }
}
