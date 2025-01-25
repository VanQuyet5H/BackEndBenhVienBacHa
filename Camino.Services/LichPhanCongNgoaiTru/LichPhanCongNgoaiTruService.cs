using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DateRange;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.LichPhanCongNgoaiTru;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhoaPhong;

namespace Camino.Services.LichPhanCongNgoaiTru
{
    [ScopedDependency(ServiceType = typeof(ILichPhanCongNgoaiTruService))]
    public class LichPhanCongNgoaiTruService: MasterFileService<Core.Domain.Entities.LichPhanCongNgoaiTrus.LichPhanCongNgoaiTru>, ILichPhanCongNgoaiTruService
    {
        IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongrepository;
        public LichPhanCongNgoaiTruService(
            IRepository<Core.Domain.Entities.LichPhanCongNgoaiTrus.LichPhanCongNgoaiTru> repository,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongrepository) : base(repository)
        {
            _khoaPhongrepository = khoaPhongrepository;
        }

        private int DemSoTuan(int month, int year)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime firstOfMonth = new DateTime(year, month, 1);
            int firstDayOfMonth;
            if (firstOfMonth.DayOfWeek == DayOfWeek.Sunday)
            {
                firstDayOfMonth = 6;
            }
            else
            {
                firstDayOfMonth = (int)firstOfMonth.DayOfWeek - 1;
            }

            int weeksInMonth = (int)Math.Ceiling((firstDayOfMonth + daysInMonth) / 7.0);
            return weeksInMonth;
        }

        private List<DateRange> GetNgayTrongTuan(int numberOfWeeks, int month, int year)
        {
            var listNgay = new List<DateRange>();
            DateTime firstOfMonth = new DateTime(year, month, 1);
            DateTime firstDateTime, lastDateTime;
            var count = 0;

            if (firstOfMonth.DayOfWeek != DayOfWeek.Monday)
            {
                while (firstOfMonth.DayOfWeek != DayOfWeek.Monday)
                {
                    firstOfMonth = firstOfMonth.AddDays(-1);
                }
            }
            firstDateTime = firstOfMonth;
            lastDateTime = firstDateTime.AddDays(6);

            while (numberOfWeeks > count)
            {
                listNgay.Add(new DateRange
                {
                    FromDate = firstDateTime,
                    ToDate = lastDateTime
                });
                firstDateTime = lastDateTime.AddDays(1);
                lastDateTime = firstDateTime.AddDays(6);
                count++;
            }

            return listNgay;
        }
        
