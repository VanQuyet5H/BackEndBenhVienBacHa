using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhVien;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace Camino.Services.BenhVien
{
    [ScopedDependency(ServiceType = typeof(IBenhVienService))]
    public class BenhVienService : MasterFileService<Core.Domain.Entities.BenhVien.BenhVien>, IBenhVienService
    {
        private readonly IRepository<Core.Domain.Entities.BenhVien.LoaiBenhViens.LoaiBenhVien> _loaiBenhVienrepo;
        public BenhVienService(IRepository<Core.Domain.Entities.BenhVien.BenhVien> repository,
            IRepository<Core.Domain.Entities.BenhVien.LoaiBenhViens.LoaiBenhVien> loaiBenhVienrepo) : base(repository)
        {
            _loaiBenhVienrepo = loaiBenhVienrepo;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var query = BaseRepository.TableNoTracking.Include(p => p.LoaiBenhVien).Include(p => p.DonViHanhChinh)
              .Select(s => new BenhVienGridVo
              {
                  Id = s.Id,
                  Ma = s.Ma,
                  Ten = s.Ten,
                  TenVietTat = s.TenVietTat,
                  TenDonViHanhChinh = s.DonViHanhChinh.Ten,
                  TenLoaiBenhVien = s.LoaiBenhVien.Ten,
                  HieuLuc = s.HieuLuc,
                  HangBenhVien = s.HangBenhVien,
                  HangBenhVienDisplay = s.HangBenhVien.GetDescription(),
                  TuyenChuyenMonKyThuat = s.TuyenChuyenMonKyThuat,
                  TuyenChuyenMonKyThuatDisplay = s.TuyenChuyenMonKyThuat.GetDescription(),
                  SoDienThoaiLanhDao = s.SoDienThoaiLanhDao,
                  SoDienThoaiDisplay = s.SoDienThoaiDisplay
              });
            query = query.ApplyLike(queryInfo.SearchTerms, 
                g => g.Ma,
                g => g.Ten,
                g => g.TenVietTat, 
                g => g.TenDonViHanhChinh, 
                g => g.TenLoaiBenhVien, 
                g => g.SoDienThoaiLanhDao,
                g => g.SoDienThoaiDisplay
                );
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Include(p => p.LoaiBenhVien).Include(p => p.DonViHanhChinh)
                .Select(s => new BenhVienGridVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenVietTat = s.TenVietTat,
                    TenDonViHanhChinh = s.DonViHanhChinh.Ten,
                    TenLoaiBenhVien = s.LoaiBenhVien.Ten,
                    HieuLuc = s.HieuLuc,
                    HangBenhVien = s.HangBenhVien,
                    HangBenhVienDisplay = s.HangBenhVien.GetDescription(),
                    TuyenChuyenMonKyThuat = s.TuyenChuyenMonKyThuat,
                    TuyenChuyenMonKyThuatDisplay = s.TuyenChuyenMonKyThuat.GetDescription(),
                    SoDienThoaiLanhDao = s.SoDienThoaiLanhDao,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay
                });
            query = query.ApplyLike(queryInfo.SearchTerms,
                g => g.Ma,
                g => g.Ten,
                g => g.TenVietTat,
                g => g.TenDonViHanhChinh,
                g => g.TenLoaiBenhVien,
                g => g.SoDienThoaiLanhDao,
                g => g.SoDienThoaiDisplay
            );
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public bool CheckTenBenhVienExits(string ten, long id, bool isTen)
        {
            bool isChecked = false;
            if (string.IsNullOrEmpty(ten) || id != 0)
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => (isTen ? x.Ten == tenCheck.Trim() : x.TenVietTat == tenCheck.Trim()) && x.Id != id);
                if (entity.Any())
                    isChecked = true;
            }
            else
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => isTen ? x.Ten == tenCheck.Trim() : x.TenVietTat == tenCheck.Trim());
                if (entity.Any())
                    isChecked = true;
            }
            return isChecked;
        }

        public async Task<Core.Domain.Entities.BenhVien.BenhVien> GetBenhVienWithMaBenhVien(string maBenhVien)
        {
            if (string.IsNullOrEmpty(maBenhVien)) return null;
            return await BaseRepository.TableNoTracking.Where(p => p.Ma.Equals(maBenhVien)).FirstOrDefaultAsync();
        }

        public bool CheckMaSoBenhVienExits(string maso, long id)
        {
            bool isChecked = false;
            if (string.IsNullOrEmpty(maso) || id != 0)
            {
                string result = !string.IsNullOrEmpty(maso) ? maso.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ma == result.Trim() && x.Id != id);
                if (entity.Any())
                    isChecked = true;
            }
            else
            {
                string result = !string.IsNullOrEmpty(maso) ? maso.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ma == result.Trim());
                if (entity.Any())
                    isChecked = true;
            }
            return isChecked;
        }

        public async Task<Core.Domain.Entities.BenhVien.BenhVien> GetBenhVienById(long id)
        {
            var entity = await BaseRepository.TableNoTracking
                .Include(p => p.LoaiBenhVien)
                .Include(p => p.DonViHanhChinh)
                .Where(p => p.Id == id).FirstOrDefaultAsync();
            return entity;
        }

        public List<LookupItemVo> GetHangBenhVienDescription(DropDownListRequestModel model)
        {
            
            var list = Enum.GetValues(typeof(Enums.HangBenhVien)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }); 
            var searchString = model.Query != null ? model.Query : "";
            var resultList = result.Where(p => Regex.IsMatch(p.DisplayName.ToLower().ConvertToUnSign(), 
                                                searchString.Trim().ToLower().ConvertToUnSign() ?? "" + ".*[mn]"));
            return resultList.ToList();
        }
        
        public List<LookupItemVo> GetTuyenChuyenMonKyThuatDescription(DropDownListRequestModel model)
        {

            var list = Enum.GetValues(typeof(Enums.TuyenChuyenMonKyThuat)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            });
            var searchString = model.Query != null ? model.Query : "";
            var resultList = result.Where(p => Regex.IsMatch(p.DisplayName.ToLower().ConvertToUnSign(), 
                                                searchString.Trim().ToLower().ConvertToUnSign() ?? "" + ".*[mn]"));
            return resultList.ToList();
        }

        public async Task<bool> CheckLoaiBenhVienAsync(long idLoai)
        {
            return await _loaiBenhVienrepo.TableNoTracking.AnyAsync(o => o.Id == idLoai);
        }

    }
}
