using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;

namespace Camino.Api.Models.MappingProfile
{
    public class DonThuocTrongNgayMappingProfile : Profile
    {
        public DonThuocTrongNgayMappingProfile()
        {
            CreateMap<DonThuocThanhToanGridVo, DonThuocTongNgayExportExcel>()
                 .AfterMap((s, d) =>
                 {
                     d.SoTienChoThanhToanString = s.SoTienChoThanhToan.ApplyFormatMoneyVND();
                     d.TongGiaTriDonThuocString = s.TongGiaTriDonThuoc.ApplyFormatMoneyVND();
                 })
               .IgnoreAllNonExisting();
            CreateMap<LichSuXuatThuocGridVo, LichSuBanThuocExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.SoTienThuString = s.SoTienThu.ApplyFormatMoneyVND();
                })
               .IgnoreAllNonExisting();
            CreateMap<LichSuXuatThuocGridVo, LichSuXuatThuocExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.SoTienThuString = s.SoTienThu.ApplyFormatMoneyVND();
                })
               .IgnoreAllNonExisting();

            CreateMap<DanhSachLichSuXuatThuocGridVo, LichSuCapThuocBHYTExportExcel>()              
              .IgnoreAllNonExisting();

            CreateMap<DonThuocThanhToanChiTiet, DonThuocThanhToanChiTietTheoPhieuThu>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedOn, o => o.Ignore())
                .ForMember(x => x.LastUserId, o => o.Ignore())
                .ForMember(x => x.LastModified, o => o.Ignore())
                .ForMember(x => x.LastTime, o => o.Ignore());
            CreateMap<DonVTYTThanhToanChiTiet, DonVTYTThanhToanChiTietTheoPhieuThu>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedOn, o => o.Ignore())
                .ForMember(x => x.LastUserId, o => o.Ignore())
                .ForMember(x => x.LastModified, o => o.Ignore())
                .ForMember(x => x.LastTime, o => o.Ignore());
        }
    }
}
