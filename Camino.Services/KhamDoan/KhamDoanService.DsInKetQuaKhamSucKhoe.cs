using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachInKetQuaKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<InKetQuaKhamSucKhoeVo>().ToArray(), TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var queryString = JsonConvert.DeserializeObject<InKetQuaKhamSucKhoeVo>(queryInfo.AdditionalSearchString);

            // to do  tu ngay toi ngay

            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(p => p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId
                         //&& p.YeuCauTiepNhans.Any()
                         && p.BenhNhanId != null
                         && (queryString.TuNgay == null || queryString.TuNgay >= p.HopDongKhamSucKhoe.NgayHopDong)
                         && (queryString.DenNgay == null || queryString.DenNgay <= p.HopDongKhamSucKhoe.NgayHopDong))
                .Select(s => new InKetQuaKhamSucKhoeVo
                {
                    Id = s.Id,
                    //YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
                    CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
                    //MaTN = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.MaYeuCauTiepNhan).FirstOrDefault(),
                    MaBN = s.BenhNhan.MaBN,
                    MaNhanVien = s.MaNhanVien,
                    HoTen = s.HoTen,
                    TenNgheNghiep = s.NgheNghiep.Ten,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    SoDienThoai = s.SoDienThoai,
                    Email = s.Email,
                    SoChungMinhThu = s.SoChungMinhThu,
                    TenDanToc = s.DanToc.Ten,
                    TenTinhThanh = s.TinhThanh.Ten,
                    NhomDoiTuongKhamSucKhoe = s.NhomDoiTuongKhamSucKhoe,
                    //KSKKetLuanPhanLoaiSucKhoe = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KSKKetLuanPhanLoaiSucKhoe).FirstOrDefault(),
                    GoiDichVuId = s.GoiKhamSucKhoeId,
                    NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
                    LaHopDongDaKetLuan = s.HopDongKhamSucKhoe.DaKetThuc,
                    //TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.TrangThaiYeuCauTiepNhan).FirstOrDefault(),
                    // BVHD-3722
                    //KetQuaKhamSucKhoeData = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KetQuaKhamSucKhoeData).FirstOrDefault()
                    InKetQuaKhamSucKhoeTiepNhanVos = s.YeuCauTiepNhans.Select(tn =>new InKetQuaKhamSucKhoeTiepNhanVo
                    {
                        Id = tn.Id,
                        MaYeuCauTiepNhan = tn.MaYeuCauTiepNhan,
                        KSKKetLuanPhanLoaiSucKhoe = tn.KSKKetLuanPhanLoaiSucKhoe,
                        TrangThaiYeuCauTiepNhan = tn.TrangThaiYeuCauTiepNhan,
                        KetQuaKhamSucKhoeData = tn.KetQuaKhamSucKhoeData
                    }).ToList()
                });

            if (queryString.LaHopDongDaKetLuan == true)
            {
                query = query.Where(p => p.LaHopDongDaKetLuan == true);
            }
            else
            {
                query = query.Where(p => p.LaHopDongDaKetLuan != true);
            }
            var allResult = query.ToList();
            var validResult = allResult.Where(o => o.InKetQuaKhamSucKhoeTiepNhanVos.Any(tn => tn.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)).ToList();
            foreach(var item in validResult)
            {
                var yctn = item.InKetQuaKhamSucKhoeTiepNhanVos.Where(tn => tn.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy).OrderByDescending(p => p.Id).First();
                item.YeuCauTiepNhanId = yctn.Id;
                item.MaTN = yctn.MaYeuCauTiepNhan;
                item.KSKKetLuanPhanLoaiSucKhoe = yctn.KSKKetLuanPhanLoaiSucKhoe;
                item.TrangThaiYeuCauTiepNhan = yctn.TrangThaiYeuCauTiepNhan;
                item.KetQuaKhamSucKhoeData = yctn.KetQuaKhamSucKhoeData;
            }

            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                validResult = validResult.Where(p => p.TinhTrang == 1).ToList();
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                validResult = validResult.Where(p => p.TinhTrang == 0).ToList();
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim().RemoveVietnameseDiacritics().ToLower();
                validResult = validResult.Where(p => (p.MaTN != null && p.MaTN.RemoveVietnameseDiacritics().ToLower().Contains(searchTerms))
                                                    || (p.MaBN != null && p.MaBN.RemoveVietnameseDiacritics().ToLower().Contains(searchTerms))
                                                    || (p.HoTen != null && p.HoTen.RemoveVietnameseDiacritics().ToLower().Contains(searchTerms))
                                                    || (p.SoDienThoai != null && p.SoDienThoai.RemoveVietnameseDiacritics().ToLower().Contains(searchTerms))).ToList();
                
            }
            if (queryString.Take != null)
            {
                queryInfo.Take = queryString.Take;
            }
            var queryResult = validResult.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            if (queryResult.Any())
            {
                var yctnIds = queryResult.Select(o => o.YeuCauTiepNhanId).ToList();
                //var yeuCauDichVus = BaseRepository.TableNoTracking.Where(tn => yctnIds.Contains(tn.Id))
                //    .Select(s => new
                //    {
                //        s.Id,
                //        DichVuDaThucHien = s.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                //                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham
                //                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(c => c.Id).Count()
                //                           + s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                //                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                //                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(c => c.Id).Count(),
                //        TongDichVu = s.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(c => c.Id).Count()
                //               + s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(c => c.Id).Count()

                //    }).ToList();

                var yeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(o => o.GoiKhamSucKhoeId != null && yctnIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    .Select(o => new
                    {
                        o.Id,
                        o.YeuCauTiepNhanId,
                        o.TrangThai,
                        GoiKhamSucKhoeId = o.GoiKhamSucKhoeId,
                        GoiKhamSucKhoeTiepNhanId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId,
                    }).ToList();
                var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(o => o.GoiKhamSucKhoeId != null && yctnIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(o => new
                    {
                        o.Id,
                        o.YeuCauTiepNhanId,
                        o.TrangThai,
                        GoiKhamSucKhoeId = o.GoiKhamSucKhoeId,
                        GoiKhamSucKhoeTiepNhanId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId,
                    }).ToList();

                foreach (var item in queryResult)
                {
                    //item.DichVuDaThucHien = yeuCauDichVus.Where(yc => yc.Id == item.YeuCauTiepNhanId).Select(yc => yc.DichVuDaThucHien).FirstOrDefault();
                    //item.TongDichVu = yeuCauDichVus.Where(yc => yc.Id == item.YeuCauTiepNhanId).Select(yc => yc.TongDichVu).FirstOrDefault();

                    item.DichVuDaThucHien = yeuCauKhamBenhs
                        .Where(o => o.YeuCauTiepNhanId == item.YeuCauTiepNhanId && o.GoiKhamSucKhoeId == o.GoiKhamSucKhoeTiepNhanId && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                        .Count() 
                        + yeuCauDichVuKyThuats
                        .Where(o => o.YeuCauTiepNhanId == item.YeuCauTiepNhanId && o.GoiKhamSucKhoeId == o.GoiKhamSucKhoeTiepNhanId && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                        .Count();
                    item.TongDichVu = yeuCauKhamBenhs
                        .Where(o => o.YeuCauTiepNhanId == item.YeuCauTiepNhanId && o.GoiKhamSucKhoeId == o.GoiKhamSucKhoeTiepNhanId)
                        .Count()
                        + yeuCauDichVuKyThuats
                        .Where(o => o.YeuCauTiepNhanId == item.YeuCauTiepNhanId && o.GoiKhamSucKhoeId == o.GoiKhamSucKhoeTiepNhanId)
                        .Count();
                }
            }

            return new GridDataSource { Data = queryResult, TotalRowCount = validResult.Count() };
        }
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachInKetQuaKhamSucKhoeOld(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<InKetQuaKhamSucKhoeVo>().ToArray(), TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var queryString = JsonConvert.DeserializeObject<InKetQuaKhamSucKhoeVo>(queryInfo.AdditionalSearchString);
            
            // to do  tu ngay toi ngay



            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(p => p.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                         && p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId
                         && p.YeuCauTiepNhans.Any()
                         && p.BenhNhanId != null 
                         && (queryString.TuNgay == null || queryString.TuNgay >= p.HopDongKhamSucKhoe.NgayHopDong)
                         && (queryString.DenNgay == null || queryString.DenNgay <= p.HopDongKhamSucKhoe.NgayHopDong))
                .Select(s => new InKetQuaKhamSucKhoeVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
                    CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
                    MaTN = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.MaYeuCauTiepNhan).FirstOrDefault(),
                    MaBN = s.BenhNhan.MaBN,
                    MaNhanVien = s.MaNhanVien,
                    HoTen = s.HoTen,
                    TenNgheNghiep = s.NgheNghiep.Ten,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    SoDienThoai = s.SoDienThoai,
                    Email = s.Email,
                    SoChungMinhThu = s.SoChungMinhThu,
                    TenDanToc = s.DanToc.Ten,
                    TenTinhThanh = s.TinhThanh.Ten,
                    NhomDoiTuongKhamSucKhoe = s.NhomDoiTuongKhamSucKhoe,
                    KSKKetLuanPhanLoaiSucKhoe = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KSKKetLuanPhanLoaiSucKhoe).FirstOrDefault(),
                    GoiDichVuId = s.GoiKhamSucKhoeId,
                    NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
                    LaHopDongDaKetLuan = s.HopDongKhamSucKhoe.DaKetThuc,
                    TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.TrangThaiYeuCauTiepNhan).FirstOrDefault(),
                    // BVHD-3722
                    KetQuaKhamSucKhoeData = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KetQuaKhamSucKhoeData).FirstOrDefault()
                });

            if (queryString.LaHopDongDaKetLuan == true)
            {
                query = query.Where(p => p.LaHopDongDaKetLuan == true);
            }
            else
            {
                query = query.Where(p => p.LaHopDongDaKetLuan != true);
            }
            // 0: ChuaKetLuan, 1 : DaKetLuan
            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.HoTen,
                   g => g.SoDienThoai
               );
            }
            if (queryString.Take != null)
            {
                queryInfo.Take = queryString.Take;
            }
            var queryResult = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            var result = new List<InKetQuaKhamSucKhoeVo>();
            if (queryResult.Any())
            {
                var yctnIds = queryResult.Select(o => o.YeuCauTiepNhanId).ToList();
                var yeuCauDichVus = BaseRepository.TableNoTracking.Where(tn => yctnIds.Contains(tn.Id))
                    .Select(s => new
                    {
                        s.Id,
                        DichVuDaThucHien = s.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(c => c.Id).Count()
                                           + s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(c => c.Id).Count(),
                        TongDichVu = s.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(c => c.Id).Count()
                               + s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(c => c.Id).Count()

                    }).ToList();
                foreach (var item in queryResult)
                {
                    item.DichVuDaThucHien = yeuCauDichVus.Where(yc => yc.Id == item.YeuCauTiepNhanId).Select(yc => yc.DichVuDaThucHien).FirstOrDefault();
                    item.TongDichVu = yeuCauDichVus.Where(yc => yc.Id == item.YeuCauTiepNhanId).Select(yc => yc.TongDichVu).FirstOrDefault();
                    result.Add(item);
                }
            }
            
            //var queryTask = queryResult.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();
            return new GridDataSource { Data = queryResult, TotalRowCount = queryResult.Count() };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachInKetQuaKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;

            }
            var queryString = JsonConvert.DeserializeObject<InKetQuaKhamSucKhoeVo>(queryInfo.AdditionalSearchString);

            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                   .Where(p => p.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                   && p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId
                   && p.YeuCauTiepNhans.Any()
                   && p.BenhNhanId != null
                   && (queryString.TuNgay == null || queryString.TuNgay >= p.HopDongKhamSucKhoe.NgayHopDong)
                   && (queryString.DenNgay == null || queryString.DenNgay <= p.HopDongKhamSucKhoe.NgayHopDong))
                .Select(s => new InKetQuaKhamSucKhoeVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
                    CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
                    MaTN = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.MaYeuCauTiepNhan).FirstOrDefault(),
                    MaBN = s.BenhNhan.MaBN,
                    MaNhanVien = s.MaNhanVien,
                    HoTen = s.HoTen,
                    TenNgheNghiep = s.NgheNghiep.Ten,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    SoDienThoai = s.SoDienThoai,
                    Email = s.Email,
                    SoChungMinhThu = s.SoChungMinhThu,
                    TenDanToc = s.DanToc.Ten,
                    TenTinhThanh = s.TinhThanh.Ten,
                    NhomDoiTuongKhamSucKhoe = s.NhomDoiTuongKhamSucKhoe,
                    KSKKetLuanPhanLoaiSucKhoe = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KSKKetLuanPhanLoaiSucKhoe).FirstOrDefault(),
                    NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
                    LaHopDongDaKetLuan = s.HopDongKhamSucKhoe.DaKetThuc,
                    TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.TrangThaiYeuCauTiepNhan).FirstOrDefault()
                });
            if (queryString.LaHopDongDaKetLuan == true)
            {
                query = query.Where(p => p.LaHopDongDaKetLuan == true);
            }
            else
            {
                query = query.Where(p => p.LaHopDongDaKetLuan != true);
            }
            // 0: ChuaKetLuan, 1 : DaKetLuan
            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.HoTen,
                   g => g.SoDienThoai
               );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemTemplateVo>> GetCongTyInKetQuaKSKs(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(CongTyKhamSucKhoe.Ma),
                nameof(CongTyKhamSucKhoe.Ten),
            };
            var lstCongTys = new List<LookupItemTemplateVo>();
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstCongTys = await _congTyKhamSucKhoeRepository.TableNoTracking
                    .Where(x =>  x.Id == queryInfo.Id)
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = item.Ma,
                    })
                    .Union(
                        _congTyKhamSucKhoeRepository.TableNoTracking
                        .Where(x =>  x.Id != queryInfo.Id)
                        .Select(item => new LookupItemTemplateVo
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            Ten = item.Ten,
                            Ma = item.Ma,
                        }))
                        .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                        .OrderByDescending(x => x.KeyId == queryInfo.Id).ThenBy(x => x.DisplayName)
                        .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstCongTyId = await _congTyKhamSucKhoeRepository
                    .ApplyFulltext(queryInfo.Query, nameof(CongTyKhamSucKhoe), lstColumnNameSearch)
                    .Where(x =>  queryInfo.Id == 0 || x.Id == queryInfo.Id)
                    .Select(x => x.Id)
                    .ToListAsync();
                lstCongTys = await _congTyKhamSucKhoeRepository.TableNoTracking
                    .Where(x => x.CoHoatDong == true && lstCongTyId.Contains(x.Id))
                    .OrderByDescending(x => x.Id == queryInfo.Id)
                    .ThenBy(p => lstCongTyId.IndexOf(p.Id) != -1 ? lstCongTyId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = item.Ma,
                    }).ToListAsync();
            }
            return lstCongTys;
        }
        public async Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> GetHopDongKhamSucKhoeInKetQuaKSKs(DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(HopDongKhamSucKhoe.SoHopDong),
            };
            var lstHopDongKhamSucKhoes = new List<LookupItemHopDingKhamSucKhoeTemplateVo>();
            var congTyId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => x.CongTyKhamSucKhoeId == congTyId
                               //&& (
                               //     (!LaHopDongKetThuc && x.NgayHieuLuc.Date <= DateTime.Now.Date && DateTime.Now.Date <= x.NgayKetThuc.Date && !x.DaKetThuc)
                               //     || (LaHopDongKetThuc && (x.DaKetThuc || DateTime.Now.Date > x.NgayKetThuc.Date))
                               //     )
                            )
                    .Select(item => new LookupItemHopDingKhamSucKhoeTemplateVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.SoHopDong,
                        CongTyKhamSucKhoeId = item.CongTyKhamSucKhoeId,
                        SoHopDong = item.SoHopDong,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayKetThuc = item.NgayKetThuc
                    })
                    .ApplyLike(queryInfo.Query, x => x.SoHopDong)
                    //.OrderByDescending(x => x.CongTyKhamSucKhoeId == congTyId).ThenBy(x => x.KeyId)
                    .OrderByDescending(x => x.NgayHieuLuc).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstHopDongKhamSucKhoeId = await _hopDongKhamSucKhoeRepository
                    .ApplyFulltext(queryInfo.Query, nameof(HopDongKhamSucKhoe), lstColumnNameSearch)
                    .Where(x => x.CongTyKhamSucKhoeId == congTyId
                               //&& (
                                //    (!LaHopDongKetThuc && x.NgayHieuLuc.Date <= DateTime.Now.Date && DateTime.Now.Date <= x.NgayKetThuc.Date && !x.DaKetThuc)
                                //    || (LaHopDongKetThuc && (x.DaKetThuc || DateTime.Now.Date > x.NgayKetThuc.Date))
                                //)
                          )
                    .Select(x => x.Id)
                    .ToListAsync();
                lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => lstHopDongKhamSucKhoeId.Contains(x.Id))
                    //.OrderByDescending(x => x.CongTyKhamSucKhoeId == congTyId)
                    .OrderByDescending(x => x.NgayHieuLuc)
                    .ThenBy(p => lstHopDongKhamSucKhoeId.IndexOf(p.Id) != -1 ? lstHopDongKhamSucKhoeId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new LookupItemHopDingKhamSucKhoeTemplateVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.SoHopDong,
                        SoHopDong = item.SoHopDong,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayKetThuc = item.NgayKetThuc
                    }).ToListAsync();
            }
            return lstHopDongKhamSucKhoes;
        }
        public async Task<KhamDoanThongTinHanhChinhInKetQuaKhamSucKhoeVo> GetThongTinHanhChinhInKetQuaKSKAsync(long yeuCauTiepNhanId)
        {
            var thongTinHanhChinh = BaseRepository.TableNoTracking
                .Where(x => x.Id == yeuCauTiepNhanId)
                .Select(item => new KhamDoanThongTinHanhChinhInKetQuaKhamSucKhoeVo()
                {
                    MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    TenGioiTinh = item.GioiTinh.GetDescription(),
                    TenCongTy = item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    NgayThangNamSinh = DateHelper.DOBFormat(item.NgaySinh, item.ThangSinh, item.NamSinh),
                    Disabled = (item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc == null || (item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc != null && (item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc > DateTime.Now  || 
                                                                                                          (item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc > DateTime.Now&& 
                                                                                                                            item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.DaKetThuc == false)))) ? true : false
           }).FirstOrDefault();

            return thongTinHanhChinh;
        }
        public List<InFoBarCodeKSK> ComPareBarCode(InFoBarCodeKSKVo searching)
        {
            var valueSearch = true;
            var listBNs = new List<InFoBarCodeKSK>();

            if (searching.Searching.Length == 4)
            {
                var start = (searching.Searching.Length - 4);
                var end = 4;

                searching.Searching = searching.Searching.Trim().ToString().Substring(start, 4);
                valueSearch = false;
            }

            if(valueSearch == true) // search chính xác 
            {
                var query = _yeuCauTiepNhanRepository.TableNoTracking
               .Where(d => (d.MaYeuCauTiepNhan == searching.Searching.Trim() ||
                            d.BenhNhan.MaBN == searching.Searching.Trim()) &&
                            d.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == searching.HopDongId);
                if (query.Any())
                {
                    listBNs = query.Select(d => new InFoBarCodeKSK()
                    {
                        YeuCauTiepNhanId = (long)d.Id,
                        HopDongKhamSucKhoeNhanVienId = (long)d.HopDongKhamSucKhoeNhanVienId,
                        MaYeuCauTiepNhan = d.MaYeuCauTiepNhan,
                        MaBN = d.BenhNhan.MaBN,
                        TenBN = d.HoTen
                    }).ToList() ; 
                }
            }
            else // search 4 số
            {
                var query = _yeuCauTiepNhanRepository.TableNoTracking
              .Where(d => (d.MaYeuCauTiepNhan.Substring(d.MaYeuCauTiepNhan.Length - 4, 4).Trim() == searching.Searching.Trim() ||
                           d.BenhNhan.MaBN.Substring(d.BenhNhan.MaBN.Length - 4, 4).Trim() == searching.Searching.Trim()) &&
                           d.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == searching.HopDongId);
                if (query.Any())
                {
                    listBNs = query.Select(d => new InFoBarCodeKSK()
                    {
                        YeuCauTiepNhanId = (long)d.Id,
                        HopDongKhamSucKhoeNhanVienId = (long)d.HopDongKhamSucKhoeNhanVienId,
                        MaYeuCauTiepNhan = d.MaYeuCauTiepNhan,
                        MaBN = d.BenhNhan.MaBN,
                        TenBN = d.HoTen
                    }).ToList();
                }
            }
           
            return listBNs;
        }

        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanDoanCantAsync(long yctnId)
        {

            var query =
                 _yeuCauTiepNhanRepository.Table
                     .Include(y => y.KetQuaSinhHieus)
                     .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauKhamBenhLichSuTrangThais)
                     .Include(y => y.HopDongKhamSucKhoeNhanVien).ThenInclude(t => t.HopDongKhamSucKhoe).ThenInclude(u => u.CongTyKhamSucKhoe)
                     .Include(y => y.YeuCauKhamBenhs).ThenInclude(z => z.PhongBenhVienHangDois)

                     .Where(x => x.Id == yctnId
                                 && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                 && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).FirstOrDefault();


            return query;
        }
        public async Task<long> GetHopDongId(long hpNhanVienId)
        {
            var id = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(d => d.Id == hpNhanVienId).Select(d => d.HopDongKhamSucKhoeId).FirstOrDefault();
            return id;
        }
    }
}
