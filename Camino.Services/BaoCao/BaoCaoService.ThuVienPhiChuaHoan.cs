using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<List<LookupItemNhomThuVienPhiVo>> GetNhomThuVienPhiChuaHoanAsync(DropDownListRequestModel queryInfo)
        {
            var khoaCoNhapVienIds = await _noiTruKhoaPhongDieuTriRepository.TableNoTracking
                .Select(x => x.KhoaPhongChuyenDenId)
                .Distinct().ToArrayAsync();

            var nhomThuVienPhiChuaHoans = EnumHelper.GetListEnum<Enums.NhomThuVienPhiChuaHoan>()
                .Select(x => new LookupItemNhomThuVienPhiVo()
                {
                    Value = (long)x,
                    DisplayName = x.GetDescription()
                }).ToList();

            nhomThuVienPhiChuaHoans.AddRange(await _KhoaPhongRepository.TableNoTracking
                .Where(x => khoaCoNhapVienIds.Contains(x.Id))
                .ApplyLike(queryInfo.Query?.Trim(), x => x.Ma, x => x.Ten)
                .Select(item => new LookupItemNhomThuVienPhiVo()
                {
                    Value = item.Id,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DisplayName = item.Ten,
                    LaKhoaNhapVien = true
                })
                .Take(queryInfo.Take)
                .ToListAsync());

            if (nhomThuVienPhiChuaHoans.Any())
            {
                nhomThuVienPhiChuaHoans.Insert(0, new LookupItemNhomThuVienPhiVo()
                {
                    Value = 0,
                    DisplayName = "Toàn Viện"
                });
            }

            return nhomThuVienPhiChuaHoans;
        }

        public async Task<GridDataSource> GetDataBaoCaoThuVienPhiChuaHoanForGridAsync(QueryInfo queryInfo)
        {
            var lstChiPhi = new List<BaoCaoThuVienPhiChuaHoanGridVo>();
            var timKiemNangCaoObj = new BaoCaoThuVienPhiChuaHoanTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoThuVienPhiChuaHoanTimKiemVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.NhomThuVienPhi))
                {
                    var nhomObj = JsonConvert.DeserializeObject<KeyIdObjectNhomThuVienPhiVo>(timKiemNangCaoObj.NhomThuVienPhi);
                    if (nhomObj.Value != 0)
                    {
                        if (nhomObj.LaKhoaNhapVien)
                        {
                            timKiemNangCaoObj.KhoaPhongId = nhomObj.Value;
                        }
                        else
                        {
                            timKiemNangCaoObj.NhomThuVienPhiEnum = (Enums.NhomThuVienPhiChuaHoan)nhomObj.Value;
                        }
                    }
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                lstChiPhi = _taiKhoanBenhNhanThuRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, 
                        x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN, x => x.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn)
                    .Where(x => x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng 
                                && x.DaHuy != true 
                                && tuNgay <= x.NgayThu 
                                && denNgay >= x.NgayThu
                                && (timKiemNangCaoObj.KhoaPhongId == null 
                                    || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru 
                                        && x.YeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVienId == timKiemNangCaoObj.KhoaPhongId
                                        && x.ThuTienGoiDichVu != true))
                                && (timKiemNangCaoObj.NhomThuVienPhiEnum == null 
                                    || (timKiemNangCaoObj.NhomThuVienPhiEnum == Enums.NhomThuVienPhiChuaHoan.NgoaiTru 
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                        && x.ThuTienGoiDichVu != true)
                                    || (timKiemNangCaoObj.NhomThuVienPhiEnum == Enums.NhomThuVienPhiChuaHoan.GoiDichVu 
                                        && x.ThuTienGoiDichVu != null && x.ThuTienGoiDichVu == true))
                                )
                    .Select(item => new BaoCaoThuVienPhiChuaHoanGridVo()
                    {
                        Id = item.Id,
                        MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,
                        MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        BienLai = item.SoPhieuHienThi,
                        NgayThu = item.NgayThu,
                        SoTienTamUngTamTinh = item.TienMat.GetValueOrDefault() + item.ChuyenKhoan.GetValueOrDefault() + item.POS.GetValueOrDefault() + item.CongNo.GetValueOrDefault(),

                        PhieuHoanUngId = item.PhieuHoanUngId,
                        LaGoi = item.ThuTienGoiDichVu != null && item.ThuTienGoiDichVu == true,
                        LaNoiTru = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru,
                        TenKhoaNhapVien = item.YeuCauTiepNhan.YeuCauNhapVien.KhoaPhongNhapVien.Ten,

                        YeuCauGoiDichVuIds = item.TaiKhoanBenhNhanChis
                                                    .Where(x => x.DaHuy != true 
                                                                && x.YeuCauGoiDichVuId != null) 
                                                                //&& x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng)
                                                    .Select(x => x.YeuCauGoiDichVuId.Value).ToList()
                    })
                    .Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();


                #region // xử lý chi phí hoàn ứng gói
                var lstChiPhiGoi = lstChiPhi.Where(x => x.LaGoi && x.YeuCauGoiDichVuIds.Any()).ToList();
                if (lstChiPhiGoi.Any())
                {
                    var lstYeuCauGOiId = lstChiPhiGoi.SelectMany(x => x.YeuCauGoiDichVuIds).Distinct().ToList();

                    var lstChiTietChiPhiHoanTheoGoi = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        .Where(x => x.DaHuy != true
                                    && x.TaiKhoanBenhNhanThuId != null
                                    && x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                                    && x.YeuCauGoiDichVuId != null
                                    && lstYeuCauGOiId.Contains(x.YeuCauGoiDichVuId.Value))
                        .Select(item => new ChiTietHoanPhiTheoGoiVo()
                        {
                            YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                            TaiKhoanBenhNhanThuId = item.TaiKhoanBenhNhanThuId.Value,
                            TongTienPhieuThu = item.TaiKhoanBenhNhanThu.TienMat.GetValueOrDefault() + item.TaiKhoanBenhNhanThu.ChuyenKhoan.GetValueOrDefault() + item.TaiKhoanBenhNhanThu.POS.GetValueOrDefault()
                        })
                        .ToList();
                    var lstPhieuThuTheoGoi = lstChiTietChiPhiHoanTheoGoi
                        .GroupBy(x => x.TaiKhoanBenhNhanThuId)
                        .Select(item => new ChiPhiHoanPhiTheoPhieuThuVo()
                        {
                            TaiKhoanBenhNhanThuId = item.Key,
                            TongTienPhieuThu = item.First().TongTienPhieuThu,
                            YeuCauGoiDichVuIds = item.Select(chiTiet => chiTiet.YeuCauGoiDichVuId).ToList()
                        })
                        .OrderBy(x => x.TaiKhoanBenhNhanThuId)
                        .ToList();

                    var lstTongTienDaHoanTheoGoi = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        .Where(x => x.DaHuy != true
                                    && x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng
                                    && x.YeuCauGoiDichVuId != null
                                    && lstYeuCauGOiId.Contains(x.YeuCauGoiDichVuId.Value))
                        .Select(item => new ChiTietTongTienHoanPhiTheoGoiVo()
                        {
                            YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                            TongTienHoanPhi = item.TienChiPhi.GetValueOrDefault() + item.TienMat.GetValueOrDefault() + item.ChuyenKhoan.GetValueOrDefault()
                        })
                        .GroupBy(x => x.YeuCauGoiDichVuId)
                        .Select(item => new ChiTietTongTienHoanPhiTheoGoiVo()
                        {
                            YeuCauGoiDichVuId = item.Key,
                            TongTienHoanPhi = item.Sum(x => x.TongTienHoanPhi)
                        }).ToList();

                    foreach (var phieuThu in lstPhieuThuTheoGoi)
                    {
                        var lstGoiIdTheoPhieuThu = phieuThu.YeuCauGoiDichVuIds.Distinct().ToList();
                        var lstHoanPhiTheoGoiId = lstTongTienDaHoanTheoGoi.Where(x => lstGoiIdTheoPhieuThu.Contains(x.YeuCauGoiDichVuId)).ToList();

                        foreach (var goi in lstHoanPhiTheoGoiId)
                        {
                            if (phieuThu.TongTienDaHoan < phieuThu.TongTienPhieuThu)
                            {
                                if (goi.TongTienHoanPhi > 0)
                                {
                                    var soTienCoTheHoanTheoPhieuThu = phieuThu.TongTienPhieuThu - phieuThu.TongTienDaHoan;
                                    if (soTienCoTheHoanTheoPhieuThu <= goi.TongTienHoanPhi)
                                    {
                                        phieuThu.TongTienDaHoan = phieuThu.TongTienPhieuThu;
                                        goi.TongTienHoanPhi -= soTienCoTheHoanTheoPhieuThu;
                                    }
                                    else
                                    {
                                        phieuThu.TongTienDaHoan += goi.TongTienHoanPhi;
                                        goi.TongTienHoanPhi = 0;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        var phieuThuTimKiem = lstChiPhiGoi.FirstOrDefault(x => x.Id == phieuThu.TaiKhoanBenhNhanThuId);
                        if (phieuThuTimKiem != null)
                        {
                            phieuThuTimKiem.SoTienDaHoanTamTinh = phieuThu.TongTienDaHoan;
                        }
                    }

                }
                #endregion
            }

            return new GridDataSource
            {
                Data = lstChiPhi.ToArray(),
                TotalRowCount = lstChiPhi.Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageBaoCaoThuVienPhiChuaHoanForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoThuVienPhiChuaHoanTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoThuVienPhiChuaHoanTimKiemVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.NhomThuVienPhi))
                {
                    var nhomObj = JsonConvert.DeserializeObject<KeyIdObjectNhomThuVienPhiVo>(timKiemNangCaoObj.NhomThuVienPhi);
                    if (nhomObj.Value != 0)
                    {
                        if (nhomObj.LaKhoaNhapVien)
                        {
                            timKiemNangCaoObj.KhoaPhongId = nhomObj.Value;
                        }
                        else
                        {
                            timKiemNangCaoObj.NhomThuVienPhiEnum = (Enums.NhomThuVienPhiChuaHoan)nhomObj.Value;
                        }
                    }
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            if (tuNgay != null && denNgay != null)
            {
                var lstChiPhi = _taiKhoanBenhNhanThuRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN, x => x.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn)
                    .Where(x => x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng
                                && x.DaHuy != true
                                && tuNgay <= x.NgayThu
                                && denNgay >= x.NgayThu
                                && (timKiemNangCaoObj.KhoaPhongId == null
                                    || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                        && x.YeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVienId == timKiemNangCaoObj.KhoaPhongId
                                        && x.ThuTienGoiDichVu != true))
                                && (timKiemNangCaoObj.NhomThuVienPhiEnum == null
                                    || (timKiemNangCaoObj.NhomThuVienPhiEnum == Enums.NhomThuVienPhiChuaHoan.NgoaiTru
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                        && x.ThuTienGoiDichVu != true)
                                    || (timKiemNangCaoObj.NhomThuVienPhiEnum == Enums.NhomThuVienPhiChuaHoan.GoiDichVu
                                        && x.ThuTienGoiDichVu != null && x.ThuTienGoiDichVu == true))
                                )
                    .Select(item => new BaoCaoThuVienPhiChuaHoanGridVo()
                    {
                        Id = item.Id
                    });

                var countTask = lstChiPhi.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoThuVienPhiChuaHoan(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoThuVienPhiChuaHoanTimKiemVo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoThuVienPhiChuaHoanTimKiemVo>(query.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.NhomThuVienPhi))
                {
                    var nhomObj = JsonConvert.DeserializeObject<KeyIdObjectNhomThuVienPhiVo>(timKiemNangCaoObj.NhomThuVienPhi);
                    if (nhomObj.Value != 0)
                    {
                        if (nhomObj.LaKhoaNhapVien)
                        {
                            timKiemNangCaoObj.KhoaPhongId = nhomObj.Value;
                        }
                        else
                        {
                            timKiemNangCaoObj.NhomThuVienPhiEnum = (Enums.NhomThuVienPhiChuaHoan)nhomObj.Value;
                        }
                    }
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var datas = (ICollection<BaoCaoThuVienPhiChuaHoanGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("THU VIỆN PHÍ CHƯA HOÀN");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 25;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.DefaultColWidth = 15;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C2"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                        range.Worksheet.Cells["A2:C2"].Merge = true;
                        range.Worksheet.Cells["A2:C2"].Value = "Phòng Tài Chính Kế Toán";
                        range.Worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A2:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:C2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:C2"].Style.Font.UnderLine = true;
                    }

                    using (var range = worksheet.Cells["F1:H2"])
                    {
                        range.Worksheet.Cells["F1:H1"].Merge = true;
                        range.Worksheet.Cells["F1:H1"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                        range.Worksheet.Cells["F1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F1:H1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F1:H1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F1:H1"].Style.Font.Bold = true;

                        range.Worksheet.Cells["F2:H2"].Merge = true;
                        range.Worksheet.Cells["F2:H2"].Value = "Độc lập - Tự do - Hạnh phúc";
                        range.Worksheet.Cells["F2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F2:H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F2:H2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F2:H2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F2:H2"].Style.Font.UnderLine = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:H3"])
                    {
                        range.Worksheet.Cells["A3:H3"].Merge = true;
                        range.Worksheet.Cells["A3:H3"].Value = "DANH SÁCH VIỆN PHÍ NỘI TRÚ/NGOẠI TRÚ CHƯA HOÀN";
                        range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:H4"])
                    {
                        range.Worksheet.Cells["A4:H4"].Merge = true;
                        range.Worksheet.Cells["A4:H4"].Value = "Từ ngày " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                                          + " đến ngày " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:H4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:H7"])
                    {
                        range.Worksheet.Cells["A7:H7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:H7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:H7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:H7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Mã NB";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Mã TN";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Họ Tên";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Biên Lai";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Ngày Thu";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Số Tiền Tạm Ứng";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Số Tiền Đã Hoàn";
                    }

                    //write data from line 8
                    int index = 8;
                    int stt = 1;
                    var formatCurrency = "#,##0.00";
                    if (datas.Any())
                    {
                        var lstNhomThuVienPhi = datas.Select(x => x.TenNhomThuVienPhiChuaHoan).Distinct().ToList();
                        foreach (var nhomThuVienPhi in lstNhomThuVienPhi)
                        {
                            var lstDataTheoNhom = datas
                                .Where(x => x.TenNhomThuVienPhiChuaHoan == nhomThuVienPhi).ToList();

                            worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                            worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = nhomThuVienPhi;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = lstDataTheoNhom.Sum(x => x.SoTienTamUng);
                            worksheet.Cells["G" + index].Style.Numberformat.Format = formatCurrency;
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = lstDataTheoNhom.Sum(x => x.SoTienDaHoan);
                            worksheet.Cells["H" + index].Style.Numberformat.Format = formatCurrency;
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            index++;

                            foreach (var item in lstDataTheoNhom)
                            {
                                // format border, font chữ,....
                                worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Row(index).Height = 20.5;

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaBN;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaYeuCauTiepNhan;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.HoTen;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.BienLai;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NgayThuDisplay;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.SoTienTamUng;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.SoTienDaHoan;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                stt++;
                                index++;
                            }
                        }

                        //total
                        worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(index).Height = 20.5;

                        using (var range = worksheet.Cells["A" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":F" + index].Value = "Tổng Cộng";
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                        }


                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["G" + index].Style.Font.Bold = true;
                        worksheet.Cells["G" + index].Value = datas.Sum(x => x.SoTienTamUng);
                        worksheet.Cells["G" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["H" + index].Style.Font.Bold = true;
                        worksheet.Cells["H" + index].Value = datas.Sum(x => x.SoTienDaHoan);
                        worksheet.Cells["H" + index].Style.Numberformat.Format = formatCurrency;
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
