using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Globalization;
using static Camino.Core.Domain.Enums;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial class YeuCauLinhDuocPhamService
    {
        #region  Ds linh duoc phẩm
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool print)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                BuildDefaultSortExpression(queryInfo);
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo);
                var queryDaDuyet = DaDuyet(queryInfo);

                var query = new List<DsLinhDuocPhamGridVo>();

                if (queryString.DangChoGoi == true)
                {
                    query = queryDangChoGoi.ToList();
                }
                if (queryString.DangChoDuyet== true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DsLinhDuocPhamGridVo>();
                    query = queryDangChoGoi.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }

                var dataOrderBy = query.AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var quaythuoc = dataOrderBy.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var queryDangChoGui = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui != true).Select(s => new DsLinhDuocPhamGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đang chờ duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null).Select(s => new DsLinhDuocPhamGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đang chờ duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false).Select(s => new DsLinhDuocPhamGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Từ Chối duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true).Select(s => new DsLinhDuocPhamGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đã duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var query = queryDangChoGui.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet);
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
           
            return null;
        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo);
                var queryDaDuyet = DaDuyet(queryInfo);

                var query = new List<DsLinhDuocPhamGridVo>();

                if (queryString.DangChoGoi == true)
                {
                    query = queryDangChoGoi.ToList();
                }
                if (queryString.DangChoDuyet == true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DsLinhDuocPhamGridVo>();
                    query = queryDangChoGoi.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }
                var dataOrderBy = query.AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var countTask = dataOrderBy.OrderBy(queryInfo.SortString).Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            return null;
        }
        #endregion
        #region child lĩnh dược phẩm con 
        #region Ds yêu cầu DuocPham child
        public async Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]); // 1 loai phieu linh
            int trangThai = 0;

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                BuildDefaultSortExpression(queryInfo);
                var trangThaiLinhBu = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(d => d.Id == long.Parse(queryString[0])).Select(d => d.DuocDuyet).FirstOrDefault();

                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            )
                            .Select(s => new YeuCauLinhDuocPhamBuGridVo
                            {
                                Id = s.Id,
                                YeuCauLinhDuocPhamId = s.YeuCauLinhDuocPhamId,
                                DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                                TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                                DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                LaBHYT = s.LaDuocPhamBHYT,
                                Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu.MathRoundNumber(2) : 0,
                                SLDaLinh = s.YeuCauLinhDuocPhamId != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                                LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                LinhVeKhoId = long.Parse(queryString[2]),
                                NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                                DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                                DaDuyet = s.YeuCauLinhDuocPhamId != null ? s.YeuCauLinhDuocPham.DuocDuyet : null,
                                SoLuongYeuCauDaDuyet = s.SoLuong // trường hợp cho đã duyệt

                            })
                            .GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                              .Select(item => new YeuCauLinhDuocPhamBuGridVo()
                              {
                                  Id = item.First().Id,
                                  DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                  LaBHYT = item.First().LaBHYT,
                                  TenVatTu = item.First().TenVatTu,
                                  Nhom = item.First().Nhom,
                                  DonViTinh = item.First().DonViTinh,
                                  HangSanXuat = item.First().HangSanXuat,
                                  NuocSanXuat = item.First().NuocSanXuat,
                                  SoLuongCanBu = item.Sum(x => x.SoLuongCanBu.MathRoundNumber(2)),
                                  LinhVeKhoId = long.Parse(queryString[2]),
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  NongDoHamLuong = item.FirstOrDefault().NongDoHamLuong,
                                  SLDaLinh = item.Sum(x => x.SLDaLinh),
                                  HoatChat = item.First().HoatChat,
                                  DuongDung = item.First().DuongDung,
                                  DaDuyet = item.First().DaDuyet,
                                  SoLuongYeuCauDaDuyet = item.Sum(s => s.SoLuongYeuCauDaDuyet)
                              })
                              ;
                var DuocPhamLinhBuGridVos = query.ToList();
                if(trangThaiLinhBu == null)
                {
                    var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                      ).ToList();

                    var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                    result = result.Select(o =>
                    {
                        o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                             : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                        return o;
                    });
                    result = result.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                    var queryTask = result.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
                else
                {
                    query = query.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                    var queryTask = query.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
               
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                        HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                        NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                        DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        SLTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == s.DuocPhamBenhVienId && o.LaDuocPhamBHYT == s.LaDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == s.YeuCauLinhDuocPham.KhoXuatId ).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber(1),
                        DuocDuyet = s.YeuCauLinhDuocPham != null ? s.YeuCauLinhDuocPham.DuocDuyet :null,
                        LaBHYT = s.LaDuocPhamBHYT
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(d=>d.LaBHYT).ThenBy(d=>d.TenVatTu).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                BuildDefaultSortExpression(queryInfo);
                if (queryString[3] == "true" || string.IsNullOrEmpty(queryString[1]))
                {
                    trangThai = 0;
                }// 3 Duoc duyet : dang duyet , đa duyet
                if (queryString[3] == "false" || queryString[3] == "False")
                {
                    trangThai = 1;
                } // tu choi duyet
                if (trangThai == 1)
                {
                    var yeuCauLinhDuocPham =
                    await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    IQueryable<DSLinhDuocPhamChildTuGridVo> queryable = null;
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DuocDuyet = item.YeuCauLinhDuocPham.DuocDuyet,
                           DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                           BacSyKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                           NgayDieuTri = item.YeuCauDuocPhamBenhVien != null ? (item.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTriId != null) ? item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : (DateTime?)item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh : null,
                           NgayKetString = item.YeuCauDuocPhamBenhVien != null ? item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH():""
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.SoLuongYeuCau
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.LaDuocPhamBHYT == item.First().LaBHYT
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           Nhom = item.First().Nhom,
                           DuocDuyet = item.First().DuocDuyet,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString =item.First().NgayKetString
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
                    var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                else
                {
                    var yeuCauLinhId = long.Parse(queryString[0]);

                    //08/11/2021: cập nhật nếu đã duyệt thì lấy từ YeuCauLinhDuocPhamChiTiet
                    var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == yeuCauLinhId);
                    IQueryable<DSLinhDuocPhamChildTuGridVo> query = null;

                    if (yeuCauLinhDuocPham.DuocDuyet != true)
                    {
                        query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                    && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && o.YeuCauTiepNhan.BenhNhanId != null)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                            HoTen = s.YeuCauTiepNhan.HoTen,
                            SoLuong = s.SoLuong,
                            DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                            NgayKe = s.ThoiDiemChiDinh,
                            DuocDuyet = s.YeuCauLinhDuocPham.DuocDuyet,
                            KhoLinhId = (long)s.KhoLinhId
                        })
                        .GroupBy(x => new
                        {
                            x.BenhNhanId,
                            x.MaYeuCauTiepNhan,
                            x.HoTen,

                        })
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                            MaBenhNhan = s.First().MaBenhNhan,
                            HoTen = s.First().HoTen,
                            SoLuong = s.Sum(a => a.SoLuong),
                            DichVuKham = s.First().DichVuKham,
                            BacSiKeToa = s.First().BacSiKeToa,
                            NgayKe = s.First().NgayKe,
                            LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                            Id = yeuCauLinhId,
                            DuocDuyet = s.First().DuocDuyet,
                            KhoLinhId = s.First().KhoLinhId
                        });
                    }
                    else
                    {
                        query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                        && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                        && o.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                        && o.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhanId != null)
                            .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                            .Select(s => new DSLinhDuocPhamChildTuGridVo
                            {
                                YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                                MaYeuCauTiepNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBenhNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                                HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                                SoLuong = s.SoLuong,
                                DichVuKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null 
                                    ? s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu
                                    : (s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                                BacSiKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh,
                                DuocDuyet = s.YeuCauLinhDuocPham.DuocDuyet,
                                KhoLinhId = (long)s.YeuCauDuocPhamBenhVien.KhoLinhId
                            })
                        .GroupBy(x => new
                        {
                            x.BenhNhanId,
                            x.MaYeuCauTiepNhan,
                            x.HoTen,

                        })
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                            MaBenhNhan = s.First().MaBenhNhan,
                            HoTen = s.First().HoTen,
                            SoLuong = s.Sum(a => a.SoLuong),
                            DichVuKham = s.First().DichVuKham,
                            BacSiKeToa = s.First().BacSiKeToa,
                            NgayKe = s.First().NgayKe,
                            LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                            Id = yeuCauLinhId,
                            DuocDuyet = s.First().DuocDuyet,
                            KhoLinhId = s.First().KhoLinhId
                        });
                    }

                    if (queryString[4] != null && queryString[4] != "" && queryString[4] != "true")
                    {
                        if(query.Any())
                        {
                            var list = DataChoGoi((long)query.First().KhoLinhId).AsQueryable();
                            query = query.Union(list);
                        }
                        
                    }

                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }

            }
            return null;
        }
        public async Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {

                //var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                //.Where(o => o.DuocPhamBenhVienId == long.Parse(queryString[2])
                //            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                //            && o.KhongLinhBu != true
                //            && o.YeuCauLinhDuocPhamId != null
                //            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                //           )
                var trangThaiDuyet = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPham.DuocDuyet).FirstOrDefault();
                var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                //    .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                //            && o.DuocPhamBenhVienId == duocPhamBenhVienId
                //            && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                //            && o.LaDuocPhamBHYT == laBHYT
                //            && o.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien)
                //.OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                            .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                             && p.DuocPhamBenhVienId == long.Parse(queryString[2])
                             && p.LaDuocPhamBHYT == bool.Parse(queryString[3])
                             && p.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                             && p.YeuCauLinhDuocPham.DuocDuyet == trangThaiDuyet
                            //&& (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            )
                .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                .Select(s => new DuocPhamLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                    SL = trangThaiDuyet == true ? s.SoLuong : s.SoLuongCanBu,
                    DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0,
                    SLDanhSachDuyet = s.SoLuong.MathRoundNumber(2),
                    SLCanBu = s.SoLuongCanBu,
                    NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                // 5 6
                BuildDefaultSortExpression(queryInfo);
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true")
                {
                    kieuIn = 0;
                }
                if (queryString[6] == "false" || queryString[6] == "False")
                {
                    kieuIn = 1;
                }
                var yeuCauLinh =
                     _yeuCauLinhDuocPhamRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));


                List<DSLinhDuocPhamChildTuGridVo> queryable = new List<DSLinhDuocPhamChildTuGridVo>();
                if (kieuIn == 0)
                {
                    if(yeuCauLinh == true)
                    {
                        var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));

                        // cập nhật 29/10/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhDuocPhamCHiTiet
                        if (yeuCauLinhDuocPham.DuocDuyet != true)
                        {
                            queryable = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                               .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                           && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                           && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                           && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                               .Select(item => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                                   DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                                   LaBHYT = item.LaDuocPhamBHYT,
                                   TenDuocPham = item.Ten,
                                   NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                                   HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                                   DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                                   DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                   HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                   NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                   SoLuongYeuCau = item.SoLuong,
                                   Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                   DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                                   BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                                   BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                                   NgayKe = item.ThoiDiemChiDinh,
                                   NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                   NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                               })
                               .GroupBy(x => new
                               {
                                   x.YeuCauLinhDuocPhamId,
                                   x.DuocPhamBenhVienId,
                                   x.LaBHYT,
                                   x.Nhom,
                                   x.NongDoHamLuong,
                                   x.HoatChat,
                                   x.DuongDung,
                                   x.DonViTinh,
                                   x.HangSanXuat,
                                   x.NuocSanXuat,
                                   x.DichVuKham,
                                   x.BacSiKeToa,
                                   //x.NgayKe
                               })
                               .Select(item => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                                   DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                   LaBHYT = item.First().LaBHYT,
                                   TenDuocPham = item.First().TenDuocPham,
                                   NongDoHamLuong = item.First().NongDoHamLuong,
                                   HoatChat = item.First().HoatChat,
                                   DuongDung = item.First().DuongDung,
                                   DonViTinh = item.First().DonViTinh,
                                   HangSanXuat = item.First().HangSanXuat,
                                   NuocSanXuat = item.First().NuocSanXuat,
                                   SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                                   SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                       .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                                   && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                                   && x.LaDuocPhamBHYT == item.First().LaBHYT
                                                   && x.NhapKhoDuocPhams.DaHet != true
                                                   && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   DichVuKham = item.First().DichVuKham,
                                   BacSiKeToa = item.First().BacSiKeToa,
                                   BacSyKeToa = item.First().BacSyKeToa,
                                   NgayKe = item.First().NgayKe,
                                   Nhom = item.First().Nhom,
                                   NgayDieuTri = item.First().NgayDieuTri,
                                   NgayKetString = item.First().NgayKetString
                               })
                               .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        }
                        else
                        {
                            queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                            && x.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                            && x.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && x.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId == long.Parse(queryString[5]))
                                .Select(item => new DSLinhDuocPhamChildTuGridVo()
                                {
                                    YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                                    DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                                    LaBHYT = item.LaDuocPhamBHYT,
                                    TenDuocPham = item.YeuCauDuocPhamBenhVien.Ten,
                                    NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                                    HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                                    DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                                    DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                    HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                    NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                    SoLuongYeuCau = item.SoLuong,
                                    Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                    DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null 
                                        ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu 
                                        : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                                    BacSiKeToa = item.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                    BacSyKeToa = item.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                    NgayKe = item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh,
                                    NgayKetString = item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                    NgayDieuTri = (item.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTriId != null)
                                        ? item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                                })
                               .GroupBy(x => new
                               {
                                   x.YeuCauLinhDuocPhamId,
                                   x.DuocPhamBenhVienId,
                                   x.LaBHYT,
                                   x.Nhom,
                                   x.NongDoHamLuong,
                                   x.HoatChat,
                                   x.DuongDung,
                                   x.DonViTinh,
                                   x.HangSanXuat,
                                   x.NuocSanXuat,
                                   x.DichVuKham,
                                   x.BacSiKeToa,
                                   //x.NgayKe
                               })
                               .Select(item => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                                   DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                   LaBHYT = item.First().LaBHYT,
                                   TenDuocPham = item.First().TenDuocPham,
                                   NongDoHamLuong = item.First().NongDoHamLuong,
                                   HoatChat = item.First().HoatChat,
                                   DuongDung = item.First().DuongDung,
                                   DonViTinh = item.First().DonViTinh,
                                   HangSanXuat = item.First().HangSanXuat,
                                   NuocSanXuat = item.First().NuocSanXuat,
                                   SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                                   SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                       .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                                   && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                                   && x.LaDuocPhamBHYT == item.First().LaBHYT
                                                   && x.NhapKhoDuocPhams.DaHet != true
                                                   && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   DichVuKham = item.First().DichVuKham,
                                   BacSiKeToa = item.First().BacSiKeToa,
                                   BacSyKeToa = item.First().BacSyKeToa,
                                   NgayKe = item.First().NgayKe,
                                   Nhom = item.First().Nhom,
                                   NgayDieuTri = item.First().NgayDieuTri,
                                   NgayKetString = item.First().NgayKetString
                               })
                               .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        }
                    }
                    else
                    {
                        long khoaId = 0;
                        var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
                        if (phongBenhVien != null)
                        {
                            khoaId = phongBenhVien.KhoaPhongId;
                        }

                        //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
                        var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
                        {
                            KeyId = (long)o.Id,
                            DisplayName = o.Ten
                        }).OrderBy(o => o.DisplayName).ToList();
                        var ques = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == null 
                                   && x.KhoLinhId == long.Parse(queryString[7]) &&
                                   phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null)? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                           ,
                           SoLuongTon = item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == item.KhoLinhId && nkct.LaDuocPhamBHYT == item.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat)
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           //x.NgayKetString
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString = item.First().NgayKetString
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        if(ques.Any())
                        {
                            queryable = queryable.Union(ques).ToList();
                        }
                    }
                }
                if (kieuIn == 1)
                {
                    var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                            NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                            HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                            DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                            DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                            NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                            DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                            BacSyKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : ""
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhDuocPhamId,
                            x.DuocPhamBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenDuocPham = item.First().TenDuocPham,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom,
                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            BacSyKeToa = item.First().BacSyKeToa,
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                }

                //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.AsQueryable().CountAsync();
                //var queryTask = queryable.AsQueryable().Skip(queryInfo.Skip)
                //    .Take(queryInfo.Take).ToArrayAsync();
                //await Task.WhenAll(countTask, queryTask);
                //return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                var dataOrderBy = queryable.AsQueryable().OrderBy(queryInfo.SortString);
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;

            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {

                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                              //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.ChuaThucHien
                              && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            //&& (p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu == null || p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu < p.YeuCauDuocPhamBenhVien.SoLuong)
                            )
                            .Select(s => new YeuCauLinhDuocPhamBuGridVo
                            {
                                Id = s.Id,
                                YeuCauLinhDuocPhamId = s.YeuCauLinhDuocPhamId,
                                DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                                TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                                DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                LaBHYT = s.LaDuocPhamBHYT,
                                Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu : 0,
                                SLDaLinh = s.YeuCauDuocPhamBenhVien != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                                LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                LinhVeKhoId = long.Parse(queryString[2])
                            })
                            .GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                              .Select(item => new YeuCauLinhDuocPhamBuGridVo()
                              {
                                  DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                  LaBHYT = item.First().LaBHYT,
                                  TenVatTu = item.First().TenVatTu,
                                  Nhom = item.First().Nhom,
                                  DonViTinh = item.First().DonViTinh,
                                  HangSanXuat = item.First().HangSanXuat,
                                  NuocSanXuat = item.First().NuocSanXuat,
                                  SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                                  LinhVeKhoId = long.Parse(queryString[2]),
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  SLDaLinh = item.Sum(x => x.SLDaLinh)
                              })
                              .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var DuocPhamLinhBuGridVos = query.ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                       ).ToList();

                var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                result = result.Select(o =>
                {
                    //o.SoLuongTon = lstVatTuBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                    //o.SoLuongYeuCau = o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon : o.SoLuongCanBu;

                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                    o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                    return o;
                });
                var dataOrderBy = result.AsQueryable().OrderBy(queryInfo.SortString);
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                int trangThai = 0;
                if (queryString[3] == "true" || string.IsNullOrEmpty(queryString[1]))
                {
                    trangThai = 0;
                }// 3 Duoc duyet : dang duyet , đa duyet
                if (queryString[3] == "false")
                {
                    trangThai = 1;
                } // tu choi duyet
                if (trangThai == 1)
                {
                    var yeuCauLinhDuocPham =
                    await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    IQueryable<DSLinhDuocPhamChildTuGridVo> queryable = null;
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.SoLuongYeuCau
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.LaDuocPhamBHYT == item.First().LaBHYT
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           Nhom = item.First().Nhom
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                    var countTask = queryable.CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                else
                {
                    var yeuCauLinhId = long.Parse(queryString[0]);

                    var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                    && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && o.YeuCauTiepNhan.BenhNhanId != null)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                            HoTen = s.YeuCauTiepNhan.HoTen,
                            SoLuong = s.SoLuong,
                            DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                            NgayKe = s.ThoiDiemChiDinh,
                            DuocDuyet = s.YeuCauLinhDuocPham.DuocDuyet,
                            NgayDieuTri = (s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTriId != null)? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                            KhoLinhId = (long)s.KhoLinhId
                        })
                        .GroupBy(x => new
                        {
                            x.BenhNhanId,
                            x.MaYeuCauTiepNhan,
                            x.HoTen,

                        })
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                            MaBenhNhan = s.First().MaBenhNhan,
                            HoTen = s.First().HoTen,
                            SoLuong = s.Sum(a => a.SoLuong),
                            DichVuKham = s.First().DichVuKham,
                            BacSiKeToa = s.First().BacSiKeToa,
                            NgayKe = s.First().NgayKe,
                            LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                            Id = yeuCauLinhId,
                            DuocDuyet = s.First().DuocDuyet,
                            NgayDieuTri = s.First().NgayDieuTri,
                            KhoLinhId = s.First().KhoLinhId
                        });
                    if (queryString[4] != null && queryString[4] != "" && queryString[4] != "true")
                    {
                        if (query.Any())
                        {
                            var list = DataChoGoi((long)query.First().KhoLinhId).AsQueryable();
                            query = query.Union(list);
                        }

                    }
                    var countTask = query.CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                           && p.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                            && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            )
                .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                .Select(s => new DuocPhamLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                    SL = s.SoLuongCanBu,
                    DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0
                });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                BuildDefaultSortExpression(queryInfo);
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true")
                {
                    kieuIn = 0;
                }
                if (queryString[6] == "false")
                {
                    kieuIn = 1;
                }
                var yeuCauLinh =
                     _yeuCauLinhDuocPhamRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));


                List<DSLinhDuocPhamChildTuGridVo> queryable = new List<DSLinhDuocPhamChildTuGridVo>();
                if (kieuIn == 0)
                {
                    if (yeuCauLinh == true)
                    {
                        var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                        queryable = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           //x.NgayKe
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                           && x.LaDuocPhamBHYT == item.First().LaBHYT
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                    }
                    else
                    {
                        long khoaId = 0;
                        var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
                        if (phongBenhVien != null)
                        {
                            khoaId = phongBenhVien.KhoaPhongId;
                        }

                        //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
                        var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
                        {
                            KeyId = (long)o.Id,
                            DisplayName = o.Ten
                        }).OrderBy(o => o.DisplayName).ToList();
                        var ques = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == null
                                   && x.KhoLinhId == long.Parse(queryString[7]) &&
                                   phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                           ,
                           SoLuongTon = item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == item.KhoLinhId && nkct.LaDuocPhamBHYT == item.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat)
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           //x.NgayKe
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        if (ques.Any())
                        {
                            queryable = queryable.Union(ques).ToList();
                        }
                    }
                }
                if (kieuIn == 1)
                {
                    var yeuCauLinhDuocPham =
                       await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                            NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                            HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                            DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                            DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                            NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhDuocPhamId,
                            x.DuocPhamBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenDuocPham = item.First().TenDuocPham,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                }
                var dataOrderBy = queryable.AsQueryable().OrderBy(queryInfo.SortString);
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            return null;
        }
        #endregion

        #endregion
        #region In lĩnh dược phẩm
        public async Task<string> InLinhDuocPham(XacNhanInLinhDuocPham xacNhanInLinhDuocPham)
        {
            var content = "";
            var content1 = "";
            var ThuocHoacVatTu = " ";
            var groupThuocBHYT = "Dược Phẩm BHYT";
            var groupThuocKhongBHYT = "Dược Phẩm Không BHYT";

            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU LĨNH DƯỢC PHẨM</div></div>";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";
            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            int? nhomVatTu = 0;
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }
            var result = _templateRepo.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuLinhThuocDuocPham"));
            var resultGayNghien = _templateRepo.TableNoTracking
             .FirstOrDefault(x => x.Name.Equals("PhieuLinhThuocTrucTiepGayNghien"));


            var kiemTraPhieuTaoDaDuyet = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(d => d.Id == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId).Select(d => d.DuocDuyet).First();
            var yeuCauLinhDuocPham = BaseRepository.GetByIdAsync(xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId,
                                                                s => s.Include(z => z.KhoNhap)
                                                                     .Include(z=>z.YeuCauLinhDuocPhamChiTiets).ThenInclude(k => k.DuocPhamBenhVien).ThenInclude(z => z.DuocPham).ThenInclude(w => w.DonViTinh)
                                                                     .Include(z => z.KhoXuat)
                                                                     .Include(z => z.NhanVienYeuCau)
                                                                     .Include(z => z.NhanVienDuyet)
                                                                     .Include(z => z.YeuCauDuocPhamBenhViens).ThenInclude(k => k.DuocPhamBenhVien).ThenInclude(z => z.DuocPham).ThenInclude(w => w.DonViTinh)
                                                                     .Include(z => z.YeuCauLinhDuocPhamChiTiets).ThenInclude(z=>z.YeuCauDuocPhamBenhVien).ThenInclude(z => z.YeuCauTiepNhan)
                                                                     .Include(z => z.YeuCauLinhDuocPhamChiTiets).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham).ThenInclude(p => p.DonViTinh)
                                                                     .Include(z => z.YeuCauLinhDuocPhamChiTiets).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham).ThenInclude(p => p.DuongDung)
                                                                     .Include(z => z.YeuCauLinhDuocPhamChiTiets)
                                                                     .Include(z => z.YeuCauLinhDuocPhamChiTiets).ThenInclude(p=>p.YeuCauDuocPhamBenhVien).ThenInclude(p=>p.YeuCauTiepNhan)
                                                                     .Include(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z=>z.YeuCauTiepNhan));

            if (xacNhanInLinhDuocPham.LoaiPhieuLinh == (int)EnumLoaiPhieuLinh.LinhChoBenhNhan)  // lĩnh  bệnh nhân
            {

                if (xacNhanInLinhDuocPham.TrangThaiIn == false)
                {
                    
                    var yeuCauLinhDuocPhamTuChoi = yeuCauLinhDuocPham.Result.YeuCauLinhDuocPhamChiTiets.Where(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId);
                    var listDuocPham = yeuCauLinhDuocPhamTuChoi.Where(d => d.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && d.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                              .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                              {
                                                                  LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                                                                  MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                  TenDuocPham = o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId != null ? Convert.ToInt32(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == Convert.ToInt32(nhomVatTu) ?
                                                                                                                                 o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                   (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                 o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : "") : "",//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                  DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                  SoLuong = o.SoLuong,
                                                                  DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                  SoLuongCoTheXuat = o.SoLuongCoTheXuat,
                                                              }).GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DonViTinh })
                                                            .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                            {
                                                                LaDuocPhamBHYT = o.First().LaDuocPhamBHYT,
                                                                MaDuocPham = o.First().MaDuocPham,
                                                                TenDuocPham = o.First().TenDuocPham,
                                                                SoLuong = o.Sum(s => s.SoLuong),
                                                                DonViTinh = o.First().DonViTinh,
                                                            }).OrderBy(p=>p.TenDuocPham).ToList();

                    var listGayNghienHuongThan = yeuCauLinhDuocPhamTuChoi.Where(d => (d.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || d.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                                                                  .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                                  {
                                                                      LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                                                                      MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                      TenDuocPham = o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId != null ? Convert.ToInt32(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == Convert.ToInt32(nhomVatTu) ?
                                                                                                                                     o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                       (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                     o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : "") : "",//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                      DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                      SoLuong = o.SoLuong,
                                                                      DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                      SoLuongCoTheXuat = o.SoLuongCoTheXuat
                                                                  }).GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DonViTinh })
                                                                .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                                {
                                                                    LaDuocPhamBHYT = o.First().LaDuocPhamBHYT,
                                                                    MaDuocPham = o.First().MaDuocPham,
                                                                    TenDuocPham = o.First().TenDuocPham,
                                                                    SoLuong = o.Sum(s => s.SoLuong),
                                                                    DonViTinh = o.First().DonViTinh,
                                                                }).OrderBy(p => p.TenDuocPham).ToList();

                    if (listDuocPham.Any())
                    {
                        var getObjData = GetHTMLLinhBenhNhanTuChoi(listDuocPham,false);
                        ThuocHoacVatTu = getObjData.html;

                        var maVachPhieuLinh = yeuCauLinhDuocPham.Result.SoPhieu.ToString();
                        var data = new
                        {
                            LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPham.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                            MaVachPhieuLinh = maVachPhieuLinh,
                            NoiGiao = yeuCauLinhDuocPham?.Result?.KhoXuat?.Ten,
                            DienGiai = yeuCauLinhDuocPham?.Result?.GhiChu,
                            TruongKhoaDuocVTYT = "",
                            NguoiGiao = yeuCauLinhDuocPham?.Result?.NhanVienDuyet?.User?.HoTen,
                            NguoiNhan = yeuCauLinhDuocPham.Result?.NhanVienYeuCau?.User?.HoTen,
                            TuNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                            DenNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                            TruongPhongKhoaPhong = "",
                            CongKhoan = getObjData.Index - 1,
                            NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                            //HeaderPhieuLinhThuoc = tenLoaiLinh,
                            ThuocHoacVatTu = ThuocHoacVatTu,
                            KhoaPhong = yeuCauLinhDuocPham?.Result?.KhoNhap?.Ten,
                            Ngay = DateTime.Now.Day,
                            Thang = DateTime.Now.Month,
                            Nam = DateTime.Now.Year,
                            NoiNhan= TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPham?.Result?.NoiYeuCauId)
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    }
                    if (listGayNghienHuongThan.Any())
                    {

                        var getObjData = GetHTMLLinhBenhNhanTuChoi(listGayNghienHuongThan,true);
                        ThuocHoacVatTu = getObjData.html;

                        var maVachPhieuLinh = yeuCauLinhDuocPham.Result.SoPhieu.ToString();
                        var data = new
                        {
                            LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPham.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                            MaVachPhieuLinh = maVachPhieuLinh,
                            NoiGiao = yeuCauLinhDuocPham?.Result?.KhoXuat?.Ten,
                            DienGiai = yeuCauLinhDuocPham?.Result?.GhiChu,
                            TruongKhoaDuocVTYT = "",
                            NguoiGiao = yeuCauLinhDuocPham?.Result?.NhanVienDuyet?.User?.HoTen,
                            NguoiNhan = yeuCauLinhDuocPham.Result?.NhanVienYeuCau?.User?.HoTen,
                            TuNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                            DenNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                            TruongPhongKhoaPhong = "",
                            CongKhoan = getObjData.Index - 1,
                            NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                            //HeaderPhieuLinhThuoc = tenLoaiLinh,
                            ThuocHoacVatTu = ThuocHoacVatTu,
                            KhoaPhong = yeuCauLinhDuocPham?.Result?.KhoNhap?.Ten,
                            Ngay = DateTime.Now.Day,
                            Thang = DateTime.Now.Month,
                            Nam = DateTime.Now.Year,
                            NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPham?.Result?.NoiYeuCauId)
                        };
                        content1 = TemplateHelpper.FormatTemplateWithContentTemplate(resultGayNghien.Body, data);
                    }
                }
                else
                {
                    if (yeuCauLinhDuocPham.Result != null)
                    {

                        string yeuCau = ""; // to do
                        var thucChat = 0; // to do
                        var tenLoaiLinh = "";
                        var donViTinh = "";

                        if(kiemTraPhieuTaoDaDuyet != true)
                        {
                            // trạng thái chưa duyệt => yêu cầu dược phẩm bệnh viện 
                            if (yeuCauLinhDuocPham.Result.YeuCauDuocPhamBenhViens
                                                       .Any(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                                                        s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                        s.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                                       (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                       ))
                            {
                                var duocPhamNormal = yeuCauLinhDuocPham.Result.YeuCauDuocPhamBenhViens
                                                                 .Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                                                                  s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                                 (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien &&
                                                                  s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan
                                                                  && s.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                                                                  ).
                                                                  Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                  {
                                                                      MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                      TenDuocPham = (int?)(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                                                                                           o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                             (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                           o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : ""),//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                      DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                      SoLuong = o.SoLuong,
                                                                      TenLoaiLinh = o.LoaiPhieuLinh.GetDescription(),
                                                                      DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                      GhiChu = o.GhiChu,
                                                                      YeuCau = o.SoLuong,
                                                                      LaDuocPhamBHYT = o.LaDuocPhamBHYT
                                                                  })
                                                                  .GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DuocDuyet, xy.DonViTinh })
                                                                  .Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                  {
                                                                      MaDuocPham = o.First().MaDuocPham,
                                                                      TenDuocPham = o.First().TenDuocPham,
                                                                      DuocDuyet = o.First().DuocDuyet,
                                                                      SoLuong = o.Sum(s => s.SoLuong),
                                                                      TenLoaiLinh = o.First().TenLoaiLinh,
                                                                      DonViTinh = o.First().DonViTinh,
                                                                      GhiChu = o.First().GhiChu,
                                                                      YeuCau = o.Sum(s => s.YeuCau),
                                                                      LaDuocPhamBHYT = o.First().LaDuocPhamBHYT
                                                                  }).OrderBy(p => p.TenDuocPham);
                                var objData = GetHTMLLinhBenhNhan(duocPhamNormal.ToList(),false);
                                ThuocHoacVatTu = objData.html;
                                var maVachPhieuLinh = yeuCauLinhDuocPham.Result.SoPhieu.ToString();
                                var data = new
                                {
                                    LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPham.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                                    MaVachPhieuLinh = maVachPhieuLinh,
                                    NoiGiao = yeuCauLinhDuocPham?.Result?.KhoXuat?.Ten,
                                    DienGiai = yeuCauLinhDuocPham?.Result?.GhiChu,
                                    TruongKhoaDuocVTYT = "",
                                    NguoiGiao = yeuCauLinhDuocPham?.Result?.NhanVienDuyet?.User?.HoTen,
                                    NguoiNhan = yeuCauLinhDuocPham.Result?.NhanVienYeuCau?.User?.HoTen,
                                    TuNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                                    DenNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                                    TruongPhongKhoaPhong = "",
                                    CongKhoan = objData.Index - 1,
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //HeaderPhieuLinhThuoc = tenLoaiLinh,
                                    ThuocHoacVatTu = ThuocHoacVatTu,
                                    KhoaPhong = yeuCauLinhDuocPham?.Result?.KhoNhap?.Ten,
                                    Ngay = DateTime.Now.Day,
                                    Thang = DateTime.Now.Month,
                                    Nam = DateTime.Now.Year,
                                    NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPham?.Result?.NoiYeuCauId)
                                };
                                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                            }

                            if (yeuCauLinhDuocPham.Result.YeuCauDuocPhamBenhViens
                                                             .Any(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                                                              s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                              s.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                                             (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                             ))
                            {

                                var duocPhamGayNghien = yeuCauLinhDuocPham.Result.YeuCauDuocPhamBenhViens
                                                                 .Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                                                                 s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                                  s.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                                                (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                                 ).
                                                                 Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                 {
                                                                     MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                     TenDuocPham = (int?)(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                                                                                          o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                            (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                          o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : ""),//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                     DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                     SoLuong = o.SoLuong,
                                                                     TenLoaiLinh = o.LoaiPhieuLinh.GetDescription(),
                                                                     DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                     GhiChu = o.GhiChu,
                                                                     YeuCau = o.SoLuong,
                                                                     LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                                                                 })
                                                                 .GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DuocDuyet, xy.DonViTinh })
                                                                 .Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                 {
                                                                     MaDuocPham = o.First().MaDuocPham,
                                                                     TenDuocPham = o.First().TenDuocPham,
                                                                     DuocDuyet = o.First().DuocDuyet,
                                                                     SoLuong = o.Sum(s => s.SoLuong),
                                                                     TenLoaiLinh = o.First().TenLoaiLinh,
                                                                     DonViTinh = o.First().DonViTinh,
                                                                     GhiChu = o.First().GhiChu,
                                                                     YeuCau = o.Sum(s => s.YeuCau),
                                                                     LaDuocPhamBHYT = o.First().LaDuocPhamBHYT
                                                                 }).OrderBy(p => p.TenDuocPham);
                                var objData = GetHTMLLinhBenhNhan(duocPhamGayNghien.ToList(),true);
                                ThuocHoacVatTu = objData.html;
                                var maVachPhieuLinh = yeuCauLinhDuocPham.Result.SoPhieu.ToString();
                                var data = new
                                {
                                    LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPham.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                                    MaVachPhieuLinh = maVachPhieuLinh,
                                    NoiGiao = yeuCauLinhDuocPham?.Result?.KhoXuat?.Ten,
                                    DienGiai = yeuCauLinhDuocPham?.Result?.GhiChu,
                                    TruongKhoaDuocVTYT = "",
                                    NguoiGiao = yeuCauLinhDuocPham?.Result?.NhanVienDuyet?.User?.HoTen,
                                    NguoiNhan = yeuCauLinhDuocPham.Result?.NhanVienYeuCau?.User?.HoTen,
                                    TuNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                                    DenNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                                    TruongPhongKhoaPhong = "",
                                    CongKhoan = objData.Index - 1,
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //HeaderPhieuLinhThuoc = tenLoaiLinh,
                                    ThuocHoacVatTu = ThuocHoacVatTu,
                                    KhoaPhong = yeuCauLinhDuocPham?.Result?.KhoNhap?.Ten,
                                    Ngay = DateTime.Now.Day,
                                    Thang = DateTime.Now.Month,
                                    Nam = DateTime.Now.Year,
                                    NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPham?.Result?.NoiYeuCauId)
                                };
                                content1 = TemplateHelpper.FormatTemplateWithContentTemplate(resultGayNghien.Body, data);
                            }





                        }
                        else
                        {

                            // trạng thái đã duyệt thì lấy theo yêu cầu lĩnh dược phẩm chi tiết 

                            if (yeuCauLinhDuocPham.Result.YeuCauLinhDuocPhamChiTiets
                                                          .Any(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId && 
                                                          (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                          ))
                            {
                                var duocPhamNormal = yeuCauLinhDuocPham.Result.YeuCauLinhDuocPhamChiTiets
                                                                 .Where(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId &&
                                                                 (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien &&
                                                                  s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                                  ).
                                                                  Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                  {
                                                                      MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                      TenDuocPham = (int?)(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                                                                                           o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                             (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                           o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : ""),//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                      DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                      SoLuong = o.SoLuong,
                                                                      TenLoaiLinh = o.YeuCauLinhDuocPham.LoaiPhieuLinh.GetDescription(),
                                                                      DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                      GhiChu = o.YeuCauLinhDuocPham.GhiChu,
                                                                      YeuCau = o.SoLuong,
                                                                      LaDuocPhamBHYT = o.LaDuocPhamBHYT
                                                                  })
                                                                  .GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DuocDuyet, xy.DonViTinh })
                                                                  .Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                  {
                                                                      MaDuocPham = o.First().MaDuocPham,
                                                                      TenDuocPham = o.First().TenDuocPham,
                                                                      DuocDuyet = o.First().DuocDuyet,
                                                                      SoLuong = o.Sum(s => s.SoLuong),
                                                                      TenLoaiLinh = o.First().TenLoaiLinh,
                                                                      DonViTinh = o.First().DonViTinh,
                                                                      GhiChu = o.First().GhiChu,
                                                                      YeuCau = o.Sum(s => s.YeuCau),
                                                                      LaDuocPhamBHYT = o.First().LaDuocPhamBHYT
                                                                  }).OrderBy(p => p.TenDuocPham);
                                var objData = GetHTMLLinhBenhNhan(duocPhamNormal.ToList(),false);
                                ThuocHoacVatTu = objData.html;
                                var maVachPhieuLinh = yeuCauLinhDuocPham.Result.SoPhieu.ToString();
                                var data = new
                                {
                                    LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPham.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                                    MaVachPhieuLinh = maVachPhieuLinh,
                                    NoiGiao = yeuCauLinhDuocPham?.Result?.KhoXuat?.Ten,
                                    DienGiai = yeuCauLinhDuocPham?.Result?.GhiChu,
                                    TruongKhoaDuocVTYT = "",
                                    NguoiGiao = yeuCauLinhDuocPham?.Result?.NhanVienDuyet?.User?.HoTen,
                                    NguoiNhan = yeuCauLinhDuocPham.Result?.NhanVienYeuCau?.User?.HoTen,
                                    TuNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                                    DenNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                                    TruongPhongKhoaPhong = "",
                                    CongKhoan = objData.Index - 1,
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //HeaderPhieuLinhThuoc = tenLoaiLinh,
                                    ThuocHoacVatTu = ThuocHoacVatTu,
                                    KhoaPhong = yeuCauLinhDuocPham?.Result?.KhoNhap?.Ten,
                                    Ngay = DateTime.Now.Day,
                                    Thang = DateTime.Now.Month,
                                    Nam = DateTime.Now.Year,
                                    NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPham?.Result?.NoiYeuCauId)
                                };
                                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                            }

                            if (yeuCauLinhDuocPham.Result.YeuCauLinhDuocPhamChiTiets
                                                             .Any(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId &&
                                                             (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                             ))
                            {

                                var duocPhamGayNghien = yeuCauLinhDuocPham.Result.YeuCauLinhDuocPhamChiTiets
                                                                 .Where(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId &&
                                                                (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                                 ).
                                                                 Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                 {
                                                                     MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                     TenDuocPham = (int?)(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                                                                                          o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                            (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                          o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : ""),//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                     DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                     SoLuong = o.SoLuong,
                                                                     TenLoaiLinh = o.YeuCauLinhDuocPham.LoaiPhieuLinh.GetDescription(),
                                                                     DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                     GhiChu = o.YeuCauLinhDuocPham.GhiChu,
                                                                     YeuCau = o.SoLuong,
                                                                     LaDuocPhamBHYT = o.LaDuocPhamBHYT
                                                                 })
                                                                 .GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DuocDuyet, xy.DonViTinh })
                                                                 .Select(o => new DuocPhamVatTuLinhTTGridVo
                                                                 {
                                                                     MaDuocPham = o.First().MaDuocPham,
                                                                     TenDuocPham = o.First().TenDuocPham,
                                                                     DuocDuyet = o.First().DuocDuyet,
                                                                     SoLuong = o.Sum(s => s.SoLuong),
                                                                     TenLoaiLinh = o.First().TenLoaiLinh,
                                                                     DonViTinh = o.First().DonViTinh,
                                                                     GhiChu = o.First().GhiChu,
                                                                 }).OrderBy(p => p.TenDuocPham);
                                var objData = GetHTMLLinhBenhNhan(duocPhamGayNghien.ToList(),true);
                                ThuocHoacVatTu = objData.html;
                                var maVachPhieuLinh = yeuCauLinhDuocPham.Result.SoPhieu.ToString();
                                var data = new
                                {
                                    LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPham.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                                    MaVachPhieuLinh = maVachPhieuLinh,
                                    NoiGiao = yeuCauLinhDuocPham?.Result?.KhoXuat?.Ten,
                                    DienGiai = yeuCauLinhDuocPham?.Result?.GhiChu,
                                    TruongKhoaDuocVTYT = "",
                                    NguoiGiao = yeuCauLinhDuocPham?.Result?.NhanVienDuyet?.User?.HoTen,
                                    NguoiNhan = yeuCauLinhDuocPham.Result?.NhanVienYeuCau?.User?.HoTen,
                                    TuNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                                    DenNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                                    TruongPhongKhoaPhong = "",
                                    CongKhoan = objData.Index - 1,
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //HeaderPhieuLinhThuoc = tenLoaiLinh,
                                    ThuocHoacVatTu = ThuocHoacVatTu,
                                    KhoaPhong = yeuCauLinhDuocPham?.Result?.KhoNhap?.Ten,
                                    Ngay = DateTime.Now.Day,
                                    Thang = DateTime.Now.Month,
                                    Nam = DateTime.Now.Year,
                                    NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPham?.Result?.NoiYeuCauId)
                                };
                                content1 = TemplateHelpper.FormatTemplateWithContentTemplate(resultGayNghien.Body, data);
                            }

                        }



                    }
                }
                if (!string.IsNullOrEmpty(content))
                {
                    content = headerTitile + content;
                }

                if (!string.IsNullOrEmpty(content1))
                {
                    content = content + headerTitile + "<div style='break-after:page'></div>" + content1;
                }
            }
            return content;
        }
        #endregion
        #region Ds duyệt dược phẩm
        public async Task<GridDataSource> GetDataDSDuyetDuocPhamForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                BuildDefaultSortExpression(queryInfo);
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                //var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo,true);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo, true);
                var queryDaDuyet = DaDuyet(queryInfo, true);

                var query = new List<DsLinhDuocPhamGridVo>();

                //if (queryString.DangChoGoi == true)
                //{
                //    query = queryDangChoGoi.ToList();
                //}
                if (queryString.DangChoDuyet == true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DsLinhDuocPhamGridVo>();
                    query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }

                var dataOrderBy = query.AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var quaythuoc = dataOrderBy.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                //var queryDangChoGui = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui != true).Select(s => new DsLinhDuocPhamGridVo()
                //{
                //    Id = s.Id,
                //    MaPL = s.SoPhieu,
                //    Loai = s.LoaiPhieuLinh.GetDescription(),
                //    LoaiPhieuLinh = s.LoaiPhieuLinh,
                //    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                //    LinhTuKho = s.KhoXuat.Ten,
                //    LinhVeKhoId = s.KhoXuatId,
                //    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                //    NgayYeuCau = s.NgayYeuCau,
                //    TinhTrang = "Đang chờ duyệt",
                //    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                //    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                //    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                //    DuocDuyet = s.DuocDuyet
                //});
                var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null).Select(s => new DsLinhDuocPhamGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đang chờ duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false).Select(s => new DsLinhDuocPhamGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Từ Chối duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true).Select(s => new DsLinhDuocPhamGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đã duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var query =queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet);
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }

            return null;
        }

        public async Task<GridDataSource> GetDSDuyetDuocPhamTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                //var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo, true);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo, true);
                var queryDaDuyet = DaDuyet(queryInfo, true);

                var query = new List<DsLinhDuocPhamGridVo>();

                //if (queryString.DangChoGoi == true)
                //{
                //    query = queryDangChoGoi.ToList();
                //}
                if (queryString.DangChoDuyet == true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DsLinhDuocPhamGridVo>();
                    query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }
                var dataOrderBy = query.AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var countTask = dataOrderBy.OrderBy(queryInfo.SortString).Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            return null;
        }
        #endregion
        private List<DsLinhDuocPhamGridVo> DangChoGoi(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null &&  x.DaGui != true &&
                                                               ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DsLinhDuocPhamGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                LinhVeKhoId = s.KhoXuatId,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đang chờ gởi",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDangChoDuyet = queryDangChoDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
            }
            return queryDangChoDuyet.OrderByDescending(d=>d.NgayYeuCau).ToList();
        }
        private List<DsLinhDuocPhamGridVo> DangChoDuyet(QueryInfo queryInfo,bool manHinhDuyet=false)
        {
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui == true 
                                                           && ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau !=null && phongBenhVien!=null &&  x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DsLinhDuocPhamGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                LinhVeKhoId = s.KhoXuatId,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đang chờ duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui

            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDangChoDuyet = queryDangChoDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
               
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
            }
            return queryDangChoDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DsLinhDuocPhamGridVo> TuChoiDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false  && x.DaGui == true  && ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DsLinhDuocPhamGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                LinhVeKhoId = s.KhoXuatId,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Từ Chối duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryTuChoiDuyet = queryTuChoiDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryTuChoiDuyet = queryTuChoiDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryTuChoiDuyet = queryTuChoiDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryTuChoiDuyet = queryTuChoiDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryTuChoiDuyet = queryTuChoiDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryTuChoiDuyet = queryTuChoiDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryTuChoiDuyet = queryTuChoiDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryTuChoiDuyet = queryTuChoiDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
            }
            return queryTuChoiDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DsLinhDuocPhamGridVo> DaDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DsLinhDuocPhamGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                LinhVeKhoId = s.KhoXuatId,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đã duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDaDuyet = queryDaDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
            }
            return queryDaDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        #region export exel
        public virtual byte[] ExportDanhSachLayDuTruLinh(ICollection<DsLinhDuocPhamGridVo> datalinhs)
        {
            var queryInfo = new DsLinhDuocPhamGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DsLinhDuocPhamGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH YÊU CẦU LĨNH DƯỢC PHẨM");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;
                    worksheet.Column(16).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH YÊU CẦU LĨNH DƯỢC PHẨM".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: ";/*+ string.Join(", ", arrTrangThai);*/
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "Mã PL" }, { "B", "Loại" }, { "C", "Lĩnh từ kho" } , { "D", "Lĩnh về kho" },
                                    { "E", "Người yêu càu" }, { "F", "Ngày yêu cầu" },{ "G", "Tình Trạng" },{ "H", "Người duyệt" },{ "I", "Ngày duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<DsLinhDuocPhamGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var yeuCauLinhDuocPham in datalinhs)
                    {
                        manager.CurrentObject = yeuCauLinhDuocPham;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = yeuCauLinhDuocPham.MaPL;
                        worksheet.Cells["B" + index].Value = yeuCauLinhDuocPham.Loai;
                        worksheet.Cells["C" + index].Value = yeuCauLinhDuocPham.LinhTuKho;
                        worksheet.Cells["D" + index].Value = yeuCauLinhDuocPham.LinhVeKho;
                        worksheet.Cells["E" + index].Value = yeuCauLinhDuocPham.NguoiYeuCau;
                        worksheet.Cells["F" + index].Value = yeuCauLinhDuocPham.NgayYeuCau.ApplyFormatDateTimeSACH();
                        worksheet.Cells["G" + index].Value = yeuCauLinhDuocPham.TinhTrang;
                        worksheet.Cells["H" + index].Value = yeuCauLinhDuocPham.Nguoiduyet;
                        worksheet.Cells["I" + index].Value = yeuCauLinhDuocPham.NgayDuyet != null ? yeuCauLinhDuocPham.NgayDuyet.Value.ApplyFormatDateTimeSACH() : "";

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;
                        int sttItems = 1;
                        // lĩnh trực tiếp
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                        {
                            if (yeuCauLinhDuocPham.DuocDuyet != false)
                            {
                                using (var range = worksheet.Cells["B" + index + ":F" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);


                                    string[,] SetColumnLinhBenhNhans = { { "B", "#" }, { "C", "Mã TN" }, { "D", "Mã BN" }, { "E", "Họ Tên" }, { "F", "SL" } };

                                    for (int i = 0; i < SetColumnLinhBenhNhans.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLinhBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnLinhBenhNhans[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLinhBenhNhans[i, 1];
                                    }
                                    index++;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                foreach (var nhom in yeuCauLinhDuocPham.ListChildLinhBenhNhan)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.MaYeuCauTiepNhan;
                                    worksheet.Cells["D" + index].Value = nhom.MaBenhNhan;
                                    worksheet.Cells["E" + index].Value = nhom.HoTen;
                                    worksheet.Cells["F" + index].Value = nhom.SoLuong + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    using (var range = worksheet.Cells["C" + index + ":P" + index])
                                    {
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column A to K
                                        string[,] SetColumnDichVus = { { "C" , "#" },{ "D" , "Tên Dược Phẩm" }, { "E", "Nồng Độ/Hàm Lượng" }, { "F", "Hoạt Chất" } , { "G", "ĐD" },
                                    { "H", "ĐVT" }, { "I", "Hãng SX" }, { "J", "Nước SX" },{ "K", "DV Khám" },{ "L", "BS Kê Toa" },
                                    { "M", "Ngày Kê" },{ "N", "Ngày Điều Trị" },{ "O", "SL Tồn" },{ "P", "SL Yêu Cầu" }};

                                        for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                        {
                                            var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                            range.Worksheet.Cells[setColumn].Merge = true;
                                            range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                        }

                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    var sttDichVu = 1;
                                    if (nhom.DuocDuyet != false)
                                    {
                                        var duocPham = nhom.ListChildChildLinhBenhNhan
                                                                                            .Select(s => new DSLinhDuocPhamChildTuGridVo
                                                                                            {
                                                                                                TenDuocPham = s.TenDuocPham,
                                                                                                NongDoHamLuong = s.NongDoHamLuong,
                                                                                                HoatChat = s.HoatChat,
                                                                                                DuongDung = s.DuongDung,
                                                                                                DonViTinh = s.DonViTinh,
                                                                                                HangSanXuat = s.HangSanXuat,
                                                                                                NuocSanXuat = s.NuocSanXuat,
                                                                                                DichVuKham = s.DichVuKham,
                                                                                                BacSiKeToa = s.BacSiKeToa,
                                                                                                NgayKe = s.NgayKe,
                                                                                                SoLuongTon = s.SoLuongTon,
                                                                                                SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                                NgayDieuTri = s.NgayDieuTri
                                                                                            }).ToList();
                                        index++;
                                        foreach (var d in duocPham) // bhyt
                                        {
                                            worksheet.Cells["C" + index].Value = sttDichVu++;
                                            worksheet.Cells["D" + index].Value = d.TenDuocPham;
                                            worksheet.Cells["E" + index].Value = d.NongDoHamLuong;
                                            worksheet.Cells["F" + index].Value = d.HoatChat;
                                            worksheet.Cells["G" + index].Value = d.DuongDung;
                                            worksheet.Cells["H" + index].Value = d.DonViTinh;
                                            worksheet.Cells["I" + index].Value = d.HangSanXuat;
                                            worksheet.Cells["J" + index].Value = d.NuocSanXuat;
                                            worksheet.Cells["K" + index].Value = d.DichVuKham;
                                            worksheet.Cells["L" + index].Value = d.BacSiKeToa;
                                            worksheet.Cells["M" + index].Value = d.NgayKetString;
                                            worksheet.Cells["N" + index].Value = d.NgayDieuTriString;
                                            worksheet.Cells["O" + index].Value = d.SoLuongTon + "";
                                            worksheet.Cells["p" + index].Value = d.SoLuongYeuCau + "";

                                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                            {
                                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                            }

                                            index++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var duocPham = yeuCauLinhDuocPham.ListChildLinhBenhNhan
                                                                                     .Select(s => new DSLinhDuocPhamChildTuGridVo
                                                                                     {
                                                                                         TenDuocPham = s.TenDuocPham,
                                                                                         NongDoHamLuong = s.NongDoHamLuong,
                                                                                         HoatChat = s.HoatChat,
                                                                                         DuongDung = s.DuongDung,
                                                                                         DonViTinh = s.DonViTinh,
                                                                                         HangSanXuat = s.HangSanXuat,
                                                                                         NuocSanXuat = s.NuocSanXuat,
                                                                                         DichVuKham = s.DichVuKham,
                                                                                         BacSiKeToa = s.BacSiKeToa,
                                                                                         NgayKe = s.NgayKe,
                                                                                         SoLuongTon = s.SoLuongTon,
                                                                                         SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                         NgayDieuTri = s.NgayDieuTri
                                                                                     }).ToList();

                                using (var range = worksheet.Cells["B" + index + ":O" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                    //Set column A to K
                                    string[,] SetColumnDichVus = { { "B" , "#" },{ "C" , "Tên Dược Phẩm" }, { "D", "Nồng Độ/Hàm Lượng" }, { "E", "Hoạt Chất" } , { "F", "ĐD" },
                                    { "G", "ĐVT" }, { "H", "Hãng SX" }, { "I", "Nước SX" },{ "J", "DV Khám" },{ "K", "BS Kê Toa" },
                                    { "L", "Ngày Điều Trị" },
                                    { "M", "Ngày Kê" },{ "N", "SL Tồn" },{ "O", "SL Yêu Cầu" }};

                                    for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }

                                var sttDichVu = 1;
                                if (duocPham.Count() > 0)
                                {
                                    foreach (var d in duocPham)
                                    {
                                        worksheet.Cells["B" + index].Value = sttDichVu++;
                                        worksheet.Cells["C" + index].Value = d.TenDuocPham;
                                        worksheet.Cells["D" + index].Value = d.NongDoHamLuong;
                                        worksheet.Cells["E" + index].Value = d.HoatChat;
                                        worksheet.Cells["F" + index].Value = d.DuongDung;
                                        worksheet.Cells["G" + index].Value = d.DonViTinh;
                                        worksheet.Cells["H" + index].Value = d.HangSanXuat;
                                        worksheet.Cells["I" + index].Value = d.NuocSanXuat;
                                        worksheet.Cells["J" + index].Value = d.DichVuKham;
                                        worksheet.Cells["K" + index].Value = d.BacSiKeToa;
                                        worksheet.Cells["L" + index].Value = d.NgayDieuTriString;
                                        worksheet.Cells["M" + index].Value = d.NgayKetString;
                                        worksheet.Cells["N" + index].Value = d.SoLuongTon;
                                        worksheet.Cells["O" + index].Value = d.SoLuongYeuCau;

                                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                        {
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        }

                                        index++;
                                    }
                                }
                            }
                        }



                        // Lĩnh bù
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            using (var range = worksheet.Cells["B" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhBus = {  { "B", "#" },{ "C", "Tên Dược Phẩm" }, { "D", "Đơn Vị Tính" }, { "E", "Hãng SX" }, { "F", "Nước SX" } ,
                                    { "G", "SL Tồn" },{ "H", "SL Đã Bù" },{ "I", "SL Cần Bù"},{ "J", "SL Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhBus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhBus[i, 0]).ToString() + index + ":" + (SetColumnLinhBus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhBus[i, 1];
                                }
                                index++;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            var duocpham = yeuCauLinhDuocPham.ListChildLinhBu
                                .Select(s => new YeuCauLinhDuocPhamBuGridVo
                                {
                                    TenVatTu = s.TenVatTu,
                                    DonViTinh = s.DonViTinh,
                                    HangSanXuat = s.HangSanXuat,
                                    NuocSanXuat = s.NuocSanXuat,
                                    SoLuongTon = s.SoLuongTon,
                                    SLDaLinh = s.SLDaLinh,
                                    SoLuongCanBu = s.SoLuongCanBu,
                                    SoLuongYeuCau = s.SoLuongYeuCau,
                                    ListChildChildLinhBu = s.ListChildChildLinhBu
                                }).ToList();
                          
                            if (duocpham.Count() > 0)
                            {
                                foreach (var nhom in duocpham)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongTon + "";
                                    worksheet.Cells["H" + index].Value = nhom.SLDaLinh + "";
                                    worksheet.Cells["I" + index].Value = nhom.SoLuongCanBu + "";
                                    worksheet.Cells["J" + index].Value = nhom.SoLuongYeuCau + "";


                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                    using (var range = worksheet.Cells["C" + index + ":M" + index])
                                    {
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column 
                                        string[,] SetColumnDichVus = { { "C" , "Mã Tiếp Nhận" },{ "D" , "Mã Tiếp Nhận" }, { "E", "Mã Người Bệnh" }, { "F", "Họ Tên" } , { "G", "Dịch Vụ Khám" },
                                    { "H", "Bác Sỹ Kê Toa" },{ "I", "Ngày điều trị" }, { "J", "Ngày Kê" }, { "K", "Số Lượng Đã Bù" },{ "L", "Số Lượng Cần Bù" },{ "M", "SL yêu cầu" }};

                                        for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                        {
                                            var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                            range.Worksheet.Cells[setColumn].Merge = true;
                                            range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                        }

                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;

                                    var sttDichVu = 1;
                                    foreach (var dichVu in nhom.ListChildChildLinhBu)
                                    {
                                        worksheet.Cells["C" + index].Value = sttDichVu++;
                                        worksheet.Cells["D" + index].Value = dichVu.MaTN;
                                        worksheet.Cells["E" + index].Value = dichVu.MaBN;
                                        worksheet.Cells["F" + index].Value = dichVu.HoTen;
                                        worksheet.Cells["G" + index].Value = dichVu.DVKham;
                                        worksheet.Cells["H" + index].Value = dichVu.BSKeToa;
                                        worksheet.Cells["I" + index].Value = dichVu.NgayDieuTriString;
                                        worksheet.Cells["J" + index].Value = dichVu.NgayKe;
                                        worksheet.Cells["K" + index].Value = dichVu.SLDaLinh + "";
                                        worksheet.Cells["L" + index].Value = dichVu.DVKham;
                                        worksheet.Cells["M" + index].Value = dichVu.SL + "";

                                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                        {
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        }

                                        index++;
                                    }
                                }
                            }
                        }
                        // lĩnh thường
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru)
                        {

                            using (var range = worksheet.Cells["B" + index + ":G" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhDuTrus = { { "B", "#" },{ "C", "Tên Dược Phẩm" }, { "d", "Đơn Vị Tính" }, { "E", "Hãng SX" } ,
                                { "F", "Nước SX" },{ "G", "Số Lượng Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhDuTrus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhDuTrus[i, 0]).ToString() + index + ":" + (SetColumnLinhDuTrus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhDuTrus[i, 1];
                                }
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            var dutruthuongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == true)
                               .Select(k => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   TenVatTu = k.TenVatTu,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            var dutruthuongKhongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == false)
                               .Select(k => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   TenVatTu = k.TenVatTu,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            if (dutruthuongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }
                            if (dutruthuongKhongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongKhongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }

                        }

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        public virtual byte[] ExportDanhSachDuyetLayDuTruLinh(ICollection<DsLinhDuocPhamGridVo> datalinhs)
        {
            var queryInfo = new DsLinhDuocPhamGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DsLinhDuocPhamGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH DUYỆT LĨNH DƯỢC PHẨM");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;
                    worksheet.Column(16).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH DUYỆT LĨNH DƯỢC PHẨM".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: ";/*+ string.Join(", ", arrTrangThai);*/
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "Mã PL" }, { "B", "Loại" }, { "C", "Lĩnh từ kho" } , { "D", "Lĩnh về kho" },
                                    { "E", "Người yêu càu" }, { "F", "Ngày yêu cầu" },{ "G", "Tình Trạng" },{ "H", "Người duyệt" },{ "I", "Ngày duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<DsLinhDuocPhamGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var yeuCauLinhDuocPham in datalinhs)
                    {
                        manager.CurrentObject = yeuCauLinhDuocPham;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = yeuCauLinhDuocPham.MaPL;
                        worksheet.Cells["B" + index].Value = yeuCauLinhDuocPham.Loai;
                        worksheet.Cells["C" + index].Value = yeuCauLinhDuocPham.LinhTuKho;
                        worksheet.Cells["D" + index].Value = yeuCauLinhDuocPham.LinhVeKho;
                        worksheet.Cells["E" + index].Value = yeuCauLinhDuocPham.NguoiYeuCau;
                        worksheet.Cells["F" + index].Value = yeuCauLinhDuocPham.NgayYeuCau.ApplyFormatDateTimeSACH();
                        worksheet.Cells["G" + index].Value = yeuCauLinhDuocPham.TinhTrang;
                        worksheet.Cells["H" + index].Value = yeuCauLinhDuocPham.Nguoiduyet;
                        worksheet.Cells["I" + index].Value = yeuCauLinhDuocPham.NgayDuyet != null ? yeuCauLinhDuocPham.NgayDuyet.Value.ApplyFormatDateTimeSACH() : "";

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;
                        int sttItems = 1;
                        // lĩnh trực tiếp
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                        {
                            if (yeuCauLinhDuocPham.DuocDuyet != false)
                            {
                                using (var range = worksheet.Cells["B" + index + ":F" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);


                                    string[,] SetColumnLinhBenhNhans = { { "B", "#" }, { "C", "Mã TN" }, { "D", "Mã BN" }, { "E", "Họ Tên" }, { "F", "SL" } };

                                    for (int i = 0; i < SetColumnLinhBenhNhans.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLinhBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnLinhBenhNhans[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLinhBenhNhans[i, 1];
                                    }
                                    index++;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                foreach (var nhom in yeuCauLinhDuocPham.ListChildLinhBenhNhan)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.MaYeuCauTiepNhan;
                                    worksheet.Cells["D" + index].Value = nhom.MaBenhNhan;
                                    worksheet.Cells["E" + index].Value = nhom.HoTen;
                                    worksheet.Cells["F" + index].Value = nhom.SoLuong + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                    using (var range = worksheet.Cells["C" + index + ":P" + index])
                                    {
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":P" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column A to K
                                        string[,] SetColumnDichVus = { { "C" , "#" },{ "D" , "Tên Dược Phẩm" }, { "E", "Nồng Độ/Hàm Lượng" }, { "F", "Hoạt Chất" } , { "G", "ĐD" },
                                    { "H", "ĐVT" }, { "I", "Hãng SX" }, { "J", "Nước SX" },{ "K", "DV Khám" },{ "L", "BS Kê Toa" },
                                    { "M", "Ngày Kê" },{ "N", "Ngày Điều Trị" },{ "O", "SL Tồn" },{ "P", "SL Yêu Cầu" }};

                                        for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                        {
                                            var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                            range.Worksheet.Cells[setColumn].Merge = true;
                                            range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                        }

                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    var sttDichVu = 1;
                                    if (nhom.DuocDuyet != false)
                                    {
                                        var duocPham = nhom.ListChildChildLinhBenhNhan
                                                                                     .Select(s => new DSLinhDuocPhamChildTuGridVo
                                                                                     {
                                                                                         TenDuocPham = s.TenDuocPham,
                                                                                         NongDoHamLuong = s.NongDoHamLuong,
                                                                                         HoatChat = s.HoatChat,
                                                                                         DuongDung = s.DuongDung,
                                                                                         DonViTinh = s.DonViTinh,
                                                                                         HangSanXuat = s.HangSanXuat,
                                                                                         NuocSanXuat = s.NuocSanXuat,
                                                                                         DichVuKham = s.DichVuKham,
                                                                                         BacSiKeToa = s.BacSiKeToa,
                                                                                         NgayKe = s.NgayKe,
                                                                                         SoLuongTon = s.SoLuongTon,
                                                                                         SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                         NgayDieuTri = s.NgayDieuTri
                                                                                     }).ToList();
                                        if (duocPham.Count() > 0)
                                        {
                                            foreach (var nhomBHYT in duocPham) // bhyt
                                            {
                                                worksheet.Cells["C" + index].Value = sttDichVu++;
                                                worksheet.Cells["D" + index].Value = nhomBHYT.TenDuocPham;
                                                worksheet.Cells["E" + index].Value = nhomBHYT.NongDoHamLuong;
                                                worksheet.Cells["F" + index].Value = nhomBHYT.HoatChat;
                                                worksheet.Cells["G" + index].Value = nhomBHYT.DuongDung;
                                                worksheet.Cells["H" + index].Value = nhomBHYT.DonViTinh;
                                                worksheet.Cells["I" + index].Value = nhomBHYT.HangSanXuat;
                                                worksheet.Cells["J" + index].Value = nhomBHYT.NuocSanXuat;
                                                worksheet.Cells["K" + index].Value = nhomBHYT.DichVuKham;
                                                worksheet.Cells["L" + index].Value = nhomBHYT.BacSiKeToa;
                                                worksheet.Cells["M" + index].Value = nhomBHYT.NgayKetString;
                                                worksheet.Cells["N" + index].Value = nhomBHYT.NgayDieuTriString;
                                                worksheet.Cells["O" + index].Value = nhomBHYT.SoLuongTon + "";
                                                worksheet.Cells["p" + index].Value = nhomBHYT.SoLuongYeuCau + "";

                                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                                {
                                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                }

                                                index++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var duocPham = yeuCauLinhDuocPham.ListChildLinhBenhNhan
                                                                                     .Select(s => new DSLinhDuocPhamChildTuGridVo
                                                                                     {
                                                                                         TenDuocPham = s.TenDuocPham,
                                                                                         NongDoHamLuong = s.NongDoHamLuong,
                                                                                         HoatChat = s.HoatChat,
                                                                                         DuongDung = s.DuongDung,
                                                                                         DonViTinh = s.DonViTinh,
                                                                                         HangSanXuat = s.HangSanXuat,
                                                                                         NuocSanXuat = s.NuocSanXuat,
                                                                                         DichVuKham = s.DichVuKham,
                                                                                         BacSiKeToa = s.BacSiKeToa,
                                                                                         NgayKe = s.NgayKe,
                                                                                         SoLuongTon = s.SoLuongTon,
                                                                                         SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                         NgayDieuTri = s.NgayDieuTri
                                                                                     }).ToList();
                                using (var range = worksheet.Cells["B" + index + ":O" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":O" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                    //Set column A to K
                                    string[,] SetColumnDichVus = { { "B" , "#" },{ "C" , "Tên Dược Phẩm" }, { "D", "Nồng Độ/Hàm Lượng" }, { "E", "Hoạt Chất" } , { "F", "ĐD" },
                                    { "G", "ĐVT" }, { "H", "Hãng SX" }, { "I", "Nước SX" },{ "J", "DV Khám" },{ "K", "BS Kê Toa" },
                                    { "L", "Ngày Điều Trị" },
                                    { "M", "Ngày Kê" },{ "N", "SL Tồn" },{ "O", "SL Yêu Cầu" }};

                                    for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                var sttDichVu = 1;
                                if (duocPham.Count() > 0)
                                {
                                    using (var range = worksheet.Cells["B" + index + ":O" + index])
                                    {
                                        range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                        //Set column A to K
                                        string[,] SetColumnLoaiDuocPham = { { "B", "Dược Phẩm BHYT" } };

                                        for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                        {
                                            var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                            range.Worksheet.Cells[setColumn].Merge = true;
                                            range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                        }

                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                    foreach (var nhomBHYT in duocPham) // bhyt
                                    {
                                        worksheet.Cells["B" + index].Value = sttDichVu++;
                                        worksheet.Cells["C" + index].Value = nhomBHYT.TenDuocPham;
                                        worksheet.Cells["D" + index].Value = nhomBHYT.NongDoHamLuong;
                                        worksheet.Cells["E" + index].Value = nhomBHYT.HoatChat;
                                        worksheet.Cells["F" + index].Value = nhomBHYT.DuongDung;
                                        worksheet.Cells["G" + index].Value = nhomBHYT.DonViTinh;
                                        worksheet.Cells["H" + index].Value = nhomBHYT.HangSanXuat;
                                        worksheet.Cells["I" + index].Value = nhomBHYT.NuocSanXuat;
                                        worksheet.Cells["J" + index].Value = nhomBHYT.DichVuKham;
                                        worksheet.Cells["K" + index].Value = nhomBHYT.BacSiKeToa;
                                        worksheet.Cells["L" + index].Value = nhomBHYT.NgayDieuTriString;
                                        worksheet.Cells["M" + index].Value = nhomBHYT.NgayKetString;
                                        worksheet.Cells["N" + index].Value = nhomBHYT.SoLuongTon;
                                        worksheet.Cells["O" + index].Value = nhomBHYT.SoLuongYeuCau;

                                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                        {
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        }

                                        index++;
                                    }
                                }
                            }
                            index++;
                        }
                        // Lĩnh bù
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            using (var range = worksheet.Cells["B" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhBus = {  { "B", "#" },{ "C", "Tên Dược Phẩm" }, { "D", "Đơn Vị Tính" }, { "E", "Hãng SX" }, { "F", "Nước SX" } ,
                                    { "G", "SL Tồn" },{ "H", "SL Đã Bù" },{ "I", "SL Cần Bù"},{ "J", "SL Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhBus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhBus[i, 0]).ToString() + index + ":" + (SetColumnLinhBus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhBus[i, 1];
                                }
                                index++;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            var duocPham = yeuCauLinhDuocPham.ListChildLinhBu
                                .Select(s => new YeuCauLinhDuocPhamBuGridVo
                                {
                                    TenVatTu = s.TenVatTu,
                                    DonViTinh = s.DonViTinh,
                                    HangSanXuat = s.HangSanXuat,
                                    NuocSanXuat = s.NuocSanXuat,
                                    SoLuongTon = s.SoLuongTon,
                                    SLDaLinh = s.SLDaLinh,
                                    SoLuongCanBu = s.SoLuongCanBu,
                                    SoLuongYeuCau = s.SoLuongYeuCau,
                                    ListChildChildLinhBu = s.ListChildChildLinhBu
                                }).ToList();
                            if (duocPham.Count() > 0)
                            {
                                foreach (var nhom in duocPham)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongTon + "";
                                    worksheet.Cells["H" + index].Value = nhom.SLDaLinh + "";
                                    worksheet.Cells["I" + index].Value = nhom.SoLuongCanBu + "";
                                    worksheet.Cells["J" + index].Value = nhom.SoLuongYeuCau + "";


                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                    using (var range = worksheet.Cells["C" + index + ":M" + index])
                                    {
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":L" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column 
                                        string[,] SetColumnDichVus = { { "C" , "Mã Tiếp Nhận" },{ "D" , "Mã Tiếp Nhận" }, { "E", "Mã Người Bệnh" }, { "F", "Họ Tên" } , { "G", "Dịch Vụ Khám" },
                                    { "H", "Bác Sỹ Kê Toa" }, { "I", "Ngày Điều Trị" },{ "J", "Ngày Kê" }, { "K", "Số Lượng Đã Bù" },{ "L", "Số Lượng Cần Bù" },{ "M", "SL Yêu Cầu" }};

                                        for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                        {
                                            var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                            range.Worksheet.Cells[setColumn].Merge = true;
                                            range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                        }

                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;

                                    var sttDichVu = 1;
                                    foreach (var dichVu in nhom.ListChildChildLinhBu)
                                    {
                                        worksheet.Cells["C" + index].Value = sttDichVu++;
                                        worksheet.Cells["D" + index].Value = dichVu.MaTN;
                                        worksheet.Cells["E" + index].Value = dichVu.MaBN;
                                        worksheet.Cells["F" + index].Value = dichVu.HoTen;
                                        worksheet.Cells["G" + index].Value = dichVu.DVKham;
                                        worksheet.Cells["H" + index].Value = dichVu.BSKeToa;
                                        worksheet.Cells["I" + index].Value = dichVu.NgayDieuTriString;
                                        worksheet.Cells["J" + index].Value = dichVu.NgayKe;
                                        worksheet.Cells["K" + index].Value = dichVu.SLDaLinh + "";
                                        worksheet.Cells["L" + index].Value = dichVu.DVKham;
                                        worksheet.Cells["M" + index].Value = dichVu.SL + "";

                                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                        {
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        }

                                        index++;
                                    }
                                }
                            }
                        }
                        // lĩnh thường
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru)
                        {

                            using (var range = worksheet.Cells["B" + index + ":G" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhDuTrus = { { "B", "#" },{ "C", "Tên Dược Phẩm" }, { "d", "Đơn Vị Tính" }, { "E", "Hãng SX" } ,
                                { "F", "Nước SX" },{ "G", "Số Lượng Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhDuTrus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhDuTrus[i, 0]).ToString() + index + ":" + (SetColumnLinhDuTrus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhDuTrus[i, 1];
                                }
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            var dutruthuongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == true)
                               .Select(k => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   TenVatTu = k.TenVatTu,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            var dutruthuongKhongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == false)
                               .Select(k => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   TenVatTu = k.TenVatTu,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            if (dutruthuongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }
                            if (dutruthuongKhongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongKhongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }

                        }

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #region Ds duyệt DuocPham child
        public async Task<GridDataSource> GetDataDSDuyetLinhDuocPhamChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]); // 1 loai phieu linh
            int trangThai = 0;

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                BuildDefaultSortExpression(queryInfo);
                var trangThaiLinhBu = _yeuCauLinhDuocPhamRepository.TableNoTracking
                                      .Where(d => d.Id == long.Parse(queryString[0]))
                                      .Select(d => d.DuocDuyet).FirstOrDefault();
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            )
                            .Select(s => new YeuCauLinhDuocPhamBuGridVo
                            {
                                Id = s.Id,
                                YeuCauLinhDuocPhamId = s.YeuCauLinhDuocPhamId,
                                DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                                TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                                DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                LaBHYT = s.LaDuocPhamBHYT,
                                Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu.MathRoundNumber(2) : 0,
                                SLDaLinh = s.YeuCauLinhDuocPhamId != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                                LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                LinhVeKhoId = long.Parse(queryString[2]),
                                NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                                DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                                DaDuyet = s.YeuCauLinhDuocPhamId != null ? s.YeuCauLinhDuocPham.DuocDuyet : null,
                                SoLuongYeuCauDaDuyet = s.SoLuong // trường hợp cho đã duyệt

                            })
                            .GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                              .Select(item => new YeuCauLinhDuocPhamBuGridVo()
                              {
                                  Id = item.First().Id,
                                  DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                  LaBHYT = item.First().LaBHYT,
                                  TenVatTu = item.First().TenVatTu,
                                  Nhom = item.First().Nhom,
                                  DonViTinh = item.First().DonViTinh,
                                  HangSanXuat = item.First().HangSanXuat,
                                  NuocSanXuat = item.First().NuocSanXuat,
                                  SoLuongCanBu = item.Sum(x => x.SoLuongCanBu.MathRoundNumber(2)),
                                  LinhVeKhoId = long.Parse(queryString[2]),
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  NongDoHamLuong = item.FirstOrDefault().NongDoHamLuong,
                                  SLDaLinh = item.Sum(x => x.SLDaLinh),
                                  HoatChat = item.First().HoatChat,
                                  DuongDung = item.First().DuongDung,
                                  DaDuyet = item.First().DaDuyet,
                                  SoLuongYeuCauDaDuyet = item.Sum(s => s.SoLuongYeuCauDaDuyet)
                              })
                              .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var DuocPhamLinhBuGridVos = query.ToList();
                if(trangThaiLinhBu == null) {

                    var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                       && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                    var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                    result = result.Select(o =>
                    {
                        //o.SoLuongTon = lstVatTuBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        //o.SoLuongYeuCau = o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon : o.SoLuongCanBu;

                        o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                             : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                        return o;
                    });
                    result = result.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();

                    var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                    var queryTask = result.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
                else
                {
                    query = query.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                    var queryTask = query.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
                
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                        HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                        NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                        DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        SLTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == s.DuocPhamBenhVienId && o.LaDuocPhamBHYT == s.LaDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == s.YeuCauLinhDuocPham.KhoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber(1),
                        DuocDuyet = s.YeuCauLinhDuocPham != null ? s.YeuCauLinhDuocPham.DuocDuyet : null,
                        LaBHYT = s.LaDuocPhamBHYT
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(d=>d.LaBHYT).ThenBy(d=>d.TenVatTu).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                BuildDefaultSortExpression(queryInfo);
                if (queryString[3] == "true" || string.IsNullOrEmpty(queryString[1]))
                {
                    trangThai = 0;
                }// 3 Duoc duyet : dang duyet , đa duyet
                if (queryString[3] == "false" || queryString[3] == "False")
                {
                    trangThai = 1;
                } // tu choi duyet
                if (trangThai == 1)
                {
                    var yeuCauLinhDuocPham =
                    await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    IQueryable<DSLinhDuocPhamChildTuGridVo> queryable = null;
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DuocDuyet = item.YeuCauLinhDuocPham.DuocDuyet,
                           DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                           BacSyKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : ""
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.SoLuongYeuCau
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.LaDuocPhamBHYT == item.First().LaBHYT
                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           Nhom = item.First().Nhom,
                           DuocDuyet = item.First().DuocDuyet,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
                    var queryTask = queryable.Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                else
                {
                    var yeuCauLinhId = long.Parse(queryString[0]);
                    var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhId);

                    // cập nhật 29/10/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhDuocPhamCHiTiet
                    IQueryable<DSLinhDuocPhamChildTuGridVo> query = null;

                    if (yeuCauLinhDuocPham.DuocDuyet != true)
                    {
                        query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                    && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && o.YeuCauTiepNhan.BenhNhanId != null)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                            HoTen = s.YeuCauTiepNhan.HoTen,
                            SoLuong = s.SoLuong,
                            DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                            NgayKe = s.ThoiDiemChiDinh,
                            DuocDuyet = s.YeuCauLinhDuocPham.DuocDuyet,
                            KhoLinhId = s.KhoLinhId
                        })
                        .GroupBy(x => new
                        {
                            x.BenhNhanId,
                            x.MaYeuCauTiepNhan,
                            x.HoTen,

                        })
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                            MaBenhNhan = s.First().MaBenhNhan,
                            HoTen = s.First().HoTen,
                            SoLuong = s.Sum(a => a.SoLuong),
                            DichVuKham = s.First().DichVuKham,
                            BacSiKeToa = s.First().BacSiKeToa,
                            NgayKe = s.First().NgayKe,
                            LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                            Id = yeuCauLinhId,
                            DuocDuyet = s.First().DuocDuyet,
                            KhoLinhId = s.First().KhoLinhId
                        });
                    }
                    else
                    {
                        query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhId
                                        && x.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                                        && x.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                        && x.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhanId != null)
                        .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaBenhNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                            HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                            SoLuong = s.SoLuong,
                            DichVuKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null 
                                ? s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu 
                                : (s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                            NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh,
                            DuocDuyet = s.YeuCauLinhDuocPham.DuocDuyet,
                            KhoLinhId = s.YeuCauDuocPhamBenhVien.KhoLinhId
                        })
                        .GroupBy(x => new
                        {
                            x.BenhNhanId,
                            x.MaYeuCauTiepNhan,
                            x.HoTen,

                        })
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                            MaBenhNhan = s.First().MaBenhNhan,
                            HoTen = s.First().HoTen,
                            SoLuong = s.Sum(a => a.SoLuong),
                            DichVuKham = s.First().DichVuKham,
                            BacSiKeToa = s.First().BacSiKeToa,
                            NgayKe = s.First().NgayKe,
                            LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                            Id = yeuCauLinhId,
                            DuocDuyet = s.First().DuocDuyet,
                            KhoLinhId = s.First().KhoLinhId
                        });
                    }

                    if (queryString[4] != null && queryString[4] != "" && queryString[4] !="true")
                    {
                        if (query.Any())
                        {
                            var list = DataChoGoi((long)query.First().KhoLinhId).AsQueryable();
                            query = query.Union(list);
                        }

                    }
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }

            }
            return null;
        }
        public async Task<GridDataSource> GetDataDSDuyetLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                var trangThaiDuyet = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPham.DuocDuyet).FirstOrDefault();
                var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();

                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                             && p.DuocPhamBenhVienId == long.Parse(queryString[2])
                             && p.LaDuocPhamBHYT == bool.Parse(queryString[3])
                             && p.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                             && p.YeuCauLinhDuocPham.DuocDuyet == trangThaiDuyet
                            )
                .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                .Select(s => new DuocPhamLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                    SL = trangThaiDuyet == true ? s.SoLuong : s.SoLuongCanBu,
                    DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0,
                    SLDanhSachDuyet = s.SoLuong.MathRoundNumber(2),
                    SLCanBu = s.SoLuongCanBu,
                    NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                // 5 6
                BuildDefaultSortExpression(queryInfo);
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true") // trạng thái chờ duyệt, đã duyệt
                {
                    kieuIn = 0;
                }

                if (queryString[6] == "false" || queryString[6] == "False") // trạng thái từ chối
                {
                    kieuIn = 1;
                }
                var yeuCauLinh =
                    _yeuCauLinhDuocPhamRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));
                List<DSLinhDuocPhamChildTuGridVo> queryable = new List<DSLinhDuocPhamChildTuGridVo>();
                if (kieuIn == 0)
                {
                    if (yeuCauLinh == true)
                    {
                        var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));

                        // cập nhật 29/10/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhDuocPhamCHiTiet
                        if (yeuCauLinhDuocPham.DuocDuyet != true)
                        {
                            queryable = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                           .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                       && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                       && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                       && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                           .Select(item => new DSLinhDuocPhamChildTuGridVo()
                           {
                               YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                               DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                               LaBHYT = item.LaDuocPhamBHYT,
                               TenDuocPham = item.Ten,
                               NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                               HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                               DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                               DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                               HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                               NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                               SoLuongYeuCau = item.SoLuong,
                               Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                               DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                               BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                               BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                               NgayKe = item.ThoiDiemChiDinh,
                               NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                               NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.NoiTruPhieuDieuTriId != null)? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                           })
                           .GroupBy(x => new
                           {
                               x.YeuCauLinhDuocPhamId,
                               x.DuocPhamBenhVienId,
                               x.LaBHYT,
                               x.Nhom,
                               x.NongDoHamLuong,
                               x.HoatChat,
                               x.DuongDung,
                               x.DonViTinh,
                               x.HangSanXuat,
                               x.NuocSanXuat,
                               x.DichVuKham,
                               x.BacSiKeToa,
                           //x.NgayKe
                            })
                           .Select(item => new DSLinhDuocPhamChildTuGridVo()
                           {
                               YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                               DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                               LaBHYT = item.First().LaBHYT,
                               TenDuocPham = item.First().TenDuocPham,
                               NongDoHamLuong = item.First().NongDoHamLuong,
                               HoatChat = item.First().HoatChat,
                               DuongDung = item.First().DuongDung,
                               DonViTinh = item.First().DonViTinh,
                               HangSanXuat = item.First().HangSanXuat,
                               NuocSanXuat = item.First().NuocSanXuat,
                               SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                               SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                   .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                               && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                               && x.LaDuocPhamBHYT == item.First().LaBHYT
                                               && x.NhapKhoDuocPhams.DaHet != true
                                               && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               DichVuKham = item.First().DichVuKham,
                               BacSiKeToa = item.First().BacSiKeToa,
                               BacSyKeToa = item.First().BacSyKeToa,
                               NgayKe = item.First().NgayKe,
                               Nhom = item.First().Nhom,
                               NgayDieuTri = item.First().NgayDieuTri,
                               NgayKetString = item.First().NgayKetString
                           })
                           .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        }
                        else
                        {
                            queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                            && x.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                            && x.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && x.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId == long.Parse(queryString[5]))
                                .Select(item => new DSLinhDuocPhamChildTuGridVo()
                                {
                                    YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                                    DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                                    LaBHYT = item.LaDuocPhamBHYT,
                                    TenDuocPham = item.YeuCauDuocPhamBenhVien.Ten,
                                    NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                                    HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                                    DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                                    DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                    HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                    NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                    SoLuongYeuCau = item.SoLuong,
                                    Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                    DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null 
                                        ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu 
                                        : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                                    BacSiKeToa = item.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                    BacSyKeToa = item.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                    NgayKe = item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh,
                                    NgayKetString = item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                    NgayDieuTri = (item.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTriId != null)
                                        ? item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                                })
                               .GroupBy(x => new
                               {
                                   x.YeuCauLinhDuocPhamId,
                                   x.DuocPhamBenhVienId,
                                   x.LaBHYT,
                                   x.Nhom,
                                   x.NongDoHamLuong,
                                   x.HoatChat,
                                   x.DuongDung,
                                   x.DonViTinh,
                                   x.HangSanXuat,
                                   x.NuocSanXuat,
                                   x.DichVuKham,
                                   x.BacSiKeToa,
                                   //x.NgayKe
                               })
                               .Select(item => new DSLinhDuocPhamChildTuGridVo()
                               {
                                   YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                                   DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                   LaBHYT = item.First().LaBHYT,
                                   TenDuocPham = item.First().TenDuocPham,
                                   NongDoHamLuong = item.First().NongDoHamLuong,
                                   HoatChat = item.First().HoatChat,
                                   DuongDung = item.First().DuongDung,
                                   DonViTinh = item.First().DonViTinh,
                                   HangSanXuat = item.First().HangSanXuat,
                                   NuocSanXuat = item.First().NuocSanXuat,
                                   SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                                   SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                       .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                                   && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                                   && x.LaDuocPhamBHYT == item.First().LaBHYT
                                                   && x.NhapKhoDuocPhams.DaHet != true
                                                   && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   DichVuKham = item.First().DichVuKham,
                                   BacSiKeToa = item.First().BacSiKeToa,
                                   BacSyKeToa = item.First().BacSyKeToa,
                                   NgayKe = item.First().NgayKe,
                                   Nhom = item.First().Nhom,
                                   NgayDieuTri = item.First().NgayDieuTri,
                                   NgayKetString = item.First().NgayKetString
                               })
                               .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        }
                    }
                    else
                    {
                        long khoaId = 0;
                        var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
                        if (phongBenhVien != null)
                        {
                            khoaId = phongBenhVien.KhoaPhongId;
                        }

                        //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
                        var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
                        {
                            KeyId = (long)o.Id,
                            DisplayName = o.Ten
                        }).OrderBy(o => o.DisplayName).ToList();
                        var ques = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == null
                                   && x.KhoLinhId == long.Parse(queryString[7]) &&
                                   phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.NoiTruPhieuDieuTriId != null)? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                           ,
                           SoLuongTon = item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == item.KhoLinhId && nkct.LaDuocPhamBHYT == item.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat)
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           //x.NgayKetString
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString = item.First().NgayKetString
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        if (ques.Any())
                        {
                            queryable = queryable.Union(ques).ToList();
                        }
                    }
                }
                if (kieuIn == 1)
                {
                    var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                            NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                            HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                            DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                            DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                            NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                            DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                            BacSyKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : ""
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhDuocPhamId,
                            x.DuocPhamBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenDuocPham = item.First().TenDuocPham,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom,
                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            BacSyKeToa = item.First().BacSyKeToa,
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                }

                var dataOrderBy = queryable.AsQueryable();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSLinhDuocPhamChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;

            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {

                BuildDefaultSortExpression(queryInfo);
                var trangThaiLinhBu = _yeuCauLinhDuocPhamRepository.TableNoTracking
                                     .Where(d => d.Id == long.Parse(queryString[0]))
                                     .Select(d => d.DuocDuyet).FirstOrDefault();

                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                              //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.ChuaThucHien
                              && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            //&& (p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu == null || p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu < p.YeuCauDuocPhamBenhVien.SoLuong)
                            )
                            .Select(s => new YeuCauLinhDuocPhamBuGridVo
                            {
                                Id = s.Id,
                                YeuCauLinhDuocPhamId = s.YeuCauLinhDuocPhamId,
                                DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                                TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                                DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                LaBHYT = s.LaDuocPhamBHYT,
                                Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu : 0,
                                SLDaLinh = s.YeuCauDuocPhamBenhVien != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                                LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                LinhVeKhoId = long.Parse(queryString[2]),
                                DaDuyet = s.YeuCauLinhDuocPhamId != null ? s.YeuCauLinhDuocPham.DuocDuyet : null,
                            })
                            .GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                              .Select(item => new YeuCauLinhDuocPhamBuGridVo()
                              {
                                  DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                  LaBHYT = item.First().LaBHYT,
                                  TenVatTu = item.First().TenVatTu,
                                  Nhom = item.First().Nhom,
                                  DonViTinh = item.First().DonViTinh,
                                  HangSanXuat = item.First().HangSanXuat,
                                  NuocSanXuat = item.First().NuocSanXuat,
                                  SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                                  LinhVeKhoId = long.Parse(queryString[2]),
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  SLDaLinh = item.Sum(x => x.SLDaLinh),
                                  DaDuyet = item.First().DaDuyet
                              })
                              .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var DuocPhamLinhBuGridVos = query.ToList();

                //var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                //       && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                //var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                //result = result.Select(o =>
                //{
                //    //o.SoLuongTon = lstVatTuBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                //    //o.SoLuongYeuCau = o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon : o.SoLuongCanBu;

                //    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                //    o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                //                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                //    return o;
                //});
                //var dataOrderBy = result.AsQueryable().OrderBy(queryInfo.SortString);
                //var countTask = dataOrderBy.Count();

                //return new GridDataSource { TotalRowCount = countTask };
                if (trangThaiLinhBu == null)
                {

                    var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                       && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                    var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                    result = result.Select(o =>
                    {
                        //o.SoLuongTon = lstVatTuBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        //o.SoLuongYeuCau = o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon : o.SoLuongCanBu;

                        o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                             : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                        return o;
                    });
                    var dataOrderBy = result.AsQueryable().OrderBy(queryInfo.SortString);
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
                else
                {
                    var dataOrderBy = query.AsQueryable();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                        
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                int trangThai = 0;
                if (queryString[3] == "true" || string.IsNullOrEmpty(queryString[1]))
                {
                    trangThai = 0;
                }// 3 Duoc duyet : dang duyet , đa duyet
                if (queryString[3] == "false")
                {
                    trangThai = 1;
                } // tu choi duyet
                if (trangThai == 1)
                {
                    var yeuCauLinhDuocPham =
                    await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    IQueryable<DSLinhDuocPhamChildTuGridVo> queryable = null;
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.SoLuongYeuCau
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.LaDuocPhamBHYT == item.First().LaBHYT
                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           Nhom = item.First().Nhom
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                    var countTask = queryable.CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                else
                {
                    BuildDefaultSortExpression(queryInfo);
                    var yeuCauLinhId = long.Parse(queryString[0]);

                    var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                    && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && o.YeuCauTiepNhan.BenhNhanId != null)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                            HoTen = s.YeuCauTiepNhan.HoTen,
                            SoLuong = s.SoLuong,
                            DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                            BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                            NgayKe = s.ThoiDiemChiDinh,
                            DuocDuyet = s.YeuCauLinhDuocPham.DuocDuyet,
                            NgayDieuTri = (s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTriId != null) ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                            KhoLinhId = s.KhoLinhId
                        })
                        .GroupBy(x => new
                        {
                            x.BenhNhanId,
                            x.MaYeuCauTiepNhan,
                            x.HoTen,

                        })
                        .Select(s => new DSLinhDuocPhamChildTuGridVo
                        {
                            BenhNhanId = s.First().YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                            MaBenhNhan = s.First().MaBenhNhan,
                            HoTen = s.First().HoTen,
                            SoLuong = s.Sum(a => a.SoLuong),
                            DichVuKham = s.First().DichVuKham,
                            BacSiKeToa = s.First().BacSiKeToa,
                            NgayKe = s.First().NgayKe,
                            LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                            Id = yeuCauLinhId,
                            DuocDuyet = s.First().DuocDuyet,
                            NgayDieuTri = s.First().NgayDieuTri,
                            KhoLinhId = s.First().KhoLinhId
                        });
                    if (queryString[4] != null && queryString[4] != "" && queryString[4] != "true")
                    {
                        if (query.Any())
                        {
                            var list = DataChoGoi((long)query.First().KhoLinhId).AsQueryable();
                            query = query.Union(list);
                        }

                    }
                    var countTask = query.CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                var trangThaiDuyet = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPham.DuocDuyet).FirstOrDefault();
                var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                //var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                //            .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                //           && p.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                //            && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                //            )
                //.OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                             && p.DuocPhamBenhVienId == long.Parse(queryString[2])
                             && p.LaDuocPhamBHYT == bool.Parse(queryString[3])
                             && p.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                             && p.YeuCauLinhDuocPham.DuocDuyet == trangThaiDuyet
                            )
                .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
               .Select(s => new DuocPhamLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                    SL = s.SoLuongCanBu,
                    DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0
                });

                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhDuocPhamChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                BuildDefaultSortExpression(queryInfo);
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true")
                {
                    kieuIn = 0;
                }
                if (queryString[6] == "false")
                {
                    kieuIn = 1;
                }
                var yeuCauLinh =
                    _yeuCauLinhDuocPhamRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));
                List<DSLinhDuocPhamChildTuGridVo> queryable = new List<DSLinhDuocPhamChildTuGridVo>();
                if (kieuIn == 0)
                {
                    if (yeuCauLinh == true)
                    {
                        var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                        queryable = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.NoiTruPhieuDieuTriId !=null)? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           //x.NgayKe
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                           && x.LaDuocPhamBHYT == item.First().LaBHYT
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString = item.First().NgayKetString
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                    }
                    else
                    {
                        long khoaId = 0;
                        var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
                        if (phongBenhVien != null)
                        {
                            khoaId = phongBenhVien.KhoaPhongId;
                        }

                        //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
                        var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
                        {
                            KeyId = (long)o.Id,
                            DisplayName = o.Ten
                        }).OrderBy(o => o.DisplayName).ToList();
                        var ques = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhDuocPhamId == null
                                   && x.KhoLinhId == long.Parse(queryString[7]) &&
                                   phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaBHYT = item.LaDuocPhamBHYT,
                           TenDuocPham = item.Ten,
                           NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                           HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                           DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                           DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                           NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,
                           Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           BacSyKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                           ,
                           SoLuongTon = item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == item.KhoLinhId && nkct.LaDuocPhamBHYT == item.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat)
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhDuocPhamId,
                           x.DuocPhamBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.NongDoHamLuong,
                           x.HoatChat,
                           x.DuongDung,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           //x.NgayKetString
                       })
                       .Select(item => new DSLinhDuocPhamChildTuGridVo()
                       {
                           YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenDuocPham = item.First().TenDuocPham,
                           NongDoHamLuong = item.First().NongDoHamLuong,
                           HoatChat = item.First().HoatChat,
                           DuongDung = item.First().DuongDung,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           BacSyKeToa = item.First().BacSyKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString = item.First().NgayKetString
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                        if (ques.Any())
                        {
                            queryable = queryable.Union(ques).ToList();
                        }
                    }
                }
                if (kieuIn == 1)
                {
                    var yeuCauLinhDuocPham =
                        await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                            NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                            HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                            DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                            DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                            NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                            DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                            BacSyKeToa = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : ""
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhDuocPhamId,
                            x.DuocPhamBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhDuocPhamChildTuGridVo()
                        {
                            YeuCauLinhDuocPhamId = long.Parse(queryString[0]),
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenDuocPham = item.First().TenDuocPham,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom,
                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            BacSyKeToa = item.First().BacSyKeToa,
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();
                }

                var dataOrderBy = queryable.AsQueryable().OrderBy(queryInfo.SortString);
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            return null;
        }
        #endregion
        private OBJList GetHTMLLinhBenhNhanTuChoi(List<DuocPhamLoaiQuanLyLinhTTGridVo> gridVos,bool loaiThuoc)
        {
            string sluongDaXuat = "";
            string ghiChu = "";
            var index = 1;
            var thuoc = "";
            foreach (var itemx in gridVos)
            {
                if (itemx.SoLuongCoTheXuat == null)
                {
                    sluongDaXuat = "";
                }
                else
                {
                    sluongDaXuat = Convert.ToString(itemx.SoLuongCoTheXuat);
                }
                thuoc = thuoc + "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.MaDuocPham
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenDuocPham
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemx.SoLuong), false) : itemx.SoLuong.MathRoundNumber(2) + "")
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        (loaiThuoc == true && itemx.SoLuongCoTheXuat != null? NumberHelper.ChuyenSoRaText(Convert.ToDouble(sluongDaXuat), false) : sluongDaXuat)
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        "&nbsp;";
                //ghiChu; => để trống 14/04/2021
            }
            var data = new OBJList
            {
                Index = index,
                html = thuoc
            };
            return data;
        }
        private OBJList GetHTMLLinhBenhNhan(List<DuocPhamVatTuLinhTTGridVo> gridVos,bool loaiThuoc)
        {
            var index = 1;
            var thuoc = "";
            string yeuCau = ""; // to do
            var thucChat = 0; // to do
            var tenLoaiLinh = "";
            var donViTinh = "";

            foreach (var itemx in gridVos)
            {
                //if (itemx.DuocDuyet == true)
                //{
                //    yeuCau = itemx.YeuCau.ToString();
                //}
                //else
                //{
                //    yeuCau = "";
                //}
                thuoc = thuoc + "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.MaDuocPham
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.TenDuocPham
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                       (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemx.SoLuong), false) : itemx.SoLuong.MathRoundNumber(2) + "")
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                         (loaiThuoc == true  && itemx.DuocDuyet == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemx.YeuCau), false) : yeuCau)
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        "&nbsp;";
                tenLoaiLinh = itemx.TenLoaiLinh;
                donViTinh = "";
            }
            var data = new OBJList
            {
                Index = index,
                html = thuoc
            };
            return data;
        }
        #endregion
        private List<DSLinhDuocPhamChildTuGridVo> DataChoGoi(long idKhoLinh)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            var yeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.KhoLinhId == idKhoLinh &&
                                                                                         x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                                                         x.YeuCauLinhDuocPhamId == null &&
                                                                                         phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                                                         x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                         )
                        .OrderBy(x => x.ThoiDiemChiDinh)
                       .Select(s => new DSLinhDuocPhamChildTuGridVo
                       {
                           YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                           MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                           MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                           HoTen = s.YeuCauTiepNhan.HoTen,
                           SoLuong = s.SoLuong,
                           DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                           NgayKe = s.ThoiDiemChiDinh,
                           DuocDuyet = s.YeuCauLinhDuocPham.DuocDuyet,
                           KhoLinhId = s.KhoLinhId
                       })
                       .GroupBy(x => new
                       {
                           x.BenhNhanId,
                           x.MaYeuCauTiepNhan,
                           x.HoTen,

                       })
                       .Select(s => new DSLinhDuocPhamChildTuGridVo
                       {
                           YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                           MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                           MaBenhNhan = s.First().MaBenhNhan,
                           HoTen = s.First().HoTen,
                           SoLuong = s.Sum(a => a.SoLuong),
                           DichVuKham = s.First().DichVuKham,
                           BacSiKeToa = s.First().BacSiKeToa,
                           NgayKe = s.First().NgayKe,
                           LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                           //Id = yeuCauLinhId,
                           DuocDuyet = s.First().DuocDuyet,
                           KhoLinhId = s.First().KhoLinhId
                       });
            return yeuCauDuocPham.ToList();
        }
    }
}
