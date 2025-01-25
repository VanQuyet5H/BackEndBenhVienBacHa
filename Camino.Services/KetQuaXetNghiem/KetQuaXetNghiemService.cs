using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Camino.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KetQuaXetNghiem;
using Camino.Core.Helpers;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Caching;
using Camino.Services.ThietBiXetNghiems;

namespace Camino.Services.KetQuaXetNghiem
{
    [ScopedDependencyAttribute(ServiceType = typeof(IKetQuaXetNghiemService))]
    public class KetQuaXetNghiemService : MasterFileService<PhienXetNghiem>, IKetQuaXetNghiemService
    {
        private readonly IStaticCacheManager _cacheManager;
        private readonly IUserAgentHelper _userAgentHelper;

        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;

        private readonly IRepository<MayXetNghiem> _mayXetNghiemRepository;

        public KetQuaXetNghiemService(IRepository<PhienXetNghiem> BaseRepository
            , IUserAgentHelper userAgentHelper, IRepository<MayXetNghiem> mayXetNghiemRepository, IStaticCacheManager cacheManager
            , IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository) : base(BaseRepository)
        {
            _userAgentHelper = userAgentHelper;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _mayXetNghiemRepository = mayXetNghiemRepository;
            _cacheManager = cacheManager;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<KetQuaXetNghiemGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    //DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out fromDate);

                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        toDate = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out toDate);
                    }
                    toDate = toDate.AddSeconds(59).AddMilliseconds(999);
                    //query = query.Where(p => p.ThoiDiemBatDau >= tuNgay && p.ThoiDiemBatDau <= denNgay);
                }
            }

            var query = BaseRepository.TableNoTracking
                            .Where(p => p.ThoiDiemBatDau >= fromDate && p.ThoiDiemBatDau <= toDate)
                            //.Where(p => p.PhienXetNghiemChiTiets.Any(z => z.KetQuaXetNghiemChiTiets.Any()))
                            .Select(s => new KetQuaXetNghiemGridVo
                            {
                                Id = s.Id,
                                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                BarCode = s.BarCodeId,
                                MaSoBHYT = s.YeuCauTiepNhan.BHYTMaSoThe,
                                MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBN = s.BenhNhan.MaBN,
                                HoTen = s.BenhNhan.HoTen,
                                GioiTinh = s.BenhNhan.GioiTinh.GetDescription(),
                                NamSinh = s.BenhNhan.NamSinh + "",
                                DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                                NguoiThucHien = s.NhanVienThucHien.User.HoTen,
                                NgayThucHien = s.ThoiDiemBatDau,
                                NguoiDuyetKQ = s.NhanVienKetLuan.User.HoTen,
                                NgayDuyetKQ = s.ThoiDiemKetLuan,
                                //ChoKetQuaChayLai = s.PhienXetNghiemChiTiets.Any(o => o.DaGoiDuyet != true && o.ChayLaiKetQua == true),
                                ChoKetQua = s.ChoKetQua,//s.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))),
                                //ChoDuyetKetQua = s.PhienXetNghiemChiTiets.Any(z => z.DaGoiDuyet == true && z.ThoiDiemKetLuan == null),
                                //ChoDuyetKetQua = !ChoKetQua && !DaCoKetQua
                                DaCoKetQua = s.ThoiDiemKetLuan != null,
                                ThoiDiemBatDau = s.ThoiDiemBatDau
                            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<KetQuaXetNghiemGridVo>(queryInfo.AdditionalSearchString);
                //if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                //{
                //    DateTime denNgay;
                //    queryString.FromDate.TryParseExactCustom(out var tuNgay);

                //    if (string.IsNullOrEmpty(queryString.ToDate))
                //    {
                //        denNgay = DateTime.Now;
                //    }
                //    else
                //    {
                //        queryString.ToDate.TryParseExactCustom(out denNgay);
                //    }
                //    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                //    query = query.Where(p => p.ThoiDiemBatDau >= tuNgay && p.ThoiDiemBatDau <= denNgay);
                //}
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.HoTen,
                         g => g.BarCode,
                         g => g.MaTN,
                         g => g.MaBN
                   );
                }

                if (queryString.ChoKetQua == true && queryString.ChoDuyetKetQua == false && queryString.DaCoKetQua == false)
                {
                    query = query.Where(p => p.ChoKetQua == true);
                }
                else if (queryString.ChoKetQua == true && queryString.ChoDuyetKetQua == true && queryString.DaCoKetQua == false)
                {
                    query = query.Where(p => p.DaCoKetQua == false);
                }
                else if (queryString.ChoKetQua == true && queryString.ChoDuyetKetQua == false && queryString.DaCoKetQua == true)
                {
                    query = query.Where(p => p.ChoKetQua == true || p.DaCoKetQua == true);
                }
                else if (queryString.ChoKetQua == false && queryString.ChoDuyetKetQua == true && queryString.DaCoKetQua == false)
                {
                    query = query.Where(p => p.ChoKetQua == false && p.DaCoKetQua == false);
                }
                else if (queryString.ChoKetQua == false && queryString.ChoDuyetKetQua == true && queryString.DaCoKetQua == true)
                {
                    query = query.Where(p => p.ChoKetQua == false);
                }
                else if (queryString.ChoKetQua == false && queryString.ChoDuyetKetQua == false && queryString.DaCoKetQua == true)
                {
                    query = query.Where(p => p.DaCoKetQua == true);
                }
            }
            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //Task<List<KetQuaXetNghiemGridVo>> queryTask;
            IQueryable<KetQuaXetNghiemGridVo> queryTask;
            //if (queryInfo.Sort != null && queryInfo.Sort.Any(o => o.Field == nameof(KetQuaXetNghiemGridVo.TrangThai)))
            //{
            //    if (queryInfo.Sort.First(o => o.Field == nameof(KetQuaXetNghiemGridVo.TrangThai)).Dir == "desc")
            //    {
            //        //queryTask = query.OrderByDescending(o => o.DaCoKetQua).ThenBy(o => o.ChoKetQua).ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //        //    .Take(queryInfo.Take).ToListAsync();
            //        queryTask = query.OrderByDescending(o => o.DaCoKetQua).ThenBy(o => o.ChoKetQua).ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //            .Take(queryInfo.Take);
            //    }
            //    else
            //    {
            //        //queryTask = query.OrderByDescending(o => o.ChoKetQua).ThenBy(o => o.DaCoKetQua).ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //        //    .Take(queryInfo.Take).ToListAsync();
            //        queryTask = query.OrderByDescending(o => o.ChoKetQua).ThenBy(o => o.DaCoKetQua).ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //            .Take(queryInfo.Take);
            //    }

            //}
            //else
            //{
            //    //queryTask = query.OrderBy(queryInfo.SortString).ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //    //    .Take(queryInfo.Take).ToListAsync();
            //    queryTask = query.OrderBy(queryInfo.SortString).ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //        .Take(queryInfo.Take);
            //}
            queryTask = query.OrderByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip).Take(queryInfo.Take);
            //await Task.WhenAll(countTask, queryTask);
            var data = queryTask.ToList();
            var count = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            data.ForEach(o => o.ChoDuyetKetQua = (o.ChoKetQua == false && o.DaCoKetQua == false));
            return new GridDataSource { Data = data.ToArray(), TotalRowCount = count };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<KetQuaXetNghiemGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    //DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out fromDate);

                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        toDate = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out toDate);
                    }
                    toDate = toDate.AddSeconds(59).AddMilliseconds(999);
                    //query = query.Where(p => p.ThoiDiemBatDau >= tuNgay && p.ThoiDiemBatDau <= denNgay);
                }
            }
            var query = BaseRepository.TableNoTracking
                            .Where(p => p.ThoiDiemBatDau >= fromDate && p.ThoiDiemBatDau <= toDate )
                            .Select(s => new KetQuaXetNghiemGridVo
                            {
                                Id = s.Id,
                                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                BarCode = s.BarCodeId,
                                MaSoBHYT = s.YeuCauTiepNhan.BHYTMaSoThe,
                                MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBN = s.BenhNhan.MaBN,
                                HoTen = s.BenhNhan.HoTen,
                                //GioiTinh = s.BenhNhan.GioiTinh.GetDescription(),
                                //NamSinh = s.BenhNhan.NamSinh + "",
                                //DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                                //NguoiThucHien = s.NhanVienThucHien.User.HoTen,
                                //NgayThucHien = s.ThoiDiemBatDau,
                                //NguoiDuyetKQ = s.NhanVienKetLuan.User.HoTen,
                                //NgayDuyetKQ = s.ThoiDiemKetLuan,
                                //ChoKetQuaChayLai = s.PhienXetNghiemChiTiets.Any(o => o.DaGoiDuyet != true && o.ChayLaiKetQua == true),
                                ChoKetQua = s.ChoKetQua,//s.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))),
                                //ChoDuyetKetQua = s.PhienXetNghiemChiTiets.Any(z => z.DaGoiDuyet == true && z.ThoiDiemKetLuan == null),
                                //ChoDuyetKetQua = !ChoKetQua && !DaCoKetQua
                                DaCoKetQua = s.ThoiDiemKetLuan != null,
                                ThoiDiemBatDau = s.ThoiDiemBatDau
                            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<KetQuaXetNghiemGridVo>(queryInfo.AdditionalSearchString);
                //if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                //{
                //    DateTime denNgay;
                //    queryString.FromDate.TryParseExactCustom(out var tuNgay);

                //    if (string.IsNullOrEmpty(queryString.ToDate))
                //    {
                //        denNgay = DateTime.Now;
                //    }
                //    else
                //    {
                //        queryString.ToDate.TryParseExactCustom(out denNgay);
                //    }
                //    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                //    query = query.Where(p => p.ThoiDiemBatDau >= tuNgay && p.ThoiDiemBatDau <= denNgay);
                //}
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.BarCode,
                        g => g.MaTN,
                        g => g.MaBN
                   );
                }
                if (queryString.ChoKetQua == true && queryString.ChoDuyetKetQua == false && queryString.DaCoKetQua == false)
                {
                    query = query.Where(p => p.ChoKetQua == true);
                }
                else if (queryString.ChoKetQua == true && queryString.ChoDuyetKetQua == true && queryString.DaCoKetQua == false)
                {
                    query = query.Where(p => p.DaCoKetQua == false);
                }
                else if (queryString.ChoKetQua == true && queryString.ChoDuyetKetQua == false && queryString.DaCoKetQua == true)
                {
                    query = query.Where(p => p.ChoKetQua == true || p.DaCoKetQua == true);
                }
                else if (queryString.ChoKetQua == false && queryString.ChoDuyetKetQua == true && queryString.DaCoKetQua == false)
                {
                    query = query.Where(p => p.ChoKetQua == false && p.DaCoKetQua == false);
                }
                else if (queryString.ChoKetQua == false && queryString.ChoDuyetKetQua == true && queryString.DaCoKetQua == true)
                {
                    query = query.Where(p => p.ChoKetQua == false);
                }
                else if (queryString.ChoKetQua == false && queryString.ChoDuyetKetQua == false && queryString.DaCoKetQua == true)
                {
                    query = query.Where(p => p.DaCoKetQua == true);
                }
            }
            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = query.Count() };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? PhienXetNghiemId = null, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par = 0;
            if (PhienXetNghiemId != null && PhienXetNghiemId != 0)
            {
                par = PhienXetNghiemId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.AdditionalSearchString);
            }

            var query = _phienXetNghiemChiTietRepository.TableNoTracking
                .Include(p => p.YeuCauChayLaiXetNghiem)
               .Where(p => p.KetQuaXetNghiemChiTiets.Any() && p.PhienXetNghiemId == par)
               .Select(s => new KetQuaXetNghiemChildGridVo
               {
                   Id = s.Id,
                   TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten,
                   NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                   MaDichVu = s.DichVuKyThuatBenhVien.Ma,
                   TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                   ThoiGianChiDinh = s.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                   NguoiChiDinh = s.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                   BenhPham = s.YeuCauDichVuKyThuat.BenhPhamXetNghiem,

                   YeuCauChayLai = s.ChayLaiKetQua,
                   DaDuyet = s.YeuCauChayLaiXetNghiem != null ? s.YeuCauChayLaiXetNghiem.DuocDuyet : false,
                   NguoiYeuCau = s.YeuCauChayLaiXetNghiem != null ? s.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                   LyDoYeuCau = s.YeuCauChayLaiXetNghiem != null ? s.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                   NguoiDuyet = s.YeuCauChayLaiXetNghiem != null ? s.YeuCauChayLaiXetNghiem.NhanVienDuyet.User.HoTen : "",

                   //
                   LoaiMau = s.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
                   NgayYeuCauDisplay = s.YeuCauChayLaiXetNghiem != null ? s.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                   NgayDuyetDisplay = s.YeuCauChayLaiXetNghiem != null ? (s.YeuCauChayLaiXetNghiem.NgayDuyet != null
                            ? (s.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "") : "",
               });
            var sortString = queryInfo.SortString.Contains("TrangThai") ? queryInfo.SortString.Replace("TrangThai", "MaDichVu") : queryInfo.SortString;
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sortString)
                //.Skip(queryInfo.Skip).Take(queryInfo.Take)
                .ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);


            //var lstDanhSachKetQuaNhomDichVu = new List<KetQuaNhomDichVu>();

            foreach (var item in queryTask.Result)
            {
                var lstLoaiMau = queryTask.Result.Where(p => p.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId).Select(p => p.LoaiMau).Distinct().ToList();
                item.DanhSachLoaiMau = lstLoaiMau;

                var lstLoaiMauTongCong = BaseRepository.TableNoTracking
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .FirstOrDefault(p => p.Id == par)?.YeuCauTiepNhan.YeuCauDichVuKyThuats
                    .Where(p => p.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                    .Select(p => p.DichVuKyThuatBenhVien).Select(p => p.LoaiMauXetNghiem.GetDescription()).Distinct()
                    .Where(p => p != null).ToList();

                var lstLoaiMauKhongDat = new List<string>();

                foreach (var loaiMau in lstLoaiMauTongCong)
                {
                    var mauXetNghiem = BaseRepository.TableNoTracking
                        .Include(p => p.MauXetNghiems)
                        .FirstOrDefault(p => p.Id == par)?.MauXetNghiems
                            .Where(p => p.LoaiMauXetNghiem.GetDescription() == loaiMau && p.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId).LastOrDefault();
                    if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
                    {
                        lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
                    }
                }

                item.DanhSachLoaiMauTongCong = lstLoaiMauTongCong;
                item.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
            }

            var result = queryTask.Result.OrderBy(p => p.Id).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var par = long.Parse(queryInfo.AdditionalSearchString);

            var query = _phienXetNghiemChiTietRepository.TableNoTracking
               .Where(p => p.KetQuaXetNghiemChiTiets.Any() && p.PhienXetNghiemId == par)
               .Select(s => new KetQuaXetNghiemChildGridVo
               {
                   Id = s.Id,
                   TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten,
                   MaDichVu = s.DichVuKyThuatBenhVien.Ma,
                   TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                   ThoiGianChiDinh = s.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                   NguoiChiDinh = s.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                   BenhPham = s.YeuCauDichVuKyThuat.BenhPhamXetNghiem,
                   LoaiMau = s.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
               });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetListMayXetNghiem(DropDownListRequestModel model)
        {
            var mayXetNghiems = _cacheManager.Get(ThietBiXetNghiemService.MAYXETNGHIEMS_PATTERN_KEY, () =>
            {
                return _mayXetNghiemRepository.TableNoTracking.Where(p => p.HieuLuc).ToList();
            });

            //var list = _mayXetNghiemRepository.TableNoTracking
            //    .Where(p => p.HieuLuc)
            //    .ApplyLike(model.Query, g => g.Ten)
            //    .Select(i => new LookupItemVo
            //    {
            //        DisplayName = i.Ten,
            //        KeyId = i.Id
            //    });
            var list = mayXetNghiems
                .Where(g => string.IsNullOrEmpty(model.Query) || g.Ten.Contains(model.Query))
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });
            return list.ToList();
        }

        public async Task<int> TrangThaiKQXNGanNhat(long phienXetNghiemId)
        {
            var phienXetNghiem = BaseRepository.TableNoTracking.Include(z => z.PhienXetNghiemChiTiets).Where(z => z.Id == phienXetNghiemId).First();
            var type = phienXetNghiem.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))) ? 1 : (phienXetNghiem.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan != null) ? 4 : 3);
            return type;
        }
    }
}
