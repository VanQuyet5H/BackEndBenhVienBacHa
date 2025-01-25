using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;

namespace Camino.Services.YeuCauHoanTra.KSNK
{
    public partial class YeuCauHoanTraKSNKService
    {
        #region Hoàn trả kiểm soát nhiễm khuẩn

        public async Task<GridDataSource> GetDanhSachHoanTraKSNKForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {

            BuildDefaultSortExpression(queryInfo);
            var queryObject = new HoanTraKSNKSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<HoanTraKSNKSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = DataYeuCauHoanTraVatTu(null, queryInfo);
            var queryTuChoiDuyet = DataYeuCauHoanTraVatTu(false, queryInfo);
            var queryTuDaDuyet = DataYeuCauHoanTraVatTu(true, queryInfo);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachHoanTraKSNKVo()).AsQueryable();

            if (queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false && queryObject.DaDuyet == false)
            {
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
                queryObject.DaDuyet = true;
            }

            var isHaveQuery = false;
            if (queryObject.DangChoDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoDuyet);
                }
                else
                {
                    query = queryDangChoDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                    isHaveQuery = true;
                }
            }

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, g => g.TenNguoiYeuCau, g => g.HoanTraTuKho, g => g.HoanTraVeKho, g => g.SoPhieu, g => g.NguoiDuyet);
                }

                if (queryObject.RangeNhap != null)
                {
                    if (queryObject.RangeNhap.startDate != null)
                    {
                        var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.NgayYeuCau.Date);
                    }

                    if (queryObject.RangeNhap.endDate != null)
                    {
                        var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault();
                        query = query.Where(p => denNgay.Date >= p.NgayYeuCau.Date);
                    }
                }

                if (queryObject.RangeDuyet != null)
                {
                    if (queryObject.RangeDuyet.startDate != null)
                    {
                        var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && tuNgay.Date <= p.NgayDuyet.Value.Date);
                    }

                    if (queryObject.RangeDuyet.endDate != null)
                    {
                        var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && denNgay.Date >= p.NgayDuyet.Value.Date);
                    }
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayYeuCau desc,SoPhieu asc") && (queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalDanhSachHoanTraKSNKForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new HoanTraKSNKSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<HoanTraKSNKSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = DataYeuCauHoanTraVatTu(null, queryInfo);
            var queryTuChoiDuyet = DataYeuCauHoanTraVatTu(false, queryInfo);
            var queryTuDaDuyet = DataYeuCauHoanTraVatTu(true, queryInfo);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachHoanTraKSNKVo()).AsQueryable();

            if (queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false && queryObject.DaDuyet == false)
            {
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
                queryObject.DaDuyet = true;
            }

            var isHaveQuery = false;
            if (queryObject.DangChoDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoDuyet);
                }
                else
                {
                    query = queryDangChoDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                    isHaveQuery = true;
                }
            }

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, g => g.TenNguoiYeuCau, g => g.HoanTraTuKho, g => g.HoanTraVeKho, g => g.SoPhieu, g => g.NguoiDuyet);
                }

                if (queryObject.RangeNhap != null)
                {
                    if (queryObject.RangeNhap.startDate != null)
                    {
                        var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.NgayYeuCau.Date);
                    }

                    if (queryObject.RangeNhap.endDate != null)
                    {
                        var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault();
                        query = query.Where(p => denNgay.Date >= p.NgayYeuCau.Date);
                    }
                }

                if (queryObject.RangeDuyet != null)
                {
                    if (queryObject.RangeDuyet.startDate != null)
                    {
                        var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && tuNgay.Date <= p.NgayDuyet.Value.Date);
                    }

                    if (queryObject.RangeDuyet.endDate != null)
                    {
                        var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && denNgay.Date >= p.NgayDuyet.Value.Date);
                    }
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDanhSachHoanTraKSNKChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var hoanTraVatTuId = Convert.ToDouble(queryInfo.AdditionalSearchString);
            var query = _ycTraVtChiTiet.TableNoTracking.Where(cc => cc.YeuCauTraVatTuId == hoanTraVatTuId)
                                                               .Include(cc => cc.KhoViTri)
                                                               .Include(cc => cc.VatTuBenhVien).ThenInclude(cc => cc.VatTus).ThenInclude(cc => cc.DonViTinh)
                                                               .Include(cc => cc.HopDongThauVatTu).ThenInclude(cc => cc.HopDongThauVatTuChiTiets)
                                                               .Select(s => new DanhSachHoanTraKSNKChiTietVo
                                                               {
                                                                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",
                                                                   Id = s.Id,
                                                                   Ma = s.VatTuBenhVien.Ma,
                                                                   VatTu = s.VatTuBenhVien.VatTus.Ten,
                                                                   DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                                                                   LoaiBHYT = s.LaVatTuBHYT,
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung,
                                                                   SoLuongHoanTra = s.SoLuongTra,
                                                                   SoLuongHoanTraDisplay = s.SoLuongTra.ApplyNumber()
                                                               });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.VatTu, g => g.Ma, g => g.SoLo);

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = isAllData == true ? query.ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            //await Task.WhenAll(countTask, queryTask);

            var queryTask = query.ToArrayAsync();

            var groupData = queryTask.Result.GroupBy(o => new { o.LoaiSuDung, o.Ma, o.VatTu, o.DonViTinh, o.LoaiBHYT, o.SoLo, o.HanSuDung },
                o => o,
                (k, v) => new DanhSachHoanTraKSNKChiTietVo
                {
                    LoaiSuDung = k.LoaiSuDung,
                    Id = v.First().Id,
                    Ma = k.Ma,
                    VatTu = k.VatTu,
                    DonViTinh = k.DonViTinh,
                    LoaiBHYT = k.LoaiBHYT,
                    SoLo = k.SoLo,
                    HanSuDung = k.HanSuDung,
                    SoLuongHoanTra = v.Sum(o=>o.SoLuongHoanTra),
                    SoLuongHoanTraDisplay = v.Sum(o => o.SoLuongHoanTra).ApplyNumber()
                }).ToList();
            var returnData = isAllData == true ? groupData.ToArray() : groupData.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = returnData, TotalRowCount = groupData.Count };

        }
        public async Task<GridDataSource> GetTotalDanhSachHoanTraKSNKChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var hoanTraVatTuId = Convert.ToDouble(queryInfo.AdditionalSearchString);
            var query = _ycTraVtChiTiet.TableNoTracking.Where(cc => cc.YeuCauTraVatTuId == hoanTraVatTuId)
                                                               .Include(cc => cc.KhoViTri)
                                                               .Include(cc => cc.VatTuBenhVien).ThenInclude(cc => cc.VatTus).ThenInclude(cc => cc.DonViTinh)
                                                               .Include(cc => cc.HopDongThauVatTu).ThenInclude(cc => cc.HopDongThauVatTuChiTiets)
                                                               .Select(s => new DanhSachHoanTraKSNKChiTietVo
                                                               {
                                                                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",

                                                                   Id = s.Id,
                                                                   Ma = s.VatTuBenhVien.Ma,
                                                                   VatTu = s.VatTuBenhVien.VatTus.Ten,
                                                                   DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                                                                   LoaiBHYT = s.LaVatTuBHYT,
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung,
                                                                   SoLuongHoanTra = s.SoLuongTra,
                                                                   SoLuongHoanTraDisplay = s.SoLuongTra.ApplyNumber()
                                                               });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.VatTu, g => g.Ma, g => g.SoLo);

            var queryTask = query.ToArrayAsync();

            var groupData = queryTask.Result.GroupBy(o => new { o.LoaiSuDung, o.Ma, o.VatTu, o.DonViTinh, o.LoaiBHYT, o.SoLo, o.HanSuDung },
                o => o,
                (k, v) => new DanhSachHoanTraKSNKChiTietVo
                {
                    LoaiSuDung = k.LoaiSuDung,
                    Id = v.First().Id,
                    Ma = k.Ma,
                    VatTu = k.VatTu,
                    DonViTinh = k.DonViTinh,
                    LoaiBHYT = k.LoaiBHYT,
                    SoLo = k.SoLo,
                    HanSuDung = k.HanSuDung,
                    SoLuongHoanTra = v.Sum(o => o.SoLuongHoanTra),
                    SoLuongHoanTraDisplay = v.Sum(o => o.SoLuongHoanTra).ApplyNumber()
                }).ToList();
            return new GridDataSource { TotalRowCount = groupData.Count };

            //var countTask = query.CountAsync(); await Task.WhenAll(countTask);
            //return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTraDuocPhamId = Convert.ToDouble(queryInfo.AdditionalSearchString);

            var query = _yeuCauTraDuocPhamChiTiet.TableNoTracking.Where(p => p.YeuCauTraDuocPhamId == yeuCauTraDuocPhamId)
                                                                           .Select(s => new DanhSachHoanTraKSNKChiTietVo
                                                                           {
                                                                               //LoaiSuDung = s.DuocPhamBenhVien.LoaiSuDung != null ? s.DuocPhamBenhVien.LoaiSuDung.GetDescription() : "",
                                                                               LoaiSuDung = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                                                                               Id = s.Id,
                                                                               Ma = s.DuocPhamBenhVien.Ma,
                                                                               VatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                                                                               DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                               LoaiBHYT = s.LaDuocPhamBHYT,
                                                                               SoLo = s.Solo,
                                                                               HanSuDung = s.HanSuDung,
                                                                               SoLuongHoanTra = s.SoLuongTra,
                                                                               SoLuongHoanTraDisplay = s.SoLuongTra.ApplyNumber()
                                                                           });


            query = query.ApplyLike(queryInfo.SearchTerms, g => g.VatTu, g => g.SoLo);

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = isAllData == true ? query.ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            //await Task.WhenAll(countTask, queryTask);

            var queryTask = query.ToArrayAsync();

            var groupData = queryTask.Result.GroupBy(o => new { o.LoaiSuDung, o.Ma, o.VatTu, o.DonViTinh, o.LoaiBHYT, o.SoLo, o.HanSuDung },
                o => o,
                (k, v) => new DanhSachHoanTraKSNKChiTietVo
                {
                    LoaiSuDung = k.LoaiSuDung,
                    Id = v.First().Id,
                    Ma = k.Ma,
                    VatTu = k.VatTu,
                    DonViTinh = k.DonViTinh,
                    LoaiBHYT = k.LoaiBHYT,
                    SoLo = k.SoLo,
                    HanSuDung = k.HanSuDung,
                    SoLuongHoanTra = v.Sum(o => o.SoLuongHoanTra),
                    SoLuongHoanTraDisplay = v.Sum(o => o.SoLuongHoanTra).ApplyNumber()
                }).ToList();
            var returnData = isAllData == true ? groupData.ToArray() : groupData.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = returnData, TotalRowCount = groupData.Count };
        }



        private IQueryable<DanhSachHoanTraKSNKVo> DataYeuCauHoanTraVatTu(bool? duocKeToanDuyet, QueryInfo queryInfo)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => p.DuocDuyet == duocKeToanDuyet && p.KhoXuat.LaKhoKSNK == true && (p.KhoNhap.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoNhap.KhoaPhongId == phongBenhVien.KhoaPhongId))
                .Select(s => new DanhSachHoanTraKSNKVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiVatTu,
                    SoPhieu = s.SoPhieu,
                    NguoiYeuCauId = s.NhanVienYeuCau.Id,
                    TenNguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    NgayYeuCau = s.NgayYeuCau,
                    NgayYeuCauDisplay = s.NgayYeuCau.ApplyFormatDateTimeSACH(),
                    TinhTrang = s.DuocDuyet,
                    HoanTraTuKhoId = s.KhoXuatId,
                    HoanTraTuKho = s.KhoXuat.Ten,
                    HoanTraVeKhoId = s.KhoXuatId,
                    HoanTraVeKho = s.KhoNhap.Ten,
                    NguoiDuyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet,
                    NgayDuyetDisplay = s.NgayDuyet != null ? s.NgayDuyet.Value.ApplyFormatDateTimeSACH() : string.Empty,
                    GhiChu = s.GhiChu
                })
                .Union(_yeuCauTraDuocPhamRepository.TableNoTracking
                    .Where(p => p.DuocDuyet == duocKeToanDuyet && p.KhoXuat.LaKhoKSNK == true && (p.KhoNhap.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoNhap.KhoaPhongId == phongBenhVien.KhoaPhongId))
                    .Select(s => new DanhSachHoanTraKSNKVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham,
                        SoPhieu = s.SoPhieu,
                        NguoiYeuCauId = s.NhanVienYeuCau.Id,
                        TenNguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                        NgayYeuCau = s.NgayYeuCau,
                        NgayYeuCauDisplay = s.NgayYeuCau.ApplyFormatDateTimeSACH(),
                        TinhTrang = s.DuocDuyet,
                        HoanTraTuKhoId = s.KhoXuatId,
                        HoanTraTuKho = s.KhoXuat.Ten,
                        HoanTraVeKhoId = s.KhoXuatId,
                        HoanTraVeKho = s.KhoNhap.Ten,
                        NguoiDuyet = s.NhanVienDuyet.User.HoTen,
                        NgayDuyet = s.NgayDuyet,
                        NgayDuyetDisplay =
                            s.NgayDuyet != null ? s.NgayDuyet.Value.ApplyFormatDateTimeSACH() : string.Empty,
                        GhiChu = s.GhiChu
                    }));
            var result = query.ApplyLike(queryInfo.SearchTerms, g => g.TenNguoiYeuCau, g => g.HoanTraTuKho, g => g.HoanTraVeKho, g => g.SoPhieu, g => g.NguoiDuyet)
                              .OrderBy((queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")) ? queryInfo.SortString : "NgayYeuCau desc,SoPhieu asc");
            return result;
        }

        public async Task<ThongTinHoanTraKSNK> GetThongTinHoanTraKSNK(long yeuCauHoanTraVatTuId)
        {
            var thongTinDuyetKhooVatTu = await BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauHoanTraVatTuId)
                                                                           .Select(cc => new ThongTinHoanTraKSNK
                                                                           {
                                                                               NgayYeuCau = cc.NgayYeuCau,
                                                                               NguoiYeuCauId = cc.NhanVienYeuCauId,
                                                                               TenNguoiYeuCau = cc.NhanVienYeuCau.User.HoTen,

                                                                               HoanTraTuKhoId = cc.KhoXuatId,
                                                                               HoanTraTuKho = cc.KhoXuat.Ten,
                                                                               HoanTraVeKhoId = cc.KhoNhapId,
                                                                               HoanTraVeKho = cc.KhoNhap.Ten,

                                                                               TinhTrang = cc.DuocDuyet,
                                                                               NgayDuyet = cc.NgayDuyet,
                                                                               NguoiDuyet = cc.NhanVienDuyet.User.HoTen,
                                                                               NguoiDuyetId = cc.NhanVienDuyetId,

                                                                               LyDoHuy = cc.LyDoKhongDuyet,
                                                                               GhiChu = cc.GhiChu,
                                                                               LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiVatTu
                                                                           }).FirstOrDefaultAsync();

            var yeuCauTraVatTuChiTiet = await _ycTraVtChiTiet.TableNoTracking.Where(p => p.YeuCauTraVatTuId == yeuCauHoanTraVatTuId)
                                                                                              .FirstOrDefaultAsync();

            if (yeuCauTraVatTuChiTiet != null)
            {
                var xuatKhoVatTu = await _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(p => p.Id == yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTriId)
                                                                                            .Select(p => p.XuatKhoVatTuChiTiet.XuatKhoVatTu)
                                                                                            .FirstOrDefaultAsync();

                if (xuatKhoVatTu != null)
                {
                    thongTinDuyetKhooVatTu.NguoiTraId = xuatKhoVatTu.NguoiXuatId;
                    thongTinDuyetKhooVatTu.TenNguoiTra = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoVatTu.NguoiXuatId).Select(p => p.HoTen).FirstOrDefaultAsync();
                    thongTinDuyetKhooVatTu.NguoiNhanId = xuatKhoVatTu.NguoiNhanId;
                    thongTinDuyetKhooVatTu.TenNguoiNhan = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoVatTu.NguoiNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
                }
            }


            return thongTinDuyetKhooVatTu;
        }

        public async Task<ThongTinHoanTraKSNK> GetThongTinDuyetHoanTraDuocPham(long yeuCauHoanTraDuocPhamId)
        {
            var yeuCauTraDuocPham = await _yeuCauTraDuocPhamRepository.TableNoTracking.Where(p => p.Id == yeuCauHoanTraDuocPhamId)
                                                                                      .Select(cc => new ThongTinHoanTraKSNK
                                                                                      {
                                                                                          NgayYeuCau = cc.NgayYeuCau,
                                                                                          NguoiYeuCauId = cc.NhanVienYeuCauId,
                                                                                          TenNguoiYeuCau = cc.NhanVienYeuCau.User.HoTen,

                                                                                          HoanTraTuKhoId = cc.KhoXuatId,
                                                                                          HoanTraTuKho = cc.KhoXuat.Ten,
                                                                                          HoanTraVeKhoId = cc.KhoNhapId,
                                                                                          HoanTraVeKho = cc.KhoNhap.Ten,

                                                                                          TinhTrang = cc.DuocDuyet,
                                                                                          NgayDuyet = cc.NgayDuyet,
                                                                                          NguoiDuyet = cc.NhanVienDuyet.User.HoTen,
                                                                                          NguoiDuyetId = cc.NhanVienDuyetId,

                                                                                          LyDoHuy = cc.LyDoKhongDuyet,
                                                                                          GhiChu = cc.GhiChu,
                                                                                          LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham
                                                                                      })
                                                                                      .FirstOrDefaultAsync();

            var yeuCauTraDuocPhamChiTiet = await _yeuCauTraDuocPhamChiTiet.TableNoTracking.Where(p => p.YeuCauTraDuocPhamId == yeuCauHoanTraDuocPhamId)
                                                                                                    .FirstOrDefaultAsync();

            if (yeuCauTraDuocPhamChiTiet != null)
            {
                var xuatKhoDuocPham = await _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(p => p.Id == yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTriId)
                                                                                              .Select(p => p.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham)
                                                                                              .FirstOrDefaultAsync();

                if (xuatKhoDuocPham != null)
                {
                    yeuCauTraDuocPham.NguoiTraId = xuatKhoDuocPham.NguoiXuatId;
                    yeuCauTraDuocPham.TenNguoiTra = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoDuocPham.NguoiXuatId).Select(p => p.HoTen).FirstOrDefaultAsync();
                    yeuCauTraDuocPham.NguoiNhanId = xuatKhoDuocPham.NguoiNhanId;
                    yeuCauTraDuocPham.TenNguoiNhan = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoDuocPham.NguoiNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
                }
            }

            return yeuCauTraDuocPham;
        }

        public async Task TuChoiDuyetHoanTraKSNK(long id, string lyDoHuy)
        {
            var yeuCauHoanTraVatTu = BaseRepository.Table
                .Where(p => p.Id == id)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.NhapKhoVatTuChiTiet)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.XuatKhoVatTuChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraVatTu != null && yeuCauHoanTraVatTu.DuocDuyet == null)
            {
                //Cập nhật trạng thái yêu cầu
                yeuCauHoanTraVatTu.DuocDuyet = false;
                yeuCauHoanTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraVatTu.NgayDuyet = DateTime.Now;
                yeuCauHoanTraVatTu.LyDoKhongDuyet = lyDoHuy;

                //Sửa nhập kho                
                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat = (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat).MathRoundNumber(2);
                }
                //Xoá xuất kho chi tiết & xuất kho chi tiết vị trí
                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                    {
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                    }
                    yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTriId = null;
                }

                BaseRepository.Context.SaveChanges();
            }
        }

        public async Task TuChoiDuyetHoanTraDuocPhamKSNK(long yeuCauTraDuocPhamId, string lyDoKhongDuyet)
        {
            var yeuCauHoanTraDuocPham = _yeuCauTraDuocPhamRepository.Table
                .Where(p => p.Id == yeuCauTraDuocPhamId)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.XuatKhoDuocPhamChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraDuocPham != null && yeuCauHoanTraDuocPham.DuocDuyet == null)
            {
                //Cập nhật trạng thái yêu cầu trả dược phẩm

                yeuCauHoanTraDuocPham.DuocDuyet = false;
                yeuCauHoanTraDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraDuocPham.NgayDuyet = DateTime.Now;
                yeuCauHoanTraDuocPham.LyDoKhongDuyet = lyDoKhongDuyet;

                //Sửa nhập kho                
                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat).MathRoundNumber(2);
                }
                //Xoá xuất kho chi tiết & xuất kho chi tiết vị trí
                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                    {
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                    }
                    yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTriId = null;
                }

                _yeuCauTraDuocPhamRepository.Context.SaveChanges();
            }
        }

        public async Task DuyetHoanTraNhapKho(long id, long nguoiNhanId, long nguoiXuatId)
        {
            var yeuCauHoanTraVatTu = BaseRepository.Table
                .Where(p => p.Id == id)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.NhapKhoVatTuChiTiet)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.XuatKhoVatTuChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraVatTu != null && yeuCauHoanTraVatTu.DuocDuyet == null)
            {
                var tenNguoiNhan = await _userRepository.TableNoTracking.Where(p => p.Id == nguoiNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
                var tenNguoiXuat = await _userRepository.TableNoTracking.Where(p => p.Id == nguoiXuatId).Select(p => p.HoTen).FirstOrDefaultAsync();
                //Yêu cầu hoàn trả vật tư
                yeuCauHoanTraVatTu.DuocDuyet = true;
                yeuCauHoanTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraVatTu.NgayDuyet = DateTime.Now;

                var xuatKhoVatTu = new XuatKhoVatTu
                {
                    KhoNhapId = yeuCauHoanTraVatTu.KhoNhapId,
                    KhoXuatId = yeuCauHoanTraVatTu.KhoXuatId,
                    LoaiXuatKho = EnumLoaiXuatKho.XuatQuaKhoKhac,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatQuaKhoKhac.GetDescription(),
                    NguoiNhanId = nguoiNhanId,
                    TenNguoiNhan = tenNguoiNhan,
                    NguoiXuatId = nguoiXuatId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayXuat = DateTime.Now,
                };

                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu = xuatKhoVatTu;
                }

                var newNhapKhoVatTu = new NhapKhoVatTu
                {
                    XuatKhoVatTu = xuatKhoVatTu,
                    TenNguoiGiao = tenNguoiXuat,
                    NguoiGiaoId = nguoiXuatId,
                    NguoiNhapId = nguoiNhanId,
                    DaHet = false,
                    LoaiNguoiGiao = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayNhap = DateTime.Now,
                    KhoId = yeuCauHoanTraVatTu.KhoNhapId,
                };

                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                    {
                        var newNhapKhoVatTuChiTiet = new NhapKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                            HopDongThauVatTuId = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            Solo = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                            HanSuDung = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                            SoLuongNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat,
                            DonGiaNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.DonGiaNhap,
                            VAT = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.VAT,
                            MaVach = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.MaVach,
                            MaRef = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.MaRef,
                            SoLuongDaXuat = 0,
                            NgayNhap = DateTime.Now,
                            LaVatTuBHYT = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                            NgayNhapVaoBenhVien = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                            PhuongPhapTinhGiaTriTonKho = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho,
                            TiLeTheoThapGia = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            KhoViTriId = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTriId,
                            TiLeBHYTThanhToan = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.TiLeBHYTThanhToan
                        };
                        newNhapKhoVatTu.NhapKhoVatTuChiTiets.Add(newNhapKhoVatTuChiTiet);
                    }
                }
                _nhapKhoVatTuRepository.Add(newNhapKhoVatTu);
            }
        }

        public async Task DuyetHoanTraDuocPham(long yeuCauTraDuocPhamId, long nhanVienNhanId, long nhanVienTraId)
        {
            var yeuCauHoanTraDuocPham = _yeuCauTraDuocPhamRepository.Table
                .Where(p => p.Id == yeuCauTraDuocPhamId)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.XuatKhoDuocPhamChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraDuocPham != null && yeuCauHoanTraDuocPham.DuocDuyet == null)
            {
                var tenNguoiNhan = await _userRepository.TableNoTracking.Where(p => p.Id == nhanVienNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
                var tenNguoiXuat = await _userRepository.TableNoTracking.Where(p => p.Id == nhanVienTraId).Select(p => p.HoTen).FirstOrDefaultAsync();
                //Yêu cầu hoàn trả dược phẩm
                yeuCauHoanTraDuocPham.DuocDuyet = true;
                yeuCauHoanTraDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraDuocPham.NgayDuyet = DateTime.Now;

                var xuatKhoDuocPham = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham
                {
                    KhoXuatId = yeuCauHoanTraDuocPham.KhoXuatId,
                    KhoNhapId = yeuCauHoanTraDuocPham.KhoNhapId,
                    LoaiXuatKho = Core.Domain.Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                    LyDoXuatKho = Core.Domain.Enums.XuatKhoDuocPham.XuatQuaKhoKhac.GetDescription(),
                    NguoiNhanId = nhanVienNhanId,
                    TenNguoiNhan = tenNguoiNhan,
                    NguoiXuatId = nhanVienTraId,
                    LoaiNguoiNhan = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayXuat = DateTime.Now
                };

                //Xuất kho dược phẩm
                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = xuatKhoDuocPham;
                }

                //Nhập kho dược phẩm
                var newNhapKhoDuocPham = new NhapKhoDuocPham
                {
                    XuatKhoDuocPham = xuatKhoDuocPham,
                    TenNguoiGiao = tenNguoiXuat,
                    NguoiGiaoId = nhanVienTraId,
                    NguoiNhapId = nhanVienNhanId,
                    DaHet = false,
                    LoaiNguoiGiao = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayNhap = DateTime.Now,
                    KhoId = yeuCauHoanTraDuocPham.KhoNhapId,
                };

                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                    {
                        var newNhapKhoDuocPhamChiTiet = new NhapKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                            HopDongThauDuocPhamId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                            Solo = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Solo,
                            HanSuDung = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung,
                            SoLuongNhap = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat,
                            DonGiaNhap = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                            VAT = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.VAT,
                            MaVach = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.MaVach,
                            MaRef = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.MaRef,
                            SoLuongDaXuat = 0,
                            NgayNhap = DateTime.Now,
                            LaDuocPhamBHYT = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                            DuocPhamBenhVienPhanNhomId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId,
                            NgayNhapVaoBenhVien = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien,
                            PhuongPhapTinhGiaTriTonKho = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                            TiLeTheoThapGia = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                            KhoViTriId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoViTriId,
                            TiLeBHYTThanhToan = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan
                        };
                        newNhapKhoDuocPham.NhapKhoDuocPhamChiTiets.Add(newNhapKhoDuocPhamChiTiet);
                    }
                }
                _nhapKhoDuocPhamRepository.Add(newNhapKhoDuocPham);
            }
        }

        private void CapNhatSoLuongNhapKhoVatTuTheoNhapKhoVatTuChiTietId(long nhapKhoVatTuChiTietId, double soLuongTra)
        {
            var nhapKhoChiTiet = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(cc => cc.Id == nhapKhoVatTuChiTietId).FirstOrDefault();

            if (nhapKhoChiTiet != null)
            {
                nhapKhoChiTiet.SoLuongDaXuat -= soLuongTra;

            }
            _nhapKhoVatTuChiTietRepository.Update(nhapKhoChiTiet);
        }

        private void XoaPhieu(YeuCauTraVatTu yeuCauTraVatTu)
        {
            var nhapKhoChiTiets = yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Select(cc => cc.XuatKhoVatTuChiTietViTri).FirstOrDefault();
            if (nhapKhoChiTiets != null)
            {
                _xuatKhoVatTuChiTietRepository.Delete(nhapKhoChiTiets.XuatKhoVatTuChiTiet);
                _xuatKhoVatTuChiTietViTriRepository.Delete(nhapKhoChiTiets);
            }
        }

        public string GetHtmlPhieuInHoanTraKSNK(long yeuCauHoanTraVatTuId, string hostingName)
        {
            var template = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTraVatTuTuTruc"));

            var hoanTraVatTuChiTiets = _ycTraVtChiTiet.TableNoTracking
                                          .Where(p => p.YeuCauTraVatTuId == yeuCauHoanTraVatTuId)
                                          .Select(s => new PhieuHoanTraKSNKChiTietData
                                          {
                                              SoPhieu = s.YeuCauTraVatTu.SoPhieu,
                                              KhoTraLai = s.YeuCauTraVatTu.KhoXuat.Ten,
                                              KhoNhan = s.YeuCauTraVatTu.KhoNhap.Ten,
                                              Ten = s.VatTuBenhVien.VatTus.Ten,
                                              SoLo = s.Solo,
                                              NuocSX = s.VatTuBenhVien.VatTus.NuocSanXuat,
                                              HanSuDung = s.HanSuDung,
                                              DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                              SoLuong = s.SoLuongTra,
                                              DonGia = s.DonGiaNhap,
                                              CreatedOn = s.YeuCauTraVatTu.CreatedOn
                                          }).OrderBy(p => p.Ten).ToList();
            var thuocChiTiet = string.Empty;
            var STT = 1;

            foreach (var item in hoanTraVatTuChiTiets)
            {
                thuocChiTiet +=
                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenVatTu
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                           + "</tr>";
                STT++;
            }
            var tongThanhTien = hoanTraVatTuChiTiets.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
            thuocChiTiet += "<tr>"
                                + "<td colspan='2'><b>Tổng cộng: </b></td>"
                                + "<td>&nbsp;</td>"
                                + "<td>&nbsp;</td>"
                                + "<td>&nbsp;</td>"
                                + "<td style = 'text-align: right; padding-right:3px'>" + tongThanhTien + "</td>"
                                + "<td>&nbsp;</td>"
                           + "</tr>";
            var data = new PhieuHoanTraKSNKData
            {
                SoPhieu = hoanTraVatTuChiTiets.First().SoPhieu,
                KhoTraLai = hoanTraVatTuChiTiets.First().KhoTraLai,
                KhoNhan = hoanTraVatTuChiTiets.First().KhoNhan,
                BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraVatTuChiTiets.First().SoPhieu),
                NgayLapPhieu = hoanTraVatTuChiTiets.First().CreatedOn?.ApplyFormatDate(),
                ThuocVatTu = thuocChiTiet,
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString()
            };
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
    }

    #endregion
}
