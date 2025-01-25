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
    public class HoSoKhacGiayChungSinhMappingProfile : Profile
    {
        public HoSoKhacGiayChungSinhMappingProfile()
        {
            CreateMap<NoiTruHoSoKhac, GiayChungSinhViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    var thongTinHoSo = JsonConvert.DeserializeObject<GiayChungSinhViewModel>(d.ThongTinHoSo);
                    d.So = thongTinHoSo.So;
                    d.QuyenSo = thongTinHoSo.QuyenSo;
                    d.HoTenCha = thongTinHoSo.HoTenCha;
                    d.GhiChu = thongTinHoSo.GhiChu;
                    d.NhanVienDoDeId = thongTinHoSo.NhanVienDoDeId;
                    d.NhanVienGhiPhieuId = thongTinHoSo.NhanVienGhiPhieuId;
                    d.GiamDocChuyenMonId = thongTinHoSo.GiamDocChuyenMonId;
                    d.ThoiDiemThucHienDisplay = d.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH();
                    d.HoSoKhacTreSoSinhs = thongTinHoSo.HoSoKhacTreSoSinhs;
                });

            CreateMap<GiayChungSinhViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
               .ForMember(d => d.NoiTruHoSoKhacFileDinhKems, o => o.Ignore());
            #region BVHD-3705

            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.GiayChungSinhNewViewModel>().IgnoreAllNonExisting();
            CreateMap<DieuTriNoiTru.GiayChungSinhNewViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting();
            
            #endregion
        }
    }
}

