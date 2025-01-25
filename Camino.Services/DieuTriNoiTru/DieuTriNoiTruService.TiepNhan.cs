using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        #region Grid
        public async Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNoiTruTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString.Trim();
                }
            }

            // sẽ load theo khoa phòng nhân viên đang login
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            var yeuCauNhapVienIdDaTiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.YeuCauNhapVienId != null 
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                .Select(x => x.YeuCauNhapVienId.Value)
                .ToList();

            var query = BaseRepository.TableNoTracking
                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => (x.BenhNhan != null ? x.BenhNhan.MaBN : null),
                                                                                x => (x.NoiTruBenhAn != null ? x.NoiTruBenhAn.SoBenhAn : null), x => x.HoTen)
                .Select(item => new TiepNhanNoiTruGridVo()
                {
                    Id = item.YeuCauNhapVienId.Value,
                    YeuCauTiepNhanId = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    BenhNhanId = item.BenhNhanId ?? 0,
                    MaBenhNhan = item.BenhNhan != null ? item.BenhNhan.MaBN : null,
                    HoTen = item.HoTen,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    KhoaNhapVienId = item.YeuCauNhapVien.KhoaPhongNhapVienId,
                    KhoaNhapVien = item.YeuCauNhapVien.KhoaPhongNhapVien.Ten,
                    ThoiGianTiepNhan = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.ThoiDiemTaoBenhAn : (DateTime?)null,
                    SoBenhAn = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.SoBenhAn : null,
                    NoiChiDinh = item.YeuCauNhapVien.NoiChiDinh.Ten,
                    ChanDoan = //item.YeuCauNhapVien.ChanDoanNhapVienICD != null ? item.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + item.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet : null,
                        (string.IsNullOrEmpty(item.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ?
                            (item.YeuCauNhapVien.ChanDoanNhapVienICD != null ? (item.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + item.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet) : null) : item.YeuCauNhapVien.ChanDoanNhapVienGhiChu),
                    //DoiTuong = (item.CoBHYT != true) ? "Viện phí" : "BHYT (" + item.YeuCauTiepNhanTheBHYTs.OrderByDescending(x => x.Id).Select(x => x.MucHuong).FirstOrDefault() + "%)",
                    //MucHuong = item.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //    ? item.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //        .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                    //        .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                    CapCuu = item.NoiTruBenhAn == null ? item.YeuCauNhapVien.LaCapCuu : item.NoiTruBenhAn.LaCapCuu,
                    TrangThai = item.NoiTruBenhAn != null ? Enums.TrangThaiBenhAn.DaTaoBenhAn : Enums.TrangThaiBenhAn.ChuaTaoBenhAn,
                    ThoiGianNhapVien = item.NoiTruBenhAn == null ? (DateTime?)null : item.NoiTruBenhAn.ThoiDiemNhapVien,

                    DaTiepNhan = true,

                    //BVHD-3754
                    MucHuong = item.CoBHYT == true ? item.BHYTMucHuong : (int?)null
                })
                .Union(
                    _yeuCauNhapVienRepository.TableNoTracking
                        .Where(x => //!x.YeuCauTiepNhans.Any()
                                  !yeuCauNhapVienIdDaTiepNhanNoiTru.Contains(x.Id)
                                  && x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                        .ApplyLike(timKiemNangCaoObj.SearchString, x => x.BenhNhan.MaBN, x => x.YeuCauKhamBenh.YeuCauTiepNhan.HoTen)
                    .Select(item => new TiepNhanNoiTruGridVo()
                    {
                        Id = item.Id,
                        YeuCauTiepNhanId = 0,
                        MaTiepNhan = null,
                        BenhNhanId = item.BenhNhanId,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.YeuCauKhamBenh.YeuCauTiepNhan.HoTen,
                        GioiTinh = item.YeuCauKhamBenh.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        KhoaNhapVienId = item.KhoaPhongNhapVienId,
                        KhoaNhapVien = item.KhoaPhongNhapVien.Ten,
                        ThoiGianTiepNhan = null,
                        SoBenhAn = null,
                        NoiChiDinh = item.NoiChiDinh.Ten,
                        ChanDoan =
                        (string.IsNullOrEmpty(item.ChanDoanNhapVienGhiChu) ?
                            (item.ChanDoanNhapVienICD != null ? (item.ChanDoanNhapVienICD.Ma + " - " + item.ChanDoanNhapVienICD.TenTiengViet) : null) : item.ChanDoanNhapVienGhiChu),
                        MucHuong = item.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong,
                        CapCuu = item.LaCapCuu,
                        TrangThai = Enums.TrangThaiBenhAn.ChoQuyetToan

                    }));
            //.ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaTiepNhan, x => x.MaBenhNhan, x => x.SoBenhAn, x => x.HoTen);

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (p.ThoiGianTiepNhan != null && p.ThoiGianTiepNhan.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (p.ThoiGianTiepNhan != null && p.ThoiGianTiepNhan.Value.Date <= denNgay.Date)));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn || timKiemNangCaoObj.TrangThai.DaTaoBenhAn || timKiemNangCaoObj.TrangThai.ChoQuyetToan))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && x.TrangThai == Enums.TrangThaiBenhAn.ChuaTaoBenhAn)
                    || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && x.TrangThai == Enums.TrangThaiBenhAn.DaTaoBenhAn)
                    || (timKiemNangCaoObj.TrangThai.ChoQuyetToan && x.TrangThai == Enums.TrangThaiBenhAn.ChoQuyetToan));
            }

            //if (timKiemNangCaoObj.KhoaPhongId != null)
            //{
            //    query = query.Where(x => x.KhoaNhapVienId == timKiemNangCaoObj.KhoaPhongId);
            //}

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ThenBy(x => x.YeuCauTiepNhanId).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            var result = queryTask.Result.ToList();
            if (result.Count > 0)
            {
                //var lstYCTNDaTiepNhanNoiTruId = queryTask.Result.Where(x => x.DaTiepNhan).Select(x => x.YeuCauTiepNhanId).ToList();
                //var lstYCTNDaTiepNhanNoiTru = BaseRepository.TableNoTracking
                //    .Include(x => x.YeuCauTiepNhanTheBHYTs)
                //    .Where(x => lstYCTNDaTiepNhanNoiTruId.Contains(x.Id)).ToList();
                //foreach (var yctn in queryTask.Result)
                //{
                //    if (yctn.DaTiepNhan)
                //    {
                //        var theBHYT = lstYCTNDaTiepNhanNoiTru.FirstOrDefault(x => x.Id == yctn.YeuCauTiepNhanId);
                //        if (theBHYT != null && theBHYT.YeuCauTiepNhanTheBHYTs.Any())
                //        {
                //            yctn.MucHuong = theBHYT.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //                ? theBHYT.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //                    .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                //                    .Select(a => a.MucHuong).FirstOrDefault() : (int?)null;
                //        }
                //    }
                //}
                if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir) && queryInfo.Sort[0].Field == "DoiTuong")
                {
                    if (queryInfo.Sort[0].Dir == "asc")
                    {
                        result = result.OrderBy(x => x.MucHuong ?? 0).ToList();
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.MucHuong ?? 0).ToList();
                    }
                }
            }

            return new GridDataSource
            {
                Data = result.ToArray(), //queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridTiepNhanNoiTruAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new TiepNhanNoiTruTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString.Trim();
                }
            }

            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            var yeuCauNhapVienIdDaTiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.YeuCauNhapVienId != null
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                .Select(x => x.YeuCauNhapVienId.Value)
                .ToList();

            var query = BaseRepository.TableNoTracking
                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => (x.BenhNhan != null ? x.BenhNhan.MaBN : null),
                    x => (x.NoiTruBenhAn != null ? x.NoiTruBenhAn.SoBenhAn : null), x => x.HoTen)
                .Select(item => new TiepNhanNoiTruGridVo()
                {
                    Id = item.YeuCauNhapVienId.Value,
                    YeuCauTiepNhanId = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    BenhNhanId = item.BenhNhanId ?? 0,
                    MaBenhNhan = item.BenhNhan != null ? item.BenhNhan.MaBN : null,
                    HoTen = item.HoTen,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    KhoaNhapVienId = item.YeuCauNhapVien.KhoaPhongNhapVienId,
                    KhoaNhapVien = item.YeuCauNhapVien.KhoaPhongNhapVien.Ten,
                    ThoiGianTiepNhan = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.ThoiDiemTaoBenhAn : (DateTime?)null,
                    SoBenhAn = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.SoBenhAn : null,
                    NoiChiDinh = item.YeuCauNhapVien.NoiChiDinh.Ten,
                    ChanDoan = //item.YeuCauNhapVien.ChanDoanNhapVienICD != null ? item.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + item.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet : null,
                        (string.IsNullOrEmpty(item.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ?
                            (item.YeuCauNhapVien.ChanDoanNhapVienICD != null ? (item.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + item.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet) : null) : item.YeuCauNhapVien.ChanDoanNhapVienGhiChu),
                    //DoiTuong = (item.CoBHYT != true) ? "Viện phí" : "BHYT (" + item.YeuCauTiepNhanTheBHYTs.OrderByDescending(x => x.Id).Select(x => x.MucHuong).FirstOrDefault() + "%)",
                    //MucHuong = item.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //    ? item.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //        .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                    //        .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                    CapCuu = item.NoiTruBenhAn == null ? item.YeuCauNhapVien.LaCapCuu : item.NoiTruBenhAn.LaCapCuu,
                    TrangThai = item.NoiTruBenhAn != null ? Enums.TrangThaiBenhAn.DaTaoBenhAn : Enums.TrangThaiBenhAn.ChuaTaoBenhAn

                })
                .Union(
                    _yeuCauNhapVienRepository.TableNoTracking
                        .Where(x => //!x.YeuCauTiepNhans.Any()
                                    !yeuCauNhapVienIdDaTiepNhanNoiTru.Contains(x.Id)
                                  && x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                        .ApplyLike(timKiemNangCaoObj.SearchString, x => x.BenhNhan.MaBN, x => x.YeuCauKhamBenh.YeuCauTiepNhan.HoTen)
                    .Select(item => new TiepNhanNoiTruGridVo()
                    {
                        Id = item.Id,
                        YeuCauTiepNhanId = 0,
                        MaTiepNhan = null,
                        BenhNhanId = item.BenhNhanId,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.YeuCauKhamBenh.YeuCauTiepNhan.HoTen,
                        GioiTinh = item.YeuCauKhamBenh.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        KhoaNhapVienId = item.KhoaPhongNhapVienId,
                        KhoaNhapVien = item.KhoaPhongNhapVien.Ten,
                        ThoiGianTiepNhan = null,
                        SoBenhAn = null,
                        NoiChiDinh = item.NoiChiDinh.Ten,
                        ChanDoan =
                        (string.IsNullOrEmpty(item.ChanDoanNhapVienGhiChu) ?
                            (item.ChanDoanNhapVienICD != null ? (item.ChanDoanNhapVienICD.Ma + " - " + item.ChanDoanNhapVienICD.TenTiengViet) : null) : item.ChanDoanNhapVienGhiChu),
                        //MucHuong = item.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong,
                        CapCuu = item.LaCapCuu,
                        TrangThai = Enums.TrangThaiBenhAn.ChoQuyetToan

                    }));
            //.ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaTiepNhan, x => x.MaBenhNhan, x => x.SoBenhAn, x => x.HoTen);

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (p.ThoiGianTiepNhan != null && p.ThoiGianTiepNhan.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (p.ThoiGianTiepNhan != null && p.ThoiGianTiepNhan.Value.Date <= denNgay.Date)));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn || timKiemNangCaoObj.TrangThai.DaTaoBenhAn || timKiemNangCaoObj.TrangThai.ChoQuyetToan))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && x.TrangThai == Enums.TrangThaiBenhAn.ChuaTaoBenhAn)
                    || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && x.TrangThai == Enums.TrangThaiBenhAn.DaTaoBenhAn)
                    || (timKiemNangCaoObj.TrangThai.ChoQuyetToan && x.TrangThai == Enums.TrangThaiBenhAn.ChoQuyetToan));
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsyncVer2(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNoiTruTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString.Trim();
                }
            }

            // sẽ load theo khoa phòng nhân viên đang login
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            IQueryable<TiepNhanNoiTruGridVo> queryDaCoTiepNhanNoiTru = null;
            var listDaCoTiepNhanNoiTru = new List<TiepNhanNoiTruGridVo>();
            IQueryable<TiepNhanNoiTruGridVo> queryChuaCoTiepNhanNoiTru = null;
            var listChuaCoTiepNhanNoiTru = new List<TiepNhanNoiTruGridVo>();

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }

            var daCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                     || timKiemNangCaoObj.TrangThai.DaTaoBenhAn
                                     || timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                     || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                         && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                         && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            var chuaCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                    || timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                    || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                            && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                            && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            // kiểm tra tìm kiếm nâng cao
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            if (daCoTiepNhanNoiTru)
            {
                queryDaCoTiepNhanNoiTru = BaseRepository.TableNoTracking
                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId
                            && (timKiemNangCaoObj.TrangThai == null
                                || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn)
                                || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && x.NoiTruBenhAn != null)
                                || (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && x.NoiTruBenhAn == null))
                            && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (x.ThoiDiemTiepNhan.Date >= tuNgay.Date))
                            && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (x.ThoiDiemTiepNhan.Date <= denNgay.Date)))
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN,
                                                                                x => x.NoiTruBenhAn.SoBenhAn, x => x.HoTen)
                .Select(item => new TiepNhanNoiTruGridVo()
                {
                    Id = item.YeuCauNhapVienId.Value,
                    YeuCauTiepNhanId = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    BenhNhanId = item.BenhNhanId ?? 0,
                    //MaBenhNhan = item.BenhNhan != null ? item.BenhNhan.MaBN : null,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    KhoaNhapVienId = item.YeuCauNhapVien.KhoaPhongNhapVienId,
                    KhoaNhapVien = item.YeuCauNhapVien.KhoaPhongNhapVien.Ten,
                    //ThoiGianTiepNhan = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.ThoiDiemTaoBenhAn : (DateTime?)null,
                    //SoBenhAn = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.SoBenhAn : null,
                    ThoiGianTiepNhan = item.NoiTruBenhAn.ThoiDiemTaoBenhAn,
                    SoBenhAn = item.NoiTruBenhAn.SoBenhAn,
                    NoiChiDinh = item.YeuCauNhapVien.NoiChiDinh.Ten,
                    //ChanDoan =
                    //    (string.IsNullOrEmpty(item.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ?
                    //        (item.YeuCauNhapVien.ChanDoanNhapVienICD != null ? (item.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + item.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet) : null) : item.YeuCauNhapVien.ChanDoanNhapVienGhiChu),
                    ChanDoan = item.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    ChanDoanNhapVienId = item.YeuCauNhapVien.ChanDoanNhapVienICDId,
                    CapCuu = item.NoiTruBenhAn == null ? item.YeuCauNhapVien.LaCapCuu : item.NoiTruBenhAn.LaCapCuu,
                    TrangThai = item.NoiTruBenhAn != null ? Enums.TrangThaiBenhAn.DaTaoBenhAn : Enums.TrangThaiBenhAn.ChuaTaoBenhAn,
                    //ThoiGianNhapVien = item.NoiTruBenhAn == null ? (DateTime?)null : item.NoiTruBenhAn.ThoiDiemNhapVien,
                    ThoiGianNhapVien = item.NoiTruBenhAn.ThoiDiemNhapVien,

                    DaTiepNhan = true,

                    //BVHD-3754
                    MucHuong = item.CoBHYT == true ? item.BHYTMucHuong : (int?)null
                });

                //Cập nhật 03/06/2022: xử lý sort sau khi đã get đủ data
                //listDaCoTiepNhanNoiTru = queryDaCoTiepNhanNoiTru.OrderBy(queryInfo.SortString).ThenBy(x => x.YeuCauTiepNhanId).ToList();
                listDaCoTiepNhanNoiTru = queryDaCoTiepNhanNoiTru.ToList();
            }

            if (chuaCoTiepNhanNoiTru)
            {
                var lstYeuCauNhapVienTheoKhoa = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                    .Select(x => new ThongTinNhapVienTheoKhoaVo()
                    {
                        YeuCauNhapVienId = x.Id,
                        YeuCauTiepNhanIds = x.YeuCauTiepNhans.Select(a => a.Id).ToList()
                    }).ToList();
                var lstYeuCauNhapVienIdChuaCoYCTN = lstYeuCauNhapVienTheoKhoa.Where(x => !x.YeuCauTiepNhanIds.Any()).Select(x => x.YeuCauNhapVienId).ToList();

                queryChuaCoTiepNhanNoiTru = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => //!x.YeuCauTiepNhans.Any()
                                //!yeuCauNhapVienIdDaTiepNhanNoiTru.Contains(x.Id)
                                //&& x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                                lstYeuCauNhapVienIdChuaCoYCTN.Contains(x.Id))
                    .ApplyLike(timKiemNangCaoObj.SearchString, x => x.BenhNhan.MaBN, x => x.YeuCauKhamBenh.YeuCauTiepNhan.HoTen, x => x.BenhNhan.HoTen)
                    .Select(item => new TiepNhanNoiTruGridVo()
                    {
                        Id = item.Id,
                        YeuCauTiepNhanId = 0,
                        MaTiepNhan = null,
                        BenhNhanId = item.BenhNhanId,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.YeuCauKhamBenh.YeuCauTiepNhan.HoTen,

                        //Cập nhật: 03/06/2022
                        HoTenTheoBenhNhanId = item.BenhNhan.HoTen,

                        GioiTinh = item.YeuCauKhamBenh.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        KhoaNhapVienId = item.KhoaPhongNhapVienId,
                        KhoaNhapVien = item.KhoaPhongNhapVien.Ten,
                        ThoiGianTiepNhan = null,
                        SoBenhAn = null,
                        NoiChiDinh = item.NoiChiDinh.Ten,
                        //ChanDoan =
                        //    (string.IsNullOrEmpty(item.ChanDoanNhapVienGhiChu) ?
                        //        (item.ChanDoanNhapVienICD != null ? (item.ChanDoanNhapVienICD.Ma + " - " + item.ChanDoanNhapVienICD.TenTiengViet) : null) : item.ChanDoanNhapVienGhiChu),
                        ChanDoan = item.ChanDoanNhapVienGhiChu,
                        ChanDoanNhapVienId = item.ChanDoanNhapVienICDId,
                        MucHuong = item.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong,
                        CapCuu = item.LaCapCuu,
                        TrangThai = Enums.TrangThaiBenhAn.ChoQuyetToan

                    });

                //Cập nhật 03/06/2022: xử lý sort sau khi đã get đủ data
                //listChuaCoTiepNhanNoiTru = queryChuaCoTiepNhanNoiTru.OrderBy(queryInfo.SortString).ThenBy(x => x.YeuCauTiepNhanId).ToList();
                listChuaCoTiepNhanNoiTru = queryChuaCoTiepNhanNoiTru.ToList();
            }
            var datas = listChuaCoTiepNhanNoiTru.Concat(listDaCoTiepNhanNoiTru).ToList();
            
            var result = datas;
            if (result.Count > 0)
            {
                //Cập nhật 03/06/2022
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.HoTen))
                    {
                        item.HoTen = item.HoTenTheoBenhNhanId;
                    }
                }

                if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir) && queryInfo.Sort[0].Field == "DoiTuong")
                {
                    if (queryInfo.Sort[0].Dir == "asc")
                    {
                        result = result.OrderBy(x => x.MucHuong ?? 0).ToList();
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.MucHuong ?? 0).ToList();
                    }
                }
                //Cập nhật 03/06/2022
                else
                {
                    if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir) && queryInfo.Sort[0].Field == "TrangThai")
                    {
                        result = result.AsQueryable().OrderBy(queryInfo.SortString).ToList();
                    }
                    else
                    {
                        result = result.AsQueryable().OrderBy(x => x.TrangThai).ThenBy(queryInfo.SortString).ToList();
                    }
                }
            }

            result = result.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            if (result.Count > 0)
            {
                var lstChanDoanNhapVienICDId = result
                    .Where(x => string.IsNullOrEmpty(x.ChanDoan) && x.ChanDoanNhapVienId != null)
                    .Select(x => x.ChanDoanNhapVienId.Value)
                    .Distinct().ToList();
                var lstChanDoan = _icdRepository.TableNoTracking
                    .Where(x => lstChanDoanNhapVienICDId.Contains(x.Id))
                    .Select(x => new LookupItemTemplateVo()
                    {
                        KeyId = x.Id,
                        Ten = x.TenTiengViet,
                        Ma = x.Ma
                    }).ToList();
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.ChanDoan) && item.ChanDoanNhapVienId != null)
                    {
                        var chanDoan = lstChanDoan.FirstOrDefault(x => x.KeyId == item.ChanDoanNhapVienId);
                        if (chanDoan != null)
                        {
                            item.ChanDoan = $"{chanDoan.Ma} - {chanDoan.Ten}";
                        }
                    }
                }
            }

            return new GridDataSource
            {
                Data = result.ToArray(),
                TotalRowCount = result.Count
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridTiepNhanNoiTruAsyncVer2(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new TiepNhanNoiTruTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString.Trim();
                }
            }

            // sẽ load theo khoa phòng nhân viên đang login
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            int  countDaCoTiepNhanNoiTru = 0;
            int countChuaCoTiepNhanNoiTru = 0;

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }

            var daCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                     || timKiemNangCaoObj.TrangThai.DaTaoBenhAn
                                     || timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                     || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                         && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                         && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            var chuaCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                    || timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                    || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                            && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                            && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            // kiểm tra tìm kiếm nâng cao
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            if (daCoTiepNhanNoiTru)
            {
                countDaCoTiepNhanNoiTru = BaseRepository.TableNoTracking
                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId
                            && (timKiemNangCaoObj.TrangThai == null
                                || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn)
                                || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && x.NoiTruBenhAn != null)
                                || (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && x.NoiTruBenhAn == null))
                            && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (x.ThoiDiemTiepNhan.Date >= tuNgay.Date))
                            && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (x.ThoiDiemTiepNhan.Date <= denNgay.Date)))
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN,
                                                                                x => x.NoiTruBenhAn.SoBenhAn, x => x.HoTen)
                .Count();
            }

            if (chuaCoTiepNhanNoiTru)
            {
                var lstYeuCauNhapVienTheoKhoa = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                    .Select(x => new ThongTinNhapVienTheoKhoaVo()
                    {
                        YeuCauNhapVienId = x.Id,
                        YeuCauTiepNhanIds = x.YeuCauTiepNhans.Select(a => a.Id).ToList()
                    }).ToList();
                var lstYeuCauNhapVienIdChuaCoYCTN = lstYeuCauNhapVienTheoKhoa.Where(x => !x.YeuCauTiepNhanIds.Any()).Select(x => x.YeuCauNhapVienId).ToList();

                countChuaCoTiepNhanNoiTru = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => //!x.YeuCauTiepNhans.Any()
                                //!yeuCauNhapVienIdDaTiepNhanNoiTru.Contains(x.Id)
                                //&& x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                                lstYeuCauNhapVienIdChuaCoYCTN.Contains(x.Id))
                    .ApplyLike(timKiemNangCaoObj.SearchString, x => x.BenhNhan.MaBN, x => x.YeuCauKhamBenh.YeuCauTiepNhan.HoTen)
                    .Count();
            }

            var total = countChuaCoTiepNhanNoiTru + countDaCoTiepNhanNoiTru;
            return new GridDataSource { TotalRowCount = total };
        }

        #region Cập nhật 11/07/2022: fix grid load chậm
        public async Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsyncVer3(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNoiTruTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString.Trim();
                }
            }

            // sẽ load theo khoa phòng nhân viên đang login
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            IQueryable<TiepNhanNoiTruGridVo> queryDaCoTiepNhanNoiTru = null;
            var listDaCoTiepNhanNoiTru = new List<TiepNhanNoiTruGridVo>();
            IQueryable<TiepNhanNoiTruGridVo> queryChuaCoTiepNhanNoiTru = null;
            var listChuaCoTiepNhanNoiTru = new List<TiepNhanNoiTruGridVo>();

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }

            var daCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                     || timKiemNangCaoObj.TrangThai.DaTaoBenhAn
                                     || timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                     || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                         && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                         && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            var chuaCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                    || timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                    || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                            && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                            && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            // kiểm tra tìm kiếm nâng cao
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            if (daCoTiepNhanNoiTru)
            {
                var khongChonTatCa = timKiemNangCaoObj.TrangThai != null 
                                     && !timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                     && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn
                                     && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn;
                var khongNhapTuNgay = string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay);
                var khongNhapDenNgay = string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay);

                queryDaCoTiepNhanNoiTru = BaseRepository.TableNoTracking
                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId
                            && (timKiemNangCaoObj.TrangThai == null
                                || (khongChonTatCa)
                                || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && x.NoiTruBenhAn != null)
                                || (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && x.NoiTruBenhAn == null))
                            && (khongNhapTuNgay || (x.ThoiDiemTiepNhan.Date >= tuNgay.Date))
                            && (khongNhapDenNgay || (x.ThoiDiemTiepNhan.Date <= denNgay.Date)))
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN,
                                                                                x => x.NoiTruBenhAn.SoBenhAn, x => x.HoTen)
                .Select(item => new TiepNhanNoiTruGridVo()
                {
                    Id = item.YeuCauNhapVienId.Value,
                    YeuCauTiepNhanId = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    BenhNhanId = item.BenhNhanId.GetValueOrDefault(),
                    //MaBenhNhan = item.BenhNhan != null ? item.BenhNhan.MaBN : null,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    KhoaNhapVienId = item.YeuCauNhapVien.KhoaPhongNhapVienId,
                    KhoaNhapVien = item.YeuCauNhapVien.KhoaPhongNhapVien.Ten,
                    //ThoiGianTiepNhan = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.ThoiDiemTaoBenhAn : (DateTime?)null,
                    //SoBenhAn = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.SoBenhAn : null,
                    ThoiGianTiepNhan = item.NoiTruBenhAn.ThoiDiemTaoBenhAn,
                    SoBenhAn = item.NoiTruBenhAn.SoBenhAn,
                    NoiChiDinh = item.YeuCauNhapVien.NoiChiDinh.Ten,
                    //ChanDoan =
                    //    (string.IsNullOrEmpty(item.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ?
                    //        (item.YeuCauNhapVien.ChanDoanNhapVienICD != null ? (item.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + item.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet) : null) : item.YeuCauNhapVien.ChanDoanNhapVienGhiChu),
                    ChanDoan = item.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    ChanDoanNhapVienId = item.YeuCauNhapVien.ChanDoanNhapVienICDId,

                    //Cập nhật 11/07/2022: fix grid load chậm
                    //CapCuu = item.NoiTruBenhAn == null ? item.YeuCauNhapVien.LaCapCuu : item.NoiTruBenhAn.LaCapCuu,
                    CapCuu = item.YeuCauNhapVien.LaCapCuu,
                    BenhAnCapCuu = item.NoiTruBenhAn.LaCapCuu,

                    //Cập nhật 11/07/2022: fix grid load chậm
                    //TrangThai = item.NoiTruBenhAn != null ? Enums.TrangThaiBenhAn.DaTaoBenhAn : Enums.TrangThaiBenhAn.ChuaTaoBenhAn,
                    NoiTruBenhAnId = item.NoiTruBenhAn.Id,

                    //ThoiGianNhapVien = item.NoiTruBenhAn == null ? (DateTime?)null : item.NoiTruBenhAn.ThoiDiemNhapVien,
                    ThoiGianNhapVien = item.NoiTruBenhAn.ThoiDiemNhapVien,

                    DaTiepNhan = true,

                    //BVHD-3754
                    //Cập nhật 11/07/2022: fix grid load chậm
                    //MucHuong = item.CoBHYT == true ? item.BHYTMucHuong : (int?)null
                    CoBHYT = item.CoBHYT,
                    BHYTMucHuong = item.BHYTMucHuong
                });

                //Cập nhật 03/06/2022: xử lý sort sau khi đã get đủ data
                //listDaCoTiepNhanNoiTru = queryDaCoTiepNhanNoiTru.OrderBy(queryInfo.SortString).ThenBy(x => x.YeuCauTiepNhanId).ToList();
                listDaCoTiepNhanNoiTru = queryDaCoTiepNhanNoiTru.ToList();

                #region //Cập nhật 11/07/2022: fix grid load chậm

                if (listDaCoTiepNhanNoiTru.Any())
                {
                    foreach (var tiepNhan in listDaCoTiepNhanNoiTru)
                    {
                        if (tiepNhan.NoiTruBenhAnId.GetValueOrDefault() != 0)
                        {
                            tiepNhan.TrangThai = Enums.TrangThaiBenhAn.DaTaoBenhAn;
                        }
                        else
                        {
                            tiepNhan.TrangThai = Enums.TrangThaiBenhAn.ChuaTaoBenhAn;
                        }

                        if (tiepNhan.CoBHYT.GetValueOrDefault())
                        {
                            tiepNhan.MucHuong = tiepNhan.BHYTMucHuong;
                        }

                        if (tiepNhan.CapCuu != true)
                        {
                            tiepNhan.CapCuu = tiepNhan.BenhAnCapCuu.GetValueOrDefault() == true;
                        }
                    }
                }

                #endregion
            }

            if (chuaCoTiepNhanNoiTru)
            {
                var lstYeuCauNhapVienTheoKhoa = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                    .Select(x => new ThongTinNhapVienTheoKhoaVo()
                    {
                        YeuCauNhapVienId = x.Id,
                        YeuCauTiepNhanIds = x.YeuCauTiepNhans.Select(a => a.Id).ToList()
                    }).ToList();
                var lstYeuCauNhapVienIdChuaCoYCTN = lstYeuCauNhapVienTheoKhoa.Where(x => !x.YeuCauTiepNhanIds.Any()).Select(x => x.YeuCauNhapVienId).ToList();

                queryChuaCoTiepNhanNoiTru = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => //!x.YeuCauTiepNhans.Any()
                                //!yeuCauNhapVienIdDaTiepNhanNoiTru.Contains(x.Id)
                                //&& x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                                lstYeuCauNhapVienIdChuaCoYCTN.Contains(x.Id))
                    .ApplyLike(timKiemNangCaoObj.SearchString, x => x.BenhNhan.MaBN, x => x.YeuCauKhamBenh.YeuCauTiepNhan.HoTen, x => x.BenhNhan.HoTen)
                    .Select(item => new TiepNhanNoiTruGridVo()
                    {
                        Id = item.Id,
                        YeuCauTiepNhanId = 0,
                        MaTiepNhan = null,
                        BenhNhanId = item.BenhNhanId,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.YeuCauKhamBenh.YeuCauTiepNhan.HoTen,

                        //Cập nhật: 03/06/2022
                        HoTenTheoBenhNhanId = item.BenhNhan.HoTen,

                        GioiTinh = item.YeuCauKhamBenh.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        KhoaNhapVienId = item.KhoaPhongNhapVienId,
                        KhoaNhapVien = item.KhoaPhongNhapVien.Ten,
                        ThoiGianTiepNhan = null,
                        SoBenhAn = null,
                        NoiChiDinh = item.NoiChiDinh.Ten,
                        //ChanDoan =
                        //    (string.IsNullOrEmpty(item.ChanDoanNhapVienGhiChu) ?
                        //        (item.ChanDoanNhapVienICD != null ? (item.ChanDoanNhapVienICD.Ma + " - " + item.ChanDoanNhapVienICD.TenTiengViet) : null) : item.ChanDoanNhapVienGhiChu),
                        ChanDoan = item.ChanDoanNhapVienGhiChu,
                        ChanDoanNhapVienId = item.ChanDoanNhapVienICDId,
                        MucHuong = item.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong,
                        CapCuu = item.LaCapCuu,
                        TrangThai = Enums.TrangThaiBenhAn.ChoQuyetToan

                    });

                //Cập nhật 03/06/2022: xử lý sort sau khi đã get đủ data
                //listChuaCoTiepNhanNoiTru = queryChuaCoTiepNhanNoiTru.OrderBy(queryInfo.SortString).ThenBy(x => x.YeuCauTiepNhanId).ToList();
                listChuaCoTiepNhanNoiTru = queryChuaCoTiepNhanNoiTru.ToList();
            }
            var datas = listChuaCoTiepNhanNoiTru.Concat(listDaCoTiepNhanNoiTru).ToList();

            var result = datas;
            if (result.Count > 0)
            {
                //Cập nhật 03/06/2022
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.HoTen))
                    {
                        item.HoTen = item.HoTenTheoBenhNhanId;
                    }
                }

                if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir) && queryInfo.Sort[0].Field == "DoiTuong")
                {
                    if (queryInfo.Sort[0].Dir == "asc")
                    {
                        result = result.OrderBy(x => x.MucHuong ?? 0).ToList();
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.MucHuong ?? 0).ToList();
                    }
                }
                //Cập nhật 03/06/2022
                else
                {
                    if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir) && queryInfo.Sort[0].Field == "TrangThai")
                    {
                        result = result.AsQueryable().OrderBy(queryInfo.SortString).ToList();
                    }
                    else
                    {
                        result = result.AsQueryable().OrderBy(x => x.TrangThai).ThenBy(queryInfo.SortString).ToList();
                    }
                }
            }

            result = result.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            if (result.Count > 0)
            {
                var lstChanDoanNhapVienICDId = result
                    .Where(x => string.IsNullOrEmpty(x.ChanDoan) && x.ChanDoanNhapVienId != null)
                    .Select(x => x.ChanDoanNhapVienId.Value)
                    .Distinct().ToList();
                var lstChanDoan = _icdRepository.TableNoTracking
                    .Where(x => lstChanDoanNhapVienICDId.Contains(x.Id))
                    .Select(x => new LookupItemTemplateVo()
                    {
                        KeyId = x.Id,
                        Ten = x.TenTiengViet,
                        Ma = x.Ma
                    }).ToList();
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.ChanDoan) && item.ChanDoanNhapVienId != null)
                    {
                        var chanDoan = lstChanDoan.FirstOrDefault(x => x.KeyId == item.ChanDoanNhapVienId);
                        if (chanDoan != null)
                        {
                            item.ChanDoan = $"{chanDoan.Ma} - {chanDoan.Ten}";
                        }
                    }
                }
            }

            return new GridDataSource
            {
                Data = result.ToArray(),
                TotalRowCount = result.Count
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridTiepNhanNoiTruAsyncVer3(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new TiepNhanNoiTruTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString.Trim();
                }
            }

            // sẽ load theo khoa phòng nhân viên đang login
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            int countDaCoTiepNhanNoiTru = 0;
            int countChuaCoTiepNhanNoiTru = 0;

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }

            var daCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                     || timKiemNangCaoObj.TrangThai.DaTaoBenhAn
                                     || timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                     || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                         && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                         && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            var chuaCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                    || timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                    || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                            && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                            && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            // kiểm tra tìm kiếm nâng cao
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            if (daCoTiepNhanNoiTru)
            {
                countDaCoTiepNhanNoiTru = BaseRepository.TableNoTracking
                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId
                            && (timKiemNangCaoObj.TrangThai == null
                                || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn)
                                || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && x.NoiTruBenhAn != null)
                                || (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && x.NoiTruBenhAn == null))
                            && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (x.ThoiDiemTiepNhan.Date >= tuNgay.Date))
                            && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (x.ThoiDiemTiepNhan.Date <= denNgay.Date)))
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN,
                                                                                x => x.NoiTruBenhAn.SoBenhAn, x => x.HoTen)
                .Select(x => x.Id)
                .Count();
            }

            if (chuaCoTiepNhanNoiTru)
            {
                var lstYeuCauNhapVienTheoKhoa = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                    .Select(x => new ThongTinNhapVienTheoKhoaVo()
                    {
                        YeuCauNhapVienId = x.Id,
                        YeuCauTiepNhanIds = x.YeuCauTiepNhans.Select(a => a.Id).ToList()
                    }).ToList();
                var lstYeuCauNhapVienIdChuaCoYCTN = lstYeuCauNhapVienTheoKhoa.Where(x => !x.YeuCauTiepNhanIds.Any()).Select(x => x.YeuCauNhapVienId).ToList();

                countChuaCoTiepNhanNoiTru = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => //!x.YeuCauTiepNhans.Any()
                                //!yeuCauNhapVienIdDaTiepNhanNoiTru.Contains(x.Id)
                                //&& x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                                lstYeuCauNhapVienIdChuaCoYCTN.Contains(x.Id))
                    .ApplyLike(timKiemNangCaoObj.SearchString, x => x.BenhNhan.MaBN, x => x.YeuCauKhamBenh.YeuCauTiepNhan.HoTen)
                    .Select(x => x.Id)
                    .Count();
            }

            var total = countChuaCoTiepNhanNoiTru + countDaCoTiepNhanNoiTru;
            return new GridDataSource { TotalRowCount = total };
        }
        #endregion

        #region Cập nhật 26/10/2022: fix grid load chậm
        public async Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsyncVer4(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNoiTruTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString.Trim();
                }
            }

            // sẽ load theo khoa phòng nhân viên đang login
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            //IQueryable<TiepNhanNoiTruGridVo> queryDaCoTiepNhanNoiTru = null;
            var listDaCoTiepNhanNoiTru = new List<TiepNhanNoiTruGridVo>();
            //IQueryable<TiepNhanNoiTruGridVo> queryChuaCoTiepNhanNoiTru = null;
            var listChuaCoTiepNhanNoiTru = new List<TiepNhanNoiTruGridVo>();

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }

            var daCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                     || timKiemNangCaoObj.TrangThai.DaTaoBenhAn
                                     || timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                     || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                         && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                         && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            var chuaCoTiepNhanNoiTru = timKiemNangCaoObj.TrangThai == null
                                    || timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                    || (!timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                            && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn
                                            && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn);

            // kiểm tra tìm kiếm nâng cao
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            if (daCoTiepNhanNoiTru)
            {
                var khongChonTatCa = timKiemNangCaoObj.TrangThai != null
                                     && !timKiemNangCaoObj.TrangThai.ChoQuyetToan
                                     && !timKiemNangCaoObj.TrangThai.DaTaoBenhAn
                                     && !timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn;
                var khongNhapTuNgay = string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay);
                var khongNhapDenNgay = string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay);

                var queryDaCoTiepNhanNoiTru = BaseRepository.TableNoTracking
                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauNhapVienId != null && x.YeuCauNhapVien.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId
                            //&& (timKiemNangCaoObj.TrangThai == null
                            //    || (khongChonTatCa)
                            //    || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && x.NoiTruBenhAn != null)
                            //    || (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && x.NoiTruBenhAn == null))
                            && (khongNhapTuNgay || (x.ThoiDiemTiepNhan.Date >= tuNgay.Date))
                            && (khongNhapDenNgay || (x.ThoiDiemTiepNhan.Date <= denNgay.Date)))
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Select(item => new TiepNhanNoiTruGridVo()
                {
                    Id = item.YeuCauNhapVienId.Value,
                    YeuCauTiepNhanId = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    BenhNhanId = item.BenhNhanId.GetValueOrDefault(),
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    KhoaNhapVienId = item.YeuCauNhapVien.KhoaPhongNhapVienId,
                    KhoaNhapVien = item.YeuCauNhapVien.KhoaPhongNhapVien.Ten,                    
                    NoiChiDinh = item.YeuCauNhapVien.NoiChiDinh.Ten,
                    ChanDoan = item.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    ChanDoanNhapVienId = item.YeuCauNhapVien.ChanDoanNhapVienICDId,
                    CapCuu = item.YeuCauNhapVien.LaCapCuu,
                    //ThoiGianTiepNhan = item.NoiTruBenhAn.ThoiDiemTaoBenhAn,
                    //SoBenhAn = item.NoiTruBenhAn.SoBenhAn,
                    //BenhAnCapCuu = item.NoiTruBenhAn.LaCapCuu,
                    //NoiTruBenhAnId = item.NoiTruBenhAn.Id,
                    //ThoiGianNhapVien = item.NoiTruBenhAn.ThoiDiemNhapVien,
                    DaTiepNhan = true,
                    CoBHYT = item.CoBHYT,
                    BHYTMucHuong = item.BHYTMucHuong
                });

                var tiepNhanNoiTrus = queryDaCoTiepNhanNoiTru.ToList();
                var yeuCauTiepNhanIds = tiepNhanNoiTrus.Select(o => o.YeuCauTiepNhanId).ToList();
                var noiTruBenhAns = _noiTruBenhAnRepository.TableNoTracking
                    .Where(o => yeuCauTiepNhanIds.Contains(o.Id))
                    .Select(o => new
                    {
                        o.ThoiDiemTaoBenhAn,
                        o.SoBenhAn,
                        o.LaCapCuu,
                        o.Id,
                        o.ThoiDiemNhapVien,
                    }).ToList();

                foreach (var tiepNhanNoiTru in tiepNhanNoiTrus)
                {
                    var noiTruBenhAn = noiTruBenhAns.FirstOrDefault(o => o.Id == tiepNhanNoiTru.YeuCauTiepNhanId);
                    if(noiTruBenhAn != null)
                    {
                        tiepNhanNoiTru.ThoiGianTiepNhan = noiTruBenhAn.ThoiDiemTaoBenhAn;
                        tiepNhanNoiTru.SoBenhAn = noiTruBenhAn.SoBenhAn;
                        tiepNhanNoiTru.BenhAnCapCuu = noiTruBenhAn.LaCapCuu;
                        tiepNhanNoiTru.NoiTruBenhAnId = noiTruBenhAn.Id;
                        tiepNhanNoiTru.ThoiGianNhapVien = noiTruBenhAn.ThoiDiemNhapVien;
                    }
                }
                listDaCoTiepNhanNoiTru = tiepNhanNoiTrus
                    .Where(o => timKiemNangCaoObj.TrangThai == null || (khongChonTatCa)
                                || (timKiemNangCaoObj.TrangThai.DaTaoBenhAn && o.NoiTruBenhAnId != null)
                                || (timKiemNangCaoObj.TrangThai.ChuaTaoBenhAn && o.NoiTruBenhAnId == null))
                    .ToList();

                if (listDaCoTiepNhanNoiTru.Any())
                {
                    foreach (var tiepNhan in listDaCoTiepNhanNoiTru)
                    {
                        if (tiepNhan.NoiTruBenhAnId.GetValueOrDefault() != 0)
                        {
                            tiepNhan.TrangThai = Enums.TrangThaiBenhAn.DaTaoBenhAn;
                        }
                        else
                        {
                            tiepNhan.TrangThai = Enums.TrangThaiBenhAn.ChuaTaoBenhAn;
                        }

                        if (tiepNhan.CoBHYT.GetValueOrDefault())
                        {
                            tiepNhan.MucHuong = tiepNhan.BHYTMucHuong;
                        }

                        if (tiepNhan.CapCuu != true)
                        {
                            tiepNhan.CapCuu = tiepNhan.BenhAnCapCuu.GetValueOrDefault() == true;
                        }
                    }
                }
            }

            if (chuaCoTiepNhanNoiTru)
            {
                var lstYeuCauNhapVienTheoKhoa = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                    .Select(x => new ThongTinNhapVienTheoKhoaVo()
                    {
                        YeuCauNhapVienId = x.Id,
                        YeuCauTiepNhanIds = x.YeuCauTiepNhans.Select(a => a.Id).ToList()
                    }).ToList();
                var lstYeuCauNhapVienIdChuaCoYCTN = lstYeuCauNhapVienTheoKhoa.Where(x => !x.YeuCauTiepNhanIds.Any()).Select(x => x.YeuCauNhapVienId).ToList();

                var queryChuaCoTiepNhanNoiTru = _yeuCauNhapVienRepository.TableNoTracking
                    .Where(x => //!x.YeuCauTiepNhans.Any()
                                //!yeuCauNhapVienIdDaTiepNhanNoiTru.Contains(x.Id)
                                //&& x.KhoaPhongNhapVienId == phongBenhVien.KhoaPhongId)
                                lstYeuCauNhapVienIdChuaCoYCTN.Contains(x.Id))
                    .ApplyLike(timKiemNangCaoObj.SearchString, x => x.BenhNhan.MaBN, x => x.YeuCauKhamBenh.YeuCauTiepNhan.HoTen, x => x.BenhNhan.HoTen)
                    .Select(item => new TiepNhanNoiTruGridVo()
                    {
                        Id = item.Id,
                        YeuCauTiepNhanId = 0,
                        MaTiepNhan = null,
                        BenhNhanId = item.BenhNhanId,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.YeuCauKhamBenh.YeuCauTiepNhan.HoTen,

                        //Cập nhật: 03/06/2022
                        HoTenTheoBenhNhanId = item.BenhNhan.HoTen,

                        GioiTinh = item.YeuCauKhamBenh.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        KhoaNhapVienId = item.KhoaPhongNhapVienId,
                        KhoaNhapVien = item.KhoaPhongNhapVien.Ten,
                        ThoiGianTiepNhan = null,
                        SoBenhAn = null,
                        NoiChiDinh = item.NoiChiDinh.Ten,
                        //ChanDoan =
                        //    (string.IsNullOrEmpty(item.ChanDoanNhapVienGhiChu) ?
                        //        (item.ChanDoanNhapVienICD != null ? (item.ChanDoanNhapVienICD.Ma + " - " + item.ChanDoanNhapVienICD.TenTiengViet) : null) : item.ChanDoanNhapVienGhiChu),
                        ChanDoan = item.ChanDoanNhapVienGhiChu,
                        ChanDoanNhapVienId = item.ChanDoanNhapVienICDId,
                        MucHuong = item.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong,
                        CapCuu = item.LaCapCuu,
                        TrangThai = Enums.TrangThaiBenhAn.ChoQuyetToan

                    });

                //Cập nhật 03/06/2022: xử lý sort sau khi đã get đủ data
                //listChuaCoTiepNhanNoiTru = queryChuaCoTiepNhanNoiTru.OrderBy(queryInfo.SortString).ThenBy(x => x.YeuCauTiepNhanId).ToList();
                listChuaCoTiepNhanNoiTru = queryChuaCoTiepNhanNoiTru.ToList();
            }
            var datas = listChuaCoTiepNhanNoiTru.Concat(listDaCoTiepNhanNoiTru).ToList();

            var result = datas;
            var totalRowCount = datas.Count();
            if (result.Count > 0)
            {
                //Cập nhật 03/06/2022
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.HoTen))
                    {
                        item.HoTen = item.HoTenTheoBenhNhanId;
                    }
                }

                if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir) && queryInfo.Sort[0].Field == "DoiTuong")
                {
                    if (queryInfo.Sort[0].Dir == "asc")
                    {
                        result = result.OrderBy(x => x.MucHuong ?? 0).ToList();
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.MucHuong ?? 0).ToList();
                    }
                }
                //Cập nhật 03/06/2022
                else
                {
                    if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir) && queryInfo.Sort[0].Field == "TrangThai")
                    {
                        result = result.AsQueryable().OrderBy(queryInfo.SortString).ToList();
                    }
                    else
                    {
                        result = result.AsQueryable().OrderBy(x => x.TrangThai).ThenBy(queryInfo.SortString).ToList();
                    }
                }
            }

            result = result.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            if (result.Count > 0)
            {
                var lstChanDoanNhapVienICDId = result
                    .Where(x => string.IsNullOrEmpty(x.ChanDoan) && x.ChanDoanNhapVienId != null)
                    .Select(x => x.ChanDoanNhapVienId.Value)
                    .Distinct().ToList();
                var lstChanDoan = _icdRepository.TableNoTracking
                    .Where(x => lstChanDoanNhapVienICDId.Contains(x.Id))
                    .Select(x => new LookupItemTemplateVo()
                    {
                        KeyId = x.Id,
                        Ten = x.TenTiengViet,
                        Ma = x.Ma
                    }).ToList();
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.ChanDoan) && item.ChanDoanNhapVienId != null)
                    {
                        var chanDoan = lstChanDoan.FirstOrDefault(x => x.KeyId == item.ChanDoanNhapVienId);
                        if (chanDoan != null)
                        {
                            item.ChanDoan = $"{chanDoan.Ma} - {chanDoan.Ten}";
                        }
                    }
                }
            }

            return new GridDataSource
            {
                Data = result.ToArray(),
                TotalRowCount = totalRowCount
            };
        }        
        #endregion

        public async Task<GridDataSource> GetDataForGridSoDoGiuongTiepNhanNoiTruAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNoiTruSoDoGiuongTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruSoDoGiuongTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
            var lstGiuongBenhIdDangSuDung = new List<long>();
            if (timKiemNangCaoObj.YeuCauTiepNhanNoiTruId != null
                && timKiemNangCaoObj.YeuCauTiepNhanNoiTruId != 0
                && (timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == null || timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == 0))
            {
                lstGiuongBenhIdDangSuDung = await GetListGiuongIdDangSuDungCuaNguoiBenhAsync(timKiemNangCaoObj.YeuCauTiepNhanNoiTruId.Value);
            }
            //--------------------------------------------------------------------------------------------------------------------

            var maxBenhNhan = int.Parse(_cauHinhService.GetSetting("CauHinhNoiTru.SoLuongBenhNhanToiDaTrenGiuong").Value);
            var query = await _phongBenhVienRepository.TableNoTracking
                .Where(x => x.KhoaPhongId == timKiemNangCaoObj.KhoaPhongId
                            && (timKiemNangCaoObj.PhongBenhVienId == null || x.Id == timKiemNangCaoObj.PhongBenhVienId)
                            && x.IsDisabled != true
                            && x.GiuongBenhs.Any(a => a.IsDisabled != true)
                            && ((!timKiemNangCaoObj.GiuongTrong && !timKiemNangCaoObj.GiuongDaCoBenhNhan)
                                || (timKiemNangCaoObj.GiuongTrong && x.GiuongBenhs.All(a => !a.HoatDongGiuongBenhs.Any(b => (timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == null || b.YeuCauDichVuGiuongBenhVienId != timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId) && (
                                                                                                                                (timKiemNangCaoObj.ThoiGianNhan == null ||
                                                                                                                                ((b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianNhan.Value && (b.ThoiDiemKetThuc == null || timKiemNangCaoObj.ThoiGianNhan.Value <= b.ThoiDiemKetThuc)) ||
                                                                                                                                b.ThoiDiemBatDau >= timKiemNangCaoObj.ThoiGianNhan.Value)) &&
                                                                                                                                (timKiemNangCaoObj.ThoiGianTra == null ||
                                                                                                                                (b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianTra.Value))
                                                                                                                            ))))
                                || (timKiemNangCaoObj.GiuongDaCoBenhNhan && x.GiuongBenhs.Any(a => a.HoatDongGiuongBenhs.Any(b => (timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == null || b.YeuCauDichVuGiuongBenhVienId != timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId) && (
                                                                                                                                      (timKiemNangCaoObj.ThoiGianNhan == null ||
                                                                                                                                      ((b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianNhan.Value && (b.ThoiDiemKetThuc == null || timKiemNangCaoObj.ThoiGianNhan.Value <= b.ThoiDiemKetThuc)) ||
                                                                                                                                      b.ThoiDiemBatDau >= timKiemNangCaoObj.ThoiGianNhan.Value)) &&
                                                                                                                                      (timKiemNangCaoObj.ThoiGianTra == null ||
                                                                                                                                      (b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianTra.Value))
                                                                                                                                  ) ||
                                                                                                                                  (timKiemNangCaoObj.ThoiGianNhan == null && timKiemNangCaoObj.ThoiGianTra == null && (b.ThoiDiemKetThuc == null || b.ThoiDiemKetThuc.Value > DateTime.Now)))))))
                //&& ((!timKiemNangCaoObj.GiuongTrong && !timKiemNangCaoObj.GiuongDaCoBenhNhan) 
                //    || (timKiemNangCaoObj.GiuongTrong && x.GiuongBenhs.All(a => !a.HoatDongGiuongBenhs.Any()))
                //    || (timKiemNangCaoObj.GiuongDaCoBenhNhan && x.GiuongBenhs.Any(a => a.HoatDongGiuongBenhs.Any(b => b.ThoiDiemKetThuc == null || b.ThoiDiemKetThuc.Value > DateTime.Now)))))
                .Select(item => new TiepNhanNoiTruSoDoGiuongGridVo()
                {
                    Id = item.Id,
                    Phong = item.Ten,
                    Tang = item.Tang,
                    GiuongBenhs = item.GiuongBenhs.Where(a => a.IsDisabled != true).Select(giuong => new TiepNhanNoiTruGiuongVo()
                    {
                        TenGiuong = giuong.Ten,
                        GiuongId = giuong.Id,
                        //SoLuongBenhNhan = giuong.HoatDongGiuongBenhs.Count(a => (timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == null || a.YeuCauDichVuGiuongBenhVienId != timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId) && a.ThoiDiemKetThuc == null),
                        SoLuongBenhNhanToiDa = maxBenhNhan,
                        BenhNhans = giuong.HoatDongGiuongBenhs
                            .Where(b => (timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == null || b.YeuCauDichVuGiuongBenhVienId != timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId) && (
                                            (timKiemNangCaoObj.ThoiGianNhan == null ||
                                            ((b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianNhan.Value && (b.ThoiDiemKetThuc == null || timKiemNangCaoObj.ThoiGianNhan.Value <= b.ThoiDiemKetThuc)) ||
                                            b.ThoiDiemBatDau >= timKiemNangCaoObj.ThoiGianNhan.Value)) &&
                                            (timKiemNangCaoObj.ThoiGianTra == null ||
                                            (b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianTra.Value))
                                        ) ||
                                        (timKiemNangCaoObj.ThoiGianNhan == null && timKiemNangCaoObj.ThoiGianTra == null && (b.ThoiDiemKetThuc == null || b.ThoiDiemKetThuc.Value > DateTime.Now)))
                            //.Where(b => b.ThoiDiemKetThuc == null || b.ThoiDiemKetThuc.Value > DateTime.Now)
                            .Select(benhNhan => new TiepNhanNoiTruBenhNhanTrenGiuongVo()
                            {
                                DichVuGiuong = benhNhan.YeuCauDichVuGiuongBenhVien.DichVuGiuongBenhVien.Ten,
                                MaGiuong = benhNhan.YeuCauDichVuGiuongBenhVien.MaGiuong,
                                Phong = item.Ten,
                                Tang = item.Tang,
                                SoBenhAn = benhNhan.YeuCauTiepNhan.NoiTruBenhAn != null ? benhNhan.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : null,
                                TenBenhNhan = benhNhan.YeuCauTiepNhan.HoTen,
                                NgayVao = benhNhan.YeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung != null ? benhNhan.YeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung.Value.ApplyFormatDateTimeSACH() : null,
                                NgayRa = benhNhan.ThoiDiemKetThuc == null ? "" : benhNhan.ThoiDiemKetThuc.Value.ApplyFormatDateTimeSACH(),
                                BaoPhong = benhNhan.YeuCauDichVuGiuongBenhVien.BaoPhong,

                                // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                                LaGiuongDangSuDung = lstGiuongBenhIdDangSuDung.Contains(benhNhan.GiuongBenhId),
                                BenhNhanId = benhNhan.YeuCauTiepNhan.BenhNhanId
                                //-------------------------------------------------------------------------
                            }).ToList()
                    }).ToList()
                }).ToListAsync();

            return new GridDataSource
            {
                Data = query.ToArray(),
                TotalRowCount = query.Count
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridSoDoGiuongTiepNhanNoiTruAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new TiepNhanNoiTruSoDoGiuongTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNoiTruSoDoGiuongTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            //var maxBenhNhan = int.Parse(_cauHinhService.GetSetting("CauHinhNoiTru.SoLuongBenhNhanToiDaTrenGiuong").Value);
            var query = await _phongBenhVienRepository.TableNoTracking
                .Where(x => x.KhoaPhongId == timKiemNangCaoObj.KhoaPhongId
                            && (timKiemNangCaoObj.PhongBenhVienId == null || x.Id == timKiemNangCaoObj.PhongBenhVienId)
                            && x.IsDisabled != true
                            && x.GiuongBenhs.Any(a => a.IsDisabled != true)
                            && ((!timKiemNangCaoObj.GiuongTrong && !timKiemNangCaoObj.GiuongDaCoBenhNhan)
                                || (timKiemNangCaoObj.GiuongTrong && x.GiuongBenhs.All(a => !a.HoatDongGiuongBenhs.Any(b => (timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == null || b.YeuCauDichVuGiuongBenhVienId != timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId) && (
                                                                                                                                (timKiemNangCaoObj.ThoiGianNhan == null ||
                                                                                                                                ((b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianNhan.Value && (b.ThoiDiemKetThuc == null || timKiemNangCaoObj.ThoiGianNhan.Value <= b.ThoiDiemKetThuc)) ||
                                                                                                                                b.ThoiDiemBatDau >= timKiemNangCaoObj.ThoiGianNhan.Value)) &&
                                                                                                                                (timKiemNangCaoObj.ThoiGianTra == null ||
                                                                                                                                (b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianTra.Value))
                                                                                                                            ))))
                                || (timKiemNangCaoObj.GiuongDaCoBenhNhan && x.GiuongBenhs.Any(a => a.HoatDongGiuongBenhs.Any(b => (timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId == null || b.YeuCauDichVuGiuongBenhVienId != timKiemNangCaoObj.YeuCauDichVuGiuongBenhVienId) && (
                                                                                                                                      (timKiemNangCaoObj.ThoiGianNhan == null ||
                                                                                                                                      ((b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianNhan.Value && (b.ThoiDiemKetThuc == null || timKiemNangCaoObj.ThoiGianNhan.Value <= b.ThoiDiemKetThuc)) ||
                                                                                                                                      b.ThoiDiemBatDau >= timKiemNangCaoObj.ThoiGianNhan.Value)) &&
                                                                                                                                      (timKiemNangCaoObj.ThoiGianTra == null ||
                                                                                                                                      (b.ThoiDiemBatDau <= timKiemNangCaoObj.ThoiGianTra.Value))
                                                                                                                                  ) ||
                                                                                                                                  (timKiemNangCaoObj.ThoiGianNhan == null && timKiemNangCaoObj.ThoiGianTra == null && (b.ThoiDiemKetThuc == null || b.ThoiDiemKetThuc.Value > DateTime.Now)))))))
                //|| (timKiemNangCaoObj.GiuongTrong && x.GiuongBenhs.All(a => !a.HoatDongGiuongBenhs.Any()))
                //|| (timKiemNangCaoObj.GiuongDaCoBenhNhan && x.GiuongBenhs.Any(a => a.HoatDongGiuongBenhs.Any(b => b.ThoiDiemKetThuc == null || b.ThoiDiemKetThuc.Value > DateTime.Now)))))
                .Select(item => new TiepNhanNoiTruSoDoGiuongGridVo()
                {
                    Phong = item.Ten,
                    Tang = item.Tang,
                    //GiuongBenhs = item.GiuongBenhs.Select(giuong => new TiepNhanNoiTruGiuongVo()
                    //{
                    //    TenGiuong = giuong.Ten,
                    //    GiuongId = giuong.Id,
                    //    SoLuongBenhNhan = giuong.HoatDongGiuongBenhs.Count(a => a.ThoiDiemKetThuc == null),
                    //    SoLuongBenhNhanToiDa = maxBenhNhan,
                    //    BenhNhans = giuong.HoatDongGiuongBenhs.Select(benhNhan => new TiepNhanNoiTruBenhNhanTrenGiuongVo()
                    //    {
                    //        DichVuGiuong = benhNhan.YeuCauDichVuGiuongBenhVien.DichVuGiuongBenhVien.Ten,
                    //        MaGiuong = benhNhan.YeuCauDichVuGiuongBenhVien.MaGiuong,
                    //        SoBenhAn = benhNhan.YeuCauTiepNhan.NoiTruBenhAn != null ? benhNhan.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : null,
                    //        TenBenhNhan = benhNhan.YeuCauTiepNhan.HoTen,
                    //        NgayVao = benhNhan.YeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung != null ? benhNhan.YeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung.Value.ApplyFormatDateTimeSACH() : null
                    //    }).ToList()
                    //}).ToList()
                }).ToListAsync();

            return new GridDataSource { TotalRowCount = query.Count };
        }

        public async Task<GridDataSource> GetDataLichSuChuyenDoiTuongForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? 0 : long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTiepNhanLichSuChuyenDoiTuongRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                .Select(item => new TiepNhanNoiTruLichSuChuyenDoiTuongGridVo()
                {
                    Id = item.Id,
                    DoiTuong = item.DoiTuongTiepNhan.GetDescription(),
                    SoTheBaoHiem = item.MaSoThe,
                    DiaChiThe = item.DiaChi,
                    MucHuong = string.IsNullOrEmpty(item.MaSoThe) ? "" : item.MucHuong + "%",
                    TuNgay = item.NgayHieuLuc,
                    DenNgay = item.NgayHetHan,
                    NoiDangKyBaoHiem = string.IsNullOrEmpty(item.MaDKBD) ? "" : _benhVienRepository.TableNoTracking.Where(a => a.Ma == item.MaDKBD).Select(a => a.Ten).FirstOrDefault(),
                    NgayNhap = item.CreatedOn,
                    ThoiGianMienDongChiTra = item.NgayDuocMienCungChiTra,
                    GiaHanThe = item.DuocGiaHanThe,
                    DaHuy = item.DaHuy
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(x => x.Id).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageLichSuChuyenDoiTuongForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? 0 : long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTiepNhanLichSuChuyenDoiTuongRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                .Select(item => new TiepNhanNoiTruLichSuChuyenDoiTuongGridVo()
                {
                    DoiTuong = item.DoiTuongTiepNhan.GetDescription(),
                    SoTheBaoHiem = item.MaSoThe,
                    DiaChiThe = item.DiaChi,
                    MucHuong = item.MucHuong + "%",
                    TuNgay = item.NgayHieuLuc,
                    DenNgay = item.NgayHetHan,
                    NoiDangKyBaoHiem = null,
                    NgayNhap = null,
                    ThoiGianMienDongChiTra = item.NgayDuocMienCungChiTra,
                    GiaHanThe = item.DuocGiaHanThe
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region get data

        public List<LookupItemVo> GetListLoaiBenhAn(DropDownListRequestModel queryInfo)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiBenhAn>();
            var result = listEnum.Where(x => x != Enums.LoaiBenhAn.TreSoSinh)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                })
                .ToList();
            return result;
        }

        public async Task<List<LookupItemNoiTruCongTyBHTNVo>> GetCongTyBaoHiemTuNhanAsync(DropDownListRequestModel queryInfo)
        {
            var lst = await _congTyBaoHiemTuNhanRepository.TableNoTracking
                .Select(item => new LookupItemNoiTruCongTyBHTNVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    DiaChi = item.DiaChi,
                    SoDienThoai = item.SoDienThoai
                })
                .ApplyLike(queryInfo.Query, x => x.DisplayName)
                .Take(queryInfo.Take)
                .ToListAsync();

            return lst;
        }

        #endregion

        #region thêm/xóa/sửa

        public async Task XuLyTaoBenhAnAsync(Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn noiTruBenhAn)
        {
            var yeuCauTiepNhanChiTiet = await BaseRepository.Table
                .Include(x => x.YeuCauNhapVien).ThenInclude(y => y.NoiChiDinh)
                .Where(x => x.Id == noiTruBenhAn.Id)
                .FirstOrDefaultAsync();
            if (yeuCauTiepNhanChiTiet == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            if ((noiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || noiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong)
                && yeuCauTiepNhanChiTiet.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam)
            {
                throw new Exception(_localizationService.GetResource("BenhAnSanKhoa.GioiTinh.KhongPhaiNu"));
            }

            noiTruBenhAn.KhoaPhongNhapVienId = yeuCauTiepNhanChiTiet.YeuCauNhapVien.KhoaPhongNhapVienId;

            var newKhoaPhongDieuTri = new NoiTruKhoaPhongDieuTri()
            {
                KhoaPhongChuyenDiId = yeuCauTiepNhanChiTiet.YeuCauNhapVien.NoiChiDinh.KhoaPhongId,
                KhoaPhongChuyenDenId = yeuCauTiepNhanChiTiet.YeuCauNhapVien.KhoaPhongNhapVienId,
                ThoiDiemVaoKhoa = noiTruBenhAn.ThoiDiemNhapVien, //yeuCauTiepNhanChiTiet.YeuCauNhapVien.CreatedOn.Value,
                ChanDoanVaoKhoaICDId = yeuCauTiepNhanChiTiet.YeuCauNhapVien.ChanDoanNhapVienICDId,
                ChanDoanVaoKhoaGhiChu = yeuCauTiepNhanChiTiet.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                NhanVienChiDinhId = yeuCauTiepNhanChiTiet.YeuCauNhapVien.BacSiChiDinhId,
                LyDoChuyenKhoa = yeuCauTiepNhanChiTiet.YeuCauNhapVien.LyDoNhapVien
            };

            noiTruBenhAn.NoiTruKhoaPhongDieuTris.Add(newKhoaPhongDieuTri);

            noiTruBenhAn.SoBenhAn = Helpers.ResourceHelper.CreateSoBenhAn();
            noiTruBenhAn.SoLuuTru = Helpers.ResourceHelper.CreateSoLuuTru();

            //await _noiTruBenhAnRepository.AddAsync(noiTruBenhAn);
            yeuCauTiepNhanChiTiet.NoiTruBenhAn = noiTruBenhAn;
            yeuCauTiepNhanChiTiet.ThoiDiemTiepNhan = DateTime.Now;
            yeuCauTiepNhanChiTiet.NhanVienTiepNhanId = _userAgentHelper.GetCurrentUserId();

            if (noiTruBenhAn.LoaiBenhAn != Enums.LoaiBenhAn.NhiKhoa)
            {
                yeuCauTiepNhanChiTiet.HoTenBo = null;
                yeuCauTiepNhanChiTiet.TrinhDoVanHoaCuaBo = null;
                yeuCauTiepNhanChiTiet.NgheNghiepCuaBoId = null;

                yeuCauTiepNhanChiTiet.HoTenMe = null;
                yeuCauTiepNhanChiTiet.TrinhDoVanHoaCuaMe = null;
                yeuCauTiepNhanChiTiet.NgheNghiepCuaMeId = null;
            }

            await BaseRepository.UpdateAsync(yeuCauTiepNhanChiTiet);
        }

        public async Task XuLyCapNhatBenhAnAsync(YeuCauTiepNhan yeuCauTiepNhan, Enums.LoaiBenhAn loaiBenhAnTruocCapNhat)
        {
            if (((!string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinBenhAn)
                  || !string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinTongKetBenhAn)
                  || !string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien))
                    && loaiBenhAnTruocCapNhat != yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn)
                || yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DaThucHien"));
            }
            //await _noiTruBenhAnRepository.UpdateAsync(noiTruBenhAn);

            if ((yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong)
                && yeuCauTiepNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam)
            {
                throw new Exception(_localizationService.GetResource("BenhAnSanKhoa.GioiTinh.KhongPhaiNu"));
            }

            if (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn != Enums.LoaiBenhAn.NhiKhoa)
            {
                yeuCauTiepNhan.HoTenBo = null;
                yeuCauTiepNhan.TrinhDoVanHoaCuaBo = null;
                yeuCauTiepNhan.NgheNghiepCuaBoId = null;

                yeuCauTiepNhan.HoTenMe = null;
                yeuCauTiepNhan.TrinhDoVanHoaCuaMe = null;
                yeuCauTiepNhan.NgheNghiepCuaMeId = null;
            }

            var khoaPhongDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.FirstOrDefault();
            if (khoaPhongDieuTri != null)
            {
                if (khoaPhongDieuTri.ThoiDiemRaKhoa != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien > khoaPhongDieuTri.ThoiDiemRaKhoa)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.ThoiDiemNhapVien.ThoiDiemVaoKhoaLonHonRaKhoa"));
                }
                khoaPhongDieuTri.ThoiDiemVaoKhoa = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien;
            }

            var yeuCauGiuong = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.FirstOrDefault();
            if (yeuCauGiuong != null)
            {
                if (yeuCauGiuong.ThoiDiemKetThucSuDung != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien > yeuCauGiuong.ThoiDiemKetThucSuDung)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.ThoiDiemNhapVien.ThoiGianNhanGiuongLonHonKetThuc"));
                }
                yeuCauGiuong.ThoiDiemBatDauSuDung = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien;
                var hoatDongGiuong = yeuCauGiuong.HoatDongGiuongBenhs.FirstOrDefault();
                if (hoatDongGiuong != null)
                {
                    hoatDongGiuong.ThoiDiemBatDau = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien;
                }
            }

            var ekipDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruEkipDieuTris.FirstOrDefault();
            if (ekipDieuTri != null)
            {
                if (ekipDieuTri.DenNgay != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien > ekipDieuTri.DenNgay)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.ThoiDiemNhapVien.ThoiGianBatDauEkipLonHonKetThuc"));
                }
                ekipDieuTri.TuNgay = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien;
            }
            await BaseRepository.UpdateAsync(yeuCauTiepNhan);
        }

        public async Task XuLyChiDinhEkipVaDichVuGiuongNoiTruAsync(ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet)
        {
            // xử lý thêm ekip thực hiện
            var newEkip = new NoiTruEkipDieuTri()
            {
                BacSiId = yeuCauVo.BacSiDieuTriId,
                DieuDuongId = yeuCauVo.DieuDuongId,
                NhanVienLapId = _userAgentHelper.GetCurrentUserId(),
                KhoaPhongDieuTriId = yeuCauTiepNhanChiTiet.NoiTruBenhAn.KhoaPhongNhapVienId,
                TuNgay = yeuCauVo.TuNgay
            };

            yeuCauTiepNhanChiTiet.NoiTruBenhAn.NoiTruEkipDieuTris.Add(newEkip);

            // xử lý thêm dịch vụ giường
            if (yeuCauVo.DichVuGiuongId != 0)
            {
                var dichVuGiuong = await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Include(x => x.DichVuGiuong)
                    .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                    .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuGiuongBenhVien)
                    .Where(x => x.Id == yeuCauVo.DichVuGiuongId
                    //&& x.HieuLuc
                    ).FirstOrDefaultAsync();
                var giuongBenh = await _giuongBenhRepository.GetByIdAsync(yeuCauVo.GiuongId);

                if (dichVuGiuong == null)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuong.NotExists"));
                }

                if (!dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Any())
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuongNhomGia.NotExists"));
                }

                var giaBaoHiem = dichVuGiuong.DichVuGiuongBenhVienGiaBaoHiems
                    .FirstOrDefault(o =>
                        o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
                var giaBenhViens = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens
                    .Where(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value))
                    .Select(x => x).ToList();
                var giaBenhVien = new DichVuGiuongBenhVienGiaBenhVien();
                if (yeuCauVo.BaoPhong)
                {
                    giaBenhVien = giaBenhViens
                        .Where(x => x.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng").FirstOrDefault();
                    if (giaBenhVien == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("NoiTruBenhAn.NhomGiaBaoPhong.KhongCoHieuLuc"));
                    }
                }
                else
                {
                    giaBenhVien = giaBenhViens
                        .Where(x => x.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() != "bao phòng").FirstOrDefault();
                    if (giaBenhVien == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("NoiTruBenhAn.NhomGiaThuong.KhongCoHieuLuc"));
                    }
                }


                var newYeuCauDichVuGiuong = new YeuCauDichVuGiuongBenhVien()
                {
                    DichVuGiuongBenhVienId = yeuCauVo.DichVuGiuongId,
                    GiuongBenhId = yeuCauVo.GiuongId,
                    NhomGiaDichVuGiuongBenhVienId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId,
                    Ten = dichVuGiuong.Ten,
                    Ma = dichVuGiuong.Ma,
                    MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong.MaTT37 : null,
                    MaGiuong = giuongBenh.Ma,
                    TenGiuong = giuongBenh.Ten,
                    LoaiGiuong = (Enums.EnumLoaiGiuong)yeuCauVo.LoaiGiuong,
                    MoTa = dichVuGiuong.MoTa,
                    KhongTinhPhi = false,
                    DuocHuongBaoHiem = yeuCauTiepNhanChiTiet.CoBHYT == true && giaBaoHiem != null,
                    BaoHiemChiTra = null,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                    TrangThai = Enums.EnumTrangThaiGiuongBenh.ChuaThucHien,
                    NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                    NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                    ThoiDiemChiDinh = DateTime.Now,
                    DonGiaBaoHiem = giaBaoHiem?.Gia,
                    TiLeBaoHiemThanhToan = giaBaoHiem?.TiLeBaoHiemThanhToan,
                    MucHuongBaoHiem = yeuCauTiepNhanChiTiet.BHYTMucHuong,
                    Gia = giaBenhVien.Gia,

                    ThoiDiemBatDauSuDung = yeuCauTiepNhanChiTiet.NoiTruBenhAn.ThoiDiemNhapVien,
                    BaoPhong = yeuCauVo.BaoPhong,
                    DoiTuongSuDung = Enums.DoiTuongSuDung.BenhNhan
                };

                // kiểm tra còn tồn dịch vụ giường trong gói ko
                var dichVuInfo = new DichVuGiuongTrongGoiVo()
                {
                    DichVuBenhVienId = yeuCauVo.DichVuGiuongId,
                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId ?? 0,
                    NhomGiaDichVuId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId,
                    YeuCauGoiDichVuId = yeuCauVo.YeuCauGoiDichVuId
                };
                GetDichVuGiuongTrongGoiTheoBenhNhan(yeuCauTiepNhanChiTiet.BenhNhan, dichVuInfo, newYeuCauDichVuGiuong);

                newYeuCauDichVuGiuong.HoatDongGiuongBenhs.Add(new HoatDongGiuongBenh()
                {
                    GiuongBenhId = yeuCauVo.GiuongId,
                    YeuCauTiepNhanId = yeuCauTiepNhanChiTiet.Id,
                    ThoiDiemBatDau = yeuCauVo.ThoiGianNhan,
                    TinhTrangBenhNhan = Enums.TinhTrangBenhNhan.DangDieuTri
                });

                yeuCauTiepNhanChiTiet.YeuCauDichVuGiuongBenhViens.Add(newYeuCauDichVuGiuong);
            }
        }

        public async Task KiemTraPhongChiDinhTiepNhanNoiTru(GiuongBenhTrongVo giuongBenhTrong)
        {
            var giuongBenh = await _giuongBenhRepository.TableNoTracking.Include(x => x.HoatDongGiuongBenhs)
                                                                        .Where(x => x.Id == giuongBenhTrong.GiuongBenhId)
                                                                        .FirstOrDefaultAsync();

            if (giuongBenh == null)
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.GiuongBenh.NotExists"));
            }

            var maxBenhNhan = int.Parse(_cauHinhService.GetSetting("CauHinhNoiTru.SoLuongBenhNhanToiDaTrenGiuong").Value);

            // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
            var lstGiuongBenhIdDangSuDung = new List<long>();
            if (giuongBenhTrong.YeuCauTiepNhanNoiTruId != null
                && giuongBenhTrong.YeuCauTiepNhanNoiTruId != 0
                && (giuongBenhTrong.YeuCauDichVuGiuongBenhVienId == null || giuongBenhTrong.YeuCauDichVuGiuongBenhVienId == 0))
            {
                lstGiuongBenhIdDangSuDung = await GetListGiuongIdDangSuDungCuaNguoiBenhAsync(giuongBenhTrong.YeuCauTiepNhanNoiTruId.Value);
            }
            //-------------------------------------------------------------------------------------------

            //if (giuongBenh.HoatDongGiuongBenhs.Count(x => (giuongBenhTrong.YeuCauDichVuGiuongBenhVienId == null || x.YeuCauDichVuGiuongBenhVienId != giuongBenhTrong.YeuCauDichVuGiuongBenhVienId) &&
            //                                              (x.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianNhan && (x.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianNhan <= x.ThoiDiemKetThuc)) &&
            //                                              (giuongBenhTrong.ThoiGianTra == null || (x.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra && (x.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianTra <= x.ThoiDiemKetThuc)))) >= maxBenhNhan)
            //{
            if (giuongBenh.HoatDongGiuongBenhs.Count(x => (giuongBenhTrong.YeuCauDichVuGiuongBenhVienId == null || x.YeuCauDichVuGiuongBenhVienId != giuongBenhTrong.YeuCauDichVuGiuongBenhVienId) &&
                                                          (
                                                            (
                                                                (x.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianNhan && (x.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianNhan <= x.ThoiDiemKetThuc)) &&
                                                                (giuongBenhTrong.ThoiGianTra == null || (x.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra && (x.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianTra <= x.ThoiDiemKetThuc)))
                                                            ) ||
                                                            (giuongBenhTrong.ThoiGianNhan <= x.ThoiDiemBatDau && (giuongBenhTrong.ThoiGianTra == null || x.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra))
                                                          )
                                                    ) >= maxBenhNhan

                // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                && lstGiuongBenhIdDangSuDung.All(x => x != giuongBenh.Id))
                //--------------------------------------------------------------------------------
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.GiuongBenh.IsFull"));
            }

            if (giuongBenhTrong.BaoPhong == true)
            {
                var kiemTraKhongPhongTrong = await _phongBenhVienRepository.TableNoTracking
                    .AnyAsync(x => x.Id == giuongBenh.PhongBenhVienId
                                   && x.GiuongBenhs
                                       // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                                       .Where(a => lstGiuongBenhIdDangSuDung.All(b => b != a.Id))
                                       //-----------------------------------------------------------------------------------
                                       .Any(a => a.HoatDongGiuongBenhs.Any(b => (giuongBenhTrong.YeuCauDichVuGiuongBenhVienId == null || b.YeuCauDichVuGiuongBenhVienId != giuongBenhTrong.YeuCauDichVuGiuongBenhVienId) &&
                                                                                            (
                                                                                                ((b.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianNhan && (b.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianNhan <= b.ThoiDiemKetThuc)) &&
                                                                                                (giuongBenhTrong.ThoiGianTra == null || (b.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra && (b.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianTra <= b.ThoiDiemKetThuc)))) ||
                                                                                                (giuongBenhTrong.ThoiGianNhan <= b.ThoiDiemBatDau && (giuongBenhTrong.ThoiGianTra == null || b.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra)))
                                                                                            )));
                //if (giuongBenh.HoatDongGiuongBenhs.Any(x => (x.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianNhan
                //                                             && (x.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianNhan <= x.ThoiDiemKetThuc))
                //                                            && (giuongBenhTrong.ThoiGianTra == null || (x.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra && (x.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianTra <= x.ThoiDiemKetThuc))))
                //    && giuongBenhTrong.BaoPhong == true)
                if (kiemTraKhongPhongTrong)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.GiuongBenhBaoPhong.NotEnough"));
                }
            }
            else
            {
                var kiemTraPhongDaDuocBao = await _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.AnyAsync(p => p.BaoPhong == true &&
                                                                                                                      p.GiuongBenh.PhongBenhVienId == giuongBenh.PhongBenhVienId &&
                                                                                                                      p.Id != giuongBenhTrong.YeuCauDichVuGiuongBenhVienId &&
                                                                                                                      // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                                                                                                                      p.YeuCauTiepNhanId != giuongBenhTrong.YeuCauTiepNhanNoiTruId &&
                                                                                                                      //------------------------------------------------------------------------
                                                                                                                      p.HoatDongGiuongBenhs.Any
                                                                                                                      (p2 =>
                                                                                                                          ((p2.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianNhan && (p2.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianNhan <= p2.ThoiDiemKetThuc)) &&
                                                                                                                          (giuongBenhTrong.ThoiGianTra == null || (p2.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra && (p2.ThoiDiemKetThuc == null || giuongBenhTrong.ThoiGianTra <= p2.ThoiDiemKetThuc)))) ||
                                                                                                                          (giuongBenhTrong.ThoiGianNhan <= p2.ThoiDiemBatDau && (giuongBenhTrong.ThoiGianTra == null || p2.ThoiDiemBatDau <= giuongBenhTrong.ThoiGianTra))
                                                                                                                      ));

                if (kiemTraPhongDaDuocBao)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.GiuongBenh.IsBaoPhong"));
                }
            }
        }

        #endregion

        #region bệnh án sơ sinh

        public async Task<bool> KiemTraTaoBenhAnSoSinhAsync(long yeuCauTiepNhanId)
        {
            //var thongTinBenhAnSanKhoa = GetThongTinBenhAnSanKhoaMoThuong(yeuCauTiepNhanId);
            var yeuCauTiepNhan = await BaseRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .FirstAsync(x => x.Id == yeuCauTiepNhanId);
            if (yeuCauTiepNhan == null || yeuCauTiepNhan.NoiTruBenhAn == null ||
                string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinBenhAn))
            {
                return false;
            }

            if (yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new Exception(_localizationService.GetResource("BenhAnSoSinh.BenhAn.DaKetThuc"));
            }

            // todo: cần kiểm tra số lượng con và bệnh án sơ sinh đã tạo
            return true;
        }


        public async Task GetThongTinTiepNhanBenhAnMeAsync(YeuCauTiepNhan yeuCauTiepNhanCon)
        {
            var quanHeThanNhan = await _quanHeThanNhanRepository.TableNoTracking
                .Where(x => x.TenVietTat == "MeDe" || x.Ten.Trim().ToLower() == "Mẹ".ToLower()).FirstOrDefaultAsync();
            var ngheNghiep = await _ngheNghiepRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Ten.ToLower() == "Trẻ em < 6 tuổi".ToLower() || x.TenVietTat == "<6Tuoi");
            yeuCauTiepNhanCon.NguoiLienHeQuanHeNhanThanId = quanHeThanNhan?.Id;
            yeuCauTiepNhanCon.NgheNghiepId = ngheNghiep?.Id;
        }

        public async Task XuLyTaoBenhAnSoSinhAsync(YeuCauTiepNhan yeuCauTiepNhanCon, long yeuCauTiepNhanMeId, long khoaChuyenBenhAnSoSinhVeId, DateTime lucDeSoSinh, long? yeuCauGoiDichVuId = null)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;

            //var phongBenhVien = await _phongBenhVienRepository.TableNoTracking
            //    .FirstAsync(x => x.Id == phongId);

            yeuCauTiepNhanCon.MaYeuCauTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan();
            yeuCauTiepNhanCon.NhanVienTiepNhanId = currentUserId;
            //yeuCauTiepNhanCon.ThoiDiemTiepNhan = thoiDiemHienTai;
            yeuCauTiepNhanCon.LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru;
            yeuCauTiepNhanCon.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien;
            yeuCauTiepNhanCon.ThoiDiemCapNhatTrangThai = thoiDiemHienTai;

            #region tạo người bệnh
            var benNhan = new BenhNhan()
            {
                HoTen = string.IsNullOrEmpty(yeuCauTiepNhanCon.TenKhaiSinh) ? yeuCauTiepNhanCon.TenBanDau : yeuCauTiepNhanCon.TenKhaiSinh,
                NgaySinh = yeuCauTiepNhanCon.NgaySinh,
                ThangSinh = yeuCauTiepNhanCon.ThangSinh,
                NamSinh = yeuCauTiepNhanCon.NamSinh,

                PhuongXaId = yeuCauTiepNhanCon.PhuongXaId,
                QuanHuyenId = yeuCauTiepNhanCon.QuanHuyenId,
                TinhThanhId = yeuCauTiepNhanCon.TinhThanhId,
                QuocTichId = yeuCauTiepNhanCon.QuocTichId,
                DanTocId = yeuCauTiepNhanCon.DanTocId,
                DiaChi = yeuCauTiepNhanCon.DiaChi,
                SoDienThoai = yeuCauTiepNhanCon.SoDienThoai,
                SoChungMinhThu = yeuCauTiepNhanCon.SoChungMinhThu,
                Email = yeuCauTiepNhanCon.Email,
                NgheNghiepId = yeuCauTiepNhanCon.NgheNghiepId,
                GioiTinh = yeuCauTiepNhanCon.GioiTinh,
                NoiLamViec = yeuCauTiepNhanCon.NoiLamViec
            };

            if (yeuCauGoiDichVuId != null)
            {
                var yeuCauGoiDicVuMeDangKy =
                    await _yeuCauGoiDichVuRepository.Table
                        .Where(x => x.Id == yeuCauGoiDichVuId
                                    && x.GoiSoSinh == true).FirstAsync();
                benNhan.YeuCauGoiDichVuSoSinhs.Add(yeuCauGoiDicVuMeDangKy);
            }

            yeuCauTiepNhanCon.BenhNhan = benNhan;
            #endregion

            #region tạo yêu cầu nhập viện
            var yeuCauNhapVien = new YeuCauNhapVien()
            {
                BenhNhan = benNhan,
                BacSiChiDinhId = currentUserId,
                NoiChiDinhId = phongId,
                ThoiDiemChiDinh = lucDeSoSinh,
                KhoaPhongNhapVienId = khoaChuyenBenhAnSoSinhVeId,
                LaCapCuu = false,
                YeuCauTiepNhanMeId = yeuCauTiepNhanMeId
            };
            #endregion

            #region tạo bệnh án

            var benhAn = new Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn()
            {
                BenhNhan = benNhan,
                SoBenhAn = Helpers.ResourceHelper.CreateSoBenhAn(),
                SoLuuTru = Helpers.ResourceHelper.CreateSoLuuTru(),
                KhoaPhongNhapVienId = khoaChuyenBenhAnSoSinhVeId,
                ThoiDiemNhapVien = lucDeSoSinh,
                LaCapCuu = false,
                LoaiBenhAn = Enums.LoaiBenhAn.TreSoSinh,
                ThoiDiemTaoBenhAn = lucDeSoSinh,
                NhanVienTaoBenhAnId = currentUserId,
            };

            var newKhoaPhongDieuTri = new NoiTruKhoaPhongDieuTri()
            {
                KhoaPhongChuyenDenId = khoaChuyenBenhAnSoSinhVeId,
                ThoiDiemVaoKhoa = lucDeSoSinh,
                NhanVienChiDinhId = currentUserId
            };
            benhAn.NoiTruKhoaPhongDieuTris.Add(newKhoaPhongDieuTri);

            #endregion



            yeuCauTiepNhanCon.NoiTruBenhAn = benhAn;
            yeuCauTiepNhanCon.YeuCauNhapVien = yeuCauNhapVien;
            //yeuCauNhapVien.YeuCauTiepNhans.Add(yeuCauTiepNhanCon);

            await BaseRepository.AddAsync(yeuCauTiepNhanCon);
        }
        public async Task<bool> KiemTraNgaySinhConVaThoiGianNhapVienMe(long yeuCauTiepNhanMeId, DateTime ngaySinhCon)
        {
            var ktTraTGNhapVienMe = await _yeuCauNhapVienRepository.TableNoTracking.AnyAsync(x => x.YeuCauTiepNhanMeId == yeuCauTiepNhanMeId
                                                                    && x.YeuCauTiepNhanMe.NoiTruBenhAn.ThoiDiemNhapVien > ngaySinhCon);
            return ktTraTGNhapVienMe;
        }

        public async Task<bool> KiemTraBenhAnMeCoConTrungTen(long? yeuCauTiepNhanMeId, string hoTen)
        {
            if (yeuCauTiepNhanMeId == null || string.IsNullOrEmpty(hoTen))
            {
                return false;
            }

            var kiemTraTreSoSinhTrungTen = await _yeuCauNhapVienRepository.TableNoTracking.AnyAsync(x => x.YeuCauTiepNhanMeId == yeuCauTiepNhanMeId
                                                                    && x.YeuCauTiepNhans.Any(c => c.HoTen.ToUpper().TrimStart().TrimEnd() == hoTen.ToUpper().TrimStart().TrimEnd()));
            return kiemTraTreSoSinhTrungTen;
        }

        public async Task<bool> KiemTraYeuCauGoiDichVuDaSuDungAsync(long? yeuCauGoiDichVuId, long? benhNhanId = null, bool isCheckBenhNhanHienTai = false, long? yeuCauTiepNhanMeId = null)
        {
            if (yeuCauGoiDichVuId == null)
            {
                return false;
            }
            var daChiDinh = _yeuCauGoiDichVuRepository.TableNoTracking.Any(x => x.Id == yeuCauGoiDichVuId
                                                                                           && x.BenhNhanSoSinhId != null
                                                                                           && x.BenhNhanSoSinhId != benhNhanId);

            if (isCheckBenhNhanHienTai)
            {
                benhNhanId = null;
            }

            var daSuDung = false;
            if (yeuCauTiepNhanMeId != null)
            {
                //daSuDung = _yeuCauTiepNhanRepository.TableNoTracking
                //    .Any(x => x.BenhNhanId != benhNhanId
                //              && x.YeuCauNhapVienId != null
                //              && x.YeuCauNhapVien.YeuCauTiepNhanMeId == yeuCauTiepNhanMeId
                //              && (x.YeuCauKhamBenhs.Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Any(y => y.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                //                    || x.YeuCauDichVuKyThuats.Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any(y => y.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                //                    || x.YeuCauDichVuGiuongBenhViens.Where(y => y.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy).Any(y => y.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                //                    || x.MienGiamChiPhis.Any(y => y.YeuCauGoiDichVuId == yeuCauGoiDichVuId && y.DaHuy != true)));

                var dsYeuCauTiepNhanConId = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.BenhNhanId != benhNhanId
                                && x.YeuCauNhapVienId != null
                                && x.YeuCauNhapVien.YeuCauTiepNhanMeId == yeuCauTiepNhanMeId)
                    .Select(x => x.Id).ToList();
                if (dsYeuCauTiepNhanConId.Any())
                {
                    daSuDung = _yeuCauKhamBenhRepository.TableNoTracking
                        .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                  && dsYeuCauTiepNhanConId.Contains(y.YeuCauTiepNhanId)
                                  && y.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                    if (!daSuDung)
                    {
                        daSuDung = _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                      && dsYeuCauTiepNhanConId.Contains(y.YeuCauTiepNhanId)
                                      && y.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                    }

                    if (!daSuDung)
                    {
                        daSuDung = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
                            .Any(y => y.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy
                                      && dsYeuCauTiepNhanConId.Contains(y.YeuCauTiepNhanId)
                                      && y.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                    }

                    if (!daSuDung)
                    {
                        daSuDung = _mienGiamChiPhiRepository.TableNoTracking
                            .Any(x => x.DaHuy != true
                                      && dsYeuCauTiepNhanConId.Contains(x.YeuCauTiepNhanId)
                                      && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)
                                      && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                    }
                }
            }
            else
            {
                //daSuDung = _yeuCauKhamBenhRepository.TableNoTracking
                //                   .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && y.YeuCauTiepNhan.BenhNhanId != benhNhanId)
                //                   .Any(y => y.YeuCauGoiDichVuId == yeuCauGoiDichVuId || y.MienGiamChiPhis.Any(z => z.YeuCauGoiDichVuId == yeuCauGoiDichVuId && z.DaHuy != true))
                //               || _yeuCauDichVuKyThuatRepository.TableNoTracking
                //                   .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && y.YeuCauTiepNhan.BenhNhanId != benhNhanId)
                //                   .Any(y => y.YeuCauGoiDichVuId == yeuCauGoiDichVuId || y.MienGiamChiPhis.Any(z => z.YeuCauGoiDichVuId == yeuCauGoiDichVuId && z.DaHuy != true))
                //               || _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
                //                   .Where(y => y.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && y.YeuCauTiepNhan.BenhNhanId != benhNhanId)
                //                   .Any(y => y.YeuCauGoiDichVuId == yeuCauGoiDichVuId || y.MienGiamChiPhis.Any(z => z.YeuCauGoiDichVuId == yeuCauGoiDichVuId && z.DaHuy != true));

                daSuDung = _yeuCauKhamBenhRepository.TableNoTracking
                               .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham 
                                           && y.YeuCauTiepNhan.BenhNhanId != benhNhanId
                                           && y.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                if (!daSuDung)
                {
                    daSuDung = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                    && y.YeuCauTiepNhan.BenhNhanId != benhNhanId 
                                    && y.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                }

                if (!daSuDung)
                {
                    daSuDung = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
                        .Any(y => y.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy 
                                    && y.YeuCauTiepNhan.BenhNhanId != benhNhanId 
                                    && y.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                }

                if (!daSuDung)
                {
                    daSuDung = _mienGiamChiPhiRepository.TableNoTracking
                        .Any(x => x.YeuCauTiepNhan.BenhNhanId != benhNhanId
                                    && x.DaHuy != true
                                    && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)
                                    && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId);
                }
            }

            return daSuDung || daChiDinh;
        }

        public async Task<bool> KiemTraYeuCauGoiDichVuDaChiDinhChoConKhacAsync(long? yeuCauGoiDichVuId)
        {
            if (yeuCauGoiDichVuId == null)
            {
                return false;
            }

            var daChiDinh = await _yeuCauGoiDichVuRepository.TableNoTracking.AnyAsync(x => x.Id == yeuCauGoiDichVuId && x.BenhNhanSoSinhId != null);
            return daChiDinh;
        }

        public async Task<List<LookupItemVo>> GetYeuCauGoiDichVuSoSinhCuaMeAsync(DropDownListRequestModel model)
        {
            var yeuCauTiepNhanId = CommonHelper.GetIdFromRequestDropDownList(model);
            var yeuCauGoiDichVus = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(x => x.Id == model.Id
                            ||
                             (x.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy
                              && x.BenhNhanSoSinhId == null
                              && x.GoiSoSinh == true
                              && x.DaQuyetToan != true
                              && x.NgungSuDung != true
                            && x.BenhNhan.YeuCauTiepNhans.Any(y => y.Id == yeuCauTiepNhanId)
                            && !x.BenhNhan.YeuCauTiepNhans.Any(y => y.YeuCauKhamBenhs.Where(z => z.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Any(t => t.YeuCauGoiDichVuId == x.Id || y.MienGiamChiPhis.Any(z => z.YeuCauGoiDichVuId == x.Id && z.DaHuy != true))
                                                              || y.YeuCauDichVuKyThuats.Where(z => z.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any(t => t.YeuCauGoiDichVuId == x.Id || y.MienGiamChiPhis.Any(z => z.YeuCauGoiDichVuId == x.Id && z.DaHuy != true))
                                                              || y.YeuCauDichVuGiuongBenhViens.Where(z => z.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy).Any(t => t.YeuCauGoiDichVuId == x.Id || y.MienGiamChiPhis.Any(z => z.YeuCauGoiDichVuId == x.Id && z.DaHuy != true)))))
                .Select(item => new LookupItemVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.TenGoiDichVu
                })
                .ApplyLike(model.Query, x => x.DisplayName)
                .Take(model.Take).ToListAsync();
            return yeuCauGoiDichVus;
        }
        #endregion

        #region Cập nhật chỉ định dịch vụ giường trong gói

        public void GetDichVuGiuongTrongGoiTheoBenhNhan(BenhNhan benhNhan, DichVuGiuongTrongGoiVo dichVuInfo, YeuCauDichVuGiuongBenhVien yeuCauDichVuGiuong)
        {
            var yeuCauGoiDichVu = benhNhan.YeuCauGoiDichVus
                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy
                            && (dichVuInfo.YeuCauGoiDichVuId == null || x.Id == dichVuInfo.YeuCauGoiDichVuId)
                            && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(y => y.DichVuGiuongBenhVienId == dichVuInfo.DichVuBenhVienId && y.NhomGiaDichVuGiuongBenhVienId == dichVuInfo.NhomGiaDichVuId))
                .Union(benhNhan.YeuCauGoiDichVuSoSinhs
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy
                                && (dichVuInfo.YeuCauGoiDichVuId == null || x.Id == dichVuInfo.YeuCauGoiDichVuId)
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(y => y.DichVuGiuongBenhVienId == dichVuInfo.DichVuBenhVienId && y.NhomGiaDichVuGiuongBenhVienId == dichVuInfo.NhomGiaDichVuId)))
                .OrderBy(x => x.CreatedOn)
                .FirstOrDefault();
            if (yeuCauGoiDichVu != null)
            {
                var chuongTrinhDvGiuong = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                    .First(y => y.DichVuGiuongBenhVienId == dichVuInfo.DichVuBenhVienId && y.NhomGiaDichVuGiuongBenhVienId == dichVuInfo.NhomGiaDichVuId);

                yeuCauDichVuGiuong.Gia = chuongTrinhDvGiuong.DonGia;
                yeuCauDichVuGiuong.DonGiaTruocChietKhau = chuongTrinhDvGiuong.DonGiaTruocChietKhau;
                yeuCauDichVuGiuong.DonGiaSauChietKhau = chuongTrinhDvGiuong.DonGiaSauChietKhau;
                yeuCauDichVuGiuong.YeuCauGoiDichVuId = yeuCauGoiDichVu.Id;
            }
            else
            {
                yeuCauDichVuGiuong.YeuCauGoiDichVuId = null;
                yeuCauDichVuGiuong.DonGiaTruocChietKhau = null;
                yeuCauDichVuGiuong.DonGiaSauChietKhau = null;
            }
        }


        #endregion

        #region kiểm tra sử dụng dịch vụ giường trong gói

        public async Task<List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>> GetThongTinSuDungDichVuGiuongTrongGoiAsync(long benhNhanId)
        {
            var yeuCauTiepNhans = BaseRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.KhoaPhong)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.PhongBenhVien)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.GiuongBenh)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.KhoaPhong)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.PhongBenhVien)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.GiuongBenh)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.NoiChiDinh).ThenInclude(gb => gb.KhoaPhong)
                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong)
                .Where(x => x.BenhNhanId == benhNhanId
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .ToList();
            var lstSuDung = new List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>();
            foreach (var yeuCauTiepNhan in yeuCauTiepNhans)
            {
                if (yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)
                {
                    var chiPhiGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
                    lstSuDung.AddRange(chiPhiGiuong.Item1
                        .Where(x => x.YeuCauGoiDichVuId != null)
                        .Select(s => new ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo
                        {
                            YeuCauGoiDichVuId = s.YeuCauGoiDichVuId.Value,
                            DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                            NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                            SoLuongDaSuDung = s.SoLuong
                        }).ToList());
                }
                else
                {
                    lstSuDung.AddRange(yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                        .Where(x => x.YeuCauGoiDichVuId != null)
                        .Select(s => new ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo
                        {
                            YeuCauGoiDichVuId = s.YeuCauGoiDichVuId.Value,
                            DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                            NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                            SoLuongDaSuDung = s.SoLuong
                        }).ToList());
                }
            }

            return lstSuDung;
        }


        #endregion

        private async Task<List<long>> GetListGiuongIdDangSuDungCuaNguoiBenhAsync(long yeuCauTiepNhanNoiTruId)
        {
            var lstGiuongBenhIdDangSuDung = await _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanNoiTruId
                            && x.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy
                            && x.ThoiDiemKetThucSuDung == null
                            && x.GiuongBenhId != null)
                .Select(x => x.GiuongBenhId.Value)
                .Distinct()
                .ToListAsync();
            return lstGiuongBenhIdDangSuDung;
        }
    }
}
