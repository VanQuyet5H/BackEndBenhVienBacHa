using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhVatTuTrucTiep;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class LinhTTVatTuMappingProfile : Profile
    {
        public LinhTTVatTuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu, LinhVatTuTrucTiepViewModel>().AfterMap((s, d) =>
            {
                d.TenKhoXuat = s.KhoXuat.Ten;
            }).IgnoreAllNonExisting()
            .ForMember(d => d.YeuCauVatTuBenhViensTT, o => o.Ignore());
            //.ForMember(d => d.YeuCauLinhVatTuChiTiets, o => o.Ignore());

            CreateMap<LinhVatTuTrucTiepViewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoNhap, o => o.Ignore())
                .ForMember(d => d.KhoXuat, o => o.Ignore())
                .ForMember(d => d.NhanVienYeuCau, o => o.Ignore())
                .ForMember(d => d.NhanVienDuyet, o => o.Ignore())
                .ForMember(d => d.YeuCauLinhVatTuChiTiets, o => o.Ignore())
                .ForMember(d => d.XuatKhoVatTus, o => o.Ignore())
                .ForMember(d => d.YeuCauVatTuBenhViens, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateLinhBuDuocPhamChiTiet(d, s);
                });
            CreateMap<LinhTrucTiepVatTuChiTietViewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet, LinhTrucTiepVatTuChiTietViewModel>().IgnoreAllNonExisting();
            CreateMap<DsLinhDuocPhamGridVo, DsLinhDuocPhamExcel>()
             .IgnoreAllNonExisting();
        }

        private void AddOrUpdateLinhBuDuocPhamChiTiet(LinhVatTuTrucTiepViewModel viewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu entity)
        {
            foreach (var item in viewModel.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet();
                    entity.YeuCauLinhVatTuChiTiets.Add(item.ToEntity(newEntity));
                  
                }
                else
                {
                    var result = entity.YeuCauLinhVatTuChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauLinhVatTuChiTiets.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
