using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;

namespace Camino.Services.NhapKhoVatTus
{
    public partial class NhapKhoVatTuService
    {
        #region Hoàn trả vật tư 

        public async Task<GridDataSource> GetDanhSachHoanTraVatTuForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {

            BuildDefaultSortExpression(queryInfo);
            var queryObject = new HoanTraVatTuSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<HoanTraVatTuSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = DataYeuCauHoanTraVatTu(null, queryInfo);
            var queryTuChoiDuyet = DataYeuCauHoanTraVatTu(false, queryInfo);
            var queryTuDaDuyet = DataYeuCauHoanTraVatTu(true, queryInfo);

            var query = _yeuCauTraVatTuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachHoanTraVatTuVo()).AsQueryable();

            if (queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false && queryObject.DaDuyet == false)
            {
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
                queryObject.DaDuyet = true;
            }

            var isHaveQuery = false;
            if (queryObject.DangChoDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoDuyet);
                }
                else
                {
                    query = queryDangChoDuyet;
                    isHaveQuery = true;
                }
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
                    isHaveQuery = true;
                }
            }

            if (queryObject != null)
            {
                if(!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, g => g.TenNguoiYeuCau, g => g.HoanTraTuKho, g => g.HoanTraVeKho, g => g.SoPhieu, g => g.NguoiDuyet);
                }

                if (queryObject.RangeNhap != null)
                {
                    if (queryObject.RangeNhap.startDate != null)
                    {
                        var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.NgayYeuCau.Date);
                    }

                    if (queryObject.RangeNhap.endDate != null)
                    {
                        var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault();
                        query = query.Where(p => denNgay.Date >= p.NgayYeuCau.Date);
                    }
                }

                if (queryObject.RangeDuyet != null)
                {
                    if(queryObject.RangeDuyet.startDate != null)
                    {
                        var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && tuNgay.Date <= p.NgayDuyet.Value.Date);
                    }

                    if (queryObject.RangeDuyet.endDate != null)
                    {
                        var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && denNgay.Date >= p.NgayDuyet.Value.Date);
                    }
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayYeuCau desc,SoPhieu asc") && (queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalDanhSachHoanTraVatTuForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryObject = new HoanTraVatTuSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<HoanTraVatTuSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = DataYeuCauHoanTraVatTu(null, queryInfo);
            var queryTuChoiDuyet = DataYeuCauHoanTraVatTu(false, queryInfo);
            var queryTuDaDuyet = DataYeuCauHoanTraVatTu(true, queryInfo);

            var query = _yeuCauTraVatTuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachHoanTraVatTuVo()).AsQueryable();

            if (queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false && queryObject.DaDuyet == false)
            {
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
                queryObject.DaDuyet = true;
            }

            var isHaveQuery = false;
            if (queryObject.DangChoDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoDuyet);
                }
                else
                {
                    query = queryDangChoDuyet;
                    isHaveQuery = true;
                }
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
                    isHaveQuery = true;
                }
            }
            
            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, g => g.TenNguoiYeuCau, g => g.HoanTraTuKho, g => g.HoanTraVeKho, g => g.SoPhieu, g => g.NguoiDuyet);
                }

                if (queryObject.RangeNhap != null)
                {
                    if (queryObject.RangeNhap.startDate != null)
                    {
                        var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.NgayYeuCau.Date);
                    }

                    if (queryObject.RangeNhap.endDate != null)
                    {
                        var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault();
                        query = query.Where(p => denNgay.Date >= p.NgayYeuCau.Date);
                    }
                }

                if (queryObject.RangeDuyet != null)
                {
                    if (queryObject.RangeDuyet.startDate != null)
                    {
                        var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && tuNgay.Date <= p.NgayDuyet.Value.Date);
                    }

                    if (queryObject.RangeDuyet.endDate != null)
                    {
                        var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault();
                        query = query.Where(p => p.NgayDuyet != null && denNgay.Date >= p.NgayDuyet.Value.Date);
                    }
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDanhSachHoanTraVatTuChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var hoanTraVatTuId = Convert.ToDouble(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraVatTuChiTietRepository.TableNoTracking.Where(cc => cc.YeuCauTraVatTuId == hoanTraVatTuId)
                                                               .Include(cc => cc.KhoViTri)
                                                               .Include(cc => cc.VatTuBenhVien).ThenInclude(cc => cc.VatTus).ThenInclude(cc => cc.DonViTinh)
                                                               .Include(cc => cc.HopDongThauVatTu).ThenInclude(cc => cc.HopDongThauVatTuChiTiets)
                                                               .Select(s => new DanhSachHoanTraVatTuChiTietVo
                                                               {
                                                                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",

                                                                   Id = s.Id,
                                                                   Ma = s.VatTuBenhVien.Ma,
                                                                   VatTu = s.VatTuBenhVien.VatTus.Ten,
                                                                   DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                                                                   LoaiBHYT = s.LaVatTuBHYT,
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung,
                                                                   SoLuongHoanTra = s.SoLuongTra,
                                                                   SoLuongHoanTraDisplay = s.SoLuongTra.ApplyNumber()
                                                               });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.VatTu, g => g.Ma, g => g.SoLo);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalDanhSachHoanTraVatTuChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var hoanTraVatTuId = Convert.ToDouble(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraVatTuChiTietRepository.TableNoTracking.Where(cc => cc.YeuCauTraVatTuId == hoanTraVatTuId)
                                                               .Include(cc => cc.KhoViTri)
                                                               .Include(cc => cc.VatTuBenhVien).ThenInclude(cc => cc.VatTus).ThenInclude(cc => cc.DonViTinh)
                                                               .Include(cc => cc.HopDongThauVatTu).ThenInclude(cc => cc.HopDongThauVatTuChiTiets)
                                                               .Select(s => new DanhSachHoanTraVatTuChiTietVo
                                                               {
                                                                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",

                                                                   Id = s.Id,
                                                                   Ma = s.VatTuBenhVien.Ma,
                                                                   VatTu = s.VatTuBenhVien.VatTus.Ten,
                                                                   DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                                                                   LoaiBHYT = s.LaVatTuBHYT,
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung,
                                                                   SoLuongHoanTra = s.SoLuongTra,
                                                                   SoLuongHoanTraDisplay = s.SoLuongTra.ApplyNumber()
                                                               });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.VatTu, g => g.Ma, g => g.SoLo);
            var countTask = query.CountAsync(); await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        private IQueryable<DanhSachHoanTraVatTuVo> DataYeuCauHoanTraVatTu(bool? duocKeToanDuyet, QueryInfo queryInfo)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = _yeuCauTraVatTuRepository.TableNoTracking.Where(p => p.DuocDuyet == duocKeToanDuyet)// && p.KhoNhap.KhoaPhongId == phongBenhVien.KhoaPhongId
                                       .Select(s => new DanhSachHoanTraVatTuVo
                                       {
                                           Id = s.Id,
                                           SoPhieu = s.SoPhieu,
                                           NguoiYeuCauId = s.NhanVienYeuCau.Id,
                                           TenNguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                                           NgayYeuCau = s.NgayYeuCau,
                                           NgayYeuCauDisplay = s.NgayYeuCau.ApplyFormatDateTimeSACH(),
                                           TinhTrang = s.DuocDuyet,
                                           HoanTraTuKhoId = s.KhoXuatId,
                                           HoanTraTuKho = s.KhoXuat.Ten,
                                           HoanTraVeKhoId = s.KhoXuatId,
                                           HoanTraVeKho = s.KhoNhap.Ten,
                                           NguoiDuyet = s.NhanVienDuyet.User.HoTen,
                                           NgayDuyet = s.NgayDuyet,
                                           NgayDuyetDisplay = s.NgayDuyet != null ? s.NgayDuyet.Value.ApplyFormatDateTimeSACH() : string.Empty,
                                           GhiChu = s.GhiChu
                                           //DTNgayNhap = s.NgayNhap,
                                       });
            var result = query.ApplyLike(queryInfo.SearchTerms, g => g.TenNguoiYeuCau, g => g.HoanTraTuKho, g => g.HoanTraVeKho, g => g.SoPhieu, g => g.NguoiDuyet)
                              .OrderBy((queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")) ? queryInfo.SortString : "NgayYeuCau desc,SoPhieu asc");
            return result;
        }

        public async Task<ThongTinHoanTraVatTu> GetThongTinHoanTraVatTu(long yeuCauHoanTraVatTuId)
        {
            var thongTinDuyetKhooVatTu = await _yeuCauTraVatTuRepository.TableNoTracking.Where(cc => cc.Id == yeuCauHoanTraVatTuId)
                                                                           .Select(cc => new ThongTinHoanTraVatTu
                                                                           {
                                                                               NgayYeuCau = cc.NgayYeuCau,
                                                                               NguoiYeuCauId = cc.NhanVienYeuCauId,
                                                                               TenNguoiYeuCau = cc.NhanVienYeuCau.User.HoTen,

                                                                               HoanTraTuKhoId = cc.KhoXuatId,
                                                                               HoanTraTuKho = cc.KhoXuat.Ten,
                                                                               HoanTraVeKhoId = cc.KhoNhapId,
                                                                               HoanTraVeKho = cc.KhoNhap.Ten,

                                                                               TinhTrang = cc.DuocDuyet,
                                                                               NgayDuyet = cc.NgayDuyet,
                                                                               NguoiDuyet = cc.NhanVienDuyet.User.HoTen,
                                                                               NguoiDuyetId = cc.NhanVienDuyetId,

                                                                               LyDoHuy = cc.LyDoKhongDuyet,
                                                                               GhiChu = cc.GhiChu
                                                                           }).FirstOrDefaultAsync();

            var yeuCauTraVatTuChiTiet = await _yeuCauTraVatTuChiTietRepository.TableNoTracking.Where(p => p.YeuCauTraVatTuId == yeuCauHoanTraVatTuId)
                                                                                              .FirstOrDefaultAsync();

            if (yeuCauTraVatTuChiTiet != null)
            {
                var xuatKhoVatTu = await _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(p => p.Id == yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTriId)
                                                                                            .Select(p => p.XuatKhoVatTuChiTiet.XuatKhoVatTu)
                                                                                            .FirstOrDefaultAsync();

                if (xuatKhoVatTu != null)
                {
                    thongTinDuyetKhooVatTu.NguoiTraId = xuatKhoVatTu.NguoiXuatId;
                    thongTinDuyetKhooVatTu.TenNguoiTra = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoVatTu.NguoiXuatId).Select(p => p.HoTen).FirstOrDefaultAsync();
                    thongTinDuyetKhooVatTu.NguoiNhanId = xuatKhoVatTu.NguoiNhanId;
                    thongTinDuyetKhooVatTu.TenNguoiNhan = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoVatTu.NguoiNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
                }
            }


            return thongTinDuyetKhooVatTu;
        }

        public async Task TuChoiDuyetHoanTraVatTu(long id, string lyDoHuy)
        {
            var yeuCauHoanTraVatTu = _yeuCauTraVatTuRepository.Table
                .Where(p => p.Id == id)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.NhapKhoVatTuChiTiet)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.XuatKhoVatTuChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraVatTu != null && yeuCauHoanTraVatTu.DuocDuyet == null)
            {
                //Cập nhật trạng thái yêu cầu
                yeuCauHoanTraVatTu.DuocDuyet = false;
                yeuCauHoanTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraVatTu.NgayDuyet = DateTime.Now;
                yeuCauHoanTraVatTu.LyDoKhongDuyet = lyDoHuy;

                //Sửa nhập kho                
                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                }
                //Xoá xuất kho chi tiết & xuất kho chi tiết vị trí
                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {                    
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                    {
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                    }
                    yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTriId = null;
                }

                _yeuCauTraVatTuRepository.Context.SaveChanges();
            }
        }

        //public async Task TuChoiDuyetHoanTraVatTuOld(long id, string lyDoHuy)
        //{
        //    var yeuCauTraVatTuChiTiets = await _yeuCauTraVatTuChiTietRepository.TableNoTracking.Where(p => p.YeuCauTraVatTuId == id)
        //                                                                                          .Include(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.NhapKhoVatTuChiTiet)
        //                                                                                          .ToListAsync();

        //    //Sửa nhập kho
        //    foreach (var item in yeuCauTraVatTuChiTiets)
        //    {
        //        item.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= item.SoLuongTra;
        //    }

        //    await _yeuCauTraVatTuChiTietRepository.UpdateAsync(yeuCauTraVatTuChiTiets);

        //    //Xoá xuất kho chi tiết & xuất kho chi tiết vị trí
        //    foreach (var item in yeuCauTraVatTuChiTiets)
        //    {

        //        long xuatKhoVatTuChiTietViTriId = item.XuatKhoVatTuChiTietViTri.Id;
        //        long xuatKhoVatTuChiTietId = item.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTietId;

        //        item.XuatKhoVatTuChiTietViTriId = null;

        //        await _xuatKhoVatTuChiTietViTriRepository.DeleteByIdAsync(xuatKhoVatTuChiTietViTriId);
        //        await _xuatKhoVatTuChiTietRepository.DeleteByIdAsync(xuatKhoVatTuChiTietId);
        //    }

        //    await _yeuCauTraVatTuChiTietRepository.UpdateAsync(yeuCauTraVatTuChiTiets);

        //    //Cập nhật trạng thái yêu cầu trả vật tư
        //    var yeuCauTraVatTu = await _yeuCauTraVatTuRepository.TableNoTracking.Where(p => p.Id == id).FirstOrDefaultAsync();

        //    yeuCauTraVatTu.DuocDuyet = false;
        //    yeuCauTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
        //    yeuCauTraVatTu.NgayDuyet = DateTime.Now;
        //    yeuCauTraVatTu.LyDoKhongDuyet = lyDoHuy;

        //    await _yeuCauTraVatTuRepository.UpdateAsync(yeuCauTraVatTu);

        //    //var yeuCauTraVatTu = await _yeuCauTraVatTuRepository.TableNoTracking.Where(o => o.Id == thongTinLyDoHuyNhapKhoVatTu.YeuCauTraVatTuId)
        //    //                                                               .FirstOrDefaultAsync();

        //    //if (yeuCauTraVatTu != null)
        //    //{
        //    //    yeuCauTraVatTu.DuocDuyet = false;
        //    //    yeuCauTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
        //    //    yeuCauTraVatTu.NgayDuyet = DateTime.Now;
        //    //    yeuCauTraVatTu.LyDoKhongDuyet = thongTinLyDoHuyNhapKhoVatTu.LyDoHuy;

        //    //    _yeuCauTraVatTuRepository.Update(yeuCauTraVatTu);

        //    //    //Câp nhật số lượng cho nhâp kho dược vật tư chi tiết theo nhapKhoVatTuChiTietId số lương nhập + sl hoàn trả
        //    //    var nhapKhoVatTuChiTietId = yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Select(cc => cc.XuatKhoVatTuChiTietViTri).Select(cc => cc.XuatKhoVatTuChiTietId).FirstOrDefault();
        //    //    var soLuongTra = yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Sum(cc => cc.SoLuongTra);
        //    //    CapNhatSoLuongNhapKhoVatTuTheoNhapKhoVatTuChiTietId(nhapKhoVatTuChiTietId, soLuongTra);


        //    //    //Xóa XuatKhoVatTuChiTietViTri và Xuất kho vật tư chi tiết
        //    //    XoaPhieu(yeuCauTraVatTu);

        //    //    return true;
        //    //}

        //    //return false;
        //}

        public async Task DuyetHoanTraNhapKho(long id, long nguoiNhanId, long nguoiXuatId)
        {
            var yeuCauHoanTraVatTu = _yeuCauTraVatTuRepository.Table
                .Where(p => p.Id == id)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.NhapKhoVatTuChiTiet)
                .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.XuatKhoVatTuChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraVatTu != null && yeuCauHoanTraVatTu.DuocDuyet == null)
            {
                var tenNguoiNhan = await _userRepository.TableNoTracking.Where(p => p.Id == nguoiNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
                var tenNguoiXuat = await _userRepository.TableNoTracking.Where(p => p.Id == nguoiXuatId).Select(p => p.HoTen).FirstOrDefaultAsync();
                //Yêu cầu hoàn trả vật tư
                yeuCauHoanTraVatTu.DuocDuyet = true;
                yeuCauHoanTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraVatTu.NgayDuyet = DateTime.Now;

                var xuatKhoVatTu = new XuatKhoVatTu
                {
                    KhoNhapId = yeuCauHoanTraVatTu.KhoNhapId,
                    KhoXuatId = yeuCauHoanTraVatTu.KhoXuatId,
                    LoaiXuatKho = EnumLoaiXuatKho.XuatQuaKhoKhac,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatQuaKhoKhac.GetDescription(),
                    NguoiNhanId = nguoiNhanId,
                    TenNguoiNhan = tenNguoiNhan,
                    NguoiXuatId = nguoiXuatId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayXuat = DateTime.Now,
                };

                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                        yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu = xuatKhoVatTu;
                }
                                
                var newNhapKhoVatTu = new NhapKhoVatTu
                {
                    XuatKhoVatTu = xuatKhoVatTu,
                    TenNguoiGiao = tenNguoiXuat,
                    NguoiGiaoId = nguoiXuatId,
                    NguoiNhapId = nguoiNhanId,
                    DaHet = false,
                    LoaiNguoiGiao = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayNhap = DateTime.Now,
                    KhoId = yeuCauHoanTraVatTu.KhoNhapId,
                };

                foreach (var yeuCauTraVatTuChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets)
                {
                    if (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri != null)
                    {
                        var newNhapKhoVatTuChiTiet = new NhapKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                            HopDongThauVatTuId = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            Solo = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                            HanSuDung = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                            SoLuongNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat,
                            DonGiaNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.DonGiaNhap,
                            VAT = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.VAT,
                            MaVach = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.MaVach,
                            MaRef = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.MaRef,
                            SoLuongDaXuat = 0,
                            NgayNhap = DateTime.Now,
                            LaVatTuBHYT = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                            NgayNhapVaoBenhVien = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                            PhuongPhapTinhGiaTriTonKho = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho,
                            TiLeTheoThapGia = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            KhoViTriId = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTriId,
                            TiLeBHYTThanhToan = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.TiLeBHYTThanhToan
                        };
                        newNhapKhoVatTu.NhapKhoVatTuChiTiets.Add(newNhapKhoVatTuChiTiet);
                    }
                }
                _nhapKhoVatTuRepository.Add(newNhapKhoVatTu);
            }
        }

        //public async Task DuyetHoanTraNhapKhoOld(long id, long nguoiNhanId, long nguoiXuatId)
        //{
        //    var yeuCauHoanTraVatTu = await _yeuCauTraVatTuRepository.TableNoTracking.Where(p => p.Id == id)
        //                                                                                  .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.XuatKhoVatTuChiTietViTri).ThenInclude(p => p.NhapKhoVatTuChiTiet).ThenInclude(p => p.NhapKhoVatTu)
        //                                                                                  .Include(p => p.YeuCauTraVatTuChiTiets).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)
        //                                                                                  .FirstOrDefaultAsync();

        //    if (yeuCauHoanTraVatTu != null)
        //    {
        //        var tenNguoiNhan = await _userRepository.TableNoTracking.Where(p => p.Id == nguoiNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
        //        var tenNguoiXuat = await _userRepository.TableNoTracking.Where(p => p.Id == nguoiXuatId).Select(p => p.HoTen).FirstOrDefaultAsync();

        //        //Yêu cầu hoàn trả vật tư
        //        yeuCauHoanTraVatTu.DuocDuyet = true;
        //        yeuCauHoanTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
        //        yeuCauHoanTraVatTu.NgayDuyet = DateTime.Now;

        //        _yeuCauTraVatTuRepository.Update(yeuCauHoanTraVatTu);

        //        //Xuất kho vật tư
        //        var xuatKhoVatTu = MapXuatKhoVatTu(yeuCauHoanTraVatTu, nguoiNhanId, tenNguoiNhan, nguoiXuatId);
        //        await _xuatKhoVatTuRepository.AddAsync(xuatKhoVatTu);

        //        var xuatKhoVatTuChiTietViTris = yeuCauHoanTraVatTu.YeuCauTraVatTuChiTiets.Select(p => p.XuatKhoVatTuChiTietViTri).ToList();
        //        foreach (var item in xuatKhoVatTuChiTietViTris)
        //        {
        //            var xuatKhoVatTuChiTiet = await _xuatKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.Id == item.XuatKhoVatTuChiTietId)
        //                                                                                          .FirstOrDefaultAsync();

        //            xuatKhoVatTuChiTiet.XuatKhoVatTuId = xuatKhoVatTu.Id;

        //            await _xuatKhoVatTuChiTietRepository.UpdateAsync(xuatKhoVatTuChiTiet);
        //        }

        //        //Nhập kho vật tư
        //        await MapNhapKhoVatTuSauKhiDuyet(yeuCauHoanTraVatTu, nguoiXuatId, nguoiNhanId, tenNguoiXuat, xuatKhoVatTu.Id);
        //    }

        //    //_yeuCauTraVatTuRepository.AutoCommitEnabled = false;
        //    //BaseRepository.AutoCommitEnabled = false;

        //    //var tenNguoiNhan = "";
        //    //var yeuCautraVatTu = await _yeuCauTraVatTuRepository.TableNoTracking.Where(o => o.Id == id)
        //    //                                                                  .Include(cc => cc.YeuCauTraVatTuChiTiets)
        //    //                                                                  .ThenInclude(cc => cc.XuatKhoVatTuChiTietViTri)
        //    //                                                                  .Include(cc => cc.YeuCauTraVatTuChiTiets)
        //    //                                                                  .ThenInclude(cc => cc.VatTuBenhVien)
        //    //                                                                  .ThenInclude(c => c.VatTus)
        //    //                                                                  .FirstOrDefaultAsync();
        //    //if (yeuCautraVatTu != null)
        //    //{
        //    //    var soLuongTra = yeuCautraVatTu.YeuCauTraVatTuChiTiets.Sum(cc => cc.SoLuongTra);
        //    //    yeuCautraVatTu.DuocDuyet = true;
        //    //    yeuCautraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
        //    //    yeuCautraVatTu.NgayDuyet = DateTime.Now;
        //    //    _yeuCauTraVatTuRepository.Update(yeuCautraVatTu);

        //    //    //Bước 1 map xuất kho vật tư
        //    //    var xuatKhoVatTuModel = MapNhapKhoVatTu(yeuCautraVatTu, nguoiNhanId, tenNguoiNhan, nguoiXuatId);
        //    //    _xuatKhoVatTuRepository.Add(xuatKhoVatTuModel);

        //    //    //Bước 2 tạo NhapKhoDuocPham và tương ứng NhapKhoDuocPhamChi : Input : XuatKhoVatTuChiTietViTri và dựa NhapKhoVatTuChiTietId select data củ
        //    //    var nhapKhoVatTu = MapNhapKhoVatTuChiTiet(yeuCautraVatTu, soLuongTra);

        //    //    BaseRepository.Add(nhapKhoVatTu);
        //    //    BaseRepository.Context.SaveChanges();
        //    //    return string.Empty;
        //    //}
        //    //return string.Empty;
        //}

        //private XuatKhoVatTu MapXuatKhoVatTu(YeuCauTraVatTu yeuCauTraVatTu, long nguoiNhanId, string tenNguoiNhan, long nguoiXuatId)
        //{
        //    var nhapKhoVatTu = new XuatKhoVatTu();

        //    nhapKhoVatTu.KhoNhapId = yeuCauTraVatTu.KhoNhapId;
        //    nhapKhoVatTu.KhoXuatId = yeuCauTraVatTu.KhoXuatId;

        //    nhapKhoVatTu.LoaiXuatKho = EnumLoaiXuatKho.XuatQuaKhoKhac;
        //    nhapKhoVatTu.LyDoXuatKho = EnumLoaiXuatKho.XuatQuaKhoKhac.GetDescription();

        //    nhapKhoVatTu.NguoiNhanId = nguoiNhanId;
        //    nhapKhoVatTu.TenNguoiNhan = tenNguoiNhan;

        //    nhapKhoVatTu.NguoiXuatId = nguoiXuatId;
        //    nhapKhoVatTu.LoaiNguoiNhan = LoaiNguoiGiaoNhan.TrongHeThong;
        //    nhapKhoVatTu.NgayXuat = DateTime.Now;
        //    nhapKhoVatTu.SoPhieu = yeuCauTraVatTu.SoPhieu;

        //    return nhapKhoVatTu;
        //}

        //private async Task MapNhapKhoVatTuSauKhiDuyet(YeuCauTraVatTu yeuCauTraVatTu, long nhanVienTraId, long nhanVienNhanId, string tenNhanVienTra, long xuatKhoVatTuId)
        //{
        //    var oldNhapKhoVatTu = yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Select(p => p.XuatKhoVatTuChiTietViTri).FirstOrDefault().NhapKhoVatTuChiTiet.NhapKhoVatTu;

        //    var newNhapKhoVatTu = new NhapKhoVatTu
        //    {
        //        SoChungTu = oldNhapKhoVatTu.SoChungTu,
        //        XuatKhoVatTuId = xuatKhoVatTuId,
        //        TenNguoiGiao = tenNhanVienTra,
        //        NguoiGiaoId = nhanVienTraId,
        //        NguoiNhapId = nhanVienNhanId,
        //        DaHet = false,
        //        LoaiNguoiGiao = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
        //        NgayNhap = DateTime.Now,
        //        //YeuCauNhapKhoVatTuId = 0,
        //        KhoId = yeuCauTraVatTu.KhoNhapId,
        //        //YeuCauLinhVatTuId = 0,
        //        SoPhieu = oldNhapKhoVatTu.SoPhieu
        //    };

        //    await _nhapKhoVatTuRepository.AddAsync(newNhapKhoVatTu);

        //    //var newNhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>();
        //    foreach (var yeucauTraVatTuChiTiet in yeuCauTraVatTu.YeuCauTraVatTuChiTiets)
        //    {
        //        var newNhapKhoVatTuChiTiet = new NhapKhoVatTuChiTiet
        //        {
        //            NhapKhoVatTuId = newNhapKhoVatTu.Id,
        //            VatTuBenhVienId = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.VatTuBenhVienId,
        //            HopDongThauVatTuId = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HopDongThauVatTuId,
        //            Solo = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
        //            HanSuDung = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
        //            SoLuongNhap = yeucauTraVatTuChiTiet.SoLuongTra,
        //            DonGiaNhap = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.DonGiaNhap,
        //            VAT = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.VAT,
        //            MaVach = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.MaVach,
        //            MaRef = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.MaRef,
        //            SoLuongDaXuat = 0,
        //            NgayNhap = DateTime.Now,
        //            LaVatTuBHYT = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT,
        //            NgayNhapVaoBenhVien = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
        //            PhuongPhapTinhGiaTriTonKho = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho,
        //            TiLeTheoThapGia = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.TiLeTheoThapGia,
        //            KhoViTriId = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTriId,
        //            DonGiaBan = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.DonGiaBan,
        //            TiLeBHYTThanhToan = yeucauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.TiLeBHYTThanhToan
        //        };

        //        await _nhapKhoVatTuChiTietRepository.AddAsync(newNhapKhoVatTuChiTiet);
        //    }
        //}

        //private NhapKhoVatTu MapNhapKhoVatTuChiTiet(YeuCauTraVatTu yeuCauTraVatTu, double soLuongTra)
        //{
        //    var xuatKhoVatTuChiTietViTri = yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Select(cc => cc.XuatKhoVatTuChiTietViTri).FirstOrDefault();
        //    if (xuatKhoVatTuChiTietViTri != null)
        //    {
        //        var oldNhapKhoVatTuChiTiet = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(cc => cc.Id == xuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId).Include(cc => cc.NhapKhoVatTu).FirstOrDefault();
        //        if (oldNhapKhoVatTuChiTiet != null)
        //        {
        //            return TaoPhieuNhapKhoChiTiet(oldNhapKhoVatTuChiTiet, soLuongTra);
        //        }

        //    }
        //    return new NhapKhoVatTu();
        //}

        //public NhapKhoVatTu TaoPhieuNhapKhoChiTiet(NhapKhoVatTuChiTiet nhapKhoVatTuChiTiet, double soLuongTra)
        //{
        //    var nhapKhoVatTu = new NhapKhoVatTu
        //    {
        //        YeuCauNhapKhoVatTuId = nhapKhoVatTuChiTiet.NhapKhoVatTu.YeuCauNhapKhoVatTuId,
        //        KhoId = nhapKhoVatTuChiTiet.NhapKhoVatTu.KhoId,
        //        SoChungTu = nhapKhoVatTuChiTiet.NhapKhoVatTu.SoChungTu,
        //        TenNguoiGiao = nhapKhoVatTuChiTiet.NhapKhoVatTu.TenNguoiGiao,
        //        NguoiGiaoId = nhapKhoVatTuChiTiet.NhapKhoVatTu.NguoiGiaoId,
        //        NguoiNhapId = nhapKhoVatTuChiTiet.NhapKhoVatTu.NguoiNhapId,
        //        LoaiNguoiGiao = nhapKhoVatTuChiTiet.NhapKhoVatTu.LoaiNguoiGiao,
        //        SoPhieu = nhapKhoVatTuChiTiet.NhapKhoVatTu.SoPhieu,
        //        NgayNhap = DateTime.Now,

        //    };
        //    var newNhapKhoVatTuChiTiet = new NhapKhoVatTuChiTiet
        //    {
        //        NhapKhoVatTuId = nhapKhoVatTuChiTiet.NhapKhoVatTuId,
        //        VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
        //        HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
        //        LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
        //        TiLeBHYTThanhToan = nhapKhoVatTuChiTiet.TiLeBHYTThanhToan,
        //        NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
        //        Solo = nhapKhoVatTuChiTiet.Solo,
        //        HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
        //        DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
        //        TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
        //        VAT = nhapKhoVatTuChiTiet.VAT,
        //        DonGiaBan = nhapKhoVatTuChiTiet.DonGiaBan,
        //        MaVach = nhapKhoVatTuChiTiet.MaVach,
        //        SoLuongDaXuat = soLuongTra,
        //        NgayNhap = DateTime.Now
        //    };

        //    nhapKhoVatTu.NhapKhoVatTuChiTiets.Add(newNhapKhoVatTuChiTiet);
        //    return nhapKhoVatTu;
        //}

        private void CapNhatSoLuongNhapKhoVatTuTheoNhapKhoVatTuChiTietId(long nhapKhoVatTuChiTietId, double soLuongTra)
        {
            var nhapKhoChiTiet = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(cc => cc.Id == nhapKhoVatTuChiTietId).FirstOrDefault();

            if (nhapKhoChiTiet != null)
            {
                nhapKhoChiTiet.SoLuongDaXuat -= soLuongTra;

            }
            _nhapKhoVatTuChiTietRepository.Update(nhapKhoChiTiet);
        }

        private void XoaPhieu(YeuCauTraVatTu yeuCauTraVatTu)
        {
            var nhapKhoChiTiets = yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Select(cc => cc.XuatKhoVatTuChiTietViTri).FirstOrDefault();
            if (nhapKhoChiTiets != null)
            {
                _xuatKhoVatTuChiTietRepository.Delete(nhapKhoChiTiets.XuatKhoVatTuChiTiet);
                _xuatKhoVatTuChiTietViTriRepository.Delete(nhapKhoChiTiets);
            }
        }

        public string GetHtmlPhieuInHoanTraVatTu(long yeuCauHoanTraVatTuId, string hostingName)
        {
            var template = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTraVatTuTuTruc"));

            var hoanTraVatTuChiTiets = _yeuCauTraVatTuChiTietRepository.TableNoTracking
                                          .Where(p => p.YeuCauTraVatTuId == yeuCauHoanTraVatTuId)
                                          .Select(s => new PhieuHoanTraVatTuChiTietData
                                          {
                                              SoPhieu = s.YeuCauTraVatTu.SoPhieu,
                                              KhoTraLai = s.YeuCauTraVatTu.KhoXuat.Ten,
                                              KhoNhan = s.YeuCauTraVatTu.KhoNhap.Ten,
                                              Ten = s.VatTuBenhVien.VatTus.Ten,
                                              SoLo = s.Solo,
                                              NuocSX = s.VatTuBenhVien.VatTus.NuocSanXuat,
                                              HanSuDung = s.HanSuDung,
                                              DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                              SoLuong = s.SoLuongTra,
                                              DonGia = s.DonGiaNhap,
                                              CreatedOn = s.YeuCauTraVatTu.CreatedOn
                                          }).OrderBy(p => p.Ten).ToList();
            var thuocChiTiet = string.Empty;
            var STT = 1;

            foreach (var item in hoanTraVatTuChiTiets)
            {
                thuocChiTiet +=
                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenVatTu
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                           + "</tr>";
                STT++;
            }
            var tongThanhTien = hoanTraVatTuChiTiets.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
            thuocChiTiet += "<tr>"
                                + "<td colspan='2'><b>Tổng cộng: </b></td>"
                                + "<td>&nbsp;</td>"
                                + "<td>&nbsp;</td>"
                                + "<td>&nbsp;</td>"
                                + "<td style = 'text-align: right; padding-right:3px'>" + tongThanhTien + "</td>"
                                + "<td>&nbsp;</td>"
                           + "</tr>";
            var data = new PhieuHoanTraVatTuData
            {
                SoPhieu = hoanTraVatTuChiTiets.First().SoPhieu,
                KhoTraLai = hoanTraVatTuChiTiets.First().KhoTraLai,
                KhoNhan = hoanTraVatTuChiTiets.First().KhoNhan,
                BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraVatTuChiTiets.First().SoPhieu),
                NgayLapPhieu = hoanTraVatTuChiTiets.First().CreatedOn?.ApplyFormatDate(),
                ThuocVatTu = thuocChiTiet,
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString()
            };
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
    }

    #endregion
}
