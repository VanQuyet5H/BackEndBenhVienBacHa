using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.HopDongThauVatTu;
using Camino.Core.Domain.ValueObject.VatTu;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.HopDongThauVatTuService
{
    [ScopedDependency(ServiceType = typeof(IHopDongThauVatTuService))]
    public class HopDongThauVatTuService
        : MasterFileService<HopDongThauVatTu>
            , IHopDongThauVatTuService
    {
        private readonly IRepository<HopDongThauVatTuChiTiet> _hopDongThauVatTuChiTiet;
        private readonly IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTu;
        private readonly IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> _vatTuBV;

        public HopDongThauVatTuService
        (
            IRepository<HopDongThauVatTu> repository,
            IRepository<HopDongThauVatTuChiTiet> hopDongThauVtCtRepository,
            IRepository<Core.Domain.Entities.VatTus.VatTu> vatTu,
            IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> vatTuBV
        )
            : base(repository)
        {
            _hopDongThauVatTuChiTiet = hopDongThauVtCtRepository;
            _vatTu = vatTu;
            _vatTuBV = vatTuBV;
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

            var query = BaseRepository.TableNoTracking
                .Where(w => w.HeThongTuPhatSinh != true)
                .Select(source => new HdtVatTuGridVo
                {
                    Id = source.Id,
                    NhaThau = source.NhaThau.Ten,
                    SoHopDong = source.SoHopDong,
                    SoQuyetDinh = source.SoQuyetDinh,
                    CongBo = source.CongBo,
                    NgayKy = source.NgayKy,
                    NgayHl = source.NgayHieuLuc,
                    NgayHh = source.NgayHetHan,
                    LoaiThau = source.LoaiThau,
                    NhomThau = source.NhomThau,
                    GoiThau = source.GoiThau,
                    Nam = source.Nam
                }).ApplyLike(queryInfo.SearchTerms,
                    g => g.NhaThau,
                    g => g.SoHopDong,
                    g => g.SoQuyetDinh,
                    g => g.NhomThau,
                    g => g.GoiThau);

            var count = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var nhaThauArray = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(count, nhaThauArray);

            return new GridDataSource
            {
                Data = nhaThauArray.Result,
                TotalRowCount = count.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(w => w.HeThongTuPhatSinh != true)
                .Select(source => new HdtVatTuGridVo
                {
                    Id = source.Id,
                    NhaThau = source.NhaThau.Ten,
                    SoHopDong = source.SoHopDong,
                    SoQuyetDinh = source.SoQuyetDinh,
                    CongBo = source.CongBo,
                    NgayKy = source.NgayKy,
                    NgayHl = source.NgayHieuLuc,
                    NgayHh = source.NgayHetHan,
                    LoaiThau = source.LoaiThau,
                    NhomThau = source.NhomThau,
                    GoiThau = source.GoiThau,
                    Nam = source.Nam
                }).ApplyLike(queryInfo.SearchTerms,
                    g => g.NhaThau,
                    g => g.SoHopDong,
                    g => g.SoQuyetDinh,
                    g => g.NhomThau,
                    g => g.GoiThau);

            var count = await query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = count
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

            var query = _hopDongThauVatTuChiTiet.TableNoTracking
                .Where(x => x.HopDongThauVatTuId == par)
                .Select(source => new HdtVatTuChiTietGridVo
                {
                    Id = source.Id,
                    VatTu = source.VatTu.Ten,
                    Gia = source.Gia,
                    SoLuong = source.SoLuong,
                    SoLuongDaCap = source.SoLuongDaCap
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _hopDongThauVatTuChiTiet.TableNoTracking
                .Where(e => e.HopDongThauVatTuId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(source => new HdtVatTuChiTietGridVo
                {
                    Id = source.Id
                });

            var countTask = await query.CountAsync();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<List<VatTuTemplateVo>> GetListVatTu(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstDuocPhamEntity = await _vatTu.TableNoTracking
                    .Where(p => p.IsDisabled != true)
                    .Select(item => new VatTuTemplateVo
                    {
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                        SuDungTaiBenhVien = item.VatTuBenhVien != null,
                        MaTaiBenhVien = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                        LoaiSuDungId = (item.VatTuBenhVien != null ? item.VatTuBenhVien.LoaiSuDung : Enums.LoaiSuDung.VatTuTieuHao)
                    })
                    .ApplyLike(model.Query, x => x.DisplayName, x => x.Ten, x => x.Ma)
                    .Take(model.Take)
                    .ToListAsync();

                return lstDuocPhamEntity;
            }
            else
            {
                var lstColumnNameSearch = new List<string> { "Ma", "Ten" };

                var lstDuocPhamEntity = await _vatTu
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                    .Where(p => p.IsDisabled != true)
                    .Take(model.Take)
                    .Select(item => new VatTuTemplateVo
                    {
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                        SuDungTaiBenhVien = item.VatTuBenhVien != null,
                        MaTaiBenhVien = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                        LoaiSuDungId = (item.VatTuBenhVien != null ? item.VatTuBenhVien.LoaiSuDung : Enums.LoaiSuDung.VatTuTieuHao)
                    })
                    .ToListAsync();

                return lstDuocPhamEntity;
            }
        }

        public bool KiemTraConVatTu(long? idVatTu)
        {
            return _vatTu.TableNoTracking
                .Any(e => e.Id == idVatTu);
        }

        public async Task<bool> GetHieuLucVatTu(long id)
        {
            var hieuLuc = await _vatTu.TableNoTracking.Where(p => p.Id == id).Select(p => p.IsDisabled)
                .FirstOrDefaultAsync();
            return hieuLuc == false;
        }

        public async Task<bool> CheckExist(long id, long nhaThauId, string soHopDong, string soQuyetDinh)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoHopDong.Equals(soHopDong) && p.SoQuyetDinh.Equals(soQuyetDinh) && p.NhaThauId == nhaThauId);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoHopDong.Equals(soHopDong) && p.SoQuyetDinh.Equals(soQuyetDinh) && p.NhaThauId == nhaThauId && p.Id != id);
            }

            return !result;
        }
        public async Task<bool> CheckVatTuBenhVienExist(long vatTuId)
        {
            var vatTuBenhVien = _vatTuBV.TableNoTracking.Where(s => s.Id == vatTuId).Select(c => c.Id).ToList();
            if (vatTuBenhVien.Count() == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<List<VatTuTemplateVo>> GetVatTus(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
                {
                    nameof(Core.Domain.Entities.VatTus.VatTu.Ten),
                    nameof(Core.Domain.Entities.VatTus.VatTu.Ma)
                };
            var vatTuId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                var vatTus = _vatTu.TableNoTracking
                .OrderByDescending(x => vatTuId == 0 || x.Id == vatTuId).ThenBy(x => x.Id)
                .Select(item => new VatTuTemplateVo
                {
                    KeyId = item.Id,
                    Ten = item.Ten,
                    Ma = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                    SuDungTaiBenhVien = item.VatTuBenhVien != null,
                    //MaTaiBenhVien = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                    MaTaiBenhVien = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : null),
                    LoaiSuDungId = (item.VatTuBenhVien != null ? item.VatTuBenhVien.LoaiSuDung : Enums.LoaiSuDung.VatTuTieuHao),
                    DichVuBenhVienId = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Id : (long?)null)
                })
                .ApplyLike(queryInfo.Query, x => x.DisplayName, x => x.Ten, x => x.Ma)
                .Take(queryInfo.Take);
                return await vatTus.ToListAsync();
            }
            else
            {
                var vatTuIds = _vatTu
                           .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                           .Select(p => p.Id).ToList();

                var dictionary = vatTuIds.Select((id, index) => new
                {
                    key = id,
                    rank = index,
                }).ToDictionary(o => o.key, o => o.rank);

                var vatTus = _vatTu
                                    .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                                    //.OrderByDescending(x => vatTuId == 0 || x.Id == vatTuId)//.ThenBy(x => x.Id)
                                    .Select(item => new VatTuTemplateVo
                                    {
                                        KeyId = item.Id,
                                        Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                        Ten = item.Ten,
                                        Ma = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                                        SuDungTaiBenhVien = item.VatTuBenhVien != null,
                                        //MaTaiBenhVien = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : item.Ma),
                                        MaTaiBenhVien = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Ma : null),
                                        LoaiSuDungId = (item.VatTuBenhVien != null ? item.VatTuBenhVien.LoaiSuDung : Enums.LoaiSuDung.VatTuTieuHao),
                                        DichVuBenhVienId = (item.VatTuBenhVien != null ? item.VatTuBenhVien.Id : (long?)null)
                                    })
                                    .OrderBy(p => dictionary.Any(a => a.Key == p.KeyId) ? dictionary[p.KeyId] : dictionary.Count)
                                    .Take(queryInfo.Take)
                                    ;
                return await vatTus.ToListAsync();
            }
        }
    }
}
