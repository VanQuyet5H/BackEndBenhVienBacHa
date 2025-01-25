using System;
using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhNhans;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.KhamDoan;
using Camino.Api.Models.TiemChung;
using Camino.Api.Models.YeuCauTiepNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanLichSuChuyenDoiTuongs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauTiepNhanMappingProfile : Profile
    {
        public YeuCauTiepNhanMappingProfile()
        {
            CreateMap<YeuCauTiepNhanViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, YeuCauTiepNhanViewModel>()
                .IgnoreAllNonExisting()
                .AfterMap((source, dest) =>
            {
                dest.DiaChi = source.DiaChiDayDu;
                dest.SoDienThoai = source.SoDienThoai.ApplyFormatPhone();
            });
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, BenhNhanViewModel>()
                .IgnoreAllNonExisting()
                .AfterMap((source, dest) =>
                {
                    dest.MaBN = source.BenhNhan != null ? source.BenhNhan.MaBN : "";
                    dest.DiaChi = source.DiaChiDayDu;
                    dest.SoDienThoai = source.SoDienThoai.ApplyFormatPhone();
                });

            CreateMap<ThongTinDoiTuongTiepNhanViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauTiepNhanTheBHYTs, o => o.Ignore())
                .ForMember(x => x.YeuCauTiepNhanCongTyBaoHiemTuNhans, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    if (d.NgayThangNamSinh != null)
                    {
                        s.NgaySinh = d.NgayThangNamSinh.Value.Day;
                        s.ThangSinh = d.NgayThangNamSinh.Value.Month;
                        s.NamSinh = d.NgayThangNamSinh.Value.Year;
                    }

                    AddOrUpdateTheBHYT(d, s);
                    AddOrUpdateCongTyBaoHiemTuNhan(d, s);
                });
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, ThongTinDoiTuongTiepNhanViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauTiepNhanTheBHYTs, o => o .MapFrom(y => y.YeuCauTiepNhanTheBHYTs))
                .ForMember(x => x.YeuCauTiepNhanCongTyBaoHiemTuNhans, o => o.MapFrom(y => y.YeuCauTiepNhanCongTyBaoHiemTuNhans))
                .AfterMap((s, d) =>
                {
                    if (s.ThangSinh != 0 && s.ThangSinh != null && s.NamSinh != null)
                    {
                        d.NgayThangNamSinh = new DateTime(s.NamSinh ?? 0, s.ThangSinh ?? 0, s.NgaySinh ?? 0);
                    }

                    d.YeuCauTiepNhanMeId = s.YeuCauNhapVien?.YeuCauTiepNhanMeId;
                    d.YeuCauGoiDichVuId = s.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() ? s.BenhNhan.YeuCauGoiDichVuSoSinhs.First().Id : (long?) null;
                });
            
            CreateMap<BenhAnSoSinhChiTietViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.HoTen = string.IsNullOrEmpty(d.TenKhaiSinh) ? d.TenBanDau : d.TenKhaiSinh;
                    s.NgaySinh = d.NgayThangNamSinh?.Day;
                    s.ThangSinh = d.NgayThangNamSinh?.Month;
                    s.NamSinh = d.NgayThangNamSinh?.Year;
                    s.GioSinh = d.NgayThangNamSinh != null ? (d.NgayThangNamSinh.Value.TimeOfDay.Hours * 3600 + d.NgayThangNamSinh.Value.TimeOfDay.Minutes * 60) : (int?)null;
                    s.ThoiDiemTiepNhan = d.NgayThangNamSinh ?? DateTime.Now;
                });
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, BenhAnSoSinhChiTietViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.NgayThangNamSinh = (d.NgaySinh != null && d.ThangSinh != null && d.NamSinh != null) ? 
                        new DateTime(d.NamSinh.Value, d.ThangSinh.Value, d.NgaySinh.Value) + (d.GioSinh != null ? TimeSpan.FromSeconds(d.GioSinh.Value) : new TimeSpan()) : (DateTime?)null;
                });

            CreateMap<NoiTruYeuCauTiepNhanTheBHYTViewModel, YeuCauTiepNhanTheBHYT>().IgnoreAllNonExisting()
                .AfterMap((d, s) => { s.DuocMienCungChiTra = d.GiayMienCungChiTra != null; });
            CreateMap<YeuCauTiepNhanTheBHYT, NoiTruYeuCauTiepNhanTheBHYTViewModel>().IgnoreAllNonExisting();

            CreateMap<NoiTruGiayMienCungChiTraViewModel, GiayMienCungChiTra>().IgnoreAllNonExisting()
                .BeforeMap((d, s) =>
                {
                    if (s.Id != 0 && d.Id == 0)
                    {
                        d.Id = s.Id;
                    }
                })
                .AfterMap((d, s) =>
                {
                    if (string.IsNullOrEmpty(d.Ma))
                    {
                        s.Ma = Guid.NewGuid().ToString();
                    }
                });
            CreateMap<GiayMienCungChiTra, NoiTruGiayMienCungChiTraViewModel>().IgnoreAllNonExisting();

            CreateMap<NoiTruYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel, YeuCauTiepNhanCongTyBaoHiemTuNhan>().IgnoreAllNonExisting();
            CreateMap<YeuCauTiepNhanCongTyBaoHiemTuNhan, NoiTruYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                    {
                        d.TenCongTyBaoHiemTuNhan = s.CongTyBaoHiemTuNhan?.Ten;
                    });

            #region Khám sức khỏe
            CreateMap<YeuCauTiepNhanKhamSucKhoeViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, YeuCauTiepNhanKhamSucKhoeViewModel>().IgnoreAllNonExisting();
            #endregion
            #region khám đoàn kết luận cls
            CreateMap<KhamDoanKetLuanCLSViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, KhamDoanKetLuanCLSViewModel>().IgnoreAllNonExisting();
            #endregion

            #region Khám bệnh

            CreateMap<KhamBenhThongTinDoiTuongViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauTiepNhanCongTyBaoHiemTuNhans, x => x.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.NgayThangNamSinh != null)
                    {
                        d.NgaySinh = s.NgayThangNamSinh.Value.Day;
                        d.ThangSinh = s.NgayThangNamSinh.Value.Month;
                        d.NamSinh = s.NgayThangNamSinh.Value.Year;
                    }
                    AddOrUpdateCongTyBaoHiemTuNhanKhamBenh(s,d);
                });
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, KhamBenhThongTinDoiTuongViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauTiepNhanCongTyBaoHiemTuNhans, x => x.MapFrom(y => y.YeuCauTiepNhanCongTyBaoHiemTuNhans))
                .ForMember(x => x.BHYTGiayMienCungChiTra, x => x.MapFrom(y => y.BHYTGiayMienCungChiTra))
                .AfterMap((d, s) =>
                {
                    if (d.ThangSinh != 0 && d.ThangSinh != null && d.NamSinh != null)
                    {
                        s.NgayThangNamSinh = new DateTime(s.NamSinh ?? 0, s.ThangSinh ?? 0, s.NgaySinh ?? 0);
                    }
                });

            CreateMap<KhamBenhYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel, YeuCauTiepNhanCongTyBaoHiemTuNhan>().IgnoreAllNonExisting();
            CreateMap<YeuCauTiepNhanCongTyBaoHiemTuNhan, KhamBenhYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.TenCongTyBaoHiemTuNhan = d.CongTyBaoHiemTuNhan?.Ten;
                });

            CreateMap<KhamBenhGiayMienCungChiTraViewModel, GiayMienCungChiTra>().IgnoreAllNonExisting();
            CreateMap<GiayMienCungChiTra, KhamBenhGiayMienCungChiTraViewModel>().IgnoreAllNonExisting();
            #endregion
        }

        private void AddOrUpdateTheBHYT(ThongTinDoiTuongTiepNhanViewModel viewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan model)
        {
            //todo: cần kiểm tra lại khi function nhập viện cập nhật xong
            #region phần xử lý này ở chức năng nhập viện phải xử lý
            if (!model.YeuCauTiepNhanTheBHYTs.Any() && !model.YeuCauTiepNhanLichSuChuyenDoiTuongs.Any() && viewModel.YeuCauTiepNhanTheBHYTs.Any())
            {
                // thêm lịch sử chuyển đối tượng
                model.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
                {
                    DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.DichVu
                });
            }
            #endregion

            foreach (var item in viewModel.YeuCauTiepNhanTheBHYTs)
            {
                if (item.Id == 0)
                {
                    var theBHYTMoiEntity = new YeuCauTiepNhanTheBHYT();
                    model.YeuCauTiepNhanTheBHYTs.Add(item.ToEntity(theBHYTMoiEntity));

                    // thêm lịch sử chuyển đối tượng
                    model.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
                    {
                        DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.BaoHiem,
                        MaSoThe = item.MaSoThe,
                        DiaChi = item.DiaChi,
                        MucHuong = item.MucHuong.Value,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayHetHan = item.NgayHetHan,
                        MaDKBD = item.MaDKBD,
                        NgayDuocMienCungChiTra = item.NgayDuocMienCungChiTra,
                        DuocGiaHanThe = item.DuocGiaHanThe,
                        DuocMienCungChiTra = item.GiayMienCungChiTra != null,
                        GiayMienCungChiTra = theBHYTMoiEntity.GiayMienCungChiTra,
                        NgayDu5Nam = item.NgayDu5Nam,
                        TinhTrangThe = item.TinhTrangThe,
                        MaKhuVuc = item.MaKhuVuc,
                        IsCheckedBHYT = item.IsCheckedBHYT
                    });
                }
                else
                {
                    var flag = false;
                    var result = model.YeuCauTiepNhanTheBHYTs.Single(p => p.Id == item.Id);
                    if (!result.MaSoThe.Equals(item.MaSoThe))
                    {
                        flag = true;
                    }
                    result = item.ToEntity(result);


                    // thêm lịch sử chuyển đối tượng
                    if (flag)
                    {
                        model.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
                        {
                            DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.BaoHiem,
                            MaSoThe = item.MaSoThe,
                            DiaChi = item.DiaChi,
                            MucHuong = item.MucHuong.Value,
                            NgayHieuLuc = item.NgayHieuLuc,
                            NgayHetHan = item.NgayHetHan,
                            MaDKBD = item.MaDKBD,
                            NgayDuocMienCungChiTra = item.NgayDuocMienCungChiTra,
                            DuocGiaHanThe = item.DuocGiaHanThe,
                            DuocMienCungChiTra = item.GiayMienCungChiTra != null,
                            GiayMienCungChiTraId = item.GiayMienCungChiTraId != 0 ? item.GiayMienCungChiTraId : null,
                            GiayMienCungChiTra = item.GiayMienCungChiTraId != 0 ? result.GiayMienCungChiTra : null,
                            NgayDu5Nam = item.NgayDu5Nam,
                            TinhTrangThe = item.TinhTrangThe,
                            MaKhuVuc = item.MaKhuVuc,
                            IsCheckedBHYT = item.IsCheckedBHYT
                        });
                    }
                }
            }
            foreach (var item in model.YeuCauTiepNhanTheBHYTs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauTiepNhanTheBHYTs.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;

                        model.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
                        {
                            DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.BaoHiem,
                            MaSoThe = item.MaSoThe,
                            DiaChi = item.DiaChi,
                            MucHuong = item.MucHuong,
                            NgayHieuLuc = item.NgayHieuLuc,
                            NgayHetHan = item.NgayHetHan,
                            MaDKBD = item.MaDKBD,
                            NgayDuocMienCungChiTra = item.NgayDuocMienCungChiTra,
                            DuocGiaHanThe = item.DuocGiaHanThe,
                            DuocMienCungChiTra = item.GiayMienCungChiTra != null,
                            GiayMienCungChiTraId = item.GiayMienCungChiTraId,
                            NgayDu5Nam = item.NgayDu5Nam,
                            TinhTrangThe = item.TinhTrangThe,
                            MaKhuVuc = item.MaKhuVuc,
                            IsCheckedBHYT = item.IsCheckedBHYT,
                            DaHuy = true
                        });
                    }
                }
            }

            if (model.YeuCauTiepNhanTheBHYTs.Any() && model.YeuCauTiepNhanTheBHYTs.All(x => x.WillDelete))
            {
                model.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
                {
                    DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.DichVu
                });
            }

            // bổ sung xử lý cập nhật thông tin thẻ có mức hưởng cao nhất vào yêu cầu tiếp nhận nội trú hiện tại
            // BVHD-3754: cập nhật ưu tiên lấy thẻ BHYT có mức hưởng cao nhất còn hiệu lực, nếu tất cả thẻ hết hiệu lực lấy thẻ mức hưởng cao nhất
            var theBHYTMucHuongCaoNhat = model.YeuCauTiepNhanTheBHYTs
                .Where(x => x.WillDelete != true 
                            && x.NgayHieuLuc.Date <= DateTime.Now.Date 
                            && (x.NgayHetHan == null || x.NgayHetHan.Value.Date >= DateTime.Now.Date || (x.DuocGiaHanThe == true && (DateTime.Now.Date - x.NgayHetHan.Value.Date).Days <= 15)))
                .OrderByDescending(x => x.MucHuong).FirstOrDefault();

            if (theBHYTMucHuongCaoNhat == null)
            {
                theBHYTMucHuongCaoNhat = model.YeuCauTiepNhanTheBHYTs.Where(x => x.WillDelete != true).OrderByDescending(x => x.MucHuong).FirstOrDefault();
            }
            if (theBHYTMucHuongCaoNhat != null)
            {
                model.BHYTMaSoThe = theBHYTMucHuongCaoNhat.MaSoThe;
                model.BHYTDiaChi = theBHYTMucHuongCaoNhat.DiaChi;
                model.BHYTMucHuong = theBHYTMucHuongCaoNhat.MucHuong;
                model.BHYTNgayHieuLuc = theBHYTMucHuongCaoNhat.NgayHieuLuc;
                model.BHYTNgayHetHan = theBHYTMucHuongCaoNhat.NgayHetHan;
                model.BHYTMaDKBD = theBHYTMucHuongCaoNhat.MaDKBD;
                model.BHYTNgayDuocMienCungChiTra = theBHYTMucHuongCaoNhat.NgayDuocMienCungChiTra;
                model.BHYTDuocMienCungChiTra = theBHYTMucHuongCaoNhat.DuocMienCungChiTra;
                model.BHYTGiayMienCungChiTraId = theBHYTMucHuongCaoNhat.GiayMienCungChiTraId;
                model.BHYTGiayMienCungChiTra = theBHYTMucHuongCaoNhat.GiayMienCungChiTra;
                model.BHYTNgayDu5Nam = theBHYTMucHuongCaoNhat.NgayDu5Nam;
                model.TinhTrangThe = theBHYTMucHuongCaoNhat.TinhTrangThe;
                model.BHYTMaKhuVuc = theBHYTMucHuongCaoNhat.MaKhuVuc;
                model.IsCheckedBHYT = theBHYTMucHuongCaoNhat.IsCheckedBHYT;

                model.BHYTCoQuanBHXH = theBHYTMucHuongCaoNhat.CoQuanBHXH;
            }
        }

        private void AddOrUpdateCongTyBaoHiemTuNhan(ThongTinDoiTuongTiepNhanViewModel viewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan model)
        {
            foreach (var item in viewModel.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            {
                if (item.Id == 0)
                {
                    var congTyBHTNEntity = new YeuCauTiepNhanCongTyBaoHiemTuNhan();
                    model.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(item.ToEntity(congTyBHTNEntity));
                }
                else
                {
                    var result = model.YeuCauTiepNhanCongTyBaoHiemTuNhans.Single(p => p.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in model.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }

            }
        }

        #region Khám bệnh
        private void AddOrUpdateCongTyBaoHiemTuNhanKhamBenh(KhamBenhThongTinDoiTuongViewModel viewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan model)
        {
            foreach (var item in viewModel.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            {
                if (item.Id == 0)
                {
                    var congTyBHTNEntity = new YeuCauTiepNhanCongTyBaoHiemTuNhan();
                    model.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(item.ToEntity(congTyBHTNEntity));
                }
                else
                {
                    var result = model.YeuCauTiepNhanCongTyBaoHiemTuNhans.Single(p => p.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in model.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }
            }
        }


        #endregion
    }
}
