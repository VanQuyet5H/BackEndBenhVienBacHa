using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.DuyetPhieuTraThuocTuBns
{
    [ScopedDependency(ServiceType = typeof(IDuyetPhieuTraThuocTuBnService))]
    public class DuyetPhieuTraThuocTuBnService : MasterFileService<YeuCauTraDuocPhamTuBenhNhan>, IDuyetPhieuTraThuocTuBnService
    {
        private readonly IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDpChiTietRepository;

        public DuyetPhieuTraThuocTuBnService(IRepository<YeuCauTraDuocPhamTuBenhNhan> repository,
            IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> yeuCauTraDuocPhamTuBenhNhanChiTietRepository,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDpChiTietRepository
            ) : base(repository)
        {
            _yeuCauTraDuocPhamTuBenhNhanChiTietRepository = yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
            _nhapKhoDpChiTietRepository = nhapKhoDpChiTietRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Select(s => new DuyetTraThuocTuBnVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    KhoaHoanTraId = s.KhoaHoanTraId,
                    KhoaHoanTraDisplay = s.KhoaHoanTra.Ten,
                    HoanTraVeKhoId = s.KhoTraId,
                    HoanTraVeKhoDisplay = s.KhoTra.Ten,
                    NguoiYeuCauId = s.NhanVienYeuCauId,
                    NguoiYeuCauDisplay = s.NhanVienYeuCau.User.HoTen,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = s.DuocDuyet,
                    NguoiDuyetId = s.NhanVienDuyetId,
                    NguoiDuyetDisplay = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryObject = JsonConvert.DeserializeObject<CheckApproveVo>(queryInfo.AdditionalSearchString);
                if (queryObject.DangChoDuyet == false && queryObject.DaDuyet)
                {
                    query = query.Where(p => p.TinhTrang == true);
                }
                else if (queryObject.DangChoDuyet && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang != true);
                }

                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.startDate != null)
                {
                    var tuNgay = queryObject.RangeYeuCau.startDate.GetValueOrDefault().Date;

                    query = query.Where(p => tuNgay <= p.NgayYeuCau.GetValueOrDefault().Date);
                }
                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.endDate != null)
                {
                    var denNgay = queryObject.RangeYeuCau.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => denNgay >= p.NgayYeuCau.GetValueOrDefault().Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    query = query.Where(p => tuNgay <= p.NgayDuyet.Value.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => denNgay >= p.NgayDuyet.Value.Date);
                }

                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoaHoanTraDisplay,
                         g => g.HoanTraVeKhoDisplay,
                         g => g.NguoiYeuCauDisplay,
                         g => g.NguoiDuyetDisplay
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.KhoaHoanTraDisplay,
                    g => g.HoanTraVeKhoDisplay,
                    g => g.NguoiYeuCauDisplay,
                    g => g.NguoiDuyetDisplay
                );
            }

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TinhTrang";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ThenByDescending(p => p.NgayYeuCau).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new DuyetTraThuocTuBnVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    KhoaHoanTraId = s.KhoaHoanTraId,
                    KhoaHoanTraDisplay = s.KhoaHoanTra.Ten,
                    HoanTraVeKhoId = s.KhoTraId,
                    HoanTraVeKhoDisplay = s.KhoTra.Ten,
                    NguoiYeuCauId = s.NhanVienYeuCauId,
                    NguoiYeuCauDisplay = s.NhanVienYeuCau.User.HoTen,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = s.DuocDuyet,
                    NguoiDuyetId = s.NhanVienDuyetId,
                    NguoiDuyetDisplay = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryObject = JsonConvert.DeserializeObject<CheckApproveVo>(queryInfo.AdditionalSearchString);
                if (queryObject.DangChoDuyet == false && queryObject.DaDuyet)
                {
                    query = query.Where(p => p.TinhTrang == true);
                }
                else if (queryObject.DangChoDuyet && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang != true);
                }

                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.startDate != null)
                {
                    var tuNgay = queryObject.RangeYeuCau.startDate.GetValueOrDefault().Date;

                    query = query.Where(p => tuNgay <= p.NgayYeuCau.GetValueOrDefault().Date);
                }
                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.endDate != null)
                {
                    var denNgay = queryObject.RangeYeuCau.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => denNgay >= p.NgayYeuCau.GetValueOrDefault().Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    query = query.Where(p => tuNgay <= p.NgayDuyet.Value.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => denNgay >= p.NgayDuyet.Value.Date);
                }

                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoaHoanTraDisplay,
                         g => g.HoanTraVeKhoDisplay,
                         g => g.NguoiYeuCauDisplay,
                         g => g.NguoiDuyetDisplay
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.KhoaHoanTraDisplay,
                    g => g.HoanTraVeKhoDisplay,
                    g => g.NguoiYeuCauDisplay,
                    g => g.NguoiDuyetDisplay
                );
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridAsyncDuocPhamChild(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var yeuCauTraDuocPhamTuBenhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId)
                        .Select(s => new DuyetTraThuocChiTietTuBnVo
                        {
                            YeuCauTraDuocPhamTuBenhNhanId = s.YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                            DuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                            HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                            LaDuocPhamBhyt = s.LaDuocPhamBHYT,
                            Dvt = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            TongSlTraLanNaySetting = s.SoLuongTra,
                            TongSlChiDinhSetting = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                            TongSlDaTraSetting = s.YeuCauDuocPhamBenhVien.SoLuongDaTra,
                            DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet
                        }).GroupBy(x => new { x.DuocPhamBenhVienId, x.DuocPham, x.LaDuocPhamBhyt, x.HoatChat })
                        .Select(item => new DuyetTraThuocChiTietTuBnVo
                        {
                            YeuCauTraDuocPhamTuBenhNhanId = item.First().YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaDuocPhamBhyt = item.First().LaDuocPhamBhyt,
                            DuocPham = item.First().DuocPham,
                            HoatChat = item.First().HoatChat,
                            Dvt = item.First().Dvt,
                            DuocDuyet = item.First().DuocDuyet,
                            TongSlTraLanNaySetting = item.Sum(x => x.TongSlTraLanNaySetting),
                            TongSlChiDinhSetting = item.Sum(x => x.TongSlChiDinhSetting),
                            TongSlDaTraSetting = item.Sum(x => x.TongSlDaTraSetting)
                        })
                    .OrderBy(x => x.LaDuocPhamBhyt).ThenBy(x => x.DuocPham).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
            var dataReturn = queryTask.Result;
            foreach (var dataItem in dataReturn)
            {
                if (dataItem.DuocDuyet != true)
                {
                    dataItem.TongSlDaTraSetting = dataItem.TongSlDaTraSetting - dataItem.TongSlTraLanNaySetting;
                }
            }
            //end update BVHD-3411

            return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridAsyncBenhNhanChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTraDuocPhamTuBenhNhanId = long.Parse(queryObj[0]);
            var duocPhamBenhVienId = long.Parse(queryObj[1]);
            var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId && p.DuocPhamBenhVienId == duocPhamBenhVienId)
                      .Select(s => new DuyetTraThuocTuBenhNhanTheoChiTietBenhNhanVo
                      {
                          Id = s.Id,
                          NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NguoiTra = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong,
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra.GetValueOrDefault(),
                          TiLeTheoThapGia = s.YeuCauDuocPhamBenhVien.TiLeTheoThapGia,
                          Vat = s.YeuCauDuocPhamBenhVien.VAT,
                          DonGiaNhap = s.YeuCauDuocPhamBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                          HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                          MaBn = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                          MaTn = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                          KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi,
                          DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet
                      }).GroupBy(x => new { x.HoTen, x.MaBn, x.MaTn })
                .Select(item => new DuyetTraThuocTuBenhNhanTheoChiTietBenhNhanVo
                {
                    Id = item.First().Id,
                    NgayDieuTri = item.First().NgayDieuTri,
                    NgayTra = item.First().NgayTra,
                    NguoiTra = item.First().NguoiTra,
                    SoLuongChiDinh = item.Sum(q => q.SoLuongChiDinh).GetValueOrDefault(),
                    SoLuongTraLanNay = item.First().SoLuongTraLanNay,
                    SoLuongDaTra = item.Sum(q => q.SoLuongDaTra).GetValueOrDefault(),
                    TiLeTheoThapGia = item.First().TiLeTheoThapGia,
                    Vat = item.First().Vat,
                    DonGiaNhap = item.First().DonGiaNhap,
                    DonGia = item.First().DonGia,
                    HoTen = item.First().HoTen,
                    MaBn = item.First().MaBn,
                    MaTn = item.First().MaTn,
                    KhongTinhPhi = item.First().KhongTinhPhi,
                    DuocDuyet = item.First().DuocDuyet,
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
            var dataReturn = queryTask.Result;
            foreach (var dataItem in dataReturn)
            {
                if (dataItem.DuocDuyet != true)
                {
                    dataItem.SoLuongDaTra = dataItem.SoLuongDaTra - dataItem.SoLuongTraLanNay;
                }
            }
            //end update BVHD-3411

            return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
        }

        public async Task<double> GetSoLuongXuat(long duocPhamBenhVienId, bool laDuocPhamBHYT, long? hopDongThauDuocPhamId, long khoId)
        {
            var soLuongDaXuat = await _nhapKhoDpChiTietRepository.TableNoTracking
                        .Where(p => p.DuocPhamBenhVienId == duocPhamBenhVienId && p.LaDuocPhamBHYT == laDuocPhamBHYT && (hopDongThauDuocPhamId == null || p.HopDongThauDuocPhamId == hopDongThauDuocPhamId) && p.NhapKhoDuocPhams.KhoId == khoId)
                        .SumAsync(z => z.SoLuongDaXuat);
            return soLuongDaXuat;
        }
    }
}
