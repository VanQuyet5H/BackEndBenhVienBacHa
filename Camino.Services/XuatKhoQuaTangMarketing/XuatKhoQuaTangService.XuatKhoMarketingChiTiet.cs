using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Marketing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Camino.Services.XuatKhoQuaTangMarketing
{
    public partial class XuatKhoQuaTangService
    {
        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var xuatKhoQuaTangId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _xuatKhoQuaTangChiTietRepository.TableNoTracking
                .Where(p => p.XuatKhoQuaTangId == xuatKhoQuaTangId)
                .Select(p => new XuatKhoQuaTangMarketingChiTietGridVo
                {
                    Id = p.Id,
                    Ten = p.QuaTang.Ten,
                    DonViTinh = p.QuaTang.DonViTinh,
                    SoLuongXuat = p.SoLuongXuat
                });


            var queryTask = query
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

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var xuatKhoQuaTangId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _xuatKhoQuaTangChiTietRepository.TableNoTracking
                .Where(p => p.XuatKhoQuaTangId == xuatKhoQuaTangId)
                .Select(p => new XuatKhoQuaTangMarketingChiTietGridVo
                {
                    Id = p.Id,
                    Ten = p.QuaTang.Ten,
                    DonViTinh = p.QuaTang.DonViTinh,
                    SoLuongXuat = p.SoLuongXuat
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetNhanVienXuat(DropDownListRequestModel queryInfo)
        {
            var result = _nhanVienRepository.TableNoTracking
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.User.HoTen
                        })
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetNguoiNhan(DropDownListRequestModel queryInfo)
        {
            var result = _benhNhanRepository.TableNoTracking
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.HoTen
                        })
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);
            return await result.ToListAsync();
        }
    }
}
