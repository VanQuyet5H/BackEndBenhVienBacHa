using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.TongHopDuTruMuaThuocTaiGiamDocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaThuocTaiGiamDocs;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class TongHopDuTruMuaDuocPhamTaiGiamDocMappingProfile : Profile
    {
        public TongHopDuTruMuaDuocPhamTaiGiamDocMappingProfile()
        {
            CreateMap<DuTruMuaDuocPhamKhoDuoc, DuTruGiamDocViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.KyDuTru = s.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() + " - " + s.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate();
                    d.TrangThai = s.GiamDocDuyet;
                });

            CreateMap<DuTruGiamDocGridVo, DuTruGiamDocDuocPhamExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) => { d.TrangThai = s.TrangThai == true ? "Đã duyệt" : s.TrangThai == false ? "Từ chối duyệt" : "Đang chờ duyệt"; });
            CreateMap<DuTruGiamDocDetailGridVo, DuTruGiamDocDuocPhamExportExcelChild>().IgnoreAllNonExisting()
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
