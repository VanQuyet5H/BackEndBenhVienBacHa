using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Camino.Core.Domain;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<GridDataSource> GetDanhSachDieuTriNoiTruForGrid(QueryInfo queryInfo, bool isAllData = false)
        {

            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new NoiTruBenhAnModelSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NoiTruBenhAnModelSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangDieuTri = DanhSachNoiTruBenhAn(DangDieuTri, queryInfo, queryObject);
            var queryTuChuyenVien = DanhSachNoiTruBenhAn(ChuyenVien, queryInfo, queryObject);
            var queryChuyenKhoa = DanhSachNoiTruBenhAnChuyenKhoa(ChuyenKhoa, queryInfo, queryObject);
            var queryTuDaRaVien = DanhSachNoiTruBenhAn(DaRaVien, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                                                     .Select(s => new DanhSachNoiTruBenhAnGridVo()).AsQueryable();

            if (queryObject.DangDieuTri == false && queryObject.ChuyenVien == false && queryObject.ChuyenKhoa == false && queryObject.DaRaVien == false)
            {
                queryObject.DangDieuTri = true;
                queryObject.ChuyenVien = true;
                queryObject.ChuyenKhoa = true;
                queryObject.DaRaVien = true;
            }

            var isHaveQuery = false;
            if (queryObject.DangDieuTri)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangDieuTri);
                }
                else
                {
                    query = queryDangDieuTri;
                    isHaveQuery = true;
                }
            }

            if (queryObject.ChuyenVien)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChuyenVien);
                }
                else
                {
                    query = queryTuChuyenVien;
                    isHaveQuery = true;
                }
            }

            if (queryObject.ChuyenKhoa)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryChuyenKhoa);
                }
                else
                {
                    query = queryChuyenKhoa;
                    isHaveQuery = true;
                }
            }

            if (queryObject.DaRaVien)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaRaVien);
                }
                else
                {
                    query = queryTuDaRaVien;
                    isHaveQuery = true;
                }
            }

          

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("ThoiGianTiepNhan desc,Id asc"))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            
            foreach (var item in queryTask.Result)
            {
                //var chiPhiKhamChuaBenh = GetDanhSachChiPhiNoiTruChuaThu(item.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                //var soTienDaTamUng = _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(item.Id).Result;
                //item.SoTienConLai = soTienDaTamUng - chiPhiKhamChuaBenh;

                item.NamSinhDisplay = DateHelper.DOBFormat(item.NgaySinh, item.ThangSinh, item.NamSinh);

            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPagesDanhSachDieuTriNoiTruForGrid(QueryInfo queryInfo)
        {

            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new NoiTruBenhAnModelSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NoiTruBenhAnModelSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangDieuTri = DanhSachNoiTruBenhAn(DangDieuTri, queryInfo, queryObject);
            var queryTuChuyenVien = DanhSachNoiTruBenhAn(ChuyenVien, queryInfo, queryObject);
            var queryChuyenKhoa = DanhSachNoiTruBenhAnChuyenKhoa(ChuyenKhoa, queryInfo, queryObject);
            var queryTuDaRaVien = DanhSachNoiTruBenhAn(DaRaVien, queryInfo, queryObject);


            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                                                     .Select(s => new DanhSachNoiTruBenhAnGridVo()).AsQueryable();

            if (queryObject.DangDieuTri == false && queryObject.ChuyenVien == false && queryObject.DaRaVien == false)
            {
                queryObject.DangDieuTri = true;
                queryObject.ChuyenVien = true;
                queryObject.DaRaVien = true;
            }

            var isHaveQuery = false;
            if (queryObject.DangDieuTri)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangDieuTri);
                }
                else
                {
                    query = queryDangDieuTri;
                    isHaveQuery = true;
                }
            }
            if (queryObject.ChuyenVien)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChuyenVien);
                }
                else
                {
                    query = queryTuChuyenVien;
                    isHaveQuery = true;
                }
            }

            if (queryObject.ChuyenKhoa)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryChuyenKhoa);
                }
                else
                {
                    query = queryChuyenKhoa;
                    isHaveQuery = true;
                }
            }

            if (queryObject.DaRaVien)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaRaVien);
                }
                else
                {
                    query = queryTuDaRaVien;
                    isHaveQuery = true;
                }
            }
           
            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("ThoiGianTiepNhan desc,Id asc"))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        private IQueryable<DanhSachNoiTruBenhAnGridVo> DanhSachNoiTruBenhAn(string trangThai, QueryInfo queryInfo, NoiTruBenhAnModelSearch noiTruBenhAnModelSearch)
        {

            var currentPhongNhanVien = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentKhoaNhanVien = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == currentPhongNhanVien)
                                                                              .Select(p => p.KhoaPhongId)
                                                                              .FirstOrDefault();
            var yeuCauTiepNhans = BaseRepository.TableNoTracking.Include(cc => cc.NoiTruBenhAn)
                 .Include(cc => cc.YeuCauDichVuGiuongBenhViens).ThenInclude(cc => cc.GiuongBenh).ThenInclude(cc => cc.PhongBenhVien).Where(cc => cc.NoiTruBenhAn != null);

            var noiTruBenhAnIds = yeuCauTiepNhans.Select(cc => cc.NoiTruBenhAn).Include(cc => cc.BenhNhan)
                                               .SelectMany(cc => cc.NoiTruKhoaPhongDieuTris.Where(p => p.ThoiDiemVaoKhoa <= DateTime.Now && (p.ThoiDiemRaKhoa == null || p.ThoiDiemRaKhoa.Value >= DateTime.Now) && p.KhoaPhongChuyenDenId == currentKhoaNhanVien)
                                               .Select(c => c.NoiTruBenhAnId));

            var noiTruBenhAns = yeuCauTiepNhans.Select(cc => cc.NoiTruBenhAn).Include(cc => cc.BenhNhan).Where(cc => cc.NoiTruKhoaPhongDieuTris.Any() ? noiTruBenhAnIds.Contains(cc.Id) : cc.KhoaPhongNhapVienId == currentKhoaNhanVien)
                                                .Include(cc => cc.KhoaPhongNhapVien).Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauNhapVien).ThenInclude(cc => cc.NoiChiDinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuGiuongBenhViens).ThenInclude(cc => cc.GiuongBenh).ThenInclude(cc => cc.PhongBenhVien);
            var today = DateTime.Now;
            var query = noiTruBenhAns.Select(s => new DanhSachNoiTruBenhAnGridVo
            {
                Id = s.Id,
                MaTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = s.SoBenhAn,
                MaBenhNhan = s.BenhNhan.MaBN,
                HoTen = s.YeuCauTiepNhan.HoTen,
                GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                KhoaPhongId = s.KhoaPhongNhapVien.Id,
                NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                ThangSinh = s.YeuCauTiepNhan.ThangSinh,
                NamSinh = s.YeuCauTiepNhan.NamSinh,
                ThoiGianNhapVien = s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                NoiChiDinh = s.YeuCauTiepNhan.YeuCauNhapVien.NoiChiDinh.Ten,
                ChanDoan = s.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienGhiChu,               
                ThoiGianRaVien = s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien,
                CoBHYT = s.YeuCauTiepNhan.CoBHYT,

                //BVHD-3754
                //MucHuong = s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //        ? s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //            .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                //            .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                MucHuong = s.YeuCauTiepNhan.CoBHYT == true ? s.YeuCauTiepNhan.BHYTMucHuong : (int?)null,

                Phong =
                 s.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any() ? s.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
                                                                                                            p.ThoiDiemBatDauSuDung <= today &&
                                                                                                            (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))
                                                                                                            .Select(a => a.GiuongBenh.PhongBenhVien.Ten)
                                                                                                            .FirstOrDefault() : string.Empty,
                CapCuu = s.LaCapCuu,
                KetThucBenhAn = s.ThoiDiemRaVien != null,
                DaQuyetToan = s.DaQuyetToan,
                TrangThai = s.YeuCauTiepNhan.NoiTruBenhAn.DaQuyetToan != true ? 1 : (s.TinhTrangRaVien == Core.Domain.Enums.EnumTinhTrangRaVien.ChuyenVien ? 2 : 3),
                TenTrangThai = s.YeuCauTiepNhan.NoiTruBenhAn.DaQuyetToan != true ? DangDieuTri : (s.TinhTrangRaVien == Core.Domain.Enums.EnumTinhTrangRaVien.ChuyenVien ? ChuyenVien : DaRaVien),
            });

            if (!string.IsNullOrEmpty(noiTruBenhAnModelSearch.SearchString))
            {
                query = query.ApplyLike(noiTruBenhAnModelSearch.SearchString.Trim(), g => g.MaBenhNhan,
                                                                                     g => g.HoTen,
                                                                                     g => g.SoBenhAn,
                                                                                     g => g.MaTiepNhan)
                                                                                     .OrderBy(queryInfo.SortString);

            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("ThoiGianTiepNhan desc,Id asc"))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            if (noiTruBenhAnModelSearch.KhoaPhongId != null)
            {
                query = query.Where(cc => cc.KhoaPhongId == noiTruBenhAnModelSearch.KhoaPhongId);
            }

            if (noiTruBenhAnModelSearch.TuNgayDenNgay != null)
            {
                if (noiTruBenhAnModelSearch.TuNgayDenNgay.startDate != null)
                {
                    query = query.Where(p => p.ThoiGianNhapVien.Value.Date >= noiTruBenhAnModelSearch.TuNgayDenNgay.startDate.Value.Date);
                }

                if (noiTruBenhAnModelSearch.TuNgayDenNgay.endDate != null)
                {
                    query = query.Where(p => p.ThoiGianNhapVien.Value.Date <= noiTruBenhAnModelSearch.TuNgayDenNgay.endDate.Value.Date);
                }
            }

            if (noiTruBenhAnModelSearch.TuNgayDenNgayRaVien != null)
            {
                if (noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.startDate != null)
                {
                    query = query.Where(p => p.ThoiGianRaVien != null && p.ThoiGianRaVien.Value.Date >= noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.startDate.Value.Date);
                }

                if (noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.endDate != null)
                {
                    query = query.Where(p => p.ThoiGianRaVien != null && p.ThoiGianRaVien.Value.Date <= noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.endDate.Value.Date);
                }
            }

            return query.Where(cc => cc.TenTrangThai.Contains(trangThai));
        }

        private IQueryable<DanhSachNoiTruBenhAnGridVo> DanhSachNoiTruBenhAnChuyenKhoa(string trangThai, QueryInfo queryInfo, NoiTruBenhAnModelSearch noiTruBenhAnModelSearch)
        {
            var currentPhongNhanVien = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentKhoaNhanVien = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == currentPhongNhanVien).Select(p => p.KhoaPhongId).FirstOrDefault();

            var noiTruBenhAnBenhNhanChuaRaKhoas = BaseRepository.TableNoTracking.SelectMany(c => c.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.
                                       Where(co => co.ThoiDiemRaKhoa == null && co.KhoaPhongChuyenDenId == currentKhoaNhanVien)).Select(o => o.NoiTruBenhAnId);

            var yeuCauTiepNhans = BaseRepository.TableNoTracking.Where(cc => cc.NoiTruBenhAn != null
                                                && cc.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any((p => !noiTruBenhAnBenhNhanChuaRaKhoas.Contains(p.NoiTruBenhAnId)
                                                && p.KhoaPhongChuyenDiId == currentKhoaNhanVien)));

            var noiTruBenhAns = yeuCauTiepNhans.Select(cc => cc.NoiTruBenhAn)
                                                .Include(cc => cc.BenhNhan).Include(cc => cc.KhoaPhongNhapVien)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauNhapVien).ThenInclude(cc => cc.NoiChiDinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuGiuongBenhViens).ThenInclude(cc => cc.GiuongBenh).ThenInclude(cc => cc.PhongBenhVien);
            var today = DateTime.Now;
            var query = noiTruBenhAns.Select(s => new DanhSachNoiTruBenhAnGridVo
            {
                Id = s.Id,
                MaTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = s.SoBenhAn,
                MaBenhNhan = s.BenhNhan.MaBN,
                HoTen = s.YeuCauTiepNhan.HoTen,
                GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                KhoaPhongId = s.KhoaPhongNhapVien.Id,
                ThoiGianNhapVien = s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                ThoiGianRaVien = s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien,
                NoiChiDinh = s.YeuCauTiepNhan.YeuCauNhapVien.NoiChiDinh.Ten,
                ChanDoan = s.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienGhiChu,

                NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                ThangSinh = s.YeuCauTiepNhan.ThangSinh,
                NamSinh = s.YeuCauTiepNhan.NamSinh,

                CoBHYT = s.YeuCauTiepNhan.CoBHYT,

                //BVHD-3754
                //MucHuong = s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //        ? s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //            .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                //            .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                MucHuong = s.YeuCauTiepNhan.CoBHYT == true ? s.YeuCauTiepNhan.BHYTMucHuong : (int?)null,

                Phong =
                 s.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any() ? s.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
                                                                                                            p.ThoiDiemBatDauSuDung <= today &&
                                                                                                            (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))
                                                                                                            .Select(a => a.GiuongBenh.PhongBenhVien.Ten)
                                                                                                            .FirstOrDefault() : string.Empty,

                CapCuu = s.LaCapCuu,
                KetThucBenhAn = s.ThoiDiemRaVien != null,
                DaQuyetToan = s.DaQuyetToan,

                TrangThai = 4,
                TenTrangThai = ChuyenKhoa,
            });

            if (!string.IsNullOrEmpty(noiTruBenhAnModelSearch.SearchString))
            {
                query = query.ApplyLike(noiTruBenhAnModelSearch.SearchString.Trim(), g => g.MaBenhNhan,
                                                                                     g => g.HoTen,
                                                                                     g => g.SoBenhAn,
                                                                                     g => g.MaTiepNhan)
                                                                                     .OrderBy(queryInfo.SortString);

            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("ThoiGianTiepNhan desc,Id asc"))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            if (noiTruBenhAnModelSearch.TuNgayDenNgay != null)
            {
                if (noiTruBenhAnModelSearch.TuNgayDenNgay.startDate != null)
                {
                    query = query.Where(p => p.ThoiGianNhapVien.Value.Date >= noiTruBenhAnModelSearch.TuNgayDenNgay.startDate.Value.Date);
                }

                if (noiTruBenhAnModelSearch.TuNgayDenNgay.endDate != null)
                {
                    query = query.Where(p => p.ThoiGianNhapVien.Value.Date <= noiTruBenhAnModelSearch.TuNgayDenNgay.endDate.Value.Date);
                }
            }

            if (noiTruBenhAnModelSearch.TuNgayDenNgayRaVien != null)
            {
                if (noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.startDate != null)
                {
                    query = query.Where(p => p.ThoiGianRaVien != null && p.ThoiGianRaVien.Value.Date >= noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.startDate.Value.Date);
                }

                if (noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.endDate != null)
                {
                    query = query.Where(p => p.ThoiGianRaVien != null && p.ThoiGianRaVien.Value.Date <= noiTruBenhAnModelSearch.TuNgayDenNgayRaVien.endDate.Value.Date);
                }
            }

            return query.Where(cc => cc.TenTrangThai.Contains(trangThai));
        }


        private readonly string DangDieuTri = "Đang điều trị";
        private readonly string ChuyenVien = "Chuyển viện";
        private readonly string ChuyenKhoa = "Chuyển khoa";
        private readonly string DaRaVien = "Đã ra viện";

    }
}
