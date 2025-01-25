using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(ILichSuXacNhanBhytService))]
    public class LichSuXacNhanBhytService : MasterFileService<DuyetBaoHiem>, ILichSuXacNhanBhytService
    {
        private readonly IRepository<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTietRepository;

        public LichSuXacNhanBhytService
        (
            IRepository<DuyetBaoHiem> repository,
            IRepository<DuyetBaoHiemChiTiet> duyetBaoHiemChiTietRepository
        ) : base(repository)
        {
            _duyetBaoHiemChiTietRepository = duyetBaoHiemChiTietRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            //todo: need improve
            var query = BaseRepository.TableNoTracking
                .Select(source => new LichSuXacNhanBHYTGridVo
                {
                    Id = source.Id,
                    MaTN = source.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = source.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = source.YeuCauTiepNhan.HoTen,
                    NamSinh = DateHelper.DOBFormat(source.YeuCauTiepNhan.NgaySinh, source.YeuCauTiepNhan.ThangSinh, source.YeuCauTiepNhan.NamSinh),
                    TenGioiTinh = source.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    DiaChi = source.YeuCauTiepNhan.DiaChiDayDu,
                    SoDienThoai = source.YeuCauTiepNhan.SoDienThoai,
                    SoDienThoaiDisplay = source.YeuCauTiepNhan.SoDienThoaiDisplay,
                    ChiTietLichSuXacNhanBHYTs = source.DuyetBaoHiemChiTiets.Select(o => new ChiTietLichSuXacNhanBHYTVo { Soluong = o.SoLuong, DgThamKhao = o.DonGiaBaoHiem.GetValueOrDefault(), TiLeDv = o.TiLeBaoHiemThanhToan.GetValueOrDefault(), MucHuong = o.MucHuongBaoHiem.GetValueOrDefault() }),
                    ThoiDiemDuyet = source.ThoiDiemDuyetBaoHiem,
                    NhanVienDuyetBaoHiem = source.NhanVienDuyetBaoHiem.User.HoTen
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXacNhanBHYTAdditionalSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    query = query.Where(p => p.ThoiDiemDuyet >= tuNgay && p.ThoiDiemDuyet <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms.RemoveUniKeyAndToLower(),
                    g => g.HoTen.RemoveUniKeyAndToLower(),
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.NamSinh,
                    g => g.DiaChi,
                    g => g.SoDienThoaiDisplay,
                    g => g.SoDienThoai
                );
            }

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

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //todo: need improve
            var query = BaseRepository.TableNoTracking
                .Select(source => new LichSuXacNhanBHYTGridVo
                {
                    Id = source.Id,
                    MaTN = source.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = source.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = source.YeuCauTiepNhan.HoTen,
                    NamSinh = source.YeuCauTiepNhan != null ? source.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    TenGioiTinh = source.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    DiaChi = source.YeuCauTiepNhan.DiaChiDayDu,
                    SoDienThoai = source.YeuCauTiepNhan.SoDienThoai,
                    SoDienThoaiDisplay = source.YeuCauTiepNhan.SoDienThoaiDisplay,
                    ThoiDiemDuyet = source.ThoiDiemDuyetBaoHiem,
                    NhanVienDuyetBaoHiem = source.NhanVienDuyetBaoHiem.User.HoTen
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXacNhanBHYTAdditionalSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    query = query.Where(p => p.ThoiDiemDuyet >= tuNgay && p.ThoiDiemDuyet <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms.RemoveUniKeyAndToLower(),
                    g => g.HoTen.RemoveUniKeyAndToLower(),
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.NamSinh,
                    g => g.DiaChi,
                    g => g.SoDienThoaiDisplay,
                    g => g.SoDienThoai
                );
            }

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }

        public GridDataSource GetDataForGridXacNhanAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var count = 1;
            var countKhamBenh = 1;
            var countKyThuat = 1;
            var countDuocPham = 1;
            var countVatTu = 1;
            var countGiuongBenh = 1;
            var countThanhToan = 1;
            var listXacNhanBhyt = new List<LichSuXacNhanBhytChiTietGridVo>();

            var duyetBaoHiemChiTiets = _duyetBaoHiemChiTietRepository.TableNoTracking
                .Where(p => p.DuyetBaoHiemId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Include(p => p.DuyetBaoHiem).ThenInclude(p => p.YeuCauTiepNhan)

                .Include(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                .Include(p => p.YeuCauKhamBenh).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                .ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)

                .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .ThenInclude(p => p.DichVuKyThuat)
                .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)

                .Include(p => p.YeuCauDuocPhamBenhVien).ThenInclude(p => p.DuocPhamBenhVien)
                .ThenInclude(p => p.DuocPham)
                .Include(p => p.YeuCauDichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVien)
                .ThenInclude(p => p.DichVuGiuong)
                .Include(p => p.YeuCauDichVuGiuongBenhVien).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                .Include(p => p.YeuCauDichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVien)
                .ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                .Include(p => p.YeuCauDichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVien)
                .ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)

                .Include(p => p.DonThuocThanhToanChiTiet).ThenInclude(p => p.DuocPham)
                .ThenInclude(p => p.DuocPhamBenhVien)
                .Include(p => p.DonThuocThanhToanChiTiet).ThenInclude(p => p.DonThuocThanhToan)
                .ToList();

            // yêu cầu khám bệnh
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauKhamBenhId != null).Select(p => new LichSuXacNhanBhytChiTietGridVo
            {
                Id = count++,
                STT = countKhamBenh++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                MaDichVu = p.YeuCauKhamBenh?.DichVuKhamBenhBenhVien.Ma,
                TenDichVu = p.YeuCauKhamBenh?.DichVuKhamBenhBenhVien.Ten,
                LoaiGia = p.YeuCauKhamBenh?.NhomGiaDichVuKhamBenhBenhVien.Ten,
                SoLuong = p.SoLuong,
                DonGiaBenhVien = p.YeuCauKhamBenh?.Gia ?? 0,
                GiaBhytThamKhaoVo = p.YeuCauKhamBenh?.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems
                                            .Where(x => x.TuNgay <= p.CreatedOn && (x.DenNgay >= p.CreatedOn || x.DenNgay == null))
                                            .Select(o => new GiaBhytThamKhaoVo { TiLeThanhToan = o.TiLeBaoHiemThanhToan, Gia = o.Gia }).LastOrDefault(),
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // yêu cầu dịch vụ kỹ thuật
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauDichVuKyThuatId != null).Select(p => new LichSuXacNhanBhytChiTietGridVo
            {
                Id = count++,
                STT = countKyThuat++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                MaDichVu = p.YeuCauDichVuKyThuat?.DichVuKyThuatBenhVien.Ma,
                TenDichVu = p.YeuCauDichVuKyThuat?.DichVuKyThuatBenhVien.Ten,
                LoaiGia = p.YeuCauDichVuKyThuat?.NhomGiaDichVuKyThuatBenhVien.Ten,
                SoLuong = p.SoLuong,
                DonGiaBenhVien = p.YeuCauDichVuKyThuat?.Gia ?? 0,
                GiaBhytThamKhaoVo = p.YeuCauDichVuKyThuat?.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems
                    .Where(x => x.TuNgay <= p.CreatedOn && (x.DenNgay >= p.CreatedOn || x.DenNgay == null))
                    .Select(o => new GiaBhytThamKhaoVo { TiLeThanhToan = o.TiLeBaoHiemThanhToan, Gia = o.Gia }).LastOrDefault(),
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // yêu cầu dược phẩm
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(p => new LichSuXacNhanBhytChiTietGridVo
            {
                Id = count++,
                STT = countDuocPham++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DuocPham.GetDescription(),
                TenDichVu = p.YeuCauDuocPhamBenhVien?.DuocPhamBenhVien.DuocPham.Ten,
                SoLuong = p.SoLuong,
                DonGiaBenhVien = p.YeuCauDuocPhamBenhVien?.DonGiaBan ?? 0,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = p.TiLeBaoHiemThanhToan ?? 0, Gia = p.DonGiaBaoHiem ?? 0 },
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // yêu cầu vật tư
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauVatTuBenhVienId != null).Select(p => new LichSuXacNhanBhytChiTietGridVo
            {
                Id = count++,
                STT = countVatTu++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                TenDichVu = p.YeuCauVatTuBenhVien?.VatTuBenhVien?.VatTus?.Ten,
                SoLuong = p.SoLuong,
                DonGiaBenhVien = p.YeuCauVatTuBenhVien?.DonGiaBan ?? 0,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = p.TiLeBaoHiemThanhToan ?? 0, Gia = p.DonGiaBaoHiem ?? 0 },
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // yêu cầu giường
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauDichVuGiuongBenhVien != null).Select(p => new LichSuXacNhanBhytChiTietGridVo
            {
                Id = count++,
                STT = countGiuongBenh++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                MaDichVu = p.YeuCauDichVuGiuongBenhVien?.DichVuGiuongBenhVien.Ma,
                TenDichVu = p.YeuCauDichVuGiuongBenhVien?.DichVuGiuongBenhVien.Ten,
                LoaiGia = p.YeuCauDichVuGiuongBenhVien?.NhomGiaDichVuGiuongBenhVien?.Ten,
                SoLuong = p.SoLuong,
                DonGiaBenhVien = p.YeuCauDichVuGiuongBenhVien?.Gia ?? 0,
                GiaBhytThamKhaoVo = p.YeuCauDichVuGiuongBenhVien?.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems
                    .Where(x => x.TuNgay <= p.CreatedOn && (x.DenNgay >= p.CreatedOn || x.DenNgay == null))
                    .Select(o => new GiaBhytThamKhaoVo { TiLeThanhToan = o.TiLeBaoHiemThanhToan, Gia = o.Gia }).LastOrDefault(),
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // đơn thuốc thanh toán
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.DonThuocThanhToanChiTiet != null).Select(p => new LichSuXacNhanBhytChiTietGridVo
            {
                Id = count++,
                STT = countThanhToan++,
                IdDatabase = p.Id,
                IdDatabaseDonThuocThanhToan = p.DonThuocThanhToanChiTiet?.DonThuocThanhToan.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DonThuocThanhToan.GetDescription(),
                TenDichVu = p.DonThuocThanhToanChiTiet?.Ten,
                SoLuong = p.SoLuong,
                DonGiaBenhVien = p.DonThuocThanhToanChiTiet?.DonGiaBan,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = p.TiLeBaoHiemThanhToan ?? 0, Gia = p.DonGiaBaoHiem ?? 0 },
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            var queryIqueryable = listXacNhanBhyt.AsQueryable();
            var queryTask = queryIqueryable.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = 0 };
        }
    }
}
