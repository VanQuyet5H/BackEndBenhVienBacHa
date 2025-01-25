using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        #region Grid
        public async Task<GridDataSource> GetDataForGridTongHopYLenhAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TongHopYLenhTimKiemNangCaoVo();
            DateTime ngayYLenh = new DateTime();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TongHopYLenhTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.strNgayYLenh))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.strNgayYLenh, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayYLenh);
                }
            }

            var query = _noiTruBenhAnRepository.TableNoTracking
                .Where(x => x.ThoiDiemNhapVien.Date <= ngayYLenh.Date
                            && (x.ThoiDiemRaVien == null || x.ThoiDiemRaVien.Value.Date >= ngayYLenh.Date)
                            && (timKiemNangCaoObj.KhoaId == null || x.NoiTruKhoaPhongDieuTris.Any(a => a.KhoaPhongChuyenDenId == timKiemNangCaoObj.KhoaId
                                                                               && (a.ThoiDiemVaoKhoa.Date <= ngayYLenh.Date)
                                                                               && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= ngayYLenh.Date)))
                            && (timKiemNangCaoObj.PhongId == null || x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
                                                                    .Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                              && a.GiuongBenh.PhongBenhVienId == timKiemNangCaoObj.PhongId
                                                                              && a.ThoiDiemBatDauSuDung != null
                                                                              && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                              && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)))
                            && (timKiemNangCaoObj.ChuaHoanThanh != true 
                                //|| !x.NoiTruPhieuDieuTriChiTietYLenhs.Any() 
                                || x.NoiTruPhieuDieuTriChiTietYLenhs.Any(a => a.NgayDieuTri.Date == ngayYLenh.Date && a.XacNhanThucHien != true))
                            )
                .Select(item => new TongHopYLenhGridVo()
                {
                    Id = item.Id,
                    Giuong = item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan 
                                                                                      && a.ThoiDiemBatDauSuDung != null
                                                                                                   && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                   && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
                        item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                   && a.ThoiDiemBatDauSuDung != null
                                                                                                && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.Ten).FirstOrDefault() : null,
                    Phong = item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                     && a.ThoiDiemBatDauSuDung != null
                                                                                                  && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                  && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
                        item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                   && a.ThoiDiemBatDauSuDung != null
                                                                                                && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault() : "Chưa cấp giường",
                    MaBenhAn = item.SoBenhAn,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    NgayYLenh = ngayYLenh.Date,//item.NgayDieuTri,
                    BacSi = item.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
                        item.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.BacSi.User.HoTen).FirstOrDefault() : null,
                    YTa = item.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
                        item.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.DieuDuong.User.HoTen).FirstOrDefault() : null,

                    SoLuongYLenh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(x => x.NgayDieuTri.Date == ngayYLenh.Date),
                    SoLuongYLenhHoanThanh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(a => a.XacNhanThucHien == true && a.NgayDieuTri.Date == ngayYLenh.Date)
                })
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.Giuong, x => x.MaBenhAn, x => x.MaBenhNhan, x => x.HoTen, x => x.BacSi, x => x.YTa);

            //var query = _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Where(x => x.NgayDieuTri.Date == ngayYLenh.Date
            //                && x.NoiTruBenhAn.NoiTruEkipDieuTris.Any(a => (a.TuNgay.Date <= ngayYLenh.Date)
            //                                                           && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date))
            //                && x.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any(a => (timKiemNangCaoObj.KhoaId == null || a.KhoaPhongChuyenDenId == timKiemNangCaoObj.KhoaId)
            //                                                                   && (a.ThoiDiemVaoKhoa.Date <= ngayYLenh.Date)
            //                                                                   && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= ngayYLenh.Date))
            //                && x.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
            //                                                        .Any(a => (timKiemNangCaoObj.PhongId == null || a.GiuongBenh.PhongBenhVienId == timKiemNangCaoObj.PhongId) 
            //                                                                  && a.ThoiDiemBatDauSuDung != null
            //                                                                  && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                  && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date))
            //                && (timKiemNangCaoObj.ChuaHoanThanh != true || !x.NoiTruPhieuDieuTriChiTietYLenhs.Any() || x.NoiTruPhieuDieuTriChiTietYLenhs.Any(a => a.XacNhanThucHien != true))
            //                )
            //    .Select(item => new TongHopYLenhGridVo()
            //    {
            //        Id = item.Id,
            //        Giuong = item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                       && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                       && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.Ten).FirstOrDefault() : null,
            //        Phong = item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                      && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                      && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault() : null,
            //        MaBenhAn = item.NoiTruBenhAn.SoBenhAn,
            //        MaBenhNhan = item.NoiTruBenhAn.BenhNhan.MaBN,
            //        HoTen = item.NoiTruBenhAn.YeuCauTiepNhan.HoTen,
            //        NgaySinh = item.NoiTruBenhAn.YeuCauTiepNhan.NgaySinh,
            //        ThangSinh = item.NoiTruBenhAn.YeuCauTiepNhan.ThangSinh,
            //        NamSinh = item.NoiTruBenhAn.YeuCauTiepNhan.NamSinh,
            //        GioiTinh = item.NoiTruBenhAn.YeuCauTiepNhan.GioiTinh.GetDescription(),
            //        NgayYLenh = item.NgayDieuTri,
            //        BacSi = item.NoiTruBenhAn.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.BacSi.User.HoTen).FirstOrDefault() : null,
            //        YTa = item.NoiTruBenhAn.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.DieuDuong.User.HoTen).FirstOrDefault() : null,

            //        SoLuongYLenh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(),
            //        SoLuongYLenhHoanThanh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(a => a.XacNhanThucHien == true)
            //    })
            //    .ApplyLike(timKiemNangCaoObj.SearchString, x => x.Giuong, x => x.MaBenhAn, x => x.MaBenhNhan, x => x.HoTen, x => x.BacSi, x => x.YTa);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridTongHopYLenhAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new TongHopYLenhTimKiemNangCaoVo();
            DateTime ngayYLenh = new DateTime();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TongHopYLenhTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.strNgayYLenh))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.strNgayYLenh, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayYLenh);
                }
            }

            var query = _noiTruBenhAnRepository.TableNoTracking
                .Where(x => x.ThoiDiemNhapVien.Date <= ngayYLenh.Date
                            && (x.ThoiDiemRaVien == null || x.ThoiDiemRaVien.Value.Date >= ngayYLenh.Date)
                            && (timKiemNangCaoObj.KhoaId == null || x.NoiTruKhoaPhongDieuTris.Any(a => a.KhoaPhongChuyenDenId == timKiemNangCaoObj.KhoaId
                                                                                                       && (a.ThoiDiemVaoKhoa.Date <= ngayYLenh.Date)
                                                                                                       && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= ngayYLenh.Date)))
                            && (timKiemNangCaoObj.PhongId == null || x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
                                    .Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                              && a.GiuongBenh.PhongBenhVienId == timKiemNangCaoObj.PhongId
                                              && a.ThoiDiemBatDauSuDung != null
                                              && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                              && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)))
                            && (timKiemNangCaoObj.ChuaHoanThanh != true 
                                //|| !x.NoiTruPhieuDieuTriChiTietYLenhs.Any() 
                                || x.NoiTruPhieuDieuTriChiTietYLenhs.Any(a => a.NgayDieuTri.Date == ngayYLenh.Date && a.XacNhanThucHien != true))
                            )
                .Select(item => new TongHopYLenhGridVo()
                {
                    Id = item.Id,
                    Giuong = item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                      && a.ThoiDiemBatDauSuDung != null
                                                                                                   && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                   && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
                        item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                   && a.ThoiDiemBatDauSuDung != null
                                                                                                && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.Ten).FirstOrDefault() : null,
                    Phong = item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                     && a.ThoiDiemBatDauSuDung != null
                                                                                                  && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                  && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
                        item.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                   && a.ThoiDiemBatDauSuDung != null
                                                                                                && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
                                                                                                && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault() : "Chưa cấp giường",
                    MaBenhAn = item.SoBenhAn,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    NgayYLenh = ngayYLenh.Date,//item.NgayDieuTri,
                    BacSi = item.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
                        item.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.BacSi.User.HoTen).FirstOrDefault() : null,
                    YTa = item.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
                        item.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.DieuDuong.User.HoTen).FirstOrDefault() : null,

                    SoLuongYLenh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(x => x.NgayDieuTri.Date == ngayYLenh.Date),
                    SoLuongYLenhHoanThanh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(a => a.XacNhanThucHien == true && a.NgayDieuTri.Date == ngayYLenh.Date)
                })
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.Giuong, x => x.MaBenhAn, x => x.MaBenhNhan, x => x.HoTen, x => x.BacSi, x => x.YTa);

            //var query = _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Where(x => x.NgayDieuTri.Date == ngayYLenh.Date
            //                && x.NoiTruBenhAn.NoiTruEkipDieuTris.Any(a => (a.TuNgay.Date <= ngayYLenh.Date)
            //                                                           && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date))
            //                && x.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any(a => (timKiemNangCaoObj.KhoaId == null || a.KhoaPhongChuyenDenId == timKiemNangCaoObj.KhoaId)
            //                                                                   && (a.ThoiDiemVaoKhoa.Date <= ngayYLenh.Date)
            //                                                                   && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= ngayYLenh.Date))
            //                && x.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
            //                                                        .Any(a => (timKiemNangCaoObj.PhongId == null || a.GiuongBenh.PhongBenhVienId == timKiemNangCaoObj.PhongId)
            //                                                                  && a.ThoiDiemBatDauSuDung != null
            //                                                                  && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                  && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date))
            //                && (timKiemNangCaoObj.ChuaHoanThanh != true || !x.NoiTruPhieuDieuTriChiTietYLenhs.Any() || x.NoiTruPhieuDieuTriChiTietYLenhs.Any(a => a.XacNhanThucHien != true))
            //                )
            //    .Select(item => new TongHopYLenhGridVo()
            //    {
            //        Id = item.Id,
            //        Giuong = item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                       && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                       && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.Ten).FirstOrDefault() : null,
            //        Phong = item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                      && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                      && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= ngayYLenh.Date
            //                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= ngayYLenh.Date)).Select(a => a.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault() : null,
            //        MaBenhAn = item.NoiTruBenhAn.SoBenhAn,
            //        MaBenhNhan = item.NoiTruBenhAn.BenhNhan.MaBN,
            //        HoTen = item.NoiTruBenhAn.YeuCauTiepNhan.HoTen,
            //        NgaySinh = item.NoiTruBenhAn.YeuCauTiepNhan.NgaySinh,
            //        ThangSinh = item.NoiTruBenhAn.YeuCauTiepNhan.ThangSinh,
            //        NamSinh = item.NoiTruBenhAn.YeuCauTiepNhan.NamSinh,
            //        GioiTinh = item.NoiTruBenhAn.YeuCauTiepNhan.GioiTinh.GetDescription(),
            //        NgayYLenh = item.NgayDieuTri,
            //        BacSi = item.NoiTruBenhAn.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.BacSi.User.HoTen).FirstOrDefault() : null,
            //        YTa = item.NoiTruBenhAn.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)) ?
            //            item.NoiTruBenhAn.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= ngayYLenh.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= ngayYLenh.Date)).Select(a => a.DieuDuong.User.HoTen).FirstOrDefault() : null,

            //        SoLuongYLenh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(),
            //        SoLuongYLenhHoanThanh = item.NoiTruPhieuDieuTriChiTietYLenhs.Count(a => a.XacNhanThucHien == true)
            //    })
            //    .ApplyLike(timKiemNangCaoObj.SearchString, x => x.Giuong, x => x.MaBenhAn, x => x.MaBenhNhan, x => x.HoTen, x => x.BacSi, x => x.YTa);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        #endregion

        #region Get data

        public async Task<TongHopYLenhThongTinHanhChinhVo> GetTongHopYLenhThongTinHanhChinhAsync(ChiTietYLenhQueryInfoVo queryInfo)
        {
            var benhAn = _noiTruBenhAnRepository.GetById(queryInfo.NoiTruBenhAnId,
                    a => a.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
                                .Include(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)
                                //.Include(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.KhoaPhongDieuTri)
                                .Include(x => x.KhoaPhongNhapVien)
                                .Include(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    
                                //BVHD-3800
                                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                    );

            var thongTinHanhChinh = new TongHopYLenhThongTinHanhChinhVo()
            {
                MaTiepNhan = benhAn.YeuCauTiepNhan.MaYeuCauTiepNhan,
                BenhNhanId = benhAn.BenhNhanId,
                MaBenhNhan = benhAn.YeuCauTiepNhan.BenhNhan.MaBN,
                HoTen = benhAn.YeuCauTiepNhan.HoTen,
                NgaySinh = benhAn.YeuCauTiepNhan.NgaySinh,
                ThangSinh = benhAn.YeuCauTiepNhan.ThangSinh,
                NamSinh = benhAn.YeuCauTiepNhan.NamSinh,
                TenGioiTinh = benhAn.YeuCauTiepNhan.GioiTinh.GetDescription(),
                DiaChi = benhAn.YeuCauTiepNhan.DiaChiDayDu,
                NgheNghiep = benhAn.YeuCauTiepNhan.NgheNghiep?.Ten,
                SoBenhAn = benhAn.SoBenhAn,
                LoaiBenhAn = benhAn.LoaiBenhAn.GetDescription(),
                Khoa =
                    //benhAn.NoiTruPhieuDieuTris.Any(x => x.NgayDieuTri.Date <= DateTime.Now.Date)
                    //? benhAn.NoiTruPhieuDieuTris.Where(x => x.NgayDieuTri.Date <= DateTime.Now.Date).OrderByDescending(x => x.NgayDieuTri).First().KhoaPhongDieuTri?.Ten
                    //: benhAn.KhoaPhongNhapVien?.Ten,
                    benhAn.NoiTruKhoaPhongDieuTris.Any(a => a.ThoiDiemVaoKhoa.Date <= DateTime.Now.Date && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= DateTime.Now.Date))
                        ? benhAn.NoiTruKhoaPhongDieuTris.Where(a => a.ThoiDiemVaoKhoa.Date <= DateTime.Now.Date && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= DateTime.Now.Date)).OrderByDescending(x => x.Id).Select(a => a.KhoaPhongChuyenDen.Ten).First()
                        : benhAn.KhoaPhongNhapVien?.Ten,
                BacSiDieuTri = benhAn.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= DateTime.Now.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date))
                    ? benhAn.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= DateTime.Now.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).OrderByDescending(x => x.Id).Select(a => a.BacSi.User.HoTen).First()
                    : null,

                Giuong = benhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                    && a.ThoiDiemBatDauSuDung != null
                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date))
                    ? benhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                   && a.ThoiDiemBatDauSuDung != null
                                                                                   && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                   && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date))
                                                                        .OrderByDescending(x => x.Id).Select(a => a.GiuongBenh.Ten).First() : null,
                Phong = benhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                   && a.ThoiDiemBatDauSuDung != null
                                                                                   && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                   && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date))
                    ? benhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                   && a.ThoiDiemBatDauSuDung != null
                                                                                   && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                   && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date))
                                                                        .OrderByDescending(x => x.Id).Select(a => a.GiuongBenh.PhongBenhVien.Ten).First() : null,
                DoiTuong = benhAn.YeuCauTiepNhan.CoBHYT == true ? "BHYT" : "Viện phí",
                NgayDieuTri = queryInfo.NgayDieuTri,

                //BVHD-3800
                LaCapCuu = benhAn.YeuCauTiepNhan.LaCapCuu ?? benhAn.YeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu,

                //BVHD-3941
                YeuCauTiepNhanId = benhAn.YeuCauTiepNhan.Id,
                CoBaoHiemTuNhan = benhAn.YeuCauTiepNhan.CoBHTN
            };
            //var phieuDieuTri = await _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Where(x => x.Id == phieuDieuTriId)
            //    .Select(item => new TongHopYLenhThongTinHanhChinhVo()
            //    {
            //        MaTiepNhan = item.NoiTruBenhAn.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //        BenhNhanId = item.NoiTruBenhAn.BenhNhanId,
            //        MaBenhNhan = item.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.MaBN,
            //        HoTen = item.NoiTruBenhAn.YeuCauTiepNhan.HoTen,
            //        NgaySinh = item.NoiTruBenhAn.YeuCauTiepNhan.NgaySinh,
            //        ThangSinh = item.NoiTruBenhAn.YeuCauTiepNhan.ThangSinh,
            //        NamSinh = item.NoiTruBenhAn.YeuCauTiepNhan.NamSinh,
            //        TenGioiTinh = item.NoiTruBenhAn.YeuCauTiepNhan.GioiTinh.GetDescription(),
            //        DiaChi = item.NoiTruBenhAn.YeuCauTiepNhan.DiaChiDayDu,
            //        NgheNghiep = item.NoiTruBenhAn.YeuCauTiepNhan.NgheNghiep.Ten,
            //        SoBenhAn = item.NoiTruBenhAn.SoBenhAn,
            //        LoaiBenhAn = item.NoiTruBenhAn.LoaiBenhAn.GetDescription(),
            //        Khoa = item.KhoaPhongDieuTri.Ten,
            //        BacSiDieuTri = item.NoiTruBenhAn.NoiTruEkipDieuTris.Any(a => a.TuNgay.Date <= item.NgayDieuTri.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= item.NgayDieuTri.Date)) ?
            //            item.NoiTruBenhAn.NoiTruEkipDieuTris.Where(a => a.TuNgay.Date <= item.NgayDieuTri.Date && (a.DenNgay == null || a.DenNgay.Value.Date >= item.NgayDieuTri.Date)).Select(a => a.BacSi.User.HoTen).FirstOrDefault() : null,
            //        Giuong = item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                       && a.ThoiDiemBatDauSuDung.Value.Date <= item.NgayDieuTri.Date
            //                                                                                       && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= item.NgayDieuTri.Date)) ?
            //            item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= item.NgayDieuTri.Date
            //                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= item.NgayDieuTri.Date)).Select(a => a.GiuongBenh.Ten).FirstOrDefault() : null,
            //        Phong = item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                      && a.ThoiDiemBatDauSuDung.Value.Date <= item.NgayDieuTri.Date
            //                                                                                      && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= item.NgayDieuTri.Date)) ?
            //            item.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= item.NgayDieuTri.Date
            //                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= item.NgayDieuTri.Date)).Select(a => a.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault() : null,
            //        DoiTuong = item.NoiTruBenhAn.YeuCauTiepNhan.CoBHYT == true ? "BHYT" : "Viện phí",
            //        NgayDieuTri = item.NgayDieuTri
            //    })
            //    .FirstOrDefaultAsync();
            return thongTinHanhChinh;
        }

        public async Task<TongHopYLenhPhieuDieuTriVo> GetThongTinChiTietYLenhPhieuDieuTriAsync(ChiTietYLenhQueryInfoVo queryInfo)
        {
            // get tổng hợp y lệnh từ tất cả các nguồn
            XuLyTongHopYLenh(queryInfo.NoiTruBenhAnId, queryInfo.NgayDieuTri);

            //var phieuDieuTri = await _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Include(x => x.NoiTruBenhAn)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienChiDinh).ThenInclude(z => z.User)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienXacNhanThucHien).ThenInclude(z => z.User)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienCapNhat).ThenInclude(z => z.User)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.YeuCauDichVuKyThuat)
            //    .Where(x => x.Id == phieuDieuTriId)
            //    .FirstOrDefaultAsync();

            var noiTruBenhAn = _noiTruBenhAnRepository.GetById(queryInfo.NoiTruBenhAnId,
                a => a.Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
                            .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienChiDinh).ThenInclude(z => z.User)
                            .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienXacNhanThucHien).ThenInclude(z => z.User)
                            .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienCapNhat).ThenInclude(z => z.User)
                            .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.YeuCauDichVuKyThuat));

            var phieuDieuTriVo = new TongHopYLenhPhieuDieuTriVo()
            {
                NgayYLenh = queryInfo.NgayDieuTri,
                //NoiTruPhieuDieuTriId = phieuDieuTri.Id,
                NoiTruBenhAnId = noiTruBenhAn.Id,
                IsBenhAnDaKetThuc = noiTruBenhAn.ThoiDiemRaVien != null,
                IsNgayDieuTriKhongHopLe = queryInfo.NgayDieuTri < noiTruBenhAn.ThoiDiemNhapVien.Date
            };

            foreach (var dienBien in noiTruBenhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri == queryInfo.NgayDieuTri))
            {
                var dienBienDieuTri = new TongHopYLenhDienBienVo()
                {
                    Id = dienBien.Id,
                    DienBien = dienBien.MoTaDienBien,
                    GioYLenh = dienBien.GioDienBien,
                    TongHopYLenhDienBienChiTiets =
                        noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                            .Where(a => a.GioYLenh == dienBien.GioDienBien 
                                        && a.NgayDieuTri.Date == queryInfo.NgayDieuTri)
                            .Select(itemYLenh => new TongHopYLenhDienBienChiTietVo()
                            {
                                Id = itemYLenh.Id,
                                //NoiTruPhieuDieuTriId = phieuDieuTriId,
                                NoiTruBenhAnId = noiTruBenhAn.Id,
                                MoTaYLenh = itemYLenh.MoTaYLenh,
                                GioYLenh = itemYLenh.GioYLenh,
                                NhanVienChiDinhId = itemYLenh.NhanVienChiDinhId,
                                NhanVienChiDinhDisplay = itemYLenh.NhanVienChiDinh?.User.HoTen,
                                NhanVienXacNhanThucHienId = itemYLenh.NhanVienXacNhanThucHienId,
                                NhanVienXacNhanThucHienDisplay = itemYLenh.NhanVienXacNhanThucHien?.User.HoTen,
                                ThoiDiemXacNhanThucHien = itemYLenh.ThoiDiemXacNhanThucHien,
                                GioThucHien = itemYLenh.ThoiDiemXacNhanThucHien?.TimeOfDay.Hours * 3600 + itemYLenh.ThoiDiemXacNhanThucHien?.TimeOfDay.Minutes * 60 + itemYLenh.ThoiDiemXacNhanThucHien?.TimeOfDay.Seconds,
                                XacNhanThucHien = itemYLenh.XacNhanThucHien,
                                NhanVienCapNhatId = itemYLenh.NhanVienCapNhatId,
                                NhanVienCapNhatDisplay = itemYLenh.NhanVienCapNhat?.User.HoTen,
                                ThoiDiemCapNhat = itemYLenh.ThoiDiemCapNhat,
                                ThoiDiemCapNhatDisplay = itemYLenh.ThoiDiemCapNhat?.ApplyFormatDateTimeSACH(),
                                //NgayThucHien = phieuDieuTri.NgayDieuTri,
                                NgayThucHien = queryInfo.NgayDieuTri,

                                YeuCauDichVuKyThuatId = itemYLenh.YeuCauDichVuKyThuatId,
                                LaSuatAn = itemYLenh.YeuCauDichVuKyThuat != null && itemYLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SuatAn,
                                YeuCauTruyenMauId = itemYLenh.YeuCauTruyenMauId,
                                YeuCauVatTuBenhVienId = itemYLenh.YeuCauVatTuBenhVienId,
                                NoiTruChiDinhDuocPhamId = itemYLenh.NoiTruChiDinhDuocPhamId,
                                IsDisabledYeuCauDichVuKyThuat = itemYLenh.YeuCauDichVuKyThuat != null && (itemYLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ||
                                                                                                         itemYLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh ||
                                                                                                         itemYLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ||
                                                                                                         itemYLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ||

                                                                                                         // bổ sung thêm dv khám tiêm vacxin và tiêm chủng
                                                                                                         itemYLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung ||
                                                                                                         itemYLenh.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null),
                                //BVHD-3575
                                YeuCauKhamBenhId = itemYLenh.YeuCauKhamBenhId
                            }).ToList()
                };
                phieuDieuTriVo.TongHopYLenhDienBiens.Add(dienBienDieuTri);
            }

            var lstYLenhChuaCoDienBien = noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                .Where(a => a.NgayDieuTri == queryInfo.NgayDieuTri 
                            && phieuDieuTriVo.TongHopYLenhDienBiens.All(b => b.TongHopYLenhDienBienChiTiets.All(c => c.Id != a.Id))).ToList();
            if (lstYLenhChuaCoDienBien.Any())
            {
                foreach (var yLenh in lstYLenhChuaCoDienBien)
                {
                    //TimeSpan ts = TimeSpan.FromSeconds(yLenh.GioYLenh);
                    // var gioYLenhLamTron = ts.Hours;
                    var dienBienDieuTriTemp =
                        phieuDieuTriVo.TongHopYLenhDienBiens.FirstOrDefault(a => a.GioYLenh == yLenh.GioYLenh);

                    var yeLenhChiTietItem = new TongHopYLenhDienBienChiTietVo()
                    {
                        Id = yLenh.Id,
                        //NoiTruPhieuDieuTriId = phieuDieuTriId,
                        NoiTruBenhAnId = noiTruBenhAn.Id,
                        MoTaYLenh = yLenh.MoTaYLenh,
                        GioYLenh = yLenh.GioYLenh,
                        NhanVienChiDinhId = yLenh.NhanVienChiDinhId,
                        NhanVienChiDinhDisplay = yLenh.NhanVienChiDinh?.User.HoTen,
                        NhanVienXacNhanThucHienId = yLenh.NhanVienXacNhanThucHienId,
                        NhanVienXacNhanThucHienDisplay = yLenh.NhanVienXacNhanThucHien?.User.HoTen,
                        ThoiDiemXacNhanThucHien = yLenh.ThoiDiemXacNhanThucHien,
                        GioThucHien = yLenh.ThoiDiemXacNhanThucHien?.TimeOfDay.Hours * 3600 + yLenh.ThoiDiemXacNhanThucHien?.TimeOfDay.Minutes * 60 + yLenh.ThoiDiemXacNhanThucHien?.TimeOfDay.Seconds,
                        XacNhanThucHien = yLenh.XacNhanThucHien,
                        NhanVienCapNhatId = yLenh.NhanVienCapNhatId,
                        NhanVienCapNhatDisplay = yLenh.NhanVienCapNhat?.User.HoTen,
                        ThoiDiemCapNhat = yLenh.ThoiDiemCapNhat,
                        ThoiDiemCapNhatDisplay = yLenh.ThoiDiemCapNhat?.ApplyFormatDateTimeSACH(),
                        //NgayThucHien = phieuDieuTri.NgayDieuTri,
                        NgayThucHien = queryInfo.NgayDieuTri,

                        YeuCauDichVuKyThuatId = yLenh.YeuCauDichVuKyThuatId,
                        LaSuatAn = yLenh.YeuCauDichVuKyThuat != null && yLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SuatAn,
                        YeuCauTruyenMauId = yLenh.YeuCauTruyenMauId,
                        YeuCauVatTuBenhVienId = yLenh.YeuCauVatTuBenhVienId,
                        NoiTruChiDinhDuocPhamId = yLenh.NoiTruChiDinhDuocPhamId,
                        IsDisabledYeuCauDichVuKyThuat = yLenh.YeuCauDichVuKyThuat != null && (yLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ||
                                                                                              yLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh ||
                                                                                              yLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ||
                                                                                              yLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ||

                                                                                              // bổ sung thêm dv khám tiêm vacxin và tiêm chủng
                                                                                              yLenh.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung ||
                                                                                              yLenh.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null)
                    };

                    if (dienBienDieuTriTemp == null)
                    {
                        dienBienDieuTriTemp = new TongHopYLenhDienBienVo()
                        {
                            GioYLenh = yLenh.GioYLenh
                        };
                        dienBienDieuTriTemp.TongHopYLenhDienBienChiTiets.Add(yeLenhChiTietItem);
                        phieuDieuTriVo.TongHopYLenhDienBiens.Add(dienBienDieuTriTemp);
                    }
                    else
                    {
                        dienBienDieuTriTemp.TongHopYLenhDienBienChiTiets.Add(yeLenhChiTietItem);
                    }
                }
            }

            return phieuDieuTriVo;
        }
        #endregion

        #region Kiểm tra data

        public async Task KiemTraPhieuDieuTriNoiTruByNgayDieuTriAsync(long? noiTruBenhAnId, string ngayDieuTri, long? yeuCauTiepNhanId)
        {
            DateTime.TryParseExact(ngayDieuTri, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var ngayYLenh);
            var noiTruBenhAn = await _noiTruBenhAnRepository.TableNoTracking
                .Where(x => (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
                            && (yeuCauTiepNhanId == null || x.YeuCauTiepNhan.Id == yeuCauTiepNhanId)
                            && x.ThoiDiemNhapVien.Date <= ngayYLenh.Date
                            && (x.ThoiDiemRaVien == null || x.ThoiDiemRaVien.Value.Date >= ngayYLenh.Date))
                .FirstOrDefaultAsync();
            if (noiTruBenhAn == null)
            {
                throw new Exception(_localizationService.GetResource("TongHopYLenh.NgayDieuTri.NotExists"));
            }

            //if (noiTruBenhAnId == null && yeuCauTiepNhanId == null)
            //{
            //    return null;
            //}
            //DateTime.TryParseExact(ngayDieuTri, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var ngayYLenh);
            //var phieuDieuTriTheoBenhNhans = await BaseRepository.TableNoTracking
            //    .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruPhieuDieuTris)
            //    .Where(x => (phieuDieuTriId == null || x.NoiTruBenhAn.NoiTruPhieuDieuTris.Any(a => a.Id == phieuDieuTriId))
            //                && (yeuCauTiepNhanId == null || x.NoiTruBenhAn.YeuCauTiepNhan.Id == yeuCauTiepNhanId))
            //    .SelectMany(x => x.NoiTruBenhAn.NoiTruPhieuDieuTris)
            //    .ToListAsync();
            //var phieuTheoNgay = phieuDieuTriTheoBenhNhans.FirstOrDefault(x => x.NgayDieuTri.Date == ngayYLenh.Date);

            //return phieuTheoNgay?.Id;
        }


        #endregion

        #region Thêm/xóa/sửa

        public async Task XuLyLuuDienBienYLenhAsync(TongHopYLenhPhieuDieuTriVo phieuDieuTriVo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var datetimeNow = DateTime.Now;

            //var phieuDieuTri = await _noiTruPhieuDieuTriRepository.Table
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
            //    .Include(x => x.YeuCauDichVuKyThuats)
            //    .Where(x => x.Id == phieuDieuTriVo.NoiTruPhieuDieuTriId)
            //    .FirstOrDefaultAsync();

            //if (phieuDieuTri == null)
            //{
            //    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            //}

            var noiTruBenhAn = await _noiTruBenhAnRepository.GetByIdAsync(phieuDieuTriVo.NoiTruBenhAnId ?? 0,
                x => x.Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                            .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                            .Include(y => y.NoiTruPhieuDieuTris).ThenInclude(y => y.YeuCauDichVuKyThuats));

            #region xử lý xóa 
            //foreach (var yLenh in phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs)
            foreach (var yLenh in noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.NgayDieuTri.Date == phieuDieuTriVo.NgayYLenh.Value.Date))
            {
                if (!phieuDieuTriVo.TongHopYLenhDienBiens.Any(x => x.TongHopYLenhDienBienChiTiets.Any(y => y.Id == yLenh.Id)))
                {
                    yLenh.WillDelete = true;
                }
            }
            //foreach (var dienBien in phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens)
            foreach (var dienBien in noiTruBenhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == phieuDieuTriVo.NgayYLenh.Value.Date))
            {
                var yLenhTheoDienBien = noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien && x.NgayDieuTri.Date == phieuDieuTriVo.NgayYLenh.Value.Date);
                if (phieuDieuTriVo.TongHopYLenhDienBiens.All(x => x.Id != dienBien.Id) || !yLenhTheoDienBien)// || string.IsNullOrEmpty(dienBien.MoTaDienBien))
                {
                    dienBien.WillDelete = true;
                }
            }
            #endregion

            #region xử lý thêm/cập nhật

            //foreach (var yLenh in phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs)
            // kiểm tra nếu y lệnh nảo chưa có diễn biến thì tạo mới diễn biến tương ứng
            foreach (var yLenh in noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.NgayDieuTri.Date == phieuDieuTriVo.NgayYLenh.Value.Date))
            {
                //if (phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.All(x => x.GioDienBien != yLenh.GioYLenh))
                if (noiTruBenhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == yLenh.NgayDieuTri.Date).All(x => x.GioDienBien != yLenh.GioYLenh))
                {
                    var newDienBien = new NoiTruPhieuDieuTriChiTietDienBien()
                    {
                        MoTaDienBien = "",
                        GioDienBien = yLenh.GioYLenh,
                        NgayDieuTri = yLenh.NgayDieuTri.Date
                    };
                    noiTruBenhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(newDienBien);
                }
            }

            foreach (var dienBien in phieuDieuTriVo.TongHopYLenhDienBiens)
            {
                if (!dienBien.TongHopYLenhDienBienChiTiets.Any())
                {
                    continue;
                }

                //var dienBienItem = phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.FirstOrDefault(x => x.GioDienBien == dienBien.GioYLenh);
                var dienBienItem = noiTruBenhAn.NoiTruPhieuDieuTriChiTietDienBiens.FirstOrDefault(x => x.NgayDieuTri == phieuDieuTriVo.NgayYLenh.Value.Date && x.GioDienBien == dienBien.GioYLenh);
                if (dienBienItem != null) // cập nhật
                {
                    dienBienItem.MoTaDienBien = string.IsNullOrEmpty(dienBien.DienBien) ? "" : dienBien.DienBien;

                    //var lstYLenhTheoDienBien = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.GioYLenh == dienBien.GioYLenh).ToList();
                    var lstYLenhTheoDienBien = noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.NgayDieuTri == phieuDieuTriVo.NgayYLenh.Value.Date && x.GioYLenh == dienBien.GioYLenh).ToList();

                    // xử lý cập nhật y lệnh cũ
                    foreach (var item in lstYLenhTheoDienBien)
                    {
                        var yLenhCapNhatVo = phieuDieuTriVo.TongHopYLenhDienBiens
                            .SelectMany(x => x.TongHopYLenhDienBienChiTiets.Where(y => y.Id == item.Id
                                                                                       && y.GioYLenh == dienBien.GioYLenh)).FirstOrDefault();
                        // xóa y lệnh đã xóa trên UI
                        if (yLenhCapNhatVo == null)
                        {
                            item.WillDelete = true;
                        }
                        else
                        {
                            //var gioThucHien = yLenhCapNhatVo.GioThucHien != null ? phieuDieuTri.NgayDieuTri.Date + TimeSpan.FromSeconds(yLenhCapNhatVo.GioThucHien.Value) : (DateTime?)null;

                            //Cập nhật 06/06/2022: Cập nhật giờ thực hiện -> đổi từ chỉ nhập giờ thành nhập ngày giờ (Áp dụng cho trường hợp check vào xác nhận thực hiện trên grid)
                            //var gioThucHien = yLenhCapNhatVo.GioThucHien != null ? phieuDieuTriVo.NgayYLenh.Value.Date + TimeSpan.FromSeconds(yLenhCapNhatVo.GioThucHien.Value) : (DateTime?)null;
                            var gioThucHien = yLenhCapNhatVo.ThoiDiemXacNhanThucHien;

                            if (item.MoTaYLenh != yLenhCapNhatVo.MoTaYLenh
                                || item.XacNhanThucHien != yLenhCapNhatVo.XacNhanThucHien
                                || item.NhanVienXacNhanThucHienId != yLenhCapNhatVo.NhanVienXacNhanThucHienId
                                || item.ThoiDiemXacNhanThucHien != gioThucHien)
                            {
                                if (item.YeuCauDichVuKyThuat != null && !(item.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ||
                                                                        item.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh ||
                                                                        item.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ||
                                                                        item.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ||

                                                                        // bổ sung thêm dv khám tiêm vacxin và tiêm chủng
                                                                        item.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung ||
                                                                        item.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null)

                                                                        && item.XacNhanThucHien != yLenhCapNhatVo.XacNhanThucHien)
                                {
                                    if (yLenhCapNhatVo.XacNhanThucHien == true)
                                    {
                                        item.YeuCauDichVuKyThuat.TrangThai =
                                            Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                                        item.YeuCauDichVuKyThuat.NhanVienThucHienId =
                                            yLenhCapNhatVo.NhanVienXacNhanThucHienId;
                                        item.YeuCauDichVuKyThuat.ThoiDiemThucHien = gioThucHien;

                                        item.YeuCauDichVuKyThuat.NhanVienKetLuanId = item.YeuCauDichVuKyThuat.NhanVienKetLuanId ?? yLenhCapNhatVo.NhanVienXacNhanThucHienId;
                                        item.YeuCauDichVuKyThuat.ThoiDiemKetLuan = item.YeuCauDichVuKyThuat.ThoiDiemKetLuan ?? gioThucHien;
                                        item.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = item.YeuCauDichVuKyThuat.ThoiDiemHoanThanh ?? gioThucHien;
                                    }
                                    else
                                    {
                                        item.YeuCauDichVuKyThuat.TrangThai =
                                            Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                                        item.YeuCauDichVuKyThuat.NhanVienThucHienId = null;
                                        item.YeuCauDichVuKyThuat.ThoiDiemThucHien = null;

                                        item.YeuCauDichVuKyThuat.NhanVienKetLuanId = null;
                                        item.YeuCauDichVuKyThuat.ThoiDiemKetLuan = null;
                                        item.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = null;
                                    }
                                }
                                item.MoTaYLenh = yLenhCapNhatVo.MoTaYLenh;
                                item.XacNhanThucHien = yLenhCapNhatVo.XacNhanThucHien;
                                item.NhanVienXacNhanThucHienId = yLenhCapNhatVo.NhanVienXacNhanThucHienId;
                                item.ThoiDiemXacNhanThucHien = gioThucHien;

                                item.NhanVienCapNhatId = currentUserId;
                                item.ThoiDiemCapNhat = datetimeNow;
                            }
                        }
                    }

                    // xử lý thêm y lệnh mới
                    foreach (var item in dienBien.TongHopYLenhDienBienChiTiets.Where(x => x.Id == 0))
                    {
                        //var gioThucHien = item.GioThucHien != null ? phieuDieuTri.NgayDieuTri.Date + TimeSpan.FromSeconds(item.GioThucHien.Value) : (DateTime?)null;

                        //Cập nhật 06/06/2022: Cập nhật giờ thực hiện -> đổi từ chỉ nhập giờ thành nhập ngày giờ (Áp dụng cho trường hợp check vào xác nhận thực hiện trên grid)
                        //var gioThucHien = item.GioThucHien != null ? phieuDieuTriVo.NgayYLenh.Value.Date + TimeSpan.FromSeconds(item.GioThucHien.Value) : (DateTime?)null;
                        var gioThucHien = item.ThoiDiemXacNhanThucHien;

                        var yeLenhNew = new NoiTruPhieuDieuTriChiTietYLenh()
                        {
                            MoTaYLenh = item.MoTaYLenh,
                            GioYLenh = item.GioYLenh.Value,
                            NhanVienChiDinhId = item.NhanVienChiDinhId.Value,
                            NoiChiDinhId = item.NoiChiDinhId.Value,
                            NhanVienXacNhanThucHienId = item.NhanVienXacNhanThucHienId,
                            ThoiDiemXacNhanThucHien = gioThucHien, //item.ThoiDiemXacNhanThucHien,
                            XacNhanThucHien = item.XacNhanThucHien,
                            NgayDieuTri = phieuDieuTriVo.NgayYLenh.Value.Date
                        };
                        noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(yeLenhNew);
                    }
                }
                else // thêm mới
                {
                    var newDienBien = new NoiTruPhieuDieuTriChiTietDienBien()
                    {
                        MoTaDienBien = string.IsNullOrEmpty(dienBien.DienBien) ? "" : dienBien.DienBien,
                        GioDienBien = dienBien.GioYLenh,
                        NgayDieuTri = phieuDieuTriVo.NgayYLenh.Value.Date
                    };
                    noiTruBenhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(newDienBien);

                    foreach (var yLenh in dienBien.TongHopYLenhDienBienChiTiets)
                    {
                        //var gioThucHien = yLenh.GioThucHien != null ? phieuDieuTri.NgayDieuTri.Date + TimeSpan.FromSeconds(yLenh.GioThucHien.Value) : (DateTime?)null;

                        //Cập nhật 06/06/2022: Cập nhật giờ thực hiện -> đổi từ chỉ nhập giờ thành nhập ngày giờ (Áp dụng cho trường hợp check vào xác nhận thực hiện trên grid)
                        //var gioThucHien = yLenh.GioThucHien != null ? phieuDieuTriVo.NgayYLenh.Value.Date + TimeSpan.FromSeconds(yLenh.GioThucHien.Value) : (DateTime?)null;
                        var gioThucHien = yLenh.ThoiDiemXacNhanThucHien;

                        var yeLenhNew = new NoiTruPhieuDieuTriChiTietYLenh()
                        {
                            MoTaYLenh = yLenh.MoTaYLenh,
                            GioYLenh = yLenh.GioYLenh.Value,
                            NhanVienChiDinhId = yLenh.NhanVienChiDinhId.Value,
                            NoiChiDinhId = yLenh.NoiChiDinhId.Value,
                            NhanVienXacNhanThucHienId = yLenh.NhanVienXacNhanThucHienId,
                            ThoiDiemXacNhanThucHien = gioThucHien, //yLenh.ThoiDiemXacNhanThucHien,
                            XacNhanThucHien = yLenh.XacNhanThucHien,
                            NgayDieuTri = newDienBien.NgayDieuTri
                        };
                        noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(yeLenhNew);
                    }
                }
            }
            #endregion
            //await _noiTruPhieuDieuTriRepository.UpdateAsync(phieuDieuTri);
            await _noiTruBenhAnRepository.UpdateAsync(noiTruBenhAn);
        }


        #endregion

        #region In phiếu/ xuất excel
        public async Task<string> InPhieuChamSocAsync(InPhieuChamSocVo detail)
        {
            var content = string.Empty;
            var header = string.Empty;
            //var lstPhieuDieuTriInPhieuChamSoc = new List<NoiTruPhieuDieuTri>();
            //detail.PhieuDieuTriId = detail.PhieuDieuTriId ?? 0;
            //detail.YeuCauTiepNhanId = detail.YeuCauTiepNhanId ?? 0;

            //var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuChamSoc"));

            //var benhAnId = await _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Where(x => x.Id == detail.PhieuDieuTriId)
            //    .Select(x => x.NoiTruBenhAnId).FirstOrDefaultAsync();

            //var lstPhieuDieuTri = await _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienXacNhanThucHien).ThenInclude(t => t.User)
            //    .Include(x => x.KhoaPhongDieuTri)
            //    .Include(x => x.ChanDoanChinhICD)
            //    .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.YeuCauTiepNhan).ThenInclude(z => z.YeuCauDichVuGiuongBenhViens).ThenInclude(t => t.GiuongBenh).ThenInclude(u => u.PhongBenhVien)
            //    .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.YeuCauTiepNhan)
            //    .Where(x => x.NoiTruBenhAnId == benhAnId
            //               //&& (x.NoiTruPhieuDieuTriChiTietDienBiens.Any() || x.NoiTruPhieuDieuTriChiTietYLenhs.Any()))
            //               || (detail.YeuCauTiepNhanId != 0 && x.NoiTruBenhAn.YeuCauTiepNhan.Id == detail.YeuCauTiepNhanId))
            //    .OrderBy(x => x.Id)
            //    .ToListAsync();

            //if (detail.InTatCa == true)
            //{
            //    //lstPhieuDieuTriInPhieuChamSoc.AddRange(lstPhieuDieuTri.Where(x => x.NoiTruPhieuDieuTriChiTietDienBiens.Any() || x.NoiTruPhieuDieuTriChiTietYLenhs.Any()).ToList());
            //    lstPhieuDieuTriInPhieuChamSoc.AddRange(lstPhieuDieuTri.Where(x => x.NoiTruPhieuDieuTriChiTietYLenhs.Any(y => y.YeuCauDichVuKyThuatId == null
            //                                                                                                                 && y.YeuCauTruyenMauId == null
            //                                                                                                                 && y.YeuCauVatTuBenhVienId == null
            //                                                                                                                 && y.NoiTruChiDinhDuocPhamId == null
            //                                                                                                                 //&& x.NoiTruPhieuDieuTriChiTietDienBiens.Any(z => z.GioDienBien == y.GioYLenh)
            //                                                                                                                 )).ToList());
            //    header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //              "<th>PHIẾU CHĂM SÓC</th>" +
            //              "</p>";
            //}
            //else
            //{
            //    if (detail.PhieuDieuTriId == null)
            //    {
            //        throw new Exception(_localizationService.GetResource("TongHopYLenh.PhieuDieuTri.NotExists"));
            //    }
            //    var phieuDieuTri = lstPhieuDieuTri.First(x => x.Id == detail.PhieuDieuTriId);
            //    lstPhieuDieuTriInPhieuChamSoc.Add(phieuDieuTri);
            //}

            //foreach (var phieuDieuTri in lstPhieuDieuTriInPhieuChamSoc)
            //{
            //    var data = new TongHopYLenhPhieuChamSocVo()
            //    {
            //        Header = header,
            //        Khoa = phieuDieuTri.KhoaPhongDieuTri.Ten,
            //        SoVaoVien = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //        PhieuSo = lstPhieuDieuTri.Count(x => x.Id <= phieuDieuTri.Id).ToString(),
            //        BenhNhanHoTen = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.HoTen,
            //        NgaySinh = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.NgaySinh,
            //        ThangSinh = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.ThangSinh,
            //        NamSinh = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.NamSinh,
            //        BenhNhanGioiTinh = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.GioiTinh.GetDescription(),
            //        SoGiuong = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                       && a.ThoiDiemBatDauSuDung.Value.Date <= phieuDieuTri.NgayDieuTri.Date
            //                                                                                       && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= phieuDieuTri.NgayDieuTri.Date)) ?
            //            string.Join("; ", phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                    && a.ThoiDiemBatDauSuDung.Value.Date <= phieuDieuTri.NgayDieuTri.Date
            //                                                                                    && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= phieuDieuTri.NgayDieuTri.Date)).Select(a => a.GiuongBenh.Ten).ToList()) : null,
            //        ChanDoan = phieuDieuTri.ChanDoanChinhICD?.TenTiengViet,
            //        Buong = phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                              && a.ThoiDiemBatDauSuDung.Value.Date <= phieuDieuTri.NgayDieuTri.Date
            //                                                                                              && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= phieuDieuTri.NgayDieuTri.Date)) ?
            //            string.Join("; ", phieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.ThoiDiemBatDauSuDung != null
            //                                                                                            && a.ThoiDiemBatDauSuDung.Value.Date <= phieuDieuTri.NgayDieuTri.Date
            //                                                                                            && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= phieuDieuTri.NgayDieuTri.Date)).Select(a => a.GiuongBenh.PhongBenhVien.Ten).ToList()) : null,
            //    };

            //    var ngay = phieuDieuTri.NgayDieuTri.ApplyFormatDate();

            //    #region cập nhật 08/06/2021: chỉ in các y lệnh thêm từ y tá/ điều dưỡng
            //    var lstYLenhDieuDuongChiDinh = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.YeuCauDichVuKyThuatId == null
            //                                                                                           && x.YeuCauTruyenMauId == null
            //                                                                                           && x.YeuCauVatTuBenhVienId == null
            //                                                                                           && x.NoiTruChiDinhDuocPhamId == null).ToList();
            //    var lstGioYLenh = lstYLenhDieuDuongChiDinh.Select(x => x.GioYLenh).Distinct().ToList();
            //    var lstDienBienTheoGioYLenh = phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens
            //        .Where(x => lstGioYLenh.Contains(x.GioDienBien))
            //        .OrderBy(x => x.GioDienBien)
            //        .ToList();

            //    data.DanhSachYLenh += "<table id='customers'>"
            //                          + "<thead>"
            //                          + "<tr>"
            //                          + "<th width='10%'> Ngày </ th >"
            //                          + "<th width='10%'> Giờ, phút </ th >"
            //                          + "<th width='30%'> Theo dõi diễn biến </ th >"
            //                          + "<th width='40%'> Thực hiện y lệnh, chăm sóc </ th >"
            //                          + "<th width='10%'> Ký tên </ th >"
            //                          + "</tr>"
            //                          + "</thead>"
            //                          + "<tbody>";
            //    #endregion

            //    //if (phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Any() && phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.Any())
            //    if (lstYLenhDieuDuongChiDinh.Any() && lstDienBienTheoGioYLenh.Any())
            //    {
            //        //data.DanhSachYLenh += "<table id='customers'>"
            //        //                      + "<thead>"
            //        //                      + "<tr>"
            //        //                      + "<th width='10%'> Ngày </ th >"
            //        //                      + "<th width='10%'> Giờ, phút </ th >"
            //        //                      + "<th width='30%'> Theo dõi diễn biến </ th >"
            //        //                      + "<th width='40%'> Thực hiện y lệnh, chăm sóc </ th >"
            //        //                      + "<th width='10%'> Ký tên </ th >"
            //        //                      + "</tr>"
            //        //                      + "</thead>"
            //        //                      + "<tbody>";
            //        //foreach (var dienBien in phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.OrderBy(x => x.GioDienBien))
            //        foreach (var dienBien in lstDienBienTheoGioYLenh)
            //        {
            //            //var lstYLenhTheoDienBien = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.GioYLenh == dienBien.GioDienBien).ToList();
            //            var lstYLenhTheoDienBien = lstYLenhDieuDuongChiDinh.Where(x => x.GioYLenh == dienBien.GioDienBien).ToList();
            //            var count = lstYLenhTheoDienBien.Count;
            //            if (count == 1)
            //            {
            //                data.DanhSachYLenh += "<tr style='border: 1px solid #020000;text-align: center; '>"
            //                                      + "<td style = 'text-align: left;vertical-align: top;'>" + ngay
            //                                      + "<td style = 'text-align: left;vertical-align: top;'>" + dienBien.GioDienBien.ConvertIntSecondsToTime12h()
            //                                      + "<td style = 'text-align: left;vertical-align: top;'>" + dienBien.MoTaDienBien.Replace("\n", "<br>")
            //                                      + "<td style = 'text-align: left;vertical-align: top;'>" + lstYLenhTheoDienBien[0].MoTaYLenh.Replace("\n", "<br>")
            //                                      + "<td style = 'text-align: left;vertical-align: top;'>" + lstYLenhTheoDienBien[0].NhanVienXacNhanThucHien?.User?.HoTen
            //                                      + "</tr>";
            //            }
            //            else
            //            {
            //                var isFirst = true;
            //                foreach (var yLenh in lstYLenhTheoDienBien)
            //                {
            //                    data.DanhSachYLenh += "<tr style='border: 1px solid #020000;text-align: left; '>"
            //                                          + (isFirst ? "<td style = 'text-align: left; vertical-align: top;' rowspan='" + count + "'>" + ngay : "")
            //                                          + (isFirst ? "<td style = 'text-align: left; vertical-align: top;' rowspan='" + count + "'>" + dienBien.GioDienBien.ConvertIntSecondsToTime12h() : "")
            //                                          + (isFirst ? "<td style = 'text-align: left; vertical-align: top;' rowspan='" + count + "'>" + dienBien.MoTaDienBien.Replace("\n", "<br>") : "")
            //                                          + "<td style = 'text-align: left;vertical-align: top;'>" + yLenh.MoTaYLenh.Replace("\n", "<br>")
            //                                          + "<td style = 'text-align: left;vertical-align: top;'>" + yLenh.NhanVienXacNhanThucHien?.User?.HoTen
            //                                          + "</tr>";

            //                    isFirst = false;
            //                }
            //            }
            //        }
            //        //data.DanhSachYLenh += "</tbody>" + "</table>";
            //    }
            //    data.DanhSachYLenh += "</tbody>" + "</table>";
            //    content += TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data) + "<div class='pagebreak'></div>";
            //}
            return content;
        }

        public async Task<string> InPhieuChamSocAsyncVer2(InPhieuChamSocVo detail)
        {
            var content = string.Empty;
            var header = string.Empty;
            var lstInPhieuChamSoc = new List<ThongTinInChiTietYLenhVo>();
            detail.NoiTruBenhAnId = detail.NoiTruBenhAnId ?? 0;
            detail.YeuCauTiepNhanId = detail.YeuCauTiepNhanId ?? 0;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuChamSoc"));

            var noiTruBenhAn = await _noiTruBenhAnRepository.TableNoTracking
                .Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
                .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs).ThenInclude(y => y.NhanVienXacNhanThucHien).ThenInclude(t => t.User)
                .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.KhoaPhongDieuTri)
                .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.ChanDoanChinhICD)
                .Include(y => y.YeuCauTiepNhan).ThenInclude(z => z.YeuCauDichVuGiuongBenhViens).ThenInclude(t => t.GiuongBenh).ThenInclude(u => u.PhongBenhVien)
                .Include(y => y.YeuCauTiepNhan).ThenInclude(y => y.YeuCauNhapVien).ThenInclude(y => y.ChanDoanNhapVienICD)
                .Include(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                .Include(x => x.KhoaPhongNhapVien)
                .Where(x => x.Id == detail.NoiTruBenhAnId)
                .FirstAsync();

            if (!noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.YeuCauDichVuKyThuatId == null
                                                                       && x.YeuCauTruyenMauId == null
                                                                       && x.YeuCauVatTuBenhVienId == null
                                                                       && x.NoiTruChiDinhDuocPhamId == null
                                                                       
                                                                       //BVHD-3575
                                                                       && x.YeuCauKhamBenhId == null
                                                                       ))
            {
                throw new Exception(_localizationService.GetResource("TongHopYLenh.InPhieuChamSoc.IsEmpty"));
            }

            var ngayDieuTriCoYLenhCuoiCungHienTai = 
                noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => noiTruBenhAn.ThoiDiemRaVien == null || x.NgayDieuTri.Date <= noiTruBenhAn.ThoiDiemRaVien.Value.Date) 
                ? noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                    .Where(x => noiTruBenhAn.ThoiDiemRaVien == null || x.NgayDieuTri.Date <= noiTruBenhAn.ThoiDiemRaVien.Value.Date)
                    .OrderByDescending(x => x.NgayDieuTri)
                    .Select(x => x.NgayDieuTri).FirstOrDefault() 
                : (DateTime?)null;
            var ngayPhieuDieuTriCuoiCungHienTai = 
                noiTruBenhAn.NoiTruPhieuDieuTris.Any(x => noiTruBenhAn.ThoiDiemRaVien == null || x.NgayDieuTri.Date <= noiTruBenhAn.ThoiDiemRaVien.Value.Date) 
                ? noiTruBenhAn.NoiTruPhieuDieuTris
                    .Where(x => noiTruBenhAn.ThoiDiemRaVien == null || x.NgayDieuTri.Date <= noiTruBenhAn.ThoiDiemRaVien.Value.Date)
                    .OrderByDescending(x => x.NgayDieuTri)
                    .Select(x => x.NgayDieuTri).FirstOrDefault()
                : (DateTime?) null;
            var ngayCuoiCung = noiTruBenhAn.ThoiDiemRaVien != null ? noiTruBenhAn.ThoiDiemRaVien.Value.Date : DateTime.Now.Date;
            if (ngayDieuTriCoYLenhCuoiCungHienTai != null || ngayPhieuDieuTriCuoiCungHienTai != null)
            {
                if (ngayDieuTriCoYLenhCuoiCungHienTai == null)
                {
                    ngayCuoiCung = ngayPhieuDieuTriCuoiCungHienTai.Value;
                }
                else if (ngayPhieuDieuTriCuoiCungHienTai == null)
                {
                    ngayCuoiCung = ngayDieuTriCoYLenhCuoiCungHienTai.Value;
                }
                else
                {
                    ngayCuoiCung =
                        ngayPhieuDieuTriCuoiCungHienTai.Value.Date > ngayDieuTriCoYLenhCuoiCungHienTai.Value.Date
                            ? ngayPhieuDieuTriCuoiCungHienTai.Value.Date
                            : ngayDieuTriCoYLenhCuoiCungHienTai.Value.Date;
                }
            }

            for (DateTime ngayDieuTri = noiTruBenhAn.ThoiDiemNhapVien.Date; ngayDieuTri <= ngayCuoiCung.Date; ngayDieuTri = ngayDieuTri.AddDays(1))
            {
                var newChiTiet = new ThongTinInChiTietYLenhVo();
                newChiTiet.NgayDieuTri = ngayDieuTri.Date;
                newChiTiet.DienBiens.AddRange(noiTruBenhAn.NoiTruPhieuDieuTriChiTietDienBiens
                                                        .Where(x => x.NgayDieuTri.Date == ngayDieuTri.Date).ToList());
                newChiTiet.YLenhs.AddRange(noiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                                                        .Where(x => x.NgayDieuTri.Date == ngayDieuTri.Date 
                                                                    && x.YeuCauDichVuKyThuatId == null
                                                                    && x.YeuCauTruyenMauId == null
                                                                    && x.YeuCauVatTuBenhVienId == null
                                                                    && x.NoiTruChiDinhDuocPhamId == null
                                                                    
                                                                    //BVHD-3575
                                                                    && x.YeuCauKhamBenhId == null
                                                                    ).ToList());
                lstInPhieuChamSoc.Add(newChiTiet);
            }
            
            if (detail.InTatCa == true)
            {
                if (detail.KhongHienHeader != true)
                {
                    header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU CHĂM SÓC</th>" +
                              "</p>";
                }

                lstInPhieuChamSoc = lstInPhieuChamSoc.Where(x => x.DienBiens.Any() && x.YLenhs.Any()).ToList();
            }
            else
            {
                lstInPhieuChamSoc = lstInPhieuChamSoc.Where(x => x.NgayDieuTri.Date == detail.NgayDieuTri.Date).ToList();
            }

            foreach (var inPhieuChamSoc in lstInPhieuChamSoc)
            {
                var data = new TongHopYLenhPhieuChamSocVo()
                {
                    Header = header,
                    Khoa =
                        //noiTruBenhAn.NoiTruPhieuDieuTris.Any(x => x.NgayDieuTri.Date <= DateTime.Now.Date)
                        //? noiTruBenhAn.NoiTruPhieuDieuTris.Where(x => x.NgayDieuTri.Date <= DateTime.Now.Date).OrderByDescending(x => x.NgayDieuTri).First().KhoaPhongDieuTri?.Ten
                        //: noiTruBenhAn.KhoaPhongNhapVien?.Ten,
                        noiTruBenhAn.NoiTruKhoaPhongDieuTris.Any(a => a.ThoiDiemVaoKhoa.Date <= DateTime.Now.Date && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= DateTime.Now.Date))
                            ? noiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(a => a.ThoiDiemVaoKhoa.Date <= DateTime.Now.Date && (a.ThoiDiemRaKhoa == null || a.ThoiDiemRaKhoa.Value.Date >= DateTime.Now.Date)).OrderByDescending(x => x.Id).Select(a => a.KhoaPhongChuyenDen.Ten).First()
                            : noiTruBenhAn.KhoaPhongNhapVien?.Ten,
                    SoVaoVien = noiTruBenhAn.SoBenhAn,
                    PhieuSo = ((inPhieuChamSoc.NgayDieuTri - noiTruBenhAn.ThoiDiemNhapVien.Date).Days + 1).ToString(), //(lstInPhieuChamSoc.IndexOf(inPhieuChamSoc) + 1).ToString(),
                    BenhNhanHoTen = noiTruBenhAn.YeuCauTiepNhan.HoTen,
                    NgaySinh = noiTruBenhAn.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = noiTruBenhAn.YeuCauTiepNhan.ThangSinh,
                    NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh,
                    BenhNhanGioiTinh = noiTruBenhAn.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    SoGiuong = noiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                                && a.ThoiDiemBatDauSuDung != null
                                                                                                   && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                                   && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date)) ?
                        string.Join("; ", noiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                                             && a.ThoiDiemBatDauSuDung != null
                                                                                                && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                                && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date)).Select(a => a.GiuongBenh.Ten).Distinct().ToList()) : null,
                    ChanDoan = //phieuDieuTri.ChanDoanChinhICD?.TenTiengViet,
                        noiTruBenhAn.NoiTruPhieuDieuTris.Any(x => x.NgayDieuTri.Date <= DateTime.Now.Date)
                            ? noiTruBenhAn.NoiTruPhieuDieuTris.Where(x => x.NgayDieuTri.Date <= DateTime.Now.Date).OrderByDescending(x => x.NgayDieuTri).First().ChanDoanChinhICD?.TenTiengViet
                            : noiTruBenhAn.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD?.TenTiengViet,
                    Buong = noiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                             && a.ThoiDiemBatDauSuDung != null
                                                                                                          && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                                          && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date)) ?
                        string.Join("; ", noiTruBenhAn.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(a => a.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan
                                                                                                             && a.ThoiDiemBatDauSuDung != null
                                                                                                        && a.ThoiDiemBatDauSuDung.Value.Date <= DateTime.Now.Date
                                                                                                        && (a.ThoiDiemKetThucSuDung == null || a.ThoiDiemKetThucSuDung.Value.Date >= DateTime.Now.Date)).Select(a => a.GiuongBenh.PhongBenhVien.Ten).Distinct().ToList()) : null,
                };

                var ngay = inPhieuChamSoc.NgayDieuTri.ApplyFormatDate();

                #region cập nhật 08/06/2021: chỉ in các y lệnh thêm từ y tá/ điều dưỡng
                var lstYLenhDieuDuongChiDinh = inPhieuChamSoc.YLenhs;
                var lstGioYLenh = lstYLenhDieuDuongChiDinh.Select(x => x.GioYLenh).Distinct().ToList();
                var lstDienBienTheoGioYLenh = inPhieuChamSoc.DienBiens
                    .Where(x => lstGioYLenh.Contains(x.GioDienBien))
                    .OrderBy(x => x.GioDienBien)
                    .ToList();

                data.DanhSachYLenh += "<table id='customers'>"
                                      + "<thead>"
                                      + "<tr>"
                                      + "<th width='10%'> Ngày </ th >"
                                      + "<th width='10%'> Giờ, phút </ th >"
                                      + "<th width='30%'> Theo dõi diễn biến </ th >"
                                      + "<th width='40%'> Thực hiện y lệnh, chăm sóc </ th >"
                                      + "<th width='10%'> Ký tên </ th >"
                                      + "</tr>"
                                      + "</thead>"
                                      + "<tbody>";
                #endregion
                
                if (lstYLenhDieuDuongChiDinh.Any() && lstDienBienTheoGioYLenh.Any())
                {
                    foreach (var dienBien in lstDienBienTheoGioYLenh)
                    {
                        var lstYLenhTheoDienBien = lstYLenhDieuDuongChiDinh.Where(x => x.GioYLenh == dienBien.GioDienBien).ToList();
                        var count = lstYLenhTheoDienBien.Count;
                        if (count == 1)
                        {
                            data.DanhSachYLenh += "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                  + "<td style = 'text-align: left;vertical-align: top;'>" + ngay
                                                  + "<td style = 'text-align: left;vertical-align: top;'>" + dienBien.GioDienBien.ConvertIntSecondsToTime12h()
                                                  + "<td style = 'text-align: left;vertical-align: top;'>" + dienBien.MoTaDienBien.Replace("\n", "<br>")
                                                  + "<td style = 'text-align: left;vertical-align: top;'>" + lstYLenhTheoDienBien[0].MoTaYLenh.Replace("\n", "<br>")
                                                  + "<td style = 'text-align: left;vertical-align: top;'>" + lstYLenhTheoDienBien[0].NhanVienXacNhanThucHien?.User?.HoTen
                                                  + "</tr>";
                        }
                        else
                        {
                            var isFirst = true;
                            foreach (var yLenh in lstYLenhTheoDienBien)
                            {
                                data.DanhSachYLenh += "<tr style='border: 1px solid #020000;text-align: left; '>"
                                                      + (isFirst ? "<td style = 'text-align: left; vertical-align: top;' rowspan='" + count + "'>" + ngay : "")
                                                      + (isFirst ? "<td style = 'text-align: left; vertical-align: top;' rowspan='" + count + "'>" + dienBien.GioDienBien.ConvertIntSecondsToTime12h() : "")
                                                      + (isFirst ? "<td style = 'text-align: left; vertical-align: top;' rowspan='" + count + "'>" + dienBien.MoTaDienBien.Replace("\n", "<br>") : "")
                                                      + "<td style = 'text-align: left;vertical-align: top;'>" + yLenh.MoTaYLenh.Replace("\n", "<br>")
                                                      + "<td style = 'text-align: left;vertical-align: top;'>" + yLenh.NhanVienXacNhanThucHien?.User?.HoTen
                                                      + "</tr>";

                                isFirst = false;
                            }
                        }
                    }
                }
                data.DanhSachYLenh += "</tbody>" + "</table>";
                content += TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data) + "<div class='pagebreak'></div>";
            }
            return content;
        }
        #endregion

        #region Tổng hợp y lệnh từ tất cả các nguồn
        // function này giờ chỉ dùng cho trường hợp vào xem chi tiết từng người bệnh
        // job tổng hợp sẽ tách ra 4 job nhỏ
        public void XuLyTongHopYLenh(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null)
        {
            //2: kiểm tra lại thông tin xác nhận thực hiện của: yc dvkt, yc vật tư, yc truyền máu
            #region Thời gian mặc định trong ngày

            var cauHinhThoiGianMacDinh = _cauHinhRepository.TableNoTracking.First(x => x.Name == "CauHinhNoiTru.ThoiGianMacDinhTrongNgay");
            var thoiGianMacDinhs = JsonConvert.DeserializeObject<List<LookupItemCauHinhVo>>(cauHinhThoiGianMacDinh.Value);
            var sang = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Sang).Value);
            var trua = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Trua).Value);
            var chieu = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Chieu).Value);
            var toi = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Toi).Value);

            #endregion

            //var noiTruBenhAnIds = _noiTruBenhAnRepository.TableNoTracking
            //    .Where(x => x.ThoiDiemRaVien == null
            //                && (phieuDieuTriId == null || x.NoiTruPhieuDieuTris.Any(y => y.Id == phieuDieuTriId))
            //                && x.NoiTruPhieuDieuTris.Any(a => a.YeuCauDichVuKyThuats.Any() || a.NoiTruChiDinhDuocPhams.Any() || a.YeuCauTruyenMaus.Any() || a.YeuCauVatTuBenhViens.Any()))
            //    .Select(x => x.Id)
            //    .ToList();


            // chỉ kiểm tra đối với trường hợp chạy job tổng hợp
            long benhAnIdPrev = 0;
            int take = 25;
            XDocument data = null;
            XElement benhAnElement = null;
            var path = @"Resource\\TongHopYLenhBenhAnCuoiCung.xml";
            if (noiTruBenhAnId == null)
            {
                if (File.Exists(path))
                {
                    data = XDocument.Load(path);
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                    benhAnIdPrev = (long)benhAnElement.Element(root + "BenhAnId");
                }
                else
                {
                    data =
                        new XDocument(
                            new XElement("TongHopYLenh",
                                new XElement("BenhAnId", benhAnIdPrev.ToString())
                            )
                        );
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                }
            }

            #region BVHD-3575: cập nhật y lệnh khám bệnh cho trường hợp chỉ định dv khám từ nội trú
            //var noiTruBenhAnCoDvKhamIds = _noiTruBenhAnRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null
            //                && (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
            //                && x.Id > benhAnIdPrev)
            //    .Select(x => new ThongTinBenhAnCoChiDinhKhamVo()
            //    {
            //        NoiTruBenhAnId = x.Id,
            //        YeuCauTiepNhanNgoaiTruId = x.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value,
            //        ThoiDiemTongHopYLenhKhamBenh = x.ThoiDiemTongHopYLenhKhamBenh
            //    })
            //    .Take(take)
            //    .ToList();

            //var lstTiepNhanIdCoKhamBenhTuNoiTru =
            //    _yeuCauKhamBenhRepository.TableNoTracking
            //        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
            //                    && noiTruBenhAnCoDvKhamIds
            //                        .Any(a => a.YeuCauTiepNhanNgoaiTruId == x.YeuCauTiepNhanId
            //                                  && (a.ThoiDiemTongHopYLenhKhamBenh == null
            //                                      || x.CreatedOn.Value >= a.ThoiDiemTongHopYLenhKhamBenh.Value.Date
            //                                      || x.LastTime.Value >= a.ThoiDiemTongHopYLenhKhamBenh.Value.Date)))
            //        .Select(x => x.YeuCauTiepNhanId)
            //        .Distinct().ToList();
            //var lstNoiTruBenhAnIdCoYLenhDVKham = noiTruBenhAnCoDvKhamIds
            //    .Where(x => lstTiepNhanIdCoKhamBenhTuNoiTru.Contains(x.YeuCauTiepNhanNgoaiTruId))
            //    .Select(x => x.NoiTruBenhAnId)
            //    .Distinct().ToList();

            #endregion

            var noiTruBenhAnIds = _noiTruBenhAnRepository.TableNoTracking
                .Where(x => 
                    //(x.ThoiDiemTongHopYLenhDichVuKyThuat == null 
                    //         || x.ThoiDiemTongHopYLenhTruyenMau == null
                    //         || x.ThoiDiemTongHopYLenhVatTu == null
                    //         || x.ThoiDiemTongHopYLenhDuocPham == null
                    //         || (x.NoiTruPhieuDieuTris.Any(y => (x.ThoiDiemTongHopYLenhDichVuKyThuat != null && y.YeuCauDichVuKyThuats.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhDichVuKyThuat
                    //                                                                                                                            || z.LastTime.Value >= x.ThoiDiemTongHopYLenhDichVuKyThuat))
                    //                                         || (x.ThoiDiemTongHopYLenhTruyenMau != null && y.YeuCauTruyenMaus.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhTruyenMau
                    //                                                                                                                            || z.LastTime.Value >= x.ThoiDiemTongHopYLenhTruyenMau))
                    //                                         || (x.ThoiDiemTongHopYLenhVatTu != null && y.YeuCauVatTuBenhViens.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhVatTu 
                    //                                                                                                                            || z.LastTime.Value >= x.ThoiDiemTongHopYLenhVatTu))
                    //                                         || (x.ThoiDiemTongHopYLenhDuocPham != null &&y.NoiTruChiDinhDuocPhams.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhDuocPham
                    //                                                                                                                            || z.LastTime.Value >= x.ThoiDiemTongHopYLenhDuocPham))))
                    //         //BVHD-3575
                    //         || lstNoiTruBenhAnIdCoYLenhDVKham.Contains(x.Id)
                    //         )
                            x.DaQuyetToan != true
                            && (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
                            && x.Id > benhAnIdPrev)
                .Select(x => x.Id)
                .Take(take)
                .ToList();
            if (noiTruBenhAnId == null)
            {
                if (noiTruBenhAnIds.Count < take)
                {
                    benhAnIdPrev = 0;
                }
                else
                {
                    benhAnIdPrev = noiTruBenhAnIds.OrderByDescending(x => x).First();
                }

                benhAnElement.Element("BenhAnId").Value = benhAnIdPrev.ToString();
                data.Save(path);
            }

            // bổ sung 14/03/2022: gán mặc định giây = 0
            var currentDateTime = DateTime.Now;
            var thoiDiemTongHop = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, 0);

            foreach (var benhAnId in noiTruBenhAnIds)
            {
                //var benhAn = _noiTruBenhAnRepository.Table
                //    .Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                //    .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                //    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.YeuCauDichVuKyThuats)
                //    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.NoiTruChiDinhDuocPhams)
                //    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.YeuCauTruyenMaus)
                //    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.YeuCauVatTuBenhViens).ThenInclude(t => t.XuatKhoVatTuChiTiet).ThenInclude(u => u.XuatKhoVatTu)
                //    .First(x => x.Id == benhAnId);

                // bổ sung 14/02/2022: gán thời điểm tổng hợp = thời điểm get data để tránh trường hợp đang tổng hợp thì có chỉ định mới
                //var thoiDiemTongHop = DateTime.Now;

                var benhAn = _noiTruBenhAnRepository.GetById(benhAnId, a => a.Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                    .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.YeuCauDichVuKyThuats)
                    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.NoiTruChiDinhDuocPhams)
                    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.YeuCauTruyenMaus)
                    .Include(x => x.NoiTruPhieuDieuTris).ThenInclude(y => y.YeuCauVatTuBenhViens).ThenInclude(t => t.XuatKhoVatTuChiTiet).ThenInclude(u => u.XuatKhoVatTu)
                
                    // BVHD-3575
                    .Include(x => x.YeuCauTiepNhan));
                var lstPhieuDieuTri = benhAn.NoiTruPhieuDieuTris.Where(x => ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date).ToList();

                var isChange = false;
                foreach (var phieuDieuTri in lstPhieuDieuTri)
                {
                    // dịch vụ kỹ thuật
                    //11/08/2022: fix bug load thiếu y lệnh so với thời gian tổng hợp -> bổ sung thêm trường hợp dịch vụ trước thời điểm tổng hợp (trạng thái khác hủy) mà chưa có trong y lệnh
                    var lstYCDVKTIdDaCoYLenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs
                        .Where(x => x.YeuCauDichVuKyThuatId != null)
                        .Select(x => x.YeuCauDichVuKyThuatId.Value)
                        .Distinct().ToList();
                    var dichVuKyThuatChanges = phieuDieuTri.YeuCauDichVuKyThuats.Where(x => benhAn.ThoiDiemTongHopYLenhDichVuKyThuat == null
                                                                                            || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhDichVuKyThuat
                                                                                            || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhDichVuKyThuat
                                                                                            || (x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && lstYCDVKTIdDaCoYLenh.All(a => a != x.Id))).ToList();
                    if (dichVuKyThuatChanges.Any())
                    {
                        isChange = true;
                        foreach (var dichVu in dichVuKyThuatChanges)
                        {
                            //var chiTietYlenh = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauDichVuKyThuatId == dichVu.Id);
                            var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauDichVuKyThuatId == dichVu.Id);

                            if (chiTietYlenh == null)
                            {
                                if (dichVu.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                {
                                    if (dichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SuatAn)
                                    {
                                        var gioDienBien = 0;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        {
                                            YeuCauDichVuKyThuatId = dichVu.Id,
                                            MoTaYLenh = dichVu.TenDichVu,
                                            GioYLenh = gioDienBien,
                                            NhanVienChiDinhId = dichVu.NhanVienChiDinhId,
                                            NoiChiDinhId = dichVu.NoiChiDinhId.Value,
                                            XacNhanThucHien = null,
                                            ThoiDiemXacNhanThucHien = null,
                                            NhanVienXacNhanThucHienId = null,
                                            LyDoKhongThucHien = null,
                                            ThoiDiemCapNhat = null,
                                            NhanVienCapNhatId = null,
                                            NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                        });
                                    }
                                    else
                                    {
                                        var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        {
                                            YeuCauDichVuKyThuatId = dichVu.Id,
                                            MoTaYLenh = dichVu.TenDichVu,
                                            GioYLenh = gioDienBien,
                                            NhanVienChiDinhId = dichVu.NhanVienChiDinhId,
                                            NoiChiDinhId = dichVu.NoiChiDinhId.Value,
                                            XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                                            ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien,
                                            NhanVienXacNhanThucHienId = dichVu.NhanVienKetLuanId ?? dichVu.NhanVienThucHienId,
                                            LyDoKhongThucHien = null,
                                            ThoiDiemCapNhat = dichVu.LastTime,
                                            NhanVienCapNhatId = dichVu.LastUserId,
                                            NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                        });
                                    }
                                }
                            }
                            else if (dichVu.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                            {
                                chiTietYlenh.WillDelete = true;
                            }
                            else
                            {
                                if (dichVu.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn)
                                {
                                    var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;

                                    chiTietYlenh.GioYLenh = gioDienBien;
                                    chiTietYlenh.XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                                    chiTietYlenh.ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien;
                                    chiTietYlenh.NhanVienXacNhanThucHienId = dichVu.NhanVienKetLuanId ?? dichVu.NhanVienThucHienId;
                                    chiTietYlenh.ThoiDiemCapNhat = dichVu.LastTime;
                                    chiTietYlenh.NhanVienCapNhatId = dichVu.LastUserId;
                                }
                            }
                        }
                    }

                    // truyền máu
                    //11/08/2022: fix bug load thiếu y lệnh so với thời gian tổng hợp -> bổ sung thêm trường hợp dịch vụ trước thời điểm tổng hợp (trạng thái khác hủy) mà chưa có trong y lệnh
                    var lstYCTruyenMauIdDaCoYLenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs
                        .Where(x => x.YeuCauTruyenMauId != null)
                        .Select(x => x.YeuCauTruyenMauId.Value)
                        .Distinct().ToList();
                    var truyenMauChanges = phieuDieuTri.YeuCauTruyenMaus.Where(x => benhAn.ThoiDiemTongHopYLenhTruyenMau == null
                                                                                    || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhTruyenMau
                                                                                    || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhTruyenMau
                                                                                    || (x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy && lstYCTruyenMauIdDaCoYLenh.All(a => a != x.Id))).ToList();
                    if (truyenMauChanges.Any())
                    {
                        isChange = true;
                        foreach (var truyenMau in truyenMauChanges)
                        {
                            //var chiTietYlenh = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauTruyenMauId == truyenMau.Id);
                            var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauTruyenMauId == truyenMau.Id);
                            if (chiTietYlenh == null)
                            {
                                if (truyenMau.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy)
                                {
                                    var gioDienBien = truyenMau.ThoiGianBatDauTruyen ?? 0; //todo: nếu ko có thời gian bắt đầu truyền, gán mặc định là 0h
                                                                                           //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    {
                                        YeuCauTruyenMauId = truyenMau.Id,
                                        MoTaYLenh = truyenMau.TenDichVu,
                                        GioYLenh = gioDienBien,
                                        NhanVienChiDinhId = truyenMau.NhanVienChiDinhId,
                                        NoiChiDinhId = truyenMau.NoiChiDinhId,
                                        XacNhanThucHien = truyenMau.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien,
                                        ThoiDiemXacNhanThucHien = truyenMau.ThoiDiemThucHien,
                                        NhanVienXacNhanThucHienId = truyenMau.NhanVienThucHienId,
                                        LyDoKhongThucHien = null,
                                        ThoiDiemCapNhat = truyenMau.LastTime,
                                        NhanVienCapNhatId = truyenMau.LastUserId,
                                        NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                    });
                                }
                            }
                            else if (truyenMau.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy)
                            {
                                chiTietYlenh.WillDelete = true;
                            }
                            else
                            {
                                var gioDienBien = truyenMau.ThoiGianBatDauTruyen ?? 0; //todo: nếu ko có thời gian bắt đầu truyền, gán mặc định là 0h

                                chiTietYlenh.GioYLenh = gioDienBien;
                                chiTietYlenh.XacNhanThucHien = truyenMau.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien;
                                chiTietYlenh.ThoiDiemXacNhanThucHien = truyenMau.ThoiDiemThucHien;
                                chiTietYlenh.NhanVienXacNhanThucHienId = truyenMau.NhanVienThucHienId;
                                chiTietYlenh.ThoiDiemCapNhat = truyenMau.LastTime;
                                chiTietYlenh.NhanVienCapNhatId = truyenMau.LastUserId;
                            }
                        }
                    }

                    // vật tư bệnh viện
                    //11/08/2022: fix bug load thiếu y lệnh so với thời gian tổng hợp -> bổ sung thêm trường hợp dịch vụ trước thời điểm tổng hợp (trạng thái khác hủy) mà chưa có trong y lệnh
                    var lstYCVatTuIdDaCoYLenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs
                        .Where(x => x.YeuCauVatTuBenhVienId != null)
                        .Select(x => x.YeuCauVatTuBenhVienId.Value)
                        .Distinct().ToList();
                    var yeuCauVatTuBenhVienChanges = phieuDieuTri.YeuCauVatTuBenhViens.Where(x => benhAn.ThoiDiemTongHopYLenhVatTu == null
                                                                                                  || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhVatTu
                                                                                                  || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhVatTu
                                                                                                  || (x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && lstYCVatTuIdDaCoYLenh.All(a => a != x.Id))).ToList();
                    if (yeuCauVatTuBenhVienChanges.Any())
                    {
                        isChange = true;
                        foreach (var yeuCauVatTuBenhVien in yeuCauVatTuBenhVienChanges)
                        {
                            //var chiTietYlenh = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauVatTuBenhVienId == yeuCauVatTuBenhVien.Id);
                            var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauVatTuBenhVienId == yeuCauVatTuBenhVien.Id);
                            if (chiTietYlenh == null)
                            {
                                if (yeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                                {
                                    //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    {
                                        YeuCauVatTuBenhVienId = yeuCauVatTuBenhVien.Id,
                                        MoTaYLenh = yeuCauVatTuBenhVien.Ten,
                                        GioYLenh = 0, // vì ko có giờ y lệnh nên sẽ lấy mặc định là 0h
                                                      //yeuCauVatTuBenhVien.ThoiDiemChiDinh.Hour * 3600 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Minute * 60 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Second, //todo: cần kiểm tra lại thời gian y lệnh
                                        NhanVienChiDinhId = yeuCauVatTuBenhVien.NhanVienChiDinhId,
                                        NoiChiDinhId = yeuCauVatTuBenhVien.NoiChiDinhId,
                                        XacNhanThucHien = yeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien,
                                        ThoiDiemXacNhanThucHien = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NgayXuat,
                                        NhanVienXacNhanThucHienId = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NguoiXuatId,
                                        LyDoKhongThucHien = null,
                                        ThoiDiemCapNhat = yeuCauVatTuBenhVien.LastTime,
                                        NhanVienCapNhatId = yeuCauVatTuBenhVien.LastUserId,
                                        NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                    });
                                }
                            }
                            else if (yeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                            {
                                chiTietYlenh.WillDelete = true;
                            }
                            else
                            {
                                chiTietYlenh.GioYLenh = 0; // vì ko có giờ y lệnh nên sẽ lấy mặc định là 0h
                                                           //yeuCauVatTuBenhVien.ThoiDiemChiDinh.Hour * 3600 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Minute * 60 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Second; //todo: cần kiểm tra lại thời gian y lệnh
                                chiTietYlenh.XacNhanThucHien = yeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien;
                                chiTietYlenh.ThoiDiemXacNhanThucHien = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NgayXuat;
                                chiTietYlenh.NhanVienXacNhanThucHienId = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NguoiXuatId;
                                chiTietYlenh.ThoiDiemCapNhat = yeuCauVatTuBenhVien.LastTime;
                                chiTietYlenh.NhanVienCapNhatId = yeuCauVatTuBenhVien.LastUserId;
                            }
                        }
                    }

                    // dược phẩm
                    //11/08/2022: fix bug load thiếu y lệnh so với thời gian tổng hợp -> bổ sung thêm trường hợp dịch vụ trước thời điểm tổng hợp (trạng thái khác hủy) mà chưa có trong y lệnh
                    var lstYCDuocPhamIdDaCoYLenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs
                        .Where(x => x.NoiTruChiDinhDuocPhamId != null)
                        .Select(x => x.NoiTruChiDinhDuocPhamId.Value)
                        .Distinct().ToList();
                    var noiTruChiDinhDuocPhamChanges = phieuDieuTri.NoiTruChiDinhDuocPhams.Where(x => benhAn.ThoiDiemTongHopYLenhDuocPham == null
                                                                                                      || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhDuocPham
                                                                                                      || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhDuocPham
                                                                                                      || (x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && lstYCDuocPhamIdDaCoYLenh.All(a => a != x.Id))).ToList();
                    if (noiTruChiDinhDuocPhamChanges.Any())
                    {
                        isChange = true;
                        foreach (var noiTruChiDinhDuocPham in noiTruChiDinhDuocPhamChanges)
                        {
                            var sangTemp = noiTruChiDinhDuocPham.ThoiGianDungSang ?? (noiTruChiDinhDuocPham.DungSang == null ? (int?)null : sang);
                            var truaTemp = noiTruChiDinhDuocPham.ThoiGianDungTrua ?? (noiTruChiDinhDuocPham.DungTrua == null ? (int?)null : trua);
                            var chieuTemp = noiTruChiDinhDuocPham.ThoiGianDungChieu ?? (noiTruChiDinhDuocPham.DungChieu == null ? (int?)null : chieu);
                            var toiTemp = noiTruChiDinhDuocPham.ThoiGianDungToi ?? (noiTruChiDinhDuocPham.DungToi == null ? (int?)null : toi);
                            if (sangTemp == null && truaTemp == null && chieuTemp == null && toiTemp == null)
                            {
                                sangTemp = sang;
                            }

                            var soLanDungTrongNgayTemp = noiTruChiDinhDuocPham.SoLanDungTrongNgay ?? 1;
                            var cachGioTruyenDichTemp = noiTruChiDinhDuocPham.CachGioTruyenDich ?? 0;
                            if (noiTruChiDinhDuocPham.CachGioTruyenDich == null)
                            {
                                soLanDungTrongNgayTemp = 1;
                            }

                            var chiTietYLenhTemp = new NoiTruPhieuDieuTriChiTietYLenh()
                            {
                                NoiTruChiDinhDuocPhamId = noiTruChiDinhDuocPham.Id,
                                MoTaYLenh = noiTruChiDinhDuocPham.Ten,
                                GioYLenh = 0,
                                NhanVienChiDinhId = noiTruChiDinhDuocPham.NhanVienChiDinhId,
                                NoiChiDinhId = noiTruChiDinhDuocPham.NoiChiDinhId,
                                XacNhanThucHien = false,
                                ThoiDiemXacNhanThucHien = null,
                                NhanVienXacNhanThucHienId = null,
                                LyDoKhongThucHien = null,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                            };

                            //var chiTietYlenhs = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.NoiTruChiDinhDuocPhamId == noiTruChiDinhDuocPham.Id).ToList();
                            var chiTietYlenhs = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.NoiTruChiDinhDuocPhamId == noiTruChiDinhDuocPham.Id).ToList();
                            if (!chiTietYlenhs.Any())
                            {
                                if (noiTruChiDinhDuocPham.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                                {
                                    if (noiTruChiDinhDuocPham.LaDichTruyen != true)
                                    {
                                        if (sangTemp != null)//(noiTruChiDinhDuocPham.ThoiGianDungSang != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = sangTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungSang.Value;
                                                                                       //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                        if (truaTemp != null) //(noiTruChiDinhDuocPham.ThoiGianDungTrua != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = truaTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungTrua.Value;
                                                                                       //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                        if (chieuTemp != null) //(noiTruChiDinhDuocPham.ThoiGianDungChieu != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = chieuTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungChieu.Value;
                                                                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                        if (toiTemp != null) //(noiTruChiDinhDuocPham.ThoiGianDungToi != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = toiTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungToi.Value;
                                                                                      //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                    }
                                    else
                                    {
                                        var thoiGianBatDauTruyen = noiTruChiDinhDuocPham.ThoiGianBatDauTruyen ?? 0;
                                        //if (noiTruChiDinhDuocPham.SoLanDungTrongNgay == null)
                                        //{
                                        //    var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        //    chiTietYLenhNew.GioYLenh = thoiGianBatDauTruyen;
                                        //    phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        //}
                                        //else
                                        //{
                                        //    for (int i = 0; i < noiTruChiDinhDuocPham.SoLanDungTrongNgay; i++)
                                        //    {
                                        //        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        //        chiTietYLenhNew.GioYLenh = thoiGianBatDauTruyen + (int)(noiTruChiDinhDuocPham.CachGioTruyenDich.Value * 3600 * i).MathRoundNumber();
                                        //        phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        //    }
                                        //}

                                        for (int i = 0; i < soLanDungTrongNgayTemp; i++)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = thoiGianBatDauTruyen + (int)(cachGioTruyenDichTemp * 3600 * i).MathRoundNumber();
                                            //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                    }
                                }
                            }
                            else if (noiTruChiDinhDuocPham.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                            {
                                foreach (var chiTietYlenh in chiTietYlenhs)
                                {
                                    chiTietYlenh.WillDelete = true;
                                }
                            }
                            else
                            {
                                foreach (var chiTietYlenh in chiTietYlenhs)
                                {
                                    if (noiTruChiDinhDuocPham.LaDichTruyen != true)
                                    {
                                        //if (chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungSang
                                        //    && chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungTrua
                                        //    && chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungChieu
                                        //    && chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungToi)
                                        if (chiTietYlenh.GioYLenh != sangTemp
                                            && chiTietYlenh.GioYLenh != truaTemp
                                            && chiTietYlenh.GioYLenh != chieuTemp
                                            && chiTietYlenh.GioYLenh != toiTemp)
                                        {
                                            chiTietYlenh.WillDelete = true;
                                        }
                                    }
                                    else
                                    {
                                        if (noiTruChiDinhDuocPham.SoLanDungTrongNgay != null)
                                        {
                                            var isExists = false;
                                            var thoiGianBatDauTruyen = noiTruChiDinhDuocPham.ThoiGianBatDauTruyen ?? 0;
                                            //for (int i = 0; i < noiTruChiDinhDuocPham.SoLanDungTrongNgay; i++)
                                            //{
                                            //    var gioYLenh = thoiGianBatDauTruyen + (int)(noiTruChiDinhDuocPham.CachGioTruyenDich.Value * 3600 * i).MathRoundNumber();
                                            //    if (chiTietYlenh.GioYLenh == gioYLenh)
                                            //    {
                                            //        isExists = true;
                                            //    }
                                            //}

                                            for (int i = 0; i < soLanDungTrongNgayTemp; i++)
                                            {
                                                var gioYLenh = thoiGianBatDauTruyen + (int)(cachGioTruyenDichTemp * 3600 * i).MathRoundNumber();
                                                if (chiTietYlenh.GioYLenh == gioYLenh)
                                                {
                                                    isExists = true;
                                                }
                                            }

                                            if (isExists == false)
                                            {
                                                chiTietYlenh.WillDelete = true;
                                            }
                                        }
                                    }
                                }

                                if (noiTruChiDinhDuocPham.LaDichTruyen != true)
                                {
                                    //if (noiTruChiDinhDuocPham.ThoiGianDungSang != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungSang))
                                    if (sangTemp != null && chiTietYlenhs.All(x => x.GioYLenh != sangTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = sangTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungSang.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }

                                    //if (noiTruChiDinhDuocPham.ThoiGianDungTrua != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungTrua))
                                    if (truaTemp != null && chiTietYlenhs.All(x => x.GioYLenh != truaTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = truaTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungTrua.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }

                                    //if (noiTruChiDinhDuocPham.ThoiGianDungChieu != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungChieu))
                                    if (toiTemp != null && chiTietYlenhs.All(x => x.GioYLenh != toiTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = toiTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungChieu.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }

                                    //if (noiTruChiDinhDuocPham.ThoiGianDungToi != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungToi))
                                    if (toiTemp != null && chiTietYlenhs.All(x => x.GioYLenh != toiTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = toiTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungToi.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }
                                }
                                else
                                {
                                    var thoiGianBatDauTruyen = noiTruChiDinhDuocPham.ThoiGianBatDauTruyen ?? 0;
                                    if (noiTruChiDinhDuocPham.SoLanDungTrongNgay != null)
                                    {
                                        //for (int i = 0; i < noiTruChiDinhDuocPham.SoLanDungTrongNgay; i++)
                                        //{
                                        //    var gioYLenh = thoiGianBatDauTruyen + (int)(noiTruChiDinhDuocPham.CachGioTruyenDich.Value * 3600 * i).MathRoundNumber();
                                        //    if (chiTietYlenhs.All(x => x.GioYLenh != gioYLenh))
                                        //    {
                                        //        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        //        chiTietYLenhNew.GioYLenh = gioYLenh;
                                        //        phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        //    }
                                        //}

                                        for (int i = 0; i < soLanDungTrongNgayTemp; i++)
                                        {
                                            var gioYLenh = thoiGianBatDauTruyen + (int)(cachGioTruyenDichTemp * 3600 * i).MathRoundNumber();
                                            if (chiTietYlenhs.All(x => x.GioYLenh != gioYLenh))
                                            {
                                                var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                                chiTietYLenhNew.GioYLenh = gioYLenh;
                                                //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                                benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var chiTietYLenhXoa in chiTietYlenhs.Where(x => x.GioYLenh != thoiGianBatDauTruyen))
                                        {
                                            chiTietYLenhXoa.WillDelete = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #region Xử lý diễn biến bị trùng
                    var dienBiens = benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == phieuDieuTri.NgayDieuTri.Date 
                                                                                         && x.WillDelete != true).ToList();
                    foreach (var dienBien in dienBiens)
                    {
                        if (dienBien.WillDelete != true)
                        {
                            //var lstDienBienTrungGioDienBien = benhAn.NoiTruPhieuDieuTriChiTietDienBiens
                            //    .Where(x => x.NgayDieuTri == dienBien.NgayDieuTri
                            //                && x.Id != dienBien.Id
                            //                && x.GioDienBien == dienBien.GioDienBien
                            //                && x.WillDelete != true)
                            //    .ToList();
                            var lstDienBienTrungGioDienBien = dienBiens
                                .Where(x => x.Id != dienBien.Id
                                            && x.GioDienBien == dienBien.GioDienBien)
                                .ToList();
                            if (lstDienBienTrungGioDienBien.Any())
                            {
                                isChange = true;
                                var lstDienBien = new List<string>();
                                lstDienBien.Add(dienBien.MoTaDienBien);
                                foreach (var dienBienTrung in lstDienBienTrungGioDienBien)
                                {
                                    dienBienTrung.WillDelete = true;
                                    lstDienBien.Add((dienBienTrung.MoTaDienBien ?? "").Trim());
                                }
                                dienBien.MoTaDienBien = string.Join(". ", lstDienBien.Distinct().ToList());
                            }
                        }
                    }
                    #endregion
                }

                #region BVHD-3575: Cập nhật y lệnh đối với bệnh án có chỉ định dv khám bệnh từ nội trú
                if (benhAn.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                {
                    //11/08/2022: fix bug load thiếu y lệnh so với thời gian tổng hợp -> bổ sung thêm trường hợp dịch vụ trước thời điểm tổng hợp (trạng thái khác hủy) mà chưa có trong y lệnh
                    var lstYCKhamIdDaCoYLenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs
                        .Where(x => x.YeuCauKhamBenhId != null)
                        .Select(x => x.YeuCauKhamBenhId.Value)
                        .Distinct().ToList();
                    var lstNgayDieuTri = lstPhieuDieuTri.Select(a => a.NgayDieuTri.Date).Distinct().ToList();
                    var yeuCauDichVuKhams = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhanId == benhAn.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                                    && x.LaChiDinhTuNoiTru != null
                                    && x.LaChiDinhTuNoiTru == true
                                    && lstNgayDieuTri.Contains(x.ThoiDiemDangKy.Date)
                                    && (
                                        benhAn.ThoiDiemTongHopYLenhKhamBenh == null
                                        || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhKhamBenh
                                        || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhKhamBenh
                                        || (x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && lstYCKhamIdDaCoYLenh.All(a => a != x.Id))
                                    ))
                        .ToList();

                    foreach (var ngayDieuTriNoiTru in lstNgayDieuTri)
                    {
                        var dichVuKhamChanges = yeuCauDichVuKhams.Where(x => x.ThoiDiemDangKy.Date == ngayDieuTriNoiTru.Date).ToList();
                        if (dichVuKhamChanges.Any())
                        {
                            isChange = true;
                            foreach (var dichVu in dichVuKhamChanges)
                            {
                                var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauKhamBenhId == dichVu.Id);

                                if (chiTietYlenh == null)
                                {
                                    if (dichVu.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                    {
                                        var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        {
                                            YeuCauKhamBenhId = dichVu.Id,
                                            MoTaYLenh = dichVu.TenDichVu,
                                            GioYLenh = gioDienBien,
                                            NhanVienChiDinhId = dichVu.NhanVienChiDinhId,
                                            NoiChiDinhId = dichVu.NoiChiDinhId.Value,
                                            XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham,
                                            ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien,
                                            NhanVienXacNhanThucHienId = dichVu.BacSiKetLuanId ?? dichVu.BacSiThucHienId,
                                            LyDoKhongThucHien = null,
                                            ThoiDiemCapNhat = dichVu.LastTime,
                                            NhanVienCapNhatId = dichVu.LastUserId,
                                            NgayDieuTri = ngayDieuTriNoiTru.Date
                                        });
                                    }
                                }
                                else if (dichVu.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                {
                                    chiTietYlenh.WillDelete = true;
                                }
                                else
                                {
                                    var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;

                                    chiTietYlenh.GioYLenh = gioDienBien;
                                    chiTietYlenh.XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
                                    chiTietYlenh.ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien;
                                    chiTietYlenh.NhanVienXacNhanThucHienId = dichVu.BacSiKetLuanId ?? dichVu.BacSiThucHienId;
                                    chiTietYlenh.ThoiDiemCapNhat = dichVu.LastTime;
                                    chiTietYlenh.NhanVienCapNhatId = dichVu.LastUserId;
                                }
                            }
                        }

                        #region Xử lý diễn biến bị trùng
                        var dienBiens = benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == ngayDieuTriNoiTru.Date
                                                                                             && x.WillDelete != true).ToList();
                        foreach (var dienBien in dienBiens)
                        {
                            if (dienBien.WillDelete != true)
                            {
                                var lstDienBienTrungGioDienBien = dienBiens
                                    .Where(x => x.Id != dienBien.Id
                                                && x.GioDienBien == dienBien.GioDienBien)
                                    .ToList();
                                if (lstDienBienTrungGioDienBien.Any())
                                {
                                    isChange = true;
                                    var lstDienBien = new List<string>();
                                    lstDienBien.Add(dienBien.MoTaDienBien);
                                    foreach (var dienBienTrung in lstDienBienTrungGioDienBien)
                                    {
                                        dienBienTrung.WillDelete = true;
                                        lstDienBien.Add((dienBienTrung.MoTaDienBien ?? "").Trim());
                                    }
                                    dienBien.MoTaDienBien = string.Join(". ", lstDienBien.Distinct().ToList());
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                if (isChange)
                {
                    #region Xử lý kiểm tra diễn biến tương ứng với y lệnh
                    //foreach (var yLenh in phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => !x.WillDelete))
                    foreach (var yLenh in benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        //if (!phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.Any(x => x.GioDienBien == yLenh.GioYLenh && x.WillDelete != true))
                        if (!benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Any(x => x.GioDienBien == yLenh.GioYLenh
                                                                                && x.NgayDieuTri.Date == yLenh.NgayDieuTri.Date
                                                                                && x.WillDelete != true))
                        {
                            isChange = true;
                            //phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.Add(new NoiTruPhieuDieuTriChiTietDienBien()
                            benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(new NoiTruPhieuDieuTriChiTietDienBien()
                            {
                                MoTaDienBien = "",
                                GioDienBien = yLenh.GioYLenh,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = yLenh.NgayDieuTri
                            });
                        }
                    }

                    //foreach (var dienBien in phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => !x.WillDelete))
                    foreach (var dienBien in benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        //if (!phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien && x.WillDelete != true))
                        if (!benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien
                                                                             && x.NgayDieuTri.Date == dienBien.NgayDieuTri.Date
                                                                             && x.WillDelete != true))
                        {
                            isChange = true;
                            dienBien.WillDelete = true;
                        }
                    }
                    #endregion
                }

                if (isChange || benhAn.ThoiDiemTongHopYLenhDichVuKyThuat == null 
                             || benhAn.ThoiDiemTongHopYLenhTruyenMau == null 
                             || benhAn.ThoiDiemTongHopYLenhVatTu == null 
                             || benhAn.ThoiDiemTongHopYLenhDuocPham == null
                             
                             //BVHD-3575
                             || benhAn.ThoiDiemTongHopYLenhKhamBenh == null
                             )
                {
                    if (ngayDieuTri == null)
                    {
                        benhAn.ThoiDiemTongHopYLenhDichVuKyThuat = thoiDiemTongHop; //DateTime.Now;
                        benhAn.ThoiDiemTongHopYLenhTruyenMau = thoiDiemTongHop; //DateTime.Now;
                        benhAn.ThoiDiemTongHopYLenhVatTu = thoiDiemTongHop; //DateTime.Now;
                        benhAn.ThoiDiemTongHopYLenhDuocPham = thoiDiemTongHop; //DateTime.Now;

                        //BVHD-3575
                        benhAn.ThoiDiemTongHopYLenhKhamBenh = thoiDiemTongHop;
                    }
                    _noiTruBenhAnRepository.Context.SaveChanges();
                }
            }
        }

        public void XuLyTongHopYLenhDichVuKyThuat(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null)
        {
            // chỉ kiểm tra đối với trường hợp chạy job tổng hợp
            long benhAnIdPrev = 0;
            int take = 25;
            XDocument data = null;
            XElement benhAnElement = null;
            var path = @"Resource\\TongHopYLenhDichVuKyThuatBenhAnCuoiCung.xml";
            if (noiTruBenhAnId == null)
            {
                if (File.Exists(path))
                {
                    data = XDocument.Load(path);
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                    benhAnIdPrev = (long)benhAnElement.Element(root + "BenhAnId");
                }
                else
                {
                    data =
                        new XDocument(
                            new XElement("TongHopYLenh",
                                new XElement("BenhAnId", benhAnIdPrev.ToString())
                            )
                        );
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                }
            }

            var noiTruBenhAnIds = _noiTruBenhAnRepository.TableNoTracking
                .Where(x => 
                    //(x.ThoiDiemTongHopYLenhDichVuKyThuat == null
                    //         || (x.NoiTruPhieuDieuTris.Any(y => y.YeuCauDichVuKyThuats.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhDichVuKyThuat
                    //                                                                         || z.LastTime.Value >= x.ThoiDiemTongHopYLenhDichVuKyThuat))))
                    x.DaQuyetToan != true  
                    && (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
                    && x.Id > benhAnIdPrev)
                .Select(x => x.Id)
                .Take(take)
                .ToList();
            if (noiTruBenhAnId == null)
            {
                if (noiTruBenhAnIds.Count < take)
                {
                    benhAnIdPrev = 0;
                }
                else
                {
                    benhAnIdPrev = noiTruBenhAnIds.OrderByDescending(x => x).First();
                }

                benhAnElement.Element("BenhAnId").Value = benhAnIdPrev.ToString();
                data.Save(path);
            }

            // bổ sung 14/03/2022: gán mặc định giây = 0
            var currentDateTime = DateTime.Now;
            var thoiDiemTongHop = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, 0);

            foreach (var benhAnId in noiTruBenhAnIds)
            {
                // bổ sung 14/02/2022: gán thời điểm tổng hợp = thời điểm get data để tránh trường hợp đang tổng hợp thì có chỉ định mới
                //var thoiDiemTongHop = DateTime.Now;

                var benhAn = _noiTruBenhAnRepository.GetById(benhAnId, 
                    a => a.Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                                .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                                .Include(y => y.NoiTruPhieuDieuTris));
                var lstPhieuDieuTri = benhAn.NoiTruPhieuDieuTris.Where(x => ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date).ToList();

                var isChange = false;

                var lstPhieuDieuTriId = lstPhieuDieuTri.Select(a => a.Id).ToList();
                var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.NoiTruPhieuDieuTriId != null
                                && lstPhieuDieuTriId.Contains(x.NoiTruPhieuDieuTriId.Value)
                                && (
                                    benhAn.ThoiDiemTongHopYLenhDichVuKyThuat == null
                                    || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhDichVuKyThuat
                                    || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhDichVuKyThuat
                                ))
                    .ToList();
                foreach (var phieuDieuTri in lstPhieuDieuTri)
                {
                    // dịch vụ kỹ thuật
                    //var dichVuKyThuatChanges = phieuDieuTri.YeuCauDichVuKyThuats.Where(x => benhAn.ThoiDiemTongHopYLenhDichVuKyThuat == null
                    //                                                                        || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhDichVuKyThuat
                    //                                                                        || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhDichVuKyThuat).ToList();

                    var dichVuKyThuatChanges = yeuCauDichVuKyThuats.Where(x => x.NoiTruPhieuDieuTriId == phieuDieuTri.Id).ToList();
                    if (dichVuKyThuatChanges.Any())
                    {
                        isChange = true;
                        foreach (var dichVu in dichVuKyThuatChanges)
                        {
                            var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauDichVuKyThuatId == dichVu.Id);

                            if (chiTietYlenh == null)
                            {
                                if (dichVu.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                {
                                    if (dichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SuatAn)
                                    {
                                        var gioDienBien = 0;
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        {
                                            YeuCauDichVuKyThuatId = dichVu.Id,
                                            MoTaYLenh = dichVu.TenDichVu,
                                            GioYLenh = gioDienBien,
                                            NhanVienChiDinhId = dichVu.NhanVienChiDinhId,
                                            NoiChiDinhId = dichVu.NoiChiDinhId.Value,
                                            XacNhanThucHien = null,
                                            ThoiDiemXacNhanThucHien = null,
                                            NhanVienXacNhanThucHienId = null,
                                            LyDoKhongThucHien = null,
                                            ThoiDiemCapNhat = null,
                                            NhanVienCapNhatId = null,
                                            NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                        });
                                    }
                                    else
                                    {
                                        var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                        {
                                            YeuCauDichVuKyThuatId = dichVu.Id,
                                            MoTaYLenh = dichVu.TenDichVu,
                                            GioYLenh = gioDienBien,
                                            NhanVienChiDinhId = dichVu.NhanVienChiDinhId,
                                            NoiChiDinhId = dichVu.NoiChiDinhId.Value,
                                            XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                                            ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien,
                                            NhanVienXacNhanThucHienId = dichVu.NhanVienKetLuanId ?? dichVu.NhanVienThucHienId,
                                            LyDoKhongThucHien = null,
                                            ThoiDiemCapNhat = dichVu.LastTime,
                                            NhanVienCapNhatId = dichVu.LastUserId,
                                            NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                        });
                                    }
                                }
                            }
                            else if (dichVu.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                            {
                                chiTietYlenh.WillDelete = true;
                            }
                            else
                            {
                                if (dichVu.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn)
                                {
                                    var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;

                                    chiTietYlenh.GioYLenh = gioDienBien;
                                    chiTietYlenh.XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                                    chiTietYlenh.ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien;
                                    chiTietYlenh.NhanVienXacNhanThucHienId = dichVu.NhanVienKetLuanId ?? dichVu.NhanVienThucHienId;
                                    chiTietYlenh.ThoiDiemCapNhat = dichVu.LastTime;
                                    chiTietYlenh.NhanVienCapNhatId = dichVu.LastUserId;
                                }
                            }
                        }
                    }

                    #region Xử lý diễn biến bị trùng
                    var dienBiens = benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == phieuDieuTri.NgayDieuTri.Date
                                                                                         && x.WillDelete != true).ToList();
                    foreach (var dienBien in dienBiens)
                    {
                        if (dienBien.WillDelete != true)
                        {
                            var lstDienBienTrungGioDienBien = dienBiens
                                .Where(x => x.Id != dienBien.Id
                                            && x.GioDienBien == dienBien.GioDienBien)
                                .ToList();
                            if (lstDienBienTrungGioDienBien.Any())
                            {
                                isChange = true;
                                var lstDienBien = new List<string>();
                                lstDienBien.Add(dienBien.MoTaDienBien);
                                foreach (var dienBienTrung in lstDienBienTrungGioDienBien)
                                {
                                    dienBienTrung.WillDelete = true;
                                    lstDienBien.Add((dienBienTrung.MoTaDienBien ?? "").Trim());
                                }
                                dienBien.MoTaDienBien = string.Join(". ", lstDienBien.Distinct().ToList());
                            }
                        }
                    }
                    #endregion
                }

                if (isChange)
                {
                    #region Xử lý kiểm tra diễn biến tương ứng với y lệnh
                    foreach (var yLenh in benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Any(x => x.GioDienBien == yLenh.GioYLenh
                                                                                && x.NgayDieuTri.Date == yLenh.NgayDieuTri.Date
                                                                                && x.WillDelete != true))
                        {
                            isChange = true;
                            benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(new NoiTruPhieuDieuTriChiTietDienBien()
                            {
                                MoTaDienBien = "",
                                GioDienBien = yLenh.GioYLenh,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = yLenh.NgayDieuTri
                            });
                        }
                    }
                    
                    foreach (var dienBien in benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien
                                                                             && x.NgayDieuTri.Date == dienBien.NgayDieuTri.Date
                                                                             && x.WillDelete != true))
                        {
                            isChange = true;
                            dienBien.WillDelete = true;
                        }
                    }
                    #endregion
                }

                if (isChange || benhAn.ThoiDiemTongHopYLenhDichVuKyThuat == null)
                {
                    if (ngayDieuTri == null)
                    {
                        benhAn.ThoiDiemTongHopYLenhDichVuKyThuat = thoiDiemTongHop; //DateTime.Now;
                    }
                    _noiTruBenhAnRepository.Context.SaveChanges();
                }
            }
        }

        public void XuLyTongHopYLenhTruyenMau(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null)
        {
            // chỉ kiểm tra đối với trường hợp chạy job tổng hợp
            long benhAnIdPrev = 0;
            int take = 30;
            XDocument data = null;
            XElement benhAnElement = null;
            var path = @"Resource\\TongHopYLenhTruyenMauBenhAnCuoiCung.xml";
            if (noiTruBenhAnId == null)
            {
                if (File.Exists(path))
                {
                    data = XDocument.Load(path);
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                    benhAnIdPrev = (long)benhAnElement.Element(root + "BenhAnId");
                }
                else
                {
                    data =
                        new XDocument(
                            new XElement("TongHopYLenh",
                                new XElement("BenhAnId", benhAnIdPrev.ToString())
                            )
                        );
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                }
            }

            var noiTruBenhAnIds = _noiTruBenhAnRepository.TableNoTracking
                .Where(x => 
                    //(x.ThoiDiemTongHopYLenhTruyenMau == null
                    //         || (x.NoiTruPhieuDieuTris.Any(y => y.YeuCauTruyenMaus.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhTruyenMau
                    //                                                                        || z.LastTime.Value >= x.ThoiDiemTongHopYLenhTruyenMau))))
                            x.DaQuyetToan != true
                            && (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
                            && x.Id > benhAnIdPrev)
                .Select(x => x.Id)
                .Take(take)
                .ToList();
            if (noiTruBenhAnId == null)
            {
                if (noiTruBenhAnIds.Count < take)
                {
                    benhAnIdPrev = 0;
                }
                else
                {
                    benhAnIdPrev = noiTruBenhAnIds.OrderByDescending(x => x).First();
                }

                benhAnElement.Element("BenhAnId").Value = benhAnIdPrev.ToString();
                data.Save(path);
            }

            // bổ sung 14/03/2022: gán mặc định giây = 0
            var currentDateTime = DateTime.Now;
            var thoiDiemTongHop = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, 0);
            foreach (var benhAnId in noiTruBenhAnIds)
            {
                // bổ sung 14/02/2022: gán thời điểm tổng hợp = thời điểm get data để tránh trường hợp đang tổng hợp thì có chỉ định mới
                //var thoiDiemTongHop = DateTime.Now;

                var benhAn = _noiTruBenhAnRepository.GetById(benhAnId, 
                    a => a.Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                                .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                                .Include(y => y.NoiTruPhieuDieuTris));
                var lstPhieuDieuTri = benhAn.NoiTruPhieuDieuTris.Where(x => ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date).ToList();

                var isChange = false;

                var lstPhieuDieuTriId = lstPhieuDieuTri.Select(a => a.Id).ToList();
                var yeuCauTruyenMaus = _yeuCauTruyenMauRepository.TableNoTracking
                    .Where(x => lstPhieuDieuTriId.Contains(x.NoiTruPhieuDieuTriId)
                    && (
                        benhAn.ThoiDiemTongHopYLenhTruyenMau == null
                        || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhTruyenMau
                        || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhTruyenMau
                    ))
                    .ToList();
                foreach (var phieuDieuTri in lstPhieuDieuTri)
                {
                    // truyền máu
                    //var truyenMauChanges = phieuDieuTri.YeuCauTruyenMaus.Where(x => benhAn.ThoiDiemTongHopYLenhTruyenMau == null
                    //                                                                || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhTruyenMau
                    //                                                                || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhTruyenMau).ToList();
                    var truyenMauChanges = yeuCauTruyenMaus.Where(x => x.NoiTruPhieuDieuTriId == phieuDieuTri.Id).ToList();
                    if (truyenMauChanges.Any())
                    {
                        isChange = true;
                        foreach (var truyenMau in truyenMauChanges)
                        {
                            var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauTruyenMauId == truyenMau.Id);
                            if (chiTietYlenh == null)
                            {
                                if (truyenMau.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy)
                                {
                                    var gioDienBien = truyenMau.ThoiGianBatDauTruyen ?? 0; //todo: nếu ko có thời gian bắt đầu truyền, gán mặc định là 0h
                                    benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    {
                                        YeuCauTruyenMauId = truyenMau.Id,
                                        MoTaYLenh = truyenMau.TenDichVu,
                                        GioYLenh = gioDienBien,
                                        NhanVienChiDinhId = truyenMau.NhanVienChiDinhId,
                                        NoiChiDinhId = truyenMau.NoiChiDinhId,
                                        XacNhanThucHien = truyenMau.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien,
                                        ThoiDiemXacNhanThucHien = truyenMau.ThoiDiemThucHien,
                                        NhanVienXacNhanThucHienId = truyenMau.NhanVienThucHienId,
                                        LyDoKhongThucHien = null,
                                        ThoiDiemCapNhat = truyenMau.LastTime,
                                        NhanVienCapNhatId = truyenMau.LastUserId,
                                        NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                    });
                                }
                            }
                            else if (truyenMau.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy)
                            {
                                chiTietYlenh.WillDelete = true;
                            }
                            else
                            {
                                var gioDienBien = truyenMau.ThoiGianBatDauTruyen ?? 0; //todo: nếu ko có thời gian bắt đầu truyền, gán mặc định là 0h

                                chiTietYlenh.GioYLenh = gioDienBien;
                                chiTietYlenh.XacNhanThucHien = truyenMau.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien;
                                chiTietYlenh.ThoiDiemXacNhanThucHien = truyenMau.ThoiDiemThucHien;
                                chiTietYlenh.NhanVienXacNhanThucHienId = truyenMau.NhanVienThucHienId;
                                chiTietYlenh.ThoiDiemCapNhat = truyenMau.LastTime;
                                chiTietYlenh.NhanVienCapNhatId = truyenMau.LastUserId;
                            }
                        }
                    }
                    #region Xử lý diễn biến bị trùng
                    var dienBiens = benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == phieuDieuTri.NgayDieuTri.Date
                                                                                         && x.WillDelete != true).ToList();
                    foreach (var dienBien in dienBiens)
                    {
                        if (dienBien.WillDelete != true)
                        {
                            var lstDienBienTrungGioDienBien = dienBiens
                                .Where(x => x.Id != dienBien.Id
                                            && x.GioDienBien == dienBien.GioDienBien)
                                .ToList();
                            if (lstDienBienTrungGioDienBien.Any())
                            {
                                isChange = true;
                                var lstDienBien = new List<string>();
                                lstDienBien.Add(dienBien.MoTaDienBien);
                                foreach (var dienBienTrung in lstDienBienTrungGioDienBien)
                                {
                                    dienBienTrung.WillDelete = true;
                                    lstDienBien.Add((dienBienTrung.MoTaDienBien ?? "").Trim());
                                }
                                dienBien.MoTaDienBien = string.Join(". ", lstDienBien.Distinct().ToList());
                            }
                        }
                    }
                    #endregion
                }

                if (isChange)
                {
                    #region Xử lý kiểm tra diễn biến tương ứng với y lệnh
                    foreach (var yLenh in benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Any(x => x.GioDienBien == yLenh.GioYLenh
                                                                                && x.NgayDieuTri.Date == yLenh.NgayDieuTri.Date
                                                                                && x.WillDelete != true))
                        {
                            isChange = true;
                            benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(new NoiTruPhieuDieuTriChiTietDienBien()
                            {
                                MoTaDienBien = "",
                                GioDienBien = yLenh.GioYLenh,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = yLenh.NgayDieuTri
                            });
                        }
                    }
                    
                    foreach (var dienBien in benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien
                                                                             && x.NgayDieuTri.Date == dienBien.NgayDieuTri.Date
                                                                             && x.WillDelete != true))
                        {
                            isChange = true;
                            dienBien.WillDelete = true;
                        }
                    }
                    #endregion
                }

                if (isChange || benhAn.ThoiDiemTongHopYLenhTruyenMau == null)
                {
                    if (ngayDieuTri == null)
                    {
                        benhAn.ThoiDiemTongHopYLenhTruyenMau = thoiDiemTongHop; //DateTime.Now;
                    }
                    _noiTruBenhAnRepository.Context.SaveChanges();
                }
            }
        }

        public void XuLyTongHopYLenhVatTu(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null)
        {
            // chỉ kiểm tra đối với trường hợp chạy job tổng hợp
            long benhAnIdPrev = 0;
            int take = 25;
            XDocument data = null;
            XElement benhAnElement = null;
            var path = @"Resource\\TongHopYLenhVatTuBenhAnCuoiCung.xml";
            if (noiTruBenhAnId == null)
            {
                if (File.Exists(path))
                {
                    data = XDocument.Load(path);
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                    benhAnIdPrev = (long)benhAnElement.Element(root + "BenhAnId");
                }
                else
                {
                    data =
                        new XDocument(
                            new XElement("TongHopYLenh",
                                new XElement("BenhAnId", benhAnIdPrev.ToString())
                            )
                        );
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                }
            }

            var noiTruBenhAnIds = _noiTruBenhAnRepository.TableNoTracking
                .Where(x =>
                    //(x.ThoiDiemTongHopYLenhVatTu == null
                    //         || (x.NoiTruPhieuDieuTris.Any(y => y.YeuCauVatTuBenhViens.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhVatTu
                    //                                                                            || z.LastTime.Value >= x.ThoiDiemTongHopYLenhVatTu))))
                    x.DaQuyetToan != true
                    && (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
                    && x.Id > benhAnIdPrev)
                .Select(x => x.Id)
                .Take(take)
                .ToList();
            if (noiTruBenhAnId == null)
            {
                if (noiTruBenhAnIds.Count < take)
                {
                    benhAnIdPrev = 0;
                }
                else
                {
                    benhAnIdPrev = noiTruBenhAnIds.OrderByDescending(x => x).First();
                }

                benhAnElement.Element("BenhAnId").Value = benhAnIdPrev.ToString();
                data.Save(path);
            }

            // bổ sung 14/03/2022: gán mặc định giây = 0
            var currentDateTime = DateTime.Now;
            var thoiDiemTongHop = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, 0);

            foreach (var benhAnId in noiTruBenhAnIds)
            {
                // bổ sung 14/02/2022: gán thời điểm tổng hợp = thời điểm get data để tránh trường hợp đang tổng hợp thì có chỉ định mới
                //var thoiDiemTongHop = DateTime.Now;

                var benhAn = _noiTruBenhAnRepository.GetById(benhAnId, a => a.Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                    .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                    .Include(y => y.NoiTruPhieuDieuTris));
                var lstPhieuDieuTri = benhAn.NoiTruPhieuDieuTris.Where(x => ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date).ToList();

                var isChange = false;

                var lstPhieuDieuTriId = lstPhieuDieuTri.Select(x => x.Id).ToList();
                var yeuCauVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Include(x => x.XuatKhoVatTuChiTiet).ThenInclude(x => x.XuatKhoVatTu)
                    .Where(x => x.NoiTruPhieuDieuTriId != null 
                                && lstPhieuDieuTriId.Contains(x.NoiTruPhieuDieuTriId.Value)
                                && (
                                    benhAn.ThoiDiemTongHopYLenhVatTu == null
                                    || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhVatTu
                                    || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhVatTu
                                ))
                    .ToList();
                foreach (var phieuDieuTri in lstPhieuDieuTri)
                {
                    // vật tư bệnh viện
                    //var yeuCauVatTuBenhVienChanges = phieuDieuTri.YeuCauVatTuBenhViens.Where(x => benhAn.ThoiDiemTongHopYLenhVatTu == null
                    //                                                                              || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhVatTu
                    //                                                                              || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhVatTu).ToList();
                    var yeuCauVatTuBenhVienChanges = yeuCauVatTus.Where(x => x.NoiTruPhieuDieuTriId == phieuDieuTri.Id).ToList();
                    if (yeuCauVatTuBenhVienChanges.Any())
                    {
                        isChange = true;
                        foreach (var yeuCauVatTuBenhVien in yeuCauVatTuBenhVienChanges)
                        {
                            //var chiTietYlenh = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauVatTuBenhVienId == yeuCauVatTuBenhVien.Id);
                            var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauVatTuBenhVienId == yeuCauVatTuBenhVien.Id);
                            if (chiTietYlenh == null)
                            {
                                if (yeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                                {
                                    //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    {
                                        YeuCauVatTuBenhVienId = yeuCauVatTuBenhVien.Id,
                                        MoTaYLenh = yeuCauVatTuBenhVien.Ten,
                                        GioYLenh = 0, // vì ko có giờ y lệnh nên sẽ lấy mặc định là 0h
                                                      //yeuCauVatTuBenhVien.ThoiDiemChiDinh.Hour * 3600 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Minute * 60 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Second, //todo: cần kiểm tra lại thời gian y lệnh
                                        NhanVienChiDinhId = yeuCauVatTuBenhVien.NhanVienChiDinhId,
                                        NoiChiDinhId = yeuCauVatTuBenhVien.NoiChiDinhId,
                                        XacNhanThucHien = yeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien,
                                        ThoiDiemXacNhanThucHien = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NgayXuat,
                                        NhanVienXacNhanThucHienId = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NguoiXuatId,
                                        LyDoKhongThucHien = null,
                                        ThoiDiemCapNhat = yeuCauVatTuBenhVien.LastTime,
                                        NhanVienCapNhatId = yeuCauVatTuBenhVien.LastUserId,
                                        NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                                    });
                                }
                            }
                            else if (yeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                            {
                                chiTietYlenh.WillDelete = true;
                            }
                            else
                            {
                                chiTietYlenh.GioYLenh = 0; // vì ko có giờ y lệnh nên sẽ lấy mặc định là 0h
                                                           //yeuCauVatTuBenhVien.ThoiDiemChiDinh.Hour * 3600 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Minute * 60 + yeuCauVatTuBenhVien.ThoiDiemChiDinh.Second; //todo: cần kiểm tra lại thời gian y lệnh
                                chiTietYlenh.XacNhanThucHien = yeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien;
                                chiTietYlenh.ThoiDiemXacNhanThucHien = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NgayXuat;
                                chiTietYlenh.NhanVienXacNhanThucHienId = yeuCauVatTuBenhVien.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.NguoiXuatId;
                                chiTietYlenh.ThoiDiemCapNhat = yeuCauVatTuBenhVien.LastTime;
                                chiTietYlenh.NhanVienCapNhatId = yeuCauVatTuBenhVien.LastUserId;
                            }
                        }
                    }

                    #region Xử lý diễn biến bị trùng
                    var dienBiens = benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == phieuDieuTri.NgayDieuTri.Date
                                                                                         && x.WillDelete != true).ToList();
                    foreach (var dienBien in dienBiens)
                    {
                        if (dienBien.WillDelete != true)
                        {
                            var lstDienBienTrungGioDienBien = dienBiens
                                .Where(x => x.Id != dienBien.Id
                                            && x.GioDienBien == dienBien.GioDienBien)
                                .ToList();
                            if (lstDienBienTrungGioDienBien.Any())
                            {
                                isChange = true;
                                var lstDienBien = new List<string>();
                                lstDienBien.Add(dienBien.MoTaDienBien);
                                foreach (var dienBienTrung in lstDienBienTrungGioDienBien)
                                {
                                    dienBienTrung.WillDelete = true;
                                    lstDienBien.Add((dienBienTrung.MoTaDienBien ?? "").Trim());
                                }
                                dienBien.MoTaDienBien = string.Join(". ", lstDienBien.Distinct().ToList());
                            }
                        }
                    }
                    #endregion
                }

                if (isChange)
                {
                    #region Xử lý kiểm tra diễn biến tương ứng với y lệnh
                    foreach (var yLenh in benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Any(x => x.GioDienBien == yLenh.GioYLenh
                                                                                && x.NgayDieuTri.Date == yLenh.NgayDieuTri.Date
                                                                                && x.WillDelete != true))
                        {
                            isChange = true;
                            benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(new NoiTruPhieuDieuTriChiTietDienBien()
                            {
                                MoTaDienBien = "",
                                GioDienBien = yLenh.GioYLenh,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = yLenh.NgayDieuTri
                            });
                        }
                    }
                    
                    foreach (var dienBien in benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien
                                                                             && x.NgayDieuTri.Date == dienBien.NgayDieuTri.Date
                                                                             && x.WillDelete != true))
                        {
                            isChange = true;
                            dienBien.WillDelete = true;
                        }
                    }
                    #endregion
                }

                if (isChange || benhAn.ThoiDiemTongHopYLenhVatTu == null)
                {
                    if (ngayDieuTri == null)
                    {
                        benhAn.ThoiDiemTongHopYLenhVatTu = thoiDiemTongHop; //DateTime.Now;
                    }
                    _noiTruBenhAnRepository.Context.SaveChanges();
                }
            }
        }

        public void XuLyTongHopYLenhDuocPham(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null)
        {
            //2: kiểm tra lại thông tin xác nhận thực hiện của: yc dvkt, yc vật tư, yc truyền máu
            #region Thời gian mặc định trong ngày

            var cauHinhThoiGianMacDinh = _cauHinhRepository.TableNoTracking.First(x => x.Name == "CauHinhNoiTru.ThoiGianMacDinhTrongNgay");
            var thoiGianMacDinhs = JsonConvert.DeserializeObject<List<LookupItemCauHinhVo>>(cauHinhThoiGianMacDinh.Value);
            var sang = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Sang).Value);
            var trua = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Trua).Value);
            var chieu = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Chieu).Value);
            var toi = int.Parse(thoiGianMacDinhs.First(x => int.Parse(x.KeyId) == (int)Enums.ThoiGianMacDinhTrongNgay.Toi).Value);

            #endregion

            // chỉ kiểm tra đối với trường hợp chạy job tổng hợp
            long benhAnIdPrev = 0;
            int take = 25;
            XDocument data = null;
            XElement benhAnElement = null;
            var path = @"Resource\\TongHopYLenhDuocPhamBenhAnCuoiCung.xml";
            if (noiTruBenhAnId == null)
            {
                if (File.Exists(path))
                {
                    data = XDocument.Load(path);
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                    benhAnIdPrev = (long)benhAnElement.Element(root + "BenhAnId");
                }
                else
                {
                    data =
                        new XDocument(
                            new XElement("TongHopYLenh",
                                new XElement("BenhAnId", benhAnIdPrev.ToString())
                            )
                        );
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                }
            }

            var noiTruBenhAnIds = _noiTruBenhAnRepository.TableNoTracking
                .Where(x =>
                    //(x.ThoiDiemTongHopYLenhDuocPham == null
                    //         || (x.NoiTruPhieuDieuTris.Any(y => y.NoiTruChiDinhDuocPhams.Any(z => z.CreatedOn.Value >= x.ThoiDiemTongHopYLenhDuocPham
                    //                                                                              || z.LastTime.Value >= x.ThoiDiemTongHopYLenhDuocPham))))
                    x.DaQuyetToan != true
                    && (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
                    && x.Id > benhAnIdPrev)
                .Select(x => x.Id)
                .Take(take)
                .ToList();
            if (noiTruBenhAnId == null)
            {
                if (noiTruBenhAnIds.Count < take)
                {
                    benhAnIdPrev = 0;
                }
                else
                {
                    benhAnIdPrev = noiTruBenhAnIds.OrderByDescending(x => x).First();
                }

                benhAnElement.Element("BenhAnId").Value = benhAnIdPrev.ToString();
                data.Save(path);
            }

            // bổ sung 14/03/2022: gán mặc định giây = 0
            var currentDateTime = DateTime.Now;
            var thoiDiemTongHop = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, 0);

            foreach (var benhAnId in noiTruBenhAnIds)
            {
                // bổ sung 14/02/2022: gán thời điểm tổng hợp = thời điểm get data để tránh trường hợp đang tổng hợp thì có chỉ định mới
                //var thoiDiemTongHop = DateTime.Now;

                var benhAn = _noiTruBenhAnRepository.GetById(benhAnId, a => a.Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                    .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                    .Include(x => x.NoiTruPhieuDieuTris));
                var lstPhieuDieuTri = benhAn.NoiTruPhieuDieuTris.Where(x => ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date).ToList();

                var isChange = false;
                var lstPhieuDieuTriId = lstPhieuDieuTri.Select(x => x.Id).ToList();
                var yeuCauDuocPhams = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                    .Where(x => x.NoiTruPhieuDieuTriId != null
                                && lstPhieuDieuTriId.Contains(x.NoiTruPhieuDieuTriId.Value)
                                && (
                                    benhAn.ThoiDiemTongHopYLenhDuocPham == null
                                    || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhDuocPham
                                    || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhDuocPham
                                ))
                    .ToList();
                foreach (var phieuDieuTri in lstPhieuDieuTri)
                {
                    // dược phẩm
                    //var noiTruChiDinhDuocPhamChanges = phieuDieuTri.NoiTruChiDinhDuocPhams.Where(x => benhAn.ThoiDiemTongHopYLenhDuocPham == null
                    //                                                                                  || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhDuocPham
                    //                                                                                  || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhDuocPham).ToList();

                    var noiTruChiDinhDuocPhamChanges = yeuCauDuocPhams.Where(x => x.NoiTruPhieuDieuTriId == phieuDieuTri.Id).ToList();
                    if (noiTruChiDinhDuocPhamChanges.Any())
                    {
                        isChange = true;
                        foreach (var noiTruChiDinhDuocPham in noiTruChiDinhDuocPhamChanges)
                        {
                            var sangTemp = noiTruChiDinhDuocPham.ThoiGianDungSang ?? (noiTruChiDinhDuocPham.DungSang == null ? (int?)null : sang);
                            var truaTemp = noiTruChiDinhDuocPham.ThoiGianDungTrua ?? (noiTruChiDinhDuocPham.DungTrua == null ? (int?)null : trua);
                            var chieuTemp = noiTruChiDinhDuocPham.ThoiGianDungChieu ?? (noiTruChiDinhDuocPham.DungChieu == null ? (int?)null : chieu);
                            var toiTemp = noiTruChiDinhDuocPham.ThoiGianDungToi ?? (noiTruChiDinhDuocPham.DungToi == null ? (int?)null : toi);
                            if (sangTemp == null && truaTemp == null && chieuTemp == null && toiTemp == null)
                            {
                                sangTemp = sang;
                            }

                            var soLanDungTrongNgayTemp = noiTruChiDinhDuocPham.SoLanDungTrongNgay ?? 1;
                            var cachGioTruyenDichTemp = noiTruChiDinhDuocPham.CachGioTruyenDich ?? 0;
                            if (noiTruChiDinhDuocPham.CachGioTruyenDich == null)
                            {
                                soLanDungTrongNgayTemp = 1;
                            }

                            var chiTietYLenhTemp = new NoiTruPhieuDieuTriChiTietYLenh()
                            {
                                NoiTruChiDinhDuocPhamId = noiTruChiDinhDuocPham.Id,
                                MoTaYLenh = noiTruChiDinhDuocPham.Ten,
                                GioYLenh = 0,
                                NhanVienChiDinhId = noiTruChiDinhDuocPham.NhanVienChiDinhId,
                                NoiChiDinhId = noiTruChiDinhDuocPham.NoiChiDinhId,
                                XacNhanThucHien = false,
                                ThoiDiemXacNhanThucHien = null,
                                NhanVienXacNhanThucHienId = null,
                                LyDoKhongThucHien = null,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = phieuDieuTri.NgayDieuTri.Date
                            };

                            //var chiTietYlenhs = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.NoiTruChiDinhDuocPhamId == noiTruChiDinhDuocPham.Id).ToList();
                            var chiTietYlenhs = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => x.NoiTruChiDinhDuocPhamId == noiTruChiDinhDuocPham.Id).ToList();
                            if (!chiTietYlenhs.Any())
                            {
                                if (noiTruChiDinhDuocPham.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                                {
                                    if (noiTruChiDinhDuocPham.LaDichTruyen != true)
                                    {
                                        if (sangTemp != null)//(noiTruChiDinhDuocPham.ThoiGianDungSang != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = sangTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungSang.Value;
                                                                                       //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                        if (truaTemp != null) //(noiTruChiDinhDuocPham.ThoiGianDungTrua != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = truaTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungTrua.Value;
                                                                                       //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                        if (chieuTemp != null) //(noiTruChiDinhDuocPham.ThoiGianDungChieu != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = chieuTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungChieu.Value;
                                                                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                        if (toiTemp != null) //(noiTruChiDinhDuocPham.ThoiGianDungToi != null)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = toiTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungToi.Value;
                                                                                      //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                    }
                                    else
                                    {
                                        var thoiGianBatDauTruyen = noiTruChiDinhDuocPham.ThoiGianBatDauTruyen ?? 0;
                                        //if (noiTruChiDinhDuocPham.SoLanDungTrongNgay == null)
                                        //{
                                        //    var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        //    chiTietYLenhNew.GioYLenh = thoiGianBatDauTruyen;
                                        //    phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        //}
                                        //else
                                        //{
                                        //    for (int i = 0; i < noiTruChiDinhDuocPham.SoLanDungTrongNgay; i++)
                                        //    {
                                        //        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        //        chiTietYLenhNew.GioYLenh = thoiGianBatDauTruyen + (int)(noiTruChiDinhDuocPham.CachGioTruyenDich.Value * 3600 * i).MathRoundNumber();
                                        //        phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        //    }
                                        //}

                                        for (int i = 0; i < soLanDungTrongNgayTemp; i++)
                                        {
                                            var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                            chiTietYLenhNew.GioYLenh = thoiGianBatDauTruyen + (int)(cachGioTruyenDichTemp * 3600 * i).MathRoundNumber();
                                            //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        }
                                    }
                                }
                            }
                            else if (noiTruChiDinhDuocPham.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                            {
                                foreach (var chiTietYlenh in chiTietYlenhs)
                                {
                                    chiTietYlenh.WillDelete = true;
                                }
                            }
                            else
                            {
                                foreach (var chiTietYlenh in chiTietYlenhs)
                                {
                                    if (noiTruChiDinhDuocPham.LaDichTruyen != true)
                                    {
                                        //if (chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungSang
                                        //    && chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungTrua
                                        //    && chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungChieu
                                        //    && chiTietYlenh.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungToi)
                                        if (chiTietYlenh.GioYLenh != sangTemp
                                            && chiTietYlenh.GioYLenh != truaTemp
                                            && chiTietYlenh.GioYLenh != chieuTemp
                                            && chiTietYlenh.GioYLenh != toiTemp)
                                        {
                                            chiTietYlenh.WillDelete = true;
                                        }
                                    }
                                    else
                                    {
                                        if (noiTruChiDinhDuocPham.SoLanDungTrongNgay != null)
                                        {
                                            var isExists = false;
                                            var thoiGianBatDauTruyen = noiTruChiDinhDuocPham.ThoiGianBatDauTruyen ?? 0;
                                            //for (int i = 0; i < noiTruChiDinhDuocPham.SoLanDungTrongNgay; i++)
                                            //{
                                            //    var gioYLenh = thoiGianBatDauTruyen + (int)(noiTruChiDinhDuocPham.CachGioTruyenDich.Value * 3600 * i).MathRoundNumber();
                                            //    if (chiTietYlenh.GioYLenh == gioYLenh)
                                            //    {
                                            //        isExists = true;
                                            //    }
                                            //}

                                            for (int i = 0; i < soLanDungTrongNgayTemp; i++)
                                            {
                                                var gioYLenh = thoiGianBatDauTruyen + (int)(cachGioTruyenDichTemp * 3600 * i).MathRoundNumber();
                                                if (chiTietYlenh.GioYLenh == gioYLenh)
                                                {
                                                    isExists = true;
                                                }
                                            }

                                            if (isExists == false)
                                            {
                                                chiTietYlenh.WillDelete = true;
                                            }
                                        }
                                    }
                                }

                                if (noiTruChiDinhDuocPham.LaDichTruyen != true)
                                {
                                    //if (noiTruChiDinhDuocPham.ThoiGianDungSang != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungSang))
                                    if (sangTemp != null && chiTietYlenhs.All(x => x.GioYLenh != sangTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = sangTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungSang.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }

                                    //if (noiTruChiDinhDuocPham.ThoiGianDungTrua != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungTrua))
                                    if (truaTemp != null && chiTietYlenhs.All(x => x.GioYLenh != truaTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = truaTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungTrua.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }

                                    //if (noiTruChiDinhDuocPham.ThoiGianDungChieu != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungChieu))
                                    if (toiTemp != null && chiTietYlenhs.All(x => x.GioYLenh != toiTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = toiTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungChieu.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }

                                    //if (noiTruChiDinhDuocPham.ThoiGianDungToi != null && chiTietYlenhs.All(x => x.GioYLenh != noiTruChiDinhDuocPham.ThoiGianDungToi))
                                    if (toiTemp != null && chiTietYlenhs.All(x => x.GioYLenh != toiTemp))
                                    {
                                        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        chiTietYLenhNew.GioYLenh = toiTemp.Value; //noiTruChiDinhDuocPham.ThoiGianDungToi.Value;
                                        //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                    }
                                }
                                else
                                {
                                    var thoiGianBatDauTruyen = noiTruChiDinhDuocPham.ThoiGianBatDauTruyen ?? 0;
                                    if (noiTruChiDinhDuocPham.SoLanDungTrongNgay != null)
                                    {
                                        //for (int i = 0; i < noiTruChiDinhDuocPham.SoLanDungTrongNgay; i++)
                                        //{
                                        //    var gioYLenh = thoiGianBatDauTruyen + (int)(noiTruChiDinhDuocPham.CachGioTruyenDich.Value * 3600 * i).MathRoundNumber();
                                        //    if (chiTietYlenhs.All(x => x.GioYLenh != gioYLenh))
                                        //    {
                                        //        var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                        //        chiTietYLenhNew.GioYLenh = gioYLenh;
                                        //        phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                        //    }
                                        //}

                                        for (int i = 0; i < soLanDungTrongNgayTemp; i++)
                                        {
                                            var gioYLenh = thoiGianBatDauTruyen + (int)(cachGioTruyenDichTemp * 3600 * i).MathRoundNumber();
                                            if (chiTietYlenhs.All(x => x.GioYLenh != gioYLenh))
                                            {
                                                var chiTietYLenhNew = chiTietYLenhTemp.Clone();
                                                chiTietYLenhNew.GioYLenh = gioYLenh;
                                                //phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                                benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(chiTietYLenhNew);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var chiTietYLenhXoa in chiTietYlenhs.Where(x => x.GioYLenh != thoiGianBatDauTruyen))
                                        {
                                            chiTietYLenhXoa.WillDelete = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #region Xử lý diễn biến bị trùng
                    var dienBiens = benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == phieuDieuTri.NgayDieuTri.Date
                                                                                         && x.WillDelete != true).ToList();
                    foreach (var dienBien in dienBiens)
                    {
                        if (dienBien.WillDelete != true)
                        {
                            var lstDienBienTrungGioDienBien = dienBiens
                                .Where(x => x.Id != dienBien.Id
                                            && x.GioDienBien == dienBien.GioDienBien)
                                .ToList();
                            if (lstDienBienTrungGioDienBien.Any())
                            {
                                isChange = true;
                                var lstDienBien = new List<string>();
                                lstDienBien.Add(dienBien.MoTaDienBien);
                                foreach (var dienBienTrung in lstDienBienTrungGioDienBien)
                                {
                                    dienBienTrung.WillDelete = true;
                                    lstDienBien.Add((dienBienTrung.MoTaDienBien ?? "").Trim());
                                }
                                dienBien.MoTaDienBien = string.Join(". ", lstDienBien.Distinct().ToList());
                            }
                        }
                    }
                    #endregion
                }

                if (isChange)
                {
                    #region Xử lý kiểm tra diễn biến tương ứng với y lệnh
                    foreach (var yLenh in benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Any(x => x.GioDienBien == yLenh.GioYLenh
                                                                                && x.NgayDieuTri.Date == yLenh.NgayDieuTri.Date
                                                                                && x.WillDelete != true))
                        {
                            isChange = true;
                            benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(new NoiTruPhieuDieuTriChiTietDienBien()
                            {
                                MoTaDienBien = "",
                                GioDienBien = yLenh.GioYLenh,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = yLenh.NgayDieuTri
                            });
                        }
                    }
                    
                    foreach (var dienBien in benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien
                                                                             && x.NgayDieuTri.Date == dienBien.NgayDieuTri.Date
                                                                             && x.WillDelete != true))
                        {
                            isChange = true;
                            dienBien.WillDelete = true;
                        }
                    }
                    #endregion
                }

                if (isChange || benhAn.ThoiDiemTongHopYLenhDuocPham == null)
                {
                    if (ngayDieuTri == null)
                    {
                        benhAn.ThoiDiemTongHopYLenhDuocPham = thoiDiemTongHop; //DateTime.Now;
                    }
                    _noiTruBenhAnRepository.Context.SaveChanges();
                }
            }
        }

        #region BVHD-3575
        public void XuLyTongHopYLenhKhamBenh(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null)
        {
            // chỉ kiểm tra đối với trường hợp chạy job tổng hợp
            long benhAnIdPrev = 0;
            int take = 25;
            XDocument data = null;
            XElement benhAnElement = null;
            var path = @"Resource\\TongHopYLenhDichVuKhamBenhAnCuoiCung.xml";
            if (noiTruBenhAnId == null)
            {
                if (File.Exists(path))
                {
                    data = XDocument.Load(path);
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                    benhAnIdPrev = (long)benhAnElement.Element(root + "BenhAnId");
                }
                else
                {
                    data =
                        new XDocument(
                            new XElement("TongHopYLenh",
                                new XElement("BenhAnId", benhAnIdPrev.ToString())
                            )
                        );
                    XNamespace root = data.Root.GetDefaultNamespace();
                    benhAnElement = data.Descendants(root + "TongHopYLenh").FirstOrDefault();
                }
            }

            var noiTruBenhAnIds = _noiTruBenhAnRepository.TableNoTracking
                .Where(x =>
                    x.DaQuyetToan != true
                    && (noiTruBenhAnId == null || x.Id == noiTruBenhAnId)
                    && x.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null
                    && x.Id > benhAnIdPrev)
                .Select(x => x.Id)
                .Take(take)
                .ToList();
            if (noiTruBenhAnId == null)
            {
                if (noiTruBenhAnIds.Count < take)
                {
                    benhAnIdPrev = 0;
                }
                else
                {
                    benhAnIdPrev = noiTruBenhAnIds.OrderByDescending(x => x).First();
                }

                benhAnElement.Element("BenhAnId").Value = benhAnIdPrev.ToString();
                data.Save(path);
            }

            // bổ sung 14/03/2022: gán mặc định giây = 0
            var currentDateTime = DateTime.Now;
            var thoiDiemTongHop = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, 0);

            foreach (var benhAnId in noiTruBenhAnIds)
            {
                var benhAn = _noiTruBenhAnRepository.GetById(benhAnId,
                    a => a.Include(y => y.NoiTruPhieuDieuTriChiTietYLenhs)
                                .Include(y => y.NoiTruPhieuDieuTriChiTietDienBiens)
                                .Include(y => y.NoiTruPhieuDieuTris)
                                .Include(y => y.YeuCauTiepNhan));
                var lstPhieuDieuTri = benhAn.NoiTruPhieuDieuTris.Where(x => ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date).ToList();

                var isChange = false;

                var lstNgayDieuTri = lstPhieuDieuTri.Select(a => a.NgayDieuTri.Date).Distinct().ToList();
                var yeuCauDichVuKhams = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == benhAn.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId 
                                && x.LaChiDinhTuNoiTru != null
                                && x.LaChiDinhTuNoiTru == true
                                && lstNgayDieuTri.Contains(x.ThoiDiemDangKy.Date)
                                && (
                                    benhAn.ThoiDiemTongHopYLenhKhamBenh == null
                                    || x.CreatedOn.Value >= benhAn.ThoiDiemTongHopYLenhKhamBenh
                                    || x.LastTime.Value >= benhAn.ThoiDiemTongHopYLenhKhamBenh
                                ))
                    .ToList();
                foreach (var ngayDieuTriNoiTru in lstNgayDieuTri)
                {
                    var dichVuKhamChanges = yeuCauDichVuKhams.Where(x => x.ThoiDiemDangKy.Date == ngayDieuTriNoiTru.Date).ToList();
                    if (dichVuKhamChanges.Any())
                    {
                        isChange = true;
                        foreach (var dichVu in dichVuKhamChanges)
                        {
                            var chiTietYlenh = benhAn.NoiTruPhieuDieuTriChiTietYLenhs.FirstOrDefault(x => x.YeuCauKhamBenhId == dichVu.Id);

                            if (chiTietYlenh == null)
                            {
                                if (dichVu.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                {
                                    var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;
                                    benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Add(new NoiTruPhieuDieuTriChiTietYLenh()
                                    {
                                        YeuCauKhamBenhId = dichVu.Id,
                                        MoTaYLenh = dichVu.TenDichVu,
                                        GioYLenh = gioDienBien,
                                        NhanVienChiDinhId = dichVu.NhanVienChiDinhId,
                                        NoiChiDinhId = dichVu.NoiChiDinhId.Value,
                                        XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham,
                                        ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien,
                                        NhanVienXacNhanThucHienId = dichVu.BacSiKetLuanId ?? dichVu.BacSiThucHienId,
                                        LyDoKhongThucHien = null,
                                        ThoiDiemCapNhat = dichVu.LastTime,
                                        NhanVienCapNhatId = dichVu.LastUserId,
                                        NgayDieuTri = ngayDieuTriNoiTru.Date
                                    });
                                }
                            }
                            else if (dichVu.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                            {
                                chiTietYlenh.WillDelete = true;
                            }
                            else
                            {
                                var gioDienBien = dichVu.ThoiDiemDangKy.Hour * 3600 + dichVu.ThoiDiemDangKy.Minute * 60;

                                chiTietYlenh.GioYLenh = gioDienBien;
                                chiTietYlenh.XacNhanThucHien = dichVu.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
                                chiTietYlenh.ThoiDiemXacNhanThucHien = dichVu.ThoiDiemHoanThanh ?? dichVu.ThoiDiemThucHien;
                                chiTietYlenh.NhanVienXacNhanThucHienId = dichVu.BacSiKetLuanId ?? dichVu.BacSiThucHienId;
                                chiTietYlenh.ThoiDiemCapNhat = dichVu.LastTime;
                                chiTietYlenh.NhanVienCapNhatId = dichVu.LastUserId;
                            }
                        }
                    }

                    #region Xử lý diễn biến bị trùng
                    var dienBiens = benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => x.NgayDieuTri.Date == ngayDieuTriNoiTru.Date
                                                                                         && x.WillDelete != true).ToList();
                    foreach (var dienBien in dienBiens)
                    {
                        if (dienBien.WillDelete != true)
                        {
                            var lstDienBienTrungGioDienBien = dienBiens
                                .Where(x => x.Id != dienBien.Id
                                            && x.GioDienBien == dienBien.GioDienBien)
                                .ToList();
                            if (lstDienBienTrungGioDienBien.Any())
                            {
                                isChange = true;
                                var lstDienBien = new List<string>();
                                lstDienBien.Add(dienBien.MoTaDienBien);
                                foreach (var dienBienTrung in lstDienBienTrungGioDienBien)
                                {
                                    dienBienTrung.WillDelete = true;
                                    lstDienBien.Add((dienBienTrung.MoTaDienBien ?? "").Trim());
                                }
                                dienBien.MoTaDienBien = string.Join(". ", lstDienBien.Distinct().ToList());
                            }
                        }
                    }
                    #endregion
                }

                if (isChange)
                {
                    #region Xử lý kiểm tra diễn biến tương ứng với y lệnh
                    foreach (var yLenh in benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Any(x => x.GioDienBien == yLenh.GioYLenh
                                                                                && x.NgayDieuTri.Date == yLenh.NgayDieuTri.Date
                                                                                && x.WillDelete != true))
                        {
                            isChange = true;
                            benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Add(new NoiTruPhieuDieuTriChiTietDienBien()
                            {
                                MoTaDienBien = "",
                                GioDienBien = yLenh.GioYLenh,
                                ThoiDiemCapNhat = null,
                                NhanVienCapNhatId = null,
                                NgayDieuTri = yLenh.NgayDieuTri
                            });
                        }
                    }

                    foreach (var dienBien in benhAn.NoiTruPhieuDieuTriChiTietDienBiens.Where(x => (ngayDieuTri == null || x.NgayDieuTri.Date == ngayDieuTri.Value.Date) && !x.WillDelete))
                    {
                        if (!benhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(x => x.GioYLenh == dienBien.GioDienBien
                                                                             && x.NgayDieuTri.Date == dienBien.NgayDieuTri.Date
                                                                             && x.WillDelete != true))
                        {
                            isChange = true;
                            dienBien.WillDelete = true;
                        }
                    }
                    #endregion
                }

                if (isChange || benhAn.ThoiDiemTongHopYLenhKhamBenh == null)
                {
                    if (ngayDieuTri == null)
                    {
                        benhAn.ThoiDiemTongHopYLenhKhamBenh = thoiDiemTongHop;
                    }
                    _noiTruBenhAnRepository.Context.SaveChanges();
                }
            }
        }


        #endregion
        #endregion
    }
}
