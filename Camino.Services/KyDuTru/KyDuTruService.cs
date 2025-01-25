using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KyDuTru;
using Camino.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using Camino.Core.Helpers;

namespace Camino.Services.KyDuTru
{
    [ScopedDependency(ServiceType = typeof(IKyDuTruService))]
    public class KyDuTruService : MasterFileService<KyDuTruMuaDuocPhamVatTu>, IKyDuTruService
    {
        public KyDuTruService(IRepository<KyDuTruMuaDuocPhamVatTu> repository) : base(repository)
        {
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new KyDuTruSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<KyDuTruSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject.DuocPham == false && queryObject.VatTu == false)
            {
                queryObject.VatTu = true;
                queryObject.DuocPham = true;
            }

            var query = BaseRepository.TableNoTracking;

            if(queryObject.DuocPham == true && queryObject.VatTu == true)
            {
                query = query.Where(p => p.MuaDuocPham == true || p.MuaVatTu == true);
            }
            else if(queryObject.DuocPham == true)
            {
                query = query.Where(p => p.MuaDuocPham == true);
            }
            else
            {
                query = query.Where(p => p.MuaVatTu == true);
            }

            var result = query.Select(p => new KyDuTruGridVo
            {
                Id = p.Id,
                KyDuTru = $"{p.TuNgay.ApplyFormatDate()} - {p.DenNgay.ApplyFormatDate()}",
                TuNgay = p.TuNgay,
                DenNgay = p.DenNgay,
                NhanVienTaoId = p.NhanVienTaoId,
                NhanVienTaoDisplay = p.NhanVien.User.HoTen,
                MuaDuocPham = p.MuaDuocPham,
                MuaVatTu = p.MuaVatTu,
                HieuLuc = p.HieuLuc,
                NgayTao = p.CreatedOn,
                NgayBatDauLap = p.NgayBatDauLap,
                NgayKetThucLap = p.NgayKetThucLap
            });

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    result = result.ApplyLike(searchTerms, p => p.NhanVienTaoDisplay);
                }

                if (queryObject.RangeApDung != null)
                {
                    //tuNgay <= p.TuNgay < p.DenNgay <= denNgay
                    if (queryObject.RangeApDung.startDate != null)
                    {
                        var tuNgay = queryObject.RangeApDung.startDate.GetValueOrDefault();
                        result = result.Where(p => tuNgay.Date <= p.TuNgay.Date);
                    }

                    if (queryObject.RangeApDung.endDate != null)
                    {
                        var denNgay = queryObject.RangeApDung.endDate.GetValueOrDefault();
                        result = result.Where(p => p.DenNgay.Date <= denNgay.Date);
                    }
                }

