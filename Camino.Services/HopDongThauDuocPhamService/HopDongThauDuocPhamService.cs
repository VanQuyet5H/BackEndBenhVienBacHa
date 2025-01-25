using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.HopDongThauDuocPham;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.HopDongThauDuocPhamService
{
    [ScopedDependency(ServiceType = typeof(IHopDongThauDuocPhamService))]
    public class HopDongThauDuocPhamService
        : MasterFileService<HopDongThauDuocPham>
        , IHopDongThauDuocPhamService
    {
        private readonly IRepository<Core.Domain.Entities.NhaThaus.NhaThau> _nhaThau;
        private readonly IRepository<HopDongThauDuocPhamChiTiet> _hopDongThauDuocPhamChiTiet;
        private readonly IRepository<DuocPham> _duocPham;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBV;

        public HopDongThauDuocPhamService
            (
              IRepository<HopDongThauDuocPham> repository
            , IRepository<Core.Domain.Entities.NhaThaus.NhaThau> nhaThau
            , IRepository<HopDongThauDuocPhamChiTiet> hopDongThauDuocPhamChiTiet
            , IRepository<DuocPham> duocPham
            , IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBV
            )
               : base(repository)
        {
            _nhaThau = nhaThau;
            _hopDongThauDuocPhamChiTiet = hopDongThauDuocPhamChiTiet;
            _duocPham = duocPham;
            _duocPhamBV = duocPhamBV;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var query = BaseRepository.TableNoTracking.Include(p => p.NhaThau)
                    .Where(w => w.HeThongTuPhatSinh != true)
                    .Select(source => new HopDongThauDuocPhamGridVo
                    {
                        Id = source.Id,
                        NhaThau = source.NhaThau.Ten,
                        SoHopDong = source.SoHopDong,
                        SoQuyetDinh = source.SoQuyetDinh,
                        CongBoDisplay = source.CongBo.ApplyFormatDate(),
                        NgayKyDisplay = source.NgayKy != null ? source.NgayKy.Value.ApplyFormatDate() : null,
                        NgayHieuLucDisplay = source.NgayHieuLuc.ApplyFormatDate(),
                        NgayHetHanDisplay = source.NgayHetHan.ApplyFormatDate(),
                        TenLoaiThau = source.LoaiThau.GetDescription(),
                        TenLoaiThuocThau = source.LoaiThuocThau.GetDescription(),
                        NhomThau = source.NhomThau,
                        GoiThau = source.GoiThau,
                        Nam = source.Nam
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.NhaThau,
                        g => g.SoHopDong,
                        g => g.SoQuyetDinh,
                        g => g.NhomThau,
                        g => g.GoiThau);

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
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking.Include(p => p.NhaThau)
                    .Where(p => (p.NhaThau.Ten.Contains(searchString) || p.SoHopDong.Contains(searchString)
                    || p.SoQuyetDinh.Contains(searchString) || p.NhomThau.Contains(searchString)
                    || p.GoiThau.Contains(searchString)) && p.HeThongTuPhatSinh != true)
                    .Select(source => new HopDongThauDuocPhamGridVo
                    {
                        Id = source.Id,
                        NhaThau = source.NhaThau.Ten,
                        SoHopDong = source.SoHopDong,
                        SoQuyetDinh = source.SoQuyetDinh,
                        CongBoDisplay = source.CongBo.ApplyFormatDate(),
                        NgayKyDisplay = source.NgayKy != null ? source.NgayKy.Value.ApplyFormatDate() : null,
                        NgayHieuLucDisplay = source.NgayHieuLuc.ApplyFormatDate(),
                        NgayHetHanDisplay = source.NgayHetHan.ApplyFormatDate(),
                        TenLoaiThau = source.LoaiThau.GetDescription(),
                        TenLoaiThuocThau = source.LoaiThuocThau.GetDescription(),
                        NhomThau = source.NhomThau,
                        GoiThau = source.GoiThau,
                        Nam = source.Nam
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.NhaThau,
                        g => g.SoHopDong,
                        g => g.SoQuyetDinh,
                        g => g.NhomThau,
                        g => g.GoiThau);

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
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var result = BaseRepository.TableNoTracking.Include(p => p.NhaThau)
                    .Where(w => w.HeThongTuPhatSinh != true)
                    .Select(source => new HopDongThauDuocPhamGridVo
                    {
                        Id = source.Id,
                        NhaThau = source.NhaThau.Ten,
                        SoHopDong = source.SoHopDong,
                        SoQuyetDinh = source.SoQuyetDinh,
                        CongBoDisplay = source.CongBo.ApplyFormatDate(),
                        NgayKyDisplay = source.NgayKy != null ? source.NgayKy.Value.ApplyFormatDate() : null,
                        NgayHieuLucDisplay = source.NgayHieuLuc.ApplyFormatDate(),
                        NgayHetHanDisplay = source.NgayHetHan.ApplyFormatDate(),
                        TenLoaiThau = source.LoaiThau.GetDescription(),
                        TenLoaiThuocThau = source.LoaiThuocThau.GetDescription(),
                        NhomThau = source.NhomThau,
                        GoiThau = source.GoiThau,
                        Nam = source.Nam
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.NhaThau,
                        g => g.SoHopDong,
                        g => g.SoQuyetDinh,
                        g => g.NhomThau,
                        g => g.GoiThau);

                var countTask = result.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking.Include(p => p.NhaThau)
                    .Where(p => (p.NhaThau.Ten.Contains(searchString) || p.SoHopDong.Contains(searchString)
                           || p.SoQuyetDinh.Contains(searchString) || p.NhomThau.Contains(searchString)
                           || p.GoiThau.Contains(searchString)) && p.HeThongTuPhatSinh != true)
                    .Select(source => new HopDongThauDuocPhamGridVo
                    {
                        Id = source.Id,
                        NhaThau = source.NhaThau.Ten,
                        SoHopDong = source.SoHopDong,
                        SoQuyetDinh = source.SoQuyetDinh,
                        CongBoDisplay = source.CongBo.ApplyFormatDate(),
                        NgayKyDisplay = source.NgayKy != null ? source.NgayKy.Value.ApplyFormatDate() : null,
                        NgayHieuLucDisplay = source.NgayHieuLuc.ApplyFormatDate(),
                        NgayHetHanDisplay = source.NgayHetHan.ApplyFormatDate(),
                        TenLoaiThau = source.LoaiThau.GetDescription(),
                        TenLoaiThuocThau = source.LoaiThuocThau.GetDescription(),
                        NhomThau = source.NhomThau,
                        GoiThau = source.GoiThau,
                        Nam = source.Nam
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.NhaThau,
                        g => g.SoHopDong,
                        g => g.SoQuyetDinh,
                        g => g.NhomThau,
                        g => g.GoiThau);

                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource
                {
                    TotalRowCount = countTask.Result
                };
            }
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? hopDongThauId, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par;
            if (hopDongThauId != null && hopDongThauId != 0)
            {
                par = (long)hopDongThauId;
            }
            else
            {
                par = long.Parse(queryInfo.AdditionalSearchString);
            }

            var query = _hopDongThauDuocPhamChiTiet.TableNoTracking
                .Include(o => o.DuocPham)
                .Where(x => x.HopDongThauDuocPhamId == par)
                .Select(source => new HopDongThauDuocPhamChiTietGridVo
                {
                    Id = source.Id,
                    DuocPham = source.DuocPham.Ten,
                    Gia = source.Gia,
                    GiaDisplay = source.Gia.ApplyFormatMoneyVND(),
                    SoLuongDisplay = source.SoLuong.ApplyVietnameseFloatNumber(),
                    SoLuongCungCapDisplay = source.SoLuongDaCap.ApplyVietnameseFloatNumber()
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _hopDongThauDuocPhamChiTiet.TableNoTracking
                .Include(o => o.DuocPham)
                .Where(x => x.HopDongThauDuocPhamId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(source => new HopDongThauDuocPhamChiTietGridVo
                {
                    Id = source.Id,
                    DuocPham = source.DuocPham.Ten,
                    GiaDisplay = source.Gia.ApplyVietnameseFloatNumber(),
                    SoLuongDisplay = source.SoLuong.ApplyVietnameseFloatNumber(),
                    SoLuongCungCapDisplay = source.SoLuongDaCap.ApplyVietnameseFloatNumber()
                }).ApplyLike(queryInfo.SearchTerms,
                        g => g.DuocPham);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
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

        public async Task<List<NhaThauTemplateVo>> GetListNhaThau(DropDownListRequestModel model)
        {
            List<NhaThauTemplateVo> result = null;
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstNhaThauQuery = _nhaThau.TableNoTracking
                    .Select(item => new NhaThauTemplateVo
                    {
                        KeyId = item.Id,
                        DiaChi = item.DiaChi,
                        Ten = item.Ten
                    })
                    .ApplyLike(model.Query, w => w.Ten, w => w.DiaChi);

                var lstNhaThauEnumerable = lstNhaThauQuery
                    .Take(model.Take).ToListAsync();

                result = await lstNhaThauEnumerable;
            }
            else
            {
                var lstColumnNameSearch = new List<string> { "Ten", "DiaChi" };
                var lstId = _nhaThau
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.NhaThaus.NhaThau), lstColumnNameSearch)
                    .Select(item => item.Id)
                    .ToList();

                var dct = lstId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var lst = _nhaThau.TableNoTracking
                    .Where(p => lstId.Contains(p.Id));
                var query = await lst.Select(item => new NhaThauTemplateVo
                {
                    KeyId = item.Id,
                    DiaChi = item.DiaChi,
                    Ten = item.Ten
                }).OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                    .Take(model.Take)
                    .ToListAsync();

                result = query;
            }
            if (model.Id > 0 && result.All(o => o.KeyId != model.Id))
            {
                var item1 = _nhaThau.TableNoTracking
                    .Where(p => p.Id == model.Id)
                    .Select(item => new NhaThauTemplateVo
                    {
                        KeyId = item.Id,
                        DiaChi = item.DiaChi,
                        Ten = item.Ten
                    });
                if (item1.Any())
                {
                    result.AddRange(item1);
                }
            }
            return result;
        }


        public List<LookupItemVo> GetListLoaiThau(LookupQueryInfo queryInfo)
        {
            var listLoaiThau = EnumHelper.GetListEnum<Enums.EnumLoaiThau>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).OrderByDescending(z => z.KeyId == (int)EnumLoaiThau.ThauRieng).ThenBy(z => z.DisplayName).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listLoaiThau = listLoaiThau.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }

            return listLoaiThau;
        }

        public List<LookupItemVo> GetListLoaiThuocThau(LookupQueryInfo queryInfo)
        {
            var listLoaiThuocThau = EnumHelper.GetListEnum<Enums.EnumLoaiThuocThau>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).OrderByDescending(z => z.KeyId == (int)EnumLoaiThuocThau.TanDuoc).ThenBy(z => z.DisplayName).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listLoaiThuocThau = listLoaiThuocThau.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }

            return listLoaiThuocThau;
        }

        public Task<List<LookupItemVo>> GetListTenHopDongThau(DropDownListRequestModel model)
        {
            var list = BaseRepository.TableNoTracking.ApplyLike(model.Query, g => g.SoHopDong)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.SoHopDong,
                    KeyId = i.Id
                }).ToList();
            return Task.FromResult(list);
        }

        public async Task<bool> IsNhaThauExists(long nhaThauId = 0, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.NhaThauId.Equals(nhaThauId));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.NhaThauId.Equals(nhaThauId) && p.Id != id);
            }

            return result;
        }

        public async Task<bool> IsSoHopDongExists(string soHopDong = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoHopDong.Equals(soHopDong));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoHopDong.Equals(soHopDong) && p.Id != id);
            }

            return result;
        }

        public async Task<bool> IsSoQuyetDinhExists(string soQuyetDinh = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoQuyetDinh.Equals(soQuyetDinh));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoQuyetDinh.Equals(soQuyetDinh) && p.Id != id);
            }

            return result;
        }

        public async Task<bool> GetHieuLucDuocPham(long id)
        {
            var hieuLuc = await _duocPham.TableNoTracking.Where(p => p.Id == id).Select(p => p.IsDisabled)
                .FirstOrDefaultAsync();

            if (hieuLuc != true)
            {
                return true;
            }

            return false;
        }

        public async Task<List<long>> GetHopDongThauChiTiet(long hopDongThauDuocPhamId, long duocPhamId)
        {
            var lstHopDongThauChiTietId = await _hopDongThauDuocPhamChiTiet.TableNoTracking
                .Where(p => p.HopDongThauDuocPhamId == hopDongThauDuocPhamId && p.DuocPhamId == duocPhamId)
                .Select(p => p.Id).ToListAsync();
            return lstHopDongThauChiTietId;
        }

        public async Task<bool> KiemTraHieuLucHopDongThau(long hopDongThauId)
        {
            if (hopDongThauId == 0)
                return true;
            return await BaseRepository.TableNoTracking
                .AnyAsync(x => (hopDongThauId == 0 || x.Id == hopDongThauId) &&
                                x.NgayHieuLuc.Date <= DateTime.Now.Date && DateTime.Now.Date <= x.NgayHetHan.Date);
        }

        public async Task<bool> KiemTraHetHieuLucHopDongThau(long[] arrHopDongThauId)
        {
            if (arrHopDongThauId == null || !arrHopDongThauId.Any())
                return false;
            return await BaseRepository.TableNoTracking
                .AnyAsync(x => (arrHopDongThauId == null || !arrHopDongThauId.Any() || arrHopDongThauId.Contains(x.Id)) &&
                                (x.NgayHieuLuc.Date > DateTime.Now.Date || DateTime.Now.Date > x.NgayHetHan.Date));
        }

        public async Task<bool> KiemTraConDuocPham(long id)
        {
            var duocPham = await _duocPham.TableNoTracking
                .AnyAsync(p => p.Id == id);

            return duocPham;
        }
        public async Task<bool> CheckDuocPhamBenhVienExist(long duocPhamId)
        {
            var duocPhamBenhVien = _duocPhamBV.TableNoTracking.Where(s => s.Id == duocPhamId).Select(c => c.Id).ToList();
            if (duocPhamBenhVien.Count() == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> IsMaDuocPhamBVExists(string maDuocPham, long id = 0)
        {
            bool result = true;
            if (id == 0)
            {
                return
                     result = await _duocPhamBV.TableNoTracking.AnyAsync(p => p.MaDuocPhamBenhVien.Equals(maDuocPham));
            }

            return false;
        }

        public async Task<List<DuocPhamTemplateVo>> GetListDuocPham(DropDownListRequestModel model)
        {
            List<DuocPhamTemplateVo> result = null;
            var duocPhamId = CommonHelper.GetIdFromRequestDropDownList(model);
            var lstColumnNameSearch = new List<string>
                {
                    nameof(DuocPham.Ten),
                    //nameof(DuocPham.HoatChat),
                    //nameof(DuocPham.NhaSanXuat)

                };
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstDuocPhamEntity = _duocPham.TableNoTracking
                    .Where(p => p.IsDisabled != true && p.Id != duocPhamId)
                    .Select(item => new DuocPhamTemplateVo
                    {
                        KeyId = item.Id,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        NhaSanXuat = item.NhaSanXuat,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    }).Union(_duocPham.TableNoTracking
                            .Where(p => p.IsDisabled != true && p.Id == duocPhamId)
                            .Select(item => new DuocPhamTemplateVo
                            {
                                KeyId = item.Id,
                                DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                Ten = item.Ten,
                                HoatChat = item.HoatChat,
                                NhaSanXuat = item.NhaSanXuat,
                                SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                            }))
                    .ApplyLike(model.Query, x => x.DisplayName, x => x.Ten, x => x.HoatChat, x => x.NhaSanXuat)
                    .OrderBy(z => z.KeyId == duocPhamId).ThenBy(z => z.Ten)
                    .Take(model.Take);
                result = await lstDuocPhamEntity.ToListAsync();
            }
            else
            {
                //var lstColumnNameSearch = new List<string> { "Ten", "HoatChat", "NhaSanXuat" };
                var duocPhamIds = _duocPham
                           .ApplyFulltext(model.Query, nameof(DuocPham), lstColumnNameSearch)
                           .Select(p => p.Id).ToList();

                var dictionary = duocPhamIds.Select((id, index) => new
                {
                    key = id,
                    rank = index,
                }).ToDictionary(o => o.key, o => o.rank);

                var lstDuocPhamEntity = await _duocPham
                    .ApplyFulltext(model.Query, nameof(DuocPham), lstColumnNameSearch)
                    .Where(p => p.IsDisabled != true)
                    //.OrderBy(z => z.Id == duocPhamId).ThenBy(z => z.Ten)
                    .Select(item => new DuocPhamTemplateVo
                    {
                        KeyId = item.Id,
                        Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        NhaSanXuat = item.NhaSanXuat,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    })
                    .OrderByDescending(x => duocPhamId == 0 || x.KeyId == duocPhamId).ThenBy(o => o.Rank)
                    .Take(model.Take)
                    .ToListAsync();

                result = lstDuocPhamEntity;
            }
            if (model.Id > 0 && result.All(o => o.KeyId != model.Id))
            {
                var duocPhamIds = _duocPham
                           .ApplyFulltext(model.Query, nameof(DuocPham), lstColumnNameSearch)
                           .Select(p => p.Id).ToList();

                var dictionary = duocPhamIds.Select((id, index) => new
                {
                    key = id,
                    rank = index,
                }).ToDictionary(o => o.key, o => o.rank);
                var item1 = _duocPham.TableNoTracking
                    .Where(p => p.Id == model.Id)
                    .Select(item => new DuocPhamTemplateVo
                    {
                        KeyId = item.Id,
                        Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        NhaSanXuat = item.NhaSanXuat,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    });
                if (item1.Any())
                {
                    result.AddRange(item1);
                }
            }
            return result;
        }
    }
}
