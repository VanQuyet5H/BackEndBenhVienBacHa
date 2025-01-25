using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.LoaiGoiDichVus;
using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuaTang;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using GoiDichVuTemplateVo = Camino.Core.Domain.ValueObject.GoiDichVu.GoiDichVuTemplateVo;

namespace Camino.Services.GoiDichVuMarketings
{
    [ScopedDependency(ServiceType = typeof(IChuongTrinhMarketingGoiDvService))]
    public class ChuongTrinhMarketingGoiDvService : MasterFileService<ChuongTrinhGoiDichVu>, IChuongTrinhMarketingGoiDvService
    {
        private readonly IRepository<GoiDichVu> _goiDichVu;
        private readonly IRepository<Core.Domain.Entities.QuaTangs.QuaTang> _quaTang;
        private readonly IRepository<NhapKhoQuaTangChiTiet> _nhapKhoQuaTangChiTiet;
        private readonly IRepository<YeuCauGoiDichVu> _ycGoiDv;
        private readonly IRepository<LoaiGoiDichVu> _loaiGoiDichVuRepository;

        public ChuongTrinhMarketingGoiDvService
            (IRepository<ChuongTrinhGoiDichVu> repository, IRepository<GoiDichVu> goiDichVu, IRepository<Core.Domain.Entities.QuaTangs.QuaTang> quaTang,
            IRepository<LoaiGoiDichVu> loaiGoiDichVuRepository,
            IRepository<NhapKhoQuaTangChiTiet> nhapKhoQuaTangChiTiet, IRepository<YeuCauGoiDichVu> ycGoiDv) : base(repository)
        {
            _goiDichVu = goiDichVu;
            _quaTang = quaTang;
            _nhapKhoQuaTangChiTiet = nhapKhoQuaTangChiTiet;
            _ycGoiDv = ycGoiDv;
            _loaiGoiDichVuRepository = loaiGoiDichVuRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new ChuongTrinhGoiDvMarketingSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<ChuongTrinhGoiDvMarketingSearch>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Select(s => new ChuongTrinhGoiDvMarketingGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                TenDv = s.GoiDichVu.Ten,
                GiaTruocChietKhau = s.GiaTruocChietKhau,
                GiaSauChietKhau = s.GiaSauChietKhau,
                TuNgay = s.TuNgay,
                DenNgay = s.DenNgay,
                TamNgung = s.TamNgung
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.Ma, g => g.TenDv);

            if (queryObject != null)
            {
                if (queryObject.RangeFromDate != null && queryObject.RangeFromDate.startDate != null)
                {
                    var tuNgay = queryObject.RangeFromDate.startDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>) query.Where(p => tuNgay <= p.TuNgay.Date);
                }

                if (queryObject.RangeFromDate != null && queryObject.RangeFromDate.endDate != null)
                {
                    var denNgay = queryObject.RangeFromDate.endDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>) query.Where(p => denNgay >= p.TuNgay.Date);
                }
                if (queryObject.RangeToDate != null && queryObject.RangeToDate.startDate != null)
                {
                    var tuNgay = queryObject.RangeToDate.startDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>)query.Where(p => tuNgay <= p.DenNgay.Value.Date);
                }
                if (queryObject.RangeToDate != null && queryObject.RangeToDate.endDate != null)
                {
                    var denNgay = queryObject.RangeToDate.endDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>)query.Where(p => denNgay >= p.DenNgay.Value.Date);
                }
            }

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

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new ChuongTrinhGoiDvMarketingSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<ChuongTrinhGoiDvMarketingSearch>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Select(s => new ChuongTrinhGoiDvMarketingGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                TenDv = s.GoiDichVu.Ten,
                TuNgay = s.TuNgay,
                DenNgay = s.DenNgay
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.Ma, g => g.TenDv);

            if (queryObject != null)
            {
                if (queryObject.RangeFromDate != null && queryObject.RangeFromDate.startDate != null)
                {
                    var tuNgay = queryObject.RangeFromDate.startDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>)query.Where(p => tuNgay <= p.TuNgay.Date);
                }

                if (queryObject.RangeFromDate != null && queryObject.RangeFromDate.endDate != null)
                {
                    var denNgay = queryObject.RangeFromDate.endDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>)query.Where(p => denNgay >= p.TuNgay.Date);
                }
                if (queryObject.RangeToDate != null && queryObject.RangeToDate.startDate != null)
                {
                    var tuNgay = queryObject.RangeToDate.startDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>)query.Where(p => tuNgay <= p.DenNgay.Value.Date);
                }
                if (queryObject.RangeToDate != null && queryObject.RangeToDate.endDate != null)
                {
                    var denNgay = queryObject.RangeToDate.endDate.GetValueOrDefault().Date;
                    query = (IOrderedQueryable<ChuongTrinhGoiDvMarketingGridVo>)query.Where(p => denNgay >= p.DenNgay.Value.Date);
                }
            }

            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<GridDataSource> GetDataForGridYeuCauGoiDichVuAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            var query = _ycGoiDv.TableNoTracking.Where(e => e.ChuongTrinhGoiDichVuId == Convert.ToInt32(queryInfo.AdditionalSearchString))
                .Select(s => new YeuCauSuDungChuongTrinhMarketingGoiDvGridVo
            {
                Id = s.Id,
                MaBn = s.BenhNhan.MaBN,
                TenBn = s.BenhNhan.HoTen,
                DiaChi = s.BenhNhan.DiaChiDayDu,
                NgayDangKy = s.ThoiDiemChiDinh,
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.MaBn,
                g => g.TenBn, g => g.DiaChi);

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

        public async Task<GridDataSource> GetTotalPageForGridYeuCauGoiDichVuAsync(QueryInfo queryInfo)
        {
            var query = _ycGoiDv.TableNoTracking.Where(e => e.ChuongTrinhGoiDichVuId == Convert.ToInt32(queryInfo.AdditionalSearchString))
                .Select(s => new YeuCauSuDungChuongTrinhMarketingGoiDvGridVo
                {
                Id = s.Id,
                MaBn = s.BenhNhan.MaBN,
                TenBn = s.BenhNhan.HoTen,
                DiaChi = s.BenhNhan.DiaChiDayDu,
                NgayDangKy = s.ThoiDiemChiDinh,
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.MaBn,
                g => g.TenBn, g => g.DiaChi);

            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
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

        public async Task<List<GoiDichVuTemplateVo>> GetGoiDv(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstEntity = await _goiDichVu.TableNoTracking
                    .Where(p => p.IsDisabled != true && p.LoaiGoiDichVu == Enums.EnumLoaiGoiDichVu.Marketing)
                    .Take(model.Take)
                    .Select(p => new GoiDichVuTemplateVo
                    {
                        DisplayName = p.Ten,
                        KeyId = p.Id,
                        MoTa = p.MoTa
                    })
                    .Where(s => s.DisplayName.Contains(model.Query ?? ""))
                    .ToListAsync();
                return lstEntity;
            }
            else
            {
                var lstColumnNameSearch = new List<string> {"Ten"};

                var lstEntity = await _goiDichVu
                    .ApplyFulltext(model.Query, nameof(GoiDichVu), lstColumnNameSearch)
                    .Where(p => p.IsDisabled != true && p.LoaiGoiDichVu == Enums.EnumLoaiGoiDichVu.Marketing)
                    .Take(model.Take)
                    .Select(p => new GoiDichVuTemplateVo
                    {
                        DisplayName = p.Ten,
                        KeyId = p.Id,
                        MoTa = p.MoTa
                    })
                    .ToListAsync();
                return lstEntity;
            }
        }

        public async Task<List<QuaTangTemplateVo>> GetListQuaTang(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstEntity = await _quaTang.TableNoTracking
                    .Where(p => p.HieuLuc)
                    .Take(model.Take)
                    .Select(p => new QuaTangTemplateVo
                    {
                        KeyId = p.Id,
                        Ten = p.Ten,
                        Dvt = p.DonViTinh
                    })
                    .Where(s => s.Ten.Contains(model.Query ?? "") || s.Dvt.Contains(model.Query ?? ""))
                    .ToListAsync();
                return lstEntity;
            }
            else
            {
                var lstColumnNameSearch = new List<string> { "Ten", "DonViTinh" };

                var lstEntity = await _quaTang
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.QuaTangs.QuaTang), lstColumnNameSearch)
                    .Where(p => p.HieuLuc)
                    .Take(model.Take)
                    .Select(p => new QuaTangTemplateVo
                    {
                        KeyId = p.Id,
                        Ten = p.Ten,
                        Dvt = p.DonViTinh
                    })
                    .ToListAsync();
                return lstEntity;
            }
        }

        public async Task<bool> IsTenExists(string ten = null, long id = 0)
        {
            bool query;

            if (id == 0)
            {
                query = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));
            }
            else
            {
                query = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }

            return query;
        }

        public async Task<bool> IsMaExists(string ma = null, long id = 0)
        {
            bool query;

            if (id == 0)
            {
                query = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));
            }
            else
            {
                query = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != id);
            }

            return query;
        }

        public async Task<List<LookupItemVo>> GetLoaiGoiDichVus(DropDownListRequestModel queryInfo)
        {
            var result = _loaiGoiDichVuRepository.TableNoTracking.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<GridDataSource> GetDataForGridLoaiGoiMarketingAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = _loaiGoiDichVuRepository.TableNoTracking
                .Select(s => new LoaiGoiDichVuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    IsDefault = s.IsDefault,
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridLoaiGoiMarketingAsync(QueryInfo queryInfo)
        {
            var query = _loaiGoiDichVuRepository.TableNoTracking
                .Select(s => new LoaiGoiDichVuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    IsDefault = s.IsDefault,
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<bool> IsMaTenExists(string maHoacTen = null, long Id = 0, bool flag = false)
        {
            var result = false;
            if (Id == 0)
            {
                result = !flag ? await _loaiGoiDichVuRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maHoacTen))
                    : await _loaiGoiDichVuRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maHoacTen));
            }
            else
            {
                result = !flag ? await _loaiGoiDichVuRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maHoacTen) && p.Id != Id) :
                     await _loaiGoiDichVuRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maHoacTen) && p.Id != Id);
            }
            if (result)
                return false;
            return true;
        }
    }
}
