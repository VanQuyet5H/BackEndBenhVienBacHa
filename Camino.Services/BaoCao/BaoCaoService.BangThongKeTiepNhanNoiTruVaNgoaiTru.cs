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
        public List<LookupItemVo> GetTatKhoaChoTiepNhanNoiVaNgoaiTru(DropDownListRequestModel queryInfo)
        {
            var allKhoas = new List<LookupItemVo>()
            {
                new LookupItemVo {KeyId = 0 , DisplayName = "Toàn viện" }
            };
            var result = _KhoaPhongRepository.TableNoTracking
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take);

            allKhoas.AddRange(result);

            return allKhoas;
        }

        public async Task<GridDataSource> GetDataBangThongKeTiepNhanNoiTruVaNgoaiTruForGrid(BangThongKeTiepNhanNoiTruVaNgoaiTruQueryInfoVo queryInfo)
        {
            var queryNgoaiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            o.ThoiDiemTiepNhan >= queryInfo.TuNgay && o.ThoiDiemTiepNhan < queryInfo.DenNgay);
            if (!string.IsNullOrEmpty(queryInfo.TimKiem))
            {
                queryNgoaiTru = queryNgoaiTru.ApplyLike(queryInfo.TimKiem, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }

            var ngoaiTruData = queryNgoaiTru.Select(o => new DanhSachBangThongKeTiepNhanNoiTruVaNgoaiTruQueryData
            {
                Id = o.Id,
                MaNB = o.BenhNhan.MaBN,
                MaTN = o.MaYeuCauTiepNhan,
                ThoiGianTiepNhan = o.ThoiDiemTiepNhan,
                HoTen = o.HoTen,
                NgaySinh = o.NgaySinh,
                ThangSinh = o.ThangSinh,
                NamSinh = o.NamSinh,
                GioiTinh = o.GioiTinh,
                SoDienThoai = o.SoDienThoai,
                DiaChi = o.DiaChiDayDu,
                CoBHYT = o.CoBHYT,
                BHYTMucHuong = o.BHYTMucHuong,
                BHYTMaSoThe = o.BHYTMaSoThe,
                QuyetToanTheoNoiTru = o.QuyetToanTheoNoiTru,
                CongTyBHTNIds = o.YeuCauTiepNhanCongTyBaoHiemTuNhans.Select(t=>t.CongTyBaoHiemTuNhanId).ToList(),
                HinhThucDenId = o.HinhThucDenId,
                NoiGioiThieuId = o.NoiGioiThieuId
            }).ToList();

            var ngoaiTruIds = ngoaiTruData.Where(o=>o.QuyetToanTheoNoiTru == true).Select(o => o.Id).ToList();
            var queryNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            o.NoiTruBenhAn != null && ngoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()));
            var noiTruData = queryNoiTru.Select(o => new DanhSachBangThongKeTiepNhanNoiTruVaNgoaiTruQueryData
            {
                Id = o.Id,
                YeuCauTiepNhanNgoaiTruCanQuyetToanId = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                CoBHYT = o.CoBHYT,
                BHYTMucHuong = o.BHYTMucHuong,
                BHYTMaSoThe = o.BHYTMaSoThe,
                SoBenhAn = o.NoiTruBenhAn.SoBenhAn,
                ThoiGianNhapVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                KhoaDieuTriId = o.NoiTruBenhAn.KhoaPhongNhapVienId,
                KhoaDieuTri = o.NoiTruBenhAn.KhoaPhongNhapVien.Ten,
                ChanDoanNhapVienICDId = o.YeuCauNhapVien.ChanDoanNhapVienICDId,
                ChanDoanNhapVienGhiChu = o.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                ChanDoanNhapVienKemTheos = o.YeuCauNhapVien.YeuCauNhapVienChanDoanKemTheos.Select(k=> new ChanDoanKemTheo{ChanDoanICDId = k.ICDId,GhiChu = k.GhiChu}).ToList(),
                KhoaPhongDieuTris = o.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Select(k=>new KhoaPhongDieuTri{ KhoaPhongChuyenDenId = k.KhoaPhongChuyenDenId, KhoaPhongChuyenDen = k.KhoaPhongChuyenDen.Ten , ThoiDiemVaoKhoa = k.ThoiDiemVaoKhoa}).ToList(),
                ChanDoanChinhRaVienICDId = o.NoiTruBenhAn.ChanDoanChinhRaVienICDId,
                ChanDoanChinhRaVienGhiChu = o.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,
                DanhSachChanDoanKemTheoRaVienICDId = o.NoiTruBenhAn.DanhSachChanDoanKemTheoRaVienICDId,
                DanhSachChanDoanKemTheoRaVienGhiChu = o.NoiTruBenhAn.DanhSachChanDoanKemTheoRaVienGhiChu,
                ThoiGianRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,
                KetQuaDieuTri = o.NoiTruBenhAn.KetQuaDieuTri,
                TheBHYTs = o.YeuCauTiepNhanTheBHYTs.Select(k => new TheBHYT { MaSoThe = k.MaSoThe, MucHuong = k.MucHuong }).ToList(),
            }).ToList();
            var icdIds = new List<long>();
            foreach (var noiTru in noiTruData)
            {
                if (noiTru.ChanDoanNhapVienICDId != null)
                {
                    icdIds.Add(noiTru.ChanDoanNhapVienICDId.Value);
                }

                if (noiTru.ChanDoanNhapVienKemTheos.Count > 0)
                {
                    icdIds.AddRange(noiTru.ChanDoanNhapVienKemTheos.Select(o => o.ChanDoanICDId).ToList());
                }
                if (noiTru.ChanDoanChinhRaVienICDId != null)
                {
                    icdIds.Add(noiTru.ChanDoanChinhRaVienICDId.Value);
                }
                if (!string.IsNullOrEmpty(noiTru.DanhSachChanDoanKemTheoRaVienICDId))
                {
                    var icdKemTheoIds = noiTru.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                    foreach (var icdKemTheoId in icdKemTheoIds)
                    {
                        icdIds.Add(long.Parse(icdKemTheoId));
                    }
                }
                
            }

            icdIds = icdIds.Distinct().ToList();
            var icdData = _icdRepository.TableNoTracking.Where(o => icdIds.Contains(o.Id)).Select(o=>new {o.Id, o.Ma}).ToList();

            var congTyBaoHiemTuNhans = _congTyBaoHiemTuNhanRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var hinhThucDens = _hinhThucDenRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var noiGioiThieus = _noiGioiThieuRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();

            var returnData = new List<DanhSachBangThongKeTiepNhanNoiTruVaNgoaiTru>();
            foreach (var yctn in ngoaiTruData)
            {
                var noiTru = noiTruData.FirstOrDefault(o =>
                    o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == yctn.Id);
                if (noiTru != null)
                {
                    yctn.CoBHYT = noiTru.CoBHYT;
                    yctn.BHYTMucHuong = noiTru.BHYTMucHuong;
                    yctn.BHYTMaSoThe = noiTru.BHYTMaSoThe;
                    yctn.SoBenhAn = noiTru.SoBenhAn;
                    yctn.ThoiGianNhapVien = noiTru.ThoiGianNhapVien;
                    yctn.KhoaDieuTriId = noiTru.KhoaDieuTriId;
                    yctn.KhoaDieuTri = noiTru.KhoaDieuTri;
                    yctn.ChanDoanNhapVienICDId = noiTru.ChanDoanNhapVienICDId;
                    yctn.ChanDoanNhapVienGhiChu = noiTru.ChanDoanNhapVienGhiChu;
                    yctn.ChanDoanNhapVienKemTheos = noiTru.ChanDoanNhapVienKemTheos;
                    yctn.KhoaPhongDieuTris = noiTru.KhoaPhongDieuTris;
                    yctn.ChanDoanChinhRaVienICDId = noiTru.ChanDoanChinhRaVienICDId;
                    yctn.ChanDoanChinhRaVienGhiChu = noiTru.ChanDoanChinhRaVienGhiChu;
                    yctn.DanhSachChanDoanKemTheoRaVienICDId = noiTru.DanhSachChanDoanKemTheoRaVienICDId;
                    yctn.DanhSachChanDoanKemTheoRaVienGhiChu = noiTru.DanhSachChanDoanKemTheoRaVienGhiChu;
                    yctn.ThoiGianRaVien = noiTru.ThoiGianRaVien;
                    yctn.KetQuaDieuTri = noiTru.KetQuaDieuTri;
                    if (noiTru.TheBHYTs.Any())
                    {
                        yctn.BHYTMucHuong = noiTru.TheBHYTs.Last().MucHuong;
                        yctn.BHYTMaSoThe = noiTru.TheBHYTs.Last().MaSoThe;
                    }
                }
                if(queryInfo.KhoaId != 0 && yctn.KhoaDieuTriId.GetValueOrDefault() != queryInfo.KhoaId)
                    continue;
                if ((queryInfo.LoaiYeuCauTiepNhan == LoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && yctn.ThoiGianNhapVien == null) || 
                    (queryInfo.LoaiYeuCauTiepNhan == LoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && yctn.ThoiGianNhapVien != null))
                    continue;
                if ((queryInfo.TrangThaiDieuTri == Core.Domain.ValueObject.BaoCaos.TrangThaiDieuTri.DangDieuTri && (yctn.ThoiGianNhapVien == null || yctn.ThoiGianRaVien != null)) || 
                    (queryInfo.TrangThaiDieuTri == Core.Domain.ValueObject.BaoCaos.TrangThaiDieuTri.DaRaVien && (yctn.ThoiGianNhapVien == null || yctn.ThoiGianRaVien == null)))
                    continue;
                var chanDoanVaoVien = string.Empty;
                if (yctn.ChanDoanNhapVienICDId != null)
                {
                    chanDoanVaoVien = $"{icdData.FirstOrDefault(o=>o.Id == yctn.ChanDoanNhapVienICDId)?.Ma} -{yctn.ChanDoanNhapVienGhiChu}";
                }

                if (yctn.ChanDoanNhapVienKemTheos != null && yctn.ChanDoanNhapVienKemTheos.Count > 0)
                {
                    foreach (var yctnChanDoanNhapVienKemTheo in yctn.ChanDoanNhapVienKemTheos)
                    {
                        chanDoanVaoVien = chanDoanVaoVien + "; " + $"{icdData.FirstOrDefault(o => o.Id == yctnChanDoanNhapVienKemTheo.ChanDoanICDId)?.Ma} -{yctnChanDoanNhapVienKemTheo.GhiChu}";
                    }
                }
                var khoaChuyenDen = string.Empty;
                var thoiGianChuyenKhoa = string.Empty;
                if (yctn.KhoaPhongDieuTris != null && yctn.KhoaPhongDieuTris.Count > 1)
                {
                    var khoaPhongDieuTris = yctn.KhoaPhongDieuTris.Where(o => o.KhoaPhongChuyenDenId != (long)EnumKhoaPhong.KhoaGMHS && o.KhoaPhongChuyenDenId != yctn.KhoaDieuTriId && o.ThoiDiemVaoKhoa < queryInfo.DenNgay)
                        .OrderBy(o => o.ThoiDiemVaoKhoa).ToList();
                    for (int i = 0; i < khoaPhongDieuTris.Count(); i++)
                    {
                        if (i == 0)
                        {
                            khoaChuyenDen = khoaPhongDieuTris[i].KhoaPhongChuyenDen;
                            thoiGianChuyenKhoa = khoaPhongDieuTris[i].ThoiDiemVaoKhoa.ApplyFormatDateTime();
                        }
                        else if(i > 0)
                        {
                            khoaChuyenDen = khoaChuyenDen + "; " + khoaPhongDieuTris[i].KhoaPhongChuyenDen;
                            thoiGianChuyenKhoa = thoiGianChuyenKhoa + "; " + khoaPhongDieuTris[i].ThoiDiemVaoKhoa.ApplyFormatDateTime();
                        }
                    }
                }

                var chanDoanRaVien = string.Empty;
                if (yctn.ChanDoanChinhRaVienICDId != null)
                {
                    chanDoanRaVien = $"{icdData.FirstOrDefault(o => o.Id == yctn.ChanDoanChinhRaVienICDId)?.Ma} -{yctn.ChanDoanChinhRaVienGhiChu}";
                }
                if (!string.IsNullOrEmpty(yctn.DanhSachChanDoanKemTheoRaVienICDId) && !string.IsNullOrEmpty(yctn.DanhSachChanDoanKemTheoRaVienGhiChu))
                {
                    var icdKemTheoIds = yctn.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                    var icdKemTheoGhiChus = yctn.DanhSachChanDoanKemTheoRaVienGhiChu.Split(Constants.ICDSeparator);
                    for (int i = 0; i < icdKemTheoIds.Count(); i++)
                    {
                        chanDoanRaVien = chanDoanRaVien + "; " + $"{icdData.FirstOrDefault(o => o.Id == long.Parse(icdKemTheoIds[i]))?.Ma} -{(icdKemTheoGhiChus.Length > i ? icdKemTheoGhiChus[i] : string.Empty )}";
                    }
                }
                var hinhThucDen = string.Empty;
                if(yctn.HinhThucDenId != null)
                {
                    if(yctn.HinhThucDenId == cauHinhBaoCao.HinhThucDenGioiThieuId)
                    {
                        hinhThucDen = $"{hinhThucDens.FirstOrDefault(o => o.Id == yctn.HinhThucDenId)?.Ten}/{noiGioiThieus.FirstOrDefault(o => o.Id == yctn.NoiGioiThieuId)?.Ten}";
                    }
                    else
                    {
                        hinhThucDen = $"{hinhThucDens.FirstOrDefault(o => o.Id == yctn.HinhThucDenId)?.Ten}";
                    }
                }
                var bhtn = string.Empty;
                if(yctn.CongTyBHTNIds != null && yctn.CongTyBHTNIds.Any())
                {
                    bhtn = string.Join(", ", congTyBaoHiemTuNhans.Where(o => yctn.CongTyBHTNIds.Contains(o.Id)).Select(o => o.Ten));
                }

                var itemData = new DanhSachBangThongKeTiepNhanNoiTruVaNgoaiTru()
                {
                    MaNB = yctn.MaNB,
                    MaTN = yctn.MaTN,
                    SoBenhAn = yctn.SoBenhAn,
                    ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,
                    HoTen = yctn.HoTen,
                    NamSinh = yctn.NamSinh?.ToString(),
                    GioiTinh = yctn.GioiTinh,
                    SoDienThoai = yctn.SoDienThoai,
                    DiaChi = yctn.DiaChi,
                    KhoaDieuTri = yctn.KhoaDieuTri,
                    ThoiGianNhapVien = yctn.ThoiGianNhapVien,
                    ChanDoanVaoVien = chanDoanVaoVien,
                    KhoaChuyenDen = khoaChuyenDen,
                    ThoiGianChuyenKhoaStr = thoiGianChuyenKhoa,
                    ChanDoanRaVien = chanDoanRaVien,
                    ThoiGianRaVien = yctn.ThoiGianRaVien,
                    KetQuaDieuTri = yctn.KetQuaDieuTri?.GetDescription(),
                    DoiTuong = yctn.CoBHYT != true ? "Viện phí" : "BHYT (" + yctn.BHYTMucHuong + "%)",
                    SoTheBHYT = yctn.BHYTMaSoThe,
                    LoaiYeuCauTiepNhan = yctn.ThoiGianNhapVien != null ? "Nội trú" : "Ngoại trú",
                    TrangThaiDieuTri = yctn.ThoiGianNhapVien == null ? "" : (yctn.ThoiGianRaVien != null ? "Đã ra viện" : "Đang điều trị"),
                    HinhThucDen = hinhThucDen,
                    BHTN = bhtn
                };
                returnData.Add(itemData);
            }


            return new GridDataSource
            {
                Data = returnData.OrderBy(o=>o.ThoiGianTiepNhan).ToArray(),
                TotalRowCount = returnData.Count()
            };
        }

        public virtual byte[] ExportBangThongKeTiepNhanNoiTruVaNgoaiTru(GridDataSource gridDataSource, BangThongKeTiepNhanNoiTruVaNgoaiTruQueryInfoVo query)
        {
            var datas = (ICollection<DanhSachBangThongKeTiepNhanNoiTruVaNgoaiTru>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoNguoiBenhDenKhamGridVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KHTH] THỐNG KÊ TIẾP NHẬN NỘI TRÚ, NGOẠI TRÚ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 50;
                    worksheet.Column(16).Width = 50;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 30;
                    worksheet.Column(24).Width = 30;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:V1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:X2"])
                    {
                        range.Worksheet.Cells["A2:X2"].Merge = true;
                        range.Worksheet.Cells["A2:X2"].Value = "BẢNG THỐNG KÊ TIẾP NHẬN NỘI TRÚ, NGOẠI TRÚ";
                        range.Worksheet.Cells["A2:X2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:X2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:X2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:X2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:X2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:X3"])
                    {
                        range.Worksheet.Cells["A3:X3"].Merge = true;
                        range.Worksheet.Cells["A3:X3"].Value = "Từ ngày: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A3:X3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:X3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:X3"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A3:X3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:X3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:X5"])
                    {
                        range.Worksheet.Cells["A5:X5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:X5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:X5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:X5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:X5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:X5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                
                        range.Worksheet.Cells["A5:A5"].Merge = true;
                        range.Worksheet.Cells["A5:A5"].Value = "STT";
                        range.Worksheet.Cells["A5:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B5:B5"].Merge = true;
                        range.Worksheet.Cells["B5:B5"].Value = "Mã BN";
                        range.Worksheet.Cells["B5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B5:B5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C5:C5"].Merge = true;
                        range.Worksheet.Cells["C5:C5"].Value = "Mã TN";
                        range.Worksheet.Cells["C5:C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C5:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D5:D5"].Merge = true;
                        range.Worksheet.Cells["D5:D5"].Value = "Số BA của NB (nếu có)";
                        range.Worksheet.Cells["D5:D5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D5:D5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E5:E5"].Merge = true;
                        range.Worksheet.Cells["E5:E5"].Value = "Thời gian tiếp nhận";
                        range.Worksheet.Cells["E5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E5:E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F5:F5"].Merge = true;
                        range.Worksheet.Cells["F5:F5"].Value = "Họ tên";
                        range.Worksheet.Cells["F5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F5:F5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G5:G5"].Merge = true;
                        range.Worksheet.Cells["G5:G5"].Value = "Năm sinh";
                        range.Worksheet.Cells["G5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G5:G5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H5:H5"].Merge = true;
                        range.Worksheet.Cells["H5:H5"].Value = "Giới tính";
                        range.Worksheet.Cells["H5:H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H5:H5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I5:I5"].Merge = true;
                        range.Worksheet.Cells["I5:I5"].Value = "Số điện thoại";
                        range.Worksheet.Cells["I5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I5:I5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J5:J5"].Merge = true;
                        range.Worksheet.Cells["J5:J5"].Value = "Địa chỉ";
                        range.Worksheet.Cells["J5:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J5:J5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K5:K5"].Merge = true;
                        range.Worksheet.Cells["K5:K5"].Value = "Khoa điều trị";
                        range.Worksheet.Cells["K5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K5:K5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L5:L5"].Merge = true;
                        range.Worksheet.Cells["L5:L5"].Value = "Thời gian nhập viện";
                        range.Worksheet.Cells["L5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L5:L5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M5:M5"].Merge = true;
                        range.Worksheet.Cells["M5:M5"].Value = "Chẩn đoán vào viện";
                        range.Worksheet.Cells["M5:M5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M5:M5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                        range.Worksheet.Cells["N5:N5"].Merge = true;
                        range.Worksheet.Cells["N5:N5"].Value = "Khoa chuyển đến";
                        range.Worksheet.Cells["N5:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N5:N5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O5:O5"].Merge = true;
                        range.Worksheet.Cells["O5:O5"].Value = "Thời gian chuyển khoa";
                        range.Worksheet.Cells["O5:O5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O5:O5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P5:P5"].Merge = true;
                        range.Worksheet.Cells["P5:P5"].Value = "Chẩn đoán ra viện";
                        range.Worksheet.Cells["P5:P5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P5:P5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q5:Q5"].Merge = true;
                        range.Worksheet.Cells["Q5:Q5"].Value = "Thời gian ra viện";
                        range.Worksheet.Cells["Q5:Q5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q5:Q5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                        range.Worksheet.Cells["R5:R5"].Merge = true;
                        range.Worksheet.Cells["R5:R5"].Value = "Kết quả điều trị";
                        range.Worksheet.Cells["R5:R5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R5:R5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S5:S5"].Merge = true;
                        range.Worksheet.Cells["S5:S5"].Value = "Đối tượng";
                        range.Worksheet.Cells["S5:S5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S5:S5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T5:T5"].Merge = true;
                        range.Worksheet.Cells["T5:T5"].Value = "Số BHYT";
                        range.Worksheet.Cells["T5:T5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["T5:T5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["U5:U5"].Merge = true;
                        range.Worksheet.Cells["U5:U5"].Value = "Loại";
                        range.Worksheet.Cells["U5:U5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["U5:U5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["V5:V5"].Merge = true;
                        range.Worksheet.Cells["V5:V5"].Value = "Trạng thái";
                        range.Worksheet.Cells["V5:V5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["V5:V5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["W5:W5"].Merge = true;
                        range.Worksheet.Cells["W5:W5"].Value = "BHTN";
                        range.Worksheet.Cells["W5:W5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["W5:W5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["X5:X5"].Merge = true;
                        range.Worksheet.Cells["X5:X5"].Value = "Hình thức đến";
                        range.Worksheet.Cells["X5:X5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["X5:X5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<BaoCaoNguoiBenhDenKhamGridVo>(requestProperties);
                    int index = 6;
                  
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":X" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":X" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":X" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":X" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaNB;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaTN;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.SoBenhAn;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.ThoiGianTiepNhanStr;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.HoTen;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.NamSinh;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.GioiTinhStr;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.SoDienThoai;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.DiaChi;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.KhoaDieuTri;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.ThoiGianNhapVienStr;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.ChanDoanVaoVien;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.KhoaChuyenDen;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.ThoiGianChuyenKhoaStr;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.ChanDoanVaoVien;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.ThoiGianRaVienStr;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.KetQuaDieuTri;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.DoiTuong;

                                worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.SoTheBHYT;

                                worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["U" + index].Value = item.LoaiYeuCauTiepNhan;
                                
                                worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["V" + index].Value = item.TrangThaiDieuTri;

                                worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["W" + index].Value = item.BHTN;

                                worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["X" + index].Value = item.HinhThucDen;

                                index++;
                                stt++;
                            }
                        }
                    }
                    
                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
