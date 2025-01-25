using Camino.Core.Data;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
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
using Camino.Core.Domain;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<List<LookupItemVo>> GetTatCakhoaNhapXuatTonLookupAsync(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(Kho.Ten),
            };

            var allKhoas = new List<LookupItemVo>()
            {
                new LookupItemVo {KeyId = 0 , DisplayName = "Toàn viện" }
            };
            var result = _KhoaPhongRepository.TableNoTracking.Where(c => c.KhoDuocPhams.Any())
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
        
        public async Task<GridDataSource> GetDataBaoCaoKTNhapXuatTonForGridAsync(BaoCaoKTNhapXuatTonQueryInfo queryInfo)
        {
            var thongTinKho = _khoRepository.TableNoTracking.Select(o => new { o.Id, o.Ten, o.LoaiKho, o.KhoaPhongId }).ToList();
            var thongTinKhoaPhong = _KhoaPhongRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var thongTinPhongBenhVien = _phongBenhVienRepository.TableNoTracking.Select(o => new { o.Id, o.KhoaPhongId, o.Ten }).ToList();
            var maxTake = 18000;
            IEnumerable<BaoCaoKTNhapXuatTonChiTietQueryData> allData = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
            //duoc pham
            if (queryInfo.CoThuoc)
            {
                var allDataNhapDuocPhamQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(o => o.NgayNhap <= queryInfo.ToDate &&
                            (o.NhapKhoDuocPhams.KhoId != (long)EnumKhoDuocPham.KhoTongDuocPham || (o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoTongDuocPham && o.KhoNhapSauKhiDuyetId == null)) &&
                            o.SoLuongNhap != 0);
                if (queryInfo.KhoaPhongId != 0)
                {
                    allDataNhapDuocPhamQuery = allDataNhapDuocPhamQuery.Where(o => o.NhapKhoDuocPhams.KhoDuocPhams.KhoaPhongId == queryInfo.KhoaPhongId);
                }
                var allDataNhapDuocPham = allDataNhapDuocPhamQuery.Select(o => new BaoCaoKTNhapXuatTonChiTietQueryData
                {
                    KhoId = o.NhapKhoDuocPhams.KhoId,
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    NhapTuXuatKhoId = o.NhapKhoDuocPhams.XuatKhoDuocPhamId,
                    SLNhap = o.SoLuongNhap,
                    SLXuat = 0,
                    DonGiaNhap = o.DonGiaNhap,
                    VAT = o.VAT,
                    DauKy = o.NgayNhap < queryInfo.FromDate,
                }).GroupBy(o => new
                {
                    o.KhoId,
                    o.DuocPhamBenhVienId,
                    o.NhapTuXuatKhoId,
                    o.DonGiaNhap,
                    o.VAT,
                    o.DauKy,
                }, o => o,
                        (k, v) => new BaoCaoKTNhapXuatTonChiTietQueryData
                        {
                            KhoId = k.KhoId,
                            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                            NhapTuXuatKhoId = k.NhapTuXuatKhoId,
                            SLNhap = v.Sum(x => x.SLNhap),
                            SLXuat = 0,
                            DonGiaNhap = k.DonGiaNhap,
                            VAT = k.VAT,
                            DauKy = k.DauKy,
                        }).ToList();

                var nhapTuXuatKhoIds = allDataNhapDuocPham.Where(o => o.NhapTuXuatKhoId != null && o.DauKy == false).Select(o => o.NhapTuXuatKhoId.Value).ToList();

                //var xuatKhoDeHoanTraIds = _xuatKhoDuocPhamRepository.TableNoTracking
                //    .Where(o => nhapTuXuatKhoIds.Contains(o.Id) && o.XuatKhoDuocPhamChiTiets.Any(ct => ct.XuatKhoDuocPhamChiTietViTris.Any(vt => vt.YeuCauTraDuocPhamChiTiets.Any())))
                //    .Select(o => o.Id).ToList();

                var xuatKhoDeHoanTraIds = _yeuCauTraDuocPhamChiTietRepository.TableNoTracking
                    .Where(o => o.XuatKhoDuocPhamChiTietViTri != null && o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null && nhapTuXuatKhoIds.Contains(o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId.GetValueOrDefault()))
                    .Select(o => o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId).Distinct().ToList();

                var xuatKhoSauKhiDuyetIds = _xuatKhoDuocPhamRepository.TableNoTracking
                    .Where(o => nhapTuXuatKhoIds.Contains(o.Id) && o.KhoXuatId == (long)EnumKhoDuocPham.KhoTongDuocPham && o.LyDoXuatKho == Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                    .Select(o => o.Id).ToList();

                foreach (var baoCaoKtNhapXuatTonChiTietQueryData in allDataNhapDuocPham)
                {
                    if (baoCaoKtNhapXuatTonChiTietQueryData.NhapTuXuatKhoId == null)
                    {
                        baoCaoKtNhapXuatTonChiTietQueryData.MuaNCC = true;
                    }
                    else
                    {
                        if (xuatKhoDeHoanTraIds.Contains(baoCaoKtNhapXuatTonChiTietQueryData.NhapTuXuatKhoId.Value))
                        {
                            baoCaoKtNhapXuatTonChiTietQueryData.NhapHoanTra = true;
                        }
                        else if (xuatKhoSauKhiDuyetIds.Contains(baoCaoKtNhapXuatTonChiTietQueryData.NhapTuXuatKhoId.Value))
                        {
                            baoCaoKtNhapXuatTonChiTietQueryData.MuaNCC = true;
                        }
                        else
                        {
                            baoCaoKtNhapXuatTonChiTietQueryData.NhapNoiBo = true;
                        }
                    }
                }

                var allDataXuatDuocPhamQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                                                                                                 (o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId != (long)EnumKhoDuocPham.KhoTongDuocPham || (o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == (long)EnumKhoDuocPham.KhoTongDuocPham && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)) &&
                                                                                                                 ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                                                                                                  (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)) &&
                                                                                                                 o.SoLuongXuat != 0);


                var allDataXuatDuocPham = allDataXuatDuocPhamQuery
                    .GroupBy(o => new
                    {
                        KhoId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId,
                        o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        o.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                        o.NhapKhoDuocPhamChiTiet.VAT,
                        o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.TraNCC,
                        XuatNoiBo = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoNhapId != null,
                        BenhNhanTraLai = o.SoLuongXuat < 0,
                        DauKy = (o.NgayXuat != null && o.NgayXuat <= queryInfo.FromDate) || (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat < queryInfo.FromDate),
                        XuatChoBenhNhan = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                    }, o => o,
                        (k, v) => new BaoCaoKTNhapXuatTonChiTietQueryData
                        {
                            KhoId = k.KhoId,
                            DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                            DonGiaNhap = k.DonGiaNhap,
                            VAT = k.VAT,
                            TraNCC = k.TraNCC,
                            XuatNoiBo = k.XuatNoiBo,
                            BenhNhanTraLai = k.BenhNhanTraLai,
                            DauKy = k.DauKy,
                            XuatChoBenhNhan = k.XuatChoBenhNhan,
                            BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans = (k.DauKy == false && k.XuatChoBenhNhan) ?
                                v.Select(x => new BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan { XuatKhoChiTietViTriId = x.Id, XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietId, SoLuongXuat = x.SoLuongXuat }).ToList() : new List<BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan>(),
                            SLNhap = v.Sum(x => x.SoLuongXuat < 0 ? x.SoLuongXuat * (-1) : 0),
                            SLXuat = v.Sum(x => x.SoLuongXuat > 0 ? x.SoLuongXuat : 0)
                        }).ToList();

                var khoTongCap2Ids = thongTinKho.Where(o => o.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2).Select(o => o.Id).ToList();
                var xuatKhoChiTietIds = allDataXuatDuocPham.Where(o => khoTongCap2Ids.Contains(o.KhoId) && o.XuatChoBenhNhan == true)
                    .SelectMany(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans).Select(o => o.XuatKhoChiTietId)
                    .Distinct().ToList();

                List<BaoCaoKTNhapXuatTonKhoTongCap2XuatKhoChiTietVo> khoTongCap2XuatKhoChiTietData = new List<BaoCaoKTNhapXuatTonKhoTongCap2XuatKhoChiTietVo>();
                for (int i = 0; i < xuatKhoChiTietIds.Count; i = i + maxTake)
                {
                    var takeXuatKhoChiTietIds = xuatKhoChiTietIds.Skip(i).Take(maxTake).ToList();
                    var ids = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.XuatKhoDuocPhamChiTietId != null && takeXuatKhoChiTietIds.Contains(o.XuatKhoDuocPhamChiTietId.Value))
                    .Select(o => new BaoCaoKTNhapXuatTonKhoTongCap2XuatKhoChiTietVo { KhoaPhongId = o.NoiChiDinh.KhoaPhongId, XuatKhoDuocPhamChiTietId = o.XuatKhoDuocPhamChiTietId, LaDuocPhamBHYT = o.LaDuocPhamBHYT }).ToList();
                    khoTongCap2XuatKhoChiTietData.AddRange(ids);
                }

                //var khoTongCap2XuatKhoChiTietData = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                //    .Where(o => o.XuatKhoDuocPhamChiTietId != null && xuatKhoChiTietIds.Contains(o.XuatKhoDuocPhamChiTietId.Value))
                //    .Select(o => new { o.NoiChiDinh.KhoaPhongId, o.XuatKhoDuocPhamChiTietId }).ToList();
                var khoTongCap2XuatKhoDuocPhamChiTietIds = khoTongCap2XuatKhoChiTietData.Select(o => o.XuatKhoDuocPhamChiTietId).ToList();

                var xuatKhoChoBenhNhanChiTietViTriIds = allDataXuatDuocPham
                    .SelectMany(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans).Select(o => o.XuatKhoChiTietViTriId)
                    .ToList();
                //List<long> xuatKhoChoKhachVanLaiChiTietViTriIds = new List<long>();

                List<BaoCaoKTNhapXuatTonDonThuocThanhToanChiTietVo> donThuocThanhToanChiTiets = new List<BaoCaoKTNhapXuatTonDonThuocThanhToanChiTietVo>();

                for (int i = 0; i < xuatKhoChoBenhNhanChiTietViTriIds.Count; i = i + maxTake)
                {
                    var takeXuatKhoChoBenhNhanChiTietViTriIds = xuatKhoChoBenhNhanChiTietViTriIds.Skip(i).Take(maxTake).ToList();
                    //var ids = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o =>
                    //        o.YeuCauKhamBenhDonThuocChiTietId == null &&
                    //        takeXuatKhoChoBenhNhanChiTietViTriIds.Contains(o.XuatKhoDuocPhamChiTietViTriId))
                    //    .Select(o => o.XuatKhoDuocPhamChiTietViTriId).ToList();
                    //xuatKhoChoKhachVanLaiChiTietViTriIds.AddRange(ids);

                    var info = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o =>
                            takeXuatKhoChoBenhNhanChiTietViTriIds.Contains(o.XuatKhoDuocPhamChiTietViTriId))
                        .Select(o => new BaoCaoKTNhapXuatTonDonThuocThanhToanChiTietVo
                        {
                            XuatKhoDuocPhamChiTietViTriId = o.XuatKhoDuocPhamChiTietViTriId,
                            YeuCauKhamBenhNoiKeDonId = o.DonThuocThanhToan.YeuCauKhamBenhDonThuoc != null ? o.DonThuocThanhToan.YeuCauKhamBenhDonThuoc.NoiKeDonId : 0,
                            NoiTruNoiKeDonId = o.DonThuocThanhToan.NoiTruDonThuoc != null ? o.DonThuocThanhToan.NoiTruDonThuoc.NoiKeDonId : 0,
                            LaDuocPhamBHYT = o.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                        }).ToList();
                    donThuocThanhToanChiTiets.AddRange(info);
                }
                var xuatKhoChoKhachVanLaiChiTietViTriIds = donThuocThanhToanChiTiets.Where(o => o.YeuCauKhamBenhNoiKeDonId == 0 && o.NoiTruNoiKeDonId == 0).Select(o => o.XuatKhoDuocPhamChiTietViTriId).ToList();
                var xuatKhoDonThuocChiTiets = donThuocThanhToanChiTiets.Where(o => o.YeuCauKhamBenhNoiKeDonId != 0 || o.NoiTruNoiKeDonId != 0).ToList();
                var xuatKhoDonThuocChiTietViTriIds = xuatKhoDonThuocChiTiets.Select(o => o.XuatKhoDuocPhamChiTietViTriId).ToList();


                var dataXuatDuocPhamNoiBos = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                var dataHoanTraDuocPhamNoiBos = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                var dataNhapDuocPhamTuKhoCap2s = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                var dataXuatChoBenhNhans = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                var dataNhapHoanTraTuBNs = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                var dataXuatKhoCap2s = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                var dataXuatDuocPhamChoKhachVanLais = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                foreach (var baoCaoKtNhapXuatTonChiTietQueryData in allDataXuatDuocPham)
                {
                    bool xuatNoiBoKhoAo = false;
                    if (baoCaoKtNhapXuatTonChiTietQueryData.DauKy == false && baoCaoKtNhapXuatTonChiTietQueryData.XuatChoBenhNhan)
                    {
                        //xuat truc tiep tu kho cap 2
                        if (baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans.Any(o => khoTongCap2XuatKhoDuocPhamChiTietIds.Contains(o.XuatKhoChiTietId)))
                        {
                            //xuat
                            var soLuongXuatDuocPhamNoiBo = baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans
                                .Where(o => o.SoLuongXuat > 0)
                                .Select(o => o.SoLuongXuat).DefaultIfEmpty().Sum();
                            var soLuongXuatHoanTraDuocPhamNoiBo = baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans
                                .Where(o => o.SoLuongXuat < 0)
                                .Select(o => o.SoLuongXuat * (-1)).DefaultIfEmpty().Sum();
                            if (!soLuongXuatDuocPhamNoiBo.AlmostEqual(0))
                            {
                                var dataXuatDuocPhamNoiBo = new BaoCaoKTNhapXuatTonChiTietQueryData
                                {
                                    KhoId = baoCaoKtNhapXuatTonChiTietQueryData.KhoId,
                                    DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                    DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                    VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                    TraNCC = null,
                                    XuatNoiBo = true,
                                    BenhNhanTraLai = false,
                                    DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                    XuatChoKhachHang = false,
                                    SLNhap = 0,
                                    SLXuat = Math.Round(soLuongXuatDuocPhamNoiBo, 2)
                                };
                                dataXuatDuocPhamNoiBos.Add(dataXuatDuocPhamNoiBo);
                            }
                            if (!soLuongXuatHoanTraDuocPhamNoiBo.AlmostEqual(0))
                            {
                                var dataHoanTraDuocPhamNoiBo = new BaoCaoKTNhapXuatTonChiTietQueryData
                                {
                                    KhoId = baoCaoKtNhapXuatTonChiTietQueryData.KhoId,
                                    DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                    DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                    VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                    TraNCC = null,
                                    XuatNoiBo = false,
                                    NhapHoanTra = true,
                                    BenhNhanTraLai = false,
                                    DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                    XuatChoKhachHang = false,
                                    SLNhap = Math.Round(soLuongXuatHoanTraDuocPhamNoiBo, 2),
                                    SLXuat = 0
                                };
                                dataHoanTraDuocPhamNoiBos.Add(dataHoanTraDuocPhamNoiBo);
                            }
                            baoCaoKtNhapXuatTonChiTietQueryData.SLXuat = 0;
                            baoCaoKtNhapXuatTonChiTietQueryData.SLNhap = 0;
                            xuatNoiBoKhoAo = true;
                            //nhap
                            List<BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhanTheoKhoa> khoCap2XuatChoBenhNhans = new List<BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhanTheoKhoa>();
                            foreach (var xuatChoBenhNhan in baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans)
                            {
                                var khoTongCap2XuatKhoChiTiet = khoTongCap2XuatKhoChiTietData.FirstOrDefault(o => o.XuatKhoDuocPhamChiTietId == xuatChoBenhNhan.XuatKhoChiTietId);
                                if (khoTongCap2XuatKhoChiTiet != null && (queryInfo.KhoaPhongId == 0 || queryInfo.KhoaPhongId == khoTongCap2XuatKhoChiTiet.KhoaPhongId))
                                {
                                    khoCap2XuatChoBenhNhans.Add(new BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhanTheoKhoa { KhoaPhongId = khoTongCap2XuatKhoChiTiet.KhoaPhongId, LaDuocPhamBHYT = khoTongCap2XuatKhoChiTiet.LaDuocPhamBHYT, BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan = xuatChoBenhNhan });
                                }
                            }
                            if (khoCap2XuatChoBenhNhans.Any())
                            {
                                foreach (var khoCap2XuatChoBenhNhanTheoKhoa in khoCap2XuatChoBenhNhans.GroupBy(o => new { o.KhoaPhongId, o.LaDuocPhamBHYT }))
                                {
                                    var soLuongNhapDuocPhamTheoKhoa = khoCap2XuatChoBenhNhanTheoKhoa
                                        .Where(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat > 0)
                                        .Select(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat).DefaultIfEmpty().Sum();
                                    var soLuongNhapHoanTraDuocPhamTheoKhoa = khoCap2XuatChoBenhNhanTheoKhoa
                                        .Where(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat < 0)
                                        .Select(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat * (-1)).DefaultIfEmpty().Sum();
                                    if (!soLuongNhapDuocPhamTheoKhoa.AlmostEqual(0))
                                    {
                                        //kho ao nhap tu kho cap 2
                                        var dataNhapDuocPhamTuKhoCap2 = new BaoCaoKTNhapXuatTonChiTietQueryData
                                        {
                                            KhoId = 0,
                                            KhoaKhoAoId = khoCap2XuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                            LaDuocPhamBHYT = khoCap2XuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                            DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                            DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                            VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                            TraNCC = null,
                                            NhapNoiBo = true,
                                            BenhNhanTraLai = false,
                                            DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                            XuatChoKhachHang = false,
                                            SLNhap = Math.Round(soLuongNhapDuocPhamTheoKhoa, 2),
                                            SLXuat = 0
                                        };
                                        dataNhapDuocPhamTuKhoCap2s.Add(dataNhapDuocPhamTuKhoCap2);
                                        //kho ap xuat cho BN
                                        var dataXuatChoBenhNhan = new BaoCaoKTNhapXuatTonChiTietQueryData
                                        {
                                            KhoId = 0,
                                            KhoaKhoAoId = khoCap2XuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                            LaDuocPhamBHYT = khoCap2XuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                            DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                            DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                            VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                            TraNCC = null,
                                            NhapNoiBo = false,
                                            BenhNhanTraLai = false,
                                            DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                            XuatChoBenhNhan = true,
                                            XuatChoKhachHang = false,
                                            SLNhap = 0,
                                            SLXuat = Math.Round(soLuongNhapDuocPhamTheoKhoa, 2)
                                        };
                                        dataXuatChoBenhNhans.Add(dataXuatChoBenhNhan);
                                    }
                                    if (!soLuongNhapHoanTraDuocPhamTheoKhoa.AlmostEqual(0))
                                    {
                                        //kho ao nhap hoan tra tu BN
                                        var dataNhapHoanTraTuBN = new BaoCaoKTNhapXuatTonChiTietQueryData
                                        {
                                            KhoId = 0,
                                            KhoaKhoAoId = khoCap2XuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                            LaDuocPhamBHYT = khoCap2XuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                            DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                            DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                            VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                            TraNCC = null,
                                            NhapNoiBo = false,
                                            BenhNhanTraLai = true,
                                            DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                            XuatChoKhachHang = false,
                                            SLNhap = Math.Round(soLuongNhapHoanTraDuocPhamTheoKhoa, 2),
                                            SLXuat = 0
                                        };
                                        dataNhapHoanTraTuBNs.Add(dataNhapHoanTraTuBN);
                                        //kho ao xuat hoan tra ve kho cap 2
                                        var dataXuatKhoCap2 = new BaoCaoKTNhapXuatTonChiTietQueryData
                                        {
                                            KhoId = 0,
                                            KhoaKhoAoId = khoCap2XuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                            LaDuocPhamBHYT = khoCap2XuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                            DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                            DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                            VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                            TraNCC = null,
                                            XuatNoiBo = true,
                                            BenhNhanTraLai = false,
                                            DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                            XuatChoBenhNhan = false,
                                            XuatChoKhachHang = false,
                                            SLNhap = 0,
                                            SLXuat = Math.Round(soLuongNhapHoanTraDuocPhamTheoKhoa, 2)
                                        };
                                        dataXuatKhoCap2s.Add(dataXuatKhoCap2);
                                    }
                                }
                            }
                        }

                        if (baoCaoKtNhapXuatTonChiTietQueryData.KhoId == (long)EnumKhoDuocPham.KhoThuocBHYT)
                        {
                            if (baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans.Any(o => xuatKhoDonThuocChiTietViTriIds.Contains(o.XuatKhoChiTietViTriId)))
                            {
                                //xuat
                                if (!xuatNoiBoKhoAo)
                                {
                                    var soLuongXuatDuocPhamNoiBo = baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans
                                    .Where(o => o.SoLuongXuat > 0)
                                    .Select(o => o.SoLuongXuat).DefaultIfEmpty().Sum();
                                        var soLuongXuatHoanTraDuocPhamNoiBo = baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans
                                            .Where(o => o.SoLuongXuat < 0)
                                            .Select(o => o.SoLuongXuat * (-1)).DefaultIfEmpty().Sum();
                                    if (!soLuongXuatDuocPhamNoiBo.AlmostEqual(0))
                                    {
                                        var dataXuatDuocPhamNoiBo = new BaoCaoKTNhapXuatTonChiTietQueryData
                                        {
                                            KhoId = baoCaoKtNhapXuatTonChiTietQueryData.KhoId,
                                            DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                            DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                            VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                            TraNCC = null,
                                            XuatNoiBo = true,
                                            BenhNhanTraLai = false,
                                            DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                            XuatChoKhachHang = false,
                                            SLNhap = 0,
                                            SLXuat = Math.Round(soLuongXuatDuocPhamNoiBo, 2)
                                        };
                                        dataXuatDuocPhamNoiBos.Add(dataXuatDuocPhamNoiBo);
                                    }
                                    if (!soLuongXuatHoanTraDuocPhamNoiBo.AlmostEqual(0))
                                    {
                                        var dataHoanTraDuocPhamNoiBo = new BaoCaoKTNhapXuatTonChiTietQueryData
                                        {
                                            KhoId = baoCaoKtNhapXuatTonChiTietQueryData.KhoId,
                                            DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                            DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                            VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                            TraNCC = null,
                                            XuatNoiBo = false,
                                            NhapHoanTra = true,
                                            BenhNhanTraLai = false,
                                            DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                            XuatChoKhachHang = false,
                                            SLNhap = Math.Round(soLuongXuatHoanTraDuocPhamNoiBo, 2),
                                            SLXuat = 0
                                        };
                                        dataHoanTraDuocPhamNoiBos.Add(dataHoanTraDuocPhamNoiBo);
                                    }
                                    baoCaoKtNhapXuatTonChiTietQueryData.SLXuat = 0;
                                    baoCaoKtNhapXuatTonChiTietQueryData.SLNhap = 0;
                                    xuatNoiBoKhoAo = true;
                                }
                                
                                //nhap
                                List<BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhanTheoKhoa> khoBHYTXuatChoBenhNhans = new List<BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhanTheoKhoa>();
                                foreach (var xuatChoBenhNhan in baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans)
                                {
                                    var khoBHYTXuatChoBenhNhan = xuatKhoDonThuocChiTiets.FirstOrDefault(o => o.XuatKhoDuocPhamChiTietViTriId == xuatChoBenhNhan.XuatKhoChiTietViTriId);
                                    if (khoBHYTXuatChoBenhNhan != null)
                                    {
                                        var noiKeDonId = khoBHYTXuatChoBenhNhan.YeuCauKhamBenhNoiKeDonId != 0 ? khoBHYTXuatChoBenhNhan.YeuCauKhamBenhNoiKeDonId : khoBHYTXuatChoBenhNhan.NoiTruNoiKeDonId;
                                        var khoaPhongId = thongTinPhongBenhVien.First(o => o.Id == noiKeDonId).KhoaPhongId;

                                        if (queryInfo.KhoaPhongId == 0 || queryInfo.KhoaPhongId == khoaPhongId)
                                        {
                                            khoBHYTXuatChoBenhNhans.Add(new BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhanTheoKhoa { KhoaPhongId = khoaPhongId, BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan = xuatChoBenhNhan, LaDuocPhamBHYT = khoBHYTXuatChoBenhNhan.LaDuocPhamBHYT });
                                        }
                                    }
                                }
                                if (khoBHYTXuatChoBenhNhans.Any())
                                {
                                    foreach (var khoBHYTXuatChoBenhNhanTheoKhoa in khoBHYTXuatChoBenhNhans.GroupBy(o => new { o.KhoaPhongId, o.LaDuocPhamBHYT }))
                                    {
                                        var soLuongNhapDuocPhamTheoKhoa = khoBHYTXuatChoBenhNhanTheoKhoa
                                            .Where(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat > 0)
                                            .Select(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat).DefaultIfEmpty().Sum();
                                        var soLuongNhapHoanTraDuocPhamTheoKhoa = khoBHYTXuatChoBenhNhanTheoKhoa
                                            .Where(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat < 0)
                                            .Select(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan.SoLuongXuat * (-1)).DefaultIfEmpty().Sum();
                                        if (!soLuongNhapDuocPhamTheoKhoa.AlmostEqual(0))
                                        {
                                            //kho ao nhap tu kho cap 2
                                            var dataNhapDuocPhamTuKhoCap2 = new BaoCaoKTNhapXuatTonChiTietQueryData
                                            {
                                                KhoId = 0,
                                                KhoaKhoAoId = khoBHYTXuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                                LaDuocPhamBHYT = khoBHYTXuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                                DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                                DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                                VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                                TraNCC = null,
                                                NhapNoiBo = true,
                                                BenhNhanTraLai = false,
                                                DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                                XuatChoKhachHang = false,
                                                SLNhap = Math.Round(soLuongNhapDuocPhamTheoKhoa, 2),
                                                SLXuat = 0
                                            };
                                            dataNhapDuocPhamTuKhoCap2s.Add(dataNhapDuocPhamTuKhoCap2);
                                            //kho ap xuat cho BN
                                            var dataXuatChoBenhNhan = new BaoCaoKTNhapXuatTonChiTietQueryData
                                            {
                                                KhoId = 0,
                                                KhoaKhoAoId = khoBHYTXuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                                LaDuocPhamBHYT = khoBHYTXuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                                DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                                DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                                VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                                TraNCC = null,
                                                NhapNoiBo = false,
                                                BenhNhanTraLai = false,
                                                DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                                XuatChoBenhNhan = true,
                                                XuatChoKhachHang = false,
                                                SLNhap = 0,
                                                SLXuat = Math.Round(soLuongNhapDuocPhamTheoKhoa, 2)
                                            };
                                            dataXuatChoBenhNhans.Add(dataXuatChoBenhNhan);
                                        }
                                        if (!soLuongNhapHoanTraDuocPhamTheoKhoa.AlmostEqual(0))
                                        {
                                            //kho ao nhap hoan tra tu BN
                                            var dataNhapHoanTraTuBN = new BaoCaoKTNhapXuatTonChiTietQueryData
                                            {
                                                KhoId = 0,
                                                KhoaKhoAoId = khoBHYTXuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                                LaDuocPhamBHYT = khoBHYTXuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                                DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                                DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                                VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                                TraNCC = null,
                                                NhapNoiBo = false,
                                                BenhNhanTraLai = true,
                                                DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                                XuatChoKhachHang = false,
                                                SLNhap = Math.Round(soLuongNhapHoanTraDuocPhamTheoKhoa, 2),
                                                SLXuat = 0
                                            };
                                            dataNhapHoanTraTuBNs.Add(dataNhapHoanTraTuBN);
                                            //kho ao xuat hoan tra ve kho cap 2
                                            var dataXuatKhoCap2 = new BaoCaoKTNhapXuatTonChiTietQueryData
                                            {
                                                KhoId = 0,
                                                KhoaKhoAoId = khoBHYTXuatChoBenhNhanTheoKhoa.Key.KhoaPhongId,
                                                LaDuocPhamBHYT = khoBHYTXuatChoBenhNhanTheoKhoa.Key.LaDuocPhamBHYT,
                                                DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                                DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                                VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                                TraNCC = null,
                                                XuatNoiBo = true,
                                                BenhNhanTraLai = false,
                                                DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                                XuatChoBenhNhan = false,
                                                XuatChoKhachHang = false,
                                                SLNhap = 0,
                                                SLXuat = Math.Round(soLuongNhapHoanTraDuocPhamTheoKhoa, 2)
                                            };
                                            dataXuatKhoCap2s.Add(dataXuatKhoCap2);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var soLuongXuatChoKhachVanLai = baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans
                            .Where(o => xuatKhoChoKhachVanLaiChiTietViTriIds.Contains(o.XuatKhoChiTietViTriId))
                            .Select(o => o.SoLuongXuat).DefaultIfEmpty().Sum();
                            if (!soLuongXuatChoKhachVanLai.AlmostEqual(0))
                            {
                                baoCaoKtNhapXuatTonChiTietQueryData.SLXuat = Math.Round(baoCaoKtNhapXuatTonChiTietQueryData.SLXuat - soLuongXuatChoKhachVanLai, 2);
                                var dataXuatDuocPhamChoKhachVanLai = new BaoCaoKTNhapXuatTonChiTietQueryData
                                {
                                    KhoId = baoCaoKtNhapXuatTonChiTietQueryData.KhoId,
                                    DuocPhamBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.DuocPhamBenhVienId,
                                    DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                    VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                    TraNCC = baoCaoKtNhapXuatTonChiTietQueryData.TraNCC,
                                    XuatNoiBo = baoCaoKtNhapXuatTonChiTietQueryData.XuatNoiBo,
                                    BenhNhanTraLai = baoCaoKtNhapXuatTonChiTietQueryData.BenhNhanTraLai,
                                    DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                    XuatChoKhachHang = true,
                                    SLNhap = 0,
                                    SLXuat = Math.Round(soLuongXuatChoKhachVanLai, 2)
                                };
                                dataXuatDuocPhamChoKhachVanLais.Add(dataXuatDuocPhamChoKhachVanLai);
                            }
                        }
                    }
                }

                allData = allDataNhapDuocPham.Concat(dataNhapDuocPhamTuKhoCap2s).Concat(dataXuatChoBenhNhans).Concat(dataNhapHoanTraTuBNs).Concat(dataXuatKhoCap2s);

                if (queryInfo.KhoaPhongId != 0)
                {
                    var khoThuocKhoaIds = thongTinKho.Where(o => o.KhoaPhongId == queryInfo.KhoaPhongId).Select(o => o.Id).ToList();
                    allData = allData.Concat(allDataXuatDuocPham.Where(o => khoThuocKhoaIds.Contains(o.KhoId)))
                                        .Concat(dataXuatDuocPhamNoiBos.Where(o => khoThuocKhoaIds.Contains(o.KhoId)))
                                        .Concat(dataHoanTraDuocPhamNoiBos.Where(o => khoThuocKhoaIds.Contains(o.KhoId)))
                                        .Concat(dataXuatDuocPhamChoKhachVanLais.Where(o => khoThuocKhoaIds.Contains(o.KhoId)));
                }
                else
                {
                    allData = allData.Concat(allDataXuatDuocPham)
                                        .Concat(dataXuatDuocPhamNoiBos)
                                        .Concat(dataHoanTraDuocPhamNoiBos)
                                        .Concat(dataXuatDuocPhamChoKhachVanLais);
                }

            }
            //vat tu
            if (queryInfo.CoVTYT)
            {
                var allDataNhapVatTuQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                .Where(o => o.NgayNhap <= queryInfo.ToDate &&
                            (o.NhapKhoVatTu.KhoId != (long)EnumKhoDuocPham.KhoVatTuYTe || (o.NhapKhoVatTu.KhoId == (long)EnumKhoDuocPham.KhoVatTuYTe && o.KhoNhapSauKhiDuyetId == null)) &&
                            o.SoLuongNhap != 0);
                if (queryInfo.KhoaPhongId != 0)
                {
                    allDataNhapVatTuQuery = allDataNhapVatTuQuery.Where(o => o.NhapKhoVatTu.Kho.KhoaPhongId == queryInfo.KhoaPhongId);
                }
                var allDataNhapVatTu = allDataNhapVatTuQuery.Select(o => new BaoCaoKTNhapXuatTonChiTietQueryData
                {
                    KhoId = o.NhapKhoVatTu.KhoId,
                    VatTuBenhVienId = o.VatTuBenhVienId,
                    NhapTuXuatKhoId = o.NhapKhoVatTu.XuatKhoVatTuId,
                    SLNhap = o.SoLuongNhap,
                    SLXuat = 0,
                    DonGiaNhap = o.DonGiaNhap,
                    VAT = o.VAT,
                    DauKy = o.NgayNhap < queryInfo.FromDate,
                }).GroupBy(o => new
                {
                    o.KhoId,
                    o.VatTuBenhVienId,
                    o.NhapTuXuatKhoId,
                    o.DonGiaNhap,
                    o.VAT,
                    o.DauKy,
                }, o => o,
                        (k, v) => new BaoCaoKTNhapXuatTonChiTietQueryData
                        {
                            KhoId = k.KhoId,
                            VatTuBenhVienId = k.VatTuBenhVienId,
                            NhapTuXuatKhoId = k.NhapTuXuatKhoId,
                            SLNhap = v.Sum(x => x.SLNhap),
                            SLXuat = 0,
                            DonGiaNhap = k.DonGiaNhap,
                            VAT = k.VAT,
                            DauKy = k.DauKy,
                        }).ToList();

                var nhapTuXuatKhoVatTuIds = allDataNhapVatTu.Where(o => o.NhapTuXuatKhoId != null && o.DauKy == false).Select(o => o.NhapTuXuatKhoId.Value).ToList();

                var xuatKhoDeHoanTraVatTuIds = _xuatKhoVatTuRepository.TableNoTracking
                    .Where(o => nhapTuXuatKhoVatTuIds.Contains(o.Id) && o.XuatKhoVatTuChiTiets.Any(ct => ct.XuatKhoVatTuChiTietViTris.Any(vt => vt.YeuCauTraVatTuChiTiets.Any())))
                    .Select(o => o.Id).ToList();

                var xuatKhoSauKhiDuyetVatTuIds = _xuatKhoVatTuRepository.TableNoTracking
                    .Where(o => nhapTuXuatKhoVatTuIds.Contains(o.Id) && o.KhoXuatId == (long)EnumKhoDuocPham.KhoVatTuYTe && o.LyDoXuatKho == Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                    .Select(o => o.Id).ToList();

                foreach (var baoCaoKtNhapXuatTonChiTietQueryData in allDataNhapVatTu)
                {
                    if (baoCaoKtNhapXuatTonChiTietQueryData.NhapTuXuatKhoId == null)
                    {
                        baoCaoKtNhapXuatTonChiTietQueryData.MuaNCC = true;
                    }
                    else
                    {
                        if (xuatKhoDeHoanTraVatTuIds.Contains(baoCaoKtNhapXuatTonChiTietQueryData.NhapTuXuatKhoId.Value))
                        {
                            baoCaoKtNhapXuatTonChiTietQueryData.NhapHoanTra = true;
                        }
                        else if (xuatKhoSauKhiDuyetVatTuIds.Contains(baoCaoKtNhapXuatTonChiTietQueryData.NhapTuXuatKhoId.Value))
                        {
                            baoCaoKtNhapXuatTonChiTietQueryData.MuaNCC = true;
                        }
                        else
                        {
                            baoCaoKtNhapXuatTonChiTietQueryData.NhapNoiBo = true;
                        }
                    }
                }

                var allDataXuatVatTuQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                                                                                        (o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId != (long)EnumKhoDuocPham.KhoVatTuYTe || (o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == (long)EnumKhoDuocPham.KhoVatTuYTe && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)) &&
                                                                                                           ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                                                                                            (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)) &&
                                                                                                                 o.SoLuongXuat != 0);
                if (queryInfo.KhoaPhongId != 0)
                {
                    allDataXuatVatTuQuery = allDataXuatVatTuQuery.Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.KhoaPhongId == queryInfo.KhoaPhongId);
                }
                var allDataXuatVatTu = allDataXuatVatTuQuery
                    .GroupBy(o => new
                    {
                        KhoId = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId,
                        o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                        o.NhapKhoVatTuChiTiet.DonGiaNhap,
                        o.NhapKhoVatTuChiTiet.VAT,
                        o.XuatKhoVatTuChiTiet.XuatKhoVatTu.TraNCC,
                        XuatNoiBo = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoNhapId != null,
                        BenhNhanTraLai = o.SoLuongXuat < 0,
                        DauKy = (o.NgayXuat != null && o.NgayXuat <= queryInfo.FromDate) || (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat < queryInfo.FromDate),
                        XuatChoBenhNhan = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LoaiXuatKho == Enums.EnumLoaiXuatKho.XuatChoBenhNhan,
                    }, o => o,
                        (k, v) => new BaoCaoKTNhapXuatTonChiTietQueryData
                        {
                            KhoId = k.KhoId,
                            VatTuBenhVienId = k.VatTuBenhVienId,
                            DonGiaNhap = k.DonGiaNhap,
                            VAT = k.VAT,
                            TraNCC = k.TraNCC,
                            XuatNoiBo = k.XuatNoiBo,
                            BenhNhanTraLai = k.BenhNhanTraLai,
                            DauKy = k.DauKy,
                            XuatChoBenhNhan = k.XuatChoBenhNhan,
                            BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans = (k.DauKy == false && k.XuatChoBenhNhan) ?
                                v.Select(x => new BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan { XuatKhoChiTietViTriId = x.Id, SoLuongXuat = x.SoLuongXuat }).ToList() : new List<BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan>(),
                            SLNhap = v.Sum(x => x.SoLuongXuat < 0 ? x.SoLuongXuat * (-1) : 0),
                            SLXuat = v.Sum(x => x.SoLuongXuat > 0 ? x.SoLuongXuat : 0)
                        }).ToList();

                var xuatKhoVatTuChoBenhNhanChiTietViTriIds = allDataXuatVatTu
                    .SelectMany(o => o.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans).Select(o => o.XuatKhoChiTietViTriId)
                    .ToList();


                List<long> xuatKhoVatTuChoKhachVanLaiChiTietViTriIds = new List<long>();
                for (int i = 0; i < xuatKhoVatTuChoBenhNhanChiTietViTriIds.Count; i = i + maxTake)
                {
                    var takeXuatKhoVatTuChoBenhNhanChiTietViTriIds = xuatKhoVatTuChoBenhNhanChiTietViTriIds.Skip(i).Take(maxTake).ToList();
                    var ids = _donVTYTThanhToanChiTietRepository.TableNoTracking.Where(o =>
                            o.YeuCauKhamBenhDonVTYTChiTietId == null &&
                            takeXuatKhoVatTuChoBenhNhanChiTietViTriIds.Contains(o.XuatKhoVatTuChiTietViTriId))
                        .Select(o => o.XuatKhoVatTuChiTietViTriId).ToList();
                    xuatKhoVatTuChoKhachVanLaiChiTietViTriIds.AddRange(ids);
                }

                var dataXuatVatTuChoKhachVanLais = new List<BaoCaoKTNhapXuatTonChiTietQueryData>();
                foreach (var baoCaoKtNhapXuatTonChiTietQueryData in allDataXuatVatTu)
                {
                    if (baoCaoKtNhapXuatTonChiTietQueryData.DauKy == false && baoCaoKtNhapXuatTonChiTietQueryData.XuatChoBenhNhan)
                    {
                        var soLuongXuatChoKhachVanLai = baoCaoKtNhapXuatTonChiTietQueryData.BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans
                            .Where(o => xuatKhoVatTuChoKhachVanLaiChiTietViTriIds.Contains(o.XuatKhoChiTietViTriId))
                            .Select(o => o.SoLuongXuat).DefaultIfEmpty().Sum();
                        if (!soLuongXuatChoKhachVanLai.AlmostEqual(0))
                        {
                            baoCaoKtNhapXuatTonChiTietQueryData.SLXuat = Math.Round(baoCaoKtNhapXuatTonChiTietQueryData.SLXuat - soLuongXuatChoKhachVanLai, 2);
                            var dataXuatVatTuChoKhachVanLai = new BaoCaoKTNhapXuatTonChiTietQueryData
                            {
                                KhoId = baoCaoKtNhapXuatTonChiTietQueryData.KhoId,
                                VatTuBenhVienId = baoCaoKtNhapXuatTonChiTietQueryData.VatTuBenhVienId,
                                DonGiaNhap = baoCaoKtNhapXuatTonChiTietQueryData.DonGiaNhap,
                                VAT = baoCaoKtNhapXuatTonChiTietQueryData.VAT,
                                TraNCC = baoCaoKtNhapXuatTonChiTietQueryData.TraNCC,
                                XuatNoiBo = baoCaoKtNhapXuatTonChiTietQueryData.XuatNoiBo,
                                BenhNhanTraLai = baoCaoKtNhapXuatTonChiTietQueryData.BenhNhanTraLai,
                                DauKy = baoCaoKtNhapXuatTonChiTietQueryData.DauKy,
                                XuatChoKhachHang = true,
                                SLNhap = 0,
                                SLXuat = Math.Round(soLuongXuatChoKhachVanLai, 2)
                            };
                            dataXuatVatTuChoKhachVanLais.Add(dataXuatVatTuChoKhachVanLai);
                        }
                    }
                }
                allData = allData.Concat(allDataNhapVatTu).Concat(allDataXuatVatTu).Concat(dataXuatVatTuChoKhachVanLais);
            }
            //var allData = allDataNhapDuocPham.Concat(dataNhapDuocPhamTuKhoCap2s).Concat(dataXuatChoBenhNhans).Concat(dataNhapHoanTraTuBNs).Concat(dataXuatKhoCap2s)
            //    .Concat(allDataNhapVatTu).Concat(allDataXuatVatTu).Concat(dataXuatVatTuChoKhachVanLais);

            //if (queryInfo.KhoaPhongId != 0)
            //{
            //    var khoThuocKhoaIds = thongTinKho.Where(o => o.KhoaPhongId == queryInfo.KhoaPhongId).Select(o => o.Id).ToList();
            //    allData = allData.Concat(allDataXuatDuocPham.Where(o => khoThuocKhoaIds.Contains(o.KhoId)))
            //                        .Concat(dataXuatDuocPhamNoiBos.Where(o => khoThuocKhoaIds.Contains(o.KhoId)))
            //                        .Concat(dataHoanTraDuocPhamNoiBos.Where(o => khoThuocKhoaIds.Contains(o.KhoId)))
            //                        .Concat(dataXuatDuocPhamChoKhachVanLais.Where(o => khoThuocKhoaIds.Contains(o.KhoId)));
            //}
            //else
            //{
            //    allData = allData.Concat(allDataXuatDuocPham)
            //                        .Concat(dataXuatDuocPhamNoiBos)
            //                        .Concat(dataHoanTraDuocPhamNoiBos)
            //                        .Concat(dataXuatDuocPhamChoKhachVanLais);
            //}

            var returnData = allData.GroupBy(o => new
            {
                o.KhoId,
                o.KhoaKhoAoId,
                o.LaDuocPhamBHYT,
                o.DuocPhamBenhVienId,
                o.VatTuBenhVienId,
                o.DonGiaNhap,
                VAT = queryInfo.CoVAT && o.KhoId != (long)Enums.EnumKhoDuocPham.KhoNhaThuoc ? o.VAT : 0
            }, o => o,
                (k, v) => new BaoCaoKTNhapXuatTonChiTietGridVo
                {
                    KhoId = k.KhoId,
                    KhoaKhoAoId = k.KhoaKhoAoId,
                    LaDuocPhamBHYT = k.LaDuocPhamBHYT,
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    VatTuBenhVienId = k.VatTuBenhVienId,
                    SLTonDauKy = v.Where(x => x.DauKy).Select(x => x.SLNhap - x.SLXuat).DefaultIfEmpty().Sum(),
                    DonGiaNhap = k.DonGiaNhap,
                    DonGia = Math.Round(k.DonGiaNhap + (k.DonGiaNhap * k.VAT / 100), 2),
                    SLNhapMuaNCCTrongKy = v.Where(x => !x.DauKy && x.MuaNCC).Select(x => x.SLNhap).DefaultIfEmpty().Sum(),
                    SLNhapNoiBoTrongKy = v.Where(x => !x.DauKy && x.NhapNoiBo).Select(x => x.SLNhap).DefaultIfEmpty().Sum(),
                    SLNhapHoanTraTrongKy = v.Where(x => !x.DauKy && x.NhapHoanTra).Select(x => x.SLNhap).DefaultIfEmpty().Sum(),
                    SLNhapKhacTrongKy = v.Where(x => !x.DauKy && x.BenhNhanTraLai).Select(x => x.SLNhap).DefaultIfEmpty().Sum(),

                    SLXuatNoiBoTrongKy = v.Where(x => !x.DauKy && x.XuatNoiBo).Select(x => x.SLXuat).DefaultIfEmpty().Sum(),
                    SLXuatTraNCCTrongKy = v.Where(x => !x.DauKy && !x.XuatNoiBo && x.TraNCC == true).Select(x => x.SLXuat).DefaultIfEmpty().Sum(),
                    SLXuatBNTrongKy = v.Where(x => !x.DauKy && !x.XuatNoiBo && !x.BenhNhanTraLai && !x.XuatChoKhachHang && x.XuatChoBenhNhan).Select(x => x.SLXuat).DefaultIfEmpty().Sum(),
                    SLXuatKHTrongKy = v.Where(x => !x.DauKy && !x.XuatNoiBo && !x.BenhNhanTraLai && x.XuatChoKhachHang).Select(x => x.SLXuat).DefaultIfEmpty().Sum(),
                    SLXuatKhacTrongKy = v.Where(x => !x.DauKy && !x.XuatNoiBo && x.TraNCC != true && !x.BenhNhanTraLai && !x.XuatChoKhachHang && !x.XuatChoBenhNhan && x.SLXuat > 0).Select(x => x.SLXuat).DefaultIfEmpty().Sum()
                }
            ).Where(o => !o.SLTonDauKy.AlmostEqual(0) || !o.SLNhapMuaNCCTrongKy.AlmostEqual(0) || !o.SLNhapNoiBoTrongKy.AlmostEqual(0) || !o.SLNhapHoanTraTrongKy.AlmostEqual(0) || !o.SLNhapKhacTrongKy.AlmostEqual(0)
                        || !o.SLXuatNoiBoTrongKy.AlmostEqual(0) || !o.SLXuatTraNCCTrongKy.AlmostEqual(0) || !o.SLXuatBNTrongKy.AlmostEqual(0) || !o.SLXuatKHTrongKy.AlmostEqual(0) || !o.SLXuatKhacTrongKy.AlmostEqual(0))
            .ToArray();


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

            foreach (var baoCaoKtNhapXuatTonChiTietGridVo in returnData)
            {
                if (baoCaoKtNhapXuatTonChiTietGridVo.KhoaKhoAoId != 0)
                {
                    baoCaoKtNhapXuatTonChiTietGridVo.Kho = $"Kho ảo {thongTinKhoaPhong.FirstOrDefault(o => o.Id == baoCaoKtNhapXuatTonChiTietGridVo.KhoaKhoAoId)?.Ten} {(baoCaoKtNhapXuatTonChiTietGridVo.LaDuocPhamBHYT ? "thuốc BHYT" : "thuốc Viện phí")}";
                }
                else
                {
                    baoCaoKtNhapXuatTonChiTietGridVo.Kho = thongTinKho.FirstOrDefault(o => o.Id == baoCaoKtNhapXuatTonChiTietGridVo.KhoId)?.Ten;
                }

                if (baoCaoKtNhapXuatTonChiTietGridVo.DuocPhamBenhVienId != 0)
                {
                    var dp = thongTinDuocPham.FirstOrDefault(o => o.Id == baoCaoKtNhapXuatTonChiTietGridVo.DuocPhamBenhVienId);
                    baoCaoKtNhapXuatTonChiTietGridVo.Nhom = dp?.Nhom;
                    baoCaoKtNhapXuatTonChiTietGridVo.DVT = dp?.DVT;
                    baoCaoKtNhapXuatTonChiTietGridVo.Ten = dp?.Ten;
                    baoCaoKtNhapXuatTonChiTietGridVo.Ma = dp?.Ma;
                }
                else if (baoCaoKtNhapXuatTonChiTietGridVo.VatTuBenhVienId != 0)
                {
                    var vt = thongTinVatTu.FirstOrDefault(o => o.Id == baoCaoKtNhapXuatTonChiTietGridVo.VatTuBenhVienId);
                    baoCaoKtNhapXuatTonChiTietGridVo.Nhom = vt?.Nhom;
                    baoCaoKtNhapXuatTonChiTietGridVo.DVT = vt?.DVT;
                    baoCaoKtNhapXuatTonChiTietGridVo.Ten = vt?.Ten;
                    baoCaoKtNhapXuatTonChiTietGridVo.Ma = vt?.Ma;
                }
            }

            //Không chia các kho nhỏ và nhóm các thuốc có cùng mã vào 1 dòng, không phân biệt lô/giá
            var groupData = returnData
                .GroupBy(o => new { o.DuocPhamBenhVienId, o.VatTuBenhVienId }, o => o, (k, v) =>
                    new BaoCaoKTNhapXuatTonGridVo
                    {
                        DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        Nhom = v.First().Nhom,
                        Ma = v.First().Ma,
                        Ten = v.First().Ten,
                        DVT = v.First().DVT,
                        SLTonDauKy = v.Sum(x => x.SLTonDauKy),
                        ThanhTienTonDauKy = v.Sum(x => x.ThanhTienTonDauKy),
                        SLNhapMuaNCCTrongKy = v.Sum(x => x.SLNhapMuaNCCTrongKy),
                        ThanhTienNhapMuaNCCTrongKy = v.Sum(x => x.ThanhTienNhapMuaNCCTrongKy),
                        SLNhapTangKiemKeTrongKy = v.Sum(x => x.SLNhapTangKiemKeTrongKy),
                        ThanhTienNhapTangKiemKeTrongKy = v.Sum(x => x.ThanhTienNhapTangKiemKeTrongKy),
                        SLNhapHoanTraTrongKy = v.Sum(x => x.SLNhapHoanTraTrongKy),
                        ThanhTienNhapHoanTraTrongKy = v.Sum(x => x.ThanhTienNhapHoanTraTrongKy),
                        SLNhapNoiBoTrongKy = v.Sum(x => x.SLNhapNoiBoTrongKy),
                        ThanhTienNhapNoiBoTrongKy = v.Sum(x => x.ThanhTienNhapNoiBoTrongKy),
                        SLNhapKhacTrongKy = v.Sum(x => x.SLNhapKhacTrongKy),
                        ThanhTienNhapKhacTrongKy = v.Sum(x => x.ThanhTienNhapKhacTrongKy),
                        SLXuatNoiBoTrongKy = v.Sum(x => x.SLXuatNoiBoTrongKy),
                        ThanhTienXuatNoiBoTrongKy = v.Sum(x => x.ThanhTienXuatNoiBoTrongKy),
                        SLXuatGiamKiemKeTrongKy = v.Sum(x => x.SLXuatGiamKiemKeTrongKy),
                        ThanhTienXuatGiamKiemKeTrongKy = v.Sum(x => x.ThanhTienXuatGiamKiemKeTrongKy),
                        SLXuatTraNCCTrongKy = v.Sum(x => x.SLXuatTraNCCTrongKy),
                        ThanhTienXuatTraNCCTrongKy = v.Sum(x => x.ThanhTienXuatTraNCCTrongKy),
                        SLXuatBNTrongKy = v.Sum(x => x.SLXuatBNTrongKy),
                        ThanhTienXuatBNTrongKy = v.Sum(x => x.ThanhTienXuatBNTrongKy),
                        SLXuatKHTrongKy = v.Sum(x => x.SLXuatKHTrongKy),
                        ThanhTienXuatKHTrongKy = v.Sum(x => x.ThanhTienXuatKHTrongKy),
                        SLXuatKhacTrongKy = v.Sum(x => x.SLXuatKhacTrongKy),
                        ThanhTienXuatKhacTrongKy = v.Sum(x => x.ThanhTienXuatKhacTrongKy),
                    }).ToArray();

            return new GridDataSource { Data = groupData, TotalRowCount = groupData.Length };
        }
        public virtual byte[] ExportBaoCaoKTNhapXuatTon(GridDataSource gridDataSource, BaoCaoKTNhapXuatTonQueryInfo query)
        {
            var datas = (ICollection<BaoCaoKTNhapXuatTonGridVo>)gridDataSource.Data;
            var ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoKTNhapXuatTonGridVo>("STT", p=> ind++)
            };

            //var lstKhoa = datas.GroupBy(s => new { Khoa = s.Kho }).Select(s => new KhoaGroupBaoCaoKTNhapXuatTonChiTietVo
            //{
            //    Khoa = s.First().Kho
            //}).OrderBy(p => p.Khoa).ToList();

            var listNhom = datas.GroupBy(s => new { s.Nhom }).Select(s => new NhomGroupBaoCaoKTNhapXuatTonVo
            {
                Nhom = s.First().Nhom
            }).OrderBy(p => p.Nhom).ToList();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO KT NHẬP XUẤT TỒN");

                    //set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 14;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 7;
                    worksheet.Column(5).Width = 7;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 7;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 7;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 7;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 7;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 7;
                    worksheet.Column(16).Width = 15;
                    worksheet.Column(17).Width = 7;
                    worksheet.Column(18).Width = 15;
                    worksheet.Column(19).Width = 7;
                    worksheet.Column(20).Width = 15;
                    worksheet.Column(21).Width = 7;
                    worksheet.Column(22).Width = 15;
                    worksheet.Column(23).Width = 7;
                    worksheet.Column(24).Width = 15;
                    worksheet.Column(25).Width = 7;
                    worksheet.Column(26).Width = 15;
                    worksheet.Column(27).Width = 7;
                    worksheet.Column(28).Width = 15;
                    worksheet.Column(29).Width = 7;
                    worksheet.Column(30).Width = 15;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(3).Height = 28.50;


                    using (var range = worksheet.Cells["A1:F1"])
                    {
                        range.Worksheet.Cells["A1:F1"].Merge = true;
                        range.Worksheet.Cells["A1:F1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:F1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:F1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:F1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:AD3"])
                    {
                        range.Worksheet.Cells["A3:AD3"].Merge = true;
                        range.Worksheet.Cells["A3:AD3"].Value = "BÁO CÁO NHẬP XUẤT TỒN";
                        range.Worksheet.Cells["A3:AD3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:AD3"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:AD3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:AD3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:AD3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:AD4"])
                    {
                        range.Worksheet.Cells["A4:AD4"].Merge = true;
                        range.Worksheet.Cells["A4:AD4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:AD4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:AD4"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A4:AD4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:AD4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:AD4"].Style.Font.Bold = true;
                    }

                    var tenKho = string.Empty;
                    if (query.KhoaPhongId == 0)
                    {
                        tenKho = "Khoa: Toàn viện";
                    }
                    else
                    {
                        tenKho = $"Khoa: {_KhoaPhongRepository.TableNoTracking.Where(p => p.Id == query.KhoaPhongId).Select(p => p.Ten).FirstOrDefault()}";
                    }

                    using (var range = worksheet.Cells["A5:AD5"])
                    {
                        range.Worksheet.Cells["A5:AD5"].Merge = true;
                        range.Worksheet.Cells["A5:AD5"].Value = tenKho;
                        range.Worksheet.Cells["A5:AD5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:AD5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:AD5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:AD5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:AD5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:AD7"])
                    {
                        range.Worksheet.Cells["A7:AD7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:AD7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:AD7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:AD7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:AD7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8:AD8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:AD8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:AD8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A8:AD8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:AD8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A9:AD9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A9:AD9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A9:AD9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A9:AD9"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A9:AD9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7:A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7:A9"].Merge = true;
                        range.Worksheet.Cells["A7:A9"].Value = "STT";

                        range.Worksheet.Cells["B7:C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7:C8"].Merge = true;
                        range.Worksheet.Cells["B7:C8"].Value = "Tên thuốc, Vật tư, Hoá chất";

                        range.Worksheet.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B9"].Value = "Mã";
                        range.Worksheet.Cells["C9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C9"].Value = "Tên";

                        range.Worksheet.Cells["D7:D9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7:D9"].Merge = true;
                        range.Worksheet.Cells["D7:D9"].Value = "ĐVT";


                        range.Worksheet.Cells["E7:F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7:F8"].Merge = true;
                        range.Worksheet.Cells["E7:F8"].Value = "Tồn đầu";

                        range.Worksheet.Cells["E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E9"].Value = "SL";

                        range.Worksheet.Cells["F9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F9"].Value = "TT";


                        range.Worksheet.Cells["G7:P7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7:P7"].Merge = true;
                        range.Worksheet.Cells["G7:P7"].Value = "Nhập";

                        range.Worksheet.Cells["G8:H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8:H8"].Merge = true;
                        range.Worksheet.Cells["G8:H8"].Value = "Mua NCC";

                        range.Worksheet.Cells["G9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G9"].Value = "SL";

                        range.Worksheet.Cells["H9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H9"].Value = "TT";

                        range.Worksheet.Cells["I8:J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8:J8"].Merge = true;
                        range.Worksheet.Cells["I8:J8"].Value = "Tăng kiểm kê";

                        range.Worksheet.Cells["I9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I9"].Value = "SL";

                        range.Worksheet.Cells["J9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J9"].Value = "TT";

                        range.Worksheet.Cells["K8:L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8:L8"].Merge = true;
                        range.Worksheet.Cells["K8:L8"].Value = "Nhập hoàn trả";

                        range.Worksheet.Cells["K9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K9"].Value = "SL";

                        range.Worksheet.Cells["L9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L9"].Value = "TT";

                        range.Worksheet.Cells["M8:N8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M8:N8"].Merge = true;
                        range.Worksheet.Cells["M8:N8"].Value = "Nhập nội bộ";

                        range.Worksheet.Cells["M9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M9"].Value = "SL";

                        range.Worksheet.Cells["N9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N9"].Value = "TT";

                        range.Worksheet.Cells["O8:P8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O8:P8"].Merge = true;
                        range.Worksheet.Cells["O8:P8"].Value = "Khác";

                        range.Worksheet.Cells["O9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O9"].Value = "SL";

                        range.Worksheet.Cells["P9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P9"].Value = "TT";


                        range.Worksheet.Cells["Q7:AB7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q7:AB7"].Merge = true;
                        range.Worksheet.Cells["Q7:AB7"].Value = "Xuất";

                        range.Worksheet.Cells["Q8:R8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q8:R8"].Merge = true;
                        range.Worksheet.Cells["Q8:R8"].Value = "Xuất nội bộ";

                        range.Worksheet.Cells["Q9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q9"].Value = "SL";

                        range.Worksheet.Cells["R9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R9"].Value = "TT";

                        range.Worksheet.Cells["S8:T8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S8:T8"].Merge = true;
                        range.Worksheet.Cells["S8:T8"].Value = "Giảm kiểm kê";

                        range.Worksheet.Cells["S9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S9"].Value = "SL";

                        range.Worksheet.Cells["T9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["T9"].Value = "TT";

                        range.Worksheet.Cells["U8:V8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["U8:V8"].Merge = true;
                        range.Worksheet.Cells["U8:V8"].Value = "Trả NCC";

                        range.Worksheet.Cells["U9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["U9"].Value = "SL";

                        range.Worksheet.Cells["V9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["V9"].Value = "TT";

                        range.Worksheet.Cells["W8:X8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["W8:X8"].Merge = true;
                        range.Worksheet.Cells["W8:X8"].Value = "Xuất BN";

                        range.Worksheet.Cells["W9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["W9"].Value = "SL";

                        range.Worksheet.Cells["X9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["X9"].Value = "TT";

                        range.Worksheet.Cells["Y8:Z8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Y8:Z8"].Merge = true;
                        range.Worksheet.Cells["Y8:Z8"].Value = "Xuất KH";

                        range.Worksheet.Cells["Y9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Y9"].Value = "SL";

                        range.Worksheet.Cells["Z9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Z9"].Value = "TT";

                        range.Worksheet.Cells["AA8:AB8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AA8:AB8"].Merge = true;
                        range.Worksheet.Cells["AA8:AB8"].Value = "Xuất khác";

                        range.Worksheet.Cells["AA9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AA9"].Value = "SL";

                        range.Worksheet.Cells["AB9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AB9"].Value = "TT";


                        range.Worksheet.Cells["AC7:AD8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AC7:AD8"].Merge = true;
                        range.Worksheet.Cells["AC7:AD8"].Value = "Tồn cuối";

                        range.Worksheet.Cells["AC9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AC9"].Value = "SL";

                        range.Worksheet.Cells["AD9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AD9"].Value = "TT";
                    }

                    //var manager = new PropertyManager<BaoCaoKTNhapXuatTonChiTietGridVo>(requestProperties);
                    int index = 10; // bắt đầu đổ data từ dòng 10
                    var stt = 1;

                    if (datas.Any())
                    {
                        if (listNhom.Any())
                        {
                            foreach (var nhom in listNhom)
                            {
                                var listVatTuTheoNhom = datas.Where(s => s.Nhom == nhom.Nhom).ToList();
                                using (var range = worksheet.Cells["A" + index + ":AD" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":AD" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["E" + index + ":AD" + index].Style.Numberformat.Format = "#,##0.00";


                                    range.Worksheet.Cells["A" + index].Value = nhom.Nhom;

                                    range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                                    range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["F" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienTonDauKy);

                                    range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["H" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienNhapMuaNCCTrongKy);


                                    range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["J" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienNhapTangKiemKeTrongKy);


                                    range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["L" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienNhapHoanTraTrongKy);


                                    range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["N" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienNhapNoiBoTrongKy);


                                    range.Worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["P" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienNhapKhacTrongKy);


                                    range.Worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["R" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienXuatNoiBoTrongKy);


                                    range.Worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["T" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienXuatGiamKiemKeTrongKy);


                                    range.Worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["V" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienXuatTraNCCTrongKy);


                                    range.Worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["X" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienXuatBNTrongKy);


                                    range.Worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["Z" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienXuatKHTrongKy);


                                    range.Worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["AB" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienXuatKhacTrongKy);


                                    range.Worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["AD" + index].Value = listVatTuTheoNhom.Sum(s => s.ThanhTienTonCuoiKy);

                                }
                                index++;

                                if (listVatTuTheoNhom.Any())
                                {
                                    foreach (var vattu in listVatTuTheoNhom)
                                    {
                                        //// format border, font chữ,....
                                        worksheet.Cells["A" + index + ":AD" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                        worksheet.Cells["A" + index + ":AD" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        worksheet.Cells["A" + index + ":AD" + index].Style.Font.Color.SetColor(Color.Black);
                                        worksheet.Cells["E" + index + ":AD" + index].Style.Numberformat.Format = "#,##0.00";


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
                                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["A" + index].Value = stt;

                                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["B" + index].Value = vattu.Ma;

                                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["C" + index].Value = vattu.Ten;

                                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        worksheet.Cells["D" + index].Value = vattu.DVT;

                                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["E" + index].Value = vattu.SLTonDauKy != 0 ? vattu.SLTonDauKy : (double?)null;

                                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["F" + index].Value = vattu.ThanhTienTonDauKy != 0 ? vattu.ThanhTienTonDauKy : (decimal?)null;

                                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["G" + index].Value = vattu.SLNhapMuaNCCTrongKy != 0 ? vattu.SLNhapMuaNCCTrongKy : (double?)null;

                                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["H" + index].Value = vattu.ThanhTienNhapMuaNCCTrongKy != 0 ? vattu.ThanhTienNhapMuaNCCTrongKy : (decimal?)null;

                                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["I" + index].Value = vattu.SLNhapTangKiemKeTrongKy != 0 ? vattu.SLNhapTangKiemKeTrongKy : (double?)null;

                                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["J" + index].Value = vattu.ThanhTienNhapTangKiemKeTrongKy != 0 ? vattu.ThanhTienNhapTangKiemKeTrongKy : (decimal?)null;

                                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["K" + index].Value = vattu.SLNhapHoanTraTrongKy != 0 ? vattu.SLNhapHoanTraTrongKy : (double?)null;

                                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["L" + index].Value = vattu.ThanhTienNhapHoanTraTrongKy != 0 ? vattu.ThanhTienNhapHoanTraTrongKy : (decimal?)null;

                                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["M" + index].Value = vattu.SLNhapNoiBoTrongKy != 0 ? vattu.SLNhapNoiBoTrongKy : (double?)null;

                                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["N" + index].Value = vattu.ThanhTienNhapNoiBoTrongKy != 0 ? vattu.ThanhTienNhapNoiBoTrongKy : (decimal?)null;

                                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["O" + index].Value = vattu.SLNhapKhacTrongKy != 0 ? vattu.SLNhapKhacTrongKy : (double?)null;

                                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["P" + index].Value = vattu.ThanhTienNhapKhacTrongKy != 0 ? vattu.ThanhTienNhapKhacTrongKy : (decimal?)null;

                                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["Q" + index].Value = vattu.SLXuatNoiBoTrongKy != 0 ? vattu.SLXuatNoiBoTrongKy : (double?)null;

                                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["R" + index].Value = vattu.ThanhTienXuatNoiBoTrongKy != 0 ? vattu.ThanhTienXuatNoiBoTrongKy : (decimal?)null;

                                        worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["S" + index].Value = vattu.SLXuatGiamKiemKeTrongKy != 0 ? vattu.SLXuatGiamKiemKeTrongKy : (double?)null;

                                        worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["T" + index].Value = vattu.ThanhTienXuatGiamKiemKeTrongKy != 0 ? vattu.ThanhTienXuatGiamKiemKeTrongKy : (decimal?)null;

                                        worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["U" + index].Value = vattu.SLXuatTraNCCTrongKy != 0 ? vattu.SLXuatTraNCCTrongKy : (double?)null;

                                        worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["V" + index].Value = vattu.ThanhTienXuatTraNCCTrongKy != 0 ? vattu.ThanhTienXuatTraNCCTrongKy : (decimal?)null;

                                        worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["W" + index].Value = vattu.SLXuatBNTrongKy != 0 ? vattu.SLXuatBNTrongKy : (double?)null;

                                        worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["X" + index].Value = vattu.ThanhTienXuatBNTrongKy != 0 ? vattu.ThanhTienXuatBNTrongKy : (decimal?)null;

                                        worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["Y" + index].Value = vattu.SLXuatKHTrongKy != 0 ? vattu.SLXuatKHTrongKy : (double?)null;

                                        worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["Z" + index].Value = vattu.ThanhTienXuatKHTrongKy != 0 ? vattu.ThanhTienXuatKHTrongKy : (decimal?)null;

                                        worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AA" + index].Value = vattu.SLXuatKhacTrongKy != 0 ? vattu.SLXuatKhacTrongKy : (double?)null;

                                        worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AB" + index].Value = vattu.ThanhTienXuatKhacTrongKy != 0 ? vattu.ThanhTienXuatKhacTrongKy : (decimal?)null;

                                        worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AC" + index].Value = vattu.SLTonCuoiKy != 0 ? vattu.SLTonCuoiKy : (double?)null;

                                        worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["AD" + index].Value = vattu.ThanhTienTonCuoiKy != 0 ? vattu.ThanhTienTonCuoiKy : (decimal?)null;
                                        index++;
                                        stt++;
                                    }
                                }


                            }
                        }
                        
                        if (listNhom.Count > 1)
                        {
                            using (var range = worksheet.Cells["A" + index + ":AD" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["E" + index + ":AD" + index].Style.Numberformat.Format = "#,##0.00";
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.Bold = true;

                                range.Worksheet.Cells["A" + index + ":D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":D" + index].Merge = true;
                                range.Worksheet.Cells["A" + index + ":D" + index].Value = "Tổng cộng";

                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Value = datas.Sum(s => s.ThanhTienTonDauKy);

                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["H" + index].Value = datas.Sum(s => s.ThanhTienNhapMuaNCCTrongKy);


                                range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["J" + index].Value = datas.Sum(s => s.ThanhTienNhapTangKiemKeTrongKy);


                                range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["L" + index].Value = datas.Sum(s => s.ThanhTienNhapHoanTraTrongKy);


                                range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["N" + index].Value = datas.Sum(s => s.ThanhTienNhapNoiBoTrongKy);


                                range.Worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["P" + index].Value = datas.Sum(s => s.ThanhTienNhapKhacTrongKy);


                                range.Worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["R" + index].Value = datas.Sum(s => s.ThanhTienXuatNoiBoTrongKy);


                                range.Worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["T" + index].Value = datas.Sum(s => s.ThanhTienXuatGiamKiemKeTrongKy);


                                range.Worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["V" + index].Value = datas.Sum(s => s.ThanhTienXuatTraNCCTrongKy);


                                range.Worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["X" + index].Value = datas.Sum(s => s.ThanhTienXuatBNTrongKy);


                                range.Worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["Z" + index].Value = datas.Sum(s => s.ThanhTienXuatKHTrongKy);


                                range.Worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["AB" + index].Value = datas.Sum(s => s.ThanhTienXuatKhacTrongKy);


                                range.Worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["AD" + index].Value = datas.Sum(s => s.ThanhTienTonCuoiKy);

                            }

                            index++;
                        }
                    }
                    index = index + 3;
                    worksheet.Cells["A" + index + ":AD" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":AD" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["A" + index + ":F" + index].Value = "Người lập";
                    worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":F" + index].Merge = true;

                    worksheet.Cells["H" + index + ":L" + index].Value = "Thủ kho";
                    worksheet.Cells["H" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H" + index + ":L" + index].Merge = true;

                    worksheet.Cells["N" + index + ":R" + index].Value = "Kế toán";
                    worksheet.Cells["N" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["N" + index + ":R" + index].Merge = true;

                    worksheet.Cells["T" + index + ":X" + index].Value = "Trưởng khoa dược/VTYT";
                    worksheet.Cells["T" + index + ":X" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["T" + index + ":X" + index].Merge = true;

                    //Ngày tháng  ký
                    var rowNgay = index - 2;
                    var ngayHienTai = DateTime.Now;
                    worksheet.Cells["Y" + rowNgay + ":AD" + rowNgay].Value = "Ngày " + ngayHienTai.Day + " Tháng " + ngayHienTai.Month + " Năm " + ngayHienTai.Year;
                    worksheet.Cells["Y" + rowNgay + ":AD" + rowNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["Y" + rowNgay + ":AD" + rowNgay].Merge = true;

                    worksheet.Cells["Y" + index + ":AD" + index].Value = "Trường bộ phận";
                    worksheet.Cells["Y" + index + ":AD" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["Y" + index + ":AD" + index].Merge = true;

                    index++;



                    worksheet.Cells["A" + index + ":AD" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":AD" + index].Style.Font.Italic = true;
                    //value
                    worksheet.Cells["A" + index + ":F" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":F" + index].Merge = true;

                    worksheet.Cells["H" + index + ":L" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["H" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H" + index + ":L" + index].Merge = true;

                    worksheet.Cells["N" + index + ":R" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["N" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["N" + index + ":R" + index].Merge = true;

                    worksheet.Cells["T" + index + ":X" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["T" + index + ":X" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["T" + index + ":X" + index].Merge = true;

                    worksheet.Cells["Y" + index + ":AD" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["Y" + index + ":AD" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["Y" + index + ":AD" + index].Merge = true;
                    index++;

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
