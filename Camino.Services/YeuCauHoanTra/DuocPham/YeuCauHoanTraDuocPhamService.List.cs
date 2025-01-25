using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.DuocPham;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.YeuCauHoanTra.DuocPham
{
    public partial class YeuCauHoanTraDuocPhamService
    {
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new NhapKhoDuocPhamSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoDuocPhamSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = GetDataYeuCauTraDuocPham(null, queryInfo, queryObject);
            var queryTuChoiDuyet = GetDataYeuCauTraDuocPham(false, queryInfo, queryObject);
            var queryTuDaDuyet = GetDataYeuCauTraDuocPham(true, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DanhSachYeuCauHoanTraDuocPhamGridVo())
                .AsQueryable();
            var isHaveQuery = false;

            if (queryObject.DangChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                }
            }

            if (queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false && queryObject.DaDuyet == false)
            {
                query = queryDangChoDuyet;
                query = query.Concat(queryTuChoiDuyet);
                query = query.Concat(queryTuDaDuyet);
            }

            if (queryInfo.SortString != null
                && !queryInfo.SortString.Equals("NgayYeuCau desc,Id asc")
                && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        private IQueryable<DanhSachYeuCauHoanTraDuocPhamGridVo> GetDataYeuCauTraDuocPham(bool? duocDuyet, QueryInfo queryInfo, NhapKhoDuocPhamSearch queryObject)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var result = BaseRepository.TableNoTracking
                .Where(p => p.DuocDuyet == duocDuyet && p.KhoXuat != null && phongBenhVien != null && (p.KhoXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId))
            .Select(o => new DanhSachYeuCauHoanTraDuocPhamGridVo
            {
                Id = o.Id,
                NgayDuyet = o.NgayDuyet,
                NgayYeuCau = o.NgayYeuCau,
                KhoHoanTraTu = o.KhoXuat.Ten,
                KhoHoanTraVe = o.KhoNhap.Ten,
                Ma = o.SoPhieu,
                NguoiDuyet = o.NhanVienDuyet.User.HoTen,
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                TinhTrang = o.DuocDuyet
            }).ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
                    q => q.KhoHoanTraVe,
                    q => q.KhoHoanTraTu,
                    q => q.NguoiDuyet,
                    q => q.NguoiYeuCau,
                    q => q.Ma).OrderBy(queryInfo.SortString);

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau desc, Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                    result = result.Where(p => tuNgay <= p.NgayYeuCau.Value.Date) as IOrderedQueryable<DanhSachYeuCauHoanTraDuocPhamGridVo>;
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    result = (result ?? throw new InvalidOperationException()).Where(p => denNgay >= p.NgayYeuCau.Value.Date)
                        as IOrderedQueryable<DanhSachYeuCauHoanTraDuocPhamGridVo>;
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    result = (result ?? throw new InvalidOperationException()).Where(p => tuNgay <= p.NgayDuyet.Value.Date)
                        as IOrderedQueryable<DanhSachYeuCauHoanTraDuocPhamGridVo>;
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    result = (result ?? throw new InvalidOperationException()).Where(p => denNgay >= p.NgayDuyet.Value.Date)
                        as IOrderedQueryable<DanhSachYeuCauHoanTraDuocPhamGridVo>;
                }
            }

            return result;
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new NhapKhoDuocPhamSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoDuocPhamSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = GetDataYeuCauTraDuocPham(null, queryInfo, queryObject);
            var queryTuChoiDuyet = GetDataYeuCauTraDuocPham(false, queryInfo, queryObject);
            var queryTuDaDuyet = GetDataYeuCauTraDuocPham(true, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DanhSachYeuCauHoanTraDuocPhamGridVo())
                .AsQueryable();
            var isHaveQuery = false;

            if (queryObject.DangChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                }
            }

            if (queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false && queryObject.DaDuyet == false)
            {
                query = queryDangChoDuyet;
                query = query.Concat(queryTuChoiDuyet);
                query = query.Concat(queryTuDaDuyet);
            }

            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? ycTraThuocId, bool forExportExcel)
        {
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par;
            if (ycTraThuocId != null && ycTraThuocId != 0)
            {
                par = (long)ycTraThuocId;
            }
            else
            {
                par = long.Parse(queryInfo.AdditionalSearchString);
            }

            var query = _ycTraDpChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamId == par)
                .Select(s => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
                {
                    Id = s.Id,
                    DuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                    Ma = s.DuocPhamBenhVien.Ma,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    HopDong = s.HopDongThauDuocPham.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    LaDuocPhamBhyt = s.LaDuocPhamBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                });
            var groupQuery = query.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBhyt, x.Ma, x.DuocPham, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
                {
                    DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                    DuocPham = g.First().DuocPham,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaDuocPhamBhyt = g.First().LaDuocPhamBhyt,
                    Nhom = g.First().Nhom,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var result = await groupQuery.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            return new GridDataSource { Data = result, TotalRowCount = result.Count() };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _ycTraDpChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(s => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
                {
                    Id = s.Id,
                    DuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                    HopDong = s.HopDongThauDuocPham.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    Nhom = s.DuocPhamBenhVienPhanNhom != null ? s.DuocPhamBenhVienPhanNhom.Ten : "",
                    LaDuocPhamBhyt = s.LaDuocPhamBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                });
            var groupQuery = query.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBhyt, x.Ma, x.DuocPham, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
                {
                    DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                    DuocPham = g.First().DuocPham,
                    DVT = g.First().DVT,
                    LaDuocPhamBhyt = g.First().LaDuocPhamBhyt,
                    Nhom = g.First().Nhom,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var countTask = groupQuery.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
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
