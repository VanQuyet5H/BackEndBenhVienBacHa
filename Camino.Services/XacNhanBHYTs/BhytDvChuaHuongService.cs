using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBhytNoiTru;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IBhytDvChuaHuongService))]
    public class BhytDvChuaHuongService : MasterFileService<YeuCauTiepNhan>, IBhytDvChuaHuongService
    {
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _ycDvKtRepository;
        private readonly IRepository<YeuCauDuocPhamBenhVien> _ycDpBvRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVien> _ycDvGiuongRepository;
        private readonly IRepository<YeuCauKhamBenhICDKhac> _yeuCauKhamBenhIcdKhacRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _ycDvGiuongBHYTRepository;
        private readonly IRepository<YeuCauTruyenMau> _yeuCauTruyenMauRepository;
        private readonly IRepository<DonThuocThanhToan> _dtThanhToanRepository;
        private readonly IRepository<YeuCauVatTuBenhVien> _ycvtBv;

        public BhytDvChuaHuongService
        (
            IRepository<YeuCauTiepNhan> repository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<YeuCauDichVuKyThuat> ycDvKtRepository,
            IRepository<YeuCauDuocPhamBenhVien> ycDpBvRepository,
            IRepository<DonThuocThanhToan> dtThanhToanRepository,
            IRepository<YeuCauDichVuGiuongBenhVien> ycDvGiuongRepository,
            IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> ycDvGiuongBHYTRepository,
            IRepository<YeuCauTruyenMau> yeuCauTruyenMauRepository,
            IRepository<YeuCauVatTuBenhVien> ycvtBv,
            IRepository<YeuCauKhamBenhICDKhac> yeuCauKhamBenhIcdKhacRepository
        ) : base(repository)
        {
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _ycDvKtRepository = ycDvKtRepository;
            _ycDpBvRepository = ycDpBvRepository;
            _ycDvGiuongRepository = ycDvGiuongRepository;
            _dtThanhToanRepository = dtThanhToanRepository;
            _ycvtBv = ycvtBv;
            _ycDvGiuongBHYTRepository = ycDvGiuongBHYTRepository;
            _yeuCauTruyenMauRepository = yeuCauTruyenMauRepository;
            _yeuCauKhamBenhIcdKhacRepository = yeuCauKhamBenhIcdKhacRepository;
        }

        public async Task<GridDataSource> GetDataForDvChuaHuongBhytAsync(QueryInfo queryInfo)
        {
            var listBhKoTraQuery = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachXacNhanBhytChuaHuongGridVo
                {
                    Id = p.Id,
                    GroupType = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                    MaDichVu = p.DichVuKhamBenhBenhVien.Ma,
                    DichVuId = p.DichVuKhamBenhBenhVienId,
                    TenDichVu = p.DichVuKhamBenhBenhVien.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    SoLuong = 1,
                    IcdChinh = p.Icdchinh.TenTiengViet,
                    DonGiaBenhVien = p.Gia,
                    GhiChuIcdChinh = p.GhiChuICDChinh,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    TrangThaiKhamBenh = p.TrangThai,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false
                              && p.TrangThaiKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Union(
                    _ycDvKtRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                                    p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachXacNhanBhytChuaHuongGridVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                    GroupType = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    MaDichVu = p.DichVuKyThuatBenhVien.Ma,
                    DichVuId = p.DichVuKyThuatBenhVienId,
                    TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    LoaiGia = p.NhomGiaDichVuKyThuatBenhVien.Ten,
                    SoLuong = p.SoLan,
                    DonGiaBenhVien = p.Gia,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    LoaiKt = p.LoaiDichVuKyThuat,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiDichVuKyThuat = p.TrangThai,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = p.DichVuKyThuatBenhVien.TiLeThanhToanBHYT
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiDichVuKyThuat != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                )
                .Union(
                    _ycDpBvRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachXacNhanBhytChuaHuongGridVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                    GroupType = Enums.EnumNhomGoiDichVu.DuocPham,
                    DichVuId = p.DuocPhamBenhVien.DuocPham.Id,
                    TenDichVu = p.DuocPhamBenhVien.DuocPham.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    SoLuong = (decimal)p.SoLuong,
                    DonGiaBenhVien = p.DonGiaBan,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiDuocPhamBenhVien = p.TrangThai,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = p.DuocPhamBenhVien.TiLeThanhToanBHYT
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiDuocPhamBenhVien != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                )
                .Union(
                    _ycvtBv.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                                    p.KhongTinhPhi != true)
                        .OrderBy(o => o.Id)
                        .Select(p => new DanhSachXacNhanBhytDaHuongGridVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.YeuCauKhamBenhId,
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
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            TrangThaiVatTuBenhVien = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                            //BVHD-3905
                            TiLeThanhToanBHYT = p.VatTuBenhVien.TiLeThanhToanBHYT
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiVatTuBenhVien != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                )
                .Union(
                    _ycDvGiuongBHYTRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .OrderBy(w => w.Id)
                        .Select(p => new DanhSachXacNhanBhytChuaHuongGridVo
                        {
                            Id = p.Id,
                            GroupType = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                            MaDichVu = p.DichVuGiuongBenhVien.Ma,
                            DichVuId = p.DichVuGiuongBenhVienId,
                            TenDichVu = p.DichVuGiuongBenhVien.Ten,
                            //LoaiGia = p.NhomGiaDichVuGiuongBenhVien.Ten,
                            //NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                            //NoiChiDinh = p.NoiChiDinh.Ten,
                            SoLuong = (decimal)p.SoLuong,
                            //DonGiaBenhVien = p.Gia.GetValueOrDefault(),
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            //YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            //TrangThaiGiuongBenh = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                            //BVHD-3905
                            TiLeThanhToanBHYT = p.DichVuGiuongBenhVien.TiLeThanhToanBHYT
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false)
                )
                .Union(
                    _dtThanhToanRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString) &&
                                    p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .OrderBy(o => o.Id)
                        .SelectMany(w => w.DonThuocThanhToanChiTiets)
                        .Select(p => new DanhSachXacNhanBhytChuaHuongGridVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.DonThuocThanhToan.YeuCauKhamBenhId,
                            IdDatabaseDonThuocThanhToan = p.DonThuocThanhToan.Id,
                            GroupType = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                            DichVuId = p.Id,
                            TenDichVu = p.Ten,
                            SoLuong = (decimal)p.SoLuong,
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            TrangThaiThanhToan = p.DonThuocThanhToan.TrangThaiThanhToan,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            TrangThaiDonThuocThanhToan = p.DonThuocThanhToan.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any(),

                            //BVHD-3905
                            TiLeThanhToanBHYT = p.DuocPham.DuocPhamBenhVien.TiLeThanhToanBHYT
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiDonThuocThanhToan != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                );

            var listBhKoTt = listBhKoTraQuery.Skip(queryInfo.Skip).ToArrayAsync();

            await Task.WhenAll(listBhKoTt);

            foreach (var khamBenhModifiedItem in listBhKoTt.Result.Where(x => x.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
            {
                var subIcds = await GetListIcdsByIdKhamBenh(khamBenhModifiedItem.Id);

                if (subIcds.Any())
                {
                    khamBenhModifiedItem.IcdKemTheos = new List<IcdKemTheoVo>();
                    khamBenhModifiedItem.IcdKemTheos.AddRange(subIcds);
                }
            }

            return new GridDataSource { Data = listBhKoTt.Result, TotalRowCount = listBhKoTt.Result.Length };
        }

        public async Task<GridDataSource> GetDataForDvChuaHuongBhytNoiTruAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId, o => o.Include(x => x.YeuCauTiepNhanTheBHYTs));

            var listBhKoTraQuery = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                            p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachChuaHuongBhytNoiTruVo
                {
                    Id = p.Id,
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
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    IcdChinh = p.Icdchinh.TenTiengViet,
                    DonGiaBenhVien = p.Gia,
                    GhiChuIcdChinh = p.GhiChuICDChinh,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    TrangThaiKhamBenh = p.TrangThai,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false
                              && p.TrangThaiKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Union(
                    _ycDvKtRepository.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                                    p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachChuaHuongBhytNoiTruVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
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
                    DonGiaBenhVien = p.Gia,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    LoaiKt = p.LoaiDichVuKyThuat,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiDichVuKyThuat = p.TrangThai,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiDichVuKyThuat != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                )
                .Union(
                    _yeuCauTruyenMauRepository.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId))
                        .OrderBy(w => w.Id)
                        .Select(p => new DanhSachChuaHuongBhytNoiTruVo
                        {
                            Id = p.Id,
                            GroupType = Enums.EnumNhomGoiDichVu.TruyenMau,
                            MaDichVu = p.MaDichVu,
                            DichVuId = p.MauVaChePhamId,
                            TenDichVu = p.TenDichVu,
                            NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                            NoiChiDinh = p.NoiChiDinh.Ten,
                            LoaiGia = string.Empty,
                            SoLuong = 1,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            TrangThaiYeuCauTruyenMau = p.TrangThai,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiYeuCauTruyenMau != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy)
                )
                .Union(
                    _ycDpBvRepository.TableNoTracking
                .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                            p.KhongTinhPhi != true)
                .OrderBy(w => w.Id)
                .Select(p => new DanhSachChuaHuongBhytNoiTruVo
                {
                    Id = p.Id,
                    YeuCauKhamBenhId = p.YeuCauKhamBenhId,
                    GroupType = Enums.EnumNhomGoiDichVu.DuocPham,
                    NgayPhatSinh = p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                    Khoa = p.NoiCapThuoc != null ? p.NoiCapThuoc.KhoaPhong.Ten : p.NoiChiDinh.KhoaPhong.Ten,
                    DichVuId = p.DuocPhamBenhVien.DuocPham.Id,
                    TenDichVu = p.DuocPhamBenhVien.DuocPham.Ten,
                    NhanVienChiDinh = p.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = p.NoiChiDinh.Ten,
                    SoLuong = (decimal)p.SoLuong,
                    TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    DonGiaBenhVien = p.DonGiaBan,
                    DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                    BaoHiemChiTra = p.BaoHiemChiTra,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                    TrangThaiThanhToan = p.TrangThaiThanhToan,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    TrangThaiDuocPhamBenhVien = p.TrangThai,
                    ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiDuocPhamBenhVien != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                )
                .Union(
                    _ycvtBv.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) &&
                                    p.KhongTinhPhi != true)
                        .OrderBy(o => o.Id)
                        .Select(p => new DanhSachChuaHuongBhytNoiTruVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.YeuCauKhamBenhId,
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
                            MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            TrangThaiThanhToan = p.TrangThaiThanhToan,
                            TrangThaiVatTuBenhVien = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiVatTuBenhVien != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                )
                .Union(
                    _ycDvGiuongBHYTRepository.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId) && p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null)
                        .OrderBy(w => w.Id)
                        .Select(p => new DanhSachChuaHuongBhytNoiTruVo
                        {
                            Id = p.Id,
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
                            MucHuongSystem = p.BaoHiemChiTra == true ? p.MucHuongBaoHiem.GetValueOrDefault() : 0,
                            DonGiaBenhVien = p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            TrangThaiThanhToan = p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien.TrangThaiThanhToan,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            YeuCauGoiDichVuId = null,
                            //TrangThaiGiuongBenh = p.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiGiuongBenh != Enums.EnumTrangThaiGiuongBenh.DaHuy)
                )
                .Union(
                    _dtThanhToanRepository.TableNoTracking
                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId))
                        .OrderBy(o => o.Id)
                        .SelectMany(w => w.DonThuocThanhToanChiTiets)
                        .Select(p => new DanhSachChuaHuongBhytNoiTruVo
                        {
                            Id = p.Id,
                            YeuCauKhamBenhId = p.DonThuocThanhToan.YeuCauKhamBenhId,
                            IdDatabaseDonThuocThanhToan = p.DonThuocThanhToan.Id,
                            Khoa = "Nhà thuốc",
                            NgayPhatSinh = p.CreatedOn.GetValueOrDefault(),
                            GroupType = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                            DichVuId = p.Id,
                            TenDichVu = p.Ten,
                            SoLuong = (decimal)p.SoLuong,
                            TiLeDv = p.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            DonGiaBenhVien = p.DonGiaBan,
                            DGBHYTThamKhao = p.DonGiaBaoHiem.GetValueOrDefault(),
                            BaoHiemChiTra = p.BaoHiemChiTra,
                            TrangThaiThanhToan = p.DonThuocThanhToan.TrangThaiThanhToan,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            MucHuongDaDuyet = p.BaoHiemChiTra == null ? null : p.MucHuongBaoHiem,
                            TrangThaiDonThuocThanhToan = p.DonThuocThanhToan.TrangThai,
                            ShowHistory = p.DuyetBaoHiemChiTiets.Any()
                        }).Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == false && p.TrangThaiDonThuocThanhToan != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                );

            var listBhKoTt = listBhKoTraQuery.ToArrayAsync();

            await Task.WhenAll(listBhKoTt);
            
            //set the BHYT
            if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            {
                foreach (var chiPhiKhamChuaBenhVo in listBhKoTt.Result)
                {
                    var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs
                        .Where(o => o.NgayHieuLuc.Date <= chiPhiKhamChuaBenhVo.NgayPhatSinh.Date &&
                                    (o.NgayHetHan == null || o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date >= chiPhiKhamChuaBenhVo.NgayPhatSinh.Date))
                        .OrderByDescending(o => o.MucHuong).FirstOrDefault();
                    if (theBHYT != null)
                    {
                        chiPhiKhamChuaBenhVo.MaSoTheBHYT = theBHYT.MaSoThe;
                        chiPhiKhamChuaBenhVo.TheBHYTId = theBHYT.Id;
                    }
                }
            }

            foreach (var khamBenhModifiedItem in listBhKoTt.Result.Where(x => x.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
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
            var dataReturn = listBhKoTt.Result;
            if (ngayHetHanThe != null)
            {
                dataReturn = listBhKoTt.Result.Where(o => o.NgayPhatSinh.Date <= ngayHetHanThe).ToArray();
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
    }
}
