using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<NgayDieuTri> GetNgayDieuTriTamThoi(CreateNewDateModel createNewDateModel)
        {
            var result = new NgayDieuTri();
            var LstYear = new List<YearModel>();
            var LstYearOrderBy = new List<LstYearOrderByModel>();
            //var yctn = await GetYeuCauTiepNhanWithIncludeUpdate(createNewDateModel.yeuCauTiepNhanId);

            var noiTruBenhAn = _noiTruBenhAnRepository.GetById(createNewDateModel.yeuCauTiepNhanId, x => x.Include(o=>o.YeuCauTiepNhan).Include(o => o.NoiTruPhieuDieuTris));

            var noiTruPhieuDieuTris = noiTruBenhAn.NoiTruPhieuDieuTris;


            var yctnNhapVien = BaseRepository.TableNoTracking
                           .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens)
                           .Where(z => z.Id == noiTruBenhAn.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();
            //Lấy DS tất cả năm
            //var noiTruPhieuDieuTris = yctn.NoiTruBenhAn.NoiTruPhieuDieuTris;
            createNewDateModel.Dates = createNewDateModel.Dates.Where(z => !noiTruPhieuDieuTris.Any(x => z == x.NgayDieuTri && x.KhoaPhongDieuTriId == createNewDateModel.KhoaId)).ToList();
            var lstYear = createNewDateModel.Dates.Select(o => o.Year).Distinct().ToList();
            var days = new List<DayModel>();
            foreach (var item in noiTruPhieuDieuTris)
            {
                LstYearOrderBy.Add(new LstYearOrderByModel
                {
                    Date = item.NgayDieuTri.Date,
                    LaNgayDieuTriTamThoi = false,
                    KhoaId = item.KhoaPhongDieuTriId,
                    PhieuDieuTriId = item.Id
                });
                if (lstYear.Count == 0 || !lstYear.Any(z => z == item.NgayDieuTri.Year))
                {
                    lstYear.Add(item.NgayDieuTri.Year);
                }
                var d = new DayModel
                {
                    Day = item.NgayDieuTri.Day,
                    Year = item.NgayDieuTri.Year,
                    Month = item.NgayDieuTri.Month,
                    FullDate = new DateTime(item.NgayDieuTri.Year, item.NgayDieuTri.Month, item.NgayDieuTri.Day),
                    FullDateDisplay = new DateTime(item.NgayDieuTri.Year, item.NgayDieuTri.Month, item.NgayDieuTri.Day).ApplyFormatDate(),
                    KhoaId = item.KhoaPhongDieuTriId,
                    TenKhoa = _khoaPhongRepository.TableNoTracking.Where(z => z.Id == item.KhoaPhongDieuTriId).Select(z => z.Ten).FirstOrDefault(),
                    PhieuDieuTriId = item.Id,
                    LaNgayDieuTriTamThoi = false
                };
                days.Add(d);
            }
            var phieuDieuTriId = 99999990;
            foreach (var ngayDieuTri in createNewDateModel.Dates)
            {
                if (noiTruBenhAn.ThoiDiemNhapVien != null && ngayDieuTri.Date < noiTruBenhAn.ThoiDiemNhapVien.Date)
                {
                    var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                    throw new Exception(string.Format(errMess, noiTruBenhAn.ThoiDiemNhapVien.ApplyFormatDate()));
                }

                LstYearOrderBy.Add(new LstYearOrderByModel
                {
                    Date = ngayDieuTri.Date,
                    LaNgayDieuTriTamThoi = true,
                    KhoaId = createNewDateModel.KhoaId.GetValueOrDefault(),
                    PhieuDieuTriId = phieuDieuTriId
                });
                phieuDieuTriId = phieuDieuTriId + 1;
            }

            var phieuDieuTriIdOther = 99999990;
            var tenKhoa = await _khoaPhongRepository.TableNoTracking.Where(z => z.Id == createNewDateModel.KhoaId).Select(z => z.Ten).FirstOrDefaultAsync();
            foreach (var year in lstYear.OrderBy(o => o))
            {
                var r = new YearModel
                {
                    Year = year,
                };
                var lstMonth = LstYearOrderBy.Where(o => o.Date.Year == year).Select(o => o.Date.Month).Distinct().ToList();
                foreach (var month in lstMonth.OrderBy(o => o))
                {
                    var monthModel = new MonthModel
                    {
                        Month = month,
                    };
                    monthModel.Days.AddRange(days.Where(z => z.Month == month && z.Year == year).ToList());
                    //Lấy DS tất cả ngày của tháng của năm
                    foreach (var ngayDieuTri in createNewDateModel.Dates.Where(o => o.Year == year && o.Month == month).OrderBy(o => o))
                    {
                        var d = new DayModel
                        {
                            Day = ngayDieuTri.Day,
                            Year = year,
                            Month = month,
                            FullDate = new DateTime(year, month, ngayDieuTri.Day),
                            FullDateDisplay = new DateTime(year, month, ngayDieuTri.Day).ApplyFormatDate(),
                            KhoaId = createNewDateModel.KhoaId.GetValueOrDefault(),
                            TenKhoa = tenKhoa,
                            PhieuDieuTriId = phieuDieuTriIdOther,
                            LaNgayDieuTriTamThoi = true
                        };
                        monthModel.Days.Add(d);
                        phieuDieuTriIdOther = phieuDieuTriIdOther + 1;
                    }
                    var dayOrders = monthModel.Days.OrderBy(z => z.Day).ToList();
                    monthModel.Days = dayOrders;
                    r.Months.Add(monthModel);
                }

                LstYear.Add(r);
            }

            result.LstYear = LstYear;
            result.LstYearOrderBy = LstYearOrderBy.OrderBy(z => z.Date).ToList();
            return result;
        }
        public async Task<NgayDieuTri> GetNgayDieuTriTamThoiOld(CreateNewDateModel createNewDateModel)
        {
            var result = new NgayDieuTri();
            var LstYear = new List<YearModel>();
            var LstYearOrderBy = new List<LstYearOrderByModel>();
            var yctn = await GetYeuCauTiepNhanWithIncludeUpdate(createNewDateModel.yeuCauTiepNhanId);
            var yctnNhapVien = BaseRepository.TableNoTracking
                           .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens)
                           .Where(z => z.Id == yctn.YeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();
            //Lấy DS tất cả năm
            var noiTruPhieuDieuTris = yctn.NoiTruBenhAn.NoiTruPhieuDieuTris;
            createNewDateModel.Dates = createNewDateModel.Dates.Where(z => !noiTruPhieuDieuTris.Any(x => z == x.NgayDieuTri && x.KhoaPhongDieuTriId == createNewDateModel.KhoaId)).ToList();
            var lstYear = createNewDateModel.Dates.Select(o => o.Year).Distinct().ToList();
            var days = new List<DayModel>();
            foreach (var item in noiTruPhieuDieuTris)
            {
                LstYearOrderBy.Add(new LstYearOrderByModel
                {
                    Date = item.NgayDieuTri.Date,
                    LaNgayDieuTriTamThoi = false,
                    KhoaId = item.KhoaPhongDieuTriId,
                    PhieuDieuTriId = item.Id
                });
                if (lstYear.Count == 0 || !lstYear.Any(z => z == item.NgayDieuTri.Year))
                {
                    lstYear.Add(item.NgayDieuTri.Year);
                }
                var d = new DayModel
                {
                    Day = item.NgayDieuTri.Day,
                    Year = item.NgayDieuTri.Year,
                    Month = item.NgayDieuTri.Month,
                    FullDate = new DateTime(item.NgayDieuTri.Year, item.NgayDieuTri.Month, item.NgayDieuTri.Day),
                    FullDateDisplay = new DateTime(item.NgayDieuTri.Year, item.NgayDieuTri.Month, item.NgayDieuTri.Day).ApplyFormatDate(),
                    KhoaId = item.KhoaPhongDieuTriId,
                    TenKhoa = _khoaPhongRepository.TableNoTracking.Where(z => z.Id == item.KhoaPhongDieuTriId).Select(z => z.Ten).FirstOrDefault(),
                    PhieuDieuTriId = item.Id,
                    LaNgayDieuTriTamThoi = false
                };
                days.Add(d);
            }
            var phieuDieuTriId = 99999990;
            foreach (var ngayDieuTri in createNewDateModel.Dates)
            {
                if (yctn.NoiTruBenhAn != null && yctn.NoiTruBenhAn.ThoiDiemNhapVien != null && ngayDieuTri.Date < yctn.NoiTruBenhAn.ThoiDiemNhapVien.Date)
                {
                    var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                    throw new Exception(string.Format(errMess, yctn.NoiTruBenhAn.ThoiDiemNhapVien.ApplyFormatDate()));
                }
                //if (yctnNhapVien != null)
                //{
                //    if (ngayDieuTri.Date < yctnNhapVien.YeuCauKhamBenhs.FirstOrDefault()?.YeuCauNhapViens.FirstOrDefault()?.ThoiDiemChiDinh.Date)
                //    {
                //        var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                //        throw new Exception(string.Format(errMess, yctnNhapVien.YeuCauKhamBenhs.FirstOrDefault()?.YeuCauNhapViens.FirstOrDefault()?.ThoiDiemChiDinh.ApplyFormatDate()));
                //    }
                //}
                //else
                //{
                //    if (yctn.YeuCauNhapVienId != null && ngayDieuTri.Date < yctn.ThoiDiemTiepNhan.Date)
                //    {
                //        var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                //        throw new Exception(string.Format(errMess, yctn.ThoiDiemTiepNhan.ApplyFormatDate()));
                //    }
                //}

                LstYearOrderBy.Add(new LstYearOrderByModel
                {
                    Date = ngayDieuTri.Date,
                    LaNgayDieuTriTamThoi = true,
                    KhoaId = createNewDateModel.KhoaId.GetValueOrDefault(),
                    PhieuDieuTriId = phieuDieuTriId
                });
                phieuDieuTriId = phieuDieuTriId + 1;
            }

            var phieuDieuTriIdOther = 99999990;
            var tenKhoa = await _khoaPhongRepository.TableNoTracking.Where(z => z.Id == createNewDateModel.KhoaId).Select(z => z.Ten).FirstOrDefaultAsync();
            foreach (var year in lstYear.OrderBy(o => o))
            {
                var r = new YearModel
                {
                    Year = year,
                };
                //Lấy DS tất cả tháng của năm
                //var lstMonth = createNewDateModel.Dates.Any() ? createNewDateModel.Dates.Where(o => o.Year == year).Select(o => o.Month).Distinct().ToList() :
                //    LstYearOrderBy.Where(o => o.Date.Year == year).Select(o => o.Date.Month).Distinct().ToList();
                var lstMonth = LstYearOrderBy.Where(o => o.Date.Year == year).Select(o => o.Date.Month).Distinct().ToList();
                foreach (var month in lstMonth.OrderBy(o => o))
                {
                    var monthModel = new MonthModel
                    {
                        Month = month,
                    };
                    monthModel.Days.AddRange(days.Where(z => z.Month == month && z.Year == year).ToList());
                    //Lấy DS tất cả ngày của tháng của năm
                    foreach (var ngayDieuTri in createNewDateModel.Dates.Where(o => o.Year == year && o.Month == month).OrderBy(o => o))
                    {
                        var d = new DayModel
                        {
                            Day = ngayDieuTri.Day,
                            Year = year,
                            Month = month,
                            FullDate = new DateTime(year, month, ngayDieuTri.Day),
                            FullDateDisplay = new DateTime(year, month, ngayDieuTri.Day).ApplyFormatDate(),
                            KhoaId = createNewDateModel.KhoaId.GetValueOrDefault(),
                            TenKhoa = tenKhoa,
                            PhieuDieuTriId = phieuDieuTriIdOther,
                            LaNgayDieuTriTamThoi = true
                        };
                        monthModel.Days.Add(d);
                        phieuDieuTriIdOther = phieuDieuTriIdOther + 1;
                    }
                    var dayOrders = monthModel.Days.OrderBy(z => z.Day).ToList();
                    monthModel.Days = dayOrders;
                    r.Months.Add(monthModel);
                }

                LstYear.Add(r);
            }

            result.LstYear = LstYear;
            result.LstYearOrderBy = LstYearOrderBy.OrderBy(z => z.Date).ToList();
            return result;
        }
        public async Task<NgayDieuTri> GetNgayDieuTri(long yeuCauTiepNhanId)
        {
            var result = new NgayDieuTri();
            var LstYear = new List<YearModel>();
            var LstYearOrderBy = new List<LstYearOrderByModel>();

            //var yctn = BaseRepository.TableNoTracking
            //    .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.KhoaPhongDieuTri)
            //    .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruChiDinhDuocPhams)
            //    .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.YeuCauVatTuBenhViens)
            //    .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruPhieuDieuTriTuVanThuocs)
            //    .FirstOrDefault(p => p.Id == yeuCauTiepNhanId);

            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking
                .Include(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.KhoaPhongDieuTri)
                .Include(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruChiDinhDuocPhams)
                .Include(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.YeuCauVatTuBenhViens)
                .Include(p => p.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruPhieuDieuTriTuVanThuocs)
                .FirstOrDefault(p => p.Id == yeuCauTiepNhanId);

            var khoaChuyenDenId = await _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(z => z.NoiTruBenhAnId == yeuCauTiepNhanId && z.ThoiDiemRaKhoa == null).Select(z => z.KhoaPhongChuyenDenId).FirstAsync();
            if (noiTruBenhAn != null && noiTruBenhAn.NoiTruPhieuDieuTris.Any())
            {
                var lstNoiTruPhieuDieuTri = noiTruBenhAn.NoiTruPhieuDieuTris.ToList();

                //var lstNgayDieuTri = lstNoiTruPhieuDieuTri.OrderBy(o => o.NgayDieuTri).Select(p => p.NgayDieuTri).ToList();
                //LstYearOrderBy = lstNgayDieuTri.Select(p => new List<DateTime> { 0 => o = p.Date }).ToList();

                var lstPhieu = lstNoiTruPhieuDieuTri.OrderBy(o => o.NgayDieuTri).ToList();
                foreach (var phieu in lstPhieu)
                {
                    if (phieu.KhoaPhongDieuTriId == khoaChuyenDenId && !LstYearOrderBy.Any(z => z.LaNgayDieuTriDauTien == true))
                    {
                        LstYearOrderBy.Add(new LstYearOrderByModel
                        {
                            Date = phieu.NgayDieuTri.Date,
                            KhoaId = phieu.KhoaPhongDieuTriId,
                            PhieuDieuTriId = phieu.Id,
                            LaNgayDieuTriDauTien = true,
                            CoDonThuocNoiTru = phieu.NoiTruChiDinhDuocPhams.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy) || phieu.NoiTruPhieuDieuTriTuVanThuocs.Any(),
                            CoDonVTYTNoiTru = phieu.YeuCauVatTuBenhViens.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                        });
                    }
                    else
                    {
                        LstYearOrderBy.Add(new LstYearOrderByModel
                        {
                            Date = phieu.NgayDieuTri.Date,
                            KhoaId = phieu.KhoaPhongDieuTriId,
                            PhieuDieuTriId = phieu.Id,
                            LaNgayDieuTriDauTien = false,
                            CoDonThuocNoiTru = phieu.NoiTruChiDinhDuocPhams.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy) || phieu.NoiTruPhieuDieuTriTuVanThuocs.Any(),
                            CoDonVTYTNoiTru = phieu.YeuCauVatTuBenhViens.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                        });
                    }

                }
                //Lấy DS tất cả năm
                var lstYear = lstPhieu.Select(o => o.NgayDieuTri.Year).Distinct().ToList();

                foreach (var year in lstYear.OrderBy(o => o))
                {
                    var r = new YearModel
                    {
                        Year = year,
                    };
                    //Lấy DS tất cả tháng của năm
                    var lstMonth = lstPhieu.Where(o => o.NgayDieuTri.Year == year).Select(o => o.NgayDieuTri.Month).Distinct().ToList();
                    foreach (var month in lstMonth.OrderBy(o => o))
                    {
                        var m = new MonthModel
                        {
                            Month = month,
                        };

                        //Lấy DS tất cả ngày của tháng của năm
                        foreach (var phieu in lstPhieu.Where(o => o.NgayDieuTri.Year == year && o.NgayDieuTri.Month == month).OrderBy(o => o.NgayDieuTri))
                        {
                            if (phieu.KhoaPhongDieuTriId == khoaChuyenDenId && !m.Days.Any(z => z.LaNgayDieuTriDauTien == true && z.KhoaId == phieu.KhoaPhongDieuTriId))
                            {
                                var d = new DayModel
                                {
                                    LaNgayDieuTriDauTien = true,
                                    Day = phieu.NgayDieuTri.Day,
                                    Year = year,
                                    Month = month,
                                    FullDate = new DateTime(year, month, phieu.NgayDieuTri.Day),
                                    FullDateDisplay = new DateTime(year, month, phieu.NgayDieuTri.Day).ApplyFormatDate(),
                                    KhoaId = phieu.KhoaPhongDieuTriId,
                                    TenKhoa = phieu.KhoaPhongDieuTri?.Ten,
                                    PhieuDieuTriId = phieu.Id,
                                    CoDonThuocNoiTru = phieu.NoiTruChiDinhDuocPhams.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy) || phieu.NoiTruPhieuDieuTriTuVanThuocs.Any(),
                                    CoDonVTYTNoiTru = phieu.YeuCauVatTuBenhViens.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                                };
                                m.Days.Add(d);

                            }
                            else
                            {
                                var d = new DayModel
                                {
                                    LaNgayDieuTriDauTien = false,
                                    Day = phieu.NgayDieuTri.Day,
                                    Year = year,
                                    Month = month,
                                    FullDate = new DateTime(year, month, phieu.NgayDieuTri.Day),
                                    FullDateDisplay = new DateTime(year, month, phieu.NgayDieuTri.Day).ApplyFormatDate(),
                                    KhoaId = phieu.KhoaPhongDieuTriId,
                                    TenKhoa = phieu.KhoaPhongDieuTri?.Ten,
                                    PhieuDieuTriId = phieu.Id,
                                    CoDonThuocNoiTru = phieu.NoiTruChiDinhDuocPhams.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy) || phieu.NoiTruPhieuDieuTriTuVanThuocs.Any(),
                                    CoDonVTYTNoiTru = phieu.YeuCauVatTuBenhViens.Any(z => z.SoLuong > 0 && z.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                                };
                                m.Days.Add(d);
                            }
                        }
                        r.Months.Add(m);
                    }

                    LstYear.Add(r);
                }
            }

            result.LstYear = LstYear;
            result.LstYearOrderBy = LstYearOrderBy;
            return result;
        }

        public async Task<bool> IsExistsDate(long yeuCauTiepNhanId, List<DateTime> dates, long? khoaId)
        {
            var yctn = await BaseRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                .FirstAsync(p => p.Id == yeuCauTiepNhanId);

            if (yctn != null && yctn.NoiTruBenhAn.NoiTruPhieuDieuTris.Any(p => (khoaId == null || khoaId == p.KhoaPhongDieuTriId) && dates.Contains(p.NgayDieuTri.Date)))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsExistsDateTamThoi(List<DateTime> dates, List<DateTime> dateAdds)
        {
            if (dateAdds.Any(z => dates.Contains(z)))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsValidateRemoveDate(long yeuCauTiepNhanId, long phieuDieuTriId)
        {
            var phieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
                .Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauTruyenMaus)
                .Include(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.YeuCauDuocPhamBenhViens)
                .FirstOrDefault(p => p.Id == phieuDieuTriId);            

            if (phieuDieuTri == null) return false;

            if (phieuDieuTri.YeuCauDichVuKyThuats.Any(p => p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien
                                                        || p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                || phieuDieuTri.YeuCauTruyenMaus.Any(p => p.TrangThai == EnumTrangThaiYeuCauTruyenMau.DaThucHien)
                || phieuDieuTri.YeuCauVatTuBenhViens.Any(p => p.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien)
                || phieuDieuTri.YeuCauDuocPhamBenhViens.Any(p => p.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien)
                ) return false;

            return true;
        }

        public async Task CreateNewDate(long yeuCauTiepNhanId, long? khoaId, List<DateTime> dates)
        {
            var userLogin = _userAgentHelper.GetCurrentUserId();
            //var yctn = await GetYeuCauTiepNhanWithIncludeUpdate(yeuCauTiepNhanId);

            var noiTruBenhAn = _noiTruBenhAnRepository.GetById(yeuCauTiepNhanId, 
                x => x.Include(o => o.YeuCauTiepNhan).ThenInclude(o=>o.YeuCauNhapVien)
                        .Include(o => o.NoiTruKhoaPhongDieuTris)
                        .Include(o => o.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruThamKhamChanDoanKemTheos));

            var yctnNhapVien = BaseRepository.TableNoTracking
                            .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens)
                            .Where(z => z.Id == noiTruBenhAn.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();

            var noitruPhieuDieuTri = new NoiTruPhieuDieuTri();
            if (noiTruBenhAn.NoiTruPhieuDieuTris.Any())
            {
                noitruPhieuDieuTri = noiTruBenhAn.NoiTruPhieuDieuTris.OrderByDescending(p => p.Id).FirstOrDefault();
            }
            else
            {
                noitruPhieuDieuTri = null;
            }
            foreach (var date in dates)
            {
                if (noiTruBenhAn.ThoiDiemNhapVien != null && date.Date < noiTruBenhAn.ThoiDiemNhapVien.Date)
                {
                    var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                    throw new Exception(string.Format(errMess, noiTruBenhAn.ThoiDiemNhapVien.ApplyFormatDate()));
                }
                long phongDieuTriId = 0;
                if (khoaId != null)
                {
                    phongDieuTriId = (long)khoaId;
                }
                else
                {
                    var lastNoiDieuTri = noiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(o => (o.ThoiDiemVaoKhoa <= date || o.ThoiDiemVaoKhoa.Date == date.Date) && (o.ThoiDiemRaKhoa == null || o.ThoiDiemRaKhoa >= date || ((DateTime)o.ThoiDiemRaKhoa).Date == date.Date)).LastOrDefault();
                    phongDieuTriId = lastNoiDieuTri != null ? lastNoiDieuTri.KhoaPhongChuyenDenId : 0;
                }
                var noiTru = new NoiTruPhieuDieuTri
                {
                    NhanVienLapId = userLogin,
                    KhoaPhongDieuTriId = phongDieuTriId,
                    NgayDieuTri = date,
                    ChanDoanChinhICDId = noitruPhieuDieuTri != null ? noitruPhieuDieuTri.ChanDoanChinhICDId : noiTruBenhAn.YeuCauTiepNhan.YeuCauNhapVien?.ChanDoanNhapVienICDId,
                    ChanDoanChinhGhiChu = noitruPhieuDieuTri != null ? noitruPhieuDieuTri.ChanDoanChinhGhiChu : noiTruBenhAn.YeuCauTiepNhan.YeuCauNhapVien?.ChanDoanNhapVienGhiChu,
                    CheDoAnId = noitruPhieuDieuTri?.CheDoAnId,
                    CheDoChamSoc = noitruPhieuDieuTri?.CheDoChamSoc
                };
                if (noiTruBenhAn.NoiTruPhieuDieuTris.Any() && noitruPhieuDieuTri != null && noitruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Any())
                {
                    foreach (var item in noitruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
                    {
                        var noiTruThamKhamChanDoanKemTheo = new NoiTruThamKhamChanDoanKemTheo
                        {
                            ICDId = item.ICDId,
                            GhiChu = item.GhiChu
                        };
                        noiTru.NoiTruThamKhamChanDoanKemTheos.Add(noiTruThamKhamChanDoanKemTheo);
                    }
                }
                noiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTru);
            }

            _noiTruBenhAnRepository.Context.SaveChanges();
        }

        public async Task<YeuCauTiepNhan> CreateNewDateOld(long yeuCauTiepNhanId, long? khoaId, List<DateTime> dates)
        {
            var userLogin = _userAgentHelper.GetCurrentUserId();
            var yctn = await GetYeuCauTiepNhanWithIncludeUpdate(yeuCauTiepNhanId);
            var yctnNhapVien = BaseRepository.TableNoTracking
                            .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens)
                            .Where(z => z.Id == yctn.YeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();

            var noitruPhieuDieuTri = new NoiTruPhieuDieuTri();
            if (yctn.NoiTruBenhAn.NoiTruPhieuDieuTris.Any())
            {
                noitruPhieuDieuTri = yctn.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderByDescending(p => p.Id).FirstOrDefault();
            }
            else
            {
                noitruPhieuDieuTri = null;
            }
            foreach (var date in dates)
            {
                if (yctn.NoiTruBenhAn != null && yctn.NoiTruBenhAn.ThoiDiemNhapVien!=null && date.Date < yctn.NoiTruBenhAn.ThoiDiemNhapVien.Date)
                {
                    var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                    throw new Exception(string.Format(errMess, yctn.NoiTruBenhAn.ThoiDiemNhapVien.ApplyFormatDate()));
                }

                //if (yctnNhapVien != null)
                //{
                //    if (date.Date < yctnNhapVien.YeuCauKhamBenhs.FirstOrDefault()?.YeuCauNhapViens.FirstOrDefault()?.ThoiDiemChiDinh.Date)
                //    {
                //        var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                //        throw new Exception(string.Format(errMess, yctnNhapVien.YeuCauKhamBenhs.FirstOrDefault()?.YeuCauNhapViens.FirstOrDefault()?.ThoiDiemChiDinh.ApplyFormatDate()));
                //    }
                //}
                //else
                //{
                //    if (yctn.YeuCauNhapVienId != null && date.Date < yctn.ThoiDiemTiepNhan.Date)
                //    {
                //        var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                //        throw new Exception(string.Format(errMess, yctn.ThoiDiemTiepNhan.ApplyFormatDate()));
                //    }
                //}
                long phongDieuTriId = 0;
                if (khoaId != null)
                {
                    phongDieuTriId = (long)khoaId;
                }
                else
                {
                    var lastNoiDieuTri = yctn.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(o => (o.ThoiDiemVaoKhoa <= date || o.ThoiDiemVaoKhoa.Date == date.Date) && (o.ThoiDiemRaKhoa == null || o.ThoiDiemRaKhoa >= date || ((DateTime)o.ThoiDiemRaKhoa).Date == date.Date)).LastOrDefault();
                    phongDieuTriId = lastNoiDieuTri != null ? lastNoiDieuTri.KhoaPhongChuyenDenId : 0;
                }
                var noiTru = new NoiTruPhieuDieuTri
                {
                    NhanVienLapId = userLogin,
                    KhoaPhongDieuTriId = phongDieuTriId,
                    NgayDieuTri = date,
                    ChanDoanChinhICDId = noitruPhieuDieuTri != null ? noitruPhieuDieuTri.ChanDoanChinhICDId : yctn.YeuCauNhapVien.ChanDoanNhapVienICDId,
                    ChanDoanChinhGhiChu = noitruPhieuDieuTri != null ? noitruPhieuDieuTri.ChanDoanChinhGhiChu : yctn.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    CheDoAnId = noitruPhieuDieuTri?.CheDoAnId,
                    CheDoChamSoc = noitruPhieuDieuTri?.CheDoChamSoc
                };
                if (yctn.NoiTruBenhAn.NoiTruPhieuDieuTris.Any() && noitruPhieuDieuTri != null && noitruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Any())
                {
                    foreach (var item in noitruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
                    {
                        var noiTruThamKhamChanDoanKemTheo = new NoiTruThamKhamChanDoanKemTheo
                        {
                            ICDId = item.ICDId,
                            GhiChu = item.GhiChu
                        };
                        noiTru.NoiTruThamKhamChanDoanKemTheos.Add(noiTruThamKhamChanDoanKemTheo);
                    }
                }
                yctn.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTru);
            }

            return yctn;
        }

        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanWithIncludeUpdate(long yeuCauTiepNhanId)
        {
            var yctn = BaseRepository.Table
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.YeuCauTruyenMaus)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.YeuCauDuocPhamBenhViens).ThenInclude(x => x.NoiTruChiDinhDuocPham)
                .Include(x => x.BenhNhan).ThenInclude(x => x.TaiKhoanBenhNhan)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.ChanDoanNhapVienICD)
                .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauNhapVienChanDoanKemTheos)

                .Include(x => x.NoiTruBenhAn).ThenInclude(c => c.NoiTruThoiGianDieuTriBenhAnSoSinhs)
                .Include(x => x.YeuCauDichVuKyThuats)

                .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris)
                            .ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
                        .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris)
                            .ThenInclude(p => p.KetQuaSinhHieus)

                .First(p => p.Id == yeuCauTiepNhanId);

            return yctn;
        }

        public async Task<NoiTruPhieuDieuTri> GetNoiTruPhieuDieuTriById(long noiTruPhieuDieuTriId, Func<IQueryable<NoiTruPhieuDieuTri>, IIncludableQueryable<NoiTruPhieuDieuTri, object>> includes = null)
        {
            return _noiTruPhieuDieuTriRepository.GetById(noiTruPhieuDieuTriId, includes);
        }
        public async Task<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> GetNoiTruBenhAnById(long noiTruBenhAnId, Func<IQueryable<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn>, IIncludableQueryable<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn, object>> includes = null)
        {
            return _noiTruBenhAnRepository.GetById(noiTruBenhAnId, includes);
        }
        public void SaveChanges()
        {
            BaseRepository.Context.SaveChanges();
        }

        public async Task<bool> IsMoreThanNow(DateTime? date, DateTime? thoidiemChiDinh)
        {
            if (date == null)
            {
                return false;
            }
            else
            {
                if (date < thoidiemChiDinh || date > DateTime.Now)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<List<ICDPhieuDieuTriTemplateVo>> GetICD(DropDownListRequestModel model, bool coHienThiMa = false)
        {
            //|| p.Ma.Contains(model.Query ?? "")
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listIcd = await _icdRepository.TableNoTracking
               .Where(p => (model.Id == 0 || (model.Id != 0 && model.Id != p.Id)) && (p.TenTiengViet.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
               .Take(model.Take)
               .ToListAsync();

                var modelResult = await _icdRepository.TableNoTracking
                    .Where(p => model.Id == p.Id)
                    .Take(model.Take)
                    .ToListAsync();

                listIcd.AddRange(modelResult);
                if (coHienThiMa)
                {
                    var query = listIcd.Select(item => new ICDPhieuDieuTriTemplateVo
                    {
                        DisplayName = item.Ma + " - " + item.TenTiengViet,
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.TenTiengViet
                    }).ToList();

                    return query;
                }
                else
                {
                    var query = listIcd.Select(item => new ICDPhieuDieuTriTemplateVo
                    {
                        DisplayName = item.TenTiengViet,
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.TenTiengViet
                    }).ToList();

                    return query;
                }
            }
            else
            {
                var lstColumnNameSearch = new List<string>
                {
                     nameof(ICD.Ma),
                     nameof(ICD.TenTiengViet),
                };
                var lstId = _icdRepository
                    .ApplyFulltext(model.Query, nameof(ICD), lstColumnNameSearch)
                    .Select(p => p.Id)
                    .Take(model.Take).ToList();

                if (coHienThiMa)
                {
                    var list = _icdRepository
                    .TableNoTracking
                    .Where(x => lstId.Any(a => a == x.Id))
                    .Take(model.Take)
                    .Select(item => new ICDPhieuDieuTriTemplateVo
                    {
                        DisplayName = item.Ma + " - " + item.TenTiengViet,
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.TenTiengViet
                    })
                    .OrderBy(x => lstId.IndexOf(x.KeyId))
                    .ToList();
                    return list;
                }
                else
                {
                    var list = _icdRepository
                    .TableNoTracking
                    .Where(x => lstId.Any(a => a == x.Id))
                    .Take(model.Take)
                    .Select(item => new ICDPhieuDieuTriTemplateVo
                    {
                        DisplayName = item.TenTiengViet,
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.TenTiengViet
                    })
                    .OrderBy(x => lstId.IndexOf(x.KeyId))
                    .ToList();
                    return list;
                }
            }
        }

        public Task<List<LookupItemVo>> GetCheDoChamSoc(DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.CheDoChamSoc>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            return Task.FromResult(result);
        }

        public async Task<List<PhieuDieuTriThuocGridVo>> GetDanhSachThuoc(long yeuCauTiepNhanId, long phieuDieuTriId)
        {
            var queryKho = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId
                          && o.NoiTruPhieuDieuTriId == phieuDieuTriId
                          && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                          //&& o.YeuCauDuocPhamBenhViens.All(x => x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                          )
                .Select(s => new PhieuDieuTriThuocGridVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    MaHoatChat = s.MaHoatChat,
                    Ma = s.DuocPhamBenhVien.Ma,
                    Ten = s.Ten,
                    KhoId = s.YeuCauDuocPhamBenhViens.First().KhoLinhId,
                    TenKho = s.YeuCauDuocPhamBenhViens.First().KhoLinh.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    ThoiGianDungSang = s.ThoiGianDungSang,
                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                    ThoiGianDungToi = s.ThoiGianDungToi,
                    //DungSang = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    DungSang = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungSang == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungSang.Value), false) + ")"
                                              : s.DungSang == null ? null : "(" + s.DungSang.FloatToStringFraction() + ")",
                    //DungTrua = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    DungTrua = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungTrua == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungTrua.Value), false) + ")"
                                              : s.DungTrua == null ? null : "(" + s.DungTrua.FloatToStringFraction() + ")",
                    //DungChieu = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    DungChieu = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungChieu == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungChieu.Value), false) + ")"
                                              : s.DungChieu == null ? null : "(" + s.DungChieu.FloatToStringFraction() + ")",
                    //DungToi = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    DungToi = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungToi == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungToi.Value), false) + ")"
                                              : s.DungToi == null ? null : "(" + s.DungToi.FloatToStringFraction() + ")",
                    SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                    TenDuongDung = s.DuongDung.Ten,
                    SoLuongDisplay = s.YeuCauDuocPhamBenhViens.Select(p => p.SoLuong).ToList(),
                    SoLuong = s.SoLuong,
                    //update BACHA-427
                    DonGias = s.YeuCauDuocPhamBenhViens.Select(p => p.DonGiaBan).ToList(),
                    ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true) ? s.YeuCauDuocPhamBenhViens.Select(p => p.GiaBan).ToList() : new List<decimal> { 0 },
                    //DonGias = s.YeuCauDuocPhamBenhViens.Select(p => CalculateHelper.TinhDonGiaBan(p.DonGiaNhap, p.TiLeTheoThapGia, p.VAT)).ToList(),
                    //ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true) ? s.YeuCauDuocPhamBenhViens.Select(p => (CalculateHelper.TinhDonGiaBan(p.DonGiaNhap, p.TiLeTheoThapGia, p.VAT)) * (decimal)p.SoLuong).ToList() : new List<decimal> { 0 },
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                    //TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                    GhiChu = s.GhiChu,
                    TinhTrang = s.YeuCauDuocPhamBenhViens.First().XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    LaDichTruyen = s.LaDichTruyen,
                    CoYeuCauTraDuocPhamTuBenhNhanChiTiet = s.YeuCauDuocPhamBenhViens.Any(yc => yc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any()),
                    KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Select(p => p.KhongTinhPhi).FirstOrDefault(),
                    LaThuocHuongThanGayNghien = s.DuocPhamBenhVien.DuocPham.LaThuocHuongThanGayNghien,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TocDoTruyen = s.TocDoTruyen,
                    DonViTocDoTruyen = s.DonViTocDoTruyen,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    CachGioTruyenDich = s.CachGioTruyenDich,
                    TheTich = s.TheTich,
                    DuoCPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                    LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                }).OrderBy(o => o.ThoiDiemChiDinh);

            var lstQueryKho = queryKho.ToList();

            //
            return lstQueryKho;
        }

        public async Task<List<PhieuDieuTriTruyenDichGridVo>> GetDanhSachTruyenDich(long yeuCauTiepNhanId, long phieuDieuTriId)
        {
            var query = _noiTruChiDinhDuocPhamRepository.TableNoTracking
               .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId
                         && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen == true
                         && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                         //&& o.YeuCauDuocPhamBenhViens.All(x => x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                         )
                       .Select(s => new PhieuDieuTriTruyenDichGridVo
                       {
                           Id = s.Id,
                           DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                           MaHoatChat = s.MaHoatChat,
                           Ma = s.DuocPhamBenhVien.Ma,
                           Ten = s.Ten,
                           KhoId = s.YeuCauDuocPhamBenhViens.First().KhoLinhId,
                           TenKho = s.YeuCauDuocPhamBenhViens.First().KhoLinh.Ten,
                           HoatChat = s.HoatChat,
                           HamLuong = s.HamLuong,
                           DVT = s.DonViTinh.Ten,
                           TocDoTruyen = s.TocDoTruyen,
                           DonViTocDoTruyen = s.DonViTocDoTruyen,
                           SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                           ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                           CachGioTruyenDich = s.CachGioTruyenDich,
                           SoLuongDisplay = s.YeuCauDuocPhamBenhViens.Select(p => p.SoLuong).ToList(),
                           SoLuong = s.SoLuong,
                           DonGias = s.YeuCauDuocPhamBenhViens.Select(p => p.DonGiaBan).ToList(),
                           ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true) ? s.YeuCauDuocPhamBenhViens.Select(p => p.GiaBan).ToList() : new List<decimal> { 0 },
                           //DonGias = s.YeuCauDuocPhamBenhViens.Select(p => CalculateHelper.TinhDonGiaBan(p.DonGiaNhap, p.TiLeTheoThapGia, p.VAT)).ToList(),
                           //ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true) ? s.YeuCauDuocPhamBenhViens.Select(p => (CalculateHelper.TinhDonGiaBan(p.DonGiaNhap, p.TiLeTheoThapGia, p.VAT)) * (decimal)p.SoLuong).ToList() : new List<decimal> { 0 },
                           LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                           DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                           //TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                           GhiChu = s.GhiChu,
                           TrangThai = s.TrangThai,
                           TinhTrang = s.YeuCauDuocPhamBenhViens.First().XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                           LaDichTruyen = s.LaDichTruyen,
                           TheTich = s.TheTich,
                           CoYeuCauTraDuocPhamTuBenhNhanChiTiet = s.YeuCauDuocPhamBenhViens.Any(yc => yc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any()),
                           KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Select(p => p.KhongTinhPhi).FirstOrDefault(),
                           ThoiDiemChiDinh = s.ThoiDiemChiDinh
                       });
            var lstQuery = query.ToList();

            return lstQuery;
        }

        //public async Task<List<PhieuDieuTriSuatAnGridVo>> GetDanhSachSuatAn(long yeuCauTiepNhanId, long phieuDieuTriId)
        //{
        //    var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
        //        .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId
        //        && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SuatAn)
        //        .Select(s => new PhieuDieuTriSuatAnGridVo
        //        {
        //            Id = s.Id,
        //            Ma = s.MaDichVu,
        //            Ten = s.TenDichVu,
        //            DoiTuong = s.DoiTuongSuDung.GetDescription(),
        //            DonGia = s.Gia,
        //            SoLan = s.SoLan,
        //        });

        //    return query.ToList();
        //}

        public int GetSoThuTuThuocGayNghien(long yeuCauTiepNhanId, long phieuDieuTriId, long duocPhamBenhVienId)
        {
            var queryKho = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId
                          && o.DuocPhamBenhVienId == duocPhamBenhVienId
                          && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                          ).Select(o => o.NoiTruPhieuDieuTri).OrderBy(o => o.NgayDieuTri).Select(o => o.Id).Distinct();
            var phieuDieuTri = queryKho.FirstOrDefault(o => o == phieuDieuTriId);
            return queryKho.ToList().IndexOf(phieuDieuTri) + 1;
        }
        public async Task<bool> IsTenICDKemTheoExists(long? idICD, long noiTruThamKhamChanDoanKemTheoId, long noiTruPhieuDieuTriId)
        {
            if (idICD == null)
            {
                return true;
            }

            var result = false;
            if (noiTruThamKhamChanDoanKemTheoId == 0)
            {
                result = await _noiTruThamKhamChanDoanKemTheoRepository.TableNoTracking.AnyAsync(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId && p.ICDId == idICD);
            }
            else
            {
                result = await _noiTruThamKhamChanDoanKemTheoRepository.TableNoTracking.AnyAsync(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId && p.ICDId == idICD && p.Id != noiTruThamKhamChanDoanKemTheoId);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<string> GetCheDoAn(long? cheDoAnId)
        {
            if (cheDoAnId == null)
            {
                return string.Empty;
            }
            return await _cheDoAnRepository.TableNoTracking.Where(z => z.Id == cheDoAnId).Select(z => z.Ten).FirstOrDefaultAsync();
        }
        public async Task<string> GetCheDoChamSoc(long? cheCheDoChamSocId)
        {
            if (cheCheDoChamSocId == null)
            {
                return string.Empty;
            }
            var listEnum = EnumHelper.GetListEnum<Enums.CheDoChamSoc>();
            return listEnum.Where(z => (long)z == cheCheDoChamSocId).Select(z => z.GetDescription()).FirstOrDefault();
        }
    }
}
