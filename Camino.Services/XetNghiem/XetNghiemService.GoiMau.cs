using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using System;
using Camino.Core.Domain.Entities.XetNghiems;
using System.Collections.Generic;

namespace Camino.Services.XetNghiem
{
    public partial class XetNghiemService
    {
        private IQueryable<GoiMauDanhSachXetNghiemGridVo> DataDanhSachPhieuGoiMauXetNghiem(bool? tinhTrang, QueryInfo queryInfo)
        {
            //var queryMauXetNghiem = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId != null && p.PhieuGoiMauXetNghiem.DaNhanMau == tinhTrang)
            //                                                               .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
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
            //                                                           .Select(p => new GoiMauDanhSachXetNghiemGridVo
            //                                                           {
            //                                                               Id = p.Id,
            //                                                               SoPhieu = p.SoPhieu,
            //                                                               NguoiGoiMauId = p.NhanVienGoiMauId,
            //                                                               NguoiGoiMauDisplay = p.NhanVienGoiMau.User.HoTen,
            //                                                               NgayGoiMau = p.ThoiDiemGoiMau,
            //                                                               //SoLuongMau = "",
            //                                                               TinhTrang = p.DaNhanMau,
            //                                                               NoiTiepNhanId = p.PhongNhanMauId,
            //                                                               NoiTiepNhan = p.PhongNhanMau.Ten,
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

            var queryPhieuGoiMauXetNghiem = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.DaNhanMau == tinhTrang)
                                                                                           .ApplyLike(queryInfo.SearchTerms, p => p.NhanVienGoiMau.User.HoTen, p => p.NhanVienNhanMau.User.HoTen, p => p.PhongNhanMau.Ten);

            var query = queryPhieuGoiMauXetNghiem.Union(queryMauXetNghiem)
                                                 .Select(p => new GoiMauDanhSachXetNghiemGridVo
                                                 {
                                                     Id = p.Id,
                                                     SoPhieu = p.SoPhieu,
                                                     NguoiGoiMauId = p.NhanVienGoiMauId,
                                                     NguoiGoiMauDisplay = p.NhanVienGoiMau.User.HoTen,
                                                     NgayGoiMau = p.ThoiDiemGoiMau,
                                                     //SoLuongMau = "",
                                                     TinhTrang = p.DaNhanMau,
                                                     NoiTiepNhanId = p.PhongNhanMauId,
                                                     NoiTiepNhan = p.PhongNhanMau.Ten,
                                                     NguoiNhanMauId = p.NhanVienNhanMauId,
                                                     NguoiNhanMauDisplay = p.NhanVienNhanMau.User.HoTen,
                                                     NgayNhanMau = p.ThoiDiemNhanMau
                                                 })
                                                 .OrderBy((queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")) ? queryInfo.SortString : "SoPhieu asc");

            return query;
        }

        public async Task<GridDataSource> GetDanhSachGoiMauXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new GoiMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<GoiMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var queryChoNhanMau = DataDanhSachPhieuGoiMauXetNghiem(null, queryInfo);
            var queryDaNhanMau = DataDanhSachPhieuGoiMauXetNghiem(true, queryInfo);
            var query = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new GoiMauDanhSachXetNghiemGridVo()).AsQueryable();

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

            //foreach (var item in queryTask.Result)
            //{
            //    item.SoLuongMau = await TinhSoLuongMauGoiMauXetNghiem(queryInfo, item.Id);
            //}

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachGoiMauXetNghiemForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new GoiMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<GoiMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var queryChoNhanMau = DataDanhSachPhieuGoiMauXetNghiem(null, queryInfo);
            var queryDaNhanMau = DataDanhSachPhieuGoiMauXetNghiem(true, queryInfo);
            var query = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new GoiMauDanhSachXetNghiemGridVo()).AsQueryable();

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

