using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.MappingProfile
{
    public class TheoDoiSauPhauThuatThuThuatMappingProfile : Profile
    {
        public TheoDoiSauPhauThuatThuThuatMappingProfile()
        {
            CreateMap<TheoDoiSauPhauThuatThuThuat, TheoDoiSauPhauThuatThuThuatViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.BacSiPhuTrachTheoDoiDisplay, o => o.MapFrom(p2 => p2.BacSiPhuTrachTheoDoi.User.HoTen))
                .ForMember(p => p.DieuDuongPhuTrachTheoDoiDisplay, o => o.MapFrom(p2 => p2.DieuDuongPhuTrachTheoDoi.User.HoTen));
            CreateMap<TheoDoiSauPhauThuatThuThuatViewModel, TheoDoiSauPhauThuatThuThuat>().IgnoreAllNonExisting();

            CreateMap<EkipFormViewModel, YeuCauDichVuKyThuat>()
                .ForMember(x => x.Id, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.YeuCauDichVuKyThuatTuongTrinhPTTT != null)
                    {
                        if (d.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId != s.ICDTruocId.GetValueOrDefault())
                        {
                            d.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId = s.ICDTruocId.GetValueOrDefault();
                        }

                        if (d.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat != s.ThoiGianPt)
                        {
                            d.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat = s.ThoiGianPt;
                        }

                        if (s.Ekips.Any())
                        {
                            MapBacSi(s, d.YeuCauDichVuKyThuatTuongTrinhPTTT);
                        }

                        return;
                    }

                    d.YeuCauDichVuKyThuatTuongTrinhPTTT = new YeuCauDichVuKyThuatTuongTrinhPTTT
                    {
                        ICDTruocPhauThuatId = s.ICDTruocId.GetValueOrDefault(),
                        ThoiDiemPhauThuat = s.ThoiGianPt
                    };
                    if (s.Ekips.Any())
                    {
                        MapBacSi(s, d.YeuCauDichVuKyThuatTuongTrinhPTTT);
                    }
                });
        }

        private void MapBacSi(EkipFormViewModel s, YeuCauDichVuKyThuatTuongTrinhPTTT d)
        {
            //foreach (var bacSi in s.Ekips.Where(w => w.VaiTroBacSi != null))
            //{
            //    if (d.PhauThuatThuThuatEkipBacSis.Any(e => e.Id == bacSi.IdDb && bacSi.IdDb != 0))
            //    {
            //        var ekipBacSiEntity = d.PhauThuatThuThuatEkipBacSis.FirstOrDefault(e => e.Id == bacSi.IdDb);

            //        if (ekipBacSiEntity != null)
            //        {
            //            if (ekipBacSiEntity.NhanVienId != bacSi.BacSiId.GetValueOrDefault())
            //            {
            //                ekipBacSiEntity.NhanVienId = bacSi.BacSiId.GetValueOrDefault();
            //            }

            //            if (ekipBacSiEntity.VaiTroBacSi != bacSi.VaiTroBacSi)
            //            {
            //                ekipBacSiEntity.VaiTroBacSi = bacSi.VaiTroBacSi.GetValueOrDefault();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var ekipBacSiAddEntity = new PhauThuatThuThuatEkipBacSi
            //        {
            //            Id = 0,
            //            YeuCauDichVuKyThuatTuongTrinhPTTTId = s.YcdvktId,
            //            NhanVienId = bacSi.BacSiId.GetValueOrDefault(),
            //            VaiTroBacSi = bacSi.VaiTroBacSi.GetValueOrDefault()
            //        };
            //        d.PhauThuatThuThuatEkipBacSis.Add(ekipBacSiAddEntity);
            //    }
            //}

            //foreach (var dieuDuong in s.Ekips.Where(w => w.VaiTroDieuDuong != null))
            //{
            //    if (d.PhauThuatThuThuatEkipDieuDuongs.Any(e => e.Id == dieuDuong.IdDb && dieuDuong.IdDb != 0))
            //    {
            //        var ekipDieuDuongEntity = d.PhauThuatThuThuatEkipDieuDuongs.FirstOrDefault(e => e.Id == dieuDuong.IdDb);

            //        if (ekipDieuDuongEntity != null)
            //        {
            //            if (ekipDieuDuongEntity.NhanVienId != dieuDuong.BacSiId.GetValueOrDefault())
            //            {
            //                ekipDieuDuongEntity.NhanVienId = dieuDuong.BacSiId.GetValueOrDefault();
            //            }

            //            if (ekipDieuDuongEntity.VaiTroDieuDuong != dieuDuong.VaiTroDieuDuong)
            //            {
            //                ekipDieuDuongEntity.VaiTroDieuDuong = dieuDuong.VaiTroDieuDuong.GetValueOrDefault();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var ekipDieuDuongAddEntity = new PhauThuatThuThuatEkipDieuDuong
            //        {
            //            Id = 0,
            //            YeuCauDichVuKyThuatTuongTrinhPTTTId = s.YcdvktId,
            //            NhanVienId = dieuDuong.BacSiId.GetValueOrDefault(),
            //            VaiTroDieuDuong = dieuDuong.VaiTroDieuDuong.GetValueOrDefault()
            //        };
            //        d.PhauThuatThuThuatEkipDieuDuongs.Add(ekipDieuDuongAddEntity);
            //    }
            //}
            var modifyIds = new List<long>();

            foreach (var bacSi in s.Ekips.Where(w => w.NhomChucDanh == EnumNhomChucDanh.BacSi && w.IdDb == 0))
            {
                var ekipBacSiAddEntity = new PhauThuatThuThuatEkipBacSi
                {
                    Id = 0,
                    YeuCauDichVuKyThuatTuongTrinhPTTTId = s.YcdvktId,
                    NhanVienId = bacSi.BacSiId.GetValueOrDefault(),
                    VaiTroBacSi = bacSi.VaiTroBacSi.GetValueOrDefault()
                };
                d.PhauThuatThuThuatEkipBacSis.Add(ekipBacSiAddEntity);
            }

            foreach (var bacSi in s.Ekips.Where(w => w.NhomChucDanh == EnumNhomChucDanh.BacSi && w.IdDb != 0))
            {
                if (d.PhauThuatThuThuatEkipBacSis.Any(e => e.Id == bacSi.IdDb && e.Id != 0))
                {
                    var ekipBacSiEntity = d.PhauThuatThuThuatEkipBacSis.FirstOrDefault(e => e.Id == bacSi.IdDb);

                    if (ekipBacSiEntity != null)
                    {
                        if (ekipBacSiEntity.NhanVienId != bacSi.BacSiId.GetValueOrDefault())
                        {
                            ekipBacSiEntity.NhanVienId = bacSi.BacSiId.GetValueOrDefault();
                        }

                        if (ekipBacSiEntity.VaiTroBacSi != bacSi.VaiTroBacSi)
                        {
                            ekipBacSiEntity.VaiTroBacSi = bacSi.VaiTroBacSi.GetValueOrDefault();
                        }
                        modifyIds.Add(ekipBacSiEntity.Id);
                    }
                }
            }

            var bacSiEntityCoSanDb =  d.PhauThuatThuThuatEkipBacSis.Select(e => e.Id);
            var ids = modifyIds;
            var deleteBacSi = bacSiEntityCoSanDb.Where(q => !ids.Contains(q) && q != 0);

            foreach (var bacSiDeleteId in deleteBacSi)
            {
                var bacSi = d.PhauThuatThuThuatEkipBacSis.FirstOrDefault(e => e.Id == bacSiDeleteId);
                if (bacSi != null)
                {
                    bacSi.WillDelete = true;
                }
            }

            modifyIds = new List<long>();

            foreach (var dieuDuong in s.Ekips.Where(w => w.NhomChucDanh == EnumNhomChucDanh.DieuDuong && w.IdDb == 0))
            {
                var ekipDieuDuongAddEntity = new PhauThuatThuThuatEkipDieuDuong
                {
                    Id = 0,
                    YeuCauDichVuKyThuatTuongTrinhPTTTId = s.YcdvktId,
                    NhanVienId = dieuDuong.BacSiId.GetValueOrDefault(),
                    VaiTroDieuDuong = dieuDuong.VaiTroDieuDuong.GetValueOrDefault()
                };
                d.PhauThuatThuThuatEkipDieuDuongs.Add(ekipDieuDuongAddEntity);
            }

            foreach (var dieuDuong in s.Ekips.Where(w => w.NhomChucDanh == EnumNhomChucDanh.DieuDuong && w.IdDb != 0))
            {
                if (d.PhauThuatThuThuatEkipDieuDuongs.Any(e => e.Id == dieuDuong.IdDb && e.Id != 0))
                {
                    var ekipDieuDuongEntity = d.PhauThuatThuThuatEkipDieuDuongs.FirstOrDefault(e => e.Id == dieuDuong.IdDb);

                    if (ekipDieuDuongEntity != null)
                    {
                        if (ekipDieuDuongEntity.NhanVienId != dieuDuong.BacSiId.GetValueOrDefault())
                        {
                            ekipDieuDuongEntity.NhanVienId = dieuDuong.BacSiId.GetValueOrDefault();
                        }

                        if (ekipDieuDuongEntity.VaiTroDieuDuong != dieuDuong.VaiTroDieuDuong)
                        {
                            ekipDieuDuongEntity.VaiTroDieuDuong = dieuDuong.VaiTroDieuDuong.GetValueOrDefault();
                        }
                        modifyIds.Add(ekipDieuDuongEntity.Id);
                    }
                }
            }

            var dieuDuongEntityCoSanDb = d.PhauThuatThuThuatEkipDieuDuongs.Select(e => e.Id);
            var idsDieuDuong = modifyIds;
            var deleteDieuDuong = dieuDuongEntityCoSanDb.Where(q => !idsDieuDuong.Contains(q) && q != 0);

            foreach (var dieuDuongDeleteId in deleteDieuDuong)
            {
                var bacSi = d.PhauThuatThuThuatEkipDieuDuongs.FirstOrDefault(e => e.Id == dieuDuongDeleteId);
                if (bacSi != null)
                {
                    bacSi.WillDelete = true;
                }
            }
        }
    }
}
