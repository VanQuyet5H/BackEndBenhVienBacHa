using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Services.Helpers;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.CauHinh;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<BaoCaoHoatDongNoiTruChiTietVo> GetDataBaoCaoHoatDongNoiTruChiTietGrid(BaoCaoHoatDongNoiTruChiTietQueryInfoVo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            string[] hoatDongNoiTruChiTietKhoaPhongIds = cauHinhBaoCao.HoatDongNoiTruChiTietKhoaPhongIds != null ? cauHinhBaoCao.HoatDongNoiTruChiTietKhoaPhongIds?.Split(";") : new string[0];
            var soNgayBaoCao = (int)Math.Round((queryInfo.DenNgay.Date - queryInfo.TuNgay.Date).TotalDays, 0) + 1;
            var soThangBaoCao = ((queryInfo.DenNgay.Year - queryInfo.TuNgay.Year) * 12) + queryInfo.DenNgay.Month - queryInfo.TuNgay.Month + 1;
            bool baoCaoThang = queryInfo.TuNgay.Day == 1 && queryInfo.DenNgay.AddDays(1).Day == 1;

            var dataHoatDongNoiTrus = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.ThoiDiemNhapVien < queryInfo.DenNgay &&
                            (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien >= queryInfo.TuNgay) &&
                            o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId == null)
                .Select(o => new DataHoatDongNoiTruChiTiet
                {
                    Id = o.Id,
                    KhoaPhongNhapVienId = o.KhoaPhongNhapVienId,
                    ThoiDiemNhapVien = o.ThoiDiemNhapVien,
                    ThoiDiemRaVien = o.ThoiDiemRaVien,
                    HinhThucRaVien = o.HinhThucRaVien,
                    LoaiBenhAn = o.LoaiBenhAn,
                    CoBHYT = o.YeuCauTiepNhan.CoBHYT,
                    LaCapCuu = o.LaCapCuu,
                    NgaySinh = o.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = o.YeuCauTiepNhan.ThangSinh,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    TinhThanhId = o.YeuCauTiepNhan.TinhThanhId,
                    KhoaPhongDieuTris = o.NoiTruKhoaPhongDieuTris.Select(k => new DataHoatDongNoiTruChiTietKhoaPhongDieuTri
                    {
                        Id = k.Id,
                        KhoaPhongChuyenDiId = k.KhoaPhongChuyenDiId,
                        KhoaPhongChuyenDenId = k.KhoaPhongChuyenDenId,
                        ThoiDiemVaoKhoa = k.ThoiDiemVaoKhoa,
                        ThoiDiemRaKhoa = k.ThoiDiemRaKhoa,
                    }).ToList(),
                    SuDungGiuongs = o.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Select(k => new DataHoatDongNoiTruChiTietSuDungGiuong
                    {
                        DoiTuongSuDung = k.DoiTuongSuDung,
                        GiuongBenhId = k.GiuongBenhId,
                        ThoiDiemBatDauSuDung = k.ThoiDiemBatDauSuDung,
                        ThoiDiemKetThucSuDung = k.ThoiDiemKetThucSuDung,
                        TrangThai = k.TrangThai,
                    }).ToList()
                }).ToList();

            var khoaPhongs = _KhoaPhongRepository.TableNoTracking.ToList();
            var giuongBenhs = _giuongBenhRepository.TableNoTracking.Where(o=>o.IsDisabled != true && o.LaGiuongNoi != true).Select(o => new { o.Id, o.PhongBenhVien.KhoaPhongId }).ToList();

            foreach (var benhAnNoiTru in dataHoatDongNoiTrus)
            {
                //NB nếu đang điều trị tại khoa GMHS thì trong bc này tính NB đó vào khoa điều trị ban đầu trước khi chuyển sang khoa GMHS
                var khoaPhongDieuTriGocs = benhAnNoiTru.KhoaPhongDieuTris.OrderBy(o => o.ThoiDiemVaoKhoa).ToList();
                var khoaPhongDieuTriCapNhaps = new List<DataHoatDongNoiTruChiTietKhoaPhongDieuTri>();

                for(int i = 0; i < khoaPhongDieuTriGocs.Count; i++)
                {
                    if (i == 0)
                    {
                        khoaPhongDieuTriCapNhaps.Add(new DataHoatDongNoiTruChiTietKhoaPhongDieuTri 
                        {
                            Id = khoaPhongDieuTriGocs[i].Id,
                            KhoaPhongChuyenDiId = khoaPhongDieuTriGocs[i].KhoaPhongChuyenDiId,
                            KhoaPhongChuyenDenId = khoaPhongDieuTriGocs[i].KhoaPhongChuyenDenId,
                            ThoiDiemVaoKhoa = khoaPhongDieuTriGocs[i].ThoiDiemVaoKhoa,
                            ThoiDiemRaKhoa = khoaPhongDieuTriGocs[i].ThoiDiemRaKhoa,
                        });
                    }
                    else
                    {
                        if(khoaPhongDieuTriGocs[i].KhoaPhongChuyenDenId == (long)EnumKhoaPhong.KhoaGMHS)
                        {
                            khoaPhongDieuTriCapNhaps.Last().ThoiDiemRaKhoa = khoaPhongDieuTriGocs[i].ThoiDiemRaKhoa;
                        }
                        else if (khoaPhongDieuTriGocs[i].KhoaPhongChuyenDenId == khoaPhongDieuTriCapNhaps.Last().KhoaPhongChuyenDenId)
                        {
                            khoaPhongDieuTriCapNhaps.Last().ThoiDiemRaKhoa = khoaPhongDieuTriGocs[i].ThoiDiemRaKhoa;
                        }
                        else
                        {
                            khoaPhongDieuTriCapNhaps.Add(new DataHoatDongNoiTruChiTietKhoaPhongDieuTri
                            {
                                Id = khoaPhongDieuTriGocs[i].Id,
                                KhoaPhongChuyenDiId = khoaPhongDieuTriGocs[i].KhoaPhongChuyenDiId,
                                KhoaPhongChuyenDenId = khoaPhongDieuTriGocs[i].KhoaPhongChuyenDenId,
                                ThoiDiemVaoKhoa = khoaPhongDieuTriGocs[i].ThoiDiemVaoKhoa,
                                ThoiDiemRaKhoa = khoaPhongDieuTriGocs[i].ThoiDiemRaKhoa,
                            });
                        }
                    }
                }

                benhAnNoiTru.KhoaPhongDieuTris = khoaPhongDieuTriCapNhaps;

                benhAnNoiTru.KhoaCuoiCungDieuTriId = benhAnNoiTru.KhoaPhongDieuTris.Where(o => o.ThoiDiemVaoKhoa < queryInfo.DenNgay).OrderBy(o => o.ThoiDiemVaoKhoa).LastOrDefault()?.KhoaPhongChuyenDenId ?? benhAnNoiTru.KhoaPhongNhapVienId;

                if (benhAnNoiTru.KhoaCuoiCungDieuTriId != benhAnNoiTru.KhoaPhongNhapVienId && benhAnNoiTru.KhoaCuoiCungDieuTriId != (long)EnumKhoaPhong.KhoaGMHS)
                {
                    benhAnNoiTru.CoChuyenKhoa = true;
                }

                var ngayBatDauTinhDieuTri = benhAnNoiTru.ThoiDiemNhapVien < queryInfo.TuNgay ? queryInfo.TuNgay : benhAnNoiTru.ThoiDiemNhapVien;
                var ngayKetThucTinhDieuTri = (benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.DenNgay) ? queryInfo.DenNgay : benhAnNoiTru.ThoiDiemRaVien.Value;

                benhAnNoiTru.SoNgayDieuTri = (int)Math.Round((ngayKetThucTinhDieuTri.Date - ngayBatDauTinhDieuTri.Date).TotalDays, 0)
                    + (ngayBatDauTinhDieuTri < ngayBatDauTinhDieuTri.Date.AddHours(20) ? 1 : 0);

                if (benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.DenNgay)
                {
                    benhAnNoiTru.SoNgayDieuTriRaVien = 0;
                }
                else
                {
                    benhAnNoiTru.SoNgayDieuTriRaVien = (int)Math.Round((benhAnNoiTru.ThoiDiemRaVien.Value.Date - benhAnNoiTru.ThoiDiemNhapVien.Date).TotalDays, 0)
                        + (benhAnNoiTru.ThoiDiemNhapVien < benhAnNoiTru.ThoiDiemNhapVien.Date.AddHours(20) ? 1 : 0);
                }
                var khoaPhongDieuTris = benhAnNoiTru.KhoaPhongDieuTris.OrderBy(o => o.ThoiDiemVaoKhoa).ToList();
                for (int i = 0; i < khoaPhongDieuTris.Count(); i++)
                {
                    var ngayTinhVaoKhoa = khoaPhongDieuTris[i].ThoiDiemVaoKhoa < queryInfo.TuNgay ? queryInfo.TuNgay : khoaPhongDieuTris[i].ThoiDiemVaoKhoa;
                    var ngayTinhRaKhoa = (khoaPhongDieuTris[i].ThoiDiemRaKhoa == null || khoaPhongDieuTris[i].ThoiDiemRaKhoa > queryInfo.DenNgay) ? ngayKetThucTinhDieuTri : khoaPhongDieuTris[i].ThoiDiemRaKhoa.Value;

                    if (ngayTinhRaKhoa > ngayTinhVaoKhoa)
                    {
                        khoaPhongDieuTris[i].SoNgayDieuTri = (int)Math.Round((ngayTinhRaKhoa.Date - ngayTinhVaoKhoa.Date).TotalDays, 0)
                            + (ngayTinhVaoKhoa < ngayTinhVaoKhoa.Date.AddHours(20) ? 1 : 0);
                    }
                    if (benhAnNoiTru.ThoiDiemRaVien == null || benhAnNoiTru.ThoiDiemRaVien > queryInfo.DenNgay)
                    {
                        khoaPhongDieuTris[i].SoNgayDieuTriRaVien = 0;
                    }
                    else
                    {
                        khoaPhongDieuTris[i].SoNgayDieuTriRaVien = (int)Math.Round(((khoaPhongDieuTris[i].ThoiDiemRaKhoa == null ? benhAnNoiTru.ThoiDiemRaVien.Value.Date : khoaPhongDieuTris[i].ThoiDiemRaKhoa.Value.Date) - khoaPhongDieuTris[i].ThoiDiemVaoKhoa.Date).TotalDays, 0)
                            + (khoaPhongDieuTris[i].ThoiDiemVaoKhoa < khoaPhongDieuTris[i].ThoiDiemVaoKhoa.Date.AddHours(20) ? 1 : 0);
                    }
                }
            }

            var baoCaoHoatDongNoiTruChiTietVo = new BaoCaoHoatDongNoiTruChiTietVo();
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTietColumnHeader.Add(new BaoCaoHoatDongNoiTruChiTietColumnHeaderVo
                {
                    Index = i,
                    CellText = khoaPhongs.Where(o => o.Id.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).FirstOrDefault()?.Ten
                });
            }

            var ct1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "1",
                ChiTieu = "TS bệnh nhân điều trị",
                Tong = dataHoatDongNoiTrus.Count(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1);

            var ct1_1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: + BHYT",
                Tong = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true).Count(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1_1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1_1);

            var ct1_2 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "         + Viện phí",
                Tong = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true).Count(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1_2.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1_2);

            var ct1a = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "a",
                ChiTieu = "Người bệnh còn lại kỳ BC trước",
                Tong = dataHoatDongNoiTrus.Count(o => o.ThoiDiemNhapVien < queryInfo.TuNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1a.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.ThoiDiemNhapVien < queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1a);

            var ct1a_1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: + BHYT",
                Tong = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && o.ThoiDiemNhapVien < queryInfo.TuNgay).Count(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1a_1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && o.ThoiDiemNhapVien < queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1a_1);

            var ct1a_2 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "         + Viện phí",
                Tong = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true && o.ThoiDiemNhapVien < queryInfo.TuNgay).Count(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1a_2.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true && o.ThoiDiemNhapVien < queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1a_2);

            var ct1b = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "b",
                ChiTieu = "Người bệnh mới vào viện",
                Tong = dataHoatDongNoiTrus.Count(o => o.ThoiDiemNhapVien >= queryInfo.TuNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b);

            var ct1b_1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: + BHYT",
                Tong = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && o.ThoiDiemNhapVien >= queryInfo.TuNgay).Count(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b_1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b_1);

            var ct1b_2 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "         + Viện phí",
                Tong = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true && o.ThoiDiemNhapVien >= queryInfo.TuNgay).Count(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b_2.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true && o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b_2);

            var ct1b_3 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trẻ em 6 - 15 tuổi",
                Tong = dataHoatDongNoiTrus.Count(o => o.TreEm6Den15Tuoi && o.ThoiDiemNhapVien >= queryInfo.TuNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b_3.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEm6Den15Tuoi && o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b_3);

            var ct1b_4 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trẻ em < 6 tuổi",
                Tong = dataHoatDongNoiTrus.Count(o => o.TreEmDuoi6Tuoi && o.ThoiDiemNhapVien >= queryInfo.TuNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b_4.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEmDuoi6Tuoi && o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b_4);

            var ct1b_5 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Chuyển khoa khác",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoChuyenKhoa && o.ThoiDiemNhapVien >= queryInfo.TuNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b_5.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoChuyenKhoa && o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaPhongNhapVienId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b_5);

            var ct1b_6 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Khoa khác chuyển đến",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoChuyenKhoa && o.ThoiDiemNhapVien >= queryInfo.TuNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b_6.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoChuyenKhoa && o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b_6);

            var ct1b_7 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "NB ngoại tỉnh mới vào",
                Tong = dataHoatDongNoiTrus.Count(o => o.TinhThanhId != cauHinhBaoCao.TinhHaNoi && o.ThoiDiemNhapVien >= queryInfo.TuNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct1b_7.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TinhThanhId != cauHinhBaoCao.TinhHaNoi && o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct1b_7);

            var ct2 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "2",
                ChiTieu = "TSNB cấp cứu",
                Tong = dataHoatDongNoiTrus.Count(o => o.LaCapCuu),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct2.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.LaCapCuu && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct2);

            var ct3 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "3",
                ChiTieu = "Trẻ em",
                Tong = dataHoatDongNoiTrus.Count(o => o.TreEm6Den15Tuoi || o.TreEmDuoi6Tuoi),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct3.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => (o.TreEm6Den15Tuoi || o.TreEmDuoi6Tuoi) && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct3);

            var ct3_1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: < 6 tuổi",
                Tong = dataHoatDongNoiTrus.Count(o => o.TreEmDuoi6Tuoi),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct3_1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEmDuoi6Tuoi && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct3_1);

            var ct4 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "4",
                ChiTieu = "Tổng số BN ra viện + chuyển viện + chết",
                Tong = dataHoatDongNoiTrus.Count(o => o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4);

            var ct4_1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: + BHYT",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoBHYT == true && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_1);

            var ct4_2 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "          + Viện phí",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoBHYT != true && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_2.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_2);

            var ct4_3 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trẻ em",
                Tong = dataHoatDongNoiTrus.Count(o => (o.TreEm6Den15Tuoi || o.TreEmDuoi6Tuoi) && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_3.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => (o.TreEm6Den15Tuoi || o.TreEmDuoi6Tuoi) && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_3);

            var ct4_4 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trẻ em < 6 tuổi",
                Tong = dataHoatDongNoiTrus.Count(o => o.TreEmDuoi6Tuoi && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_4.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEmDuoi6Tuoi && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_4);

            var ct4_5 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: Bệnh nhân chuyển viện",
                Tong = dataHoatDongNoiTrus.Count(o => o.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_5.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_5);

            var ct4_6 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: BHYT",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoBHYT == true && o.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_6.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && o.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_6);

            var ct4_7 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Tổng số bệnh nhân chết",
                Tong = dataHoatDongNoiTrus.Count(o => (o.HinhThucRaVien == EnumHinhThucRaVien.TuVong || o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H) && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_7.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => (o.HinhThucRaVien == EnumHinhThucRaVien.TuVong || o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H) && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_7);

            var ct4_8 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trong đó: Chết trước 24 giờ",
                Tong = dataHoatDongNoiTrus.Count(o => o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct4_8.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H && o.ThoiDiemRaVien != null && o.ThoiDiemRaVien <= queryInfo.DenNgay && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct4_8);

            var ct5 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "5",
                ChiTieu = "Bệnh nhân còn lại cuối kỳ báo cáo",
                Tong = dataHoatDongNoiTrus.Count(o => (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay)),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct5.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay) && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct5);

            var ct5_1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "T.đó: + BHYT",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoBHYT == true && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay)),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct5_1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT == true && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay) && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct5_1);

            var ct5_2 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "          + Viện phí",
                Tong = dataHoatDongNoiTrus.Count(o => o.CoBHYT != true && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay)),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct5_2.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.CoBHYT != true && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay) && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct5_2);

            var ct5_3 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trẻ em 6 - 15 tuổi",
                Tong = dataHoatDongNoiTrus.Count(o => o.TreEm6Den15Tuoi && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay)),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct5_3.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEm6Den15Tuoi && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay) && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct5_3);

            var ct5_4 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trẻ em < 6 tuổi",
                Tong = dataHoatDongNoiTrus.Count(o => o.TreEmDuoi6Tuoi && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay)),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct5_4.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEmDuoi6Tuoi && (o.ThoiDiemRaVien == null || o.ThoiDiemRaVien > queryInfo.DenNgay) && o.KhoaCuoiCungDieuTriId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Count()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct5_4);

            var ct6 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "6",
                ChiTieu = "TS ngày điều trị nội trú",
                Tong = dataHoatDongNoiTrus.Select(o => o.SoNgayDieuTri).DefaultIfEmpty().Sum(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct6.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.SelectMany(o => o.KhoaPhongDieuTris).Where(o => o.KhoaPhongChuyenDenId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Select(o => o.SoNgayDieuTri).DefaultIfEmpty().Sum()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct6);

            var ct7 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "7",
                ChiTieu = "TS ngày điều trị ra viện (RV + CV + TV)",
                Tong = dataHoatDongNoiTrus.Select(o => o.SoNgayDieuTriRaVien).DefaultIfEmpty().Sum(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct7.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.SelectMany(o => o.KhoaPhongDieuTris).Where(o => o.KhoaPhongChuyenDenId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Select(o => o.SoNgayDieuTriRaVien).DefaultIfEmpty().Sum()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct7);

            var ct7_1 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "Trẻ em",
                Tong = dataHoatDongNoiTrus.Where(o => o.TreEm6Den15Tuoi || o.TreEmDuoi6Tuoi).Select(o => o.SoNgayDieuTriRaVien).DefaultIfEmpty().Sum(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct7_1.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEm6Den15Tuoi || o.TreEmDuoi6Tuoi).SelectMany(o => o.KhoaPhongDieuTris).Where(o => o.KhoaPhongChuyenDenId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Select(o => o.SoNgayDieuTriRaVien).DefaultIfEmpty().Sum()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct7_1);

            var ct7_2 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "",
                ChiTieu = "trong đó: trẻ em < 6 tuổi",
                Tong = dataHoatDongNoiTrus.Where(o => o.TreEmDuoi6Tuoi).Select(o => o.SoNgayDieuTriRaVien).DefaultIfEmpty().Sum(),
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct7_2.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = dataHoatDongNoiTrus.Where(o => o.TreEmDuoi6Tuoi).SelectMany(o => o.KhoaPhongDieuTris).Where(o => o.KhoaPhongChuyenDenId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Select(o => o.SoNgayDieuTriRaVien).DefaultIfEmpty().Sum()
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct7_2);

            var ct8 = new BaoCaoHoatDongNoiTruChiTiet
            {
                STT = "8",
                ChiTieu = "Ngày điều trị trung bình/ 1 NB ra viện",
                Tong = ct4.Tong.GetValueOrDefault() != 0 ? Math.Round((ct7.Tong.GetValueOrDefault() / ct4.Tong.GetValueOrDefault()), 1) : (decimal?)null,
                BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            };
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                ct8.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                {
                    Index = i,
                    CellValue = ct4.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() != 0 ? Math.Round((ct7.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / ct4.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault()), 1) : (decimal?)null,
                });
            }
            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct8);

            var soGiuongKeHoach = khoaPhongs.Where(o => hoatDongNoiTruChiTietKhoaPhongIds.Contains(o.Id.ToString())).Select(o => o.SoGiuongKeHoach.GetValueOrDefault()).DefaultIfEmpty().Sum();

            //var giuongThucKeIds = dataHoatDongNoiTrus.SelectMany(o => o.SuDungGiuongs)
            //    .Where(o => o.GiuongBenhId != null && o.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
            //                (o.ThoiDiemBatDauSuDung != null && o.ThoiDiemBatDauSuDung < queryInfo.DenNgay) &&
            //                (o.ThoiDiemKetThucSuDung == null || o.ThoiDiemKetThucSuDung > queryInfo.TuNgay))
            //    .Select(o => o.GiuongBenhId.GetValueOrDefault()).Distinct().ToList();
            //var soGiuongThucKe = giuongThucKeIds.Count();
            var soGiuongThucKe = giuongBenhs.Count();

            List<DataHoatDongNoiTruChiTietSuDungGiuongTheoKhoa> dataHoatDongNoiTruChiTietSuDungGiuongTheoKhoas = new List<DataHoatDongNoiTruChiTietSuDungGiuongTheoKhoa>();
            for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
            {
                var giuongTheoKhoa = giuongBenhs.Where(g => g.KhoaPhongId.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i]).Select(o => o.Id).ToList();
                var suDungGiuongTheoKhoa = new DataHoatDongNoiTruChiTietSuDungGiuongTheoKhoa
                {
                    KhoaPhongId = hoatDongNoiTruChiTietKhoaPhongIds[i],
                    //GiuongBenhIds = giuongThucKeIds.Where(o => giuongTheoKhoa.Contains(o)).ToList()
                    GiuongBenhIds = giuongTheoKhoa
                };
                dataHoatDongNoiTruChiTietSuDungGiuongTheoKhoas.Add(suDungGiuongTheoKhoa);
            }

            if (baoCaoThang)
            {
                var ctGiuongKeHoach = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "",
                    ChiTieu = "Tính theo giường kế hoạch",
                    CanhGiuaToDam = true,
                    Tong = soGiuongKeHoach,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = khoaPhongs.FirstOrDefault(o => o.Id.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i])?.SoGiuongKeHoach ?? 0,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ctGiuongKeHoach);

                var ct9 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "9",
                    ChiTieu = "Công suất sử dụng giường",
                    IsPerCent = true,
                    Tong = ctGiuongKeHoach.Tong.GetValueOrDefault() != 0 ? Math.Round((ct6.Tong.GetValueOrDefault() / (ctGiuongKeHoach.Tong.GetValueOrDefault() * soNgayBaoCao)), 4) : (decimal?)null,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct9.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() != 0
                            ? Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / (ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() * soNgayBaoCao)), 4)
                            : (decimal?)null,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct9);

                var ct10 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "10",
                    ChiTieu = "Ngày sử dụng giường",
                    Tong = ctGiuongKeHoach.Tong.GetValueOrDefault() != 0 ? Math.Round((ct6.Tong.GetValueOrDefault() / (ctGiuongKeHoach.Tong.GetValueOrDefault() * soThangBaoCao)), 2) : (decimal?)null,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct10.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() != 0
                            ? Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / (ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() * soThangBaoCao)), 2)
                            : (decimal?)null,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct10);

                var ct11 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "11",
                    ChiTieu = "Giường thực hiện",
                    Tong = Math.Round((ct6.Tong.GetValueOrDefault() / soNgayBaoCao), 2),
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct11.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / soNgayBaoCao), 2),
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct11);

                var ctGiuongThucKe = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "",
                    ChiTieu = "Tính theo giường thực kê",
                    CanhGiuaToDam = true,
                    Tong = soGiuongThucKe,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = dataHoatDongNoiTruChiTietSuDungGiuongTheoKhoas.FirstOrDefault(o => o.KhoaPhongId == hoatDongNoiTruChiTietKhoaPhongIds[i])?.GiuongBenhIds.Count() ?? 0,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ctGiuongThucKe);

                var ct12 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "12",
                    ChiTieu = "Công suất sử dụng giường",
                    IsPerCent = true,
                    Tong = ctGiuongThucKe.Tong.GetValueOrDefault() != 0 ? Math.Round((ct6.Tong.GetValueOrDefault() / (ctGiuongThucKe.Tong.GetValueOrDefault() * soNgayBaoCao)), 4) : (decimal?)null,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct12.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() != 0
                            ? Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / (ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() * soNgayBaoCao)), 4)
                            : (decimal?)null,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct12);

                var ct13 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "13",
                    ChiTieu = "Ngày sử dụng giường",
                    Tong = ctGiuongThucKe.Tong.GetValueOrDefault() != 0 ? Math.Round((ct6.Tong.GetValueOrDefault() / (ctGiuongThucKe.Tong.GetValueOrDefault() * soThangBaoCao)), 2) : (decimal?)null,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct13.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() != 0
                            ? Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / (ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() * soThangBaoCao)), 2)
                            : (decimal?)null,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct13);

                var ct14 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "14",
                    ChiTieu = "Giường thực hiện",
                    Tong = Math.Round((ct6.Tong.GetValueOrDefault() / soNgayBaoCao), 2),
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct14.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / soNgayBaoCao), 2),
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct14);
            }
            else
            {
                var ctGiuongKeHoach = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "",
                    ChiTieu = "Tính theo giường kế hoạch",
                    CanhGiuaToDam = true,
                    Tong = soGiuongKeHoach,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = khoaPhongs.FirstOrDefault(o => o.Id.ToString() == hoatDongNoiTruChiTietKhoaPhongIds[i])?.SoGiuongKeHoach ?? 0,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ctGiuongKeHoach);

                var ct9 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "9",
                    ChiTieu = "Công suất sử dụng giường",
                    IsPerCent = true,
                    Tong = ctGiuongKeHoach.Tong.GetValueOrDefault() != 0 ? Math.Round((ct6.Tong.GetValueOrDefault() / (ctGiuongKeHoach.Tong.GetValueOrDefault() * soNgayBaoCao)), 4) : (decimal?)null,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct9.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() != 0
                            ? Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / (ctGiuongKeHoach.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() * soNgayBaoCao)), 4)
                            : (decimal?)null,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct9);

                var ct11 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "10",
                    ChiTieu = "Giường thực hiện",
                    Tong = Math.Round((ct6.Tong.GetValueOrDefault() / soNgayBaoCao), 2),
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct11.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / soNgayBaoCao), 2),
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct11);

                var ctGiuongThucKe = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "",
                    ChiTieu = "Tính theo giường thực kê",
                    CanhGiuaToDam = true,
                    Tong = soGiuongThucKe,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = dataHoatDongNoiTruChiTietSuDungGiuongTheoKhoas.FirstOrDefault(o => o.KhoaPhongId == hoatDongNoiTruChiTietKhoaPhongIds[i])?.GiuongBenhIds.Count() ?? 0,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ctGiuongThucKe);

                var ct12 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "11",
                    ChiTieu = "Công suất sử dụng giường",
                    IsPerCent = true,
                    Tong = ctGiuongThucKe.Tong.GetValueOrDefault() != 0 ? Math.Round((ct6.Tong.GetValueOrDefault() / (ctGiuongThucKe.Tong.GetValueOrDefault() * soNgayBaoCao)), 4) : (decimal?)null,
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct12.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() != 0
                            ? Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / (ctGiuongThucKe.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() * soNgayBaoCao)), 4)
                            : (decimal?)null,
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct12);

                var ct14 = new BaoCaoHoatDongNoiTruChiTiet
                {
                    STT = "12",
                    ChiTieu = "Giường thực hiện",
                    Tong = Math.Round((ct6.Tong.GetValueOrDefault() / soNgayBaoCao), 2),
                    BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
                };
                for (int i = 0; i < hoatDongNoiTruChiTietKhoaPhongIds.Length; i++)
                {
                    ct14.BaoCaoHoatDongNoiTruChiTietColumns.Add(new BaoCaoHoatDongNoiTruChiTietColumn
                    {
                        Index = i,
                        CellValue = Math.Round((ct6.BaoCaoHoatDongNoiTruChiTietColumns[i].CellValue.GetValueOrDefault() / soNgayBaoCao), 2),
                    });
                }
                baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.Add(ct14);
            }

            return baoCaoHoatDongNoiTruChiTietVo;
        }

        public virtual byte[] ExportBaoCaoHoatDongNoiTruChiTiet(BaoCaoHoatDongNoiTruChiTietVo datas, BaoCaoHoatDongNoiTruChiTietQueryInfoVo query)
        {
            int ind = 1;
            var baoCaoHDNoiTruColumnHeaderVoCounts = datas.BaoCaoHoatDongNoiTruChiTietColumnHeader.Count();

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoHoatDongNoiTruChiTietVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KHTH] HOẠT ĐỘNG NỘI TRÚ CHI TIẾT");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 35;
                    worksheet.Column(3).Width = 25;

                    for (int i = 0; i < baoCaoHDNoiTruColumnHeaderVoCounts; i++)
                    {
                        worksheet.Column(i + 4).Width = 25;
                    }

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    string[] SetColumnItems = { "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                    var setTitleColumnItems = new List<(int, string)>();


                    for (int i = 0; i < SetColumnItems.Length; i++)
                    {
                        if (i <= baoCaoHDNoiTruColumnHeaderVoCounts)
                        {
                            setTitleColumnItems.Add((i, SetColumnItems[i]));
                        }
                    }

                    var symbolFirst = setTitleColumnItems.Select(c => c.Item2).FirstOrDefault();
                    var symbolLast = setTitleColumnItems.Select(c => c.Item2).LastOrDefault();

                    using (var range = worksheet.Cells["A2:" + symbolLast + 2])
                    {
                        range.Worksheet.Cells["A2:" + symbolLast + 2].Merge = true;
                        range.Worksheet.Cells["A2:" + symbolLast + 2].Value = "BÁO CÁO CHI TIẾT HOẠT ĐỘNG NỘI TRÚ";
                        range.Worksheet.Cells["A2:" + symbolLast + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:" + symbolLast + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:" + symbolLast + 2].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:" + symbolLast + 2].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:" + symbolLast + 2].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:" + symbolLast + 3])
                    {
                        range.Worksheet.Cells["A3:" + symbolLast + 3].Merge = true;
                        range.Worksheet.Cells["A3:" + symbolLast + 3].Value = "Từ ngày: " + query.TuNgay.FormatNgayTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.DenNgay.FormatNgayTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A3:" + symbolLast + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:" + symbolLast + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:" + symbolLast + 3].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A3:" + symbolLast + 3].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:" + symbolLast + 3].Style.Font.Bold = true;
                    }


                    int indexHeader6 = 4;
                    using (var range = worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + indexHeader6])
                    {
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + baoCaoHDNoiTruColumnHeaderVoCounts].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + baoCaoHDNoiTruColumnHeaderVoCounts].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + baoCaoHDNoiTruColumnHeaderVoCounts].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + baoCaoHDNoiTruColumnHeaderVoCounts].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + baoCaoHDNoiTruColumnHeaderVoCounts].Style.Font.Bold = true;

                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader6].Merge = true;
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader6].Value = "STT";
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader6].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader6].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + indexHeader6 + ":B" + indexHeader6].Merge = true;
                        range.Worksheet.Cells["B" + indexHeader6 + ":B" + indexHeader6].Value = "CHI TIÊU";
                        range.Worksheet.Cells["B" + indexHeader6 + ":B" + indexHeader6].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + indexHeader6 + ":B" + indexHeader6].Style.Font.Bold = true;
                        range.Worksheet.Cells["B" + indexHeader6 + ":B" + indexHeader6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + indexHeader6 + ":B" + indexHeader6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + indexHeader6 + ":B" + indexHeader6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C" + indexHeader6 + ":C" + indexHeader6].Merge = true;
                        range.Worksheet.Cells["C" + indexHeader6 + ":C" + indexHeader6].Value = "TỔNG";
                        range.Worksheet.Cells["C" + indexHeader6 + ":C" + indexHeader6].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + indexHeader6 + ":C" + indexHeader6].Style.Font.Bold = true;
                        range.Worksheet.Cells["C" + indexHeader6 + ":C" + indexHeader6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C" + indexHeader6 + ":C" + indexHeader6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + indexHeader6 + ":C" + indexHeader6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        foreach (var columnHeader in datas.BaoCaoHoatDongNoiTruChiTietColumnHeader)
                        {
                            foreach (var setTitleColumn in setTitleColumnItems)
                            {
                                if (columnHeader.Index == setTitleColumn.Item1)
                                {
                                    range.Worksheet.Cells[setTitleColumn.Item2 + indexHeader6 + ":" + setTitleColumn.Item2 + indexHeader6].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells[setTitleColumn.Item2 + indexHeader6 + ":" + setTitleColumn.Item2 + indexHeader6].Style.Font.Bold = true;
                                    range.Worksheet.Cells[setTitleColumn.Item2 + indexHeader6 + ":" + setTitleColumn.Item2 + indexHeader6].Value = columnHeader.CellText;
                                    range.Worksheet.Cells[setTitleColumn.Item2 + indexHeader6 + ":" + setTitleColumn.Item2 + indexHeader6].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells[setTitleColumn.Item2 + indexHeader6 + ":" + setTitleColumn.Item2 + indexHeader6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                }
                            }
                        }
                    }
                    var manager = new PropertyManager<BaoCaoHoatDongNoiTruChiTietVo>(requestProperties);

                    int index = 5;
                    if (datas.BaoCaoHoatDongNoiTruChiTiets.Any())
                    {
                        foreach (var item in datas.BaoCaoHoatDongNoiTruChiTiets)
                        {
                            using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                            {
                                range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                                range.Worksheet.Cells["A" + index + ":A" + index].Value = item.STT;
                                range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));


                                range.Worksheet.Cells["B" + index + ":B" + index].Merge = true;
                                range.Worksheet.Cells["B" + index + ":B" + index].Value = item.ChiTieu;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = item.CanhGiuaToDam == true ? ExcelHorizontalAlignment.Center : ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = item.CanhGiuaToDam ?? false;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.UnderLine = item.CanhGiuaToDam ?? false;

                                range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                                range.Worksheet.Cells["C" + index + ":C" + index].Value = item.Tong;


                                if (item.IsPerCent)
                                {
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Numberformat.Format = "0.00%";
                                }
                                //else
                                //{
                                //    range.Worksheet.Cells["C" + index + ":C" + index].Style.Numberformat.Format = "#,##0.##";
                                //}

                                range.Worksheet.Cells["C" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            }

                            if (item.BaoCaoHoatDongNoiTruChiTietColumns.Any())
                            {
                                foreach (var columnHeader in item.BaoCaoHoatDongNoiTruChiTietColumns)
                                {
                                    foreach (var setTitleColumn in setTitleColumnItems)
                                    {
                                        if (columnHeader.Index == setTitleColumn.Item1)
                                        {
                                            using (var range = worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index])
                                            {
                                                range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Merge = true;
                                                range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Value = columnHeader.CellValue;
                                                if (item.IsPerCent)
                                                {
                                                    range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Style.Numberformat.Format = "0.00%";
                                                }
                                                //else
                                                //{
                                                //    range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Style.Numberformat.Format = "#,##0.##";
                                                //}                                                
                                                range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                                range.Worksheet.Cells[setTitleColumn.Item2 + index + ":" + setTitleColumn.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            }
                                        }
                                    }
                                }
                            }

                            index++;
                        }
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        public virtual string HtmlBaoCaoHoatDongNoiTruChiTiet(BaoCaoHoatDongNoiTruChiTietVo datas, BaoCaoHoatDongNoiTruChiTietQueryInfoVo query)
        {
            var table = $"<table class='boder-table-main'>" +
                "<tr>" +
                "<th  class='boder-table'>STT</th>" +
                "<th  class='boder-table'>CHI TIÊU</th>" +
                "<th class='boder-table'>TỔNG</th>";

            var baoCaoHDNoiTruColumnHeaderVoCounts = datas.BaoCaoHoatDongNoiTruChiTietColumnHeader.Count();

            var khoa = string.Empty;


            foreach (var columnHeader in datas.BaoCaoHoatDongNoiTruChiTietColumnHeader)
            {
                khoa += $"<th class='boder-table'>{columnHeader.CellText}</th>";
            }

            khoa += "</tr>";
            table = table + khoa;

            var rowItem = string.Empty;
            if (datas.BaoCaoHoatDongNoiTruChiTiets.Any())
            {
                foreach (var item in datas.BaoCaoHoatDongNoiTruChiTiets)
                {
                    rowItem += "<tr>";
                    rowItem += $"<td class='boder-table'>{item.STT}</td>";
                    if (item.CanhGiuaToDam == true)
                    {
                        rowItem += $"<td class='boder-table toDamCanhGiua'>{item.ChiTieu}</td>";
                    }
                    else
                    {
                        rowItem += $"<td class='boder-table-left'>{item.ChiTieu}</td>";
                    }

                    if (item.IsPerCent && item.Tong != null)
                    {
                        rowItem += $"<td class='boder-table'>{(item.Tong * 100)?.ToString("#.##")}%</td>";
                    }
                    else
                    {
                        rowItem += $"<td class='boder-table'>{item.Tong}</td>";
                    }

                    foreach (var setTitleColumnRow in item.BaoCaoHoatDongNoiTruChiTietColumns)
                    {
                        foreach (var setTitleColumn in datas.BaoCaoHoatDongNoiTruChiTietColumnHeader)
                        {
                            if (setTitleColumnRow.Index == setTitleColumn.Index)
                            {
                                if (item.IsPerCent && setTitleColumnRow.CellValue != null)
                                {
                                    rowItem += $"<td class='boder-table'>{(setTitleColumnRow.CellValue * 100)?.ToString("#.##")}%</td>";
                                }
                                else
                                {
                                    rowItem += $"<td class='boder-table'>{setTitleColumnRow.CellValue}</td>";
                                }
                            }
                        }
                    }
                    rowItem += "</tr>";

                }
            }
            table = table + rowItem + "</table>";
            return table;
        }

        private BaoCaoHoatDongNoiTruChiTietVo BaoCaoHoatDongNoiTruChiTietDummy()
        {
            var baoCaoHoatDongNoiTruChiTietVo = new BaoCaoHoatDongNoiTruChiTietVo();

            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTietColumnHeader.AddRange(new List<BaoCaoHoatDongNoiTruChiTietColumnHeaderVo>()
            {
                new BaoCaoHoatDongNoiTruChiTietColumnHeaderVo {Index = 0 ,  CellText = "NỘI"},
                new BaoCaoHoatDongNoiTruChiTietColumnHeaderVo {Index = 1 , CellText = "NHI"},
                new BaoCaoHoatDongNoiTruChiTietColumnHeaderVo {Index = 2 ,  CellText = "NGOẠI"},
                new BaoCaoHoatDongNoiTruChiTietColumnHeaderVo {Index = 3 , CellText = "PHỤ SẢN"},
                new BaoCaoHoatDongNoiTruChiTietColumnHeaderVo {Index = 4 ,  CellText = "THẨM MỸ"},
            });


            var baoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>()
            {
                new BaoCaoHoatDongNoiTruChiTietColumn {Index = 0 ,  CellValue = 1},
                new BaoCaoHoatDongNoiTruChiTietColumn {Index = 1 , CellValue = 1},
                new BaoCaoHoatDongNoiTruChiTietColumn {Index = 2 ,  CellValue = 1},
                new BaoCaoHoatDongNoiTruChiTietColumn {Index = 3 , CellValue = 1},
                new BaoCaoHoatDongNoiTruChiTietColumn {Index = 4 ,  CellValue = 1},
            };

            var baoCaoHoatDongNoiTruChiTiets = new List<BaoCaoHoatDongNoiTruChiTiet>() {
                 new BaoCaoHoatDongNoiTruChiTiet { STT = "1" ,ChiTieu = "TS bệnh nhân điều trị" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },
                 new BaoCaoHoatDongNoiTruChiTiet { STT = "" ,ChiTieu = "T.đó: + BHYT" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },
                 new BaoCaoHoatDongNoiTruChiTiet { STT = "" ,ChiTieu = "+ Viện phí" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },


                 new BaoCaoHoatDongNoiTruChiTiet { STT = "a" ,ChiTieu = "Người bệnh còn lại kỳ BC trước" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },
                 new BaoCaoHoatDongNoiTruChiTiet { STT = "" ,ChiTieu = "T.đó: + BHYT" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },
                 new BaoCaoHoatDongNoiTruChiTiet { STT = "" ,ChiTieu = "+ Viện phí" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },


                 new BaoCaoHoatDongNoiTruChiTiet { STT = "b" ,ChiTieu = "Người bệnh mới vào viện" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },
                 new BaoCaoHoatDongNoiTruChiTiet { STT = "" ,ChiTieu = "T.đó: + BHYT" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },
                 new BaoCaoHoatDongNoiTruChiTiet { STT = "" ,ChiTieu = "+ Viện phí" ,Tong = 5 ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },

                new BaoCaoHoatDongNoiTruChiTiet { STT = "" ,ChiTieu = "Tính theo giường kế hoạch" ,Tong = 5, CanhGiuaToDam = true ,BaoCaoHoatDongNoiTruChiTietColumns = baoCaoHoatDongNoiTruChiTietColumns },
            };

            baoCaoHoatDongNoiTruChiTietVo.BaoCaoHoatDongNoiTruChiTiets.AddRange(baoCaoHoatDongNoiTruChiTiets);
            return baoCaoHoatDongNoiTruChiTietVo;
        }

    }
}
