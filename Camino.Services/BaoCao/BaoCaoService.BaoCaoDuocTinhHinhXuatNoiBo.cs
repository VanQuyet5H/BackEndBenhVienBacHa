using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoDuocTinhHinhXuatNoiBoForGridAsync(BaoCaoDuocTinhHinhXuatNoiBoQueryInfo queryInfo)
        {
            var xuatKhoDuocPhamQuery = _xuatKhoDuocPhamRepository.TableNoTracking
                .Where(o => o.KhoXuatId == queryInfo.KhoId && o.KhoNhapId != null && o.NgayXuat >= queryInfo.FromDate && o.NgayXuat < queryInfo.ToDate);

            var xuatKhoDuocPhamData = xuatKhoDuocPhamQuery.Select(o => new
            {
                YeuCauLinhs = o.YeuCauLinhDuocPham.YeuCauLinhDuocPhamChiTiets.Select(x=>new { x.DuocPhamBenhVienId, x.SoLuong }).ToList(),
                ChiTietDuocPhams = o.XuatKhoDuocPhamChiTiets.SelectMany(x => x.XuatKhoDuocPhamChiTietViTris)
                    .Select(y => new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                    {
                        Id = y.Id,
                        DuocPhamBenhVienId = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        Ma = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                        Ten = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        Nhom = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        SoLuongThucXuat = y.SoLuongXuat,
                        DonGia = y.NhapKhoDuocPhamChiTiet.DonGiaBan,
                        SoLuongYeuCau = y.YeuCauDieuChuyenDuocPhamChiTiets.Select(z => z.SoLuongDieuChuyen).DefaultIfEmpty().Sum()
                    }).ToList()
            }).ToList();
            var baoCaoDuocTinhHinhXuatNoiBoGridVos = new List<BaoCaoDuocTinhHinhXuatNoiBoGridVo>();
            foreach (var xuatKhoDuocPham in xuatKhoDuocPhamData)
            {
                if (xuatKhoDuocPham.YeuCauLinhs.Any())
                {
                    var chiTietOrders = xuatKhoDuocPham.ChiTietDuocPhams.OrderBy(o => o.DuocPhamBenhVienId).ToList();
                    long currentDuocPhamBenhVienId = 0;
                    double soLuongYeuCauConLai = 0;
                    for (int i = 0; i < chiTietOrders.Count; i++)
                    {
                        if(chiTietOrders[i].DuocPhamBenhVienId != currentDuocPhamBenhVienId)
                        {
                            currentDuocPhamBenhVienId = chiTietOrders[i].DuocPhamBenhVienId;
                            soLuongYeuCauConLai = xuatKhoDuocPham.YeuCauLinhs.Where(o => o.DuocPhamBenhVienId == currentDuocPhamBenhVienId).Select(o => o.SoLuong).DefaultIfEmpty().Sum();
                        }
                        if(i < (chiTietOrders.Count - 1) && chiTietOrders[i+1].DuocPhamBenhVienId == currentDuocPhamBenhVienId)
                        {
                            chiTietOrders[i].SoLuongYeuCau = chiTietOrders[i].SoLuongThucXuat;
                            soLuongYeuCauConLai = soLuongYeuCauConLai - chiTietOrders[i].SoLuongYeuCau;
                        }
                        else
                        {
                            chiTietOrders[i].SoLuongYeuCau = Math.Round(soLuongYeuCauConLai,2);
                        }
                    }
                    baoCaoDuocTinhHinhXuatNoiBoGridVos.AddRange(chiTietOrders);
                }
                else
                {
                    baoCaoDuocTinhHinhXuatNoiBoGridVos.AddRange(xuatKhoDuocPham.ChiTietDuocPhams);
                }
            }
            var dataReturn = baoCaoDuocTinhHinhXuatNoiBoGridVos
                .GroupBy(o => new { o.DuocPhamBenhVienId, o.DonGia }, o => o,
                (k, v) => new BaoCaoDuocTinhHinhXuatNoiBoGridVo 
                { 
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    DonGia = k.DonGia,
                    Ma = v.First().Ma,
                    Ten = v.First().Ten,
                    DVT = v.First().DVT,
                    Nhom = v.First().Nhom,
                    SoLuongYeuCau = v.Sum(x=>x.SoLuongYeuCau).MathRoundNumber(2),
                    SoLuongThucXuat = v.Sum(x => x.SoLuongThucXuat).MathRoundNumber(2)
                }).ToArray();

            /*
            var data = new List<BaoCaoDuocTinhHinhXuatNoiBoGridVo>()
            {
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=1,
                    Ma="LIHT200",
                    Ten="Lidocain Hydroclorid 40mg/2ml",
                    DVT="Ống",
                    SoLuongYeuCau=46,
                    SoLuongThucXuat=46,
                    DonGia = 610,
                    Nhom="Các thuốc khác"
                },
                 new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=2,
                    Ma="NACT456",
                    Ten="Natri clorid 0,9 % 10ml",
                    DVT="Lọ",
                    SoLuongYeuCau=207,
                    SoLuongThucXuat=207,
                    DonGia = 2800,
                    Nhom="Các thuốc khác"
                },
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=3,
                    Ma="NEMT422",
                    Ten="Nexium 10mg",
                    DVT="Gói",
                    SoLuongYeuCau=8,
                    SoLuongThucXuat=8,
                    DonGia = (decimal)21787.67,
                    Nhom="Các thuốc khác"
                },
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=4,
                    Ma="NUCT408",
                    Ten="Nước cất 5ml 5ml",
                    DVT="Ống",
                    SoLuongYeuCau=808,
                    SoLuongThucXuat=808,
                    DonGia = (decimal)708.68,
                    Nhom="Các thuốc khác"
                },
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=5,
                    Ma="STGT200",
                    Ten="Stiprol 9g",
                    DVT="Tube",
                    SoLuongYeuCau=47,
                    SoLuongThucXuat=47,
                    DonGia = 6930,
                    Nhom="Các thuốc khác"
                },

                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=6,
                    Ma="GLUT422",
                    Ten="Glucose 10% 500ml 10%",
                    DVT="Chai",
                    SoLuongYeuCau=3,
                    SoLuongThucXuat=3,
                    DonGia = (decimal)13499.85,
                    Nhom="Dịch Truyền*"
                },
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=7,
                    Ma="GLUT430",
                    Ten="Glucose 5% 500ml 5%",
                    DVT="Chai",
                    SoLuongYeuCau=49,
                    SoLuongThucXuat=49,
                    DonGia = (decimal)11088.00,
                    Nhom="Dịch Truyền*"
                },
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=8,
                    Ma="NACT420",
                    Ten="Natri clorid 0,9% 500ml 0,9%",
                    DVT="Chai",
                    SoLuongYeuCau=3,
                    SoLuongThucXuat=3,
                    DonGia =(decimal) 10815.00,
                    Nhom="Dịch Truyền*"
                },
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=9,
                    Ma="RILT406",
                    Ten="Ringer lactat & glucose 5% 500ml 5%",
                    DVT="Chai",
                    SoLuongYeuCau=1,
                    SoLuongThucXuat=1,
                    DonGia = (decimal)12705.00,
                    Nhom="Dịch Truyền*"
                },
                new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=10,
                    Ma="RIMT202",
                    Ten="Ringerlactat 500ml",
                    DVT="Chai",
                    SoLuongYeuCau=69,
                    SoLuongThucXuat=69,
                    DonGia = (decimal)11100.66,
                    Nhom="Dịch Truyền*"
                },

                 new BaoCaoDuocTinhHinhXuatNoiBoGridVo
                {
                    Id=10,
                    Ma="MABT412",
                    Ten="Magnesi - BFS 15%",
                    DVT="Ống",
                    SoLuongYeuCau=2,
                    SoLuongThucXuat=2,
                    DonGia = (decimal)3700.00,
                    Nhom="Dung dịch tiêm truyền"
                },
            };
            */
            return new GridDataSource { Data = dataReturn, TotalRowCount = dataReturn.Length };

        }

        public async Task<List<LookupItemVo>> GetKhoWithoutKhoLe(LookupQueryInfo queryInfo)
        {
            var result = _khoRepository.TableNoTracking
                         .Where(p => p.LoaiKho != EnumLoaiKhoDuocPham.KhoLe && p.LoaiDuocPham ==true)
                         .Select(s => new LookupItemVo
                         {
                             KeyId = s.Id,
                             DisplayName = s.Ten
                         })
                         .ApplyLike(queryInfo.Query, o => o.DisplayName)
                         .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public virtual byte[] ExportBaoCaoDuocTinhHinhXuatNoiBo(GridDataSource gridDataSource, BaoCaoDuocTinhHinhXuatNoiBoQueryInfo query)
        {
            var datas = (ICollection<BaoCaoDuocTinhHinhXuatNoiBoGridVo>)gridDataSource.Data;
            var lstNhom = datas.GroupBy(s => s.Nhom).Select(s => s.First().Nhom).ToList();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO DƯỢC TÌNH HÌNH XUẤT NỘI BỘ");

                    //set row
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 40;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    worksheet.Row(3).Height = 20;
                    worksheet.Row(4).Height = 20;


                    using (var range = worksheet.Cells["A1:D1"])
                    {
                        range.Worksheet.Cells["A1:D1"].Merge = true;
                        range.Worksheet.Cells["A1:D1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:D1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:I4"])
                    {
                        range.Worksheet.Cells["A3:I4"].Merge = true;
                        range.Worksheet.Cells["A3:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:I4"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:I4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:I4"].Style.Font.Bold = true;
                        var date = "Thời gian từ: " + query.FromDate.ApplyFormatDate() + " - " + query.ToDate.ApplyFormatDate();
                        range.Worksheet.Cells["A3:I4"].Value = $"TÌNH HÌNH XUẤT NỘI BỘ{Environment.NewLine}{date}";
                        range.Worksheet.Cells["A3:I4"].Style.WrapText = true;

                    }

                    var tenKho = string.Empty;
                    if (query.KhoId == 0)
                    {
                        tenKho = "Tất cả";
                    }
                    else
                    {
                        tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();
                    }
                    using (var range = worksheet.Cells["A5:I5"])
                    {
                        range.Worksheet.Cells["A5:I5"].Merge = true;
                        range.Worksheet.Cells["A5:I5"].Value = "Kho xuất: " + tenKho;
                        range.Worksheet.Cells["A5:I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:I5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:I5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A7:I7"])
                    {
                        range.Worksheet.Cells["A7:I7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:I7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A7:I7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:I7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8:I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:I8"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A8:I8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:I8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7:A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7:A8"].Merge = true;
                        range.Worksheet.Cells["A7:A8"].Value = "STT";

                        range.Worksheet.Cells["B7:B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7:B8"].Merge = true;
                        range.Worksheet.Cells["B7:B8"].Value = "Mã dược";

                        range.Worksheet.Cells["C7:C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7:C8"].Merge = true;
                        range.Worksheet.Cells["C7:C8"].Value = "Tên dược, Vật tư, Hoá chất";

                        range.Worksheet.Cells["D7:D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7:D8"].Merge = true;
                        range.Worksheet.Cells["D7:D8"].Value = "ĐVT";

                        range.Worksheet.Cells["E7:F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7:F7"].Merge = true;
                        range.Worksheet.Cells["E7:F7"].Value = "Số lượng";

                        range.Worksheet.Cells["E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E8"].Merge = true;
                        range.Worksheet.Cells["E8"].Value = "Yêu cầu";

                        range.Worksheet.Cells["F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8"].Merge = true;
                        range.Worksheet.Cells["F8"].Value = "Thực xuất";

                        range.Worksheet.Cells["G7:G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7:G8"].Merge = true;
                        range.Worksheet.Cells["G7:G8"].Value = "Đơn giá";

                        range.Worksheet.Cells["H7:H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7:H8"].Merge = true;
                        range.Worksheet.Cells["H7:H8"].Value = "Thành tiền";

                        range.Worksheet.Cells["I7:I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7:I8"].Merge = true;
                        range.Worksheet.Cells["I7:I8"].Value = "Ghi chú";
                    }

                    int index = 9;
                    ///Đổ dât vào
                    ///
                    var stt = 1;
                    if (lstNhom.Any())
                    {
                        foreach (var nhom in lstNhom)
                        {
                            var lstDuocTheoNhom = datas.Where(s => s.Nhom == nhom).ToList();
                            if (lstDuocTheoNhom.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":I" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index].Value = nhom;
                                    range.Worksheet.Cells["A" + index].Style.Font.Italic = true;


                                    range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                                }
                                index++;

                                foreach (var duoc in lstDuocTheoNhom)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":I" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);

                                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        worksheet.Cells["A" + index].Value = stt;

                                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["B" + index].Value = duoc.Ma;

                                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["C" + index].Value = duoc.Ten;

                                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        worksheet.Cells["D" + index].Value = duoc.DVT;

                                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        worksheet.Cells["E" + index].Value = duoc.SoLuongYeuCau;

                                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        worksheet.Cells["F" + index].Value = duoc.SoLuongThucXuat;
                                        worksheet.Cells["F" + index].Style.Font.Bold = true;


                                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        worksheet.Cells["G" + index].Value = duoc.DonGia;
                                        worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";

                                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        worksheet.Cells["H" + index].Value = duoc.ThanhTien;
                                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";

                                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["I" + index].Value = duoc.GhiChu;
                                        index++;
                                        stt++;
                                    }

                                }
                            }

                        }
                    }

                    worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;


                    worksheet.Cells["B" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["B" + index + ":F" + index].Value = "Tổng cộng";
                    worksheet.Cells["B" + index + ":F" + index].Merge = true;
                    worksheet.Cells["B" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheet.Cells["G" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["G" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["G" + index + ":H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["G" + index + ":H" + index].Merge = true;
                    worksheet.Cells["G" + index + ":H" + index].Value = datas.Sum(p => p.ThanhTien);

                    index = index + 3;

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["G" + index + ":I" + index].Style.Font.Italic = true;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    index++;


                    worksheet.Cells["A" + index + ":I" + (index + 1)].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":I" + (index + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A" + index + ":I" + (index + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var row1 = "Trưởng khoa dược";
                    var row2 = "(Ký, ghi rõ họ tên)";
                    var boldRichText = worksheet.Cells["A" + index + ":C" + (index + 1)].RichText.Add(row1 + "\r\n");
                    boldRichText.Bold = true;
                    var italicRichText = worksheet.Cells["A" + index + ":C" + (index + 1)].RichText.Add(row2);
                    italicRichText.Bold = false;
                    italicRichText.Italic = true;

                    worksheet.Cells["A" + index + ":C" + (index + 1)].Style.WrapText = true;
                    worksheet.Cells["A" + index + ":C" + (index + 1)].Merge = true;


                    row1 = "Thủ kho";
                    boldRichText = worksheet.Cells["D" + index + ":F" + (index + 1)].RichText.Add(row1 + "\r\n");
                    boldRichText.Bold = true;

                    italicRichText = worksheet.Cells["D" + index + ":F" + (index + 1)].RichText.Add(row2);
                    italicRichText.Bold = false;
                    italicRichText.Italic = true;

                    worksheet.Cells["D" + index + ":F" + (index + 1)].Style.WrapText = true;
                    worksheet.Cells["D" + index + ":F" + (index + 1)].Merge = true;

                    row1 = "Người lập";
                    boldRichText = worksheet.Cells["G" + index + ":I" + (index + 1)].RichText.Add(row1 + "\r\n");
                    boldRichText.Bold = true;

                    italicRichText = worksheet.Cells["G" + index + ":I" + (index + 1)].RichText.Add(row2);
                    italicRichText.Bold = false;
                    italicRichText.Italic = true;

                    worksheet.Cells["G" + index + ":I" + (index + 1)].Style.WrapText = true;
                    worksheet.Cells["G" + index + ":I" + (index + 1)].Merge = true;



                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
