using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoBienBanKiemKeDPVT;
using Camino.Core.Domain.ValueObject.Grid;
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
using Camino.Core.Helpers;
using Camino.Data;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {

        public async Task<List<LookupItemVo>> GetKhoNhanVienLookupAsync(LookupQueryInfo queryInfo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var lookup = await _khoRepository.TableNoTracking.Where(s => s.KhoNhanVienQuanLys.Any(o => o.NhanVienId == currentUserId))
                                                             .Select(item => new LookupItemVo
                                                             {
                                                                 DisplayName = item.Ten,
                                                                 KeyId = Convert.ToInt32(item.Id),
                                                             })
                                                           .ApplyLike(queryInfo.Query, g => g.DisplayName)
                                                           .Take(queryInfo.Take)
                                                           .ToListAsync();
            return lookup;
        }

        public async Task<GridDataSource> OldGetDataBaoCaoBienBanKiemKeDPVTForGridAsync(BaoCaoBienBanKiemKeDPVTQueryInfo queryInfo, bool exportExcel = false)
        {
            var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NgayNhap <= queryInfo.GioThongKe && o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId)
                .Select(o=> new
                {
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    SoLuongNhap = o.SoLuongNhap,
                    SoLuongXuat = 0,
                    SoLo = o.Solo,
                    HanDung = o.HanSuDung
                })
                .GroupBy(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.SoLo,
                    o.HanDung,
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                    {
                        DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                        SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                        SoLuongXuat = 0,
                        SoLo = k.SoLo,
                        HanDung = k.HanDung
                    }).ToList();


            var allDataXuatDuocPham = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(o =>
                    o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId &&
                    o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                    ((o.NgayXuat != null && o.NgayXuat <= queryInfo.GioThongKe) ||
                     (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.GioThongKe)))
                .Select(o => new
                {
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    SoLuongNhap = 0,
                    SoLuongXuat = o.SoLuongXuat,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    HanDung = o.NhapKhoDuocPhamChiTiet.HanSuDung
                })
                .GroupBy(o => new
                    {
                        o.DuocPhamBenhVienId,
                        o.SoLo,
                        o.HanDung
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                    {
                        DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                        SoLuongNhap = 0,
                        SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                        SoLo = k.SoLo,
                        HanDung = k.HanDung
                    }).ToList();

            //vat tu
            
            var allDataNhapVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.GioThongKe)
                .Select(o => new
                {
                    VatTuBenhVienId = o.VatTuBenhVienId,
                    SoLuongNhap = o.SoLuongNhap,
                    SoLuongXuat = 0,
                    SoLo = o.Solo,
                    HanDung = o.HanSuDung
                })
                .GroupBy(o => new
                {
                    o.VatTuBenhVienId,
                    o.SoLo,
                    o.HanDung
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                    {
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                        SoLuongXuat = 0,
                        SoLo = k.SoLo,
                        HanDung = k.HanDung
                    }).ToList();    
            System.Threading.Thread.Sleep(1000);

            var sql = $@"SELECT [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung] AS [HanDung]
            FROM[XuatKhoVatTuChiTietViTri] AS[o]
            INNER JOIN[NhapKhoVatTuChiTiet] AS[o.NhapKhoVatTuChiTiet] ON[o].[NhapKhoVatTuChiTietId] = [o.NhapKhoVatTuChiTiet].[Id]
            INNER JOIN[XuatKhoVatTuChiTiet] AS[o.XuatKhoVatTuChiTiet] ON[o].[XuatKhoVatTuChiTietId] = [o.XuatKhoVatTuChiTiet].[Id]
            LEFT JOIN[XuatKhoVatTu] AS[o.XuatKhoVatTuChiTiet.XuatKhoVatTu] ON[o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] = [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[Id]
            WHERE(([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId] = {queryInfo.KhoId}) AND[o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] IS NOT NULL) 
                    AND(([o].[NgayXuat] IS NOT NULL AND([o].[NgayXuat] <= '{queryInfo.GioThongKe.ToString("yyyy-MM-dd HH:mm:ss")}')) OR([o].[NgayXuat] IS NULL AND([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[NgayXuat] <= '{queryInfo.GioThongKe.ToString("yyyy-MM-dd HH:mm:ss")}')))
            GROUP BY[o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung]";

            if(_nhapKhoVatTuChiTietRepository.Context is CaminoObjectContext context)
            {
                var t = context.BaoCaoBienBanKiemKeXuatVTDbQuery.FromSql(sql).ToList();
                Console.WriteLine(t.Count);
            }
            

            var allDataXuatVatTu = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId 
                                                                                                  && o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                                                                                  ((o.NgayXuat != null && o.NgayXuat <= queryInfo.GioThongKe) ||
                                                                                                   (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.GioThongKe)))
                .Select(o => new
                {
                    VatTuBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                    SoLuongNhap = 0,
                    SoLuongXuat = o.SoLuongXuat,
                    SoLo = o.NhapKhoVatTuChiTiet.Solo,
                    HanDung = o.NhapKhoVatTuChiTiet.HanSuDung
                })
                .GroupBy(o => new
                {
                    o.VatTuBenhVienId,
                    o.SoLo,
                    o.HanDung
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                    {
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        SoLuongNhap = 0,
                        SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                        SoLo = k.SoLo,
                        HanDung = k.HanDung
                    }).ToList();

            var allData = allDataNhapDuocPham.Concat(allDataXuatDuocPham).Concat(allDataNhapVatTu).Concat(allDataXuatVatTu);
            var returnData = allData.GroupBy(o => new
            {
                o.DuocPhamBenhVienId,
                o.VatTuBenhVienId,
                o.SoLo,
                o.HanDung,
            }, o => o,
                (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    VatTuBenhVienId = k.VatTuBenhVienId,
                    SoLuongHienCo = v.Select(x => x.SoLuongNhap - x.SoLuongXuat).DefaultIfEmpty().Sum(),
                    SoLo = k.SoLo,
                    HanDung = k.HanDung
                }
            ).Where(o => !o.SoLuongHienCo.AlmostEqual(0))
            .ToArray();

            var thongTinKho = _khoRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var thongTinDuocPham = _duocPhamBenhVienRepository.TableNoTracking.Select(o =>
                new
                {
                    o.Id,
                    Nhom = o.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    DVT = o.DuocPham.DonViTinh.Ten,
                    o.DuocPham.Ten,
                    o.Ma
                }).ToList();
            var thongTinVatTu = _vatTuBenhVienRepository.TableNoTracking.Select(o =>
                new
                {
                    o.Id,
                    Nhom = o.VatTus.NhomVatTu.Ten,
                    DVT = o.VatTus.DonViTinh,
                    o.VatTus.Ten,
                    o.Ma
                }).ToList();

            foreach (var baoCaoBienBanKiemKeDPVTGridVo in returnData)
            {
                if (baoCaoBienBanKiemKeDPVTGridVo.DuocPhamBenhVienId != 0)
                {
                    var dp = thongTinDuocPham.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeDPVTGridVo.DuocPhamBenhVienId);
                    baoCaoBienBanKiemKeDPVTGridVo.DVT = dp?.DVT;
                    baoCaoBienBanKiemKeDPVTGridVo.Ten = dp?.Ten;
                    baoCaoBienBanKiemKeDPVTGridVo.MaDuoc = dp?.Ma;
                }
                else if (baoCaoBienBanKiemKeDPVTGridVo.VatTuBenhVienId != 0)
                {
                    var vt = thongTinVatTu.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeDPVTGridVo.VatTuBenhVienId);
                    baoCaoBienBanKiemKeDPVTGridVo.DVT = vt?.DVT;
                    baoCaoBienBanKiemKeDPVTGridVo.Ten = vt?.Ten;
                    baoCaoBienBanKiemKeDPVTGridVo.MaDuoc = vt?.Ma;
                }
            }

            var returnDataOrderly = returnData.OrderBy(o => o.MaDuoc);
            return new GridDataSource { Data = exportExcel ? returnDataOrderly.ToArray() : returnDataOrderly.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = returnData.Count() };
            
            /*
            var data = new List<BaoCaoBienBanKiemKeDPVTGridVo>()
            {
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 1,
                    MaDuoc = "AOPV204",
                    Ten = "Áo phẫu thuật size L",
                    DVT = "cái",
                    SoLuongHienCo = 3,
                    SoLo = "201970039134",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 2,
                    MaDuoc = "AOPV206",
                    Ten = "Áo phẫu thuật với khẩu trang M",
                    DVT = "cái",
                    SoLuongHienCo = 7,
                    SoLo = "170606",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 3,
                    MaDuoc = "BADV200",
                    Ten = "Băng dính cá nhân Urgo 2cm x 6cm",
                    DVT = "miếng",
                    SoLuongHienCo = 29,
                    SoLo = "662074",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 4,
                    MaDuoc = "BAVV202",
                    Ten = "Băng vệ sinh Diana(mama)",
                    DVT = "Cái",
                    SoLuongHienCo = 519,
                    SoLo = "81207",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 5,
                    MaDuoc = "BADV226",
                    Ten = "Bao đo máu",
                    DVT = "cái",
                    SoLuongHienCo = 7,
                    SoLo = "130820",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 6,
                    MaDuoc = "BICV200",
                    Ten = "Bỉm Caryl XL",
                    DVT = "cái",
                    SoLuongHienCo = 536,
                    SoLo = "212107",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 7,
                    MaDuoc = "BOOV214",
                    Ten = "Bộ ống nội khí quản có cuff số 6,5, Thailand",
                    DVT = "bộ",
                    SoLuongHienCo = 2,
                    SoLo = "1830413FED",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 8,
                    MaDuoc = "BOOV216",
                    Ten = "Bộ ống nội khí quản có cuff số 7, Thailand",
                    DVT = "bộ",
                    SoLuongHienCo = 2,
                    SoLo = "201970039134",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 9,
                    MaDuoc = "B00V244",
                    Ten = "Bộ ống nội khí quản không cuf số 2.5",
                    DVT = "cái",
                    SoLuongHienCo = 4,
                    SoLo = "1823545FED",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 10,
                    MaDuoc = "BOOV202",
                    Ten = "Bộ ống nội khí quản không cuff số 3.5, Thailand",
                    DVT = "bộ",
                    SoLuongHienCo = 2,
                    SoLo = "1823545FED",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 11,
                    MaDuoc = "BOOV202",
                    Ten = "Bộ ống nội khí quản không cuff số 3.5, Thailand",
                    DVT = "bộ",
                    SoLuongHienCo = 2,
                    SoLo = "1614432FED",
                    HanDung = new DateTime(2022,2,7),

                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 12,
                    MaDuoc = "BOOV222",
                    Ten = "Bộ ống nội khí quản không cuff số 3.0, Thailand",
                    DVT = "chiếc",
                    SoLuongHienCo = 4,
                    SoLo = "17232244FED",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 13,
                    MaDuoc = "BOTV2342",
                    Ten = "Bơm tiêm 50ml, VN",
                    DVT = "chiếc",
                    SoLuongHienCo = 1,
                    SoLo = "120",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 14,
                    MaDuoc = "BOTV244",
                    Ten = "Bơm tiêm 5ml (Đốc vàng), VN",
                    DVT = "chiếc",
                    SoLuongHienCo = 100,
                    SoLo = "620",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 15,
                    MaDuoc = "BOTV244",
                    Ten = "Bơm tiêm 5ml (Đốc vàng), VN",
                    DVT = "chiếc",
                    SoLuongHienCo = 36,
                    SoLo = "520",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 16,
                    MaDuoc = "BOTV240",
                    Ten = "Bơm tiêm 10ml, Việt Nam",
                    DVT = "chiếc",
                    SoLuongHienCo = 408,
                    SoLo = "420",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 17,
                    MaDuoc = "BOTV202",
                    Ten = "Bơm tiêm 1ml",
                    DVT = "chiếc",
                    SoLuongHienCo = 100,
                    SoLo = "1808",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 18,
                    MaDuoc = "BOTV202",
                    Ten = "Bơm tiêm 1ml",
                    DVT = "chiếc",
                    SoLuongHienCo = 39,
                    SoLo = "910",
                    HanDung = new DateTime(2022,2,7),
                },
                new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    Id = 19,
                    MaDuoc = "BOTV238",
                    Ten = "Bơm tiêm 20ml, Việt Nam",
                    DVT = "chiếc",
                    SoLuongHienCo = 171,
                    SoLo = "201970039134",
                    HanDung = new DateTime(2022,2,7),
                },
            };
            return new GridDataSource { Data = data.ToArray(), TotalRowCount = data.Count() };
            */
        }

        public async Task<GridDataSource> GetDataBaoCaoBienBanKiemKeDPVTForGridAsync(BaoCaoBienBanKiemKeDPVTQueryInfo queryInfo, bool exportExcel = false)
        {
            if (_nhapKhoVatTuChiTietRepository.Context is CaminoObjectContext context)
            {
                var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NgayNhap <= queryInfo.GioThongKe && o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId)
                    .Select(o => new
                    {
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        SoLuongNhap = o.SoLuongNhap,
                        SoLuongXuat = 0,
                        SoLo = o.Solo,
                        HanDung = o.HanSuDung
                    })
                    .GroupBy(o => new
                        {
                            o.DuocPhamBenhVienId,
                            o.SoLo,
                            o.HanDung,
                        }, o => o,
                        (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                        {
                            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                            SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                            SoLuongXuat = 0,
                            SoLo = k.SoLo,
                            HanDung = k.HanDung
                        }).ToList();

                var sqlXuatDuocPham =
                    $@"SELECT [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung] AS [HanDung]
                    FROM [XuatKhoDuocPhamChiTietViTri] AS [o]
                    INNER JOIN [NhapKhoDuocPhamChiTiet] AS [o.NhapKhoDuocPhamChiTiet] ON [o].[NhapKhoDuocPhamChiTietId] = [o.NhapKhoDuocPhamChiTiet].[Id]
                    INNER JOIN [XuatKhoDuocPhamChiTiet] AS [o.XuatKhoDuocPhamChiTiet] ON [o].[XuatKhoDuocPhamChiTietId] = [o.XuatKhoDuocPhamChiTiet].[Id]
                    LEFT JOIN [XuatKhoDuocPham] AS [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham] ON [o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] = [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[Id]
                    WHERE (([o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId] = {queryInfo.KhoId}) AND [o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] IS NOT NULL) AND (([o].[NgayXuat] IS NOT NULL 
                        AND ([o].[NgayXuat] <= '{queryInfo.GioThongKe.ToString("yyyy-MM-dd HH:mm:ss")}')) OR ([o].[NgayXuat] IS NULL AND ([o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[NgayXuat] <= '{queryInfo.GioThongKe.ToString("yyyy-MM-dd HH:mm:ss")}')))
                    GROUP BY [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung]";

                var xuatDuocPhamDbQuery = context.BaoCaoBienBanKiemKeXuatDPDbQuery.FromSql(sqlXuatDuocPham).ToList();
                var allDataXuatDuocPham = xuatDuocPhamDbQuery.Select(o => new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    SoLuongNhap = 0,
                    SoLuongXuat = o.SoLuongXuat,
                    SoLo = o.SoLo,
                    HanDung = o.HanDung
                }).ToList();

                //var allDataXuatDuocPham = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(o =>
                //        o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId &&
                //        o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                //        ((o.NgayXuat != null && o.NgayXuat <= queryInfo.GioThongKe) ||
                //         (o.NgayXuat == null &&
                //          o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.GioThongKe)))
                //    .Select(o => new
                //    {
                //        DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                //        SoLuongNhap = 0,
                //        SoLuongXuat = o.SoLuongXuat,
                //        SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                //        HanDung = o.NhapKhoDuocPhamChiTiet.HanSuDung
                //    })
                //    .GroupBy(o => new
                //        {
                //            o.DuocPhamBenhVienId,
                //            o.SoLo,
                //            o.HanDung
                //        }, o => o,
                //        (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                //        {
                //            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                //            SoLuongNhap = 0,
                //            SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                //            SoLo = k.SoLo,
                //            HanDung = k.HanDung
                //        }).ToList();

                //vat tu

                var allDataNhapVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.GioThongKe)
                    .Select(o => new
                    {
                        VatTuBenhVienId = o.VatTuBenhVienId,
                        SoLuongNhap = o.SoLuongNhap,
                        SoLuongXuat = 0,
                        SoLo = o.Solo,
                        HanDung = o.HanSuDung
                    })
                    .GroupBy(o => new
                        {
                            o.VatTuBenhVienId,
                            o.SoLo,
                            o.HanDung
                        }, o => o,
                        (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                        {
                            VatTuBenhVienId = k.VatTuBenhVienId,
                            SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                            SoLuongXuat = 0,
                            SoLo = k.SoLo,
                            HanDung = k.HanDung
                        }).ToList();

                var sql =
                    $@"SELECT [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung] AS [HanDung]
                    FROM[XuatKhoVatTuChiTietViTri] AS[o]
                    INNER JOIN[NhapKhoVatTuChiTiet] AS[o.NhapKhoVatTuChiTiet] ON[o].[NhapKhoVatTuChiTietId] = [o.NhapKhoVatTuChiTiet].[Id]
                    INNER JOIN[XuatKhoVatTuChiTiet] AS[o.XuatKhoVatTuChiTiet] ON[o].[XuatKhoVatTuChiTietId] = [o.XuatKhoVatTuChiTiet].[Id]
                    LEFT JOIN[XuatKhoVatTu] AS[o.XuatKhoVatTuChiTiet.XuatKhoVatTu] ON[o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] = [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[Id]
                    WHERE(([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId] = {queryInfo.KhoId}) AND[o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] IS NOT NULL) 
                            AND(([o].[NgayXuat] IS NOT NULL AND([o].[NgayXuat] <= '{queryInfo.GioThongKe.ToString("yyyy-MM-dd HH:mm:ss")}')) OR([o].[NgayXuat] IS NULL AND([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[NgayXuat] <= '{queryInfo.GioThongKe.ToString("yyyy-MM-dd HH:mm:ss")}')))
                    GROUP BY[o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung]";


                var xuatVatTuDbQuery = context.BaoCaoBienBanKiemKeXuatVTDbQuery.FromSql(sql).ToList();
                var allDataXuatVatTu = xuatVatTuDbQuery.Select(o => new BaoCaoBienBanKiemKeDPVTGridVo
                {
                    VatTuBenhVienId = o.VatTuBenhVienId,
                    SoLuongNhap = 0,
                    SoLuongXuat = o.SoLuongXuat,
                    SoLo = o.SoLo,
                    HanDung = o.HanDung
                }).ToList();

                //var allDataXuatVatTu = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(o =>
                //        o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId
                //        && o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                //        ((o.NgayXuat != null && o.NgayXuat <= queryInfo.GioThongKe) ||
                //         (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.GioThongKe)))
                //    .Select(o => new
                //    {
                //        VatTuBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                //        SoLuongNhap = 0,
                //        SoLuongXuat = o.SoLuongXuat,
                //        SoLo = o.NhapKhoVatTuChiTiet.Solo,
                //        HanDung = o.NhapKhoVatTuChiTiet.HanSuDung
                //    })
                //    .GroupBy(o => new
                //        {
                //            o.VatTuBenhVienId,
                //            o.SoLo,
                //            o.HanDung
                //        }, o => o,
                //        (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                //        {
                //            VatTuBenhVienId = k.VatTuBenhVienId,
                //            SoLuongNhap = 0,
                //            SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                //            SoLo = k.SoLo,
                //            HanDung = k.HanDung
                //        }).ToList();

                var allData = allDataNhapDuocPham.Concat(allDataXuatDuocPham).Concat(allDataNhapVatTu)
                    .Concat(allDataXuatVatTu);
                var returnData = allData.GroupBy(o => new
                        {
                            o.DuocPhamBenhVienId,
                            o.VatTuBenhVienId,
                            o.SoLo,
                            o.HanDung,
                        }, o => o,
                        (k, v) => new BaoCaoBienBanKiemKeDPVTGridVo
                        {
                            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                            VatTuBenhVienId = k.VatTuBenhVienId,
                            SoLuongHienCo = v.Select(x => x.SoLuongNhap - x.SoLuongXuat).DefaultIfEmpty().Sum().MathRoundNumber(2),
                            SoLo = k.SoLo,
                            HanDung = k.HanDung
                        }
                    ).Where(o => !o.SoLuongHienCo.AlmostEqual(0))
                    .ToArray();

                var thongTinKho = _khoRepository.TableNoTracking.Select(o => new {o.Id, o.Ten}).ToList();
                var thongTinDuocPham = _duocPhamBenhVienRepository.TableNoTracking.Select(o =>
                    new
                    {
                        o.Id,
                        Nhom = o.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        DVT = o.DuocPham.DonViTinh.Ten,
                        o.DuocPham.Ten,
                        o.Ma,
                        o.DuocPham.HamLuong,
                        o.DuocPham.HoatChat
                    }).ToList();
                var thongTinVatTu = _vatTuBenhVienRepository.TableNoTracking.Select(o =>
                    new
                    {
                        o.Id,
                        Nhom = o.VatTus.NhomVatTu.Ten,
                        DVT = o.VatTus.DonViTinh,
                        o.VatTus.Ten,
                        o.Ma
                    }).ToList();

                foreach (var baoCaoBienBanKiemKeDPVTGridVo in returnData)
                {
                    if (baoCaoBienBanKiemKeDPVTGridVo.DuocPhamBenhVienId != 0)
                    {
                        var dp = thongTinDuocPham.FirstOrDefault(o =>
                            o.Id == baoCaoBienBanKiemKeDPVTGridVo.DuocPhamBenhVienId);
                        baoCaoBienBanKiemKeDPVTGridVo.DVT = dp?.DVT;
                        baoCaoBienBanKiemKeDPVTGridVo.Ten = dp?.Ten;
                        baoCaoBienBanKiemKeDPVTGridVo.MaDuoc = dp?.Ma;
                        baoCaoBienBanKiemKeDPVTGridVo.HamLuong = dp?.HamLuong;
                        baoCaoBienBanKiemKeDPVTGridVo.HoatChat = dp?.HoatChat;
                    }
                    else if (baoCaoBienBanKiemKeDPVTGridVo.VatTuBenhVienId != 0)
                    {
                        var vt = thongTinVatTu.FirstOrDefault(
                            o => o.Id == baoCaoBienBanKiemKeDPVTGridVo.VatTuBenhVienId);
                        baoCaoBienBanKiemKeDPVTGridVo.DVT = vt?.DVT;
                        baoCaoBienBanKiemKeDPVTGridVo.Ten = vt?.Ten;
                        baoCaoBienBanKiemKeDPVTGridVo.MaDuoc = vt?.Ma;
                    }
                }

                var returnDataOrderly = returnData.OrderBy(o => o.MaDuoc);
                return new GridDataSource
                {
                    Data = exportExcel
                        ? returnDataOrderly.ToArray()
                        : returnDataOrderly.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(),
                    TotalRowCount = returnData.Count()
                };
            }

            return null;
        }

        public virtual byte[] ExportBaoCaoBienBanKiemKeDPVT(GridDataSource gridDataSource, BaoCaoBienBanKiemKeDPVTQueryInfo query)
        {
            var datas = (ICollection<BaoCaoBienBanKiemKeDPVTGridVo>)gridDataSource.Data;

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BIÊN BẢN KIỂM KÊ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(4).Height = 24;
                    using (var range = worksheet.Cells["A1:F1"])
                    {
                        range.Worksheet.Cells["A1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:F1"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A1:F1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:F1"].Style.Font.Bold = true;

                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["D1:F1"].Merge = true;
                        range.Worksheet.Cells["D1:F1"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                        range.Worksheet.Cells["D1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    }

                    var tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A2:F2"])
                    {
                        range.Worksheet.Cells["A2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:F2"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A2:F2"].Style.Font.Color.SetColor(Color.Black);

                        range.Worksheet.Cells["A2:C2"].Merge = true;
                        range.Worksheet.Cells["A2:C2"].Value = "Bộ phận: " + tenKho;
                        range.Worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["D2:F2"].Merge = true;
                        range.Worksheet.Cells["D2:F2"].Value = "Độc lập - Tự do - Hạnh phúc";
                        range.Worksheet.Cells["D2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D2:F2"].Style.Font.Bold = true;
                        range.Worksheet.Cells["D2:F2"].Style.Font.Italic = true;

                    }


                    using (var range = worksheet.Cells["A4:J4"])
                    {
                        range.Worksheet.Cells["A4:J4"].Merge = true;
                        range.Worksheet.Cells["A4:J4"].Value = "BIÊN BẢN KIỂM KÊ";
                        range.Worksheet.Cells["A4:J4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:J4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:J4"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A4:J4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:J4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:G5"])
                    {
                        range.Worksheet.Cells["A5:G5"].Merge = true;
                        range.Worksheet.Cells["A5:G5"].Value = $"Hôm nay, ngày {query.GioThongKe.Day} tháng {query.GioThongKe.Month} năm {query.GioThongKe.Year}, chúng tôi gồm:";
                        range.Worksheet.Cells["A5:G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:G5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:G5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:G5"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6:G8"])
                    {
                        
                        range.Worksheet.Cells["A6:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:G8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:G8"].Style.Font.Color.SetColor(Color.Black);

                        range.Worksheet.Cells["A6"].Value = 1;
                        range.Worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        range.Worksheet.Cells["A7"].Value = 2;
                        range.Worksheet.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        range.Worksheet.Cells["A8"].Value = 3;
                        range.Worksheet.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        range.Worksheet.Cells["B6"].Value = "Ông/ bà:";
                        range.Worksheet.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["B7"].Value = "Ông/ bà:";
                        range.Worksheet.Cells["B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["B8"].Value = "Ông/ bà:";
                        range.Worksheet.Cells["B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["C6:C8"].Merge = true;

                        range.Worksheet.Cells["F6"].Value = "Bộ phận:";
                        range.Worksheet.Cells["F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["F7"].Value = "Bộ phận:";
                        range.Worksheet.Cells["F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["F8"].Value = "Khoa:";
                        range.Worksheet.Cells["F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        range.Worksheet.Cells["G6:G8"].Merge = true;

                    }

                    using (var range = worksheet.Cells["A9:G9"])
                    {
                        range.Worksheet.Cells["A9:G9"].Merge = true;
                        range.Worksheet.Cells["A9:G9"].Value = $"Cùng nhau tiến hành kiểm kê số lượng tồn kho thực tế tại kho {tenKho} cụ thể như sau:						";
                        range.Worksheet.Cells["A9:G9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A9:G9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:G9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A9:G9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:J10"])
                    {
                        range.Worksheet.Cells["A10:J10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A10:J10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A10:J10"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A10:J10"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A10:J10"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A10:J10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Worksheet.Cells["A10:J10"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C4BD97"));
                        range.Worksheet.Cells["A10:J10"].Style.Font.Bold = true;

                        range.Worksheet.Cells["A10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A10"].Value = "STT";

                        range.Worksheet.Cells["B10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B10"].Value = "Mã dược";

                        range.Worksheet.Cells["C10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C10"].Value = "Tên thuốc, vật tư, hoá chất";

                        range.Worksheet.Cells["D10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D10"].Value = "Hàm lượng";

                        range.Worksheet.Cells["E10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E10"].Value = "Tên hoạt chất";

                        range.Worksheet.Cells["F10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F10"].Value = "ĐVT";

                        range.Worksheet.Cells["G10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G10"].Value = "Số lượng hiện có";

                        range.Worksheet.Cells["H10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H10"].Value = "Số lô SX";

                        range.Worksheet.Cells["I10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I10"].Value = "Hạn dùng";

                        range.Worksheet.Cells["J10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J10"].Value = "Ghi chú";
                    }

                    int stt = 1;
                    int index = 11;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = stt;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = item.MaDuoc;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.WrapText = true;
                            worksheet.Cells["C" + index].Value = item.Ten;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = item.HamLuong;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = item.HoatChat;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = item.DVT;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = item.SoLuongHienCo;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = item.SoLo;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Value = item.HanDungStr;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Value = item.GhiChu;

                           
                            index++;
                            stt++;
                        }
                    }

                    worksheet.Cells["F" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["F" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["F" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["F" + index + ":J" + index].Style.Font.Italic = true;
                    worksheet.Cells["F" + index + ":J" + index].Merge = true;
                    worksheet.Cells["F" + index + ":J" + index].Value = $"Hà Nội, Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    index++;

                    worksheet.Cells["C" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["C" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["C" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["C" + index + ":J" + index].Style.Font.Bold = true;

                    worksheet.Cells["C" + index].Value = "ĐẠI DIỆN PHÒNG KẾ TOÁN";
                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    worksheet.Cells["F" + index + ":J" + index].Style.Font.Bold = true;
                    worksheet.Cells["F" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index + ":J" + index].Merge = true;
                    worksheet.Cells["F" + index + ":J" + index].Value = "ĐẠI DIỆN KHOA PHÒNG";


                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

    }
}
