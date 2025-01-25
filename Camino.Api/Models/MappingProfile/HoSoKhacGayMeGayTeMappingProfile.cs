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
    public class HoSoKhacGayMeGayTeMappingProfile : Profile
    {
        public HoSoKhacGayMeGayTeMappingProfile()
        {
            //Cam Kết Gây Tê Gây Mê
            CreateMap<NoiTruHoSoKhac, HoSoKhacGayMeGayTeViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenNhanVienThucHien = s.NhanVienThucHien.User.HoTen;
                });

            CreateMap<HoSoKhacGayMeGayTeViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
               .ForMember(d => d.NoiTruHoSoKhacFileDinhKems, o => o.Ignore())
               .AfterMap((d, s) =>
               {
                   AddOrUpdateFileDinhKem(d, s);
               });


            CreateMap<NoiTruHoSoKhacFileDinhKem, NoiTruHoSoKhacFileDinhKemViewModel>().IgnoreAllNonExisting();
            CreateMap<NoiTruHoSoKhacFileDinhKemViewModel, NoiTruHoSoKhacFileDinhKem>().IgnoreAllNonExisting();

            CreateMap<LuuTruHoSoGridVo, LuuTruHoSoExportExcel>().IgnoreAllNonExisting()
               ;

            //Tiêm Chủng Trẻ Em
            CreateMap<NoiTruHoSoKhac, HoSoKhacBanKiemTiemChungTreEmViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    var thongTinHoSo = JsonConvert.DeserializeObject<HoSoKhacBanKiemTiemChungTreEmViewModel>(d.ThongTinHoSo);
                    d.DuocPhamIds = thongTinHoSo.DuocPhamIds;
                    d.SotHaThanNhiet = thongTinHoSo.SotHaThanNhiet;
                    d.NgheTimBatThuong = thongTinHoSo.NgheTimBatThuong;
                    d.NghePhoiBatThuong = thongTinHoSo.NghePhoiBatThuong;
                    d.TriGiacBatThuong = thongTinHoSo.TriGiacBatThuong;
                    d.CanNangDuoi2000g = thongTinHoSo.CanNangDuoi2000g;
                    d.CoCacChongChiDinhKhac = thongTinHoSo.CoCacChongChiDinhKhac;
                    d.DuDieuKienTiemChung = thongTinHoSo.DuDieuKienTiemChung;
                    d.TamHoanTiemChung = thongTinHoSo.TamHoanTiemChung;
                    d.TenNhanVienThucHien = s.NhanVienThucHien.User.HoTen;
                    d.ThoiDiemThucHienDisplay = d.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH();
                });

            CreateMap<HoSoKhacBanKiemTiemChungTreEmViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
               .ForMember(d => d.NoiTruHoSoKhacFileDinhKems, o => o.Ignore())
               .AfterMap((d, s) =>
               {
                   AddOrUpdateFileDinhKemTiemChung(d, s);
               });
        }

        private void AddOrUpdateFileDinhKem(HoSoKhacGayMeGayTeViewModel viewModel, NoiTruHoSoKhac entity)
        {
            foreach (var item in viewModel.NoiTruHoSoKhacFileDinhKems)
            {
                if (item.Id == 0)
                {
                    var newEntity = new NoiTruHoSoKhacFileDinhKem();
                    entity.NoiTruHoSoKhacFileDinhKems.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.NoiTruHoSoKhacFileDinhKems.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.NoiTruHoSoKhacFileDinhKems)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.NoiTruHoSoKhacFileDinhKems.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }

        private void AddOrUpdateFileDinhKemTiemChung(HoSoKhacBanKiemTiemChungTreEmViewModel viewModel, NoiTruHoSoKhac entity)
        {
            foreach (var item in viewModel.NoiTruHoSoKhacFileDinhKems)
            {
                if (item.Id == 0)
                {
                    var newEntity = new NoiTruHoSoKhacFileDinhKem();
                    entity.NoiTruHoSoKhacFileDinhKems.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.NoiTruHoSoKhacFileDinhKems.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.NoiTruHoSoKhacFileDinhKems)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.NoiTruHoSoKhacFileDinhKems.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
