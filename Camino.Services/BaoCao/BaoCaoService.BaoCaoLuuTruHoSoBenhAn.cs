using Camino.Core.Domain.ValueObject.BaoCaos;
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
using Camino.Core.Data;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoLuuTruHoSoBenhAnForGridAsync(BaoCaoLuuTruHoSoBenhAnQueryInfo queryInfo)
        {
            var benhAnRaViens = _noiTruBenhAnRepository.TableNoTracking
                .Include(x => x.YeuCauTiepNhan)
                .Where(x => x.ThoiDiemRaVien != null
                            && x.ThoiDiemRaVien >= queryInfo.FromDate
                            && x.ThoiDiemRaVien <= queryInfo.ToDate
                            && (queryInfo.KhoaId == null || queryInfo.KhoaId == 0 || x.NoiTruKhoaPhongDieuTris.Where(a => a.ThoiDiemRaKhoa == null).Any(a => a.KhoaPhongChuyenDenId == queryInfo.KhoaId))
                            && ((queryInfo.BHYT == true
                                    && (x.YeuCauTiepNhan.CoBHYT == true || x.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any()))
                                || (queryInfo.VienPhi == true
                                        && x.YeuCauTiepNhan.CoBHYT != true && !x.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any()))

                            //BVHD-3765: cập nhật bỏ BA con
                            && x.YeuCauTiepNhan.YeuCauNhapVien != null
                            && x.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId == null
                            )
                .OrderBy(x => x.ThuTuSapXepLuuTru)
                .ToList();

            var lstIcd = new List<ICD>();
            var result = new List<BaoCaoLuuTruHoSoBenhAnGridVo>();
            foreach (var item in benhAnRaViens)
            {
                var newThongTinLuuTru = new BaoCaoLuuTruHoSoBenhAnGridVo()
                {
                    ThuTuSapXep = item.ThuTuSapXepLuuTru,
                    SoLuuTru = item.SoLuuTru,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    GioiTinh = item.YeuCauTiepNhan.GioiTinh?.GetDescription(),
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    ThoiGianVaoVien = item.ThoiDiemNhapVien,
                    ThoiGianRaVien = item.ThoiDiemRaVien,
                };

                var thongTinRaVien = !string.IsNullOrEmpty(item.ThongTinRaVien) ? JsonConvert.DeserializeObject<RaVien>(item.ThongTinRaVien) : new RaVien();
                newThongTinLuuTru.ChanDoan = thongTinRaVien.GhiChuChuanDoanRaVien;

                var icdTemp = lstIcd.FirstOrDefault(x => x.Id == thongTinRaVien.ChuanDoanRaVienId);
                if (icdTemp == null && thongTinRaVien.ChuanDoanRaVienId != null)
                {
                    icdTemp = _icdRepository.TableNoTracking.FirstOrDefault(x => x.Id == thongTinRaVien.ChuanDoanRaVienId);
                    lstIcd.Add(icdTemp);
                }
                newThongTinLuuTru.ICD = icdTemp?.Ma;

                result.Add(newThongTinLuuTru);
            }
            
            return new GridDataSource { Data = result.ToArray(), TotalRowCount = result.Count };
        }

        public virtual byte[] ExportBaoCaoLuuTruHoSoBenhAn(GridDataSource gridDataSource, BaoCaoLuuTruHoSoBenhAnQueryInfo query)
        {
            var datas = (ICollection<BaoCaoLuuTruHoSoBenhAnGridVo>)gridDataSource.Data;
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();

            //var ind = 1;
            //var requestProperties = new[]
            //{
            //    new PropertyByName<BaoCaoLuuHoSoBenhAnGridVo>("STT", p => ind++)
            //};
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO LƯU TRỮ HỒ SƠ BỆNH ÁN");

                    //set row
                    worksheet.DefaultRowHeight = 16;

                    //set chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    worksheet.Row(7).Height = 21;

                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    }

                    using (var range = worksheet.Cells["A7:J7"])
                    {
                        range.Worksheet.Cells["A7:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 17));
                        range.Worksheet.Cells["A7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A7:J7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:J7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:J7"].Merge = true;
                        range.Worksheet.Cells["A7:J7"].Value = "BÁO CÁO LƯU TRỮ HỒ SƠ BỆNH ÁN";
                    }

                    using (var range = worksheet.Cells["A8:J8"])
                    {
                        range.Worksheet.Cells["A8:J8"].Merge = true;
                        range.Worksheet.Cells["A8:J8"].Value = "Từ: " + query.FromDate.ApplyFormatDateTime()
                                                          + " đến: " + query.ToDate.ApplyFormatDateTime();
                        range.Worksheet.Cells["A8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:J8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:J8"].Style.Font.Italic = true;
                    }

                    var tenKhoa = "Toàn viện";
                    if (query.KhoaId != null && query.KhoaId != 0)
                    {
                        tenKhoa = _KhoaPhongRepository.TableNoTracking.Where(p => p.Id == query.KhoaId).Select(p => p.Ten).FirstOrDefault();
                    }
                    using (var range = worksheet.Cells["A9:J9"])
                    {
                        range.Worksheet.Cells["A9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A9:J9"].Style.Font.Color.SetColor(Color.Black);

                        range.Worksheet.Cells["E9"].Value = "Khoa: ";

                        range.Worksheet.Cells["F9:G9"].Value = tenKhoa;
                        range.Worksheet.Cells["F9:G9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["F9:G9"].Merge = true;

                    }

                    using (var range= worksheet.Cells["A11:J11"])
                    {
                        range.Worksheet.Cells["A11:J11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A11:J11"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A11:J11"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A11:J11"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A11:J11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A11"].Value = "STT";

                        range.Worksheet.Cells["B11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B11"].Value = "Thứ tự sắp xếp";
                        range.Worksheet.Cells["B11"].Style.WrapText = true;


                        range.Worksheet.Cells["C11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C11"].Value = "Số lưu trữ";

                        range.Worksheet.Cells["D11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D11"].Value = "Họ tên";

                        range.Worksheet.Cells["E11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E11"].Value = "Giới tính";

                        range.Worksheet.Cells["F11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F11"].Value = "Ngày sinh";

                        range.Worksheet.Cells["G11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G11"].Style.WrapText = true;
                        range.Worksheet.Cells["G11"].Value = "Thời gian vào viện";

                        range.Worksheet.Cells["H11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H11"].Value = "Thời gian ra viện";
                        range.Worksheet.Cells["H11"].Style.WrapText = true;

                        range.Worksheet.Cells["I11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I11"].Value = "Chẩn đoán điều trị";
                        range.Worksheet.Cells["I11"].Style.WrapText = true;

                        range.Worksheet.Cells["J11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J11"].Value = "ICD";
                    }
                    //var manager = new PropertyManager<BaoCaoLuuTruHoSoBenhAnGridVo>(requestProperties);
                    int index = 12; // bắt đầu đổ data từ dòng 12

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach(var item in datas)
                        {
                            using(var range= worksheet.Cells["A" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Value=stt;

                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Value = item.ThuTuSapXep;

                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Value = item.SoLuuTru;

                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Value = item.HoTen;

                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Value = item.GioiTinh;

                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Value = item.NgaySinhDisplay;

                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Value = item.ThoiGianVaoVienString;

                                range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["H" + index].Value = item.ThoiGianRaVienString;

                                range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["I" + index].Value = item.ChanDoan;

                                range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["J" + index].Value = item.ICD;
                                index++;
                                stt++;
                            }
                        }
                    }

                    index++;

                    worksheet.Cells["H" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["H" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.Bold = true;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.Italic = true;
                    worksheet.Cells["H" + index + ":J" + index].Value= $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    worksheet.Cells["H" + index + ":J" + index].Merge = true;

                    index++;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["H" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.Bold = true;
                    worksheet.Cells["H" + index + ":J" + index].Value="Người lập";
                    worksheet.Cells["H" + index + ":J" + index].Merge = true;

                    index++;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["H" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.Italic = true;
                    worksheet.Cells["H" + index + ":J" + index].Value = "(Ký, ghi rõ họ tên)";
                    worksheet.Cells["H" + index + ":J" + index].Merge = true;

                    index = index + 4;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["H" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["H" + index + ":J" + index].Style.Font.Bold = true;
                    worksheet.Cells["H" + index + ":J" + index].Value = nguoiLogin;
                    worksheet.Cells["H" + index + ":J" + index].Merge = true;

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        public async Task<List<LookupItemTemplateVo>> GetKhoaKhamNoiTruAsync(DropDownListRequestModel model)
        {
            var khoaCoNhapVienIds = await _noiTruKhoaPhongDieuTriRepository.TableNoTracking
                .Select(x => x.KhoaPhongChuyenDenId)
                .Distinct().ToArrayAsync();

            var khoaPhongs = await _KhoaPhongRepository.TableNoTracking
                .Where(x => khoaCoNhapVienIds.Contains(x.Id))
                .ApplyLike(model.Query?.Trim(), x => x.Ma, x => x.Ten)
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.Id,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DisplayName = item.Ten
                })
                .Take(model.Take)
                .ToListAsync();
            return khoaPhongs;
        }
    }
}
