using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.TongHopDuTruMuaKSNKTaiGiamDocs;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaKSNKTaiGiamDocs;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class TongHopDuTruMuaKSNKTaiGiamDocMappingProfile : Profile
    {
        public TongHopDuTruMuaKSNKTaiGiamDocMappingProfile()
        {
            CreateMap<DuTruMuaVatTuKhoDuoc, DuTruKSNKGiamDocViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.KyDuTru = s.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() + " - " + s.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate();
                    d.TrangThai = s.GiamDocDuyet;
                });

            CreateMap<DuTruGiamDocKSNKGridVo, DuTruGiamDocVatTuExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) => { d.TrangThai = s.TrangThai == true ? "Đã duyệt" : s.TrangThai == false ? "Từ chối duyệt" : "Đang chờ duyệt"; });
            CreateMap<DuTruGiamDocKSNKDetailGridVo, DuTruGiamDocVatTuExportExcelChild>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.SoLuongDuTru = s.SoLuongDuTru.ApplyNumber();
                    d.SoLuongDuKienTrongKy = s.SoLuongDuKienTrongKy.ApplyNumber();
                    d.SoLuongDuTruTrKhoa = s.SoLuongDuTruTrKhoa.ApplyNumber();
                    d.SoLuongDuTruKhDuoc = s.SoLuongDuTruKhDuoc.ApplyNumber();
                    d.SoLuongDuTruDirector = s.SoLuongDuTruDirector.GetValueOrDefault().ApplyNumber();
                });
        }
    }
}