        public async Task<List<LichPhanCongNgoaiTruGridVo>> XepLich(DateTime date, int khoaId)
        {
            var danhsach = _khoaPhongrepository.TableNoTracking
                .Include(o => o.PhongBenhViens)
                .Where(x => x.IsDisabled != true && x.PhongBenhViens.Any())
               .ToList();
            var thisMonth = date.Month;
            var thisYear = date.Year;
            var soTuan = DemSoTuan(thisMonth, thisYear);
            var listNgay = GetNgayTrongTuan(soTuan, thisMonth, thisYear);
            var countSoTuan = 1;
            DateRange dayTuan1 = null,
                dayTuan2 = null,
                dayTuan3 = null,
                dayTuan4 = null,
                dayTuan5 = null,
                dayTuan6 = null;

            DateTime dayStartTuan1 = new DateTime(1998, 04, 30),
                dayEndTuan1 = new DateTime(1998, 04, 30),
                dayStartTuan2 = new DateTime(1998, 04, 30),
                dayEndTuan2 = new DateTime(1998, 04, 30),
                dayStartTuan3 = new DateTime(1998, 04, 30),
                dayEndTuan3 = new DateTime(1998, 04, 30),
                dayStartTuan4 = new DateTime(1998, 04, 30),
                dayEndTuan4 = new DateTime(1998, 04, 30),
                dayStartTuan5 = new DateTime(1998, 04, 30),
                dayEndTuan5 = new DateTime(1998, 04, 30),
                dayStartTuan6 = new DateTime(1998, 04, 30),
                dayEndTuan6 = new DateTime(1998, 04, 30);

            string dayTuan1Display = null,
                dayTuan2Display = null,
                dayTuan3Display = null,
                dayTuan4Display = null,
                dayTuan5Display = null,
                dayTuan6Display = null;

            foreach (var ngayTheoTuan in listNgay)
            {
                if (countSoTuan == 1)
                {
                    dayTuan1 = ngayTheoTuan;
                    dayStartTuan1 = dayTuan1.FromDate;
                    dayEndTuan1 = dayTuan1.ToDate;
                    dayTuan1Display = FormatDateRange(1, dayTuan1);
                }

                if (countSoTuan == 2)
                {
                    dayTuan2 = ngayTheoTuan;
                    dayStartTuan2 = dayTuan2.FromDate;
                    dayEndTuan2 = dayTuan2.ToDate;
                    dayTuan2Display = FormatDateRange(2, dayTuan2);
                }

                if (countSoTuan == 3)
                {
                    dayTuan3 = ngayTheoTuan;
                    dayStartTuan3 = dayTuan3.FromDate;
                    dayEndTuan3 = dayTuan3.ToDate;
                    dayTuan3Display = FormatDateRange(3, dayTuan3);
                }

                if (countSoTuan == 4)
                {
                    dayTuan4 = ngayTheoTuan;
                    dayStartTuan4 = dayTuan4.FromDate;
                    dayEndTuan4 = dayTuan4.ToDate;
                    dayTuan4Display = FormatDateRange(4, dayTuan4);
                }

                if (countSoTuan == 5)
                {
                    dayTuan5 = ngayTheoTuan;
                    dayStartTuan5 = dayTuan5.FromDate;
                    dayEndTuan5 = dayTuan5.ToDate;
                    dayTuan5Display = FormatDateRange(5, dayTuan5);
                }

                if (countSoTuan == 6)
                {
                    dayTuan6 = ngayTheoTuan;
                    dayStartTuan6 = dayTuan6.FromDate;
                    dayEndTuan6 = dayTuan6.ToDate;
                    dayTuan6Display = FormatDateRange(6, dayTuan6);
                }

                countSoTuan++;
            }
            var result = new List<LichPhanCongNgoaiTruGridVo>();
            if (khoaId != 0)
            {
                danhsach = danhsach.Where(x => x.Id == khoaId).ToList();
                for (int i = 0; i < danhsach.Count; i++)
                {
                    result.Add(new LichPhanCongNgoaiTruGridVo
                    {
                        SoTuan = soTuan,
                        IdKhoa = danhsach[i].Id,
                        TenKhoa = danhsach[i].Ten,
                        Tuan1Display = dayTuan1Display,
                        Tuan2Display = dayTuan2Display,
                        Tuan3Display = dayTuan3Display,
                        Tuan4Display = dayTuan4Display,
                        Tuan5Display = dayTuan5Display,
                        Tuan6Display = dayTuan6Display,
                        NgayStartTuan1 = dayStartTuan1,
                        NgayStartTuan2 = dayStartTuan2,
                        NgayStartTuan3 = dayStartTuan3,
                        NgayStartTuan4 = dayStartTuan4,
                        NgayStartTuan5 = dayStartTuan5,
                        NgayStartTuan6 = dayStartTuan6,
                        NgayEndTuan1 = dayEndTuan1,
                        NgayEndTuan2 = dayEndTuan2,
                        NgayEndTuan3 = dayEndTuan3,
                        NgayEndTuan4 = dayEndTuan4,
                        NgayEndTuan5 = dayEndTuan5,
                        NgayEndTuan6 = dayEndTuan6,
                        XepLichTuan1 = dayTuan1 != null && IsXepLich(dayTuan1.FromDate, dayTuan1.ToDate, danhsach[i].Id),
                        XepLichTuan2 = dayTuan2 != null && IsXepLich(dayTuan2.FromDate, dayTuan2.ToDate, danhsach[i].Id),
                        XepLichTuan3 = dayTuan3 != null && IsXepLich(dayTuan3.FromDate, dayTuan3.ToDate, danhsach[i].Id),
                        XepLichTuan4 = dayTuan4 != null && IsXepLich(dayTuan4.FromDate, dayTuan4.ToDate, danhsach[i].Id),
                        XepLichTuan5 = dayTuan5 != null && IsXepLich(dayTuan5.FromDate, dayTuan5.ToDate, danhsach[i].Id),
                        XepLichTuan6 = dayTuan6 != null && IsXepLich(dayTuan6.FromDate, dayTuan6.ToDate, danhsach[i].Id),
                        Now = DateTime.Now
                    });
                }
            }
            else
            {
                for (int i = 0; i < danhsach.Count; i++)
                {
                    result.Add(new LichPhanCongNgoaiTruGridVo
                    {
                        SoTuan = soTuan,
                        IdKhoa = danhsach[i].Id,
                        TenKhoa = danhsach[i].Ten,
                        Tuan1Display = dayTuan1Display,
                        Tuan2Display = dayTuan2Display,
                        Tuan3Display = dayTuan3Display,
                        Tuan4Display = dayTuan4Display,
                        Tuan5Display = dayTuan5Display,
                        Tuan6Display = dayTuan6Display,
                        NgayStartTuan1 = dayStartTuan1,
                        NgayStartTuan2 = dayStartTuan2,
                        NgayStartTuan3 = dayStartTuan3,
                        NgayStartTuan4 = dayStartTuan4,
                        NgayStartTuan5 = dayStartTuan5,
                        NgayStartTuan6 = dayStartTuan6,
                        NgayEndTuan1 = dayEndTuan1,
                        NgayEndTuan2 = dayEndTuan2,
                        NgayEndTuan3 = dayEndTuan3,
                        NgayEndTuan4 = dayEndTuan4,
                        NgayEndTuan5 = dayEndTuan5,
                        NgayEndTuan6 = dayEndTuan6,
                        XepLichTuan1 = dayTuan1 != null && IsXepLich(dayTuan1.FromDate, dayTuan1.ToDate, danhsach[i].Id),
                        XepLichTuan2 = dayTuan2 != null && IsXepLich(dayTuan2.FromDate, dayTuan2.ToDate, danhsach[i].Id),
                        XepLichTuan3 = dayTuan3 != null && IsXepLich(dayTuan3.FromDate, dayTuan3.ToDate, danhsach[i].Id),
                        XepLichTuan4 = dayTuan4 != null && IsXepLich(dayTuan4.FromDate, dayTuan4.ToDate, danhsach[i].Id),
                        XepLichTuan5 = dayTuan5 != null && IsXepLich(dayTuan5.FromDate, dayTuan5.ToDate, danhsach[i].Id),
                        XepLichTuan6 = dayTuan6 != null && IsXepLich(dayTuan6.FromDate, dayTuan6.ToDate, danhsach[i].Id),
                        Now = DateTime.Now
                    });
                }
            }
           
            return result;
        }

