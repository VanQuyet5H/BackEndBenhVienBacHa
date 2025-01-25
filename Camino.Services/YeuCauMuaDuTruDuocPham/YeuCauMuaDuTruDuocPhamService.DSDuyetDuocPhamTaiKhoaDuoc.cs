using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using static Camino.Core.Domain.Enums;
using System.Globalization;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;

namespace Camino.Services.YeuCauMuaDuTruDuocPham
{
    public partial class YeuCauMuaDuTruDuocPhamService
    {
        #region chờ xử lý
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new DuTruMuaDuocPhamTaiKhoaDuocSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaDuocSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyetDuTruMua = GetDataYeuCauMuaDuocPham(null, queryInfo, queryObject);
            var queryDangChoDuyetDuTruMuaTaiKhoa = GetDataYeuCauMuaDuocPhamTaiKhoaDuoc(null, queryInfo, queryObject);
            var queryChoGoiDuTruMu = GetDataYeuCauMuaDuocPham(true, queryInfo, queryObject);
            var queryChoGoiDuTruMuTaiKhoa = GetDataYeuCauMuaDuocPhamTaiKhoaDuoc(true, queryInfo, queryObject);


            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaDuocPhamTaiKhoaDuocGridVo())
                .AsQueryable();
            var isHaveQuery = false;

            if (queryObject.ChoDuyet)
            {
                query = queryDangChoDuyetDuTruMua.Concat(queryDangChoDuyetDuTruMuaTaiKhoa);

                isHaveQuery = true;
            }

            if (queryObject.ChoGoi)
            {
                query = query.Concat(queryChoGoiDuTruMuTaiKhoa.Concat(queryChoGoiDuTruMu));
            }

