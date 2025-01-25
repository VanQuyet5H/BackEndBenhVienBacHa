using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LoaiGiaDichVus;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.LoaiGiaDichVus
{
    [ScopedDependency(ServiceType = typeof(ILoaiGiaDichVuService))]
    public class LoaiGiaDichVuService : MasterFileService<NhomGiaDichVuKhamBenhBenhVien>, ILoaiGiaDichVuService
    {
        private IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;
        private IRepository<NhomGiaDichVuGiuongBenhVien> _nhomGiaDichVuGiuongBenhVienRepository;
        private readonly ILocalizationService _localizationService;
        public LoaiGiaDichVuService(IRepository<NhomGiaDichVuKhamBenhBenhVien> repository
            , IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository
            , IRepository<NhomGiaDichVuGiuongBenhVien> nhomGiaDichVuGiuongBenhVienRepository
            , ILocalizationService localizationService
            ) : base(repository)
        {
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _nhomGiaDichVuGiuongBenhVienRepository = nhomGiaDichVuGiuongBenhVienRepository;
            _localizationService = localizationService;
        }

        #region Grid
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstGiaDichVuKham = BaseRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten)
                .Select(s => new LoaiGiaDichVuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Nhom = Enums.NhomDichVuLoaiGia.DichVuKhamBenh
                }).OrderBy(queryInfo.SortString).ToList();

            var lstGiaDichVuKyThuat = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten)
                .Select(s => new LoaiGiaDichVuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Nhom = Enums.NhomDichVuLoaiGia.DichVuKyThuat
                }).OrderBy(queryInfo.SortString).ToList();

            var lstGiaDichVuGiuong = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten)
                .Select(s => new LoaiGiaDichVuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Nhom = Enums.NhomDichVuLoaiGia.DichVuGiuongBenh
                }).OrderBy(queryInfo.SortString).ToList();

            var results = lstGiaDichVuKham.Concat(lstGiaDichVuKyThuat).Concat(lstGiaDichVuGiuong)
                .Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            return new GridDataSource { Data = results.ToArray(), TotalRowCount = results.Count };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var countGiaDichVuKham = BaseRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten).Count();

            var countGiaDichVuKyThuat = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten).Count();

            var countGiaDichVuGiuong = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten).Count();

            var total = countGiaDichVuKham + countGiaDichVuKyThuat + countGiaDichVuGiuong;
            return new GridDataSource { TotalRowCount = total };
        }


        #endregion

        #region CRUD

        public async Task<LoaiGiaDichVuGridVo> GetThongTinLoaiGiaAsync(LoaiGiaDichVuInfoVo info)
        {
            var result = new LoaiGiaDichVuGridVo();
            if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuKhamBenh)
            {
                result = BaseRepository.TableNoTracking
                    .Where(x => x.Id == info.Id)
                    .Select(x => new LoaiGiaDichVuGridVo()
                    {
                        Id = x.Id,
                        Ten = x.Ten,
                        Nhom = Enums.NhomDichVuLoaiGia.DichVuKhamBenh
                    }).FirstOrDefault();
                
            }
            else if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuKyThuat)
            {
                result = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.Id == info.Id)
                    .Select(x => new LoaiGiaDichVuGridVo()
                    {
                        Id = x.Id,
                        Ten = x.Ten,
                        Nhom = Enums.NhomDichVuLoaiGia.DichVuKyThuat
                    }).FirstOrDefault();
            }
            else if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuGiuongBenh)
            {
                result = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
                    .Where(x => x.Id == info.Id)
                    .Select(x => new LoaiGiaDichVuGridVo()
                    {
                        Id = x.Id,
                        Ten = x.Ten,
                        Nhom = Enums.NhomDichVuLoaiGia.DichVuGiuongBenh
                    }).FirstOrDefault();
            }

            if (result == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            return result;
        }

        public async Task ThemLoaiGia(LoaiGiaDichVuGridVo info)
        {
            if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuKhamBenh)
            {
                var entity = new NhomGiaDichVuKhamBenhBenhVien()
                {
                    Ten = info.Ten
                };
                BaseRepository.Add(entity);
            }
            else if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuKyThuat)
            {
                var entity = new NhomGiaDichVuKyThuatBenhVien()
                {
                    Ten = info.Ten
                };
                _nhomGiaDichVuKyThuatBenhVienRepository.Add(entity);
            }
            else if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuGiuongBenh)
            {
                var entity = new NhomGiaDichVuGiuongBenhVien()
                {
                    Ten = info.Ten
                };
                _nhomGiaDichVuGiuongBenhVienRepository.Add(entity);
            }
        }

        public async Task CapNhatLoaiGia(LoaiGiaDichVuGridVo info)
        {
            if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuKhamBenh)
            {
                var entity = BaseRepository.GetById(info.Id);
                info.MapTo(entity);
                BaseRepository.Context.SaveChanges();
            }
            else if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuKyThuat)
            {
                var entity = _nhomGiaDichVuKyThuatBenhVienRepository.GetById(info.Id);
                info.MapTo(entity);
                _nhomGiaDichVuKyThuatBenhVienRepository.Context.SaveChanges();
            }
            else if (info.Nhom == Enums.NhomDichVuLoaiGia.DichVuGiuongBenh)
            {
                var entity = _nhomGiaDichVuGiuongBenhVienRepository.GetById(info.Id);
                info.MapTo(entity);
                _nhomGiaDichVuGiuongBenhVienRepository.Context.SaveChanges();
            }
        }

        public async Task XuLyXoaLoaiGiaAsync(long id, Enums.NhomDichVuLoaiGia nhom)
        {
            if (nhom == Enums.NhomDichVuLoaiGia.DichVuKhamBenh)
            {
                var loaiGia = BaseRepository.GetById(id);
                BaseRepository.Delete(loaiGia);
            }
            else if (nhom == Enums.NhomDichVuLoaiGia.DichVuKyThuat)
            {
                var loaiGia = _nhomGiaDichVuKyThuatBenhVienRepository.GetById(id);
                _nhomGiaDichVuKyThuatBenhVienRepository.Delete(loaiGia);
            }
            else if (nhom == Enums.NhomDichVuLoaiGia.DichVuGiuongBenh)
            {
                var loaiGia = _nhomGiaDichVuGiuongBenhVienRepository.GetById(id);
                _nhomGiaDichVuGiuongBenhVienRepository.Delete(loaiGia);
            }
        }

        public async Task<bool> KiemTraTrungTenTheoNhom(long id, Enums.NhomDichVuLoaiGia? nhom, string ten)
        {
            if (nhom == null || string.IsNullOrEmpty(ten))
            {
                return false;
            }

            var check = false;
            if (nhom == Enums.NhomDichVuLoaiGia.DichVuKhamBenh)
            {
                check = BaseRepository.TableNoTracking
                    .Any(a => a.Id != id
                              && a.Ten.Equals(ten));
            }
            else if (nhom == Enums.NhomDichVuLoaiGia.DichVuKyThuat)
            {
                check = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
                    .Any(a => a.Id != id
                              && a.Ten.Equals(ten));
            }
            else if (nhom == Enums.NhomDichVuLoaiGia.DichVuGiuongBenh)
            {
                check = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
                    .Any(a => a.Id != id
                              && a.Ten.Equals(ten));
            }

            return check;
        }
        #endregion
    }
}
