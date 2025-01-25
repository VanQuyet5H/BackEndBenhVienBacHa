using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhapKhoMarketting;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Camino.Services.NhapKhoQuaTangMarketing
{
    public partial class NhapKhoQuaTangMarketingService
    {
        #region DS nhập kho marketing
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Select(p => new NhapKhoQuaTangMarketingGridVo
                {
                    Id = p.Id,
                    SoPhieu = p.SoPhieu,
                    NguoiNhapId = p.NguoiNhapId,
                    NhanVienNhap = p.NguoiNhap.User.HoTen,
                    NgayNhap = p.NgayNhap,
                    NgayNhapDisplay = p.NgayNhap.ApplyFormatDateTimeSACH(),
                    SoChungTu = p.SoChungTu,
                    LoaiNguoiGiao = p.LoaiNguoiGiao,
                    NguoiGiaoId = p.NguoiGiaoId,
                    TenNguoiGiao = p.TenNguoiGiao
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhapKhoQuaTangMarketingGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.NgayNhap >= tuNgay && p.NgayNhap <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.SoPhieu,
                        g => g.NhanVienNhap,
                        g => g.TenNguoiGiao,
                        g => g.SoChungTu
                        );
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.SoPhieu,
                        g => g.NhanVienNhap,
                        g => g.TenNguoiGiao,
                        g => g.SoChungTu
                        );

            }
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
                        .Select(p => new NhapKhoQuaTangMarketingGridVo
                        {
                            Id = p.Id,
                            SoPhieu = p.SoPhieu,
                            NguoiNhapId = p.NguoiNhapId,
                            NhanVienNhap = p.NguoiNhap.User.HoTen,
                            NgayNhap = p.NgayNhap,
                            NgayNhapDisplay = p.NgayNhap.ApplyFormatDateTimeSACH(),
                            SoChungTu = p.SoChungTu,
                            LoaiNguoiGiao = p.LoaiNguoiGiao,
                            NguoiGiaoId = p.NguoiGiaoId,
                            TenNguoiGiao = p.TenNguoiGiao
                        });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhapKhoQuaTangMarketingGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.NgayNhap >= tuNgay && p.NgayNhap <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.SoPhieu,
                        g => g.NhanVienNhap,
                        g => g.TenNguoiGiao,
                        g => g.SoChungTu
                        );
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.SoPhieu,
                        g => g.NhanVienNhap,
                        g => g.TenNguoiGiao,
                        g => g.SoChungTu,
                        g => g.SoChungTu
                        );

            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion
        #region CRUD
        public ThongTinNhapKhoQuaTangGridVo GetThongTinNhapKhoQuaTang(long nhapKhoQuaTangId)
        {
            var queryNhapKhoQuaTang = BaseRepository.TableNoTracking.Where(x => x.Id == nhapKhoQuaTangId)
                                                      .Select(item => new ThongTinNhapKhoQuaTangGridVo()
                                                      {
                                                          SoChungTu = item.SoChungTu,
                                                          LoaiNguoiGiao = item.LoaiNguoiGiao,
                                                          TenNguoiGiao = item.TenNguoiGiao,
                                                          NguoiGiaoId = item.NguoiGiaoId,
                                                          TenNguoiNhap = item.NguoiNhap.User.HoTen,
                                                          NguoiNhapId = item.NguoiNhapId,
                                                          NgayNhap = item.NgayNhap,
                                                          DanhSachQuaTangDuocThemList = _nhapKhoQuaTangChiTietRepository.TableNoTracking.Where(x => x.NhapKhoQuaTangId == item.Id)
                                                                                           .Select(m => new DanhSachQuaTangDuocThemGridVo()
                                                                                           {
                                                                                               Id = m.Id,
                                                                                               NhapKhoQuaTangId = m.NhapKhoQuaTangId,
                                                                                               SoLuongDaXuat = m.SoLuongDaXuat,
                                                                                               QuaTangId = m.QuaTangId,
                                                                                               DonViTinh = m.QuaTang.DonViTinh,
                                                                                               DonGiaNhap = m.DonGiaNhap,
                                                                                               QuaTang = m.QuaTang.Ten,
                                                                                               NhaCungCap = m.NhaCungCap,
                                                                                               SoLuongNhap = m.SoLuongNhap,
                                                                                               ThanhTien = m.SoLuongNhap * m.DonGiaNhap
                                                                                           }).ToList()
                                                      });

            return queryNhapKhoQuaTang.FirstOrDefault();
        }
        public async Task<List<LookupItemVo>> GetListNhanVienAsync(DropDownListRequestModel model)
        {
            var lstNhanVien = await
                _userRepository.TableNoTracking.Where(x => x.Id == model.Id)
                    .Select(item => new LookupItemVo()
                    {
                        DisplayName = item.HoTen,
                        KeyId = item.Id
                    }).Union(
                _userRepository.TableNoTracking
                .Where(x => x.IsActive && x.Id != model.Id)
                .Select(item => new LookupItemVo()
                {
                    DisplayName = item.HoTen,
                    KeyId = item.Id
                }))
                .ApplyLike(model.Query, x => x.DisplayName)
                .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName).Distinct()
                .Take(model.Take)
                .ToListAsync();
            return lstNhanVien;
        }
        public async Task<List<LookupItemVo>> GetListDanhSachQuaTangAsync(DropDownListRequestModel model)
        {
            var lstQuaTang = await
                _quaTangRepository.TableNoTracking.Where(x =>x.HieuLuc == true)
                    .Select(item => new LookupItemVo()
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id
                    })
                .ApplyLike(model.Query, x => x.DisplayName)
                .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                .Take(model.Take)
                .ToListAsync();
            return lstQuaTang;
        }
        public string GetDonViTinhQuaTang(long IdQuaTang)
        {
            var lstQuaTang = _quaTangRepository.TableNoTracking.Where(x => x.Id == IdQuaTang).Select(x => x.DonViTinh).FirstOrDefault();
            return lstQuaTang;
        }
        public string GetTenQuaTang(long IdQuaTang)
        {
            var lstQuaTang = _quaTangRepository.TableNoTracking.Where(x => x.Id == IdQuaTang).Select(x => x.Ten).FirstOrDefault();
            return lstQuaTang;
        }
        public LookupItemVo GetThongTinNhanVienLoginAsync(long nhanVienId)
        {
            var lstNhanVien =
               _userRepository.TableNoTracking.Where(x => x.Id == nhanVienId)
                   .Select(item => new LookupItemVo()
                   {
                       DisplayName = item.HoTen,
                       KeyId = item.Id
                   });
            return lstNhanVien.FirstOrDefault();
        }
        public async Task<bool> IsTenExists(string ten, long quaTangId)
        {
            var result = false;
            var resultData = await BaseRepository.TableNoTracking.SelectMany(x => x.NhapKhoQuaTangChiTiets).Where(p => p.NhaCungCap == ten.ToLower() && p.QuaTangId == quaTangId).ToListAsync();
            return resultData.Count() == 0 ? false :true;
        }
        #endregion
    }
}
