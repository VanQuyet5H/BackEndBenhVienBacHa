using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaKSNKTaiGiamDocs;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan
{
    public partial class YeuCauMuaDuTruKiemSoatNhiemKhuanService
    {
        private IQueryable<DuTruGiamDocKSNKGridVo> GetDuTru(bool? giamDocDuyet, QueryInfo queryInfo, DuTruGiamDocKSNKQueryVo queryObject)
        {
            var result = _duTruMuaVatTuKhoDuocRepository.TableNoTracking
                .Where(p => p.GiamDocDuyet == giamDocDuyet)
                .Select(s => new DuTruGiamDocKSNKGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    TuNgay = s.TuNgay,
                    DenNgay = s.DenNgay,
                    NgayYeuCau = s.NgayYeuCau,
                    NgayDuyet = s.NgayGiamDocDuyet,
                    TrangThai = s.GiamDocDuyet,
                    KyDuTru = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen
                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.NguoiYeuCau
                );

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau asc,Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.startDate != null)
                {
                    var tuNgay = queryObject.RangeYeuCau.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => tuNgay <= p.NgayYeuCau.Date);
                }
                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.endDate != null)
                {
                    var denNgay = queryObject.RangeYeuCau.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayYeuCau.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    result = result.Where(p => tuNgay <= p.NgayDuyet.Value.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayDuyet.Value.Date);
                }
            }

            return result;
        }

        public async Task<GridDataSource> GetDataGiamDocForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new DuTruGiamDocKSNKQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruGiamDocKSNKQueryVo>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }

            var queryDangChoDuyet = GetDuTru(null, queryInfo, queryObject);
            var queryDaDuyet = GetDuTru(true, queryInfo, queryObject);
            var queryTuChoiDuyet = GetDuTru(false, queryInfo, queryObject);

            var query = _duTruMuaVatTuKhoDuocRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruGiamDocKSNKGridVo()).AsQueryable();

            var isHaveQuery = false;
            if (queryObject != null && queryObject.DangChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject != null && queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject != null && queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayYeuCau asc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetGiamDocTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new DuTruGiamDocKSNKQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruGiamDocKSNKQueryVo>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }

            var queryDangChoDuyet = GetDuTru(null, queryInfo, queryObject);
            var queryDaDuyet = GetDuTru(true, queryInfo, queryObject);
            var queryTuChoiDuyet = GetDuTru(false, queryInfo, queryObject);

            var query = _duTruMuaVatTuKhoDuocRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruGiamDocKSNKGridVo()).AsQueryable();

            var isHaveQuery = false;
            if (queryObject != null && queryObject.DangChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject != null && queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject != null && queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayYeuCau asc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<GridDataSource> GetDataForGridChildChuaDuyetAsync(QueryInfo queryInfo)
        {
            var query = _duTruMuaVatTuKhoDuocChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(s => new DuTruGiamDocKSNKDetailGridVo
                {
                    Id = s.Id,
                    IsBhyt = s.LaVatTuBHYT,
                    VatTu = s.VatTu.Ten,
                    VatTuId = s.VatTuId,
                    QuyCach = s.VatTu.QuyCach,
                    Dvt = s.VatTu.DonViTinh,
                    NhaSx = s.VatTu.NhaSanXuat,
                    NuocSx = s.VatTu.NuocSanXuat,
                    SoLuongDuTruDirector = s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true
                        ? s.SoLuongDuTruGiamDocDuyet
                        : s.SoLuongDuTru,
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienTrongKy = s.SoLuongDuKienSuDung,
                    SoLuongDuTruKhDuoc = s.SoLuongDuTruKhoDuocDuyet,
                    SoLuongDuTruTrKhoa = s.SoLuongDuTruTruongKhoaDuyet,
                    KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && x.NhapKhoVatTu.Kho.LoaiKho ==
                                        Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                    x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && x.NhapKhoVatTu.Kho.LoaiKho ==
                                        Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                    && x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new TonKSNKChuaNhap
                                    {
                                        Name = x.NhapKhoVatTu.Kho.Ten,
                                        TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                    }).ToList(),
                    HdChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuId == s.VatTuId
                                    && x.SoLuongDaCap < x.SoLuong && x.HopDongThauVatTu.NgayHetHan >= DateTime.Now).Sum(x => x.SoLuong - x.SoLuongDaCap),
                    HdChuaNhapList = _hopDongThauVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuId == s.VatTuId && x.HopDongThauVatTu.NgayHetHan >= DateTime.Now
                                                           && x.SoLuongDaCap < x.SoLuong).Select(z => new TonKSNKChuaNhap
                                                           {
                                                               Name = z.HopDongThauVatTu.SoHopDong,
                                                               TongTon = z.SoLuong - z.SoLuongDaCap
                                                           }).ToList()
                });

            var queryTask = query.OrderBy(queryInfo.SortString)
                .Skip(queryInfo.Skip)
                .Take(queryInfo.Take)
                .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            await Task.WhenAll(queryTask);

            foreach (var item in queryTask.Result)
            {
                item.KhoDuTruTon = await GetKhoDuTruTon(item);
            }

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = await countTask
            };
        }

        private async Task<double> GetKhoDuTruTon(DuTruGiamDocKSNKDetailGridVo thongTinChiTietVatTuTonKho)
        {
            var duTruMuaDpTheoKhoaQueryableIds = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                .Where(q => q.DuTruMuaVatTuKhoDuocChiTietId == thongTinChiTietVatTuTonKho.Id)
                .Select(q => q.DuTruMuaVatTuTheoKhoaId);

            var khoQueryableId = _duTruMuaVatTuChiTietRepository.TableNoTracking
                .Where(q => q.DuTruMuaVatTuKhoDuocChiTietId == thongTinChiTietVatTuTonKho.Id)
                .Select(q => q.DuTruMuaVatTu.KhoId);

            var duTruMuaVtTheoKhoaIds = await duTruMuaDpTheoKhoaQueryableIds.ToListAsync();

            var seperatedKhoId = await khoQueryableId.FirstOrDefaultAsync();

            List<long> khos = new List<long>();

            foreach (var duTruMuaDpTheoKhoaId in duTruMuaVtTheoKhoaIds)
            {
                var khosQueryable = BaseRepository.TableNoTracking
                    .Where(e => e.DuTruMuaVatTuTheoKhoaId == duTruMuaDpTheoKhoaId)
                    .Select(e => e.KhoId);
                var khoCurrents = await khosQueryable.ToListAsync();
                khos.AddRange(khoCurrents);
            }
            var lstKho = khos.Distinct();

            double result = 0;
            foreach (var khoId in lstKho)
            {
                var tong = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.VatTuBenhVienId == thongTinChiTietVatTuTonKho.VatTuId
                                && x.NhapKhoVatTu.KhoId == khoId
                                && x.LaVatTuBHYT == thongTinChiTietVatTuTonKho.IsBhyt
                                && x.NhapKhoVatTu.DaHet != true
                                && x.SoLuongDaXuat < x.SoLuongNhap).SumAsync(x => x.SoLuongNhap - x.SoLuongDaXuat);
                result += tong;
            }

            var tongSeperated = seperatedKhoId != 0 ? await _nhapKhoVatTuChiTietRepository.TableNoTracking
                .Where(x => x.VatTuBenhVienId == thongTinChiTietVatTuTonKho.VatTuId
                            && x.NhapKhoVatTu.KhoId == seperatedKhoId
                            && x.LaVatTuBHYT == thongTinChiTietVatTuTonKho.IsBhyt
                            && x.NhapKhoVatTu.DaHet != true
                            && x.SoLuongDaXuat < x.SoLuongNhap).SumAsync(x => x.SoLuongNhap - x.SoLuongDaXuat) : 0;
            result += tongSeperated;
            return result;
        }

        public async Task<GridDataSource> GetDataForGridChildDuyetAsync(QueryInfo queryInfo)
        {
            var query = _duTruMuaVatTuKhoDuocChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(s => new DuTruGiamDocKSNKDetailGridVo
                {
                    Id = s.Id,
                    IsBhyt = s.LaVatTuBHYT,
                    VatTu = s.VatTu.Ten,
                    VatTuId = s.VatTuId,
                    QuyCach = s.VatTu.QuyCach,
                    Dvt = s.VatTu.DonViTinh,
                    NhaSx = s.VatTu.NhaSanXuat,
                    NuocSx = s.VatTu.NuocSanXuat,
                    SoLuongDuTruDirector = s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true ? s.SoLuongDuTruGiamDocDuyet : s.SoLuongDuTru,
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienTrongKy = s.SoLuongDuKienSuDung,
                    SoLuongDuTruKhDuoc = s.SoLuongDuTruKhoDuocDuyet,
                    SoLuongDuTruTrKhoa = s.SoLuongDuTruTruongKhoaDuyet,
                    HdChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuId == s.VatTuId
                                    && x.SoLuongDaCap < x.SoLuong && x.HopDongThauVatTu.NgayHetHan >= DateTime.Now).Sum(x => x.SoLuong - x.SoLuongDaCap),
                    KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    HdChuaNhapList = _hopDongThauVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuId == s.VatTuId && x.HopDongThauVatTu.NgayHetHan >= DateTime.Now
                                    && x.SoLuongDaCap < x.SoLuong).Select(z => new TonKSNKChuaNhap
                                    {
                                        Name = z.HopDongThauVatTu.SoHopDong,
                                        TongTon = z.SoLuong - z.SoLuongDaCap
                                    }).ToList(),
                    TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                    && x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new TonKSNKChuaNhap
                                    {
                                        Name = x.NhapKhoVatTu.Kho.Ten,
                                        TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                    }).GroupBy(q => q.Name)
                                    .Select(e => new TonKSNKChuaNhap
                                    {
                                        Name = e.First().Name,
                                        TongTon = e.Sum(q => q.TongTon)
                                    }).ToList()
                });

            var queryTask = query.OrderBy(queryInfo.SortString)
                .Skip(queryInfo.Skip)
                .Take(queryInfo.Take)
                .ToArrayAsync();
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(queryTask, countTask);
            foreach (var item in queryTask.Result)
            {
                item.KhoDuTruTon = await GetKhoDuTruTon(item);
            }
            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _duTruMuaVatTuKhoDuocChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(s => new DuTruGiamDocKSNKDetailGridVo
                {
                    Id = s.Id,
                    IsBhyt = s.LaVatTuBHYT,
                    VatTu = s.VatTu.Ten,
                    VatTuId = s.VatTuId,
                    QuyCach = s.VatTu.QuyCach,
                    Dvt = s.VatTu.DonViTinh,
                    NhaSx = s.VatTu.NhaSanXuat,
                    NuocSx = s.VatTu.NuocSanXuat,
                    SoLuongDuTruDirector = s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true ? s.SoLuongDuTruGiamDocDuyet : s.SoLuongDuTru,
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienTrongKy = s.SoLuongDuKienSuDung,
                    SoLuongDuTruKhDuoc = s.SoLuongDuTruKhoDuocDuyet,
                    SoLuongDuTruTrKhoa = s.SoLuongDuTruTruongKhoaDuyet,
                    HdChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuId == s.VatTuId
                                    && x.SoLuongDaCap < x.SoLuong).Sum(x => x.SoLuong - x.SoLuongDaCap),
                    KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && (x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap1 ||
                                        x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2) && x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    HdChuaNhapList = _hopDongThauVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuId == s.VatTuId
                                    && x.SoLuongDaCap < x.SoLuong).Select(z => new TonKSNKChuaNhap
                                    {
                                        Name = z.HopDongThauVatTu.NhaThau.Ten,
                                        TongTon = z.SoLuong - z.SoLuongDaCap
                                    }).ToList(),
                    TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.VatTuBenhVienId == s.VatTuId
                                    && (x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap1 ||
                                        x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                                    && x.LaVatTuBHYT == s.LaVatTuBHYT
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new TonKSNKChuaNhap
                                    {
                                        Name = x.NhapKhoVatTu.Kho.Ten,
                                        TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                    }).ToList()
                });

            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<GridDataSource> GetDataForGridDuyetDetail(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long id = Convert.ToInt64(queryString[0]);
            bool laBhyt = bool.Parse(queryString[1]);
            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruKSNKTheoChiTietGridVo()).AsQueryable();
            var duTruMuaVtTheoKhoaDetailedIds = await _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocChiTietId == id)
                .Select(e => e.Id).ToListAsync();
            foreach (var duTruMuaDpTheoKhoaDetailedIdItem in duTruMuaVtTheoKhoaDetailedIds)
            {
                var duTruMuaVtChiTiets = _duTruMuaVatTuChiTietRepository.TableNoTracking
                    .Where(e => e.DuTruMuaVatTuTheoKhoaChiTietId == duTruMuaDpTheoKhoaDetailedIdItem &&
                                e.LaVatTuBHYT == laBhyt)
                    .Select(e => new DuTruKSNKTheoChiTietGridVo
                    {
                        Id = e.Id,
                        Kho = e.DuTruMuaVatTu.Kho.Ten,
                        KhoaId = e.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoa.KhoaPhongId,
                        Khoa = e.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoa.KhoaPhong.Ten,
                        KyDuTruDisplay = e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                         " - " +
                                         e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                        SoLuongDuTru = e.SoLuongDuTru,
                        SoLuongDuKienTrongKy = e.SoLuongDuKienSuDung
                    });
                query = query.Concat(duTruMuaVtChiTiets);
            }

            var duTruMuaVtChiTietTuKhoTongs = _duTruMuaVatTuChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocChiTietId == id &&
                            e.LaVatTuBHYT == laBhyt)
                .Select(e => new DuTruKSNKTheoChiTietGridVo
                {
                    Id = e.Id,
                    Kho = e.DuTruMuaVatTu.Kho.Ten,
                    KhoaId = 57,
                    Khoa = "Khoa Dược",
                    KyDuTruDisplay = e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                     " - " +
                                     e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                    SoLuongDuTru = e.SoLuongDuTru,
                    SoLuongDuKienTrongKy = e.SoLuongDuKienSuDung
                });
            query = query.Concat(duTruMuaVtChiTietTuKhoTongs);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task Approve(long id)
        {
            var duTruMuaVtKhoDuocEntity = await _duTruMuaVatTuKhoDuocRepository.TableNoTracking
                .Include(q => q.DuTruMuaVatTuKhoDuocChiTiets)
                .FirstAsync(e => e.Id == id);
            duTruMuaVtKhoDuocEntity.GiamDocDuyet = true;
            duTruMuaVtKhoDuocEntity.GiamDocId = _userAgentHelper.GetCurrentUserId();
            duTruMuaVtKhoDuocEntity.NgayGiamDocDuyet = DateTime.Now;
            foreach (var duTruChiTiet in duTruMuaVtKhoDuocEntity.DuTruMuaVatTuKhoDuocChiTiets)
            {
                duTruChiTiet.SoLuongDuTruGiamDocDuyet = duTruChiTiet.SoLuongDuTru;
            }
            await _duTruMuaVatTuKhoDuocRepository.UpdateAsync(duTruMuaVtKhoDuocEntity);
        }

        public async Task ApproveForm(DuTruGiamDocKSNKApproveGridVo duTruGiamDoc)
        {
            var duTruChiTietVtIds = await _duTruMuaVatTuKhoDuocChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocId == duTruGiamDoc.Id)
                .Select(e => e.Id).ToListAsync();
            var duTruMuaDpKhoDuocEntity = await _duTruMuaVatTuKhoDuocRepository.TableNoTracking
                .Include(q => q.DuTruMuaVatTuKhoDuocChiTiets)
                .FirstAsync(e => e.Id == duTruGiamDoc.Id);


            foreach (var duTruChiTietId in duTruChiTietVtIds)
            {
                if (duTruGiamDoc.ChiTietKSNKList.Any(e => e.Id == duTruChiTietId))
                {
                    duTruMuaDpKhoDuocEntity.DuTruMuaVatTuKhoDuocChiTiets.First(q => q.Id == duTruChiTietId)
                        .SoLuongDuTruGiamDocDuyet = duTruGiamDoc.ChiTietKSNKList
                        .First(w => w.Id == duTruChiTietId).SoLuongDuyet;
                }
            }

            duTruMuaDpKhoDuocEntity.GiamDocDuyet = true;
            duTruMuaDpKhoDuocEntity.GiamDocId = _userAgentHelper.GetCurrentUserId();
            duTruMuaDpKhoDuocEntity.NgayGiamDocDuyet = DateTime.Now;
            await _duTruMuaVatTuKhoDuocRepository.UpdateAsync(duTruMuaDpKhoDuocEntity);
        }

        public async Task Decline(DuTruGiamDocKSNKApproveGridVo duTruGiamDoc)
        {
            var duTruMuaDpKhoDuocEntity = await _duTruMuaVatTuKhoDuocRepository.TableNoTracking.FirstAsync(e => e.Id == duTruGiamDoc.Id);
            duTruMuaDpKhoDuocEntity.GiamDocDuyet = false;
            duTruMuaDpKhoDuocEntity.GiamDocId = _userAgentHelper.GetCurrentUserId();
            duTruMuaDpKhoDuocEntity.NgayGiamDocDuyet = DateTime.Now;
            duTruMuaDpKhoDuocEntity.LyDoGiamDocTuChoi = duTruGiamDoc.LyDo;

            await _duTruMuaVatTuKhoDuocRepository.UpdateAsync(duTruMuaDpKhoDuocEntity);
        }

        public async Task<DuTruMuaVatTuKhoDuoc> GetDuTruMuaVatTuKhoDuocByIdAsync(long duTruMuaVatTuKhoDuocId)
        {
            var duTruMuaDpKhoDuocEntity = await _duTruMuaVatTuKhoDuocRepository                                    
                                    .TableNoTracking.Include(o=>o.KyDuTruMuaDuocPhamVatTu)
                                    .Include(o => o.KyDuTruMuaDuocPhamVatTu)
                                    .Include(q => q.GiamDoc).ThenInclude(q => q.User)
                                    .Include(q => q.NhanVienYeuCau).ThenInclude(q => q.User)
                                    .FirstAsync(e => e.Id == duTruMuaVatTuKhoDuocId);

            return duTruMuaDpKhoDuocEntity;
        }

    }
}
