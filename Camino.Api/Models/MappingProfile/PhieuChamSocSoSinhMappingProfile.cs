using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Api.Models.MappingProfile
{
    public class PhieuChamSocSoSinhMappingProfile : Profile
    {
        public PhieuChamSocSoSinhMappingProfile()
        {
            //Phiếu chăm sóc
            CreateMap<NoiTruHoSoKhac, PhieuChamSocSoSinhViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    var thongTinHoSo = JsonConvert.DeserializeObject<PhieuChamSocSoSinhViewModel>(d.ThongTinHoSo);
                    d.ICDId = thongTinHoSo.ICDId;
                    d.TenICD = thongTinHoSo.TenICD;
                    d.TenNhanVienThucHien = s.NhanVienThucHien.User.HoTen;
                    d.ThoiDiemThucHienDisplay = d.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH();
                    d.TenNhanVienThucHien = s.NhanVienThucHien.User.HoTen;
                    d.ThongTinHoSoPhieuChamSocSoSinhs = thongTinHoSo.ThongTinHoSoPhieuChamSocSoSinhs;
                });

            CreateMap<PhieuChamSocSoSinhViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
               .ForMember(d => d.NoiTruHoSoKhacFileDinhKems, o => o.Ignore());
        }
    }
}
