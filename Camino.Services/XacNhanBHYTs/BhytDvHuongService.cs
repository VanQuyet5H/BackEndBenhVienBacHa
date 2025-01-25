using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBhytNoiTru;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IBhytDvHuongService))]
    public class BhytDvHuongService : MasterFileService<YeuCauTiepNhan>, IBhytDvHuongService
    {
        private readonly IRepository<DuyetBaoHiem> _duyetBaoHiemRepository;
        private readonly IRepository<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTietRepository;
        private readonly IRepository<YeuCauKhamBenhICDKhac> _yeuCauKhamBenhIcdKhacRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _ycDvKtRepository;
        private readonly IRepository<YeuCauDuocPhamBenhVien> _ycDpBvRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVien> _ycDvGiuongRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _ycDvGiuongBHYTRepository;
        private readonly IRepository<YeuCauTruyenMau> _yeuCauTruyenMauRepository;
        private readonly IRepository<DonThuocThanhToan> _dtThanhToanRepository;
        private readonly IRepository<YeuCauVatTuBenhVien> _ycvtBv;

        public BhytDvHuongService
        (
            IRepository<YeuCauTiepNhan> repository,
            IRepository<DuyetBaoHiem> duyetBaoHiemRepository,
            IRepository<DuyetBaoHiemChiTiet> duyetBaoHiemChiTietRepository,
            IRepository<YeuCauKhamBenhICDKhac> yeuCauKhamBenhIcdKhacRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<YeuCauDichVuKyThuat> ycDvKtRepository,
            IRepository<YeuCauDuocPhamBenhVien> ycDpBvRepository,
            IRepository<DonThuocThanhToan> dtThanhToanRepository,
            IRepository<YeuCauDichVuGiuongBenhVien> ycDvGiuongRepository,
            IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> ycDvGiuongBHYTRepository,
            IRepository<YeuCauTruyenMau> yeuCauTruyenMauRepository,
            IRepository<YeuCauVatTuBenhVien> ycvtBv
        ) : base(repository)
        {
            _duyetBaoHiemRepository = duyetBaoHiemRepository;
            _duyetBaoHiemChiTietRepository = duyetBaoHiemChiTietRepository;
            _yeuCauKhamBenhIcdKhacRepository = yeuCauKhamBenhIcdKhacRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _ycDvKtRepository = ycDvKtRepository;
            _ycDpBvRepository = ycDpBvRepository;
            _ycDvGiuongRepository = ycDvGiuongRepository;
            _dtThanhToanRepository = dtThanhToanRepository;
            _ycDvGiuongBHYTRepository = ycDvGiuongBHYTRepository;
            _yeuCauTruyenMauRepository = yeuCauTruyenMauRepository;
            _ycvtBv = ycvtBv;
        }

        public async Task<GridDataSource> GetDataForDvHuongBhytAsync(QueryInfo queryInfo)
        {
            var listBhHuongQuery = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachXacNhanBhytDaHuongGridVo
                {
                    Id = p.Id,
                    CheckedDefault = p.BaoHiemChiTra == null, 
                    GroupType = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                    MaDichVu = p.DichVuKhamBenhBenhVien.Ma,
                    DichVuId = p.DichVuKhamBenhBenhVienId,
                    TenDichVu = p.DichVuKhamBenhBenhVien.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    SoLuong = 1,
                    DonGiaBenhVien = p.Gia,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    IcdChinh = p.Icdchinh.TenTiengViet,
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    GhiChuIcdChinh = p.GhiChuICDChinh,
                    MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiKhamBenh = p.TrangThai,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Union(
                    _ycDvKtRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .OrderBy(p => p.Id)
                .Select(p => new DanhSachXacNhanBhytDaHuongGridVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                    CheckedDefault = p.BaoHiemChiTra == null,
                    GroupType = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    DichVuId = p.DichVuKyThuatBenhVienId,
                    MaDichVu = p.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    LoaiGia = p.NhomGiaDichVuKyThuatBenhVien.Ten,
                    SoLuong = p.SoLan,
                    DonGiaBenhVien = p.Gia,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    LoaiKt = p.LoaiDichVuKyThuat,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiDichVuKyThuat = p.TrangThai,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = p.DichVuKyThuatBenhVien.TiLeThanhToanBHYT
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiDichVuKyThuat != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                )
                .Union(
                    _ycDpBvRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachXacNhanBhytDaHuongGridVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                    CheckedDefault = p.BaoHiemChiTra == null,
                    GroupType = Enums.EnumNhomGoiDichVu.DuocPham,
                    DichVuId = p.DuocPhamBenhVien.DuocPham.Id,
                    TenDichVu = p.DuocPhamBenhVien.DuocPham.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    SoLuong = (decimal)p.SoLuong,
                    DonGiaBenhVien = p.DonGiaBan,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    TrangThaiDuocPhamBenhVien = p.TrangThai,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = p.DuocPhamBenhVien.TiLeThanhToanBHYT
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiDuocPhamBenhVien != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                )
                .Union(
                    _ycvtBv.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .OrderBy(o => o.Id)
                        .Select(p => new DanhSachXacNhanBhytDaHuongGridVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                            CheckedDefault = p.BaoHiemChiTra == null,
                            GroupType = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                            MaDichVu = p.VatTuBenhVien.Ma,
                            DichVuId = p.VatTuBenhVienId,
                            TenDichVu = p.VatTuBenhVien.VatTus.Ten,
                            NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                            NoiChiDinh = p.NoiChiDinh.Ten,
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            SoLuong = (decimal)p.SoLuong,
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            TrangThaiVatTuBenhVien = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                            //BVHD-3905
                            TiLeThanhToanBHYT = p.VatTuBenhVien.TiLeThanhToanBHYT
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiVatTuBenhVien != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                    )
                .Union(
                    _ycDvGiuongRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .OrderBy(w => w.Id)
                        .Select(p => new DanhSachXacNhanBhytDaHuongGridVo
                        {
                            Id = p.Id,
                            CheckedDefault = p.BaoHiemChiTra == null,
                            GroupType = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                            MaDichVu = p.DichVuGiuongBenhVien.Ma,
                            DichVuId = p.DichVuGiuongBenhVienId,
                            TenDichVu = p.DichVuGiuongBenhVien.Ten,
                            NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                            NoiChiDinh = p.NoiChiDinh.Ten,
                            LoaiGia = p.NhomGiaDichVuGiuongBenhVien.Ten,
                            SoLuong = 1,
                            DonGiaBenhVien = p.Gia.GetValueOrDefault(),
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            TrangThaiGiuongBenh = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                            //BVHD-3905
                            TiLeThanhToanBHYT = p.DichVuGiuongBenhVien.TiLeThanhToanBHYT
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiGiuongBenh != Enums.EnumTrangThaiGiuongBenh.DaHuy)
                )
                .Union(
                    _dtThanhToanRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .OrderBy(o => o.Id)
                        .SelectMany(w => w.DonThuocThanhToanChiTiets)
                        .Select(p => new DanhSachXacNhanBhytDaHuongGridVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.DonThuocThanhToan.YeuCauKhamBenhId,
                            CheckedDefault = p.BaoHiemChiTra == null,
                            GroupType = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                            IdDatabaseDonThuocThanhToan = p.DonThuocThanhToan.Id,
                            DichVuId = p.Id,
                            TenDichVu = p.Ten,
                            SoLuong = (decimal)p.SoLuong,
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            TrangThaiThanhToan = p.DonThuocThanhToan.TrangThaiThanhToan,
                            TrangThaiDonThuocThanhToan = p.DonThuocThanhToan.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                            //BVHD-3905
                            TiLeThanhToanBHYT = p.DuocPham.DuocPhamBenhVien.TiLeThanhToanBHYT
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiDonThuocThanhToan != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                );

            var listXacNhanBhyt = listBhHuongQuery.Skip(queryInfo.Skip).ToArrayAsync();

            await Task.WhenAll(listXacNhanBhyt);

            foreach (var khamBenhModifiedItem in listXacNhanBhyt.Result.Where(x => x.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
            {
                var subIcds = await GetListIcdsByIdKhamBenh(khamBenhModifiedItem.Id);

                if (subIcds.Any())
                {
                    khamBenhModifiedItem.IcdKemTheos = new List<IcdKemTheoVo>();
                    khamBenhModifiedItem.IcdKemTheos.AddRange(subIcds);
                }
            }

            return new GridDataSource { Data = listXacNhanBhyt.Result, TotalRowCount = listXacNhanBhyt.Result.Length };
        }

        public async Task<GridDataSource> GetDataForDvHuongBhytNoiTruAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId, o => o.Include(x => x.YeuCauTiepNhanTheBHYTs));
            var listBhytQuery = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                            p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachHuongBhytNoiTruVo
                {
                    Id = p.Id,
                    CheckedDefault = p.BaoHiemChiTra == null,
                    GroupType = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                    Khoa = p.NoiThucHien != null ? p.NoiThucHien.KhoaPhong.Ten : p.NoiChiDinh.KhoaPhong.Ten,
                    NgayPhatSinh = p.ThoiDiemChiDinh,
                    MaDichVu = p.DichVuKhamBenhBenhVien.Ma,
                    DichVuId = p.DichVuKhamBenhBenhVienId,
                    TenDichVu = p.DichVuKhamBenhBenhVien.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    SoLuong = 1,
                    IcdChinh = p.Icdchinh.TenTiengViet,
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    MucHuongSystem = p.MucHuongBaoHiem.GetValueOrDefault(),
                    TheBHYTId = p.YeuCauTiepNhanTheBHYTId,
                    MaSoTheBHYT = p.MaSoTheBHYT,
                    DonGiaBenhVien = p.Gia,
                    GhiChuIcdChinh = p.GhiChuICDChinh,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    TrangThaiKhamBenh = p.TrangThai,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false
                              && p.TrangThaiKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Union(
                    _ycDvKtRepository.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                                    p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachHuongBhytNoiTruVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                    CheckedDefault = p.BaoHiemChiTra == null,
                    GroupType = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    NgayPhatSinh = p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                    Khoa = p.NoiThucHien != null ? p.NoiThucHien.KhoaPhong.Ten : p.NoiChiDinh.KhoaPhong.Ten,
                    MaDichVu = p.DichVuKyThuatBenhVien.Ma,
                    DichVuId = p.DichVuKyThuatBenhVienId,
                    TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    LoaiGia = p.NhomGiaDichVuKyThuatBenhVien.Ten,
                    SoLuong = p.SoLan,
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    MucHuongSystem = p.MucHuongBaoHiem.GetValueOrDefault(),
                    TheBHYTId = p.YeuCauTiepNhanTheBHYTId,
                    MaSoTheBHYT = p.MaSoTheBHYT,
                    DonGiaBenhVien = p.Gia,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    LoaiKt = p.LoaiDichVuKyThuat,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiDichVuKyThuat = p.TrangThai,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiDichVuKyThuat != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                )
                .Union(
                    _yeuCauTruyenMauRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId)
                        .OrderBy(w => w.Id)
                        .Select(p => new DanhSachHuongBhytNoiTruVo
                        {
                            Id = p.Id,
                            CheckedDefault = p.BaoHiemChiTra == null,
                            GroupType = Enums.EnumNhomGoiDichVu.TruyenMau,
                            NgayPhatSinh = p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                            Khoa = p.NoiThucHien != null ? p.NoiThucHien.KhoaPhong.Ten : p.NoiChiDinh.KhoaPhong.Ten,
                            MaDichVu = p.MaDichVu,
                            DichVuId = p.MauVaChePhamId,
                            TenDichVu = p.TenDichVu,
                            NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                            NoiChiDinh = p.NoiChiDinh.Ten,
                            LoaiGia = string.Empty,
                            SoLuong = 1,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            MucHuongSystem = p.MucHuongBaoHiem.GetValueOrDefault(),
                            TheBHYTId = p.YeuCauTiepNhanTheBHYTId,
                            MaSoTheBHYT = p.MaSoTheBHYT,
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            TrangThaiYeuCauTruyenMau = p.TrangThai,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiYeuCauTruyenMau != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy)
                )
                .Union(
                    _ycDpBvRepository.TableNoTracking
                .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                            p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachHuongBhytNoiTruVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                    CheckedDefault = p.BaoHiemChiTra == null,
                    GroupType = Enums.EnumNhomGoiDichVu.DuocPham,
                    NgayPhatSinh = p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                    Khoa = p.NoiCapThuoc != null ? p.NoiCapThuoc.KhoaPhong.Ten : p.NoiChiDinh.KhoaPhong.Ten,
                    DichVuId = p.DuocPhamBenhVien.DuocPham.Id,
                    TenDichVu = p.DuocPhamBenhVien.DuocPham.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    SoLuong = (decimal)p.SoLuong,
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    MucHuongSystem = p.MucHuongBaoHiem.GetValueOrDefault(),
                    TheBHYTId = p.YeuCauTiepNhanTheBHYTId,
                    MaSoTheBHYT = p.MaSoTheBHYT,
                    DonGiaBenhVien = p.DonGiaBan,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiDuocPhamBenhVien = p.TrangThai,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiDuocPhamBenhVien != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                )
                .Union(
                    _ycvtBv.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                                    p.KhongTinhPhi != true)
                        .OrderBy(o => o.Id)
                        .Select(p => new DanhSachHuongBhytNoiTruVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                            CheckedDefault = p.BaoHiemChiTra == null,
                            GroupType = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                            NgayPhatSinh = p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                            Khoa = p.NoiCapVatTu != null ? p.NoiCapVatTu.KhoaPhong.Ten : p.NoiChiDinh.KhoaPhong.Ten,
                            MaDichVu = p.VatTuBenhVien.Ma,
                            DichVuId = p.VatTuBenhVienId,
                            TenDichVu = p.VatTuBenhVien.VatTus.Ten,
                            NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                            NoiChiDinh = p.NoiChiDinh.Ten,
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            SoLuong = (decimal)p.SoLuong,
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            MucHuongSystem = p.MucHuongBaoHiem.GetValueOrDefault(),
                            TheBHYTId = p.YeuCauTiepNhanTheBHYTId,
                            MaSoTheBHYT = p.MaSoTheBHYT,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            TrangThaiVatTuBenhVien = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiVatTuBenhVien != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                )
                .Union(
                    _ycDvGiuongBHYTRepository.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) && p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null)
                        .OrderBy(w => w.Id)
                        .Select(p => new DanhSachHuongBhytNoiTruVo
                        {
                            Id = p.Id,
                            CheckedDefault = p.BaoHiemChiTra == null,
                            GroupType = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                            NgayPhatSinh = p.NgayPhatSinh,
                            Khoa = p.KhoaPhong.Ten,
                            MaDichVu = p.DichVuGiuongBenhVien.Ma,
                            DichVuId = p.DichVuGiuongBenhVienId,
                            TenDichVu = p.DichVuGiuongBenhVien.Ten,
                            LoaiGia = string.Empty,
                            //NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                            //NoiChiDinh = p.NoiChiDinh.Ten,
                            SoLuong = (decimal)p.SoLuong,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            MucHuongSystem = p.MucHuongBaoHiem.GetValueOrDefault(),
                            TheBHYTId = p.YeuCauTiepNhanTheBHYTId,
                            MaSoTheBHYT = p.MaSoTheBHYT,
                            DonGiaBenhVien = p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            TrangThaiThanhToan = p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien.TrangThaiThanhToan,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            YeuCauGoiDichVuId = null,
                            //TrangThaiGiuongBenh = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false)
                )
                .Union(
                    _dtThanhToanRepository.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId))
                        .OrderBy(o => o.Id)
                        .SelectMany(w => w.DonThuocThanhToanChiTiets)
                        .Select(p => new DanhSachHuongBhytNoiTruVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.DonThuocThanhToan.YeuCauKhamBenhId,
                            CheckedDefault = p.BaoHiemChiTra == null,
                            IdDatabaseDonThuocThanhToan = p.DonThuocThanhToan.Id,
                            Khoa = "Nhà thuốc",
                            NgayPhatSinh = p.CreatedOn.GetValueOrDefault(),
                            GroupType = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                            DichVuId = p.Id,
                            TenDichVu = p.Ten,
                            SoLuong = (decimal)p.SoLuong,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            MucHuongSystem = p.MucHuongBaoHiem.GetValueOrDefault(),
                            TheBHYTId = p.YeuCauTiepNhanTheBHYTId,
                            MaSoTheBHYT = p.MaSoTheBHYT,
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            TrangThaiThanhToan = p.DonThuocThanhToan.TrangThaiThanhToan,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            TrangThaiDonThuocThanhToan = p.DonThuocThanhToan.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThaiDonThuocThanhToan != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                );

            var listBhyt = listBhytQuery.ToArrayAsync();

            await Task.WhenAll(listBhyt);

            

            //set the BHYT
            if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            {
                foreach (var chiPhiKhamChuaBenhVo in listBhyt.Result.Where(o=>o.BaoHiemChiTra == null || o.TheBHYTId == null))
                {
                    var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs
                        .Where(o => o.NgayHieuLuc.Date <= chiPhiKhamChuaBenhVo.NgayPhatSinh.Date &&
                                    (o.NgayHetHan == null || o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date >= chiPhiKhamChuaBenhVo.NgayPhatSinh.Date))
                        .OrderByDescending(o => o.MucHuong).FirstOrDefault();
                    if (theBHYT != null)
                    {
                        if(chiPhiKhamChuaBenhVo.BaoHiemChiTra == null)
                        {
                            chiPhiKhamChuaBenhVo.MucHuongSystem = theBHYT.MucHuong;
                            chiPhiKhamChuaBenhVo.TiLeDv = chiPhiKhamChuaBenhVo.TiLeDv != 0 ? chiPhiKhamChuaBenhVo.TiLeDv : 100;
                        }                        
                        chiPhiKhamChuaBenhVo.MaSoTheBHYT = theBHYT.MaSoThe;
                        chiPhiKhamChuaBenhVo.TheBHYTId = theBHYT.Id;
                    }
                }
            }

            foreach (var khamBenhModifiedItem in listBhyt.Result.Where(x => x.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
            {
                var subIcds = await GetListIcdsByIdKhamBenh(khamBenhModifiedItem.Id);

                if (subIcds.Any())
                {
                    khamBenhModifiedItem.IcdKemTheos = new List<IcdKemTheoVo>();
                    khamBenhModifiedItem.IcdKemTheos.AddRange(subIcds);
                }
            }

            //BVHD-3754: Khi thẻ BHYT đã hết hạn không hiển thị tất cả các DVKT, thuốc... tại MH xét duyệt BHYT
            DateTime? ngayHetHanThe = null;
            if (yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.All(o => o.NgayHetHan != null) && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            {
                ngayHetHanThe = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Select(o => o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date).OrderBy(o => o).LastOrDefault();
            }
            var dataReturn = listBhyt.Result;
            if (ngayHetHanThe != null)
            {
                dataReturn = listBhyt.Result.Where(o => o.BaoHiemChiTra != null || o.NgayPhatSinh.Date <= ngayHetHanThe).ToArray();
            }

            return new GridDataSource { Data = dataReturn, TotalRowCount = dataReturn.Length };
        }

        private async Task<List<IcdKemTheoVo>> GetListIcdsByIdKhamBenh(long idKhamBenh)
        {
            var icdKemTheosQuery = _yeuCauKhamBenhIcdKhacRepository.TableNoTracking
                .Where(p => p.YeuCauKhamBenhId == idKhamBenh)
                .Select(s => new IcdKemTheoVo
                {
                    Icd = s.ICD.TenTiengViet,
                    GhiChu = s.GhiChu
                });
            var icdKemTheos = icdKemTheosQuery.ToListAsync();
            await Task.WhenAll(icdKemTheos);
            return icdKemTheos.Result;
        }

        public async Task<ActionResult<LichSuVo>> GetHistoryLog(LichSuXacNhanVo lichSuXacNhanVo)
        {
            var gridLichSu = new List<GridLichSu>();
            var count = 1;
            if (lichSuXacNhanVo.Group == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                var query = await _duyetBaoHiemChiTietRepository.TableNoTracking.Where(p => p.YeuCauKhamBenhId == lichSuXacNhanVo.Id)
                    .Select(p => new DanhSachLichSuXacNhanGridVo
                    {
                        SoLuong = p.SoLuong,
                        DgBh = p.DonGiaBaoHiem.GetValueOrDefault(),
                        TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                        DuyetBaoHiemId = p.DuyetBaoHiemId
                    }).ToListAsync();

                foreach (var item in query)
                {
                    var duyetBaoHiem = _duyetBaoHiemRepository.TableNoTracking
                        .Include(p => p.NhanVienDuyetBaoHiem).ThenInclude(p => p.User)
                        .Where(p => p.Id == item.DuyetBaoHiemId)
                        .Select(p => new DuyetBaoHiemXacNhanGridVo
                        {
                            NgayDuyet = p.ThoiDiemDuyetBaoHiem.ApplyFormatDateTimeSACH(),
                            NhanVien = p.NhanVienDuyetBaoHiem.User.HoTen
                        }).FirstOrDefault();
                    gridLichSu.Add(new GridLichSu
                    {
                        Id = count++,
                        NgayDuyet = duyetBaoHiem?.NgayDuyet,
                        TenNhanVien = duyetBaoHiem?.NhanVien,
                        SoLuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).SoLuong,
                        MucHuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).MucHuong,
                        TiLeDv = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).TiLeDv,
                        DgBh = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).DgBh
                    });
                }
            }

            if (lichSuXacNhanVo.Group == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
            {
                var query = await _duyetBaoHiemChiTietRepository.TableNoTracking.Where(p => p.YeuCauDichVuKyThuatId == lichSuXacNhanVo.Id)
                    .Select(p => new DanhSachLichSuXacNhanGridVo
                    {
                        SoLuong = p.SoLuong,
                        DgBh = p.DonGiaBaoHiem.GetValueOrDefault(),
                        TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                        DuyetBaoHiemId = p.DuyetBaoHiemId
                    }).ToListAsync();

                foreach (var item in query)
                {
                    var duyetBaoHiem = _duyetBaoHiemRepository.TableNoTracking
                        .Include(p => p.NhanVienDuyetBaoHiem).ThenInclude(p => p.User)
                        .Where(p => p.Id == item.DuyetBaoHiemId)
                        .Select(p => new DuyetBaoHiemXacNhanGridVo
                        {
                            NgayDuyet = p.ThoiDiemDuyetBaoHiem.ApplyFormatDateTimeSACH(),
                            NhanVien = p.NhanVienDuyetBaoHiem.User.HoTen
                        }).FirstOrDefault();
                    gridLichSu.Add(new GridLichSu
                    {
                        Id = count++,
                        NgayDuyet = duyetBaoHiem?.NgayDuyet,
                        TenNhanVien = duyetBaoHiem?.NhanVien,
                        SoLuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).SoLuong,
                        MucHuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).MucHuong,
                        TiLeDv = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).TiLeDv,
                        DgBh = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).DgBh
                    });
                }
            }

            if (lichSuXacNhanVo.Group == Enums.EnumNhomGoiDichVu.DuocPham)
            {
                var query = await _duyetBaoHiemChiTietRepository.TableNoTracking.Where(p => p.YeuCauDuocPhamBenhVienId == lichSuXacNhanVo.Id)
                    .Select(p => new DanhSachLichSuXacNhanGridVo
                    {
                        SoLuong = p.SoLuong,
                        DgBh = p.DonGiaBaoHiem.GetValueOrDefault(),
                        TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                        DuyetBaoHiemId = p.DuyetBaoHiemId
                    }).ToListAsync();

                foreach (var item in query)
                {
                    var duyetBaoHiem = _duyetBaoHiemRepository.TableNoTracking
                        .Include(p => p.NhanVienDuyetBaoHiem).ThenInclude(p => p.User)
                        .Where(p => p.Id == item.DuyetBaoHiemId)
                        .Select(p => new DuyetBaoHiemXacNhanGridVo
                        {
                            NgayDuyet = p.ThoiDiemDuyetBaoHiem.ApplyFormatDateTimeSACH(),
                            NhanVien = p.NhanVienDuyetBaoHiem.User.HoTen
                        }).FirstOrDefault();
                    gridLichSu.Add(new GridLichSu
                    {
                        Id = count++,
                        NgayDuyet = duyetBaoHiem?.NgayDuyet,
                        TenNhanVien = duyetBaoHiem?.NhanVien,
                        SoLuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).SoLuong,
                        MucHuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).MucHuong,
                        TiLeDv = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).TiLeDv,
                        DgBh = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).DgBh
                    });
                }
            }

            if (lichSuXacNhanVo.Group == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
            {
                var query = await _duyetBaoHiemChiTietRepository.TableNoTracking.Where(p => p.YeuCauDichVuGiuongBenhVienChiPhiBHYTId == lichSuXacNhanVo.Id)
                    .Select(p => new DanhSachLichSuXacNhanGridVo
                    {
                        SoLuong = p.SoLuong,
                        DgBh = p.DonGiaBaoHiem.GetValueOrDefault(),
                        TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                        DuyetBaoHiemId = p.DuyetBaoHiemId
                    }).ToListAsync();

                foreach (var item in query)
                {
                    var duyetBaoHiem = _duyetBaoHiemRepository.TableNoTracking
                        .Include(p => p.NhanVienDuyetBaoHiem).ThenInclude(p => p.User)
                        .Where(p => p.Id == item.DuyetBaoHiemId)
                        .Select(p => new DuyetBaoHiemXacNhanGridVo
                        {
                            NgayDuyet = p.ThoiDiemDuyetBaoHiem.ApplyFormatDateTimeSACH(),
                            NhanVien = p.NhanVienDuyetBaoHiem.User.HoTen
                        }).FirstOrDefault();
                    gridLichSu.Add(new GridLichSu
                    {
                        Id = count++,
                        NgayDuyet = duyetBaoHiem?.NgayDuyet,
                        TenNhanVien = duyetBaoHiem?.NhanVien,
                        SoLuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).SoLuong,
                        MucHuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).MucHuong,
                        TiLeDv = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).TiLeDv,
                        DgBh = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).DgBh
                    });
                }
            }

            if (lichSuXacNhanVo.Group == Enums.EnumNhomGoiDichVu.DonThuocThanhToan)
            {
                var query = await _duyetBaoHiemChiTietRepository.TableNoTracking.Where(p => p.DonThuocThanhToanChiTietId == lichSuXacNhanVo.Id)
                    .Select(p => new DanhSachLichSuXacNhanGridVo
                    {
                        SoLuong = p.SoLuong,
                        DgBh = p.DonGiaBaoHiem.GetValueOrDefault(),
                        TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = p.MucHuongBaoHiem.GetValueOrDefault(),
                        DuyetBaoHiemId = p.DuyetBaoHiemId
                    }).ToListAsync();

                foreach (var item in query)
                {
                    var duyetBaoHiem = _duyetBaoHiemRepository.TableNoTracking
                        .Include(p => p.NhanVienDuyetBaoHiem).ThenInclude(p => p.User)
                        .Where(p => p.Id == item.DuyetBaoHiemId)
                        .Select(p => new DuyetBaoHiemXacNhanGridVo
                        {
                            NgayDuyet = p.ThoiDiemDuyetBaoHiem.ApplyFormatDateTimeSACH(),
                            NhanVien = p.NhanVienDuyetBaoHiem.User.HoTen
                        }).FirstOrDefault();
                    gridLichSu.Add(new GridLichSu
                    {
                        Id = count++,
                        NgayDuyet = duyetBaoHiem?.NgayDuyet,
                        TenNhanVien = duyetBaoHiem?.NhanVien,
                        SoLuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).SoLuong,
                        MucHuong = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).MucHuong,
                        TiLeDv = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).TiLeDv,
                        DgBh = query.First(x => x.DuyetBaoHiemId == item.DuyetBaoHiemId).DgBh
                    });
                }
            }

            var lichSu = new LichSuVo
            {
                Value = gridLichSu
            };

            return lichSu;
        }
    }
}
