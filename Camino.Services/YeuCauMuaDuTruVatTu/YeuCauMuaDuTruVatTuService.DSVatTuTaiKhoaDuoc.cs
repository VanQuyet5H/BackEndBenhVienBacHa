using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoa;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.YeuCauMuaVatTu;

namespace Camino.Services.YeuCauMuaDuTruVatTu
{
    public partial class YeuCauMuaDuTruVatTuService
    {
        #region chờ xử lý
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new DuTruMuaVatTuTaiKhoaDuocSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaVatTuTaiKhoaDuocSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyetDuTruMua = GetDataYeuCauMuaVatTu(null, queryInfo, queryObject);
            var queryDangChoDuyetDuTruMuaTaiKhoa = GetDataYeuCauMuaVatTuTaiKhoaDuoc(null, queryInfo, queryObject);
            var queryChoGoiDuTruMu = GetDataYeuCauMuaVatTu(true, queryInfo, queryObject);
            var queryChoGoiDuTruMuTaiKhoa = GetDataYeuCauMuaVatTuTaiKhoaDuoc(true, queryInfo, queryObject);


            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaVatTuTaiKhoaDuocGridVo())
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new DuTruMuaVatTuTaiKhoaDuocSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaVatTuTaiKhoaDuocSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyetDuTruMua = GetDataYeuCauMuaVatTu(null, queryInfo, queryObject);
            var queryDangChoDuyetDuTruMuaTaiKhoa = GetDataYeuCauMuaVatTuTaiKhoaDuoc(null, queryInfo, queryObject);
            var queryChoGoiDuTruMu = GetDataYeuCauMuaVatTu(true, queryInfo, queryObject);
            var queryChoGoiDuTruMuTaiKhoa = GetDataYeuCauMuaVatTuTaiKhoaDuoc(true, queryInfo, queryObject);


            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaVatTuTaiKhoaDuocGridVo())
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
        private IQueryable<DuTruMuaVatTuTaiKhoaDuocGridVo> GetDataYeuCauMuaVatTu(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaVatTuTaiKhoaDuocSearch queryObject)
        {
            var result = BaseRepository.TableNoTracking.Include(cc => cc.NhanVienYeuCau)
                .Include(cc => cc.Kho)
                 .Include(cc => cc.NhanVienYeuCau).ThenInclude(x => x.User)
                .Where(p => p.TruongKhoaDuyet == duocDuyet && p.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && p.DuTruMuaVatTuKhoDuocId == null)
            .Select(o => new DuTruMuaVatTuTaiKhoaDuocGridVo
            {
                Id = o.Id,
                KhoId = (long)o.Kho.LoaiKho,
                SoPhieu = o.SoPhieu,
                DuTruMuaVatTuKhoDuocId = o.DuTruMuaVatTuKhoDuocId,
                TrangThaiDuTru = o.TruongKhoaDuyet == null ? (int)EnumTrangThaiLoaiDuTru.ChoDuyet : (int)EnumTrangThaiLoaiDuTru.ChoGoi,
                TrangThai = o.TruongKhoaDuyet == null ? EnumTrangThaiLoaiDuTru.ChoDuyet.GetDescription() : EnumTrangThaiLoaiDuTru.ChoGoi.GetDescription() + "-" + o.TuNgay.ApplyFormatDate() + "-" + o.DenNgay.ApplyFormatDate() + '-' + o.KyDuTruMuaDuocPhamVatTuId,
                KhoaKhoa = o.Kho.Ten,
                KhoaIdKhoId = o.KhoId,
                //LoaiNhom = o.NhomVatTuDuTru != null ? o.NhomVatTuDuTru.GetDescription() : "",
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                //DuTruMuaId = 1,
                TuNgay = o.TuNgay,
                DenNgay = o.DenNgay,
                KyDuTruMuaDuocPhamVatTuId = o.KyDuTruMuaDuocPhamVatTuId,
                NgayYeuCau = o.NgayYeuCau,
                NgayKhoaDuocDuyet = o.NgayTruongKhoaDuyet,
                KyDuTru = o.TuNgay.ApplyFormatDate() + '-' + o.DenNgay.ApplyFormatDate()
            }).ApplyLike(queryInfo.SearchTerms.Trim(),
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
        private IQueryable<DuTruMuaVatTuTaiKhoaDuocGridVo> GetDataYeuCauMuaVatTuTaiKhoaDuoc(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaVatTuTaiKhoaDuocSearch queryObject)
        {
            var result = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                .Where(p => p.KhoDuocDuyet == duocDuyet
                            && p.DuTruMuaVatTuKhoDuocId == null)
            .Select(o => new DuTruMuaVatTuTaiKhoaDuocGridVo
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
                       q => q.KhoaKhoa, q => q.NguoiYeuCau);
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaVatTuId);
            var loaiKho = Convert.ToInt32(queryString[1]);
            int trangThai = Convert.ToInt32(queryString[2]);
            long.TryParse(queryString[3], out long khoVatTuChiTietId);
            //Kho
            if (loaiKho != (int)EnumLoaiKhoDuocPham.KhoLe)
            {
                var dataList = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuId == duTruMuaVatTuId)
                            .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                            {
                                Id = p.Id,
                                LoaiVatTu = p.LaVatTuBHYT,
                                HoatChat = p.VatTu.QuyCach,
                                Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                VatTuId = p.VatTuId,
                                VatTu = p.VatTu.Ten,
                                DVT = p.VatTu.DonViTinh,
                                NhaSX = p.VatTu.NhaSanXuat,
                                NuocSX = p.VatTu.NuocSanXuat,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoId = p.DuTruMuaVatTu.KhoId,
                                KhoaPhongId = 0,
                                KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && x.NhapKhoVatTu.KhoId == khoVatTuChiTietId
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && (x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 ||
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && (x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 ||
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                 TenKhoTong = e.First().TenKhoTong,
                                                                                                 TongTon = e.Sum(q => q.TongTon)
                                                                                               }).ToList(),
                                HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId ).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId &&  x.SoLuong  > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap                                               
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                     TenHopDong = e.First().TenHopDong,
                                                                                                     TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                            }).ToList();
                var query = dataList.GroupBy(x => new
                {
                    x.VatTuId,
                    x.Loai
                }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien
                {
                    Loai = item.First().Loai,
                    VatTuId = item.First().VatTuId,
                    VatTu = item.First().VatTu,
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
                    HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                    HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                    NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                    //NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                    SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                    TongTonList = item.FirstOrDefault().TongTonList,
                    LoaiVatTu = item.First().LoaiVatTu,
                    TrangThai = item.First().TrangThai,
                    KhoId = item.First().KhoId,
                    KhoaPhongId = item.First().KhoaPhongId,
                    Id = item.First().Id
                });
                var dataOrderBy = query.AsQueryable().OrderBy(x => x.LoaiVatTu == true);
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            //Khoa
            else
            {
                long.TryParse(queryString[3], out long khoaPhongId);
                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuId)
                            .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                HoatChat = p.VatTu.QuyCach,
                                LoaiVatTu = p.LaVatTuBHYT,
                                VatTuId = p.VatTuId,
                                VatTu = p.VatTu.Ten,
                                DVT = p.VatTu.DonViTinh,
                                NhaSX = p.VatTu.NhaSanXuat,
                                NuocSX = p.VatTu.NuocSanXuat,
                                KhoaPhongId = khoaPhongId,
                                KhoId = 0,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            &&
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,// todo
                                TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                  TenKhoTong = e.First().TenKhoTong,
                                                                                                  TongTon = e.Sum(q => q.TongTon)
                                                                                              }).ToList(),
                                HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                            }).GroupBy(x => new
                            {
                                x.VatTuId,
                                x.Loai
                            }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiVatTu = item.First().LoaiVatTu,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                VatTuId = item.First().VatTuId,
                                VatTu = item.First().VatTu,
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
                                KhoId = item.First().KhoId,
                            });
                var dataOrderBy = query.OrderBy(x => x.LoaiVatTu == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = data.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.Id == duTruMuaVatTuId).SelectMany(s => s.DuTruMuaVatTus).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == o.VatTuId
                                                                                               && x.NhapKhoVatTu.KhoId == lst.KhoId
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                                && x.LaVatTuBHYT == o.LoaiVatTu
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                    }
                    return o;
                }
               );

                var queryTask = resdataOrderBy.ToArray();
                return new GridDataSource { Data = queryTask };

            }

        }

        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaVatTuId);
            var loaiKho = Convert.ToInt32(queryString[1]);
            int trangThai = Convert.ToInt32(queryString[2]);
            long.TryParse(queryString[3], out long khoVatTuChiTietId);
            //Kho
            if (loaiKho != (int)EnumLoaiKhoDuocPham.KhoLe)
            {
                var dataList = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuId == duTruMuaVatTuId)
                            .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                            {
                                Id = p.Id,
                                LoaiVatTu = p.LaVatTuBHYT,
                                HoatChat = p.VatTu.QuyCach,
                                Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                VatTuId = p.VatTuId,
                                VatTu = p.VatTu.Ten,
                                DVT = p.VatTu.DonViTinh,
                                NhaSX = p.VatTu.NhaSanXuat,
                                NuocSX = p.VatTu.NuocSanXuat,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoId = p.DuTruMuaVatTu.KhoId,
                                KhoaPhongId = 0,
                                KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && x.NhapKhoVatTu.KhoId == khoVatTuChiTietId
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                           &&
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                  TenKhoTong = e.First().TenKhoTong,
                                                                                                  TongTon = e.Sum(q => q.TongTon)
                                                                                              }).ToList(),
                                HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                            }).ToList();
                var query = dataList.GroupBy(x => new
                {
                    x.VatTuId,
                    x.Loai
                }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien
                {
                    Loai = item.First().Loai,
                    VatTuId = item.First().VatTuId,
                    VatTu = item.First().VatTu,
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
                    HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                    HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                    NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                    //NhomDieuTriDuPhong = item.FirstOrDefault().NhomDieuTriDuPhong,
                    SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                    TongTonList = item.FirstOrDefault().TongTonList,
                    LoaiVatTu = item.First().LoaiVatTu,
                    TrangThai = item.First().TrangThai,
                    KhoId = item.First().KhoId,
                    KhoaPhongId = item.First().KhoaPhongId,
                    Id = item.First().Id
                });
                var dataOrderBy = query.AsQueryable().OrderBy(x => x.LoaiVatTu == true);
                var queryTask = dataOrderBy.Count();
                return new GridDataSource { TotalRowCount = queryTask };
            }
            //Khoa
            else
            {
                long.TryParse(queryString[3], out long khoaPhongId);
                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuId)
                            .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                HoatChat = p.VatTu.QuyCach,
                                LoaiVatTu = p.LaVatTuBHYT,
                                VatTuId = p.VatTuId,
                                VatTu = p.VatTu.Ten,
                                DVT = p.VatTu.DonViTinh,
                                NhaSX = p.VatTu.NhaSanXuat,
                                NuocSX = p.VatTu.NuocSanXuat,
                                KhoaPhongId = khoaPhongId,
                                KhoId = 0,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                               
                                KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            &&
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,// todo
                                TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                  TenKhoTong = e.First().TenKhoTong,
                                                                                                  TongTon = e.Sum(q => q.TongTon)
                                                                                              }).ToList(),
                                HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                            }).GroupBy(x => new
                            {
                                x.VatTuId,
                                x.Loai
                            }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiVatTu = item.First().LoaiVatTu,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                VatTuId = item.First().VatTuId,
                                VatTu = item.First().VatTu,
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
                                KhoId = item.First().KhoId,
                            });
                var dataOrderBy = query.OrderBy(x => x.LoaiVatTu == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = dataOrderBy.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.Id == duTruMuaVatTuId).SelectMany(s => s.DuTruMuaVatTus).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == o.VatTuId
                                                                                               && x.NhapKhoVatTu.KhoId == lst.KhoId
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                                 && x.LaVatTuBHYT == o.LoaiVatTu
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruVatTuChiTietId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            var khoId = Convert.ToInt32(queryString[2]);
            var khoaPhong = Convert.ToInt32(queryString[3]);
            if (loaiDP == true)  // khoid
            {
                if (khoId != 0)
                {
                    var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == loaiDP)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                 });
                    var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
                }
                if (khoaPhong != 0)
                {
                    var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == loaiDP)
                                                               .SelectMany(s => s.DuTruMuaVatTuChiTiets)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
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
                    var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == loaiDP)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                 });
                    var dataOrderBy = query.AsQueryable();
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
                }
                if (khoaPhong != 0)
                {
                    var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == loaiDP)
                                                               .SelectMany(s => s.DuTruMuaVatTuChiTiets)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                 });
                    var dataOrderBy = query.AsQueryable();
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
                }
            }
            return null;
        }
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long VatTuId);
            var loaiDP = Convert.ToBoolean(queryString[1]);

            int khoaPhongId = Convert.ToInt32(queryString[2]);
            int trangThai = Convert.ToInt32(queryString[3]);
            if (trangThai == 1)
            {
                if (VatTuId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.Kho.KhoaPhongId == khoaPhongId
                                                                          && x.VatTuId == VatTuId
                                                                          && x.LaVatTuBHYT == loaiDP)
                            .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                            {
                               
                                Kho = cc.DuTruMuaVatTu.Kho.Ten,
                                KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                                SLDuTru = cc.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                            });
                    var dataOrderBy = query.AsQueryable();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
            }
            if (trangThai == 2)
            {
                int dutruMuaVatTuTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var queryDuTruMuaVatTuKhoaId = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == dutruMuaVatTuTheoKhoaChiTietId).FirstOrDefault().DuTruMuaVatTuTheoKhoaId;
                if (queryDuTruMuaVatTuKhoaId != null && VatTuId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.DuTruMuaVatTuTheoKhoaId == queryDuTruMuaVatTuKhoaId
                                                                          && x.VatTuId == VatTuId
                                                                          && x.LaVatTuBHYT == loaiDP)
                            .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                            {
                               
                                Kho = cc.DuTruMuaVatTu.Kho.Ten,
                                KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
        public async Task<GridDataSource> GetDataChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long vatTuDuTruId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            int vatTuId = Convert.ToInt32(queryString[2]);

            if (vatTuDuTruId != null && vatTuId != null && loaiDP != null)
            {
                var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == vatTuDuTruId
                                                                      && x.VatTuId == vatTuId
                                                                      && x.LaVatTuBHYT == loaiDP)
                        .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                        {
                           
                            Kho = cc.DuTruMuaVatTu.Kho.Ten,
                            KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
        public async Task<GridDataSource> GetDataChildKhoaForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long vatTuDuTruId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            int vatTuId = Convert.ToInt32(queryString[2]);

            if (vatTuDuTruId != null && vatTuId != null && loaiDP != null)
            {
                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == vatTuDuTruId
                                                                      //&& x.VatTuId == vatTuId
                                                                      && x.LaVatTuBHYT == loaiDP).SelectMany(x => x.DuTruMuaVatTuChiTiets)
                        .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                        {
                           
                            Kho = cc.DuTruMuaVatTu.Kho.Ten,
                            KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
        public List<DuTruMuaVatTuChiTietGridVo> GetDuTruMuaVatTuChiTiet(long idDuTruMua)
        {
            var duTruMuaVatTuChiTiets = _duTruMuaVatTuChiTietRepository.Table.Where(cc => cc.DuTruMuaVatTuId == idDuTruMua)
                .Select(x => new DuTruMuaVatTuChiTietGridVo()
                {
                    Id = x.Id,
                    VatTuId = x.VatTuId,
                    DuTruMuaVatTuId = x.DuTruMuaVatTuId,
                    DuTruMuaVatTuKhoDuocChiTietId = x.DuTruMuaVatTuKhoDuocChiTietId,
                    DuTruMuaVatTuTheoKhoaChiTietId = x.DuTruMuaVatTuTheoKhoaChiTietId,
                    LaVatTuBHYT = x.LaVatTuBHYT,
                    SoLuongDuKienSuDung = x.SoLuongDuKienSuDung,
                    SoLuongDuTru = x.SoLuongDuTru,
                    SoLuongDuTruTruongKhoaDuyet = x.SoLuongDuTruTruongKhoaDuyet,
                });
            return duTruMuaVatTuChiTiets.ToList();
        }
        public List<DuTruMuaVatTuKhoaChiTietGridVo> GetDuTruMuaVatTuKhoaChiTiet(long idDuTruMuaKhoa)
        {
            var duTruMuaVatTuChiTiets = _duTruMuaVatTuTheoKhoaChiTietRepository.Table.Where(cc => cc.DuTruMuaVatTuTheoKhoaId == idDuTruMuaKhoa)
                .Select(x => new DuTruMuaVatTuKhoaChiTietGridVo()
                {
                    Id = x.Id,
                    VatTuId = x.VatTuId,
                    DuTruMuaVatTuTheoKhoaId = x.DuTruMuaVatTuTheoKhoaId,
                    DuTruMuaVatTuKhoDuocChiTietId = x.DuTruMuaVatTuKhoDuocChiTietId,
                    LaVatTuBHYT = x.LaVatTuBHYT,
                    SoLuongDuKienSuDung = x.SoLuongDuKienSuDung,
                    SoLuongDuTru = x.SoLuongDuTru,
                    SoLuongDuTruTruongKhoaDuyet = x.SoLuongDuTruTruongKhoaDuyet,
                    SoLuongDuTruKhoDuocDuyet = x.SoLuongDuTruKhoDuocDuyet
                });
            return duTruMuaVatTuChiTiets.ToList();
        }
        public async Task<DuTruMuaVatTuTheoKhoa> GetDuTruVatTuTheoKhoaByIdAsync(long duTruMuaVatTuKhoaId)
        {
            var duTruVatTuTheoKhoa = await _duTruMuaVatTuTheoKhoaRepository.Table.Where(x => x.Id == duTruMuaVatTuKhoaId)
                .Include(x => x.DuTruMuaVatTuTheoKhoaChiTiets)
                .FirstAsync();
            return duTruVatTuTheoKhoa;
        }
        #endregion
        //#region get data duyet
        //#endregion
        #region update duyet  
        public bool? GetTrangThaiDuyet(long Id, long? khoaId, long? khoId)
        {
            if (khoaId != 0 && khoaId != null)
            {
                var duocDuyet = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
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
        public DuTruMuaVatTuChiTietViewGridVo GetDataUpdate(long Id, bool typeLoaiKho)
        {


            //Lấy thông tin chi tiết của Khoa

            if (typeLoaiKho == true)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == nguoiDangLogin).Select(s => s.User.HoTen).FirstOrDefault();
                var queryChiTiet = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Include(o => o.DuTruMuaVatTuKhoDuoc)
                                       .Where(s => s.Id == Id)
                                       .Select(item => new DuTruMuaVatTuChiTietViewGridVo()
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
                    queryChiTiet.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                                                                                                              .Where(item => item.DuTruMuaVatTuTheoKhoaId == queryChiTiet.Id)
                                                                                                              .Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocList()
                                                                                                              {

                                                                                                                  Id = itemc.Id,
                                                                                                                  Loai = itemc.LaVatTuBHYT,
                                                                                                                  VatTuId = itemc.VatTuId,
                                                                                                                  TenVatTu = itemc.VatTu.Ten,
                                                                                                                  DVT = itemc.VatTu.DonViTinh,
                                                                                                                  NhaSX = itemc.VatTu.NhaSanXuat,
                                                                                                                  NuocSX = itemc.VatTu.NuocSanXuat,
                                                                                                                  SLDuTru = itemc.SoLuongDuTru,
                                                                                                                  SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                                                                                                                  KhoaId = queryChiTiet.KhoaId,
                                                                                                                  SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet,
                                                                                                                  SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruKhoDuocDuyet != null ? itemc.SoLuongDuTruKhoDuocDuyet : itemc.SoLuongDuTru,
                                                                                                                  VatTuDuTruTheoKhoaId = itemc.DuTruMuaVatTuTheoKhoaId,
                                                                                                                  DuTruMuaVatTuTheoKhoaId = itemc.DuTruMuaVatTuTheoKhoaId,
                                                                                                                
                                                                                                                  KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                                                            .Where(x => x.VatTuBenhVienId == itemc.VatTuId
                                                                                                                                        &&
                                                                                                                                        x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                                                                        && x.LaVatTuBHYT == itemc.LaVatTuBHYT
                                                                                                                                        && x.NhapKhoVatTu.DaHet != true
                                                                                                                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                                                                                                  TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                                                            .Where(x => x.VatTuBenhVienId == itemc.VatTuId
                                                                                                                                        && 
                                                                                                                                        x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                                                                        && x.LaVatTuBHYT == itemc.LaVatTuBHYT
                                                                                                                                        && x.NhapKhoVatTu.DaHet != true
                                                                                                                                        && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                                                                        {
                                                                                                                                            TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                                                            TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                                                                        }).GroupBy(q => q.TenKhoTong)
                                                                                                                                          .Select(e => new KhoTongTon
                                                                                                                                          {
                                                                                                                                              TenKhoTong = e.First().TenKhoTong,
                                                                                                                                              TongTon = e.Sum(q => q.TongTon)
                                                                                                                                          }).ToList(),
                                                                                                                                         HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == itemc.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                                                                                                                          HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == itemc.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                                                            .Select(z => new HopDongChuaNhap()
                                                                                                                                            {
                                                                                                                                                TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                                                                TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                                                            }).GroupBy(q => q.TenHopDong)
                                                                                                                                               .Select(e => new HopDongChuaNhap
                                                                                                                                               {
                                                                                                                                                   TenHopDong = e.First().TenHopDong,
                                                                                                                                                   TongTon = e.Sum(q => q.TongTon)
                                                                                                                                               }).ToList(),
                                                                                                              }).GroupBy(x => new
                                                                                                              {
                                                                                                                  x.Loai,
                                                                                                                  x.VatTuId
                                                                                                              }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocList()
                                                                                                              {
                                                                                                                  Id = itemcc.First().Id,
                                                                                                                  Loai = itemcc.First().Loai,
                                                                                                                  VatTuId = itemcc.First().VatTuId,
                                                                                                                  TenVatTu = itemcc.First().TenVatTu,
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
                                                                                                                  SLDuTruTKhoDuyet = itemcc.Sum(s => s.SLDuTruTKhoDuyet),
                                                                                                                  SLDuTruKhoDuocDuyet = itemcc.Sum(s => s.SLDuTruKhoDuocDuyet),
                                                                                                                  VatTuDuTruTheoKhoaId = itemcc.First().VatTuDuTruTheoKhoaId,
                                                                                                                  DuTruMuaVatTuTheoKhoaId = itemcc.First().DuTruMuaVatTuTheoKhoaId,
                                                                                                                  VatTuDuTruId = itemcc.First().VatTuDuTruId,
                                                                                                                  HDChuaNhap = itemcc.Sum(x => x.HDChuaNhap),
                                                                                                                  KhoTongTon = itemcc.Sum(x => x.KhoTongTon),
                                                                                                                  TongTonList = itemcc.First().TongTonList,
                                                                                                                  HopDongChuahapList = itemcc.First().HopDongChuahapList

                                                                                                              }).ToList();
                    var dataOrderBy = queryChiTiet.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList.OrderBy(x => x.Loai == true).ToList();
                    var resdataOrderBy = dataOrderBy.Select(o =>
                    {
                        var listKhoTheoKhoaPhong = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.Id == Id).SelectMany(s => s.DuTruMuaVatTus).ToList();
                        var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                        foreach (var lst in groupKho)
                        {
                            o.KhoDuTruTon += _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                       .Where(x => x.VatTuBenhVienId == o.VatTuId
                                                                                                   && x.NhapKhoVatTu.KhoId == lst.KhoId
                                                                                                   && x.NhapKhoVatTu.DaHet != true
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
                var queryChiTiet = BaseRepository.TableNoTracking.Include(o => o.DuTruMuaVatTuKhoDuoc)
                                          .Where(s => s.Id == Id)
                                          .Select(item => new DuTruMuaVatTuChiTietViewGridVo()
                                          {
                                              Id = item.Id,
                                              SoPhieu = item.SoPhieu,
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
                    queryChiTiet.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList = _duTruMuaVatTuChiTietRepository.TableNoTracking
                                                                                                              .Where(item => item.DuTruMuaVatTuId == queryChiTiet.Id)
                                                                                                              .Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocList()
                                                                                                              {
                                                                                                                  Id = itemc.Id,
                                                                                                                  Loai = itemc.LaVatTuBHYT,
                                                                                                                  VatTuId = itemc.VatTuId,
                                                                                                                  TenVatTu = itemc.VatTu.Ten,
                                                                                                                  DVT = itemc.VatTu.DonViTinh,
                                                                                                                  NhaSX = itemc.VatTu.NhaSanXuat,
                                                                                                                  NuocSX = itemc.VatTu.NuocSanXuat,
                                                                                                                  SLDuTru = itemc.SoLuongDuTru,
                                                                                                                  SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                                                                                                                  KhoaId = queryChiTiet.KhoaId,
                                                                                                                  SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet != null ? itemc.SoLuongDuTruTruongKhoaDuyet : itemc.SoLuongDuTru,
                                                                                                                  SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruTruongKhoaDuyet != null ? itemc.SoLuongDuTruTruongKhoaDuyet : itemc.SoLuongDuTru,
                                                                                                                  VatTuDuTruId = itemc.DuTruMuaVatTuId,
                                                                                                                  KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                                                                .Where(x => x.VatTuBenhVienId == itemc.VatTuId
                                                                                                                                           && x.NhapKhoVatTu.KhoId == queryChiTiet.KhoId
                                                                                                                                            && x.LaVatTuBHYT == itemc.LaVatTuBHYT
                                                                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                                                                                                  KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                                                                .Where(x => x.VatTuBenhVienId == itemc.VatTuId
                                                                                                                                            && 
                                                                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                                                                            && x.LaVatTuBHYT == itemc.LaVatTuBHYT
                                                                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                                                                                                  TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                                                                .Where(x => x.VatTuBenhVienId == itemc.VatTuId
                                                                                                                                            && 
                                                                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                                                                            && x.LaVatTuBHYT == itemc.LaVatTuBHYT
                                                                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                                                                            {
                                                                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                                                                            }).GroupBy(q => q.TenKhoTong)
                                                                                                                                          .Select(e => new KhoTongTon
                                                                                                                                          {
                                                                                                                                              TenKhoTong = e.First().TenKhoTong,
                                                                                                                                              TongTon = e.Sum(q => q.TongTon)
                                                                                                                                          }).ToList(),
                                                                                                                  HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == itemc.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                                                                                                  HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == itemc.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                                                            .Select(z => new HopDongChuaNhap()
                                                                                                                                            {
                                                                                                                                                TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                                                                TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                                                            }).GroupBy(q => q.TenHopDong)
                                                                                                                                               .Select(e => new HopDongChuaNhap
                                                                                                                                               {
                                                                                                                                                   TenHopDong = e.First().TenHopDong,
                                                                                                                                                   TongTon = e.Sum(q => q.TongTon)
                                                                                                                                               }).ToList(),
                                                                                                              }).GroupBy(x => new
                                                                                                              {
                                                                                                                  x.Loai,
                                                                                                                  x.VatTuId
                                                                                                              }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocList()
                                                                                                              {
                                                                                                                  Id = itemcc.First().Id,
                                                                                                                  Loai = itemcc.First().Loai,
                                                                                                                  VatTuId = itemcc.First().VatTuId,
                                                                                                                  TenVatTu = itemcc.First().TenVatTu,
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
                                                                                                                  VatTuDuTruId = itemcc.First().VatTuDuTruId,
                                                                                                                  HDChuaNhap = itemcc.Sum(x => x.HDChuaNhap),
                                                                                                                  KhoTongTon = itemcc.Sum(x => x.KhoTongTon),
                                                                                                                  TongTonList = itemcc.First().TongTonList,
                                                                                                                  HopDongChuahapList = itemcc.First().HopDongChuahapList,
                                                                                                                  KhoDuTruTon = itemcc.FirstOrDefault().KhoDuTruTon
                                                                                                              }).ToList();
                }
                return queryChiTiet;
            }
            return null;
        }

        private EnumTrangThaiDuTruKhoaDuoc GetTrangThaiDuTruKhoTaiKhoaDuoc(DuTruMuaVatTu item)
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
                    if (item.DuTruMuaVatTuKhoDuoc != null)
                    {
                        if (item.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == null)
                        {
                            return EnumTrangThaiDuTruKhoaDuoc.DaGoiVaChoDuyet;
                        }
                        else
                        {
                            if (item.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false)
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
        private EnumTrangThaiDuTruKhoaDuoc GetTrangThaiDuTruKhoaTaiKhoaDuoc(DuTruMuaVatTuTheoKhoa item)
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
                    if (item.DuTruMuaVatTuKhoDuoc != null)
                    {
                        if (item.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == null)
                        {
                            return EnumTrangThaiDuTruKhoaDuoc.DaGoiVaChoDuyet;
                        }
                        else
                        {
                            if (item.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false)
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
        private EnumTrangThaiDuTruKhoaDuoc GetTrangThaiDuTruMuaVatTuKhoDuoc(DuTruMuaVatTuKhoDuoc item)
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new DuTruMuaVatTuTaiKhoaDuocSearchDaXuLy();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaVatTuTaiKhoaDuocSearchDaXuLy>(queryInfo.AdditionalSearchString);
            }

            var queryDaGoiVaChoDuyet = GetDataYeuCauMuaVatTuDaGoiVaChoDuyet(null, queryInfo, queryObject);
            var queryDaDuyet = GetDataYeuCauMuaVatTuDaDuyet(true, queryInfo, queryObject);
            var queryTuChoi = GetDataYeuCauMuaVatTuTuChoi(false, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaVatTuTaiKhoaDuocDaXuLy())
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocDaXuLyToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            var queryObject = new DuTruMuaVatTuTaiKhoaDuocSearchDaXuLy();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaVatTuTaiKhoaDuocSearchDaXuLy>(queryInfo.AdditionalSearchString);
            }

            var queryDaGoiVaChoDuyet = GetDataYeuCauMuaVatTuDaGoiVaChoDuyet(null, queryInfo, queryObject);
            var queryDaDuyet = GetDataYeuCauMuaVatTuDaDuyet(true, queryInfo, queryObject);
            var queryTuChoi = GetDataYeuCauMuaVatTuTuChoi(false, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaVatTuTaiKhoaDuocDaXuLy())
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
        private IQueryable<DuTruMuaVatTuTaiKhoaDuocDaXuLy> GetDataYeuCauMuaVatTuDaGoiVaChoDuyet(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaVatTuTaiKhoaDuocSearchDaXuLy queryObject)
        {
            var result = _duTruMuaVatTuKhoDuocRepository.TableNoTracking
                .Where(p => p.GiamDocDuyet == duocDuyet)
            .Select(o => new DuTruMuaVatTuTaiKhoaDuocDaXuLy()
            {
                Id = o.Id,
                SoPhieu = o.SoPhieu,
                DuTruTheo = o.TuNgay.ApplyFormatDate() + '-' + o.DenNgay.ApplyFormatDate(),
                NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                NgayYeuCau = o.NgayYeuCau,
                TinhTrang = EnumTrangThaiDuTruKhoaDuoc.DaGoiVaChoDuyet,
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
        private IQueryable<DuTruMuaVatTuTaiKhoaDuocDaXuLy> GetDataYeuCauMuaVatTuDaDuyet(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaVatTuTaiKhoaDuocSearchDaXuLy queryObject)
        {
            var result = _duTruMuaVatTuKhoDuocRepository.TableNoTracking
                .Where(p => p.GiamDocDuyet == true)
            .Select(o => new DuTruMuaVatTuTaiKhoaDuocDaXuLy()
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
        private IQueryable<DuTruMuaVatTuTaiKhoaDuocDaXuLy> GetDataYeuCauMuaVatTuTuChoi(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaVatTuTaiKhoaDuocSearchDaXuLy queryObject)
        {
            var result = _duTruMuaVatTuKhoDuocRepository.TableNoTracking
                .Where(p => p.GiamDocDuyet == false)
            .Select(o => new DuTruMuaVatTuTaiKhoaDuocDaXuLy()
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long IdDuTruMuaVatTuKhoaDuoc);

            long.TryParse(queryString[1], out long TinhTrang);

            // nha thuoc
            var queryKho = BaseRepository.TableNoTracking
                .Where(x =>
                     x.DuTruMuaVatTuKhoDuocId == IdDuTruMuaVatTuKhoaDuoc
                    )
                .Select(s => new DuTruMuaVatTuKhoaDuocChild()
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
                    DuTruMuaVatTuKhoDuocId = IdDuTruMuaVatTuKhoaDuoc,
                    KhoaKhoaString = s.Kho.Ten,
                    LoaiKho = s.Kho.LoaiKho
                });
            // khoa le kho
            var querykhoa = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                .Where(x =>
                 x.DuTruMuaVatTuKhoDuocId == IdDuTruMuaVatTuKhoaDuoc
                )
                .Select(s => new DuTruMuaVatTuKhoaDuocChild()
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
                    DuTruMuaVatTuKhoDuocId = IdDuTruMuaVatTuKhoaDuoc,
                    KhoaKhoaString = s.KhoaPhong.Ten,
                    LoaiKho = EnumLoaiKhoDuocPham.KhoLe
                });
            List<DuTruMuaVatTuKhoaDuocChild> listQuery = new List<DuTruMuaVatTuKhoaDuocChild>();
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long IdDuTruMuaVatTuKhoaDuoc);

            long.TryParse(queryString[1], out long TinhTrang);

            // nha thuoc
            var queryKho = BaseRepository.TableNoTracking
                .Where(x =>
                     x.DuTruMuaVatTuKhoDuocId == IdDuTruMuaVatTuKhoaDuoc
                    )
                .Select(s => new DuTruMuaVatTuKhoaDuocChild()
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
                    DuTruMuaVatTuKhoDuocId = IdDuTruMuaVatTuKhoaDuoc,
                    KhoaKhoaString = s.Kho.Ten,
                    LoaiKho = s.Kho.LoaiKho
                });
            // khoa le kho
            var querykhoa = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                .Where(x =>
                 x.DuTruMuaVatTuKhoDuocId == IdDuTruMuaVatTuKhoaDuoc
                )
                .Select(s => new DuTruMuaVatTuKhoaDuocChild()
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
                    DuTruMuaVatTuKhoDuocId = IdDuTruMuaVatTuKhoaDuoc,
                    KhoaKhoaString = s.KhoaPhong.Ten,
                    LoaiKho = EnumLoaiKhoDuocPham.KhoLe
                });
            List<DuTruMuaVatTuKhoaDuocChild> listQuery = new List<DuTruMuaVatTuKhoaDuocChild>();
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildForGridAsync(QueryInfo queryInfo)
        {
            var querystring = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(querystring[0], out long idDuTruMua);
            long.TryParse(querystring[1], out long khoId);
            long.TryParse(querystring[2], out long khoaId);
            long.TryParse(querystring[3], out long Id);
            long.TryParse(querystring[4], out long TinhTrang);
            long.TryParse(querystring[5], out long DuTruMuaVatTuKhoDuocId);
            long IdDuTruKhoaVatTu = 0;
            long IdDuTruVatTu = 0;

            if (khoId != 0)
            {
                bool kiemTraLoaiKho = false;
                
                    var dataList = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuId == Id
                                                                                            )
                                .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                                {
                                    Id = p.Id,
                                    Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                    VatTuId = p.VatTuId,
                                    HoatChat = p.VatTu.QuyCach,
                                    VatTu = p.VatTu.Ten,
                                    DVT = p.VatTu.DonViTinh,
                                    NhaSX = p.VatTu.NhaSanXuat,
                                    NuocSX = p.VatTu.NuocSanXuat,
                                    SLDuTru = p.SoLuongDuTru,
                                    SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                    KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                    .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                                && x.NhapKhoVatTu.KhoId == khoId
                                                                                                && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                                && x.NhapKhoVatTu.DaHet != true
                                                                                                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                    KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                    .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                                &&
                                                                                                x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                                && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                                && x.NhapKhoVatTu.DaHet != true
                                                                                                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                    SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                    TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                    .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                                && 
                                                                                                x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                                && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                                && x.NhapKhoVatTu.DaHet != true
                                                                                                && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                                {
                                                                                                    TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                    TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                                }).ToList(),
                                    HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                    HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                                }).ToList();
                    var query = dataList.GroupBy(x => new
                    {
                        x.VatTuId,
                        x.Loai
                    }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien
                    {
                        Loai = item.First().Loai,
                        VatTuId = item.First().VatTuId,
                        VatTu = item.First().VatTu,
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
                        HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                        HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                        NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                        SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                        TongTonList = item.FirstOrDefault().TongTonList
                    });
                    var dataOrderBy = query.AsQueryable();
                    var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            if (khoaId != 0)
            {
                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                    .Where(x => x.DuTruMuaVatTuTheoKhoaId == Id
                                                                                       )
                            .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                LoaiVatTu = p.LaVatTuBHYT,
                                VatTuId = p.VatTuId,
                                HoatChat = p.VatTu.QuyCach,
                                VatTu = p.VatTu.Ten,
                                DVT = p.VatTu.DonViTinh,
                                NhaSX = p.VatTu.NhaSanXuat,
                                NuocSX = p.VatTu.NuocSanXuat,
                                KhoaPhongId = khoaId,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,

                                KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,
                                TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).ToList(),
                                HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                            }).GroupBy(x => new
                            {
                                x.VatTuId,
                                x.Loai
                            }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiVatTu = item.First().LoaiVatTu,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                VatTuId = item.First().VatTuId,
                                VatTu = item.First().VatTu,
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
                var dataOrderBy = query.OrderBy(x => x.LoaiVatTu == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = data.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.Id == Id).SelectMany(s => s.DuTruMuaVatTus).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == o.VatTuId
                                                                                               && x.NhapKhoVatTu.KhoId == lst.KhoId
                                                                                               && x.NhapKhoVatTu.DaHet != true
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var querystring = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(querystring[0], out long idDuTruMua);
            long.TryParse(querystring[1], out long khoId);
            long.TryParse(querystring[2], out long khoaId);
            long.TryParse(querystring[3], out long Id);
            long.TryParse(querystring[4], out long TinhTrang);
            long.TryParse(querystring[5], out long DuTruMuaVatTuKhoDuocId);
            long IdDuTruKhoaVatTu = 0;
            long IdDuTruVatTu = 0;

            if (khoId != 0)
            {
                bool kiemTraLoaiKho = false;

                var dataList = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuId == Id
                                                                                        )
                            .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                            {
                                Id = p.Id,
                                Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                VatTuId = p.VatTuId,
                                HoatChat = p.VatTu.QuyCach,
                                VatTu = p.VatTu.Ten,
                                DVT = p.VatTu.DonViTinh,
                                NhaSX = p.VatTu.NhaSanXuat,
                                NuocSX = p.VatTu.NuocSanXuat,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && x.NhapKhoVatTu.KhoId == khoId
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTru,
                                TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).ToList(),
                                HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                            }).ToList();
                var query = dataList.GroupBy(x => new
                {
                    x.VatTuId,
                    x.Loai
                }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien
                {
                    Loai = item.First().Loai,
                    VatTuId = item.First().VatTuId,
                    VatTu = item.First().VatTu,
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
                    HDChuaNhap = item.Sum(x => x.HDChuaNhap),
                    HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                    NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                    SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                    TongTonList = item.FirstOrDefault().TongTonList
                });
                var queryTask = query.Count();
                return new GridDataSource { TotalRowCount = queryTask };
            }
            if (khoaId != 0)
            {
                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuTheoKhoaId == Id
                                                                                       )
                            .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildGridVo()
                            {
                                Id = p.Id,
                                Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                LoaiVatTu = p.LaVatTuBHYT,
                                VatTuId = p.VatTuId,
                                HoatChat = p.VatTu.QuyCach,
                                VatTu = p.VatTu.Ten,
                                DVT = p.VatTu.DonViTinh,
                                NhaSX = p.VatTu.NhaSanXuat,
                                NuocSX = p.VatTu.NuocSanXuat,
                                KhoaPhongId = khoaId,
                                SLDuTru = p.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                SLDuTruTKhoaDuyet = p.SoLuongDuTru,
                                SLDuTruKDuocDuyet = p.SoLuongDuTru,// todo
                                TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                            && 
                                                                                            x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                            && x.NhapKhoVatTu.DaHet != true
                                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                            {
                                                                                                TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                            }).ToList(),
                                HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                            }).GroupBy(x => new
                            {
                                x.VatTuId,
                                x.Loai
                            }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildGridVo
                            {
                                Id = item.First().Id,
                                LoaiVatTu = item.First().LoaiVatTu,
                                TrangThai = item.First().TrangThai,
                                Loai = item.First().Loai,
                                VatTuId = item.First().VatTuId,
                                VatTu = item.First().VatTu,
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
                var dataOrderBy = query.OrderBy(x => x.LoaiVatTu == true).ToList();
                var data = dataOrderBy.ToArray();
                var resdataOrderBy = dataOrderBy.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.Id == Id).SelectMany(s => s.DuTruMuaVatTus).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == o.VatTuId
                                                                                               && x.NhapKhoVatTu.KhoId == lst.KhoId
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                           && x.LaVatTuBHYT == o.LoaiVatTu
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long VatTuId);
            var loaiDP = Convert.ToBoolean(queryString[1]);

            int khoaPhongId = Convert.ToInt32(queryString[2]);
            int trangThai = Convert.ToInt32(queryString[3]);
            if (trangThai == 1)
            {
                int dutruMuaVatTuTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var duTruMuaVatTuTheoKhoaId = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == dutruMuaVatTuTheoKhoaChiTietId).FirstOrDefault().DuTruMuaVatTuTheoKhoaId;
                if (dutruMuaVatTuTheoKhoaChiTietId != 0)
                {
                    if (VatTuId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId
                                                                       && x.VatTuId == VatTuId
                                                                       && x.LaVatTuBHYT == loaiDP)
                         .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                         {
                            
                             Kho = cc.DuTruMuaVatTu.Kho.Ten,
                             KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                             SLDuTru = cc.SoLuongDuTru,
                             SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                         });
                        var dataOrderBy = query.AsQueryable();
                        var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                        var countTask = dataOrderBy.Count();

                        return new GridDataSource { Data = data, TotalRowCount = countTask };
                    }
                }
                else
                {
                    if (VatTuId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.Kho.KhoaPhongId == khoaPhongId
                                                                              && x.VatTuId == VatTuId
                                                                              && x.LaVatTuBHYT == loaiDP)
                                .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                                {
                                   
                                    Kho = cc.DuTruMuaVatTu.Kho.Ten,
                                    KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                                    SLDuTru = cc.SoLuongDuTru,
                                    SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
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
                int dutruMuaVatTuTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var queryDuTruMuaVatTuKhoaId = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == dutruMuaVatTuTheoKhoaChiTietId).FirstOrDefault().DuTruMuaVatTuTheoKhoaId;
                if (queryDuTruMuaVatTuKhoaId != null && VatTuId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.DuTruMuaVatTuTheoKhoaId == queryDuTruMuaVatTuKhoaId
                                                                          && x.VatTuId == VatTuId
                                                                          && x.LaVatTuBHYT == loaiDP)
                            .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                            {
                               
                                Kho = cc.DuTruMuaVatTu.Kho.Ten,
                                KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                                SLDuTru = cc.SoLuongDuTru,
                                SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long VatTuId);
            var loaiDP = Convert.ToBoolean(queryString[1]);

            int khoaPhongId = Convert.ToInt32(queryString[2]);
            int trangThai = Convert.ToInt32(queryString[3]);
            if (trangThai == 1)
            {
                int dutruMuaVatTuTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var duTruMuaVatTuTheoKhoaId = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == dutruMuaVatTuTheoKhoaChiTietId).FirstOrDefault().DuTruMuaVatTuTheoKhoaId;
                if (dutruMuaVatTuTheoKhoaChiTietId != 0)
                {
                    if (VatTuId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId
                                                                       && x.VatTuId == VatTuId
                                                                       && x.LaVatTuBHYT == loaiDP)
                         .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                         {
                            
                             Kho = cc.DuTruMuaVatTu.Kho.Ten,
                             KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
                    if (VatTuId != null && loaiDP != null && khoaPhongId != null)
                    {
                        var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.Kho.KhoaPhongId == khoaPhongId
                                                                              && x.VatTuId == VatTuId
                                                                              && x.LaVatTuBHYT == loaiDP)
                                .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                                {
                                   
                                    Kho = cc.DuTruMuaVatTu.Kho.Ten,
                                    KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
                int dutruMuaVatTuTheoKhoaChiTietId = Convert.ToInt32(queryString[4]);
                var queryDuTruMuaVatTuKhoaId = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == dutruMuaVatTuTheoKhoaChiTietId).FirstOrDefault().DuTruMuaVatTuTheoKhoaId;
                if (queryDuTruMuaVatTuKhoaId != null && VatTuId != null && loaiDP != null && khoaPhongId != null)
                {
                    var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTu.DuTruMuaVatTuTheoKhoaId == queryDuTruMuaVatTuKhoaId
                                                                          && x.VatTuId == VatTuId
                                                                          && x.LaVatTuBHYT == loaiDP)
                            .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                            {
                               
                                Kho = cc.DuTruMuaVatTu.Kho.Ten,
                                KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
        public DuTruMuaVatTuChiTietGoiViewGridVo GetDuTruMuaVatTuChiTietGoiView(long idDuTruMuaKhoaDuoc, long tinhTrang)
        {

            var queryVatTuKhoDuoc = _duTruMuaVatTuKhoDuocRepository.TableNoTracking.Where(x => x.Id == idDuTruMuaKhoaDuoc)
                    .Select(item => new DuTruMuaVatTuChiTietGoiViewGridVo()
                    {
                        Id = item.Id,
                        KyDuTru = item.TuNgay.ApplyFormatDate() + '-' + item.DenNgay.ApplyFormatDate(),
                        KyDuTruId = item.KyDuTruMuaDuocPhamVatTuId,
                        NguoiYeuCauId = item.NhanVienYeuCauId,
                        TenNguoiYeuCau = item.NhanVienYeuCau.User.HoTen,
                        NgayYeuCau = item.NgayYeuCau,
                        GhiChu = item.GhiChu,
                        TrangThai = GetTrangThaiDuTruMuaVatTuKhoDuoc(item),
                        TrangThaiHienThi = GetTrangThaiDuTruMuaVatTuKhoDuoc(item).GetDescription(),
                        TuNgay = item.TuNgay,
                        DenNgay = item.DenNgay,
                        DuTruVatTuTheoKhoaId = item.Id,
                        LyDoTuChoi = item.LyDoGiamDocTuChoi
                    }).FirstOrDefault();
            if (queryVatTuKhoDuoc != null)
            {
                queryVatTuKhoDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = _duTruMuaVatTuKhoDuocChiTietRepository.TableNoTracking
                                                                                           .Where(item => item.DuTruMuaVatTuKhoDuocId == queryVatTuKhoDuoc.Id)
                                                                                            .Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList1()
                                                                                            {
                                                                                                Id = itemc.Id,
                                                                                                DuTruMuaVatTuKhoaDuId = itemc.Id,
                                                                                                Loai = itemc.LaVatTuBHYT,
                                                                                                VatTuId = itemc.VatTuId,
                                                                                                TenVatTu = itemc.VatTu.Ten,
                                                                                               
                                                                                              
                                                                                               
                                                                                               
                                                                                                DVT = itemc.VatTu.DonViTinh,
                                                                                                NhaSX = itemc.VatTu.NhaSanXuat,
                                                                                                NuocSX = itemc.VatTu.NuocSanXuat,
                                                                                                SLDuTru = itemc.SoLuongDuTru,
                                                                                                SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                                                                                                SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ? itemc.SoLuongDuTru : itemc.SoLuongDuTruTruongKhoaDuyet,
                                                                                                SLDuTruKhoDuocDuyet =  itemc.SoLuongDuTruKhoDuocDuyet == null ?itemc.SoLuongDuTru : itemc.SoLuongDuTruKhoDuocDuyet,
                                                                                                DuTruMuaVatTuKhoDuocId = idDuTruMuaKhoaDuoc
                                                                                            }).GroupBy(x => new
                                                                                            {
                                                                                                x.Loai,
                                                                                                x.VatTuId
                                                                                            }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList1()
                                                                                            {
                                                                                                Id = itemcc.First().Id,
                                                                                                DuTruMuaVatTuKhoaDuId = itemcc.First().DuTruMuaVatTuKhoaDuId,
                                                                                                DuTruMuaVatTuKhoId = itemcc.First().DuTruMuaVatTuKhoId,
                                                                                                Loai = itemcc.First().Loai,
                                                                                                VatTuId = itemcc.First().VatTuId,
                                                                                                TenVatTu = itemcc.First().TenVatTu,
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
                                                                                                VatTuDuTruTheoKhoaId = itemcc.First().VatTuDuTruTheoKhoaId,
                                                                                                DuTruMuaVatTuTheoKhoaId = itemcc.First().DuTruMuaVatTuTheoKhoaId,
                                                                                                VatTuDuTruId = itemcc.First().VatTuDuTruId,
                                                                                                KyDuTruMuaDuocPhamVatTuId = itemcc.First().KyDuTruMuaDuocPhamVatTuId,
                                                                                                DuTruMuaVatTuKhoDuocId = itemcc.First().DuTruMuaVatTuKhoDuocId

                                                                                            }).ToList();
            }

            return queryVatTuKhoDuoc;
        }
        //view child
        public async Task<GridDataSource> GetDuTruMuaVatTuChiTietGoiViewChild(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long idDuTruMuaVatTuKhoDuocChiTiet);
            //long.TryParse(queryString[1], out long vatTuId);
            //long.TryParse(queryString[2], out long IdKyDuTru);
            //bool.TryParse(queryString[3], out bool loai);

            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuTruMuaVatTuKhoaDuocChildChild()).AsQueryable();
            var duTruMuaDpTheoKhoaDetailedIds = await _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocChiTietId == idDuTruMuaVatTuKhoDuocChiTiet)
                .Select(e => e.Id).ToListAsync();
            foreach (var duTruMuaDpTheoKhoaDetailedIdItem in duTruMuaDpTheoKhoaDetailedIds)
            {
                var duTruMuaDpChiTiets = _duTruMuaVatTuChiTietRepository.TableNoTracking
                    .Where(e => e.DuTruMuaVatTuTheoKhoaChiTietId == duTruMuaDpTheoKhoaDetailedIdItem
                                )
                    .Select(e => new DuTruMuaVatTuKhoaDuocChildChild
                    {
                        Id = e.Id,
                       
                        Kho = e.DuTruMuaVatTu.Kho.Ten,
                        Khoa = e.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoa.KhoaPhong.Ten,
                        KyDuTru = e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                         " - " +
                                         e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                        SLDuTru = e.SoLuongDuTru,
                        SLDuKienTrongKy = e.SoLuongDuKienSuDung,
                    });
                query = query.Concat(duTruMuaDpChiTiets);
            }

            var duTruMuaDpChiTietTuKhoTongs = _duTruMuaVatTuChiTietRepository.TableNoTracking
                .Where(e => e.DuTruMuaVatTuKhoDuocChiTietId == idDuTruMuaVatTuKhoDuocChiTiet
                            )
                .Select(e => new DuTruMuaVatTuKhoaDuocChildChild
                {
                    Id = e.Id,
                    Kho = e.DuTruMuaVatTu.Kho.Ten,
                    Khoa = "Khoa Dược",
                    KyDuTru = e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.TuNgay.ApplyFormatDate() +
                                     " - " +
                                     e.DuTruMuaVatTu.KyDuTruMuaDuocPhamVatTu.DenNgay.ApplyFormatDate(),
                    SLDuTru = e.SoLuongDuTru,
                    SLDuKienTrongKy = e.SoLuongDuKienSuDung,
                });
            query = query.Concat(duTruMuaDpChiTietTuKhoTongs);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryKho = BaseRepository.TableNoTracking
                .Where(s => s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                            s.TruongKhoaDuyet == false
                ).Select(s => new DuTruMuaVatTuTuChoiGridVo()
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
                }).ApplyLike(queryInfo.SearchTerms, g => g.SoPhieu, g => g.LyDo, g => g.NguoiYeuCau, g => g.NguoiTuChoi, g => g.KhoaKho);
            var queryKhoa =_duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.KhoDuocDuyet == false)
                                                                       .Select(s => new DuTruMuaVatTuTuChoiGridVo()
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

                var queryObject = JsonConvert.DeserializeObject<DuTruMuaVatTuTaiKhoaDuocTuChoiSearch>(queryInfo.AdditionalSearchString);

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
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            var queryKho = BaseRepository.TableNoTracking
               .Where(s => s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                           s.TruongKhoaDuyet == false
               ).Select(s => new DuTruMuaVatTuTuChoiGridVo()
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
               }).ApplyLike(queryInfo.SearchTerms, g => g.SoPhieu, g => g.LyDo, g => g.NguoiYeuCau, g => g.NguoiTuChoi, g => g.KhoaKho);
            var queryKhoa = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.KhoDuocDuyet == false)
                                                                       .Select(s => new DuTruMuaVatTuTuChoiGridVo()
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

                var queryObject = JsonConvert.DeserializeObject<DuTruMuaVatTuTaiKhoaDuocTuChoiSearch>(queryInfo.AdditionalSearchString);

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
                //if (queryObject.SearchString != null)
                //{
                //    query = result.ApplyLike(queryObject.SearchString.Replace("\t", "").Trim(),
                //    q => q.SoPhieu,
                //    q => q.KhoaKhoa,
                //    q => q.NguoiYeuCau,
                //    q => q.KyDuTru);
                //}
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaVatTuId);
            var khoId = Convert.ToInt32(queryString[1]);
            int khoaId = Convert.ToInt32(queryString[2]);
            if (khoId != 0)
            {
                var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuId == duTruMuaVatTuId)
                           .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               Id = p.Id,
                               Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                               LoaiVatTu = p.LaVatTuBHYT,
                               VatTuId = p.VatTuId,
                               VatTu = p.VatTu.Ten,
                                HoatChat = p.VatTu.QuyCach,
                             
                             
                               DVT = p.VatTu.DonViTinh,
                            
                               NhaSX = p.VatTu.NhaSanXuat,
                               NuocSX = p.VatTu.NuocSanXuat,
                               SLDuTru = p.SoLuongDuTru,
                               TrangThai = EnumTrangThaiLoaiDuTru.ChoGoi,
                               SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                               LoaiKhoHayKhoa = true, // khoid
                               KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                               .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                           && x.NhapKhoVatTu.KhoId == khoId
                                                                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                           && x.NhapKhoVatTu.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                               .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                           && 
                                                                                           x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                           && x.NhapKhoVatTu.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                               TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                               .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                           && 
                                                                                           x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                           && x.NhapKhoVatTu.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                           {
                                                                                               TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                               TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                           }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                  TenKhoTong = e.First().TenKhoTong,
                                                                                                  TongTon = e.Sum(q => q.TongTon)
                                                                                              }).ToList(),
                               HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                               HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                           }).GroupBy(x => new
                           {
                               x.VatTuId,
                               x.Loai
                           }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                               Loai = item.First().Loai,
                               VatTuId = item.First().VatTuId,
                               VatTu = item.First().VatTu,
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
                               // = item.FirstOrDefault().SLDuTruKDuocDuyet,
                               HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                               NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                               SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                               TongTonList = item.FirstOrDefault().TongTonList,
                               LoaiVatTu = item.FirstOrDefault().LoaiVatTu,
                               Id = item.FirstOrDefault().Id
                           });
                var dataOrderBy = query.AsQueryable();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            if (khoaId != 0)
            {
                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuId)
                               .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildGridVo()
                               {
                                   Id = p.Id,
                                   Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                   LoaiVatTu = p.LaVatTuBHYT,
                                   VatTuId = p.VatTuId,
                                   VatTu = p.VatTu.Ten,
                                   HoatChat = p.VatTu.QuyCach,


                                   DVT = p.VatTu.DonViTinh,
                                
                                   NhaSX = p.VatTu.NhaSanXuat,
                                   NuocSX = p.VatTu.NuocSanXuat,
                                   KhoaPhongId = khoaId,
                                   SLDuTru = p.SoLuongDuTru,
                                   SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                   TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                   LoaiKhoHayKhoa = false,// khoaid
                                   //KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                   //                                                .Where(x => x.VatTuBenhVienId == p.VatTuId
                                   //                                                            //&& x.NhapKhoVatTu.kh == khoVatTuChiTietId
                                   //                                                            && x.LaVatTuBHYT == p.LaVatTuBHYT
                                   //                                                            && x.NhapKhoVatTu.DaHet != true
                                   //                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                               && 
                                                                                               x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                               && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruTruongKhoaDuyet,
                                   SLDuTruKDuocDuyet = p.SoLuongDuTruKhoDuocDuyet == null ? p.SoLuongDuTru : p.SoLuongDuTruKhoDuocDuyet,
                                   TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                               && 
                                                                                               x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                               && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                               {
                                                                                                   TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                   TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                               }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                  TenKhoTong = e.First().TenKhoTong,
                                                                                                  TongTon = e.Sum(q => q.TongTon)
                                                                                              }).ToList(),
                                   HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                   HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId && x.SoLuong > x.SoLuongDaCap)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                               }).GroupBy(x => new
                               {
                                   x.VatTuId,
                                   x.Loai
                               }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildGridVo
                               {
                                   LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                                   Id = item.First().Id,
                                   LoaiVatTu = item.First().LoaiVatTu,
                                   TrangThai = item.First().TrangThai,
                                   Loai = item.First().Loai,
                                   VatTuId = item.First().VatTuId,
                                   VatTu = item.First().VatTu,
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

                var dataOrderBy = query.OrderBy(x => x.LoaiVatTu == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = dataOrderBy.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.Id == duTruMuaVatTuId).SelectMany(s => s.DuTruMuaVatTus).ToList();
                    var groupKho = listKhoTheoKhoaPhong.GroupBy(x => x.KhoId).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var lst in groupKho)
                    {
                        o.KhoDuTruTon += _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == o.VatTuId
                                                                                               && x.NhapKhoVatTu.KhoId == lst.KhoId
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                                && x.LaVatTuBHYT == o.LoaiVatTu
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildForGridAsync(QueryInfo queryInfo)
        {
            // Id DuTruMuaDuocPHam , LoaiKho, TrangThai
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruMuaVatTuId);
            var khoId = Convert.ToInt32(queryString[1]);
            int khoaId = Convert.ToInt32(queryString[2]);
            if (khoId != 0)
            {
                var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuId == duTruMuaVatTuId)
                           .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               Id = p.Id,
                               Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                               LoaiVatTu = p.LaVatTuBHYT,
                               VatTuId = p.VatTuId,
                               VatTu = p.VatTu.Ten,
                               HoatChat = p.VatTu.QuyCach,


                               DVT = p.VatTu.DonViTinh,

                               NhaSX = p.VatTu.NhaSanXuat,
                               NuocSX = p.VatTu.NuocSanXuat,
                               SLDuTru = p.SoLuongDuTru,
                               TrangThai = EnumTrangThaiLoaiDuTru.ChoGoi,
                               SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                               LoaiKhoHayKhoa = true, // khoid
                               KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                               .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                           && x.NhapKhoVatTu.KhoId == khoId
                                                                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                           && x.NhapKhoVatTu.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                               .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                           &&
                                                                                           x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                           && x.NhapKhoVatTu.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               SLDuTruTKhoaDuyet = p.SoLuongDuTruTruongKhoaDuyet,
                               TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                               .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                           && 
                                                                                           x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                           && x.NhapKhoVatTu.DaHet != true
                                                                                           && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                           {
                                                                                               TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                               TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                           }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                  TenKhoTong = e.First().TenKhoTong,
                                                                                                  TongTon = e.Sum(q => q.TongTon)
                                                                                              }).ToList(),
                               HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                               HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                           }).GroupBy(x => new
                           {
                               x.VatTuId,
                               x.Loai
                           }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildNhaThuocBenhVien()
                           {
                               LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                               Loai = item.First().Loai,
                               VatTuId = item.First().VatTuId,
                               VatTu = item.First().VatTu,
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
                               // = item.FirstOrDefault().SLDuTruKDuocDuyet,
                               HopDongChuahapList = item.FirstOrDefault().HopDongChuahapList,
                               NhomDieuTri = item.FirstOrDefault().NhomDieuTri,
                               SLDuTruTKhoaDuyet = item.FirstOrDefault().SLDuTruTKhoaDuyet,
                               TongTonList = item.FirstOrDefault().TongTonList,
                               LoaiVatTu = item.FirstOrDefault().LoaiVatTu,
                               Id = item.FirstOrDefault().Id
                           });
                var dataOrderBy = query.AsQueryable();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            if (khoaId != 0)
            {
                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuId)
                               .Select(p => new DuTruMuaVatTuTaiKhoaDuocChildGridVo()
                               {
                                   Id = p.Id,
                                   Loai = p.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                   LoaiVatTu = p.LaVatTuBHYT,
                                   VatTuId = p.VatTuId,
                                   VatTu = p.VatTu.Ten,
                                   HoatChat = p.VatTu.QuyCach,


                                   DVT = p.VatTu.DonViTinh,

                                   NhaSX = p.VatTu.NhaSanXuat,
                                   NuocSX = p.VatTu.NuocSanXuat,
                                   KhoaPhongId = khoaId,
                                   SLDuTru = p.SoLuongDuTru,
                                   SLDuKienSuDungTrongKy = p.SoLuongDuKienSuDung,
                                   TrangThai = EnumTrangThaiLoaiDuTru.ChoDuyet,
                                   LoaiKhoHayKhoa = false,// khoaid
                                   KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                               //&& x.NhapKhoVatTu.kh == khoVatTuChiTietId
                                                                                               && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   KhoTongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                               && 
                                                                                               x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                               && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                   SLDuTruTKhoaDuyet = p.SoLuongDuTru,
                                   SLDuTruKDuocDuyet = p.SoLuongDuTru,// todo
                                   TongTonList = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == p.VatTuId
                                                                                               && 
                                                                                               x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                               && x.LaVatTuBHYT == p.LaVatTuBHYT
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                               && x.SoLuongDaXuat < x.SoLuongNhap).Select(x => new KhoTongTon()
                                                                                               {
                                                                                                   TenKhoTong = x.NhapKhoVatTu.Kho.Ten,
                                                                                                   TongTon = x.SoLuongNhap - x.SoLuongDaXuat
                                                                                               }).GroupBy(q => q.TenKhoTong)
                                                                                              .Select(e => new KhoTongTon
                                                                                              {
                                                                                                  TenKhoTong = e.First().TenKhoTong,
                                                                                                  TongTon = e.Sum(q => q.TongTon)
                                                                                              }).ToList(),
                                   HDChuaNhap = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId).Sum(a => a.SoLuong - a.SoLuongDaCap),
                                   HopDongChuahapList = _hopDongThauVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuId == p.VatTuId)
                                                                                                .Select(z => new HopDongChuaNhap()
                                                                                                {
                                                                                                    TenHopDong = z.HopDongThauVatTu.SoHopDong,
                                                                                                    TongTon = z.SoLuong - z.SoLuongDaCap
                                                                                                }).GroupBy(q => q.TenHopDong)
                                                                                                   .Select(e => new HopDongChuaNhap
                                                                                                   {
                                                                                                       TenHopDong = e.First().TenHopDong,
                                                                                                       TongTon = e.Sum(q => q.TongTon)
                                                                                                   }).ToList(),
                               }).GroupBy(x => new
                               {
                                   x.VatTuId,
                                   x.Loai
                               }).Select(item => new DuTruMuaVatTuTaiKhoaDuocChildGridVo
                               {
                                   LoaiKhoHayKhoa = item.First().LoaiKhoHayKhoa,
                                   Id = item.First().Id,
                                   LoaiVatTu = item.First().LoaiVatTu,
                                   TrangThai = item.First().TrangThai,
                                   Loai = item.First().Loai,
                                   VatTuId = item.First().VatTuId,
                                   VatTu = item.First().VatTu,
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

                var dataOrderBy = query.OrderBy(x => x.LoaiVatTu == true).ToList();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var resdataOrderBy = dataOrderBy.Select(o =>
                {
                    var listKhoTheoKhoaPhong = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(x => x.Id == duTruMuaVatTuId).SelectMany(s => s.DuTruMuaVatTus).ToList();
                    foreach (var lst in listKhoTheoKhoaPhong)
                    {
                        o.KhoDuTruTon += _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                                                   .Where(x => x.VatTuBenhVienId == o.VatTuId
                                                                                               && x.NhapKhoVatTu.KhoId == lst.KhoId
                                                                                               && x.NhapKhoVatTu.DaHet != true
                                                                                                && x.LaVatTuBHYT == o.LoaiVatTu
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruVatTuChiTietId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            var laDPHYT = Convert.ToBoolean(queryString[2]);
            if (loaiDP == true)  // khoid
            {
                var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == laDPHYT)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                 });
                var dataOrderBy = query.AsQueryable();
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };

            }
            if (loaiDP == false)  // khoaId
            {

                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == laDPHYT)
                                                               .SelectMany(s => s.DuTruMuaVatTuChiTiets)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
        public async Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            long.TryParse(queryString[0], out long duTruVatTuChiTietId);
            var loaiDP = Convert.ToBoolean(queryString[1]);
            var laDPHYT = Convert.ToBoolean(queryString[2]);
            if (loaiDP == true)  // khoid
            {
                var query = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == laDPHYT)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                     SLDuTru = cc.SoLuongDuTru,
                     SLDuKienSuDungTrongKy = cc.SoLuongDuKienSuDung
                 });
                var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };

            }
            if (loaiDP == false)  // khoaId
            {

                var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking.Where(x => x.Id == duTruVatTuChiTietId
                                                               && x.LaVatTuBHYT == laDPHYT)
                                                               .SelectMany(s => s.DuTruMuaVatTuChiTiets)
                 .Select(cc => new DuTruMuaVatTuTaiKhoaDuocChildChildGridVo()
                 {
                    
                     Kho = cc.DuTruMuaVatTu.Kho.Ten,
                     KyDuTru = cc.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + cc.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
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
        public DuTruMuaVatTuChiTietGoiViewGridVo GetDuTruMuaVatTuChiTietGoi(long idDuTruMua)
        {
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == nguoiDangLogin).Select(s => s.User.HoTen).FirstOrDefault();
            var listDuTruVatTuTheoKhoaChiTiet = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                                                                    .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.KhoDuocDuyet == true && s.DuTruMuaVatTuKhoDuocId == null)
                                                                    .SelectMany(o => o.DuTruMuaVatTuTheoKhoaChiTiets)
                                                                    .Include(o => o.DuTruMuaVatTuTheoKhoa).ThenInclude(o => o.DuTruMuaVatTus).Include(o => o.DuTruMuaVatTuChiTiets)
                                                                    .Include(o => o.DuTruMuaVatTuTheoKhoa).ThenInclude(o => o.KhoaPhong)
                                                                    .Include(o => o.VatTu)
                                                                    ;
            var listDuTruVatTuChiIiet = BaseRepository.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.TruongKhoaDuyet == true && s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && s.DuTruMuaVatTuKhoDuocId == null)
                                                                    .SelectMany(o => o.DuTruMuaVatTuChiTiets)
                                                                    .Include(o => o.DuTruMuaVatTu).ThenInclude(o => o.Kho)
                                                                    .Include(o => o.VatTu)
                                                                    .ToList();
            var duTruTheoKhoa = listDuTruVatTuTheoKhoaChiTiet.Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList1()
            {
                Id = itemc.Id,
                DuTruMuaVatTuTheoKhoaChiTietId = itemc.Id,
                //DuTruMuaVatTuKhoaDuId = itemc.Id,
                Khoa = itemc.DuTruMuaVatTuTheoKhoa.KhoaPhong.Ten,
                Loai = itemc.LaVatTuBHYT,
                VatTuId = itemc.VatTuId,
                TenVatTu = itemc.VatTu.Ten,
              
                NhaSX = itemc.VatTu.NhaSanXuat,
                NuocSX = itemc.VatTu.NuocSanXuat,
                SLDuTru = itemc.SoLuongDuTru,
                SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ?itemc.SoLuongDuTru : itemc.SoLuongDuTruTruongKhoaDuyet,
                SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruKhoDuocDuyet == null ? itemc.SoLuongDuTru : (int)itemc.SoLuongDuTruKhoDuocDuyet,
                VatTuDuTruTheoKhoaId = itemc.DuTruMuaVatTuTheoKhoaId,
                DuTruMuaVatTuTheoKhoaId = itemc.DuTruMuaVatTuTheoKhoaId,
            }).ToList();
            var duTruTaiKho = listDuTruVatTuChiIiet.Select(itemc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList1()
            {
                Id = itemc.Id,
                DuTruMuaVatTuChiTietId = itemc.Id,
                //DuTruMuaVatTuKhoId = itemc.Id,
                DuTruMuaVatTuId = itemc.DuTruMuaVatTuId,
                DuTruMuaVatTuTheoKhoaId = 0,
                Khoa = "Kho Duoc",
                Loai = itemc.LaVatTuBHYT,
                VatTuId = itemc.VatTuId,
                TenVatTu = itemc.VatTu.Ten,
                NhaSX = itemc.VatTu.NhaSanXuat,
                NuocSX = itemc.VatTu.NuocSanXuat,
                SLDuTru = itemc.SoLuongDuTru,
                SLDuKienSuDungTrongKho = itemc.SoLuongDuKienSuDung,
                SLDuTruTKhoDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ? itemc.SoLuongDuTru : (int)itemc.SoLuongDuTruTruongKhoaDuyet,
                SLDuTruKhoDuocDuyet = itemc.SoLuongDuTruTruongKhoaDuyet == null ? itemc.SoLuongDuTru : (int)itemc.SoLuongDuTruTruongKhoaDuyet
            }).ToList();
            var dutruUnion = duTruTheoKhoa.Union(duTruTaiKho);
            var dutrus = dutruUnion.GroupBy(x => new
            {
                x.Loai,
                x.VatTuId
            }).Select(itemcc => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList1()
            {
                Id = itemcc.FirstOrDefault().Id,
                Loai = itemcc.FirstOrDefault().Loai,
                VatTuId = itemcc.First().VatTuId,
                TenVatTu = itemcc.First().TenVatTu,
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
                var listKho = _khoRepository.TableNoTracking.ToList();
                foreach (var dutruTong in dutrus)
                {
                    foreach (var dutru in dutruUnion.Where(o => o.VatTuId == dutruTong.VatTuId && o.Loai == dutruTong.Loai))
                    {
                        if (dutru.DuTruMuaVatTuTheoKhoaId != 0)
                        {
                            var listDuTruMuaVatTu = listDuTruVatTuTheoKhoaChiTiet.First(o => o.DuTruMuaVatTuTheoKhoaId == dutru.DuTruMuaVatTuTheoKhoaId).DuTruMuaVatTuTheoKhoa.DuTruMuaVatTus;
                           var itemChild = listDuTruVatTuTheoKhoaChiTiet.Where(o => o.DuTruMuaVatTuTheoKhoaId == dutru.DuTruMuaVatTuTheoKhoaId && o.VatTuId == dutruTong.VatTuId && o.LaVatTuBHYT == dutruTong.Loai)
                                .SelectMany(o => o.DuTruMuaVatTuChiTiets).Select(o => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListGoiChild1
                                {
                                    Kho = listKho.Where(x => x.Id == listDuTruMuaVatTu.First(k => k.Id == o.DuTruMuaVatTuId).KhoId).First().Ten,
                                    KyDuTru = o.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + o.DuTruMuaVatTu.TuNgay.ApplyFormatDate(),
                                    SLDuKienTrongKy = o.SoLuongDuKienSuDung,
                                    SLDuTru = o.SoLuongDuTru,
                                    DuTruMuaVatTuTheoKhoaChiTietId = o.DuTruMuaVatTuTheoKhoaChiTietId.GetValueOrDefault(),
                                    DuTruMuaVatTuTheoKhoaId = o.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoaId,
                                    Khoa = o.DuTruMuaVatTuTheoKhoaChiTiet != null && o.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoa != null &&
                                          o.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoa.KhoaPhong != null ? o.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoa.KhoaPhong.Ten : null
                                }).ToList();
                            dutruTong.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild.AddRange(itemChild);
                        }
                        if (dutru.DuTruMuaVatTuId != 0)
                        {
                            var test = listDuTruVatTuChiIiet.Where(o => o.DuTruMuaVatTuId == dutru.DuTruMuaVatTuId && o.VatTuId == dutruTong.VatTuId && o.LaVatTuBHYT == dutruTong.Loai);
                            var itemChild = listDuTruVatTuChiIiet.Where(o => o.DuTruMuaVatTuId == dutru.DuTruMuaVatTuId && o.VatTuId == dutruTong.VatTuId && o.LaVatTuBHYT == dutruTong.Loai)
                                .Select(o => new ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListGoiChild1
                                {
                                    Kho = o.DuTruMuaVatTu.Kho.Ten,
                                    KyDuTru = o.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + '-' + o.DuTruMuaVatTu.TuNgay.ApplyFormatDate(),
                                    SLDuKienTrongKy = o.SoLuongDuKienSuDung,
                                    SLDuTru = o.SoLuongDuTru,
                                    DuTruMuaVatTuId = o.DuTruMuaVatTuId,
                                    DuTruMuaVatTuChiTietId = o.Id,
                                    Khoa = "Khoa Dược"
                                }).ToList();
                            dutruTong.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild.AddRange(itemChild);
                        }
                    }
                }
            }




            var listDuTruVatTuTheoKhoaId = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                                                                    .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.KhoDuocDuyet == true && s.DuTruMuaVatTuKhoDuocId == null)
                                                                    .Select(x => x.Id).ToList();
            var duTruVatTuId = BaseRepository.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.TruongKhoaDuyet == true && s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && s.DuTruMuaVatTuKhoDuocId == null)
                                                                    .Select(x => x.Id).ToList();
            var queryChiTietKhoLe = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.KhoDuocDuyet == true && s.KhoaPhongId != null && s.DuTruMuaVatTuKhoDuocId == null)
                                          .Select(item => new DuTruMuaVatTuChiTietGoiViewGridVo()
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
                                              DuTruVatTuTheoKhoaId = item.Id,
                                              ListDuTruVatTuTheoKhoaId = listDuTruVatTuTheoKhoaId,
                                              TrangThaiHienThi = EnumTrangThaiDuTruKhoaDuoc.ChoGoi.GetDescription(),
                                              TrangThai = EnumTrangThaiDuTruKhoaDuoc.ChoGoi
                                          }).ToList();
            var queryChiTietKhacKhoLe = BaseRepository.TableNoTracking
                                          .Where(s => s.KyDuTruMuaDuocPhamVatTuId == idDuTruMua && s.TruongKhoaDuyet == true && s.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && s.DuTruMuaVatTuKhoDuocId == null)
                                          .Select(item => new DuTruMuaVatTuChiTietGoiViewGridVo()
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
                                              ListDuTruVatTuId = duTruVatTuId,
                                              TrangThaiHienThi = EnumTrangThaiDuTruKhoaDuoc.ChoGoi.GetDescription(),
                                              TrangThai = EnumTrangThaiDuTruKhoaDuoc.ChoGoi
                                          }).ToList();
            List<DuTruMuaVatTuChiTietGoiViewGridVo> list = new List<DuTruMuaVatTuChiTietGoiViewGridVo>();
            if (queryChiTietKhoLe.Count() > 0 && queryChiTietKhacKhoLe.Count() > 0)
            {
                list = queryChiTietKhoLe.Union(queryChiTietKhacKhoLe).Select(item => new DuTruMuaVatTuChiTietGoiViewGridVo()
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
                    ListDuTruVatTuId = item.ListDuTruVatTuId,
                    thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = dutrus,
                    TrangThaiHienThi = EnumTrangThaiDuTruKhoaDuoc.ChoGoi.GetDescription(),
                    TrangThai = EnumTrangThaiDuTruKhoaDuoc.ChoGoi 
                }).ToList();
            }
            if (queryChiTietKhoLe.Count() > 0 && queryChiTietKhacKhoLe.Count() == 0)
            {
                list = queryChiTietKhoLe.Select(item => new DuTruMuaVatTuChiTietGoiViewGridVo()
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
                    ListDuTruVatTuId = item.ListDuTruVatTuId,
                    thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = dutrus,
                    TrangThai = EnumTrangThaiDuTruKhoaDuoc.ChoGoi,
                    TrangThaiHienThi = EnumTrangThaiDuTruKhoaDuoc.ChoGoi.GetDescription(),
                }).ToList();
            }
            if (queryChiTietKhoLe.Count() == 0 && queryChiTietKhacKhoLe.Count() > 0)
            {
                list = queryChiTietKhacKhoLe.Select(item => new DuTruMuaVatTuChiTietGoiViewGridVo()
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
                    ListDuTruVatTuId = item.ListDuTruVatTuId,
                    thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = dutrus,
                    TrangThai = EnumTrangThaiDuTruKhoaDuoc.ChoGoi,
                    TrangThaiHienThi = EnumTrangThaiDuTruKhoaDuoc.ChoGoi.GetDescription()
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
            var templatePhieuInTongHopDuTruVatTuTaiKhoa = new Template();

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

            var kyDuTruId = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                                            .Where(p => p.DuTruMuaVatTuKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId) // khoaduocId
                                            .SelectMany(d => d.DuTruMuaVatTus)
                                            .Select(d => d.KyDuTruMuaDuocPhamVatTuId)
                                            .ToList();
            if (kyDuTruId.Count() > 0)
            {
                var kyDuTruInfo = _kyDuTruMuaDuocPhamVatTuRepository.TableNoTracking.Where(d => d.Id == kyDuTruId.First())
                    .Select(d => new {
                        TuNgay = d.TuNgay.ApplyFormatDate(),
                        DenNgay = d.DenNgay.ApplyFormatDate()
                    }).First();
                if (kyDuTruInfo != null)
                {
                    tuNgay = kyDuTruInfo.TuNgay;
                    denNgay = kyDuTruInfo.DenNgay;
                }
            }

            var duTruMuaVatTuChiTiet = string.Empty;
            templatePhieuInTongHopDuTruVatTuTaiKhoa = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuInTongHopDuTruVatTuTaiKhoa")).First();
            var duTruMuaVatTuKhoaChiTiets = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                                            .Where(p => p.DuTruMuaVatTuKhoDuocChiTiet.DuTruMuaVatTuKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId) // khoaduocId
                                            .Select(s => new PhieuMuaDuTruVatTuChiTietData
                                            {
                                                MaHang = s.VatTu.Ma, // todo: cần confirm hỏi lại
                                                Ten = s.VatTu.Ten,
                                                DonVi = s.VatTu.DonViTinh,
                                                SoLuong = s.SoLuongDuTruTruongKhoaDuyet,
                                                GhiChu = "",
                                                LaVatTuBHYT = s.LaVatTuBHYT,
                                                VatTuId = s.VatTuId
                                            })
                                            .GroupBy(d => new { d.VatTuId, d.LaVatTuBHYT })
                                            .Select(item => new PhieuMuaDuTruVatTuChiTietData
                                            {
                                                MaHang = item.First().MaHang, // todo: cần confirm hỏi lại
                                                Ten = item.First().Ten,
                                                DonVi = item.First().DonVi,
                                                SoLuong = item.Sum(a => a.SoLuong),
                                                GhiChu = "",
                                                LaVatTuBHYT = item.First().LaVatTuBHYT,
                                                VatTuId = item.First().VatTuId
                                            })

                                            .ToList();

            var duTruMuaVatTuChiTiets = _duTruMuaVatTuChiTietRepository.TableNoTracking
                                          .Where(p => p.DuTruMuaVatTuKhoDuocChiTiet.DuTruMuaVatTuKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId) // khoaduocId
                                          .Select(s => new PhieuMuaDuTruVatTuChiTietData
                                          {
                                              MaHang = s.VatTu.Ma, // todo: cần confirm hỏi lại
                                              Ten = s.VatTu.Ten,
                                             
                                            
                                              DonVi = s.VatTu.DonViTinh,
                                              SoLuong = (int)s.SoLuongDuTruTruongKhoaDuyet,
                                              GhiChu = "",
                                              LaVatTuBHYT = s.LaVatTuBHYT,
                                              VatTuId = s.VatTuId
                                          })
                                          .GroupBy(d => new { d.VatTuId, d.LaVatTuBHYT })
                                            .Select(item => new PhieuMuaDuTruVatTuChiTietData
                                            {
                                                MaHang = item.First().MaHang, // todo: cần confirm hỏi lại
                                                Ten = item.First().Ten,
                                                DonVi = item.First().DonVi,
                                                SoLuong = item.Sum(a => a.SoLuong),
                                                GhiChu = "",
                                                LaVatTuBHYT = item.First().LaVatTuBHYT,
                                                VatTuId = item.First().VatTuId
                                            })
                                            .ToList();
            var query = duTruMuaVatTuKhoaChiTiets.Union(duTruMuaVatTuChiTiets).ToList();
            var STT = 1;

            if (query.Any(p => p.LaVatTuBHYT))
            {
                duTruMuaVatTuChiTiet += headerBHYT;
                var queryBHYT = query.Where(x => x.LaVatTuBHYT).ToList();
                foreach (var item in queryBHYT)
                {
                    duTruMuaVatTuChiTiet = duTruMuaVatTuChiTiet
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
            if (query.Any(p => !p.LaVatTuBHYT))
            {
                duTruMuaVatTuChiTiet += headerKhongBHYT;
                var queryKhongBHYT = query.Where(x => !x.LaVatTuBHYT).ToList();
                foreach (var item in queryKhongBHYT)
                {
                    duTruMuaVatTuChiTiet = duTruMuaVatTuChiTiet
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
            var tennhanVienLap = _duTruMuaVatTuKhoDuocChiTietRepository.TableNoTracking.Include(s => s.DuTruMuaVatTuKhoDuoc).Where(c => c.DuTruMuaVatTuKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId).Select(x => x.DuTruMuaVatTuKhoDuoc.GiamDoc.User.HoTen).FirstOrDefault();
            var data = new
            {
                Header = hearder,
                QuyCachPhieuMuaDuTruVatTu = "BMBH-KD01.01",
                DuTruMuaVatTuChiTiet = duTruMuaVatTuChiTiet,
                KhoaPhong = "Khoa VTYT",
                NhanVienLap = tennhanVienLap != null ? tennhanVienLap : "",

                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString(),
                TuNgay = tuNgay,
                DenNgay =denNgay
            };

            contentThuoc = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInTongHopDuTruVatTuTaiKhoa.Body, data);

            return contentThuoc;

        }

        #endregion
    }
}
