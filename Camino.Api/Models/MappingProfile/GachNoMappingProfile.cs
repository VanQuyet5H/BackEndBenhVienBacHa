using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GachNo;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GachNos;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class GachNoMappingProfile : Profile
    {
        public GachNoMappingProfile()
        {
            CreateMap<GachNoViewModel, Core.Domain.Entities.GachNos.GachNo>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.Ignore())
                .ForMember(x => x.CongTyBaoHiemTuNhan, o => o.Ignore());

            CreateMap<Core.Domain.Entities.GachNos.GachNo, GachNoViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.MapFrom(a => a.BenhNhan))
                .ForMember(x => x.CongTyBaoHiemTuNhan, o => o.MapFrom(a => a.CongTyBaoHiemTuNhan));

            CreateMap<CongTyBaoHiemTuNhan, GachNoCongTyBaoHiemTuNhanViewModel>().IgnoreAllNonExisting();

            CreateMap<BenhNhan, GachNoBenhNhanViewModel>().IgnoreAllNonExisting();

            // export excel
            CreateMap<GachNoGridVo, GachNoExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TienHachToan = s.LoaiTienTe == Enums.LoaiTienTe.USD ? s.TienHachToan.ApplyFormatMoneyUSD() : s.TienHachToan.ApplyFormatMoneyVND();
                    d.TienThueHachToan = s.LoaiTienTe == Enums.LoaiTienTe.USD ? s.TienThueHachToan.ApplyFormatMoneyUSD() : s.TienThueHachToan.ApplyFormatMoneyVND();
                    d.TongTienHachToan = s.TongTienHachToan.ApplyFormatMoneyVND();
                });
        }
    }
}
