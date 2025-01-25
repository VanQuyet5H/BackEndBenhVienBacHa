using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuKyThuatMappingProfile : Profile
    {
        public DichVuKyThuatMappingProfile()
        {
            CreateMap<DichVuKyThuat, DichVuKyThuatViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                    {
                        d.TenNhomDichVuKyThuat = s.NhomDichVuKyThuat?.Ten;
                    });

            CreateMap<DichVuKyThuatViewModel, DichVuKyThuat>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuKyThuatThongTinGias, o => o.Ignore())
                    .AfterMap((d, s) =>
                    {
                        AddOrUpdateDichVuKyThuatThongTinGiaChiTiet(d, s);
                    });

            CreateMap<DichVuKyThuatGridVo, DichVuKyThuatExportExcel>().IgnoreAllNonExisting();
            CreateMap<DichVuKyThuatThongTinGiaGridVo, DichVuKyThuatExportExcelChild>().IgnoreAllNonExisting();

            CreateMap<DichVuKyThuatThongTinGia, DichVuKyThuatThongTinGiaViewModel>().IgnoreAllNonExisting()
                  .AfterMap((s, d) =>
                  {
                      d.TenHangBenhVien = s.HangBenhVien.GetDescription();
                  });
            CreateMap<DichVuKyThuatThongTinGiaViewModel, DichVuKyThuatThongTinGia>().IgnoreAllNonExisting();
        }

        private void AddOrUpdateDichVuKyThuatThongTinGiaChiTiet(DichVuKyThuatViewModel viewModel, DichVuKyThuat entity)
        {
            foreach (var item in viewModel.DichVuKyThuatThongTinGias)
            {
                if (item.Id == 0)
                {
                    var newEntity = new DichVuKyThuatThongTinGia();
                    entity.DichVuKyThuatThongTinGias.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.DichVuKyThuatThongTinGias.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.DichVuKyThuatThongTinGias)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DichVuKyThuatThongTinGias.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