                if (queryObject.RangeLap != null)
                {
                    //tuNgay <= p.NgayBatDau <= p.NgayKetThuc <= denNgay
                    if (queryObject.RangeLap.startDate != null)
                    {
                        var tuNgay = queryObject.RangeLap.startDate.GetValueOrDefault();
                        result = result.Where(p => tuNgay.Date <= p.NgayBatDauLap.Date);
                    }

                    if (queryObject.RangeLap.endDate != null)
                    {
                        var denNgay = queryObject.RangeLap.endDate.GetValueOrDefault();
                        result = result.Where(p => p.NgayKetThucLap.Date <= denNgay.Date);
                    }
                }
            }

            //Sort KyDuTru (DateTime - DateTime)
            var sortString = queryInfo.SortString.Contains("KyDuTru") ? queryInfo.SortString.Replace("KyDuTru", "TuNgay") : queryInfo.SortString;

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : result.CountAsync();
            var queryTask = result.OrderBy(sortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new KyDuTruSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<KyDuTruSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject.DuocPham == false && queryObject.VatTu == false)
            {
                queryObject.VatTu = true;
                queryObject.DuocPham = true;
            }

            var query = BaseRepository.TableNoTracking;

            if (queryObject.DuocPham == true && queryObject.VatTu == true)
            {
                query = query.Where(p => p.MuaDuocPham == true || p.MuaVatTu == true);
            }
            else if (queryObject.DuocPham == true)
            {
                query = query.Where(p => p.MuaDuocPham == true);
            }
            else
            {
                query = query.Where(p => p.MuaVatTu == true);
            }

            var result = query.Select(p => new KyDuTruGridVo
            {
                Id = p.Id,
                KyDuTru = $"{p.TuNgay.ApplyFormatDate()} - {p.DenNgay.ApplyFormatDate()}",
                TuNgay = p.TuNgay,
                DenNgay = p.DenNgay,
                NhanVienTaoId = p.NhanVienTaoId,
                NhanVienTaoDisplay = p.NhanVien.User.HoTen,
                MuaDuocPham = p.MuaDuocPham,
                MuaVatTu = p.MuaVatTu,
                HieuLuc = p.HieuLuc,
                NgayTao = p.CreatedOn,
                NgayBatDauLap = p.NgayBatDauLap,
                NgayKetThucLap = p.NgayKetThucLap
            });

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    result = result.ApplyLike(searchTerms, p => p.NhanVienTaoDisplay);
                }

                if (queryObject.RangeApDung != null)
                {
                    //tuNgay <= p.TuNgay < p.DenNgay <= denNgay
                    if (queryObject.RangeApDung.startDate != null)
                    {
                        var tuNgay = queryObject.RangeApDung.startDate.GetValueOrDefault();
                        result = result.Where(p => tuNgay.Date <= p.TuNgay.Date);
                    }

                    if (queryObject.RangeApDung.endDate != null)
                    {
                        var denNgay = queryObject.RangeApDung.endDate.GetValueOrDefault();
                        result = result.Where(p => p.DenNgay.Date <= denNgay.Date);
                    }
                }

                if (queryObject.RangeLap != null)
                {
                    //tuNgay <= p.NgayBatDau <= p.NgayKetThuc <= denNgay
                    if (queryObject.RangeLap.startDate != null)
                    {
                        var tuNgay = queryObject.RangeLap.startDate.GetValueOrDefault();
                        result = result.Where(p => tuNgay.Date <= p.NgayBatDauLap.Date);
                    }

                    if (queryObject.RangeLap.endDate != null)
                    {
                        var denNgay = queryObject.RangeLap.endDate.GetValueOrDefault();
                        result = result.Where(p => p.NgayKetThucLap.Date <= denNgay.Date);
                    }
                }
            }

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public bool KiemTraTuNgayDaTonTai(DateTime tuNgay, long kyDuTruId)
        {
            if(kyDuTruId == 0)
            {
                return BaseRepository.TableNoTracking.Any(p => p.TuNgay.Date <= tuNgay.Date && tuNgay.Date <= p.DenNgay.Date);
            }
            else
            {
                return BaseRepository.TableNoTracking.Any(p => p.TuNgay.Date <= tuNgay.Date && tuNgay.Date <= p.DenNgay.Date && p.Id != kyDuTruId);
            }
        }

        public bool KiemTraDenNgayDaTonTai(DateTime denNgay, long kyDuTruId)
        {
            if(kyDuTruId == 0)
            {
                return BaseRepository.TableNoTracking.Any(p => p.TuNgay.Date <= denNgay.Date && denNgay.Date <= p.DenNgay.Date);
            }
            else
            {
                return BaseRepository.TableNoTracking.Any(p => p.TuNgay.Date <= denNgay.Date && denNgay.Date <= p.DenNgay.Date && p.Id != kyDuTruId);
            }
        }

        public bool KiemTraNgayBatDauLapVoiHienTai(DateTime ngayBatDauLap, long kyDuTruId)
        {
            if(kyDuTruId == 0)
            {
                return ngayBatDauLap.Date >= DateTime.Now.Date;
            }
            else
            {
                var kyDuTru = BaseRepository.TableNoTracking.Where(p => p.Id == kyDuTruId).FirstOrDefault();

                if(kyDuTru.NgayBatDauLap.Date == ngayBatDauLap.Date)
                {
                    return true;
                }
                else
                {
                    return ngayBatDauLap.Date >= DateTime.Now.Date;
                }
            }
        }

        public bool KiemTraNgayKetThucLapVoiHienTai(DateTime ngayKetThucLap, long kyDuTruId)
        {
            if (kyDuTruId == 0)
            {
                return ngayKetThucLap.Date >= DateTime.Now.Date;
            }
            else
            {
                var kyDuTru = BaseRepository.TableNoTracking.Where(p => p.Id == kyDuTruId).FirstOrDefault();

                if (kyDuTru.NgayKetThucLap.Date == ngayKetThucLap.Date)
                {
                    return true;
                }
                else
                {
                    return ngayKetThucLap.Date >= DateTime.Now.Date;
                }
            }
        }

        public async Task<bool> IsDaDuocSuDung(long kyDuTruId)
        {
            return await BaseRepository.TableNoTracking.AnyAsync(p => p.Id == kyDuTruId &&
                                                                      (p.DuTruMuaDuocPhams.Any() ||
                                                                      p.DuTruMuaDuocPhamTheoKhoas.Any() ||
                                                                      p.DuTruMuaDuocPhamKhoDuocs.Any() ||
                                                                      p.DuTruMuaVatTus.Any() ||
                                                                      p.DuTruMuaVatTuTheoKhoas.Any() ||
                                                                      p.DuTruMuaVatTuKhoDuocs.Any()));
        }

        public async Task<bool> IsDaDuocSuDungChoDuTruMuaDuocPham(long kyDuTruId)
        {
            return await BaseRepository.TableNoTracking.AnyAsync(p => p.Id == kyDuTruId &&
                                                                      (p.DuTruMuaDuocPhams.Any() ||
                                                                      p.DuTruMuaDuocPhamTheoKhoas.Any() ||
                                                                      p.DuTruMuaDuocPhamKhoDuocs.Any()));
        }

        public async Task<bool> IsDaDuocSuDungChoDuTruMuaVatTu(long kyDuTruId)
        {
            return await BaseRepository.TableNoTracking.AnyAsync(p => p.Id == kyDuTruId &&
                                                                      (p.DuTruMuaVatTus.Any() ||
                                                                      p.DuTruMuaVatTuTheoKhoas.Any() ||
                                                                      p.DuTruMuaVatTuKhoDuocs.Any()));
        }
    }
}
