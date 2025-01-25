using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;
using static Camino.Core.Domain.Enums;
using Camino.Data;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.BaoCaoTheKhoVatTus;
using Camino.Core.Domain.ValueObject.BaoCaoTheKhos;
using Newtonsoft.Json;

namespace Camino.Services.BaoCaoVatTus
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoVatTuService))]
    public class BaoCaoVatTuService : MasterFileService<NhapKhoVatTu>, IBaoCaoVatTuService
    {
        public IRepository<Kho> _khoRepository;
        public IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        public IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        public IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> _vatTuBenhVienRepository;
        IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;

        public BaoCaoVatTuService(IRepository<NhapKhoVatTu> repository, IRepository<Kho> khoRepository, IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository, IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository, IRepository<XuatKhoVatTu> xuatKhoVatTuRepository,
            IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> vatTuBenhVienRepository) : base(repository)
        {
            _khoRepository = khoRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
        }

        #region Báo cáo vật tư tồn kho 

        public async Task<List<LookupItemVo>> GetKhoChoBaoCaoVatTu(LookupQueryInfo queryInfo)
        {
            var lst = new List<LookupItemVo>();

            var khoVatTus = lst.Union(_khoRepository.TableNoTracking
                   .Where(p => (p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || p.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK
                          || p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                          || p.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) && p.LoaiVatTu == true)
                          .Select(item => new LookupItemVo
                          {
                              DisplayName = item.Ten,
                              KeyId = Convert.ToInt32(item.Id),
                          })
                          .ApplyLike(queryInfo.Query, g => g.DisplayName)
                          .Take(queryInfo.Take))
                          .ToList();
            return khoVatTus;
        }

        public async Task<GridDataSource> GetDataBaoCaoTonKhoVatTuForGridAsync(BaoCaoTonKhoVatTuQueryInfo queryInfo)
        {
            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate)
                    .Select(o => new BaoCaoChiTietTonKhoVatTuGridVo
                    {
                        Id = o.Id,
                        VatTuBenhVienId = o.VatTuBenhVienId,
                        Ma = o.VatTuBenhVien.Ma,
                        Ten = o.VatTuBenhVien.VatTus.Ten,
                        NhaSanXuat = o.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = o.VatTuBenhVien.VatTus.NuocSanXuat,
                        DVT = o.VatTuBenhVien.VatTus.DonViTinh,
                        NgayNhapXuat = o.NgayNhap,
                        LaVatTuBHYT = o.LaVatTuBHYT,
                        LoaiSuDung = o.VatTuBenhVien.LoaiSuDung,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            var allDataXuat = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)))
                .Select(o => new BaoCaoChiTietTonKhoVatTuGridVo
                {
                    Id = o.Id,
                    VatTuBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ma = o.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    Ten = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    NhaSanXuat = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhaSanXuat,
                    NuocSanXuat = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NuocSanXuat,
                    DVT = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                    LaVatTuBHYT = o.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    LoaiSuDung = o.NhapKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.Ten).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.LaVatTuBHYT, o.VatTuBenhVienId });
            var dataReturn = new List<BaoCaoTonKhoVatTuGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var noiSanXuat = "";
                if (!string.IsNullOrEmpty(xuatNhapDuocPham.First().NhaSanXuat) && !string.IsNullOrEmpty(xuatNhapDuocPham.First().NuocSanXuat))
                {
                    noiSanXuat = $" ({xuatNhapDuocPham.First().NhaSanXuat}, {xuatNhapDuocPham.First().NuocSanXuat})";
                }
                else if (!string.IsNullOrEmpty(xuatNhapDuocPham.First().NhaSanXuat) || !string.IsNullOrEmpty(xuatNhapDuocPham.First().NuocSanXuat))
                {
                    noiSanXuat = $" ({xuatNhapDuocPham.First().NhaSanXuat}{xuatNhapDuocPham.First().NuocSanXuat})";
                }

                var tenDayDu = $"{xuatNhapDuocPham.First().Ten}{noiSanXuat}";
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= queryInfo.FromDate).ToList();
                var baoCaoTonKhoGridVo = new BaoCaoTonKhoVatTuGridVo
                {
                    Ma = xuatNhapDuocPham.First().Ma,
                    Ten = tenDayDu,
                    DVT = xuatNhapDuocPham.First().DVT,
                    TonDau = tonDau,
                    Nhap = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Xuat = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Loai = xuatNhapDuocPham.Key.LaVatTuBHYT ? "Vật tư BHYT" : "Viện phí",
                    Nhom = xuatNhapDuocPham.First().Nhom
                };
                if (baoCaoTonKhoGridVo.TonCuoi != null && !baoCaoTonKhoGridVo.TonCuoi.Value.AlmostEqual(0))
                {
                    dataReturn.Add(baoCaoTonKhoGridVo);
                }
            }
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.Loai).ThenBy(s => s.Nhom).ToArray(), TotalRowCount = dataReturn.Count };
        }

        public virtual byte[] ExportBaoCaoTonKhoVatTu(GridDataSource gridDataSource, BaoCaoTonKhoVatTuQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTonKhoVatTuGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTonKhoVatTuGridVo>("STT", p => ind++)
            };
            var lstLoai = datas.GroupBy(x => new { x.Loai })
                .Select(item => new LoaiGroupVo
                {
                    Loai = item.First().Loai

                }).OrderBy(p => p.Loai).ToList();
            var lstNhom = datas.GroupBy(x => new { x.Loai, x.Nhom })
               .Select(item => new NhomGroupVo
               {
                   Loai = item.First().Loai,
                   Nhom = item.First().Nhom

               }).OrderBy(p => p.Nhom).ToList();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[VTYT] Báo cáo tồn kho");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 7;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 40;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    using (var range = worksheet.Cells["A1:I1"])
                    {
                        range.Worksheet.Cells["A1:G1"].Merge = true;
                        range.Worksheet.Cells["A1:G1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                    }

                    var tenKho = query.KhoId == 0 ? "Tất cả" : _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();

                    using (var range = worksheet.Cells["A2:I2"])
                    {
                        range.Worksheet.Cells["A2:I2"].Merge = true;
                        range.Worksheet.Cells["A2:I2"].Value = tenKho;
                        range.Worksheet.Cells["A2:I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:I2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:I2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:I2"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:I3"])
                    {
                        range.Worksheet.Cells["A3:I3"].Merge = true;
                        range.Worksheet.Cells["A3:I3"].Value = "BÁO CÁO TỒN KHO";
                        range.Worksheet.Cells["A3:I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:I3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:I3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:I4"])
                    {
                        range.Worksheet.Cells["A4:I4"].Merge = true;
                        range.Worksheet.Cells["A4:I4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:I4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:I4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:I4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:I7"])
                    {
                        range.Worksheet.Cells["A7:I7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:I7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:I7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:I7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:I7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7"].Value = "STT";
                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Mã VTYT";
                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Tên VTYT (Hãng SX, Nước SX)";
                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "ĐVT";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Tồn đầu";
                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Nhập";
                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Tổng số";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Xuất";
                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Tồn cuối";
                    }

                    var manager = new PropertyManager<BaoCaoTonKhoVatTuGridVo>(requestProperties);
                    int index = 8; // bắt đầu đổ data từ dòng 13

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;
                    if (lstLoai.Any())
                    {
                        foreach (var loai in lstLoai)
                        {
                            using (var range = worksheet.Cells["A" + index + ":I" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Red);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;

                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Value = loai.Loai;
                                range.Worksheet.Cells["A" + index + ":I" + index].Merge = true;
                            }
                            index++;


                            var listNhomTheoLoai = lstNhom.Where(o => o.Loai == loai.Loai).ToList();
                            if (listNhomTheoLoai.Any())
                            {
                                foreach (var nhom in listNhomTheoLoai)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":I" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Italic = true;


                                        range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":I" + index].Value = nhom.Nhom;
                                        range.Worksheet.Cells["A" + index + ":I" + index].Merge = true;
                                    }
                                    index++;

                                    var listDuocPhamTheoNhom = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();
                                    if (listDuocPhamTheoNhom.Any())
                                    {
                                        foreach (var duocPham in listDuocPhamTheoNhom)
                                        {
                                            manager.CurrentObject = duocPham;

                                            worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                            worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);

                                            worksheet.Row(index).Height = 20.5;
                                            manager.WriteToXlsx(worksheet, index);
                                            // Đổ data
                                            worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                            worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);

                                            worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                            worksheet.Cells["A" + index].Value = stt;
                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Cells["B" + index].Value = duocPham.Ma;
                                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Cells["C" + index].Value = duocPham.Ten;
                                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Cells["D" + index].Value = duocPham.DVT;
                                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                                            worksheet.Cells["E" + index].Value = duocPham.TonDau;
                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                            worksheet.Cells["F" + index].Value = duocPham.Nhap;
                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                            worksheet.Cells["G" + index].Value = duocPham.TongSo;
                                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Cells["H" + index].Value = duocPham.Xuat;
                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                            worksheet.Cells["I" + index].Value = duocPham.TonCuoi;
                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                            index++;
                                            stt++;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":I" + index].Style.Font.Italic = true;
                    //value
                    var now = DateTime.Now;
                    worksheet.Cells["G" + index + ":I" + index].Value = "Ngày " + now.Day + " tháng " + now.Month + " năm " + now.Year;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    index++;


                    worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["A" + index + ":B" + index].Value = "Trưởng khoa";
                    worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":B" + index].Merge = true;

                    worksheet.Cells["C" + index + ":E" + index].Value = "Thủ kho";
                    worksheet.Cells["C" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["C" + index + ":F" + index].Merge = true;

                    worksheet.Cells["G" + index + ":I" + index].Value = "Người lập";
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    index++;

                    worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":I" + index].Style.Font.Italic = true;
                    //value
                    worksheet.Cells["A" + index + ":B" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":B" + index].Merge = true;

                    worksheet.Cells["C" + index + ":F" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["C" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["C" + index + ":F" + index].Merge = true;

                    worksheet.Cells["G" + index + ":I" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    index++;


                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        #endregion


        #region Báo cáo thẻ kho vật tư

        public async Task<List<DuocPhamTheoKhoBaoCaoLookup>> GetKhoDuocPhamVatTuTheoKhoHangHoa(DropDownListRequestModel queryInfo, long khoId)
        {
            var query =
                _vatTuBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoVatTuChiTiets.Any(kho => kho.NhapKhoVatTu.KhoId == khoId))
                    .ApplyLike(queryInfo.Query, g => g.VatTus.Ten)
                    .Select(s => new DuocPhamTheoKhoBaoCaoLookup
                    {
                        KeyId = s.Id,
                        DisplayName = s.VatTus.Ten,
                        LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                        DuocPhamHoacVatTuBenhVienId = s.Id,
                        Ten = s.VatTus.Ten,
                        Ma = s.Ma
                    }).OrderBy(o => o.DisplayName).Take(queryInfo.Take);

            return query.ToList();
        }

        private ICollection<BaoCaoTheKhoVatTuGridVo> GetDataBaoCaoTheKhoVatTu(BaoCaoTheKhoQueryInfo queryInfo)
        {
            var fromDate = queryInfo.startDate.Date;
            var toDate = queryInfo.endDate.Date.AddDays(1).AddMilliseconds(-1);
            if (queryInfo.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien)
            {
                IQueryable<NhapKhoVatTuChiTiet> allDataNhapQuery = null;
                if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoVatTuYTe)
                {
                    allDataNhapQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                        .Where(o =>
                                            o.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                            o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= toDate);
                }
                else
                {
                    allDataNhapQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                        .Where(o =>
                                            o.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId && o.KhoNhapSauKhiDuyetId == null &&
                                            o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= toDate);                    
                }

                var allDataNhap = allDataNhapQuery
                    .Select(o => new BaoCaoTheKhoVatTuChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.NhapKhoVatTu.Kho.Ten,
                        NhapTuKho = o.NhapKhoVatTu.XuatKhoVatTuId != null ? o.NhapKhoVatTu.XuatKhoVatTu.KhoVatTuXuat.Ten : "",
                        SoHoaDon = o.NhapKhoVatTu.XuatKhoVatTuId == null ? o.NhapKhoVatTu.SoChungTu : "",
                        TenNhaThau = o.HopDongThauVatTu.NhaThau.Ten,
                        HopDongThauId = o.HopDongThauVatTuId,
                        SoHopDongThau = o.HopDongThauVatTu.SoHopDong,
                        //NhapTuNhaCungCap = o.HopDongThauVatTu.HeThongTuPhatSinh != null && o.HopDongThauVatTu.HeThongTuPhatSinh == true,
                        NgayThang = o.NgayNhap,
                        SCTNhap = o.NhapKhoVatTu.SoPhieu,
                        NhapTuXuatKhoId = o.NhapKhoVatTu.XuatKhoVatTuId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

                var nhapTuXuatKhoIds = allDataNhap.Where(o => o.NhapTuXuatKhoId != null && o.NgayThang >= fromDate).Select(o => o.NhapTuXuatKhoId.Value).Distinct().ToList();
                var xuatKhoSauKhiDuyetIds = _xuatKhoVatTuRepository.TableNoTracking
                .Where(o => nhapTuXuatKhoIds.Contains(o.Id) && o.KhoXuatId == (long)EnumKhoDuocPham.KhoVatTuYTe && o.LyDoXuatKho == Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                .Select(o => o.Id).ToList();

                foreach (var baoCaoTheKhoChiTietData in allDataNhap)
                {
                    if (baoCaoTheKhoChiTietData.NhapTuXuatKhoId == null || xuatKhoSauKhiDuyetIds.Contains(baoCaoTheKhoChiTietData.NhapTuXuatKhoId.Value))
                    {
                        baoCaoTheKhoChiTietData.NhapTuNhaCungCap = true;
                    }
                }

                var groupDataNhap = allDataNhap.GroupBy(o => new
                {
                    SCTNhap = o.SCTNhap,
                    TenKho = o.TenKho,
                    NhapTuKho = o.NhapTuKho,
                    TenNhaThau = o.TenNhaThau,
                    HopDongThauId = o.HopDongThauId,
                    SoHopDongThau = o.SoHopDongThau,
                    NhapTuNhaCungCap = o.NhapTuNhaCungCap,
                    NgayThang = o.NgayThang,
                }, o => o,
                    (k, v) => new BaoCaoTheKhoVatTuChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        NhapTuXuatKhoId = v.First().NhapTuXuatKhoId,
                        SoHoaDon = v.First().SoHoaDon,
                        TenKho = k.TenKho,
                        NhapTuKho = k.NhapTuKho,
                        TenNhaThau = k.TenNhaThau,
                        HopDongThauId = k.HopDongThauId,
                        SoHopDongThau = k.SoHopDongThau,
                        NhapTuNhaCungCap = k.NhapTuNhaCungCap,
                        NgayThang = k.NgayThang,
                        SLNhap = v.Sum(x => x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = 0
                    }).ToList();
                //var nhapTuXuatKhoIds = groupDataNhap.Where(o => o.NhapTuXuatKhoId != null && o.NgayThang >= fromDate).Select(o => o.NhapTuXuatKhoId.Value).ToList();
                var thongTinXuatKhos = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                nhapTuXuatKhoIds.Contains(o.XuatKhoVatTuChiTiet.XuatKhoVatTuId.Value))
                    .Select(o => new BaoCaoTheKhoThongTinXuatKhoDeNhapVe
                    {
                        XuatKhoId = o.XuatKhoVatTuChiTiet.XuatKhoVatTuId.Value,
                        KhoXuatId = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId,
                        TenKhoXuat = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.Ten,
                        DuocPhamBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                        HopDongThauId = o.NhapKhoVatTuChiTiet.HopDongThauVatTuId,
                        SoHoaDon = o.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoChungTu,
                        KhoKhacHoanTra = o.YeuCauTraVatTuChiTiets.Any()
                    }).ToList();

                IQueryable<XuatKhoVatTuChiTietViTri> allDataXuatQuery = null;
                if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoVatTuYTe)
                {
                    allDataXuatQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                                        .Where(o => o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                                    o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                                    o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId
                                                    && ((o.NgayXuat != null && o.NgayXuat <= toDate) ||
                                                        (o.NgayXuat == null &&
                                                         o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= toDate)));
                }
                else
                {
                    allDataXuatQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                                        .Where(o => o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                                    o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                                    o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet
                                                    && ((o.NgayXuat != null && o.NgayXuat <= toDate) ||
                                                        (o.NgayXuat == null &&
                                                         o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= toDate)));                    
                }

                var allDataXuat = allDataXuatQuery
                    .Select(o => new BaoCaoTheKhoVatTuChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.Ten,
                        LoaiXuatKhoVatTu = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LoaiXuatKho,
                        XuatQuaKho = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoNhapId != null
                            ? o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuNhap.Ten
                            : "",
                        NgayThang = o.NgayXuat != null
                            ? o.NgayXuat.Value
                            : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                        SCTNhap = o.SoLuongXuat < 0 ? o.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : string.Empty,
                        SCTXuat = o.SoLuongXuat > 0 ? o.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : string.Empty,
                        SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                        SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                        //HoTenBenhNhan = o.DonVTYTThanhToanChiTiets.Select(c => c.DonVTYTThanhToan.YeuCauTiepNhan.HoTen).FirstOrDefault(),
                        //MaTiepNhan = o.DonVTYTThanhToanChiTiets.Select(c => c.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan).FirstOrDefault(),
                        BenhNhanTraLai = o.SoLuongXuat < 0
                    }).ToList();

                var groupDataXuat = allDataXuat.GroupBy(o => new
                {
                    SCTNhap = o.SCTNhap,
                    SCTXuat = o.SCTXuat,
                    TenKho = o.TenKho,
                    LoaiXuatKhoVatTu = o.LoaiXuatKhoVatTu,
                    XuatQuaKho = o.XuatQuaKho,
                    NgayThang = o.NgayThang,
                    BenhNhanTraLai = o.BenhNhanTraLai,
                }, o => o,
                    (k, v) => new BaoCaoTheKhoVatTuChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        SCTXuat = k.SCTXuat,
                        TenKho = k.TenKho,
                        LoaiXuatKhoVatTu = k.LoaiXuatKhoVatTu,
                        XuatQuaKho = k.XuatQuaKho,
                        NgayThang = k.NgayThang,
                        SLNhap = v.Sum(x => x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = v.Sum(x => x.SLXuat.GetValueOrDefault()).MathRoundNumber(2),
                        HoTenBenhNhan = v.First().HoTenBenhNhan,
                        MaTiepNhan = v.First().MaTiepNhan,
                        BenhNhanTraLai = k.BenhNhanTraLai
                    }).ToList();

                var allDataNhapXuat = groupDataNhap.Concat(groupDataXuat).OrderBy(o => o.NgayThang).ToList();
                var tonDauKy = allDataNhapXuat.Where(o => o.NgayThang < fromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.NgayThang >= fromDate).ToList();
                for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
                {
                    if (i == 0)
                    {
                        allDataNhapXuatTuNgay[i].SLTon = (tonDauKy + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }
                    else
                    {
                        allDataNhapXuatTuNgay[i].SLTon = (allDataNhapXuatTuNgay[i - 1].SLTon + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }

                    if (allDataNhapXuatTuNgay[i].NhapTuXuatKhoId != null)
                    {
                        var thongTinXuatKho = thongTinXuatKhos.FirstOrDefault(o =>
                            o.XuatKhoId == allDataNhapXuatTuNgay[i].NhapTuXuatKhoId &&
                            o.HopDongThauId == allDataNhapXuatTuNgay[i].HopDongThauId);
                        if (thongTinXuatKho != null)
                        {
                            allDataNhapXuatTuNgay[i].KhoKhacHoanTra = thongTinXuatKho.KhoKhacHoanTra;
                            allDataNhapXuatTuNgay[i].SoHoaDon = thongTinXuatKho.SoHoaDon;
                        }
                    }
                }
                var baoCaoTheKhoGridVos = allDataNhapXuatTuNgay.GroupBy(o => o.NgayThangDisplay).Select(o => new BaoCaoTheKhoVatTuGridVo
                {
                    TongSLNhap = o.Sum(i => i.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                    TongSLXuat = o.Sum(i => i.SLXuat.GetValueOrDefault()).MathRoundNumber(2),
                    NgayThang = o.First().NgayThang.Date,
                    BaoCaoTheKhoVatTuChiTiets = o.ToList()
                }).ToArray();
                for (int i = 0; i < baoCaoTheKhoGridVos.Length; i++)
                {
                    if (i == 0)
                    {
                        baoCaoTheKhoGridVos[i].TongSLTonDauKy = tonDauKy;
                        baoCaoTheKhoGridVos[i].TongSLTon = (baoCaoTheKhoGridVos[i].TongSLTonDauKy + baoCaoTheKhoGridVos[i].TongSLNhap.GetValueOrDefault() - baoCaoTheKhoGridVos[i].TongSLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }
                    else
                    {
                        baoCaoTheKhoGridVos[i].TongSLTonDauKy = baoCaoTheKhoGridVos[i - 1].TongSLTon;
                        baoCaoTheKhoGridVos[i].TongSLTon = (baoCaoTheKhoGridVos[i].TongSLTonDauKy + baoCaoTheKhoGridVos[i].TongSLNhap.GetValueOrDefault() - baoCaoTheKhoGridVos[i].TongSLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }
                }
                return baoCaoTheKhoGridVos.ToArray();
            }
            return null;
            /*
            var fromDate = queryInfo.startDate.Date;
            var toDate = queryInfo.endDate.Date.AddDays(1).AddMilliseconds(-1);

            var allDataNhap1 = new List<BaoCaoTheKhoVatTuChiTietGridVo>()
            {
                 new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn A" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang =DateTime.Now.AddMonths(-3) , NhapTuKho = "Kho A" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                },
                 new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn B" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang =DateTime.Now.AddMonths(-3) , NhapTuKho = "Kho B" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                },
                  new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn C" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang = DateTime.Now.AddMonths(-3) , NhapTuKho = "Kho C" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                }
            };

            var allDataNhap2 = new List<BaoCaoTheKhoVatTuChiTietGridVo>()
            {
                 new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn A" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang =DateTime.Now.AddMonths(-2) , NhapTuKho = "Kho A" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                },
                 new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn B" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang =DateTime.Now.AddMonths(-2) , NhapTuKho = "Kho B" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                },
                  new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn C" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang = DateTime.Now.AddMonths(-2) , NhapTuKho = "Kho C" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                }
            };

            var allDataNhap3 = new List<BaoCaoTheKhoVatTuChiTietGridVo>()
            {
                 new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn A" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang =DateTime.Now , NhapTuKho = "Kho A" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                },
                 new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn B" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang =DateTime.Now , NhapTuKho = "Kho B" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                },
                  new BaoCaoTheKhoVatTuChiTietGridVo()
                {
                    Id = 1 , KhoId = 1 ,BenhNhanTraLai = true ,VatTuBenhVienId = 1 ,DonGia= 100000 ,
                    HopDongThauId = 1 ,HoTenBenhNhan = "Nguyễn văn C" , DVT = "Bộ" ,KhoKhacHoanTra = true,
                    NgayThang = DateTime.Now , NhapTuKho = "Kho C" ,SLNhap = 100 ,SLTon = 100000 ,SLXuat = 10000 ,SLTonDauKy = 10000,
                    SCTNhap= "SCTN01",SCTXuat = "SCTX02" ,NgayThangCT =  DateTime.Now.AddDays(-1)
                }
            };

            var thongTinXuatKhos = new BaoCaoTheKhoVatTuThongTinXuatKhoDeNhapVe();

            var baoCaoTheKhoGridVos = new List<BaoCaoTheKhoVatTuGridVo>
            {
               new  BaoCaoTheKhoVatTuGridVo { Id = 1 , VatTuBenhVienId = 1 , KhoId= 1 , NgayThang= DateTime.Now.AddMonths(-3) ,TongSLNhap = 10000 ,TongSLTonDauKy = 10000 ,TongSLTon = 120000 ,TongSLXuat = 10000 ,BaoCaoTheKhoVatTuChiTiets = allDataNhap1},
               new  BaoCaoTheKhoVatTuGridVo { Id = 2 , VatTuBenhVienId = 2 , KhoId= 3 , NgayThang= DateTime.Now.AddMonths(-2) ,TongSLNhap = 104000 ,TongSLTonDauKy = 101000 ,TongSLTon = 130000 ,TongSLXuat = 101000 ,BaoCaoTheKhoVatTuChiTiets = allDataNhap2},
               new  BaoCaoTheKhoVatTuGridVo { Id = 3 , VatTuBenhVienId = 2 , KhoId= 2 , NgayThang= DateTime.Now ,TongSLNhap = 109000 ,TongSLTonDauKy = 102000 ,TongSLTon = 120000 ,TongSLXuat = 101000 ,BaoCaoTheKhoVatTuChiTiets = allDataNhap3}
            };

            return baoCaoTheKhoGridVos.ToArray();*/
        }

        public async Task<GridDataSource> GetDataTheKhoVatTuForGridAsync(BaoCaoTheKhoQueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var baoCaoTheKhoGridVos = GetDataBaoCaoTheKhoVatTu(queryInfo);
            if (baoCaoTheKhoGridVos != null)
                return new GridDataSource { Data = baoCaoTheKhoGridVos.ToArray(), TotalRowCount = baoCaoTheKhoGridVos.Count() };
            else
                return null;
        }

        public async Task<GridDataSource> GetDataTheKhoVatTuForGridChildAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            var timKiemNangCaoObj = new BaoCaoTheKhoAdditionalSearchString();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTheKhoAdditionalSearchString>(queryInfo.AdditionalSearchString);
            }

            var baoCaoTheKhoQueryInfo = new BaoCaoTheKhoQueryInfo
            {
                KhoId = timKiemNangCaoObj.KhoId,
                DuocPhamHoacVatTuBenhVienId = timKiemNangCaoObj.DuocPhamHoacVatTuBenhVienId,
                LoaiDuocPhamHoacVatTu = timKiemNangCaoObj.LoaiDuocPhamHoacVatTu,
                startDate = timKiemNangCaoObj.NgayThang,
                endDate = timKiemNangCaoObj.NgayThang,
            };

            var baoCaoTheKhoGridVos = GetDataBaoCaoTheKhoVatTu(baoCaoTheKhoQueryInfo);

            if (baoCaoTheKhoGridVos != null && baoCaoTheKhoGridVos.Any())
                return new GridDataSource { Data = baoCaoTheKhoGridVos.First().BaoCaoTheKhoVatTuChiTiets.ToArray(), TotalRowCount = baoCaoTheKhoGridVos.First().BaoCaoTheKhoVatTuChiTiets.Count() };
            else
                return null;
        }

        public virtual byte[] ExportTheKhoVatTu(BaoCaoTheKhoQueryInfo queryInfo)
        {
            ICollection<BaoCaoTheKhoVatTuGridVo> dataBaoCaoTheKho = GetDataBaoCaoTheKhoVatTu(queryInfo);
            var vatTu = _vatTuBenhVienRepository.TableNoTracking.Where(p => p.Id == queryInfo.DuocPhamHoacVatTuBenhVienId).Select(p => new
            {
                TenVatTu = p.VatTus.Ten,
                p.Ma,
                DVT = p.VatTus.DonViTinh
            }).First();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[VTYT] Báo cáo thẻ kho");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 60;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;

                    worksheet.DefaultColWidth = 7;

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    var tenKho = queryInfo.KhoId == 0 ? "Tất cả" : _khoRepository.TableNoTracking.Where(p => p.Id == queryInfo.KhoId).Select(p => p.Ten).FirstOrDefault();

                    using (var range = worksheet.Cells["A2:C2"])
                    {
                        range.Worksheet.Cells["A2:C2"].Merge = true;
                        range.Worksheet.Cells["A2:C2"].Value = tenKho;
                        range.Worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:C2"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["H1:I1"])
                    {
                        range.Worksheet.Cells["H1:I1"].Merge = true;
                        range.Worksheet.Cells["H1:I1"].Value = "MS: 16D/BV - 01";
                        range.Worksheet.Cells["H1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["H1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["H1:I1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["H1:I1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["H2:I2"])
                    {
                        range.Worksheet.Cells["H2:I2"].Merge = true;
                        range.Worksheet.Cells["H2:I2"].Value = "Số:........................";
                        range.Worksheet.Cells["H2:I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["H2:I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H2:I2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["H2:I2"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A3:H3"])
                    {
                        range.Worksheet.Cells["A3:H3"].Merge = true;
                        range.Worksheet.Cells["A3:H3"].Value = "BÁO CÁO THẺ KHO";
                        range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:H4"])
                    {
                        range.Worksheet.Cells["A4:H4"].Merge = true;
                        range.Worksheet.Cells["A4:H4"].Value = "Thời gian từ: " + queryInfo.startDate.ApplyFormatDate()
                                                          + " -  " + queryInfo.endDate.ApplyFormatDate();
                        range.Worksheet.Cells["A4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:H4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }


                    using (var range = worksheet.Cells["A7:H7"])
                    {
                        range.Worksheet.Cells["A7:H7"].Merge = true;
                        range.Worksheet.Cells["A7:H7"].Value = "Tên thuốc, hóa chất, vật tư y tế tiêu hao: " + vatTu.Ma + " - " + vatTu.TenVatTu;
                        range.Worksheet.Cells["A7:H7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A7:H7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:H7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:H8"])
                    {
                        range.Worksheet.Cells["A8:H8"].Merge = true;
                        range.Worksheet.Cells["A8:H8"].Value = "Đơn vị: " + vatTu.DVT;
                        range.Worksheet.Cells["A8:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A8:H8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:H8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:I10"])
                    {
                        range.Worksheet.Cells["A10:I10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A10:I10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:I10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:I10"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A10:I10"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A10:I10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        //range.Worksheet.Cells["A10:A11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["A10:A11"].Merge = true;
                        //range.Worksheet.Cells["A10:A11"].Value = "STT";
                        //range.Worksheet.Cells["A10:A11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        //range.Worksheet.Cells["A10:A11"].Style.Font.Bold = true;


                        range.Worksheet.Cells["A10:B11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A10:B11"].Merge = true;
                        range.Worksheet.Cells["A10:B11"].Value = "Ngày tháng";
                        range.Worksheet.Cells["A10:B11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:B11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["C10:D10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C10:D10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C10:D10"].Merge = true;
                        range.Worksheet.Cells["C10:D10"].Value = "Số chứng từ";
                        range.Worksheet.Cells["C10:D10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C10:D10"].Style.Font.Bold = true;

                        range.Worksheet.Cells["C11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C11"].Value = "Nhập";
                        range.Worksheet.Cells["C11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["D11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D11"].Value = "Xuất";
                        range.Worksheet.Cells["D11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["E10:E11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E10:E11"].Merge = true;
                        range.Worksheet.Cells["E10:E11"].Value = "Diễn giải";
                        range.Worksheet.Cells["E10:E11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E10:E11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["F10:F11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F10:F11"].Merge = true;
                        range.Worksheet.Cells["F10:F11"].Value = "SL Tồn Đầu Kỳ";
                        range.Worksheet.Cells["F10:F11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F10:F11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["G10:I10"].Merge = true;
                        range.Worksheet.Cells["G10:I10"].Value = "Số lượng";
                        range.Worksheet.Cells["G10:I10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G10:I10"].Style.Font.Bold = true;

                        range.Worksheet.Cells["G11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G11"].Value = "Nhập";
                        range.Worksheet.Cells["G11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["H11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H11"].Value = "Xuất";
                        range.Worksheet.Cells["H11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["H11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["I11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I11"].Value = "Tồn";
                        range.Worksheet.Cells["I11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["I11"].Style.Font.Bold = true;

                    }
                    int ind = 1;
                    var requestProperties = new[]
                    {
                        new PropertyByName<BaoCaoTheKhoVatTuGridVo>("STT", p => ind++)
                    };

                    var manager = new PropertyManager<BaoCaoTheKhoVatTuGridVo>(requestProperties);
                    var index = 12;

                    foreach (var data in dataBaoCaoTheKho)
                    {
                        manager.CurrentObject = data;
                        using (var range = worksheet.Cells["A" + index + ":I" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                        }

                        worksheet.Row(index).Height = 20.5;

                        using (var range = worksheet.Cells["A" + index + ":E" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":E" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":E" + index].Value = data.NgayThangDisplay;
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        using (var range = worksheet.Cells["F" + index])
                        {
                            range.Worksheet.Cells["F" + index].Value = data.TongSLTonDauKy;
                            range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }

                        using (var range = worksheet.Cells["G" + index])
                        {
                            range.Worksheet.Cells["G" + index].Value = data.TongSLNhap;
                            range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        }

                        using (var range = worksheet.Cells["H" + index])
                        {
                            range.Worksheet.Cells["H" + index].Value = data.TongSLXuat;
                            range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        }

                        using (var range = worksheet.Cells["I" + index])
                        {
                            range.Worksheet.Cells["I" + index].Value = data.TongSLTon;
                            range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }

                        index++;
                        foreach (var chitiet in data.BaoCaoTheKhoVatTuChiTiets)
                        {
                            using (var range = worksheet.Cells["A" + index + ":I" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["B" + index])
                            {
                                range.Worksheet.Cells["B" + index].Value = chitiet.NgayThangDisplay;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["C" + index])
                            {
                                range.Worksheet.Cells["C" + index].Value = chitiet.SCTNhap;
                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["D" + index])
                            {
                                range.Worksheet.Cells["D" + index].Value = chitiet.SCTXuat;
                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["E" + index])
                            {
                                range.Worksheet.Cells["E" + index].Value = chitiet.DienGiai;
                                range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["F" + index])
                            {
                                range.Worksheet.Cells["F" + index].Value = chitiet.SLTonDauKy;                               
                                range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["G" + index])
                            {
                                range.Worksheet.Cells["G" + index].Value = chitiet.SLNhap;
                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["H" + index])
                            {
                                range.Worksheet.Cells["H" + index].Value = chitiet.SLXuat;
                                range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["I" + index])
                            {
                                range.Worksheet.Cells["I" + index].Value = chitiet.SLTon;
                                range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            index++;
                        }
                    }

                    index++;
                    using (var range = worksheet.Cells["G" + index + ":I" + index])
                    {
                        range.Worksheet.Cells["G" + index + ":I" + index].Merge = true;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G" + index + ":I" + index].Value = "Người Lập";
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                        index++;
                        range.Worksheet.Cells["G" + index + ":I" + index].Merge = true;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G" + index + ":I" + index].Value = "(ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                    }


                    xlPackage.Save();
                }

                return stream.ToArray();

            }
        }

        #endregion
    }
}
