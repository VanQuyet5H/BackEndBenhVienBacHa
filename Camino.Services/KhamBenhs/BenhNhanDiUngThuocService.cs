using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauChanDoanPhanBiet;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IBenhNhanDiUngThuocService))]
    public class BenhNhanDiUngThuocService
        : MasterFileService<BenhNhanDiUngThuoc>
            , IBenhNhanDiUngThuocService
    {
        private IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private IRepository<ThuocHoacHoatChat> _thuocHoacHoatChatRepository;
        private IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet> _yeuCauKhamBenhChanDoanPhanBietRepository;
        public BenhNhanDiUngThuocService
        (
            IRepository<BenhNhanDiUngThuoc> repository, IRepository<ThuocHoacHoatChat> thuocHoacHoatChatRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet> yeuCauKhamBenhChanDoanPhanBietRepository
        )
            : base(repository)
        {
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _thuocHoacHoatChatRepository = thuocHoacHoatChatRepository;
            _yeuCauKhamBenhChanDoanPhanBietRepository = yeuCauKhamBenhChanDoanPhanBietRepository;
        }

        public async Task<ActionResult<GridDataSource>> GetDataGridBenhNhanDiUngThuoc(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(p => p.BenhNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(source => new BenhNhanDiUngThuocKhamBenhGridVo
                {
                    Id = source.Id,
                    BieuHienDiUng = source.BieuHienDiUng,
                    LoaiDiUng = source.LoaiDiUng,
                    TenLoaiDiUng = source.LoaiDiUng.GetDescription(),
                    TenDiUng = source.TenDiUng,
                    MucDo = source.MucDo,
                    TenMucDo = source.MucDo.GetDescription()
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        // get 
        public async Task<ActionResult<GridDataSource>> GetDataGridChanDoanPhanBiet(long idYCKB)
        {
            var query = _yeuCauKhamBenhChanDoanPhanBietRepository.TableNoTracking
                .Where(p => p.YeuCauKhamBenhId == idYCKB)
                .Select(source => new YeuCauChanDoanPhanBietGridVo
                {
                    Id = source.Id,
                    MaICd = source.ICD.Ma + "-" + source.ICD.TenTiengViet,
                    ICDId = source.ICDId,
                    GhiChu = source.GhiChu,
                    YeuCauKhamBenhId = source.YeuCauKhamBenhId

                });
            var countTask = query.CountAsync();
            var queryTask = query.ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<ActionResult<GridDataSource>> GetDataGridChanDoanKemTheo(long idYCKB)
        {
            var query = _yeuCauKhamBenhRepository.TableNoTracking.Where(p => p.Id == idYCKB).SelectMany(cc=>cc.YeuCauKhamBenhICDKhacs)
                .Select(source => new YeuCauChanDoanPhanBietGridVo
                {
                    Id = source.Id,
                    MaICd = source.ICD.Ma + "-" + source.ICD.TenTiengViet,
                    ICDId = source.ICDId,
                    GhiChu = source.GhiChu,
                    YeuCauKhamBenhId = source.YeuCauKhamBenhId

                });
            var countTask = query.CountAsync();
            var queryTask = query.ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<bool> IsThuocExists(Enums.LoaiDiUng loaiDiUng, long thuocId, long benhNhanId, long id)
        {
            if (thuocId != 0 && loaiDiUng == Enums.LoaiDiUng.Thuoc)
            {
                bool result;
                var tenDiUng = await _thuocHoacHoatChatRepository.TableNoTracking
                    .Where(p => p.Id == thuocId)
                    .Select(p => p.Ten)
                    .FirstOrDefaultAsync();

                if (id == 0)
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p =>
                        p.TenDiUng == tenDiUng && p.BenhNhanId == benhNhanId);
                    return result;
                }

                result = await BaseRepository.TableNoTracking.AnyAsync(p =>
                    p.TenDiUng == tenDiUng && p.BenhNhanId == benhNhanId && p.Id != id);
                return result;
            }

            return false;
        }

        public List<LookupItemVo> GetListMucDoDiUng()
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.EnumMucDoDiUng>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();
            return lstEnums;
        }
    }
}
