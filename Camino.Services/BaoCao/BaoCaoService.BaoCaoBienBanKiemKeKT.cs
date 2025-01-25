using Camino.Core.Domain.ValueObject;
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
using System.Threading.Tasks;

using Camino.Core.Data;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<List<LookupItemVo>> GetTatCaKhoaBaoCaoKiemKeKTs(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(Kho.Ten),
            };

            var result = _KhoaPhongRepository.TableNoTracking.Where(c => c.KhoDuocPhams.Any())
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take);

            return result.ToList();
        }

        public async Task<List<LookupItemVo>> GetTatCaKhoTheoKhoaBaoCaoKiemKeKTs(DropDownListRequestModel queryInfo, long khoaId)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(Kho.Ten),
            };
            var allKhos = new List<LookupItemVo>()
            {
                new LookupItemVo {KeyId = 0 , DisplayName = "Tất cả" }
            };


            var result = khoaId != 0 ? _khoRepository.TableNoTracking.Where(c => c.KhoaPhongId == khoaId)
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               }).ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take)
               :
               _khoRepository.TableNoTracking
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take);

            allKhos.AddRange(result);

            return allKhos;
        }

        public async Task<GridDataSource> GetDataBaoCaoBienBanKiemKeKTForGridAsync(BaoCaoBienBanKiemKeKTQueryInfo queryInfo, bool exportExcel = false)
        {
            if (_nhapKhoDuocPhamChiTietRepository.Context is CaminoObjectContext context)
            {
                var allDataNhapDuocPhamQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NgayNhap <= queryInfo.ToDate);
                if (queryInfo.KhoId != 0)
                {
                    allDataNhapDuocPhamQuery = allDataNhapDuocPhamQuery.Where(o => o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId);
                }
                else if (queryInfo.KhoaPhongId != 0)
                {
                    allDataNhapDuocPhamQuery = allDataNhapDuocPhamQuery.Where(o => o.NhapKhoDuocPhams.KhoDuocPhams.KhoaPhongId == queryInfo.KhoaPhongId);
                }
                var allDataNhapDuocPham = allDataNhapDuocPhamQuery
                .GroupBy(o => new
                {
                    o.NhapKhoDuocPhams.KhoId,
                    o.DuocPhamBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                }, o => o,
                        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                        {
                            KhoId = k.KhoId,
                            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                            SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                            SoLuongXuat = 0,
                            DonGia = k.DonGiaTonKho,
                            SoLo = k.Solo,
                            HanDung = k.HanSuDung
                        }).ToList();

                //todo:
                string sqlXuatDuocPham = string.Empty;
                if (queryInfo.KhoId != 0)
                {
                    sqlXuatDuocPham =
                        $@"SELECT [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId] AS [KhoId], [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], [o.NhapKhoDuocPhamChiTiet].[DonGiaTonKho] AS [DonGia], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung] AS [HanDung]
                            FROM [XuatKhoDuocPhamChiTietViTri] AS [o]
                            INNER JOIN [NhapKhoDuocPhamChiTiet] AS [o.NhapKhoDuocPhamChiTiet] ON [o].[NhapKhoDuocPhamChiTietId] = [o.NhapKhoDuocPhamChiTiet].[Id]
                            INNER JOIN [XuatKhoDuocPhamChiTiet] AS [o.XuatKhoDuocPhamChiTiet] ON [o].[XuatKhoDuocPhamChiTietId] = [o.XuatKhoDuocPhamChiTiet].[Id]
                            LEFT JOIN [XuatKhoDuocPham] AS [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham] ON [o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] = [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[Id]
                            WHERE ([o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] IS NOT NULL AND (([o].[NgayXuat] IS NOT NULL AND ([o].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')) OR ([o].[NgayXuat] IS NULL AND ([o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')))) AND ([o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId] = {queryInfo.KhoId})
                            GROUP BY [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId], [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], [o.NhapKhoDuocPhamChiTiet].[DonGiaTonKho], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung]
                            ";
                }
                else if (queryInfo.KhoaPhongId != 0)
                {
                    sqlXuatDuocPham =
                        $@"SELECT [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId] AS [KhoId], [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], [o.NhapKhoDuocPhamChiTiet].[DonGiaTonKho] AS [DonGia], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung] AS [HanDung]
                            FROM [XuatKhoDuocPhamChiTietViTri] AS [o]
                            INNER JOIN [NhapKhoDuocPhamChiTiet] AS [o.NhapKhoDuocPhamChiTiet] ON [o].[NhapKhoDuocPhamChiTietId] = [o.NhapKhoDuocPhamChiTiet].[Id]
                            INNER JOIN [XuatKhoDuocPhamChiTiet] AS [o.XuatKhoDuocPhamChiTiet] ON [o].[XuatKhoDuocPhamChiTietId] = [o.XuatKhoDuocPhamChiTiet].[Id]
                            LEFT JOIN [XuatKhoDuocPham] AS [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham] ON [o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] = [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[Id]
                            LEFT JOIN [Kho] AS [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat] ON [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId] = [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat].[Id]
                            WHERE ([o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] IS NOT NULL AND (([o].[NgayXuat] IS NOT NULL AND ([o].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')) OR ([o].[NgayXuat] IS NULL AND ([o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')))) AND ([o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat].[KhoaPhongId] = {queryInfo.KhoaPhongId})
                            GROUP BY [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId], [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], [o.NhapKhoDuocPhamChiTiet].[DonGiaTonKho], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung]
                            ";
                }
                else
                {
                    sqlXuatDuocPham =
                        $@"SELECT [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId] AS [KhoId], [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], [o.NhapKhoDuocPhamChiTiet].[DonGiaTonKho] AS [DonGia], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung] AS [HanDung]
                            FROM [XuatKhoDuocPhamChiTietViTri] AS [o]
                            INNER JOIN [NhapKhoDuocPhamChiTiet] AS [o.NhapKhoDuocPhamChiTiet] ON [o].[NhapKhoDuocPhamChiTietId] = [o.NhapKhoDuocPhamChiTiet].[Id]
                            INNER JOIN [XuatKhoDuocPhamChiTiet] AS [o.XuatKhoDuocPhamChiTiet] ON [o].[XuatKhoDuocPhamChiTietId] = [o.XuatKhoDuocPhamChiTiet].[Id]
                            LEFT JOIN [XuatKhoDuocPham] AS [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham] ON [o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] = [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[Id]
                            WHERE ([o.XuatKhoDuocPhamChiTiet].[XuatKhoDuocPhamId] IS NOT NULL AND (([o].[NgayXuat] IS NOT NULL AND ([o].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')) OR ([o].[NgayXuat] IS NULL AND ([o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}'))))
                            GROUP BY [o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham].[KhoXuatId], [o.NhapKhoDuocPhamChiTiet].[DuocPhamBenhVienId], [o.NhapKhoDuocPhamChiTiet].[DonGiaTonKho], [o.NhapKhoDuocPhamChiTiet].[Solo], [o.NhapKhoDuocPhamChiTiet].[HanSuDung]";
                }

                var xuatDuocPhamDbQuery = context.BaoCaoBienBanKiemKeKTXuatDPDbQuery.FromSql(sqlXuatDuocPham).ToList();
                var allDataXuatDuocPham = xuatDuocPhamDbQuery.Select(o => new BaoCaoBienBanKiemKeKTGridVo
                {
                    KhoId = o.KhoId,
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    DonGia = o.DonGia,
                    SoLuongNhap = 0,
                    SoLuongXuat = o.SoLuongXuat,
                    SoLo = o.SoLo,
                    HanDung = o.HanDung
                }).ToList();


                //var allDataXuatDuocPhamQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(o =>
                //    o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                //    ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                //     (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));
                //if (queryInfo.KhoId != 0)
                //{
                //    allDataXuatDuocPhamQuery = allDataXuatDuocPhamQuery.Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId);
                //}
                //else if (queryInfo.KhoaPhongId != 0)
                //{
                //    allDataXuatDuocPhamQuery = allDataXuatDuocPhamQuery.Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.KhoaPhongId == queryInfo.KhoaPhongId);
                //}
                //var allDataXuatDuocPham = allDataXuatDuocPhamQuery
                //    .GroupBy(o => new
                //    {
                //        KhoId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId,
                //        o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                //        o.NhapKhoDuocPhamChiTiet.DonGiaTonKho,
                //        o.NhapKhoDuocPhamChiTiet.Solo,
                //        o.NhapKhoDuocPhamChiTiet.HanSuDung
                //    }, o => o,
                //        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                //        {
                //            KhoId = k.KhoId,
                //            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                //            DonGia = k.DonGiaTonKho,
                //            SoLuongNhap = 0,
                //            SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                //            SoLo = k.Solo,
                //            HanDung = k.HanSuDung
                //        }).ToList();

                //vat tu
                var allDataNhapVatTuQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NgayNhap <= queryInfo.ToDate);
                if (queryInfo.KhoId != 0)
                {
                    allDataNhapVatTuQuery = allDataNhapVatTuQuery.Where(o => o.NhapKhoVatTu.KhoId == queryInfo.KhoId);
                }
                else if (queryInfo.KhoaPhongId != 0)
                {
                    allDataNhapVatTuQuery = allDataNhapVatTuQuery.Where(o => o.NhapKhoVatTu.Kho.KhoaPhongId == queryInfo.KhoaPhongId);
                }

                var allDataNhapVatTu = allDataNhapVatTuQuery
                    .GroupBy(o => new
                    {
                        o.NhapKhoVatTu.KhoId,
                        o.VatTuBenhVienId,
                        o.DonGiaTonKho,
                        o.Solo,
                        o.HanSuDung
                    }, o => o,
                        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                        {
                            KhoId = k.KhoId,
                            VatTuBenhVienId = k.VatTuBenhVienId,
                            SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                            SoLuongXuat = 0,
                            DonGia = k.DonGiaTonKho,
                            SoLo = k.Solo,
                            HanDung = k.HanSuDung
                        }).ToList();


                //todo:
                string sqlXuatVatTu = string.Empty;
                if (queryInfo.KhoId != 0)
                {
                    sqlXuatVatTu =
                        $@"SELECT [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId] AS [KhoId], [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[DonGiaTonKho] AS [DonGia], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung] AS [HanDung]
                            FROM [XuatKhoVatTuChiTietViTri] AS [o]
                            INNER JOIN [NhapKhoVatTuChiTiet] AS [o.NhapKhoVatTuChiTiet] ON [o].[NhapKhoVatTuChiTietId] = [o.NhapKhoVatTuChiTiet].[Id]
                            INNER JOIN [XuatKhoVatTuChiTiet] AS [o.XuatKhoVatTuChiTiet] ON [o].[XuatKhoVatTuChiTietId] = [o.XuatKhoVatTuChiTiet].[Id]
                            LEFT JOIN [XuatKhoVatTu] AS [o.XuatKhoVatTuChiTiet.XuatKhoVatTu] ON [o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] = [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[Id]
                            WHERE ([o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] IS NOT NULL AND (([o].[NgayXuat] IS NOT NULL AND ([o].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')) OR ([o].[NgayXuat] IS NULL AND ([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')))) AND ([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId] = {queryInfo.KhoId})
                            GROUP BY [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId], [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[DonGiaTonKho], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung]
                            ";
                }
                else if (queryInfo.KhoaPhongId != 0)
                {
                    sqlXuatVatTu =
                        $@"SELECT [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId] AS [KhoId], [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[DonGiaTonKho] AS [DonGia], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung] AS [HanDung]
                            FROM [XuatKhoVatTuChiTietViTri] AS [o]
                            INNER JOIN [NhapKhoVatTuChiTiet] AS [o.NhapKhoVatTuChiTiet] ON [o].[NhapKhoVatTuChiTietId] = [o.NhapKhoVatTuChiTiet].[Id]
                            INNER JOIN [XuatKhoVatTuChiTiet] AS [o.XuatKhoVatTuChiTiet] ON [o].[XuatKhoVatTuChiTietId] = [o.XuatKhoVatTuChiTiet].[Id]
                            LEFT JOIN [XuatKhoVatTu] AS [o.XuatKhoVatTuChiTiet.XuatKhoVatTu] ON [o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] = [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[Id]
                            LEFT JOIN [Kho] AS [o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat] ON [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId] = [o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat].[Id]
                            WHERE ([o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] IS NOT NULL AND (([o].[NgayXuat] IS NOT NULL AND ([o].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')) OR ([o].[NgayXuat] IS NULL AND ([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')))) AND ([o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat].[KhoaPhongId] = {queryInfo.KhoaPhongId})
                            GROUP BY [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId], [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[DonGiaTonKho], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung]
                            ";
                }
                else
                {
                    sqlXuatVatTu =
                        $@"SELECT [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId] AS [KhoId], [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[DonGiaTonKho] AS [DonGia], SUM([o].[SoLuongXuat]) AS [SoLuongXuat], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung] AS [HanDung]
                            FROM [XuatKhoVatTuChiTietViTri] AS [o]
                            INNER JOIN [NhapKhoVatTuChiTiet] AS [o.NhapKhoVatTuChiTiet] ON [o].[NhapKhoVatTuChiTietId] = [o.NhapKhoVatTuChiTiet].[Id]
                            INNER JOIN [XuatKhoVatTuChiTiet] AS [o.XuatKhoVatTuChiTiet] ON [o].[XuatKhoVatTuChiTietId] = [o.XuatKhoVatTuChiTiet].[Id]
                            LEFT JOIN [XuatKhoVatTu] AS [o.XuatKhoVatTuChiTiet.XuatKhoVatTu] ON [o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] = [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[Id]
                            WHERE ([o.XuatKhoVatTuChiTiet].[XuatKhoVatTuId] IS NOT NULL AND (([o].[NgayXuat] IS NOT NULL AND ([o].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}')) OR ([o].[NgayXuat] IS NULL AND ([o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[NgayXuat] <= '{queryInfo.ToDate.ToString("yyyy-MM-dd HH:mm:ss")}'))))
                            GROUP BY [o.XuatKhoVatTuChiTiet.XuatKhoVatTu].[KhoXuatId], [o.NhapKhoVatTuChiTiet].[VatTuBenhVienId], [o.NhapKhoVatTuChiTiet].[DonGiaTonKho], [o.NhapKhoVatTuChiTiet].[Solo], [o.NhapKhoVatTuChiTiet].[HanSuDung]
                            ";
                }


                var xuatVatTuDbQuery = context.BaoCaoBienBanKiemKeKTXuatVTDbQuery.FromSql(sqlXuatVatTu).ToList();
                var allDataXuatVatTu = xuatVatTuDbQuery.Select(o => new BaoCaoBienBanKiemKeKTGridVo
                {
                    KhoId = o.KhoId,
                    VatTuBenhVienId = o.VatTuBenhVienId,
                    SoLuongNhap = 0,
                    SoLuongXuat = o.SoLuongXuat,
                    DonGia = o.DonGia,
                    SoLo = o.SoLo,
                    HanDung = o.HanDung
                }).ToList();

                //var allDataXuatVatTuQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                //                                                                                           ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                //                                                                                            (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));
                //if (queryInfo.KhoId != 0)
                //{
                //    allDataXuatVatTuQuery = allDataXuatVatTuQuery.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId);
                //}
                //else if (queryInfo.KhoaPhongId != 0)
                //{
                //    allDataXuatVatTuQuery = allDataXuatVatTuQuery.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.KhoaPhongId == queryInfo.KhoaPhongId);
                //}
                //var allDataXuatVatTu = allDataXuatVatTuQuery
                //    .GroupBy(o => new
                //    {
                //        KhoId = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId,
                //        o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                //        o.NhapKhoVatTuChiTiet.DonGiaTonKho,
                //        o.NhapKhoVatTuChiTiet.Solo,
                //        o.NhapKhoVatTuChiTiet.HanSuDung
                //    }, o => o,
                //        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                //        {
                //            KhoId = k.KhoId,
                //            VatTuBenhVienId = k.VatTuBenhVienId,
                //            DonGia = k.DonGiaTonKho,
                //            SoLuongNhap = 0,
                //            SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                //            SoLo = k.Solo,
                //            HanDung = k.HanSuDung
                //        }).ToList();



                var allData = allDataNhapDuocPham.Concat(allDataXuatDuocPham).Concat(allDataNhapVatTu).Concat(allDataXuatVatTu);
                var returnData = allData.GroupBy(o => new
                {
                    o.KhoId,
                    o.DuocPhamBenhVienId,
                    o.VatTuBenhVienId,
                    o.DonGia,
                    o.SoLo,
                    o.HanDung,
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                    {
                        KhoId = k.KhoId,
                        DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        SLSoSach = v.Select(x => x.SoLuongNhap - x.SoLuongXuat).DefaultIfEmpty().Sum().MathRoundNumber(2),
                        DonGia = k.DonGia,
                        SoLo = k.SoLo,
                        HanDung = k.HanDung
                    }
                ).Where(o => !o.SLSoSach.AlmostEqual(0))
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

                foreach (var baoCaoBienBanKiemKeKTGridVo in returnData)
                {
                    baoCaoBienBanKiemKeKTGridVo.Kho = thongTinKho.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.KhoId)?.Ten;
                    if (baoCaoBienBanKiemKeKTGridVo.DuocPhamBenhVienId != 0)
                    {
                        var dp = thongTinDuocPham.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.DuocPhamBenhVienId);
                        baoCaoBienBanKiemKeKTGridVo.NhomVatTuName = dp?.Nhom;
                        baoCaoBienBanKiemKeKTGridVo.DonVi = dp?.DVT;
                        baoCaoBienBanKiemKeKTGridVo.TenVatTu = dp?.Ten;
                        baoCaoBienBanKiemKeKTGridVo.MaVatTu = dp?.Ma;
                    }
                    else if (baoCaoBienBanKiemKeKTGridVo.VatTuBenhVienId != 0)
                    {
                        var vt = thongTinVatTu.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.VatTuBenhVienId);
                        baoCaoBienBanKiemKeKTGridVo.NhomVatTuName = vt?.Nhom;
                        baoCaoBienBanKiemKeKTGridVo.DonVi = vt?.DVT;
                        baoCaoBienBanKiemKeKTGridVo.TenVatTu = vt?.Ten;
                        baoCaoBienBanKiemKeKTGridVo.MaVatTu = vt?.Ma;
                    }
                }
                var returnDataOrderly = returnData.OrderBy(o => o.KhoId).ThenBy(o => o.NhomVatTuName).ThenBy(o => o.MaVatTu);
                return new GridDataSource { Data = exportExcel ? returnDataOrderly.ToArray() : returnDataOrderly.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = returnData.Count() };
            }
            return null;
        }

        public async Task<GridDataSource> GetDataBaoCaoBienBanKiemKeKTDP09282021ForGridAsync(BaoCaoBienBanKiemKeKTQueryInfo queryInfo)
        {

            var allDataNhapDuocPhamQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NgayNhap <= queryInfo.ToDate);
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataNhapDuocPhamQuery = allDataNhapDuocPhamQuery.Where(o => o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataNhapDuocPhamQuery = allDataNhapDuocPhamQuery.Where(o => o.NhapKhoDuocPhams.KhoDuocPhams.KhoaPhongId == queryInfo.KhoaPhongId);
            //}
            var allDataNhapDuocPham = allDataNhapDuocPhamQuery
            .GroupBy(o => new
            {
                o.NhapKhoDuocPhams.KhoId,
                o.DuocPhamBenhVienId,
                o.DonGiaTonKho,
                o.DonGiaNhap,
                o.DonGiaBan,
                o.LaDuocPhamBHYT,
                o.Solo,
                o.HanSuDung,
            }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                    {
                        KhoId = k.KhoId,
                        DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                        SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                        SoLuongXuat = 0,
                        DonGia = k.DonGiaTonKho,
                        DonGiaNhap = k.DonGiaNhap,
                        DonGiaBan = k.DonGiaBan,
                        CoBHYT = k.LaDuocPhamBHYT,
                        SoLo = k.Solo,
                        HanDung = k.HanSuDung
                    }).ToList();

            var allDataXuatDuocPhamQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(o =>
                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                 (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataXuatDuocPhamQuery = allDataXuatDuocPhamQuery.Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataXuatDuocPhamQuery = allDataXuatDuocPhamQuery.Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.KhoaPhongId == queryInfo.KhoaPhongId);
            //}
            var allDataXuatDuocPham = allDataXuatDuocPhamQuery
                .GroupBy(o => new
                {
                    KhoId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId,
                    o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    o.NhapKhoDuocPhamChiTiet.DonGiaTonKho,
                    o.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                    o.NhapKhoDuocPhamChiTiet.DonGiaBan,
                    o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    o.NhapKhoDuocPhamChiTiet.Solo,
                    o.NhapKhoDuocPhamChiTiet.HanSuDung
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                    {
                        KhoId = k.KhoId,
                        DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                        DonGia = k.DonGiaTonKho,
                        DonGiaNhap = k.DonGiaNhap,
                        DonGiaBan = k.DonGiaBan,
                        CoBHYT = k.LaDuocPhamBHYT,
                        SoLuongNhap = 0,
                        SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                        SoLo = k.Solo,
                        HanDung = k.HanSuDung
                    }).ToList();

            //vat tu
            //var allDataNhapVatTuQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NgayNhap <= queryInfo.ToDate);
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataNhapVatTuQuery = allDataNhapVatTuQuery.Where(o => o.NhapKhoVatTu.KhoId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataNhapVatTuQuery = allDataNhapVatTuQuery.Where(o => o.NhapKhoVatTu.Kho.KhoaPhongId == queryInfo.KhoaPhongId);
            //}

            //var allDataNhapVatTu = allDataNhapVatTuQuery
            //    .GroupBy(o => new
            //    {
            //        o.NhapKhoVatTu.KhoId,
            //        o.VatTuBenhVienId,
            //        o.DonGiaTonKho,
            //        o.Solo,
            //        o.HanSuDung
            //    }, o => o,
            //        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
            //        {
            //            KhoId = k.KhoId,
            //            VatTuBenhVienId = k.VatTuBenhVienId,
            //            SoLuongNhap = v.Sum(x => x.SoLuongNhap),
            //            SoLuongXuat = 0,
            //            DonGia = k.DonGiaTonKho,
            //            SoLo = k.Solo,
            //            HanDung = k.HanSuDung
            //        }).ToList();

            //var allDataXuatVatTuQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
            //                                                                                           ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
            //                                                                                            (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataXuatVatTuQuery = allDataXuatVatTuQuery.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataXuatVatTuQuery = allDataXuatVatTuQuery.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.KhoaPhongId == queryInfo.KhoaPhongId);
            //}
            //var allDataXuatVatTu = allDataXuatVatTuQuery
            //    .GroupBy(o => new
            //    {
            //        KhoId = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId,
            //        o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
            //        o.NhapKhoVatTuChiTiet.DonGiaTonKho,
            //        o.NhapKhoVatTuChiTiet.Solo,
            //        o.NhapKhoVatTuChiTiet.HanSuDung
            //    }, o => o,
            //        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
            //        {
            //            KhoId = k.KhoId,
            //            VatTuBenhVienId = k.VatTuBenhVienId,
            //            DonGia = k.DonGiaTonKho,
            //            SoLuongNhap = 0,
            //            SoLuongXuat = v.Sum(x => x.SoLuongXuat),
            //            SoLo = k.Solo,
            //            HanDung = k.HanSuDung
            //        }).ToList();

            //var allData = allDataNhapDuocPham.Concat(allDataXuatDuocPham).Concat(allDataNhapVatTu).Concat(allDataXuatVatTu);
            var allData = allDataNhapDuocPham.Concat(allDataXuatDuocPham);
            var returnData = allData.GroupBy(o => new
            {
                o.KhoId,
                o.DuocPhamBenhVienId,
                o.VatTuBenhVienId,
                o.DonGia,
                o.DonGiaNhap,
                o.DonGiaBan,
                o.CoBHYT,
                o.SoLo,
                o.HanDung,
            }, o => o,
                (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                {
                    KhoId = k.KhoId,
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    VatTuBenhVienId = k.VatTuBenhVienId,
                    SLSoSach = v.Select(x => x.SoLuongNhap - x.SoLuongXuat).DefaultIfEmpty().Sum(),
                    DonGia = k.DonGia,
                    DonGiaNhap = k.DonGiaNhap,
                    DonGiaBan = k.DonGiaBan,
                    CoBHYT = k.CoBHYT,
                    SoLo = k.SoLo,
                    HanDung = k.HanDung
                }
            ).Where(o => !o.SLSoSach.AlmostEqual(0))
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

            foreach (var baoCaoBienBanKiemKeKTGridVo in returnData)
            {
                baoCaoBienBanKiemKeKTGridVo.Kho = thongTinKho.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.KhoId)?.Ten;
                if (baoCaoBienBanKiemKeKTGridVo.DuocPhamBenhVienId != 0)
                {
                    var dp = thongTinDuocPham.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.DuocPhamBenhVienId);
                    baoCaoBienBanKiemKeKTGridVo.NhomVatTuName = dp?.Nhom;
                    baoCaoBienBanKiemKeKTGridVo.DonVi = dp?.DVT;
                    baoCaoBienBanKiemKeKTGridVo.TenVatTu = dp?.Ten;
                    baoCaoBienBanKiemKeKTGridVo.MaVatTu = dp?.Ma;
                }
                else if (baoCaoBienBanKiemKeKTGridVo.VatTuBenhVienId != 0)
                {
                    var vt = thongTinVatTu.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.VatTuBenhVienId);
                    baoCaoBienBanKiemKeKTGridVo.NhomVatTuName = vt?.Nhom;
                    baoCaoBienBanKiemKeKTGridVo.DonVi = vt?.DVT;
                    baoCaoBienBanKiemKeKTGridVo.TenVatTu = vt?.Ten;
                    baoCaoBienBanKiemKeKTGridVo.MaVatTu = vt?.Ma;
                }
            }
            var returnDataOrderly = returnData.OrderBy(o => o.KhoId).ThenBy(o => o.NhomVatTuName).ThenBy(o => o.MaVatTu);
            return new GridDataSource { Data = returnDataOrderly.ToArray(), TotalRowCount = returnData.Count() };
        }

        public async Task<GridDataSource> GetDataBaoCaoBienBanKiemKeKTVT09282021ForGridAsync(BaoCaoBienBanKiemKeKTQueryInfo queryInfo)
        {
            //var allDataNhapDuocPhamQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NgayNhap <= queryInfo.ToDate);
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataNhapDuocPhamQuery = allDataNhapDuocPhamQuery.Where(o => o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataNhapDuocPhamQuery = allDataNhapDuocPhamQuery.Where(o => o.NhapKhoDuocPhams.KhoDuocPhams.KhoaPhongId == queryInfo.KhoaPhongId);
            //}
            //var allDataNhapDuocPham = allDataNhapDuocPhamQuery
            //.GroupBy(o => new
            //{
            //    o.NhapKhoDuocPhams.KhoId,
            //    o.DuocPhamBenhVienId,
            //    o.DonGiaTonKho,
            //    o.DonGiaNhap,
            //    o.DonGiaBan,
            //    o.LaDuocPhamBHYT,
            //    o.Solo,
            //    o.HanSuDung,
            //}, o => o,
            //        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
            //        {
            //            KhoId = k.KhoId,
            //            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
            //            SoLuongNhap = v.Sum(x => x.SoLuongNhap),
            //            SoLuongXuat = 0,
            //            DonGia = k.DonGiaTonKho,
            //            DonGiaNhap = k.DonGiaNhap,
            //            DonGiaBan = k.DonGiaBan,
            //            CoBHYT = k.LaDuocPhamBHYT,
            //            SoLo = k.Solo,
            //            HanDung = k.HanSuDung
            //        }).ToList();

            //var allDataXuatDuocPhamQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(o =>
            //    o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
            //    ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
            //     (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataXuatDuocPhamQuery = allDataXuatDuocPhamQuery.Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataXuatDuocPhamQuery = allDataXuatDuocPhamQuery.Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.KhoaPhongId == queryInfo.KhoaPhongId);
            //}
            //var allDataXuatDuocPham = allDataXuatDuocPhamQuery
            //    .GroupBy(o => new
            //    {
            //        KhoId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId,
            //        o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
            //        o.NhapKhoDuocPhamChiTiet.DonGiaTonKho,
            //        o.NhapKhoDuocPhamChiTiet.DonGiaNhap,
            //        o.NhapKhoDuocPhamChiTiet.DonGiaBan,
            //        o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
            //        o.NhapKhoDuocPhamChiTiet.Solo,
            //        o.NhapKhoDuocPhamChiTiet.HanSuDung
            //    }, o => o,
            //        (k, v) => new BaoCaoBienBanKiemKeKTGridVo
            //        {
            //            KhoId = k.KhoId,
            //            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
            //            DonGia = k.DonGiaTonKho,
            //            DonGiaNhap = k.DonGiaNhap,
            //            DonGiaBan = k.DonGiaBan,
            //            CoBHYT = k.LaDuocPhamBHYT,
            //            SoLuongNhap = 0,
            //            SoLuongXuat = v.Sum(x => x.SoLuongXuat),
            //            SoLo = k.Solo,
            //            HanDung = k.HanSuDung
            //        }).ToList();

            //vat tu
            var allDataNhapVatTuQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NgayNhap <= queryInfo.ToDate);
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataNhapVatTuQuery = allDataNhapVatTuQuery.Where(o => o.NhapKhoVatTu.KhoId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataNhapVatTuQuery = allDataNhapVatTuQuery.Where(o => o.NhapKhoVatTu.Kho.KhoaPhongId == queryInfo.KhoaPhongId);
            //}

            var allDataNhapVatTu = allDataNhapVatTuQuery
                .GroupBy(o => new
                {
                    o.NhapKhoVatTu.KhoId,
                    o.VatTuBenhVienId,
                    o.DonGiaTonKho,
                    o.DonGiaNhap,
                    o.DonGiaBan,
                    o.LaVatTuBHYT,
                    o.Solo,
                    o.HanSuDung
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                    {
                        KhoId = k.KhoId,
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        SoLuongNhap = v.Sum(x => x.SoLuongNhap),
                        SoLuongXuat = 0,
                        DonGia = k.DonGiaTonKho,
                        DonGiaNhap = k.DonGiaNhap,
                        DonGiaBan = k.DonGiaBan,
                        CoBHYT = k.LaVatTuBHYT,
                        SoLo = k.Solo,
                        HanDung = k.HanSuDung
                    }).ToList();

            var allDataXuatVatTuQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                                                                                       ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                                                                                        (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));
            //if (queryInfo.KhoId != 0)
            //{
            //    allDataXuatVatTuQuery = allDataXuatVatTuQuery.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId);
            //}
            //else if (queryInfo.KhoaPhongId != 0)
            //{
            //    allDataXuatVatTuQuery = allDataXuatVatTuQuery.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.KhoaPhongId == queryInfo.KhoaPhongId);
            //}
            var allDataXuatVatTu = allDataXuatVatTuQuery
                .GroupBy(o => new
                {
                    KhoId = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId,
                    o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                    o.NhapKhoVatTuChiTiet.DonGiaTonKho,
                    o.NhapKhoVatTuChiTiet.DonGiaNhap,
                    o.NhapKhoVatTuChiTiet.DonGiaBan,
                    o.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    o.NhapKhoVatTuChiTiet.Solo,
                    o.NhapKhoVatTuChiTiet.HanSuDung
                }, o => o,
                    (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                    {
                        KhoId = k.KhoId,
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        DonGia = k.DonGiaTonKho,
                        DonGiaNhap = k.DonGiaNhap,
                        DonGiaBan = k.DonGiaBan,
                        CoBHYT = k.LaVatTuBHYT,
                        SoLuongNhap = 0,
                        SoLuongXuat = v.Sum(x => x.SoLuongXuat),
                        SoLo = k.Solo,
                        HanDung = k.HanSuDung
                    }).ToList();

            //var allData = allDataNhapDuocPham.Concat(allDataXuatDuocPham).Concat(allDataNhapVatTu).Concat(allDataXuatVatTu);
            var allData = allDataNhapVatTu.Concat(allDataXuatVatTu);
            var returnData = allData.GroupBy(o => new
            {
                o.KhoId,
                o.DuocPhamBenhVienId,
                o.VatTuBenhVienId,
                o.DonGia,
                o.DonGiaNhap,
                o.DonGiaBan,
                o.CoBHYT,
                o.SoLo,
                o.HanDung,
            }, o => o,
                (k, v) => new BaoCaoBienBanKiemKeKTGridVo
                {
                    KhoId = k.KhoId,
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    VatTuBenhVienId = k.VatTuBenhVienId,
                    SLSoSach = v.Select(x => x.SoLuongNhap - x.SoLuongXuat).DefaultIfEmpty().Sum(),
                    DonGia = k.DonGia,
                    DonGiaNhap = k.DonGiaNhap,
                    DonGiaBan = k.DonGiaBan,
                    CoBHYT = k.CoBHYT,
                    SoLo = k.SoLo,
                    HanDung = k.HanDung
                }
            ).Where(o => !o.SLSoSach.AlmostEqual(0))
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

            foreach (var baoCaoBienBanKiemKeKTGridVo in returnData)
            {
                baoCaoBienBanKiemKeKTGridVo.Kho = thongTinKho.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.KhoId)?.Ten;
                if (baoCaoBienBanKiemKeKTGridVo.DuocPhamBenhVienId != 0)
                {
                    var dp = thongTinDuocPham.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.DuocPhamBenhVienId);
                    baoCaoBienBanKiemKeKTGridVo.NhomVatTuName = dp?.Nhom;
                    baoCaoBienBanKiemKeKTGridVo.DonVi = dp?.DVT;
                    baoCaoBienBanKiemKeKTGridVo.TenVatTu = dp?.Ten;
                    baoCaoBienBanKiemKeKTGridVo.MaVatTu = dp?.Ma;
                }
                else if (baoCaoBienBanKiemKeKTGridVo.VatTuBenhVienId != 0)
                {
                    var vt = thongTinVatTu.FirstOrDefault(o => o.Id == baoCaoBienBanKiemKeKTGridVo.VatTuBenhVienId);
                    baoCaoBienBanKiemKeKTGridVo.NhomVatTuName = vt?.Nhom;
                    baoCaoBienBanKiemKeKTGridVo.DonVi = vt?.DVT;
                    baoCaoBienBanKiemKeKTGridVo.TenVatTu = vt?.Ten;
                    baoCaoBienBanKiemKeKTGridVo.MaVatTu = vt?.Ma;
                }
            }
            var returnDataOrderly = returnData.OrderBy(o => o.KhoId).ThenBy(o => o.NhomVatTuName).ThenBy(o => o.MaVatTu);
            return new GridDataSource { Data = returnDataOrderly.ToArray(), TotalRowCount = returnData.Count() };
        }

        public virtual byte[] ExportBaoCaoBienBanKiemKeKTGridVo(GridDataSource gridDataSource, BaoCaoBienBanKiemKeKTQueryInfo query)
        {
            var datas = (ICollection<BaoCaoBienBanKiemKeKTGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoBienBanKiemKeKTGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO BIÊN BẢN KIỂM KÊ");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.DefaultColWidth = 7;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:D1"])
                    {
                        range.Worksheet.Cells["A1:D1"].Merge = true;
                        range.Worksheet.Cells["A1:D1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:D1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "BIÊN BẢN KIỂM KÊ";
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:K4"])
                    {
                        range.Worksheet.Cells["A4:K4"].Merge = true;
                        //range.Worksheet.Cells["A4:J4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                        //                             + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:K4"].Value = "Thời gian từ............... giờ............... phút ngày...............tháng...............năm..............." +
                            "đến............... giờ............... phút ngày...............tháng...............năm...............";
                        range.Worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);      
                    }

                    using (var range = worksheet.Cells["A6:K6"])
                    {
                        range.Worksheet.Cells["A6:K6"].Merge = true;
                        range.Worksheet.Cells["A6:K6"].Value = "-Tổ kiếm kê gồm có:";
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:K6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:K7"])
                    {
                        range.Worksheet.Cells["A7:K7"].Merge = true;
                        range.Worksheet.Cells["A7:K7"].Value = "1.......................................Khoa/Phòng:.............................";
                        range.Worksheet.Cells["A7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:K7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A7:K7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:K7"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A8:K8"])
                    {
                        range.Worksheet.Cells["A8:K8"].Merge = true;
                        range.Worksheet.Cells["A8:K8"].Value = "2.......................................Khoa/Phòng:.............................";
                        range.Worksheet.Cells["A8:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A8:K8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A8:K8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var tenKho = _khoRepository.TableNoTracking.Where(c => c.Id == query.KhoId).Select(c => c.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A9:K9"])
                    {
                        range.Worksheet.Cells["A9:K9"].Merge = true;

                        range.Worksheet.Cells["A9:K9"].Merge = true;
                        range.Worksheet.Cells["A9:K9"].Style.WrapText = true;
                        var txtValue =  tenKho == null ? "Tất cả kho" :  tenKho; 

                        var title = range.Worksheet.Cells["A9:K9"].RichText.Add("-Kho kiểm kê: ");
                        var value = range.Worksheet.Cells["A9:K9"].RichText.Add(txtValue);
                        value.Bold = true;

               
                        range.Worksheet.Cells["A9:K9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:K9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A9:K9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A9:K9"].Style.Font.Color.SetColor(Color.Black);

                        //range.Worksheet.Cells["A9:K9"].Style.Font.Bold = true;
                    }

                    //header
                    using (var range = worksheet.Cells["A12:K13"])
                    {
                        range.Worksheet.Cells["A12:K13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A12:K13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:K13"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A12:K13"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A12:K13"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A12:K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A13:K13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A13:K13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A13:K13"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A13:K13"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A13:K13"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A13:K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A12:A13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A12:A13"].Merge = true;
                        range.Worksheet.Cells["A12:A13"].Value = "STT";

                        range.Worksheet.Cells["B12:B13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B12:B13"].Merge = true;
                        range.Worksheet.Cells["B12:B13"].Value = "Mã dược/ Mã VTYT";

                        range.Worksheet.Cells["C12:C13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C12:C13"].Merge = true;
                        range.Worksheet.Cells["C12:C13"].Value = "Tên dược, hàm lượng/ VTYT/Hoá chất";

                        range.Worksheet.Cells["D12:D13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D12:D13"].Merge = true;
                        range.Worksheet.Cells["D12:D13"].Value = "ĐVT";

                        range.Worksheet.Cells["E12:E13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E12:E13"].Merge = true;
                        range.Worksheet.Cells["E12:E13"].Value = "Lô";

                        range.Worksheet.Cells["F12:F13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F12:F13"].Merge = true;
                        range.Worksheet.Cells["F12:F13"].Value = "Hạn dùng";                       

                        range.Worksheet.Cells["G12:G13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G12:G13"].Merge = true;
                        range.Worksheet.Cells["G12:G13"].Value = "Đơn giá";

                        range.Worksheet.Cells["H12:I12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H12:I12"].Merge = true;
                        range.Worksheet.Cells["H12:I12"].Value = "Số lượng";

                        range.Worksheet.Cells["H13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H13"].Value = "Sổ sách";
                        range.Worksheet.Cells["I13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I13"].Value = "Thực tế";

                        range.Worksheet.Cells["J12:J13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J12:J13"].Merge = true;
                        range.Worksheet.Cells["J12:J13"].Value = "SL Hỏng vỡ";

                        range.Worksheet.Cells["K12:K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K12:K13"].Merge = true;
                        range.Worksheet.Cells["K12:K13"].Value = "Ghi chú";
                    }

                    //write data from line 12
                    int index = 14;

                    //var dataTheoKho = datas.GroupBy(x => x.KhoId).Select(x => x.Key);
                    //var dataTheoNhomVT = datas.GroupBy(x => x.NhomVatTuName).Select(x => x.Key);


                    var lstKhoa = datas.GroupBy(s => new { Khoa = s.Kho }).Select(s => new KhoaGroupBaoCaoKTNhapXuatTonChiTietVo
                    {
                        Khoa = s.First().Kho
                    }).OrderBy(p => p.Khoa).ToList();

                    var listNhom = datas.GroupBy(s => new { Khoa = s.Kho, s.NhomVatTuName }).Select(s => new NhomGroupBaoCaoKTNhapXuatTonChiTietVo
                    {
                        Khoa = s.First().Kho,
                        Nhom = s.First().NhomVatTuName
                    }).OrderBy(p => p.Nhom).ToList();



                    var stt = 1;
                    if (lstKhoa.Any())
                    {
                        foreach (var khoa in lstKhoa)
                        {
                            var lstNhomTheoKhoa = listNhom.Where(o => o.Khoa == khoa.Khoa).Select(s => s.Nhom).ToList();
                            if (lstNhomTheoKhoa.Any())
                            {
                                using (var range = worksheet.Cells["A" + index])
                                {
                                    range.Worksheet.Cells["A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["A" + index].Value = khoa.Khoa;
                                }

                                index++;

                                if (datas.Any())
                                {
                                    foreach (var data in lstNhomTheoKhoa)
                                    {
                                        var listDataTheoNhomVT = datas.Where(x => x.NhomVatTuName == data && x.Kho == khoa.Khoa).ToList();
                                        if (listDataTheoNhomVT.Any())
                                        {
                                            //loai HC
                                            worksheet.Cells["A" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                            worksheet.Cells["A" + index + ":K" + index].Merge = true;
                                            worksheet.Cells["A" + index + ":K" + index].Style.Font.Bold = true;
                                            worksheet.Cells["A" + index + ":K" + index].Value = listDataTheoNhomVT.FirstOrDefault().NhomVatTuName;
                                            index++;

                                            foreach (var item in listDataTheoNhomVT)
                                            {
                                                // format border, font chữ,....
                                                worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                                worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                                worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                                                worksheet.Row(index).Height = 20.5;

                                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A" + index].Value = stt;

                                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["B" + index].Value = item.MaVatTu;

                                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["C" + index].Value = item.TenVatTu;

                                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["D" + index].Value = item.DonVi;

                                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["E" + index].Value = item.SoLo;

                                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["F" + index].Value = item.HanDungDisplay;

                                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["G" + index].Value = item.DonGia != 0 ? item.DonGia : (decimal?)null; ;

                                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["H" + index].Value = item.SLSoSach;

                                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                //worksheet.Cells["I" + index].Value = item.SLThucTe;

                                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["J" + index].Value = item.TinhTrangHuTon;

                                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["K" + index].Value = item.GhiChu;
                                                stt++;
                                                index++;
                                            }
                                        }
                                    }
                                }

                                index++;
                            }

                        }
                    }

                    index++;
                    worksheet.Cells["A" + index].Merge = true;
                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index].Value = "Ý kiến đề xuất";
                    index++;


                    var space = "....................................................................................................................................................................................................................................................................................................................................................................................................................................";
                    using (var range = worksheet.Cells["A" + index + ":K" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":K" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":K" + index].Value = space;
                        index++;
                    }

                    using (var range = worksheet.Cells["A" + index + ":K" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":K" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":K" + index].Value = space;
                        index++;
                    }

                    using (var range = worksheet.Cells["A" + index + ":K" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":K" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":K" + index].Value = space;
                        index += 2;
                    }

                    var dateNow = DateTime.Now;
                    using (var range = worksheet.Cells["H" + index + ":K" + index])
                    {
                        range.Worksheet.Cells["H" + index + ":K" + index].Merge = true;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H" + index + ":K" + index].Value = $"Ngày {dateNow.Day} tháng {dateNow.Month} năm {dateNow.Year}";
                        index++;
                    }

                    worksheet.Cells["B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["B" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["B" + index].Style.Font.Bold = true;
                    worksheet.Cells["B" + index].Value = "P.Tài chính kế toán";

                    using (var range = worksheet.Cells["C" + index + ":D" + index])
                    {
                        range.Worksheet.Cells["C" + index + ":D" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["C" + index + ":D" + index].Value = "Người lập";
                    }

                    using (var range = worksheet.Cells["E" + index + ":G" + index])
                    {
                        range.Worksheet.Cells["E" + index + ":G" + index].Merge = true;
                        range.Worksheet.Cells["E" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["E" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E" + index + ":G" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["E" + index + ":G" + index].Value = "Thủ kho";
                    }

                    using (var range = worksheet.Cells["H" + index + ":K" + index])
                    {
                        range.Worksheet.Cells["H" + index + ":K" + index].Merge = true;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["H" + index + ":K" + index].Value = "Trưởng khoa phòng";
                        index++;
                    }

                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["B" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["B" + index].Style.Font.Italic = true;
                    worksheet.Cells["B" + index].Value = "(Ký, ghi rõ họ tên)";

                    using (var range = worksheet.Cells["C" + index + ":D" + index])
                    {
                        range.Worksheet.Cells["C" + index + ":D" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Italic = true;
                        range.Worksheet.Cells["C" + index + ":D" + index].Value = "(Ký, ghi rõ họ tên)";
                    }

                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["F" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["F" + index].Style.Font.Italic = true;
                    worksheet.Cells["F" + index].Value = "(Ký, ghi rõ họ tên)";

                    using (var range = worksheet.Cells["H" + index + ":K" + index])
                    {
                        range.Worksheet.Cells["H" + index + ":K" + index].Merge = true;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H" + index + ":K" + index].Style.Font.Italic = true;
                        range.Worksheet.Cells["H" + index + ":K" + index].Value = "(Ký, ghi rõ họ tên)";
                    }


                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        public virtual byte[] ExportBaoCaoBienBanKiemKe28092021GridVo(BaoCaoBienBanKiemKeKTQueryInfo query)
        {
            var gridData = GetDataBaoCaoBienBanKiemKeKTDP09282021ForGridAsync(query).Result;

            var datas = (ICollection<BaoCaoBienBanKiemKeKTGridVo>)gridData.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoBienBanKiemKeKTGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO BIÊN BẢN KIỂM KÊ");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 20;
                    worksheet.DefaultColWidth = 7;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:D1"])
                    {
                        range.Worksheet.Cells["A1:D1"].Merge = true;
                        range.Worksheet.Cells["A1:D1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:D1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "BIÊN BẢN KIỂM KÊ";
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:K4"])
                    {
                        range.Worksheet.Cells["A4:K4"].Merge = true;
                        //range.Worksheet.Cells["A4:J4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                        //                             + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:K4"].Value = "Thời gian từ ... giờ ... phút... ngày...tháng...năm đến ... giờ ... phút... ngày...tháng...năm ";
                        range.Worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:K4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:K6"])
                    {
                        range.Worksheet.Cells["A6:K6"].Merge = true;
                        range.Worksheet.Cells["A6:K6"].Value = "-Tổ kiếm kê gồm có:";
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:K6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:K7"])
                    {
                        range.Worksheet.Cells["A7:K7"].Merge = true;
                        range.Worksheet.Cells["A7:K7"].Value = "1.......................................Khoa/Phòng:.............................";
                        range.Worksheet.Cells["A7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:K7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A7:K7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:K7"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A8:K8"])
                    {
                        range.Worksheet.Cells["A8:K8"].Merge = true;
                        range.Worksheet.Cells["A8:K8"].Value = "2.......................................Khoa/Phòng:.............................";
                        range.Worksheet.Cells["A8:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A8:K8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A8:K8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var tenKho = _khoRepository.TableNoTracking.Where(c => c.Id == query.KhoId).Select(c => c.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A9:K9"])
                    {
                        range.Worksheet.Cells["A9:K9"].Merge = true;
                        range.Worksheet.Cells["A9:K9"].Value = tenKho == null ? "-Kho kiểm kê: Tất cả kho" : "-Kho kiểm kê:" + tenKho;
                        range.Worksheet.Cells["A9:K9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:K9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A9:K9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        range.Worksheet.Cells["A9:K9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A9:K9"].Style.Font.Bold = true;
                    }

                    //header
                    using (var range = worksheet.Cells["A12:K13"])
                    {
                        range.Worksheet.Cells["A12:K13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A12:K13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:K13"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A12:K13"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A12:K13"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A12:K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A13:K13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A13:K13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A13:K13"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A13:K13"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A13:K13"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A13:K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A12:A13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A12:A13"].Merge = true;
                        range.Worksheet.Cells["A12:A13"].Value = "STT";

                        range.Worksheet.Cells["B12:B13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B12:B13"].Merge = true;
                        range.Worksheet.Cells["B12:B13"].Value = "Mã dược/ Mã VTYT";

                        range.Worksheet.Cells["C12:C13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C12:C13"].Merge = true;
                        range.Worksheet.Cells["C12:C13"].Value = "Tên dược, hàm lượng/ VTYT/Hoá chất";

                        range.Worksheet.Cells["D12:D13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D12:D13"].Merge = true;
                        range.Worksheet.Cells["D12:D13"].Value = "ĐVT";

                        range.Worksheet.Cells["E12:E13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E12:E13"].Merge = true;
                        range.Worksheet.Cells["E12:E13"].Value = "Lô";

                        range.Worksheet.Cells["F12:F13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F12:F13"].Merge = true;
                        range.Worksheet.Cells["F12:F13"].Value = "Hạn dùng";

                        range.Worksheet.Cells["G12:G13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G12:G13"].Merge = true;
                        range.Worksheet.Cells["G12:G13"].Value = "BHYT";

                        range.Worksheet.Cells["H12:H13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H12:H13"].Merge = true;
                        range.Worksheet.Cells["H12:H13"].Value = "Đơn giá";//G=>H

                        range.Worksheet.Cells["I12:I13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I12:I13"].Merge = true;
                        range.Worksheet.Cells["I12:I13"].Value = "Đơn giá nhập";//I
                        range.Worksheet.Cells["J12:J13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J12:J13"].Merge = true;
                        range.Worksheet.Cells["J12:J13"].Value = "Đơn giá bán";//J

                        range.Worksheet.Cells["K12:L12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K12:L12"].Merge = true;
                        range.Worksheet.Cells["K12:L12"].Value = "Số lượng";

                        range.Worksheet.Cells["K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K13"].Value = "Sổ sách";//H->K
                        range.Worksheet.Cells["L13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L13"].Value = "Thực tế";

                        //range.Worksheet.Cells["J12:J13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["J12:J13"].Merge = true;
                        //range.Worksheet.Cells["J12:J13"].Value = "Hàng vỡ";

                        //range.Worksheet.Cells["K12:K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["K12:K13"].Merge = true;
                        //range.Worksheet.Cells["K12:K13"].Value = "Ghi chú";
                    }

                    //write data from line 12
                    int index = 14;

                    //var dataTheoKho = datas.GroupBy(x => x.KhoId).Select(x => x.Key);
                    //var dataTheoNhomVT = datas.GroupBy(x => x.NhomVatTuName).Select(x => x.Key);


                    var lstKhoa = datas.GroupBy(s => new { Khoa = s.Kho }).Select(s => new KhoaGroupBaoCaoKTNhapXuatTonChiTietVo
                    {
                        Khoa = s.First().Kho
                    }).OrderBy(p => p.Khoa).ToList();

                    var listNhom = datas.GroupBy(s => new { Khoa = s.Kho, s.NhomVatTuName }).Select(s => new NhomGroupBaoCaoKTNhapXuatTonChiTietVo
                    {
                        Khoa = s.First().Kho,
                        Nhom = s.First().NhomVatTuName
                    }).OrderBy(p => p.Nhom).ToList();



                    
                    if (lstKhoa.Any())
                    {
                        foreach (var khoa in lstKhoa)
                        {
                            var stt = 1;
                            var lstNhomTheoKhoa = listNhom.Where(o => o.Khoa == khoa.Khoa).Select(s => s.Nhom).ToList();
                            if (lstNhomTheoKhoa.Any())
                            {
                                using (var range = worksheet.Cells["A" + index])
                                {
                                    range.Worksheet.Cells["A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["A" + index].Value = khoa.Khoa;
                                }

                                index++;

                                if (datas.Any())
                                {
                                    foreach (var data in lstNhomTheoKhoa)
                                    {
                                        var listDataTheoNhomVT = datas.Where(x => x.NhomVatTuName == data && x.Kho == khoa.Khoa).ToList();
                                        if (listDataTheoNhomVT.Any())
                                        {
                                            //loai HC
                                            worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                            worksheet.Cells["A" + index + ":L" + index].Merge = true;
                                            worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;
                                            worksheet.Cells["A" + index + ":L" + index].Value = listDataTheoNhomVT.FirstOrDefault().NhomVatTuName;
                                            index++;

                                            foreach (var item in listDataTheoNhomVT)
                                            {
                                                // format border, font chữ,....
                                                worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                                worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                                worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                                                worksheet.Row(index).Height = 20.5;

                                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A" + index].Value = stt;

                                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["B" + index].Value = item.MaVatTu;

                                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["C" + index].Value = item.TenVatTu;

                                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["D" + index].Value = item.DonVi;

                                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["E" + index].Value = item.SoLo;

                                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["F" + index].Value = item.HanDungDisplay;

                                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["G" + index].Value = item.CoBHYT  ? "X" : null;

                                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["H" + index].Value = item.DonGia;

                                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["I" + index].Value = item.DonGiaNhap;

                                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["J" + index].Value = item.DonGiaBan;

                                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["K" + index].Value = item.SLSoSach;

                                                //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                //worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                //worksheet.Cells["I" + index].Value = item.SLThucTe;

                                                //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                //worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                //worksheet.Cells["J" + index].Value = item.TinhTrangHuTon;

                                                //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                //worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                //worksheet.Cells["K" + index].Value = item.GhiChu;
                                                stt++;
                                                index++;
                                            }
                                        }
                                    }
                                }

                                index++;
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