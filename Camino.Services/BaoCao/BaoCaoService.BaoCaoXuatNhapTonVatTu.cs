using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        #region Báo cáo xuất nhập tồn vật tư

        public async Task<GridDataSource> GetDataBaoCaoXuatNhapTonVTForGridAsync(BaoCaoXuatNhapTonVTQueryInfo queryInfo)
        {
            IQueryable<NhapKhoVatTuChiTiet> allDataNhapQuery = null;
            if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoVatTuYTe)
            {
                allDataNhapQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate);
            }
            else
            {
                allDataNhapQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.KhoNhapSauKhiDuyetId == null && o.NgayNhap <= queryInfo.ToDate);                
            }

            var allDataNhap = allDataNhapQuery
                    .Select(o => new BaoCaoChiTietXuatNhapTonVTGridVo()
                    {
                        Id = o.Id,
                        VatTuBenhVienId = o.VatTuBenhVienId,
                        Ma = o.VatTuBenhVien.Ma,
                        Ten = o.VatTuBenhVien.VatTus.Ten,
                        NuocSanXuat = o.VatTuBenhVien.VatTus.NuocSanXuat,
                        DVT = o.VatTuBenhVien.VatTus.DonViTinh,
                        SoLo = o.Solo,
                        DonGiaNhap = o.DonGiaNhap,
                        VAT = o.VAT,
                        NgayNhapXuat = o.NgayNhap,
                        LaVatTuBHYT = o.LaVatTuBHYT,
                        Nhom = o.VatTuBenhVien.VatTus != null ? o.VatTuBenhVien.VatTus.NhomVatTu != null ? o.VatTuBenhVien.VatTus.NhomVatTu.Ten : "" : "",
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            IQueryable<XuatKhoVatTuChiTietViTri> allDataXuatQuery = null;
            if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoVatTuYTe)
            {
                allDataXuatQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));
            }
            else
            {
                allDataXuatQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));                
            }

            var allDataXuat = allDataXuatQuery
                .Select(o => new BaoCaoChiTietXuatNhapTonVTGridVo
                {
                    Id = o.Id,
                    VatTuBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ma = o.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    Ten = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    NuocSanXuat = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NuocSanXuat,
                    DVT = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    SoLo = o.NhapKhoVatTuChiTiet.Solo,
                    DonGiaNhap = o.NhapKhoVatTuChiTiet.DonGiaNhap,
                    VAT = o.NhapKhoVatTuChiTiet.VAT,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                    LaVatTuBHYT = o.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    Nhom = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus != null ? o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu != null ? o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten : "" : "",
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.Ten).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.LaVatTuBHYT, o.VatTuBenhVienId, o.SoLo, o.DonGiaNhapSauVAT });
            var dataReturn = new List<BaoCaoXuatNhapTonGridVo>();
            foreach (var xuatNhapVatTu in allDataGroup)
            {
                var tonDau = xuatNhapVatTu.Where(o => o.NgayNhapXuat < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = xuatNhapVatTu.Where(o => o.NgayNhapXuat >= queryInfo.FromDate).ToList();
                var nhapTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2);
                var xuatTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2);
                var tonCuoi = (tonDau + nhapTrongKy - xuatTrongKy).MathRoundNumber(2);

                if (!tonDau.AlmostEqual(0) || !nhapTrongKy.AlmostEqual(0) || !xuatTrongKy.AlmostEqual(0) || !tonCuoi.AlmostEqual(0))
                {
                    dataReturn.Add(new BaoCaoXuatNhapTonGridVo
                    {
                        Ten = xuatNhapVatTu.First().Ten,
                        NuocSanXuat = xuatNhapVatTu.First().NuocSanXuat,
                        DVT = xuatNhapVatTu.First().DVT,
                        SoLo = xuatNhapVatTu.Key.SoLo,
                        SLTonDauKy = tonDau,
                        DonGiaTonDauKy = tonDau > 0 ? xuatNhapVatTu.Key.DonGiaNhapSauVAT : (decimal?)null,
                        SLNhapTrongKy = nhapTrongKy,
                        DonGiaNhapTrongKy = nhapTrongKy > 0 ? xuatNhapVatTu.Key.DonGiaNhapSauVAT : (decimal?)null,
                        SLXuatTrongKy = xuatTrongKy,
                        DonGiaXuatTrongKy = xuatTrongKy > 0 ? xuatNhapVatTu.Key.DonGiaNhapSauVAT : (decimal?)null,
                        SLTonCuoiKy = tonCuoi,
                        DonGiaTonCuoiKy = tonCuoi > 0 ? xuatNhapVatTu.Key.DonGiaNhapSauVAT : (decimal?)null,
                        Loai = xuatNhapVatTu.Key.LaVatTuBHYT ? "Thuốc BHYT" : "Viện phí",
                        Nhom = xuatNhapVatTu.First().Nhom
                    });
                }
            }

            /*
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var khoId = long.Parse(queryObj[0]);

            //get thuốc vật tư theo kho và theo ngày
            // điều kiện khoId và filter từ ngày đến ngày nếu có
            var datas = new List<BaoCaoXuatNhapTonGridVo>()
            {
                new BaoCaoXuatNhapTonGridVo
                {
                    Id = 1,
                    Ten = "Băng chun 3 móc QM 10.2cm x 5.5m XXXXX",
                    HamLuong = "ABC",
                    SoLo = "123",
                    DVT = "Cuộn",
                    SLTonDauKy = 21,
                    SLNhapTrongKy = 0,
                    SLXuatTrongKy = 1,
                    SLTonCuoiKy = 20,
                    DonGiaTonCuoiKy = 14000,
                    Loai = "Viện phí",
                    Nhom = "A1",
                    DonGiaTonDauKy = 2000,
                    DonGiaNhapTrongKy = 3000,
                    DonGiaXuatTrongKy = 4000

                },
                new BaoCaoXuatNhapTonGridVo
                {
                    Id = 2,
                    Ten = "Bông viên 10g (Bảo Thạch, Việt Nam)",
                    HamLuong = "ABC",
                    SoLo = "123",
                    DVT = "Gói",
                    SLTonDauKy = 20,
                    SLNhapTrongKy = 0,
                    SLXuatTrongKy = 4,
                    SLTonCuoiKy = 16,
                    DonGiaTonCuoiKy = 2700,
                    Loai = "Viện phí",
                    Nhom = "A1",
                    DonGiaTonDauKy = 2000,
                    DonGiaNhapTrongKy = 3000,
                    DonGiaXuatTrongKy = 4000

                },
            new BaoCaoXuatNhapTonGridVo
            {
                Id = 3,
                Ten = "Gạc miếng 10x10cm x 12 lớp",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Túi",
                SLTonDauKy = 103,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 38,
                SLTonCuoiKy = 65,
                DonGiaTonCuoiKy = 7970,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 4,
                Ten = "Tưa lưỡi trẻ em hộp (Đông Pha, Việt Nam)",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Hộp",
                SLTonDauKy = 100,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 9,
                SLTonCuoiKy = 91,
                DonGiaTonCuoiKy = 1600,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 5,
                Ten = "Aerius 0,5mg/ml 60ml",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Chai",
                SLTonDauKy = 15,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 15,
                SLTonCuoiKy = 0,
                DonGiaTonCuoiKy = 0,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000


            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 6,
                Ten = "Rocuronium-BFS",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Gói",
                SLTonDauKy = 16,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000


            },new BaoCaoXuatNhapTonGridVo
            {
                Id = 7,
                Ten = "Rhomatic Gel a",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Lít",
                SLTonDauKy = 46,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 8,
                Ten = "Flucinar 15g",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Tuýp",
                SLTonDauKy = 6,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 11,
                Ten = "Băng chun 16 móc QM 10.2cm x 5.5m 1235435435",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Cuộn",
                SLTonDauKy = 25,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 1,
                SLTonCuoiKy = 20,
                DonGiaTonCuoiKy = 14000,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            },new BaoCaoXuatNhapTonGridVo
            {
                Id = 12,
                Ten = "Băng chun 13 móc QM 10.2cm x 5.5m ABCSTTS",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Cuộn",
                SLTonDauKy = 11,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 1,
                SLTonCuoiKy = 20,
                DonGiaTonCuoiKy = 14000,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }
            };*/
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.Loai).ThenBy(s => s.Nhom).ToArray(), TotalRowCount = dataReturn.Count };
        }
        public async Task<GridDataSource> GetTotalBaoCaoXuatNhapTonVTForGridAsync(BaoCaoXuatNhapTonVTQueryInfo queryInfo)
        {
            return null;
        }

        public async Task<GridDataSource> GetDataBaoCaoXuatNhapTonVTForGridAsyncChild(BaoCaoXuatNhapTonVTQueryInfo queryInfo, bool exportExcel)
        {
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var khoId = long.Parse(queryObj[0]);
            var NhomVatTuVatTuId = long.Parse(queryObj[1]);
            //var tuNgay = queryObj[2];
            //var denNgay = queryObj[3];
            var listGridVos = new List<BaoCaoXuatNhapTonVTGridVo>();
            return new GridDataSource { Data = listGridVos.Take(queryInfo.Take).ToArray(), TotalRowCount = listGridVos.Count };
        }

        public async Task<GridDataSource> GetTotalBaoCaoXuatNhapTonVTForGridAsyncChild(BaoCaoXuatNhapTonVTQueryInfo queryInfo)
        {
            return null;
        }

        public string InBaoCaoXuatNhapTonVT(InBaoCaoXuatNhapTonVTVo inBaoCaoXuatNhapTon)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoXuatNhapTon")).First();
            var datas = new List<BaoCaoXuatNhapTonVTGridVo>()
            {
                new BaoCaoXuatNhapTonVTGridVo
                {
                    Id = 1,
                    Ten = "Băng chun 3 móc QM 10.2cm x 5.5m XXXXX",
                    HamLuong = "ABC",
                    SoLo = "123",
                    DVT = "Cuộn",
                    SLTonDauKy = 21,
                    SLNhapTrongKy = 0,
                    SLXuatTrongKy = 1,
                    SLTonCuoiKy = 20,
                    DonGiaTonCuoiKy = 14000,
                    Loai = "Viện phí",
                    Nhom = "A1",
                    DonGiaTonDauKy = 2000,
                    DonGiaNhapTrongKy = 3000,
                    DonGiaXuatTrongKy = 4000

                },
                new BaoCaoXuatNhapTonVTGridVo
                {
                    Id = 2,
                    Ten = "Bông viên 10g (Bảo Thạch, Việt Nam)",
                    HamLuong = "ABC",
                    SoLo = "123",
                    DVT = "Gói",
                    SLTonDauKy = 20,
                    SLNhapTrongKy = 0,
                    SLXuatTrongKy = 4,
                    SLTonCuoiKy = 16,
                    DonGiaTonCuoiKy = 2700,
                    Loai = "Viện phí",
                    Nhom = "A1",
                    DonGiaTonDauKy = 2000,
                    DonGiaNhapTrongKy = 3000,
                    DonGiaXuatTrongKy = 4000

                },
            new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 3,
                Ten = "Gạc miếng 10x10cm x 12 lớp",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Túi",
                SLTonDauKy = 103,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 38,
                SLTonCuoiKy = 65,
                DonGiaTonCuoiKy = 7970,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 4,
                Ten = "Tưa lưỡi trẻ em hộp (Đông Pha, Việt Nam)",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Hộp",
                SLTonDauKy = 100,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 9,
                SLTonCuoiKy = 91,
                DonGiaTonCuoiKy = 1600,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 5,
                Ten = "Aerius 0,5mg/ml 60ml",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Chai",
                SLTonDauKy = 15,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 15,
                SLTonCuoiKy = 0,
                DonGiaTonCuoiKy = 0,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000


            }, new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 6,
                Ten = "Rocuronium-BFS",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Gói",
                SLTonDauKy = 16,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000


            },new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 7,
                Ten = "Rhomatic Gel a",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Lít",
                SLTonDauKy = 46,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 8,
                Ten = "Flucinar 15g",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Tuýp",
                SLTonDauKy = 6,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 11,
                Ten = "Băng chun 16 móc QM 10.2cm x 5.5m 1235435435",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Cuộn",
                SLTonDauKy = 25,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 1,
                SLTonCuoiKy = 20,
                DonGiaTonCuoiKy = 14000,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            },new BaoCaoXuatNhapTonVTGridVo
            {
                Id = 12,
                Ten = "Băng chun 13 móc QM 10.2cm x 5.5m ABCSTTS",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Cuộn",
                SLTonDauKy = 11,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 1,
                SLTonCuoiKy = 20,
                DonGiaTonCuoiKy = 14000,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }
            };

            var lstLoai = datas.GroupBy(x => new { x.Loai })
                .Select(item => new LoaiGroupVoVT
                {
                    Loai = item.First().Loai

                }).OrderBy(p => p.Loai).ToList();
            var lstNhom = datas.GroupBy(x => new { x.Loai })
                .Select(item => new NhomGroupVoVT
                {
                    Loai = item.First().Loai,
                    Nhom = item.First().Nhom

                }).OrderBy(p => p.Nhom).ToList();
            var stt = 1;
            var html = "";
            if (lstLoai.Any())
            {
                foreach (var loai in lstLoai)
                {
                    var listNhomTheoLoai = lstNhom.Where(o => o.Loai == loai.Loai).ToList();
                    if (listNhomTheoLoai.Any())
                    {
                        html += "<tr><td colspan='5'>" + loai.Loai + "</td><td></td><td></td><td></td><td></td><td></td></tr>";
                        foreach (var nhom in listNhomTheoLoai)
                        {
                            var listVatTuTheoNhom =
                                datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();
                            if (listVatTuTheoNhom.Any())
                            {
                                html += "<tr><td></td><td colspan='11'>" + nhom.Loai + "</td></tr>";
                                foreach (var VatTu in listVatTuTheoNhom)
                                {
                                    stt++;
                                }
                            }
                        }
                    }
                }
            }

            var data = new InBaoCaoXuatNhapTonVTData
            {
                LogoUrl = inBaoCaoXuatNhapTon.HostingName + "/assets/img/logo-bacha-full.png",
                TenKho = "Tất cả",
                ThoiGian = inBaoCaoXuatNhapTon.FromDate.Replace("AM", "SA").Replace("PM", "CH") + " đến ngày " + inBaoCaoXuatNhapTon.ToDate.Replace("AM", "SA").Replace("PM", "CH"),
                BaoCaoXuatNhapTon = html
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public virtual byte[] ExportBaoCaoXuatNhapTonVT(GridDataSource gridDataSource, BaoCaoXuatNhapTonVTQueryInfo query)
        {
            var datas = (ICollection<BaoCaoXuatNhapTonGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoXuatNhapTonGridVo>("STT", p => ind++)
            };
            var lstLoai = datas.GroupBy(x => new { x.Loai })
                .Select(item => new LoaiGroupVo
                {
                    Loai = item.First().Loai

                }).OrderBy(p => p.Loai).ToList();
            var lstNhom = datas.GroupBy(x => new { x.Nhom, x.Loai })
               .Select(item => new NhomGroupVo
               {
                   Loai = item.First().Loai,
                   Nhom = item.First().Nhom

               }).OrderBy(p => p.Nhom).ToList();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO XUẤT NHẬP TỒN VẬT TƯ");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;
                    worksheet.Column(17).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    using (var range = worksheet.Cells["A1:Q1"])
                    {
                        //                        var url = hostingName + "/assets/img/logo-bacha-full.png";
                        //                        WebClient wc = new WebClient();
                        //                        byte[] bytes = wc.DownloadData(url); // download file từ server
                        //                        MemoryStream ms = new MemoryStream(bytes); //
                        //                        Image img = Image.FromStream(ms); // chuyển đổi thành img
                        //                        ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                        //                        pic.SetPosition(0, 0, 0, 0);
                        //                        var height = 120; // chiều cao từ A1 đến A6
                        //                        var width = 510; // chiều rộng từ A1 đến D1
                        //                        pic.SetSize(width, height);
                        //                        range.Worksheet.Protection.IsProtected = false;
                        //                        range.Worksheet.Protection.AllowSelectLockedCells = false;
                        range.Worksheet.Cells["A1:G1"].Merge = true;
                        range.Worksheet.Cells["A1:G1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:Q3"])
                    {
                        range.Worksheet.Cells["A3:Q3"].Merge = true;
                        range.Worksheet.Cells["A3:Q3"].Value = "BÁO CÁO XUẤT NHẬP TỒN VẬT TƯ";
                        range.Worksheet.Cells["A3:Q3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:Q3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:Q3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:Q3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:Q3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:Q4"])
                    {
                        range.Worksheet.Cells["A4:Q4"].Merge = true;
                        range.Worksheet.Cells["A4:Q4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:Q4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:Q4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:Q4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:Q4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:Q4"].Style.Font.Bold = true;
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
                    using (var range = worksheet.Cells["A5:Q5"])
                    {
                        range.Worksheet.Cells["A5:Q5"].Merge = true;
                        range.Worksheet.Cells["A5:Q5"].Value = "Kho: " + tenKho;
                        range.Worksheet.Cells["A5:Q5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:Q5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:Q5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:Q5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:Q5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:Q7"])
                    {
                        range.Worksheet.Cells["A7:Q7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:Q7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:Q7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:Q7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:Q7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8:Q8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:Q8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:Q8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:Q8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:Q8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:Q8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7:A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7:A8"].Merge = true;
                        range.Worksheet.Cells["A7:A8"].Value = "STT";

                        range.Worksheet.Cells["B7:B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7:B8"].Merge = true;
                        range.Worksheet.Cells["B7:B8"].Value = "Tên vật tư";

                        range.Worksheet.Cells["C7:C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7:C8"].Merge = true;
                        range.Worksheet.Cells["C7:C8"].Value = "Nước sản xuất";

                        range.Worksheet.Cells["D7:D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7:D8"].Merge = true;
                        range.Worksheet.Cells["D7:D8"].Value = "Số lô";

                        range.Worksheet.Cells["E7:E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7:E8"].Merge = true;
                        range.Worksheet.Cells["E7:E8"].Value = "ĐVT";

                        range.Worksheet.Cells["F7:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7:H7"].Merge = true;
                        range.Worksheet.Cells["F7:H7"].Value = "Tồn đầu kỳ";
                        range.Worksheet.Cells["F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F8"].Value = "Số lượng";
                        range.Worksheet.Cells["G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G8"].Value = "Giá";
                        range.Worksheet.Cells["H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H8"].Value = "Thành tiền";

                        range.Worksheet.Cells["I7:K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7:K7"].Merge = true;
                        range.Worksheet.Cells["I7:K7"].Value = "Nhập trong kỳ";
                        range.Worksheet.Cells["I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I8"].Value = "Số lượng";
                        range.Worksheet.Cells["J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J8"].Value = "Giá";
                        range.Worksheet.Cells["K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K8"].Value = "Thành tiền";

                        range.Worksheet.Cells["L7:N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7:N7"].Merge = true;
                        range.Worksheet.Cells["L7:N7"].Value = "Xuất trong kỳ";
                        range.Worksheet.Cells["L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L8"].Value = "Số lượng";
                        range.Worksheet.Cells["M8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M8"].Value = "Giá";
                        range.Worksheet.Cells["N8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N8"].Value = "Thành tiền";

                        range.Worksheet.Cells["O7:Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O7:Q7"].Merge = true;
                        range.Worksheet.Cells["O7:Q7"].Value = "Tồn cuối kỳ";
                        range.Worksheet.Cells["O8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O8"].Value = "Số lượng";
                        range.Worksheet.Cells["P8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["P8"].Value = "Giá";
                        range.Worksheet.Cells["Q8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q8"].Value = "Thành tiền";
                    }

                    var manager = new PropertyManager<BaoCaoXuatNhapTonGridVo>(requestProperties);
                    int index = 9; // bắt đầu đổ data từ dòng 13

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;
                    if (lstLoai.Any())
                    {
                        foreach (var loai in lstLoai)
                        {
                            var listNhomTheoLoai = lstNhom.Where(o => o.Loai == loai.Loai).ToList();
                            if (listNhomTheoLoai.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":Q" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":E" + index].Value = loai.Loai;
                                    range.Worksheet.Cells["A" + index + ":E" + index].Merge = true;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["H" + index].Style.Font.Bold = true;
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienTonDauKy);

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["K" + index].Style.Font.Bold = true;
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienNhapTrongKy);

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["N" + index].Style.Font.Bold = true;
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienXuatTrongKy);

                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["Q" + index].Style.Font.Bold = true;
                                    worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["Q" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienTonCuoiKy);
                                }
                                index++;
                                foreach (var nhom in listNhomTheoLoai)
                                {
                                    using (var range = worksheet.Cells["B" + index + ":Q" + index])
                                    {
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Font.Bold = true;

                                        range.Worksheet.Cells["B" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index + ":E" + index].Value = nhom.Nhom;
                                        range.Worksheet.Cells["B" + index + ":E" + index].Merge = true;

                                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["H" + index].Style.Font.Bold = true;
                                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["H" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienTonDauKy);

                                        range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["KI" + index].Style.Font.Bold = true;
                                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["K" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienNhapTrongKy);

                                        range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["N" + index].Style.Font.Bold = true;
                                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["N" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienXuatTrongKy);

                                        range.Worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["Q" + index].Style.Font.Bold = true;
                                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["Q" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienTonCuoiKy);
                                    }
                                    index++;

                                    var listVatTuTheoNhom = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();
                                    if (listVatTuTheoNhom.Any())
                                    {
                                        foreach (var VatTu in listVatTuTheoNhom)
                                        {
                                            manager.CurrentObject = VatTu;
                                            //// format border, font chữ,....
                                            worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                            //worksheet.Cells["A" + index + ":B" + index].Style.Font.Bold = true;
                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Row(index).Height = 20.5;
                                            manager.WriteToXlsx(worksheet, index);
                                            // Đổ data
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index].Value = stt;

                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["B" + index].Value = VatTu.Ten;

                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["C" + index].Value = VatTu.NuocSanXuat;

                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["D" + index].Value = VatTu.SoLo;

                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["E" + index].Value = VatTu.DVT;

                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["F" + index].Value = VatTu.SLTonDauKy;

                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["G" + index].Value = VatTu.DonGiaTonDauKy;

                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["H" + index].Value = VatTu.ThanhTienTonDauKy;

                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["I" + index].Value = VatTu.SLNhapTrongKy;

                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["J" + index].Value = VatTu.DonGiaNhapTrongKy;

                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["K" + index].Value = VatTu.ThanhTienNhapTrongKy;

                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["L" + index].Value = VatTu.SLXuatTrongKy;

                                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["M" + index].Value = VatTu.DonGiaXuatTrongKy;

                                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["N" + index].Value = VatTu.ThanhTienXuatTrongKy;

                                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["O" + index].Value = VatTu.SLTonCuoiKy;

                                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["P" + index].Value = VatTu.DonGiaTonCuoiKy;

                                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["Q" + index].Value = VatTu.ThanhTienTonCuoiKy;
                                            index++;
                                            stt++;
                                        }
                                    }
                                }
                            }
                        }

                        //footer tính tổng số tiền
                        //set font size, merge,...
                        worksheet.Cells["A" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //set border
                        worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        worksheet.Cells["A" + index + ":E" + index].Merge = true;
                        //value
                        worksheet.Cells["A" + index].Value = "Tổng cộng";
                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index].Value = datas.Sum(p => p.ThanhTienTonDauKy);


                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index].Value = datas.Sum(p => p.ThanhTienNhapTrongKy);

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Value = datas.Sum(p => p.ThanhTienXuatTrongKy);

                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Value = datas.Sum(p => p.ThanhTienTonCuoiKy);
                        index++;
                    }

                    index = index + 3;
                    worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["A" + index + ":E" + index].Value = "Người lập";
                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":E" + index].Merge = true;

                    worksheet.Cells["F" + index + ":H" + index].Value = "Thủ kho";
                    worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index + ":H" + index].Merge = true;

                    worksheet.Cells["I" + index + ":K" + index].Value = "Kế toán";
                    worksheet.Cells["I" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":K" + index].Merge = true;

                    worksheet.Cells["L" + index + ":N" + index].Value = "Trưởng khoa dược/VTYT";
                    worksheet.Cells["L" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["L" + index + ":N" + index].Merge = true;

                    //Ngày tháng  ký
                    var rowNgay = index - 2;
                    var ngayHienTai = DateTime.Now;
                    worksheet.Cells["O" + rowNgay + ":Q" + rowNgay].Value = "Ngày " + ngayHienTai.Day + " Tháng " + ngayHienTai.Month + " Năm " + ngayHienTai.Year;
                    worksheet.Cells["O" + rowNgay + ":Q" + rowNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["O" + rowNgay + ":Q" + rowNgay].Merge = true;
                   

                    worksheet.Cells["O" + index + ":Q" + index].Value = "Trường bộ phận";
                    worksheet.Cells["O" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["O" + index + ":Q" + index].Merge = true;

                    index++;

                    worksheet.Cells["A" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":O" + index].Style.Font.Italic = true;
                    //value
                    worksheet.Cells["A" + index + ":E" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":E" + index].Merge = true;

                    worksheet.Cells["F" + index + ":H" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index + ":H" + index].Merge = true;

                    worksheet.Cells["I" + index + ":K" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["I" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":K" + index].Merge = true;

                    worksheet.Cells["L" + index + ":N" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["L" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["L" + index + ":N" + index].Merge = true;

                    worksheet.Cells["O" + index + ":Q" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["O" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["O" + index + ":Q" + index].Merge = true;
                    index++;


                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
        #endregion

    }
}
