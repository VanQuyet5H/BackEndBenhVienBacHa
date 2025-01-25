using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;

namespace Camino.Services.NhapKhoDuocPhams
{
    public partial class NhapKhoDuocPhamService
    {
        public async Task<GridDataSource> GetDanhSachDuyetHoanTraDuocPhamForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new HoanTraDuocPhamSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<HoanTraDuocPhamSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = DataYeuCauHoanTraDuocPham(null, queryInfo);
            var queryTuChoiDuyet = DataYeuCauHoanTraDuocPham(false, queryInfo);
            var queryDaDuyet = DataYeuCauHoanTraDuocPham(true, queryInfo);
            var query = _yeuCauTraDuocPhamRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachDuyetHoanTraDuocPhamVo()).AsQueryable();

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
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }


            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, p => p.SoPhieu, p => p.TenNhanVienYeuCau, p => p.TenKhoCanHoanTra, p => p.TenKhoNhanHoanTra, p => p.TenNhanVienDuyet);
                }

                if (queryObject.RangeYeuCau != null)
                {
                    if (queryObject.RangeYeuCau.startDate != null)
                    {
                        var tuNgay = queryObject.RangeYeuCau.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.NgayYeuCau.Date);
                    }

                    if (queryObject.RangeYeuCau.endDate != null)
                    {
                        var denNgay = queryObject.RangeYeuCau.endDate.GetValueOrDefault();
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
                        query = query.Where(p => denNgay.Date >= p.NgayDuyet.Value.Date);
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

        public async Task<GridDataSource> GetTotalDanhSachDuyetHoanTraDuocPhamForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new HoanTraDuocPhamSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<HoanTraDuocPhamSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = DataYeuCauHoanTraDuocPham(null, queryInfo);
            var queryTuChoiDuyet = DataYeuCauHoanTraDuocPham(false, queryInfo);
            var queryDaDuyet = DataYeuCauHoanTraDuocPham(true, queryInfo);
            var query = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachDuyetHoanTraDuocPhamVo()).AsQueryable();

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
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, p => p.SoPhieu, p => p.TenNhanVienYeuCau, p => p.TenKhoCanHoanTra, p => p.TenKhoNhanHoanTra, p => p.TenNhanVienDuyet);
                }

                if (queryObject.RangeYeuCau != null)
                {
                    if (queryObject.RangeYeuCau.startDate != null)
                    {
                        var tuNgay = queryObject.RangeYeuCau.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.NgayYeuCau.Date);
                    }

                    if (queryObject.RangeYeuCau.endDate != null)
                    {
                        var denNgay = queryObject.RangeYeuCau.endDate.GetValueOrDefault();
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
                        query = query.Where(p => denNgay.Date >= p.NgayDuyet.Value.Date);
                    }
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTraDuocPhamId = Convert.ToDouble(queryInfo.AdditionalSearchString);

            var query = _yeuCauTraDuocPhamChiTietRepository.TableNoTracking.Where(p => p.YeuCauTraDuocPhamId == yeuCauTraDuocPhamId)
                                                                           .Select(p => new DanhSachDuyetHoanTraDuocPhamChiTietVo
                                                                           {
                                                                               Id = p.Id,
                                                                               Nhom = p.DuocPhamBenhVienPhanNhom != null ? p.DuocPhamBenhVienPhanNhom.Ten : p.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten,
                                                                               Ma = p.DuocPhamBenhVien.Ma,
                                                                               DuocPham = p.DuocPhamBenhVien.DuocPham.Ten,
                                                                               DonViTinh = p.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                               LoaiBHYT = p.LaDuocPhamBHYT,
                                                                               SoLo = p.Solo,
                                                                               HanSuDung = p.HanSuDung,
                                                                               //HanSuDungDisplay = p.HanSuDung.ApplyFormatDate(),
                                                                               SoLuongHoanTra = p.SoLuongTra,
                                                                               //SoLuongHoanTraDisplay = p.SoLuongTra.ApplyNumber()
                                                                           });

            //var groupQuery = query.GroupBy(x => new { x.DuocPhamBenhVienId, x.LoaiBHYT, x.Ma, x.DuocPham, x.SoLo, x.HanSuDung, x.DonViTinh })
            //    .Select(g => new DanhSachDuyetHoanTraDuocPhamChiTietVo
            //    {
            //        DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
            //        DuocPham = g.First().DuocPham,
            //        DonViTinh = g.First().DonViTinh,
            //        LoaiBHYT = g.First().LoaiBHYT,
            //        Nhom = g.First().Nhom,
            //        Ma = g.First().Ma,
            //        SoLo = g.First().SoLo,
            //        HanSuDung = g.First().HanSuDung,
            //        SoLuongHoanTra = g.Sum(c => c.SoLuongHoanTra),
            //    });
            //var result = await groupQuery.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            //return new GridDataSource { Data = result, TotalRowCount = result.Count() };
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.DuocPham, g => g.SoLo);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTraDuocPhamId = Convert.ToDouble(queryInfo.AdditionalSearchString);

            var query = _yeuCauTraDuocPhamChiTietRepository.TableNoTracking.Where(p => p.YeuCauTraDuocPhamId == yeuCauTraDuocPhamId)
                                                                           .Select(p => new DanhSachDuyetHoanTraDuocPhamChiTietVo
                                                                           {
                                                                               Id = p.Id,
                                                                               Nhom = p.DuocPhamBenhVienPhanNhom != null ? p.DuocPhamBenhVienPhanNhom.Ten : p.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten,
                                                                               Ma = p.DuocPhamBenhVien.Ma,
                                                                               DuocPham = p.DuocPhamBenhVien.DuocPham.Ten,
                                                                               DonViTinh = p.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                               LoaiBHYT = p.LaDuocPhamBHYT,
                                                                               SoLo = p.Solo,
                                                                               HanSuDung = p.HanSuDung,
                                                                               //HanSuDungDisplay = p.HanSuDung.ApplyFormatDate(),
                                                                               SoLuongHoanTra = p.SoLuongTra,
                                                                               //SoLuongHoanTraDisplay = p.SoLuongTra.ApplyNumber()
                                                                           });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.DuocPham, g => g.SoLo);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        private IQueryable<DanhSachDuyetHoanTraDuocPhamVo> DataYeuCauHoanTraDuocPham(bool? duocKeToanDuyet, QueryInfo queryInfo)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = _yeuCauTraDuocPhamRepository.TableNoTracking.Where(p => p.DuocDuyet == duocKeToanDuyet)// && p.KhoNhap.KhoaPhongId == phongBenhVien.KhoaPhongId
                                                                    .Select(p => new DanhSachDuyetHoanTraDuocPhamVo
                                                                    {
                                                                        Id = p.Id,
                                                                        SoPhieu = p.SoPhieu,
                                                                        NhanVienYeuCauId = p.NhanVienYeuCauId,
                                                                        TenNhanVienYeuCau = p.NhanVienYeuCau.User.HoTen,
                                                                        KhoCanHoanTraId = p.KhoXuatId,
                                                                        TenKhoCanHoanTra = p.KhoXuat.Ten,
                                                                        KhoNhanHoanTraId = p.KhoNhapId,
                                                                        TenKhoNhanHoanTra = p.KhoNhap.Ten,
                                                                        NgayYeuCau = p.NgayYeuCau,
                                                                        NgayYeuCauDisplay = p.NgayYeuCau.ApplyFormatDateTimeSACH(),
                                                                        TinhTrang = p.DuocDuyet,
                                                                        NhanVienDuyetId = p.NhanVienDuyetId,
                                                                        TenNhanVienDuyet = p.NhanVienDuyet.User.HoTen,
                                                                        NgayDuyet = p.NgayDuyet,
                                                                        NgayDuyetDisplay = p.NgayDuyet != null ? p.NgayDuyet.Value.ApplyFormatDateTimeSACH() : string.Empty
                                                                    });

            var result = query.ApplyLike(queryInfo.SearchTerms, p => p.SoPhieu, p => p.TenNhanVienYeuCau, p => p.TenKhoCanHoanTra, p => p.TenKhoNhanHoanTra, p => p.TenNhanVienDuyet)
                              .OrderBy((queryInfo.SortString.Contains("asc") || queryInfo.SortString.Contains("desc")) ? queryInfo.SortString : "NgayYeuCau desc,SoPhieu asc");

            return result;
        }

        public async Task<ThongTinDuyetHoanTraDuocPham> GetThongTinDuyetHoanTraDuocPham(long yeuCauHoanTraDuocPhamId)
        {
            var yeuCauTraDuocPham = await _yeuCauTraDuocPhamRepository.TableNoTracking.Where(p => p.Id == yeuCauHoanTraDuocPhamId)
                                                                                      .Select(p => new ThongTinDuyetHoanTraDuocPham
                                                                                      {
                                                                                          SoPhieu = p.SoPhieu,
                                                                                          NhanVienYeuCauId = p.NhanVienYeuCauId,
                                                                                          TenNhanVienYeuCau = p.NhanVienYeuCau.User.HoTen,
                                                                                          KhoCanHoanTraId = p.KhoXuatId,
                                                                                          TenKhoCanHoanTra = p.KhoXuat.Ten,
                                                                                          KhoNhanHoanTraId = p.KhoNhapId,
                                                                                          TenKhoNhanHoanTra = p.KhoNhap.Ten,
                                                                                          NgayYeuCau = p.NgayYeuCau,
                                                                                          NgayYeuCauDisplay = p.NgayYeuCau.ApplyFormatDateTimeSACH(),
                                                                                          TinhTrang = p.DuocDuyet,
                                                                                          NhanVienDuyetId = p.NhanVienDuyetId,
                                                                                          TenNhanVienDuyet = p.NhanVienDuyet.User.HoTen,
                                                                                          NgayDuyet = p.NgayDuyet,
                                                                                          NgayDuyetDisplay = p.NgayDuyet != null ? p.NgayDuyet.Value.ApplyFormatDateTimeSACH() : string.Empty,
                                                                                          GhiChu = p.GhiChu,
                                                                                          //NhanVienTraId = p.YeuCauTraDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NguoiXuatId,
                                                                                          //TenNhanVienTra = p.YeuCauTraDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NguoiXuat.User.HoTen,
                                                                                          //NhanVienNhanId = p.YeuCauTraDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NguoiNhanId,
                                                                                          //TenNhanVienNhan = p.YeuCauTraDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NguoiNhan.User.HoTen,
                                                                                          LyDoHuy = p.LyDoKhongDuyet
                                                                                      })
                                                                                      .FirstOrDefaultAsync();

            var yeuCauTraDuocPhamChiTiet = await _yeuCauTraDuocPhamChiTietRepository.TableNoTracking.Where(p => p.YeuCauTraDuocPhamId == yeuCauHoanTraDuocPhamId)
                                                                                                    .FirstOrDefaultAsync();

            if (yeuCauTraDuocPhamChiTiet != null)
            {
                var xuatKhoDuocPham = await _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(p => p.Id == yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTriId)
                                                                                              .Select(p => p.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham)
                                                                                              .FirstOrDefaultAsync();

                if (xuatKhoDuocPham != null)
                {
                    yeuCauTraDuocPham.NhanVienTraId = xuatKhoDuocPham.NguoiXuatId;
                    yeuCauTraDuocPham.TenNhanVienTra = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoDuocPham.NguoiXuatId).Select(p => p.HoTen).FirstOrDefaultAsync();
                    yeuCauTraDuocPham.NhanVienNhanId = xuatKhoDuocPham.NguoiNhanId;
                    yeuCauTraDuocPham.TenNhanVienNhan = await _userRepository.TableNoTracking.Where(p => p.Id == xuatKhoDuocPham.NguoiNhanId).Select(p => p.HoTen).FirstOrDefaultAsync();
                }
            }

            return yeuCauTraDuocPham;
        }

        public async Task DuyetHoanTraDuocPham(long yeuCauTraDuocPhamId, long nhanVienTraId, long nhanVienNhanId, string tenNhanVienNhan, string tenNhanVienTra)
        {
            var yeuCauHoanTraDuocPham = _yeuCauTraDuocPhamRepository.Table
                .Where(p => p.Id == yeuCauTraDuocPhamId)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.XuatKhoDuocPhamChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraDuocPham != null && yeuCauHoanTraDuocPham.DuocDuyet == null)
            {
                //Yêu cầu hoàn trả dược phẩm
                yeuCauHoanTraDuocPham.DuocDuyet = true;
                yeuCauHoanTraDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraDuocPham.NgayDuyet = DateTime.Now;

                var xuatKhoDuocPham = new XuatKhoDuocPham
                {
                    KhoXuatId = yeuCauHoanTraDuocPham.KhoXuatId,
                    KhoNhapId = yeuCauHoanTraDuocPham.KhoNhapId,
                    LoaiXuatKho = Core.Domain.Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                    LyDoXuatKho = Core.Domain.Enums.XuatKhoDuocPham.XuatQuaKhoKhac.GetDescription(),
                    NguoiNhanId = nhanVienNhanId,
                    TenNguoiNhan = tenNhanVienNhan,
                    NguoiXuatId = nhanVienTraId,
                    LoaiNguoiNhan = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayXuat = DateTime.Now
                };

                //Xuất kho dược phẩm
                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = xuatKhoDuocPham;
                }

                //Nhập kho dược phẩm
                var newNhapKhoDuocPham = new NhapKhoDuocPham
                {
                    XuatKhoDuocPham = xuatKhoDuocPham,
                    TenNguoiGiao = tenNhanVienTra,
                    NguoiGiaoId = nhanVienTraId,
                    NguoiNhapId = nhanVienNhanId,
                    DaHet = false,
                    LoaiNguoiGiao = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayNhap = DateTime.Now,
                    KhoId = yeuCauHoanTraDuocPham.KhoNhapId,
                };

                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                    {
                        var newNhapKhoDuocPhamChiTiet = new NhapKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                            HopDongThauDuocPhamId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                            Solo = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Solo,
                            HanSuDung = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung,
                            SoLuongNhap = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat,
                            DonGiaNhap = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                            VAT = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.VAT,
                            MaVach = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.MaVach,
                            MaRef = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.MaRef,
                            SoLuongDaXuat = 0,
                            NgayNhap = DateTime.Now,
                            LaDuocPhamBHYT = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                            DuocPhamBenhVienPhanNhomId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId,
                            NgayNhapVaoBenhVien = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien,
                            PhuongPhapTinhGiaTriTonKho = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                            TiLeTheoThapGia = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                            KhoViTriId = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoViTriId,
                            TiLeBHYTThanhToan = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan
                        };
                        newNhapKhoDuocPham.NhapKhoDuocPhamChiTiets.Add(newNhapKhoDuocPhamChiTiet);
                    }
                }
                _nhapKhoDuocPhamRepository.Add(newNhapKhoDuocPham);
            }
        }

        //public async Task DuyetHoanTraDuocPhamOld(long yeuCauTraDuocPhamId, long nhanVienTraId, long nhanVienNhanId, string tenNhanVienNhan, string tenNhanVienTra)
        //{
        //    var yeuCauHoanTraDuocPham = await _yeuCauTraDuocPhamRepository.TableNoTracking.Where(p => p.Id == yeuCauTraDuocPhamId)
        //                                                                                  .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.NhapKhoDuocPhamChiTiet).ThenInclude(p => p.NhapKhoDuocPhams)
        //                                                                                  .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)
        //                                                                                  .FirstOrDefaultAsync();

        //    if (yeuCauHoanTraDuocPham != null)
        //    {
        //        //Yêu cầu hoàn trả dược phẩm
        //        yeuCauHoanTraDuocPham.DuocDuyet = true;
        //        yeuCauHoanTraDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
        //        yeuCauHoanTraDuocPham.NgayDuyet = DateTime.Now;

        //        _yeuCauTraDuocPhamRepository.Update(yeuCauHoanTraDuocPham);

        //        //Xuất kho dược phẩm
        //        var xuatKhoDuocPham = MapXuatKhoDuocPham(yeuCauHoanTraDuocPham, nhanVienTraId, nhanVienNhanId, tenNhanVienNhan);
        //        await _xuatKhoDuocPhamRepository.AddAsync(xuatKhoDuocPham);

        //        var xuatKhoDuocPhamChiTietViTris = yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets.Select(p => p.XuatKhoDuocPhamChiTietViTri).ToList();
        //        foreach (var item in xuatKhoDuocPhamChiTietViTris)
        //        {
        //            var xuatKhoDuocPhamChiTiet = await _xuatKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => p.Id == item.XuatKhoDuocPhamChiTietId)
        //                                                                                                .FirstOrDefaultAsync();

        //            xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId = xuatKhoDuocPham.Id;

        //            await _xuatKhoDuocPhamChiTietRepository.UpdateAsync(xuatKhoDuocPhamChiTiet);
        //        }

        //        //Nhập kho dược phẩm
        //        await MapNhapKhoDuocPham(yeuCauHoanTraDuocPham, nhanVienTraId, nhanVienNhanId, tenNhanVienTra, xuatKhoDuocPham.Id);
        //    }
        //}

        //private XuatKhoDuocPham MapXuatKhoDuocPham(YeuCauTraDuocPham yeuCauTraDuocPham, long nhanVienTraId, long nhanVienNhanId, string tenNhanVienNhan)
        //{
        //    var xuatKhoDuocPham = new XuatKhoDuocPham();

        //    xuatKhoDuocPham.KhoXuatId = yeuCauTraDuocPham.KhoXuatId;
        //    xuatKhoDuocPham.KhoNhapId = yeuCauTraDuocPham.KhoNhapId;

        //    xuatKhoDuocPham.LoaiXuatKho = Core.Domain.Enums.XuatKhoDuocPham.XuatQuaKhoKhac;
        //    xuatKhoDuocPham.LyDoXuatKho = Core.Domain.Enums.XuatKhoDuocPham.XuatQuaKhoKhac.GetDescription();

        //    xuatKhoDuocPham.NguoiNhanId = nhanVienNhanId;
        //    xuatKhoDuocPham.TenNguoiNhan = tenNhanVienNhan;

        //    xuatKhoDuocPham.NguoiXuatId = nhanVienTraId;
        //    xuatKhoDuocPham.LoaiNguoiNhan = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong;

        //    xuatKhoDuocPham.SoPhieu = yeuCauTraDuocPham.SoPhieu;
        //    xuatKhoDuocPham.NgayXuat = DateTime.Now;

        //    return xuatKhoDuocPham;
        //}

        //private async Task MapNhapKhoDuocPham(YeuCauTraDuocPham yeuCauTraDuocPham, long nhanVienTraId, long nhanVienNhanId, string tenNhanVienTra, long xuatKhoDuocPhamId)
        //{
        //    var oldNhapKhoDuocPham = yeuCauTraDuocPham.YeuCauTraDuocPhamChiTiets.Select(p => p.XuatKhoDuocPhamChiTietViTri).FirstOrDefault().NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams;

        //    var newNhapKhoDuocPham = new NhapKhoDuocPham
        //    {
        //        SoChungTu = oldNhapKhoDuocPham.SoChungTu,
        //        XuatKhoDuocPhamId = xuatKhoDuocPhamId,
        //        TenNguoiGiao = tenNhanVienTra,
        //        NguoiGiaoId = nhanVienTraId,
        //        NguoiNhapId = nhanVienNhanId,
        //        DaHet = false,
        //        LoaiNguoiGiao = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,
        //        NgayNhap = DateTime.Now,
        //        //YeuCauNhapKhoDuocPhamId = 0,
        //        KhoId = yeuCauTraDuocPham.KhoNhapId,
        //        //YeuCauLinhDuocPhamId = 0,
        //        SoPhieu = oldNhapKhoDuocPham.SoPhieu
        //    };

        //    await _nhapKhoDuocPhamRepository.AddAsync(newNhapKhoDuocPham);

        //    //var newNhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>();
        //    foreach (var yeucauTraDuocPhamChiTiet in yeuCauTraDuocPham.YeuCauTraDuocPhamChiTiets)
        //    {
        //        var newNhapKhoDuocPhamChiTiet = new NhapKhoDuocPhamChiTiet
        //        {
        //            NhapKhoDuocPhamId = newNhapKhoDuocPham.Id,
        //            DuocPhamBenhVienId = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
        //            HopDongThauDuocPhamId = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
        //            Solo = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Solo,
        //            HanSuDung = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung,
        //            SoLuongNhap = yeucauTraDuocPhamChiTiet.SoLuongTra,
        //            DonGiaNhap = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DonGiaNhap,
        //            VAT = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.VAT,
        //            MaVach = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.MaVach,
        //            MaRef = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.MaRef,
        //            SoLuongDaXuat = 0,
        //            NgayNhap = DateTime.Now,
        //            LaDuocPhamBHYT = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
        //            DuocPhamBenhVienPhanNhomId = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId,
        //            NgayNhapVaoBenhVien = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien,
        //            PhuongPhapTinhGiaTriTonKho = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
        //            TiLeTheoThapGia = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
        //            KhoViTriId = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoViTriId,
        //            DonGiaBan = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.DonGiaBan,
        //            TiLeBHYTThanhToan = yeucauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan
        //        };

        //        await _nhapKhoDuocPhamChiTietRepository.AddAsync(newNhapKhoDuocPhamChiTiet);
        //    }
        //}

        public async Task TuChoiDuyetHoanTraDuocPham(long yeuCauTraDuocPhamId, string lyDoKhongDuyet)
        {
            var yeuCauHoanTraDuocPham = _yeuCauTraDuocPhamRepository.Table
                .Where(p => p.Id == yeuCauTraDuocPhamId)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
                .Include(p => p.YeuCauTraDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.XuatKhoDuocPhamChiTiet)
                .FirstOrDefault();

            if (yeuCauHoanTraDuocPham != null && yeuCauHoanTraDuocPham.DuocDuyet == null)
            {
                //Cập nhật trạng thái yêu cầu trả dược phẩm

                yeuCauHoanTraDuocPham.DuocDuyet = false;
                yeuCauHoanTraDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauHoanTraDuocPham.NgayDuyet = DateTime.Now;
                yeuCauHoanTraDuocPham.LyDoKhongDuyet = lyDoKhongDuyet;

                //Sửa nhập kho                
                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                }
                //Xoá xuất kho chi tiết & xuất kho chi tiết vị trí
                foreach (var yeuCauTraDuocPhamChiTiet in yeuCauHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
                {
                    if (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri != null)
                    {
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                        yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                    }
                    yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTriId = null;
                }

                _yeuCauTraDuocPhamRepository.Context.SaveChanges();
            }
        }

        //public async Task TuChoiDuyetHoanTraDuocPhamOld(long yeuCauTraDuocPhamId, string lyDoKhongDuyet)
        //{
        //    var yeuCauTraDuocPhamChiTiets = await _yeuCauTraDuocPhamChiTietRepository.TableNoTracking.Where(p => p.YeuCauTraDuocPhamId == yeuCauTraDuocPhamId)
        //                                                                                             .Include(p => p.XuatKhoDuocPhamChiTietViTri).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
        //                                                                                             .ToListAsync();

        //    //Sửa nhập kho
        //    foreach (var item in yeuCauTraDuocPhamChiTiets)
        //    {
        //        item.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= item.SoLuongTra;
        //    }

        //    await _yeuCauTraDuocPhamChiTietRepository.UpdateAsync(yeuCauTraDuocPhamChiTiets);

        //    //Xoá xuất kho chi tiết & xuất kho chi tiết vị trí
        //    foreach (var item in yeuCauTraDuocPhamChiTiets)
        //    {

        //        long xuatKhoDuocPhamChiTietViTriId = item.XuatKhoDuocPhamChiTietViTri.Id;
        //        long xuatKhoDuocPhamChiTietId = item.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTietId;

        //        item.XuatKhoDuocPhamChiTietViTriId = null;

        //        await _xuatKhoDuocPhamChiTietViTriRepository.DeleteByIdAsync(xuatKhoDuocPhamChiTietViTriId);
        //        await _xuatKhoDuocPhamChiTietRepository.DeleteByIdAsync(xuatKhoDuocPhamChiTietId);
        //    }

        //    await _yeuCauTraDuocPhamChiTietRepository.UpdateAsync(yeuCauTraDuocPhamChiTiets);

        //    //Cập nhật trạng thái yêu cầu trả dược phẩm
        //    var yeuCauTraDuocPham = await _yeuCauTraDuocPhamRepository.TableNoTracking.Where(p => p.Id == yeuCauTraDuocPhamId).FirstOrDefaultAsync();

        //    yeuCauTraDuocPham.DuocDuyet = false;
        //    yeuCauTraDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
        //    yeuCauTraDuocPham.NgayDuyet = DateTime.Now;
        //    yeuCauTraDuocPham.LyDoKhongDuyet = lyDoKhongDuyet;

        //    await _yeuCauTraDuocPhamRepository.UpdateAsync(yeuCauTraDuocPham);
        //}

        public string GetHtmlPhieuInHoanTraDuocPham(long yeuCauHoanTraDuocPhamId, string hostingName)
        {
            var template = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTraDuocPhamTuTruc"));

            var templateGayNghien = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTraThuocGayNghienTuTruc"));
            var headerTitile = "<div style=\'width: 100%; height: 40px; background: #005dab;color:#fff;text-align: center;font-size: 23px\'> PHIẾU HOÀN TRẢ THUỐC</div>";

            var hoanTraDuocPhamChiTiets = _yeuCauTraDuocPhamChiTietRepository.TableNoTracking
                                          .Where(p => p.YeuCauTraDuocPhamId == yeuCauHoanTraDuocPhamId &&
                                          (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                                          .Select(s => new PhieuHoanTraDuocPhamChiTietData
                                          {
                                              SoPhieu = s.YeuCauTraDuocPham.SoPhieu,
                                              KhoTraLai = s.YeuCauTraDuocPham.KhoXuat.Ten,
                                              KhoNhan = s.YeuCauTraDuocPham.KhoNhap.Ten,
                                              Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                              HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                              SoLo = s.Solo,
                                              NuocSX = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                              HanSuDung = s.HanSuDung,
                                              DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                              SoLuong = s.SoLuongTra,
                                              DonGia = s.DonGiaNhap,
                                              CreatedOn = s.YeuCauTraDuocPham.CreatedOn
                                          }).OrderBy(p => p.Ten).ToList();

            var hoanTraDuocPhamChiTietsGayNghiens = _yeuCauTraDuocPhamChiTietRepository.TableNoTracking
                                                       .Where(p => p.YeuCauTraDuocPhamId == yeuCauHoanTraDuocPhamId &&
                                                       (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                                                       .Select(s => new PhieuHoanTraDuocPhamChiTietData
                                                       {
                                                           SoPhieu = s.YeuCauTraDuocPham.SoPhieu,
                                                           KhoTraLai = s.YeuCauTraDuocPham.KhoXuat.Ten,
                                                           KhoNhan = s.YeuCauTraDuocPham.KhoNhap.Ten,
                                                           Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                                           HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                                           SoLo = s.Solo,
                                                           NuocSX = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                                           HanSuDung = s.HanSuDung,
                                                           DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                           SoLuong = s.SoLuongTra,
                                                           DonGia = s.DonGiaNhap,
                                                           CreatedOn = s.YeuCauTraDuocPham.CreatedOn
                                                       }).OrderBy(p => p.Ten).ToList();
            var content = string.Empty;
            var contentGayNghien = string.Empty;
            if (hoanTraDuocPhamChiTiets.Any())
            {
                var thuocChiTiet = string.Empty;
                var STT = 1;

                foreach (var item in hoanTraDuocPhamChiTiets)
                {
                    thuocChiTiet +=
                                               "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenThuoc
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                               + "</tr>";
                    STT++;
                }
                var tongThanhTien = hoanTraDuocPhamChiTiets.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                thuocChiTiet += "<tr>"
                                    + "<td colspan='2'><b>Tổng cộng: </b></td>"
                                    + "<td>&nbsp;</td>"
                                    + "<td>&nbsp;</td>"
                                    + "<td>&nbsp;</td>"
                                    + "<td style = 'text-align: right; padding-right:3px'>" + tongThanhTien + "</td>"
                                    + "<td>&nbsp;</td>"
                               + "</tr>";
                var data = new PhieuHoanTraDuocPhamData
                {
                    SoPhieu = hoanTraDuocPhamChiTiets.First().SoPhieu,
                    KhoTraLai = hoanTraDuocPhamChiTiets.First().KhoTraLai,
                    KhoNhan = hoanTraDuocPhamChiTiets.First().KhoNhan,
                    BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTiets.First().SoPhieu),
                    NgayLapPhieu = hoanTraDuocPhamChiTiets.First().CreatedOn?.ApplyFormatDate(),
                    ThuocVatTu = thuocChiTiet,
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    //Ngay = DateTime.Now.Day.ConvertDateToString(),
                    //Thang = DateTime.Now.Month.ConvertMonthToString(),
                    //Nam = DateTime.Now.Year.ConvertYearToString()
                    NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            }
            if (hoanTraDuocPhamChiTietsGayNghiens.Any())
            {
                var thuocChiTiet = string.Empty;
                var STT = 1;

                foreach (var item in hoanTraDuocPhamChiTietsGayNghiens)
                {
                    thuocChiTiet +=
                                               "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenThuoc
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                               + "</tr>";
                    STT++;
                }
                var tongThanhTien = hoanTraDuocPhamChiTietsGayNghiens.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                thuocChiTiet += "<tr>"
                                    + "<td colspan='2'><b>Tổng cộng: </b></td>"
                                    + "<td>&nbsp;</td>"
                                    + "<td>&nbsp;</td>"
                                    + "<td>&nbsp;</td>"
                                    + "<td style = 'text-align: right; padding-right:3px'>" + tongThanhTien + "</td>"
                                    + "<td>&nbsp;</td>"
                               + "</tr>";
                var data = new PhieuHoanTraDuocPhamData
                {
                    SoPhieu = hoanTraDuocPhamChiTietsGayNghiens.First().SoPhieu,
                    KhoTraLai = hoanTraDuocPhamChiTietsGayNghiens.First().KhoTraLai,
                    KhoNhan = hoanTraDuocPhamChiTietsGayNghiens.First().KhoNhan,
                    BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTietsGayNghiens.First().SoPhieu),
                    NgayLapPhieu = hoanTraDuocPhamChiTietsGayNghiens.First().CreatedOn?.ApplyFormatDate(),
                    ThuocVatTu = thuocChiTiet,
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()

                };
                contentGayNghien = TemplateHelpper.FormatTemplateWithContentTemplate(templateGayNghien.Body, data);
            }
            if (content != "")
            {
                content = headerTitile + content + "<div class=\"pagebreak\"> </div>";
            }
            if (contentGayNghien != "")
            {
                content = content + headerTitile + contentGayNghien;
            }
            return content;
        }

        public string InPhieuHoanTraDuocPhamVatTu(PhieuHoanTraDuocPhamVatTu phieuHoanTraDuocPhamVatTu)
        {
            var content = "";
            var template = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTra"));
            var khoaId = _phongBenhVienRepository.GetById(_userAgentHelper.GetCurrentNoiLLamViecId())?.KhoaPhongId;
            var khoaPhong = _khoaPhongRepository.TableNoTracking.Where(z => z.Id == khoaId).Select(z => z.Ten).FirstOrDefault();
            if (phieuHoanTraDuocPhamVatTu.LaTuTruc) // Trả dp/vt từ tủ trực
            {
                if (phieuHoanTraDuocPhamVatTu.LaDuocPham)
                {
                    var hoanTraDuocPhamChiTiets = _yeuCauTraDuocPhamChiTietRepository.TableNoTracking
                                           .Where(p => p.YeuCauTraDuocPhamId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                           .Select(s => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData
                                           {
                                               SoPhieu = s.YeuCauTraDuocPham.SoPhieu,
                                               Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                               DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                               SoLo = s.Solo,
                                               HanSuDung = s.HanSuDung,
                                               SoLuong = s.SoLuongTra,
                                               DonGia = s.DonGiaNhap,
                                               //GhiChu = s.YeuCauTraDuocPham.GhiChu
                                           }).ToList();

                    var hoanTraDuocPhamChiTietGroup = hoanTraDuocPhamChiTiets
                                                    .GroupBy(x => new { x.Ten, x.DVT, x.SoLoHSD, x.DonGia })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();

                    if (hoanTraDuocPhamChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        foreach (var item in hoanTraDuocPhamChiTietGroup)
                        {
                            thuocChiTiet +=
                                                       "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                       + "<td style = 'border: 1px solid #020000;'>" + item.SoLoHSD //Số kiểm soát
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong.ApplyNumber()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                                       + "</tr>";
                            STT++;
                        }
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraDuocPhamChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = khoaPhong,
                            //LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                            NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
                else
                {
                    var hoanTraVatTuChiTiets = _yeuCauTraVatTuChiTietRepository.TableNoTracking
                                              .Where(p => p.YeuCauTraVatTuId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                              .Select(s => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData
                                              {
                                                  SoPhieu = s.YeuCauTraVatTu.SoPhieu,
                                                  Ten = s.VatTuBenhVien.VatTus.Ten,
                                                  DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                  SoLo = s.Solo,
                                                  HanSuDung = s.HanSuDung,
                                                  SoLuong = s.SoLuongTra,
                                                  DonGia = s.DonGiaNhap,
                                                  //GhiChu = s.YeuCauTraDuocPham.GhiChu
                                              }).ToList();

                    var hoanTraVatTuChiTietGroup = hoanTraVatTuChiTiets
                                                    .GroupBy(x => new { x.Ten, x.DVT, x.SoLoHSD, x.DonGia })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();
                    if (hoanTraVatTuChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        foreach (var item in hoanTraVatTuChiTietGroup)
                        {
                            thuocChiTiet +=
                                                       "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                       + "<td style = 'border: 1px solid #020000;'>" + item.SoLoHSD //Số kiểm soát
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong.ApplyNumber()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                                       + "</tr>";
                            STT++;
                        }
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraVatTuChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraVatTuChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = khoaPhong,
                            NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
            }
            else // Trả dp/vt từ bn
            {
                var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
                if (phieuHoanTraDuocPhamVatTu.LaDuocPham)
                {
                    var hoanTraDuocPhamChiTiets = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                                         .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                         .Select(s => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData
                                         {
                                             SoPhieu = s.YeuCauTraDuocPhamTuBenhNhan.SoPhieu,
                                             Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                             SoLo = s.YeuCauDuocPhamBenhVien.SoHopDongThau,
                                             HanSuDung = s.YeuCauDuocPhamBenhVien.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                                   .Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoTraId
                                                       && nkct.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                       && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT)
                                                  .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                               .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                               .Select(o => o.HanSuDung).FirstOrDefault(),
                                             DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                             SoLuong = s.SoLuongTra,
                                             DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                                             KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi,
                                             KhoaPhong = s.YeuCauTraDuocPhamTuBenhNhan.KhoaHoanTra.Ten
                                         }).OrderBy(p => p.Ten).ToList();

                    var hoanTraDuocPhamChiTietGroup = hoanTraDuocPhamChiTiets
                                                    .GroupBy(x => new { x.Ten, x.DVT, x.SoLoHSD, x.DonGia, x.KhongTinhPhi, x.KhoaPhong })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        KhoaPhong = item.First().KhoaPhong,
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();

                    if (hoanTraDuocPhamChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        foreach (var item in hoanTraDuocPhamChiTietGroup)
                        {
                            thuocChiTiet +=
                                                       "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                       + "<td style = 'border: 1px solid #020000;'>" + item.SoLoHSD //Số kiểm soát
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong.ApplyNumber()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                                       + "</tr>";
                            STT++;
                        }
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraDuocPhamChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = hoanTraDuocPhamChiTietGroup.First().KhoaPhong,
                            NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
                else
                {
                    var hoanTraVatTuChiTiets = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                                             .Where(p => p.YeuCauTraVatTuTuBenhNhanId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                             .Select(s => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData
                                             {
                                                 SoPhieu = s.YeuCauTraVatTuTuBenhNhan.SoPhieu,
                                                 Ten = s.VatTuBenhVien.VatTus.Ten,
                                                 SoLo = s.YeuCauVatTuBenhVien.SoHopDongThau,
                                                 HanSuDung = s.YeuCauVatTuBenhVien.VatTuBenhVien.NhapKhoVatTuChiTiets
                                                       .Where(nkct => nkct.NhapKhoVatTu.KhoId == s.KhoTraId
                                                           && nkct.VatTuBenhVienId == s.VatTuBenhVienId
                                                           && nkct.LaVatTuBHYT == s.LaVatTuBHYT)
                                                      .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                                   .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                   .Select(o => o.HanSuDung).FirstOrDefault(),
                                                 DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                 SoLuong = s.SoLuongTra,
                                                 DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                                                 KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi,
                                                 KhoaPhong = s.YeuCauTraVatTuTuBenhNhan.KhoaHoanTra.Ten
                                             }).OrderBy(p => p.Ten).ToList();
                    var hoanTraVatTuChiTietGroup = hoanTraVatTuChiTiets
                                                    .GroupBy(x => new { x.Ten, x.DVT, x.SoLoHSD, x.DonGia, x.KhongTinhPhi, x.KhoaPhong })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        KhoaPhong = item.First().KhoaPhong,
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();
                    if (hoanTraVatTuChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        foreach (var item in hoanTraVatTuChiTietGroup)
                        {
                            thuocChiTiet +=
                                                       "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                       + "<td style = 'border: 1px solid #020000;'>" + item.SoLoHSD //Số kiểm soát
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong.ApplyNumber()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                                       + "</tr>";
                            STT++;
                        }
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraVatTuChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraVatTuChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = hoanTraVatTuChiTietGroup.First().KhoaPhong,
                            NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
            }

            return content;
        }


        public string InPhieuHoanTraDuocPhamVatTuUpdate(PhieuHoanTraDuocPhamVatTu phieuHoanTraDuocPhamVatTu)
        {
            var content = "";
            var template = phieuHoanTraDuocPhamVatTu.DuocDuyet ? _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTraUpdateDuyet")) : _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTraUpdate"));
            var khoaId = _phongBenhVienRepository.GetById(_userAgentHelper.GetCurrentNoiLLamViecId())?.KhoaPhongId;
            var khoaPhong = _khoaPhongRepository.TableNoTracking.Where(z => z.Id == khoaId).Select(z => z.Ten).FirstOrDefault();
            if (phieuHoanTraDuocPhamVatTu.LaTuTruc) // Trả dp/vt từ tủ trực
            {
                if (phieuHoanTraDuocPhamVatTu.LaDuocPham)
                {
                    var hoanTraDuocPhamChiTiets = _yeuCauTraDuocPhamChiTietRepository.TableNoTracking
                                           .Where(p => p.YeuCauTraDuocPhamId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                           .Select(s => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData
                                           {
                                               SoPhieu = s.YeuCauTraDuocPham.SoPhieu,
                                               Ma = s.DuocPhamBenhVien.MaDuocPhamBenhVien,
                                               Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                               DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                               SoLo = s.Solo,
                                               HanSuDung = s.HanSuDung,
                                               SoLuong = s.SoLuongTra,
                                               DonGia = s.DonGiaNhap,
                                               //HoTenBenhNhan= s.YeuCauTraDuocPham.
                                               KhoTraLai = s.YeuCauTraDuocPham.KhoXuat.Ten,
                                               KhoNhan = s.YeuCauTraDuocPham.KhoNhap.Ten,
                                               GhiChu = s.YeuCauTraDuocPham.GhiChu,
                                               CreatedOn = s.YeuCauTraDuocPham.CreatedOn,
                                               DuocDuyet = s.YeuCauTraDuocPham.DuocDuyet,
                                               NgayDuyet = s.YeuCauTraDuocPham.NgayDuyet,
                                               NgayYeuCau = s.YeuCauTraDuocPham.NgayYeuCau
                                           }).ToList();

                    var hoanTraDuocPhamChiTietGroup = hoanTraDuocPhamChiTiets
                                                    .GroupBy(x => new { x.Ma, x.Ten, x.DVT, x.SoLoHSD, x.DonGia })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        KhoTraLai = item.First().KhoTraLai,
                                                        KhoNhan = item.First().KhoNhan,
                                                        GhiChu = item.First().GhiChu,
                                                        Ma = item.First().Ma,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        CreatedOn = item.First().CreatedOn,
                                                        NgayDuyet = item.First().NgayDuyet,
                                                        DuocDuyet = item.First().DuocDuyet,
                                                        NgayYeuCau = item.First().NgayYeuCau,
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();

                    if (hoanTraDuocPhamChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        if (phieuHoanTraDuocPhamVatTu.DuocDuyet)
                        {
                            foreach (var item in hoanTraDuocPhamChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ma
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung.ApplyFormatDate()
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "</tr>";
                                STT++;
                            }
                        }
                        else
                        {
                            foreach (var item in hoanTraDuocPhamChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung.ApplyFormatDate()
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + string.Empty
                                                           + "</tr>";
                                STT++;
                            }
                        }

                        var tongThanhTien = hoanTraDuocPhamChiTietGroup.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraDuocPhamChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = khoaPhong,
                            CongKhoan = STT - 1,
                            TongCong = tongThanhTien,
                            KhoTraLai = hoanTraDuocPhamChiTietGroup.First().KhoTraLai,
                            KhoNhan = hoanTraDuocPhamChiTietGroup.First().KhoNhan,
                            DienGiai = "",
                            GhiChu = hoanTraDuocPhamChiTietGroup.First().GhiChu,
                            NgayThangNam = hoanTraDuocPhamChiTietGroup.First().DuocDuyet == true ? hoanTraDuocPhamChiTietGroup.First().NgayDuyet?.ApplyFormatNgayThangNam() : hoanTraDuocPhamChiTietGroup.First().CreatedOn?.ApplyFormatNgayThangNam(),
                            NgayLapPhieu = hoanTraDuocPhamChiTietGroup.First().DuocDuyet == true ? hoanTraDuocPhamChiTietGroup.First().NgayDuyet?.ApplyFormatDate() : hoanTraDuocPhamChiTietGroup.First().NgayYeuCau.ApplyFormatDate()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
                else
                {
                    var hoanTraVatTuChiTiets = _yeuCauTraVatTuChiTietRepository.TableNoTracking
                                              .Where(p => p.YeuCauTraVatTuId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                              .Select(s => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData
                                              {
                                                  SoPhieu = s.YeuCauTraVatTu.SoPhieu,
                                                  Ma = s.VatTuBenhVien.MaVatTuBenhVien,
                                                  Ten = s.VatTuBenhVien.VatTus.Ten,
                                                  DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                  SoLo = s.Solo,
                                                  HanSuDung = s.HanSuDung,
                                                  SoLuong = s.SoLuongTra,
                                                  DonGia = s.DonGiaNhap,
                                                  KhoTraLai = s.YeuCauTraVatTu.KhoXuat.Ten,
                                                  KhoNhan = s.YeuCauTraVatTu.KhoNhap.Ten,
                                                  GhiChu = s.YeuCauTraVatTu.GhiChu,
                                                  CreatedOn = s.YeuCauTraVatTu.CreatedOn,
                                                  DuocDuyet = s.YeuCauTraVatTu.DuocDuyet,
                                                  NgayDuyet = s.YeuCauTraVatTu.NgayDuyet,
                                                  NgayYeuCau = s.YeuCauTraVatTu.NgayYeuCau

                                              }).ToList();

                    var hoanTraVatTuChiTietGroup = hoanTraVatTuChiTiets
                                                    .GroupBy(x => new { x.Ma, x.Ten, x.DVT, x.SoLoHSD, x.DonGia })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuTuTrucChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        Ma = item.First().Ma,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        KhoTraLai = item.First().KhoTraLai,
                                                        KhoNhan = item.First().KhoNhan,
                                                        GhiChu = item.First().GhiChu,
                                                        CreatedOn = item.First().CreatedOn,
                                                        DuocDuyet = item.First().DuocDuyet,
                                                        NgayDuyet = item.First().NgayDuyet,
                                                        NgayYeuCau = item.First().NgayYeuCau
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();
                    if (hoanTraVatTuChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        if (phieuHoanTraDuocPhamVatTu.DuocDuyet)
                        {
                            foreach (var item in hoanTraVatTuChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align:center'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ma
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung.ApplyFormatDate() //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "</tr>";
                                STT++;
                            }
                        }
                        else
                        {
                            foreach (var item in hoanTraVatTuChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align:center'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung.ApplyFormatDate() //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + string.Empty
                                                           + "</tr>";
                                STT++;
                            }
                        }

                        var tongThanhTien = hoanTraVatTuChiTietGroup.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraVatTuChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraVatTuChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = khoaPhong,
                            KhoTraLai = hoanTraVatTuChiTietGroup.First().KhoTraLai,
                            KhoNhan = hoanTraVatTuChiTietGroup.First().KhoNhan,
                            DienGiai = "",
                            GhiChu = hoanTraVatTuChiTietGroup.First().GhiChu,
                            CongKhoan = STT - 1,
                            TongCong = tongThanhTien,
                            NgayThangNam = hoanTraVatTuChiTietGroup.First().DuocDuyet == true ? hoanTraVatTuChiTietGroup.First().NgayDuyet?.ApplyFormatNgayThangNam() : hoanTraVatTuChiTietGroup.First().CreatedOn?.ApplyFormatNgayThangNam(),
                            NgayLapPhieu = hoanTraVatTuChiTietGroup.First().DuocDuyet == true ? hoanTraVatTuChiTietGroup.First().NgayDuyet?.ApplyFormatDate() : hoanTraVatTuChiTietGroup.First().NgayYeuCau.ApplyFormatDate()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
            }
            else // Trả dp/vt từ bn
            {
                var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
                if (phieuHoanTraDuocPhamVatTu.LaDuocPham)
                {
                    var ycTraDuocPhamBN = _yeuCauTraDuocPhamTuBenhNhanRepository.GetById(phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId);
                    var hoanTraDuocPhamChiTiets = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                                         .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                         .Select(s => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData
                                         {
                                             SoPhieu = s.YeuCauTraDuocPhamTuBenhNhan.SoPhieu,
                                             Ma = s.DuocPhamBenhVien.MaDuocPhamBenhVien,
                                             Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                             //SoLo = s.YeuCauDuocPhamBenhVien.SoHopDongThau,
                                             //SoLo = s.YeuCauDuocPhamBenhVien.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                             //      .Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoTraId
                                             //          && nkct.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                             //          && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                             //          && nkct.HanSuDung >= DateTime.Now)
                                             //     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                             //                  .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                             //                  .Select(o => o.Solo).FirstOrDefault(),
                                             //HanSuDung = s.YeuCauDuocPhamBenhVien.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                             //      .Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoTraId
                                             //          && nkct.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                             //          && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                             //          && nkct.HanSuDung >= DateTime.Now)
                                             //     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                             //                  .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                             //                  .Select(o => o.HanSuDung).FirstOrDefault(),
                                             SoLo = s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet != null && s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.FirstOrDefault()!=null && s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.FirstOrDefault().NhapKhoDuocPhamChiTiet!=null ? s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.FirstOrDefault().NhapKhoDuocPhamChiTiet.Solo : "",
                                             HanSuDung = s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet != null && s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.FirstOrDefault() != null && s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.FirstOrDefault().NhapKhoDuocPhamChiTiet != null ? s.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.FirstOrDefault().NhapKhoDuocPhamChiTiet.HanSuDung : (DateTime?)null,
                                             DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                             SoLuong = s.SoLuongTra,
                                             DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                                             KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi,
                                             KhoaPhong = s.YeuCauTraDuocPhamTuBenhNhan.KhoaHoanTra.Ten,
                                             KhoNhan = s.YeuCauTraDuocPhamTuBenhNhan.KhoTra.Ten,
                                             GhiChu = s.YeuCauTraDuocPhamTuBenhNhan.GhiChu,
                                             CreatedOn = s.YeuCauTraDuocPhamTuBenhNhan.CreatedOn,
                                             NgayDuyet = s.YeuCauTraDuocPhamTuBenhNhan.NgayDuyet,
                                             DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet,
                                             NgayYeuCau = s.YeuCauTraDuocPhamTuBenhNhan.NgayYeuCau,
                                             HoTenBenhNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen
                                             + (!string.IsNullOrEmpty(DateHelper.DOBFormat(s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.NgaySinh, s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.ThangSinh, s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.NamSinh)) ? " (" + DateHelper.DOBFormat(s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.NgaySinh, s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.ThangSinh, s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.NamSinh) + ")" : "")
                                         }).OrderBy(p => p.Ten).ToList();

                    var hoanTraDuocPhamChiTietGroup = hoanTraDuocPhamChiTiets
                                                    .GroupBy(x => new { x.Ma, x.Ten, x.DVT, x.SoLoHSD, x.DonGia, x.KhongTinhPhi, x.KhoaPhong })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        Ma = item.First().Ma,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        KhoaPhong = item.First().KhoaPhong,
                                                        KhoNhan = item.First().KhoNhan,
                                                        GhiChu = item.First().GhiChu,
                                                        CreatedOn = item.First().CreatedOn,
                                                        NgayDuyet = item.First().NgayDuyet,
                                                        DuocDuyet = item.First().DuocDuyet,
                                                        NgayYeuCau = item.First().NgayYeuCau,
                                                        HoTenBenhNhan = item.First().HoTenBenhNhan,
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();

                    if (hoanTraDuocPhamChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        if (phieuHoanTraDuocPhamVatTu.DuocDuyet)
                        {
                            foreach (var item in hoanTraDuocPhamChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ma
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung?.ApplyFormatDate() //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "</tr>";
                                STT++;
                            }
                        }
                        else
                        {
                            foreach (var item in hoanTraDuocPhamChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung?.ApplyFormatDate() //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + string.Empty
                                                           + "</tr>";
                                STT++;
                            }
                        }

                        var tongThanhTien = hoanTraDuocPhamChiTietGroup.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                        var dienGiai = string.Join("; ", hoanTraDuocPhamChiTietGroup.Select(c => c.HoTenBenhNhan).Distinct());
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraDuocPhamChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = hoanTraDuocPhamChiTietGroup.First().KhoaPhong,
                            KhoTraLai = hoanTraDuocPhamChiTietGroup.First().KhoaPhong,
                            KhoNhan = hoanTraDuocPhamChiTietGroup.First().KhoNhan,
                            DienGiai = dienGiai,
                            GhiChu = hoanTraDuocPhamChiTietGroup.First().GhiChu,
                            CongKhoan = STT - 1,
                            TongCong = tongThanhTien,
                            NgayThangNam = hoanTraDuocPhamChiTietGroup.First().DuocDuyet == true ? hoanTraDuocPhamChiTietGroup.First().NgayDuyet?.ApplyFormatNgayThangNam() : hoanTraDuocPhamChiTietGroup.First().CreatedOn?.ApplyFormatNgayThangNam(),
                            NgayLapPhieu = hoanTraDuocPhamChiTietGroup.First().DuocDuyet == true ? hoanTraDuocPhamChiTietGroup.First().NgayDuyet?.ApplyFormatDate() : ycTraDuocPhamBN.ThoiDiemHoanTraTongHopTuNgay?.ApplyFormatDate() + " - " + ycTraDuocPhamBN.ThoiDiemHoanTraTongHopDenNgay?.ApplyFormatDate()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
                else
                {
                    var ycTraVatTuBN = _yeuCauTraVatTuTuBenhNhanRepository.GetById(phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId);
                    var hoanTraVatTuChiTiets = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                                             .Where(p => p.YeuCauTraVatTuTuBenhNhanId == phieuHoanTraDuocPhamVatTu.YeuCauHoanTraDuocPhamVatTuId)
                                             .Select(s => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData
                                             {
                                                 SoPhieu = s.YeuCauTraVatTuTuBenhNhan.SoPhieu,
                                                 Ma = s.VatTuBenhVien.MaVatTuBenhVien,
                                                 Ten = s.VatTuBenhVien.VatTus.Ten,
                                                 //SoLo = s.YeuCauVatTuBenhVien.VatTuBenhVien.NhapKhoVatTuChiTiets
                                                 //      .Where(nkct => nkct.NhapKhoVatTu.KhoId == s.KhoTraId
                                                 //          && nkct.VatTuBenhVienId == s.VatTuBenhVienId
                                                 //          && nkct.LaVatTuBHYT == s.LaVatTuBHYT
                                                 //          && nkct.HanSuDung >= DateTime.Now)
                                                 //     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                 //                  .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                 //                  .Select(o => o.Solo).FirstOrDefault(),
                                                 //HanSuDung = s.YeuCauVatTuBenhVien.VatTuBenhVien.NhapKhoVatTuChiTiets
                                                 //      .Where(nkct => nkct.NhapKhoVatTu.KhoId == s.KhoTraId
                                                 //          && nkct.VatTuBenhVienId == s.VatTuBenhVienId
                                                 //          && nkct.LaVatTuBHYT == s.LaVatTuBHYT
                                                 //          && nkct.HanSuDung >= DateTime.Now)
                                                 //     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                 //                  .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                 //                  .Select(o => o.HanSuDung).FirstOrDefault(),
                                                 SoLo = s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet != null && s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.FirstOrDefault()!=null && s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.FirstOrDefault().NhapKhoVatTuChiTiet!=null ? s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.FirstOrDefault().NhapKhoVatTuChiTiet.Solo : "",
                                                 HanSuDung = s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet != null && s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.FirstOrDefault() != null && s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.FirstOrDefault().NhapKhoVatTuChiTiet != null ? s.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.FirstOrDefault().NhapKhoVatTuChiTiet.HanSuDung : (DateTime?)null,
                                                 DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                 SoLuong = s.SoLuongTra,
                                                 DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                                                 KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi,
                                                 KhoaPhong = s.YeuCauTraVatTuTuBenhNhan.KhoaHoanTra.Ten,
                                                 KhoNhan = s.YeuCauTraVatTuTuBenhNhan.KhoTra.Ten,
                                                 GhiChu = s.YeuCauTraVatTuTuBenhNhan.GhiChu,
                                                 CreatedOn = s.YeuCauTraVatTuTuBenhNhan.CreatedOn,
                                                 DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet,
                                                 NgayDuyet = s.YeuCauTraVatTuTuBenhNhan.NgayDuyet,
                                                 NgayYeuCau = s.YeuCauTraVatTuTuBenhNhan.NgayYeuCau,
                                                 HoTenBenhNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen
                                             + (!string.IsNullOrEmpty(DateHelper.DOBFormat(s.YeuCauVatTuBenhVien.YeuCauTiepNhan.NgaySinh, s.YeuCauVatTuBenhVien.YeuCauTiepNhan.ThangSinh, s.YeuCauVatTuBenhVien.YeuCauTiepNhan.NamSinh)) ? " (" + DateHelper.DOBFormat(s.YeuCauVatTuBenhVien.YeuCauTiepNhan.NgaySinh, s.YeuCauVatTuBenhVien.YeuCauTiepNhan.ThangSinh, s.YeuCauVatTuBenhVien.YeuCauTiepNhan.NamSinh) + ")" : "")
                                             }).OrderBy(p => p.Ten).ToList();
                    var hoanTraVatTuChiTietGroup = hoanTraVatTuChiTiets
                                                    .GroupBy(x => new { x.Ma, x.Ten, x.DVT, x.SoLoHSD, x.DonGia, x.KhongTinhPhi, x.KhoaPhong })
                                                    .Select(item => new PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData()
                                                    {
                                                        SoPhieu = item.First().SoPhieu,
                                                        Ma = item.First().Ma,
                                                        Ten = item.First().Ten,
                                                        DVT = item.First().DVT,
                                                        SoLo = item.First().SoLo,
                                                        HanSuDung = item.First().HanSuDung,
                                                        SoLuong = item.Sum(z => z.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        KhoaPhong = item.First().KhoaPhong,
                                                        KhoNhan = item.First().KhoNhan,
                                                        GhiChu = item.First().GhiChu,
                                                        CreatedOn = item.First().CreatedOn,
                                                        DuocDuyet = item.First().DuocDuyet,
                                                        NgayDuyet = item.First().NgayDuyet,
                                                        NgayYeuCau = item.First().NgayYeuCau,
                                                        HoTenBenhNhan = item.First().HoTenBenhNhan,
                                                    })
                                                    .OrderBy(x => x.Ten).ToList();
                    if (hoanTraVatTuChiTietGroup.Any())
                    {
                        var thuocChiTiet = string.Empty;
                        var STT = 1;
                        if (phieuHoanTraDuocPhamVatTu.DuocDuyet)
                        {
                            foreach (var item in hoanTraVatTuChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ma
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung?.ApplyFormatDate() //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "</tr>";
                                STT++;
                            }
                        }
                        else
                        {
                            foreach (var item in hoanTraVatTuChiTietGroup)
                            {
                                thuocChiTiet +=
                                                           "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.SoLo //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;'>" + item.HanSuDung?.ApplyFormatDate() //Số kiểm soát
                                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + item.SoLuong.ApplyNumber()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND()
                                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + string.Empty
                                                           + "</tr>";
                                STT++;
                            }
                        }

                        var tongThanhTien = hoanTraVatTuChiTietGroup.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                        var dienGiai = string.Join("; ", hoanTraVatTuChiTietGroup.Select(c => c.HoTenBenhNhan).Distinct());
                        var data = new PhieuHoanTraDuocPhamVatTuData
                        {
                            SoPhieu = hoanTraVatTuChiTietGroup.First().SoPhieu,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraVatTuChiTietGroup.First().SoPhieu),
                            ThuocVatTu = thuocChiTiet,
                            KhoaPhong = hoanTraVatTuChiTietGroup.First().KhoaPhong,
                            KhoTraLai = hoanTraVatTuChiTietGroup.First().KhoaPhong,
                            KhoNhan = hoanTraVatTuChiTietGroup.First().KhoNhan,
                            DienGiai = dienGiai,
                            GhiChu = hoanTraVatTuChiTietGroup.First().GhiChu,
                            CongKhoan = STT - 1,
                            TongCong = tongThanhTien,
                            //NgayThangNam = hoanTraVatTuChiTietGroup.First().CreatedOn?.ApplyFormatNgayThangNam(),
                            //NgayLapPhieu = ycTraVatTuBN.ThoiDiemHoanTraTongHopTuNgay?.ApplyFormatDate() + " - " + ycTraVatTuBN.ThoiDiemHoanTraTongHopDenNgay?.ApplyFormatDate()
                            NgayThangNam = hoanTraVatTuChiTietGroup.First().DuocDuyet == true ? hoanTraVatTuChiTietGroup.First().NgayDuyet?.ApplyFormatNgayThangNam() : hoanTraVatTuChiTietGroup.First().CreatedOn?.ApplyFormatNgayThangNam(),
                            NgayLapPhieu = hoanTraVatTuChiTietGroup.First().DuocDuyet == true ? hoanTraVatTuChiTietGroup.First().NgayDuyet?.ApplyFormatDate() : ycTraVatTuBN.ThoiDiemHoanTraTongHopTuNgay?.ApplyFormatDate() + " - " + ycTraVatTuBN.ThoiDiemHoanTraTongHopDenNgay?.ApplyFormatDate()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    }
                }
            }

            return content;
        }
    }
}