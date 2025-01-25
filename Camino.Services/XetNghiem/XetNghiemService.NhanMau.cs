using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.XetNghiem
{
    public partial class XetNghiemService
    {
        private IQueryable<NhanMauDanhSachXetNghiemGridVo> DataDanhSachPhieuNhanMauXetNghiem(bool? tinhTrang, QueryInfo queryInfo)
        {
            //var queryMauXetNghiem = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId != null && p.PhieuGoiMauXetNghiem.DaNhanMau == tinhTrang)
            //                                                               .Select(p => new NhanMauDanhSachNhomXetNghiemGridVo
            //                                                               {
            //                                                                   Id = p.Id,
            //                                                                   PhieuGoiMauXetNghiemId = p.PhieuGoiMauXetNghiemId.Value,
            //                                                                   SoPhieu = p.PhieuGoiMauXetNghiem.SoPhieu,
            //                                                                   PhienXetNghiemId = p.PhienXetNghiemId,
            //                                                                   NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
            //                                                                   NhomDichVuBenhVienDisplay = p.NhomDichVuBenhVien.Ten,
            //                                                                   Barcode = p.BarCodeId,
            //                                                                   BarCodeNumber = p.BarCodeNumber + "",
            //                                                                   MaTiepNhan = p.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //                                                                   MaBenhNhan = p.PhienXetNghiem.BenhNhan.MaBN,
            //                                                                   HoTen = p.PhienXetNghiem.BenhNhan.HoTen,
            //                                                                   NamSinh = p.PhienXetNghiem.BenhNhan.NamSinh,
            //                                                                   GioiTinh = p.PhienXetNghiem.BenhNhan.GioiTinh
            //                                                               });

            //queryMauXetNghiem = queryMauXetNghiem.ApplyLike(queryInfo.SearchTerms, p => p.SoPhieu, p => p.MaBenhNhan, p => p.MaTiepNhan, p => p.HoTen, p => p.Barcode, p => p.BarCodeNumber);

            //var lstPhieuGoiMauXetNghiemIds = queryMauXetNghiem.GroupBy(p => p.PhieuGoiMauXetNghiemId).ToList();

            //var query = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => lstPhieuGoiMauXetNghiemIds.Any(p2 => p2.Key == p.Id))
            //                                                           .Select(p => new NhanMauDanhSachXetNghiemGridVo
            //                                                           {
            //                                                               Id = p.Id,
            //                                                               SoPhieu = p.SoPhieu,
            //                                                               NguoiGoiMauId = p.NhanVienGoiMauId,
            //                                                               NguoiGoiMauDisplay = p.NhanVienGoiMau.User.HoTen,
            //                                                               NgayGoiMau = p.ThoiDiemGoiMau,
            //                                                               SoLuongMau = "",
            //                                                               TinhTrang = p.DaNhanMau,
            //                                                               NoiTiepNhanId = p.PhongNhanMauId,
            //                                                               NoiTiepNhanDisplay = p.PhongNhanMau.Ten,
            //                                                               NguoiNhanMauId = p.NhanVienNhanMauId,
            //                                                               NguoiNhanMauDisplay = p.NhanVienNhanMau.User.HoTen,
            //                                                               NgayNhanMau = p.ThoiDiemNhanMau
            //                                                           })
            //                                                           .ApplyLike(queryInfo.SearchTerms, p => p.NguoiGoiMauDisplay, p => p.NguoiNhanMauDisplay)
            //                                                           .OrderBy((queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")) ? queryInfo.SortString : "SoPhieu asc");

            var queryMauXetNghiem = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId != null && p.PhieuGoiMauXetNghiem.DaNhanMau == tinhTrang)
                                                                           .ApplyLike(queryInfo.SearchTerms,
                                                                                        p => p.PhieuGoiMauXetNghiem.SoPhieu,
                                                                                        p => p.PhienXetNghiem.BenhNhan.MaBN,
                                                                                        p => p.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                                        p => p.PhienXetNghiem.BenhNhan.HoTen,
                                                                                        p => p.BarCodeId,
                                                                                        p => p.BarCodeNumber + "")
                                                                           .GroupBy(p => p.PhieuGoiMauXetNghiemId)
                                                                           .Select(p => p.First().PhieuGoiMauXetNghiem);

            var queryPhieuNhanMauXetNghiem = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.DaNhanMau == tinhTrang)
                                                                                            .ApplyLike(queryInfo.SearchTerms, p => p.NhanVienGoiMau.User.HoTen, p => p.NhanVienNhanMau.User.HoTen, p => p.PhongNhanMau.Ten);

            var query = queryPhieuNhanMauXetNghiem.Union(queryMauXetNghiem)
                                                  .Select(p => new NhanMauDanhSachXetNghiemGridVo
                                                  {
                                                      Id = p.Id,
                                                      SoPhieu = p.SoPhieu,
                                                      NguoiGoiMauId = p.NhanVienGoiMauId,
                                                      NguoiGoiMauDisplay = p.NhanVienGoiMau.User.HoTen,
                                                      NgayGoiMau = p.ThoiDiemGoiMau,
                                                      SoLuongMau = "",
                                                      TinhTrang = p.DaNhanMau,
                                                      NoiTiepNhanId = p.PhongNhanMauId,
                                                      NoiTiepNhanDisplay = p.PhongNhanMau.Ten,
                                                      NguoiNhanMauId = p.NhanVienNhanMauId,
                                                      NguoiNhanMauDisplay = p.NhanVienNhanMau.User.HoTen,
                                                      NgayNhanMau = p.ThoiDiemNhanMau
                                                  })
                                                  .OrderBy((queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")) ? queryInfo.SortString : "SoPhieu asc");

            return query;
        }

        public async Task<GridDataSource> GetDanhSachNhanMauXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new NhanMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhanMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var queryChoNhanMau = DataDanhSachPhieuNhanMauXetNghiem(null, queryInfo);
            var queryDaNhanMau = DataDanhSachPhieuNhanMauXetNghiem(true, queryInfo);
            var query = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new NhanMauDanhSachXetNghiemGridVo()).AsQueryable();

            if (queryObject.ChoNhanMau == false && queryObject.DaNhanMau == false)
            {
                queryObject.ChoNhanMau = true;
                queryObject.DaNhanMau = true;
            }

            var isHaveQuery = false;
            if (queryObject.ChoNhanMau)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryChoNhanMau);
                }
                else
                {
                    query = queryChoNhanMau;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaNhanMau)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaNhanMau);
                }
                else
                {
                    query = queryDaNhanMau;
                    isHaveQuery = true;
                }
            }

            if (queryObject != null)
            {
                if (queryObject.RangeNgayGoiMau.startDate != null)
                {
                    query = query.Where(p => p.NgayGoiMau.Date >= queryObject.RangeNgayGoiMau.startDate.Value.Date);
                }

                if (queryObject.RangeNgayGoiMau.endDate != null)
                {
                    query = query.Where(p => p.NgayGoiMau.Date <= queryObject.RangeNgayGoiMau.endDate.Value.Date);
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("SoPhieu asc") && (queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var queryTask = query.Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            foreach (var item in queryTask.Result)
            {
                item.SoLuongMau = await TinhSoLuongMauGoiMauXetNghiem(queryInfo, item.Id);
                item.SoLuongMauCoTheTuChoi = await TinhSoLuongMauCoTheTuChoi(item.Id);
            }

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachNhanMauXetNghiemForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new NhanMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhanMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var queryChoNhanMau = DataDanhSachPhieuNhanMauXetNghiem(null, queryInfo);
            var queryDaNhanMau = DataDanhSachPhieuNhanMauXetNghiem(true, queryInfo);
            var query = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new NhanMauDanhSachXetNghiemGridVo()).AsQueryable();

            if (queryObject.ChoNhanMau == false && queryObject.DaNhanMau == false)
            {
                queryObject.ChoNhanMau = true;
                queryObject.DaNhanMau = true;
            }

            var isHaveQuery = false;
            if (queryObject.ChoNhanMau)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryChoNhanMau);
                }
                else
                {
                    query = queryChoNhanMau;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaNhanMau)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaNhanMau);
                }
                else
                {
                    query = queryDaNhanMau;
                    isHaveQuery = true;
                }
            }

            if (queryObject != null)
            {
                if (queryObject.RangeNgayGoiMau.startDate != null)
                {
                    query = query.Where(p => p.NgayGoiMau.Date >= queryObject.RangeNgayGoiMau.startDate.Value.Date);
                }

                if (queryObject.RangeNgayGoiMau.endDate != null)
                {
                    query = query.Where(p => p.NgayGoiMau.Date <= queryObject.RangeNgayGoiMau.endDate.Value.Date);
                }
            }

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetDanhSachNhanMauNhomXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new NhanMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhanMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var query = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId == queryObject.PhieuGoiMauXetNghiemId.GetValueOrDefault())
                                                               .Select(p => new NhanMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
                                                                   PhieuGoiMauXetNghiemId = p.PhieuGoiMauXetNghiemId,
                                                                   SoPhieu = p.PhieuGoiMauXetNghiem.SoPhieu,
                                                                   PhienXetNghiemId = p.PhienXetNghiemId,
                                                                   NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
                                                                   NhomDichVuBenhVienDisplay = p.NhomDichVuBenhVien.Ten,
                                                                   Barcode = p.BarCodeId,
                                                                   BarCodeNumber = p.BarCodeNumber + "",
                                                                   MaTiepNhan = p.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                   MaBenhNhan = p.PhienXetNghiem.BenhNhan.MaBN,
                                                                   HoTen = p.PhienXetNghiem.BenhNhan.HoTen,
                                                                   NamSinh = p.PhienXetNghiem.BenhNhan.NamSinh,
                                                                   GioiTinh = p.PhienXetNghiem.BenhNhan.GioiTinh,
                                                                   SoLuongMauCoTheTuChoi = p.DatChatLuong == false ? 0 : 1,
                                                                   LoaiMauXetNghiem = new LoaiMauNhanMauXetNghiemVo()
                                                                   {
                                                                       LoaiMau = p.LoaiMauXetNghiem,
                                                                       DatChatLuong = p.DatChatLuong,
                                                                       NguoiTuChoiId = p.NhanVienXetKhongDatId,
                                                                       NguoiTuChoiDisplay = p.NhanVienXetKhongDat.User.HoTen,
                                                                       NgayTuChoi = p.ThoiDiemXetKhongDat,
                                                                       LyDoTuChoi = p.LyDoKhongDat
                                                                   }
                                                               })
                                                               .GroupBy(p => new { p.PhienXetNghiemId, p.NhomDichVuBenhVienId, p.MaTiepNhan })
                                                               .Select(p => new NhanMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
                                                                   PhieuGoiMauXetNghiemId = p.First().PhieuGoiMauXetNghiemId,
                                                                   SoPhieu = p.First().SoPhieu,
                                                                   PhienXetNghiemId = p.First().PhienXetNghiemId,
                                                                   NhomDichVuBenhVienId = p.First().NhomDichVuBenhVienId,
                                                                   NhomDichVuBenhVienDisplay = p.First().NhomDichVuBenhVienDisplay,
                                                                   Barcode = p.First().Barcode,
                                                                   BarCodeNumber = p.First().BarCodeNumber,
                                                                   MaTiepNhan = p.First().MaTiepNhan,
                                                                   MaBenhNhan = p.First().MaBenhNhan,
                                                                   HoTen = p.First().HoTen,
                                                                   NamSinh = p.First().NamSinh,
                                                                   GioiTinh = p.First().GioiTinh,
                                                                   SoLuongMauCoTheTuChoi = p.Sum(p2 => p2.SoLuongMauCoTheTuChoi),
                                                                   LoaiMauXetNghiems = p.Select(p2 => p2.LoaiMauXetNghiem)
                                                                                        .Where(p2 => p2.LoaiMau != null)
                                                                                        .GroupBy(p2 => new { p2.LoaiMau })
                                                                                        .Select(p2 => new LoaiMauNhanMauXetNghiemVo()
                                                                                        {
                                                                                            LoaiMau = p2.First().LoaiMau,
                                                                                            DatChatLuong = p2.First().DatChatLuong,
                                                                                            NguoiTuChoiId = p2.First().NguoiTuChoiId,
                                                                                            NguoiTuChoiDisplay = p2.First().NguoiTuChoiDisplay,
                                                                                            NgayTuChoi = p2.First().NgayTuChoi,
                                                                                            LyDoTuChoi = p2.First().LyDoTuChoi
                                                                                        })
                                                                                        .ToList()
                                                               });

            var tempQuery = query;

            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, p => p.SoPhieu, p => p.MaBenhNhan, p => p.MaTiepNhan, p => p.HoTen, p => p.Barcode, p => p.BarCodeNumber);
            }

            if (query.Count() == 0)
            {
                query = tempQuery;
            }

            var sortString = exportExcel == false && (queryInfo.SortString != null && !queryInfo.SortString.Equals("MaTiepNhan asc,NhomDichVuBenhVienId asc") && !queryInfo.SortString.Equals("Id") && (queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc"))) ? queryInfo.SortString : "MaTiepNhan asc,NhomDichVuBenhVienId asc";

            var queryTask = query.OrderBy(sortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachNhanMauNhomXetNghiemForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new NhanMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhanMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var query = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId == queryObject.PhieuGoiMauXetNghiemId)
                                                               .Select(p => new NhanMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
                                                                   PhieuGoiMauXetNghiemId = p.PhieuGoiMauXetNghiemId,
                                                                   SoPhieu = p.PhieuGoiMauXetNghiem.SoPhieu,
                                                                   PhienXetNghiemId = p.PhienXetNghiemId,
                                                                   NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
                                                                   NhomDichVuBenhVienDisplay = p.NhomDichVuBenhVien.Ten,
                                                                   Barcode = p.BarCodeId,
                                                                   BarCodeNumber = p.BarCodeNumber + "",
                                                                   MaTiepNhan = p.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                   MaBenhNhan = p.PhienXetNghiem.BenhNhan.MaBN,
                                                                   HoTen = p.PhienXetNghiem.BenhNhan.HoTen,
                                                                   NamSinh = p.PhienXetNghiem.BenhNhan.NamSinh,
                                                                   GioiTinh = p.PhienXetNghiem.BenhNhan.GioiTinh,
                                                                   SoLuongMauCoTheTuChoi = p.DatChatLuong == false ? 0 : 1,
                                                                   LoaiMauXetNghiem = new LoaiMauNhanMauXetNghiemVo()
                                                                   {
                                                                       LoaiMau = p.LoaiMauXetNghiem,
                                                                       DatChatLuong = p.DatChatLuong,
                                                                       NguoiTuChoiId = p.NhanVienXetKhongDatId,
                                                                       NguoiTuChoiDisplay = p.NhanVienXetKhongDat.User.HoTen,
                                                                       NgayTuChoi = p.ThoiDiemXetKhongDat,
                                                                       LyDoTuChoi = p.LyDoKhongDat
                                                                   }
                                                               })
                                                               .GroupBy(p => new { p.PhienXetNghiemId, p.NhomDichVuBenhVienId, p.MaTiepNhan })
                                                               .Select(p => new NhanMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
                                                                   PhieuGoiMauXetNghiemId = p.First().PhieuGoiMauXetNghiemId,
                                                                   SoPhieu = p.First().SoPhieu,
                                                                   PhienXetNghiemId = p.First().PhienXetNghiemId,
                                                                   NhomDichVuBenhVienId = p.First().NhomDichVuBenhVienId,
                                                                   NhomDichVuBenhVienDisplay = p.First().NhomDichVuBenhVienDisplay,
                                                                   Barcode = p.First().Barcode,
                                                                   BarCodeNumber = p.First().BarCodeNumber,
                                                                   MaTiepNhan = p.First().MaTiepNhan,
                                                                   MaBenhNhan = p.First().MaBenhNhan,
                                                                   HoTen = p.First().HoTen,
                                                                   NamSinh = p.First().NamSinh,
                                                                   GioiTinh = p.First().GioiTinh,
                                                                   SoLuongMauCoTheTuChoi = p.Sum(p2 => p2.SoLuongMauCoTheTuChoi),
                                                                   LoaiMauXetNghiems = p.Select(p2 => p2.LoaiMauXetNghiem)
                                                                                        .Where(p2 => p2.LoaiMau != null)
                                                                                        .GroupBy(p2 => new { p2.LoaiMau })
                                                                                        .Select(p2 => new LoaiMauNhanMauXetNghiemVo()
                                                                                        {
                                                                                            LoaiMau = p2.First().LoaiMau,
                                                                                            DatChatLuong = p2.First().DatChatLuong,
                                                                                            NguoiTuChoiId = p2.First().NguoiTuChoiId,
                                                                                            NguoiTuChoiDisplay = p2.First().NguoiTuChoiDisplay,
                                                                                            NgayTuChoi = p2.First().NgayTuChoi,
                                                                                            LyDoTuChoi = p2.First().LyDoTuChoi
                                                                                        })
                                                                                        .ToList()
                                                               });

            var tempQuery = query;

            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, p => p.SoPhieu, p => p.MaBenhNhan, p => p.MaTiepNhan, p => p.HoTen, p => p.Barcode, p => p.BarCodeNumber);
            }

            if (query.Count() == 0)
            {
                query = tempQuery;
            }

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetDanhSachNhanMauDichVuXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new DichVuXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DichVuXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var query = _phienXetNghiemChiTietRepository.TableNoTracking.Where(p => p.PhienXetNghiemId == queryObject.PhienXetNghiemId && p.NhomDichVuBenhVienId == queryObject.NhomDichVuBenhVienId)
                                                                        .Select(p => new NhanMauDanhSachDichVuXetNghiemGridVo
                                                                        {
                                                                            Id = p.Id,
                                                                            MaDichVu = p.YeuCauDichVuKyThuat.MaDichVu,
                                                                            TenDichVu = p.YeuCauDichVuKyThuat.TenDichVu,
                                                                            ThoiGianChiDinh = p.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                                                                            NguoiChiDinhId = p.YeuCauDichVuKyThuat.NhanVienChiDinhId,
                                                                            NguoiChiDinhDisplay = p.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                                                                            BenhPham = p.YeuCauDichVuKyThuat.BenhPhamXetNghiem,
                                                                            LoaiMau = p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                                                                            LanThucHien = p.LanThucHien,
                                                                            YeuCauDichVuKyThuatId = p.YeuCauDichVuKyThuatId
                                                                        })
                                                                        .OrderByDescending(p => p.LanThucHien)
                                                                        .GroupBy(p => new { p.YeuCauDichVuKyThuatId })
                                                                        .Select(p => new NhanMauDanhSachDichVuXetNghiemGridVo
                                                                        {
                                                                            Id = p.First().Id,
                                                                            MaDichVu = p.First().MaDichVu,
                                                                            TenDichVu = p.First().TenDichVu,
                                                                            ThoiGianChiDinh = p.First().ThoiGianChiDinh,
                                                                            NguoiChiDinhId = p.First().NguoiChiDinhId,
                                                                            NguoiChiDinhDisplay = p.First().NguoiChiDinhDisplay,
                                                                            BenhPham = p.First().BenhPham,
                                                                            LoaiMau = p.First().LoaiMau,
                                                                            LanThucHien = p.First().LanThucHien
                                                                        });

            //var sortString = exportExcel ? "MaDichVu asc" : queryInfo.SortString;
            var sortString = !exportExcel || (queryInfo.SortString != null && !queryInfo.SortString.Equals("MaDichVu asc") && (queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc"))) ? queryInfo.SortString : "MaDichVu asc";

            var queryTask = query.OrderBy(sortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachNhanMauDichVuXetNghiemForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new DichVuXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DichVuXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var query = _phienXetNghiemChiTietRepository.TableNoTracking.Where(p => p.PhienXetNghiemId == queryObject.PhienXetNghiemId && p.NhomDichVuBenhVienId == queryObject.NhomDichVuBenhVienId)
                                                                        .Select(p => new NhanMauDanhSachDichVuXetNghiemGridVo
                                                                        {
                                                                            Id = p.Id,
                                                                            MaDichVu = p.YeuCauDichVuKyThuat.MaDichVu,
                                                                            TenDichVu = p.YeuCauDichVuKyThuat.TenDichVu,
                                                                            ThoiGianChiDinh = p.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                                                                            NguoiChiDinhId = p.YeuCauDichVuKyThuat.NhanVienChiDinhId,
                                                                            NguoiChiDinhDisplay = p.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                                                                            BenhPham = p.YeuCauDichVuKyThuat.BenhPhamXetNghiem,
                                                                            LoaiMau = p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                                                                            LanThucHien = p.LanThucHien,
                                                                            YeuCauDichVuKyThuatId = p.YeuCauDichVuKyThuatId
                                                                        })
                                                                        .OrderByDescending(p => p.LanThucHien)
                                                                        .GroupBy(p => new { p.YeuCauDichVuKyThuatId })
                                                                        .Select(p => new NhanMauDanhSachDichVuXetNghiemGridVo
                                                                        {
                                                                            Id = p.First().Id,
                                                                            MaDichVu = p.First().MaDichVu,
                                                                            TenDichVu = p.First().TenDichVu,
                                                                            ThoiGianChiDinh = p.First().ThoiGianChiDinh,
                                                                            NguoiChiDinhId = p.First().NguoiChiDinhId,
                                                                            NguoiChiDinhDisplay = p.First().NguoiChiDinhDisplay,
                                                                            BenhPham = p.First().BenhPham,
                                                                            LoaiMau = p.First().LoaiMau,
                                                                            LanThucHien = p.First().LanThucHien
                                                                        });

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetDanhSachKhongTiepNhanMau(QueryInfo queryInfo)
        {
            var queryObject = new KhongTiepNhanMauSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<KhongTiepNhanMauSearch>(queryInfo.AdditionalSearchString);
            }

            var query = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId == queryObject.PhieuGoiMauXetNghiemId &&
                                                                           p.NhomDichVuBenhVienId == queryObject.NhomDichVuBenhVienId &&
                                                                           p.PhienXetNghiemId == queryObject.PhienXetNghiemId &&
                                                                           p.DatChatLuong != false)
                                                               .Select(p => new DanhSachKhongTiepNhanMauGridVo
                                                               {
                                                                   Id = p.Id,
                                                                   LoaiMauXetNghiem = p.LoaiMauXetNghiem,
                                                                   //DatChatLuong = p.DatChatLuong,
                                                                   KhongDatChatLuong = !p.DatChatLuong,
                                                                   LyDoKhongDat = p.LyDoKhongDat
                                                               });

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<int> SoLuongMauCoTheTuChoi(long phieuGoiMauXetNghiemId, long nhomDichVuBenhVienId, long phienXetNghiemId)
        {
            return await _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId == phieuGoiMauXetNghiemId &&
                                                                            p.NhomDichVuBenhVienId == nhomDichVuBenhVienId &&
                                                                            p.PhienXetNghiemId == phienXetNghiemId &&
                                                                            p.DatChatLuong != false)
                                                                .CountAsync();
        }

        public async Task TuChoiMau(long mauXetNghiemId, long nhanVienXetKhongDatId, string lyDoTuChoi)
        {
            var mauXetNghiem = await _mauXetNghiemRepository.TableNoTracking.Where(p => p.Id == mauXetNghiemId)
                                                                            .FirstOrDefaultAsync();

            if(mauXetNghiem != null)
            {
                mauXetNghiem.DatChatLuong = false;
                mauXetNghiem.NhanVienXetKhongDatId = nhanVienXetKhongDatId;
                mauXetNghiem.ThoiDiemXetKhongDat = DateTime.Now;
                mauXetNghiem.LyDoKhongDat = lyDoTuChoi;

                await _mauXetNghiemRepository.UpdateAsync(mauXetNghiem);
            }
        }

        public async Task<int> TinhSoLuongMauCoTheTuChoi(long phieuGoiMauId)
        {
            var query = await _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId == phieuGoiMauId)
                                                                     .Select(p => new NhanMauDanhSachNhomXetNghiemGridVo
                                                                     {
                                                                         //Id = p.Id,
                                                                         PhienXetNghiemId = p.PhienXetNghiemId,
                                                                         NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
                                                                         MaTiepNhan = p.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                         SoLuongMauCoTheTuChoi = p.DatChatLuong == false ? 0 : 1
                                                                     })
                                                                     .GroupBy(p => new { p.PhienXetNghiemId, p.NhomDichVuBenhVienId, p.MaTiepNhan })
                                                                     .Select(p => new NhanMauDanhSachNhomXetNghiemGridVo
                                                                     {
                                                                         //Id = p.Id,
                                                                         PhienXetNghiemId = p.First().PhienXetNghiemId,
                                                                         NhomDichVuBenhVienId = p.First().NhomDichVuBenhVienId,
                                                                         MaTiepNhan = p.First().MaTiepNhan,
                                                                         SoLuongMauCoTheTuChoi = p.Sum(p2 => p2.SoLuongMauCoTheTuChoi)
                                                                     })
                                                                     .ToListAsync();

            return query?.Select(p => p.SoLuongMauCoTheTuChoi).Sum() ?? 0;
        }
    }
}
