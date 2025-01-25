using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Camino.Api.Models.MappingProfile
{
    public class HoSoKhacGiayRaVienMappingProfile : Profile
    {
        public HoSoKhacGiayRaVienMappingProfile()
        {
            CreateMap<NoiTruHoSoKhac, GiayRaVienViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    var thongTinHoSo = JsonConvert.DeserializeObject<GiayRaVienViewModel>(d.ThongTinHoSo);
                    d.ChanDoan = thongTinHoSo.ChanDoan;
                    d.PhuongPhapDieuTri = thongTinHoSo.PhuongPhapDieuTri;
                   
                    if (thongTinHoSo.GhiChu != null)
                    {
                        var itemList = thongTinHoSo.GhiChu.Split("<br>");
                        d.GhiChu = itemList.Join("\n");
                    }
                   
                    d.TruongKhoaId = thongTinHoSo.TruongKhoaId;
                    d.GiamDocChuyenMonId = thongTinHoSo.GiamDocChuyenMonId;
                    d.ThoiDiemThucHienDisplay = d.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH();
                    d.IdGhiChu = thongTinHoSo.IdGhiChu;
                });

            CreateMap<GiayRaVienViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
               .ForMember(d => d.NoiTruHoSoKhacFileDinhKems, o => o.Ignore());
        }
    }
}
