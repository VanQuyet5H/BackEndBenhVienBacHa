using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauChanDoanPhanBiet;
using Camino.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Services.YeucCauKhamBenhChanDoanPhanBiet
{
    [ScopedDependency(ServiceType = typeof(IYeuCauKhamBenhChanDoanPhanBietService))]
    public class YeuCauKhamBenhChanDoanPhanBietService : MasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet>, IYeuCauKhamBenhChanDoanPhanBietService
    {
        IRepository<Core.Domain.Entities.ICDs.ICD> _ICDrepository;
        public YeuCauKhamBenhChanDoanPhanBietService(IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet> repository, IRepository<Core.Domain.Entities.ICDs.ICD> ICDrepository) : base(repository)
        {
            _ICDrepository = ICDrepository;
        }
        public async Task<List<ChucDanhItemVo>> GetListICD(DropDownListRequestModel model)
        {
            var lstICD = await _ICDrepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ma, g => g.TenTiengViet)
                .Take(model.Take)
                .ToListAsync();

            var query = lstICD.Select(item => new ChucDanhItemVo()
            {
                DisplayName = item.Ma + " - " + item.TenTiengViet,
                KeyId = item.Id,
                Ten = item.TenTiengViet,
                Ma = item.Ma,
            }).ToList();
            return query;
        }
        public async Task<ActionResult<GridDataSource>> GetDataGridChanDoanPhanBiet(long idYCKB)
        {
            var query = BaseRepository.TableNoTracking
                .Where(p => p.YeuCauKhamBenhId == idYCKB)
                .Select(source => new YeuCauChanDoanPhanBietGridVo
                {
                    Id = source.Id,
                    ICDId = source.ICDId,
                    GhiChu = source.GhiChu,
                    YeuCauKhamBenhId = source.YeuCauKhamBenhId
                    
                });
            var countTask = query.CountAsync();
            var queryTask = query.ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<List<YeuCauKhamBenhChanDoanPhanBiet>> GetChanDoanPhanBiet(long id)
        {
            var query = BaseRepository.TableNoTracking
                .Where(p => p.Id == id)
                .Select(source => new YeuCauKhamBenhChanDoanPhanBiet
                {
                    Id = source.Id,
                    ICDId = source.ICDId,
                    GhiChu = source.GhiChu,
                    YeuCauKhamBenhId = source.YeuCauKhamBenhId

                }).ToList();

            return query.ToList();
        }
        public async Task<bool> IsTenExists(long? idICD, long yeuCauKhamBenhChanDoanICDId, long yeuCauKhamBenhId)
        {
            if (idICD == null)
            {
                return true;
            }

            var result = false;
            if (yeuCauKhamBenhChanDoanICDId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.ICDId == idICD);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.ICDId == idICD && p.Id != yeuCauKhamBenhChanDoanICDId);
            }
            if (result)
                return false;
            return true;
        }
    }
}
