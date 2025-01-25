using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        private async Task<List<BaoCaoHoatDongNoiTruGridVo>> GetDataForDieuTriNoiTruSoYTe(BaoCaoHoatDongNoiTruQueryInfo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            var soNgayBaoCao = (int)Math.Round((queryInfo.ToDate.Date - queryInfo.FromDate.Date).TotalDays, 0) + 1;
            var soThangBaoCao = ((queryInfo.ToDate.Year - queryInfo.FromDate.Year) * 12) + queryInfo.ToDate.Month - queryInfo.FromDate.Month + 1;
            bool baoCaoThang = queryInfo.FromDate.Day == 1 && queryInfo.ToDate.AddDays(1).Day == 1;

            var dataHoatDongNoiTrus = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId == null &&
                            o.ThoiDiemNhapVien < queryInfo.ToDate &&
                            (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien >= queryInfo.FromDate))
                .Select(o => new DataHoatDongNoiTru
                {
                    Id = o.Id,
                    ThoiDiemNhapVien = o.ThoiDiemNhapVien,
                    ThoiDiemRaVien = o.ThoiDiemRaVien,
                    HinhThucRaVien = o.HinhThucRaVien,
                    LoaiBenhAn = o.LoaiBenhAn,
                    CoBHYT = o.YeuCauTiepNhan.CoBHYT,
                    NgaySinh = o.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = o.YeuCauTiepNhan.ThangSinh,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    KhoaPhongDieuTris = o.NoiTruKhoaPhongDieuTris.Select(k => new DataHoatDongNoiTruKhoaPhongDieuTri
                    {
                        Id = k.Id,
                        KhoaPhongChuyenDiId = k.KhoaPhongChuyenDiId,
                        KhoaPhongChuyenDenId = k.KhoaPhongChuyenDenId,
                        ThoiDiemVaoKhoa = k.ThoiDiemVaoKhoa
                    }).ToList(),
                    SuDungGiuongs = o.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Select(k => new DataHoatDongNoiTruSuDungGiuong
                    {
                        DoiTuongSuDung = k.DoiTuongSuDung,
                        GiuongBenhId = k.GiuongBenhId,
                        ThoiDiemBatDauSuDung = k.ThoiDiemBatDauSuDung,
                        ThoiDiemKetThucSuDung = k.ThoiDiemKetThucSuDung,
                        TrangThai = k.TrangThai,
                    }).ToList()
                }).ToList();

            foreach (var benhAnNoiTru in dataHoatDongNoiTrus)
            {
                var ngayBatDauTinhDieuTri = benhAnNoiTru.ThoiDiemNhapVien < queryInfo.FromDate ? queryInfo.FromDate : benhAnNoiTru.ThoiDiemNhapVien;
                var ngayKetThucTinhDieuTri = (benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.ToDate) ? queryInfo.ToDate : benhAnNoiTru.ThoiDiemRaVien.Value;

                benhAnNoiTru.SoNgayDieuTri = (int)Math.Round((ngayKetThucTinhDieuTri.Date - ngayBatDauTinhDieuTri.Date).TotalDays, 0) + 1;
                if(benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.ToDate)
                {
                    benhAnNoiTru.SoNgayDieuTriRaVien = 0;
                    benhAnNoiTru.SoNgayDieuTriRaVienBHYT = 0;
                }
                else
                {
                    benhAnNoiTru.SoNgayDieuTriRaVien = (int)Math.Round((benhAnNoiTru.ThoiDiemRaVien.Value.Date - benhAnNoiTru.ThoiDiemNhapVien.Date).TotalDays, 0) + 1;
                    benhAnNoiTru.SoNgayDieuTriRaVienBHYT = benhAnNoiTru.CoBHYT == true ? benhAnNoiTru.SoNgayDieuTriRaVien - 1 : 0;
                }

                if (benhAnNoiTru.KhoaPhongDieuTris.Count > 1)
                {
                    var khoaPhongDieuTriTrongKys = benhAnNoiTru.KhoaPhongDieuTris.Where(o => o.ThoiDiemVaoKhoa >= queryInfo.FromDate && o.ThoiDiemVaoKhoa < queryInfo.ToDate).ToList();
                    if (khoaPhongDieuTriTrongKys.Count > 1 
                        || (khoaPhongDieuTriTrongKys.Count == 1 && benhAnNoiTru.KhoaPhongDieuTris.OrderBy(o => o.Id).First().Id != khoaPhongDieuTriTrongKys.First().Id))
                    {
                        benhAnNoiTru.CoChuyenKhoa = true;
                    }
                }
            }

            //var soGiuongKeHoach = cauHinhBaoCao.SoGiuongKeHoach;//need to update
            //var soGiuongThucKe = dataHoatDongNoiTrus.SelectMany(o => o.SuDungGiuongs)
            //    .Where(o => o.GiuongBenhId != null && o.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
            //                (o.ThoiDiemBatDauSuDung != null && o.ThoiDiemBatDauSuDung < queryInfo.ToDate) &&
            //                (o.ThoiDiemKetThucSuDung == null || o.ThoiDiemKetThucSuDung > queryInfo.FromDate))
            //    .Select(o=>o.GiuongBenhId).Distinct().Count();

            var soGiuongKeHoach = _KhoaPhongRepository.TableNoTracking.Select(o => o.SoGiuongKeHoach.GetValueOrDefault()).DefaultIfEmpty().Sum();
            var soGiuongThucKe = _giuongBenhRepository.TableNoTracking.Where(o => o.IsDisabled != true && o.LaGiuongNoi != true).Count();

            var tongBNRaVien = dataHoatDongNoiTrus.Count(o =>
                o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                                         && o.HinhThucRaVien != Core.Domain.Enums.EnumHinhThucRaVien.ChuyenVien &&
                                         o.HinhThucRaVien != Core.Domain.Enums.EnumHinhThucRaVien.TuVong &&
                                         o.HinhThucRaVien != Core.Domain.Enums.EnumHinhThucRaVien.TuVongTruoc24H);
            var tongBNChuyenVien = dataHoatDongNoiTrus.Count(o =>
                o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                                         && o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.ChuyenVien);
            var tongBNTuVong = dataHoatDongNoiTrus.Count(o =>
                o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                                         && (o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.TuVong ||
                                             o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.TuVongTruoc24H));

            var tongSoNgayDieuTri = dataHoatDongNoiTrus.Sum(o => o.SoNgayDieuTri);
            var tongSoNgayDieuTriRaVien = dataHoatDongNoiTrus.Sum(o => o.SoNgayDieuTriRaVien);
            var tongSoNgayDieuTriTrungBinh = (tongBNRaVien + tongBNChuyenVien + tongBNTuVong) == 0
                ? 0
                : Math.Round(tongSoNgayDieuTriRaVien / (tongBNRaVien + tongBNChuyenVien + tongBNTuVong), 1);


            var data = new List<BaoCaoHoatDongNoiTruGridVo>();
            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 1,
                Muc = "1.TS bệnh nhân điều trị",
                Tong = dataHoatDongNoiTrus.Count()
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 2,
                Muc = "a.Người bệnh còn lại kỳ BC trước",
                Tong = dataHoatDongNoiTrus.Count(o=>o.ThoiDiemNhapVien < queryInfo.FromDate)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 3,
                Muc = "b.Người bệnh mới vào viện",
                Tong = dataHoatDongNoiTrus.Count(o => o.ThoiDiemNhapVien >= queryInfo.FromDate)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 4,
                Muc = "2.BN khoa khác chuyển đến",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoChuyenKhoa)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 5,
                Muc = "3.BN chuyển khoa khác",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoChuyenKhoa)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 6,
                Muc = "4.TS người bệnh ra viện",
                Tong = tongBNRaVien
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 7,
                Muc = "5.TS BN chuyển viện",
                Tong = tongBNChuyenVien
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 8,
                Muc = "6.TS BN tử vong:",
                Tong = tongBNTuVong
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 9,
                Muc = "trong đó: < 24H",
                Tong = dataHoatDongNoiTrus.Count(o => o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                && o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.TuVongTruoc24H)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 10,
                Muc = "7.TS người bệnh hiện có",
                Tong = dataHoatDongNoiTrus.Count(o => o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.ToDate)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 11,
                Muc = "8.TS ngày điều trị nội trú",
                Tong = tongSoNgayDieuTri
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 12,
                Muc = "9. TS ngày điều trị ra viện (RV + CV + TV)",
                Tong = tongSoNgayDieuTriRaVien
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 13,
                Muc = "trong đó: trẻ em < 6 tuổi",
                Tong = dataHoatDongNoiTrus.Where(o=>o.TreEmDuoi6Tuoi).Sum(o => o.SoNgayDieuTriRaVien)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 14,
                Muc = "TS ngày điều trị BHYT",
                Tong = dataHoatDongNoiTrus.Sum(o => o.SoNgayDieuTriRaVienBHYT)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 15,
                Muc = "10.Ngày điều trị trung bình",
                Tong = tongSoNgayDieuTriTrungBinh
            });
            

            if (baoCaoThang)
            {
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 16,
                    Muc = "Tính theo giường kế hoạch",
                    Tong = soGiuongKeHoach,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 17,
                    Muc = "11. Ngày sử dụng giường",
                    Tong = Math.Round(tongSoNgayDieuTri/(soGiuongKeHoach * soThangBaoCao), 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 18,
                    Muc = "12. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 19,
                    Muc = "13. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongKeHoach * soNgayBaoCao), 3),
                    IsPerCent = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 20,
                    Muc = "Tính theo giường thực kê",
                    Tong = soGiuongThucKe,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 21,
                    Muc = "14. Ngày sử dụng giường",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongThucKe * soThangBaoCao), 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 22,
                    Muc = "15. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 23,
                    Muc = "16. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongThucKe * soNgayBaoCao), 3),
                    IsPerCent = true
                });
            }
            else
            {
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 16,
                    Muc = "Tính theo giường kế hoạch",
                    Tong = soGiuongKeHoach,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 18,
                    Muc = "11. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 19,
                    Muc = "12. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongKeHoach * soNgayBaoCao), 3),
                    IsPerCent = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 20,
                    Muc = "Tính theo giường thực kê",
                    Tong = soGiuongThucKe,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 22,
                    Muc = "13. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 23,
                    Muc = "14. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongThucKe * soNgayBaoCao), 3),
                    IsPerCent = true
                });
            }

            return data;
        }
        private async Task<List<BaoCaoHoatDongNoiTruGridVo>> GetDataForDieuTriNoiTruTaiBV(BaoCaoHoatDongNoiTruQueryInfo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            var soNgayBaoCao = (int)Math.Round((queryInfo.ToDate.Date - queryInfo.FromDate.Date).TotalDays, 0) + 1;
            var soThangBaoCao = ((queryInfo.ToDate.Year - queryInfo.FromDate.Year) * 12) + queryInfo.ToDate.Month - queryInfo.FromDate.Month + 1;
            bool baoCaoThang = queryInfo.FromDate.Day == 1 && queryInfo.ToDate.AddDays(1).Day == 1;

            var dataHoatDongNoiTrus = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.ThoiDiemNhapVien < queryInfo.ToDate &&
                            (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien >= queryInfo.FromDate))
                .Select(o => new DataHoatDongNoiTru
                {
                    Id = o.Id,
                    ThoiDiemNhapVien = o.ThoiDiemNhapVien,
                    ThoiDiemRaVien = o.ThoiDiemRaVien,
                    HinhThucRaVien = o.HinhThucRaVien,
                    LoaiBenhAn = o.LoaiBenhAn,
                    CoBHYT = o.YeuCauTiepNhan.CoBHYT,
                    NgaySinh = o.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = o.YeuCauTiepNhan.ThangSinh,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    LaBenhAnCon = o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null,
                    KhoaPhongDieuTris = o.NoiTruKhoaPhongDieuTris.Select(k => new DataHoatDongNoiTruKhoaPhongDieuTri
                    {
                        Id = k.Id,
                        KhoaPhongChuyenDiId = k.KhoaPhongChuyenDiId,
                        KhoaPhongChuyenDenId = k.KhoaPhongChuyenDenId,
                        ThoiDiemVaoKhoa = k.ThoiDiemVaoKhoa
                    }).ToList(),
                    SuDungGiuongs = o.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Select(k => new DataHoatDongNoiTruSuDungGiuong
                    {
                        DoiTuongSuDung = k.DoiTuongSuDung,
                        GiuongBenhId = k.GiuongBenhId,
                        ThoiDiemBatDauSuDung = k.ThoiDiemBatDauSuDung,
                        ThoiDiemKetThucSuDung = k.ThoiDiemKetThucSuDung,
                        TrangThai = k.TrangThai,
                    }).ToList(),
                    DieuTriBenhAnSoSinhs = o.NoiTruThoiGianDieuTriBenhAnSoSinhs.Select(k=>new DataHoatDongNoiTruDieuTriBenhAnSoSinh
                    {
                        NgayDieuTri = k.NgayDieuTri,
                        GioBatDau = k.GioBatDau,
                        GioKetThuc = k.GioKetThuc
                    }).ToList()
                }).ToList();

            foreach (var benhAnNoiTru in dataHoatDongNoiTrus)
            {
                if (benhAnNoiTru.LaBenhAnCon)
                {
                    if (benhAnNoiTru.DieuTriBenhAnSoSinhs.Any())
                    {
                         var tongSoGiayDieuTri = benhAnNoiTru.DieuTriBenhAnSoSinhs
                            .Where(o => o.NgayDieuTri.Date >= queryInfo.FromDate.Date &&
                                        o.NgayDieuTri.Date <= queryInfo.ToDate.Date)
                            .Select(o => o.SoGiayDieuTri).DefaultIfEmpty().Sum();
                         benhAnNoiTru.SoNgayDieuTri = Math.Round((decimal)tongSoGiayDieuTri / (24 * 60 * 60), 1);

                        if (benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.ToDate)
                        {
                            benhAnNoiTru.SoNgayDieuTriRaVien = 0;
                            benhAnNoiTru.SoNgayDieuTriRaVienBHYT = 0;
                        }
                        else
                        {
                            var tongSoGiayDieuTriRaVien = benhAnNoiTru.DieuTriBenhAnSoSinhs
                                .Select(o => o.SoGiayDieuTri).DefaultIfEmpty().Sum();
                            benhAnNoiTru.SoNgayDieuTriRaVien = Math.Round((decimal)tongSoGiayDieuTriRaVien / (24 * 60 * 60), 1);
                            benhAnNoiTru.SoNgayDieuTriRaVienBHYT = 0;
                        }

                        if (benhAnNoiTru.KhoaPhongDieuTris.Count > 1)
                        {
                            var khoaPhongDieuTriTrongKys = benhAnNoiTru.KhoaPhongDieuTris.Where(o => o.ThoiDiemVaoKhoa >= queryInfo.FromDate && o.ThoiDiemVaoKhoa < queryInfo.ToDate).ToList();
                            if (khoaPhongDieuTriTrongKys.Count > 1
                                || (khoaPhongDieuTriTrongKys.Count == 1 && benhAnNoiTru.KhoaPhongDieuTris.OrderBy(o => o.Id).First().Id != khoaPhongDieuTriTrongKys.First().Id))
                            {
                                benhAnNoiTru.CoChuyenKhoa = true;
                            }
                        }
                    }
                }
                else
                {
                    var ngayBatDauTinhDieuTri = benhAnNoiTru.ThoiDiemNhapVien < queryInfo.FromDate ? queryInfo.FromDate : benhAnNoiTru.ThoiDiemNhapVien;
                    var ngayKetThucTinhDieuTri = (benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.ToDate) ? queryInfo.ToDate : benhAnNoiTru.ThoiDiemRaVien.Value;
                    benhAnNoiTru.SoNgayDieuTri = (int)Math.Round((ngayKetThucTinhDieuTri.Date - ngayBatDauTinhDieuTri.Date).TotalDays, 0) + 1;
                    if (benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.ToDate)
                    {
                        benhAnNoiTru.SoNgayDieuTriRaVien = 0;
                        benhAnNoiTru.SoNgayDieuTriRaVienBHYT = 0;
                    }
                    else
                    {
                        benhAnNoiTru.SoNgayDieuTriRaVien = (int)Math.Round((benhAnNoiTru.ThoiDiemRaVien.Value.Date - benhAnNoiTru.ThoiDiemNhapVien.Date).TotalDays, 0) + 1;
                        benhAnNoiTru.SoNgayDieuTriRaVienBHYT = benhAnNoiTru.CoBHYT == true ? benhAnNoiTru.SoNgayDieuTriRaVien - 1 : 0;
                    }

                    if (benhAnNoiTru.KhoaPhongDieuTris.Count > 1)
                    {
                        var khoaPhongDieuTriTrongKys = benhAnNoiTru.KhoaPhongDieuTris.Where(o => o.ThoiDiemVaoKhoa >= queryInfo.FromDate && o.ThoiDiemVaoKhoa < queryInfo.ToDate).ToList();
                        if (khoaPhongDieuTriTrongKys.Count > 1
                            || (khoaPhongDieuTriTrongKys.Count == 1 && benhAnNoiTru.KhoaPhongDieuTris.OrderBy(o => o.Id).First().Id != khoaPhongDieuTriTrongKys.First().Id))
                        {
                            benhAnNoiTru.CoChuyenKhoa = true;
                        }
                    }
                }
            }

            //var soGiuongKeHoach = cauHinhBaoCao.SoGiuongKeHoach;
            //var soGiuongThucKe = dataHoatDongNoiTrus.SelectMany(o => o.SuDungGiuongs)
            //    .Where(o => o.GiuongBenhId != null && o.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
            //                (o.ThoiDiemBatDauSuDung != null && o.ThoiDiemBatDauSuDung < queryInfo.ToDate) &&
            //                (o.ThoiDiemKetThucSuDung == null || o.ThoiDiemKetThucSuDung > queryInfo.FromDate))
            //    .Select(o => o.GiuongBenhId).Distinct().Count();
            var soGiuongKeHoach = _KhoaPhongRepository.TableNoTracking.Select(o => o.SoGiuongKeHoach.GetValueOrDefault()).DefaultIfEmpty().Sum();
            var soGiuongThucKe = _giuongBenhRepository.TableNoTracking.Where(o => o.IsDisabled != true && o.LaGiuongNoi != true).Count();

            var tongBNRaVien = dataHoatDongNoiTrus.Count(o => (!o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any()) &&
                                                              o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                                                              && o.HinhThucRaVien != Core.Domain.Enums.EnumHinhThucRaVien.ChuyenVien &&
                                                              o.HinhThucRaVien != Core.Domain.Enums.EnumHinhThucRaVien.TuVong &&
                                                              o.HinhThucRaVien != Core.Domain.Enums.EnumHinhThucRaVien.TuVongTruoc24H);
            var tongBNChuyenVien = dataHoatDongNoiTrus.Count(o => (!o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any()) &&
                                                                  o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                                                                  && o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.ChuyenVien);
            var tongBNTuVong = dataHoatDongNoiTrus.Count(o => (!o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any()) &&
                                                              o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                                                              && (o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.TuVong ||
                                                                  o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.TuVongTruoc24H));

            var tongSoNgayDieuTri = dataHoatDongNoiTrus.Sum(o => o.SoNgayDieuTri);
            var tongSoNgayDieuTriRaVien = dataHoatDongNoiTrus.Sum(o => o.SoNgayDieuTriRaVien);
            var tongSoNgayDieuTriTrungBinh = (tongBNRaVien + tongBNChuyenVien + tongBNTuVong) == 0
                ? 0
                : Math.Round(tongSoNgayDieuTriRaVien / (tongBNRaVien + tongBNChuyenVien + tongBNTuVong), 1);


            var data = new List<BaoCaoHoatDongNoiTruGridVo>();
            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 1,
                Muc = "1.TS bệnh nhân điều trị",
                Tong = dataHoatDongNoiTrus.Count(o => !o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any())
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 2,
                Muc = "a.Người bệnh còn lại kỳ BC trước",
                Tong = dataHoatDongNoiTrus.Count(o => (!o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any()) && o.ThoiDiemNhapVien < queryInfo.FromDate)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 3,
                Muc = "b.Người bệnh mới vào viện",
                Tong = dataHoatDongNoiTrus.Count(o => (!o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any()) && o.ThoiDiemNhapVien >= queryInfo.FromDate)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 4,
                Muc = "2.BN khoa khác chuyển đến",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoChuyenKhoa)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 5,
                Muc = "3.BN chuyển khoa khác",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoChuyenKhoa)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 6,
                Muc = "4.TS người bệnh ra viện",
                Tong = tongBNRaVien
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 7,
                Muc = "5.TS BN chuyển viện",
                Tong = tongBNChuyenVien
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 8,
                Muc = "6.TS BN tử vong:",
                Tong = tongBNTuVong
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 9,
                Muc = "trong đó: < 24H",
                Tong = dataHoatDongNoiTrus.Count(o => (!o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any()) && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.ToDate
                                                      && o.HinhThucRaVien == Core.Domain.Enums.EnumHinhThucRaVien.TuVongTruoc24H)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 10,
                Muc = "7.TS người bệnh hiện có",
                Tong = dataHoatDongNoiTrus.Count(o => (!o.LaBenhAnCon || o.DieuTriBenhAnSoSinhs.Any()) && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.ToDate))
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 11,
                Muc = "8.TS ngày điều trị nội trú",
                Tong = tongSoNgayDieuTri
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 12,
                Muc = "9. TS ngày điều trị ra viện (RV + CV + TV)",
                Tong = tongSoNgayDieuTriRaVien
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 13,
                Muc = "trong đó: trẻ em < 6 tuổi",
                Tong = dataHoatDongNoiTrus.Where(o => o.TreEmDuoi6Tuoi).Sum(o => o.SoNgayDieuTriRaVien)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 14,
                Muc = "TS ngày điều trị BHYT",
                Tong = dataHoatDongNoiTrus.Sum(o => o.SoNgayDieuTriRaVienBHYT)
            });

            data.Add(new BaoCaoHoatDongNoiTruGridVo
            {
                Id = 15,
                Muc = "10.Ngày điều trị trung bình",
                Tong = tongSoNgayDieuTriTrungBinh
            });


            if (baoCaoThang)
            {
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 16,
                    Muc = "Tính theo giường kế hoạch",
                    Tong = soGiuongKeHoach,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 17,
                    Muc = "11. Ngày sử dụng giường",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongKeHoach * soThangBaoCao), 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 18,
                    Muc = "12. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 19,
                    Muc = "13. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongKeHoach * soNgayBaoCao), 3),
                    IsPerCent = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 20,
                    Muc = "Tính theo giường thực kê",
                    Tong = soGiuongThucKe,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 21,
                    Muc = "14. Ngày sử dụng giường",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongThucKe * soThangBaoCao), 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 22,
                    Muc = "15. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 23,
                    Muc = "16. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongThucKe * soNgayBaoCao), 3),
                    IsPerCent = true
                });
            }
            else
            {
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 16,
                    Muc = "Tính theo giường kế hoạch",
                    Tong = soGiuongKeHoach,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 18,
                    Muc = "11. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 19,
                    Muc = "12. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongKeHoach * soNgayBaoCao), 3),
                    IsPerCent = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 20,
                    Muc = "Tính theo giường thực kê",
                    Tong = soGiuongThucKe,
                    IsCenter = true
                });

                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 22,
                    Muc = "13. Giường bệnh sử dụng (giường thực hiện)",
                    Tong = Math.Round(tongSoNgayDieuTri / soNgayBaoCao, 1)
                });
                data.Add(new BaoCaoHoatDongNoiTruGridVo
                {
                    Id = 23,
                    Muc = "14. Công suất sử dụng giường bệnh",
                    Tong = Math.Round(tongSoNgayDieuTri / (soGiuongThucKe * soNgayBaoCao), 3),
                    IsPerCent = true
                });
            }

            return data;
        }
        public async Task<GridDataSource> GetDataBaoCaoHoatDongNoiTruForGridAsync(BaoCaoHoatDongNoiTruQueryInfo queryInfo)
        {
            var allData = new List<BaoCaoHoatDongNoiTruGridVo>();
            if (queryInfo.NoiDieuTriId == 0) // Sở Y Tế
            {
                allData = await GetDataForDieuTriNoiTruSoYTe(queryInfo);
            }
            else if (queryInfo.NoiDieuTriId == 1) // Tại Bệnh Viện
            {
                allData = await GetDataForDieuTriNoiTruTaiBV(queryInfo);
            }

            return new GridDataSource { Data = allData.ToArray(), TotalRowCount = allData.Count() };
        }
        public virtual byte[] ExportBaoCaoHoatDongNoiTruGridVo(GridDataSource gridDataSource, BaoCaoHoatDongNoiTruQueryInfo query)
        {
            var datas = (ICollection<BaoCaoHoatDongNoiTruGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoHoatDongNoiTruGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HOẠT ĐỘNG NỘI TRÚ");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 80;
                    worksheet.Column(2).Width = 20;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:b1"])
                    {
                        range.Worksheet.Cells["A1:B1"].Merge = true;
                        range.Worksheet.Cells["A1:B1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:B1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:B1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:B3"])
                    {
                        range.Worksheet.Cells["A3:B3"].Merge = true;
                        range.Worksheet.Cells["A3:B3"].Value = "BÁO CÁO HOẠT ĐỘNG NỘI TRÚ";
                        range.Worksheet.Cells["A3:B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:B3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:B3"].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells["A3:B3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:B3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:B4"])
                    {
                        range.Worksheet.Cells["A4:B4"].Merge = true;
                        range.Worksheet.Cells["A4:B4"].Value = "Thời gian: từ " + query.FromDate.ToString("HH:mm") + " ngày " + query.FromDate.ToString("dd/MM/yyyy")
                                                       + " đến " + query.ToDate.ToString("HH:mm") + " ngày " + query.ToDate.ToString("dd/MM/yyyy");
                        range.Worksheet.Cells["A4:B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:B4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:B4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:B4"].Style.Font.Color.SetColor(Color.Black);
                    }

                    string NoiDieuTri = "";
                    if (query.NoiDieuTriId == 0)
                    {
                        NoiDieuTri = "Điều Trị Nội Trú Sở Y Tế";
                    }
                    else if (query.NoiDieuTriId == 1)
                    {
                        NoiDieuTri = "Điều Trị Nội Trú Tại Bệnh Viện";
                    }
                    using (var range = worksheet.Cells["A5:B5"])
                    {
                        range.Worksheet.Cells["A5:B5"].Merge = true;
                        range.Worksheet.Cells["A5:B5"].Value = NoiDieuTri;
                        range.Worksheet.Cells["A5:B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:B5"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A5:B5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:B5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:B7"])
                    {
                        range.Worksheet.Cells["A7:B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:B7"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A7:B7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:B7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "MỤC";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "TỔNG";
                    }

                    //write data from line 8
                    int index = 8;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = item.IsCenter ? ExcelHorizontalAlignment.Center : ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = item.Muc;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            if (item.IsPerCent)
                            {
                                worksheet.Cells["B" + index].Style.Numberformat.Format = "0.0%";
                            }
                            worksheet.Cells["B" + index].Value = item.Tong;

                            index++;
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