            if (queryObject.ChoDuyet == false && queryObject.ChoGoi == false)
            {
                query = queryDangChoDuyetDuTruMua.Concat(queryDangChoDuyetDuTruMuaTaiKhoa);
                query = query.Concat(queryChoGoiDuTruMuTaiKhoa.Concat(queryChoGoiDuTruMu));
            }
            if (queryInfo.Sort.Count == 1 && queryInfo.Sort[0].Dir != null)
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }
            query = query.OrderBy(queryInfo.SortString);
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
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new DuTruMuaDuocPhamTaiKhoaDuocSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaDuocSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyetDuTruMua = GetDataYeuCauMuaDuocPham(null, queryInfo, queryObject);
            var queryDangChoDuyetDuTruMuaTaiKhoa = GetDataYeuCauMuaDuocPhamTaiKhoaDuoc(null, queryInfo, queryObject);
            var queryChoGoiDuTruMu = GetDataYeuCauMuaDuocPham(true, queryInfo, queryObject);
            var queryChoGoiDuTruMuTaiKhoa = GetDataYeuCauMuaDuocPhamTaiKhoaDuoc(true, queryInfo, queryObject);


            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaDuocPhamTaiKhoaDuocGridVo())
                .AsQueryable();
            var isHaveQuery = false;

            if (queryObject.ChoDuyet)
            {
                query = queryDangChoDuyetDuTruMua.Concat(queryDangChoDuyetDuTruMuaTaiKhoa);

                isHaveQuery = true;
            }

            if (queryObject.ChoGoi)
            {
                query = query.Concat(queryChoGoiDuTruMuTaiKhoa.Concat(queryChoGoiDuTruMu));
            }

            if (queryObject.ChoDuyet == false && queryObject.ChoGoi == false)
            {
                query = queryDangChoDuyetDuTruMua.Concat(queryDangChoDuyetDuTruMuaTaiKhoa);
                query = query.Concat(queryChoGoiDuTruMuTaiKhoa.Concat(queryChoGoiDuTruMu));
            }
            if (queryInfo.Sort.Count == 1 && queryInfo.Sort[0].Dir != null)
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        private IQueryable<DuTruMuaDuocPhamTaiKhoaDuocGridVo> GetDataYeuCauMuaDuocPham(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaDuocPhamTaiKhoaDuocSearch queryObject)
        {
            var result = BaseRepository.TableNoTracking.Include(cc => cc.NhanVienYeuCau)
                .Include(cc => cc.Kho)
                 .Include(cc => cc.NhanVienYeuCau).ThenInclude(x => x.User)
                .Where(p => p.TruongKhoaDuyet == duocDuyet && p.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && p.DuTruMuaDuocPhamKhoDuocId == null)
            .Select(o => new DuTruMuaDuocPhamTaiKhoaDuocGridVo
            {
                Id = o.Id,
                KhoId = (long)o.Kho.LoaiKho,
                SoPhieu = o.SoPhieu,
                DuTruMuaDuocPhamKhoDuocId = o.DuTruMuaDuocPhamKhoDuocId,
                TrangThaiDuTru = o.TruongKhoaDuyet == null ? (int)EnumTrangThaiLoaiDuTru.ChoDuyet : (int)EnumTrangThaiLoaiDuTru.ChoGoi,
                TrangThai = o.TruongKhoaDuyet == null ? EnumTrangThaiLoaiDuTru.ChoDuyet.GetDescription() : EnumTrangThaiLoaiDuTru.ChoGoi.GetDescription() + "-" + o.TuNgay.ApplyFormatDate() + "-" + o.DenNgay.ApplyFormatDate() + '-' + o.KyDuTruMuaDuocPhamVatTuId,
                KhoaKhoa = o.Kho.Ten,
                KhoaIdKhoId = o.KhoId,
                LoaiNhom = o.NhomDuocPhamDuTru != null ? o.NhomDuocPhamDuTru.GetDescription() : "",
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                //DuTruMuaId = 1,
                TuNgay = o.TuNgay,
                DenNgay = o.DenNgay,
                KyDuTruMuaDuocPhamVatTuId = o.KyDuTruMuaDuocPhamVatTuId,
                NgayYeuCau = o.NgayYeuCau,
                NgayKhoaDuocDuyet = o.NgayTruongKhoaDuyet,
                KyDuTru = o.TuNgay.ApplyFormatDate() + '-' + o.DenNgay.ApplyFormatDate()
            }).ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
                    q => q.SoPhieu,
                    q=> q.KhoaKhoa,
                    q=> q.NguoiYeuCau);
            if (queryObject != null)
            {
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);

                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    var denNgayFormat = new DateTime(queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null && queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);
                    var denNgayFormat = new DateTime(queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat && p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.SearchString != null)
                {
                    var search = queryObject.SearchString.Trim();
                    result = result.ApplyLike(search,
                    q => q.SoPhieu,
                    q => q.KhoaKhoa,
                    q => q.NguoiYeuCau);
                }
            }
            return result;
        }
        private IQueryable<DuTruMuaDuocPhamTaiKhoaDuocGridVo> GetDataYeuCauMuaDuocPhamTaiKhoaDuoc(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaDuocPhamTaiKhoaDuocSearch queryObject)
        {
            var result = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
                .Where(p => p.KhoDuocDuyet == duocDuyet
                            && p.DuTruMuaDuocPhamKhoDuocId == null)
            .Select(o => new DuTruMuaDuocPhamTaiKhoaDuocGridVo
            {
                Id = o.Id,
                KhoId = (long)EnumLoaiKhoDuocPham.KhoLe,  // kho le
                SoPhieu = o.SoPhieu,
                TrangThaiDuTru = o.KhoDuocDuyet == null ? (int)EnumTrangThaiLoaiDuTru.ChoDuyet : (int)EnumTrangThaiLoaiDuTru.ChoGoi,
                TrangThai = o.KhoDuocDuyet == null ? EnumTrangThaiLoaiDuTru.ChoDuyet.GetDescription() : EnumTrangThaiLoaiDuTru.ChoGoi.GetDescription() + "-" + o.TuNgay.ApplyFormatDate() + "-" + o.DenNgay.ApplyFormatDate() + '-' + o.KyDuTruMuaDuocPhamVatTuId,
                KhoaIdKhoId = o.KhoaPhongId,
                KhoaKhoa = o.KhoaPhong.Ten,
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                //DuTruMuaId = 1,
                KyDuTruMuaDuocPhamVatTuId = o.KyDuTruMuaDuocPhamVatTuId,
                TuNgay = o.TuNgay,
                DenNgay = o.DenNgay,
                NgayYeuCau = o.NgayYeuCau,
                NgayKhoaDuocDuyet = o.NgayKhoDuocDuyet,
                KyDuTru = o.TuNgay.ApplyFormatDate() + '-' + o.DenNgay.ApplyFormatDate()
            }).ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
                      q => q.SoPhieu,
                    q => q.KhoaKhoa,
                    q => q.NguoiYeuCau);
            if (queryObject != null)
            {
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);

                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    var denNgayFormat = new DateTime(queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null && queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);
                    var denNgayFormat = new DateTime(queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Year, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Month, queryObject.RangeNhap.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat && p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.SearchString != null)
                {
                    var search = queryObject.SearchString.Trim();
                    result = result.ApplyLike(search,
                    q => q.SoPhieu,
                    q => q.KhoaKhoa,
                    q => q.NguoiYeuCau);
                }
            }
            return result;
        }
        #region child chờ xử lý  // to do
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaDuocPhamId);
            var loaiKho = Convert.ToInt32(queryString[1]);
            int trangThai = Convert.ToInt32(queryString[2]);
            long.TryParse(queryString[3], out long khoDuocPhamChiTietId);
            //Kho
            if (loaiKho != (int)EnumLoaiKhoDuocPham.KhoLe)
            {
                var dataList = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                            .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                            {
                                Id = p.Id,
                                LoaiDuocPham = p.LaDuocPhamBHYT,
                                Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                DuocPhamId = p.DuocPhamId,
                                DuocPham = p.DuocPham.Ten,
                                HoatChat = p.DuocPham.HoatChat,
                                NongDoVaHamLuong = p.DuocPham.HamLuong,
                                SDK = p.DuocPham.SoDangKy,
                                DVT = p.DuocPham.DonViTinh.Ten,
                                DD = p.DuocPham.DuongDung.Ten,
                                NhaSX = p.DuocPham.NhaSanXuat,
                                NuocSX = p.DuocPham.NuocSanXuat,
                                NhomDieuTri = p.NhomDieuTriDuPhong ,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoId = p.DuTruMuaDuocPham.KhoId,
                                KhoaPhongId = 0,
                                KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoId == khoDuocPhamChiTietId
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) ,

                                TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                            .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                                                                            .Select(x => new KhoTongTonDuocPham()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                                                        .Select(e => new KhoTongTonDuocPham
                                                                                                                                        {
                                                                                                                                            TenKhoTong = e.First().TenKhoTong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x=>x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                                                        .Select(e => new HopDongChuaNhapDuoc
                                                                                                                                        {
                                                                                                                                            TenHopDong = e.First().TenHopDong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),

                            }).ToList();
                var query = dataList.GroupBy(x => new
                {
                    x.DuocPhamId,
                    x.Loai
                }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien
                {
                    Loai = item.First().Loai,
                    DuocPhamId = item.First().DuocPhamId,
                    DuocPham = item.First().DuocPham,
                    HoatChat = item.First().HoatChat,
                    NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                    SDK = item.First().SDK,
                    DVT = item.First().DVT,
                    DD = item.First().DD,
                    NhaSX = item.First().NhaSX,
                    NuocSX = item.First().NuocSX,
                    SLDuTru = item.First().SLDuTru,
                    SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                    KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                    KhoTongTon = item.Sum(x => x.KhoTongTon),
                    HDChuaNhap = item.FirstOrDefault().HDChuaNhap,
                    HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                    NhomDieuTri= item.FirstOrDefault().NhomDieuTri,
                    SLDuTruTKhoaDuyet = item.Sum(x=>x.SLDuTruTKhoaDuyet),
                    TongTonList = item.FirstOrDefault().TongTonList,
                    LoaiDuocPham = item.First().LoaiDuocPham,
                    TrangThai = item.First().TrangThai,
                    KhoId = item.First().KhoId,
                    KhoaPhongId = item.First().KhoaPhongId,
                    Id = item.First().Id
                });
                var dataOrderBy = query.OrderBy(x => x.LoaiDuocPham == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                return new GridDataSource { Data = data };
            }
            //Khoa
            else
            {
                long.TryParse(queryString[3], out long khoaPhongId);
                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamId)
                            .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                LoaiDuocPham = p.LaDuocPhamBHYT,
                                DuocPhamId = p.DuocPhamId,
                                DuocPham = p.DuocPham.Ten,
                                HoatChat = p.DuocPham.HoatChat,
                                NongDoVaHamLuong = p.DuocPham.HamLuong,
                                SDK = p.DuocPham.SoDangKy,
                                DVT = p.DuocPham.DonViTinh.Ten,
                                DD = p.DuocPham.DuongDung.Ten,
                                NhaSX = p.DuocPham.NhaSanXuat,
                                NuocSX = p.DuocPham.NuocSanXuat,
                                KhoaPhongId = khoaPhongId,
                                KhoId = 0,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,// todo
                                KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            &&  x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               
                                TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                            .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                                                                            .Select(x => new KhoTongTonDuocPham()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                                                        .Select(e => new KhoTongTonDuocPham
                                                                                                                                        {
                                                                                                                                            TenKhoTong = e.First().TenKhoTong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                //KhoDuTruTon = ,
                                HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                                                        .Select(e => new HopDongChuaNhapDuoc
                                                                                                                                        {
                                                                                                                                            TenHopDong = e.First().TenHopDong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),

                            }).GroupBy(x => new
                            {
                                x.DuocPhamId,
                                x.Loai
                            }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiDuocPham = item.First().LoaiDuocPham,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                DuocPhamId = item.First().DuocPhamId,
                                DuocPham = item.First().DuocPham,
                                HoatChat = item.First().HoatChat,
                                NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                                SDK = item.First().SDK,
                                DVT = item.First().DVT,
                                DD = item.First().DD,
                                NhaSX = item.First().NhaSX,
                                NuocSX = item.First().NuocSX,
                                KhoaPhongId = item.First().KhoaPhongId,
                                SLDuTru = item.First().SLDuTru,
                                SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                                KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                                KhoTongTon = item.Sum(x => x.KhoTongTon),
                                HDChuaNhap = item.FirstOrDefault().HDChuaNhap,
                                SLDuTruKDuocDuyet = item.FirstOrDefault().SLDuTruKDuocDuyet,
                                HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                                NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                                SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                                TongTonList = item.FirstOrDefault().TongTonList,
                                KhoId = item.First().KhoId,
                            });
                var dataOrderBy = query.OrderBy(x => x.LoaiDuocPham == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = dataOrderBy.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.Id == duTruMuaDuocPhamId).SelectMany(s => s.DuTruMuaDuocPhams).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s=>s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == o.DuocPhamId
                                                                                               && x.NhapKhoDuocPhams.KhoId == lst.KhoId
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.LaDuocPhamBHYT == o.LoaiDuocPham
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    }
                    return o;
                }
                );
                var queryTask = resdataOrderBy.ToArray();
                return new GridDataSource { Data = queryTask };

            }

        }

        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaDuocPhamId);
            var loaiKho = Convert.ToInt32(queryString[1]);
            int trangThai = Convert.ToInt32(queryString[2]);
            long.TryParse(queryString[3], out long khoDuocPhamChiTietId);
            //Kho
            if (loaiKho != (int)EnumLoaiKhoDuocPham.KhoLe)
            {
                var dataList = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                            .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                            {
                                Id = p.Id,
                                LoaiDuocPham = p.LaDuocPhamBHYT,
                                Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                DuocPhamId = p.DuocPhamId,
                                DuocPham = p.DuocPham.Ten,
                                HoatChat = p.DuocPham.HoatChat,
                                NongDoVaHamLuong = p.DuocPham.HamLuong,
                                SDK = p.DuocPham.SoDangKy,
                                DVT = p.DuocPham.DonViTinh.Ten,
                                DD = p.DuocPham.DuongDung.Ten,
                                NhaSX = p.DuocPham.NhaSanXuat,
                                NuocSX = p.DuocPham.NuocSanXuat,
                                NhomDieuTri = p.NhomDieuTriDuPhong,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoId = p.DuTruMuaDuocPham.KhoId,
                                KhoaPhongId = 0,
                                KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoId == khoDuocPhamChiTietId
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) ,
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                                TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                            .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                                                                            .Select(x => new KhoTongTonDuocPham()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                                                        .Select(e => new KhoTongTonDuocPham
                                                                                                                                        {
                                                                                                                                            TenKhoTong = e.First().TenKhoTong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                                                        .Select(e => new HopDongChuaNhapDuoc
                                                                                                                                        {
                                                                                                                                            TenHopDong = e.First().TenHopDong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),

                            }).ToList();
                var query = dataList.GroupBy(x => new
                {
                    x.DuocPhamId,
                    x.Loai
                }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien
                {
                    Loai = item.First().Loai,
                    DuocPhamId = item.First().DuocPhamId,
                    DuocPham = item.First().DuocPham,
                    HoatChat = item.First().HoatChat,
                    NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                    SDK = item.First().SDK,
                    DVT = item.First().DVT,
                    DD = item.First().DD,
                    NhaSX = item.First().NhaSX,
                    NuocSX = item.First().NuocSX,
                    SLDuTru = item.First().SLDuTru,
                    SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                    KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                    KhoTongTon = item.Sum(x => x.KhoTongTon),
                    HDChuaNhap = item.FirstOrDefault().HDChuaNhap,
                    HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                    NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                    SLDuTruTKhoaDuyet = item.Sum(x => x.SLDuTruTKhoaDuyet),
                    TongTonList = item.FirstOrDefault().TongTonList,
                    LoaiDuocPham = item.First().LoaiDuocPham,
                    TrangThai = item.First().TrangThai,
                    KhoId = item.First().KhoId,
                    KhoaPhongId = item.First().KhoaPhongId,
                    Id = item.First().Id
                });
                var dataOrderBy = query.OrderBy(x => x.LoaiDuocPham == true).ToList();
                var queryTask = dataOrderBy.Count();
                return new GridDataSource { TotalRowCount = queryTask };

            }
            //Khoa
            else
            {
                long.TryParse(queryString[3], out long khoaPhongId);
                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamId)
                            .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                LoaiDuocPham = p.LaDuocPhamBHYT,
                                DuocPhamId = p.DuocPhamId,
                                DuocPham = p.DuocPham.Ten,
                                HoatChat = p.DuocPham.HoatChat,
                                NongDoVaHamLuong = p.DuocPham.HamLuong,
                                SDK = p.DuocPham.SoDangKy,
                                DVT = p.DuocPham.DonViTinh.Ten,
                                DD = p.DuocPham.DuongDung.Ten,
                                NhaSX = p.DuocPham.NhaSanXuat,
                                NuocSX = p.DuocPham.NuocSanXuat,
                                KhoaPhongId = khoaPhongId,
                                KhoId = 0,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,// todo
                                KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                                TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                            .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                                                                            .Select(x => new KhoTongTonDuocPham()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                                                        .Select(e => new KhoTongTonDuocPham
                                                                                                                                        {
                                                                                                                                            TenKhoTong = e.First().TenKhoTong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                //KhoDuTruTon = ,
                                HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                                                        .Select(e => new HopDongChuaNhapDuoc
                                                                                                                                        {
                                                                                                                                            TenHopDong = e.First().TenHopDong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),

                            }).GroupBy(x => new
                            {
                                x.DuocPhamId,
                                x.Loai
                            }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiDuocPham = item.First().LoaiDuocPham,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                DuocPhamId = item.First().DuocPhamId,
                                DuocPham = item.First().DuocPham,
                                HoatChat = item.First().HoatChat,
                                NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                                SDK = item.First().SDK,
                                DVT = item.First().DVT,
                                DD = item.First().DD,
                                NhaSX = item.First().NhaSX,
                                NuocSX = item.First().NuocSX,
                                KhoaPhongId = item.First().KhoaPhongId,
                                SLDuTru = item.First().SLDuTru,
                                SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                                KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                                KhoTongTon = item.Sum(x => x.KhoTongTon),
                                HDChuaNhap = item.FirstOrDefault().HDChuaNhap,
                                SLDuTruKDuocDuyet = item.FirstOrDefault().SLDuTruKDuocDuyet,
                                HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                                NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                                SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                                TongTonList = item.FirstOrDefault().TongTonList,
                                KhoId = item.First().KhoId,
                            });
                var dataOrderBy = query.OrderBy(x => x.LoaiDuocPham == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = dataOrderBy.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.Id == duTruMuaDuocPhamId).SelectMany(s => s.DuTruMuaDuocPhams).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == o.DuocPhamId
                                                                                               && x.NhapKhoDuocPhams.KhoId == lst.KhoId
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.LaDuocPhamBHYT == o.LoaiDuocPham
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    }
                    return o;
                }
                );
                var queryTask = resdataOrderBy.Count();
                return new GridDataSource { TotalRowCount = queryTask };

            }

        }
        #endregion
        #region child child chờ xử lý
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruDuocPhamChiTietId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            var khoId = Convert.ToInt32(queryString[2]);
            var khoaPhong = Convert.ToInt32(queryString[3]);
            if (loaiDP == true)  // khoid
            {
                if (khoId != 0)
                {
                    var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == loaiDP)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                     NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription() :""
                 });
                    var dataOrderBy = query.AsQueryable();
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
                }
                if (khoaPhong != 0)
                {
                    var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == loaiDP)
                                                               .SelectMany(s => s.DuTruMuaDuocPhamChiTiets)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                     NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription() : ""
                 });
                    var dataOrderBy = query.AsQueryable();
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
                }

            }
            if (loaiDP == false)  // khoaId
            {
                if (khoId != 0)
                {
                    var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == loaiDP)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                     NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription() : ""
                 });
                    var dataOrderBy = query.AsQueryable();
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
                }
                if (khoaPhong != 0)
                {
                    var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == loaiDP)
                                                               .SelectMany(s => s.DuTruMuaDuocPhamChiTiets)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                     NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription() : ""
                 });
                    var dataOrderBy = query.AsQueryable();
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
                }
            }
            return null;
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long DuocPhamId);
            var loaiDP = Convert.ToBoolean(queryString[1]);

            int khoaPhongId = Convert.ToInt32(queryString[2]);
            int trangThai = Convert.ToInt32(queryString[3]);
            if (trangThai == 1)
            {
                if (DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.Kho.KhoaPhongId == khoaPhongId
                                                                          && x.DuocPhamId == DuocPhamId
                                                                          && x.LaDuocPhamBHYT == loaiDP)
                            .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                            {
                                Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                                Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                                KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                                SLDuTru = cc.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                            });
                    var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
            }
            if (trangThai == 2)
            {
                int dutruMuaDuocPhamTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var queryDuTruMuaDuocPhamKhoaId = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == dutruMuaDuocPhamTheoKhoaChiTietId).FirstOrDefault().DuTruMuaDuocPhamTheoKhoaId;
                if (queryDuTruMuaDuocPhamKhoaId != null && DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.DuTruMuaDuocPhamTheoKhoaId == queryDuTruMuaDuocPhamKhoaId
                                                                          && x.DuocPhamId == DuocPhamId
                                                                          && x.LaDuocPhamBHYT == loaiDP)
                            .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                            {
                                Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                                Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                                KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                                SLDuTru = cc.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                            });
                    var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
            }

            return null;
        }
        public async Task<GridDataSource> GetDataChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duocPhamDuTruId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            int duocPhamId = Convert.ToInt32(queryString[2]);

            if (duocPhamDuTruId != null && duocPhamId != null && loaiDP != null )
            {
                var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.Id == duocPhamDuTruId
                                                                      && x.DuocPhamId == duocPhamId
                                                                      && x.LaDuocPhamBHYT == loaiDP)
                        .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                        {
                            Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                            Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                            KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                            SLDuTru = cc.SoLuongDuTru,
                            SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                        });
                var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetDataChildKhoaForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duocPhamDuTruId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            int duocPhamId = Convert.ToInt32(queryString[2]);

            if (duocPhamDuTruId != null && duocPhamId != null && loaiDP != null)
            {
                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == duocPhamDuTruId
                                                                      //&& x.DuocPhamId == duocPhamId
                                                                      && x.LaDuocPhamBHYT == loaiDP).SelectMany(x=>x.DuTruMuaDuocPhamChiTiets)
                        .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                        {
                            NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong.GetDescription(),
                            Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                            Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                            KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                            SLDuTru = cc.SoLuongDuTru,
                            SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                        });
                var dataOrderBy = query.AsQueryable();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            return null;
        }
        #endregion
        #region duyệt
        public List<DuTruMuaDuocPhamChiTietGridVo> GetDuTruMuaDuocPhamChiTiet(long idDuTruMua)
        {
            var duTruMuaDuocPhamChiTiets = _duTruMuaDuocPhamChiTietRepo.Table.Where(cc => cc.DuTruMuaDuocPhamId == idDuTruMua)
                .Select(x => new DuTruMuaDuocPhamChiTietGridVo()
                {
                    Id = x.Id,
                    DuocPhamId = x.DuocPhamId,
                    DuTruMuaDuocPhamId = x.DuTruMuaDuocPhamId,
                    DuTruMuaDuocPhamKhoDuocChiTietId = x.DuTruMuaDuocPhamKhoDuocChiTietId,
                    DuTruMuaDuocPhamTheoKhoaChiTietId = x.DuTruMuaDuocPhamTheoKhoaChiTietId,
                    LaDuocPhamBHYT = x.LaDuocPhamBHYT,
                    NhomDieuTriDuPhong = x.NhomDieuTriDuPhong,
                    SoLuongDuKienSuDung = x.SoLuongDuKienSuDung,
                    SoLuongDuTru = x.SoLuongDuTru,
                    SoLuongDuTruTruongKhoaDuyet = x.SoLuongDuTruTruongKhoaDuyet,
                });
            return duTruMuaDuocPhamChiTiets.ToList();
        }
        public List<DuTruMuaDuocPhamKhoaChiTietGridVo> GetDuTruMuaDuocPhamKhoaChiTiet(long idDuTruMuaKhoa)
        {
            var duTruMuaDuocPhamChiTiets = _duTruMuaDuocPhamTheoKhoaChiTietRepo.Table.Where(cc => cc.DuTruMuaDuocPhamTheoKhoaId == idDuTruMuaKhoa)
                .Select(x => new DuTruMuaDuocPhamKhoaChiTietGridVo()
                {
                    Id = x.Id,
                    DuocPhamId = x.DuocPhamId,
                    DuTruMuaDuocPhamTheoKhoaId = x.DuTruMuaDuocPhamTheoKhoaId,
                    DuTruMuaDuocPhamKhoDuocChiTietId = x.DuTruMuaDuocPhamKhoDuocChiTietId,
                    LaDuocPhamBHYT = x.LaDuocPhamBHYT,
                    SoLuongDuKienSuDung = x.SoLuongDuKienSuDung,
                    SoLuongDuTru = x.SoLuongDuTru,
                    SoLuongDuTruTruongKhoaDuyet = x.SoLuongDuTruTruongKhoaDuyet,
                    SoLuongDuTruKhoDuocDuyet = x.SoLuongDuTruKhoDuocDuyet
                });
            return duTruMuaDuocPhamChiTiets.ToList();
        }
        public async Task<DuTruMuaDuocPhamTheoKhoa> GetDuTruDuocPhamTheoKhoaByIdAsync(long duTruMuaDuocPhamKhoaId)
        {
            var duTruDuocPhamTheoKhoa = await _duTruMuaDuocPhamTheoKhoaRepo.Table.Where(x => x.Id == duTruMuaDuocPhamKhoaId)
                .Include(x => x.DuTruMuaDuocPhamTheoKhoaChiTiets)
                .FirstAsync();
            return duTruDuocPhamTheoKhoa;
        }
        #endregion
        //#region get data duyet
        //#endregion
        #region update duyet  
        public bool? GetTrangThaiDuyet(long Id, long? khoaId, long? khoId)
        {
            if (khoaId != 0 && khoaId != null)
            {
                var duocDuyet = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
               .Where(x => x.Id == Id).Select(p => p.KhoDuocDuyet).FirstOrDefault();
                return duocDuyet;
            }
            if (khoId != 0 && khoId != null)
            {
                var duocDuyet = BaseRepository.TableNoTracking
               .Where(x => x.Id == Id).Select(p => p.TruongKhoaDuyet).FirstOrDefault();
                return duocDuyet;
            }
            return null;
        }
        // != kho le
        public DuTruMuaDuocPhamChiTietViewGridVo GetDataUpdate(long Id, bool typeLoaiKho)
        {


            //Lấy thông tin chi tiết của Khoa

            if (typeLoaiKho == true)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == nguoiDangLogin).Select(s => s.User.HoTen).FirstOrDefault();
                var queryChiTiet = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Include(o => o.DuTruMuaDuocPhamKhoDuoc)
                                       .Where(s => s.Id == Id)
                                       .Select(item => new DuTruMuaDuocPhamChiTietViewGridVo()
                                       {
                                           Id = item.Id,
                                           SoPhieu = item.SoPhieu,
                                           NgayYeuCau = item.NgayYeuCau,
                                           KhoaId = item.KhoaPhongId,
                                           TenKhoa = item.KhoaPhong.Ten,
                                           TrangThai = GetTrangThaiDuTruKhoaTaiKhoaDuoc(item),
                                           TrangThaiHienThi = GetTrangThaiDuTruKhoaTaiKhoaDuoc(item).GetDescription(),
                                           GhiChu = item.GhiChu,
                                           NguoiYeuCauId = item.NhanVienYeuCauId == null ? nguoiDangLogin : item.NhanVienYeuCauId,
                                           TenNguoiYeuCau = item.NhanVienYeuCauId == null ? nguoiLogin : item.NhanVienYeuCau.User.HoTen,
                                           KyDuTru = item.TuNgay.ApplyFormatDate() + '-' + item.DenNgay.ApplyFormatDate(),
                                           LyDoTuChoi = item.LyDoKhoDuocTuChoi
                                       }).FirstOrDefault();
                // child
                if (queryChiTiet != null)
                {
                    queryChiTiet.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking
                                                                                                              .Where(item => item.DuTruMuaDuocPhamTheoKhoaId == queryChiTiet.Id)
                                                                                                              .Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamList()
                                                                                                              {

                                                                                                                  Id = itemc.Id,
                                                                                                                  Loai = itemc.LaDuocPhamBHYT,
                                                                                                                  DuocPhamId = itemc.DuocPhamId,
                                                                                                                  TenDuocPham = itemc.DuocPham.Ten,
                                                                                                                  HoatChat = itemc.DuocPham.HoatChat,
                                                                                                                  NongDoVaHamLuong = itemc.DuocPham.HamLuong,
                                                                                                                  SDK = itemc.DuocPham.SoDangKy,
                                                                                                                  DVT = itemc.DuocPham.DonViTinh.Ten,
                                                                                                                  DD = itemc.DuocPham.DuongDung.Ten,
                                                                                                                  NhaSX = itemc.DuocPham.NhaSanXuat,
                                                                                                                  NuocSX = itemc.DuocPham.NuocSanXuat,
                                                                                                                  SLDuTru = itemc.SoLuongDuTru,
                                                                                                                  SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                                                                                                                  KhoaId = queryChiTiet.KhoaId,
                                                                                                                  //NhomDieuTriDuPhong = itemc.NhomDieuTriDuPhong != null ? itemc.NhomDieuTriDuPhong.GetDescription() : "",
                                                                                                                  SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet,
                                                                                                                  SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruKhoDuocDuyet !=null ? itemc.SoLuongDuTruKhoDuocDuyet : itemc.SoLuongDuTru,
                                                                                                                  DuocPhamDuTruTheoKhoaId = itemc.DuTruMuaDuocPhamTheoKhoaId,
                                                                                                                  DuTruMuaDuocPhamTheoKhoaId = itemc.DuTruMuaDuocPhamTheoKhoaId,
                                                                                                               
                                                                                                                  KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                                                            .Where(x => x.DuocPhamBenhVienId == itemc.DuocPhamId
                                                                                                                                        && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                                                                        && x.LaDuocPhamBHYT == itemc.LaDuocPhamBHYT
                                                                                                                                        && x.NhapKhoDuocPhams.DaHet != true
                                                                                                                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                                                                                                  TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                                                            .Where(x => x.DuocPhamBenhVienId == itemc.DuocPhamId
                                                                                                                                        && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                                                                        && x.LaDuocPhamBHYT == itemc.LaDuocPhamBHYT
                                                                                                                                        && x.NhapKhoDuocPhams.DaHet != true
                                                                                                                                        && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                                                                        {
                                                                                                                                            TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                                                            TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                                                                        }).GroupBy(q => q.TenKhoTong)
                                                                                                                                        .Select(e => new KhoTongTonDuocPham
                                                                                                                                        {
                                                                                                                                            TenKhoTong = e.First().TenKhoTong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                                                                                                  HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == itemc.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                                                                                                  HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == itemc.DuocPhamId)
                                                                                                                                        .Select(z => new HopDongChuaNhapDuoc()
                                                                                                                                        {
                                                                                                                                            TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                                                            TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                                                        }).GroupBy(q => q.TenHopDong)
                                                                                                                                        .Select(e => new HopDongChuaNhapDuoc
                                                                                                                                        {
                                                                                                                                            TenHopDong = e.First().TenHopDong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                                                                                              }).GroupBy(x => new
                                                                                                              {
                                                                                                                  x.Loai,
                                                                                                                  x.DuocPhamId
                                                                                                              }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamList()
                                                                                                              {
                                                                                                                  Id = itemcc.First().Id,
                                                                                                                  Loai = itemcc.First().Loai,
                                                                                                                  DuocPhamId = itemcc.First().DuocPhamId,
                                                                                                                  TenDuocPham = itemcc.First().TenDuocPham,
                                                                                                                  HoatChat = itemcc.First().HoatChat,
                                                                                                                  NongDoVaHamLuong = itemcc.First().NongDoVaHamLuong,
                                                                                                                  SDK = itemcc.First().SDK,
                                                                                                                  DVT = itemcc.First().DVT,
                                                                                                                  DD = itemcc.First().DD,
                                                                                                                  NhaSX = itemcc.First().NhaSX,
                                                                                                                  NuocSX = itemcc.First().NuocSX,
                                                                                                                  SLDuTru = itemcc.First().SLDuTru,
                                                                                                                  SLDuKienSuDungTrongKho = itemcc.First().SLDuKienSuDungTrongKho,
                                                                                                                  KhoaId = itemcc.First().KhoaId,
                                                                                                                  SLDuTruTKhoDuyet = itemcc.Sum(s=> s.SLDuTruTKhoDuyet),
                                                                                                                  SLDuTruKhoDuocDuyet = itemcc.Sum(s => s.SLDuTruKhoDuocDuyet),
                                                                                                                  DuocPhamDuTruTheoKhoaId = itemcc.First().DuocPhamDuTruTheoKhoaId,
                                                                                                                  DuTruMuaDuocPhamTheoKhoaId = itemcc.First().DuTruMuaDuocPhamTheoKhoaId,
                                                                                                                  DuocPhamDuTruId = itemcc.First().DuocPhamDuTruId,
                                                                                                                  HDChuaNhap = itemcc.Sum(x => x.HDChuaNhap),
                                                                                                                  KhoTongTon = itemcc.Sum(x => x.KhoTongTon),
                                                                                                                  TongTonList = itemcc.First().TongTonList,
                                                                                                                  HopDongChuahapList = itemcc.First().HopDongChuahapList
                                                                                                              }).ToList();
                    var dataOrderBy = queryChiTiet.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList.OrderBy(x => x.Loai == true).ToList();
                    var resdataOrderBy = dataOrderBy.Select(o =>
                    {
                        var listKhoTheoKhoaPhong = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.Id == queryChiTiet.Id).SelectMany(s => s.DuTruMuaDuocPhams).ToList();
                        var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                        foreach (var lst in groupKho)
                        {
                            o.KhoDuTruTon += _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                       .Where(x => x.DuocPhamBenhVienId == o.DuocPhamId
                                                                                                   && x.NhapKhoDuocPhams.KhoId == lst.KhoId
                                                                                                   && x.NhapKhoDuocPhams.DaHet != true
                                                                                                   && x.LaDuocPhamBHYT == o.Loai
                                                                                                   && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                        }
                        return o;
                    }
                    );
                    var queryTask = resdataOrderBy.ToArray();
                }
                return queryChiTiet;

            }
            //Lấy thông tin chi tiết của Kho
            if (typeLoaiKho == false)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == nguoiDangLogin).Select(s => s.User.HoTen).FirstOrDefault();
                var queryChiTiet = BaseRepository.TableNoTracking.Include(o => o.DuTruMuaDuocPhamKhoDuoc)
                                          .Where(s => s.Id == Id)
                                          .Select(item => new DuTruMuaDuocPhamChiTietViewGridVo()
                                          {
                                              Id = item.Id,
                                              SoPhieu = item.SoPhieu,
                                              LoaiDuTruId = (long)item.NhomDuocPhamDuTru,
                                              TenLoaiDuTru = item.NhomDuocPhamDuTru.GetDescription(),
                                              NgayYeuCau = item.NgayYeuCau,
                                              KhoId = item.KhoId,
                                              TenKho = item.Kho.Ten,
                                              TrangThai = GetTrangThaiDuTruKhoTaiKhoaDuoc(item),
                                              TrangThaiHienThi = GetTrangThaiDuTruKhoTaiKhoaDuoc(item).GetDescription(),
                                              GhiChu = item.GhiChu,
                                              NguoiYeuCauId = item.NhanVienYeuCauId == null ? nguoiDangLogin : item.NhanVienYeuCauId,
                                              TenNguoiYeuCau = item.NhanVienYeuCauId == null ? nguoiLogin : item.NhanVienYeuCau.User.HoTen,
                                              KyDuTru = item.TuNgay.ApplyFormatDate() + '-' + item.DenNgay.ApplyFormatDate(),
                                              LyDoTuChoi = item.LyDoTruongKhoaTuChoi
                                          }).FirstOrDefault();
                // child
                if (queryChiTiet != null)
                {
                    var KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                                                               .Where(x => x.DuocPhamBenhVienId == 5837
                                                                                                                                          && x.NhapKhoDuocPhams.KhoId == 3
                                                                                                                                           && x.LaDuocPhamBHYT == false
                                                                                                                                           && x.NhapKhoDuocPhams.DaHet != true
                                                                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    queryChiTiet.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList = _duTruMuaDuocPhamChiTietRepo.TableNoTracking
                                                                                                              .Where(item => item.DuTruMuaDuocPhamId == queryChiTiet.Id)
                                                                                                              .Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamList()
                                                                                                              {
                                                                                                                  Id = itemc.Id,
                                                                                                                  Loai = itemc.LaDuocPhamBHYT,
                                                                                                                  DuocPhamId = itemc.DuocPhamId,
                                                                                                                  TenDuocPham = itemc.DuocPham.Ten,
                                                                                                                  HoatChat = itemc.DuocPham.HoatChat,
                                                                                                                  NongDoVaHamLuong = itemc.DuocPham.HamLuong,
                                                                                                                  SDK = itemc.DuocPham.SoDangKy,
                                                                                                                  DVT = itemc.DuocPham.DonViTinh.Ten,
                                                                                                                  DD = itemc.DuocPham.DuongDung.Ten,
                                                                                                                  NhaSX = itemc.DuocPham.NhaSanXuat,
                                                                                                                  NuocSX = itemc.DuocPham.NuocSanXuat,
                                                                                                                  SLDuTru = itemc.SoLuongDuTru,
                                                                                                                  SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                                                                                                                  KhoaId = queryChiTiet.KhoaId,
                                                                                                                  NhomDieuTriDuPhong = itemc.NhomDieuTriDuPhong != null ? itemc.NhomDieuTriDuPhong.GetDescription() : "",
                                                                                                                  SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet!=null? itemc.SoLuongDuTruTruongKhoaDuyet: itemc.SoLuongDuTru,
                                                                                                                  SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruTruongKhoaDuyet != null ? itemc.SoLuongDuTruTruongKhoaDuyet : itemc.SoLuongDuTru,
                                                                                                                  DuocPhamDuTruId = itemc.DuTruMuaDuocPhamId,
                                                                                                                  KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                                                                .Where(x => x.DuocPhamBenhVienId == itemc.DuocPhamId
                                                                                                                                           && x.NhapKhoDuocPhams.KhoId == queryChiTiet.KhoId
                                                                                                                                            && x.LaDuocPhamBHYT == itemc.LaDuocPhamBHYT
                                                                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                                                                                                  KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                                                                .Where(x => x.DuocPhamBenhVienId == itemc.DuocPhamId
                                                                                                                                            &&  x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                                                                            && x.LaDuocPhamBHYT == itemc.LaDuocPhamBHYT
                                                                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                                                                                                  TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                                                                .Where(x => x.DuocPhamBenhVienId == itemc.DuocPhamId
                                                                                                                                            && 
                                                                                                                                            x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                                                                            && x.LaDuocPhamBHYT == itemc.LaDuocPhamBHYT
                                                                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                                                                            {
                                                                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                                                        .Select(e => new KhoTongTonDuocPham
                                                                                                                                        {
                                                                                                                                            TenKhoTong = e.First().TenKhoTong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                                                                                                  HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == itemc.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                                                                                                  HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == itemc.DuocPhamId)
                                                                                                                                        .Select(z => new HopDongChuaNhapDuoc()
                                                                                                                                        {
                                                                                                                                            TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                                                            TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                                                        }).GroupBy(q => q.TenHopDong)
                                                                                                                                        .Select(e => new HopDongChuaNhapDuoc
                                                                                                                                        {
                                                                                                                                            TenHopDong = e.First().TenHopDong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                                                                                              }).GroupBy(x => new
                                                                                                              {
                                                                                                                  x.Loai,
                                                                                                                  x.DuocPhamId
                                                                                                              }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamList()
                                                                                                              {
                                                                                                                  Id = itemcc.First().Id,
                                                                                                                  Loai = itemcc.First().Loai,
                                                                                                                  DuocPhamId = itemcc.First().DuocPhamId,
                                                                                                                  TenDuocPham = itemcc.First().TenDuocPham,
                                                                                                                  HoatChat = itemcc.First().HoatChat,
                                                                                                                  NongDoVaHamLuong = itemcc.First().NongDoVaHamLuong,
                                                                                                                  SDK = itemcc.First().SDK,
                                                                                                                  DVT = itemcc.First().DVT,
                                                                                                                  DD = itemcc.First().DD,
                                                                                                                  NhaSX = itemcc.First().NhaSX,
                                                                                                                  NuocSX = itemcc.First().NuocSX,
                                                                                                                  SLDuTru = itemcc.Sum(x => x.SLDuTru),
                                                                                                                  SLDuKienSuDungTrongKho = itemcc.Sum(x => x.SLDuKienSuDungTrongKho),
                                                                                                                  KhoaId = itemcc.First().KhoaId,
                                                                                                                  SLDuTruTKhoDuyet = itemcc.Sum(x => x.SLDuTruTKhoDuyet),
                                                                                                                  SLDuTruKhoDuocDuyet = itemcc.Sum(x => x.SLDuTruKhoDuocDuyet),
                                                                                                                  DuocPhamDuTruId = itemcc.First().DuocPhamDuTruId,
                                                                                                                  HDChuaNhap = itemcc.Sum(x => x.HDChuaNhap),
                                                                                                                  KhoTongTon = itemcc.Sum(x => x.KhoTongTon),
                                                                                                                  TongTonList = itemcc.First().TongTonList,
                                                                                                                  HopDongChuahapList = itemcc.First().HopDongChuahapList,
                                                                                                                  NhomDieuTriDuPhong = itemcc.FirstOrDefault().NhomDieuTriDuPhong,
                                                                                                                  KhoDuTruTon = itemcc.FirstOrDefault().KhoDuTruTon
                                                                                                              }).ToList();
                    queryChiTiet.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList.OrderBy(x => x.Loai == true);
                }
                return queryChiTiet;
            }
            return null;
        }

        private EnumTrangThaiDuTruKhoaDuoc GetTrangThaiDuTruKhoTaiKhoaDuoc(DuTruMuaDuocPham item)
        {
            if (item.TruongKhoaDuyet == null)
            {
                return EnumTrangThaiDuTruKhoaDuoc.ChoDuyet;
            }
            else
            {
                if (item.TruongKhoaDuyet == false)
                {
                    return EnumTrangThaiDuTruKhoaDuoc.TuChoi;
                }
                else
                {
                    if (item.DuTruMuaDuocPhamKhoDuoc != null)
                    {
                        if (item.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == null)
                        {
                            return EnumTrangThaiDuTruKhoaDuoc.DaGoiVaChoDuyet;
                        }
                        else
                        {
                            if (item.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false)
                            {
                                return EnumTrangThaiDuTruKhoaDuoc.TuChoi;
                            }
                            else
                            {
                                return EnumTrangThaiDuTruKhoaDuoc.DaDuyet;
                            }
                        }
                    }
                    else
                    {
                        return EnumTrangThaiDuTruKhoaDuoc.ChoGoi;
                    }
                }
            }
        }
        private EnumTrangThaiDuTruKhoaDuoc GetTrangThaiDuTruKhoaTaiKhoaDuoc(DuTruMuaDuocPhamTheoKhoa item)
        {
            if (item.KhoDuocDuyet == null)
            {
                return EnumTrangThaiDuTruKhoaDuoc.ChoDuyet;
            }
            else
            {
                if (item.KhoDuocDuyet == false)
                {
                    return EnumTrangThaiDuTruKhoaDuoc.TuChoi;
                }
                else
                {
                    if (item.DuTruMuaDuocPhamKhoDuoc != null)
                    {
                        if (item.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == null)
                        {
                            return EnumTrangThaiDuTruKhoaDuoc.DaGoiVaChoDuyet;
                        }
                        else
                        {
                            if (item.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false)
                            {
                                return EnumTrangThaiDuTruKhoaDuoc.TuChoi;
                            }
                            else
                            {
                                return EnumTrangThaiDuTruKhoaDuoc.DaDuyet;
                            }
                        }
                    }
                    else
                    {
                        return EnumTrangThaiDuTruKhoaDuoc.ChoGoi;
                    }
                }
            }
        }
        private EnumTrangThaiDuTruKhoaDuoc GetTrangThaiDuTruMuaDuocPhamKhoDuoc(DuTruMuaDuocPhamKhoDuoc item)
        {
            if (item.GiamDocDuyet == null)
            {
                return EnumTrangThaiDuTruKhoaDuoc.DaGoiVaChoDuyet;
            }
            else
            {
                if (item.GiamDocDuyet == false)
                {
                    return EnumTrangThaiDuTruKhoaDuoc.TuChoi;
                }
                else
                {
                    return EnumTrangThaiDuTruKhoaDuoc.DaDuyet;
                }
            }
        }

        #endregion
        #endregion
        #region đã xử lý
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy>(queryInfo.AdditionalSearchString);
            }

            var queryDaGoiVaChoDuyet = GetDataYeuCauMuaDuocPhamDaGoiVaChoDuyet(null, queryInfo, queryObject);
            var queryDaDuyet = GetDataYeuCauMuaDuocPhamDaDuyet(true, queryInfo, queryObject);
            var queryTuChoi = GetDataYeuCauMuaDuocPhamTuChoi(false, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaDuocPhamTaiKhoaDuocDaXuLy())
                .AsQueryable();
            var isHaveQuery = false;

            if (queryObject.DaGoiVaDangChoDuyet)  // chờ gởi và đã duyệt
            {
                query = query.Concat(queryDaGoiVaChoDuyet);

                isHaveQuery = true;
            }

            if (queryObject.DaDuyet)
            {
                query = query.Concat(queryDaDuyet);
            }
            if (queryObject.TuChoiDuyet)
            {
                query = query.Concat(queryTuChoi);
            }

            if (queryObject.DaGoiVaDangChoDuyet == false && queryObject.DaDuyet == false && queryObject.TuChoiDuyet == false)
            {
                query = query.Concat(queryDaGoiVaChoDuyet.Concat(queryDaDuyet.Concat(queryTuChoi)));
            }
            if (queryInfo.Sort.Count == 1 && queryInfo.Sort[0].Dir != null)
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TinhTrang";
            }
            query = query.OrderBy(queryInfo.SortString);
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
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocDaXuLyToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            var queryObject = new DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy>(queryInfo.AdditionalSearchString);
            }

            var queryDaGoiVaChoDuyet = GetDataYeuCauMuaDuocPhamDaGoiVaChoDuyet(null, queryInfo, queryObject);
            var queryDaDuyet = GetDataYeuCauMuaDuocPhamDaDuyet(true, queryInfo, queryObject);
            var queryTuChoi = GetDataYeuCauMuaDuocPhamTuChoi(false, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaDuocPhamTaiKhoaDuocDaXuLy())
                .AsQueryable();
            var isHaveQuery = false;

            if (queryObject.DaGoiVaDangChoDuyet)  // chờ gởi và đã duyệt
            {
                query = query.Concat(queryDaGoiVaChoDuyet);

                isHaveQuery = true;
            }

            if (queryObject.DaDuyet)
            {
                query = query.Concat(queryDaDuyet);
            }
            if (queryObject.TuChoiDuyet)
            {
                query = query.Concat(queryTuChoi);
            }

            if (queryObject.DaGoiVaDangChoDuyet == false && queryObject.DaDuyet == false && queryObject.TuChoiDuyet == false)
            {
                query = query.Concat(queryDaGoiVaChoDuyet.Concat(queryDaDuyet.Concat(queryTuChoi)));
            }
            if (queryInfo.Sort.Count == 1 && queryInfo.Sort[0].Dir != null)
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThai";
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        private IQueryable<DuTruMuaDuocPhamTaiKhoaDuocDaXuLy> GetDataYeuCauMuaDuocPhamDaGoiVaChoDuyet(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy queryObject)
        {
            var result = _duTruMuaDuocPhamKhoDuocRepo.TableNoTracking
                .Where(p => p.GiamDocDuyet == duocDuyet)
            .Select(o => new DuTruMuaDuocPhamTaiKhoaDuocDaXuLy()
            {
                Id = o.Id,
                SoPhieu = o.SoPhieu,
                DuTruTheo = o.TuNgay.ApplyFormatDate() + '-' + o.DenNgay.ApplyFormatDate(),
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                NgayYeuCau = o.NgayYeuCau,
                TinhTrang = EnumTrangThaiDuTruKhoaDuoc.DaGoiVaChoDuyet,
                GhiChu = o.GhiChu,
                NgayGiamDocDuyet = o.NgayGiamDocDuyet,
            })
            .ApplyLike(queryInfo.SearchTerms.Trim(),
                    q => q.SoPhieu,
                    //q => q.DuTruTheo,
                    q => q.NguoiYeuCau,
                    q => q.GhiChu);
            if (queryObject != null)
            {
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime( queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Day , 0, 0, 0);

                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    var denNgayFormat = new DateTime(queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null && queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);
                    var denNgayFormat = new DateTime(queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat && p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.SearchString != null)
                {
                    var search = queryObject.SearchString.Trim();
                    result = result.ApplyLike(search,
                   q => q.SoPhieu,
                    //q => q.DuTruTheo,
                    q => q.NguoiYeuCau,
                    q => q.GhiChu);
                }
            }
            return result;
        }
        private IQueryable<DuTruMuaDuocPhamTaiKhoaDuocDaXuLy> GetDataYeuCauMuaDuocPhamDaDuyet(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy queryObject)
        {
            var result = _duTruMuaDuocPhamKhoDuocRepo.TableNoTracking
                .Where(p => p.GiamDocDuyet == true)
            .Select(o => new DuTruMuaDuocPhamTaiKhoaDuocDaXuLy()
            {
                Id = o.Id,
                SoPhieu = o.SoPhieu,
                DuTruTheo = o.TuNgay.ApplyFormatDate() + '-' + o.DenNgay.ApplyFormatDate(),
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                NgayYeuCau = o.NgayYeuCau,
                TinhTrang = EnumTrangThaiDuTruKhoaDuoc.DaDuyet,
                GhiChu = o.GhiChu,
                NgayGiamDocDuyet = o.NgayGiamDocDuyet,
            }).ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
                    q => q.SoPhieu,
                    q => q.DuTruTheo,
                    q => q.NguoiYeuCau,
                    q => q.GhiChu);
            if (queryObject != null)
            {
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);

                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    var denNgayFormat = new DateTime(queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null && queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);
                    var denNgayFormat = new DateTime(queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat && p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.SearchString != null)
                {
                    var search = queryObject.SearchString.Trim();
                    result = result.ApplyLike(search,
                   q => q.SoPhieu,
                    //q => q.DuTruTheo,
                    q => q.NguoiYeuCau,
                    q => q.GhiChu);
                }
            }
            return result;
        }
        private IQueryable<DuTruMuaDuocPhamTaiKhoaDuocDaXuLy> GetDataYeuCauMuaDuocPhamTuChoi(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy queryObject)
        {
            var result = _duTruMuaDuocPhamKhoDuocRepo.TableNoTracking
                .Where(p => p.GiamDocDuyet == false)
            .Select(o => new DuTruMuaDuocPhamTaiKhoaDuocDaXuLy()
            {
                Id = o.Id,
                SoPhieu = o.SoPhieu,
                DuTruTheo = o.TuNgay.ApplyFormatDate() + '-' + o.DenNgay.ApplyFormatDate(),
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                NgayYeuCau = o.NgayYeuCau,
                TinhTrang = EnumTrangThaiDuTruKhoaDuoc.TuChoi,
                GhiChu = o.GhiChu,
                NgayGiamDocDuyet = o.NgayGiamDocDuyet,
            }).ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
                    q => q.SoPhieu,
                    q => q.DuTruTheo,
                    q => q.NguoiYeuCau,
                    q => q.GhiChu);
            if (queryObject != null)
            {
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);

                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    var denNgayFormat = new DateTime(queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null && queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var tuNgayFormat = new DateTime(queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.startDate.GetValueOrDefault().Date.Day, 0, 0, 0);
                    var denNgayFormat = new DateTime(queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Year, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Month, queryObject.RangeDuyet.endDate.GetValueOrDefault().Date.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayYeuCau >= tuNgayFormat && p.NgayYeuCau <= denNgayFormat);
                }
                if (queryObject.SearchString != null)
                {
                    var search = queryObject.SearchString.Trim();
                    result = result.ApplyLike(search,
                   q => q.SoPhieu,
                    //q => q.DuTruTheo,
                    q => q.NguoiYeuCau,
                    q => q.GhiChu);
                }
            }
            return result;
        }
        // child
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long IdDuTruMuaDuocPhamKhoaDuoc);

            long.TryParse(queryString[1], out long TinhTrang);
           
                // nha thuoc
                var queryKho = BaseRepository.TableNoTracking
                    .Where(x => 
                         x.DuTruMuaDuocPhamKhoDuocId == IdDuTruMuaDuocPhamKhoaDuoc
                        )
                    .Select(s => new DuTruMuaDuocPhamKhoaDuocChild()
                    {
                        Id = s.Id,
                        KhoId = s.KhoId,
                        KyDuTruTheo = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                        KyDuTruTheoId = s.KyDuTruMuaDuocPhamVatTuId,
                        NgayKhoaDuocDuyet = s.NgayTruongKhoaDuyet,
                        SoPhieu = s.SoPhieu,
                        NgayYeuCau = s.NgayYeuCau,
                        NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                        TinhTrang = TinhTrang,
                        DuTruMuaDuocPhamKhoDuocId = IdDuTruMuaDuocPhamKhoaDuoc,
                        KhoaKhoaString = s.Kho.Ten,
                        LoaiKho = s.Kho.LoaiKho
                    });
                // khoa le kho
                var querykhoa = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
                    .Where(x => 
                     x.DuTruMuaDuocPhamKhoDuocId == IdDuTruMuaDuocPhamKhoaDuoc
                    )
                    .Select(s => new DuTruMuaDuocPhamKhoaDuocChild()
                    {
                        Id = s.Id,
                        KhoaId = s.KhoaPhongId,
                        KyDuTruTheo = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                        KyDuTruTheoId = s.KyDuTruMuaDuocPhamVatTuId,
                        NgayKhoaDuocDuyet = s.NgayKhoDuocDuyet,
                        SoPhieu = s.SoPhieu,
                        NgayYeuCau = s.NgayYeuCau,
                        NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                        TinhTrang = TinhTrang,
                        DuTruMuaDuocPhamKhoDuocId = IdDuTruMuaDuocPhamKhoaDuoc,
                        KhoaKhoaString = s.KhoaPhong.Ten,
                        LoaiKho = EnumLoaiKhoDuocPham.KhoLe
                    });
                List<DuTruMuaDuocPhamKhoaDuocChild> listQuery = new List<DuTruMuaDuocPhamKhoaDuocChild>();
                if (querykhoa.Count() > 0 && queryKho.Count() > 0)
                {
                    listQuery = queryKho.Union(querykhoa).AsQueryable().ToList();
                }
                if (querykhoa.Count() > 0 && queryKho.Count() == 0)
                {
                    listQuery = querykhoa.AsQueryable().ToList();
                }
                if (querykhoa.Count() == 0 && queryKho.Count() > 0)
                {
                    listQuery = queryKho.AsQueryable().ToList();
                }
                var dataOrderBy = listQuery.AsQueryable();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long IdDuTruMuaDuocPhamKhoaDuoc);

            long.TryParse(queryString[1], out long TinhTrang);

            // nha thuoc
            var queryKho = BaseRepository.TableNoTracking
                .Where(x =>
                     x.DuTruMuaDuocPhamKhoDuocId == IdDuTruMuaDuocPhamKhoaDuoc
                    )
                .Select(s => new DuTruMuaDuocPhamKhoaDuocChild()
                {
                    Id = s.Id,
                    KhoId = s.KhoId,
                    KyDuTruTheo = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                    KyDuTruTheoId = s.KyDuTruMuaDuocPhamVatTuId,
                    NgayKhoaDuocDuyet = s.NgayTruongKhoaDuyet,
                    SoPhieu = s.SoPhieu,
                    NgayYeuCau = s.NgayYeuCau,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    TinhTrang = TinhTrang,
                    DuTruMuaDuocPhamKhoDuocId = IdDuTruMuaDuocPhamKhoaDuoc,
                    KhoaKhoaString = s.Kho.Ten,
                    LoaiKho = s.Kho.LoaiKho
                });
            // khoa le kho
            var querykhoa = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
                .Where(x =>
                 x.DuTruMuaDuocPhamKhoDuocId == IdDuTruMuaDuocPhamKhoaDuoc
                )
                .Select(s => new DuTruMuaDuocPhamKhoaDuocChild()
                {
                    Id = s.Id,
                    KhoaId = s.KhoaPhongId,
                    KyDuTruTheo = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                    KyDuTruTheoId = s.KyDuTruMuaDuocPhamVatTuId,
                    NgayKhoaDuocDuyet = s.NgayKhoDuocDuyet,
                    SoPhieu = s.SoPhieu,
                    NgayYeuCau = s.NgayYeuCau,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    TinhTrang = TinhTrang,
                    DuTruMuaDuocPhamKhoDuocId = IdDuTruMuaDuocPhamKhoaDuoc,
                    KhoaKhoaString = s.KhoaPhong.Ten,
                    LoaiKho = EnumLoaiKhoDuocPham.KhoLe
                });
            List<DuTruMuaDuocPhamKhoaDuocChild> listQuery = new List<DuTruMuaDuocPhamKhoaDuocChild>();
            if (querykhoa.Count() > 0 && queryKho.Count() > 0)
            {
                listQuery = queryKho.Union(querykhoa).AsQueryable().ToList();
            }
            if (querykhoa.Count() > 0 && queryKho.Count() == 0)
            {
                listQuery = querykhoa.AsQueryable().ToList();
            }
            if (querykhoa.Count() == 0 && queryKho.Count() > 0)
            {
                listQuery = queryKho.AsQueryable().ToList();
            }

            var dataOrderBy = listQuery.AsQueryable();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }
        // child child
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildForGridAsync(QueryInfo queryInfo)
        {
            var querystring = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(querystring[0], out long idDuTruMua);
            long.TryParse(querystring[1], out long khoId);
            long.TryParse(querystring[2], out long khoaId);
            long.TryParse(querystring[3], out long Id);  // id khoa , hoac kho
            long.TryParse(querystring[4], out long TinhTrang);
            long.TryParse(querystring[5], out long DuTruMuaDuocPhamKhoDuocId);
            long IdDuTruKhoaDuocPham = 0;
            long IdDuTruDuocPham = 0;

            if (khoId != 0)
            {
                bool kiemTraLoaiKho = false;
                var loaiKho = _khoRepo.TableNoTracking.Where(x => x.Id == khoId).Select(x => x.LoaiKho).FirstOrDefault();
               
                    var dataList = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamId == Id
                                                                                            )
                                .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                                {
                                    Id = p.Id,
                                    Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                    DuocPhamId = p.DuocPhamId,
                                    DuocPham = p.DuocPham.Ten,
                                    HoatChat = p.DuocPham.HoatChat,
                                    NongDoVaHamLuong = p.DuocPham.HamLuong,
                                    SDK = p.DuocPham.SoDangKy,
                                    DVT = p.DuocPham.DonViTinh.Ten,
                                    DD = p.DuocPham.DuongDung.Ten,
                                    NhaSX = p.DuocPham.NhaSanXuat,
                                    NuocSX = p.DuocPham.NuocSanXuat,
                                    NhomDieuTri = p.NhomDieuTriDuPhong, /// to do
                                    SLDuTru = p.SoLuongDuTru,
                                    SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                    KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                                && x.NhapKhoDuocPhams.KhoId == khoId
                                                                                                && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                                && x.NhapKhoDuocPhams.DaHet != true
                                                                                                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                    KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                                && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                                && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                                && x.NhapKhoDuocPhams.DaHet != true 
                                                                                                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                    SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                    TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                               && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                                && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                                && x.NhapKhoDuocPhams.DaHet != true
                                                                                                && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                                {
                                                                                                    TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                    TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                                }).GroupBy(q => q.TenKhoTong)
                                                                                                                                        .Select(e => new KhoTongTonDuocPham
                                                                                                                                        {
                                                                                                                                            TenKhoTong = e.First().TenKhoTong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                    HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                    HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                                                        .Select(z => new HopDongChuaNhapDuoc()
                                                                                                                                        {
                                                                                                                                            TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                                                            TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                                                        }).GroupBy(q => q.TenHopDong)
                                                                                                                                        .Select(e => new HopDongChuaNhapDuoc
                                                                                                                                        {
                                                                                                                                            TenHopDong = e.First().TenHopDong,
                                                                                                                                            TongTon = e.Sum(q => q.TongTon)
                                                                                                                                        }).ToList(),
                                }).ToList();
                    var query = dataList.GroupBy(x => new
                    {
                        x.DuocPhamId,
                        x.Loai
                    }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien
                    {
                        Loai = item.First().Loai,
                        DuocPhamId = item.First().DuocPhamId,
                        DuocPham = item.First().DuocPham,
                        HoatChat = item.First().HoatChat,
                        NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                        SDK = item.First().SDK,
                        DVT = item.First().DVT,
                        DD = item.First().DD,
                        NhaSX = item.First().NhaSX,
                        NuocSX = item.First().NuocSX,
                        SLDuTru = item.First().SLDuTru,
                        SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                        KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                        KhoTongTon = item.Sum(x => x.KhoTongTon),
                        HDChuaNhap = item.FirstOrDefault().HDChuaNhap,
                        HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                        NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                        SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                        TongTonList = item.FirstOrDefault().TongTonList
                    });
                var dataOrderBy = query.OrderBy(x => x.LoaiDuocPham == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                return new GridDataSource { Data = data };

            }
            if(khoaId != 0)
            {
                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamTheoKhoaId == Id)
                                                                                      
                            .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                LoaiDuocPham = p.LaDuocPhamBHYT,
                                DuocPhamId = p.DuocPhamId,
                                DuocPham = p.DuocPham.Ten,
                                HoatChat = p.DuocPham.HoatChat,
                                NongDoVaHamLuong = p.DuocPham.HamLuong,
                                SDK = p.DuocPham.SoDangKy,
                                DVT = p.DuocPham.DonViTinh.Ten,
                                DD = p.DuocPham.DuongDung.Ten,
                                NhaSX = p.DuocPham.NhaSanXuat,
                                NuocSX = p.DuocPham.NuocSanXuat,
                                KhoaPhongId = khoaId,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,// todo
                                TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                          &&  x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                                HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                .Select(e => new HopDongChuaNhapDuoc
                                                                                                {
                                                                                                    TenHopDong = e.First().TenHopDong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                            }).GroupBy(x => new
                            {
                                x.DuocPhamId,
                                x.Loai
                            }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiDuocPham = item.First().LoaiDuocPham,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                DuocPhamId = item.First().DuocPhamId,
                                DuocPham = item.First().DuocPham,
                                HoatChat = item.First().HoatChat,
                                NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                                SDK = item.First().SDK,
                                DVT = item.First().DVT,
                                DD = item.First().DD,
                                NhaSX = item.First().NhaSX,
                                NuocSX = item.First().NuocSX,
                                KhoaPhongId = item.First().KhoaPhongId,
                                SLDuTru = item.First().SLDuTru,
                                SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                                KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                                KhoTongTon = item.Sum(x => x.KhoTongTon),
                                HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                                SLDuTruKDuocDuyet = item.FirstOrDefault().SLDuTruKDuocDuyet,
                                HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                                NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                                SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                                TongTonList = item.FirstOrDefault().TongTonList
                            }); 
                var dataOrderBy = query.OrderBy(x => x.LoaiDuocPham == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = data.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.Id == Id).SelectMany(s => s.DuTruMuaDuocPhams).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == o.DuocPhamId
                                                                                               && x.NhapKhoDuocPhams.KhoId == lst.KhoId
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.LaDuocPhamBHYT == o.LoaiDuocPham
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    }
                    return o;
                }
                );
                var queryTask = resdataOrderBy.ToArray();
                return new GridDataSource { Data = queryTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var querystring = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(querystring[0], out long idDuTruMua);
            long.TryParse(querystring[1], out long khoId);
            long.TryParse(querystring[2], out long khoaId);
            long.TryParse(querystring[3], out long Id);  // id khoa , hoac kho
            long.TryParse(querystring[4], out long TinhTrang);
            long.TryParse(querystring[5], out long DuTruMuaDuocPhamKhoDuocId);
            long IdDuTruKhoaDuocPham = 0;
            long IdDuTruDuocPham = 0;

            if (khoId != 0)
            {
                bool kiemTraLoaiKho = false;
                var loaiKho = _khoRepo.TableNoTracking.Where(x => x.Id == khoId).Select(x => x.LoaiKho).FirstOrDefault();

                var dataList = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamId == Id
                                                                                        )
                            .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                            {
                                Id = p.Id,
                                Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                DuocPhamId = p.DuocPhamId,
                                DuocPham = p.DuocPham.Ten,
                                HoatChat = p.DuocPham.HoatChat,
                                NongDoVaHamLuong = p.DuocPham.HamLuong,
                                SDK = p.DuocPham.SoDangKy,
                                DVT = p.DuocPham.DonViTinh.Ten,
                                DD = p.DuocPham.DuongDung.Ten,
                                NhaSX = p.DuocPham.NhaSanXuat,
                                NuocSX = p.DuocPham.NuocSanXuat,
                                NhomDieuTri = p.NhomDieuTriDuPhong, /// to do
                                    SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoId == khoId
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTru,
                                TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                           && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                                HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                .Select(e => new HopDongChuaNhapDuoc
                                                                                                {
                                                                                                    TenHopDong = e.First().TenHopDong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                            }).ToList();
                var query = dataList.GroupBy(x => new
                {
                    x.DuocPhamId,
                    x.Loai
                }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien
                {
                    Loai = item.First().Loai,
                    DuocPhamId = item.First().DuocPhamId,
                    DuocPham = item.First().DuocPham,
                    HoatChat = item.First().HoatChat,
                    NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                    SDK = item.First().SDK,
                    DVT = item.First().DVT,
                    DD = item.First().DD,
                    NhaSX = item.First().NhaSX,
                    NuocSX = item.First().NuocSX,
                    SLDuTru = item.First().SLDuTru,
                    SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                    KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                    KhoTongTon = item.Sum(x => x.KhoTongTon),
                    HDChuaNhap = item.FirstOrDefault().HDChuaNhap,
                    HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                    NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                    SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                    TongTonList = item.FirstOrDefault().TongTonList
                });
                var dataOrderBy = query.Count();
                return new GridDataSource { TotalRowCount = dataOrderBy };

            }
            if (khoaId != 0)
            {
                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamTheoKhoaId == Id)

                            .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                LoaiDuocPham = p.LaDuocPhamBHYT,
                                DuocPhamId = p.DuocPhamId,
                                DuocPham = p.DuocPham.Ten,
                                HoatChat = p.DuocPham.HoatChat,
                                NongDoVaHamLuong = p.DuocPham.HamLuong,
                                SDK = p.DuocPham.SoDangKy,
                                DVT = p.DuocPham.DonViTinh.Ten,
                                DD = p.DuocPham.DuongDung.Ten,
                                NhaSX = p.DuocPham.NhaSanXuat,
                                NuocSX = p.DuocPham.NuocSanXuat,
                                KhoaPhongId = khoaId,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                            && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTru,
                                SLDuTruKDuocDuyet = p.SoLuongDuTru,// todo
                                TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                          && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                            && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                            && x.NhapKhoDuocPhams.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                                HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                .Select(e => new HopDongChuaNhapDuoc
                                                                                                {
                                                                                                    TenHopDong = e.First().TenHopDong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                            }).GroupBy(x => new
                            {
                                x.DuocPhamId,
                                x.Loai
                            }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiDuocPham = item.First().LoaiDuocPham,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                DuocPhamId = item.First().DuocPhamId,
                                DuocPham = item.First().DuocPham,
                                HoatChat = item.First().HoatChat,
                                NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                                SDK = item.First().SDK,
                                DVT = item.First().DVT,
                                DD = item.First().DD,
                                NhaSX = item.First().NhaSX,
                                NuocSX = item.First().NuocSX,
                                KhoaPhongId = item.First().KhoaPhongId,
                                SLDuTru = item.First().SLDuTru,
                                SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                                KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                                KhoTongTon = item.Sum(x => x.KhoTongTon),
                                HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                                SLDuTruKDuocDuyet = item.FirstOrDefault().SLDuTruKDuocDuyet,
                                HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                                NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                                SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                                TongTonList = item.FirstOrDefault().TongTonList
                            });
                var dataOrderBy = query.OrderBy(x => x.LoaiDuocPham == true).ToList();
                var data = dataOrderBy.ToArray();
                var resdataOrderBy = dataOrderBy.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.Id == Id).SelectMany(s => s.DuTruMuaDuocPhams).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == o.DuocPhamId
                                                                                               && x.NhapKhoDuocPhams.KhoId == lst.KhoId
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.LaDuocPhamBHYT == o.LoaiDuocPham
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    }
                    return o;
                }
                );
                var queryTask = resdataOrderBy.Count();
                return new GridDataSource { TotalRowCount = queryTask };
            }
            return null;

        }
        // child child child
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long DuocPhamId);
            var loaiDP = Convert.ToBoolean(queryString[1]);

            int khoaPhongId = Convert.ToInt32(queryString[2]);
            int trangThai = Convert.ToInt32(queryString[3]);
            if (trangThai == 1)
            {
                int dutruMuaDuocPhamTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var duTruMuaDuocPhamTheoKhoaId = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == dutruMuaDuocPhamTheoKhoaChiTietId).FirstOrDefault().DuTruMuaDuocPhamTheoKhoaId;
                if (dutruMuaDuocPhamTheoKhoaChiTietId != 0)
                {
                    if (DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId
                                                                       && x.DuocPhamId == DuocPhamId
                                                                       && x.LaDuocPhamBHYT == loaiDP)
                         .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                         {
                             Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                             Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                             KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                             SLDuTru = cc.SoLuongDuTru,
                             SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                             NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription():""
                         });
                        var dataOrderBy = query.AsQueryable();
                        var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                        var countTask = dataOrderBy.Count();

                        return new GridDataSource { Data = data, TotalRowCount = countTask };
                    }
                }
                else
                {
                    if (DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.Kho.KhoaPhongId == khoaPhongId
                                                                              && x.DuocPhamId == DuocPhamId
                                                                              && x.LaDuocPhamBHYT == loaiDP)
                                .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                                {
                                    Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                                    Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                                    KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                                    SLDuTru = cc.SoLuongDuTru,
                                    SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                                    NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription() : ""
                                });
                        query = query;
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
                }

            }
            if (trangThai == 2)
            {
                int dutruMuaDuocPhamTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var queryDuTruMuaDuocPhamKhoaId = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == dutruMuaDuocPhamTheoKhoaChiTietId).FirstOrDefault().DuTruMuaDuocPhamTheoKhoaId;
                if (queryDuTruMuaDuocPhamKhoaId != null && DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.DuTruMuaDuocPhamTheoKhoaId == queryDuTruMuaDuocPhamKhoaId
                                                                          && x.DuocPhamId == DuocPhamId
                                                                          && x.LaDuocPhamBHYT == loaiDP)
                            .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                            {
                                Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                                Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                                KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                                SLDuTru = cc.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                                NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription() : ""
                            });
                    query = query;
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
            }

            return null;
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long DuocPhamId);
            var loaiDP = Convert.ToBoolean(queryString[1]);

            int khoaPhongId = Convert.ToInt32(queryString[2]);
            int trangThai = Convert.ToInt32(queryString[3]);
            if (trangThai == 1)
            {
                int dutruMuaDuocPhamTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var duTruMuaDuocPhamTheoKhoaId = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == dutruMuaDuocPhamTheoKhoaChiTietId).FirstOrDefault().DuTruMuaDuocPhamTheoKhoaId;
                if (dutruMuaDuocPhamTheoKhoaChiTietId != 0)
                {
                    if (DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId
                                                                       && x.DuocPhamId == DuocPhamId
                                                                       && x.LaDuocPhamBHYT == loaiDP)
                         .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                         {
                             Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                             Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                             KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                             SLDuTru = cc.SoLuongDuTru,
                             SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                         });
                        var dataOrderBy = query.AsQueryable();
                        var countTask = dataOrderBy.Count();

                        return new GridDataSource { TotalRowCount = countTask };
                    }
                }
                else
                {
                    if (DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.Kho.KhoaPhongId == khoaPhongId
                                                                              && x.DuocPhamId == DuocPhamId
                                                                              && x.LaDuocPhamBHYT == loaiDP)
                                .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                                {
                                    Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                                    Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                                    KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                                    SLDuTru = cc.SoLuongDuTru,
                                    SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                                });
                        var dataOrderBy = query.AsQueryable();
                        var countTask = dataOrderBy.Count();

                        return new GridDataSource { TotalRowCount = countTask };
                    }
                }

            }
            if (trangThai == 2)
            {
                int dutruMuaDuocPhamTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var queryDuTruMuaDuocPhamKhoaId = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == dutruMuaDuocPhamTheoKhoaChiTietId).FirstOrDefault().DuTruMuaDuocPhamTheoKhoaId;
                if (queryDuTruMuaDuocPhamKhoaId != null && DuocPhamId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPham.DuTruMuaDuocPhamTheoKhoaId == queryDuTruMuaDuocPhamKhoaId
                                                                          && x.DuocPhamId == DuocPhamId
                                                                          && x.LaDuocPhamBHYT == loaiDP)
                            .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                            {
                                Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                                Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                                KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                                SLDuTru = cc.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                            });
                    var dataOrderBy = query.AsQueryable();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
            }

            return null;
        }
        // view 
        public DuTruMuaDuocPhamChiTietGoiViewGridVo GetDuTruMuaDuocPhamChiTietGoiView(long idDuTruMuaKhoaDuoc, long tinhTrang)
       {

            var queryDuocPhamKhoDuoc = _duTruMuaDuocPhamKhoDuocRepo.TableNoTracking.Where(x => x.Id == idDuTruMuaKhoaDuoc)
                    .Select(item => new DuTruMuaDuocPhamChiTietGoiViewGridVo()
                    {
                        Id = item.Id,
                        KyDuTru = item.TuNgay.ApplyFormatDate() + '-' + item.DenNgay.ApplyFormatDate(),
                        KyDuTruId = item.KyDuTruMuaDuocPhamVatTuId,
                        NguoiYeuCauId = item.NhanVienYeuCauId,
                        TenNguoiYeuCau = item.NhanVienYeuCau.User.HoTen,
                        NgayYeuCau = item.NgayYeuCau,
                        GhiChu = item.GhiChu,
                        TrangThai = GetTrangThaiDuTruMuaDuocPhamKhoDuoc(item),
                        TrangThaiHienThi = GetTrangThaiDuTruMuaDuocPhamKhoDuoc(item).GetDescription(),
                        TuNgay = item.TuNgay,
                        DenNgay = item.DenNgay,
                        DuTruDuocPhamTheoKhoaId = item.Id,
                        LyDoTuChoi = item.LyDoGiamDocTuChoi
                    }).FirstOrDefault();
           if(queryDuocPhamKhoDuoc != null)
            {
                queryDuocPhamKhoDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = _duTruMuaDuocPhamKhoDuocChiTietRepo.TableNoTracking
                                                                                           .Where(item => item.DuTruMuaDuocPhamKhoDuocId == queryDuocPhamKhoDuoc.Id)
                                                                                            .Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList()
                                                                                            {
                                                                                                Id = itemc.Id,
                                                                                                DuTruMuaDuocPhamKhoaDuId = itemc.Id,
                                                                                                Loai = itemc.LaDuocPhamBHYT,
                                                                                                DuocPhamId = itemc.DuocPhamId,
                                                                                                TenDuocPham = itemc.DuocPham.Ten,
                                                                                                HoatChat = itemc.DuocPham.HoatChat,
                                                                                                NongDoVaHamLuong = itemc.DuocPham.HamLuong,
                                                                                                SDK = itemc.DuocPham.SoDangKy,
                                                                                                DVT = itemc.DuocPham.DonViTinh.Ten,
                                                                                                DD = itemc.DuocPham.DuongDung.Ten,
                                                                                                NhaSX = itemc.DuocPham.NhaSanXuat,
                                                                                                NuocSX = itemc.DuocPham.NuocSanXuat,
                                                                                                SLDuTru = itemc.SoLuongDuTru,
                                                                                                SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                                                                                                SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ? itemc.SoLuongDuTru : itemc.SoLuongDuTruTruongKhoaDuyet,
                                                                                                SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruKhoDuocDuyet == null ?itemc.SoLuongDuTru : itemc.SoLuongDuTruKhoDuocDuyet,
                                                                                                DuTruMuaDuocPhamKhoDuocId = idDuTruMuaKhoaDuoc
                                                                                            }).GroupBy(x => new
                                                                                                {
                                                                                                    x.Loai,
                                                                                                    x.DuocPhamId
                                                                                                }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList()
                                                                                                {
                                                                                                    Id = itemcc.First().Id,
                                                                                                    DuTruMuaDuocPhamKhoaDuId = itemcc.First().DuTruMuaDuocPhamKhoaDuId,
                                                                                                    DuTruMuaDuocPhamKhoId = itemcc.First().DuTruMuaDuocPhamKhoId,
                                                                                                    Loai = itemcc.First().Loai,
                                                                                                    DuocPhamId = itemcc.First().DuocPhamId,
                                                                                                    TenDuocPham = itemcc.First().TenDuocPham,
                                                                                                    HoatChat = itemcc.First().HoatChat,
                                                                                                    NongDoVaHamLuong = itemcc.First().NongDoVaHamLuong,
                                                                                                    SDK = itemcc.First().SDK,
                                                                                                    DVT = itemcc.First().DVT,
                                                                                                    DD = itemcc.First().DD,
                                                                                                    NhaSX = itemcc.First().NhaSX,
                                                                                                    NuocSX = itemcc.First().NuocSX,
                                                                                                    SLDuTru = itemcc.First().SLDuTru,
                                                                                                    SLDuKienSuDungTrongKho = itemcc.First().SLDuKienSuDungTrongKho,
                                                                                                    //KhoaId = itemcc.First().KhoaId,
                                                                                                    SLDuTruTKhoDuyet = itemcc.First().SLDuTruTKhoDuyet,
                                                                                                    SLDuTruKhoDuocDuyet = itemcc.First().SLDuTruKhoDuocDuyet,
                                                                                                    DuocPhamDuTruTheoKhoaId = itemcc.First().DuocPhamDuTruTheoKhoaId,
                                                                                                    DuTruMuaDuocPhamTheoKhoaId = itemcc.First().DuTruMuaDuocPhamTheoKhoaId,
                                                                                                    DuocPhamDuTruId = itemcc.First().DuocPhamDuTruId,
                                                                                                    KyDuTruMuaDuocPhamVatTuId = itemcc.First().KyDuTruMuaDuocPhamVatTuId,
                                                                                                    DuTruMuaDuocPhamKhoDuocId = itemcc.First().DuTruMuaDuocPhamKhoDuocId

                                                                                                }).ToList();
            }
           
            return queryDuocPhamKhoDuoc;
        }
        // view child
        public async Task<GridDataSource> GetDuTruMuaDuocPhamChiTietGoiViewChild(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long idDuTruMuaDuocPhamKhoDuocChiTiet);
            //long.TryParse(queryString[1], out long duocPhamId);
            //long.TryParse(queryString[2], out long IdKyDuTru);
            //bool.TryParse(queryString[3], out bool loai);

            var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruMuaDuocPhamKhoaDuocChildChild()).AsQueryable();
            var duTruMuaDpTheoKhoaDetailedIds = await _duTruMuaDuocPhamTheoKhoaChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocChiTietId == idDuTruMuaDuocPhamKhoDuocChiTiet)
                .Select(e => e.Id).ToListAsync();
            foreach (var duTruMuaDpTheoKhoaDetailedIdItem in duTruMuaDpTheoKhoaDetailedIds)
            {
                var duTruMuaDpChiTiets = _duTruMuaDuocPhamChiTietRepo.TableNoTracking
                    .Where(e => e.DuTruMuaDuocPhamTheoKhoaChiTietId == duTruMuaDpTheoKhoaDetailedIdItem 
                                )
                    .Select(e => new DuTruMuaDuocPhamKhoaDuocChildChild
                    {
                        Id = e.Id,
                        Nhom = e.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                        Kho = e.DuTruMuaDuocPham.Kho.Ten,
                        Khoa = e.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoa.KhoaPhong.Ten,
                        KyDuTru = e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                         " - " +
                                         e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                        SLDuTru = e.SoLuongDuTru,
                        SLDuKienTrongKy = e.SoLuongDuKienSuDung,
                        NhomDieuTri = e.NhomDieuTriDuPhong != null
                            ? e.NhomDieuTriDuPhong.GetValueOrDefault().GetDescription()
                            : string.Empty,
                    });
                query = query.Concat(duTruMuaDpChiTiets);
            }

            var duTruMuaDpChiTietTuKhoTongs = _duTruMuaDuocPhamChiTietRepo.TableNoTracking
                .Where(e => e.DuTruMuaDuocPhamKhoDuocChiTietId == idDuTruMuaDuocPhamKhoDuocChiTiet 
                            )
                .Select(e => new DuTruMuaDuocPhamKhoaDuocChildChild
                {
                    Id = e.Id,
                    Nhom = e.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                    Kho = e.DuTruMuaDuocPham.Kho.Ten,
                    Khoa = "Khoa Dược",
                    KyDuTru = e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                     " - " +
                                     e.DuTruMuaDuocPham.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                    SLDuTru = e.SoLuongDuTru,
                    SLDuKienTrongKy = e.SoLuongDuKienSuDung,
                    NhomDieuTri = e.NhomDieuTriDuPhong != null
                        ? e.NhomDieuTriDuPhong.GetValueOrDefault().GetDescription()
                        : string.Empty
                });
            query = query.Concat(duTruMuaDpChiTietTuKhoTongs);
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
        #endregion
        #region Từ chối
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryKho = BaseRepository.TableNoTracking
                .Where(s => s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                            s.TruongKhoaDuyet == false
                ).Select(s => new DuTruMuaDuocPhamTuChoiGridVo()
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    KhoaKho = s.Kho.Ten,
                    KhoaId = 0,
                    KhoId = s.KhoId,
                    KyDuTru = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                    NgayTuChoi = s.NgayTruongKhoaDuyet,
                    NgayYeuCau = s.NgayYeuCau,
                    NguoiTuChoiId = s.TruongKhoaId,
                    NguoiYeuCauId = s.NhanVienYeuCauId,
                    LyDo = s.LyDoTruongKhoaTuChoi,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    NguoiTuChoi = s.TruongKhoa.User.HoTen,
                    TinhTrang = EnumTrangThaiLoaiDuTruDaXuLy.TuChoi,
                    LoaiKho = s.Kho.LoaiKho
                }).ApplyLike(queryInfo.SearchTerms, g => g.SoPhieu, g => g.LyDo, g => g.NguoiYeuCau, g => g.NguoiTuChoi,g=>g.KhoaKho);

                var queryKhoa = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.KhoDuocDuyet == false)
                                                                       .Select(s => new DuTruMuaDuocPhamTuChoiGridVo()
                                                                       {
                                                                           Id = s.Id,
                                                                           SoPhieu = s.SoPhieu,
                                                                           KhoaKho = s.KhoaPhong.Ten,
                                                                           KhoaId = s.KhoaPhongId,
                                                                           KhoId = 0,
                                                                           KyDuTru = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                                                                           NgayTuChoi = s.NgayKhoDuocDuyet,
                                                                           NgayYeuCau = s.NgayYeuCau,
                                                                           NguoiTuChoiId = s.NhanVienKhoDuocId,
                                                                           NguoiYeuCauId = s.NhanVienYeuCauId,
                                                                           LyDo = s.LyDoKhoDuocTuChoi,
                                                                           NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                                                                           NguoiTuChoi = s.NhanVienKhoDuoc.User.HoTen,
                                                                           TinhTrang = EnumTrangThaiLoaiDuTruDaXuLy.TuChoi,
                                                                           LoaiKho = EnumLoaiKhoDuocPham.KhoLe
                                                                       }
                ).ApplyLike(queryInfo.SearchTerms, g => g.SoPhieu, g => g.LyDo, g => g.NguoiYeuCau, g => g.NguoiTuChoi, g=>g.KhoaKho);
         var  query = queryKho.Concat(queryKhoa);



            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaDuocTuChoiSearch>(queryInfo.AdditionalSearchString);

                if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;

                    query = query.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayYeuCau <= denNgay);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null && queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryObject.SearchString != null)
                {
                    var search = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(search,
                    q => q.SoPhieu,
                    q => q.KhoaKho,
                    q => q.NguoiYeuCau,
                    g => g.LyDo,
                    g => g.NguoiTuChoi
                   );
                }
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            var queryKho = BaseRepository.TableNoTracking
                .Where(s => s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                            s.TruongKhoaDuyet == false
                ).Select(s => new DuTruMuaDuocPhamTuChoiGridVo()
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    KhoaKho = s.Kho.LoaiKho.GetDescription(),
                    KhoaId = 0,
                    KhoId = s.KhoId,
                    KyDuTru = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                    NgayTuChoi = s.NgayTruongKhoaDuyet,
                    NgayYeuCau = s.NgayYeuCau,
                    NguoiTuChoiId = s.TruongKhoaId,
                    NguoiYeuCauId = s.NhanVienYeuCauId,
                    LyDo = s.LyDoTruongKhoaTuChoi,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    NguoiTuChoi = s.TruongKhoa.User.HoTen,
                    TinhTrang = EnumTrangThaiLoaiDuTruDaXuLy.TuChoi,
                    LoaiKho = s.Kho.LoaiKho
                }).ApplyLike(queryInfo.SearchTerms, g => g.SoPhieu, g => g.LyDo, g => g.NguoiYeuCau, g => g.NguoiTuChoi, g => g.KhoaKho);

            var queryKhoa = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.KhoDuocDuyet == false)
                                                                   .Select(s => new DuTruMuaDuocPhamTuChoiGridVo()
                                                                   {
                                                                       Id = s.Id,
                                                                       SoPhieu = s.SoPhieu,
                                                                       KhoaKho = s.KhoaPhong.Ten,
                                                                       KhoaId = s.KhoaPhongId,
                                                                       KhoId = 0,
                                                                       KyDuTru = s.TuNgay.ApplyFormatDate() + '-' + s.DenNgay.ApplyFormatDate(),
                                                                       NgayTuChoi = s.NgayKhoDuocDuyet,
                                                                       NgayYeuCau = s.NgayYeuCau,
                                                                       NguoiTuChoiId = s.NhanVienKhoDuocId,
                                                                       NguoiYeuCauId = s.NhanVienYeuCauId,
                                                                       LyDo = s.LyDoKhoDuocTuChoi,
                                                                       NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                                                                       NguoiTuChoi = s.NhanVienKhoDuoc.User.HoTen,
                                                                       TinhTrang = EnumTrangThaiLoaiDuTruDaXuLy.TuChoi,
                                                                       LoaiKho = EnumLoaiKhoDuocPham.KhoLe
                                                                   }
            ).ApplyLike(queryInfo.SearchTerms, g => g.SoPhieu, g => g.LyDo, g => g.NguoiYeuCau, g => g.NguoiTuChoi,g=>g.KhoaKho);
            var query = queryKho.Concat(queryKhoa);


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaDuocTuChoiSearch>(queryInfo.AdditionalSearchString);

                if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;

                    query = query.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayYeuCau <= denNgay);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null && queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryObject.SearchString != null)
                {
                    var search = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(search,
                    q => q.SoPhieu,
                    q => q.KhoaKho,
                    q => q.NguoiYeuCau,
                    g => g.LyDo,
                    g => g.NguoiTuChoi
                   );
                }
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaDuocPhamId);
            var khoId = Convert.ToInt32(queryString[1]);
            int khoaId = Convert.ToInt32(queryString[2]);
            if (khoId != 0)
            {
                var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                           .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               Id = p.Id,
                               Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                               LoaiDuocPham = p.LaDuocPhamBHYT,
                               DuocPhamId = p.DuocPhamId,
                               DuocPham = p.DuocPham.Ten,
                               HoatChat = p.DuocPham.HoatChat,
                               NongDoVaHamLuong = p.DuocPham.HamLuong,
                               SDK = p.DuocPham.SoDangKy,
                               DVT = p.DuocPham.DonViTinh.Ten,
                               DD = p.DuocPham.DuongDung.Ten,
                               NhaSX = p.DuocPham.NhaSanXuat,
                               NuocSX = p.DuocPham.NuocSanXuat,
                               SLDuTru = p.SoLuongDuTru,
                               TrangThai = EnumTrangThaiLoaiDuTru.ChoGoi,
                               SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                               LoaiKhoHayKhoa = true, // khoid
                               NhomDieuTri = p.NhomDieuTriDuPhong,
                               SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                               KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                           && x.NhapKhoDuocPhams.KhoId == khoId
                                                                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                           && x.NhapKhoDuocPhams.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                           && 
                                                                                           x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                           && x.NhapKhoDuocPhams.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                               TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                           && 
                                                                                           x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                           && x.NhapKhoDuocPhams.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                           {
                                                                                               TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                               TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                           }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                               HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                               HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                .Select(e => new HopDongChuaNhapDuoc
                                                                                                {
                                                                                                    TenHopDong = e.First().TenHopDong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                           }).GroupBy(x => new
                           {
                               x.DuocPhamId,
                               x.Loai
                           }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                               Loai = item.First().Loai,
                               DuocPhamId = item.First().DuocPhamId,
                               DuocPham = item.First().DuocPham,
                               HoatChat = item.First().HoatChat,
                               NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                               SDK = item.First().SDK,
                               DVT = item.First().DVT,
                               DD = item.First().DD,
                               NhaSX = item.First().NhaSX,
                               NuocSX = item.First().NuocSX,
                               KhoaPhongId = item.First().KhoaPhongId,
                               TrangThai = item.First().TrangThai,
                               SLDuTru = item.First().SLDuTru,
                               SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                               KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                               KhoTongTon = item.Sum(x => x.KhoTongTon),
                               HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                               HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                               NhomDieuTri= item.FirstOrDefault().NhomDieuTri,
                               SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                               TongTonList = item.FirstOrDefault().TongTonList,
                               LoaiDuocPham = item.FirstOrDefault().LoaiDuocPham,
                               Id = item.FirstOrDefault().Id
                           });
                var dataOrderBy = query.ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                return new GridDataSource { Data = data };
            }
            if (khoaId != 0)
            {
                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamId)
                               .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo()
                               {
                                   Id = p.Id,
                                   Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                   LoaiDuocPham = p.LaDuocPhamBHYT,
                                   DuocPhamId = p.DuocPhamId,
                                   DuocPham = p.DuocPham.Ten,
                                   HoatChat = p.DuocPham.HoatChat,
                                   NongDoVaHamLuong = p.DuocPham.HamLuong,
                                   SDK = p.DuocPham.SoDangKy,
                                   DVT = p.DuocPham.DonViTinh.Ten,
                                   DD = p.DuocPham.DuongDung.Ten,
                                   NhaSX = p.DuocPham.NhaSanXuat,
                                   NuocSX = p.DuocPham.NuocSanXuat,
                                   KhoaPhongId = khoaId,
                                   SLDuTru = p.SoLuongDuTru,
                                   SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                   TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                   LoaiKhoHayKhoa = false,// khoaid
                                   KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                              && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                               && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                   SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,
                                   TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                               && 
                                                                                               x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                               && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                               {
                                                                                                   TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                   TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                               }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),

                                   HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                   HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                .Select(e => new HopDongChuaNhapDuoc
                                                                                                {
                                                                                                    TenHopDong = e.First().TenHopDong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),

                               }).GroupBy(x => new
                               {
                                   x.DuocPhamId,
                                   x.Loai
                               }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo
                               {
                                   LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                                   Id = item.First().Id,
                                   LoaiDuocPham = item.First().LoaiDuocPham,
                                   TrangThai = item.First().TrangThai,
                                   Loai = item.First().Loai,
                                   DuocPhamId = item.First().DuocPhamId,
                                   DuocPham = item.First().DuocPham,
                                   HoatChat = item.First().HoatChat,
                                   NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                                   SDK = item.First().SDK,
                                   DVT = item.First().DVT,
                                   DD = item.First().DD,
                                   NhaSX = item.First().NhaSX,
                                   NuocSX = item.First().NuocSX,
                                   KhoaPhongId = item.First().KhoaPhongId,
                                   SLDuTru = item.First().SLDuTru,
                                   SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                                   KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                                   KhoTongTon = item.Sum(x => x.KhoTongTon),
                                   HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                                   SLDuTruKDuocDuyet = item.FirstOrDefault().SLDuTruKDuocDuyet,
                                   HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                                   NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                                   SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                                   TongTonList = item.FirstOrDefault().TongTonList,
                               });
                var dataOrderBy = query.ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

                var resdataOrderBy = dataOrderBy.Select(o =>
                {

                    var listKhoTheoKhoaPhong = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.Id == duTruMuaDuocPhamId).SelectMany(s => s.DuTruMuaDuocPhams).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == o.DuocPhamId
                                                                                               && x.NhapKhoDuocPhams.KhoId == lst.KhoId
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.LaDuocPhamBHYT == o.LoaiDuocPham
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    }
                    return o;
                }
                );
                var queryTask = resdataOrderBy.ToArray();
                return new GridDataSource { Data = queryTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaDuocPhamId);
            var khoId = Convert.ToInt32(queryString[1]);
            int khoaId = Convert.ToInt32(queryString[2]);
            if (khoId != 0)
            {
                var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                           .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               Id = p.Id,
                               Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                               LoaiDuocPham = p.LaDuocPhamBHYT,
                               DuocPhamId = p.DuocPhamId,
                               DuocPham = p.DuocPham.Ten,
                               HoatChat = p.DuocPham.HoatChat,
                               NongDoVaHamLuong = p.DuocPham.HamLuong,
                               SDK = p.DuocPham.SoDangKy,
                               DVT = p.DuocPham.DonViTinh.Ten,
                               DD = p.DuocPham.DuongDung.Ten,
                               NhaSX = p.DuocPham.NhaSanXuat,
                               NuocSX = p.DuocPham.NuocSanXuat,
                               SLDuTru = p.SoLuongDuTru,
                               TrangThai = EnumTrangThaiLoaiDuTru.ChoGoi,
                               SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                               LoaiKhoHayKhoa = true, // khoid
                               SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet,
                               KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                           && x.NhapKhoDuocPhams.KhoId == khoId
                                                                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                           && x.NhapKhoDuocPhams.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                           && 
                                                                                           x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                           && x.NhapKhoDuocPhams.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                               TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                           && 
                                                                                           x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                           && x.NhapKhoDuocPhams.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                           {
                                                                                               TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                               TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                           }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                               HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                               HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                .Select(e => new HopDongChuaNhapDuoc
                                                                                                {
                                                                                                    TenHopDong = e.First().TenHopDong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),
                           }).GroupBy(x => new
                           {
                               x.DuocPhamId,
                               x.Loai
                           }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                               Loai = item.First().Loai,
                               DuocPhamId = item.First().DuocPhamId,
                               DuocPham = item.First().DuocPham,
                               HoatChat = item.First().HoatChat,
                               NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                               SDK = item.First().SDK,
                               DVT = item.First().DVT,
                               DD = item.First().DD,
                               NhaSX = item.First().NhaSX,
                               NuocSX = item.First().NuocSX,
                               KhoaPhongId = item.First().KhoaPhongId,
                               TrangThai = item.First().TrangThai,
                               SLDuTru = item.First().SLDuTru,
                               SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                               KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                               KhoTongTon = item.Sum(x => x.KhoTongTon),
                               HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                               HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                               NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                               SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                               TongTonList = item.FirstOrDefault().TongTonList,
                               LoaiDuocPham = item.FirstOrDefault().LoaiDuocPham,
                               Id = item.FirstOrDefault().Id
                           });
                var queryTask = query.Count();
                return new GridDataSource { TotalRowCount = queryTask };
            }
            if (khoaId != 0)
            {
                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamId)
                               .Select(p => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo()
                               {
                                   Id = p.Id,
                                   Loai = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                   LoaiDuocPham = p.LaDuocPhamBHYT,
                                   DuocPhamId = p.DuocPhamId,
                                   DuocPham = p.DuocPham.Ten,
                                   HoatChat = p.DuocPham.HoatChat,
                                   NongDoVaHamLuong = p.DuocPham.HamLuong,
                                   SDK = p.DuocPham.SoDangKy,
                                   DVT = p.DuocPham.DonViTinh.Ten,
                                   DD = p.DuocPham.DuongDung.Ten,
                                   NhaSX = p.DuocPham.NhaSanXuat,
                                   NuocSX = p.DuocPham.NuocSanXuat,
                                   KhoaPhongId = khoaId,
                                   SLDuTru = p.SoLuongDuTru,
                                   SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                   TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                   LoaiKhoHayKhoa = false,// khoaid
                                   KhoTongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                              && x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                               && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   SLDuTruTKhoaDuyet = p.SoLuongDuTru,
                                   SLDuTruKDuocDuyet = p.SoLuongDuTru,// todo
                                   TongTonList = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == p.DuocPhamId
                                                                                               && 
                                                                                               x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                                               && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTonDuocPham()
                                                                                               {
                                                                                                   TenKhoTong = x.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                                                   TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                               }).GroupBy(q => q.TenKhoTong)
                                                                                                .Select(e => new KhoTongTonDuocPham
                                                                                                {
                                                                                                    TenKhoTong = e.First().TenKhoTong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList(),

                                   HDChuaNhap = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                   HopDongChuahapList = _hopDongThauDuocPhamChiTiet.TableNoTracking.Where(x => x.DuocPhamId == p.DuocPhamId)
                                                                                                .Select(z => new HopDongChuaNhapDuoc()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauDuocPham.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                .Select(e => new HopDongChuaNhapDuoc
                                                                                                {
                                                                                                    TenHopDong = e.First().TenHopDong,
                                                                                                    TongTon = e.Sum(q => q.TongTon)
                                                                                                }).ToList()
                               }).GroupBy(x => new
                               {
                                   x.DuocPhamId,
                                   x.Loai
                               }).Select(item => new DuTruMuaDuocPhamTaiKhoaDuocChildGridVo
                               {
                                   LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                                   Id = item.First().Id,
                                   LoaiDuocPham = item.First().LoaiDuocPham,
                                   TrangThai = item.First().TrangThai,
                                   Loai = item.First().Loai,
                                   DuocPhamId = item.First().DuocPhamId,
                                   DuocPham = item.First().DuocPham,
                                   HoatChat = item.First().HoatChat,
                                   NongDoVaHamLuong = item.First().NongDoVaHamLuong,
                                   SDK = item.First().SDK,
                                   DVT = item.First().DVT,
                                   DD = item.First().DD,
                                   NhaSX = item.First().NhaSX,
                                   NuocSX = item.First().NuocSX,
                                   KhoaPhongId = item.First().KhoaPhongId,
                                   SLDuTru = item.First().SLDuTru,
                                   SLDuKienSuDungTrongKy = item.First().SLDuKienSuDungTrongKy,
                                   KhoDuTruTon = item.Sum(x => x.KhoDuTruTon),
                                   KhoTongTon = item.Sum(x => x.KhoTongTon),
                                   HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                                   SLDuTruKDuocDuyet = item.FirstOrDefault().SLDuTruKDuocDuyet,
                                   HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                                   NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                                   SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                                   TongTonList = item.FirstOrDefault().TongTonList,
                               });
                var dataOrderBy = query.ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

                var resdataOrderBy = dataOrderBy.Select(o =>
                {

                    var listKhoTheoKhoaPhong = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking.Where(x => x.Id == duTruMuaDuocPhamId).SelectMany(s => s.DuTruMuaDuocPhams).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.DuocPhamBenhVienId == o.DuocPhamId
                                                                                               && x.NhapKhoDuocPhams.KhoId == lst.KhoId
                                                                                               && x.NhapKhoDuocPhams.DaHet != true
                                                                                               && x.LaDuocPhamBHYT == o.LoaiDuocPham
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    }
                    return o;
                }
                );
                var queryTask = resdataOrderBy.Count();
                return new GridDataSource { TotalRowCount = queryTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruDuocPhamChiTietId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            var laDPHYT = Convert.ToBoolean(queryString[2]);
            if (loaiDP == true)  // khoid
            {
                var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == laDPHYT)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                 });
                var dataOrderBy = query.AsQueryable();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };

            }
            if (loaiDP == false)  // khoaId
            {

                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == laDPHYT)
                                                               .SelectMany(s => s.DuTruMuaDuocPhamChiTiets)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung,
                     NhomDieuTriDuPhong = cc.NhomDieuTriDuPhong != null ? cc.NhomDieuTriDuPhong.GetDescription():""
                 });
                var dataOrderBy = query.AsQueryable();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };

            }
            return null;
        }
        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruDuocPhamChiTietId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            var laDPHYT = Convert.ToBoolean(queryString[2]);
            if (loaiDP == true)  // khoid
            {
                var query = _duTruMuaDuocPhamChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == laDPHYT)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                 });
                var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };

            }
            if (loaiDP == false)  // khoaId
            {

                var query = _duTruMuaDuocPhamTheoKhoaChiTietRepo.TableNoTracking.Where(x => x.Id == duTruDuocPhamChiTietId
                                                               && x.LaDuocPhamBHYT == laDPHYT)
                                                               .SelectMany(s => s.DuTruMuaDuocPhamChiTiets)
                 .Select(cc => new DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo()
                 {
                     Nhom = cc.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                     Kho = cc.DuTruMuaDuocPham.Kho.Ten,
                     KyDuTru = cc.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                 });
                var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };

            }
            return null;
        }
        #endregion
        #region gởi
        public DuTruMuaDuocPhamChiTietGoiViewGridVo GetDuTruMuaDuocPhamChiTietGoi(long idDuTruMua)
        {
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == nguoiDangLogin).Select(s => s.User.HoTen).FirstOrDefault();
            var listDuTruDuocPhamTheoKhoaChiTiet = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
                                                                    .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.KhoDuocDuyet == true && s.DuTruMuaDuocPhamKhoDuocId == null)
                                                                    .SelectMany(o => o.DuTruMuaDuocPhamTheoKhoaChiTiets)
                                                                    .Include(o => o.DuTruMuaDuocPhamTheoKhoa).ThenInclude(o => o.DuTruMuaDuocPhams).Include(o => o.DuTruMuaDuocPhamChiTiets)
                                                                    .Include(o => o.DuTruMuaDuocPhamTheoKhoa).ThenInclude(o => o.KhoaPhong)
                                                                    .Include(o => o.DuocPham).ThenInclude(o => o.DonViTinh)
                                                                    .Include(o => o.DuocPham).ThenInclude(o => o.DuongDung);
            var listDuTruDuocPhamChiIiet = BaseRepository.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.TruongKhoaDuyet == true && s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && s.DuTruMuaDuocPhamKhoDuocId == null)
                                                                    .SelectMany(o => o.DuTruMuaDuocPhamChiTiets)
                                                                    .Include(o => o.DuTruMuaDuocPham).ThenInclude(o => o.Kho)
                                                                    .Include(o => o.DuocPham).ThenInclude(o => o.DonViTinh)
                                                                    .Include(o => o.DuocPham).ThenInclude(o => o.DuongDung).ToList();
            var duTruTheoKhoa = listDuTruDuocPhamTheoKhoaChiTiet.Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList()
            {
                Id = itemc.Id,
                DuTruMuaDuocPhamTheoKhoaChiTietId = itemc.Id,
                //DuTruMuaDuocPhamKhoaDuId = itemc.Id,
                Khoa = itemc.DuTruMuaDuocPhamTheoKhoa.KhoaPhong.Ten,
                Loai = itemc.LaDuocPhamBHYT,
                DuocPhamId = itemc.DuocPhamId,
                TenDuocPham = itemc.DuocPham.Ten,
                HoatChat = itemc.DuocPham.HoatChat,
                NongDoVaHamLuong = itemc.DuocPham.HamLuong,
                SDK = itemc.DuocPham.SoDangKy,
                DVT = itemc.DuocPham.DonViTinh.Ten,
                DD = itemc.DuocPham.DuongDung.Ten,
                NhaSX = itemc.DuocPham.NhaSanXuat,
                NuocSX = itemc.DuocPham.NuocSanXuat,
                SLDuTru = itemc.SoLuongDuTru,
                SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ? itemc.SoLuongDuTru : itemc.SoLuongDuTruTruongKhoaDuyet,
                SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruKhoDuocDuyet == null ? itemc.SoLuongDuTru : itemc.SoLuongDuTruKhoDuocDuyet,
                DuocPhamDuTruTheoKhoaId = itemc.DuTruMuaDuocPhamTheoKhoaId,
                DuTruMuaDuocPhamTheoKhoaId = itemc.DuTruMuaDuocPhamTheoKhoaId,
            }).ToList();
            var duTruTaiKho = listDuTruDuocPhamChiIiet.Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList()
            {
                Id = itemc.Id,
                DuTruMuaDuocPhamChiTietId = itemc.Id,
                //DuTruMuaDuocPhamKhoId = itemc.Id,
                DuTruMuaDuocPhamId = itemc.DuTruMuaDuocPhamId,
                DuTruMuaDuocPhamTheoKhoaId = 0,
                Khoa = "Kho Duoc",
                Loai = itemc.LaDuocPhamBHYT,
                DuocPhamId = itemc.DuocPhamId,
                TenDuocPham = itemc.DuocPham.Ten,
                HoatChat = itemc.DuocPham.HoatChat,
                NongDoVaHamLuong = itemc.DuocPham.HamLuong,
                SDK = itemc.DuocPham.SoDangKy,
                DVT = itemc.DuocPham.DonViTinh.Ten,
                DD = itemc.DuocPham.DuongDung.Ten,
                NhaSX = itemc.DuocPham.NhaSanXuat,
                NuocSX = itemc.DuocPham.NuocSanXuat,
                SLDuTru = itemc.SoLuongDuTru,
                SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ? itemc.SoLuongDuTru : itemc.SoLuongDuTruTruongKhoaDuyet,
                SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ? itemc.SoLuongDuTru : itemc.SoLuongDuTruTruongKhoaDuyet,
            }).ToList();
            var dutruUnion = duTruTheoKhoa.Union(duTruTaiKho);
            var dutrus = dutruUnion.GroupBy(x => new
            {
                x.Loai,
                x.DuocPhamId
            }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList()
            {
                Id = itemcc.FirstOrDefault().Id,
                Loai = itemcc.FirstOrDefault().Loai,
                DuocPhamId = itemcc.First().DuocPhamId,
                TenDuocPham = itemcc.First().TenDuocPham,
                HoatChat = itemcc.First().HoatChat,
                NongDoVaHamLuong = itemcc.First().NongDoVaHamLuong,
                SDK = itemcc.FirstOrDefault()?.SDK,
                DVT = itemcc.FirstOrDefault()?.DVT,
                DD = itemcc.FirstOrDefault()?.DD,
                NhaSX = itemcc.FirstOrDefault()?.NhaSX,
                NuocSX = itemcc.FirstOrDefault()?.NuocSX,
                SLDuTru = itemcc.Sum(x => x.SLDuTru),
                SLDuKienSuDungTrongKho = itemcc.Sum(x => x.SLDuKienSuDungTrongKho),
                SLDuTruTKhoDuyet = itemcc.Sum(x => x.SLDuTruTKhoDuyet),
                SLDuTruKhoDuocDuyet = itemcc.Sum(x => x.SLDuTruKhoDuocDuyet),
            }).ToList();
            if (dutrus.Count() > 0)
            {
                var listKho = _khoRepo.TableNoTracking.ToList();
                foreach (var dutruTong in dutrus)
                {
                    foreach (var dutru in dutruUnion.Where(o => o.DuocPhamId == dutruTong.DuocPhamId && o.Loai == dutruTong.Loai))
                    {
                        if (dutru.DuTruMuaDuocPhamTheoKhoaId != 0)
                        {
                            var listDuTruMuaDuocPham = listDuTruDuocPhamTheoKhoaChiTiet.First(o => o.DuTruMuaDuocPhamTheoKhoaId == dutru.DuTruMuaDuocPhamTheoKhoaId).DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhams;
                            var itemChild = listDuTruDuocPhamTheoKhoaChiTiet.Where(o => o.DuTruMuaDuocPhamTheoKhoaId == dutru.DuTruMuaDuocPhamTheoKhoaId && o.DuocPhamId == dutruTong.DuocPhamId && o.LaDuocPhamBHYT == dutruTong.Loai)
                                .SelectMany(o => o.DuTruMuaDuocPhamChiTiets).Select(o => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListGoiChild
                                {
                                    Nhom = listDuTruMuaDuocPham.First(x => x.Id == o.DuTruMuaDuocPhamId).NhomDuocPhamDuTru.GetDescription(),
                                    Kho = listKho.Where(x => x.Id == listDuTruMuaDuocPham.First(k => k.Id == o.DuTruMuaDuocPhamId).KhoId).First().Ten,
                                    KyDuTru = o.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + o.DuTruMuaDuocPham.TuNgay.ApplyFormatDate(),
                                    SLDuKienTrongKy = o.SoLuongDuKienSuDung,
                                    SLDuTru = o.SoLuongDuTru,
                                    DuTruMuaDuocPhamTheoKhoaChiTietId = o.DuTruMuaDuocPhamTheoKhoaChiTietId.GetValueOrDefault(),
                                    DuTruMuaDuocPhamTheoKhoaId = o.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoaId,
                                    Khoa = o.DuTruMuaDuocPhamTheoKhoaChiTiet != null && o.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoa != null &&
                                          o.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoa.KhoaPhong != null ? o.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoa.KhoaPhong.Ten : null,
                                    NhomDieuTriDuPhong = o.NhomDieuTriDuPhong != null ? o.NhomDieuTriDuPhong.GetDescription() : ""
                                }).ToList();
                            dutruTong.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild.AddRange(itemChild);
                        }
                        if (dutru.DuTruMuaDuocPhamId != 0)
                        {
                            var test = listDuTruDuocPhamChiIiet.Where(o => o.DuTruMuaDuocPhamId == dutru.DuTruMuaDuocPhamId && o.DuocPhamId == dutruTong.DuocPhamId && o.LaDuocPhamBHYT == dutruTong.Loai);
                            var itemChild = listDuTruDuocPhamChiIiet.Where(o => o.DuTruMuaDuocPhamId == dutru.DuTruMuaDuocPhamId && o.DuocPhamId == dutruTong.DuocPhamId && o.LaDuocPhamBHYT == dutruTong.Loai)
                                .Select(o => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListGoiChild
                                {
                                    Nhom = o.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                                    Kho = o.DuTruMuaDuocPham.Kho.Ten,
                                    KyDuTru = o.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + '-' + o.DuTruMuaDuocPham.TuNgay.ApplyFormatDate(),
                                    SLDuKienTrongKy = o.SoLuongDuKienSuDung,
                                    SLDuTru = o.SoLuongDuTru,
                                    DuTruMuaDuocPhamId = o.DuTruMuaDuocPhamId,
                                    DuTruMuaDuocPhamChiTietId = o.Id,
                                    Khoa = "Khoa Dược",
                                    NhomDieuTriDuPhong = o.NhomDieuTriDuPhong != null ? o.NhomDieuTriDuPhong.GetDescription() : ""
                                }).ToList();
                            dutruTong.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild.AddRange(itemChild);
                        }
                    }
                }
            }




            var listDuTruDuocPhamTheoKhoaId = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
                                                                    .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.KhoDuocDuyet == true && s.DuTruMuaDuocPhamKhoDuocId == null)
                                                                    .Select(x => x.Id).ToList();
            var duTruDuocPhamId = BaseRepository.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.TruongKhoaDuyet == true && s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && s.DuTruMuaDuocPhamKhoDuocId == null)
                                                                    .Select(x => x.Id).ToList();
            var queryChiTietKhoLe = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.KhoDuocDuyet == true && s.KhoaPhongId != null && s.DuTruMuaDuocPhamKhoDuocId == null)
                                          .Select(item => new DuTruMuaDuocPhamChiTietGoiViewGridVo()
                                          {
                                              Id = item.Id,
                                              KyDuTru = item.TuNgay.ApplyFormatDate() + '-' + item.DenNgay.ApplyFormatDate(),
                                              KyDuTruId = item.KyDuTruMuaDuocPhamVatTuId,
                                              NguoiYeuCauId = nguoiDangLogin,
                                              TenNguoiYeuCau = nguoiLogin,
                                              NgayYeuCau = item.NgayYeuCau,
                                              GhiChu = item.GhiChu,
                                              TuNgay = item.TuNgay,
                                              DenNgay = item.DenNgay,
                                              DuTruDuocPhamTheoKhoaId = item.Id,
                                              ListDuTruDuocPhamTheoKhoaId = listDuTruDuocPhamTheoKhoaId,
                                              TrangThaiHienThi = EnumTrangThaiDuTruKhoaDuoc.ChoGoi.GetDescription()
                                          }).ToList();
            var queryChiTietKhacKhoLe = BaseRepository.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.TruongKhoaDuyet == true && s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && s.DuTruMuaDuocPhamKhoDuocId == null)
                                          .Select(item => new DuTruMuaDuocPhamChiTietGoiViewGridVo()
                                          {
                                              Id = item.Id,
                                              KyDuTru = item.TuNgay.ApplyFormatDate() + '-' + item.DenNgay.ApplyFormatDate(),
                                              KyDuTruId = item.KyDuTruMuaDuocPhamVatTuId,
                                              NguoiYeuCauId = nguoiDangLogin,
                                              TenNguoiYeuCau = nguoiLogin,
                                              NgayYeuCau = item.NgayYeuCau,
                                              GhiChu = item.GhiChu,
                                              TuNgay = item.TuNgay,
                                              DenNgay = item.DenNgay,
                                              ListDuTruDuocPhamId = duTruDuocPhamId,
                                              TrangThaiHienThi = EnumTrangThaiDuTruKhoaDuoc.ChoGoi.GetDescription()
                                          }).ToList();
            List<DuTruMuaDuocPhamChiTietGoiViewGridVo> list = new List<DuTruMuaDuocPhamChiTietGoiViewGridVo>();
            if (queryChiTietKhoLe.Count() > 0 && queryChiTietKhacKhoLe.Count() > 0)
            {
                list = queryChiTietKhoLe.Union(queryChiTietKhacKhoLe).Select(item => new DuTruMuaDuocPhamChiTietGoiViewGridVo()
                {
                    Id = item.Id,
                    KyDuTru = item.KyDuTru,
                    KyDuTruId = item.KyDuTruId,
                    NguoiYeuCauId = item.NguoiYeuCauId,
                    TenNguoiYeuCau = item.TenNguoiYeuCau,
                    NgayYeuCau = item.NgayYeuCau,
                    GhiChu = item.GhiChu,
                    TuNgay = item.TuNgay,
                    DenNgay = item.DenNgay,
                    ListDuTruDuocPhamId = item.ListDuTruDuocPhamId,
                    thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = dutrus,
                }).ToList();
            }
            if (queryChiTietKhoLe.Count() > 0 && queryChiTietKhacKhoLe.Count() == 0)
            {
                list = queryChiTietKhoLe.Select(item => new DuTruMuaDuocPhamChiTietGoiViewGridVo()
                {
                    Id = item.Id,
                    KyDuTru = item.KyDuTru,
                    KyDuTruId = item.KyDuTruId,
                    NguoiYeuCauId = item.NguoiYeuCauId,
                    TenNguoiYeuCau = item.TenNguoiYeuCau,
                    NgayYeuCau = item.NgayYeuCau,
                    GhiChu = item.GhiChu,
                    TuNgay = item.TuNgay,
                    DenNgay = item.DenNgay,
                    ListDuTruDuocPhamId = item.ListDuTruDuocPhamId,
                    thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = dutrus,
                }).ToList();
            }
            if (queryChiTietKhoLe.Count() == 0 && queryChiTietKhacKhoLe.Count() > 0)
            {
                list = queryChiTietKhacKhoLe.Select(item => new DuTruMuaDuocPhamChiTietGoiViewGridVo()
                {
                    Id = item.Id,
                    KyDuTru = item.KyDuTru,
                    KyDuTruId = item.KyDuTruId,
                    NguoiYeuCauId = item.NguoiYeuCauId,
                    TenNguoiYeuCau = item.TenNguoiYeuCau,
                    NgayYeuCau = item.NgayYeuCau,
                    GhiChu = item.GhiChu,
                    TuNgay = item.TuNgay,
                    DenNgay = item.DenNgay,
                    ListDuTruDuocPhamId = item.ListDuTruDuocPhamId,
                    thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = dutrus
                }).ToList();
            }
            return list.FirstOrDefault();
        }
        #endregion
        #region In
        public string InPhieuDuTruMuaTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var contentThuoc = string.Empty;
            var contentHoaChat = string.Empty;
            var hearder = string.Empty;
            var templatePhieuInTongHopDuTruDuocPhamTaiKhoa = new Template();

            //if (phieuInDuTruMuaTaiKhoa.Header)
            //{
            //    hearder = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                           "<th>PHIẾU TỔNG HỢP DỰ TRÙ THUỐC,VÁC XIN,HÓA CHẤT XÉT NGHIỆM</th>" +
            //                      "</p>";
            //}

            var groupBHYT = "BHYT";
            var groupKhongBHYT = "Không BHYT";
            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                     + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupBHYT.ToUpper()
                                     + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupKhongBHYT.ToUpper()
                                        + "</b></tr>";
            var tuNgay = string.Empty;
            var denNgay = string.Empty;

            var kyDuTruId = _duTruMuaDuocPhamTheoKhoaRepo.TableNoTracking
                                            .Where(p => p.DuTruMuaDuocPhamKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId) // kho duoc Id
                                            .SelectMany(d => d.DuTruMuaDuocPhams)
                                            .Select(d=>d.KyDuTruMuaDuocPhamVatTuId)
                                            .ToList();
            if(kyDuTruId.Count() > 0)
            {
                var kyDuTruInfo = _kyDuTruMuaDuocPhamVatTuRepository.TableNoTracking.Where(d => d.Id == kyDuTruId.First())
                    .Select(d => new {
                        TuNgay = d.TuNgay.ApplyFormatDate(),
                        DenNgay = d.DenNgay.ApplyFormatDate()
                    }).First();
                if(kyDuTruInfo != null)
                {
                    tuNgay = kyDuTruInfo.TuNgay;
                    denNgay = kyDuTruInfo.DenNgay;
                }
            }



            var duTruMuaDuocPhamChiTiet = string.Empty;
            templatePhieuInTongHopDuTruDuocPhamTaiKhoa = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuInTongHopDuTruDuocPhamTaiKhoa")).First();
            var duTruMuaDuocPhamKhoaChiTiets = _duTruMuaDuocPhamTheoKhoaChiTietRepository.TableNoTracking
                                            .Where(p => p.DuTruMuaDuocPhamKhoDuocChiTiet.DuTruMuaDuocPhamKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId) // khoaduocId
                                            .Select(s => new PhieuMuaDuTruDuocPhamChiTietData
                                            {
                                                MaHang = s.DuocPham.MaHoatChat, // todo: cần confirm hỏi lại
                                                Ten = s.DuocPham.Ten,
                                                HoatChat = s.DuocPham.HoatChat,
                                                HamLuong = s.DuocPham.HamLuong,
                                                DonVi = s.DuocPham.DonViTinh.Ten,
                                                SoLuong = s.SoLuongDuTruTruongKhoaDuyet,
                                                GhiChu = "",
                                                LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                                DuocPhamId = s.DuocPhamId
                                            }).GroupBy(x => new
                                            {
                                                x.LaDuocPhamBHYT,
                                                x.DuocPhamId
                                            })
                                            .Select(item => new PhieuMuaDuTruDuocPhamChiTietData {
                                                MaHang = item.First().MaHang, // todo: cần confirm hỏi lại
                                                Ten = item.First().Ten,
                                                HoatChat = item.First().HoatChat,
                                                HamLuong = item.First().HamLuong,
                                                DonVi = item.First().DonVi,
                                                SoLuong = item.Sum(f=>f.SoLuong),
                                                GhiChu = item.First().GhiChu,
                                                LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                                                DuocPhamId = item.First().DuocPhamId
                                            }).ToList();
            var duTruMuaDuocPhamChiTiets = _duTruMuaDuocPhamChiTietRepo.TableNoTracking
                                          .Where(p => p.DuTruMuaDuocPhamKhoDuocChiTiet.DuTruMuaDuocPhamKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId) // khoaduocId
                                          .Select(s => new PhieuMuaDuTruDuocPhamChiTietData
                                          {
                                              MaHang = s.DuocPham.MaHoatChat, // todo: cần confirm hỏi lại
                                              Ten = s.DuocPham.Ten,
                                              HoatChat = s.DuocPham.HoatChat,
                                              HamLuong = s.DuocPham.HamLuong,
                                              DonVi = s.DuocPham.DonViTinh.Ten,
                                              SoLuong = (int)s.SoLuongDuTruTruongKhoaDuyet,
                                              GhiChu = "",
                                              LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                              DuocPhamId = s.DuocPhamId
                                          }).GroupBy(x => new
                                          {
                                              x.LaDuocPhamBHYT,
                                              x.DuocPhamId
                                          })
                                            .Select(item => new PhieuMuaDuTruDuocPhamChiTietData
                                            {
                                                MaHang = item.First().MaHang, // todo: cần confirm hỏi lại
                                                Ten = item.First().Ten,
                                                HoatChat = item.First().HoatChat,
                                                HamLuong = item.First().HamLuong,
                                                DonVi = item.First().DonVi,
                                                SoLuong = item.Sum(f => f.SoLuong),
                                                GhiChu = item.First().GhiChu,
                                                LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                                                DuocPhamId = item.First().DuocPhamId
                                            }).ToList(); ;
            var query = duTruMuaDuocPhamKhoaChiTiets.Union(duTruMuaDuocPhamChiTiets).ToList();
            var STT = 1;

            if (query.Any(p => p.LaDuocPhamBHYT))
            {
                duTruMuaDuocPhamChiTiet += headerBHYT;
                var queryBHYT = query.Where(x => x.LaDuocPhamBHYT).ToList();
                foreach (var item in queryBHYT)
                {
                    duTruMuaDuocPhamChiTiet = duTruMuaDuocPhamChiTiet
                                    + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                    + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.MaHang
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.Ten
                                            + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.DonVi
                                    + "<td style = 'border: 1px solid #020000;text-align: right;  padding-right:3px'>" + item.SoLuong
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.GhiChu
                                    + "</tr>";
                    STT++;
                    groupBHYT = string.Empty;
                }
            }
            if (query.Any(p => !p.LaDuocPhamBHYT))
            {
                duTruMuaDuocPhamChiTiet += headerKhongBHYT;
                var queryKhongBHYT = query.Where(x => !x.LaDuocPhamBHYT).ToList();
                foreach (var item in queryKhongBHYT)
                {
                    duTruMuaDuocPhamChiTiet = duTruMuaDuocPhamChiTiet
                                     + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                     + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.MaHang
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.Ten
                                             + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.DonVi
                                     + "<td style = 'border: 1px solid #020000;text-align: right;  padding-right:3px'>" + item.SoLuong
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.GhiChu
                                     + "</tr>";
                    STT++;
                    groupKhongBHYT = string.Empty;
                }
            }
            var tennhanVienLap = _duTruMuaDuocPhamKhoDuocChiTietRepo.TableNoTracking.Include(s => s.DuTruMuaDuocPhamKhoDuoc).Where(c => c.DuTruMuaDuocPhamKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId).Select(x => x.DuTruMuaDuocPhamKhoDuoc.GiamDoc.User.HoTen).FirstOrDefault();
            var data = new
            {
                Header = hearder,
                MaPhieuMuaDuTruDuocPham = "BMBH-KD01.01",
                DuTruMuaDuocPhamChiTiet = duTruMuaDuocPhamChiTiet,
                KhoaPhong = "Khoa Dược",
                NhanVienLap = tennhanVienLap != null ? tennhanVienLap : "",

                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString(),
                TuNgay =tuNgay,
                DenNgay =denNgay
            };

            contentThuoc = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInTongHopDuTruDuocPhamTaiKhoa.Body, data);

            return contentThuoc;

        }
        #endregion
    }
}