        public async Task<GridDataSource> GetDanhSachGoiMauNhomXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new GoiMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<GoiMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            //long.TryParse(queryInfo.AdditionalSearchString, out long phieuGoiMauXetNghiemId);
            var query = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId == queryObject.PhieuGoiMauXetNghiemId.GetValueOrDefault())
                                                               .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
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
                                                               .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
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

            //if (queryInfo.SortString != null && !queryInfo.SortString.Equals("SoPhieu asc") && (queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")))
            //{
            //    query = query.OrderBy(queryInfo.SortString);
            //}

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

        public async Task<GridDataSource> GetTotalPagesDanhSachGoiMauNhomXetNghiemForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new GoiMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<GoiMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            //long.TryParse(queryInfo.AdditionalSearchString, out long phieuGoiMauXetNghiemId);

            var query = _mauXetNghiemRepository.TableNoTracking.Where(p => p.PhieuGoiMauXetNghiemId == queryObject.PhieuGoiMauXetNghiemId)
                                                               .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
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
                                                               .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                               {
                                                                   //Id = p.Id,
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

        public async Task<GridDataSource> GetDanhSachGoiMauDichVuXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel)
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
                                                                        .Select(p => new GoiMauDanhSachDichVuXetNghiemGridVo
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
                                                                        .Select(p => new GoiMauDanhSachDichVuXetNghiemGridVo
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

        public async Task<GridDataSource> GetTotalPagesDanhSachGoiMauDichVuXetNghiemForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new DichVuXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DichVuXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var query = _phienXetNghiemChiTietRepository.TableNoTracking.Where(p => p.PhienXetNghiemId == queryObject.PhienXetNghiemId && p.NhomDichVuBenhVienId == queryObject.NhomDichVuBenhVienId)
                                                                        .Select(p => new GoiMauDanhSachDichVuXetNghiemGridVo
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
                                                                        .Select(p => new GoiMauDanhSachDichVuXetNghiemGridVo
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

        public async Task<string> TinhSoLuongMauGoiMauXetNghiem(QueryInfo queryInfo, long phieuGoiMauId)
        {
            var tongSoLuongMau = await TongSoLuongMauGoiMauXetNghiem(queryInfo, phieuGoiMauId);
            var tongSoLuongMauDaHoanThanh = await SoLuongMauDaHoanThanhGoiMauXetNghiem(queryInfo, phieuGoiMauId);

            return $"{tongSoLuongMauDaHoanThanh}/{tongSoLuongMau}";
        }

        public async Task<int> TongSoLuongMauGoiMauXetNghiem(QueryInfo queryInfo, long phieuGoiMauId)
        {
            var queryObject = new GoiMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<GoiMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var queryMauXetNghiem = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.Id == phieuGoiMauId)
                                                                                   .SelectMany(p => p.MauXetNghiems)
                                                                                   .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                                                   {
                                                                                       //Id = p.Id,
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
                                                                                   .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                                                   {
                                                                                       //Id = p.Id,
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

            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                queryMauXetNghiem = queryMauXetNghiem.ApplyLike(searchTerms, p => p.SoPhieu, p => p.MaBenhNhan, p => p.MaTiepNhan, p => p.HoTen, p => p.Barcode, p => p.BarCodeNumber);
            }

            return queryMauXetNghiem.Count();
        }

        public async Task<int> SoLuongMauDaHoanThanhGoiMauXetNghiem(QueryInfo queryInfo, long phieuGoiMauId)
        {
            var queryObject = new GoiMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<GoiMauXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var queryMauXetNghiem = _phieuGoiMauXetNghiemRepository.TableNoTracking.Where(p => p.Id == phieuGoiMauId)
                                                                                   .SelectMany(p => p.MauXetNghiems)
                                                                                   .Where(p => p.PhienXetNghiem.PhienXetNghiemChiTiets.All(p2 => p2.NhanVienKetLuanId != null))
                                                                                   .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                                                   {
                                                                                       //Id = p.Id,
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
                                                                                   .Select(p => new GoiMauDanhSachNhomXetNghiemGridVo
                                                                                   {
                                                                                       //Id = p.Id,
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

            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                queryMauXetNghiem = queryMauXetNghiem.ApplyLike(searchTerms, p => p.SoPhieu, p => p.MaBenhNhan, p => p.MaTiepNhan, p => p.HoTen, p => p.Barcode, p => p.BarCodeNumber);
            }

            return queryMauXetNghiem.Count();
        }

        public async Task XoaPhieuGoiMauXetNghiem(long phieuGoiMauId)
        {
            var phieuGoiMauXetNghiem = await _phieuGoiMauXetNghiemRepository.GetByIdAsync(phieuGoiMauId, o => o.Include(p => p.MauXetNghiems));

            if (phieuGoiMauXetNghiem.DaNhanMau == true)
            {
                throw new Exception(_localizationService.GetResource("XetNghiem.GoiMau.Xoa.DaNhanMau"));
            }

            //set null
            foreach (var item in phieuGoiMauXetNghiem.MauXetNghiems)
            {
                item.PhieuGoiMauXetNghiemId = null;
            }

            //xoá phiếu gởi mẫu
            phieuGoiMauXetNghiem.WillDelete = true;
            await _phieuGoiMauXetNghiemRepository.UpdateAsync(phieuGoiMauXetNghiem);
        }

        public async Task<PhieuGoiMauXetNghiem> GetPhieuGoiMauXetNghiem(long phieuGoiMauId)
        {
            return await _phieuGoiMauXetNghiemRepository.GetByIdAsync(phieuGoiMauId, o => o.Include(p => p.NhanVienGoiMau).ThenInclude(p => p.User)
                                                                                           .Include(p => p.NhanVienNhanMau).ThenInclude(p => p.User)
                                                                                           .Include(p => p.PhongNhanMau));
        }
        #region search grid popup in xet nghiem
        public async Task<List<DuyetKqXetNghiemChiTietGridVo>> GetDanhSachSearchPopupForGrid(TimKiemPopupInXetNghiemVo model)
        {
            var queryObjects = new List<DuyetKqXetNghiemChiTietGridVo>();
            if (model.DanhSachCanSearchs != null)
            {
              
                queryObjects = JsonConvert.DeserializeObject<List<DuyetKqXetNghiemChiTietGridVo>>(model.DanhSachCanSearchs);
            }

            if(!string.IsNullOrEmpty(model.Searching))
            {
                queryObjects = queryObjects.Where(s => s.Ten.RemoveVietnameseDiacritics().ToLower().Trim().Contains(model.Searching.RemoveVietnameseDiacritics().ToLower().Trim())).ToList();
            }


            return queryObjects;
        }
        #endregion
    }
}