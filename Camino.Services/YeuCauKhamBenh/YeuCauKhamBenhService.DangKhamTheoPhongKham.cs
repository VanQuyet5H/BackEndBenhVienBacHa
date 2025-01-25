using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {
        #region Grid

        public async Task<GridDataSource> GetDataForGridKhamBenhDangKhamAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            //TODO: đợi update lại logic phần login
            // hoạt động nhân viên là bác sĩ
            var hoatDongNhanVienBacSi = await _hoatDongNhanVienRepository.TableNoTracking
                .Include(x => x.NhanVien).ThenInclude(y => y.User)
                .Where(x => x.NhanVien.User.IsActive
                            && x.NhanVien.ChucDanh.NhomChucDanh.Ten.Contains("bác sĩ"))
                //&& x.ThoiDiemBatDau.Date == DateTime.Now.Date)
                .Select(item => new HoatDongBacSiVo()
                {
                    PhongBenhVienId = item.PhongBenhVienId,
                    NhanVienId = item.NhanVienId,
                    HoTen = item.NhanVien.User.HoTen,
                    ThoiDiemBatDau = item.ThoiDiemBatDau
                })
                .Distinct()
                //.GroupBy(x => new { x.NhanVienId })
                //.Select(item => new HoatDongBacSiVo()
                //{
                //    PhongBenhVienId = item.OrderByDescending(x => x.ThoiDiemBatDau).First().PhongBenhVienId,
                //    HoTen = item.First().HoTen,
                //})
                .ToListAsync();

            var query = _phongBenhVienRepository.TableNoTracking
                .Where(x => x.PhongBenhVienHangDois.Any(y => y.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe 
                                                             && y.YeuCauKhamBenhId != null))
                .Select(item => new KhamBenhDangKhamGridVo()
                {
                    PhongBenhVienId = item.Id,
                    TenPhongBenhVien = item.Ten,
                    Phong = item.Ten,
                    KhoaId = item.KhoaPhongId,
                    Khoa = item.KhoaPhong.Ten,
                    BacSiDangKham = string.Join(", ", hoatDongNhanVienBacSi.Where(x => x.PhongBenhVienId == item.Id).Select(a => a.HoTen)),
                    //BacSiDangKham = string.Join(", ", item.HoatDongNhanViens.Select(a => a.NhanVien.User.HoTen).Distinct()),
                    BenhNhanDangKham = item.PhongBenhVienHangDois.Any(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe 
                                                                           && x.YeuCauKhamBenhId != null 
                                                                           && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham) ? 
                                                item.PhongBenhVienHangDois.FirstOrDefault(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe 
                                                                                               && x.YeuCauKhamBenhId != null 
                                                                                               && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham).YeuCauTiepNhan.HoTen : "",
                    SoLuongBenhNhan = item.PhongBenhVienHangDois.Where(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Count(x => x.YeuCauKhamBenhId != null)
                });
            //.Where(x => x.BenhNhanDangKham.Trim().ToLower().Contains(queryInfo.SearchTerms.Trim().ToLower()) && x.BacSiDangKham.Trim().ToLower().Contains(queryInfo.SearchTerms.Trim().ToLower()));
            //.ApplyLike(queryInfo.SearchTerms, x => x.BacSiDangKham, x => x.BenhNhanDangKham);

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var searchString = queryInfo.SearchTerms.Trim().RemoveVietnameseDiacritics().ToLower();
                query = query.Where(x =>
                    x.BenhNhanDangKham.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString) ||
                    x.BacSiDangKham.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var timKiemNangCaoObj = JsonConvert.DeserializeObject<KhamBenhDangKhamTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
                if (timKiemNangCaoObj.PhongBenhVienId != null && timKiemNangCaoObj.PhongBenhVienId != 0)
                {
                    query = query.Where(x => x.PhongBenhVienId == timKiemNangCaoObj.PhongBenhVienId);
                }
                if (timKiemNangCaoObj.KhoaPhongId != null && timKiemNangCaoObj.KhoaPhongId != 0)
                {
                    query = query.Where(x => x.KhoaId == timKiemNangCaoObj.KhoaPhongId);
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridKhamBenhDangKhamAsync(QueryInfo queryInfo)
        {
            var hoatDongNhanVienBacSi = await _hoatDongNhanVienRepository.TableNoTracking
                .Include(x => x.NhanVien).ThenInclude(y => y.User)
                .Where(x => x.NhanVien.User.IsActive
                            && x.NhanVien.ChucDanh.NhomChucDanh.Ten.Contains("bác sĩ"))
                .Select(item => new HoatDongBacSiVo()
                {
                    PhongBenhVienId = item.PhongBenhVienId,
                    NhanVienId = item.NhanVienId,
                    HoTen = item.NhanVien.User.HoTen,
                    ThoiDiemBatDau = item.ThoiDiemBatDau
                })
                .Distinct()
                .ToListAsync();

            var query = _phongBenhVienRepository.TableNoTracking
                .Where(x => x.PhongBenhVienHangDois.Any(y => y.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe 
                                                             && y.YeuCauKhamBenhId != null))
                .Select(item => new KhamBenhDangKhamGridVo()
                {
                    PhongBenhVienId = item.Id,
                    TenPhongBenhVien = item.Ten,
                    Phong = item.Ten,
                    KhoaId = item.KhoaPhongId,
                    Khoa = item.KhoaPhong.Ten,
                    BacSiDangKham = string.Join(", ", hoatDongNhanVienBacSi.Where(x => x.PhongBenhVienId == item.Id).Select(a => a.HoTen)),
                    //BenhNhanDangKham = item.PhongBenhVienHangDois.Any(x => x.YeuCauKhamBenhId != null && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham) ?
                    //    item.PhongBenhVienHangDois.FirstOrDefault(x => x.YeuCauKhamBenhId != null && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham).YeuCauTiepNhan.HoTen : "",
                    //SoLuongBenhNhan = item.PhongBenhVienHangDois.Count(x => x.YeuCauKhamBenhId != null)
                    BenhNhanDangKham = item.PhongBenhVienHangDois.Any(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                                                           && x.YeuCauKhamBenhId != null
                                                                           && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham) ?
                        item.PhongBenhVienHangDois.FirstOrDefault(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                                                       && x.YeuCauKhamBenhId != null
                                                                       && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham).YeuCauTiepNhan.HoTen : "",
                    SoLuongBenhNhan = item.PhongBenhVienHangDois.Where(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Count(x => x.YeuCauKhamBenhId != null)
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var searchString = queryInfo.SearchTerms.Trim().RemoveVietnameseDiacritics().ToLower();
                query = query.Where(x =>
                    x.BenhNhanDangKham.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString) ||
                    x.BacSiDangKham.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var timKiemNangCaoObj = JsonConvert.DeserializeObject<KhamBenhDangKhamTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
                if (timKiemNangCaoObj.PhongBenhVienId != null && timKiemNangCaoObj.PhongBenhVienId != 0)
                {
                    query = query.Where(x => x.PhongBenhVienId == timKiemNangCaoObj.PhongBenhVienId);
                }
                if (timKiemNangCaoObj.KhoaPhongId != null && timKiemNangCaoObj.KhoaPhongId != 0)
                {
                    query = query.Where(x => x.KhoaId == timKiemNangCaoObj.KhoaPhongId);
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridKhamBenhDangKhamTheoPhongKhamAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long phongBenhVienId = 0;
            var timKiemNangCaoObj = new KhamBenhDangKhamTimKiemNangCapVo();
            if (queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<KhamBenhDangKhamTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
                phongBenhVienId = timKiemNangCaoObj.PhongBenhVienId ?? 0;
            }
            else if(!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && !queryInfo.AdditionalSearchString.Contains("undefined") && !queryInfo.AdditionalSearchString.Contains("null"))
            {
                phongBenhVienId = long.Parse(queryInfo.AdditionalSearchString);
            }
            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId == phongBenhVienId
                            && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                            && x.YeuCauKhamBenhId != null
                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                .Select(item => new KhamBenhDangKhamTheoPhongKhamGridVo()
                {
                    PhongBenhVienHangDoiId = item.Id,
                    YeuCauKhamBenhId = item.YeuCauKhamBenhId.Value,
                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                    PhongBenhVienId = item.PhongBenhVienId,
                    MaTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = item.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                    SoDienThoaiFilter = item.YeuCauTiepNhan.SoDienThoai,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    TrangThai = item.YeuCauKhamBenh.TrangThai,
                    CoBaoHiem = item.YeuCauTiepNhan.CoBHYT ?? false,
                    SoThuTu = item.SoThuTu
                })
                .ApplyLike(queryInfo.SearchTerms, x => x.MaTiepNhan, x => x.MaBenhNhan, x => x.HoTen,
                                                                    x => (x.NamSinh != null ? x.NamSinh.Value.ToString() : ""),
                                                                    x => x.SoDienThoaiFilter);
            //.OrderBy(x => x.TrangThai).ThenBy(x => x.ThoiDiemTiepNhan);

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || p.ThoiDiemTiepNhan.Date >= tuNgay.Date) 
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || p.ThoiDiemTiepNhan.Date <= denNgay.Date));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaKham || timKiemNangCaoObj.TrangThai.DangKham ||
                timKiemNangCaoObj.TrangThai.DangDoiKetLuan || timKiemNangCaoObj.TrangThai.DangLamChiDinh))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaKham && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                    || (timKiemNangCaoObj.TrangThai.DangKham && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                    || (timKiemNangCaoObj.TrangThai.DangLamChiDinh &&  x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
                    || (timKiemNangCaoObj.TrangThai.DangDoiKetLuan &&  x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan));
            }

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ThenBy(x => x.SoThuTu).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridKhamBenhDangKhamTheoPhongKhamAsync(QueryInfo queryInfo)
        {
            long phongBenhVienId = 0;
            var timKiemNangCaoObj = new KhamBenhDangKhamTimKiemNangCapVo();
            if (queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<KhamBenhDangKhamTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
                phongBenhVienId = timKiemNangCaoObj.PhongBenhVienId ?? 0;
            }
            else if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && !queryInfo.AdditionalSearchString.Contains("undefined") && !queryInfo.AdditionalSearchString.Contains("null"))
            {
                phongBenhVienId = long.Parse(queryInfo.AdditionalSearchString);
            }
            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId == phongBenhVienId
                            && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                            && x.YeuCauKhamBenhId != null
                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                .Select(item => new KhamBenhDangKhamTheoPhongKhamGridVo()
                {
                    PhongBenhVienHangDoiId = item.Id,
                    YeuCauKhamBenhId = item.YeuCauKhamBenhId.Value,
                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                    PhongBenhVienId = item.PhongBenhVienId,
                    MaTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = item.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                    SoDienThoaiFilter = item.YeuCauTiepNhan.SoDienThoai,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    TrangThai = item.YeuCauKhamBenh.TrangThai,
                    CoBaoHiem = item.YeuCauTiepNhan.CoBHYT ?? false,
                    SoThuTu = item.SoThuTu
                })
                .ApplyLike(queryInfo.SearchTerms, x => x.MaTiepNhan, x => x.MaBenhNhan, x => x.HoTen,
                                                                    x => (x.NamSinh != null ? x.NamSinh.Value.ToString() : ""),
                                                                    x => x.SoDienThoaiFilter);
            //.OrderBy(x => x.TrangThai).ThenBy(x => x.ThoiDiemTiepNhan);

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || p.ThoiDiemTiepNhan.Date >= tuNgay.Date)
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || p.ThoiDiemTiepNhan.Date <= denNgay.Date));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaKham || timKiemNangCaoObj.TrangThai.DangKham ||
                timKiemNangCaoObj.TrangThai.DangDoiKetLuan || timKiemNangCaoObj.TrangThai.DangLamChiDinh))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaKham && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                    || (timKiemNangCaoObj.TrangThai.DangKham && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                    || (timKiemNangCaoObj.TrangThai.DangLamChiDinh && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
                    || (timKiemNangCaoObj.TrangThai.DangDoiKetLuan && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan));
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region get list

        public async Task<List<LookupItemTemplateVo>> GetListPhongBenhVienAsync(DropDownListRequestModel model)
        {
            var khoaPhongId = CommonHelper.GetIdFromRequestDropDownList(model);
            var lstPhong = await _phongBenhVienRepository.TableNoTracking
                .Where(x => x.IsDisabled != true && (khoaPhongId == 0 || x.KhoaPhongId == khoaPhongId))
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    Ma = item.Ma,
                    Ten = item.Ten
                })
                .ApplyLike(model.Query, x => x.Ten, x => x.Ma)
                .Take(model.Take).ToListAsync();
            return lstPhong;
        }

        public async Task<List<LookupItemTemplateVo>> GetListKhoaBenhVienAsync(DropDownListRequestModel model)
        {
            var lstKhoa = await _khoaPhongRepository.TableNoTracking
                .Where(x => x.IsDisabled != true)
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    Ma = item.Ma,
                    Ten = item.Ten
                })
                .ApplyLike(model.Query, x => x.Ten, x => x.Ma)
                .Take(model.Take).ToListAsync();
            return lstKhoa;
        }

        #endregion

        #region get data
        public async Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamAsync(long phongBenhVienHangDoiId)
        {
            #region Code cũ
            //var query =
            //    await _phongBenhVienHangDoiRepository.Table
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.PhuongXa)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.QuanHuyen)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanDiUngThuocs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanTienSuBenhs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NhanVienTiepNhan).ThenInclude(u => u.User)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.LyDoTiepNhan)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauDichVuKyThuats)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.Icdchinh)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhICDKhacs).ThenInclude(y => y.ICD)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DuongDung)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DonViTinh)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonVTYTs).ThenInclude(z => z.YeuCauKhamBenhDonVTYTChiTiets)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTrieuChungs).ThenInclude(z => z.TrieuChung)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChuanDoans).ThenInclude(z => z.ChuanDoan)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTruoc)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.ChanDoanSoBoICD)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.KhoaPhongNhapVien)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.BenhVienChuyenVien)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhKhamBoPhanKhacs)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(z => z.ICD)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhBoPhanTonThuongs)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.User)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.ChucDanh).ThenInclude(u => u.NhomChucDanh)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.VanBangChuyenMon)

            //        //BVHD-3825
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)

            //        .Where(x => x.Id == phongBenhVienHangDoiId
            //                    && x.YeuCauKhamBenh != null
            //                    //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
            //                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
            //                    ).FirstOrDefaultAsync();
            #endregion

            #region Cập nhật 28/03/2022
            var hangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                .FirstOrDefault(x => x.Id == phongBenhVienHangDoiId
                                     && x.YeuCauKhamBenhId != null
                                     //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                                     && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
            if (hangDoi == null)
            {
                return null;
            }

            var query =
                 _phongBenhVienHangDoiRepository.Table.Where(x => x.Id == hangDoi.Id)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanTienSuBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NhanVienTiepNhan).ThenInclude(u => u.User)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.LyDoTiepNhan)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.Icdchinh)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhICDKhacs).ThenInclude(y => y.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DuongDung)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DonViTinh)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonVTYTs).ThenInclude(z => z.YeuCauKhamBenhDonVTYTChiTiets)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTrieuChungs).ThenInclude(z => z.TrieuChung)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChuanDoans).ThenInclude(z => z.ChuanDoan)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTruoc)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.ChanDoanSoBoICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.KhoaPhongNhapVien)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.BenhVienChuyenVien)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhKhamBoPhanKhacs)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(z => z.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhBoPhanTonThuongs)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.User)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.ChucDanh).ThenInclude(u => u.NhomChucDanh)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.VanBangChuyenMon)

                    //BVHD-3825
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .FirstOrDefault();

            #endregion
            return query;
        }

        public async Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamLuuTabKhamBenhAsync(long phongBenhVienHangDoiId)
        {
            var hangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                .FirstOrDefault(x => x.Id == phongBenhVienHangDoiId
                                     && x.YeuCauKhamBenhId != null
                                     //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                                     && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
            if (hangDoi == null)
            {
                return null;
            }

            var query =
                 _phongBenhVienHangDoiRepository.Table.Where(x => x.Id == hangDoi.Id)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanTienSuBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhICDKhacs)//.ThenInclude(y => y.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhKhamBoPhanKhacs)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChanDoanPhanBiets)//.ThenInclude(z => z.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhBoPhanTonThuongs)
                    //.Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhLichSuTrangThais)

                    .FirstOrDefault();
            return query;
        }

        public async Task KiemTraDatayeuCauKhamBenhDangKhamAsync(long yeuCauKhamBenhId)
        {
            //todo: có cập nhật bỏ await
            var resourceName = "";
            var yeuCauKhamBenh = BaseRepository.TableNoTracking
                .Include(x => x.PhongBenhVienHangDois)
                .FirstOrDefault(x => x.Id == yeuCauKhamBenhId);

            if (yeuCauKhamBenh == null)
            {
                resourceName = "KhamBenh.YeuCauKhamBenh.NotExists";
            }
            else
            {
                switch (yeuCauKhamBenh.TrangThai)
                {
                    case Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham:
                        resourceName = "KhamBenh.YeuCauKhamBenh.DaHuy"; break;
                    case Enums.EnumTrangThaiYeuCauKhamBenh.DaKham:
                        resourceName = "KhamBenh.YeuCauKhamBenh.DaHoanThanhKham"; break;

                    default:
                        resourceName = ""; break;
                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                var currentUserLanguge = _userAgentHelper.GetUserLanguage();
                var mess = _localeStringResourceRepository.TableNoTracking
                    .Where(x => x.ResourceName == resourceName && x.Language == (int)currentUserLanguge)
                    .Select(x => x.ResourceValue).FirstOrDefault();
                throw new Exception(mess ?? resourceName);
            }
        }
        #endregion
    }
}
