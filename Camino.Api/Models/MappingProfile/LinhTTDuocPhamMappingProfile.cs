using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhDuocPhamTrucTiep;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using System.Linq;


namespace Camino.Api.Models.MappingProfile
{
    public class LinhTTDuocPhamMappingProfile :Profile
    {
        public LinhTTDuocPhamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham, LinhDuocPhamTrucTiepViewModel>().AfterMap((s, d) =>
            {
                d.TenKhoXuat = s.KhoXuat.Ten;
            }).IgnoreAllNonExisting()
            .ForMember(d => d.YeuCauDuocPhamBenhViensTT, o => o.Ignore());

            CreateMap<LinhDuocPhamTrucTiepViewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoNhap, o => o.Ignore())
                .ForMember(d => d.KhoXuat, o => o.Ignore())
                .ForMember(d => d.NhanVienYeuCau, o => o.Ignore())
                .ForMember(d => d.NhanVienDuyet, o => o.Ignore())
                .ForMember(d => d.YeuCauLinhDuocPhamChiTiets, o => o.Ignore())
                .ForMember(d => d.XuatKhoDuocPhams, o => o.Ignore())
                .ForMember(d => d.YeuCauDuocPhamBenhViens, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateLinhBuDuocPhamChiTiet(d, s);
                });
            CreateMap<DSLinhVatTuGridVo, DsLinhDuocPhamExcel>()
             .IgnoreAllNonExisting();
        }

        private void AddOrUpdateLinhBuDuocPhamChiTiet(LinhDuocPhamTrucTiepViewModel viewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham entity)
        {
            foreach (var item in viewModel.YeuCauLinhDuocPhamChiTiets)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet();
                    entity.YeuCauLinhDuocPhamChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.YeuCauLinhDuocPhamChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
        //    foreach (var item in entity.YeuCauLinhDuocPhamChiTiets)
        //    {
        //        if (item.Id != 0)
        //        {
        //            var countModel = viewModel.YeuCauLinhDuocPhamChiTiets.Where(x => x.Id == item.Id).ToList();
        //            if (countModel.Count == 0)
        //            {
        //                //item.WillDelete = true;
        //            }

        //        }
        //    }
        }
    }
}
