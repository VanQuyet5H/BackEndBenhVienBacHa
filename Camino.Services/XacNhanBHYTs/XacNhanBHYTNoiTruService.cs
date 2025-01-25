using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IXacNhanBHYTNoiTruService))]
    public class XacNhanBHYTNoiTruService : YeuCauTiepNhanBaseService, IXacNhanBHYTNoiTruService
    {
        public XacNhanBHYTNoiTruService(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, IUserAgentHelper userAgentHelper, ICauHinhService cauHinhService, ILocalizationService localizationService, ITaiKhoanBenhNhanService taiKhoanBenhNhanService) : base(yeuCauTiepNhanRepository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        {
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            //todo: need improve

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && p.CoBHYT == true && p.NoiTruBenhAn != null)
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
                    YeuCauTiepNhanNgoaiTruCanQuyetToanId = source.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    MaTN = source.MaYeuCauTiepNhan,
                    MaBN = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NgaySinh = source.NgaySinh,
                    ThangSinh = source.ThangSinh,
                    NamSinh = source.NamSinh,
                    TenGioiTinh = source.GioiTinh.GetDescription(),
                    ThoiDiemTiepNhan = source.ThoiDiemTiepNhan,
                    SoDienThoai = source.SoDienThoai,
                    SoDienThoaiFormat = source.SoDienThoaiDisplay,
                    DiaChi = source.DiaChiDayDu,
                    //ChuaXacNhan = true
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.SoDienThoai,
                    g => g.SoDienThoaiFormat
                );
            }

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);

            var queryResult = query.OrderBy(queryInfo.SortString).ToArray();
            if (queryResult.Length > 0)
            {
                var dsNoiTruId = queryResult.Select(o => o.Id).ToList();
                var dsNgoaiTruId = queryResult.Where(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null).Select(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId).ToList();
                var danhSachDichVuDuocHuongBhyTs = BaseRepository.TableNoTracking
                .Where(tn => dsNoiTruId.Contains(tn.Id) || dsNgoaiTruId.Contains(tn.Id)).Select(tn =>
                    new DanhSachDichVuDuocHuongBHYTVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBHYT = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DichVuKyThuatDuocHuongBHYT = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem && o.KhongTinhPhi != true).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        YeuCauTruyenMauDuocHuongBhyt = tn.YeuCauTruyenMaus
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = 1,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        DichVuGiuongDuocHuongBHYT = tn.YeuCauDichVuGiuongBenhVienChiPhiBHYTs
                            .Where(o => o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DuocPhamDuocHuongBHYT = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        ToaThuocDuocHuongBHYT = tn.DonThuocThanhToans
                            .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT &&
                                        o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                            .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(ct => ct.DuocHuongBaoHiem).Select(yc =>
                                new DichVuDuocHuongBHYTVo
                                {
                                    Soluong = yc.SoLuong,
                                    BaoHiemChiTra = yc.BaoHiemChiTra,
                                    DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                }).ToList()
                    }
                ).ToList();
                foreach (var danhSachChoGridVo in queryResult)
                {
                    danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.Id);
                    danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.YeuCauTiepNhanNgoaiTruCanQuyetToanId);
                    danhSachChoGridVo.NamSinhDisplay = DateHelper.DOBFormat(danhSachChoGridVo.NgaySinh, danhSachChoGridVo.ThangSinh, danhSachChoGridVo.NamSinh);
                    danhSachChoGridVo.ChuaXacNhan = (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.YeuCauTruyenMauDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuGiuongDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false);
                }
            }
            return new GridDataSource
            {
                Data = queryResult.Where(o => o.ChuaXacNhan == true).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(),
                TotalRowCount = queryResult.Where(o => o.ChuaXacNhan == true).Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //todo: need improve
            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && p.CoBHYT == true && p.NoiTruBenhAn != null
                            && (p.YeuCauKhamBenhs.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauDichVuKyThuats.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Where(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauTruyenMaus.Where(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauDuocPhamBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauVatTuBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                    .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Where(yc => yc.DuocHuongBaoHiem).Any(yc => yc.BaoHiemChiTra == null) ||
                            (p.YeuCauTiepNhanNgoaiTruCanQuyetToan != null &&
                             (p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauKhamBenhs.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ||
                              p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDichVuKyThuats.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) ||
                              p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDuocPhamBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) ||
                              p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauVatTuBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy) ||
                              p.YeuCauTiepNhanNgoaiTruCanQuyetToan.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                  .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Any(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null)))
                            ))
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
                    MaTN = source.MaYeuCauTiepNhan,
                    MaBN = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NgaySinh = source.NgaySinh,
                    ThangSinh = source.ThangSinh,
                    NamSinh = source.NamSinh,
                    TenGioiTinh = source.GioiTinh.GetDescription(),
                    ThoiDiemTiepNhan = source.ThoiDiemTiepNhan,
                    DiaChi = source.DiaChiDayDu,
                    SoDienThoai = source.SoDienThoai,
                    SoDienThoaiFormat = source.SoDienThoaiDisplay,
                    ChuaXacNhan = true
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.SoDienThoai,
                    g => g.SoDienThoaiFormat
                );
            }

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }

        public async Task<GridDataSource> GetDataForDaXacNhanAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            //todo: need improve

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                            && p.CoBHYT == true && p.NoiTruBenhAn != null)
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
                    YeuCauTiepNhanNgoaiTruCanQuyetToanId = source.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    MaTN = source.MaYeuCauTiepNhan,
                    MaBN = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NgaySinh = source.NgaySinh,
                    ThangSinh = source.ThangSinh,
                    NamSinh = source.NamSinh,
                    ThoiDiemTiepNhan = source.ThoiDiemTiepNhan,
                    TenGioiTinh = source.GioiTinh.GetDescription(),
                    DiaChi = source.DiaChiDayDu,
                    SoDienThoai = source.SoDienThoai,
                    SoDienThoaiFormat = source.SoDienThoaiDisplay,
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.SoDienThoai,
                    g => g.SoDienThoaiFormat
                );
            }

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);

            var queryResult = query.OrderBy(queryInfo.SortString).ToArray();
            if (queryResult.Length > 0)
            {
                var dsNoiTruId = queryResult.Select(o => o.Id).ToList();
                var dsNgoaiTruId = queryResult.Where(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null).Select(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId).ToList();
                var danhSachDichVuDuocHuongBhyTs = BaseRepository.TableNoTracking
                .Where(tn => dsNoiTruId.Contains(tn.Id) || dsNgoaiTruId.Contains(tn.Id)).Select(tn =>
                    new DanhSachDichVuDuocHuongBHYTVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBHYT = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DichVuKyThuatDuocHuongBHYT = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem && o.KhongTinhPhi != true).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        YeuCauTruyenMauDuocHuongBhyt = tn.YeuCauTruyenMaus
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = 1,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        DichVuGiuongDuocHuongBHYT = tn.YeuCauDichVuGiuongBenhVienChiPhiBHYTs
                            .Where(o => o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DuocPhamDuocHuongBHYT = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        ToaThuocDuocHuongBHYT = tn.DonThuocThanhToans
                            .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT &&
                                        o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                            .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(ct => ct.DuocHuongBaoHiem).Select(yc =>
                                new DichVuDuocHuongBHYTVo
                                {
                                    Soluong = yc.SoLuong,
                                    BaoHiemChiTra = yc.BaoHiemChiTra,
                                    DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                }).ToList()
                    }
                ).ToList();
                foreach (var danhSachChoGridVo in queryResult)
                {
                    danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.Id);
                    danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.YeuCauTiepNhanNgoaiTruCanQuyetToanId);
                    danhSachChoGridVo.NamSinhDisplay = DateHelper.DOBFormat(danhSachChoGridVo.NgaySinh, danhSachChoGridVo.ThangSinh, danhSachChoGridVo.NamSinh);
                    danhSachChoGridVo.ChuaXacNhan = (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.YeuCauTruyenMauDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuGiuongDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false);
                }
            }
            return new GridDataSource
            {
                Data = queryResult.Where(o => o.ChuaXacNhan == false).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(),
                TotalRowCount = queryResult.Where(o => o.ChuaXacNhan == false).Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageForDaXacNhanAsync(QueryInfo queryInfo)
        {
            //todo: need improve
            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                            && p.CoBHYT == true && p.NoiTruBenhAn != null
                                            // && (p.YeuCauKhamBenhs.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ||
                                            //     p.YeuCauDichVuKyThuats.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) ||
                                            //     p.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(yc => yc.DuocHuongBaoHiem) ||
                                            //     p.YeuCauTruyenMaus.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy) ||
                                            //     p.YeuCauDuocPhamBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) ||
                                            //     p.YeuCauVatTuBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy) ||
                                            //     p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                            //         .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Any(yc => yc.DuocHuongBaoHiem))
                                            && p.YeuCauKhamBenhs.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauDichVuKyThuats.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Where(yc => yc.DuocHuongBaoHiem).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauTruyenMaus.Where(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauDuocPhamBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauVatTuBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                                 .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Where(yc => yc.DuocHuongBaoHiem).All(yc => yc.BaoHiemChiTra != null)
                                                && (p.YeuCauTiepNhanNgoaiTruCanQuyetToan == null  ||
                                                    p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauKhamBenhs.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).All(yc => yc.BaoHiemChiTra != null) &&
                                                    p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDichVuKyThuats.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                    p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDuocPhamBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                    p.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauVatTuBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                    p.YeuCauTiepNhanNgoaiTruCanQuyetToan.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                                        .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Where(yc => yc.DuocHuongBaoHiem).All(yc => yc.BaoHiemChiTra != null))
                                            )
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
                    MaTN = source.MaYeuCauTiepNhan,
                    MaBN = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NgaySinh = source.NgaySinh,
                    ThangSinh = source.ThangSinh,
                    NamSinh = source.NamSinh,
                    TenGioiTinh = source.GioiTinh.GetDescription(),
                    DiaChi = source.DiaChiDayDu,
                    SoDienThoai = source.SoDienThoai,
                    ThoiDiemTiepNhan = source.ThoiDiemTiepNhan,
                    SoDienThoaiFormat = source.SoDienThoaiDisplay,
                    ChuaXacNhan = false
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.SoDienThoai,
                    g => g.SoDienThoaiFormat
                );
            }

            return new GridDataSource
            {
                TotalRowCount = await query.CountAsync()
            };
        }

        public async Task<GridDataSource> GetDataForBothBhyt(QueryInfo queryInfo, bool forExportExcel)
        {
            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            //todo: need improve
            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                && p.CoBHYT == true && p.NoiTruBenhAn != null
                                )
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
                    YeuCauTiepNhanNgoaiTruCanQuyetToanId = source.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    MaTN = source.MaYeuCauTiepNhan,
                    MaBN = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NgaySinh = source.NgaySinh,
                    ThangSinh = source.ThangSinh,
                    NamSinh = source.NamSinh,
                    TenGioiTinh = source.GioiTinh.GetDescription(),
                    DiaChi = source.DiaChiDayDu,
                    SoDienThoai = source.SoDienThoai,
                    SoDienThoaiFormat = source.SoDienThoaiDisplay,
                    ThoiDiemTiepNhan = source.ThoiDiemTiepNhan,
                    //ChuaXacNhan = source.YeuCauKhamBenhs.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ||
                    //              source.YeuCauDichVuKyThuats.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) ||
                    //              source.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null) ||
                    //              source.YeuCauTruyenMaus.Any(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy) ||
                    //              source.YeuCauDuocPhamBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) ||
                    //              source.YeuCauVatTuBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy) ||
                    //              source.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                    //                        .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Any(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null) ||
                    //              (source.YeuCauTiepNhanNgoaiTruCanQuyetToan != null && 
                    //                    (source.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauKhamBenhs.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ||
                    //                     source.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDichVuKyThuats.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) ||
                    //                     source.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDuocPhamBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) ||
                    //                     source.YeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauVatTuBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy) ||
                    //                     source.YeuCauTiepNhanNgoaiTruCanQuyetToan.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                    //                         .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Any(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null)))
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.SoDienThoai,
                    g => g.SoDienThoaiFormat
                );
            }
            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryResult = query
                .OrderBy(queryInfo.SortString)
                .Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            var totalRowCount = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            //await Task.WhenAll(countTask, queryTask);
            //var queryResult = queryTask.Result;
            if (queryResult.Length > 0)
            {
                var dsNoiTruId = queryResult.Select(o => o.Id).ToList();
                var dsNgoaiTruId = queryResult.Where(o=>o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null).Select(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId).ToList();
                var danhSachDichVuDuocHuongBhyTs = BaseRepository.TableNoTracking
                .Where(tn => dsNoiTruId.Contains(tn.Id) || dsNgoaiTruId.Contains(tn.Id)).Select(tn =>
                    new DanhSachDichVuDuocHuongBHYTVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBHYT = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DichVuKyThuatDuocHuongBHYT = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem && o.KhongTinhPhi != true).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        YeuCauTruyenMauDuocHuongBhyt = tn.YeuCauTruyenMaus
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DichVuGiuongDuocHuongBHYT = tn.YeuCauDichVuGiuongBenhVienChiPhiBHYTs
                            .Where(o => o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DuocPhamDuocHuongBHYT = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem && o.KhongTinhPhi != true)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        ToaThuocDuocHuongBHYT = tn.DonThuocThanhToans
                            .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT &&
                                        o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                            .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(ct => ct.DuocHuongBaoHiem).Select(yc =>
                                new DichVuDuocHuongBHYTVo
                                {
                                    Soluong = yc.SoLuong,
                                    BaoHiemChiTra = yc.BaoHiemChiTra,
                                    DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                }).ToList()
                    }
                ).ToList();
                foreach (var danhSachChoGridVo in queryResult)
                {
                    danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.Id);
                    danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.YeuCauTiepNhanNgoaiTruCanQuyetToanId);

                    danhSachChoGridVo.ChuaXacNhan = (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.YeuCauTruyenMauDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuGiuongDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuNgoaiTruDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false);

                    danhSachChoGridVo.NamSinhDisplay = DateHelper.DOBFormat(danhSachChoGridVo.NgaySinh, danhSachChoGridVo.ThangSinh, danhSachChoGridVo.NamSinh);
                }
            }

            return new GridDataSource
            {
                Data = queryResult,
                TotalRowCount = totalRowCount
            };
        }

        public async Task<GridDataSource> GetTotalPageForBothBhyt(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                && p.CoBHYT == true && p.NoiTruBenhAn != null 
                                //&& (p.YeuCauKhamBenhs.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ||
                                //p.YeuCauDichVuKyThuats.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) ||
                                //p.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(yc => yc.DuocHuongBaoHiem) ||
                                //p.YeuCauTruyenMaus.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy) ||
                                //p.YeuCauDuocPhamBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) ||
                                //p.YeuCauVatTuBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy) ||
                                //p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                //    .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Any(yc => yc.DuocHuongBaoHiem))
                                )
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
                    MaTN = source.MaYeuCauTiepNhan,
                    MaBN = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NgaySinh = source.NgaySinh,
                    ThangSinh = source.ThangSinh,
                    NamSinh = source.NamSinh,
                    TenGioiTinh = source.GioiTinh.GetDescription(),
                    DiaChi = source.DiaChiDayDu,
                    SoDienThoai = source.SoDienThoai,
                    ThoiDiemTiepNhan = source.ThoiDiemTiepNhan,
                    SoDienThoaiFormat = source.SoDienThoaiDisplay
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTN,
                    g => g.MaBN,
                    g => g.SoDienThoai,
                    g => g.SoDienThoaiFormat
                );
            }
            return new GridDataSource
            {
                TotalRowCount = await query.CountAsync()
            };
        }
    }
}
