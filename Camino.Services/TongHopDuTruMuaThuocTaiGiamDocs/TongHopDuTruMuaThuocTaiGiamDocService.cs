using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaThuocTaiGiamDocs;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.TongHopDuTruMuaThuocTaiGiamDocs
{
    [ScopedDependency(ServiceType = typeof(ITongHopDuTruMuaThuocTaiGiamDocService))]
    public class TongHopDuTruMuaThuocTaiGiamDocService : MasterFileService<DuTruMuaDuocPhamKhoDuoc>, ITongHopDuTruMuaThuocTaiGiamDocService
    {
        private readonly IRepository<DuTruMuaDuocPhamKhoDuocChiTiet> _duTruMuaKhoDuocChiTiet;
        private readonly IRepository<DuTruMuaDuocPham> _duTruMuaDp;
        private readonly IRepository<DuTruMuaDuocPhamChiTiet> _duTruMuaDpDetailed;
        private readonly IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> _duTruMuaDpTheoKhoaChiTiet;
        private readonly IRepository<HopDongThauDuocPhamChiTiet> _hopDongThauDuocPhamChiTiet;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IUserAgentHelper _userAgentHelper;

        public TongHopDuTruMuaThuocTaiGiamDocService(
            IRepository<DuTruMuaDuocPhamKhoDuoc> repository,
            IRepository<DuTruMuaDuocPhamKhoDuocChiTiet> duTruMuaKhoDuocChiTiet,
            IRepository<DuTruMuaDuocPham> duTruMuaDp,
            IRepository<HopDongThauDuocPhamChiTiet> hopDongThauDuocPhamChiTiet,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> duTruMuaDpTheoKhoaChiTiet,
            IRepository<DuTruMuaDuocPhamChiTiet> duTruMuaDpDetailed,
            IUserAgentHelper userAgentHelper
        ) : base(repository)
        {
            _duTruMuaKhoDuocChiTiet = duTruMuaKhoDuocChiTiet;
            _hopDongThauDuocPhamChiTiet = hopDongThauDuocPhamChiTiet;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _duTruMuaDpTheoKhoaChiTiet = duTruMuaDpTheoKhoaChiTiet;
            _duTruMuaDpDetailed = duTruMuaDpDetailed;
            _userAgentHelper = userAgentHelper;
            _duTruMuaDp = duTruMuaDp;
        }

        private IQueryable<DuTruGiamDocGridVo> GetDuTru(bool? giamDocDuyet, QueryInfo queryInfo, DuTruGiamDocQueryVo queryObject)
        {
            var result = BaseRepository.TableNoTracking
                .Where(p => p.GiamDocDuyet == giamDocDuyet)
                .Select(s => new DuTruGiamDocGridVo
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

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new DuTruGiamDocQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruGiamDocQueryVo>(queryInfo.AdditionalSearchString);
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

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruGiamDocGridVo()).AsQueryable();

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

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new DuTruGiamDocQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruGiamDocQueryVo>(queryInfo.AdditionalSearchString);
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

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruGiamDocGridVo()).AsQueryable();

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
            var query = _duTruMuaKhoDuocChiTiet.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(s => new DuTruGiamDocDetailGridVo
                {
                    Id = s.Id,
                    IsBhyt = s.LaDuocPhamBHYT,
                    DuocPham = s.DuocPham.Ten,
                    DuocPhamId = s.DuocPhamId,
                    HoatChat = s.DuocPham.HoatChat,
                    NongDo = s.DuocPham.HamLuong,
                    Sdk = s.DuocPham.SoDangKy,
                    DuongDung = s.DuocPham.DuongDung.Ten,
                    Dvt = s.DuocPham.DonViTinh.Ten,
                    NhaSx = s.DuocPham.NhaSanXuat,
                    NuocSx = s.DuocPham.NuocSanXuat,
                    SoLuongDuTruDirector = s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true
                        ? s.SoLuongDuTruGiamDocDuyet
                        : s.SoLuongDuTru,
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienTrongKy = s.SoLuongDuKienSuDung,
                    SoLuongDuTruKhDuoc = s.SoLuongDuTruKhoDuocDuyet,
                    SoLuongDuTruTrKhoa = s.SoLuongDuTruTruongKhoaDuyet,
                    HdChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId
                                    && x.SoLuongDaCap < x.SoLuong && x.HopDongThauDuocPham.NgayHetHan >= DateTime.Now).Sum(x => x.SoLuong - x.SoLuongDaCap),
                    HdChuaNhapList = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId && x.HopDongThauDuocPham.NgayHetHan >= DateTime.Now
                                    && x.SoLuongDaCap < x.SoLuong).Select(z => new TonChuaNhap
                        {
                            Name = z.HopDongThauDuocPham.SoHopDong,
                            TongTon = z.SoLuong - z.SoLuongDaCap
                        }).ToList(),
                    KhoTongTon = s.DuocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho ==
                                    Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                    x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    TongTonList = s.DuocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho ==
                                    Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                    && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new TonChuaNhap
                                    {
                                        Name = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                        TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                    }).GroupBy(q => q.Name)
                                    .Select(e => new TonChuaNhap
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
        public async Task<GridDataSource> GetDataForGridChildChuaDuyetTaiGiamDocAsync(QueryInfo queryInfo)
        {
            var query = _duTruMuaKhoDuocChiTiet.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(s => new DuTruGiamDocDetailGridVo
                {
                    Id = s.Id,
                    IsBhyt = s.LaDuocPhamBHYT,
                    DuocPham = s.DuocPham.Ten,
                    DuocPhamId = s.DuocPhamId,
                    HoatChat = s.DuocPham.HoatChat,
                    NongDo = s.DuocPham.HamLuong,
                    Sdk = s.DuocPham.SoDangKy,
                    DuongDung = s.DuocPham.DuongDung.Ten,
                    Dvt = s.DuocPham.DonViTinh.Ten,
                    NhaSx = s.DuocPham.NhaSanXuat,
                    NuocSx = s.DuocPham.NuocSanXuat,
                    SoLuongDuTruDirector = s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true
                        ? s.SoLuongDuTruGiamDocDuyet
                        : s.SoLuongDuTru,
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienTrongKy = s.SoLuongDuKienSuDung,
                    SoLuongDuTruKhDuoc = s.SoLuongDuTruKhoDuocDuyet,
                    SoLuongDuTruTrKhoa = s.SoLuongDuTruTruongKhoaDuyet,
                    HdChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId
                                    && x.SoLuongDaCap < x.SoLuong && x.HopDongThauDuocPham.NgayHetHan >= DateTime.Now).Sum(x => x.SoLuong - x.SoLuongDaCap),
                    HdChuaNhapList = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId && x.HopDongThauDuocPham.NgayHetHan >= DateTime.Now
                                    && x.SoLuongDaCap < x.SoLuong).Select(z => new TonChuaNhap
                                    {
                                        Name = z.HopDongThauDuocPham.SoHopDong,
                                        TongTon = z.SoLuong - z.SoLuongDaCap
                                    }).ToList(),
                    KhoTongTon = s.DuocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho ==
                                    Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                    x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    TongTonList = s.DuocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho ==
                                    Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                    && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new TonChuaNhap
                                    {
                                        Name = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                        TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                    }).GroupBy(q => q.Name)
                                    .Select(e => new TonChuaNhap
                                    {
                                        Name = e.First().Name,
                                        TongTon = e.Sum(q => q.TongTon)
                                    }).ToList()
                });

            var queryTask = query.OrderBy(queryInfo.SortString)
                //.Skip(queryInfo.Skip)
                //.Take(queryInfo.Take)
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

        private async Task<double> GetKhoDuTruTon(DuTruGiamDocDetailGridVo thongTinChiTietDuocPhamTonKho)
        {
            var duTruMuaDpTheoKhoaQueryableIds = _duTruMuaDpTheoKhoaChiTiet.TableNoTracking
                .Where(q => q.DuTruMuaDuocPhamKhoDuocChiTietId == thongTinChiTietDuocPhamTonKho.Id)
                .Select(q => q.DuTruMuaDuocPhamTheoKhoaId);

            var khoQueryableId = _duTruMuaDpDetailed.TableNoTracking
                .Where(q => q.DuTruMuaDuocPhamKhoDuocChiTietId == thongTinChiTietDuocPhamTonKho.Id)
                .Select(q => q.DuTruMuaDuocPham.KhoId);

            var duTruMuaDpTheoKhoaIds = await duTruMuaDpTheoKhoaQueryableIds.ToListAsync();

            var seperatedKhoId = await khoQueryableId.FirstOrDefaultAsync();

            List<long> khos = new List<long>();

            foreach (var duTruMuaDpTheoKhoaId in duTruMuaDpTheoKhoaIds)
            {
                var khosQueryable = _duTruMuaDp.TableNoTracking
                    .Where(e => e.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDpTheoKhoaId)
                    .Select(e => e.KhoId);
                var khoCurrents = await khosQueryable.ToListAsync();
                khos.AddRange(khoCurrents);
            }
            var lstKho = khos.Distinct();

            double result = 0;
            foreach (var khoId in lstKho)
            {
                var tong = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.DuocPhamBenhVienId == thongTinChiTietDuocPhamTonKho.DuocPhamId
                                && x.NhapKhoDuocPhams.KhoId == khoId
                                && x.LaDuocPhamBHYT == thongTinChiTietDuocPhamTonKho.IsBhyt
                                && x.NhapKhoDuocPhams.DaHet != true
                                && x.SoLuongDaXuat < x.SoLuongNhap).SumAsync(x => x.SoLuongNhap - x.SoLuongDaXuat);
                result += tong;
            }

            var tongSeperated = seperatedKhoId != 0 ? await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(x => x.DuocPhamBenhVienId == thongTinChiTietDuocPhamTonKho.DuocPhamId
                            && x.NhapKhoDuocPhams.KhoId == seperatedKhoId
                            && x.LaDuocPhamBHYT == thongTinChiTietDuocPhamTonKho.IsBhyt
                            && x.NhapKhoDuocPhams.DaHet != true
                            && x.SoLuongDaXuat < x.SoLuongNhap).SumAsync(x => x.SoLuongNhap - x.SoLuongDaXuat) : 0;
            result += tongSeperated;
            return result;
        }

        public async Task<GridDataSource> GetDataForGridChildDuyetAsync(QueryInfo queryInfo)
        {
            var query = _duTruMuaKhoDuocChiTiet.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(s => new DuTruGiamDocDetailGridVo
                {
                    Id = s.Id,
                    IsBhyt = s.LaDuocPhamBHYT,
                    DuocPham = s.DuocPham.Ten,
                    DuocPhamId = s.DuocPhamId,
                    HoatChat = s.DuocPham.HoatChat,
                    NongDo = s.DuocPham.HamLuong,
                    Sdk = s.DuocPham.SoDangKy,
                    DuongDung = s.DuocPham.DuongDung.Ten,
                    Dvt = s.DuocPham.DonViTinh.Ten,
                    NhaSx = s.DuocPham.NhaSanXuat,
                    NuocSx = s.DuocPham.NuocSanXuat,
                    SoLuongDuTruDirector = s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true ? s.SoLuongDuTruGiamDocDuyet : s.SoLuongDuTru,
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienTrongKy = s.SoLuongDuKienSuDung,
                    SoLuongDuTruKhDuoc = s.SoLuongDuTruKhoDuocDuyet,
                    SoLuongDuTruTrKhoa = s.SoLuongDuTruTruongKhoaDuyet,
                    HdChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId
                                    && x.SoLuongDaCap < x.SoLuong && x.HopDongThauDuocPham.NgayHetHan >= DateTime.Now).Sum(x => x.SoLuong - x.SoLuongDaCap),
                    KhoTongTon = s.DuocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                    && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    HdChuaNhapList = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId && x.HopDongThauDuocPham.NgayHetHan >= DateTime.Now
                                    && x.SoLuongDaCap < x.SoLuong).Select(z => new TonChuaNhap
                                    {
                                        Name = z.HopDongThauDuocPham.SoHopDong,
                                        TongTon = z.SoLuong - z.SoLuongDaCap
                                    }).ToList(),
                    TongTonList = s.DuocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                    && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new TonChuaNhap
                                    {
                                        Name = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                        TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                    }).GroupBy(q => q.Name)
                                    .Select(e => new TonChuaNhap
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
            var query = _duTruMuaKhoDuocChiTiet.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(s => new DuTruGiamDocDetailGridVo
                {
                    Id = s.Id,
                    IsBhyt = s.LaDuocPhamBHYT,
                    DuocPham = s.DuocPham.Ten,
                    DuocPhamId = s.DuocPhamId,
                    HoatChat = s.DuocPham.HoatChat,
                    NongDo = s.DuocPham.HamLuong,
                    Sdk = s.DuocPham.SoDangKy,
                    DuongDung = s.DuocPham.DuongDung.Ten,
                    Dvt = s.DuocPham.DonViTinh.Ten,
                    NhaSx = s.DuocPham.NhaSanXuat,
                    NuocSx = s.DuocPham.NuocSanXuat,
                    SoLuongDuTruDirector = s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true ? s.SoLuongDuTruGiamDocDuyet : s.SoLuongDuTru,
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienTrongKy = s.SoLuongDuKienSuDung,
                    SoLuongDuTruKhDuoc = s.SoLuongDuTruKhoDuocDuyet,
                    SoLuongDuTruTrKhoa = s.SoLuongDuTruTruongKhoaDuyet,
                    HdChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId
                                    && x.SoLuongDaCap < x.SoLuong).Sum(x => x.SoLuong - x.SoLuongDaCap),
                    KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && (x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 ||
                                        x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2) && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    HdChuaNhapList = _hopDongThauDuocPhamChiTiet.TableNoTracking
                        .Where(x => x.DuocPhamId == s.DuocPhamId
                                    && x.SoLuongDaCap < x.SoLuong).Select(z => new TonChuaNhap
                                    {
                                        Name = z.HopDongThauDuocPham.NhaThau.Ten,
                                        TongTon = z.SoLuong - z.SoLuongDaCap
                                    }).ToList(),
                    TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.DuocPhamBenhVienId == s.DuocPhamId
                                    && (x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 ||
                                        x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                                    && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new TonChuaNhap
                                    {
                                        Name = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
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
            var query = _duTruMuaDpDetailed.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruDuocPhamTheoChiTietGridVo()).AsQueryable();
            var duTruMuaDpTheoKhoaDetailedIds = await _duTruMuaDpTheoKhoaChiTiet.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocChiTietId == id)
                .Select(e => e.Id).ToListAsync();
            foreach (var duTruMuaDpTheoKhoaDetailedIdItem in duTruMuaDpTheoKhoaDetailedIds)
            {
                var duTruMuaDpChiTiets = _duTruMuaDpDetailed.TableNoTracking
                    .Where(e => e.DuTruMuaDuocPhamTheoKhoaChiTietId == duTruMuaDpTheoKhoaDetailedIdItem &&
                                e.LaDuocPhamBHYT == laBhyt)
                    .Select(e => new DuTruDuocPhamTheoChiTietGridVo
                    {
                        Id = e.Id,
                        Nhom = e.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                        Kho = e.DuTruMuaDuocPham.Kho.Ten,
                        KhoaId = e.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoa.KhoaPhongId,
                        Khoa = e.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoa.KhoaPhong.Ten,
                        KyDuTruDisplay = e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                         " - " +
                                         e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                        SoLuongDuTru = e.SoLuongDuTru,
                        SoLuongDuKienTrongKy = e.SoLuongDuKienSuDung,
                        NhomDieuTri = e.NhomDieuTriDuPhong != null
                            ? e.NhomDieuTriDuPhong.GetValueOrDefault().GetDescription()
                            : string.Empty
                    });
                query = query.Concat(duTruMuaDpChiTiets);
            }

            var duTruMuaDpChiTietTuKhoTongs = _duTruMuaDpDetailed.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocChiTietId == id &&
                            e.LaDuocPhamBHYT == laBhyt)
                .Select(e => new DuTruDuocPhamTheoChiTietGridVo
                {
                    Id = e.Id,
                    Nhom = e.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                    Kho = e.DuTruMuaDuocPham.Kho.Ten,
                    KhoaId = 57,
                    Khoa = "Khoa Dược",
                    KyDuTruDisplay = e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                     " - " +
                                     e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                    SoLuongDuTru = e.SoLuongDuTru,
                    SoLuongDuKienTrongKy = e.SoLuongDuKienSuDung,
                    NhomDieuTri = e.NhomDieuTriDuPhong != null
                        ? e.NhomDieuTriDuPhong.GetValueOrDefault().GetDescription()
                        : string.Empty
                });
            query = query.Concat(duTruMuaDpChiTietTuKhoTongs);
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
            var duTruMuaDpKhoDuocEntity = await BaseRepository.Table
                .Include(q => q.DuTruMuaDuocPhamKhoDuocChiTiets)
                .FirstAsync(e => e.Id == id);
            duTruMuaDpKhoDuocEntity.GiamDocDuyet = true;
            duTruMuaDpKhoDuocEntity.GiamDocId = _userAgentHelper.GetCurrentUserId();
            duTruMuaDpKhoDuocEntity.NgayGiamDocDuyet = DateTime.Now;
            foreach (var duTruChiTiet in duTruMuaDpKhoDuocEntity.DuTruMuaDuocPhamKhoDuocChiTiets)
            {
                duTruChiTiet.SoLuongDuTruGiamDocDuyet = duTruChiTiet.SoLuongDuTru;
            }
            await BaseRepository.UpdateAsync(duTruMuaDpKhoDuocEntity);
        }

        public async Task ApproveForm(DuTruGiamDocApproveGridVo duTruGiamDoc)
        {
            var duTruChiTietDpIds = await _duTruMuaKhoDuocChiTiet.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocId == duTruGiamDoc.Id)
                .Select(e => e.Id).ToListAsync();
            var duTruMuaDpKhoDuocEntity = await BaseRepository.Table
                .Include(q => q.DuTruMuaDuocPhamKhoDuocChiTiets)
                .FirstAsync(e => e.Id == duTruGiamDoc.Id);


            foreach (var duTruChiTietId in duTruChiTietDpIds)
            {
                if (duTruGiamDoc.ChiTietDuocPhamList.Any(e => e.Id == duTruChiTietId))
                {
                    duTruMuaDpKhoDuocEntity.DuTruMuaDuocPhamKhoDuocChiTiets.First(q => q.Id == duTruChiTietId)
                        .SoLuongDuTruGiamDocDuyet = duTruGiamDoc.ChiTietDuocPhamList
                        .First(w => w.Id == duTruChiTietId).SoLuongDuyet;
                }
            }

            duTruMuaDpKhoDuocEntity.GiamDocDuyet = true;
            duTruMuaDpKhoDuocEntity.GiamDocId = _userAgentHelper.GetCurrentUserId();
            duTruMuaDpKhoDuocEntity.NgayGiamDocDuyet = DateTime.Now;
            await BaseRepository.UpdateAsync(duTruMuaDpKhoDuocEntity);
        }

        public async Task Decline(DuTruGiamDocApproveGridVo duTruGiamDoc)
        {
            var duTruMuaDpKhoDuocEntity = await BaseRepository.TableNoTracking.FirstAsync(e => e.Id == duTruGiamDoc.Id);
            duTruMuaDpKhoDuocEntity.GiamDocDuyet = false;
            duTruMuaDpKhoDuocEntity.GiamDocId = _userAgentHelper.GetCurrentUserId();
            duTruMuaDpKhoDuocEntity.NgayGiamDocDuyet = DateTime.Now;
            duTruMuaDpKhoDuocEntity.LyDoGiamDocTuChoi = duTruGiamDoc.LyDo;

            await BaseRepository.UpdateAsync(duTruMuaDpKhoDuocEntity);
        }
    }
}
