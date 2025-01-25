using Camino.Core.Data;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using static Camino.Core.Domain.Enums;
namespace Camino.Services.YeuCauHoanTra.KSNK
{
    public partial class YeuCauHoanTraKSNKService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDpVtKSNKTuTrucDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauTraKSNKTuTrucVoSearch>(queryInfo.AdditionalSearchString);

            var khos = _khoRepository.TableNoTracking.ToList();
            var khoXuat = khos.FirstOrDefault(o => o.Id == info.KhoXuatId);
            var khoNhap = khos.FirstOrDefault(o => o.Id == info.KhoNhapId);
            var hoanTraVTHanhChinh = khoXuat != null && khoNhap != null && khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && khoNhap.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh;

            List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>();
            if ((info.LoaiDuocPhamVatTu == null || info.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham) && !hoanTraVTHanhChinh)
            {
                var nhapKhoDuocPhamChiTietQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dpbv => dpbv.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .Include(nkct => nkct.NhapKhoDuocPhams)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat && x.NhapKhoDuocPhams.KhoId == info.KhoXuatId);
                if (!string.IsNullOrEmpty(info.SearchString))
                {
                    var searchTerms = info.SearchString.Replace("\t", "").Trim();
                    nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietQuery.ApplyLike(searchTerms,
                        g => g.DuocPhamBenhViens.DuocPham.Ten,
                        g => g.DuocPhamBenhViens.Ma,
                        g => g.Solo
                    ).ToList();
                }
                else
                {
                    nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietQuery.ToList();
                }
            }

            List<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>();
            if (info.LoaiDuocPhamVatTu == null || info.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu)
            {
                var nhapKhoVatTuChiTietQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(dpbv => dpbv.VatTus).ThenInclude(dpbv => dpbv.NhomVatTu)
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat && x.NhapKhoVatTu.KhoId == info.KhoXuatId && (!hoanTraVTHanhChinh || x.VatTuBenhVien.VatTus.NhomVatTuId == (long)EnumNhomVatTu.NhomHanhChinh));
                if (!string.IsNullOrEmpty(info.SearchString))
                {
                    var searchTerms = info.SearchString.Replace("\t", "").Trim();
                    nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietQuery.ApplyLike(searchTerms,
                        g => g.VatTuBenhVien.VatTus.Ten,
                        g => g.VatTuBenhVien.Ma,
                        g => g.Solo
                    ).ToList();
                }
                else
                {
                    nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietQuery.ToList();
                }
            }

            var yeuCauTraChiTiets = new List<YeuCauTraKSNKGridVo>();

            var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Solo, x.HanSuDung });

            foreach (var item in nhapKhoDuocPhamChiTietGroup)
            {
                var slTon = item.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                var yeuCauXuatKhoDuocPhamGridVo = new YeuCauTraKSNKGridVo
                {
                    Id = item.First().Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    VatTuBenhVienId = item.Key.DuocPhamBenhVienId,
                    Ten = item.First().DuocPhamBenhViens.DuocPham.Ten,
                    DVT = item.First().DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    LaVatTuBHYT = item.Key.LaDuocPhamBHYT,
                    //NhomVatTuId = item.First().DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    TenNhom = item.First().DuocPhamBenhViens.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = item.First().DuocPhamBenhViens.Ma,
                    SoLo = item.Key.Solo,
                    HanSuDung = item.Key.HanSuDung,
                    NgayNhap = item.First().NgayNhapVaoBenhVien,
                    VAT = item.First().VAT,
                    TiLeThapGia = item.First().TiLeTheoThapGia,
                    DonGiaNhap = item.First().DonGiaNhap,
                    KhoXuatId = item.First().NhapKhoDuocPhams.KhoId,
                    SoLuongTon = slTon,
                    SoLuongTra = slTon
                };
                yeuCauTraChiTiets.Add(yeuCauXuatKhoDuocPhamGridVo);
            }

            var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Solo, x.HanSuDung });

            foreach (var item in nhapKhoVatTuChiTietGroup)
            {
                var slTon = item.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                var yeuCauXuatKhoVatTuGridVo = new YeuCauTraKSNKGridVo
                {
                    Id = item.First().Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    VatTuBenhVienId = item.Key.VatTuBenhVienId,
                    Ten = item.First().VatTuBenhVien.VatTus.Ten,
                    DVT = item.First().VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = item.Key.LaVatTuBHYT,
                    //NhomVatTuId = item.First().VatTuBenhVien.VatTus.NhomVatTuId,
                    TenNhom = item.First().VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = item.First().VatTuBenhVien.Ma,
                    SoLo = item.Key.Solo,
                    HanSuDung = item.Key.HanSuDung,
                    NgayNhap = item.First().NgayNhapVaoBenhVien,
                    VAT = item.First().VAT,
                    TiLeThapGia = item.First().TiLeTheoThapGia,
                    DonGiaNhap = item.First().DonGiaNhap,
                    KhoXuatId = item.First().NhapKhoVatTu.KhoId,
                    SoLuongTon = slTon,
                    SoLuongTra = slTon
                };
                yeuCauTraChiTiets.Add(yeuCauXuatKhoVatTuGridVo);
            }


            //var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
            //        .Include(nkct => nkct.NhapKhoVatTu)
            //        .Include(nkct => nkct.VatTuBenhVien).ThenInclude(nkct => nkct.VatTus)
            //        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //        .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
            //        //&& x.HanSuDung >= DateTime.Now 
            //        && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);
            //if (!string.IsNullOrEmpty(info.SearchString))
            //{
            //    var searchTerms = info.SearchString.Replace("\t", "").Trim();
            //    nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
            //        g => g.VatTuBenhVien.VatTus.Ten,
            //        g => g.VatTuBenhVien.Ma,
            //        g => g.VatTuBenhVien.VatTus.DonViTinh,
            //        g => g.Solo
            //   );
            //}
            //var yeuCauTraVatTuChiTiets = new List<YeuCauTraKSNKGridVo>();
            //var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh, x.DonGiaNhap }).Select(g => new { nhapKhoVatTuChiTiet = g.FirstOrDefault() });

            //foreach (var item in nhapKhoVatTuChiTietGroup)
            //{
            //    var yeuCauTraVatTuChiTiet = new YeuCauTraKSNKGridVo
            //    {
            //        Id = item.nhapKhoVatTuChiTiet.Id,
            //        VatTuBenhVienId = item.nhapKhoVatTuChiTiet.VatTuBenhVienId,
            //        Ten = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
            //        Ma = item.nhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
            //        DVT = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
            //        LaVatTuBHYT = item.nhapKhoVatTuChiTiet.LaVatTuBHYT,
            //        LoaiSuDung = item.nhapKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung,
            //        SoLo = item.nhapKhoVatTuChiTiet.Solo,
            //        HanSuDung = item.nhapKhoVatTuChiTiet.HanSuDung,
            //        DonGiaNhap = item.nhapKhoVatTuChiTiet.DonGiaNhap,
            //        KhoXuatId = item.nhapKhoVatTuChiTiet.NhapKhoVatTu.KhoId,
            //        NgayNhap = item.nhapKhoVatTuChiTiet.NgayNhap,
            //        VAT = item.nhapKhoVatTuChiTiet.VAT,
            //        TiLeThapGia = item.nhapKhoVatTuChiTiet.TiLeTheoThapGia
            //    };
            //    yeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTiet);
            //}

            //var result = yeuCauTraVatTuChiTiets.Select(o =>
            //{
            //    o.SoLuongTon = nhapKhoVatTuChiTiets.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
            //    o.SoLuongTra = nhapKhoVatTuChiTiets.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
            //    return o;
            //}).ToList();
            var dataReturn = new List<YeuCauTraKSNKGridVo>();
            var yeuCauTraLuuTamGridVos = new List<YeuCauTraKSNKGridVo>();
            if (!info.IsCreate)
            {
                if (info.LoaiDuocPhamVatTu != null && info.LoaiDuocPhamVatTu.Value == LoaiDuocPhamVatTu.LoaiVatTu)
                {
                    yeuCauTraLuuTamGridVos = _ycTraVtChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraVatTuId == info.YeuCauTraVatTuId)
                                            .Select(s => new YeuCauTraKSNKGridVo
                                            {
                                                Id = s.Id,
                                                VatTuBenhVienId = s.VatTuBenhVienId,
                                                Ten = s.VatTuBenhVien.VatTus.Ten,
                                                DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                LaVatTuBHYT = s.LaVatTuBHYT,
                                                TenNhom = s.VatTuBenhVien.VatTus.NhomVatTu.Ten,
                                                //LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                                                Ma = s.VatTuBenhVien.VatTus.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraVatTu.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra,
                                                NgayNhap = s.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhap,
                                                VAT = s.VAT,
                                                TiLeThapGia = s.TiLeTheoThapGia
                                            })
                                            .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.SoLo, x.HanSuDung })
                                          .Select(g => new YeuCauTraKSNKGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              TenNhom = g.First().TenNhom,
                                              //LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.Sum(z => z.SoLuongTra),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
                }

                if (info.LoaiDuocPhamVatTu != null && info.LoaiDuocPhamVatTu.Value == LoaiDuocPhamVatTu.LoaiDuocPham)
                {
                    yeuCauTraLuuTamGridVos = _yeuCauTraDuocPhamChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraDuocPhamId == info.YeuCauTraVatTuId)
                                            .Select(s => new YeuCauTraKSNKGridVo
                                            {
                                                Id = s.Id,
                                                VatTuBenhVienId = s.DuocPhamBenhVienId,
                                                Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                                DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                LaVatTuBHYT = s.LaDuocPhamBHYT,
                                                TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM",
                                                //LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                                                Ma = s.DuocPhamBenhVien.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraDuocPham.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra,
                                                NgayNhap = s.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NgayNhap,
                                                VAT = s.VAT,
                                                TiLeThapGia = s.TiLeTheoThapGia
                                            })
                                            .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.SoLo, x.HanSuDung })
                                          .Select(g => new YeuCauTraKSNKGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              TenNhom = g.First().TenNhom,
                                              //LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.Sum(z => z.SoLuongTra),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
                }

                //result.AddRange(yeuCauTraVatTuGridVos);
                //result = result.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                //                          .Select(g => new YeuCauTraKSNKGridVo
                //                          {
                //                              Id = g.First().Id,
                //                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                //                              Ten = g.First().Ten,
                //                              DVT = g.First().DVT,
                //                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                //                              LoaiSuDung = g.First().LoaiSuDung,
                //                              Ma = g.First().Ma,
                //                              SoLo = g.First().SoLo,
                //                              HanSuDung = g.First().HanSuDung,
                //                              DonGiaNhap = g.First().DonGiaNhap,
                //                              KhoXuatId = g.First().KhoXuatId,
                //                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                //                              SoLuongTon = g.Sum(z => z.SoLuongTon),
                //                              NgayNhap = g.First().NgayNhap,
                //                              VAT = g.First().VAT,
                //                              TiLeThapGia = g.First().TiLeThapGia,
                //                          }).ToList();

                foreach (var yeuCauTraLuuTamGridVo in yeuCauTraLuuTamGridVos)
                {
                    var yeuCauTraChiTiet = yeuCauTraChiTiets.FirstOrDefault(o =>
                        o.LoaiDuocPhamVatTu == yeuCauTraLuuTamGridVo.LoaiDuocPhamVatTu
                        && o.VatTuBenhVienId == yeuCauTraLuuTamGridVo.VatTuBenhVienId
                        && o.LaVatTuBHYT == yeuCauTraLuuTamGridVo.LaVatTuBHYT
                        && o.SoLo == yeuCauTraLuuTamGridVo.SoLo
                        && o.HanSuDung == yeuCauTraLuuTamGridVo.HanSuDung);
                    if (yeuCauTraChiTiet != null)
                    {
                        yeuCauTraChiTiet.SoLuongTon += yeuCauTraLuuTamGridVo.SoLuongTra;
                        yeuCauTraChiTiet.SoLuongTra += yeuCauTraLuuTamGridVo.SoLuongTra;
                    }
                    else
                    {
                        dataReturn.Add(yeuCauTraLuuTamGridVo);
                    }
                }
            }

            dataReturn.AddRange(yeuCauTraChiTiets);

            if (info.VatTuBenhVienVos.Any())
            {
                dataReturn = dataReturn
                    .Where(x => !info.VatTuBenhVienVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung))
                    .ToList();
            }
            return new GridDataSource { Data = dataReturn.OrderBy(o => o.Ma).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count };


            //if (info.VatTuBenhVienVos.Any())
            //{
            //    result = result.Where(x => !info.VatTuBenhVienVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT
            //                                                            && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim()
            //                                                            && z.HanSuDung == x.HanSuDung)).ToList();
            //}
            //var query = result.AsQueryable();
            //var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArray();
            //return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDataForGridAsyncKSNKTuTrucDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauTraKSNKTuTrucVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(nkct => nkct.VatTus)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);
            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauTraVatTuChiTiets = new List<YeuCauTraKSNKGridVo>();
            var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh, x.DonGiaNhap }).Select(g => new { nhapKhoVatTuChiTiet = g.FirstOrDefault() });

            foreach (var item in nhapKhoVatTuChiTietGroup)
            {
                var yeuCauTraVatTuChiTiet = new YeuCauTraKSNKGridVo
                {
                    Id = item.nhapKhoVatTuChiTiet.Id,
                    VatTuBenhVienId = item.nhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ten = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    Ma = item.nhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    DVT = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = item.nhapKhoVatTuChiTiet.LaVatTuBHYT,
                    //LoaiSuDung = item.nhapKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung,
                    SoLo = item.nhapKhoVatTuChiTiet.Solo,
                    HanSuDung = item.nhapKhoVatTuChiTiet.HanSuDung,
                    DonGiaNhap = item.nhapKhoVatTuChiTiet.DonGiaNhap,
                    KhoXuatId = item.nhapKhoVatTuChiTiet.NhapKhoVatTu.KhoId,
                    NgayNhap = item.nhapKhoVatTuChiTiet.NgayNhap,
                    VAT = item.nhapKhoVatTuChiTiet.VAT,
                    TiLeThapGia = item.nhapKhoVatTuChiTiet.TiLeTheoThapGia
                };
                yeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTiet);
            }

            var result = yeuCauTraVatTuChiTiets.Select(o =>
            {
                o.SoLuongTon = nhapKhoVatTuChiTiets.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                o.SoLuongTra = nhapKhoVatTuChiTiets.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            }).ToList();
            var yeuCauTraVatTuGridVos = new List<YeuCauTraKSNKGridVo>();
            if (!info.IsCreate)
            {
                yeuCauTraVatTuGridVos = _ycTraVtChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraVatTuId == info.YeuCauTraVatTuId && ct.YeuCauTraVatTu.KhoXuatId == info.KhoXuatId)
                                            .Select(s => new YeuCauTraKSNKGridVo
                                            {
                                                Id = s.Id,
                                                VatTuBenhVienId = s.VatTuBenhVienId,
                                                Ten = s.VatTuBenhVien.VatTus.Ten,
                                                DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                LaVatTuBHYT = s.LaVatTuBHYT,
                                                //LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                                                Ma = s.VatTuBenhVien.VatTus.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraVatTu.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra,
                                                NgayNhap = s.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhap,
                                                VAT = s.VAT,
                                                TiLeThapGia = s.TiLeTheoThapGia
                                            })
                                            .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraKSNKGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              //LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
                result.AddRange(yeuCauTraVatTuGridVos);
                result = result.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraKSNKGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              //LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.Sum(z => z.SoLuongTon),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
            }

            if (info.VatTuBenhVienVos.Any())
            {
                result = result.Where(x => !info.VatTuBenhVienVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT
                                                                        && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim()
                                                                        && z.HanSuDung == x.HanSuDung)).ToList();
            }
            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncKSNKTuTrucDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauTraKSNKTuTrucVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(nkct => nkct.VatTus)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);
            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauTraVatTuChiTiets = new List<YeuCauTraKSNKGridVo>();
            var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh, x.DonGiaNhap }).Select(g => new { nhapKhoVatTuChiTiet = g.FirstOrDefault() });

            foreach (var item in nhapKhoVatTuChiTietGroup)
            {
                var yeuCauTraVatTuChiTiet = new YeuCauTraKSNKGridVo
                {
                    Id = item.nhapKhoVatTuChiTiet.Id,
                    VatTuBenhVienId = item.nhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ten = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    Ma = item.nhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    DVT = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = item.nhapKhoVatTuChiTiet.LaVatTuBHYT,
                    //LoaiSuDung = item.nhapKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung,
                    SoLo = item.nhapKhoVatTuChiTiet.Solo,
                    HanSuDung = item.nhapKhoVatTuChiTiet.HanSuDung,
                    DonGiaNhap = item.nhapKhoVatTuChiTiet.DonGiaNhap,
                    KhoXuatId = item.nhapKhoVatTuChiTiet.NhapKhoVatTu.KhoId,
                    NgayNhap = item.nhapKhoVatTuChiTiet.NgayNhap,
                    VAT = item.nhapKhoVatTuChiTiet.VAT,
                    TiLeThapGia = item.nhapKhoVatTuChiTiet.TiLeTheoThapGia
                };
                yeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTiet);
            }

            var yeuCauTraVatTuGridVos = new List<YeuCauTraKSNKGridVo>();
            if (!info.IsCreate)
            {
                yeuCauTraVatTuGridVos = _ycTraVtChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraVatTuId == info.YeuCauTraVatTuId && ct.YeuCauTraVatTu.KhoXuatId == info.KhoXuatId)
                                            .Select(s => new YeuCauTraKSNKGridVo
                                            {
                                                Id = s.Id,
                                                VatTuBenhVienId = s.VatTuBenhVienId,
                                                Ten = s.VatTuBenhVien.VatTus.Ten,
                                                DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                LaVatTuBHYT = s.LaVatTuBHYT,
                                                //LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                                                Ma = s.VatTuBenhVien.VatTus.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraVatTu.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra,
                                                NgayNhap = s.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhap,
                                                VAT = s.VAT,
                                                TiLeThapGia = s.TiLeTheoThapGia
                                            })
                                            .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraKSNKGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              //LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
                yeuCauTraVatTuChiTiets.AddRange(yeuCauTraVatTuGridVos);
                yeuCauTraVatTuChiTiets = yeuCauTraVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                         .Select(g => new YeuCauTraKSNKGridVo
                                         {
                                             Id = g.First().Id,
                                             VatTuBenhVienId = g.First().VatTuBenhVienId,
                                             Ten = g.First().Ten,
                                             DVT = g.First().DVT,
                                             LaVatTuBHYT = g.First().LaVatTuBHYT,
                                             //LoaiSuDung = g.First().LoaiSuDung,
                                             Ma = g.First().Ma,
                                             SoLo = g.First().SoLo,
                                             HanSuDung = g.First().HanSuDung,
                                             DonGiaNhap = g.First().DonGiaNhap,
                                             KhoXuatId = g.First().KhoXuatId,
                                             SoLuongTra = g.First().SoLuongTra,
                                             SoLuongTon = g.First().SoLuongTon,
                                             NgayNhap = g.First().NgayNhap,
                                             VAT = g.First().VAT,
                                             TiLeThapGia = g.First().TiLeThapGia,
                                         }).ToList();
            }

            if (info.VatTuBenhVienVos.Any())
            {
                yeuCauTraVatTuChiTiets = yeuCauTraVatTuChiTiets.Where(x => !info.VatTuBenhVienVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT
                                                                       && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim()
                                                                       && z.HanSuDung == x.HanSuDung)).ToList();
            }
            var countTask = yeuCauTraVatTuChiTiets.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDataForGridChildAsyncDaDuyetKSNK(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            var yeuCauTraVTVo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietKSNKVo>(queryInfo.AdditionalSearchString);
            var query = _ycTraVtChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraVatTuId == yeuCauTraVTVo.YeuCauTraVatTuId)
                .Select(s => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    Ma = s.VatTuBenhVien.Ma,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                    HopDong = s.HopDongThauVatTu.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                }).ApplyLike(yeuCauTraVTVo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.Ten,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    LoaiSuDung = g.First().LoaiSuDung,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var result = await groupQuery.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            return new GridDataSource { Data = result, TotalRowCount = result.Count() };
        }

        public async Task<GridDataSource> GetDataForGridChildDuocPhamAsyncDaDuyet(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }

            var yeucauInfo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietKSNKVo>(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraDuocPhamChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamId == yeucauInfo.YeuCauTraVatTuId)
                .Select(s => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    Id = s.Id,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    Ma = s.DuocPhamBenhVien.Ma,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    VatTuBenhVienId = s.DuocPhamBenhVienId,
                    HopDong = s.HopDongThauDuocPham.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    LaVatTuBHYT = s.LaDuocPhamBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                }).ApplyLike(yeucauInfo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.Ten,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    Nhom = g.First().Nhom,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var result = await groupQuery.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            return new GridDataSource { Data = result, TotalRowCount = result.Count() };
        }
        public async Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }

            var yeucauInfo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietKSNKVo>(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraDuocPhamChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamId == yeucauInfo.YeuCauTraVatTuId)
                .Select(s => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    Id = s.Id,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    Ma = s.DuocPhamBenhVien.Ma,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    VatTuBenhVienId = s.DuocPhamBenhVienId,
                    HopDong = s.HopDongThauDuocPham.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    LaVatTuBHYT = s.LaDuocPhamBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                }).ApplyLike(yeucauInfo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.Ten,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    Nhom = g.First().Nhom,
                    LoaiSuDung = g.First().LoaiSuDung,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });

            var countTask = groupQuery.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
        }


        public async Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyetKSNK(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { TotalRowCount = 0 };
            }
            var yeuCauTraVTVo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietKSNKVo>(queryInfo.AdditionalSearchString);
            var query = _ycTraVtChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraVatTuId == yeuCauTraVTVo.YeuCauTraVatTuId)
                .Select(s => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    Ma = s.VatTuBenhVien.Ma,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                    HopDong = s.HopDongThauVatTu.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                }).ApplyLike(yeuCauTraVTVo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.Ten,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraKSNKChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    LoaiSuDung = g.First().LoaiSuDung,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var countTask = groupQuery.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
        }
        public async Task<TrangThaiDuyetVo> GetTrangThaiYeuCauHoanTraKSNK(long phieuLinhId)
        {
            var yeuCauLinhDuocPham = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).FirstAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
            if (yeuCauLinhDuocPham.DuocDuyet == true)
            {
                trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                trangThaiVo.TrangThai = true;
                trangThaiVo.Ten = EnumTrangThaiPhieuLinh.DaDuyet.GetDescription();
                return trangThaiVo;
            }
            else if (yeuCauLinhDuocPham.DuocDuyet == false)
            {
                trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                trangThaiVo.TrangThai = false;
                trangThaiVo.Ten = EnumTrangThaiPhieuLinh.TuChoiDuyet.GetDescription();
                return trangThaiVo;
            }
            else
            {
                trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                trangThaiVo.TrangThai = null;
                trangThaiVo.Ten = EnumTrangThaiPhieuLinh.DangChoDuyet.GetDescription();
                return trangThaiVo;
            }
        }

        public async Task XoaYeuCauHoanTraDuocPhamAsync(long yeuCauHoanTraDuocPhamId)
        {
            var ycHoanTraDuocPham = _yeuCauTraDuocPhamRepository.GetById(yeuCauHoanTraDuocPhamId, s => s
                .Include(w => w.YeuCauTraDuocPhamChiTiets).ThenInclude(w => w.XuatKhoDuocPhamChiTietViTri).ThenInclude(w => w.NhapKhoDuocPhamChiTiet)
                .Include(w => w.YeuCauTraDuocPhamChiTiets).ThenInclude(w => w.XuatKhoDuocPhamChiTietViTri).ThenInclude(w => w.XuatKhoDuocPhamChiTiet));
            if (ycHoanTraDuocPham.DuocDuyet != null)
            {
                throw new Exception("Yêu cầu hoàn trả này đã được duyệt.");
            }
            foreach (var yeuCauTraDuocPhamChiTiets in ycHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
            {
                yeuCauTraDuocPhamChiTiets.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = (yeuCauTraDuocPhamChiTiets.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - yeuCauTraDuocPhamChiTiets.XuatKhoDuocPhamChiTietViTri.SoLuongXuat).MathRoundNumber(2);
                yeuCauTraDuocPhamChiTiets.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                yeuCauTraDuocPhamChiTiets.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                yeuCauTraDuocPhamChiTiets.WillDelete = true;
            }
            ycHoanTraDuocPham.WillDelete = true;
            BaseRepository.Context.SaveChanges();
        }

        public async Task XoaYeuCauHoanTraVatTuAsync(long yeuCauHoanTraVatTuId)
        {
            var ycHoanTraVatTu = BaseRepository.GetById(yeuCauHoanTraVatTuId, s => s
                .Include(w => w.YeuCauTraVatTuChiTiets).ThenInclude(w => w.XuatKhoVatTuChiTietViTri).ThenInclude(w => w.NhapKhoVatTuChiTiet)
                .Include(w => w.YeuCauTraVatTuChiTiets).ThenInclude(w => w.XuatKhoVatTuChiTietViTri).ThenInclude(w => w.XuatKhoVatTuChiTiet));
            if (ycHoanTraVatTu.DuocDuyet != null)
            {
                throw new Exception("Yêu cầu hoàn trả này đã được duyệt.");
            }
            foreach (var entityYeuCauTraVatTuChiTiet in ycHoanTraVatTu.YeuCauTraVatTuChiTiets)
            {
                entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat = (entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat).MathRoundNumber(2);
                entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                entityYeuCauTraVatTuChiTiet.WillDelete = true;
            }
            ycHoanTraVatTu.WillDelete = true;
            BaseRepository.Context.SaveChanges();
        }

        public async Task<ThemHoanTraKSNKResultVo> XuLyThemHoanTraKSNKAsync(YeuCauTraVatTu yeuCauTraVatTu, YeuCauTraDuocPham yeuCauTraDuocPham, List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVTTuTrucChiTiets)
        {
            var yeuCauTraDuocPhamChiTietVos = yeuCauTraVTTuTrucChiTiets.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham).ToList();
            var yeuCauTraVatTuChiTietVos = yeuCauTraVTTuTrucChiTiets.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu).ToList();

            var khoNhap = _khoRepository.TableNoTracking.First(p => p.Id == yeuCauTraVatTu.KhoNhapId);

            if (khoNhap.LoaiDuocPham != true && yeuCauTraDuocPhamChiTietVos.Any())
            {
                throw new Exception($"Không thể hoàn trả dược phẩm về {khoNhap.Ten}");
            }
            if (khoNhap.LoaiVatTu != true && yeuCauTraVatTuChiTietVos.Any())
            {
                throw new Exception($"Không thể hoàn trả vật tư về {khoNhap.Ten}");
            }

            if (yeuCauTraDuocPhamChiTietVos.Any())
            {
                var duocPhamBenhVienIds = yeuCauTraDuocPhamChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                var soLos = yeuCauTraDuocPhamChiTietVos.Select(o => o.SoLo).ToList();

                var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                    .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauTraDuocPham.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                    .ToList();

                foreach (var chiTietVo in yeuCauTraDuocPhamChiTietVos)
                {
                    var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                    .Where(o => o.DuocPhamBenhVienId == chiTietVo.VatTuBenhVienId && o.LaDuocPhamBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date)
                    .ToList();
                    var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (nhapKhoDuocPhamChiTietXuats.Count == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongTra.Value) && slTon < chiTietVo.SoLuongTra))
                    {
                        throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                    }
                    double soLuongCanXuat = chiTietVo.SoLuongTra.Value;
                    while (!soLuongCanXuat.AlmostEqual(0))
                    {
                        // tinh so luong xuat
                        var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats.Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                        var soLuongTon = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                        var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                        var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId
                        };
                        var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                        {
                            SoLuongXuat = soLuongXuat,
                            NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                            XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet
                        };

                        var yeuCauTraDuocPhamChiTietNew = new YeuCauTraDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                            HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                            LaDuocPhamBHYT = nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                            Solo = nhapKhoDuocPhamChiTiet.Solo,
                            HanSuDung = nhapKhoDuocPhamChiTiet.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoDuocPhamChiTiet.VAT,
                            MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                            MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                            KhoViTriId = nhapKhoDuocPhamChiTiet.KhoViTriId,
                            SoLuongTra = soLuongXuat,
                            XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri
                        };
                        yeuCauTraDuocPham.YeuCauTraDuocPhamChiTiets.Add(yeuCauTraDuocPhamChiTietNew);

                        soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                    }
                }
                _yeuCauTraDuocPhamRepository.AutoCommitEnabled = false;
                _yeuCauTraDuocPhamRepository.Add(yeuCauTraDuocPham);
            }
            if (yeuCauTraVatTuChiTietVos.Any())
            {
                var vatTuBenhVienIds = yeuCauTraVatTuChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                var soLos = yeuCauTraVatTuChiTietVos.Select(o => o.SoLo).ToList();

                var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                    .Where(o => o.NhapKhoVatTu.KhoId == yeuCauTraVatTu.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                    .ToList();

                foreach (var chiTietVo in yeuCauTraVatTuChiTietVos)
                {
                    var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                    .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date)
                    .ToList();
                    var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (nhapKhoVatTuChiTietXuats.Count == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongTra.Value) && slTon < chiTietVo.SoLuongTra))
                    {
                        throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                    }
                    double soLuongCanXuat = chiTietVo.SoLuongTra.Value;
                    while (!soLuongCanXuat.AlmostEqual(0))
                    {
                        // tinh so luong xuat
                        var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats.Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                        var soLuongTon = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                        var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                        nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                        var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId
                        };
                        var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                        {
                            SoLuongXuat = soLuongXuat,
                            NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                            XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet
                        };

                        var yeuCauTraVatTuChiTietNew = new YeuCauTraVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTiet.Solo,
                            HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTiet.VAT,
                            MaVach = nhapKhoVatTuChiTiet.MaVach,
                            MaRef = nhapKhoVatTuChiTiet.MaRef,
                            KhoViTriId = nhapKhoVatTuChiTiet.KhoViTriId,
                            SoLuongTra = soLuongXuat,
                            XuatKhoVatTuChiTietViTri = xuatKhoVatTuChiTietViTri
                        };
                        yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTietNew);

                        soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                    }
                }
                _yeuCauTraVatTuRepository.AutoCommitEnabled = false;
                _yeuCauTraVatTuRepository.Add(yeuCauTraVatTu);
            }
            BaseRepository.Context.SaveChanges();
            return new ThemHoanTraKSNKResultVo() { HoanTraDuocPhamId = (yeuCauTraDuocPham.Id != 0 ? yeuCauTraDuocPham.Id : (long?)null), HoanTraVatTuId = (yeuCauTraVatTu.Id != 0 ? yeuCauTraVatTu.Id : (long?)null) };
        }

        public async Task XuLyCapNhatHoanTraDuocPhamKSNKAsync(YeuCauTraDuocPham yeuCauTraDuocPham, List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVTTuTrucChiTiets)
        {
            var yeuCauTraDuocPhamChiTietVos = yeuCauTraVTTuTrucChiTiets.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham).ToList();

            var khoNhap = _khoRepository.TableNoTracking.First(p => p.Id == yeuCauTraDuocPham.KhoNhapId);
            if (khoNhap.LoaiDuocPham != true && yeuCauTraDuocPhamChiTietVos.Any())
            {
                throw new Exception($"Không thể hoàn trả dược phẩm về {khoNhap.Ten}");
            }

            var duocPhamBenhVienIds = yeuCauTraDuocPhamChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
            var soLos = yeuCauTraDuocPhamChiTietVos.Select(o => o.SoLo).ToList();

            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauTraDuocPham.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo))
                .ToList();


            foreach (var chiTiet in yeuCauTraDuocPham.YeuCauTraDuocPhamChiTiets)
            {
                chiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = (chiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - chiTiet.SoLuongTra).MathRoundNumber(2);
                chiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                chiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                chiTiet.WillDelete = true;
            }

            foreach (var chiTietVo in yeuCauTraDuocPhamChiTietVos)
            {
                var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                .Where(o => o.DuocPhamBenhVienId == chiTietVo.VatTuBenhVienId && o.LaDuocPhamBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date)
                .ToList();
                var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                if (nhapKhoDuocPhamChiTietXuats.Count == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongTra.Value) && slTon < chiTietVo.SoLuongTra))
                {
                    throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                }
                double soLuongCanXuat = chiTietVo.SoLuongTra.Value;
                while (!soLuongCanXuat.AlmostEqual(0))
                {
                    // tinh so luong xuat
                    var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats.Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                    var soLuongTon = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                    var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                    nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                    var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                    {
                        DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId
                    };
                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat,
                        NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                        XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet
                    };

                    var yeuCauTraDuocPhamChiTietNew = new YeuCauTraDuocPhamChiTiet
                    {
                        DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                        LaDuocPhamBHYT = nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                        Solo = nhapKhoDuocPhamChiTiet.Solo,
                        HanSuDung = nhapKhoDuocPhamChiTiet.HanSuDung,
                        NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien,
                        DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                        TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                        VAT = nhapKhoDuocPhamChiTiet.VAT,
                        MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                        MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                        KhoViTriId = nhapKhoDuocPhamChiTiet.KhoViTriId,
                        SoLuongTra = soLuongXuat,
                        XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri
                    };
                    yeuCauTraDuocPham.YeuCauTraDuocPhamChiTiets.Add(yeuCauTraDuocPhamChiTietNew);

                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                }
            }
            _yeuCauTraDuocPhamRepository.Update(yeuCauTraDuocPham);
        }

        public async Task XuLyCapNhatHoanTraVatTuKSNKAsync(YeuCauTraVatTu yeuCauTraVatTu, List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVTTuTrucChiTiets)
        {
            var yeuCauTraVatTuChiTietVos = yeuCauTraVTTuTrucChiTiets.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu).ToList();
            var khoNhap = _khoRepository.TableNoTracking.First(p => p.Id == yeuCauTraVatTu.KhoNhapId);
            if (khoNhap.LoaiVatTu != true && yeuCauTraVatTuChiTietVos.Any())
            {
                throw new Exception($"Không thể hoàn trả vật tư về {khoNhap.Ten}");
            }

            var vatTuBenhVienIds = yeuCauTraVatTuChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
            var soLos = yeuCauTraVatTuChiTietVos.Select(o => o.SoLo).ToList();

            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                .Where(o => o.NhapKhoVatTu.KhoId == yeuCauTraVatTu.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo))
                .ToList();


            foreach (var chiTiet in yeuCauTraVatTu.YeuCauTraVatTuChiTiets)
            {
                chiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat = (chiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - chiTiet.SoLuongTra).MathRoundNumber(2);
                chiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                chiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                chiTiet.WillDelete = true;
            }

            foreach (var chiTietVo in yeuCauTraVatTuChiTietVos)
            {
                var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date)
                .ToList();
                var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                if (nhapKhoVatTuChiTietXuats.Count == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongTra.Value) && slTon < chiTietVo.SoLuongTra))
                {
                    throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                }
                double soLuongCanXuat = chiTietVo.SoLuongTra.Value;
                while (!soLuongCanXuat.AlmostEqual(0))
                {
                    // tinh so luong xuat
                    var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats.Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                    var soLuongTon = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                    var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                    nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                    var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                    {
                        VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId
                    };
                    var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat,
                        NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                        XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet
                    };

                    var yeuCauTraVatTuChiTietNew = new YeuCauTraVatTuChiTiet
                    {
                        VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                        HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                        LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                        Solo = nhapKhoVatTuChiTiet.Solo,
                        HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                        NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                        DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                        TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                        VAT = nhapKhoVatTuChiTiet.VAT,
                        MaVach = nhapKhoVatTuChiTiet.MaVach,
                        MaRef = nhapKhoVatTuChiTiet.MaRef,
                        KhoViTriId = nhapKhoVatTuChiTiet.KhoViTriId,
                        SoLuongTra = soLuongXuat,
                        XuatKhoVatTuChiTietViTri = xuatKhoVatTuChiTietViTri
                    };
                    yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTietNew);

                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                }
            }

            BaseRepository.Update(yeuCauTraVatTu);
        }

        public async Task XuLyThemHoacCapNhatHoanTraKSNKAsync(YeuCauTraVatTu yeuCauTraVatTu, List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVTTuTrucChiTiets, bool isCreate)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (!isCreate)
            {
                await XoaChiTietHoanTra(yeuCauTraVatTu);
            }
            var khoXuatIds = yeuCauTraVTTuTrucChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
            var nhapKhoVatTuChiTietAlls = _nhapKhoVatTuChiTietRepository.Table
                                         .Include(nk => nk.NhapKhoVatTu)
                                         .Where(o => khoXuatIds.Contains(o.NhapKhoVatTu.KhoId) && o.SoLuongNhap > o.SoLuongDaXuat)
                                         .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                          .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
            foreach (var chiTiet in yeuCauTraVTTuTrucChiTiets)
            {
                var nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietAlls.Where(o =>
                                      o.NhapKhoVatTu.KhoId == chiTiet.KhoXuatId
                                      && o.LaVatTuBHYT == chiTiet.LaVatTuBHYT
                                      && o.VatTuBenhVienId == chiTiet.VatTuBenhVienId
                                      && o.Solo == chiTiet.SoLo
                                      && o.HanSuDung == chiTiet.HanSuDung
                                      && o.DonGiaNhap == chiTiet.DonGiaNhap)
                                      .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

                var SLTon = nhapKhoVatTuChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat);

                if (SLTon < chiTiet.SoLuongTra)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }
                var soLuongTra = chiTiet.SoLuongTra;// số lượng trả

                var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                {
                    VatTuBenhVienId = chiTiet.VatTuBenhVienId,
                };
                var yeuCauTraVatTuChiTietNews = new List<YeuCauTraVatTuChiTiet>();
                foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                {
                    if (soLuongTra == 0)
                    {
                        break;
                    }
                    var yeuCauTraVatTuChiTietNew = new YeuCauTraVatTuChiTiet
                    {
                        VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                        HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                        LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                        Solo = nhapKhoVatTuChiTiet.Solo,
                        HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                        NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                        DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                        TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                        VAT = nhapKhoVatTuChiTiet.VAT,
                        MaVach = nhapKhoVatTuChiTiet.MaVach,
                        MaRef = nhapKhoVatTuChiTiet.MaRef,
                        KhoViTriId = nhapKhoVatTuChiTiet.KhoViTriId
                    };
                    var SLTonHienTai = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                    if (SLTonHienTai > soLuongTra || SLTonHienTai.AlmostEqual(soLuongTra.Value))
                    {
                        nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongTra.Value).MathRoundNumber(2);
                        yeuCauTraVatTuChiTietNew.SoLuongTra = soLuongTra.Value;
                        soLuongTra = 0;
                    }
                    else
                    {
                        nhapKhoVatTuChiTiet.SoLuongDaXuat = nhapKhoVatTuChiTiet.SoLuongNhap;
                        yeuCauTraVatTuChiTietNew.SoLuongTra = SLTonHienTai;
                        soLuongTra -= SLTonHienTai;
                    }
                    var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = yeuCauTraVatTuChiTietNew.SoLuongTra,
                        GhiChu = "Hoàn trả vật tư tủ trực.",
                        XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet,
                    };
                    yeuCauTraVatTuChiTietNew.XuatKhoVatTuChiTietViTri = xuatKhoVatTuChiTietViTri;
                    yeuCauTraVatTuChiTietNew.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet;
                    yeuCauTraVatTuChiTietNew.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet;
                    yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTietNew);
                }
            }
            await BaseRepository.UpdateAsync(yeuCauTraVatTu);
        }

        public async Task<List<YeuCauTraKSNKGridVo>> YeuCauHoanTraKSNKChiTiets(long yeuCauTraVatTuId)
        {
            var query = _ycTraVtChiTiet.TableNoTracking.Where(z => z.YeuCauTraVatTuId == yeuCauTraVatTuId)
                .Select(s => new YeuCauTraKSNKGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    //LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                    Ma = s.VatTuBenhVien.Ma,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauTraVatTu.KhoXuatId,
                    SoLuongTra = s.SoLuongTra,
                    SoLuongTon = s.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(z => z.VatTuBenhVienId == s.VatTuBenhVienId && z.LaVatTuBHYT == s.LaVatTuBHYT && z.NhapKhoVatTu.KhoId == s.YeuCauTraVatTu.KhoXuatId && z.HanSuDung == s.HanSuDung && z.Solo == s.Solo && z.DonGiaNhap == s.DonGiaNhap).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat),
                    NgayNhap = s.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhap,
                    VAT = s.VAT,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiVatTu
                })
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new YeuCauTraKSNKGridVo
                {
                    Id = g.First().Id,
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    //LoaiSuDung = g.First().LoaiSuDung,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                    NgayNhap = g.First().NgayNhap,
                    TiLeThapGia = g.First().TiLeThapGia,
                    VAT = g.First().VAT,
                    LoaiDuocPhamVatTu = g.First().LoaiDuocPhamVatTu,
                }).ToList();
            return query;
        }

        private async Task XoaChiTietHoanTra(YeuCauTraVatTu yeuCauTraVatTu)
        {
            foreach (var chiTiet in yeuCauTraVatTu.YeuCauTraVatTuChiTiets)
            {
                chiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                                                                    (chiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - chiTiet.SoLuongTra).MathRoundNumber(2);
                chiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                chiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                chiTiet.WillDelete = true;
            }
            await BaseRepository.UpdateAsync(yeuCauTraVatTu);
        }

        public async Task<List<YeuCauTraKSNKGridVo>> YeuCauTraDuocPhamTuTrucChiTiets(long yeuCauTraDuocPhamId)
        {
            var query = _yeuCauTraDuocPhamChiTiet.TableNoTracking.Where(z => z.YeuCauTraDuocPhamId == yeuCauTraDuocPhamId)
                .Select(s => new YeuCauTraKSNKGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.DuocPhamBenhVienId,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    LaVatTuBHYT = s.LaDuocPhamBHYT,
                    //DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                    TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = s.DuocPhamBenhVien.Ma,
                    //SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauTraDuocPham.KhoXuatId,
                    SoLuongTra = s.SoLuongTra,
                    SoLuongTon = s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(z => z.DuocPhamBenhVienId == s.DuocPhamBenhVienId && z.LaDuocPhamBHYT == s.LaDuocPhamBHYT && z.NhapKhoDuocPhams.KhoId == s.YeuCauTraDuocPham.KhoXuatId && z.HanSuDung == s.HanSuDung && z.Solo == s.Solo && z.DonGiaNhap == s.DonGiaNhap).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat),
                    NgayNhap = s.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NgayNhap,
                    LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham
                })
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new YeuCauTraKSNKGridVo
                {
                    Id = g.First().Id,
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    // DuocPhamBenhVienPhanNhomId = g.First().DuocPhamBenhVienPhanNhomId,
                    TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                    Ma = g.First().Ma,
                    // SoDangKy = g.First().SoDangKy,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                    NgayNhap = g.First().NgayNhap,
                    LoaiDuocPhamVatTu = g.First().LoaiDuocPhamVatTu,
                }).ToList();
            return query;
        }
    }
}
