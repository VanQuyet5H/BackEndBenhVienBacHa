using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain;
using System.Globalization;
using Camino.Core.Domain.ValueObject.BenhVien;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject.CauHinh;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachChoKhams(QueryInfo queryInfo)
        {
            //   BuildDefaultSortExpression(queryInfo);
            //   var query = _phongBenhVienHangDoiRepository.TableNoTracking
            //       .Include(o => o.PhongBenhVien)
            //       .Include(o => o.YeuCauTiepNhan).ThenInclude(tn => tn.PhuongXa)
            //       .Include(o => o.YeuCauTiepNhan).ThenInclude(tn => tn.QuanHuyen)
            //       .Include(o => o.YeuCauTiepNhan).ThenInclude(tn => tn.TinhThanh)
            //       .Include(o => o.YeuCauKhamBenh)
            //       .OrderByDescending(s => s.YeuCauKhamBenh.ThoiDiemThucHien)
            //       .Where(o => o.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham && 
            //                   o.YeuCauKhamBenh != null 
            //                   && (o.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham || o.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham))
            //       .Select(s => new KhamBenhDSCKGGridVoItem
            //       {
            //           Id = s.Id,
            //           PhongBenhVienId = s.PhongBenhVienId,
            //           MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //           HoTen = s.YeuCauTiepNhan.HoTen,
            //           NamSinh = s.YeuCauTiepNhan.NamSinh,
            //           DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
            //           TenDiaChi = s.YeuCauTiepNhan.DiaChi,
            //           TrangThai = s.TrangThai,
            //           TrangThaiDisplay = s.TrangThai.GetDescription(),
            //           ThoiDiemTiepNhanDisplay = s.YeuCauTiepNhan.ThoiDiemTiepNhan == null ? null : s.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
            //           ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
            //           TenBenh = s.YeuCauKhamBenh.TenBenh,
            //           DoiTuong = s.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + s.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
            //           //LoaiHangDoi = s.LoaiHangDoi
            //       });

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //   {
            //       var queryString = JsonConvert.DeserializeObject<KhamBenhDSCKGGridVoItem>(queryInfo.AdditionalSearchString);
            //       if ((queryString.PhongBenhVienId) != null)
            //       {
            //           query = query.Where(p => p.PhongBenhVienId == queryString.PhongBenhVienId);
            //       }
            //       if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            //       {
            //           DateTime denNgay;
            //           DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
            //               out var tuNgay);
            //           if (string.IsNullOrEmpty(queryString.ToDate))
            //           {
            //               denNgay = DateTime.Now;
            //           }
            //           else
            //           {
            //               DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //           }
            //           query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay.AddSeconds(59));
            //       }

            //   }
            //   if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            //   {
            //       var queryStrings = JsonConvert.DeserializeObject<KhamBenhDSCKGSearchGridVoItem>(queryInfo.SearchTerms);
            //       if (queryStrings != null)
            //       {
            //           if (queryStrings.PhongBenhVienId != 0 && queryStrings.searchString == null)
            //           {
            //               query = query.Where(p => p.PhongBenhVienId == queryStrings.PhongBenhVienId);
            //           }
            //           if (queryStrings.PhongBenhVienId != 0 && queryStrings.searchString != null)
            //           {
            //               var diachi = queryStrings.searchString.RemoveVietnameseDiacritics().ToLower().TrimStart().TrimEnd();
            //               query = query.Where(p => p.PhongBenhVienId == queryStrings.PhongBenhVienId
            //                                     && (p.HoTen.RemoveUniKeyAndToLower().Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                          || p.NamSinh.ToString().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                          || p.DiaChi.RemoveUniKeyAndToLower()
            //                                               .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                          || p.DiaChiSearch.RemoveUniKeyAndToLower()
            //                                               .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                          || p.DiaChiSearch2.RemoveUniKeyAndToLower()
            //                                               .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                          //|| p.TenDiaChi.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                          //|| p.TenPhuongXa.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                          //|| p.TenQuanHuyen.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                          //|| p.TenTinhThanh.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                          || p.DoiTuong.RemoveUniKeyAndToLower()
            //                                               .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                          //|| p.TenBenh.Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                          //|| p.TenBenh.ToLower().Contains(queryStrings.searchString.ToLower().TrimStart().TrimEnd())
            //                                          //|| p.TenBenh.RemoveVietnameseDiacritics().ToLower().Contains(queryStrings.searchString.ToLower().TrimStart().TrimEnd())
            //                                          ));
            //           }
            //       }
            //   }
            //   //query = query.Where(o => Convert.ToDateTime(o.ThoiDiemTiepNhan).Date == DateTime.Now.Date);
            //   var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //   var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //       .Take(queryInfo.Take).ToArrayAsync();
            //   await Task.WhenAll(countTask, queryTask);
            //   return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            return null;
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachChoKhams(QueryInfo queryInfo)
        {
            //var query = _phongBenhVienHangDoiRepository.TableNoTracking
            // .Include(o => o.PhongBenhVien)
            // .Include(o => o.YeuCauTiepNhan).ThenInclude(tn => tn.PhuongXa)
            // .Include(o => o.YeuCauTiepNhan).ThenInclude(tn => tn.QuanHuyen)
            // .Include(o => o.YeuCauTiepNhan).ThenInclude(tn => tn.TinhThanh)
            // .Include(o => o.YeuCauKhamBenh)
            // .OrderByDescending(s => s.YeuCauKhamBenh.ThoiDiemThucHien)
            // .Where(o => o.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham &&
            //             o.YeuCauKhamBenh != null
            //             && (o.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham || o.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham))
            // .Select(s => new KhamBenhDSCKGGridVoItem
            // {
            //     Id = s.Id,
            //     PhongBenhVienId = s.PhongBenhVienId,
            //     MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //     HoTen = s.YeuCauTiepNhan.HoTen,
            //     NamSinh = s.YeuCauTiepNhan.NamSinh,
            //     DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
            //     TenDiaChi = s.YeuCauTiepNhan.DiaChi,
            //     TenPhuongXa = s.YeuCauTiepNhan.PhuongXa.Ten,
            //     TenQuanHuyen = s.YeuCauTiepNhan.QuanHuyen.Ten,
            //     TenTinhThanh = s.YeuCauTiepNhan.TinhThanh.Ten,
            //     DiaChiSearch = (s.YeuCauTiepNhan.DiaChi ?? "") + " " + (s.YeuCauTiepNhan.PhuongXa.Ten ?? "") + " " + (s.YeuCauTiepNhan.QuanHuyen.Ten ?? "") + " " + (s.YeuCauTiepNhan.TinhThanh.Ten ?? ""),
            //     DiaChiSearch2 = (s.YeuCauTiepNhan.DiaChi ?? "") + ", " + (s.YeuCauTiepNhan.PhuongXa.Ten ?? "") + ", " + (s.YeuCauTiepNhan.QuanHuyen.Ten ?? "") + ", " + (s.YeuCauTiepNhan.TinhThanh.Ten ?? ""),
            //     TrangThai = s.TrangThai,
            //     TrangThaiDisplay = s.TrangThai.GetDescription(),
            //     ThoiDiemTiepNhanDisplay = s.YeuCauTiepNhan.ThoiDiemTiepNhan == null ? null : s.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
            //     ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
            //     TenBenh = s.YeuCauKhamBenh.TenBenh,
            //     DoiTuong = s.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + s.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
            //     //LoaiHangDoi = s.LoaiHangDoi
            // });

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{
            //    var queryString = JsonConvert.DeserializeObject<KhamBenhDSCKGGridVoItem>(queryInfo.AdditionalSearchString);
            //    if ((queryString.PhongBenhVienId) != null)
            //    {
            //        query = query.Where(p => p.PhongBenhVienId == queryString.PhongBenhVienId);
            //    }
            //    if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            //    {
            //        DateTime denNgay;
            //        DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
            //            out var tuNgay);
            //        if (string.IsNullOrEmpty(queryString.ToDate))
            //        {
            //            denNgay = DateTime.Now;
            //        }
            //        else
            //        {
            //            DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //        }
            //        query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay.AddSeconds(59));
            //    }

            //}
            //if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            //{
            //    var queryStrings = JsonConvert.DeserializeObject<KhamBenhDSCKGSearchGridVoItem>(queryInfo.SearchTerms);
            //    if (queryStrings != null)
            //    {
            //        if (queryStrings.PhongBenhVienId != 0 && queryStrings.searchString == null)
            //        {
            //            query = query.Where(p => p.PhongBenhVienId == queryStrings.PhongBenhVienId);
            //        }
            //        if (queryStrings.PhongBenhVienId != 0 && queryStrings.searchString != null)
            //        {
            //            var diachi = queryStrings.searchString.RemoveVietnameseDiacritics().ToLower().TrimStart().TrimEnd();
            //            query = query.Where(p => p.PhongBenhVienId == queryStrings.PhongBenhVienId
            //                                  && (p.HoTen.RemoveUniKeyAndToLower().Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                       || p.NamSinh.ToString().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                       || p.DiaChi.RemoveUniKeyAndToLower()
            //                                            .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                       || p.DiaChiSearch.RemoveUniKeyAndToLower()
            //                                            .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                       || p.DiaChiSearch2.RemoveUniKeyAndToLower()
            //                                            .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                       //|| p.TenDiaChi.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                       //|| p.TenPhuongXa.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                       //|| p.TenQuanHuyen.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                       //|| p.TenTinhThanh.ToLower().Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                       || p.DoiTuong.RemoveUniKeyAndToLower()
            //                                            .Contains(queryStrings.searchString.RemoveUniKeyAndToLower().TrimStart().TrimEnd())
            //                                       //|| p.TenBenh.Contains(queryStrings.searchString.TrimStart().TrimEnd())
            //                                       //|| p.TenBenh.ToLower().Contains(queryStrings.searchString.ToLower().TrimStart().TrimEnd())
            //                                       //|| p.TenBenh.RemoveVietnameseDiacritics().ToLower().Contains(queryStrings.searchString.ToLower().TrimStart().TrimEnd())
            //                                       ));
            //        }
            //    }
            //}
            ////query = query.Where(o => Convert.ToDateTime(o.ThoiDiemTiepNhan).Date == DateTime.Now.Date);
            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);

            //return new GridDataSource { TotalRowCount = countTask.Result };
            return null;
        }

        public async Task<List<DoiTuongPhongChoTemplateVo>> GetDoiTuongPhongCho(DropDownListRequestModel queryInfo)
        {
            var result = _phongBenhVienHangDoiRepository.TableNoTracking
                .Include(o => o.YeuCauTiepNhan)
                .OrderByDescending(o => o.YeuCauTiepNhan.ThoiDiemTiepNhan)
                .Where(o => o.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)
                .Select(item => new DoiTuongPhongChoTemplateVo
                {
                    ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    DisplayName = SetSoPhanTramHuongBHYT(item.YeuCauTiepNhan.BHYTMaSoThe),
                    KeyId = item.YeuCauTiepNhanId,
                });
            result = result.GroupBy(p => new
            {
                p.DisplayName
            }).Select(g => g.First());
            result = result.Where(o => Convert.ToDateTime(o.ThoiDiemTiepNhan).Date == DateTime.Now.Date);
            await Task.WhenAll(result.ToListAsync());
            return result.ToList();
        }

        public async Task<Core.Domain.Entities.BenhVien.BenhVien> BenhVienHienTai()
        {
            //BvBacHa
            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var result = _benhVienRepository.TableNoTracking
                          .Where(p => p.Ma == settings.BenhVienTiepNhan).FirstOrDefault();
            //result.HangBenhVien = (Enums.HangBenhVien)settings.HangBenhVien;
            return result;
        }

        public Core.Domain.Entities.BenhVien.BenhVien GetBenhViens(long? benhVienId)
        {
            var result = _benhVienRepository.TableNoTracking
                          .Where(p => p.Id == benhVienId).FirstOrDefault();
            return result;
        }
    }
}
