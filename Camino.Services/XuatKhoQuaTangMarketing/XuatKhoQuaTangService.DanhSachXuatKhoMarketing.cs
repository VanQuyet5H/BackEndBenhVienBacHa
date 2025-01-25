using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Marketing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System;
using System.Globalization;
using Camino.Core.Helpers;
namespace Camino.Services.XuatKhoQuaTangMarketing
{
    public partial class XuatKhoQuaTangService
    {
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Select(p => new XuatKhoQuaTangMarketingGridVo
                {
                    Id = p.Id,
                    SoPhieu = p.SoPhieu,
                    NoiXuat = "Kho Marketing",
                    NguoiXuatId = p.NguoiXuatId,
                    NhanVienXuat = p.NguoiXuat.User.HoTen,
                    BenhNhanId = p.BenhNhanId,
                    NguoiNhan = p.BenhNhan.HoTen,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    NgayXuat = p.NgayXuat,
                    NgayXuatDisplay = p.NgayXuat.ApplyFormatDateTimeSACH(),
                    GhiChu = p.GhiChu
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoQuaTangMarketingGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.NgayXuat >= tuNgay && p.NgayXuat <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(queryInfo.SearchTerms, 
                        g => g.SoPhieu,
                        g => g.NhanVienXuat,
                        g => g.NguoiNhan,
                        g => g.NhanVienXuat,
                        g => g.GhiChu);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.SoPhieu,
                        g => g.NhanVienXuat,
                        g => g.NguoiNhan,
                        g => g.NhanVienXuat,
                        g => g.GhiChu
                        );

            }
            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                 .Select(p => new XuatKhoQuaTangMarketingGridVo
                 {
                     Id = p.Id,
                     SoPhieu = p.SoPhieu,
                     NoiXuat = "Kho Marketing",
                     NguoiXuatId = p.NguoiXuatId,
                     NhanVienXuat = p.NguoiXuat.User.HoTen,
                     BenhNhanId = p.BenhNhanId,
                     NguoiNhan = p.BenhNhan.HoTen,
                     YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                     NgayXuat = p.NgayXuat,
                     NgayXuatDisplay = p.NgayXuat.ApplyFormatDateTimeSACH(),
                     GhiChu = p.GhiChu
                 });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoQuaTangMarketingGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.NgayXuat >= tuNgay && p.NgayXuat <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.SoPhieu,
                        g => g.NhanVienXuat,
                        g => g.NguoiNhan,
                        g => g.NhanVienXuat,
                        g => g.GhiChu);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.SoPhieu,
                        g => g.NhanVienXuat,
                        g => g.NguoiNhan,
                        g => g.NhanVienXuat,
                        g => g.GhiChu
                        );

            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
    }
}
