using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.YeuCauHoanTra.KSNK
{
    public partial class YeuCauHoanTraKSNKService
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

            var queryObject = new NhapKhoKSNKSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoKSNKSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = GetDataYeuCauTraVatTu(null, queryInfo, queryObject);
            var queryTuChoiDuyet = GetDataYeuCauTraVatTu(false, queryInfo, queryObject);
            var queryTuDaDuyet = GetDataYeuCauTraVatTu(true, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DanhSachYeuCauHoanTraKSNKGridVo())
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

        private IQueryable<DanhSachYeuCauHoanTraKSNKGridVo> GetDataYeuCauTraVatTu(bool? duocDuyet, QueryInfo queryInfo, NhapKhoKSNKSearch queryObject)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var result = BaseRepository.TableNoTracking
                .Where(p => p.DuocDuyet == duocDuyet && p.KhoXuat != null && p.KhoXuat.LaKhoKSNK == true &&
                phongBenhVien != null && (p.KhoXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId))
                .Select(o => new DanhSachYeuCauHoanTraKSNKGridVo
                {
                    Id = o.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    NgayDuyet = o.NgayDuyet,
                    NgayYeuCau = o.NgayYeuCau,
                    KhoHoanTraTu = o.KhoXuat.Ten,
                    KhoHoanTraVe = o.KhoNhap.Ten,
                    Ma = o.SoPhieu,
                    NguoiDuyet = o.NhanVienDuyet.User.HoTen,
                    NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                    TinhTrang = o.DuocDuyet
                })
                .Union(_yeuCauTraDuocPhamRepository.TableNoTracking
                    .Where(p => p.DuocDuyet == duocDuyet && p.KhoXuat != null && p.KhoXuat.LaKhoKSNK == true &&
                                phongBenhVien != null && (p.KhoXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId))
                    .Select(o => new DanhSachYeuCauHoanTraKSNKGridVo
                    {
                        Id = o.Id,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                        NgayDuyet = o.NgayDuyet,
                        NgayYeuCau = o.NgayYeuCau,
                        KhoHoanTraTu = o.KhoXuat.Ten,
                        KhoHoanTraVe = o.KhoNhap.Ten,
                        Ma = o.SoPhieu,
                        NguoiDuyet = o.NhanVienDuyet.User.HoTen,
                        NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                        TinhTrang = o.DuocDuyet
                    }))
            .ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
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
                    result = result
                        .Where(p => tuNgay <= p.NgayYeuCau.Value.Date)
                        as IOrderedQueryable<DanhSachYeuCauHoanTraKSNKGridVo>;
                }

                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    result = (result ?? throw new InvalidOperationException())
                        .Where(p => denNgay >= p.NgayYeuCau.Value.Date)
                        as IOrderedQueryable<DanhSachYeuCauHoanTraKSNKGridVo>;
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    result = (result ?? throw new InvalidOperationException()).Where(p => tuNgay <= p.NgayDuyet.Value.Date)
                        as IOrderedQueryable<DanhSachYeuCauHoanTraKSNKGridVo>;
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    result = (result ?? throw new InvalidOperationException()).Where(p => denNgay >= p.NgayDuyet.Value.Date)
                        as IOrderedQueryable<DanhSachYeuCauHoanTraKSNKGridVo>;
                }
            }

            return result;
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new NhapKhoKSNKSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoKSNKSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = GetDataYeuCauTraVatTu(null, queryInfo, queryObject);
            var queryTuChoiDuyet = GetDataYeuCauTraVatTu(false, queryInfo, queryObject);
            var queryTuDaDuyet = GetDataYeuCauTraVatTu(true, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DanhSachYeuCauHoanTraKSNKGridVo())
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

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) &&
                queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?
                    .Replace("Format", "");
            }
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _ycTraVtChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraVatTuId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(s => new DanhSachYCHoanTraKSNKChiTietGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    VatTu = s.VatTuBenhVien.VatTus.Ten,
                    Ma = s.VatTuBenhVien.Ma,
                    HopDong = s.HopDongThauVatTu.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    Nhom = s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription(),
                    LaVatTuBhyt = s.LaVatTuBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                });

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBhyt, x.Ma, x.Vat, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new DanhSachYCHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    VatTu = g.First().VatTu,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBhyt = g.First().LaVatTuBhyt,
                    Ma = g.First().Ma,
                    Nhom = g.First().Nhom,
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
            var query = _ycTraVtChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraVatTuId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(s => new DanhSachYCHoanTraKSNKChiTietGridVo
                {
                    Id = s.Id,
                    VatTu = s.VatTuBenhVien.VatTus.Ten,
                    HopDong = s.HopDongThauVatTu.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    Nhom = s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription(),
                    LaVatTuBhyt = s.LaVatTuBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                });
            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBhyt, x.Ma, x.Vat, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new DanhSachYCHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    VatTu = g.First().VatTu,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBhyt = g.First().LaVatTuBhyt,
                    Ma = g.First().Ma,
                    Nhom = g.First().Nhom,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var countTask = groupQuery.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
        }


        public async Task<GridDataSource> GetDataForGridDuocPhamChildAsync(QueryInfo queryInfo)
        {
            var query = _yeuCauTraDuocPhamChiTiet.TableNoTracking
               .Where(p => p.YeuCauTraDuocPhamId == long.Parse(queryInfo.AdditionalSearchString))
               .Select(s => new DanhSachYCHoanTraKSNKChiTietGridVo
               {
                   Id = s.Id,
                   VatTuBenhVienId = s.DuocPhamBenhVienId,
                   VatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                   Ma = s.DuocPhamBenhVien.Ma,
                   HopDong = s.HopDongThauDuocPham.SoHopDong,
                   SoLuongTra = s.SoLuongTra,
                   HanSuDung = s.HanSuDung,
                   MaVach = s.MaVach,
                   DonGiaNhap = s.DonGiaNhap,
                   Vat = s.VAT,
                   Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   LaVatTuBhyt = s.LaDuocPhamBHYT,
                   SoLo = s.Solo,
                   TiLeThapGia = s.TiLeTheoThapGia,
                   NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien,
                   DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
               });

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBhyt, x.Ma, x.Vat, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new DanhSachYCHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    VatTu = g.First().VatTu,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBhyt = g.First().LaVatTuBhyt,
                    Ma = g.First().Ma,
                    Nhom = g.First().Nhom,
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

        public async Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsync(QueryInfo queryInfo)
        {
            var query = _yeuCauTraDuocPhamChiTiet.TableNoTracking
               .Where(p => p.YeuCauTraDuocPhamId == long.Parse(queryInfo.AdditionalSearchString))
               .Select(s => new DanhSachYCHoanTraKSNKChiTietGridVo
               {
                   Id = s.Id,
                   VatTuBenhVienId = s.DuocPhamBenhVienId,
                   VatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                   Ma = s.DuocPhamBenhVien.Ma,
                   HopDong = s.HopDongThauDuocPham.SoHopDong,
                   SoLuongTra = s.SoLuongTra,
                   HanSuDung = s.HanSuDung,
                   MaVach = s.MaVach,
                   DonGiaNhap = s.DonGiaNhap,
                   Vat = s.VAT,
                   Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   LaVatTuBhyt = s.LaDuocPhamBHYT,
                   SoLo = s.Solo,
                   TiLeThapGia = s.TiLeTheoThapGia,
                   NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
               });

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBhyt, x.Ma, x.Vat, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new DanhSachYCHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    VatTu = g.First().VatTu,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBhyt = g.First().LaVatTuBhyt,
                    Ma = g.First().Ma,
                    Nhom = g.First().Nhom,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var countTask = groupQuery.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
        }
    }
}
