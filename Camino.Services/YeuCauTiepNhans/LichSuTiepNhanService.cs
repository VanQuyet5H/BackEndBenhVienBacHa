using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Globalization;

namespace Camino.Services.YeuCauTiepNhans
{
    [ScopedDependency(ServiceType = typeof(ILichSuTiepNhanService))]

    public class LichSuTiepNhanService : MasterFileService<YeuCauTiepNhan>, ILichSuTiepNhanService
    {
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        public LichSuTiepNhanService(IRepository<YeuCauTiepNhan> repository, IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository) : base(repository)
        {
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsyncLichSuTiepNhan(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = BaseRepository.TableNoTracking
                .Where(o => o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && o.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.MuaThuoc)
                .Select(s => new DanhSachChoKhamGridVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.MaYeuCauTiepNhan,
                    BenhNhanId = s.BenhNhanId,
                    MaBenhNhan = s.BenhNhan.MaBN,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh,
                    NgayThangNam = s.NgaySinh != null && s.NgaySinh != 0 && s.ThangSinh != null && s.ThangSinh != 0 && s.NamSinh != null ? new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1).ToString("dd/MM/yyyy") : s.NamSinh.ToString(),
                    DiaChi = s.DiaChiDayDu,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                    TrieuChungTiepNhan = s.TrieuChungTiepNhan,
                    BHYTMaSoThe = s.BHYTMaSoThe,
                    DoiTuong = s.CoBHYT != true ? "Viện phí" : "BHYT (" + s.BHYTMucHuong.ToString() + "%)",
                    TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                    TrangThaiYeuCauTiepNhanSearch = s.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat ? "Đã hoàn tất" : "Đã hủy",
                    CoBaoHiemTuNhan = s.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any() ? "Có" : "Không",
                    TenNhanVienTiepNhan = s.NhanVienTiepNhan.User.HoTen

                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanTu) || !string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanDen))
                {
                    var tuNgayTemp = DateTime.Now;
                    var denNgayTemp = DateTime.Now;
                    queryString.ThoiDiemTiepNhanTu.TryParseExactCustom(out tuNgayTemp);
                    //DateTime.TryParseExact(queryString.ThoiDiemTiepNhanTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgayTemp);
                    if (string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanDen))
                    {
                        denNgayTemp = DateTime.Now;
                    }
                    else
                    {
                        queryString.ThoiDiemTiepNhanDen.TryParseExactCustom(out denNgayTemp);
                        //DateTime.TryParseExact(queryString.ThoiDiemTiepNhanDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgayTemp);
                    }
                    denNgayTemp = denNgayTemp.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgayTemp && p.ThoiDiemTiepNhan <= denNgayTemp);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.MaYeuCauTiepNhan,
                        g => g.NamSinh.ToString(),
                        g => g.DiaChi,
                        g => g.MaBenhNhan,
                        g => g.TrieuChungTiepNhan,
                        g => g.TenNhanVienTiepNhan

                   );

                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                   g => g.HoTen,
                   g => g.MaYeuCauTiepNhan,
                   g => g.NamSinh.ToString(),
                   g => g.DiaChi,
                   g => g.MaBenhNhan,
                   g => g.TrieuChungTiepNhan,
                   g => g.TenNhanVienTiepNhan

                   );
            }
            //query = query.Where(o =>o.ThoiDiemTiepNhan.Value.Date == DateTime.Now.Date);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var queryResult = queryTask.Result;

            if (queryResult.Length > 0)
            {
                var yctnIds = queryResult.Select(o => o.Id).ToList();

                var yeuCauTiepNhanCoNhapVienIds = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(o => o.CoNhapVien == true && yctnIds.Contains(o.YeuCauTiepNhanId))
                    .Select(o => o.YeuCauTiepNhanId)
                    .ToList();

                foreach (var danhSachChoKham in queryResult)
                {
                    if (yeuCauTiepNhanCoNhapVienIds.Contains(danhSachChoKham.Id))
                    {
                        danhSachChoKham.CoYeuCauKhamBenhNhapVien = true;
                    }
                }
            }

            return new GridDataSource { Data = queryResult, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuTiepNhan(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(o => o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && o.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.MuaThuoc)
                .Select(s => new DanhSachChoKhamGridVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.MaYeuCauTiepNhan,
                    BenhNhanId = s.BenhNhanId,
                    MaBenhNhan = s.BenhNhan.MaBN,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh,
                    DiaChi = s.DiaChiDayDu,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                    TrieuChungTiepNhan = s.TrieuChungTiepNhan,
                    BHYTMaSoThe = s.BHYTMaSoThe,
                    DoiTuong = s.CoBHYT != true ? "Viện phí" : "BHYT (" + s.BHYTMucHuong.ToString() + "%)",
                    TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                    TrangThaiYeuCauTiepNhanSearch = s.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat ? "Đã hoàn tất" : "Đã hủy",
                    CoBaoHiemTuNhan = s.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any() ? "Có" : "Không",
                    TenNhanVienTiepNhan = s.NhanVienTiepNhan.User.HoTen
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanTu) || !string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanDen))
                {
                    var tuNgayTemp = DateTime.Now;
                    var denNgayTemp = DateTime.Now;
                    queryString.ThoiDiemTiepNhanTu.TryParseExactCustom(out tuNgayTemp);
                    //DateTime.TryParseExact(queryString.ThoiDiemTiepNhanTu, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgayTemp);
                    if (string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanDen))
                    {
                        denNgayTemp = DateTime.Now;
                    }
                    else
                    {
                        queryString.ThoiDiemTiepNhanDen.TryParseExactCustom(out denNgayTemp);
                        //DateTime.TryParseExact(queryString.ThoiDiemTiepNhanDen, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgayTemp);
                    }
                    denNgayTemp = denNgayTemp.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgayTemp && p.ThoiDiemTiepNhan <= denNgayTemp);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.MaYeuCauTiepNhan,
                        g => g.NamSinh.ToString(),
                        g => g.DiaChi,
                        g => g.MaBenhNhan,
                        g => g.TrieuChungTiepNhan,
                        g => g.TenNhanVienTiepNhan

                   );

                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                   g => g.HoTen,
                   g => g.MaYeuCauTiepNhan,
                   g => g.NamSinh.ToString(),
                   g => g.DiaChi,
                   g => g.TrieuChungTiepNhan,
                   g => g.MaBenhNhan,
                   g => g.TenNhanVienTiepNhan

                   );
            }
            //query = query.Where(o =>o.ThoiDiemTiepNhan.Value.Date == DateTime.Now.Date);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };

        }
    }
}
