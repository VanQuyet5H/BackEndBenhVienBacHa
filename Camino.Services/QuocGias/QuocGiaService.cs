using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuocGia;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.QuocGias
{
    public class QuocGiaRelationships
    {
        public long Id { get; set; }
        public string Ten { get; set; }
        public bool NhaSanXuatTheoQuocGias { get; set; }
        public bool QuocTichBenhNhans { get; set; }
        public bool YeuCauTiepNhans { get; set; }
        public bool DanTocs { get; set; }
    }

    [ScopedDependency(ServiceType = typeof(IQuocGiaService))]
    public class QuocGiaService : MasterFileService<QuocGia> , IQuocGiaService
    {
        public QuocGiaService (IRepository<QuocGia> repository) : base(repository)
        {
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            
            var query = BaseRepository.TableNoTracking
                .Select(s => new QuocGiaGridVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenVietTat = s.TenVietTat,
                    QuocTich = s.QuocTich,
                    MaDienThoaiQuocTe = s.MaDienThoaiQuocTe,
                    ThuDo = s.ThuDo,
                    ChauLuc = s.ChauLuc.GetDescription(),
                    IsDisabled = s.IsDisabled
                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.Ma,
                    g => g.Ten,
                    g => g.TenVietTat,
                    g => g.QuocTich,
                    g => g.MaDienThoaiQuocTe,
                    g => g.ThuDo
                );

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
                .Select(s => new QuocGiaGridVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenVietTat = s.TenVietTat,
                    QuocTich = s.QuocTich,
                    MaDienThoaiQuocTe = s.MaDienThoaiQuocTe,
                    ThuDo = s.ThuDo,
                    //ChauLuc = s.ChauLuc.GetDescription(),
                    IsDisabled = s.IsDisabled
                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.Ma,
                    g => g.Ten,
                    g => g.TenVietTat,
                    g => g.QuocTich,
                    g => g.MaDienThoaiQuocTe,
                    g => g.ThuDo
                );

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
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

        public List<ChauLucVo> GetListChauLuc(DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumChauLuc>();

            var result = listEnum.Select(item => new ChauLucVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return result;
        }

        public async Task<QuocGiaRelationships> GetQuocGiaRelationshipsById(long id)
        {
            var quocGia = await BaseRepository.TableNoTracking.Where(q => q.Id == id).Select(q => new QuocGiaRelationships
            {
                Id = q.Id,
                Ten = q.Ten,
                NhaSanXuatTheoQuocGias = q.NhaSanXuatTheoQuocGias.Any(),
                QuocTichBenhNhans = q.QuocTichBenhNhans.Any(),
                YeuCauTiepNhans = q.YeuCauTiepNhans.Any(),
                DanTocs = q.DanTocs.Any()
            }).FirstOrDefaultAsync();

            return quocGia;
        }

        public async Task<IEnumerable<QuocGiaRelationships>> GetQuocGiaRelationshipsByIds(long[] ids)
        {
            var quocGia = await BaseRepository.TableNoTracking.Where(q => ids.IndexOf(q.Id) >= 0).Select(q => new QuocGiaRelationships
            {
                Id = q.Id,
                Ten = q.Ten,
                NhaSanXuatTheoQuocGias = q.NhaSanXuatTheoQuocGias.Any(),
                QuocTichBenhNhans = q.QuocTichBenhNhans.Any(),
                YeuCauTiepNhans = q.YeuCauTiepNhans.Any(),
                DanTocs = q.DanTocs.Any()
            }).ToListAsync();

            return quocGia;
        }

        public async Task<bool> IsMaExists(string ma = null, long quocGiaId = 0)
        {
            bool result;

            if(quocGiaId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != quocGiaId);
            }

            return result;
        }

        public async Task<bool> IsTenVietTatExists(string tenVietTat = null, long quocGiaId = 0)
        {
            bool result;
            
            if(quocGiaId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenVietTat.Equals(tenVietTat));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenVietTat.Equals(tenVietTat) && p.Id != quocGiaId);
            }

            return result;
        }

        public bool CheckChauLuc(Enums.EnumChauLuc param)
        {
            if (param != 0)
            {
                return Enum.IsDefined(typeof(Enums.EnumChauLuc), param.ToString());
            }

            return true;
        }

        public string GetTenChauLuc(Enums.EnumChauLuc param)
        {
            return EnumHelper.GetDescription(param);
        }

        public bool ContainNumber(string param)
        {
            if(!string.IsNullOrEmpty(param) && param.Length <= 100)
            {
                return NumberHelper.ContainNumber(param);
            }
            
            return true;
        }
    }
}
