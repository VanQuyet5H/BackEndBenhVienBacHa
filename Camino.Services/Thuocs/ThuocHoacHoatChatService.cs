using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.Thuocs
{
    [ScopedDependency(ServiceType = typeof(IThuocHoacHoatChatService))]
    public class ThuocHoacHoatChatService : MasterFileService<ThuocHoacHoatChat>, IThuocHoacHoatChatService
    {
        public ThuocHoacHoatChatService(IRepository<ThuocHoacHoatChat> repository) : base(repository)
        {

        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            {
                BuildDefaultSortExpression(queryInfo);
                RenameSortForFormatColumn(queryInfo);
                if (forExportExcel)
                {
                    queryInfo.Skip = 0;
                    queryInfo.Take = 20000;
                }
                var query = BaseRepository.TableNoTracking
                    .Include(p => p.DuongDung)
                    .Include(p => p.NhomThuoc)
                    .Select(source => new ThuocHoacHoatChatGridVo
                    {
                        Id = source.Id,
                        STTHoatChat = source.STTHoatChat ?? 0,
                        STTThuoc = source.STTThuoc ?? 0,
                        Ma = source.Ma,
                        MaATC = source.MaATC,
                        Ten = source.Ten,
                        DuongDungId = source.DuongDung != null ? source.DuongDung.Id : 0,
                        DuongDung = source.DuongDung != null ? source.DuongDung.Ten : "Không có",
                        HoiChan = source.HoiChan,
                        TyLeBaoHiemThanhToan = source.TyLeBaoHiemThanhToan,
                        CoDieuKienThanhToan = source.CoDieuKienThanhToan,
                        NhomThuocId = source.NhomThuoc.Id,
                        NhomThuoc = source.NhomThuoc.Ten,
                        MoTa = source.MoTa,
                        BenhVienHang1 = source.BenhVienHang1,
                        BenhVienHang2 = source.BenhVienHang2,
                        BenhVienHang3 = source.BenhVienHang3,
                        BenhVienHang4 = source.BenhVienHang4
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.MaATC,
                        g => g.DuongDung,
                        g => g.NhomThuoc,
                        g => g.MoTa);

                var countTask = queryInfo.LazyLoadPage == true ?
                    Task.FromResult(0) :
                    query.CountAsync();

                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);

                return new GridDataSource
                {
                    Data = queryTask.Result,
                    TotalRowCount = countTask.Result
                };
            }
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var query = BaseRepository.TableNoTracking
                    .Include(p => p.DuongDung)
                    .Include(p => p.NhomThuoc)
                    .Select(source => new ThuocHoacHoatChatGridVo
                    {
                        Id = source.Id,
                        STTHoatChat = source.STTHoatChat ?? 0,
                        STTThuoc = source.STTThuoc ?? 0,
                        Ma = source.Ma,
                        MaATC = source.MaATC,
                        Ten = source.Ten,
                        DuongDungId = source.DuongDung != null ? source.DuongDung.Id : 0,
                        DuongDung = source.DuongDung != null ? source.DuongDung.Ten : "Không có",
                        HoiChan = source.HoiChan,
                        TyLeBaoHiemThanhToan = source.TyLeBaoHiemThanhToan,
                        CoDieuKienThanhToan = source.CoDieuKienThanhToan,
                        NhomThuocId = source.NhomThuoc.Id,
                        NhomThuoc = source.NhomThuoc.Ten,
                        MoTa = source.MoTa,
                        BenhVienHang1 = source.BenhVienHang1,
                        BenhVienHang2 = source.BenhVienHang2,
                        BenhVienHang3 = source.BenhVienHang3,
                        BenhVienHang4 = source.BenhVienHang4
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.MaATC,
                        g => g.DuongDung,
                        g => g.NhomThuoc,
                        g => g.MoTa);

                var countTask = query.CountAsync();

                await Task.WhenAll(countTask);

                return new GridDataSource
                {
                    TotalRowCount = countTask.Result
                };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking
                    .Include(p => p.DuongDung)
                    .Include(p => p.NhomThuoc)
                    .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
                     || p.MaATC.Contains(searchString) || p.DuongDung.Ten.Contains(searchString)
                     || p.NhomThuoc.Ten.Contains(searchString) || p.MoTa.Contains(searchString))
                    .Select(source => new ThuocHoacHoatChatGridVo
                    {
                        Id = source.Id,
                        STTHoatChat = source.STTHoatChat ?? 0,
                        STTThuoc = source.STTThuoc ?? 0,
                        Ma = source.Ma,
                        MaATC = source.MaATC,
                        Ten = source.Ten,
                        DuongDungId = source.DuongDung != null ? source.DuongDung.Id : 0,
                        DuongDung = source.DuongDung != null ? source.DuongDung.Ten : "Không có",
                        HoiChan = source.HoiChan,
                        TyLeBaoHiemThanhToan = source.TyLeBaoHiemThanhToan,
                        CoDieuKienThanhToan = source.CoDieuKienThanhToan,
                        NhomThuocId = source.NhomThuoc.Id,
                        NhomThuoc = source.NhomThuoc.Ten,
                        MoTa = source.MoTa,
                        BenhVienHang1 = source.BenhVienHang1,
                        BenhVienHang2 = source.BenhVienHang2,
                        BenhVienHang3 = source.BenhVienHang3,
                        BenhVienHang4 = source.BenhVienHang4
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.MaATC,
                        g => g.DuongDung,
                        g => g.NhomThuoc,
                        g => g.MoTa);

                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource
                {
                    TotalRowCount = countTask.Result
                };
            }
        }

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) &&
                queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?
                    .Replace("Format", "");
            }
        }

        public async Task<bool> IsMaAtcExists(string maAtc = null, long thuocHoacHoatChatId = 0)
        {
            bool result;

            if (thuocHoacHoatChatId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.MaATC.Equals(maAtc));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.MaATC.Equals(maAtc) && p.Id != thuocHoacHoatChatId);
            }

            return result;
        }

        public async Task<List<MaHoatChatHoatChatDuongDungTemplateVo>> LookupThuocHoacHoatChat(DropDownListRequestModel model)
        {
            var listDuongDung = await BaseRepository.TableNoTracking.Include(o=>o.DuongDung)
                .Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? ""))
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .OrderBy(o=>o.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = listDuongDung.Select(item => new MaHoatChatHoatChatDuongDungTemplateVo
            {
                HoatChat = item.Ten,
                MaHoatChat = item.Ma,
                DuongDung = item.DuongDung?.Ten
            }).ToList();

            return query;
        }
    }
}
