using System.Linq;
using AutoMapper;
using Camino.Api.Models.KhoaPhong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoaPhong;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class KhoaPhongMappingProfile : Profile
    {
        public KhoaPhongMappingProfile()
        {
            CreateMap<Core.Domain.Entities.KhoaPhongs.KhoaPhong, KhoaPhongViewModel>()
                .AfterMap((source, dest) =>
                    {
                        dest.KhoaIds = source.KhoaPhongChuyenKhoas.Select(r => r.KhoaId).ToList();
                        dest.TenLoaiKhoaPhong = source.LoaiKhoaPhong.GetDescription();
                        dest.CoKhamNgoaiTru = source.CoKhamNgoaiTru;
                        dest.CoKhamNoiTru = source.CoKhamNoiTru;
                        dest.KieuKhamDisplay = source.CoKhamNgoaiTru == true ? "Ngoại Trú" : "Nội Trú";
                        dest.SoTienThuTamUng = source.SoTienThuTamUng ?? 0;
                    });

            CreateMap<KhoaPhongViewModel, Core.Domain.Entities.KhoaPhongs.KhoaPhong>()
                .ForMember(x => x.KhoaPhongChuyenKhoas, o => o.Ignore())
                .ForMember(x => x.DichVuKhamBenhBenhVienNoiThucHiens, o => o.Ignore())
                .AfterMap((c, d) =>
                {
                    foreach (var model in d.KhoaPhongChuyenKhoas)
                    {
                        if (c.KhoaPhongChuyenKhoas.All(x => x.KhoaId != 0))
                        {
                            model.WillDelete = true;
                        }
                    }

                    foreach (var item in c.KhoaIds)
                    {

                        if (item != 0)
                        {
                            var entity = new Core.Domain.Entities.KhoaPhongChuyenKhoas.KhoaPhongChuyenKhoa()
                            {
                                KhoaId = item,
                            };
                            d.KhoaPhongChuyenKhoas.Add(entity);
                        }

                    }

                });

            CreateMap<KhoaPhongGridVo, KhoaPhongExportExcel>()
                .AfterMap((source, dest) =>
                {
                    dest.CoKhamNgoaiTru = ProcessResultForKieuKham(source.CoKhamNgoaiTru,source.CoKhamNoiTru);
                    dest.IsDisabled = source.IsDisabled != true ? "Đang sử dụng" : "Ngừng sử dụng";
                    dest.SoTienThuTamUng = source.SoTienThuTamUng.GetValueOrDefault().ApplyFormatMoneyVND();
                });
        }

        private string ProcessResultForKieuKham(bool? khamNgoaiTru, bool? khamNoiTru)
        {
            if (khamNgoaiTru == null && khamNoiTru == null)
            {
                return string.Empty;
            }
            var result = string.Empty;
            if (khamNoiTru == true)
            {
                result += "Nội Trú";
            }
            if (khamNgoaiTru == true)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += "; ";
                }
                result += "Ngoại Trú";
            }
            return result;
        }
    }
}
