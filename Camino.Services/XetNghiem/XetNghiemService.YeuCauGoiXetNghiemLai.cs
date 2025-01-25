using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Camino.Services.XetNghiem
{
    public partial class XetNghiemService
    {
        #region Danh sách yêu cầu chạy lại xét nghiệm

        public async Task<GridDataSource> GetDanhSachYeuCauChayLaiXetNghiemForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new YeuCauGoiLaiXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<YeuCauGoiLaiXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = DataYeuCauChayLaiXetNghiem(null, queryInfo, queryObject);
            var queryTuDaDuyet = DataYeuCauChayLaiXetNghiem(true, queryInfo, queryObject);
            var query = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new YeuCauGoiLaiXetNghiemGridVo()).AsQueryable();

            if (queryObject.DangChoDuyet == false && queryObject.DaDuyet == false)
            {
                queryObject.DangChoDuyet = true;
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

            if (queryObject.TuNgayDenNgay != null)
            {
                if (queryObject.TuNgayDenNgay != null && queryObject.TuNgayDenNgay.startDate != null)
                {
                    var tuNgay = queryObject.TuNgayDenNgay.startDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayDuyetKetQua != null && tuNgay <= p.NgayDuyetKetQua.Value.Date);
                }
                if (queryObject.TuNgayDenNgay != null && queryObject.TuNgayDenNgay.endDate != null)
                {
                    var denNgay = queryObject.TuNgayDenNgay.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayDuyetKetQua != null && denNgay >= p.NgayDuyetKetQua.Value.Date);
                }
            }


            var resThenGroups = query.Select(cc => cc).OrderByDescending(cc => cc.Id).GroupBy(cc => new { cc.PhienXetNghiemId })
                                                    .Select(s => new YeuCauGoiLaiXetNghiemGridVo
                                                    {
                                                        Id = s.First().Id,
                                                        MaTN = s.First().MaTN,
                                                        MaBN = s.First().MaBN,
                                                        HoTen = s.First().HoTen,
                                                        GioiTinh = s.First().GioiTinh,
                                                        NamSinh = s.First().NamSinh,
                                                        TrangThai = s.First().TrangThai,
                                                        TenTrangThai = s.First().TenTrangThai,
                                                        DiaChi = s.First().DiaChi,
                                                        NguoiDuyetKetQua = s.First().NguoiDuyetKetQua,
                                                        NgayDuyetKetQua = s.First().NgayDuyetKetQua,
                                                        PhienXetNghiemId = s.First().PhienXetNghiemId,
                                                        MaBarCode = s.First().MaBarCode,
                                                        NhanVienYeuCauId = s.First().NhanVienYeuCauId,
                                                        DanhSachPhienXetNghiemIds = s.Where(cc => cc.TrangThai == 1).Select(cc => cc.Id).ToList()
                                                    });


            var countTask = resThenGroups.CountAsync();
            var queryTask = isAllData == true ? resThenGroups.ToArrayAsync() : resThenGroups
                .OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalDanhSachYeuCauChayLaiXetNghiemForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new YeuCauGoiLaiXetNghiemSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<YeuCauGoiLaiXetNghiemSearch>(queryInfo.AdditionalSearchString);
            }


            var queryDangChoDuyet = DataYeuCauChayLaiXetNghiem(null, queryInfo, queryObject);
            var queryTuDaDuyet = DataYeuCauChayLaiXetNghiem(true, queryInfo, queryObject);
            var query = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new YeuCauGoiLaiXetNghiemGridVo()).AsQueryable();

            if (queryObject.DangChoDuyet == false && queryObject.DaDuyet == false)
            {
                queryObject.DangChoDuyet = true;
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

            if (queryObject.TuNgayDenNgay != null)
            {
                if (queryObject.TuNgayDenNgay != null && queryObject.TuNgayDenNgay.startDate != null)
                {
                    var tuNgay = queryObject.TuNgayDenNgay.startDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayDuyetKetQua != null && tuNgay <= p.NgayDuyetKetQua.Value.Date);
                }
                if (queryObject.TuNgayDenNgay != null && queryObject.TuNgayDenNgay.endDate != null)
                {
                    var denNgay = queryObject.TuNgayDenNgay.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayDuyetKetQua != null && denNgay >= p.NgayDuyetKetQua.Value.Date);
                }
            }

            if (queryInfo.SortString != null
                 && !queryInfo.SortString.Equals("NgayNhap desc,Id asc")
                 && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var resThenGroups = query.Select(cc => cc).GroupBy(cc => new { cc.PhienXetNghiemId });
            var countTask = resThenGroups.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        private IQueryable<YeuCauGoiLaiXetNghiemGridVo> DataYeuCauChayLaiXetNghiem(bool? duocDuyet, QueryInfo queryInfo, YeuCauGoiLaiXetNghiemSearch yeuCauChayLaiXN)
        {
            var query = _yeuCauChayLaiXetNghiemRepository.TableNoTracking;
            if (duocDuyet == true)
            {
                query = query.Where(cc => cc.DuocDuyet != null);
            }
            else
            {
                query = query.Where(p => p.DuocDuyet == duocDuyet);
            }
            var res = query.Include(c => c.PhienXetNghiem).ThenInclude(cc => cc.YeuCauTiepNhan)
                                                    .Include(c => c.PhienXetNghiem).ThenInclude(cc => cc.PhienXetNghiemChiTiets).ThenInclude(cc => cc.KetQuaXetNghiemChiTiets)
                                                    .Select(s => new YeuCauGoiLaiXetNghiemGridVo
                                                    {
                                                        Id = s.Id,
                                                        MaTN = s.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                        MaBN = s.PhienXetNghiem.YeuCauTiepNhan.BenhNhan.MaBN,
                                                        HoTen = s.PhienXetNghiem.YeuCauTiepNhan.HoTen,
                                                        GioiTinh = s.PhienXetNghiem.YeuCauTiepNhan.GioiTinh.GetDescription(),
                                                        NamSinh = s.PhienXetNghiem.YeuCauTiepNhan.NamSinh,
                                                        TrangThai = s.DuocDuyet == null ? 1 : 2,
                                                        TenTrangThai = s.DuocDuyet == null ? "Chờ xử lý" : "Đã duyệt",
                                                        DiaChi = s.PhienXetNghiem.YeuCauTiepNhan.DiaChiDayDu,
                                                        NguoiDuyetKetQua = s.PhienXetNghiem.NhanVienKetLuan.User.HoTen,
                                                        NgayDuyetKetQua = s.PhienXetNghiem.ThoiDiemKetLuan,
                                                        PhienXetNghiemId = s.PhienXetNghiem.Id,
                                                        NhanVienYeuCauId = s.NhanVienYeuCauId,
                                                        MaBarCode = s.PhienXetNghiem.BarCodeId
                                                    });

            if (!string.IsNullOrEmpty(yeuCauChayLaiXN.SearchString))
            {
                res = res.ApplyLike(yeuCauChayLaiXN.SearchString.Trim(),
                                                 g => g.HoTen,
                                                 g => g.MaBN,
                                                 g => g.MaTN,
                                                 g => g.NamSinh.ToString(),
                                                 g => g.DiaChi,
                                                 g => g.NguoiDuyetKetQua,
                                                 g => g.MaBarCode);
            }
            if (queryInfo.SortString != null && queryInfo.SortString.Equals("TrangThai asc"))
            {
                res = res.OrderBy(queryInfo.SortString);
            }
            return res;
        }

        #endregion

        #region Danh sách yêu cầu chạy lại xét nghiệm chi tiết

        public async Task<GridDataSource> GetDanhSachYeuCauChayLaiXetNghiemChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            long phienXetNghiemId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;
            if (isAllData)
                phienXetNghiemId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var nhomDichVuBenhVienIds = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(o => o.PhienXetNghiemId == phienXetNghiemId).Select(c => c.NhomDichVuBenhVienId).ToList();
            var query = _yeuCauChayLaiXetNghiemRepository.TableNoTracking
                                                             .Include(c => c.NhomDichVuBenhVien)
                                                             .Where(cc => cc.PhienXetNghiemId == phienXetNghiemId)
                                                             .Select(s => new YeuCauGoiLaiXetNghiemChiTietGridVo
                                                             {
                                                                 Id = s.Id,
                                                                 NhomXetNghiem = s.NhomDichVuBenhVien.Ten,
                                                                 NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                                                                 NgayYeuCau = s.NgayYeuCau,
                                                                 LyDoYeuCau = s.LyDoYeuCau,
                                                                 TrangThai = s.DuocDuyet == null ? 1 : s.DuocDuyet == true ? 2 : 3,
                                                                 TenTrangThai = s.DuocDuyet == null ? "Chờ Duyệt" : s.DuocDuyet == true ? "Đã duyệt" : "Từ chối",
                                                                 NgayDuyet = s.NgayDuyet,
                                                                 NguoiDuyet = s.NhanVienDuyet.User.HoTen,
                                                                 LyDoTuChoi = s.LyDoKhongDuyet
                                                             });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalDanhSachYeuCauChayLaiXetNghiemChiTietForGridAsync(QueryInfo queryInfo)
        {
            long phienXetNghiemId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;
            var nhomDichVuBenhVienIds = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(o => o.PhienXetNghiemId == phienXetNghiemId).Select(c => c.NhomDichVuBenhVienId).ToList();
            var query = _yeuCauChayLaiXetNghiemRepository.TableNoTracking
                                                             .Include(c => c.NhomDichVuBenhVien)
                                                             .Where(cc => cc.PhienXetNghiemId == phienXetNghiemId)
                                                             .Select(s => new YeuCauGoiLaiXetNghiemChiTietGridVo
                                                             {
                                                                 Id = s.Id,
                                                                 NhomXetNghiem = s.NhomDichVuBenhVien.Ten,
                                                                 NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                                                                 NgayYeuCau = s.NgayYeuCau,
                                                                 LyDoYeuCau = s.LyDoYeuCau,
                                                                 TrangThai = s.DuocDuyet == null ? 1 : s.DuocDuyet == true ? 2 : 3,
                                                                 TenTrangThai = s.DuocDuyet == null ? "Chờ Duyệt" : s.DuocDuyet == true ? "Đã duyệt" : "Từ chối",
                                                                 NgayDuyet = s.NgayDuyet,
                                                                 LyDoTuChoi = s.LyDoKhongDuyet
                                                             });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region Chi tiết Duyệt Yêu Cầu Chạy Lại Xét Nghiệm

        public ThongTinHanhChinhXN ThongTinHanhChinhXN(long phienXetNghiemId)
        {
            var thongTinYeuCauTiepNhan = BaseRepository.TableNoTracking.Where(cc => cc.Id == phienXetNghiemId)
                                                       .Select(scc => scc.YeuCauTiepNhan).Include(cc => cc.PhienXetNghiems)
                                                       .Include(cc => cc.BenhNhan)
                                                       .Include(cc => cc.DanToc)
                                                       .Include(cc => cc.NgheNghiep)
                                                       .Include(cc => cc.YeuCauKhamBenhs).ThenInclude(ccc => ccc.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                                                       .Include(cc => cc.YeuCauKhamBenhs).ThenInclude(ccc => ccc.NoiThucHien)

                                                       //BVHD-3800
                                                       .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)

                                                       .FirstOrDefault();

            var thongTinHanhChinhXN = new ThongTinHanhChinhXN
            {
                MaBarCode = thongTinYeuCauTiepNhan.PhienXetNghiems.FirstOrDefault().BarCodeId,
                MaBN = thongTinYeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = thongTinYeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = thongTinYeuCauTiepNhan.HoTen,
                DungTuyen = thongTinYeuCauTiepNhan.LyDoVaoVien.GetDescription(),
                Tuoi = thongTinYeuCauTiepNhan.NamSinh != null ? (DateTime.Now.Year - thongTinYeuCauTiepNhan.NamSinh) : null,
                GioiTinh = thongTinYeuCauTiepNhan.GioiTinh.GetDescription(),
                MucHuong = thongTinYeuCauTiepNhan.BHYTMucHuong,
                DanToc = thongTinYeuCauTiepNhan.DanToc?.Ten,
                DiaChi = thongTinYeuCauTiepNhan.DiaChiDayDu,
                NgheNghiep = thongTinYeuCauTiepNhan.NgheNghiep?.Ten,
                MaBhyt = thongTinYeuCauTiepNhan.BHYTMaSoThe,

                //BVHD-3800
                LaCapCuu = thongTinYeuCauTiepNhan.LaCapCuu ?? thongTinYeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu
            };

            if (thongTinYeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiChiDinhId != null))
            {
                var yckb = thongTinYeuCauTiepNhan.YeuCauKhamBenhs
                    .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiChiDinhId != null).ToList();

                thongTinHanhChinhXN.PhongKham = yckb.Select(p => p.NoiChiDinh).Select(p => p.Ten).Distinct().Join(" , ");
                thongTinHanhChinhXN.KhoaChiDinh = yckb.Select(p => p.NoiChiDinh).Select(p => p.KhoaPhong).Select(p => p.Ten).Distinct().Join(" , ");
            }

            var yeuCauKhamBenhs = thongTinYeuCauTiepNhan?.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null)
             .OrderBy(cc => cc.ThoiDiemHoanThanh).ToList();

            var tenicdKemTheos = yeuCauKhamBenhs.Select(cc => cc.GhiChuICDChinh);
            thongTinHanhChinhXN.ChuanDoan = string.Join(";", tenicdKemTheos);

            thongTinHanhChinhXN.TrangThai = TrangThaiYeuCauXetNghiemLai(phienXetNghiemId);

            return thongTinHanhChinhXN;
        }

        private bool TrangThaiYeuCauXetNghiemLai(long phienXetNghiemId)
        {
            var yeuCauChayLaiXetNghiems = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId && cc.DuocDuyet == null);
            if (yeuCauChayLaiXetNghiems.Any())
                return true;
            return false;
        }

        public GridDataSource GetDanhSachKQChiTietXetNghiemForGrid(QueryInfo queryInfo)
        {
            long phienXetNghiemId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var result = BaseRepository.TableNoTracking.Where(cc => cc.Id == phienXetNghiemId)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(cc => cc.MayXetNghiem)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                        .Include(x => x.MauXetNghiems)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                        .ThenInclude(x => x.NhomDichVuBenhVien)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                        .ThenInclude(x => x.YeuCauDichVuKyThuat)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem)
                                        .Include(x => x.YeuCauChayLaiXetNghiems)
                                        .ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                        .Include(x => x.YeuCauChayLaiXetNghiems)
                                        .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                        .Include(x => x.YeuCauChayLaiXetNghiems)
                                        .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                        .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                        .FirstOrDefault();

            //Lấy ra yêu cầu của phiên xét nghiệm lại được duyệt == null           
            var yeuCauXetNghiemLais = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId && cc.DuocDuyet == null)
                                                                       .Include(cc => cc.NhomDichVuBenhVien)
                                                                       .Include(cc => cc.PhienXetNghiem)
                                                                       .Include(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                       .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                       .Select(x => x);


            if (!yeuCauXetNghiemLais.Any())
            {               
                yeuCauXetNghiemLais = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId && cc.DuocDuyet != null)
                                                                       .Include(cc => cc.NhomDichVuBenhVien)
                                                                       .Include(cc => cc.PhienXetNghiem).ThenInclude(cc => cc.PhienXetNghiemChiTiets)
                                                                       .Include(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                       .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User);
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                yeuCauXetNghiemLais = yeuCauXetNghiemLais.AsQueryable().ApplyLike(queryInfo.SearchTerms.Trim(),
                                                 g => g.PhienXetNghiem.BarCodeId,
                                                 g => g.NhomDichVuBenhVien.Ten,
                                                 g => g.NhanVienYeuCau.User.HoTen,
                                                 g => g.NhanVienDuyet.User.HoTen,
                                                 g => g.LyDoKhongDuyet);
            }

            var resultData = new List<KetQuaXetNghiemChiTietVo>();
            //add detail
            var nhomDichVuId = new List<long>();
            var nhomDichVu = new List<string>();


            var phienXetNghiems = yeuCauXetNghiemLais.Select(cc => cc.PhienXetNghiem).Include(cc => cc.YeuCauChayLaiXetNghiems);
            if (phienXetNghiems.Any())
            {
                var items = phienXetNghiems.SelectMany(cc => cc.PhienXetNghiemChiTiets).Include(cc => cc.PhienXetNghiem)
                                                                             .ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                                             .ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                             .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                                             .Include(cc => cc.PhienXetNghiem).ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                                             .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                             .Include(cc => cc.PhienXetNghiem).ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                                             .ThenInclude(cc => cc.NhomDichVuBenhVien)
                                                                             .GroupBy(cc => cc.YeuCauChayLaiXetNghiemId)
                                                                             .Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();
                if (items.Any())
                {
                    var dsYeuCauChayLaiXetNghiems = items.SelectMany(cc => cc.PhienXetNghiem.YeuCauChayLaiXetNghiems).Distinct();

                    var phienXNChiTietIds = yeuCauXetNghiemLais.Select(c => c.Id);
                    dsYeuCauChayLaiXetNghiems = dsYeuCauChayLaiXetNghiems.Where(c => phienXNChiTietIds.Contains(c.Id));

                    foreach (var yeuCauXetNghiemLai in dsYeuCauChayLaiXetNghiems)
                    {
                        if (yeuCauXetNghiemLai != null)
                        {
                            var child = new KetQuaXetNghiemChiTietVo
                            {
                                Id = yeuCauXetNghiemLai.Id,
                                PhienXetNghiemId = yeuCauXetNghiemLai.PhienXetNghiemId,
                                Barcode = yeuCauXetNghiemLai.PhienXetNghiem.BarCodeId,
                                TenNhomDichVuBenhVien = yeuCauXetNghiemLai.NhomDichVuBenhVien.Ten,
                                NhomDichVuBenhVienId = yeuCauXetNghiemLai.NhomDichVuBenhVienId,
                                TrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? 1 : yeuCauXetNghiemLai.DuocDuyet == true ? 2 : 3,
                                TenTrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? "Chờ duyệt" : yeuCauXetNghiemLai.DuocDuyet == true ? "Đã duyệt" : "Từ chối",
                                NhanVienYeuCauId = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NhanVienYeuCau.User.Id : 0,
                                NguoiYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai?.NhanVienYeuCau?.User?.HoTen : "",
                                NgayYeuCauDisplay = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai?.NgayYeuCau.ApplyFormatDateTime() : "",
                                LyDoYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.LyDoYeuCau : "",
                                NguoiDuyet = yeuCauXetNghiemLai.NhanVienDuyet != null ? yeuCauXetNghiemLai.NhanVienDuyet.User.HoTen : "",
                                NgayDuyetDisplay = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.NgayDuyet?.ApplyFormatDateTime() : "",
                                LyDo = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.LyDoKhongDuyet : "",
                                LanThucHien = yeuCauXetNghiemLai.LanThucHien
                            };

                            resultData.Add(child);
                        }
                    }
                }
            }
            else
            {
                foreach (var yeuCauXetNghiemLai in yeuCauXetNghiemLais)
                {

                    var item = result.PhienXetNghiemChiTiets.Where(cc => cc.PhienXetNghiemId == yeuCauXetNghiemLai.PhienXetNghiemId
                                                 && cc.NhomDichVuBenhVienId == yeuCauXetNghiemLai.NhomDichVuBenhVienId).LastOrDefault();
                    if (nhomDichVuId.Any(p => p == item.NhomDichVuBenhVienId)) continue;
                    if (item != null)
                    {
                        var child = new KetQuaXetNghiemChiTietVo
                        {
                            Id = yeuCauXetNghiemLai.Id,
                            PhienXetNghiemId = yeuCauXetNghiemLai.PhienXetNghiemId,
                            Barcode = yeuCauXetNghiemLai.PhienXetNghiem.BarCodeId,
                            TenNhomDichVuBenhVien = item.NhomDichVuBenhVien.Ten,
                            NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                            TrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? 1 : yeuCauXetNghiemLai.DuocDuyet == true ? 2 : 3,
                            TenTrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? "Chờ duyệt" : yeuCauXetNghiemLai.DuocDuyet == true ? "Đã duyệt" : "Từ chối",
                            NhanVienYeuCauId = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NhanVienYeuCau.User.Id : 0,
                            NguoiYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NhanVienYeuCau.User.HoTen : "",
                            NgayYeuCauDisplay = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NgayYeuCau.ApplyFormatDateTime() : "",
                            LyDoYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.LyDoYeuCau : "",
                            NguoiDuyet = item.NhanVienKetLuan != null ? item.NhanVienKetLuan.User.HoTen : "",
                            NgayDuyetDisplay = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.NgayDuyet?.ApplyFormatDateTime() : "",
                            LyDo = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.LyDoKhongDuyet : "",
                            LanThucHien = yeuCauXetNghiemLai.LanThucHien
                        };
                        nhomDichVuId.Add(item.NhomDichVuBenhVienId);
                        nhomDichVu.Add(item.NhomDichVuBenhVien.Ten);
                        resultData.Add(child);
                    }

                }
            }

            //add data detail
            //add data detail         
            foreach (var detail in resultData)
            {
                detail.DanhSachLoaiMauDaCoKetQua = nhomDichVu;
                detail.DanhSachLoaiMau = result.PhienXetNghiemChiTiets.Select(p => p.NhomDichVuBenhVien.Ten).Distinct().ToList();
                var listChiTIet = result.PhienXetNghiemChiTiets.Where(cc => cc.PhienXetNghiemId == detail.PhienXetNghiemId && cc.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
                                                               .SelectMany(cc => cc.KetQuaXetNghiemChiTiets).Where(XX => XX.LanThucHien == detail.LanThucHien)
                                                               .ToList();

                listChiTIet = listChiTIet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.Id).ToList();
                detail.datas = addDetailDataChild(queryInfo, listChiTIet, new List<KetQuaXetNghiemChiTiet>(), true);
            }

            var dataOrderBy = resultData.AsQueryable();
            var dataQuery = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();
            return new GridDataSource
            {
                Data = dataQuery,
                TotalRowCount = countTask
            };
        }
        public GridDataSource GetTotalDanhSachKQChiTietXetNghiemForGrid(QueryInfo queryInfo)
        {
            long phienXetNghiemId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var result = BaseRepository.TableNoTracking.Where(cc => cc.Id == phienXetNghiemId)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(cc => cc.MayXetNghiem)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                                      .Include(x => x.MauXetNghiems)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                                          .ThenInclude(x => x.NhomDichVuBenhVien)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                                          .ThenInclude(x => x.YeuCauDichVuKyThuat)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem)
                                                      .Include(x => x.YeuCauChayLaiXetNghiems)
                                                          .ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                      .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                                          .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                      .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                       .FirstOrDefault();



            var yeuCauXetNghiemLais = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId && cc.DuocDuyet == null)
                                                                       .Include(cc => cc.NhomDichVuBenhVien)
                                                                       .Include(cc => cc.PhienXetNghiem)
                                                                       .Include(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                       .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                       .Select(c => c);


            if (!yeuCauXetNghiemLais.Any())
            {
                yeuCauXetNghiemLais = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId && cc.DuocDuyet != null)
                                                                       .Include(cc => cc.NhomDichVuBenhVien)
                                                                       .Include(cc => cc.PhienXetNghiem).ThenInclude(cc => cc.PhienXetNghiemChiTiets)
                                                                       .Include(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                       .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User);
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                yeuCauXetNghiemLais = yeuCauXetNghiemLais.AsQueryable().ApplyLike(queryInfo.SearchTerms.Trim(),
                                                 g => g.PhienXetNghiem.BarCodeId,
                                                 g => g.NhomDichVuBenhVien.Ten,
                                                 g => g.NhanVienYeuCau.User.HoTen,
                                                 g => g.NhanVienDuyet.User.HoTen,
                                                 g => g.LyDoKhongDuyet);
            }

            var resultData = new List<KetQuaXetNghiemChiTietVo>();
            //add detail
            var nhomDichVuId = new List<long>();
            var nhomDichVu = new List<string>();


            var phienXetNghiems = yeuCauXetNghiemLais.Select(cc => cc.PhienXetNghiem).Include(cc => cc.YeuCauChayLaiXetNghiems);
            if (phienXetNghiems.Any())
            {
                var items = phienXetNghiems.SelectMany(cc => cc.PhienXetNghiemChiTiets).Include(cc => cc.PhienXetNghiem)
                                                                             .ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                                             .ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                             .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                                             .Include(cc => cc.PhienXetNghiem).ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                                             .ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                             .Include(cc => cc.PhienXetNghiem).ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                                             .ThenInclude(cc => cc.NhomDichVuBenhVien)
                                                                             .GroupBy(cc => cc.YeuCauChayLaiXetNghiemId)
                                                                             .Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();
                if (items.Any())
                {
                    var dsYeuCauChayLaiXetNghiems = items.SelectMany(cc => cc.PhienXetNghiem.YeuCauChayLaiXetNghiems).Distinct();
                    foreach (var yeuCauXetNghiemLai in dsYeuCauChayLaiXetNghiems)
                    {
                        if (yeuCauXetNghiemLai != null)
                        {
                            var child = new KetQuaXetNghiemChiTietVo
                            {
                                Id = yeuCauXetNghiemLai.Id,
                                PhienXetNghiemId = yeuCauXetNghiemLai.PhienXetNghiemId,
                                Barcode = yeuCauXetNghiemLai.PhienXetNghiem.BarCodeId,
                                TenNhomDichVuBenhVien = yeuCauXetNghiemLai.NhomDichVuBenhVien.Ten,
                                NhomDichVuBenhVienId = yeuCauXetNghiemLai.NhomDichVuBenhVienId,
                                TrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? 1 : yeuCauXetNghiemLai.DuocDuyet == true ? 2 : 3,
                                TenTrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? "Chờ duyệt" : yeuCauXetNghiemLai.DuocDuyet == true ? "Đã duyệt" : "Từ chối",
                                NhanVienYeuCauId = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NhanVienYeuCau.User.Id : 0,
                                NguoiYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai?.NhanVienYeuCau?.User?.HoTen : "",
                                NgayYeuCauDisplay = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai?.NgayYeuCau.ApplyFormatDateTime() : "",
                                LyDoYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.LyDoYeuCau : "",
                                NguoiDuyet = yeuCauXetNghiemLai.NhanVienDuyet != null ? yeuCauXetNghiemLai.NhanVienDuyet.User.HoTen : "",
                                NgayDuyetDisplay = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.NgayDuyet?.ApplyFormatDateTime() : "",
                                LyDo = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.LyDoKhongDuyet : "",
                                LanThucHien = yeuCauXetNghiemLai.LanThucHien
                            };

                            resultData.Add(child);
                        }
                    }
                }
            }
            else
            {
                foreach (var yeuCauXetNghiemLai in yeuCauXetNghiemLais)
                {

                    var item = result.PhienXetNghiemChiTiets.Where(cc => cc.PhienXetNghiemId == yeuCauXetNghiemLai.PhienXetNghiemId
                                                 && cc.NhomDichVuBenhVienId == yeuCauXetNghiemLai.NhomDichVuBenhVienId).LastOrDefault();
                    if (nhomDichVuId.Any(p => p == item.NhomDichVuBenhVienId)) continue;
                    if (item != null)
                    {
                        var child = new KetQuaXetNghiemChiTietVo
                        {
                            Id = yeuCauXetNghiemLai.Id,
                            PhienXetNghiemId = yeuCauXetNghiemLai.PhienXetNghiemId,
                            Barcode = yeuCauXetNghiemLai.PhienXetNghiem.BarCodeId,
                            TenNhomDichVuBenhVien = item.NhomDichVuBenhVien.Ten,
                            NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                            TrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? 1 : yeuCauXetNghiemLai.DuocDuyet == true ? 2 : 3,
                            TenTrangThai = yeuCauXetNghiemLai.DuocDuyet == null ? "Chờ duyệt" : yeuCauXetNghiemLai.DuocDuyet == true ? "Đã duyệt" : "Từ chối",
                            NhanVienYeuCauId = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NhanVienYeuCau.User.Id : 0,
                            NguoiYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NhanVienYeuCau.User.HoTen : "",
                            NgayYeuCauDisplay = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.NgayYeuCau.ApplyFormatDateTime() : "",
                            LyDoYeuCau = yeuCauXetNghiemLai != null ? yeuCauXetNghiemLai.LyDoYeuCau : "",
                            NguoiDuyet = item.NhanVienKetLuan != null ? item.NhanVienKetLuan.User.HoTen : "",
                            NgayDuyetDisplay = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.NgayDuyet?.ApplyFormatDateTime() : "",
                            LyDo = yeuCauXetNghiemLai.NgayDuyet != null ? yeuCauXetNghiemLai.LyDoKhongDuyet : "",
                            LanThucHien = yeuCauXetNghiemLai.LanThucHien
                        };
                        nhomDichVuId.Add(item.NhomDichVuBenhVienId);
                        nhomDichVu.Add(item.NhomDichVuBenhVien.Ten);
                        resultData.Add(child);
                    }

                }
            }

            //add data detail         
            foreach (var detail in resultData)
            {
                detail.DanhSachLoaiMauDaCoKetQua = nhomDichVu;
                detail.DanhSachLoaiMau = result.PhienXetNghiemChiTiets.Select(p => p.NhomDichVuBenhVien.Ten).Distinct().ToList();
                var listChiTIet = result.PhienXetNghiemChiTiets.Where(cc => cc.PhienXetNghiemId == detail.PhienXetNghiemId && cc.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
                                                               .SelectMany(cc => cc.KetQuaXetNghiemChiTiets).Where(XX => XX.LanThucHien == detail.LanThucHien)
                                                               .ToList();

                detail.datas = addDetailDataChild(queryInfo, listChiTIet, new List<KetQuaXetNghiemChiTiet>(), true);
            }

            var dataOrderBy = resultData.AsQueryable();
            var countTask = dataOrderBy.Count();

            return new GridDataSource
            {
                TotalRowCount = countTask
            };
        }


        private List<ListDataChild> addDetailDataChild(QueryInfo queryInfo, List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai
           , List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild
           , bool theFirst = false)
        {
            var result = new List<ListDataChild>();
            if (!lstChiTietNhomChild.Any() && theFirst != true) return null;

            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                foreach (var parent in lstParent)
                {
                    var ketQua = new ListDataChild
                    {
                        Id = parent.Id,
                        Ten = parent.DichVuXetNghiem?.Ten,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),//(parent.GiaTriMin != null || parent.GiaTriMax != null) ? parent.GiaTriMin + " - " + parent.GiaTriMax : "",
                        DonVi = parent.DonVi,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        LoaiMau = parent.NhomDichVuBenhVien.Ten,
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        //structure tree
                        IsRoot = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                        IsParent = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                    };
                    var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                    ketQua.Items = addDetailDataChild(queryInfo, lstChiTietNhomConLai, lstChiTietChild);
                    //
                    result.Add(ketQua);
                }
            }
            else
            {
                if (lstChiTietNhomChild != null)
                {
                    foreach (var parent in lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.Id))
                    {
                        var ketQua = new ListDataChild
                        {
                            Id = parent.Id,
                            Ten = parent.DichVuXetNghiem?.Ten,
                            GiaTriCu = parent.GiaTriCu,
                            GiaTriNhapTay = parent.GiaTriNhapTay,
                            GiaTriTuMay = parent.GiaTriTuMay,
                            GiaTriDuyet = parent.GiaTriDuyet,
                            ToDamGiaTri = parent.ToDamGiaTri,
                            CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),//(parent.GiaTriMin != null || parent.GiaTriMax != null) ? parent.GiaTriMin + " - " + parent.GiaTriMax : "",
                            DonVi = parent.DonVi,
                            ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                            ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                            MayXetNghiemId = parent.MayXetNghiemId,
                            TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                            ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                            NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                            LoaiMau = parent.NhomDichVuBenhVien.Ten,
                            DichVuXetNghiemId = parent.DichVuXetNghiemId,
                            //structure tree
                            IsRoot = false,
                            IsParent = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                            LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                        };

                        var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                        ketQua.Items = addDetailDataChild(queryInfo, lstChiTietNhomConLai, lstChiTietChild);
                        //
                        result.Add(ketQua);
                    }
                }
            }
            return result;
        }

        #endregion

        #region Từ chối và duyệt yêu cầu hcay lại xét nghiệm

        public bool TuChoiXetNghiem(TuChoiYeuCauGoiLaiXetNghiem tuChoiYeuCauGoiLaiXetNghiem)
        {
            var yeuCauChayLaiXetNghiems = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == tuChoiYeuCauGoiLaiXetNghiem.PhienXetNghiemId && cc.DuocDuyet == null).ToList();
            foreach (var item in yeuCauChayLaiXetNghiems)
            {
                item.DuocDuyet = false;
                item.NgayDuyet = DateTime.Now;
                item.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                item.LyDoKhongDuyet = tuChoiYeuCauGoiLaiXetNghiem.LyDoTuChoi;
                _yeuCauChayLaiXetNghiemRepository.Update(item);
            }

            return true;
        }

        public async Task DuyetXetNghiem(DanhSachGoiXetNghiemLai duyetYeuCauGoiLaiXetNghiem)
        {
            if (duyetYeuCauGoiLaiXetNghiem != null && duyetYeuCauGoiLaiXetNghiem.DuyetYeuCauGoiLaiXetNghiems.Any())
            {
                foreach (var item in duyetYeuCauGoiLaiXetNghiem.DuyetYeuCauGoiLaiXetNghiems)
                {
                    await DuyetYeuCauChayLaiXetNghiem(item.Id, item.NhanVienYeuCauId);
                }
            }

            //Cập nhật trạng thái YeuCauDichVuKyThuat Đang thực hiện

            if (duyetYeuCauGoiLaiXetNghiem != null && duyetYeuCauGoiLaiXetNghiem.DuyetYeuCauGoiLaiXetNghiems.Any())
            {
                foreach (var item in duyetYeuCauGoiLaiXetNghiem.DuyetYeuCauGoiLaiXetNghiems)
                {
                    var chayLaiXetNghiems = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(c => c.Id == item.Id).ToList();
                    if (chayLaiXetNghiems.Any())
                    {
                        foreach (var chayLaiXetNghiem in chayLaiXetNghiems)
                        {
                            var yeuCauDichVuKyThuats = _phienXetNghiemChiTietRepository.TableNoTracking
                                                                               .Where(cc => cc.PhienXetNghiemId == chayLaiXetNghiem.PhienXetNghiemId &&
                                                                                     cc.YeuCauChayLaiXetNghiemId == chayLaiXetNghiem.Id
                                                                                     && cc.KetLuan == null).Select(c => c.YeuCauDichVuKyThuat).ToList();
                            if (yeuCauDichVuKyThuats.Any())
                            {
                                foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
                                {
                                    yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien;
                                    _yeuCauDichVuKyThuatRepository.Update(yeuCauDichVuKyThuats);
                                }
                            }
                        }

                    }
                }


            }
        }




        #endregion
    }
}