        public List<LichTuanGridVo> GetDataForTuanAsync(DateTime date)
        {
            var thisMonth = date.Month;
            var thisYear = date.Year;
            var soTuan = DemSoTuan(thisMonth, thisYear);
            var listNgay = GetNgayTrongTuan(soTuan, thisMonth, thisYear);
            var countSoTuan = 1;
            DateTime dayTuan1Start = new DateTime(1998, 04, 30),
                dayTuan1End,
                dayTuan2Start = new DateTime(1998, 04, 30),
                dayTuan2End,
                dayTuan3Start = new DateTime(1998, 04, 30),
                dayTuan3End,
                dayTuan4Start = new DateTime(1998, 04, 30),
                dayTuan4End,
                dayTuan5Start = new DateTime(1998, 04, 30),
                dayTuan5End,
                dayTuan6Start = new DateTime(1998, 04, 30),
                dayTuan6End;
            string dayTuan1DisplayStart = null,
                dayTuan1DisplayEnd = null,
                dayTuan2DisplayStart = null,
                dayTuan2DisplayEnd = null,
                dayTuan3DisplayStart = null,
                dayTuan3DisplayEnd = null,
                dayTuan4DisplayStart = null,
                dayTuan4DisplayEnd = null,
                dayTuan5DisplayStart = null,
                dayTuan5DisplayEnd = null,
                dayTuan6DisplayStart = null,
                dayTuan6DisplayEnd = null;

            foreach (var ngayTheoTuan in listNgay)
            {
                if (countSoTuan == 1)
                {
                    dayTuan1Start = ngayTheoTuan.FromDate;
                    dayTuan1End = ngayTheoTuan.ToDate;
                    dayTuan1DisplayStart = FormatDate(dayTuan1Start);
                    dayTuan1DisplayEnd = FormatDate(dayTuan1End);
                }

                if (countSoTuan == 2)
                {
                    dayTuan2Start = ngayTheoTuan.FromDate;
                    dayTuan2End = ngayTheoTuan.ToDate;
                    dayTuan2DisplayStart = FormatDate(dayTuan2Start);
                    dayTuan2DisplayEnd = FormatDate(dayTuan2End);
                }

                if (countSoTuan == 3)
                {
                    dayTuan3Start = ngayTheoTuan.FromDate;
                    dayTuan3End = ngayTheoTuan.ToDate;
                    dayTuan3DisplayStart = FormatDate(dayTuan3Start);
                    dayTuan3DisplayEnd = FormatDate(dayTuan3End);
                }

                if (countSoTuan == 4)
                {
                    dayTuan4Start = ngayTheoTuan.FromDate;
                    dayTuan4End = ngayTheoTuan.ToDate;
                    dayTuan4DisplayStart = FormatDate(dayTuan4Start);
                    dayTuan4DisplayEnd = FormatDate(dayTuan4End);
                }
                if (countSoTuan == 5)
                {
                    dayTuan5Start = ngayTheoTuan.FromDate;
                    dayTuan5End = ngayTheoTuan.ToDate;
                    dayTuan5DisplayStart = FormatDate(dayTuan5Start);
                    dayTuan5DisplayEnd = FormatDate(dayTuan5End);
                }

                if (countSoTuan == 6)
                {
                    dayTuan6Start = ngayTheoTuan.FromDate;
                    dayTuan6End = ngayTheoTuan.ToDate;
                    dayTuan6DisplayStart = FormatDate(dayTuan6Start);
                    dayTuan6DisplayEnd = FormatDate(dayTuan6End);
                }

                countSoTuan++;
            }

            var listTuan = new List<LichTuanGridVo>();

            if (dayTuan1Start != new DateTime(1998, 04, 30))
            {
                listTuan.Add(new LichTuanGridVo
                {
                    Value = 1,
                    Name = "Tuần 1",
                    StartDate = dayTuan1DisplayStart,
                    EndDate = dayTuan1DisplayEnd
                });
            }

            if (dayTuan2Start != new DateTime(1998, 04, 30))
            {
                listTuan.Add(new LichTuanGridVo
                {
                    Value = 2,
                    Name = "Tuần 2",
                    StartDate = dayTuan2DisplayStart,
                    EndDate = dayTuan2DisplayEnd
                });
            }

            if (dayTuan3Start != new DateTime(1998, 04, 30))
            {
                listTuan.Add(new LichTuanGridVo
                {
                    Value = 3,
                    Name = "Tuần 3",
                    StartDate = dayTuan3DisplayStart,
                    EndDate = dayTuan3DisplayEnd
                });
            }

            if (dayTuan4Start != new DateTime(1998, 04, 30))
            {
                listTuan.Add(new LichTuanGridVo
                {
                    Value = 4,
                    Name = "Tuần 4",
                    StartDate = dayTuan4DisplayStart,
                    EndDate = dayTuan4DisplayEnd
                });
            }

            if (dayTuan5Start != new DateTime(1998, 04, 30))
            {
                listTuan.Add(new LichTuanGridVo
                {
                    Value = 5,
                    Name = "Tuần 5",
                    StartDate = dayTuan5DisplayStart,
                    EndDate = dayTuan5DisplayEnd
                });
            }

            if (dayTuan6Start != new DateTime(1998, 04, 30))
            {
                listTuan.Add(new LichTuanGridVo
                {
                    Value = 6,
                    Name = "Tuần 6",
                    StartDate = dayTuan6DisplayStart,
                    EndDate = dayTuan6DisplayEnd
                });
            }


            return listTuan;
        }

