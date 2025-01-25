using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.XacNhanBhytDaHoanThanh
{
    [ScopedDependency(ServiceType = typeof(IXacNhanBhytDaHoanThanhListService))]
    public class XacNhanBhytDaHoanThanhListService : MasterFileService<YeuCauTiepNhan>, IXacNhanBhytDaHoanThanhListService
    {
        public XacNhanBhytDaHoanThanhListService(IRepository<YeuCauTiepNhan> repository) : base(repository)
        { }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            DateTime tuNgay = DateTime.Now.AddYears(-1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachBHYTJson>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.ThoiDiemDuyetBaoHiemTu))
                {
                    DateTime.TryParse(queryString.ThoiDiemDuyetBaoHiemTu, out tuNgay);
                }
                if (!string.IsNullOrEmpty(queryString.ThoiDiemDuyetBaoHiemDen))
                {
                    DateTime.TryParse(queryString.ThoiDiemDuyetBaoHiemDen, out denNgay);
                }
            }

            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat && p.CoBHYT == true && p.ThoiDiemTiepNhan > tuNgay)
                .Select(source => new ListXacNhanBhytDaHoanThanhGridVo
                {
                    Id = source.Id,
                    MaTn = source.MaYeuCauTiepNhan,
                    MaBn = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NamSinh = DateHelper.DOBFormat(source.NgaySinh, source.ThangSinh, source.NamSinh),
                    GioiTinh = source.GioiTinh.GetDescription(),
                    DiaChi = source.DiaChiDayDu,
                    SoDienThoai = source.SoDienThoai,
                    SoDienThoaiDisplay = source.SoDienThoaiDisplay,
                    ThoiDiemDuyetBaoHiems = source.DuyetBaoHiems.Select(o=>o.ThoiDiemDuyetBaoHiem).ToList()
                    //ThoiDiemDuyetBaoHiem = source.DuyetBaoHiems
                    //                             .OrderByDescending(c => c.ThoiDiemDuyetBaoHiem)
                    //                             .Select(c => c.ThoiDiemDuyetBaoHiem).FirstOrDefault(),
                });
            //query = query.Where(p => p.ThoiDiemDuyetBaoHiem >= tuNgay && p.ThoiDiemDuyetBaoHiem <= denNgay);
            

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTn,
                    g => g.MaBn,
                    g => g.SoDienThoaiDisplay,
                    g => g.SoDienThoai
                );
            }

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString)
            //                     .Skip(queryInfo.Skip)
            //                     .Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);


            var queryResult = query.ToList();

            var allData = queryResult.Where(o => o.ThoiDiemDuyetBaoHiem != null && o.ThoiDiemDuyetBaoHiem.Value > tuNgay && o.ThoiDiemDuyetBaoHiem.Value < denNgay);
            var gridData = allData.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            if (gridData.Any())
            {
                var yeuCauTiepNhanIds = gridData.Select(o => o.Id).ToList();
                var danhSachDichVuDuocHuongBhyTs = BaseRepository.TableNoTracking
                .Where(tn => yeuCauTiepNhanIds.Contains(tn.Id)).Select(tn =>
                    new ListDichVuDuocHuongBhytHoanThanhVo
                    {
                        YeuCauTiepNhanId = tn.Id,
                        DichVuKhamBenhDuocHuongBhyt = tn.YeuCauKhamBenhs
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBhytHoanThanhVo
                            {
                                Soluong = 1,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DichVuKyThuatDuocHuongBhyt = tn.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        o.DuocHuongBaoHiem).Select(yc => new DichVuDuocHuongBhytHoanThanhVo
                                        {
                                            Soluong = yc.SoLan,
                                            BaoHiemChiTra = yc.BaoHiemChiTra,
                                            DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                            TiLeDv = yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                            MucHuong = yc.MucHuongBaoHiem.GetValueOrDefault()
                                        }).ToList(),
                        DichVuGiuongDuocHuongBhyt = tn.YeuCauDichVuGiuongBenhVienChiPhiBHYTs
                            .Where(o => o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBhytHoanThanhVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        DuocPhamDuocHuongBhyt = tn.YeuCauDuocPhamBenhViens
                            .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBhytHoanThanhVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList(),
                        ToaThuocDuocHuongBhyt = tn.DonThuocThanhToans
                            .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT &&
                                        o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                            .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(ct => ct.DuocHuongBaoHiem).Select(yc =>
                                new DichVuDuocHuongBhytHoanThanhVo
                                {
                                    Soluong = yc.SoLuong,
                                    BaoHiemChiTra = yc.BaoHiemChiTra,
                                    DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeDv = yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuong = yc.MucHuongBaoHiem.GetValueOrDefault()
                                }).ToList(),
                        VatTuBenhVienDuocHuongBhyt = tn.YeuCauVatTuBenhViens
                            .Where(o => o.YeuCauGoiDichVuId == null &&
                                        o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem)
                            .Select(yc => new DichVuDuocHuongBhytHoanThanhVo
                            {
                                Soluong = yc.SoLuong,
                                BaoHiemChiTra = yc.BaoHiemChiTra,
                                DgThamKhao = yc.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeDv = yc.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yc.MucHuongBaoHiem.GetValueOrDefault()
                            }).ToList()
                    }
                ).ToList();
                foreach (var danhSachChoGridVo in gridData)
                {
                    danhSachChoGridVo.DanhSachDichVuDuocHuongBhyt = danhSachDichVuDuocHuongBhyTs.FirstOrDefault(o => o.YeuCauTiepNhanId == danhSachChoGridVo.Id);
                }
            }

            return new GridDataSource
            {
                Data = gridData,
                TotalRowCount = allData.Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat
                                && p.CoBHYT == true && p.DuyetBaoHiems.Any())
                .Select(source => new ListXacNhanBhytDaHoanThanhGridVo
                {
                    Id = source.Id,
                    MaTn = source.MaYeuCauTiepNhan,
                    MaBn = source.BenhNhan.MaBN,
                    HoTen = source.HoTen,
                    NamSinh = DateHelper.DOBFormat(source.NgaySinh, source.ThangSinh, source.NamSinh),
                    GioiTinh = source.GioiTinh.GetDescription(),
                    DiaChi = source.DiaChiDayDu,
                    SoDienThoai = source.SoDienThoai,
                    SoDienThoaiDisplay = source.SoDienThoaiDisplay
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachBHYTJson>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);

                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemDuyetBaoHiem >= tuNgay && p.ThoiDiemDuyetBaoHiem <= denNgay);
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaTn,
                    g => g.MaBn,
                    g => g.NamSinh,
                    g => g.DiaChi,
                    g => g.SoDienThoaiDisplay,
                    g => g.SoDienThoai
                );
            }

            return new GridDataSource
            {
                TotalRowCount = await query.CountAsync()
            };
        }

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) &&
                queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?
                    .Replace("Format", "");
            }
        }
    }
}
