using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HeThong;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.HeThong
{
    [ScopedDependency(ServiceType = typeof(INhatKyHeThongService))]
    public class NhatKyHeThongService:MasterFileService<NhatKyHeThong>, INhatKyHeThongService
    {
        private readonly IRepository<User> _userRepository;
        public NhatKyHeThongService(IRepository<NhatKyHeThong> repository, IRepository<User> userRepository):base(repository)
        {
            _userRepository = userRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            
            var query = BaseRepository.TableNoTracking.Include(rf => rf.UserDetails)
                .Select(x => new NhatKyHeThongGridVo
            {
                TenHoatDong = x.HoatDong.GetDescription(),
                HoatDong = x.HoatDong,
                MaDoiTuong = x.MaDoiTuong,
                IdDoiTuong = x.IdDoiTuong,
                NoiDung = x.NoiDung,
                NguoiTao = x.UserDetails.HoTen,
                NguoiTaoId = x.CreatedById,
                NgayTaoFormat = x.CreatedOn != null ? x.CreatedOn.GetValueOrDefault().ApplyFormatDateTimeSACH() : string.Empty,
                NgayTao = x.CreatedOn
            });

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,g=>g.NoiDung, g => g.NguoiTao);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<NhatKyHeThongGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.HoatDong != 0)
                {
                    query = query.Where(p => p.HoatDong == queryString.HoatDong);
                }
                if (queryString.NguoiTaoId != null && queryString.NguoiTaoId != 0)
                {
                    query = query.Where(p => p.NguoiTaoId == queryString.NguoiTaoId);
                }
                if (!string.IsNullOrEmpty(queryString.NoiDung))
                {
                    query = query.Where(p => p.NoiDung.ToLower()
                                                 .Contains(queryString.NoiDung.ToLower().TrimEnd().TrimStart()));
                }
                if (!string.IsNullOrEmpty(queryString.TuNgay) || !string.IsNullOrEmpty(queryString.DenNgay))
                {
                    var tuNgayTemp = string.IsNullOrEmpty(queryString.TuNgay)
                        ? new DateTime().Date
                        : Convert.ToDateTime(queryString.TuNgay);
                    var denNgayTemp = string.IsNullOrEmpty(queryString.DenNgay)
                        ? DateTime.Now.Date
                        : Convert.ToDateTime(queryString.DenNgay);

                    query = query.Where(p => p.NgayTao.Value.Date >= tuNgayTemp && p.NgayTao.Value.Date <= denNgayTemp);
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Include(rf => rf.UserDetails)
                .Select(x => new NhatKyHeThongGridVo
            {
                TenHoatDong = x.HoatDong.GetDescription(),
                HoatDong = x.HoatDong,
                MaDoiTuong = x.MaDoiTuong,
                IdDoiTuong = x.IdDoiTuong,
                NoiDung = x.NoiDung,
                NguoiTao = x.UserDetails.HoTen,
                NguoiTaoId = x.CreatedById,
                NgayTaoFormat = x.CreatedOn != null ? x.CreatedOn.GetValueOrDefault().ApplyFormatDateTime() : string.Empty,
                NgayTao = x.CreatedOn
                });

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                query = query.ApplyLike(queryInfo.SearchTerms, g=>g.NoiDung, g => g.NguoiTao);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<NhatKyHeThongGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.HoatDong != 0)
                {
                    query = query.Where(p => p.HoatDong == queryString.HoatDong);
                }
                if (queryString.NguoiTaoId != null && queryString.NguoiTaoId != 0)
                {
                    query = query.Where(p => p.NguoiTaoId == queryString.NguoiTaoId);
                }
                if (!string.IsNullOrEmpty(queryString.NoiDung))
                {
                    query = query.Where(p => p.NoiDung.ToLower()
                        .Contains(queryString.NoiDung.ToLower().TrimEnd().TrimStart()));
                }
                if (!string.IsNullOrEmpty(queryString.TuNgay) || !string.IsNullOrEmpty(queryString.DenNgay))
                {
                    var tuNgayTemp = string.IsNullOrEmpty(queryString.TuNgay)
                        ? new DateTime().Date
                        : Convert.ToDateTime(queryString.TuNgay);
                    var denNgayTemp = string.IsNullOrEmpty(queryString.DenNgay)
                        ? DateTime.Now.Date
                        : Convert.ToDateTime(queryString.DenNgay);

                    query = query.Where(p => p.NgayTao.Value.Date >= tuNgayTemp && p.NgayTao.Value.Date <= denNgayTemp);
                }
            }

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetHoatDongAsync()
        {
            var listLoaiMua = Enum.GetValues(typeof(Enums.EnumNhatKyHeThong)).Cast<Enum>();
            var result = listLoaiMua.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            return result;
        }

        public async Task<List<LookupItemVo>> GetNguoiTaoAsync(DropDownListRequestModel queryInfo)
        {
            var result = _userRepository.TableNoTracking//.Where(x => x.FullName.Contains(queryInfo.Query))
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.HoTen,
                    KeyId = item.Id,
                }).ApplyLike(queryInfo.Query, o=>o.DisplayName).Take(queryInfo.Take).ToListAsync();

            await Task.WhenAll(result);
            var defaultItem = new LookupItemVo()
            {
                DisplayName = "Tất cả",
                KeyId = 0
            };
            result.Result.Insert(0, defaultItem);
            return result.Result;
        }
    }
}
