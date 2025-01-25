using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NoiGioiThieu;
using Camino.Core.Domain.Entities.DonViMaus;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.NoiGioiThieu;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiGioiThieuMappingProfile : Profile
    {
        public NoiGioiThieuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu, NoiGioiThieuViewModel>().IgnoreAllNonExisting()
                 .AfterMap((s, d) =>
                 {
                     d.HoTenNguoiQuanLy = s.NhanVienQuanLy?.User?.HoTen + "  -  " + s.NhanVienQuanLy?.User?.SoDienThoai;
                     d.TenDonViMau = s.DonViMau?.Ten;
                     d.IsDisabled = s.IsDisabled == true;
                 });
            CreateMap<NoiGioiThieuViewModel, Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>().IgnoreAllNonExisting()
                .ForMember(x => x.NoiGioiThieuChiTietMienGiams, o => o.Ignore())
                .ForMember(x => x.MienGiamChiPhis, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateNoiGioiThieuChiTietMienGiams(s,d);
                });

            CreateMap<DonViMau, DonViMauViewModel>().IgnoreAllNonExisting();
            CreateMap<DonViMauViewModel, DonViMau>().IgnoreAllNonExisting();

            CreateMap<NoiGioiThieuGridVo, NoiGioiThieuExportExcel>().IgnoreAllNonExisting()
               .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));

            CreateMap<CauHinhHeSoTheoNoiGioiThieuHoaHongGridVo, CauHinhNoiGioiThieuVaHoaHongExportExcel>().IgnoreAllNonExisting();

        }

        private void AddOrUpdateNoiGioiThieuChiTietMienGiams(NoiGioiThieuViewModel viewModel, Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu model)
        {
            foreach (var item in viewModel.NoiGioiThieuChiTietMienGiams)
            {
                if (item.Id == 0)
                {
                    var mienGiamEntity = new NoiGioiThieuChiTietMienGiam();
                    model.NoiGioiThieuChiTietMienGiams.Add(item.ToEntity(mienGiamEntity));
                }
                else
                {
                    var result = model.NoiGioiThieuChiTietMienGiams.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.NoiGioiThieuChiTietMienGiams)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.NoiGioiThieuChiTietMienGiams.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }
    }
}
