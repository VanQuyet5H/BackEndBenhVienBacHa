using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns;
using Camino.Core.Domain.ValueObject.DuyetTraVatTuTuBns;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.DuyetPhieuTraVatTuTuBns
{
    [ScopedDependency(ServiceType = typeof(IDuyetPhieuTraVatTuTuBnService))]
    public class DuyetPhieuTraVatTuTuBnService : MasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan>, IDuyetPhieuTraVatTuTuBnService
    {
        private readonly IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTietRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVtChiTietRepository;

        public DuyetPhieuTraVatTuTuBnService(IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan> repository,
            IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> yeuCauTraVatTuTuBenhNhanChiTietRepository,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVtChiTietRepository
        ) : base(repository)
        {
            _yeuCauTraVatTuTuBenhNhanChiTietRepository = yeuCauTraVatTuTuBenhNhanChiTietRepository;
            _nhapKhoVtChiTietRepository = nhapKhoVtChiTietRepository;
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
                .Select(s => new DuyetTraVatTuTuBnVo
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
                .Select(s => new DuyetTraVatTuTuBnVo
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

        public async Task<GridDataSource> GetDataForGridAsyncVatTuChild(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var yeuCauTraVatTuTuBenhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauTraVatTuTuBenhNhanId == yeuCauTraVatTuTuBenhNhanId)
                        .Select(s => new DuyetTraVatTuChiTietTuBnVo
                        {
                            YeuCauTraVatTuTuBenhNhanId = s.YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = s.VatTuBenhVienId,
                            VatTu = s.VatTuBenhVien.VatTus.Ten,
                            Dvt = s.VatTuBenhVien.VatTus.DonViTinh,
                            LaVatTuBhyt = s.LaVatTuBHYT,
                            TongSlTraLanNaySetting = s.SoLuongTra,
                            TongSlChiDinhSetting = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                            TongSlDaTraSetting = s.YeuCauVatTuBenhVien.SoLuongDaTra,
                            DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet
                        }).GroupBy(x => new { x.VatTuBenhVienId, x.VatTu, x.LaVatTuBhyt })
                        .Select(item => new DuyetTraVatTuChiTietTuBnVo
                        {
                            YeuCauTraVatTuTuBenhNhanId = item.First().YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaVatTuBhyt = item.First().LaVatTuBhyt,
                            VatTu = item.First().VatTu,
                            Dvt = item.First().Dvt,
                            DuocDuyet = item.First().DuocDuyet,
                            TongSlTraLanNaySetting = item.Sum(x => x.TongSlTraLanNaySetting),
                            TongSlChiDinhSetting = item.Sum(x => x.TongSlChiDinhSetting),
                            TongSlDaTraSetting = item.Sum(x => x.TongSlDaTraSetting)
                        })
                    .OrderBy(x => x.LaVatTuBhyt).ThenBy(x => x.VatTu).Distinct();

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
            var vatTuBenhVienId = long.Parse(queryObj[1]);
            var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraVatTuTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId && p.VatTuBenhVienId == vatTuBenhVienId)
                      .Select(s => new DuyetTraVatTuTuBenhNhanTheoChiTietBenhNhanVo
                      {
                          Id = s.Id,
                          NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NguoiTra = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauVatTuBenhVien.SoLuong,
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra.GetValueOrDefault(),
                          TiLeTheoThapGia = s.YeuCauVatTuBenhVien.TiLeTheoThapGia,
                          Vat = s.YeuCauVatTuBenhVien.VAT,
                          DonGiaNhap = s.YeuCauVatTuBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                          HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                          MaBn = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                          MaTn = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                          KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi,
                          DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet
                      }).GroupBy(x => new { x.HoTen, x.MaBn, x.MaTn })
                .Select(item => new DuyetTraVatTuTuBenhNhanTheoChiTietBenhNhanVo
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
                    DuocDuyet = item.First().DuocDuyet
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

        public async Task<double> GetSoLuongXuat(long vatTuBenhVienId, bool laVatTuBHYT, long? hopDongThauVatTuId, long khoId)
        {
            var soLuongDaXuat = await _nhapKhoVtChiTietRepository.TableNoTracking
                       .Where(p => p.VatTuBenhVienId == vatTuBenhVienId && p.LaVatTuBHYT == laVatTuBHYT && (hopDongThauVatTuId == null || p.HopDongThauVatTuId == hopDongThauVatTuId) && p.NhapKhoVatTu.KhoId == khoId)
                       .SumAsync(z => z.SoLuongDaXuat);
            return soLuongDaXuat;
        }
    }
}
