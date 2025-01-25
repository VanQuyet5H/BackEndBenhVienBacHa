using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKhamBenh;
using Camino.Core.Domain.Entities.DichVuKhamBenhs;
using Camino.Core.Domain.ValueObject.DichVuKhamBenh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuKhamBenhMappingProfile : Profile
    {
        public DichVuKhamBenhMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh, DichVuKhamBenhViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenKhoa = s.Khoa?.Ten;
                });

            CreateMap<DichVuKhamBenhViewModel, Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuKhamBenhThongTinGias, o => o.Ignore())
                    .AfterMap((d, s) =>
                    {
                        AddOrUpdateDichVuKhamBenhThongTinGiaChiTiet(d, s);
                    });

            CreateMap<DichVuKhamBenhThongTinGia, DichVuKhamBenhThongTinGiaViewModel>().IgnoreAllNonExisting();
            CreateMap<DichVuKhamBenhThongTinGiaViewModel, DichVuKhamBenhThongTinGia>().IgnoreAllNonExisting();

            CreateMap<DichVuKhamBenhGridVo, DichVuKhamBenhExportExcel>().IgnoreAllNonExisting();
            CreateMap<DichVuKhamBenhThongTinGiaGridVo, DichVuKhamBenhExportExcelChild>().IgnoreAllNonExisting();
        }

        private void AddOrUpdateDichVuKhamBenhThongTinGiaChiTiet(DichVuKhamBenhViewModel viewModel, Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh entity)
        {
            foreach (var item in viewModel.DichVuKhamBenhThongTinGias)
            {
                if (item.Id == 0)
                {
                    var newEntity = new DichVuKhamBenhThongTinGia();
                    entity.DichVuKhamBenhThongTinGias.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.DichVuKhamBenhThongTinGias.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.DichVuKhamBenhThongTinGias)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DichVuKhamBenhThongTinGias.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
