using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenefitInsurance;
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
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IBhytConfirmByDayService))]
    public class BhytConfirmByDayService : YeuCauTiepNhanBaseService, IBhytConfirmByDayService
    {
        private new readonly ILocalizationService _localizationService;
        private new readonly IUserAgentHelper _userAgentHelper;

        public BhytConfirmByDayService(IRepository<YeuCauTiepNhan> repository, IUserAgentHelper userAgentHelper, ILocalizationService localizationService, ICauHinhService cauHinhService, ITaiKhoanBenhNhanService taiKhoanBenhNhanService)
            : base(repository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        {
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                            && p.YeuCauTiepNhanNoiTruQuyetToans.All(nt => nt.NoiTruBenhAn == null)
                            && p.CoBHYT == true)
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
                    SoDienThoai = source.SoDienThoai,
                    SoDienThoaiFormat = source.SoDienThoaiDisplay,
                    DiaChi = source.DiaChiDayDu
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
                var yctnIds = queryResult.Select(o => o.Id).ToList();
                var danhSachDichVuDuocHuongBhyTs = BaseRepository.TableNoTracking
                .Where(tn => yctnIds.Contains(tn.Id)).Select(tn =>
                    new DanhSachDichVuDuocHuongBHYTVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBHYT = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault(),
                                CachGiaiQuyet = yc.CachGiaiQuyet
                            }).ToList(),
                        DichVuKyThuatDuocHuongBHYT = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        DuocPhamDuocHuongBHYT = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem)
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
                                }).ToList(),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
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
                    danhSachChoGridVo.NamSinhDisplay = DateHelper.DOBFormat(danhSachChoGridVo.NgaySinh, danhSachChoGridVo.ThangSinh, danhSachChoGridVo.NamSinh);
                    danhSachChoGridVo.ChuaXacNhan = (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false);
                    if (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT != null && danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT.DichVuKhamBenhDuocHuongBHYT != null)
                    {
                        var cachGiaiQuyets = danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT.DichVuKhamBenhDuocHuongBHYT.Where(o => !string.IsNullOrEmpty(o.CachGiaiQuyet)).Select(o => o.CachGiaiQuyet).ToList();
                        danhSachChoGridVo.HuongXuLy = string.Join(',', cachGiaiQuyets);                       

                    }
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
            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                            && p.YeuCauTiepNhanNoiTruQuyetToans.All(nt => nt.NoiTruBenhAn == null)
                            && (p.YeuCauKhamBenhs.Where(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauDichVuKyThuats.Where(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauDuocPhamBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.YeuCauVatTuBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.BaoHiemChiTra == null && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).Any(yc => yc.BaoHiemChiTra == null) ||
                                p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                    .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Where(yc => yc.DuocHuongBaoHiem).Any(yc => yc.BaoHiemChiTra == null)))
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
            
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                             && p.YeuCauTiepNhanNoiTruQuyetToans.All(nt => nt.NoiTruBenhAn == null)
                                             && p.CoBHYT == true)
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
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

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            //await Task.WhenAll(countTask, queryTask);

            var queryResult = query.OrderBy(queryInfo.SortString).ToArray();
            if (queryResult.Length > 0)
            {
                var yctnIds = queryResult.Select(o => o.Id).ToList();
                var danhSachDichVuDuocHuongBhyTs = BaseRepository.TableNoTracking
                .Where(tn => yctnIds.Contains(tn.Id)).Select(tn =>
                    new DanhSachDichVuDuocHuongBHYTVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBHYT = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault(),
                                CachGiaiQuyet = yc.CachGiaiQuyet
                            }).ToList(),
                        DichVuKyThuatDuocHuongBHYT = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        DuocPhamDuocHuongBHYT = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem)
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
                    danhSachChoGridVo.NamSinhDisplay = DateHelper.DOBFormat(danhSachChoGridVo.NgaySinh, danhSachChoGridVo.ThangSinh, danhSachChoGridVo.NamSinh);
                    danhSachChoGridVo.ChuaXacNhan = (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false);
                    if (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT != null && danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT.DichVuKhamBenhDuocHuongBHYT != null)
                    {
                        var cachGiaiQuyets = danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT.DichVuKhamBenhDuocHuongBHYT.Where(o => !string.IsNullOrEmpty(o.CachGiaiQuyet)).Select(o => o.CachGiaiQuyet).ToList();
                        danhSachChoGridVo.HuongXuLy = string.Join(',', cachGiaiQuyets);
                        

                    }
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
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                             && p.YeuCauTiepNhanNoiTruQuyetToans.All(nt => nt.NoiTruBenhAn == null)
                                             && (p.YeuCauKhamBenhs.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ||
                                                 p.YeuCauDichVuKyThuats.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) ||
                                                 p.YeuCauDuocPhamBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) ||
                                                 p.YeuCauVatTuBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy) ||
                                                 p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                                     .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Any(yc => yc.DuocHuongBaoHiem))
                                            && p.YeuCauKhamBenhs.Where(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauDichVuKyThuats.Where(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauDuocPhamBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.YeuCauVatTuBenhViens.Where(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).All(yc => yc.BaoHiemChiTra != null) &&
                                                p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                                 .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Where(yc => yc.DuocHuongBaoHiem).All(yc => yc.BaoHiemChiTra != null))
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
            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                && p.YeuCauTiepNhanNoiTruQuyetToans.All(nt => nt.NoiTruBenhAn == null)
                                && p.CoBHYT == true)
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
                    SoDienThoaiFormat = source.SoDienThoaiDisplay,
                    ThoiDiemTiepNhan = source.ThoiDiemTiepNhan                    
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
                var yctnIds = queryResult.Select(o => o.Id).ToList();
                var danhSachDichVuDuocHuongBhyTs = BaseRepository.TableNoTracking
                .Where(tn => yctnIds.Contains(tn.Id)).Select(tn =>
                    new DanhSachDichVuDuocHuongBHYTVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBHYT = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault(),
                                CachGiaiQuyet = yc.CachGiaiQuyet
                            }).ToList(),
                        DichVuKyThuatDuocHuongBHYT = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        DuocPhamDuocHuongBHYT = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem)
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
                    danhSachChoGridVo.NamSinhDisplay = DateHelper.DOBFormat(danhSachChoGridVo.NgaySinh, danhSachChoGridVo.ThangSinh, danhSachChoGridVo.NamSinh);
                    danhSachChoGridVo.ChuaXacNhan = (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Any(o => o.BaoHiemChiTra == null) ?? false)
                        || (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Any(o => o.BaoHiemChiTra == null) ?? false);
                    if (danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT != null && danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT.DichVuKhamBenhDuocHuongBHYT != null)
                    {
                        var cachGiaiQuyets = danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT.DichVuKhamBenhDuocHuongBHYT.Where(o => !string.IsNullOrEmpty(o.CachGiaiQuyet)).Select(o => o.CachGiaiQuyet).ToList();
                        danhSachChoGridVo.HuongXuLy = string.Join(',', cachGiaiQuyets);
                    }
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
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                && p.YeuCauTiepNhanNoiTruQuyetToans.All(nt => nt.NoiTruBenhAn == null)
                                && p.CoBHYT == true)
                .Select(source => new DanhSachChoGridVo
                {
                    Id = source.Id,
                    MaTN = source.MaYeuCauTiepNhan,
                    MaBN = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    //NamSinh = source.NamSinh != null ? source.NamSinh.ToString() : string.Empty,
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
                TotalRowCount = query.Count()
            };
        }

        public async Task<DanhSachChoGridVo[]> GetXacNhanBhytByMaBnVaMaTt(TimKiemThongTinBenhNhan timKiemThongTinBenhNhan)
        {
            var query = BaseRepository.TableNoTracking
              .Where(cc => cc.MaYeuCauTiepNhan.Contains(timKiemThongTinBenhNhan.TimKiemMaBNVaMaTN) || cc.BenhNhanId.ToString().Contains(timKiemThongTinBenhNhan.TimKiemMaBNVaMaTN))
              .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                          && (p.YeuCauKhamBenhs.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ||
                              p.YeuCauDichVuKyThuats.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) ||
                              p.YeuCauDuocPhamBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) ||
                              p.YeuCauVatTuBenhViens.Any(yc => yc.DuocHuongBaoHiem && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy) ||
                              p.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                  .SelectMany(dt => dt.DonThuocThanhToanChiTiets).Any(yc => yc.DuocHuongBaoHiem)))
              .Select(source => new DanhSachChoGridVo
              {
                  Id = source.Id,
                  MaTN = source.MaYeuCauTiepNhan,
                  MaBN = source.BenhNhanId.ToString(),
              });

            var countTask = query.CountAsync();
            var queryTask = query.ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            var queryResult = queryTask.Result;
            if (queryResult.Length > 0)
            {
                var danhSachDichVuDuocHuongBhyTs = await BaseRepository.TableNoTracking
                .Where(tn => queryResult.Select(o => o.Id).Contains(tn.Id)).Select(tn =>
                    new DanhSachDichVuDuocHuongBHYTVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBHYT = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }),
                        DichVuKyThuatDuocHuongBHYT = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBHYTVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }),
                        DuocPhamDuocHuongBHYT = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBHYTVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.BaoHiemChiTra == null ? 100 : yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.BaoHiemChiTra == null ? 100 : yc.MucHuongBaoHiem.GetValueOrDefault()
                            }),
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
                                })
                    }
                ).ToListAsync();
                foreach (var danhSachChoGridVo in queryResult)
                {
                    danhSachChoGridVo.DanhSachDichVuDuocHuongBHYT = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.Id);
                }
            }

            return queryResult;
        }

        public async Task DuyetBaoHiemAsync(long yeuCauTiepNhanId)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentNoiLLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var ycTiepNhan =
                await BaseRepository.GetByIdAsync(yeuCauTiepNhanId,
                    x => x.Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                        .Include(o => o.YeuCauKhamBenhs).Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.YeuCauDichVuGiuongBenhViens).Include(o => o.YeuCauDuocPhamBenhViens).Include(o => o.YeuCauVatTuBenhViens)
                        .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets));
            var soDuTk = await GetSoTienDaTamUngAsync(yeuCauTiepNhanId);
            soDuTk -= GetSoTienCanThanhToanNgoaiTru(ycTiepNhan);
            DuyetBHYT(ycTiepNhan, currentUserId, currentNoiLLamViecId, soDuTk);
            await BaseRepository.UpdateAsync(ycTiepNhan);
        }

        public async Task<BenefitInsuranceResultVo> ConfirmBenefitInsuranceAsync(BenefitInsuranceVo duyetBaoHiemVo)
        {
            var dsXacNhanDuocChiTra = duyetBaoHiemVo.BenefitInsurance;
            var dsXacNhanKhongChiTra = duyetBaoHiemVo.NonBenefitInsurance;
            var dsXacNhan = new List<InsuranceConfirmVo>();
            dsXacNhan.AddRange(dsXacNhanDuocChiTra.Where(o => o.CheckedDefault));
            dsXacNhan.AddRange(dsXacNhanKhongChiTra.Where(o => o.CheckedDefault).Select(o => { o.MucHuong = 0; return o; }).ToList());

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var ycTiepNhan =
                BaseRepository.GetById(duyetBaoHiemVo.IdYeuCauTiepNhan,
                    x => x.Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                        .Include(o => o.YeuCauKhamBenhs).Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.YeuCauDichVuGiuongBenhViens).Include(o => o.YeuCauDuocPhamBenhViens).Include(o => o.YeuCauVatTuBenhViens)
                        .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets));// DonThuocThanhToanChiTiets include DonThuocThanhToan ?

            //kiem tra dv bi huy
            var ycKhamBenhExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauKhamBenhs
                    .Where(o => o.DuocHuongBaoHiem && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    .Select(o => o.Id));

            var ycKyThuatExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DichVuKyThuat).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauDichVuKyThuats
                    .Where(o => o.DuocHuongBaoHiem && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(o => o.Id));
            var ycGiuongExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauDichVuGiuongBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy)
                    .Select(o => o.Id));
            var ycDuocPhamExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DuocPham).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauDuocPhamBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.KhongTinhPhi != true && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(o => o.Id));
            var ycVatTuExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.VatTuTieuHao).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauVatTuBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.KhongTinhPhi != true && o.DuocHuongBaoHiem && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                    .Select(o => o.Id));
            var ycToaThuocExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DonThuocThanhToan).Select(o => o.Id).Except(ycTiepNhan
                    .DonThuocThanhToans
                    .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.DuocHuongBaoHiem)
                    .Select(o => o.Id));
            if (ycKhamBenhExcept.Any() || ycKyThuatExcept.Any() || ycGiuongExcept.Any() || ycDuocPhamExcept.Any() ||
                ycToaThuocExcept.Any() || ycVatTuExcept.Any())
            {
                return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
            }
            if (ycTiepNhan.YeuCauDichVuKyThuats
                .Any(o => o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && dsXacNhanDuocChiTra
                              .Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !_cauHinhService.GetTiLeHuongBaoHiemDichVuPTTT().Contains(s.TiLeTheoDichVu.GetValueOrDefault()))
                              .Select(s => s.Id).Contains(o.Id)))
            {
                return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.TiLeHuongPttt.NotValid") };
            }


            //var soTienBhytDaDuyet = GetSoTienBHYTDaDuyet(ycTiepNhan);
            //var soTienTheoMucHuong100 = GetSoTienBhytSeDuyetTheoMucHuong(ycTiepNhan, dsXacNhanDuocChiTra, 100);
            //bool capNhatMucHuong;
            //int mucHuong = 100;

            //if (soTienTheoMucHuong100 <= SoTienBHYTSeThanhToanToanBo())
            //{
            //    if (dsXacNhanDuocChiTra.Any(o => o.MucHuong.GetValueOrDefault() != 100))
            //    {
            //        return new BenefitInsuranceResultVo { IsError = true, ErrorType = 1, ErrorMessage = _localizationService.GetResource("BHYT.TiLeHuong.LessThan100Percent") };
            //    }
            //    capNhatMucHuong = soTienBhytDaDuyet > SoTienBHYTSeThanhToanToanBo();
            //}
            //else
            //{
            //    if (dsXacNhanDuocChiTra.Any(o => o.MucHuong.GetValueOrDefault() > ycTiepNhan.BHYTMucHuong.GetValueOrDefault()))
            //    {
            //        return new BenefitInsuranceResultVo { IsError = true, ErrorType = 2, ErrorMessage = _localizationService.GetResource("BHYT.TiLeHuong.GreaterThanCurrentBenefitPercent") };
            //    }
            //    capNhatMucHuong = soTienBhytDaDuyet <= SoTienBHYTSeThanhToanToanBo();
            //    mucHuong = ycTiepNhan.BHYTMucHuong.GetValueOrDefault();
            //}
            //kiem tra dv da thanh toan
            var dichVuDaThanhToanError = new BenefitInsuranceResultVo
            {
                IsError = true,
                ErrorType = 3,
                ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan")
            };
            foreach (var insuranceConfirmVo in dsXacNhan)
            {
                switch (insuranceConfirmVo.GroupType)
                {
                    case Enums.EnumNhomGoiDichVu.DichVuKhamBenh:
                        var yckb = ycTiepNhan.YeuCauKhamBenhs.First(o => o.Id == insuranceConfirmVo.Id);
                        //if (yckb.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && !KiemTraXacNhanDichVuDaThanhToan(yckb.BaoHiemChiTra, yckb.MucHuongBaoHiem, insuranceConfirmVo.MucHuong, capNhatMucHuong, mucHuong))
                        if (yckb.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return dichVuDaThanhToanError;
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuKyThuat:
                        var yckt = ycTiepNhan.YeuCauDichVuKyThuats.First(o => o.Id == insuranceConfirmVo.Id);
                        //if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && !KiemTraXacNhanDichVuDaThanhToan(yckt.BaoHiemChiTra, yckt.MucHuongBaoHiem, insuranceConfirmVo.MucHuong, capNhatMucHuong, mucHuong))
                        if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return dichVuDaThanhToanError;
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DuocPham:
                        var ycdp = ycTiepNhan.YeuCauDuocPhamBenhViens.First(o => o.Id == insuranceConfirmVo.Id);
                        //if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && !KiemTraXacNhanDichVuDaThanhToan(ycdp.BaoHiemChiTra, ycdp.MucHuongBaoHiem, insuranceConfirmVo.MucHuong, capNhatMucHuong, mucHuong))
                        if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return dichVuDaThanhToanError;
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.VatTuTieuHao:
                        var ycvt = ycTiepNhan.YeuCauVatTuBenhViens.First(o => o.Id == insuranceConfirmVo.Id);
                        //if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && !KiemTraXacNhanDichVuDaThanhToan(ycvt.BaoHiemChiTra, ycvt.MucHuongBaoHiem, insuranceConfirmVo.MucHuong, capNhatMucHuong, mucHuong))
                        if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return dichVuDaThanhToanError;
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DonThuocThanhToan:
                        var ycdt = ycTiepNhan.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).First(o => o.Id == insuranceConfirmVo.Id);
                        //if (ycdt.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && !KiemTraXacNhanDichVuDaThanhToan(ycdt.BaoHiemChiTra, ycdt.MucHuongBaoHiem, insuranceConfirmVo.MucHuong, capNhatMucHuong, mucHuong))
                        if (ycdt.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return dichVuDaThanhToanError;
                        }
                        break;
                }
            }

            //xac nhan
            var duyetBaoHiem = new DuyetBaoHiem
            {
                NhanVienDuyetBaoHiemId = currentUserId,
                ThoiDiemDuyetBaoHiem = DateTime.Now,
                NoiDuyetBaoHiemId = _userAgentHelper.GetCurrentNoiLLamViecId()
            };

            foreach (var insuranceConfirmVo in dsXacNhan)
            {
                switch (insuranceConfirmVo.GroupType)
                {
                    case Enums.EnumNhomGoiDichVu.DichVuKhamBenh:
                        var yeuCauKhamBenh = ycTiepNhan.YeuCauKhamBenhs.First(o => o.Id == insuranceConfirmVo.Id);
                        int tiLeBaoHiemThanhToanDvKham = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0 ? TiLeHuongBHYTTheoLanKham(GetThuTuLanKham(dsXacNhanDuocChiTra, yeuCauKhamBenh.Id)) : 0;
                        if (yeuCauKhamBenh.BaoHiemChiTra == null ||
                            yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault() ||
                            yeuCauKhamBenh.TiLeBaoHiemThanhToan != tiLeBaoHiemThanhToanDvKham)
                        {
                            //int tiLeBaoHiemThanhToanTruoc = yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault();
                            //int mucHuongBaoHiemTruoc = yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault();
                            //bool coCapNhatBHYT = yeuCauKhamBenh.TiLeBaoHiemThanhToan != GetThuTuLanKham(ycTiepNhan.YeuCauKhamBenhs, dsXacNhanDuocChiTra, yeuCauKhamBenh.Id) || yeuCauKhamBenh.MucHuongBaoHiem != insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            yeuCauKhamBenh.MucHuongBaoHiem = tiLeBaoHiemThanhToanDvKham != 0 ? insuranceConfirmVo.MucHuong.GetValueOrDefault() : 0;
                            yeuCauKhamBenh.BaoHiemChiTra = tiLeBaoHiemThanhToanDvKham != 0 && insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            yeuCauKhamBenh.TiLeBaoHiemThanhToan = tiLeBaoHiemThanhToanDvKham;
                            yeuCauKhamBenh.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            yeuCauKhamBenh.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauKhamBenhId = yeuCauKhamBenh.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = yeuCauKhamBenh.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = yeuCauKhamBenh.MucHuongBaoHiem,
                                DonGiaBaoHiem = yeuCauKhamBenh.DonGiaBaoHiem
                            });
                        }

                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuKyThuat:
                        var yckt = ycTiepNhan.YeuCauDichVuKyThuats.First(o => o.Id == insuranceConfirmVo.Id);
                        int tiLeBaoHiemThanhDvkt = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0 ? (yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? insuranceConfirmVo.TiLeTheoDichVu.GetValueOrDefault() : 100) : 0;
                        if (yckt.BaoHiemChiTra == null ||
                            yckt.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault() ||
                            yckt.TiLeBaoHiemThanhToan != tiLeBaoHiemThanhDvkt)
                        {
                            yckt.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            yckt.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            yckt.TiLeBaoHiemThanhToan = tiLeBaoHiemThanhDvkt;
                            yckt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            yckt.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauDichVuKyThuatId = yckt.Id,
                                SoLuong = yckt.SoLan,
                                TiLeBaoHiemThanhToan = yckt.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = yckt.MucHuongBaoHiem,
                                DonGiaBaoHiem = yckt.DonGiaBaoHiem
                            });
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DuocPham:
                        var ycdp = ycTiepNhan.YeuCauDuocPhamBenhViens.First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycdp.BaoHiemChiTra == null || ycdp.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            ycdp.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            ycdp.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            ycdp.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            ycdp.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauDuocPhamBenhVienId = ycdp.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = ycdp.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = ycdp.MucHuongBaoHiem,
                                DonGiaBaoHiem = ycdp.DonGiaBaoHiem
                            });
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.VatTuTieuHao:
                        var ycvt = ycTiepNhan.YeuCauVatTuBenhViens.First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycvt.BaoHiemChiTra == null || ycvt.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            ycvt.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            ycvt.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            ycvt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            ycvt.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauVatTuBenhVienId = ycvt.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = ycvt.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = ycvt.MucHuongBaoHiem,
                                DonGiaBaoHiem = ycvt.DonGiaBaoHiem
                            });
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DonThuocThanhToan:
                        var ycdt = ycTiepNhan.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycdt.BaoHiemChiTra == null || ycdt.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            ycdt.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            ycdt.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            ycdt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            ycdt.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                DonThuocThanhToanChiTietId = ycdt.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = ycdt.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = ycdt.MucHuongBaoHiem,
                                DonGiaBaoHiem = ycdt.DonGiaBaoHiem
                            });
                        }

                        break;
                }

            }

            foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.YeuCauDuocPhamBenhViens)
            {
                if (yeuCauDuocPhamBenhVien.SoLuong.AlmostEqual(0)
                    && yeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                    && yeuCauDuocPhamBenhVien.DuocHuongBaoHiem
                    && yeuCauDuocPhamBenhVien.BaoHiemChiTra == null)
                {                    
                    yeuCauDuocPhamBenhVien.MucHuongBaoHiem = 0;
                    yeuCauDuocPhamBenhVien.BaoHiemChiTra = false;
                    yeuCauDuocPhamBenhVien.ThoiDiemDuyetBaoHiem = DateTime.Now;
                    yeuCauDuocPhamBenhVien.NhanVienDuyetBaoHiemId = currentUserId;

                    duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                    {
                        YeuCauDuocPhamBenhVienId = yeuCauDuocPhamBenhVien.Id,
                        SoLuong = yeuCauDuocPhamBenhVien.SoLuong,
                        TiLeBaoHiemThanhToan = yeuCauDuocPhamBenhVien.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = yeuCauDuocPhamBenhVien.MucHuongBaoHiem,
                        DonGiaBaoHiem = yeuCauDuocPhamBenhVien.DonGiaBaoHiem
                    });
                }
            }

            if (duyetBaoHiem.DuyetBaoHiemChiTiets.Any())
            {
                if (!KiemTraDuyetVuotMuocHuong(ycTiepNhan, ycTiepNhan.BHYTMucHuong.GetValueOrDefault()))
                {
                    return new BenefitInsuranceResultVo
                    {
                        IsError = true,
                        ErrorType = 3,
                        ErrorMessage = _localizationService.GetResource("BHYT.DichVuVuotMuocHuong")
                    };
                }

                var soTienTamUng = _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(ycTiepNhan.Id).Result;
                BaoLanhDvNgoaiGoiMarketing(ycTiepNhan, soTienTamUng - GetSoTienCanThanhToanNgoaiTru(ycTiepNhan));
                ycTiepNhan.DuyetBaoHiems.Add(duyetBaoHiem);
                BaseRepository.Update(ycTiepNhan);
            }

            return new BenefitInsuranceResultVo { IsError = false };
        }

        public BenefitInsuranceResultVo HuyDuyetBaoHiemYte(BenefitInsuranceVo duyetBaoHiemVo)
        {
            var dsXacNhanDuocChiTra = duyetBaoHiemVo.BenefitInsurance;
            var dsXacNhanKhongChiTra = duyetBaoHiemVo.NonBenefitInsurance;
            var dsXacNhan = new List<InsuranceConfirmVo>();
            dsXacNhan.AddRange(dsXacNhanDuocChiTra.Where(o => o.CheckedDefault));
            dsXacNhan.AddRange(dsXacNhanKhongChiTra.Where(o => o.CheckedDefault));

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var ycTiepNhan =
                BaseRepository.GetById(duyetBaoHiemVo.IdYeuCauTiepNhan,
                    x => x.Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                        .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                        .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                        .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                        .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                        .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets));// DonThuocThanhToanChiTiets include DonThuocThanhToan ?

            //kiem tra dv da thanh toan
            var ycKhamBenhExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauKhamBenhs
                    .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan)
                    .Select(o => o.Id));

            var ycKyThuatExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DichVuKyThuat).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauDichVuKyThuats
                    .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan)
                    .Select(o => o.Id));
            var ycDuocPhamExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DuocPham).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauDuocPhamBenhViens
                    .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan)
                    .Select(o => o.Id));
            var ycVatTuExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.VatTuTieuHao).Select(o => o.Id).Except(ycTiepNhan
                    .YeuCauVatTuBenhViens
                    .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan)
                    .Select(o => o.Id));
            var ycToaThuocExcept = dsXacNhan
                .Where(o => o.GroupType == Enums.EnumNhomGoiDichVu.DonThuocThanhToan).Select(o => o.Id).Except(ycTiepNhan
                    .DonThuocThanhToans
                    .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.DuocHuongBaoHiem)
                    .Select(o => o.Id));
            if (ycKhamBenhExcept.Any() || ycKyThuatExcept.Any() || ycDuocPhamExcept.Any() ||
                ycToaThuocExcept.Any() || ycVatTuExcept.Any())
            {
                return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
            }

            foreach (var insuranceConfirmVo in dsXacNhan)
            {
                switch (insuranceConfirmVo.GroupType)
                {
                    case Enums.EnumNhomGoiDichVu.DichVuKhamBenh:
                        var yeuCauKhamBenh = ycTiepNhan.YeuCauKhamBenhs.First(o => o.Id == insuranceConfirmVo.Id);
                        if (yeuCauKhamBenh.BaoHiemChiTra != null)
                        {
                            yeuCauKhamBenh.BaoHiemChiTra = null;
                            yeuCauKhamBenh.ThoiDiemDuyetBaoHiem = null;
                            yeuCauKhamBenh.NhanVienDuyetBaoHiemId = null;
                            foreach (var duyetBaoHiemChiTiet in yeuCauKhamBenh.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }

                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuKyThuat:
                        var yckt = ycTiepNhan.YeuCauDichVuKyThuats.First(o => o.Id == insuranceConfirmVo.Id);
                        if (yckt.BaoHiemChiTra != null)
                        {
                            yckt.BaoHiemChiTra = null;
                            yckt.ThoiDiemDuyetBaoHiem = null;
                            yckt.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in yckt.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DuocPham:
                        var ycdp = ycTiepNhan.YeuCauDuocPhamBenhViens.First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycdp.BaoHiemChiTra != null)
                        {
                            ycdp.BaoHiemChiTra = null;
                            ycdp.ThoiDiemDuyetBaoHiem = null;
                            ycdp.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in ycdp.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.VatTuTieuHao:
                        var ycvt = ycTiepNhan.YeuCauVatTuBenhViens.First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycvt.BaoHiemChiTra != null)
                        {
                            ycvt.BaoHiemChiTra = null;
                            ycvt.ThoiDiemDuyetBaoHiem = null;
                            ycvt.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in ycvt.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DonThuocThanhToan:
                        var ycdt = ycTiepNhan.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycdt.BaoHiemChiTra != null)
                        {
                            ycdt.BaoHiemChiTra = null;
                            ycdt.ThoiDiemDuyetBaoHiem = null;
                            ycdt.NhanVienDuyetBaoHiemId = null;
                            foreach (var duyetBaoHiemChiTiet in ycdt.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }

                        break;
                }

            }

            if (dsXacNhan.Any())
            {
                BaseRepository.Update(ycTiepNhan);
            }

            return new BenefitInsuranceResultVo { IsError = false };
        }

        private bool KiemTraXacNhanDichVuDaThanhToan(bool? daXacNhanChiTra, int? mucHuongDaXacNhan, int? mucHuongMoi, bool capNhatMucHuong, int mucHuong)
        {
            if (daXacNhanChiTra == true)
            {
                if (mucHuongMoi.GetValueOrDefault() == 0)//chuyen duoc chi tra -> khong duoc chi tra
                {
                    return false;
                }
                if (mucHuongMoi.GetValueOrDefault() != mucHuongDaXacNhan.GetValueOrDefault())
                {
                    if (!capNhatMucHuong || mucHuong != mucHuongMoi.GetValueOrDefault())
                    {
                        return false;
                    }
                }
            }
            else if (daXacNhanChiTra == false)
            {
                if (mucHuongMoi.GetValueOrDefault() != 0)//chuyen khong duoc chi tra -> duoc chi tra
                {
                    return false;
                }
            }
            return true;
        }

        private decimal GetSoTienBHYTDaDuyet(YeuCauTiepNhan yeuCauTiepNhan)
        {
            decimal total = 0;
            total += yeuCauTiepNhan.YeuCauKhamBenhs
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100).Sum();

            total += yeuCauTiepNhan.YeuCauDichVuKyThuats
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * o.SoLan).Sum();

            total += yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            total += yeuCauTiepNhan.YeuCauVatTuBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            total += yeuCauTiepNhan.DonThuocThanhToans
                    .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            return total;
        }

        private bool KiemTraDuyetVuotMuocHuong(YeuCauTiepNhan yeuCauTiepNhan, int mucHuongThe)
        {
            decimal total = 0;
            var yeuCauKhamBenhs = yeuCauTiepNhan.YeuCauKhamBenhs
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
            var yeuCauDichVuKyThuats = yeuCauTiepNhan.YeuCauDichVuKyThuats
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
            var yeuCauDuocPhamBenhViens = yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy);
            var yeuCauVatTuBenhViens = yeuCauTiepNhan.YeuCauVatTuBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy);
            var donThuocThanhToans = yeuCauTiepNhan.DonThuocThanhToans
                    .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true);


            total += yeuCauKhamBenhs.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100).Sum();
            total += yeuCauDichVuKyThuats.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * o.SoLan).Sum();
            total += yeuCauDuocPhamBenhViens.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            total += yeuCauVatTuBenhViens.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            total += donThuocThanhToans.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();

            if (total >= SoTienBHYTSeThanhToanToanBo())
            {
                if (yeuCauKhamBenhs.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > mucHuongThe)
                    || yeuCauDichVuKyThuats.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > mucHuongThe)
                    || yeuCauDuocPhamBenhViens.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > mucHuongThe)
                    || yeuCauVatTuBenhViens.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > mucHuongThe)
                    || donThuocThanhToans.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > mucHuongThe))
                {
                    return false;
                }
            }
            return true;
        }

        private bool KiemTraDuyetVuotMuocHuongNoiTru(YeuCauTiepNhan yeuCauTiepNhanNoiTru, YeuCauTiepNhan yeuCauTiepNhanNgoaiTru)
        {
            decimal total = 0;
            var yeuCauKhamBenhs = yeuCauTiepNhanNgoaiTru?.YeuCauKhamBenhs
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
            var yeuCauDichVuKyThuats = yeuCauTiepNhanNgoaiTru?.YeuCauDichVuKyThuats
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
            var yeuCauDuocPhamBenhViens = yeuCauTiepNhanNgoaiTru?.YeuCauDuocPhamBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy);
            var yeuCauVatTuBenhViens = yeuCauTiepNhanNgoaiTru?.YeuCauVatTuBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy);
            var donThuocThanhToans = yeuCauTiepNhanNgoaiTru?.DonThuocThanhToans
                    .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true);

            var yeuCauDichVuKyThuatNoiTrus = yeuCauTiepNhanNoiTru.YeuCauDichVuKyThuats
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
            var yeuCauTruyenMauNoiTrus = yeuCauTiepNhanNoiTru.YeuCauTruyenMaus
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true);
            var yeuCauGiuongBenhNoiTrus = yeuCauTiepNhanNoiTru.YeuCauDichVuGiuongBenhVienChiPhiBHYTs
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null);
            var yeuCauDuocPhamBenhVienNoiTrus = yeuCauTiepNhanNoiTru.YeuCauDuocPhamBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy);
            var yeuCauVatTuBenhVienNoiTrus = yeuCauTiepNhanNoiTru.YeuCauVatTuBenhViens
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy);
            var donThuocThanhToanNoiTrus = yeuCauTiepNhanNoiTru.DonThuocThanhToans
                    .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true);


            total += yeuCauKhamBenhs?.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100).Sum() ?? 0;
            total += yeuCauDichVuKyThuats?.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * o.SoLan).Sum() ?? 0;
            total += yeuCauDuocPhamBenhViens?.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum() ?? 0;
            total += yeuCauVatTuBenhViens?.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum() ?? 0;
            total += donThuocThanhToans?.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum() ?? 0;

            total += yeuCauDichVuKyThuatNoiTrus.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * o.SoLan).Sum();
            total += yeuCauTruyenMauNoiTrus.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100).Sum();
            total += yeuCauGiuongBenhNoiTrus.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            total += yeuCauDuocPhamBenhVienNoiTrus.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            total += yeuCauVatTuBenhVienNoiTrus.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();
            total += donThuocThanhToanNoiTrus.Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong).Sum();

            if (total >= SoTienBHYTSeThanhToanToanBo())
            {
                if (yeuCauDichVuKyThuatNoiTrus.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || yeuCauTruyenMauNoiTrus.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || yeuCauGiuongBenhNoiTrus.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || yeuCauDuocPhamBenhVienNoiTrus.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || yeuCauVatTuBenhVienNoiTrus.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || donThuocThanhToanNoiTrus.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0)))
                {
                    return false;
                }
                if (yeuCauTiepNhanNgoaiTru != null)
                {
                    if (yeuCauKhamBenhs.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || yeuCauDichVuKyThuats.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || yeuCauDuocPhamBenhViens.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || yeuCauVatTuBenhViens.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0))
                    || donThuocThanhToans.Any(o => o.MucHuongBaoHiem.GetValueOrDefault() > (yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(t => o.YeuCauTiepNhanTheBHYTId == null || t.Id == o.YeuCauTiepNhanTheBHYTId)?.MucHuong ?? 0)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int GetThuTuLanKham(List<InsuranceConfirmVo> dsXacNhanDuocChiTra, long yeuCauKhamBenhId)
        {
            return dsXacNhanDuocChiTra.Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh).OrderBy(s => s.Id).Select(s => s.Id).IndexOf(yeuCauKhamBenhId);
        }

        public async Task<BenefitInsuranceResultVo> XacNhanBHYTNoiTruAsync(BenefitInsuranceVo duyetBaoHiemVo)
        {
            var dsXacNhanDuocChiTra = duyetBaoHiemVo.BenefitInsurance;
            var dsXacNhanKhongChiTra = duyetBaoHiemVo.NonBenefitInsurance;
            var dsXacNhan = new List<InsuranceConfirmVo>();
            dsXacNhan.AddRange(dsXacNhanDuocChiTra.Where(o => o.CheckedDefault));
            dsXacNhan.AddRange(dsXacNhanKhongChiTra.Where(o => o.CheckedDefault).Select(o => { o.MucHuong = 0; return o; }).ToList());

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var ycTiepNhan =
                BaseRepository.GetById(duyetBaoHiemVo.IdYeuCauTiepNhan,
                    x => x.Include(o => o.NoiTruBenhAn).Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                        .Include(o => o.YeuCauKhamBenhs).Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.YeuCauDichVuGiuongBenhViens)
                        .Include(o => o.YeuCauDuocPhamBenhViens)
                        .Include(o => o.YeuCauVatTuBenhViens)
                        .Include(o => o.YeuCauTruyenMaus)
                        .Include(o => o.YeuCauTiepNhanTheBHYTs)
                        .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                        .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets));

            var ycTiepNhanNgoaiTru = ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId == null ? null :
                BaseRepository.GetById(ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value,
                    x => x.Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                        .Include(o => o.YeuCauKhamBenhs).Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.YeuCauDuocPhamBenhViens)
                        .Include(o => o.YeuCauVatTuBenhViens)
                        .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets));

            if (ycTiepNhan.NoiTruBenhAn.DaQuyetToan == true)
            {
                return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Người bệnh đã được quyết toán" };
            }

            //xac nhan
            var duyetBaoHiem = new DuyetBaoHiem
            {
                NhanVienDuyetBaoHiemId = currentUserId,
                ThoiDiemDuyetBaoHiem = DateTime.Now,
                NoiDuyetBaoHiemId = _userAgentHelper.GetCurrentNoiLLamViecId()
            };
            var dichVuDaThanhToanError = new BenefitInsuranceResultVo
            {
                IsError = true,
                ErrorType = 3,
                ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan")
            };
            foreach (var insuranceConfirmVo in dsXacNhan)
            {
                switch (insuranceConfirmVo.GroupType)
                {
                    case Enums.EnumNhomGoiDichVu.DichVuKhamBenh:
                        var yeuCauKhamBenh = ycTiepNhan.YeuCauKhamBenhs.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauKhamBenhs.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);
                        if (yeuCauKhamBenh == null || yeuCauKhamBenh.DuocHuongBaoHiem == false || yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan") };
                        }

                        int tiLeBaoHiemThanhToanDvKham = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0 ? TiLeHuongBHYTTheoLanKham(GetThuTuLanKham(dsXacNhanDuocChiTra, yeuCauKhamBenh.Id)) : 0;
                        if (yeuCauKhamBenh.BaoHiemChiTra == null ||
                            yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault() ||
                            yeuCauKhamBenh.TiLeBaoHiemThanhToan != tiLeBaoHiemThanhToanDvKham)
                        {
                            yeuCauKhamBenh.YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId;
                            yeuCauKhamBenh.MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT;
                            yeuCauKhamBenh.MucHuongBaoHiem = tiLeBaoHiemThanhToanDvKham != 0 ? insuranceConfirmVo.MucHuong.GetValueOrDefault() : 0;
                            yeuCauKhamBenh.BaoHiemChiTra = tiLeBaoHiemThanhToanDvKham != 0 && insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            yeuCauKhamBenh.TiLeBaoHiemThanhToan = tiLeBaoHiemThanhToanDvKham;
                            yeuCauKhamBenh.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            yeuCauKhamBenh.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId,
                                MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT,
                                YeuCauKhamBenhId = yeuCauKhamBenh.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = yeuCauKhamBenh.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = yeuCauKhamBenh.MucHuongBaoHiem,
                                DonGiaBaoHiem = yeuCauKhamBenh.DonGiaBaoHiem
                            });
                        }

                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuKyThuat:
                        var yckt = ycTiepNhan.YeuCauDichVuKyThuats.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauDichVuKyThuats.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);

                        if (yckt == null || yckt.DuocHuongBaoHiem == false || yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan") };
                        }

                        int tiLeBaoHiemThanhDvkt = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0 ? (yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? insuranceConfirmVo.TiLeTheoDichVu.GetValueOrDefault() : 100) : 0;
                        if (yckt.BaoHiemChiTra == null ||
                            yckt.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault() ||
                            yckt.TiLeBaoHiemThanhToan != tiLeBaoHiemThanhDvkt)
                        {
                            yckt.YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId;
                            yckt.MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT;
                            yckt.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            yckt.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            yckt.TiLeBaoHiemThanhToan = tiLeBaoHiemThanhDvkt;
                            yckt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            yckt.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId,
                                MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT,
                                YeuCauDichVuKyThuatId = yckt.Id,
                                SoLuong = yckt.SoLan,
                                TiLeBaoHiemThanhToan = yckt.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = yckt.MucHuongBaoHiem,
                                DonGiaBaoHiem = yckt.DonGiaBaoHiem
                            });
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.TruyenMau:
                        var yctm = ycTiepNhan.YeuCauTruyenMaus.First(o => o.Id == insuranceConfirmVo.Id);
                        if (yctm.DuocHuongBaoHiem == false)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (yctm.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan") };
                        }

                        if (yctm.BaoHiemChiTra == null ||
                            yctm.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            yctm.YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId;
                            yctm.MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT;
                            yctm.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            yctm.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            yctm.TiLeBaoHiemThanhToan = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0 ? 100 : 0;
                            yctm.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            yctm.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId,
                                MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT,
                                YeuCauTruyenMauId = yctm.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = yctm.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = yctm.MucHuongBaoHiem,
                                DonGiaBaoHiem = yctm.DonGiaBaoHiem
                            });
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuGiuongBenh:
                        var ycg = ycTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycg.DuocHuongBaoHiem == false)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (ycg.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan") };
                        }

                        if (ycg.BaoHiemChiTra == null || ycg.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            ycg.YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId;
                            ycg.MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT;
                            ycg.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            ycg.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            ycg.TiLeBaoHiemThanhToan = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0 ? 100 : 0;
                            ycg.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            ycg.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId,
                                MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT,
                                YeuCauDichVuGiuongBenhVienChiPhiBHYTId = ycg.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = ycg.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = ycg.MucHuongBaoHiem,
                                DonGiaBaoHiem = ycg.DonGiaBaoHiem
                            });
                        }

                        break;
                    case Enums.EnumNhomGoiDichVu.DuocPham:
                        var ycdp = ycTiepNhan.YeuCauDuocPhamBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauDuocPhamBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);

                        if (ycdp == null || ycdp.DuocHuongBaoHiem == false || ycdp.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan") };
                        }

                        if (ycdp.BaoHiemChiTra == null || ycdp.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            ycdp.YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId;
                            ycdp.MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT;
                            ycdp.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            ycdp.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            ycdp.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            ycdp.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId,
                                MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT,
                                YeuCauDuocPhamBenhVienId = ycdp.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = ycdp.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = ycdp.MucHuongBaoHiem,
                                DonGiaBaoHiem = ycdp.DonGiaBaoHiem
                            });
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.VatTuTieuHao:
                        var ycvt = ycTiepNhan.YeuCauVatTuBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauVatTuBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);

                        if (ycvt == null || ycvt.DuocHuongBaoHiem == false || ycvt.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan") };
                        }

                        if (ycvt.BaoHiemChiTra == null || ycvt.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            ycvt.YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId;
                            ycvt.MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT;
                            ycvt.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            ycvt.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            ycvt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            ycvt.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId,
                                MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT,
                                YeuCauVatTuBenhVienId = ycvt.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = ycvt.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = ycvt.MucHuongBaoHiem,
                                DonGiaBaoHiem = ycvt.DonGiaBaoHiem
                            });
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DonThuocThanhToan:
                        var ycdt = ycTiepNhan.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);

                        if (ycdt == null || ycdt.DuocHuongBaoHiem == false)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (ycdt.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.DichVuDaThanhToan") };
                        }

                        if (ycdt.BaoHiemChiTra == null || ycdt.MucHuongBaoHiem.GetValueOrDefault() != insuranceConfirmVo.MucHuong.GetValueOrDefault())
                        {
                            ycdt.YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId;
                            ycdt.MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT;
                            ycdt.MucHuongBaoHiem = insuranceConfirmVo.MucHuong.GetValueOrDefault();
                            ycdt.BaoHiemChiTra = insuranceConfirmVo.MucHuong.GetValueOrDefault() != 0;
                            ycdt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                            ycdt.NhanVienDuyetBaoHiemId = currentUserId;

                            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                            {
                                YeuCauTiepNhanTheBHYTId = insuranceConfirmVo.TheBHYTId,
                                MaSoTheBHYT = insuranceConfirmVo.MaSoTheBHYT,
                                DonThuocThanhToanChiTietId = ycdt.Id,
                                SoLuong = 1,
                                TiLeBaoHiemThanhToan = ycdt.TiLeBaoHiemThanhToan,
                                MucHuongBaoHiem = ycdt.MucHuongBaoHiem,
                                DonGiaBaoHiem = ycdt.DonGiaBaoHiem
                            });
                        }
                        break;
                }

            }

            foreach(var yeuCauDuocPhamBenhVien in ycTiepNhan.YeuCauDuocPhamBenhViens)
            {
                if(yeuCauDuocPhamBenhVien.SoLuong.AlmostEqual(0) 
                    && yeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy 
                    && yeuCauDuocPhamBenhVien.DuocHuongBaoHiem 
                    && yeuCauDuocPhamBenhVien.BaoHiemChiTra == null)
                {
                    var theBHYT = ycTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

                    yeuCauDuocPhamBenhVien.YeuCauTiepNhanTheBHYTId = theBHYT?.Id;
                    yeuCauDuocPhamBenhVien.MaSoTheBHYT = theBHYT?.MaSoThe;
                    yeuCauDuocPhamBenhVien.MucHuongBaoHiem = 0;
                    yeuCauDuocPhamBenhVien.BaoHiemChiTra = false;
                    yeuCauDuocPhamBenhVien.ThoiDiemDuyetBaoHiem = DateTime.Now;
                    yeuCauDuocPhamBenhVien.NhanVienDuyetBaoHiemId = currentUserId;

                    duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                    {
                        YeuCauTiepNhanTheBHYTId = theBHYT?.Id,
                        MaSoTheBHYT = theBHYT?.MaSoThe,
                        YeuCauDuocPhamBenhVienId = yeuCauDuocPhamBenhVien.Id,
                        SoLuong = yeuCauDuocPhamBenhVien.SoLuong,
                        TiLeBaoHiemThanhToan = yeuCauDuocPhamBenhVien.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = yeuCauDuocPhamBenhVien.MucHuongBaoHiem,
                        DonGiaBaoHiem = yeuCauDuocPhamBenhVien.DonGiaBaoHiem
                    });
                }    
            }    
            if(ycTiepNhanNgoaiTru != null)
            {
                foreach (var yeuCauDuocPhamBenhVien in ycTiepNhanNgoaiTru.YeuCauDuocPhamBenhViens)
                {
                    if (yeuCauDuocPhamBenhVien.SoLuong.AlmostEqual(0)
                        && yeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                        && yeuCauDuocPhamBenhVien.DuocHuongBaoHiem
                        && yeuCauDuocPhamBenhVien.BaoHiemChiTra == null)
                    {
                        var theBHYT = ycTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

                        yeuCauDuocPhamBenhVien.YeuCauTiepNhanTheBHYTId = theBHYT?.Id;
                        yeuCauDuocPhamBenhVien.MaSoTheBHYT = theBHYT?.MaSoThe;
                        yeuCauDuocPhamBenhVien.MucHuongBaoHiem = 0;
                        yeuCauDuocPhamBenhVien.BaoHiemChiTra = false;
                        yeuCauDuocPhamBenhVien.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        yeuCauDuocPhamBenhVien.NhanVienDuyetBaoHiemId = currentUserId;

                        duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                        {
                            YeuCauTiepNhanTheBHYTId = theBHYT?.Id,
                            MaSoTheBHYT = theBHYT?.MaSoThe,
                            YeuCauDuocPhamBenhVienId = yeuCauDuocPhamBenhVien.Id,
                            SoLuong = yeuCauDuocPhamBenhVien.SoLuong,
                            TiLeBaoHiemThanhToan = yeuCauDuocPhamBenhVien.TiLeBaoHiemThanhToan,
                            MucHuongBaoHiem = yeuCauDuocPhamBenhVien.MucHuongBaoHiem,
                            DonGiaBaoHiem = yeuCauDuocPhamBenhVien.DonGiaBaoHiem
                        });
                    }
                }
            }

            if (duyetBaoHiem.DuyetBaoHiemChiTiets.Any())
            {
                if (!KiemTraDuyetVuotMuocHuongNoiTru(ycTiepNhan, ycTiepNhanNgoaiTru))
                {
                    return new BenefitInsuranceResultVo
                    {
                        IsError = true,
                        ErrorType = 3,
                        ErrorMessage = _localizationService.GetResource("BHYT.DichVuVuotMuocHuong")
                    };
                }

                ycTiepNhan.DuyetBaoHiems.Add(duyetBaoHiem);
                BaseRepository.Update(ycTiepNhan);
            }

            return new BenefitInsuranceResultVo { IsError = false };
        }

        public BenefitInsuranceResultVo HuyDuyetBaoHiemYteNoiTru(BenefitInsuranceVo duyetBaoHiemVo)
        {
            var dsXacNhanDuocChiTra = duyetBaoHiemVo.BenefitInsurance;
            var dsXacNhanKhongChiTra = duyetBaoHiemVo.NonBenefitInsurance;
            var dsXacNhan = new List<InsuranceConfirmVo>();
            dsXacNhan.AddRange(dsXacNhanDuocChiTra.Where(o => o.CheckedDefault));
            dsXacNhan.AddRange(dsXacNhanKhongChiTra.Where(o => o.CheckedDefault));

            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var ycTiepNhan =
                BaseRepository.GetById(duyetBaoHiemVo.IdYeuCauTiepNhan,
                    x => x.Include(o => o.NoiTruBenhAn).Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                        .Include(o => o.YeuCauKhamBenhs).Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.YeuCauDichVuGiuongBenhViens)
                        .Include(o => o.YeuCauDuocPhamBenhViens)
                        .Include(o => o.YeuCauVatTuBenhViens)
                        .Include(o => o.YeuCauTruyenMaus)
                        .Include(o => o.YeuCauTiepNhanTheBHYTs)
                        .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                        .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets));

            var ycTiepNhanNgoaiTru = ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId == null ? null :
                BaseRepository.GetById(ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value,
                    x => x.Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                        .Include(o => o.YeuCauKhamBenhs).Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.YeuCauDuocPhamBenhViens)
                        .Include(o => o.YeuCauVatTuBenhViens)
                        .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets));

            foreach (var insuranceConfirmVo in dsXacNhan)
            {
                switch (insuranceConfirmVo.GroupType)
                {
                    case Enums.EnumNhomGoiDichVu.DichVuKhamBenh:

                        var yeuCauKhamBenh = ycTiepNhan.YeuCauKhamBenhs.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauKhamBenhs.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);
                        if (yeuCauKhamBenh == null)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
                        }

                        if (yeuCauKhamBenh.BaoHiemChiTra != null)
                        {
                            yeuCauKhamBenh.BaoHiemChiTra = null;
                            yeuCauKhamBenh.ThoiDiemDuyetBaoHiem = null;
                            yeuCauKhamBenh.NhanVienDuyetBaoHiemId = null;
                            foreach (var duyetBaoHiemChiTiet in yeuCauKhamBenh.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuKyThuat:
                        var yckt = ycTiepNhan.YeuCauDichVuKyThuats.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauDichVuKyThuats.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);
                        if (yckt == null)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
                        }

                        if (yckt.BaoHiemChiTra != null)
                        {
                            yckt.BaoHiemChiTra = null;
                            yckt.ThoiDiemDuyetBaoHiem = null;
                            yckt.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in yckt.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;

                    case Enums.EnumNhomGoiDichVu.TruyenMau:
                        var yctm = ycTiepNhan.YeuCauTruyenMaus.First(o => o.Id == insuranceConfirmVo.Id);
                        if (yctm.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
                        }
                        if (yctm.BaoHiemChiTra != null)
                        {
                            yctm.BaoHiemChiTra = null;
                            yctm.ThoiDiemDuyetBaoHiem = null;
                            yctm.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in yctm.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuGiuongBenh:
                        var ycg = ycTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.First(o => o.Id == insuranceConfirmVo.Id);
                        if (ycg.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
                        }
                        if (ycg.BaoHiemChiTra != null)
                        {
                            ycg.BaoHiemChiTra = null;
                            ycg.ThoiDiemDuyetBaoHiem = null;
                            ycg.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in ycg.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;

                    case Enums.EnumNhomGoiDichVu.DuocPham:
                        var ycdp = ycTiepNhan.YeuCauDuocPhamBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauDuocPhamBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);

                        if (ycdp == null)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
                        }
                        if (ycdp.BaoHiemChiTra != null)
                        {
                            ycdp.BaoHiemChiTra = null;
                            ycdp.ThoiDiemDuyetBaoHiem = null;
                            ycdp.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in ycdp.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.VatTuTieuHao:
                        var ycvt = ycTiepNhan.YeuCauVatTuBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.YeuCauVatTuBenhViens.FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);

                        if (ycvt == null)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
                        }
                        if (ycvt.BaoHiemChiTra != null)
                        {
                            ycvt.BaoHiemChiTra = null;
                            ycvt.ThoiDiemDuyetBaoHiem = null;
                            ycvt.NhanVienDuyetBaoHiemId = null;

                            foreach (var duyetBaoHiemChiTiet in ycvt.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }
                        break;
                    case Enums.EnumNhomGoiDichVu.DonThuocThanhToan:
                        var ycdt = ycTiepNhan.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).FirstOrDefault(o => o.Id == insuranceConfirmVo.Id) ?? ycTiepNhanNgoaiTru?.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).FirstOrDefault(o => o.Id == insuranceConfirmVo.Id);

                        if (ycdt == null)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = _localizationService.GetResource("BHYT.Confirm.NotValid") };
                        }
                        if (ycdt.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            return new BenefitInsuranceResultVo { IsError = true, ErrorType = 3, ErrorMessage = "Không thể hủy duyệt dịch vụ đã thanh toán" };
                        }

                        if (ycdt.BaoHiemChiTra != null)
                        {
                            ycdt.BaoHiemChiTra = null;
                            ycdt.ThoiDiemDuyetBaoHiem = null;
                            ycdt.NhanVienDuyetBaoHiemId = null;
                            foreach (var duyetBaoHiemChiTiet in ycdt.DuyetBaoHiemChiTiets)
                            {
                                duyetBaoHiemChiTiet.WillDelete = true;
                            }
                        }

                        break;
                }

            }

            if (dsXacNhan.Any())
            {
                BaseRepository.Update(ycTiepNhan);
            }

            return new BenefitInsuranceResultVo { IsError = false };
        }
    }
}