        private bool IsXepLich(DateTime startDateTime, DateTime endDateTime, long khoaId)
        {
            var entity = BaseRepository.TableNoTracking.Include(p => p.NhanVien)
                .Include(p => p.PhongBenhVien).ThenInclude(p => p.KhoaPhong).Where(x => x.NgayPhanCong >= startDateTime && x.NgayPhanCong <= endDateTime && x.PhongBenhVien.KhoaPhong.Id == khoaId);
            bool xepLich = entity.Any();
            return xepLich;
        }

        private string FormatDateRange(int tuanThu, DateRange dateRange)
        {
            DateTime ngayDauTien = dateRange.FromDate;
            DateTime ngayKetThuc = dateRange.ToDate;

            return "Tuần " + tuanThu + " (" + ngayDauTien.Day + "/" + ngayDauTien.Month + "/" + ngayDauTien.Year +
                   " - " + ngayKetThuc.Day + "/" + ngayKetThuc.Month + "/" + ngayKetThuc.Year + ")";
        }

        public Task<List<Core.Domain.Entities.LichPhanCongNgoaiTrus.LichPhanCongNgoaiTru>> GetListLichPhanCong(List<long> listIds, DateTime fromDate, DateTime toDate)
        {
            var modelEntity = BaseRepository.TableNoTracking.Include(p => p.NhanVien).ThenInclude(p => p.User)
                .Include(p => p.NhanVien).ThenInclude(p=>p.ChucDanh).ThenInclude(p=>p.NhomChucDanh)
                .Include(p => p.PhongBenhVien)
                .ThenInclude(p => p.KhoaPhong)
                .Where(x => listIds.Contains(x.PhongNgoaiTruId) && (x.NgayPhanCong.Date >= toDate.Date && x.NgayPhanCong.Date <= fromDate));
            return modelEntity.ToListAsync();
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");

        }

