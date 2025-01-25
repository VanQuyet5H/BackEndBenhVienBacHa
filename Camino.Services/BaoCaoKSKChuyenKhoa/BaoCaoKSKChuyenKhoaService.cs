using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using System.Globalization;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System.Collections.Generic;
using Camino.Core.Helpers;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.ExportImport.Help;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System.Text.RegularExpressions;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.KhamDoan;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.BaoCaoKSKChuyenKhoa
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoKSKChuyenKhoaService))]

    public class BaoCaoKSKChuyenKhoaService : MasterFileService<HopDongKhamSucKhoeNhanVien>, IBaoCaoKSKChuyenKhoaService
    {
        private readonly IRepository<CongTyKhamSucKhoe> _congTyKhamSucKhoeRepository;
        private readonly IRepository<HopDongKhamSucKhoe> _hopDongKhamSucKhoeRepository;
        private readonly IRepository<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanVienRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<GoiKhamSucKhoe> _goiKhamSucKhoeRepository;


        private readonly IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSoRepository;

        private readonly IKhamDoanService _iKhamDoanService;
        public BaoCaoKSKChuyenKhoaService(
            IRepository<HopDongKhamSucKhoeNhanVien> repository,
            IRepository<CongTyKhamSucKhoe> congTyKhamSucKhoeRepository,
            IRepository<HopDongKhamSucKhoe> hopDongKhamSucKhoeRepository,
            IRepository<HopDongKhamSucKhoeNhanVien> hopDongKhamSucKhoeNhanVienRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
             IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
        IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
        IRepository<GoiKhamSucKhoe> goiKhamSucKhoeRepository,
        IRepository<Template> templateRepository,
         IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSoRepository,
         IKhamDoanService iKhamDoanService
            ) : base(repository)
        {
            _congTyKhamSucKhoeRepository = congTyKhamSucKhoeRepository;
            _hopDongKhamSucKhoeRepository = hopDongKhamSucKhoeRepository;
            _hopDongKhamSucKhoeNhanVienRepository = hopDongKhamSucKhoeNhanVienRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _dichVuXetNghiemKetNoiChiSoRepository = dichVuXetNghiemKetNoiChiSoRepository;
            _iKhamDoanService = iKhamDoanService;
            _templateRepository = templateRepository;
            _goiKhamSucKhoeRepository = goiKhamSucKhoeRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsyncKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            if (!string.IsNullOrEmpty(queryInfo.FromDateString))
            {
                queryInfo.FromDateString.TryParseExactCustom(out DateTime tuNgayTemp);
                queryInfo.FromDate = tuNgayTemp;
            }
            if (!string.IsNullOrEmpty(queryInfo.ToDateString))
            {
                queryInfo.ToDateString.TryParseExactCustom(out DateTime denNgayTemp);
                if (denNgayTemp == null)
                {
                    denNgayTemp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }
                queryInfo.ToDate = denNgayTemp;
            }
            else
            {
                DateTime denNgay = DateTime.Now;
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                queryInfo.ToDate = denNgay;
            }
            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.GoiKhamSucKhoeId != null
                            && o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham
                            && o.ChuyenKhoaKhamSucKhoe != null
                            && o.NoiThucHienId != null
                            && o.ThoiDiemThucHien < queryInfo.ToDate);
            if (queryInfo.FromDate != null)
            {
                query = query.Where(z => z.ThoiDiemThucHien >= queryInfo.FromDate);
            }
            if (queryInfo.CongTyKhamSucKhoeId.GetValueOrDefault() != 0)
            {
                query = queryInfo.HopDongKhamSucKhoeId.GetValueOrDefault() != 0
                    ? query.Where(o => o.GoiKhamSucKhoe.HopDongKhamSucKhoeId == queryInfo.HopDongKhamSucKhoeId)
                    : query.Where(o => o.GoiKhamSucKhoe.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryInfo.CongTyKhamSucKhoeId);
            }

            var queryData = query.Select(o => new NguoiBenhKhamDichVuTheoPhongQueryData
            {
                Id = o.Id,
                HopDongKhamSucKhoeId = o.GoiKhamSucKhoe.HopDongKhamSucKhoeId,
                TenHopDongKhamSucKhoe = o.GoiKhamSucKhoe.HopDongKhamSucKhoe.SoHopDong,
                CongTyKhamSucKhoeId = o.GoiKhamSucKhoe.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                TenCongTyKhamSucKhoe = o.GoiKhamSucKhoe.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                NoiThucHienId = o.NoiThucHienId.Value,
                TenNoiThucHien = o.NoiThucHien.Ten,
                ChuyenKhoaKhamSucKhoe = o.ChuyenKhoaKhamSucKhoe,
                TenChuyenKhoaKhamSucKhoe = o.ChuyenKhoaKhamSucKhoe.GetDescription()
            });
            var noiThucHienCuaNguoiBenhTheoChuyenKhoas = queryData.GroupBy(o => o.ChuyenKhoaKhamSucKhoe).Select(o =>
                new NoiThucHienCuaNguoiBenhTheoChuyenKhoa()
                {
                    ChuyenKhoaKhamSucKhoe = o.Key,
                    TenChuyenKhoaKhamSucKhoe = o.First().TenChuyenKhoaKhamSucKhoe
                }).ToList();

            var dataSource = new List<NguoiBenhKhamDichVuTheoChuyenKhoa>();
            foreach (var groupHopDongKhamSucKhoe in queryData.GroupBy(o => o.HopDongKhamSucKhoeId))
            {
                var nguoiBenhKhamDichVuTheoPhong = new NguoiBenhKhamDichVuTheoChuyenKhoa()
                {
                    Id = groupHopDongKhamSucKhoe.Key,
                    CongTyKhamSucKhoeId = groupHopDongKhamSucKhoe.First().CongTyKhamSucKhoeId,
                    HopDongKhamSucKhoeId = groupHopDongKhamSucKhoe.Key,
                    TenCongTyKhamSucKhoe = groupHopDongKhamSucKhoe.First().TenCongTyKhamSucKhoe,
                    TenHopDongKhamSucKhoe = groupHopDongKhamSucKhoe.First().TenHopDongKhamSucKhoe
                };
                foreach (var noiThucHien in noiThucHienCuaNguoiBenhTheoChuyenKhoas)
                {
                    nguoiBenhKhamDichVuTheoPhong.NoiThucHienCuaNguoiBenhs.Add(new NoiThucHienCuaNguoiBenhTheoChuyenKhoa
                    {
                        ChuyenKhoaKhamSucKhoe = noiThucHien.ChuyenKhoaKhamSucKhoe,
                        TenChuyenKhoaKhamSucKhoe = noiThucHien.TenChuyenKhoaKhamSucKhoe,
                        SoLan = groupHopDongKhamSucKhoe.Count(o => o.ChuyenKhoaKhamSucKhoe == noiThucHien.ChuyenKhoaKhamSucKhoe)
                    });
                }
                dataSource.Add(nguoiBenhKhamDichVuTheoPhong);
            }

            return new GridDataSource { Data = dataSource.ToArray(), TotalRowCount = dataSource.Count };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            return null;
        }

        public virtual byte[] ExportBaoCaoKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, List<NguoiBenhKhamDichVuTheoChuyenKhoa> nguoiBenhKhamDichVuTheoPhongs)
        {
            if (!string.IsNullOrEmpty(queryInfo.FromDateString))
            {
                queryInfo.FromDateString.TryParseExactCustom(out DateTime tuNgayTemp);
                queryInfo.FromDate = tuNgayTemp;
            }
            if (!string.IsNullOrEmpty(queryInfo.ToDateString))
            {
                queryInfo.ToDateString.TryParseExactCustom(out DateTime denNgayTemp);
                if (denNgayTemp == null)
                {
                    denNgayTemp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }
                queryInfo.ToDate = denNgayTemp;
            }
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO KSK THEO CHUYÊN KHOA");
                    var noiThucHienCuaNguoiBenhs = new List<NoiThucHienCuaNguoiBenhTheoChuyenKhoa>();
                    foreach (var item in nguoiBenhKhamDichVuTheoPhongs)
                    {
                        foreach (var nth in item.NoiThucHienCuaNguoiBenhs)
                        {
                            var noiThucHienCuaNguoiBenh = new NoiThucHienCuaNguoiBenhTheoChuyenKhoa
                            {
                                ChuyenKhoaKhamSucKhoe = nth.ChuyenKhoaKhamSucKhoe,
                                TenChuyenKhoaKhamSucKhoe = nth.TenChuyenKhoaKhamSucKhoe,
                                SoLan = nth.SoLan
                            };
                            noiThucHienCuaNguoiBenhs.Add(noiThucHienCuaNguoiBenh);
                        }
                    }
                    var noiThucHienCuaNguoiBenhHeaders = noiThucHienCuaNguoiBenhs.GroupBy(g => new
                    {
                        g.ChuyenKhoaKhamSucKhoe,
                        g.TenChuyenKhoaKhamSucKhoe
                    }).Select(g => g.First()).OrderBy(z => z.ChuyenKhoaKhamSucKhoe).ThenBy(z => z.TenChuyenKhoaKhamSucKhoe).ToList();
                    var tongDichVus = noiThucHienCuaNguoiBenhHeaders.Count();
                    var danhSachKyTuXuLy = new List<string>();
                    if (tongDichVus > 26)
                    {
                        danhSachKyTuXuLy = XuLyDanhSachKyTu(tongDichVus, 26);
                    }
                    else
                    {
                        danhSachKyTuXuLy = KyTus();
                    }
                    worksheet.Row(danhSachKyTuXuLy.Count()).Height = 24.5;
                    for (int i = 1; i <= tongDichVus; i++)
                    {
                        worksheet.Column(i).Width = 20;
                        worksheet.Column(i + 5).Width = 20;
                        worksheet.Column(i + 6).Width = 20;
                    }


                    var index = 3; // bắt đầu từ A3(dòng 3)
                    using (var range = worksheet.Cells["A" + index])
                    {
                        range.Worksheet.Cells["A" + index].Value = "STT";
                        range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["B" + index])
                    {
                        range.Worksheet.Cells["B" + index].Value = "Tên công ty/ Đối tượng khám sức khỏe";
                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["C" + index])
                    {
                        range.Worksheet.Cells["C" + index].Value = "Số hợp đồng";
                        range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C" + index].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["D" + index])
                    {
                        range.Worksheet.Cells["D" + index].Value = "Tổng số";
                        range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D" + index].Style.Font.Bold = true;
                    }
                    var noiThucHienCuaNguoiBenhHeaderClones = new List<NoiThucHienCuaNguoiBenhTheoChuyenKhoa>(); // Dùng để tính tổng ngang số lần từng phòng
                    // Hiển thị các dịch vụ theo column
                    var indexKyTu = 4;
                    var kyTuCuoiCung = "E";
                    foreach (var dv in noiThucHienCuaNguoiBenhHeaders)
                    {
                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + index])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Value = dv.TenChuyenKhoaKhamSucKhoe;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Font.Bold = true;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            }
                            kyTuCuoiCung = danhSachKyTuXuLy[indexKyTu];
                            break;
                        }
                        indexKyTu++;
                        var soLan = noiThucHienCuaNguoiBenhs.Where(z => z.ChuyenKhoaKhamSucKhoe == dv.ChuyenKhoaKhamSucKhoe).Sum(z => z.SoLan) ?? 0;
                        var noiThucHienCuaNguoiBenhHeaderCloneNew = new NoiThucHienCuaNguoiBenhTheoChuyenKhoa
                        {
                            ChuyenKhoaKhamSucKhoe = dv.ChuyenKhoaKhamSucKhoe,
                            TenChuyenKhoaKhamSucKhoe = dv.TenChuyenKhoaKhamSucKhoe,
                            SoLan = soLan
                        };
                        noiThucHienCuaNguoiBenhHeaderClones.Add(noiThucHienCuaNguoiBenhHeaderCloneNew);
                    }

                    using (var range = worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"])
                    {
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Merge = true;
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Value = "BÁO CÁO KHÁM SỨC KHỎE THEO CHUYÊN KHOA";
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"])
                    {
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Merge = true;
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Value = "Từ ngày: " + queryInfo.FromDate?.ApplyFormatDateTime()
                                                          + " đến " + queryInfo.ToDate?.ApplyFormatDateTime();
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.Font.Color.SetColor(Color.Black);
                    }

                    index++;
                    var STT = 1;
                    foreach (var data in nguoiBenhKhamDichVuTheoPhongs)
                    {
                        indexKyTu = 0;// gán lại để đổ data
                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = STT;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;
                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = data.TenCongTyKhamSucKhoe;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;
                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = data.TenHopDongKhamSucKhoe;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;

                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = data.TongSo;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;
                        foreach (var item in data.NoiThucHienCuaNguoiBenhs.OrderBy(z => z.ChuyenKhoaKhamSucKhoe).ThenBy(z => z.TenChuyenKhoaKhamSucKhoe))
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = item.SoLan;
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                        }
                        STT++;
                        index++;
                    }
                    //Tổng cộng
                    using (var range = worksheet.Cells["A" + index + ":C" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":C" + index].Value = "TỔNG CỘNG: ";
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;

                    }
                    indexKyTu = 3; // gán lại cho tổng số
                    using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                    {
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = nguoiBenhKhamDichVuTheoPhongs.Sum(z => z.TongSo);
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Bold = true;

                    }
                    indexKyTu++;

                    foreach (var item in noiThucHienCuaNguoiBenhHeaderClones.OrderBy(z => z.ChuyenKhoaKhamSucKhoe).ThenBy(z => z.TenChuyenKhoaKhamSucKhoe))
                    {
                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = item.SoLan;
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Bold = true;
                            }
                            break;
                        }
                        indexKyTu++;
                    }
                    index = index + 2;
                    var indexChuKy = index;
                    using (var range = worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy])
                    {
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Value = DateTime.Now.ApplyFormatNgayThangNam();
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    indexChuKy++;
                    index++;
                    using (var range = worksheet.Cells["B" + indexChuKy])
                    {
                        range.Worksheet.Cells["B" + index + ":B" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["B" + indexChuKy].Value = "NGƯỜI LẬP PHIẾU";
                        range.Worksheet.Cells["B" + indexChuKy].Style.WrapText = true;
                        range.Worksheet.Cells["B" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["D" + indexChuKy])
                    {
                        range.Worksheet.Cells["D" + index + ":D" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["D" + indexChuKy].Value = "TRƯỞNG PHÒNG KTTH";
                        range.Worksheet.Cells["D" + indexChuKy].Style.WrapText = true;
                        range.Worksheet.Cells["D" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["F" + index + ":G" + indexChuKy])
                    {
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Value = "GIÁM ĐỐC";
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Bold = true;
                    }

                    indexChuKy++;
                    using (var range = worksheet.Cells["B" + indexChuKy])
                    {
                        range.Worksheet.Cells["B" + indexChuKy].Value = "(Ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["B" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D" + indexChuKy])
                    {
                        range.Worksheet.Cells["D" + indexChuKy].Value = "(Ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["D" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["F" + index + ":G" + indexChuKy])
                    {
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Value = "(Ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
        private List<string> KyTus()
        {
            var kyTus = new List<string>()
                    { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };// 26 ký tự
            return kyTus;
        }
        private List<string> XuLyDanhSachKyTu(int soDichVu, int tongSoKyTu)
        {
            var danhSachKyTuSauKhiXuKy = new List<string>();
            var danhSachKyTuPre = KyTus();
            var danhSachKyTuResult = KyTus();
            var soLanLap = soDichVu / tongSoKyTu;
            danhSachKyTuPre.Take(soLanLap);
            for (int i = 0; i < soLanLap; i++)
            {
                foreach (var result in danhSachKyTuPre)
                {
                    foreach (var item in danhSachKyTuResult)
                    {
                        danhSachKyTuSauKhiXuKy.Add(result + item);
                    }
                }
            }
            danhSachKyTuResult.AddRange(danhSachKyTuResult);
            return danhSachKyTuResult;
        }
    }
}
