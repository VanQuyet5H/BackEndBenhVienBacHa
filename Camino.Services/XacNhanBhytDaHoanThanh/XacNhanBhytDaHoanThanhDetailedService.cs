using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.XacNhanBhytDaHoanThanh
{
    [ScopedDependency(ServiceType = typeof(IXacNhanBhytDaHoanThanhDetailedService))]
    public class XacNhanBhytDaHoanThanhDetailedService : MasterFileService<YeuCauTiepNhan>, IXacNhanBhytDaHoanThanhDetailedService
    {
        private readonly IRepository<DuyetBaoHiem> _duyetBaoHiemRepository;
        private readonly IRepository<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTietRepository;

        public XacNhanBhytDaHoanThanhDetailedService
        (
            IRepository<YeuCauTiepNhan> repository,
            IRepository<DuyetBaoHiem> duyetBaoHiemRepository,
            IRepository<DuyetBaoHiemChiTiet> duyetBaoHiemChiTietRepository
        ) : base(repository)
        {
            _duyetBaoHiemRepository = duyetBaoHiemRepository;
            _duyetBaoHiemChiTietRepository = duyetBaoHiemChiTietRepository;
        }

        public async Task<GridDataSource> GetDataForGridDaXacNhanAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var count = 1;
            var countKhamBenh = 1;
            var countKyThuat = 1;
            var countDuocPham = 1;
            var countVatTu = 1;
            var countGiuongBenh = 1;
            var countThanhToan = 1;
            var listXacNhanBhyt = new List<ListDaXacNhanBhytGridVo>();

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking
                .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenh)
                .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)
                .Include(p => p.YeuCauVatTuBenhViens).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)                
                .Include(p => p.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(p => p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien)
                .Include(p => p.DonThuocThanhToans).ThenInclude(p => p.DonThuocThanhToanChiTiets).ThenInclude(p => p.DuocPham).ThenInclude(p => p.DuocPhamBenhVien)

                .Where(p => p.Id == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                            p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat).FirstOrDefaultAsync();

            // yêu cầu khám bệnh
            listXacNhanBhyt.AddRange(yeuCauTiepNhan?.YeuCauKhamBenhs.Select(p => new ListDaXacNhanBhytGridVo
            {
                Id = count++,
                Stt = countKhamBenh++,
                IdDatabase = p.Id,
                GroupType = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                MaDichVu = p.DichVuKhamBenhBenhVien?.Ma,
                TenDichVu = p.DichVuKhamBenhBenhVien?.Ten,
                LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                SoLuong = 1,
                DonGiaBenhVien = p.Gia,
                GiaBhyt = p.DonGiaBaoHiem,                
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                BaoHiemChiTra = p.BaoHiemChiTra,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                TrangThaiKhamBenh = p.TrangThai
            }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != null && p.TrangThaiKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            );

            // yêu cầu dịch vụ kỹ thuật
            listXacNhanBhyt.AddRange(yeuCauTiepNhan?.YeuCauDichVuKyThuats.Select(p => new ListDaXacNhanBhytGridVo
            {
                Id = count++,
                Stt = countKyThuat++,
                IdDatabase = p.Id,
                GroupType = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                MaDichVu = p.DichVuKyThuatBenhVien?.Ma,
                TenDichVu = p.DichVuKyThuatBenhVien?.Ten,
                LoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                SoLuong = p.SoLan,
                DonGiaBenhVien = p.Gia,
                GiaBhyt = p.DonGiaBaoHiem,                
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                BaoHiemChiTra = p.BaoHiemChiTra,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                TrangThaiDichVuKyThuat = p.TrangThai
            }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != null && p.TrangThaiDichVuKyThuat != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            );

            // yêu cầu dược phẩm
            listXacNhanBhyt.AddRange(yeuCauTiepNhan?.YeuCauDuocPhamBenhViens.Select(p => new ListDaXacNhanBhytGridVo
            {
                Id = count++,
                Stt = countDuocPham++,
                IdDatabase = p.Id,
                GroupType = Enums.EnumNhomGoiDichVu.DuocPham,
                TenDichVu = p.Ten,
                SoLuong = (decimal)p.SoLuong,
                DonGiaBenhVien = p.DonGiaBan,
                GiaBhyt = p.DonGiaBaoHiem,
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                BaoHiemChiTra = p.BaoHiemChiTra,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                TrangThaiDuocPhamBenhVien = p.TrangThai
            }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != null && p.TrangThaiDuocPhamBenhVien != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
            );

            listXacNhanBhyt.AddRange(yeuCauTiepNhan?.YeuCauVatTuBenhViens.Select(p => new ListDaXacNhanBhytGridVo
            {
                Id = count++,
                Stt = countVatTu++,
                IdDatabase = p.Id,
                GroupType = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                TenDichVu = p.Ten,
                SoLuong = (decimal)p.SoLuong,
                DonGiaBenhVien = p.DonGiaBan,
                GiaBhyt = p.DonGiaBaoHiem,
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                BaoHiemChiTra = p.BaoHiemChiTra,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                TrangThaiVatTuTieuHao = p.TrangThai
            }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != null && p.TrangThaiVatTuTieuHao != Enums.EnumYeuCauVatTuBenhVien.DaHuy));

            // yêu cầu giường
            listXacNhanBhyt.AddRange(yeuCauTiepNhan?.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Select(p => new ListDaXacNhanBhytGridVo
            {
                Id = count++,
                Stt = countGiuongBenh++,
                IdDatabase = p.Id,
                GroupType = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                MaDichVu = p.Ma,
                TenDichVu = p.Ten,
                //LoaiGia = p.NhomGiaDichVuGiuongBenhVien?.Ten,
                SoLuong = 1,
                DonGiaBenhVien = p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien?.Gia ?? 0,
                GiaBhyt = p.DonGiaBaoHiem,                
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                BaoHiemChiTra = p.BaoHiemChiTra,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                YeuCauGoiDichVuId = p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien?.YeuCauGoiDichVuId,
                TrangThaiGiuongBenh = Enums.EnumTrangThaiGiuongBenh.DaThucHien
            }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != null && p.TrangThaiGiuongBenh != Enums.EnumTrangThaiGiuongBenh.DaHuy)
            );

            // đơn thuốc thanh toán
            listXacNhanBhyt.AddRange(yeuCauTiepNhan?.DonThuocThanhToans?.SelectMany(s => s.DonThuocThanhToanChiTiets).Select(p => new ListDaXacNhanBhytGridVo
            {
                Id = count++,
                Stt = countThanhToan++,
                IdDatabase = p.Id,
                IdDatabaseDonThuocThanhToan = p.DonThuocThanhToan.Id,
                GroupType = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                TenDichVu = p.Ten,
                SoLuong = (decimal)p.SoLuong,
                DonGiaBenhVien = p.DonGiaBan,
                GiaBhyt = p.DonGiaBaoHiem,
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                BaoHiemChiTra = p.BaoHiemChiTra,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                TrangThaiDonThuocThanhToan = p.DonThuocThanhToan.TrangThai
            }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != null && p.TrangThaiDonThuocThanhToan != Enums.TrangThaiDonThuocThanhToan.DaHuy)
            );

            var queryIqueryable = listXacNhanBhyt.AsQueryable();
            var queryTask = queryIqueryable.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = queryTask.Length };
        }

        public async Task<GridDataSource> GetDataForDuyetBaoHiemAsync(QueryInfo queryInfo)
        {
            var query = _duyetBaoHiemRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(source => new DuyetBaoHiemHoanThanhGridVo
                {
                    Id = source.Id,
                    NguoiXacNhan = source.NhanVienDuyetBaoHiem.User.HoTen,
                    ThoiDiemDuyet = source.ThoiDiemDuyetBaoHiem
                });
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            await Task.WhenAll(queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = await countTask
            };
        }

        public GridDataSource GetDataForGridDuyetBaoHiemChiTietAsync(QueryInfo queryInfo)
        {
            var count = 1;
            var countKhamBenh = 1;
            var countKyThuat = 1;
            var countDuocPham = 1;
            var countVatTu = 1;
            var countGiuongBenh = 1;
            var countThanhToan = 1;
            var listXacNhanBhyt = new List<DuyetChiTietBaoHiemChiTietHoanThanhGridVo>();

            var duyetBaoHiemChiTiets = _duyetBaoHiemChiTietRepository.TableNoTracking
                .Where(p => p.DuyetBaoHiemId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Include(p => p.DuyetBaoHiem).ThenInclude(p => p.YeuCauTiepNhan)
                .Include(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                .Include(p => p.YeuCauKhamBenh).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .ThenInclude(p => p.DichVuKyThuat)
                .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                .Include(p => p.YeuCauDuocPhamBenhVien).ThenInclude(p => p.DuocPhamBenhVien)
                .ThenInclude(p => p.DuocPham)
                .Include(p => p.YeuCauDuocPhamBenhVien).ThenInclude(p => p.DuocPhamBenhVien)
                .Include(p => p.YeuCauDichVuGiuongBenhVienChiPhiBHYT).ThenInclude(p => p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien)
                .Include(p => p.DonThuocThanhToanChiTiet).ThenInclude(p => p.DuocPham)
                .ThenInclude(p => p.DuocPhamBenhVien)
                .Include(p => p.DonThuocThanhToanChiTiet).ThenInclude(p => p.DonThuocThanhToan)
                .ToList();

            // yêu cầu khám bệnh
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauKhamBenhId != null).Select(p => new DuyetChiTietBaoHiemChiTietHoanThanhGridVo
            {
                Id = count++,
                Stt = countKhamBenh++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                MaDichVu = p.YeuCauKhamBenh?.DichVuKhamBenhBenhVien.Ma,
                TenDichVu = p.YeuCauKhamBenh?.DichVuKhamBenhBenhVien.Ten,
                LoaiGia = p.YeuCauKhamBenh?.NhomGiaDichVuKhamBenhBenhVien.Ten,
                SoLuong = 1,
                DonGiaBenhVien = p.YeuCauKhamBenh?.Gia ?? 0,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = 100, Gia = p.YeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() },
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // yêu cầu dịch vụ kỹ thuật
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauDichVuKyThuatId != null).Select(p => new DuyetChiTietBaoHiemChiTietHoanThanhGridVo
            {
                Id = count++,
                Stt = countKyThuat++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                MaDichVu = p.YeuCauDichVuKyThuat?.DichVuKyThuatBenhVien.Ma,
                TenDichVu = p.YeuCauDichVuKyThuat?.DichVuKyThuatBenhVien.Ten,
                LoaiGia = p.YeuCauDichVuKyThuat?.NhomGiaDichVuKyThuatBenhVien.Ten,
                SoLuong = p.YeuCauDichVuKyThuat?.SoLan ?? 0,
                DonGiaBenhVien = p.YeuCauDichVuKyThuat?.Gia ?? 0,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = 100, Gia = p.YeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() },
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // yêu cầu dược phẩm
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(p => new DuyetChiTietBaoHiemChiTietHoanThanhGridVo
            {
                Id = count++,
                Stt = countDuocPham++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DuocPham.GetDescription(),
                TenDichVu = p.YeuCauDuocPhamBenhVien?.DuocPhamBenhVien.DuocPham.Ten,
                SoLuong = p.YeuCauDuocPhamBenhVien?.SoLuong ?? 0,
                DonGiaBenhVien = p.YeuCauDuocPhamBenhVien?.DonGiaBan ?? 0,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = 100, Gia = p.YeuCauDuocPhamBenhVien?.DonGiaBaoHiem ?? 0 },
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauVatTuBenhVienId != null).Select(p => new DuyetChiTietBaoHiemChiTietHoanThanhGridVo
            {
                Id = count++,
                Stt = countVatTu++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                TenDichVu = p.YeuCauVatTuBenhVien?.VatTuBenhVien?.VatTus.Ten,
                SoLuong = p.YeuCauVatTuBenhVien?.SoLuong ?? 0,
                DonGiaBenhVien = p.YeuCauVatTuBenhVien?.DonGiaBan ?? 0,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = 100, Gia = p.YeuCauDuocPhamBenhVien?.DonGiaBaoHiem ?? 0 },
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // yêu cầu giường
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYT != null).Select(p => new DuyetChiTietBaoHiemChiTietHoanThanhGridVo
            {
                Id = count++,
                Stt = countGiuongBenh++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                MaDichVu = p.YeuCauDichVuGiuongBenhVienChiPhiBHYT.Ma,
                TenDichVu = p.YeuCauDichVuGiuongBenhVienChiPhiBHYT.Ten,                
                SoLuong = p.SoLuong,
                DonGiaBenhVien = p.YeuCauDichVuGiuongBenhVienChiPhiBHYT.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien?.Gia,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = 100, Gia = p.YeuCauDichVuGiuongBenhVienChiPhiBHYT.DonGiaBaoHiem ?? 0},
                TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                MucHuong = p.MucHuongBaoHiem.GetValueOrDefault()
            }));

            // đơn thuốc thanh toán
            listXacNhanBhyt.AddRange(duyetBaoHiemChiTiets.Where(o => o.DonThuocThanhToanChiTiet != null).Select(p => new DuyetChiTietBaoHiemChiTietHoanThanhGridVo
            {
                Id = count++,
                Stt = countThanhToan++,
                IdDatabase = p.Id,
                IdDatabaseDonThuocThanhToan = p.DonThuocThanhToanChiTiet?.DonThuocThanhToan.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DonThuocThanhToan.GetDescription(),
                TenDichVu = p.DonThuocThanhToanChiTiet?.Ten,
                SoLuong = p.DonThuocThanhToanChiTiet?.SoLuong ?? 0,
                DonGiaBenhVien = p.DonThuocThanhToanChiTiet?.DonGiaBan ?? 0,
                GiaBhytThamKhaoVo = new GiaBhytThamKhaoVo { TiLeThanhToan = 100, Gia = p.DonThuocThanhToanChiTiet?.DonGiaBaoHiem ?? 0 },
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
