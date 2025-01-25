using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinhThuePhong;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.CauHinhThuePhong
{
    [ScopedDependency(ServiceType = typeof(ICauHinhThuePhongService))]
    public class CauHinhThuePhongService : MasterFileService<Core.Domain.Entities.CauHinhs.CauHinhThuePhong>, ICauHinhThuePhongService
    {
        public CauHinhThuePhongService(IRepository<Core.Domain.Entities.CauHinhs.CauHinhThuePhong> repository) : base(repository)
        {

        }

        #region Grid
        public async Task<GridDataSource> GetDataForGridCauHinhThuePhongAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var timKiemNangCaoObj = new CauHinhThuePhongTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<CauHinhThuePhongTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.Ten)
                .Where(x => (timKiemNangCaoObj.LoaiPhauThuatId == null || x.LoaiThuePhongPhauThuatId == timKiemNangCaoObj.LoaiPhauThuatId)
                            && (timKiemNangCaoObj.NoiThucHienId == null || x.LoaiThuePhongNoiThucHienId == timKiemNangCaoObj.NoiThucHienId))
                .Select(s => new CauHinhThuePhongGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    LoaiThuePhongPhauThuat = s.LoaiThuePhongPhauThuat.Ten,
                    LoaiThuePhongNoiThucHien = s.LoaiThuePhongNoiThucHien.Ten,
                    BlockThoiGianTheoPhut = s.BlockThoiGianTheoPhut,
                    GiaThue = s.GiaThue,
                    PhanTramNgoaiGio = s.PhanTramNgoaiGio,
                    PhanTramLeTet = s.PhanTramLeTet,
                    GiaThuePhatSinh = s.GiaThuePhatSinh,
                    PhanTramPhatSinhNgoaiGio = s.PhanTramPhatSinhNgoaiGio,
                    PhanTramPhatSinhLeTet = s.PhanTramPhatSinhLeTet,
                    HieuLuc = s.HieuLuc
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridCauHinhThuePhongAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new CauHinhThuePhongTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<CauHinhThuePhongTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.Ten)
                .Where(x => (timKiemNangCaoObj.LoaiPhauThuatId == null || x.LoaiThuePhongPhauThuatId == timKiemNangCaoObj.LoaiPhauThuatId)
                            && (timKiemNangCaoObj.NoiThucHienId == null || x.LoaiThuePhongNoiThucHienId == timKiemNangCaoObj.NoiThucHienId))
                .Select(s => new CauHinhThuePhongGridVo
                {
                    Id = s.Id
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region Get data
        public async Task<List<LookupItemVo>> GetListCauHinhThuePhong(DropDownListRequestModel model)
        {
            var lookups = await BaseRepository.TableNoTracking
                .Where(x => x.HieuLuc)
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                .ApplyLike(model.Query, o => o.Ten)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id
                })
                .Take(model.Take)
                .ToListAsync();
            return lookups;
        }


        #endregion

        #region check data
        public async Task<bool> KiemTraTrungTenAsync(long cauHinhThuePhongId, string ten)
        {
            if (string.IsNullOrEmpty(ten))
            {
                return true;
            }

            var check = await BaseRepository.TableNoTracking
                .AnyAsync(x => x.Ten.Equals(ten)
                               && (cauHinhThuePhongId == 0 || x.Id != cauHinhThuePhongId));
            return !check;
        }


        #endregion
    }
}
