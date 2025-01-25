using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using System.Globalization;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using NLog;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachDaKham(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<DanhSachDaKhamGridVo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                            && (queryString.IsKhamDoanTatCa == true || o.NoiThucHienId == queryString.NoiTiepNhanId)
                            && ((queryString.LaKhamDoan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                                || (!queryString.LaKhamDoan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))
                            && (queryString.IsKhamDoanTatCa != true || ((queryString.CongTyId == null || o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyId))
                                                                        && (queryString.HopDongId == null || o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongId)))
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                    PhongBenhVienId = s.NoiThucHienId,
                    NoiTiepNhanId = s.YeuCauTiepNhan.NoiTiepNhanId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    NamSinhDisplay = s.YeuCauTiepNhan.NamSinh.ToString(),
                    DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemChiDinh,
                    BacSiThucHien = s.BacSiThucHien.User.HoTen,
                    BacSiThucHienId = s.BacSiThucHien.User.Id,
                    BacSiKetLuan = s.BacSiKetLuanId != null ? s.BacSiKetLuan.User.HoTen : null,
                    TrieuChungTiepNhan = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    DoiTuong = SetSoPhanTramHuongBHYT(s.YeuCauTiepNhan.BHYTMaSoThe),
                    TrangThaiYeuCauTiepNhanDisplay = s.TrangThai.GetDescription(),
                    BaoHiemChiTra = s.YeuCauTiepNhan.CoBHYT == true ? true : false,
                    NgayThucHien = s.ThoiDiemThucHien,
                    CachGiaiQuyet = s.CachGiaiQuyet,
                    BenhNhanDaTaoBenhAn = s.YeuCauNhapViens.SelectMany(c => c.YeuCauTiepNhans).Any(c => c.NoiTruBenhAn != null),
                    KhoaNhapVien = s.KhoaPhongNhapVien.Ten,
                    ThoiDiemHoanThanhKham = s.ThoiDiemHoanThanh,
                    ThoiDiemHoanThanhKhamDisplay = s.ThoiDiemHoanThanh != null ? s.ThoiDiemHoanThanh.Value.ApplyFormatDateTimeSACH() : string.Empty,
                    YeuCauTiepNhanDaHoanTat = s.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat,
                    KhamTheoPhongYeuCauKhamBenhId = s.Id,
                    CoNhapVien = s.CoNhapVien,

                    //BVHD-3924
                    TenCongTy = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten
                });
            //var queryId = query.Where(d => d.YeuCauTiepNhanId == 82031).Select(d=>d.Id).ToList();

            if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            {
                DateTime denNgay;
                queryString.FromDate.TryParseExactCustom(out var tuNgay);
                //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                //    out var tuNgay);
                if (string.IsNullOrEmpty(queryString.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryString.ToDate.TryParseExactCustom(out denNgay);
                    //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                }
                var denNgayFormat = denNgay.AddSeconds(59).AddMilliseconds(999);
                //(tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay)
                query = query.Where(p => p.ThoiDiemHoanThanhKham != null && (tuNgay == null || p.ThoiDiemHoanThanhKham >= tuNgay) && (denNgay == null || p.ThoiDiemHoanThanhKham <= denNgayFormat));
            }

            query = query.ApplyLike(queryInfo.SearchTerms,
                                                g => g.HoTen,
                                                g => g.BacSiThucHien,
                                                g => g.MaYeuCauTiepNhan);

            //query = query.ApplyLike(queryInfo.SearchTerms,
            //                                    g => g.HoTen,
            //                                    g => g.NamSinhDisplay,
            //                                    g => g.NamSinhRemoveDiacritics,
            //                                    g => g.HoTenRemoveDiacritics,
            //                                    g => g.DiaChiRemoveDiacritics,
            //                                    g => g.TrieuChungTiepNhanRemoveDiacritics,
            //                                    g => g.DoiTuongRemoveDiacritics,
            //                                    g => g.DiaChi,
            //                                    g => g.BacSiThucHienRemoveDiacritics,
            //                                    g => g.TrieuChungTiepNhan,
            //                                    g => g.MaYeuCauTiepNhan);

            //BVHD-3368
            if (queryString.IsKhamDoanTatCa == true)
            {
                //queryTask = queryTask.GroupBy(x => x.YeuCauTiepNhanId).Select(x => x.First()).ToArray();
                query = query.GroupBy(x => x.YeuCauTiepNhanId)
                    .Select(s => new DanhSachDaKhamGridVo
                    {
                        Id = s.FirstOrDefault().Id,
                        PhongBenhVienId = s.FirstOrDefault().PhongBenhVienId,
                        NoiTiepNhanId = s.FirstOrDefault().NoiTiepNhanId,
                        MaYeuCauTiepNhan = s.FirstOrDefault().MaYeuCauTiepNhan,
                        BenhNhanId = s.FirstOrDefault().BenhNhanId,
                        HoTen = s.FirstOrDefault().HoTen,
                        NamSinh = s.FirstOrDefault().NamSinh,
                        NamSinhDisplay = s.FirstOrDefault().NamSinhDisplay,
                        DiaChi = s.FirstOrDefault().DiaChi,
                        YeuCauTiepNhanId = s.FirstOrDefault().YeuCauTiepNhanId,
                        ThoiDiemTiepNhanDisplay = s.FirstOrDefault().ThoiDiemTiepNhanDisplay,
                        ThoiDiemTiepNhan = s.FirstOrDefault().ThoiDiemTiepNhan,
                        //BacSiThucHien = string.Join(", ", s.Select(x => x.BacSiKetLuan).Where(x => !string.IsNullOrEmpty(x)).Distinct()),
                        BacSiThucHien = s.FirstOrDefault().BacSiThucHien,
                        BacSiThucHienId = s.FirstOrDefault().BacSiThucHienId,
                        BacSiKetLuan = s.FirstOrDefault().BacSiKetLuan,
                        TrieuChungTiepNhan = s.FirstOrDefault().TrieuChungTiepNhan,
                        DoiTuong = s.FirstOrDefault().DoiTuong,
                        TrangThaiYeuCauTiepNhanDisplay = s.FirstOrDefault().TrangThaiYeuCauTiepNhanDisplay,
                        BaoHiemChiTra = s.FirstOrDefault().BaoHiemChiTra,
                        NgayThucHien = s.FirstOrDefault().NgayThucHien,
                        CachGiaiQuyet = s.FirstOrDefault().CachGiaiQuyet,
                        BenhNhanDaTaoBenhAn = s.FirstOrDefault().BenhNhanDaTaoBenhAn,
                        KhoaNhapVien = s.FirstOrDefault().KhoaNhapVien,
                        ThoiDiemHoanThanhKham = s.FirstOrDefault().ThoiDiemHoanThanhKham,
                        ThoiDiemHoanThanhKhamDisplay = s.FirstOrDefault().ThoiDiemHoanThanhKhamDisplay,
                        KhamTheoPhongYeuCauKhamBenhId = null,

                        //BVHD-3924
                        TenCongTy = s.FirstOrDefault().TenCongTy
                    });
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            if (queryTask.Length > 0)
            {
                var currentUserBSId = _userAgentHelper.GetCurrentUserId();
                var kiemTraBScoTrongDS = query.Where(c => c.BacSiThucHienId == currentUserBSId).Any();

                var dsKhamBenhs = new List<DanhSachDaKhamGridVo>();

                if (kiemTraBScoTrongDS)
                {
                    dsKhamBenhs.AddRange(queryTask.Where(c => c.BacSiThucHienId == currentUserBSId).OrderByDescending(c => c.ThoiDiemHoanThanhKham).ToList());
                    dsKhamBenhs.AddRange(queryTask.Where(c => c.BacSiThucHienId != currentUserBSId).OrderByDescending(c => c.ThoiDiemHoanThanhKham).ToList());
                    queryTask = dsKhamBenhs.ToArray();
                }

                //BVHD-3368
                //if (queryString.IsKhamDoanTatCa == true)
                //{
                //    //queryTask = queryTask.GroupBy(x => x.YeuCauTiepNhanId).Select(x => x.First()).ToArray();
                //    queryTask = queryTask.GroupBy(x => x.YeuCauTiepNhanId)
                //        .Select(s => new DanhSachDaKhamGridVo
                //        {
                //            Id = s.First().Id,
                //            PhongBenhVienId = s.First().PhongBenhVienId,
                //            NoiTiepNhanId = s.First().NoiTiepNhanId,
                //            MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                //            BenhNhanId = s.First().BenhNhanId,
                //            HoTen = s.First().HoTen,
                //            NamSinh = s.First().NamSinh,
                //            NamSinhDisplay = s.First().NamSinhDisplay,
                //            DiaChi = s.First().DiaChi,
                //            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                //            ThoiDiemTiepNhanDisplay = s.First().ThoiDiemTiepNhanDisplay,
                //            ThoiDiemTiepNhan = s.First().ThoiDiemTiepNhan,
                //            BacSiThucHien = string.Join(", ", s.Select(x => x.BacSiKetLuan).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList()),
                //            BacSiThucHienId = s.First().BacSiThucHienId,
                //            BacSiKetLuan = s.First().BacSiKetLuan,
                //            TrieuChungTiepNhan = s.First().TrieuChungTiepNhan,
                //            DoiTuong = s.First().DoiTuong,
                //            TrangThaiYeuCauTiepNhanDisplay = s.First().TrangThaiYeuCauTiepNhanDisplay,
                //            BaoHiemChiTra = s.First().BaoHiemChiTra,
                //            NgayThucHien = s.First().NgayThucHien,
                //            CachGiaiQuyet = s.First().CachGiaiQuyet,
                //            BenhNhanDaTaoBenhAn = s.First().BenhNhanDaTaoBenhAn,
                //            KhoaNhapVien = s.First().KhoaNhapVien,
                //            ThoiDiemHoanThanhKham = s.First().ThoiDiemHoanThanhKham,
                //            ThoiDiemHoanThanhKhamDisplay = s.First().ThoiDiemHoanThanhKhamDisplay,
                //            KhamTheoPhongYeuCauKhamBenhId = null
                //        }).ToArray();
                //}
            }

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask.Result };
        }



        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachDaKham(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<DanhSachDaKhamGridVo>(queryInfo.AdditionalSearchString);
            //todo: need improve
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                //.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham 
                //            && o.NoiThucHienId == queryString.NoiTiepNhanId && o.YeuCauNhapViens.Count() == 0
                //            && ((queryString.LaKhamDoan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                //                || (!queryString.LaKhamDoan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)))
                .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                            && (queryString.IsKhamDoanTatCa == true || o.NoiThucHienId == queryString.NoiTiepNhanId)
                            && ((queryString.LaKhamDoan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                                || (!queryString.LaKhamDoan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))
                            && (queryString.IsKhamDoanTatCa != true || ((queryString.CongTyId == null || o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyId))
                                && (queryString.HopDongId == null || o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongId)))
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                    PhongBenhVienId = s.NoiThucHienId,
                    NoiTiepNhanId = s.YeuCauTiepNhan.NoiTiepNhanId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    NamSinhDisplay = s.YeuCauTiepNhan.NamSinh.ToString(),
                    DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemChiDinh,
                    BacSiThucHien = s.BacSiThucHien.User.HoTen,
                    TrieuChungTiepNhan = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    DoiTuong = SetSoPhanTramHuongBHYT(s.YeuCauTiepNhan.BHYTMaSoThe),
                    TrangThaiYeuCauTiepNhanDisplay = s.TrangThai.GetDescription(),
                    BaoHiemChiTra = s.YeuCauTiepNhan.CoBHYT == true ? true : false,
                    ThoiDiemHoanThanhKham = s.ThoiDiemHoanThanh
                });
            //if ((queryString.NoiTiepNhanId) != null)
            //{
            //    query = query.Where(p => p.PhongBenhVienId == queryString.NoiTiepNhanId);
            //}
            if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            {
                DateTime denNgay;
                queryString.FromDate.TryParseExactCustom(out var tuNgay);
                //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                //    out var tuNgay);
                if (string.IsNullOrEmpty(queryString.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryString.ToDate.TryParseExactCustom(out denNgay);
                    //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                }
                var denNgayFormat = denNgay.AddSeconds(59).AddMilliseconds(999);
                query = query.Where(p => p.ThoiDiemHoanThanhKham != null && (tuNgay == null || p.ThoiDiemHoanThanhKham >= tuNgay) && (denNgay == null || p.ThoiDiemHoanThanhKham <= denNgayFormat));
            }
            query = query.ApplyLike(queryInfo.SearchTerms,
                                                g => g.HoTen,
                                                g => g.BacSiThucHien,
                                                g => g.MaYeuCauTiepNhan);


            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);

            //BVHD-3368
            var queryTask = await query.ToArrayAsync();
            if (queryTask.Length > 0)
            {
                if (queryString.IsKhamDoanTatCa == true)
                {
                    queryTask = queryTask.GroupBy(x => x.YeuCauTiepNhanId).Select(x => x.First()).ToArray();
                }
            }
            return new GridDataSource { TotalRowCount = queryTask.Length }; //countTask.Result
        }
        public string SetSoPhanTramHuongBHYT(string maThe)
        {
            if (string.IsNullOrEmpty(maThe))
                return "Viện phí";
            var maTheArray = maThe.Substring(2, 1);
            var soPhanTramHuongBHYT = 0;
            if (maTheArray == "1" || maTheArray == "2")
            {
                soPhanTramHuongBHYT = 100;
            }
            else if (maTheArray == "3")
            {
                soPhanTramHuongBHYT = 95;
            }
            else if (maTheArray == "4")
            {
                soPhanTramHuongBHYT = 80;
            }
            else
            {
                soPhanTramHuongBHYT = 100;
            }
            var result = "BHYT (" + soPhanTramHuongBHYT.ToString() + "%)";
            return result;
        }
        public async Task<string> UpdateBenhNhanCanKhamLai(long yckbId, long phongbenhvienId)
        {
            var yeuCauKhamBenhCanKhamLai = await BaseRepository.GetByIdAsync(yckbId, s => s.Include(p => p.PhongBenhVienHangDois));
            var trangThaiYeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking.Where(x => x.Id == yeuCauKhamBenhCanKhamLai.YeuCauTiepNhanId && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien);
            if (!trangThaiYeuCauTiepNhan.Any())
            {
                return "Người bệnh này không còn khám";
            }
            else if (yeuCauKhamBenhCanKhamLai.BaoHiemChiTra != null)
            {
                var errMessage = _localizationService.GetResource("KhamBenh.KhamLai.DaDuyetBaoHiem");
                throw new Exception(_localizationService.GetResource(errMessage));
            }
            else
            {
                // trạng thái trước đó . trạng thái khi hoàn thành khám có 3 loại trạng thái {'đã khám','đang làm chỉ định','đang khám'}
                var trangThaiBenhNhanTruocDo = _yeuCauKhamBenhLichSuTrangThaiRepository.TableNoTracking.Where(x => x.YeuCauKhamBenhId == yckbId && x.TrangThaiYeuCauKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).Select(s => new YeuCauKhamBenhLichSuTrangThaiGridVo
                {
                    Id = s.Id,
                    TrangThaiYeuCauKhamBenh = s.TrangThaiYeuCauKhamBenh
                }).OrderByDescending(o => o.Id);
                if (trangThaiBenhNhanTruocDo.Any())
                {
                    yeuCauKhamBenhCanKhamLai.TrangThai = trangThaiBenhNhanTruocDo.First().TrangThaiYeuCauKhamBenh;
                }
                var yckbLichSuTrangThai = new YeuCauKhamBenhLichSuTrangThai();
                yckbLichSuTrangThai.YeuCauKhamBenhId = yckbId;
                yckbLichSuTrangThai.TrangThaiYeuCauKhamBenh = trangThaiBenhNhanTruocDo.First().TrangThaiYeuCauKhamBenh;
                yckbLichSuTrangThai.MoTa = trangThaiBenhNhanTruocDo.First().TrangThaiYeuCauKhamBenh.GetDescription();
                yeuCauKhamBenhCanKhamLai.YeuCauKhamBenhLichSuTrangThais.Add(yckbLichSuTrangThai);

                var maxSoThuTuHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking.Select(x => x.SoThuTu).Max();
                var phongBenhVienHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauKhamBenhCanKhamLai.YeuCauTiepNhanId && x.YeuCauKhamBenhId == yckbId);
                if (!phongBenhVienHangDoi.Any())
                {
                    var phongBenhVienHangDoiEntity = new PhongBenhVienHangDoi();
                    phongBenhVienHangDoiEntity.PhongBenhVienId = phongbenhvienId;
                    phongBenhVienHangDoiEntity.YeuCauTiepNhanId = yeuCauKhamBenhCanKhamLai.YeuCauTiepNhanId;
                    phongBenhVienHangDoiEntity.YeuCauKhamBenhId = yckbId;
                    phongBenhVienHangDoiEntity.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                    phongBenhVienHangDoiEntity.SoThuTu = maxSoThuTuHangDoi + 1;

                    yeuCauKhamBenhCanKhamLai.PhongBenhVienHangDois.Add(phongBenhVienHangDoiEntity);
                    try
                    {
                        LogManager.GetCurrentClassLogger().Info($"UpdateBenhNhanCanKhamLai PhongBenhVienId{phongBenhVienHangDoiEntity.PhongBenhVienId}, YeuCauKhamBenhId{phongBenhVienHangDoiEntity.YeuCauKhamBenhId}");
                    }
                    catch (Exception e)
                    {

                    }

                }
                await BaseRepository.UpdateAsync(yeuCauKhamBenhCanKhamLai);
            }

            return string.Empty;
        }

        #region [BVHD-3368]
        public async Task CapNhatKhamLaiKhamSucKhoeAsync(KhamLaiKhamDoanVo khamLaiVo)
        {
            var yeuCauTiepNhanKhamSucKhoe = _yeuCauTiepNhanRepository.GetById(khamLaiVo.YeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais));

            if (yeuCauTiepNhanKhamSucKhoe.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
            {
                throw new Exception(_localizationService.GetResource("MoLaiKhamSucKhoe.YeuCauTiepNhan.DaHuy"));
            }

            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            // mở lại YCTN
            yeuCauTiepNhanKhamSucKhoe.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien;

            // mở lại YCKB đã khám
            var maxSoThuTu = _phongBenhVienHangDoiRepository.TableNoTracking.Select(x => x.SoThuTu).Max() + 1;
            foreach (var yeuCauKhamBenh in yeuCauTiepNhanKhamSucKhoe.YeuCauKhamBenhs.Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                                                                                && (khamLaiVo.YeuCauKhamBenhId == null || x.Id == khamLaiVo.YeuCauKhamBenhId)))
            {
                if (yeuCauKhamBenh.BaoHiemChiTra != null)
                {
                    var errMessage = _localizationService.GetResource("KhamBenh.KhamLai.DaDuyetBaoHiem");
                    throw new Exception(_localizationService.GetResource(errMessage));
                }
                var trangThaiCuoiCung =
                    yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais
                        .Where(x => x.TrangThaiYeuCauKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                    && x.TrangThaiYeuCauKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.TrangThaiYeuCauKhamBenh != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                yeuCauKhamBenh.TrangThai = trangThaiCuoiCung?.TrangThaiYeuCauKhamBenh ?? Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
                yeuCauKhamBenh.ThoiDiemHoanThanh = null;

                var lichSuTrangThai = new YeuCauKhamBenhLichSuTrangThai()
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                    MoTa = "Mở lại khám sức khỏe"
                };
                yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuTrangThai);

                if (!yeuCauKhamBenh.PhongBenhVienHangDois.Any())
                {
                    var newHangDoi = new PhongBenhVienHangDoi()
                    {
                        PhongBenhVienId = yeuCauKhamBenh.NoiThucHienId ?? phongHienTaiId,
                        YeuCauTiepNhanId = yeuCauKhamBenh.YeuCauTiepNhanId,
                        YeuCauKhamBenhId = yeuCauKhamBenh.Id,
                        TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham,
                        SoThuTu = maxSoThuTu++
                    };
                    yeuCauKhamBenh.PhongBenhVienHangDois.Add(newHangDoi);
                    try
                    {
                        LogManager.GetCurrentClassLogger().Info($"UpdateBenhNhanCanKhamLai PhongBenhVienId{newHangDoi.PhongBenhVienId}, YeuCauKhamBenhId{newHangDoi.YeuCauKhamBenhId}");
                    }
                    catch (Exception e)
                    {

                    }
                }

            }

            _yeuCauTiepNhanRepository.Context.SaveChanges();
        }


        #endregion
        #region [BVHD-3514]
        public async Task<DanhSachChoKhamGridVo> GetYeuCauKhamBenh(long yckbId, long yctnId)
        {

            var query = BaseRepository.TableNoTracking
                 .Where(yckb => yckb.YeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                 && (yckb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham || yckb.TrangThai == EnumTrangThaiYeuCauKhamBenh.HuyKham)
                 && yckb.Id == yckbId && yckb.YeuCauTiepNhanId == yctnId)
                .Select(s => new DanhSachChoKhamGridVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                    ThoiDiemTiepNhanDisplay = s.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    TrieuChungTiepNhan = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    BHYTMaSoThe = s.YeuCauTiepNhan.BHYTMaSoThe,
                    DoiTuong = s.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + s.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    TrangThaiYeuCauKhamBenh = s.TrangThai,
                    TrangThaiYeuCauKhamBenhSearch = s.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham ? "Đã khám" : "Hủy khám",
                    TenDichVu = s.TenDichVu,
                    CoDonThuocBHYT = s.YeuCauKhamBenhDonThuocs.Any(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && p.YeuCauKhamBenhDonThuocChiTiets.Any()),
                    CoDonThuocKhongBHYT = s.YeuCauKhamBenhDonThuocs.Any(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.YeuCauKhamBenhDonThuocChiTiets.Any()),
                    CoDonVatTu = s.YeuCauKhamBenhDonVTYTs.Any(),
                    TenNhanVienTiepNhan = s.YeuCauTiepNhan.NhanVienTiepNhan.User.HoTen,
                    CachGiaiQuyet = s.CachGiaiQuyet,
                    ChuanDoan = s.GhiChuICDChinh,
                    BSKham = s.BacSiKetLuanId != null ? s.BacSiKetLuan.User.HoTen : null,
                    ThoiDiemThucHien = s.ThoiDiemThucHien,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen

                });
            return query.FirstOrDefault();
        }
        #endregion[BVHD-3514]
        #region BVHD-3698
        public async Task<List<long>> GetYeuCauKhamBenhKhamThaiIds( long yctnId)
        {
            var lstIds = BaseRepository.TableNoTracking.Where(d => d.YeuCauTiepNhanId == yctnId && 
                                                              d.DuongThaiDenNgay != null && 
                                                              d.DuongThaiTuNgay != null)
                .Select(d => d.Id).ToList();
            return lstIds;
        }
        public async Task<List<InfoYeuCauKhamBenhVo>> GetYeuCauKhamBenhNghiHuongBHXH(long yctnId)
        {
            var lstIds = BaseRepository.TableNoTracking.Where(d => d.YeuCauTiepNhanId == yctnId &&
                                                              d.NghiHuongBHXHTuNgay != null &&
                                                              d.NghiHuongBHXHDenNgay != null &&
                                                              d.NghiHuongBHXHNgayIn != null &&
                                                              d.NghiHuongBHXHNguoiInId != null 
                                                             )
                .Select(d => new InfoYeuCauKhamBenhVo
                { 
                    Id = d.Id ,
                    NghiHuongBHXHTuNgay = d.NghiHuongBHXHTuNgay,
                    NghiHuongBHXHDenNgay = d.NghiHuongBHXHDenNgay,
                    PhuongPhapDieuTriNghiHuongBHYT = d.PhuongPhapDieuTriNghiHuongBHYT,
                    TenICDChinhNghiHuongBHYT = d.TenICDChinhNghiHuongBHYT,
                    ICDChinhNghiHuongBHYT = d.ICDChinhNghiHuongBHYT

                }).ToList();
            return lstIds;
        }
        #endregion
    }
}