        public async Task<List<KhoaPhongTemplateVo>> GetListKhoaPhong(DropDownListRequestModel model)
        {
            var listKhoaPhong = await _khoaPhongrepository.TableNoTracking
                .Include(o => o.PhongBenhViens)
                .Where(p => p.IsDisabled != true && p.PhongBenhViens.Any() && (p.Ma.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? "")))
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoaPhong.Select(item => new KhoaPhongTemplateVo
            {
                DisplayName = item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return query;
        }

        public Task<List<Core.Domain.Entities.LichPhanCongNgoaiTrus.LichPhanCongNgoaiTru>> GetListLichLastWeek(List<long> listIds)
        {
            var lastData = BaseRepository.TableNoTracking.Where(x => listIds.Contains(x.PhongNgoaiTruId) 
                                                                && x.NgayPhanCong.Date.DayOfWeek == DayOfWeek.Monday ).FirstOrDefault();
            if (lastData != null) {
                //DateTime LastMonDate = lastData.NgayPhanCong.AddDays((int)DayOfWeek.Monday - (int)lastData.NgayPhanCong.DayOfWeek-7);
                DateTime lastMonDate = lastData.NgayPhanCong;
                DateTime lastSunDate = lastMonDate.AddDays(6);

             var modelEntity = BaseRepository.TableNoTracking.Include(p => p.NhanVien).ThenInclude(p => p.User)
                  .Include(p => p.NhanVien).ThenInclude(p => p.ChucDanh).ThenInclude(p => p.NhomChucDanh)
                  .Include(p => p.PhongBenhVien)
                  .ThenInclude(p => p.KhoaPhong)
                  .Where(x => listIds.Contains(x.PhongNgoaiTruId) && x.PhongBenhVien.IsDisabled != true
                  && (x.NgayPhanCong.Date >= lastMonDate.Date && x.NgayPhanCong.Date <= lastSunDate));
                return modelEntity.ToListAsync();
            }
            return null;
        }

        public async Task<bool> IsNhanVienIdExists(List<long> listIdValidator, long nhanVienId)
        {
            //int count = 0;
            //for (int i = 0; i < ListIdValidator.Count; i++)
            //{
            //    if (ListIdValidator[i] == NhanVienId)
            //    { count++; }
            //}
            //if (count > 0)
            //{
            //    return true;
            //}
            //return false;
            if (listIdValidator.Any())
            {
                return true;
            }
            return false;
        }

    }
}
