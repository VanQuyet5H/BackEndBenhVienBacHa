using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Data;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;
using Camino.Core.Domain;

namespace Camino.Services.Thuocs
{
    [ScopedDependency(ServiceType = typeof(IADRService))]
    public class ADRService : MasterFileService<ADR>, IADRService
    {
        private readonly IRepository<ThuocHoacHoatChat> _thuocHoacHoatChatRepository;
        public ADRService(
            IRepository<ADR> repository,
            IRepository<ThuocHoacHoatChat> thuocHoacHoatChatRepository
            ) : base(repository)
        {
            _thuocHoacHoatChatRepository = thuocHoacHoatChatRepository;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository
                .TableNoTracking
                .Include(o => o.ThuocHoacHoatChat1)
                .Include(o => o.ThuocHoacHoatChat2)
                .Select(s => new ADRGridVo
                {
                    Id = s.Id,
                    ThuocHoacHoatChat1Id = s.ThuocHoacHoatChat1Id,
                    Ma1ThuocHoacHoatChat = s.ThuocHoacHoatChat1.Ma,
                    TenThuocHoacHoatChat1 = s.ThuocHoacHoatChat1.Ten,
                    MaTenHoatChat1 = s.ThuocHoacHoatChat1.Ma + "-" + s.ThuocHoacHoatChat1.Ten,
                    ThuocHoacHoatChat2Id = s.ThuocHoacHoatChat2Id,
                    Ma2ThuocHoacHoatChat = s.ThuocHoacHoatChat2.Ma,
                    TenThuocHoacHoatChat2 = s.ThuocHoacHoatChat2.Ten,
                    MaTenHoatChat2 = s.ThuocHoacHoatChat2.Ma + "-" + s.ThuocHoacHoatChat2.Ten,
                    MucDoChuYKhiChiDinh = s.MucDoChuYKhiChiDinh,
                    MucDoChuYKhiChiDinhDisplay = s.MucDoChuYKhiChiDinh.GetDescription(),
                    MucDoTuongTac = s.MucDoTuongTac,
                    MucDoTuongTacDisplay = s.MucDoTuongTac.GetDescription(),
                    DuocDongHoc = s.DuocDongHoc,
                    DuocDongHocDisplay = s.DuocDongHoc == true ? "Có" : "Không",
                    DuocLucHoc = s.DuocLucHoc,
                    DuocLucHocDisplay = s.DuocLucHoc == true ? "Có" : "Không",
                    ThuocThucAn = s.ThuocThucAn,
                    ThuocThucAnDisplay = s.ThuocThucAn == true ? "Có" : "Không",
                    QuyTac = s.QuyTac,
                    QuyTacDisplay = s.QuyTac == true ? "Có" : "Không",
                    TuongTacHauQua = s.TuongTacHauQua,
                    CachXuLy = s.CachXuLy,
                    GhiChu = s.GhiChu
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenThuocHoacHoatChat1, g => g.TenThuocHoacHoatChat2, g => g.TuongTacHauQua, g => g.CachXuLy);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Include(o => o.ThuocHoacHoatChat1)
                .Include(o => o.ThuocHoacHoatChat2)
                .Select(s => new ADRGridVo
                {
                    Id = s.Id,
                    ThuocHoacHoatChat1Id = s.ThuocHoacHoatChat1Id,
                    Ma1ThuocHoacHoatChat = s.ThuocHoacHoatChat1.Ma,
                    TenThuocHoacHoatChat1 = s.ThuocHoacHoatChat1.Ten,
                    MaTenHoatChat1 = s.ThuocHoacHoatChat1.Ma + "-" + s.ThuocHoacHoatChat1.Ten,
                    ThuocHoacHoatChat2Id = s.ThuocHoacHoatChat2Id,
                    Ma2ThuocHoacHoatChat = s.ThuocHoacHoatChat2.Ma,
                    TenThuocHoacHoatChat2 = s.ThuocHoacHoatChat2.Ten,
                    MaTenHoatChat2 = s.ThuocHoacHoatChat2.Ma + "-" + s.ThuocHoacHoatChat2.Ten,
                    MucDoChuYKhiChiDinh = s.MucDoChuYKhiChiDinh,
                    MucDoChuYKhiChiDinhDisplay = s.MucDoChuYKhiChiDinh.GetDescription(),
                    MucDoTuongTac = s.MucDoTuongTac,
                    MucDoTuongTacDisplay = s.MucDoTuongTac.GetDescription(),
                    DuocDongHoc = s.DuocDongHoc,
                    DuocDongHocDisplay = s.DuocDongHoc == true ? "Có" : "Không",
                    DuocLucHoc = s.DuocLucHoc,
                    DuocLucHocDisplay = s.DuocLucHoc == true ? "Có" : "Không",
                    ThuocThucAn = s.ThuocThucAn,
                    ThuocThucAnDisplay = s.ThuocThucAn == true ? "Có" : "Không",
                    QuyTac = s.QuyTac,
                    QuyTacDisplay = s.QuyTac == true ? "Có" : "Không",
                    TuongTacHauQua = s.TuongTacHauQua,
                    CachXuLy = s.CachXuLy,
                    GhiChu = s.GhiChu
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenThuocHoacHoatChat1, g => g.TenThuocHoacHoatChat2, g => g.TuongTacHauQua, g => g.CachXuLy);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<HoatChatTemplateVo>> GetListHoatChatAsync(DropDownListRequestModel queryInfo)
        {
            var lstHoatChat = await _thuocHoacHoatChatRepository.TableNoTracking
                .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten).Take(queryInfo.Take)
                .ToListAsync();
            var result = lstHoatChat.Select(item => new HoatChatTemplateVo
            {
                DisplayName = item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
            }).ToList();
            return result;
        }

        public async Task<List<LookupItemVo>> GetHoatChatAsync(DropDownListRequestModel queryInfo)
        {
            var result = _thuocHoacHoatChatRepository.TableNoTracking
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).Distinct().ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take).ToListAsync();
            await Task.WhenAll(result);
            return result.Result;
        }
        public List<LookupItemVo> GetChuYKhiChiDinhDescription()
        {

            var list = Enum.GetValues(typeof(Enums.MucDoChuYKhiChiDinh)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return result;
        }

        public List<LookupItemVo> GetTuongTacDescription()
        {

            var list = Enum.GetValues(typeof(Enums.MucDoTuongTac)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return result;
        }

        public async Task<bool> IsTenHoatChatExists(long hoatChatId1 = 0, long hoatChatId2 = 0, long Id = 0)
        {
            bool result;
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.ThuocHoacHoatChat1Id == hoatChatId1 && p.ThuocHoacHoatChat2Id == hoatChatId2);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.ThuocHoacHoatChat1Id == hoatChatId1 && p.ThuocHoacHoatChat2Id == hoatChatId2 && p.Id != Id);
            }
            return result;
        }


        public async Task<bool> CheckHoatChatDeleted(long id)
        {
            var check = await _thuocHoacHoatChatRepository.TableNoTracking
                .Where(x => x.Id == id).ToListAsync();
            if (check.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
